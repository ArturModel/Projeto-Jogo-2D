using System.Collections.Generic;
using UnityEngine;

// A sensação de estar dirigindo.
//
// Não há física de direção nem cenário 3D: o que existe é um punhado de
// sprites nascendo no horizonte e vindo em direção ao jogador, crescendo em
// perspectiva. É o truque clássico de pseudo-3D, e é o suficiente porque a
// câmera nunca sai de dentro do carro.
//
// A curva de entrada em Santa Rita é o mesmo truque: deslocar lateralmente os
// objetos em função da distância faz a estrada inteira parecer virar.
public class EstradaScroller : MonoBehaviour
{
    [System.Serializable]
    public class Fileira
    {
        public string nome = "fileira";
        public Sprite sprite;

        [Tooltip("Distância lateral do centro da pista, em metros.")]
        public float lateral = 6f;

        [Tooltip("Espaço entre um objeto e o próximo, em metros.")]
        public float espacamento = 12f;

        [Tooltip("Alterna esquerda/direita a cada objeto.")]
        public bool doisLados;

        [Tooltip("Multiplica o tamanho na tela.")]
        public float escala = 1f;

        [Tooltip("Desligada até o diretor ligar (ex: casas, só na cidade).")]
        public bool comecaDesligada;

        [System.NonSerialized] public List<Transform> itens = new List<Transform>();
        [System.NonSerialized] public List<SpriteRenderer> renders = new List<SpriteRenderer>();
        [System.NonSerialized] public List<float> profundidades = new List<float>();
        [System.NonSerialized] public List<float> lados = new List<float>();
        [System.NonSerialized] public bool ligada;
    }

    [Header("Perspectiva")]
    [Tooltip("Distância focal. Maior = perspectiva mais fechada.")]
    [SerializeField] private float foco = 5.2f;

    [Tooltip("Altura dos olhos acima do asfalto, em metros.")]
    [SerializeField] private float alturaCamera = 1.15f;

    [Tooltip("Objeto some antes disto (metros). Evita ver o sprite gigante.")]
    [SerializeField] private float distanciaMinima = 1.6f;

    [Tooltip("Objeto nasce aqui (metros).")]
    [SerializeField] private float distanciaMaxima = 90f;

    [Tooltip("Distância em que o objeto termina de aparecer.")]
    [SerializeField] private float distanciaFade = 62f;

    [Header("Conteúdo")]
    [SerializeField] private Fileira[] fileiras;

    [Header("Estado (o diretor controla)")]
    [SerializeField] private float velocidade = 14f;

    [SerializeField] private float curvatura;

    public float Velocidade
    {
        get => velocidade;
        set => velocidade = value;
    }

    // Positivo curva para a direita. O diretor anima isto na entrada da cidade.
    public float Curvatura
    {
        get => curvatura;
        set => curvatura = value;
    }

    private void Start()
    {
        foreach (Fileira f in fileiras)
        {
            Construir(f);
        }
    }

    public void Ligar(string nome, bool ligada)
    {
        foreach (Fileira f in fileiras)
        {
            if (f.nome != nome)
            {
                continue;
            }

            f.ligada = ligada;

            foreach (Transform t in f.itens)
            {
                t.gameObject.SetActive(ligada);
            }
        }
    }

    private void Construir(Fileira f)
    {
        f.ligada = !f.comecaDesligada;

        if (f.sprite == null || f.espacamento <= 0.01f)
        {
            return;
        }

        int quantidade = Mathf.Clamp(
            Mathf.CeilToInt((distanciaMaxima - distanciaMinima) / f.espacamento) + 1,
            1, 64
        );

        for (int i = 0; i < quantidade; i++)
        {
            GameObject go = new GameObject($"{f.nome}_{i}");
            go.transform.SetParent(transform, false);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = f.sprite;
            sr.sortingOrder = 0;

            f.itens.Add(go.transform);
            f.renders.Add(sr);
            f.profundidades.Add(distanciaMinima + i * f.espacamento);
            f.lados.Add(f.doisLados && i % 2 == 1 ? -1f : 1f);

            go.SetActive(f.ligada);
        }
    }

    private void Update()
    {
        float avanco = velocidade * Time.deltaTime;

        foreach (Fileira f in fileiras)
        {
            if (!f.ligada)
            {
                continue;
            }

            float alcance = f.espacamento * f.itens.Count;

            for (int i = 0; i < f.itens.Count; i++)
            {
                float z = f.profundidades[i] - avanco;

                // Passou pelo carro: volta para o fim da fila.
                while (z < distanciaMinima)
                {
                    z += alcance;
                    f.lados[i] = f.doisLados ? -f.lados[i] : f.lados[i];
                }

                f.profundidades[i] = z;
                Posicionar(f, i, z);
            }
        }
    }

    private void Posicionar(Fileira f, int i, float z)
    {
        // Projeção em perspectiva: tudo escala com 1/z.
        float k = foco / z;

        float lateral = f.lateral * f.lados[i];
        lateral += curvatura * z * z;   // a estrada inteira desviando

        float x = lateral * k;
        float y = -alturaCamera * k;

        Transform t = f.itens[i];
        t.localPosition = new Vector3(x, y, 0f);
        t.localScale = Vector3.one * (k * f.escala);

        // Aparece devagar em vez de brotar do nada no horizonte.
        float alfa = Mathf.InverseLerp(distanciaMaxima, distanciaFade, z);
        SpriteRenderer sr = f.renders[i];
        Color c = sr.color;
        c.a = Mathf.Clamp01(alfa);
        sr.color = c;
    }
}
