using System.Collections.Generic;
using UnityEngine;

// Mixer simples de camadas de ambiência.
//
// O documento (§9) trata o áudio do prólogo como camadas independentes que
// mudam de peso ao longo da cena: a chuva troca de textura ao chegar nas
// casas, o motor some ao estacionar, e é justamente esse silêncio relativo
// que prepara a troca de perspectiva. Um AudioSource por camada, com volume
// indo ao alvo por interpolação — nada de parar e recomeçar, que estala.
public class AudioAmbiente : MonoBehaviour
{
    [System.Serializable]
    public class Camada
    {
        public string nome = "camada";
        public AudioClip clipe;
        public bool loop = true;

        [Range(0f, 1f)]
        public float volumeInicial;

        [System.NonSerialized] public AudioSource fonte;
        [System.NonSerialized] public float alvo;
        [System.NonSerialized] public float volumeAtual;
        [System.NonSerialized] public float velocidade = 1f;
    }

    public static AudioAmbiente Instance { get; private set; }

    [SerializeField] private Camada[] camadas;

    private readonly Dictionary<string, Camada> porNome = new Dictionary<string, Camada>();
    private bool pausado;
    private bool foco = true;

    private void Awake()
    {
        Instance = this;

        foreach (Camada c in camadas)
        {
            AudioSource fonte = gameObject.AddComponent<AudioSource>();
            fonte.clip = c.clipe;
            fonte.loop = c.loop;
            fonte.playOnAwake = false;
            fonte.volume = c.volumeInicial;
            fonte.spatialBlend = 0f;

            c.fonte = fonte;
            c.alvo = c.volumeInicial;
            c.volumeAtual = c.volumeInicial;
            porNome[c.nome] = c;

            if (c.loop && c.clipe != null)
            {
                fonte.Play();
            }
        }
    }

    private void Update()
    {
        bool pausa = !foco || Time.timeScale == 0 || MenuPausa.Aberto;
        if (pausa != pausado)
        {
            pausado = pausa;
            foreach (Camada c in camadas)
                if (c.fonte != null) { if (pausa) c.fonte.Pause(); else c.fonte.UnPause(); }
        }
        if (pausa) return;
        float efeitos = AudioSettingsController.VolumeEfeitos;
        foreach (Camada c in camadas)
        {
            if (c.fonte == null)
            {
                continue;
            }

            c.volumeAtual = Mathf.MoveTowards(
                c.volumeAtual, c.alvo, c.velocidade * Time.deltaTime
            );
            float ganho = c.nome == "chuva" ? AudioSettingsController.VolumeChuva : efeitos;
            c.fonte.volume = c.loop ? c.volumeAtual * ganho : ganho;
        }
    }

    // tempo = segundos até chegar no volume pedido.
    public void Definir(string nome, float volume, float tempo = 1f)
    {
        if (!porNome.TryGetValue(nome, out Camada c))
        {
            return;
        }

        float distancia = Mathf.Abs(volume - c.volumeAtual);
        c.alvo = Mathf.Clamp01(volume);
        c.velocidade = tempo <= 0.01f ? 99f : Mathf.Max(0.01f, distancia / tempo);
    }

    public void Pitch(string nome, float pitch)
    {
        if (porNome.TryGetValue(nome, out Camada c) && c.fonte != null)
        {
            c.fonte.pitch = pitch;
        }
    }

    // O prólogo reaproveita a fonte do motor e acrescenta só a superfície nova.
    // As camadas continuam pertencendo a este mixer (volume, foco e pausa comuns).
    public void ConfigurarLoop(string nome, AudioClip clipe)
    {
        if (clipe == null) { Debug.LogError("Loop ausente: " + nome); return; }
        Camada c;
        if (!porNome.TryGetValue(nome, out c))
        {
            c = new Camada { nome = nome, fonte = gameObject.AddComponent<AudioSource>() };
            System.Array.Resize(ref camadas, camadas.Length + 1);
            camadas[camadas.Length - 1] = c;
            porNome[nome] = c;
        }
        c.clipe = clipe; c.loop = true;
        c.alvo = c.volumeAtual = 0;
        c.fonte.Stop(); c.fonte.clip = clipe; c.fonte.loop = true;
        c.fonte.playOnAwake = false; c.fonte.spatialBlend = 0; c.fonte.volume = 0;
        c.fonte.Play();
        if (pausado) c.fonte.Pause();
    }

    // Som avulso: uma passada de limpador, um clique de seta.
    public void Disparar(string nome, float volume = 1f)
    {
        if (pausado || !foco || Time.timeScale == 0) return;
        if (porNome.TryGetValue(nome, out Camada c) && c.clipe != null)
        {
            // Camadas avulsas normalmente têm volumeInicial = 0.
            c.fonte.volume = AudioSettingsController.VolumeEfeitos;
            c.fonte.PlayOneShot(c.clipe, volume);
        }
    }

    public bool Tocando(string nome)
    {
        return porNome.TryGetValue(nome, out Camada c) && c.fonte != null && c.fonte.volume > 0.01f;
    }

    private void OnApplicationFocus(bool ativo) { foco = ativo; }
    private void OnDestroy() { if (Instance == this) Instance = null; }
}
