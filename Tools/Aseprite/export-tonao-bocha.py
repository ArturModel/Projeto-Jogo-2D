"""Pack the selected source drawings onto a shared foot pivot."""
from pathlib import Path
import struct
import zlib

from PIL import Image

ROOT = Path(__file__).resolve().parents[2]
ART = ROOT / "Arte/Producao/TonaoBocha"
RUNTIME = ROOT / "Projeto Jogo 2D/Assets/Resources/Bocha"
CELL = 128
BASELINE = 112
TAGS = (
    "Andar frente", "Andar costas", "Andar perfil",
    "Bocha frente", "Bocha costas", "Bocha perfil direito", "Bocha perfil esquerdo",
)


def cells(file, rows, scale):
    image = Image.open(file).convert("RGBA")
    width, height = image.size
    for row in range(rows):
        for col in range(4):
            cut = image.crop((width * col // 4, height * row // rows,
                              width * (col + 1) // 4, height * (row + 1) // rows))
            alpha = cut.getchannel("A").point(lambda a: 255 if a >= 160 else 0)
            left, top, right, bottom = alpha.getbbox()
            sole = alpha.crop((0, bottom - max(4, round((bottom - top) * .085)), cut.width, bottom))
            heel, _, toe, _ = sole.getbbox()
            pivot_x = (heel + toe) / 2
            clean = cut.crop((left, top, right, bottom))
            clean.putalpha(alpha.crop((left, top, right, bottom)))
            frame = clean.resize((round(clean.width * scale), round(clean.height * scale)),
                                 Image.Resampling.NEAREST)
            x = round(CELL / 2 + (left - pivot_x) * scale)
            y = BASELINE - frame.height
            assert 0 < x < x + frame.width < CELL and y > 0
            canvas = Image.new("RGBA", (CELL, CELL))
            canvas.paste(frame, (x, y))
            yield canvas


def aseprite(frames):
    def chunk(kind, payload):
        return struct.pack("<IH", len(payload) + 6, kind) + payload

    def string(value):
        data = value.encode("utf-8")
        return struct.pack("<H", len(data)) + data

    layer = chunk(0x2004, struct.pack("<HHHHHHB3x", 1, 0, 0, CELL, CELL, 0, 255) + string("Tonão"))
    tags = struct.pack("<H8x", len(TAGS))
    for i, tag in enumerate(TAGS):
        tags += struct.pack("<HHBH10x", i * 4, i * 4 + 3, 0, 0) + string(tag)
    tags = chunk(0x2018, tags)
    file_frames = []
    for i, frame in enumerate(frames):
        cel = chunk(0x2005, struct.pack("<HhhBHh5xHH", 0, 0, 0, 255, 2, 0, CELL, CELL)
                    + zlib.compress(frame.tobytes()))
        data = layer + tags + cel if i == 0 else cel
        duration = 120 if i < 12 else (235, 260, 155, 340)[i % 4]
        file_frames.append(struct.pack("<IHHH2xI", len(data) + 16, 0xF1FA, 3 if i == 0 else 1, duration, 0) + data)
    payload = b"".join(file_frames)
    header = struct.pack("<IHHHHHIH8xB3xHBB", len(payload) + 128, 0xA5E0, len(frames), CELL, CELL, 32, 1, 120, 0, 0, 1, 1)
    (ART / "TonaoBocha.aseprite").write_bytes(header.ljust(128, b"\0") + payload)


frames = list(cells(ART / "Tonao_Corrigido_Gerado.png", 6, 98 / 208))
frames.extend(cells(ART / "Tonao_PerfilEsquerdo_Gerado.png", 1, 94 / 468))
atlas = Image.new("RGBA", (4 * CELL, len(TAGS) * CELL))
for i, frame in enumerate(frames):
    assert frame.getchannel("A").getbbox()[3] == BASELINE
    atlas.paste(frame, (i % 4 * CELL, i // 4 * CELL))
atlas.save(RUNTIME / "TonaoBocha.png")
aseprite(frames)
print(f"{len(frames)} frames com pivô comum · {atlas.size[0]}×{atlas.size[1]}")
