using System.Collections;
using UnityEngine;

// Ocorrências únicas chamadas após a leitura, registradas no diário pelo diretor.
// Não alteram colisões, pistas materiais ou estado da partida.
public sealed class Capitulo2Perception : MonoBehaviour
{
    public static void Tocar(string flag)
    {
        if(MenuPausa.ReduzirEfeitosIntensos)return; // texto/diário sempre disponíveis
        var host=new GameObject("Percepcao_"+flag);host.AddComponent<Capitulo2Perception>().StartCoroutine(Executar(host,flag));
    }
    private static IEnumerator Executar(GameObject host,string flag)
    {
        if(flag=="c2_sino_percebido")
        {
            const int hz=22050;var samples=new float[hz*3];
            for(int i=0;i<samples.Length;i++)
            {float t=i/(float)hz;samples[i]=(Mathf.Sin(2*Mathf.PI*293.66f*t)+.28f*Mathf.Sin(2*Mathf.PI*806*t)+.13f*Mathf.Sin(2*Mathf.PI*1134*t))*Mathf.Exp(-t*1.9f)*Mathf.Min(1,t*40)*.2f;}
            var clip=AudioClip.Create("C2_SinoPercebidoOriginal",samples.Length,1,hz,false);clip.SetData(samples,0);
            var audio=host.AddComponent<AudioSource>();audio.playOnAwake=false;audio.clip=clip;audio.volume=.32f*PlayerPrefs.GetFloat("audio_efeitos",1);audio.Play();
            float elapsed=0;while(elapsed<3){if(!MenuPausa.Aberto)elapsed+=Time.deltaTime;audio.Pause();if(!MenuPausa.Aberto)audio.UnPause();yield return null;}
            Destroy(clip);
        }
        else
        {
            var sr=Capitulo2Director.Instance!=null?Capitulo2Director.Instance.memorialPercepcao:null;
            if(sr==null)Debug.LogError("Percepção do Cemitério sem referência serializada ao jazigo.");
            if(sr!=null)
            {
                Color original=sr.color;float elapsed=0;
                while(elapsed<2.4f){if(!MenuPausa.Aberto)elapsed+=Time.deltaTime;float peso=Mathf.Sin(Mathf.Clamp01(elapsed/2.4f)*Mathf.PI)*.25f;sr.color=Color.Lerp(original,new Color(.33f,.47f,.51f,1),peso);yield return null;}
                sr.color=original;
            }
        }
        Destroy(host);
    }
}
