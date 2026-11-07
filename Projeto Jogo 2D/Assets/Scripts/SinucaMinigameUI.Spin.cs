using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public partial class SinucaMinigameUI
{
    private bool ajustandoEfeito;
    private GameObject painelEfeito;
    private SinucaSpinSelector previewEfeito;
    private TextMeshProUGUI txtAnguloEfeito;

    private void ConstruirAjusteEfeito()
    {
        painelEfeito=CriarPainelHUD(raiz.transform,"AjusteEfeito",new Vector2(206,-65),new Vector2(208,138));
        var rect=painelEfeito.GetComponent<RectTransform>();
        rect.anchorMin=rect.anchorMax=new Vector2(1,0);
        rect.anchoredPosition=new Vector2(-116,110);
        painelEfeito.GetComponent<Image>().color=new Color32(22,24,21,235);
        var titulo=CriarTexto("TituloEfeito",painelEfeito.transform,"EFEITO NA BRANCA",10,new Color32(242,220,170,255),TextAlignmentOptions.Center);
        Posicionar(titulo.rectTransform,new Vector2(0,54),new Vector2(196,16));
        var graphic=new GameObject("BolaContatoETaco",typeof(RectTransform),typeof(CanvasRenderer),typeof(SinucaSpinSelector));
        graphic.transform.SetParent(painelEfeito.transform,false);
        Posicionar(graphic.GetComponent<RectTransform>(),new Vector2(-32,4),new Vector2(78,78));
        previewEfeito=graphic.GetComponent<SinucaSpinSelector>();
        previewEfeito.ShowCue=true;
        previewEfeito.raycastTarget=false;
        txtAnguloEfeito=CriarTexto("AnguloEfeito",painelEfeito.transform,"",8.5f,new Color32(224,234,219,255),TextAlignmentOptions.Center);
        Posicionar(txtAnguloEfeito.rectTransform,new Vector2(0,-48),new Vector2(198,28));
        foreach(var g in painelEfeito.GetComponentsInChildren<Graphic>())g.raycastTarget=false;
        painelEfeito.SetActive(false);
    }

    private void IniciarAjusteEfeito()
    {
        if(estado!=EstadoPartida.MirandoJogador || EmPe || interfaceLivre || !tableView3D.Room.SettledAtCue)return;
        ajustandoEfeito=true;
        miraMouseAtiva=referenciaMiraMouse=false;
        tableView3D.ReiniciarReferenciaPonteiro();
        tableView3D.Room.SetSpinFocus(true);
        painelEfeito.SetActive(true);
        AtualizarIndicadorEfeito();
    }

    private void ArrastarEfeito(Vector2 pixels)
    {
        float scale=Mathf.Max(.01f,mesaRect.GetComponentInParent<Canvas>().scaleFactor);
        AplicarContatoEfeito(contatoTaco+pixels/(scale*110f));
    }

    private void AplicarContatoEfeito(Vector2 contato)
    {
        if(estado!=EstadoPartida.MirandoJogador)return;
        contatoTaco=Vector2.ClampMagnitude(contato,.75f);
        efeitoEscolhido=contatoTaco.y>.15f?SinucaCueSpin.Avancar:contatoTaco.y<-.15f?SinucaCueSpin.Recuar:SinucaCueSpin.Normal;
        if(spinSelector!=null)spinSelector.SetContact(contatoTaco);
        if(txtTendenciaEfeito!=null)txtTendenciaEfeito.text="Contato: "+(contatoTaco.x<-.15f?"esquerda / ":contatoTaco.x>.15f?"direita / ":"")+(contatoTaco.y<-.15f?"recuo":contatoTaco.y>.15f?"avanço":"centro");
        AtualizarIndicadorEfeito();
        AtualizarVisualTacoEMira();
    }

    private void AtualizarIndicadorEfeito()
    {
        if(previewEfeito==null)return;
        previewEfeito.SetContact(contatoTaco);
        txtAnguloEfeito.text=$"Lateral {contatoTaco.x*18f:+0.0;-0.0;0}° · Altura {Mathf.Abs(contatoTaco.y)*14f:0.0}°\nSolte o direito para manter";
    }

    private void EncerrarAjusteEfeito()
    {
        if(!ajustandoEfeito)return;
        ajustandoEfeito=false;
        if(painelEfeito!=null)painelEfeito.SetActive(false);
        if(tableView3D!=null){tableView3D.Room?.SetSpinFocus(false);tableView3D.ReiniciarReferenciaPonteiro();}
    }

    private void AtualizarAjusteEfeito()
    {
        if(!ajustandoEfeito)return;
        if(estado!=EstadoPartida.MirandoJogador || EmPe || interfaceLivre || !Application.isFocused || Mouse.current==null || !Mouse.current.rightButton.isPressed)
        {EncerrarAjusteEfeito();return;}
        // With the cursor released, sample only here; captured input is sampled in LerMouseCapturado.
        if(Cursor.lockState!=CursorLockMode.Locked)ArrastarEfeito(Mouse.current.delta.ReadValue());
    }
}
