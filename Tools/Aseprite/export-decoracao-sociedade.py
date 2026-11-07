"""Draw original 32 PPU furniture sprites. Run with Python and Pillow."""

from pathlib import Path
from random import Random

from PIL import Image, ImageDraw

OUT = Path(__file__).resolve().parents[2] / "Projeto Jogo 2D/Assets/Resources/Bocha"
WOOD = "#684935"
EDGE = "#9a7251"
SHADOW = "#292722"


def frame(width, height, background):
    image = Image.new("RGBA", (width, height), SHADOW)
    draw = ImageDraw.Draw(image)
    draw.rectangle((1, 1, width - 2, height - 2), fill=WOOD)
    draw.rectangle((2, 2, width - 3, height - 3), outline=EDGE)
    draw.rectangle((4, 4, width - 5, height - 5), fill=background)
    draw.line((4, height - 4, width - 5, height - 4), fill="#3e2c25")
    return image, draw


def pendant():
    image = Image.new("RGBA", (48, 44))
    draw = ImageDraw.Draw(image)
    draw.rectangle((20, 1, 27, 4), fill="#32372f")
    draw.rectangle((23, 4, 25, 26), fill="#262c29")
    draw.line((23, 5, 23, 23), fill="#6d6c58")
    draw.polygon([(14, 25), (33, 25), (41, 33), (41, 37), (6, 37), (6, 33)], fill="#2c3430")
    draw.polygon([(15, 26), (32, 26), (38, 33), (9, 33)], fill="#505e4a")
    draw.line((11, 32, 36, 32), fill="#747c61")
    draw.rectangle((9, 35, 38, 38), fill="#8f6841")
    draw.rectangle((11, 35, 36, 37), fill="#cda76d")
    draw.rectangle((14, 35, 32, 36), fill="#e7c788")
    image.save(OUT / "PendenteSociedade.png")


def placar():
    image, draw = frame(54, 36, "#263930")
    draw.line((26, 7, 26, 29), fill="#5e6b56")
    draw.line((7, 16, 46, 16), fill="#6b7159")
    glyphs = {
        "M": ["10001", "11011", "10101", "10001", "10001"],
        "T": ["11111", "00100", "00100", "00100", "00100"],
        "0": ["01110", "10001", "10001", "10001", "01110"],
    }
    for glyph, x, y in [("M", 13, 8), ("T", 36, 8), ("0", 13, 21), ("0", 36, 21)]:
        for dy, row in enumerate(glyphs[glyph]):
            for dx, mark in enumerate(row):
                if mark == "1":
                    draw.point((x + dx, y + dy), fill="#c2be9a")
    draw.line((36, 32, 42, 32), fill="#d0c8a4")
    image.save(OUT / "PlacarSociedade.png")


def mural():
    image, draw = frame(70, 44, "#775a3d")
    rng = Random(2004)
    for _ in range(165):
        draw.point((rng.randrange(5, 65), rng.randrange(5, 39)), fill=rng.choice(["#856447", "#674e37", "#907254"]))
    for x, y, w, h, color in [
        (9, 9, 15, 19, "#c7b391"), (25, 16, 16, 18, "#9fada0"),
        (43, 8, 16, 20, "#c2a782"), (46, 29, 15, 7, "#ac936d"),
    ]:
        draw.rectangle((x + 1, y + 1, x + w, y + h), fill="#47372a")
        draw.rectangle((x, y, x + w - 1, y + h - 1), fill=color)
        draw.point((x + w // 2, y + 1), fill="#76593b")
        for line in range(y + 5, y + h - 1, 3):
            draw.line((x + 2, line, x + rng.randrange(6, w - 1), line), fill="#766c54")
    image.save(OUT / "MuralSociedade.png")


def flamula():
    image, draw = frame(37, 43, "#a38e71")
    draw.polygon([(8, 9), (28, 9), (28, 26), (18, 35), (8, 26)], fill="#253b32")
    draw.line([(8, 9), (8, 26), (18, 35), (28, 26)], fill="#6f6e47")
    draw.line([(12, 21), (15, 19), (19, 20), (24, 17)], fill="#b7a374", width=2)
    draw.line([(12, 25), (16, 23), (20, 24), (24, 21)], fill="#869273")
    image.save(OUT / "FlamulaSociedade.png")


if __name__ == "__main__":
    pendant()
    placar()
    mural()
    flamula()
