#!/usr/bin/env python3
"""
Gera a folha direcional de caminhada do Marcus segurando a lanterna (576x256, 4 linhas x 9 colunas de 64x64)
a partir de Arte/Producao/Marcus/MarcusCaminhadaV20.png usando Pillow (PIL).

Regras visuais:
- Preserva 100% da animação existente de pernas, cabeça, tronco e braço livre.
- Mantém o braço que segura a lanterna fixo/estável (acompanhando apenas o bob vertical de 1px do tronco)
  para que a lanterna não balance durante a caminhada.
- Lanterna compacta de época (2004), coerente com a mão de 3x2 px em 32 PPU.
"""

from pathlib import Path
from PIL import Image


def gerar_marcus_lanterna(src_path: Path, dst_paths: list[Path]) -> None:
    im = Image.open(src_path).convert("RGBA")
    if im.size != (576, 256):
        raise ValueError(f"Dimensão inesperada em {src_path}: {im.size} (esperado 576x256)")

    out = im.copy()

    # Paleta da lanterna compacta (alinhada ao contorno e iluminação de Santa Rita)
    OUT = (18, 15, 22, 255)      # contorno escuro
    BODY = (48, 52, 62, 255)     # corpo metálico escuro
    HI = (86, 92, 106, 255)      # brilho especular discreto no cilindro
    LENS = (232, 238, 246, 255)  # lente branca suave
    RIM = (148, 158, 172, 255)   # anel frontal metálico frio
    JEANS = (54, 57, 79, 255)    # tom base da calça jeans para recompor onde a mão antiga descia
    JEANS_DARK = (39, 41, 58, 255)

    # Bob vertical da cabeça/tronco por coluna (0..7 = walk, 8 = idle)
    dys = [0, 1, 0, -1, 0, 1, 0, -1, 0]

    # =========================================================================
    # LINHA 0 (FRENTE / SUL): braço à direita da tela (x >= 36) fixo + lanterna frontal
    # =========================================================================
    ref_down = im.crop((2 * 64, 0, 3 * 64, 64))
    for col in range(9):
        dy = dys[col]
        cell = out.crop((col * 64, 0, (col + 1) * 64, 64))

        for y in range(27 + dy, 44):
            for x in range(36, 44):
                cell.putpixel((x, y), (0, 0, 0, 0))

        for y in range(27, 42):
            yd = y + dy
            if 0 <= yd < 64:
                for x in range(36, 44):
                    cell.putpixel((x, yd), ref_down.getpixel((x, y)))

        hy = 37 + dy
        # Lanterna apontando para frente/baixo na mão (x=37..39, y=37..39+dy)
        cell.putpixel((37, hy), OUT)
        cell.putpixel((38, hy), BODY)
        cell.putpixel((39, hy), OUT)

        cell.putpixel((36, hy + 1), OUT)
        cell.putpixel((37, hy + 1), RIM)
        cell.putpixel((38, hy + 1), LENS)
        cell.putpixel((39, hy + 1), OUT)

        cell.putpixel((37, hy + 2), OUT)
        cell.putpixel((38, hy + 2), OUT)

        out.paste(cell, (col * 64, 0))

    # =========================================================================
    # LINHA 1 (COSTAS / NORTE): braço à direita da tela (x >= 36) fixo + lanterna apontando para o norte
    # =========================================================================
    ref_up = im.crop((2 * 64, 64, 3 * 64, 128))
    for col in range(9):
        dy = dys[col]
        cell = out.crop((col * 64, 64, (col + 1) * 64, 128))

        for y in range(27 + dy, 44):
            for x in range(36, 44):
                cell.putpixel((x, y), (0, 0, 0, 0))

        for y in range(27, 42):
            yd = y + dy
            if 0 <= yd < 64:
                for x in range(36, 44):
                    cell.putpixel((x, yd), ref_up.getpixel((x, y)))

        hy = 37 + dy
        # Traseira do cabo aparecendo na mão (visão de costas)
        cell.putpixel((37, hy), OUT)
        cell.putpixel((38, hy), HI)
        cell.putpixel((39, hy), OUT)
        cell.putpixel((37, hy + 1), OUT)
        cell.putpixel((38, hy + 1), BODY)
        cell.putpixel((39, hy + 1), OUT)

        out.paste(cell, (col * 64, 64))

    # =========================================================================
    # LINHA 2 (ESQUERDA / OESTE) e LINHA 3 (DIREITA / LESTE espelhada)
    # =========================================================================
    ref_left = im.crop((0, 2 * 64, 64, 3 * 64))
    for col in range(9):
        dy = dys[col]
        cell = out.crop((col * 64, 2 * 64, (col + 1) * 64, 3 * 64))

        # Limpa a faixa do tronco/braço e aplica o braço fixo da coluna 0 (deslocado por dy)
        for y in range(26 + dy, 37 + dy):
            for x in range(22, 42):
                cell.putpixel((x, y), (0, 0, 0, 0))

        for y in range(26, 37):
            yd = y + dy
            for x in range(24, 39):
                cell.putpixel((x, yd), ref_left.getpixel((x, y)))

        # Recompõe a cintura (y = 37+dy e 38+dy) onde a mão antiga descia sobre o jeans nas colunas 1..8
        for y_off in (37, 38):
            yd = y_off + dy
            for x in range(25, 42):
                r, g, b, a = cell.getpixel((x, yd))
                if a == 0:
                    continue
                is_skin = r > 110 and g > 65
                is_shirt = (r > 30 and abs(r - g) < 12 and b < r + 6)
                if x < 27 or x > 35:
                    # Remove pixels de braço/mão antigos fora da largura do quadril
                    if is_skin or is_shirt or x >= 36:
                        cell.putpixel((x, yd), (0, 0, 0, 0))
                elif is_skin or is_shirt:
                    # Substitui a mão antiga que cobria a calça pelo jeans
                    cell.putpixel((x, yd), JEANS if (x + y_off) % 2 == 0 else JEANS_DARK)

            # Garante contorno contínuo na parte de trás da cintura (x=35) alinhado ao tronco
            if cell.getpixel((34, yd))[3] > 0 and cell.getpixel((35, yd))[3] == 0:
                cell.putpixel((35, yd), OUT)
            elif cell.getpixel((33, yd))[3] > 0 and cell.getpixel((34, yd))[3] == 0 and y_off == 37:
                cell.putpixel((34, yd), JEANS_DARK)
                cell.putpixel((35, yd), OUT)

        # Na coluna 8 (idle), limpa resquício da mão frontal em x=24..25 e da mão traseira em y=39
        if col == 8:
            for y_c8 in range(35, 40):
                cell.putpixel((24, y_c8), (0, 0, 0, 0))
                cell.putpixel((25, y_c8), (0, 0, 0, 0))
            cell.putpixel((26, 37), OUT)
            cell.putpixel((26, 38), OUT)
            cell.putpixel((26, 39), OUT)
            for x in range(32, 37):
                if cell.getpixel((x, 39))[0] < 30 and cell.getpixel((x, 40))[3] == 0:
                    cell.putpixel((x, 39), (0, 0, 0, 0))
            cell.putpixel((33, 40), OUT)

        # Desenha a lanterna compacta na mão (mão está em x=30..32, y=35+dy..36+dy)
        # Projeta apenas 3 pixels à frente da mão (x=27..29)
        hy = 35 + dy
        cell.putpixel((27, hy - 1), OUT)
        cell.putpixel((28, hy - 1), OUT)
        cell.putpixel((29, hy - 1), OUT)

        cell.putpixel((27, hy), LENS)
        cell.putpixel((28, hy), RIM)
        cell.putpixel((29, hy), HI)
        cell.putpixel((30, hy), BODY)

        cell.putpixel((27, hy + 1), LENS)
        cell.putpixel((28, hy + 1), BODY)
        cell.putpixel((29, hy + 1), BODY)

        cell.putpixel((27, hy + 2), OUT)
        cell.putpixel((28, hy + 2), OUT)
        cell.putpixel((29, hy + 2), OUT)

        out.paste(cell, (col * 64, 2 * 64))
        out.paste(cell.transpose(Image.FLIP_LEFT_RIGHT), (col * 64, 3 * 64))

    # Validação de contrato (mesmas regras de PlayerController.ValidarFolhaMarcus)
    for row in range(4):
        for col in range(9):
            c = out.crop((col * 64, row * 64, (col + 1) * 64, (row + 1) * 64))
            opaque = 0
            for y in range(64):
                for x in range(64):
                    a = c.getpixel((x, y))[3]
                    if a > 32 and (x == 0 or x == 63 or y == 0 or y == 63):
                        raise AssertionError(f"Pixel na borda em row={row}, col={col} ({x},{y})")
                    if a > 10:
                        opaque += 1
            if not (180 <= opaque <= 1400):
                raise AssertionError(f"Contagem opaca inválida em row={row}, col={col}: {opaque}")

    for dst in dst_paths:
        dst.parent.mkdir(parents=True, exist_ok=True)
        out.save(dst)
        print(f"Salvo com sucesso: {dst} ({out.size[0]}x{out.size[1]})")


if __name__ == "__main__":
    root = Path(__file__).resolve().parents[2]
    src = root / "Arte" / "Producao" / "Marcus" / "MarcusCaminhadaV20.png"
    dsts = [
        root / "Arte" / "Producao" / "Marcus" / "MarcusLanterna.png",
        root / "Projeto Jogo 2D" / "Assets" / "StreamingAssets" / "MarcusLanterna.png",
    ]
    gerar_marcus_lanterna(src, dsts)
