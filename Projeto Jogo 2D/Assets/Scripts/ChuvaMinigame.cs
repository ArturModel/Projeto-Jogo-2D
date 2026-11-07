using UnityEngine;

// A raiz de cada minigame controla a duração desta ambiência, inclusive no treino.
[RequireComponent(typeof(AudioSource))]
internal sealed class ChuvaMinigame : MonoBehaviour
{
    private AudioSource fonte;
    private bool pausado;

    private void Awake()
    {
        fonte = GetComponent<AudioSource>();
        fonte.playOnAwake = false;
        fonte.loop = true;
        fonte.spatialBlend = 0f;
        fonte.priority = 192;
        fonte.volume = 0f;
        fonte.clip = Resources.Load<AudioClip>("Audio/ChuvaMinigames");
    }

    private void OnEnable() => AudioSettingsController.AtualizarChuvaMenu();

    private void Update()
    {
        bool pausa = MenuPausa.Aberto || Time.timeScale == 0f || !Application.isFocused;
        if (pausa)
        {
            if (!pausado) fonte.Pause();
            pausado = true;
            return;
        }

        if (pausado) fonte.UnPause();
        pausado = false;

        // Na última mão da sinuca a chuva cresce junto com a tensão da partida.
        float ganho = SinucaMinigameUI.MaoEmCurso >= 3 ? 1.4f : 1.2f;
        float alvo = AudioSettingsController.VolumeChuva * ganho;
        fonte.volume = Mathf.MoveTowards(fonte.volume, alvo, Time.unscaledDeltaTime * .5f);
        if (fonte.clip != null && !fonte.isPlaying) fonte.Play();
    }

    private void OnDisable()
    {
        fonte.Stop();
        fonte.volume = 0f;
        pausado = false;
        AudioSettingsController.AtualizarChuvaMenu();
    }
}
