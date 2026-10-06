using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;

// A chuva da gameplay: visual e som.
//
// VISUAL. Nada de partícula borrada: tudo é desenhado pixel a pixel em duas
// texturas do tamanho exato da tela do jogo (640x360 a 32 PPU), presas à câmera.
//   - camada ALTA (na frente de tudo): os traços das gotas caindo.
//   - camada do CHÃO (logo acima do piso, atrás de casas, carro e player):
//     poças, respingos e ondinhas.
// Em vista de cima, o que vende a chuva é cada gota ter um ponto no chão: ela
// cai até ali e vira respingo — ou ondinha, se o ponto estiver dentro de uma poça.
//
// LUZ. A chuva usa material sem iluminação, então é ela mesma que decide o
// próprio brilho: quase invisível no escuro, e acesa onde há luz — sob os postes
// (lendo a textura da camada de luz pintada) e dentro do feixe da lanterna. É
// assim que chuva de noite aparece de verdade.
//
// SOM. O loop de chuva em duas fontes defasadas (disfarça a repetição), volume
// acompanhando as rajadas, saindo pelo grupo de efeitos do mixer. Com "abafado",
// passa por um filtro passa-baixa: chuva ouvida de dentro de casa.
[DefaultExecutionOrder(1000)] // desenha depois que a câmera se moveu no quadro
public class Chuva : MonoBehaviour
{
    [System.Serializable]
    public class Poca
    {
        public Vector2 centro;
        public Vector2 raio = new Vector2(1f, 0.35f);
    }

    [Header("Visual")]
    [SerializeField] private bool desenharChuva = true;

    [Tooltip("Sprite-Unlit-Default. Sem ele a luz global de 0,12 apagaria a chuva.")]
    [SerializeField] private Material material;

    [Tooltip("Gotas por segundo numa tela cheia (20 x 11,25 unidades).")]
    [SerializeField] private float gotasPorSegundo = 900f;

    [Range(0f, 1f)]
    [SerializeField] private float variacaoIntensidade = 0.35f;

    [Tooltip("Inclinação média da chuva, em graus a partir da vertical.")]
    [SerializeField] private float ventoGraus = 14f;

    [SerializeField] private float variacaoVento = 7f;

    [SerializeField] private Color corGota = new Color(0.72f, 0.8f, 0.9f, 1f);

    [Tooltip("Visibilidade da gota no escuro total.")]
    [Range(0f, 1f)]
    [SerializeField] private float alfaBase = 0.2f;

    [Tooltip("Quanto a luz (postes, lanterna) acende a chuva.")]
    [SerializeField] private float ganhoLuz = 0.85f;

    [Tooltip("Cor da água parada. Escura: poça de noite é um espelho do céu.")]
    [SerializeField] private Color corPoca = new Color(0.04f, 0.055f, 0.08f, 1f);

    [Range(0f, 1f)]
    [SerializeField] private float alfaPoca = 0.38f;

    [SerializeField] private List<Poca> pocas = new List<Poca>();

    [Header("Som")]
    [SerializeField] private AudioClip som;
    [SerializeField] private AudioMixerGroup grupoMixer;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.55f;

    [Tooltip("Chuva ouvida de dentro de um lugar fechado.")]
    [SerializeField] private bool abafado;

    [SerializeField] private float corteAbafado = 750f;

    // --- simulação --------------------------------------------------

    private struct Gota
    {
        public Vector2 alvo;      // onde ela bate no chão
        public float altura;      // quanto falta cair (unidades)
        public float velocidade;
        public float comprimento; // rastro, em unidades
        public float peso;        // 0,5 longe ... 1,4 perto da câmera
        public bool respinga;     // gotas "perto da câmera" passam direto
    }

    private struct Respingo
    {
        public Vector2 pos;
        public float t;
        public float duracao;
        public bool naPoca;
        public float alfa;
        public Color cor;
    }

    private const int MaxGotas = 2200;
    private const int MaxRespingos = 1400;
    private const float AlturaMaxima = 9f;

    private readonly List<Gota> gotas = new List<Gota>(MaxGotas);
    private readonly List<Respingo> respingos = new List<Respingo>(MaxRespingos);
    private float acumulado;
    private float intensidade = 1f;
    private float angulo;

    // --- desenho ----------------------------------------------------

    private class Camada
    {
        public Texture2D textura;
        public NativeArray<Color32> pixels;
        public SpriteRenderer render;
        public readonly List<int> sujos = new List<int>(32000);
    }

