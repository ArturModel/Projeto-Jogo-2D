"""Gera Relevo.png (altura, gradientes XY, oclusão) a partir dos atlas da Rua Norte."""
import sys
from pathlib import Path

import numpy as np
from PIL import Image, ImageFilter


def blur(field, radius):
    source = Image.fromarray(np.uint8(np.clip(field, 0, 1) * 255 + 0.5))
    return np.asarray(source.filter(ImageFilter.GaussianBlur(radius))) / 255.0


def ramp(field, low, high):
    t = np.clip((field - low) / (high - low), 0, 1)
    return t * t * (3 - 2 * t)


def tile_relief(rgb, surface, row, road_rgb, road_surface):
    luma = rgb @ np.array([0.25, 0.55, 0.20])
    roughness, cavities = surface[:, :, 2], surface[:, :, 3]
    coarse = blur(luma, 2.0) - blur(luma, 5.5)
    grain = luma - blur(luma, 1.35)
    x, y = np.meshgrid(np.arange(64), np.arange(64))

    aggregate = blur(luma, 1.0) - blur(luma, 3.5)
    road = 0.31 + np.clip(aggregate * 10 + coarse * 5 + grain * 1.2, -0.14, 0.17)
    # Broad clumps follow the original painted foliage and survive the camera's pixel scale.
    tufts = ramp(coarse, -0.009, 0.015)
    grass = 0.40 + 0.38 * tufts + np.clip(aggregate * 8 + grain * 1.9, -0.14, 0.18)
    dirt = 0.35 + np.clip(coarse * 12 + aggregate * 6 + grain * 1.6, -0.18, 0.21)
    grout = ramp(1 - cavities, 0.065, 0.24)
    bevel = blur(grout, 1.45)
    stone = 0.85 - 0.49 * ramp(bevel, 0.045, 0.64) - 0.18 * grout
    stone += np.clip(coarse * 6 + grain * 1.5, -0.08, 0.09)

    if row in (1, 7, 8, 9):
        # Trincas, poças e faixas usam o mesmo perfil do asfalto da variante
        # correspondente. Só a área pintada do detalhe muda de altura.
        road_luma = road_rgb @ np.array([0.25, 0.55, 0.20])
        base_fine = road_luma - blur(road_luma, 1.35)
        base_aggregate = blur(road_luma, 1.0) - blur(road_luma, 3.5)
        base_coarse = blur(road_luma, 2.0) - blur(road_luma, 5.5)
        height = 0.31 + np.clip(base_aggregate * 10 + base_coarse * 5 + base_fine * 1.2, -0.14, 0.17)
        if row == 7:
            crack = ramp(np.max(np.abs(rgb - road_rgb), axis=-1), 0.015, 0.070)
            height -= 0.085 * crack + 0.075 * blur(crack, 1.15)
        if row == 8:
            water = ramp(road_surface[:, :, 2] - roughness, 0.035, 0.24)
            basin = ramp(blur(water, 2.3), 0.08, 0.72)
            height = height * (1 - water) + 0.16 * water - 0.06 * basin * (1 - water)
    elif row in (2, 10):
        height = stone
        if row == 10:
            height -= 0.44 * ((x >= 12) & (x <= 50) & (y >= 35) & (y <= 56)) * (1 - ramp(luma, 0.21, 0.31))
    elif row in (4, 11, 12):
        # Os tons da vegetação definem os recortes irregulares das transições.
        foliage = 1 - ramp(luma, 0.164, 0.200 if row == 12 else 0.227)
        base = dirt if row == 12 else stone
        height = base * (1 - foliage) + grass * foliage
    elif row == 5:
        curb = 1 - ramp(y, 32, 36)
        lip = ramp(y, 27, 31) * curb
        gutter = ramp(y, 34, 36) * (1 - ramp(y, 37, 42))
        height = road * (1 - curb) + stone * curb + 0.13 * lip - 0.11 * gutter
    elif row == 6:
        shoulder = ramp(roughness, 0.755, 0.812)
        height = road * (1 - shoulder) + dirt * shoulder
    else:
        height = grass if row == 0 else dirt

    height = blur(height, 0.62)
    # Derive signed slopes in UV orientation, independently for each tile.
    dy, dx = np.gradient(height)
    normal_x = 0.5 - np.clip(dx * 1.9, -0.49, 0.49)
    normal_y = 0.5 + np.clip(dy * 1.9, -0.49, 0.49)
    contact = 1 - np.clip((blur(height, 2.6) - height) * 1.65, 0, 0.48)
    return np.stack([height, normal_x, normal_y, contact], axis=-1)


def generate(atlas, output):
    atlas = Path(atlas)
    rgb = np.asarray(Image.open(atlas / "Terreno.png").convert("RGB")) / 255.0
    surface = np.asarray(Image.open(atlas / "Superficie.png").convert("RGBA")) / 255.0
    if rgb.shape[:2] != (832, 256) or surface.shape[:2] != (832, 256):
        raise ValueError("Esperados 13 tipos de terreno com 4 variantes de 64 px.")
    packed = np.empty(surface.shape)
    for row in range(13):
        for variant in range(4):
            box = np.s_[row * 64:(row + 1) * 64, variant * 64:(variant + 1) * 64]
            road_box = np.s_[64:128, variant * 64:(variant + 1) * 64]
            packed[box] = tile_relief(rgb[box], surface[box], row, rgb[road_box], surface[road_box])
    Image.fromarray(np.uint8(np.clip(packed, 0, 1) * 255 + 0.5)).save(output)


if __name__ == "__main__":
    generate(*sys.argv[1:])
