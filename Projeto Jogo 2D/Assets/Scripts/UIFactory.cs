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

    public static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
