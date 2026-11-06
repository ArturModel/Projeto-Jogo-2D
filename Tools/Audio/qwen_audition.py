"""Small, opt-in audition against the official public Qwen demo; never publishes to Assets."""
import argparse
import json
import urllib.error
import urllib.request
from pathlib import Path

HOST = 'https://qwen-qwen3-tts.hf.space'
OUT = Path.home() / '.santarita/voice/previews'
TEXT = 'Bah, Marcus, tu deixou gravando de novo? Espera. Agora fala alguma coisa. Não, assim não, fala normal.'
DIRECTION = ('A fictional adolescent girl from rural Rio Grande do Sul in Brazil, speaking native '
             'Brazilian Portuguese with a subtle natural gaucho accent. Light youthful mid-high '
             'register, relaxed conversational delivery to a friend, small spontaneous pauses and '
             'a hint of amused embarrassment. Not an adult announcer, no cartoon voice, no '
             'theatrical sing-song cadence.')


def fetch(url, payload=None, timeout=30):
    body = json.dumps(payload).encode() if payload is not None else None
    req = urllib.request.Request(url, body, {'Content-Type': 'application/json'})
    with urllib.request.urlopen(req, timeout=timeout) as response:
        return response.read()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--generate', action='store_true', help='Send the fictional audition to the public demo.')
    parser.add_argument('--resume', action='store_true', help='Wait for the last submitted job, without submitting again.')
    parser.add_argument('--text', default=TEXT)
    parser.add_argument('--direction', default=DIRECTION)
    args = parser.parse_args()
    metadata = dict(provider='Qwen official public demo', model='Qwen3-TTS-12Hz-1.7B-VoiceDesign',
                    text=args.text, direction=args.direction, language='Portuguese', approved=False)
    if not args.generate and not args.resume:
        print(json.dumps(metadata, ensure_ascii=False, indent=2))
        print('Dry-run. --generate submits only this text/direction; quotas and availability belong to the demo.')
        return
    OUT.mkdir(parents=True, exist_ok=True)
    job_file = OUT / 'qwen-audition-job.json'
    try:
        if args.resume:
            metadata = json.loads(job_file.read_text(encoding='utf-8'))
        else:
            result = json.loads(fetch(HOST + '/gradio_api/call/generate_voice_design',
                                      {'data': [args.text, 'Portuguese', args.direction]}))
            metadata['event_id'] = result['event_id']
            job_file.write_text(json.dumps(metadata, ensure_ascii=False, indent=2), encoding='utf-8')
        event_id = metadata['event_id']
        if not event_id.isalnum():
            raise ValueError('Invalid event ID.')
        stream = fetch(HOST + '/gradio_api/call/generate_voice_design/' + event_id, timeout=60).decode()
        (OUT / 'qwen-audition-result.txt').write_text(stream, encoding='utf-8')
        if 'event: error' in stream:
            raise RuntimeError('Demo returned an error; see the private result. Do not retry indefinitely or evade quotas.')
        line = next((line[6:] for line in stream.splitlines() if line.startswith('data: [')), None)
        if line is None:
            raise RuntimeError('No complete audio received. Use --resume later.')
        url = json.loads(line)[0]['url']
        if not url.startswith(HOST + '/gradio_api/file='):
            raise ValueError('Unexpected audio origin.')
        audio = OUT / ('julia-qwen-' + event_id[:8] + '.wav')
        audio.write_bytes(fetch(url))
        audio.with_suffix('.json').write_text(json.dumps(metadata, ensure_ascii=False, indent=2), encoding='utf-8')
        print(audio)
        print('Unapproved audition only. No game assets or credentials were uploaded.')
    except (urllib.error.URLError, TimeoutError, RuntimeError, ValueError, OSError) as error:
        raise SystemExit(str(error) + '\nNo local model installed and no automatic publication. An existing job may be resumed.')


if __name__ == '__main__':
    main()