    private Camera cam;
    private int ppu = 32;
    private int largura, altura;
    private Camada alta, chao;
    private Vector2 centroTela;

    // --- luz --------------------------------------------------------

    private readonly List<Light2D> luzesPintadas = new List<Light2D>();
    private readonly List<Light2D> luzesPonto = new List<Light2D>();
    private float proximaBuscaLuz;

    // --- som --------------------------------------------------------

    private AudioSource fonteA, fonteB;

    // =================================================================

    private void Start()
    {
        cam = Camera.main;

        PixelPerfectCamera pixelPerfect = cam != null ? cam.GetComponent<PixelPerfectCamera>() : null;
        if (pixelPerfect != null)
        {
            ppu = pixelPerfect.assetsPPU;
        }

        MontarSom();
        BuscarLuzes();

        // aquece: a cena já abre chovendo, sem a primeira leva caindo em bloco
        if (desenharChuva && cam != null)
        {
            for (int i = 0; i < 40; i++)
            {
                Simular(1f / 30f);
            }
        }
    }

    private void OnDisable()
    {
        if (fonteA != null) fonteA.Stop();
        if (fonteB != null) fonteB.Stop();
    }

    private void OnEnable()
    {
        if (fonteA != null && som != null) fonteA.Play();
        if (fonteB != null && som != null) fonteB.Play();
    }

    private void OnDestroy()
    {
        LiberarCamada(alta);
        LiberarCamada(chao);
    }

    private void LateUpdate()
    {
        float dt = Time.deltaTime;
        AtualizarClima();
        AtualizarSom();

        if (!desenharChuva || cam == null)
        {
            return;
        }

        if (Time.unscaledTime >= proximaBuscaLuz)
        {
            BuscarLuzes();
        }

        GarantirCamadas();
        centroTela = cam.transform.position;

        Simular(dt);
        Desenhar();
    }

    // =================================================================
    // CLIMA
    // =================================================================

    private void AtualizarClima()
    {
        float t = Time.time;

        // intensidade: ondas lentas, com rajadas mais curtas por cima
        float lenta = Mathf.PerlinNoise(t * 0.045f, 3.1f) * 2f - 1f;
        float rajada = Mathf.Max(0f, Mathf.PerlinNoise(t * 0.18f, 7.7f) - 0.62f) * 2.6f;
        intensidade = Mathf.Clamp(1f + variacaoIntensidade * lenta + rajada * 0.35f, 0.35f, 1.6f);

        float vento = Mathf.PerlinNoise(t * 0.06f, 11.3f) * 2f - 1f;
        angulo = (ventoGraus + variacaoVento * vento + rajada * 6f) * Mathf.Deg2Rad;
    }

    // =================================================================
    // SIMULAÇÃO
    // =================================================================

    private Vector2 MetadeDaTela()
    {
        return new Vector2(largura * 0.5f / ppu, altura * 0.5f / ppu);
    }

    private void Simular(float dt)
    {
        if (largura == 0)
        {
            GarantirCamadas();
            centroTela = cam.transform.position;
        }

        Vector2 meia = MetadeDaTela();
        float tangente = Mathf.Tan(angulo);

        // alvos cobrem a tela e uma faixa abaixo dela: sem isso, a parte de baixo
        // da tela teria menos traços (a gota é vista acima do ponto onde cai)
        float xMin = centroTela.x - meia.x - Mathf.Max(0f, tangente) * AlturaMaxima - 1f;
        float xMax = centroTela.x + meia.x - Mathf.Min(0f, tangente) * AlturaMaxima + 1f;
        float yMin = centroTela.y - meia.y - AlturaMaxima;
        float yMax = centroTela.y + meia.y;

        float area = (xMax - xMin) * (yMax - yMin);
        acumulado += gotasPorSegundo * intensidade * (area / (20f * 11.25f)) * dt;

        while (acumulado >= 1f && gotas.Count < MaxGotas)
        {
            acumulado -= 1f;
            gotas.Add(NovaGota(xMin, xMax, yMin, yMax));
        }
        acumulado = Mathf.Min(acumulado, 50f);

        for (int i = gotas.Count - 1; i >= 0; i--)
        {
            Gota g = gotas[i];
            g.altura -= g.velocidade * dt;

            if (g.altura <= 0f)
            {
                if (g.respinga)
                {
                    NovoRespingo(g);
                }

                gotas[i] = gotas[gotas.Count - 1];
                gotas.RemoveAt(gotas.Count - 1);
                continue;
            }

            gotas[i] = g;
        }

        for (int i = respingos.Count - 1; i >= 0; i--)
        {
            Respingo r = respingos[i];
            r.t += dt;

            if (r.t >= r.duracao)
            {
                respingos[i] = respingos[respingos.Count - 1];
                respingos.RemoveAt(respingos.Count - 1);
                continue;
            }

            respingos[i] = r;
        }
    }

