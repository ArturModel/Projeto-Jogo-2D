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
        [System.NonSerialized] public float velocidade = 1f;
    }

    public static AudioAmbiente Instance { get; private set; }

    [SerializeField] private Camada[] camadas;

    private readonly Dictionary<string, Camada> porNome = new Dictionary<string, Camada>();

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
            porNome[c.nome] = c;

            if (c.loop && c.clipe != null)
            {
                fonte.Play();
            }
        }
    }

    private void Update()
    {
        foreach (Camada c in camadas)
        {
            if (c.fonte == null)
            {
                continue;
            }

            c.fonte.volume = Mathf.MoveTowards(
                c.fonte.volume, c.alvo, c.velocidade * Time.deltaTime
            );
        }
    }

    // tempo = segundos até chegar no volume pedido.
    public void Definir(string nome, float volume, float tempo = 1f)
    {
        if (!porNome.TryGetValue(nome, out Camada c))
        {
            return;
        }

        float distancia = Mathf.Abs(volume - c.fonte.volume);
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

    // Som avulso: uma passada de limpador, um clique de seta.
    public void Disparar(string nome, float volume = 1f)
    {
        if (porNome.TryGetValue(nome, out Camada c) && c.clipe != null)
        {
            c.fonte.PlayOneShot(c.clipe, volume);
        }
    }

    public bool Tocando(string nome)
    {
        return porNome.TryGetValue(nome, out Camada c) && c.fonte != null && c.fonte.volume > 0.01f;
    }
}
