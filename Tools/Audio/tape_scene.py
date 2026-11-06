"""Offline room-scene renderer. Requires actual per-character takes; no missing-voice substitution."""
import argparse
import json
from pathlib import Path


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--inputs', type=Path, default=Path.home() / '.santarita/voice/room-pilot')
    parser.add_argument('--ambience-only', action='store_true', help='Export the room/foley bed, explicitly without characters.')
    parser.add_argument('--dry-run', action='store_true')
    parser.add_argument('--scene', type=Path, default=Path(__file__).with_name('quarto_scene.json'))
    args = parser.parse_args()
    scene = json.loads(args.scene.read_text(encoding='utf-8'))
    required = [scene['ambience']['file']] + [f['file'] for f in scene['foley']]
    if not args.ambience_only:
        required += [v['file'] for v in scene['cast'].values()]
    missing = [name for name in required if not (args.inputs / name).is_file()]
    if args.dry_run:
        print(json.dumps(dict(mode='ambience_only' if args.ambience_only else 'scene', missing=missing,
                              approved=False, cast=scene['cast']), ensure_ascii=False, indent=2))
        return
    if missing:
        raise SystemExit('Missing takes/assets: ' + ', '.join(missing) + '. No complete scene exported.')
    import numpy as np
    import soundfile as sf
    rate = 24000

    def load(name, trim=False):
        y, sr = sf.read(str(args.inputs / name), dtype='float64', always_2d=True)
        y = y.mean(axis=1)
        if not len(y) or not np.all(np.isfinite(y)):
            raise ValueError('Invalid audio: ' + name)
        if sr != rate:
            y = np.interp(np.arange(round(len(y) * rate / sr)) * sr / rate, np.arange(len(y)), y)
        if trim:
            active = np.flatnonzero(np.abs(y) > max(0.002, np.max(np.abs(y)) * 0.015))
            if not len(active):
                raise ValueError('Empty take: ' + name)
            y = y[max(0, active[0] - 800):min(len(y), active[-1] + 1000)]
        return y

    def fade(y, seconds=0.02):
        y = y.copy()
        n = min(len(y) // 2, round(rate * seconds))
        if n:
            y[:n] *= np.linspace(0, 1, n)
            y[-n:] *= np.linspace(1, 0, n)
        return y

    def rms_gain(y, db):
        return y * (10 ** (db / 20) / max(1e-8, np.sqrt(np.mean(y * y))))

    def lowpass(y, cutoff):
        size = 1 << max(1, len(y) - 1).bit_length()
        freq = np.fft.rfftfreq(size, 1 / rate)
        response = 1 / np.sqrt(1 + (freq / cutoff) ** 8)
        return np.fft.irfft(np.fft.rfft(y, n=size) * response, n=size)[:len(y)]

    cues, takes, cursor = [], [], 0.0
    if not args.ambience_only:
        for line in scene['lines']:
            actor = scene['cast'][line['speaker']]
            take = fade(rms_gain(lowpass(load(actor['file'], trim=True), actor['lowpass_hz']), actor['rms_db']), 0.008)
            start = max(0, cursor + line['gap_before'])
            cursor = start + len(take) / rate
            cues.append(dict(start=start, end=cursor, speaker=line['speaker'], text=line['text']))
            takes.append((start, take, actor))
    duration = max(14.0, cursor + 1.4)
    mix = np.zeros((round(duration * rate), 2))

    def add(y, at, pan=0, gain=1):
        first = round(at * rate)
        n = min(len(y), len(mix) - first)
        if n <= 0:
            return
        angle = (pan + 1) * np.pi / 4
        mix[first:first + n, 0] += y[:n] * np.cos(angle) * gain
        mix[first:first + n, 1] += y[:n] * np.sin(angle) * gain

    room = load(scene['ambience']['file'])
    # The current source is over two minutes long. No artificial loop is needed for this short study.
    if len(room) < len(mix):
        raise ValueError('Room recording shorter than the scene; provide a longer source.')
    room = fade(rms_gain(room[:len(mix)], scene['ambience']['rms_db']), 0.12)
    add(room, 0)
    for f in scene['foley']:
        y = load(f['file'])
        # A short real event, not the entire recording of repeated actions.
        energy = np.convolve(y * y, np.ones(240) / 240, mode='same')
        peak = int(np.argmax(energy))
        before, after = (0.06, 0.35) if f['kind'] == 'button' else (0.18, 0.85)
        y = fade(y[max(0, peak - round(before * rate)):min(len(y), peak + round(after * rate))])
        y *= 10 ** (f['peak_db'] / 20) / max(1e-8, np.max(np.abs(y)))
        add(y, f['time'], 0.05 if f['kind'] == 'button' else 0.3)
    for at, take, actor in takes:
        add(take, at, actor['pan'])
        # Early room reflections: short, quiet, different arrivals; no cavern-style long reverb.
        for delay, scale, side in [(0.023, 0.55, -1), (0.041, 0.36, 1), (0.073, 0.2, -1)]:
            add(lowpass(take, 2200), at + delay, side * 0.3, actor['room_send'] * scale)
    # Optional mono portable-cassette study: bandwidth, mild saturation, wow and hiss.
    if scene.get('cassette'):
        mono = mix.mean(axis=1)
        mono = lowpass(mono, 4800) - lowpass(mono, 120)
        clock = np.arange(len(mono)) / rate
        warped = np.arange(len(mono)) + rate * (0.00032 * np.sin(2*np.pi*.65*clock) + .000045*np.sin(2*np.pi*7*clock))
        mono = np.interp(warped, np.arange(len(mono)), mono)
        mono = np.tanh(mono * 1.5) / 1.5
        noise = np.random.default_rng(1990).normal(size=len(mono))
        noise = lowpass(noise, 7500) - lowpass(noise, 1700)
        mono += rms_gain(noise, -37)
        mono = fade(mono, .035)
        mix = np.column_stack((mono, mono))
    peak = float(np.max(np.abs(mix)))
    if peak > 10 ** (-1.5 / 20):
        mix *= 10 ** (-1.5 / 20) / peak
    name = 'quarto-base-sem-vozes' if args.ambience_only else 'quarto-cena-estudo'
    output = args.inputs / (name + '.wav')
    sf.write(str(output), mix, rate, subtype='PCM_16')
    manifest = dict(status='ambience_only_no_voices' if args.ambience_only else 'unapproved_casting_study',
                    approved=False, seconds=duration, cues=cues, sources=[scene['ambience']] + scene['foley'],
                    peak_db=float(20 * np.log10(max(1e-8, np.max(np.abs(mix))))))
    output.with_suffix('.json').write_text(json.dumps(manifest, ensure_ascii=False, indent=2), encoding='utf-8')
    print(output)
    print('Private study; no export to the game. Cast/acting still require human audition.')


if __name__ == '__main__':
    main()