    private Gota NovaGota(float xMin, float xMax, float yMin, float yMax)
    {
        Gota g = new Gota
        {
            alvo = new Vector2(Random.Range(xMin, xMax), Random.Range(yMin, yMax)),
        };

        float sorteio = Random.value;

        if (sorteio < 0.07f)
        {
            // perto da câmera: rápida, longa, passa direto sem respingar
            g.peso = 1.4f;
            g.velocidade = Random.Range(34f, 42f);
            g.comprimento = Random.Range(0.8f, 1.15f);
            g.altura = Random.Range(5f, AlturaMaxima);
            g.respinga = false;
        }
        else if (sorteio < 0.45f)
        {
            g.peso = 0.5f; // longe
            g.velocidade = Random.Range(13f, 17f);
            g.comprimento = Random.Range(0.18f, 0.28f);
            g.altura = Random.Range(3f, 6.5f);
            g.respinga = true;
        }
        else
        {
            g.peso = Random.Range(0.75f, 1f);
            g.velocidade = Random.Range(18f, 27f);
            g.comprimento = Random.Range(0.3f, 0.52f);
            g.altura = Random.Range(4f, 8f);
            g.respinga = true;
        }

        return g;
    }

    private void NovoRespingo(Gota g)
    {
        if (respingos.Count >= MaxRespingos)
        {
            return;
        }

        // respingo fora da tela não é visto: nem gasta vaga
        Vector2 meia = MetadeDaTela();
        if (Mathf.Abs(g.alvo.x - centroTela.x) > meia.x + 0.2f ||
            Mathf.Abs(g.alvo.y - centroTela.y) > meia.y + 0.2f)
        {
            return;
        }

        bool naPoca = DentroDePoca(g.alvo);
        Color luz = BrilhoEm(g.alvo, out float forca);

        respingos.Add(new Respingo
        {
            pos = g.alvo,
            t = 0f,
            duracao = naPoca ? Random.Range(0.45f, 0.62f) : Random.Range(0.13f, 0.19f),
            naPoca = naPoca,
            // na poça escura a ondinha precisa de mais brilho para aparecer
            alfa = Mathf.Clamp01((alfaBase * (naPoca ? 1.8f : 1.3f) + ganhoLuz * forca) * g.peso),
            cor = Color.Lerp(corGota, luz, Mathf.Clamp01(forca) * 0.6f),
        });
    }

    private bool DentroDePoca(Vector2 p)
    {
        foreach (Poca poca in pocas)
        {
            Vector2 d = p - poca.centro;
            float nx = d.x / Mathf.Max(0.01f, poca.raio.x);
            float ny = d.y / Mathf.Max(0.01f, poca.raio.y);

            if (nx * nx + ny * ny <= 1f)
            {
                return true;
            }
        }

        return false;
    }

    // =================================================================
    // LUZ
    // =================================================================

    private void BuscarLuzes()
    {
        proximaBuscaLuz = Time.unscaledTime + 1f;
        luzesPintadas.Clear();
        luzesPonto.Clear();

        HashSet<Sprite> vistas = new HashSet<Sprite>();

        foreach (Light2D luz in FindObjectsByType<Light2D>(FindObjectsSortMode.None))
        {
            if (!luz.isActiveAndEnabled)
            {
                continue;
            }

            if (luz.lightType == Light2D.LightType.Sprite && luz.lightCookieSprite != null &&
                luz.lightCookieSprite.texture.isReadable)
            {
                // a camada pintada usa a mesma textura em duas passadas: conta uma vez
                if (vistas.Add(luz.lightCookieSprite))
                {
                    luzesPintadas.Add(luz);
                }
            }
            else if (luz.lightType == Light2D.LightType.Point)
            {
                luzesPonto.Add(luz);
            }
        }
    }

