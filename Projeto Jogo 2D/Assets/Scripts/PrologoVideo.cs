using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;

// Versão de teste do prólogo: um vídeo em tela cheia com as legendas por cima.
//
// Não substitui o prólogo em camadas (carro, estrada, objetos clicáveis) —
// aquilo continua na cena, só desativado. Serve para avaliar ritmo e clima com
// uma referência visual pronta antes de investir na arte definitiva.
//
// O vídeo fica em StreamingAssets e é lido por URL, sem passar pelo importador
// de vídeo do Unity: trocar o arquivo por uma versão melhor é só sobrescrever
// o .mp4 com o mesmo nome.
public class PrologoVideo : MonoBehaviour
{
    [System.Serializable]
    public class Fala
    {
        [Tooltip("Segundo do vídeo em que a fala entra.")]
        public float tempo;

        [TextArea]
        public string texto;

        public float duracao = 1.5f;
    }

    [Header("Vídeo")]
    [Tooltip("Arquivo dentro de Assets/StreamingAssets.")]
    [SerializeField] private string arquivo = "Prologo_teste.mp4";

    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    [Header("Destino")]
    [SerializeField] private string cenaSeguinte = "EntradaDaCidade";
    [SerializeField] private string entradaNaCena = "saida_carro";

    // Cabem quatro falas em 10 segundos: cada legenda ocupa a duração mais
    // ~0,85 s de fade e respiro. Os tempos seguem o vídeo — a última entra
    // quando a igreja aparece.
    [Header("Legendas")]
    [SerializeField]
    private Fala[] falas =
    {
        new Fala { tempo = 0.3f, texto = "\"Desde cedo sem atender.\"", duracao = 1.2f },
        new Fala { tempo = 2.35f, texto = "\"O vô Adílio nunca deixa o telefone tocar até cair.\"", duracao = 1.8f },
        new Fala { tempo = 5.0f, texto = "\"Seis anos sem voltar pra Santa Rita.\"", duracao = 1.4f },
        new Fala { tempo = 7.25f, texto = "\"Certo... vamos ver por que ele não atendeu hoje.\"", duracao = 1.8f },
    };

    private VideoPlayer player;
    private RenderTexture alvo;
    private bool encerrando;

    private IEnumerator Start()
    {
        MontarTela();

        player = gameObject.AddComponent<VideoPlayer>();
        player.playOnAwake = false;
        player.isLooping = false;
        player.source = VideoSource.Url;
        player.url = Path.Combine(Application.streamingAssetsPath, arquivo);
        player.renderMode = VideoRenderMode.RenderTexture;
        player.targetTexture = alvo;
        player.audioOutputMode = VideoAudioOutputMode.Direct;
        player.controlledAudioTrackCount = 1;
        player.EnableAudioTrack(0, true);

        player.loopPointReached += _ => Encerrar();
        player.errorReceived += (_, mensagem) =>
        {
            Debug.LogError($"Vídeo do prólogo falhou ({player.url}): {mensagem}");
            Encerrar();
        };

        player.Prepare();

        while (!player.isPrepared && !encerrando)
        {
            yield return null;
        }

        if (encerrando)
        {
            yield break;
        }

        player.SetDirectAudioVolume(0, volume);
        player.Play();
        StartCoroutine(Legendar());
    }

    private void MontarTela()
    {
        alvo = new RenderTexture(1280, 720, 0);

        // Abaixo da legenda (500) e do fade de cena (9999).
        Canvas canvas = UIFactory.CreateCanvas(transform, "VideoCanvas", 100);

        Image fundo = new GameObject("Preto").AddComponent<Image>();
        fundo.transform.SetParent(canvas.transform, false);
        fundo.color = Color.black;
        fundo.raycastTarget = false;
        UIFactory.Stretch(fundo.rectTransform);

        RawImage tela = new GameObject("Video").AddComponent<RawImage>();
        tela.transform.SetParent(canvas.transform, false);
        tela.texture = alvo;
        tela.raycastTarget = false;
        UIFactory.Stretch(tela.rectTransform);

        // Preenche a tela sem distorcer, qualquer que seja a proporção da janela.
        AspectRatioFitter ajuste = tela.gameObject.AddComponent<AspectRatioFitter>();
        ajuste.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        ajuste.aspectRatio = 16f / 9f;
    }

    // Segue o relógio do próprio vídeo, não o do jogo: se o vídeo engasgar no
    // começo, a legenda espera junto em vez de sair adiantada.
    private IEnumerator Legendar()
    {
        foreach (Fala fala in falas)
        {
            while (!encerrando && player.time < fala.tempo)
            {
                yield return null;
            }

            if (encerrando)
            {
                yield break;
            }

            Legenda.Dizer(fala.texto, fala.duracao, Legenda.PrioridadeRoteiro);
        }
    }

    private void Update()
    {
        // Atalho de desenvolvimento, igual ao prólogo em camadas.
        if (!encerrando && Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Encerrar();
        }
    }

    private void Encerrar()
    {
        if (encerrando)
        {
            return;
        }

        encerrando = true;
        Legenda.Limpar();

        // O último quadro fica congelado na tela enquanto o fade da cena cobre.
        GameState.Marcar(Flags.PrologoConcluido);
        GameState.RegistrarNotaDiario(
            "prologo_chegada_2004",
            "Chegada a Santa Rita (2004)",
            "FATO",
            "Adílio parou de atender o telefone hoje cedo. Peguei meu carro na chuva e voltei a Santa Rita pela primeira vez desde 1998."
        );
        GameState.DefinirObjetivo("Ir até a casa de Adílio no Bairro das Casas, nº 40 (Leste).");
        SceneTransition.Ir(cenaSeguinte, entradaNaCena);
    }

    private void OnDestroy()
    {
        if (alvo != null)
        {
            alvo.Release();
            Destroy(alvo);
        }
    }
}
