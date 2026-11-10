using UnityEngine;

internal sealed class FontePracaAnimada : MonoBehaviour
{
    private const int Celula = 128;
    private const int Colunas = 4;
    private const int Linhas = 2;
    private const int QuadrosPorSegundo = 10;
    private const float Alcance = 6f;

    private SpriteRenderer alvo;
    private Transform ouvinte;
    private Sprite[] quadros;
    private Sprite original;
    private SpriteRenderer sombraContato;
    private Sprite spriteSombra;
    private Texture2D texturaSombra;
    private AudioSource audioLocal;
    private AudioClip agua;
    private float tempo;
    private int quadro;
    private bool pausado;

    internal static void Instalar(SpriteRenderer renderer, Transform marcus)
    {
        if (renderer == null || renderer.GetComponent<FontePracaAnimada>() != null) return;
        var animacao = renderer.gameObject.AddComponent<FontePracaAnimada>();
        animacao.Iniciar(renderer, marcus);
    }

    private void Iniciar(SpriteRenderer renderer, Transform marcus)
    {
        alvo = renderer;
        ouvinte = marcus;
        original = renderer.sprite;
        CriarSombraContato(renderer);
        var atlas = Resources.Load<Texture2D>("Art/Props/FontePracaQuadros");
        if (original != null && atlas != null && atlas.width == Celula * Colunas && atlas.height == Celula * Linhas)
        {
            Rect recorte = original.rect;
            Vector2 pivo = original.pivot / recorte.size;
            quadros = new Sprite[Colunas * Linhas];
            for (int i = 0; i < quadros.Length; i++)
            {
                // O PNG começa pela linha superior, e o Sprite usa coordenadas a partir do rodapé.
                Vector2 deslocamento = new(i % Colunas * Celula, (Linhas - 1 - i / Colunas) * Celula);
                quadros[i] = Sprite.Create(atlas, new Rect(recorte.position + deslocamento, recorte.size),
                    pivo, original.pixelsPerUnit, 0, SpriteMeshType.FullRect);
            }

            // O material do Centro mantém a textura no bloco junto do receptor da neblina.
            alvo.sprite = quadros[0];
            DefinirTextura(atlas);
        }
        else
        {
            Debug.LogWarning("Fonte da praça: folha de animação ausente ou com tamanho inválido.", this);
        }

        audioLocal = gameObject.AddComponent<AudioSource>();
        agua = SantaRitaBarAudio.CriarTextura(1);
        audioLocal.clip = agua;
        audioLocal.loop = true;
        audioLocal.playOnAwake = false;
        audioLocal.volume = 0f;
    }

    private void Update()
    {
        bool pausa = Time.timeScale == 0 || MenuPausa.Aberto || !Application.isFocused;
        if (pausa != pausado)
        {
            if (pausa) audioLocal.Pause();
            else audioLocal.UnPause();
            pausado = pausa;
        }
        if (pausa) return;

        if (quadros != null)
        {
            tempo = Mathf.Repeat(tempo + Time.deltaTime * QuadrosPorSegundo, quadros.Length);
            int proximo = Mathf.FloorToInt(tempo);
            if (quadro != proximo)
            {
                quadro = proximo;
                alvo.sprite = quadros[quadro];
            }
        }

        Vector2 deslocamento = ouvinte != null
            ? transform.position - ouvinte.position
            : Vector2.right * Alcance;
        float proximidade = 1f - Mathf.SmoothStep(0f, 1f, deslocamento.magnitude / Alcance);
        float volume = .65f * proximidade * AudioSettingsController.VolumeEfeitos;
        if (SinucaMinigameUI.IsShowing || BochaMinigameUI.IsShowing) volume = 0f;
        if (GameMessageUI.IsShowing) volume *= .4f;
        audioLocal.volume = Mathf.MoveTowards(audioLocal.volume, volume, Time.unscaledDeltaTime * .2f);
        audioLocal.panStereo = Mathf.Clamp(deslocamento.x / Alcance, -.65f, .65f);
        if (volume > 0 && !audioLocal.isPlaying) audioLocal.Play();
        if (volume == 0 && audioLocal.volume == 0 && audioLocal.isPlaying) audioLocal.Stop();
    }

    private void DefinirTextura(Texture2D textura)
    {
        var bloco = new MaterialPropertyBlock();
        alvo.GetPropertyBlock(bloco);
        bloco.SetTexture("_MainTex", textura);
        alvo.SetPropertyBlock(bloco);
    }

    private void CriarSombraContato(SpriteRenderer renderer)
    {
        const int largura = 128;
        const int altura = 32;
        texturaSombra = new Texture2D(largura, altura, TextureFormat.RGBA32, false, true)
        {
            name = "Sombra de contato · fonte da praça",
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };
        var pixels = new Color32[largura * altura];
        for (int y = 0; y < altura; y++)
        for (int x = 0; x < largura; x++)
        {
            float nx = (x + .5f) / largura * 2f - 1f;
            float ny = (y + .5f) / altura * 2f - 1f;
            float distancia = nx * nx * .86f + ny * ny;
            float alpha = .80f * (1f - Mathf.SmoothStep(.22f, 1f, distancia));
            alpha = Mathf.Round(alpha * 5f) / 5f;
            pixels[y * largura + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(alpha * 255f));
        }
        texturaSombra.SetPixels32(pixels);
        texturaSombra.Apply(false, true);
        spriteSombra = Sprite.Create(texturaSombra, new Rect(0, 0, largura, altura),
            new Vector2(.5f, .5f), 32f, 0, SpriteMeshType.FullRect);
        spriteSombra.name = "Sombra de contato · fonte da praça";
        spriteSombra.hideFlags = HideFlags.HideAndDontSave;

        var objeto = new GameObject("Sombra de contato da fonte", typeof(SpriteRenderer));
        sombraContato = objeto.GetComponent<SpriteRenderer>();
        sombraContato.sprite = spriteSombra;
        sombraContato.color = new Color(.12f, .17f, .23f, .42f);
        sombraContato.sortingLayerID = renderer.sortingLayerID;
        sombraContato.sortingOrder = renderer.sortingOrder - 1;
        objeto.transform.position = new Vector3(renderer.bounds.center.x,
            renderer.bounds.min.y + .12f, renderer.transform.position.z + .01f);
    }

    private void OnDestroy()
    {
        if (quadros != null)
        {
            if (alvo != null && original != null)
            {
                alvo.sprite = original;
                DefinirTextura(original.texture);
            }
            foreach (var frame in quadros) Destroy(frame);
        }
        if (sombraContato != null) Destroy(sombraContato.gameObject);
        if (spriteSombra != null) Destroy(spriteSombra);
        if (texturaSombra != null) Destroy(texturaSombra);
        if (agua != null) Destroy(agua);
    }
}
