using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// A régua de tempo do prólogo (§2 do documento de roteiro).
//
// O prólogo não é um tutorial e não pode ser interrompido por objetivo nenhum:
// quem só olha para a frente chega ao fim igual. Por isso tudo aqui é tempo
// corrido — o diretor mexe em velocidade, curva, áudio e falas, e o jogador
// mexe na câmera e nos três objetos, sem que uma coisa espere a outra.
public class PrologoDirector : MonoBehaviour
{
    [System.Serializable]
    public class FalaMarcada
    {
        [Tooltip("Segundos desde o início da cena.")]
        public float tempo;

        [TextArea]
        public string texto;

        public float duracao = 3f;
    }

    [Header("Peças da cena")]
    [SerializeField] private EstradaScroller estrada;
    [SerializeField] private LimpadorParabrisa limpadores;
    [SerializeField] private PonteiroCarro ponteiro;

    [Header("Destino")]
    [SerializeField] private string cenaSeguinte = "EntradaDaCidade";
    [SerializeField] private string entradaNaCena = "saida_carro";

    [Header("Abertura")]
    [Tooltip("Tela preta só com som, antes da imagem (§2, 0:00–0:03).")]
    [SerializeField] private float esperaNoEscuro = 3f;

    [SerializeField] private float duracaoAberturaImagem = 2.5f;

    [Header("Estrada")]
    [SerializeField] private float velocidadeEstrada = 15f;
    [SerializeField] private float tempoAtePlaca = 15f;
    [SerializeField] private float tempoAteCurva = 22f;
    [SerializeField] private float duracaoCurva = 5f;
    [SerializeField] private float curvaturaMaxima = 0.0016f;

    [Header("Cidade")]
    [SerializeField] private float velocidadeCidade = 9f;
    [SerializeField] private float duracaoCidade = 10f;

    [Header("Estacionamento")]
    [SerializeField] private float duracaoFreada = 3.5f;
    [SerializeField] private float pausaAntesDaUltimaFala = 1.6f;

    [Header("Falas roteirizadas")]
    [SerializeField]
    private FalaMarcada[] falas =
    {
        new FalaMarcada { tempo = 6f,  texto = "\"Três dias.\"", duracao = 2.5f },
        new FalaMarcada { tempo = 10f, texto = "\"Ele nunca ficou três dias sem atender.\"", duracao = 3.5f },
        new FalaMarcada { tempo = 15f, texto = "\"Talvez eu esteja exagerando.\"", duracao = 3f },
        new FalaMarcada { tempo = 20f, texto = "\"Eu devia ter vindo antes.\"", duracao = 3.5f },
    };

    [TextArea]
    [SerializeField] private string falaAoReconhecer = "\"Não mudou tanto...\"";

    [TextArea]
    [SerializeField] private string falaComplemento = "\"Pelo menos é o que eu lembrava.\"";

    [TextArea]
    [SerializeField] private string falaFinal = "\"Certo... vamos descobrir o que aconteceu.\"";

    private Image cortina;
    private float relogio;
    private int proximaFala;
    private bool encerrando;

    private void Awake()
    {
        MontarCortina();
    }

    private void Start()
    {
        StartCoroutine(Rodar());
    }

