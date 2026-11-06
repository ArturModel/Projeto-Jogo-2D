"""Prepara derivações de gravações CC0; requer ffmpeg no PATH e numpy.

Fontes e licença: Arte/Producao/PrologoJogavel/Audio.md.
Uso: python Tools/Audio/prepare-prologo-loops.py --motor fonte.mp3 --terra fonte.mp3
"""
import argparse
import subprocess
import wave
from pathlib import Path

import numpy as np


def preparar(origem, destino, inicio, duracao, filtro, rms_alvo):
    raw = subprocess.check_output([
        "ffmpeg", "-v", "error", "-ss", str(inicio), "-i", str(origem),
        "-t", str(duracao), "-af", filtro, "-ar", "44100", "-ac", "2",
        "-f", "f32le", "pipe:1",
    ])
    x = np.frombuffer(raw, dtype="<f4").reshape(-1, 2).copy()
    x -= x.mean(axis=0)
    k = 44100  # costura de um segundo; sem silêncio entre repetições
    if len(x) <= k * 3:
        raise ValueError("Fonte curta demais para o loop")
    t = np.linspace(0, np.pi / 2, k)[:, None]
    costura = x[-k:] * np.cos(t) + x[:k] * np.sin(t)
    x = np.concatenate((x[k:-k], costura))
    ganho = min(rms_alvo / np.sqrt(np.mean(x*x)), .62 / np.max(np.abs(x)))
    x *= ganho
    destino.parent.mkdir(parents=True, exist_ok=True)
    with wave.open(str(destino), "wb") as w:
        w.setnchannels(2)
        w.setsampwidth(2)
        w.setframerate(44100)
        w.writeframes((x * 32767).astype("<i2").tobytes())
    print(f"{destino.name}: {len(x)/44100:.2f}s; pico={np.max(np.abs(x)):.4f}; "
          f"RMS={np.sqrt(np.mean(x*x)):.4f}; salto_loop={np.max(np.abs(x[-1]-x[0])):.5f}")


if __name__ == "__main__":
    p = argparse.ArgumentParser(description=__doc__)
    p.add_argument("--motor", type=Path, required=True)
    p.add_argument("--terra", type=Path, required=True)
    p.add_argument("--output", type=Path, default=Path("Projeto Jogo 2D/Assets/Resources/Audio/Prologo"))
    a = p.parse_args()
    preparar(a.motor, a.output / "MotorInterior.wav", 6, 16,
             "highpass=f=35,lowpass=f=1500", .14)
    preparar(a.terra, a.output / "RodagemTerra.wav", 1, 10,
             "highpass=f=130,lowpass=f=3200,acompressor=threshold=0.035:ratio=5:attack=8:release=130:makeup=2", .17)
