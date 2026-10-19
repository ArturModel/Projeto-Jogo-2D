using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraFollow : MonoBehaviour
{
    [Header("Alvo")]
    [SerializeField] private Transform target;

    [Header("Enquadramento")]
    [SerializeField] private Vector3 offset =
        new Vector3(0f, 1.1f, -10f);

    [Header("Suavização")]
    [SerializeField, Range(0.01f, 0.5f)]
    private float smoothTime = 0.12f;

    [Header("Pixel Art")]
    [SerializeField] private int pixelsPerUnit = 32;

    [Header("Contenção pelas Bordas da Região (Regra R1)")]
    [SerializeField] private bool usarLimitesDaCena = true;
    [SerializeField] private Vector2 limiteMin = new Vector2(0f, 0f);
    [SerializeField] private Vector2 limiteMax = new Vector2(64f, 48f);

    private Vector3 velocidadeAtual;
    private Vector3 posicaoSuave;
    private Camera cam;
    private bool limitesDetectados;
    private bool limitesManualmenteDefinidos;

    public Vector2 LimiteMin => limiteMin;
    public Vector2 LimiteMax => limiteMax;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        GarantirFundoEscuroCamera();
        posicaoSuave = transform.position;
    }

    private void Start()
    {
        GarantirFundoEscuroCamera();

        if (target == null)
        {
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null) target = player.transform;
        }

        if (!limitesManualmenteDefinidos)
        {
            DetectarLimitesDaRegiao();
        }

        SnapImediatoParaAlvo();
    }

    private void GarantirFundoEscuroCamera()
    {
        if (cam == null) cam = GetComponent<Camera>();
        if (cam != null)
        {
            // Nunca exibe o azul padrão da Unity em interiores ou bordas de cena
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.03f, 0.03f, 0.04f, 1f);
        }
    }

    /// <summary>
    /// Define explicitamente o retângulo da Zona/Cena ou Interior atual (estilo Pokémon FireRed / OMORI / Corpse Party)
    /// e reposiciona a câmera instantaneamente sem deslizar pelo vazio.
    /// </summary>
    public void DefinirLimitesRegiao(Vector2 novoMin, Vector2 novoMax, float novoOrthoSize = -1f, bool snapImediato = true)
    {
        GarantirFundoEscuroCamera();

        usarLimitesDaCena = true;
        limitesDetectados = true;
        limitesManualmenteDefinidos = true;
        limiteMin = novoMin;
        limiteMax = novoMax;

        if (novoOrthoSize > 1f && cam != null && cam.orthographic)
        {
            cam.orthographicSize = novoOrthoSize;
        }

        if (snapImediato)
        {
            SnapImediatoParaAlvo();
        }
    }

    public void SnapImediatoParaAlvo()
    {
        if (target == null)
        {
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null) target = player.transform;
        }

        if (target != null)
        {
            velocidadeAtual = Vector3.zero;
            posicaoSuave = AplicarClampBordas(target.position + offset);
            float pixel = 1f / Mathf.Max(1, pixelsPerUnit);
            transform.position = new Vector3(
                Mathf.Round(posicaoSuave.x / pixel) * pixel,
                Mathf.Round(posicaoSuave.y / pixel) * pixel,
                -10f
            );
        }
    }

    private void DetectarLimitesDaRegiao()
    {
        if (limitesManualmenteDefinidos || limitesDetectados || !usarLimitesDaCena) return;
        limitesDetectados = true;

        // 1. Verifica se é uma cena de Interior com "Paredes" ou "Chao" SpriteRenderer (ex.: CasaDoAvo.unity, BarDoZe.unity)
        GameObject goParedes = GameObject.Find("Paredes");
        GameObject goChao = GameObject.Find("Chao");
        if (goParedes != null)
        {
            BoxCollider2D[] colsParedes = goParedes.GetComponents<BoxCollider2D>();
            if (colsParedes != null && colsParedes.Length >= 4)
            {
                float minX = float.MaxValue, minY = float.MaxValue;
                float maxX = float.MinValue, maxY = float.MinValue;
                for (int i = 0; i < colsParedes.Length; i++)
                {
                    Bounds b = colsParedes[i].bounds;
                    if (b.min.x < minX) minX = b.min.x;
                    if (b.min.y < minY) minY = b.min.y;
                    if (b.max.x > maxX) maxX = b.max.x;
                    if (b.max.y > maxY) maxY = b.max.y;
                }
                limiteMin = new Vector2(minX, minY);
                limiteMax = new Vector2(maxX, maxY);
                if (cam != null && cam.orthographic && cam.orthographicSize > 5.8f)
                {
                    cam.orthographicSize = 5.4f;
                }
                return;
            }
        }

        if (goChao != null && goChao.GetComponent<SpriteRenderer>() != null)
        {
            Bounds b = goChao.GetComponent<SpriteRenderer>().bounds;
            GameObject goParede = GameObject.Find("Parede");
            if (goParede != null && goParede.GetComponent<SpriteRenderer>() != null)
            {
                b.Encapsulate(goParede.GetComponent<SpriteRenderer>().bounds);
            }
            if (b.size.x > 4f && b.size.y > 4f)
            {
                limiteMin = new Vector2(b.min.x, b.min.y);
                limiteMax = new Vector2(b.max.x, b.max.y);
                if (cam != null && cam.orthographic && cam.orthographicSize > 5.8f)
                {
                    cam.orthographicSize = 5.4f;
                }
                return;
            }
        }

        // 2. Verifica se existe o objeto "Limites" com 4 BoxCollider2D
        GameObject goLimites = GameObject.Find("Limites");
        if (goLimites != null && goLimites.activeInHierarchy)
        {
            BoxCollider2D[] cols = goLimites.GetComponents<BoxCollider2D>();
            if (cols != null && cols.Length >= 4)
            {
                float minX = float.MaxValue, minY = float.MaxValue;
                float maxX = float.MinValue, maxY = float.MinValue;
                for (int i = 0; i < cols.Length; i++)
                {
                    Vector2 c = (Vector2)goLimites.transform.position + cols[i].offset;
                    if (c.x < minX) minX = c.x;
                    if (c.y < minY) minY = c.y;
                    if (c.x > maxX) maxX = c.x;
                    if (c.y > maxY) maxY = c.y;
                }
                limiteMin = new Vector2(minX + 1f, minY + 1f);
                limiteMax = new Vector2(maxX - 1f, maxY - 1f);
                return;
            }
        }

        // 3. Fallback por Tilemaps ativos da cena
        var tilemaps = FindObjectsByType<Tilemap>();
        if (tilemaps != null && tilemaps.Length > 0)
        {
            Bounds total = new Bounds();
            bool temAlgum = false;
            for (int i = 0; i < tilemaps.Length; i++)
            {
                if (!tilemaps[i].gameObject.activeInHierarchy) continue;
                var tr = tilemaps[i].GetComponent<TilemapRenderer>();
                if (tr != null && !tr.enabled) continue;

                tilemaps[i].CompressBounds();
                Bounds b = tilemaps[i].localBounds;
                if (b.size.sqrMagnitude < 0.1f) continue;
                Vector3 wMin = tilemaps[i].transform.TransformPoint(b.min);
                Vector3 wMax = tilemaps[i].transform.TransformPoint(b.max);
                Bounds wb = new Bounds();
                wb.SetMinMax(wMin, wMax);
                if (!temAlgum) { total = wb; temAlgum = true; }
                else total.Encapsulate(wb);
            }

            if (temAlgum && total.size.x > 4f && total.size.y > 4f)
            {
                limiteMin = new Vector2(total.min.x, total.min.y);
                limiteMax = new Vector2(total.max.x, total.max.y);
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            var player = FindAnyObjectByType<PlayerController>();
            if (player != null) target = player.transform;
            if (target == null) return;
        }

        if (cam == null)
        {
            cam = GetComponent<Camera>();
            GarantirFundoEscuroCamera();
        }

        Vector3 posicaoDesejada = AplicarClampBordas(target.position + offset);

        posicaoSuave = Vector3.SmoothDamp(
            posicaoSuave,
            posicaoDesejada,
            ref velocidadeAtual,
            smoothTime
        );

        posicaoSuave = AplicarClampBordas(posicaoSuave);

        float meioPixel = 0.5f / Mathf.Max(1, pixelsPerUnit);

        if (Vector2.Distance(posicaoSuave, posicaoDesejada) < meioPixel)
        {
            posicaoSuave.x = posicaoDesejada.x;
            posicaoSuave.y = posicaoDesejada.y;
            velocidadeAtual = Vector3.zero;
        }

        float pixel = 1f / Mathf.Max(1, pixelsPerUnit);

        transform.position = new Vector3(
            Mathf.Round(posicaoSuave.x / pixel) * pixel,
            Mathf.Round(posicaoSuave.y / pixel) * pixel,
            -10f
        );
    }

    private Vector3 AplicarClampBordas(Vector3 pos)
    {
        if (!usarLimitesDaCena) return pos;

        float halfHeight = (cam != null && cam.orthographic) ? cam.orthographicSize : 5.4f;
        float aspect = (cam != null && cam.aspect > 0.1f) ? cam.aspect : (16f / 9f);
        float halfWidth = halfHeight * aspect;

        float mapW = limiteMax.x - limiteMin.x;
        float mapH = limiteMax.y - limiteMin.y;

        if (mapW > halfWidth * 2f)
            pos.x = Mathf.Clamp(pos.x, limiteMin.x + halfWidth, limiteMax.x - halfWidth);
        else
            pos.x = (limiteMin.x + limiteMax.x) * 0.5f;

        if (mapH > halfHeight * 2f)
            pos.y = Mathf.Clamp(pos.y, limiteMin.y + halfHeight, limiteMax.y - halfHeight);
        else
            pos.y = (limiteMin.y + limiteMax.y) * 0.5f;

        return pos;
    }
}
