using System;
using UnityEngine;
using UnityEngine.UI;

// A deliberate collection/hand-over action on an existing inspection; E still cancels/closes.
public sealed class SantaRitaItemAction : MonoBehaviour
{
    public static void Oferecer(string text, Action action)
    {
        foreach (var old in FindObjectsByType<SantaRitaItemAction>(FindObjectsSortMode.None)) Destroy(old.gameObject);
        var canvas=UIFactory.CreateCanvas(null,"AcaoDoObjeto",9100);
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        var host=canvas.gameObject.AddComponent<SantaRitaItemAction>();
        var image=UIFactory.CreateBox(canvas.transform,"Acao",null);
        image.color=new Color(.15f,.23f,.18f,.98f);image.raycastTarget=true;
        var rect=image.rectTransform;rect.anchorMin=rect.anchorMax=new Vector2(.5f,0);rect.anchoredPosition=new Vector2(0,48);rect.sizeDelta=new Vector2(170,26);
        var button=image.gameObject.AddComponent<Button>();button.targetGraphic=image;
        var label=UIFactory.CreateLabel(image.transform,"Texto",15);label.text=text;label.alignment=TMPro.TextAlignmentOptions.Center;UIFactory.Stretch(label.rectTransform);
        bool used=false;
        button.onClick.AddListener(()=>{
            if(used||!GameMessageUI.IsShowing||MenuPausa.Aberto)return;
            used=true;button.interactable=false;
            action();GameMessageUI.Instance.Avancar();Destroy(host.gameObject);
        });
    }
    private void LateUpdate(){if(!GameMessageUI.IsShowing)Destroy(gameObject);}
}