    // Uma cortina própria, além da do SceneFader: o documento pede três
    // segundos de tela preta com som antes da imagem, e o fade padrão de
    // entrada é curto demais para isso.
    private void MontarCortina()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform, "CortinaPrologo", 800);
        cortina = new GameObject("Preto").AddComponent<Image>();
        cortina.transform.SetParent(canvas.transform, false);
        cortina.color = Color.black;
        cortina.raycastTarget = false;
        UIFactory.Stretch(cortina.rectTransform);
    }

    private IEnumerator Rodar()
    {
        AudioAmbiente audio = AudioAmbiente.Instance;

        if (ponteiro != null)
        {
            ponteiro.Ativo = false;
        }

        // --- 0:00–0:03 — só som ---------------------------------------
        if (audio != null)
        {
            audio.Definir("chuva", 0.55f, 0.6f);
            audio.Definir("motor", 0.40f, 1.2f);
            audio.Definir("pneus", 0.30f, 1.5f);
        }

        if (estrada != null)
        {
            estrada.Velocidade = velocidadeEstrada;
        }

        yield return new WaitForSeconds(esperaNoEscuro);

        // --- 0:03–0:06 — a imagem aparece, controle já liberado --------
        yield return Revelar(duracaoAberturaImagem);

        if (ponteiro != null)
        {
            ponteiro.Ativo = true;
        }

        // --- estrada aberta -------------------------------------------
        yield return AteOTempo(tempoAtePlaca, audio);

        // A placa passa uma vez e some: é o nome da cidade chegando ao
        // jogador pelo mundo, não por um letreiro de UI.
        if (estrada != null)
        {
            estrada.Ligar("placa", true);
        }

        yield return AteOTempo(tempoAtePlaca + 4f, audio);
        Legenda.Dizer(falaAoReconhecer, 3f);

        yield return AteOTempo(tempoAteCurva, audio);

        if (estrada != null)
        {
            estrada.Ligar("placa", false);
        }

        // --- a curva: entra à direita ---------------------------------
        yield return Curvar(audio);

        Legenda.Dizer(falaComplemento, 3f);

        // --- cidade se aproximando ------------------------------------
        if (estrada != null)
        {
            estrada.Ligar("casa", true);
            estrada.Ligar("arvore", false);
        }

        if (audio != null)
        {
            // Sem estrada aberta, a chuva muda de textura: agora ela bate em
            // telhado. Mesma faixa, peso diferente.
            audio.Definir("chuva", 0.7f, 3f);
        }

        yield return Desacelerar(velocidadeCidade, 3f);
        yield return AteOTempo(relogio + duracaoCidade, audio);

        // --- estacionamento -------------------------------------------
        if (ponteiro != null)
        {
            ponteiro.Ativo = false;
        }

        Legenda.Limpar();

        yield return Desacelerar(0f, duracaoFreada);

        if (audio != null)
        {
            audio.Disparar("motorDesliga", 0.8f);
            audio.Definir("motor", 0f, 1.2f);
            audio.Definir("pneus", 0f, 1f);
            audio.Definir("radio", 0f, 1.5f);

            // Sem motor, a chuva parece mais alta. Esse vazio é o que prepara a
            // troca de perspectiva (§7).
            audio.Definir("chuva", 0.85f, 2f);
        }

        if (limpadores != null)
        {
            limpadores.Ativo = false;
        }

        yield return new WaitForSeconds(pausaAntesDaUltimaFala);

        Legenda.Dizer(falaFinal, 3f, Legenda.PrioridadeCritica);

        yield return new WaitForSeconds(3.4f);

        Encerrar();
    }

    // Deixa o relógio correr até o instante pedido, disparando as falas
    // roteirizadas que vencerem no caminho.
    private IEnumerator AteOTempo(float alvo, AudioAmbiente audio)
    {
        while (relogio < alvo)
        {
            if (encerrando)
            {
                yield break;
            }

            relogio += Time.deltaTime;
            DispararFalasVencidas();
            yield return null;
        }
    }

    private void DispararFalasVencidas()
    {
        while (proximaFala < falas.Length && relogio >= falas[proximaFala].tempo)
        {
            FalaMarcada f = falas[proximaFala];
            Legenda.Dizer(f.texto, f.duracao);
            proximaFala++;
        }
    }

    private IEnumerator Curvar(AudioAmbiente audio)
    {
        if (audio != null)
        {
            audio.Disparar("seta", 0.6f);
        }

        float t = 0f;
        float piscada = 0f;

        while (t < duracaoCurva)
        {
            t += Time.deltaTime;
            relogio += Time.deltaTime;
            DispararFalasVencidas();

            // Entra e sai da curva; no meio dela o desvio é máximo.
            float f = Mathf.Sin(t / duracaoCurva * Mathf.PI);

            if (estrada != null)
            {
                estrada.Curvatura = curvaturaMaxima * f;
                estrada.Velocidade = Mathf.Lerp(velocidadeEstrada, velocidadeCidade, f * 0.6f);
            }

            piscada += Time.deltaTime;

            if (piscada >= 0.5f && audio != null)
            {
                piscada = 0f;
                audio.Disparar("seta", 0.5f);
            }

            yield return null;
        }

        if (estrada != null)
        {
            estrada.Curvatura = 0f;
        }
    }

    private IEnumerator Desacelerar(float alvo, float tempo)
    {
        if (estrada == null)
        {
            yield break;
        }

        float inicio = estrada.Velocidade;
        float t = 0f;

        while (t < tempo)
        {
            t += Time.deltaTime;
            relogio += Time.deltaTime;
            DispararFalasVencidas();

            float f = t / tempo;
            estrada.Velocidade = Mathf.Lerp(inicio, alvo, f * f * (3f - 2f * f));

            // O motor cai de rotação junto com a velocidade.
            if (AudioAmbiente.Instance != null)
            {
                AudioAmbiente.Instance.Pitch(
                    "motor", Mathf.Lerp(1f, 0.72f, 1f - estrada.Velocidade / Mathf.Max(1f, velocidadeEstrada))
                );
            }

            yield return null;
        }

        estrada.Velocidade = alvo;
    }

    private IEnumerator Revelar(float tempo)
    {
        float t = 0f;

        while (t < tempo)
        {
            t += Time.deltaTime;
            cortina.color = new Color(0f, 0f, 0f, 1f - t / tempo);
            yield return null;
        }

        cortina.color = new Color(0f, 0f, 0f, 0f);
    }

    private void Update()
    {
        // Atalho de desenvolvimento: pular o prólogo sem passar pelo menu.
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
        StopAllCoroutines();
        Legenda.Limpar();

        GameState.Marcar(Flags.PrologoConcluido);
        SceneTransition.Ir(cenaSeguinte, entradaNaCena);
    }
}
