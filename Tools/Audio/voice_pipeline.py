"""ElevenLabs production helper. Dry-run by default; keys never enter the repository or Player."""
import argparse,base64,json,os,sys,urllib.request,urllib.error
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
CONFIG=Path.home()/'.codex/private/santa-rita-voice.json'
def main():
 p=argparse.ArgumentParser();p.add_argument('--tape',default='fita01');p.add_argument('--sample',action='store_true');p.add_argument('--generate',action='store_true');p.add_argument('--list-voices',action='store_true');a=p.parse_args()
 cfg=json.loads(CONFIG.read_text(encoding='utf-8-sig')) if CONFIG.exists() else {}
 key=os.getenv('ELEVENLABS_API_KEY') or cfg.get('api_key','')
 catalog=json.loads((ROOT/'Projeto Jogo 2D/Assets/Resources/Tapes/Catalog.json').read_text(encoding='utf-8-sig'))
 tape=next((t for t in catalog['tapes'] if t['id']==a.tape),None)
 if tape is None:raise SystemExit('Fita desconhecida.')
 def request(path,payload=None):
  if not key:raise SystemExit('Falta chave ElevenLabs no arquivo privado ou variável ELEVENLABS_API_KEY.')
  headers={'xi-api-key':key,'Content-Type':'application/json'}
  req=urllib.request.Request('https://api.elevenlabs.io'+path,json.dumps(payload).encode() if payload is not None else None,headers)
  try:
   with urllib.request.urlopen(req,timeout=180) as r:return json.load(r)
  except urllib.error.HTTPError as e:raise SystemExit('ElevenLabs respondeu HTTP '+str(e.code)+'. Conferir acesso/créditos; chave e resposta privada não foram impressas.')
 if a.list_voices:
  r=request('/v2/voices?page_size=100');print(json.dumps([{'name':v['name'],'voice_id':v['voice_id']} for v in r.get('voices',[])],ensure_ascii=False,indent=2));return
 lines=[l for l in tape['lines'] if l['speaker']!='AMBIENTE']
 if a.sample:lines=[l for l in lines if l['speaker']=='JÚLIA'][:2]
 cast=dict(cfg.get('voices',{}))
 if a.sample and cfg.get('sample_voice'):cast['JÚLIA']=cfg['sample_voice']
 missing=sorted({l['speaker'] for l in lines if not cast.get(l['speaker'])})
 print('Fita:',a.tape,'| falas:',len(lines),'| modo:', 'GERAR (consome créditos da conta)' if a.generate else 'PRÉVIA SEM REDE / SEM CUSTO')
 if missing:print('Vozes ainda sem escolha:',', '.join(missing))
 if not a.generate:return
 if missing:raise SystemExit('Escolher os voice_ids no arquivo privado antes de gerar.')
 payload={'inputs':[{'text':l['text'],'voice_id':cast[l['speaker']]} for l in lines]}
 if a.sample:
  r=request('/v1/text-to-speech/'+cast['JÚLIA']+'/with-timestamps?output_format=mp3_44100_128',{'text':' '.join(l['text'] for l in lines),'model_id':'eleven_v3','language_code':'pt'})
 else:r=request('/v1/text-to-dialogue/with-timestamps?output_format=mp3_44100_128',payload)
 out=Path.home()/'.santarita/voice/previews';out.mkdir(parents=True,exist_ok=True);name=a.tape+('-amostra' if a.sample else '')
 (out/(name+'.mp3')).write_bytes(base64.b64decode(r['audio_base64']))
 cues=[]
 for segment in r.get('voice_segments',[]):
  line=lines[segment['dialogue_input_index']];cues.append(dict(start=segment['start_time_seconds'],end=segment['end_time_seconds'],speaker=line['speaker'],text=line['text']))
 (out/(name+'.json')).write_text(json.dumps({'cues':cues,'provider':'ElevenLabs','approved':False},ensure_ascii=False,indent=2),encoding='utf-8')
 print('Amostra local:',out/(name+'.mp3'));print('Sem publicação automática. Ouvir e aprovar atuação/sotaque antes de integrar.')
if __name__=='__main__':main()
