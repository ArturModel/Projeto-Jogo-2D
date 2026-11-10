"""Compõe as pedras da Matriz sobre os três atlas da Rua Norte (Pillow)."""

from pathlib import Path
import uuid

from PIL import Image, ImageStat


ROOT = Path(__file__).resolve().parents[1]
ART = ROOT / "Projeto Jogo 2D/Assets/Resources/Art"
SOURCE = ROOT / "Arte/Producao/Capitulo2"
MATRIZ = ART / "Cap2/Remodelagem/Matriz"
TILE = 64
MACRO = TILE * 2


def rua_norte_palette():
    with Image.open(ART / "RuaNorte/Terreno.png") as atlas:
        # Amostras inteiras das linhas de grama, asfalto e calçada do atlas de jogo.
        return [atlas.crop((TILE, row * TILE, TILE * 2, (row + 1) * TILE)).convert("RGB")
                for row in range(3)]


def material_samples(source_name, *, pavers):
    overlap_x = 6 if pavers else 12
    overlap_y = 0 if pavers else 12
    with Image.open(SOURCE / source_name) as source:
        texture = source.convert("RGB")
        if pavers:
            # Quatorze fiadas da imagem, com o recorte sobre as juntas
            # horizontais, para a paginação alternar também no macro seguinte.
            texture = texture.crop((0, 0, texture.width, round(texture.height * .936)))
        texture = texture.resize((MACRO + overlap_x, MACRO + overlap_y), Image.Resampling.LANCZOS)

    def taps(i, overlap):
        if i < MACRO - overlap:
            return ((i + overlap, 1.0),)
        low = i - (MACRO - overlap)
        weight = low / (overlap - 1)
        return ((i + overlap, 1 - weight), (low, weight))

    colors = []
    for y in range(MACRO):
        for x in range(MACRO):
            samples = [(texture.getpixel((sx, sy)), wx * wy)
                       for sx, wx in taps(x, overlap_x) for sy, wy in taps(y, overlap_y)]
            colors.append(tuple(sum(color[c] * weight for color, weight in samples) for c in range(3)))

    luminances = [.22 * r + .70 * g + .08 * b for r, g, b in colors]
    sorted_values = sorted(luminances)
    low, high = sorted_values[len(colors) // 12], sorted_values[len(colors) * 7 // 8]
    grass, asphalt, sidewalk = rua_norte_palette()
    grass_mean = ImageStat.Stat(grass).mean
    asphalt_mean = ImageStat.Stat(asphalt).mean
    sidewalk_mean = ImageStat.Stat(sidewalk).mean
    tone = sum(luminances) / len(luminances)
    values = []
    for i, value in enumerate(luminances):
        face = min(1, max(0, (value - low) / max(1, high - low)))
        # Mantém a altura do desenho original. A cor vem da pedra da calçada
        # com a escala de contraste e o grão dos próprios pisos da Rua Norte.
        height = .23 + face * (.23 if pavers else .19)
        x, y = i % MACRO, i // MACRO
        sample = min(x, MACRO - 1 - x), min(y, MACRO - 1 - y)
        if pavers:
            grain = sidewalk.getpixel(sample)
            color = tuple(base + 8 + (value - tone) * .46 + (pixel - base) * .65
                          for base, pixel in zip(sidewalk_mean, grain))
        else:
            grain = asphalt.getpixel(sample)
            moss = max(0, .28 - face)
            color = tuple(paving + 5 + (road - paving) * .13 + (value - tone) * .54
                          + (pixel - road) * .85 + moss * (leaf - grass_mean[0]) * 1.4
                          for paving, road, pixel, leaf in zip(sidewalk_mean, asphalt_mean, grain, grass_mean))
        values.append((color, height, face))
    return [values[y:y + MACRO] for y in range(0, len(values), MACRO)]


def byte(value):
    return min(255, max(0, round(value)))


def make_rows():
    images = [Image.new("RGBA", (TILE * 4, TILE * 2)) for _ in range(3)]
    materials = (
        material_samples("Tijolinho-Matriz-ImageGen.png", pavers=True),
        material_samples("Pedregulho-Matriz-ImageGen.png", pavers=False),
    )
    for row, values in enumerate(materials):
        sprite = Image.new("RGBA", (MACRO, MACRO))
        sprite.putdata([(*map(byte, color), 255) for line in values for color, _, _ in line])
        sprite.save(MATRIZ / ("LajeGranito.png" if row == 0 else "Pedregulho.png"))
        for variant in range(4):
            for y in range(TILE):
                for x in range(TILE):
                    mx = x + (variant % 2) * TILE
                    my = y + (variant // 2) * TILE
                    color, height, edge = values[my][mx]
                    dx = values[my][(mx - 1) % MACRO][1] - values[my][(mx + 1) % MACRO][1]
                    dy = values[(my + 1) % MACRO][mx][1] - values[(my - 1) % MACRO][mx][1]
                    position = (variant * TILE + x, row * TILE + y)
                    images[0].putpixel(position, (*map(byte, color), 255))
                    images[1].putpixel(position, (byte(128 + dx * 88), byte(128 + dy * 88),
                                                  byte(196 - edge * 52), byte(204 + edge * 51)))
                    images[2].putpixel(position, (byte(height * 255), byte(128 + dx * 61),
                                                  byte(128 + dy * 61), byte(204 + edge * 51)))
    return images


def save_atlas(source, rows):
    path = ART / "RuaNorte" / f"{source}.png"
    with Image.open(path) as base:
        if base.size != (256, 832):
            raise ValueError(f"Tamanho inesperado no atlas {source}: {base.size}")
        atlas = Image.new("RGBA", (base.width, base.height + rows.height))
        atlas.paste(rows, (0, 0))
        atlas.paste(base.convert("RGBA"), (0, rows.height))
    dest = MATRIZ / f"PisosCentro{source}.png"
    atlas.save(dest)
    meta = dest.with_suffix(".png.meta")
    template = path.with_suffix(".png.meta").read_text().splitlines()
    if meta.exists():
        lines = meta.read_text().splitlines()
        # Conserva o GUID e faz os mapas de dados seguirem a cor linear da Rua Norte.
        color_space = next(line for line in template if line.strip().startswith("sRGBTexture:"))
        corrected = [color_space if line.strip().startswith("sRGBTexture:") else line for line in lines]
        if corrected != lines:
            meta.write_text("\n".join(corrected) + "\n")
    else:
        template[1] = f"guid: {uuid.uuid4().hex}"
        meta.write_text("\n".join(template) + "\n")


if __name__ == "__main__":
    for name, rows in zip(("Terreno", "Superficie", "Relevo"), make_rows()):
        save_atlas(name, rows)
