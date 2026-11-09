"""Bake linear Sprite secondary textures from the current pixel art.
Requires Pillow and NumPy. Run from any directory with the scene dependency list
exported by Unity's AssetDatabase.GetDependencies for the enabled gameplay scenes.
"""
import argparse
from collections import Counter
from hashlib import sha256
from io import BytesIO
from pathlib import Path
import re

import numpy as np
from PIL import Image

PROJECT = Path(__file__).resolve().parents[1] / "Projeto Jogo 2D"
ASSETS = PROJECT / "Assets"
DEST = ASSETS / "Art" / "SuperficiesSprites"
DYNAMIC = ("Resources/Art/", "Resources/Cap1/", "Resources/Cap2/", "Resources/Bocha/")
SKIP = re.compile(r"/(?:Relevos|SuperficiesSprites|UI|Tapes|Fonts|Baked)/|"
                  r"(?:Superficie|Relevo|SombraContato|Gradiente|Nevoeiro|LuzPintada)$", re.I)
PROFILES = (
    ("folhagem", r"arvore|folha|folhico|copa|moita|mata|capim|grama|veget|samambaia|araucaria|bracatinga|camboata|pinheiro|jardineira", .79, .0, .87),
    ("veiculo", r"carro|veiculo|caminh|onibus|fusca|chevette|variant|ford|fiat|pickup|parati|gol_|santana|saveiro|beetle|van_", .39, .31, .62),
    ("metal", r"poste|antena|bomba|tambor|extintor|medidor|telha.*zinco|portao|grade|corrimao|chapa|metal|lixeira|ferrament", .49, .36, .63),
    ("madeira", r"cadeira|banco|mesa|balcao|madeira|prateleira|armario|cerca|tronco|galho|caixote|estante", .68, .0, .68),
    ("alvenaria", r"casa|fachada|mureta|muro|parede|pedra|tijolo|concreto|calcada|meio.fio|degrau|canaleta|telhado|posto|igreja", .85, .0, .79),
    ("chao", r"chao|piso|terra|saibro|asfalto|acostamento|road|ground|tiles|terr[ea]no|trilha|areia", .84, .0, .52),
)


def profile(path, sprite_name=None):
    for kind, pattern, rough, metal, strength in PROFILES:
        if re.search(pattern, sprite_name or path.stem, re.I) or (kind == "veiculo" and "Veiculos" in path.parts):
            return kind, rough, metal, strength
    return "fosco", .74, .0, .52


def neighbor(image, axis, step):
    """Shift by one pixel without wrapping at the texture edge."""
    index = np.clip(np.arange(image.shape[axis]) + step, 0, image.shape[axis] - 1)
    return np.take(image, index, axis=axis)


def bake(source, material):
    kind, rough, metal, strength = material
    rgba = np.asarray(source.convert("RGBA"), dtype=np.float32) / 255
    rgb = rgba[:, :, :3]
    alpha = rgba[:, :, 3]
    lightness = rgb @ np.array([.299, .587, .114], dtype=np.float32)
    chroma = rgb.max(axis=2) - rgb.min(axis=2)
    height = lightness * .78 + chroma * .22

    def slope(axis):
        lo, hi = (neighbor(height, axis, step) for step in (-1, 1))
        a, b = (neighbor(alpha, axis, step) for step in (-1, 1))
        # Pixels outside the silhouette contribute a small bevel without importing
        # stray RGB from transparent pixels or the next cell on an animation sheet.
        return ((hi - height) * b + (height - lo) * a + (b - a) * .20) * strength

    normal_x = -slope(1)
    normal_y = slope(0)  # PNG rows increase down; Unity UV increases up.
    roughness = rough + (.48 - lightness) * .16 + chroma * .09
    metallic = np.full_like(lightness, metal)
    if kind == "veiculo":
        black = np.clip((.23 - lightness) * 5, 0, 1) * np.clip(1 - chroma * 7, 0, 1)
        glass = np.clip((rgb[:, :, 2] - rgb[:, :, 0]) * 4, 0, 1)
        roughness += .47 * black - .13 * glass
        metallic *= 1 - .92 * np.maximum(black, glass)
    elif kind == "metal":
        metallic *= .28 + .72 * np.clip((lightness - .13) * 2.5, 0, 1)
    normal_x = np.where(alpha > 0, normal_x, 0)
    normal_y = np.where(alpha > 0, normal_y, 0)
    channels = np.stack((.5 + normal_x * .5, .5 + normal_y * .5,
                         np.clip(roughness, .23, .95), metallic), axis=2)
    return Image.fromarray(np.rint(np.clip(channels, 0, 1) * 255).astype(np.uint8))


