using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Peças compartilhadas da UI criada em runtime: canvas com escala de pixel art,
// a fonte do menu e os sprites das caixas.
public static class UIFactory
{
    // Mesma resolução de referência da PixelPerfectCamera, para a UI crescer
    // junto com o jogo em vez de ficar minúscula em telas grandes.
    public static readonly Vector2 ReferenceResolution = new Vector2(640f, 360f);

    private const string FontPath = "Fonts/VT323-Regular Bitmap";

    private static TMP_FontAsset cachedFont;

    public static TMP_FontAsset Font
    {
        get
        {
            if (cachedFont == null)
            {
                cachedFont = Resources.Load<TMP_FontAsset>(FontPath);
            }

            return cachedFont;
        }
    }

    public static Sprite LoadSprite(string nome)
    {
        return Resources.Load<Sprite>("UI/" + nome);
    }

    public static Canvas CreateCanvas(Transform parent, string nome, int sortingOrder)
    {
        GameObject go = new GameObject(nome);
        go.transform.SetParent(parent, false);

        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = ReferenceResolution;
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1f;

        return canvas;
    }

    public static Image CreateBox(Transform parent, string nome, string spriteName)
    {
        GameObject go = new GameObject(nome);
        go.transform.SetParent(parent, false);

        Image image = go.AddComponent<Image>();
        image.sprite = LoadSprite(spriteName);
        image.type = Image.Type.Sliced;
        image.raycastTarget = false;

        return image;
    }

    public static TextMeshProUGUI CreateLabel(Transform parent, string nome, float fontSize)
    {
        GameObject go = new GameObject(nome);
        go.transform.SetParent(parent, false);

        TextMeshProUGUI label = go.AddComponent<TextMeshProUGUI>();

        if (Font != null)
        {
            label.font = Font;
        }

        label.fontSize = fontSize;
        label.color = new Color(0.90f, 0.88f, 0.82f, 1f);
        label.raycastTarget = false;

        return label;
    }

    private static Sprite weatheredSurface;

    // Small painted surface shared by HUD and menu. The border keeps its pixel width when sliced.
    public static void ApplyWeatheredSurface(Image image, float opacity)
    {
        if (weatheredSurface == null)
        {
            const int width = 96, height = 64;
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
                { name = "MetalPintadoUI", filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
            var pixels = new Color32[width * height];
            var blue = new Color32(24, 35, 36, 255);
            var brown = new Color32(45, 42, 35, 255);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int edgeX = Mathf.Min(x, width - 1 - x);
                    int edgeY = Mathf.Min(y, height - 1 - y);
                    int edge = Mathf.Min(edgeX, edgeY);
                    if (edgeX + edgeY < 4) continue;
                    uint grain = unchecked((uint)(x * 92837111 ^ y * 689287499));
                    grain ^= grain >> 13;
                    float wear = .32f * Mathf.PerlinNoise(x * .047f, y * .19f) + .08f * (grain & 7) / 7f;
                    float brush = y % 11 == 7 ? .022f : 0;
                    Color color = Color32.Lerp(blue, brown, wear + .24f * y / height + brush);
                    if (edge == 0 || edgeX + edgeY == 4) color = new Color32(9, 16, 17, 255);
                    else if (edge == 1) color = Color.Lerp(new Color32(49, 44, 34, 255),
                        new Color32(111, 89, 57, 255), .25f + .62f * y / height);
                    else if (edge == 2) color = Color.Lerp(color, new Color32(109, 98, 73, 255), .29f);
                    pixels[y * width + x] = color;
                }
            }
            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            weatheredSurface = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(.5f, .5f),
                100, 0, SpriteMeshType.FullRect, new Vector4(5, 5, 5, 5));
        }
        image.sprite = weatheredSurface;
        image.type = Image.Type.Sliced;
        image.color = new Color(1, 1, 1, opacity);
    }

    public static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