    // Quanta luz chega num ponto do mundo, e de que cor.
    private Color BrilhoEm(Vector2 p, out float forca)
    {
        Color soma = Color.black;
        forca = 0f;

        foreach (Light2D luz in luzesPintadas)
        {
            if (luz == null) continue;

            Sprite s = luz.lightCookieSprite;
            Vector3 escala = luz.transform.lossyScale;
            Vector2 tamanho = new Vector2(s.bounds.size.x * escala.x, s.bounds.size.y * escala.y);
            Vector2 origem = (Vector2)luz.transform.position - tamanho * 0.5f;

            float u = (p.x - origem.x) / tamanho.x;
            float v = (p.y - origem.y) / tamanho.y;

            if (u < 0f || u > 1f || v < 0f || v > 1f) continue;

            Color c = s.texture.GetPixelBilinear(u, v) * luz.intensity;
            soma += c;
            forca += c.r * 0.3f + c.g * 0.59f + c.b * 0.11f;
        }

        foreach (Light2D luz in luzesPonto)
        {
            if (luz == null) continue;

            Vector2 delta = p - (Vector2)luz.transform.position;
            float raio = luz.pointLightOuterRadius;
            float distancia = delta.magnitude;

            if (distancia >= raio) continue;

            float cone = 1f;
            float abertura = luz.pointLightOuterAngle * 0.5f;

            if (abertura < 179f && distancia > 0.01f)
            {
                float desvio = Vector2.Angle(luz.transform.up, delta);
                float miolo = luz.pointLightInnerAngle * 0.5f;
                if (desvio >= abertura) continue;
                cone = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(miolo, abertura, desvio));
            }

            float valor = Mathf.Pow(1f - distancia / raio, 1.5f) * cone * luz.intensity * 0.45f;
            soma += luz.color * valor;
            forca += valor;
        }