SPRITE_RECT = re.compile(
    r"^      name: (.+)\n      rect:\n        serializedVersion: \d+\n"
    r"        x: (\d+)\n        y: (\d+)\n        width: (\d+)\n        height: (\d+)$", re.M)


def bake_sheet(source, path, meta, material):
    rects = list(SPRITE_RECT.finditer(meta))
    if not rects:
        return bake(source, material)

    result = Image.new("RGBA", source.size, (128, 128, 204, 0))
    for rect in rects:
        x, y, width, height = map(int, rect.groups()[1:])
        top = source.height - y - height
        region = (x, top, x + width, top + height)
        result.paste(bake(source.crop(region), profile(path, rect[1])), region[:2])
    return result


def update(path, data):
    if not path.exists() or path.read_bytes() != data:
        path.write_bytes(data)


def link(source_meta, guid):
    data = source_meta.read_bytes()
    newline = b"\r\n" if b"\r\n" in data else b"\n"
    existing = rb"^( +)- texture: [^\r\n]+\r?\n +name: _SurfaceMap\r?\n"
    header = re.search(rb"^( +)secondaryTextures:(?: \[\])?\r?\n", data, re.M)
    if not header:
        raise ValueError(f"TextureImporter without secondary texture settings: {source_meta}")
    indent = header[1]
    entry = (indent + b"- texture: {fileID: 2800000, guid: " + guid.encode() +
             b", type: 3}" + newline + indent + b"  name: _SurfaceMap" + newline)
    if re.search(existing, data, re.M):
        data = re.sub(existing, lambda _: entry, data, flags=re.M)
    else:
        data = data[:header.start()] + indent + b"secondaryTextures:" + newline + entry + data[header.end():]
    update(source_meta, data)


def texture_meta(guid, maximum, compressed):
    # Keep small maps lossless. Use high-quality GPU compression for larger
    # secondary textures on the desktop, with lossless settings elsewhere.
    desktop = f"""  - serializedVersion: 4
    buildTarget: Standalone
    maxTextureSize: {maximum}
    textureFormat: 25
    textureCompression: 2
    compressionQuality: 100
    overridden: 1
""" if compressed else ""
    return f"""fileFormatVersion: 2
guid: {guid}
TextureImporter:
  serializedVersion: 13
  mipmaps:
    enableMipMap: 0
    sRGBTexture: 0
  isReadable: 0
  textureSettings:
    serializedVersion: 2
    filterMode: 0
    aniso: 1
    wrapU: 1
    wrapV: 1
    wrapW: 1
  nPOTScale: 0
  maxTextureSize: {maximum}
  alphaUsage: 1
  alphaIsTransparency: 0
  textureType: 0
  platformSettings:
  - serializedVersion: 4
    buildTarget: DefaultTexturePlatform
    maxTextureSize: {maximum}
    textureFormat: -1
    textureCompression: 0
    overridden: 0
{desktop}""".encode()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("dependencies", type=Path)
    parser.add_argument("--dry-run", action="store_true")
    args = parser.parse_args()
    dependencies = {PROJECT / line for line in args.dependencies.read_text(encoding="utf-8-sig").splitlines()}
    paths = dependencies | {p for folder in DYNAMIC for p in (ASSETS / folder).rglob("*.png")}
    counts = Counter()
    for path in sorted(paths):
        relative = path.relative_to(ASSETS).as_posix()
        if SKIP.search("/" + str(Path(relative).with_suffix("")).replace("\\", "/")):
            continue
        source_meta = Path(str(path) + ".meta")
        if path.suffix.lower() not in (".png", ".jpg", ".jpeg", ".tga", ".psd") or not source_meta.is_file():
            continue
        meta = source_meta.read_text(encoding="utf-8")
        if not re.search(r"^  textureType: 8$", meta, re.M):
            continue
        with Image.open(path) as source:
            # URP secondary textures use the same UVs and imported size as their source.
            maximum = int(re.search(r"^  maxTextureSize: (\d+)", meta, re.M)[1])
            if max(source.size) > maximum:
                counts["relevo no shader (fonte redimensionada pelo Unity)"] += 1
                continue
            material = profile(path)
            counts[material[0]] += 1
            if args.dry_run:
                continue
            key = re.search(r"^guid: ([a-f0-9]+)", meta, re.M)[1]
            guid = sha256(("SantaRita/SpriteSurface/" + key).encode()).hexdigest()[:32]
            DEST.mkdir(parents=True, exist_ok=True)
            dest = DEST / (key + ".png")
            buffer = BytesIO()
            bake_sheet(source, path, meta, material).save(buffer, format="PNG", compress_level=6)
            update(dest, buffer.getvalue())
            update(Path(str(dest) + ".meta"), texture_meta(guid, maximum, source.width * source.height >= 256 * 256))
            link(source_meta, guid)
    print(dict(counts))


if __name__ == "__main__":
    main()

