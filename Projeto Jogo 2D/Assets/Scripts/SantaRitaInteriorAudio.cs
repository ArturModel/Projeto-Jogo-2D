using UnityEngine;

// As salas são renderizadas fora do mundo 2D. A câmera da sala é o ouvinte virtual.
internal sealed class SantaRitaInteriorAudio : MonoBehaviour
{
    private AudioSource fonte;
    private Transform origem, ouvinte;
    private float alcance, ganho;
    internal bool Ligado { get; set; } = true;

    internal static SantaRitaInteriorAudio Criar(Transform dono, Transform origem, Transform ouvinte,
        string efeito, float alcance, float ganho)
    {
        var objeto = new GameObject("Som local — " + efeito);
        objeto.transform.SetParent(dono, false);
        var som = objeto.AddComponent<SantaRitaInteriorAudio>();
        som.origem = origem;
        som.ouvinte = ouvinte;
        som.alcance = alcance;
        som.ganho = ganho;
        som.fonte = objeto.AddComponent<AudioSource>();
        som.fonte.playOnAwake = false;
        som.fonte.spatialBlend = 0f;
        som.fonte.loop = true;
        som.fonte.volume = 0f;
        som.fonte.clip = Resources.Load<AudioClip>("Audio/Interiores/" + efeito);
        return som;
    }

    private void Update()
    {
        if (fonte.clip == null || origem == null || ouvinte == null) return;
        if (Time.timeScale == 0f || MenuPausa.Aberto || !Application.isFocused)
        {
            if (fonte.isPlaying) fonte.Pause();
            return;
        }

        Vector3 delta = origem.position - ouvinte.position;
        float proximidade = 1f - Mathf.SmoothStep(0f, 1f, delta.magnitude / alcance);
        bool minigame = SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing;
        float alvo = Ligado && !minigame ? ganho * proximidade * AudioSettingsController.VolumeEfeitos : 0f;
        if (GameMessageUI.IsShowing) alvo *= .4f;
        fonte.volume = Mathf.MoveTowards(fonte.volume, alvo, Time.unscaledDeltaTime * .13f);
        fonte.panStereo = Pan(ouvinte, delta);
        if (alvo > 0f && !fonte.isPlaying)
        {
            if (fonte.timeSamples > 0) fonte.UnPause();
            else fonte.Play();
        }
        if (alvo == 0f && fonte.volume == 0f && fonte.isPlaying) fonte.Stop();
    }

    internal static void Tocar(string efeito, Vector3 ponto, Transform ouvinte, float ganho, float pitch = 1f)
    {
        Vector3 delta = ponto - ouvinte.position;
        float volume = ganho / (1f + .12f * delta.sqrMagnitude);
        SantaRitaSound.Play("Interiores/" + efeito, volume, pitch, pan: Pan(ouvinte, delta));
    }

    private static float Pan(Transform ouvinte, Vector3 delta) =>
        Mathf.Clamp(Vector3.Dot(ouvinte.right, delta.normalized) * .65f, -.65f, .65f);
}