        forca = Mathf.Min(forca, 1.4f);
        float maior = Mathf.Max(soma.r, Mathf.Max(soma.g, soma.b));
        return maior > 0.001f ? new Color(soma.r / maior, soma.g / maior, soma.b / maior, 1f) : corGota;
    }

    // =================================================================
    // DESENHO
    // =================================================================

    private void GarantirCamadas()
    {
        float alturaMundo = cam.orthographicSize * 2f;
        int h = Mathf.RoundToInt(alturaMundo * ppu / 2f) * 2;
        int w = Mathf.RoundToInt(alturaMundo * cam.aspect * ppu / 2f) * 2;

        if (w == largura && h == altura && alta != null)
        {
            return;
        }

        LiberarCamada(alta);
        LiberarCamada(chao);

        largura = Mathf.Max(2, w);
        altura = Mathf.Max(2, h);

        alta = CriarCamada("ChuvaAlta", 900);
        chao = CriarCamada("ChuvaChao", -15); // acima do piso (-20/-19), abaixo de tudo mais
    }

    private Camada CriarCamada(string nome, int ordem)
    {
        Camada c = new Camada();

        c.textura = new Texture2D(largura, altura, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            name = nome,
        };
        c.pixels = c.textura.GetPixelData<Color32>(0);
        for (int i = 0; i < c.pixels.Length; i++) c.pixels[i] = default;
        c.textura.Apply(false, false);

        GameObject go = new GameObject(nome);
        go.transform.SetParent(cam.transform, false);
        go.transform.localPosition = new Vector3(0f, 0f, 10f);

        c.render = go.AddComponent<SpriteRenderer>();
        c.render.sprite = Sprite.Create(c.textura, new Rect(0, 0, largura, altura),
                                        new Vector2(0.5f, 0.5f), ppu, 0, SpriteMeshType.FullRect);
        c.render.sortingOrder = ordem;
        if (material != null) c.render.sharedMaterial = material;

        return c;
    }

    private static void LiberarCamada(Camada c)
    {
        if (c == null) return;
        if (c.render != null) Destroy(c.render.gameObject);
        if (c.textura != null) Destroy(c.textura);
    }

    private void Desenhar()
    {
        Limpar(alta);
        Limpar(chao);

        DesenharPocas();

        foreach (Respingo r in respingos)
        {
            if (r.naPoca) DesenharOndinha(r);
            else DesenharRespingo(r);
        }

        float tangente = Mathf.Tan(angulo);
        foreach (Gota g in gotas)
        {
            DesenharGota(g, tangente);
        }

        alta.textura.Apply(false, false);
        chao.textura.Apply(false, false);
    }

    private static void Limpar(Camada c)
    {
        foreach (int i in c.sujos) c.pixels[i] = default;
        c.sujos.Clear();
    }

    private bool ParaPixel(Vector2 mundo, out int x, out int y)
    {
        x = Mathf.FloorToInt((mundo.x - centroTela.x) * ppu + largura * 0.5f);
        y = Mathf.FloorToInt((mundo.y - centroTela.y) * ppu + altura * 0.5f);
        return x >= -8 && x < largura + 8 && y >= -8 && y < altura + 8;
    }

    // Mistura por transparência ("por cima"), não "fica o mais opaco": sem isso a
    // ondinha, mais transparente que a poça, sumia dentro dela.
    private void Pintar(Camada c, int x, int y, Color cor, float alfa)
    {
        if (x < 0 || y < 0 || x >= largura || y >= altura) return;

        alfa = Mathf.Clamp01(alfa);
        if (alfa <= 0.004f) return;

        int i = y * largura + x;
        Color32 atual = c.pixels[i];

        if (atual.a == 0)
        {
            c.sujos.Add(i);
            c.pixels[i] = new Color32((byte)(cor.r * 255f), (byte)(cor.g * 255f),
                                      (byte)(cor.b * 255f), (byte)(alfa * 255f));
            return;
        }

        float embaixo = atual.a / 255f * (1f - alfa);
        float total = alfa + embaixo;

        c.pixels[i] = new Color32(
            (byte)((cor.r * alfa + atual.r / 255f * embaixo) / total * 255f),
            (byte)((cor.g * alfa + atual.g / 255f * embaixo) / total * 255f),
            (byte)((cor.b * alfa + atual.b / 255f * embaixo) / total * 255f),
            (byte)(total * 255f));
    }

    private void DesenharGota(Gota g, float tangente)
    {
        Vector2 cabeca = g.alvo + new Vector2(tangente * g.altura, g.altura);
        Vector2 cauda = g.alvo + new Vector2(tangente * (g.altura + g.comprimento), g.altura + g.comprimento);

        if (!ParaPixel(cabeca, out int x0, out int y0) & !ParaPixel(cauda, out int x1, out int y1))
        {
            return;
        }

        Color luz = BrilhoEm(cabeca, out float forca);
        Color cor = Color.Lerp(corGota, luz, Mathf.Clamp01(forca) * 0.55f);
        float alfa = (alfaBase + ganhoLuz * forca) * g.peso;

        // Bresenham, com o rastro sumindo da cabeça para a cauda
        int dx = Mathf.Abs(x1 - x0), dy = -Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1;
        int erro = dx + dy;
        int passos = Mathf.Max(dx, -dy);
        int x = x0, y = y0;

        for (int n = 0; ; n++)
        {
            float k = passos == 0 ? 1f : 1f - 0.75f * n / passos;
            Pintar(alta, x, y, cor, alfa * k);

            if (x == x1 && y == y1) break;
            int e2 = 2 * erro;
            if (e2 >= dy) { erro += dy; x += sx; }
            if (e2 <= dx) { erro += dx; y += sy; }
        }
    }

    private void DesenharRespingo(Respingo r)
    {
        if (!ParaPixel(r.pos, out int x, out int y)) return;

        float fase = r.t / r.duracao;
        float a = r.alfa;

        if (fase < 0.33f)
        {
            Pintar(chao, x, y, r.cor, a);
            Pintar(chao, x, y + 1, r.cor, a * 0.6f);
        }
        else if (fase < 0.66f)
        {
            float b = a * 0.8f;
            Pintar(chao, x - 1, y, r.cor, b);
            Pintar(chao, x + 1, y, r.cor, b);
            Pintar(chao, x, y + 1, r.cor, b * 0.7f);
        }
        else
        {
            float b = a * 0.55f * (1f - (fase - 0.66f) / 0.34f);
            Pintar(chao, x - 2, y, r.cor, b);
            Pintar(chao, x + 2, y, r.cor, b);
            Pintar(chao, x - 1, y + 1, r.cor, b * 0.8f);
            Pintar(chao, x + 1, y + 1, r.cor, b * 0.8f);
        }
    }

    private void DesenharOndinha(Respingo r)
    {
        if (!ParaPixel(r.pos, out int cx, out int cy)) return;

        float fase = r.t / r.duracao;
        float rx = 1f + 5f * fase;
        float ry = Mathf.Max(0.6f, rx * 0.45f); // perspectiva de cima: achatada
        float a = r.alfa * Mathf.Pow(1f - fase, 1.3f);

        int passos = Mathf.Max(8, Mathf.RoundToInt(rx * 7f));
        for (int i = 0; i < passos; i++)
        {
            float ang = i * Mathf.PI * 2f / passos;
            int x = cx + Mathf.RoundToInt(Mathf.Cos(ang) * rx);
            int y = cy + Mathf.RoundToInt(Mathf.Sin(ang) * ry);
            Pintar(chao, x, y, r.cor, a);
        }
    }

    // Poça de noite é escura: um espelho do céu. Ela escurece o chão molhado, e
    // só acende onde reflete luz — a borda de cima e alguns pontos de brilho.
    private void DesenharPocas()
    {
        Vector2 meia = MetadeDaTela();

        foreach (Poca poca in pocas)
        {
            if (Mathf.Abs(poca.centro.x - centroTela.x) > meia.x + poca.raio.x + 1f ||
                Mathf.Abs(poca.centro.y - centroTela.y) > meia.y + poca.raio.y + 1f)
            {
                continue;
            }

            Color luz = BrilhoEm(poca.centro, out float forca);
            Color reflexo = Color.Lerp(corGota, luz, Mathf.Clamp01(forca) * 0.7f);
            float brilho = Mathf.Clamp01(forca);

            ParaPixel(poca.centro, out int cx, out int cy);
            int rx = Mathf.CeilToInt(poca.raio.x * ppu);
            int ry = Mathf.CeilToInt(poca.raio.y * ppu);

            for (int y = -ry; y <= ry; y++)
            {
                for (int x = -rx; x <= rx; x++)
                {
                    float nx = x / (float)rx, ny = y / (float)ry;
                    float d = nx * nx + ny * ny;
                    if (d > 1f) continue;

                    int px = cx + x, py = cy + y;
                    bool borda = d > 0.72f;

                    // borda pontilhada: a poça não termina numa linha dura
                    if (borda && ((px + py) & 1) == 1) continue;

                    Pintar(chao, px, py, corPoca, alfaPoca * (borda ? 0.55f : 1f));

                    // reflexo na borda de cima
                    if (ny > 0.5f && d > 0.5f)
                    {
                        Pintar(chao, px, py, reflexo, 0.06f + ganhoLuz * 0.5f * brilho);
                    }
                    // pontos de brilho soltos, fixos no mundo
                    else if (!borda && (((px * 73856093) ^ (py * 19349663)) & 31) == 0)
                    {
                        Pintar(chao, px, py, reflexo, 0.04f + ganhoLuz * 0.45f * brilho);
                    }
                }
            }
        }
    }

    // =================================================================
    // SOM
    // =================================================================

    private void MontarSom()
    {
        if (som == null) return;

        fonteA = CriarFonte("SomChuvaA", 1f);
        fonteB = CriarFonte("SomChuvaB", 0.97f);

        // segunda fonte começa no meio do loop e um pouco mais grave: as duas nunca
        // se alinham, e o ouvido não pega o ponto em que o arquivo recomeça
        fonteA.Play();
        fonteB.Play();
        fonteB.time = som.length * 0.5f;
    }

    private AudioSource CriarFonte(string nome, float pitch)
    {
        GameObject go = new GameObject(nome);
        go.transform.SetParent(transform, false);

        AudioSource fonte = go.AddComponent<AudioSource>();
        fonte.clip = som;
        fonte.loop = true;
        fonte.playOnAwake = false;
        fonte.spatialBlend = 0f;
        fonte.pitch = pitch;
        fonte.volume = 0f;
        fonte.outputAudioMixerGroup = grupoMixer;

        if (abafado)
        {
            AudioLowPassFilter filtro = go.AddComponent<AudioLowPassFilter>();
            filtro.cutoffFrequency = corteAbafado;
            filtro.lowpassResonanceQ = 1f;
        }

        return fonte;
    }

    private void AtualizarSom()
    {
        if (fonteA == null) return;

        float k = Mathf.Lerp(0.85f, 1.12f, Mathf.InverseLerp(0.35f, 1.6f, intensidade));
        float suave = 1f - Mathf.Exp(-2f * Time.unscaledDeltaTime);

        fonteA.volume = Mathf.Lerp(fonteA.volume, volume * 0.75f * k, suave);
        fonteB.volume = Mathf.Lerp(fonteB.volume, volume * 0.45f * k, suave);
    }
}
