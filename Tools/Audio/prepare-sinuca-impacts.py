"""Cut CC0 field-recorded ball collisions. Requires ffmpeg and numpy.

Usage: python Tools/Audio/prepare-sinuca-impacts.py /path/to/62331_791247-hq.mp3
Source download is deliberately separate; no network or Unity scene changes.
"""
from pathlib import Path
import hashlib
import json
import subprocess
import sys
import wave
import numpy as np

root = Path(__file__).resolve().parents[2]
source = Path(sys.argv[1])
rate = 44100
signal = np.frombuffer(subprocess.check_output([
    'ffmpeg', '-v', 'error', '-i', str(source), '-ac', '1', '-ar', str(rate),
    '-f', 'f32le', 'pipe:1']), dtype='<f4')
destination = root / 'Projeto Jogo 2D/Assets/Resources/Audio/Sinuca'
destination.mkdir(parents=True, exist_ok=True)
report = {'source': 'https://freesound.org/people/bsumusictech/sounds/62331/',
          'author': 'bsumusictech', 'license': 'CC0-1.0',
          'input_sha256': hashlib.sha256(source.read_bytes()).hexdigest(),
          'listening': 'Pending human evaluation in game', 'clips': []}
for index, approximate_peak in enumerate((8.14, 14.85, 16.99), 1):
    # Retain 3 ms before the local transient and its natural 177 ms decay.
    lo, hi = (int((approximate_peak + offset) * rate) for offset in (-.01, .02))
    peak = lo + int(np.argmax(np.abs(signal[lo:hi])))
    start = peak - int(.003 * rate)
    samples = signal[start:start + int(.18 * rate)].astype(np.float64).copy()
    samples -= np.mean(samples)
    # Gentle 100 Hz high-pass removes rumble without resonant/synthetic tones.
    alpha = np.exp(-2 * np.pi * 100 / rate)
    previous_input = previous_output = 0.
    for i, value in enumerate(samples.copy()):
        previous_output = alpha * (previous_output + value - previous_input)
        previous_input = value
        samples[i] = previous_output
    samples[:44] *= np.linspace(0, 1, 44)
    samples[-1323:] *= np.linspace(1, 0, 1323)
    samples *= .72 / np.max(np.abs(samples))
    pcm = np.rint(samples * 32767).astype('<i2')
    path = destination / f'Colisao_{index:02}.wav'
    with wave.open(str(path), 'wb') as output:
        output.setparams((1, 2, rate, 0, 'NONE', 'not compressed'))
        output.writeframes(pcm.tobytes())
    # Inspect the actual PCM deliverable, not only the float processing buffer.
    with wave.open(str(path), 'rb') as output:
        assert output.getparams()[:3] == (1, 2, rate)
        decoded = np.frombuffer(output.readframes(output.getnframes()), '<i2') / 32768.
    assert np.isfinite(decoded).all() and 0 < np.max(np.abs(decoded)) < .73
    assert decoded[0] == decoded[-1] == 0
    report['clips'].append({'file': path.name, 'start_seconds': start / rate,
        'duration_seconds': len(decoded) / rate,
        'peak': float(np.max(np.abs(decoded))), 'dc': float(np.mean(decoded)),
        'rms': float(np.sqrt(np.mean(decoded ** 2))),
        'sha256': hashlib.sha256(path.read_bytes()).hexdigest()})
assert len({clip['sha256'] for clip in report['clips']}) == 3
evidence = root / 'Docs/Evidencias/SinucaMecanicas/bolas-reais-qa.json'
evidence.write_text(json.dumps(report, indent=2) + '\n', encoding='utf-8')
print(json.dumps(report, indent=2))
