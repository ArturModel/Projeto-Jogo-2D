using UnityEngine;

internal sealed partial class Capitulo2Exterior
{
    // Reconstitui somente os vínculos de renderização. A geometria, as luzes 2D e
    // as colisões continuam nos objetos editáveis da cena.
    private void VincularEfeitosDoCentro(CentroEfeitosCena dados)
    {
        oclusoresMin.AddRange(dados.oclusoresMin);
        oclusoresMax.AddRange(dados.oclusoresMax);
        fontes.AddRange(dados.fontes);
        indiceLuzEntradaMatriz = dados.indiceLuzIgreja;
        foreach (var luz in dados.luzes)
            luzesVolumetricasPraca.Add((luz.posicao, luz.raio, luz.cor));

        var bloco = new MaterialPropertyBlock();
        foreach (var receptor in dados.receptores)
        {
            var sprite = receptor.sprite;
            if (sprite == null || sprite.sprite == null) continue;
            sprite.sharedMaterial = materialDoPiso;
            sprite.GetPropertyBlock(bloco);
            bloco.SetTexture("_MainTex", sprite.sprite.texture);
            Vector3 delta = sprite.transform.position - receptor.posicaoOriginal;
            if (receptor.temReceiver)
            {
                var receiver = receptor.receiver;
                receiver.x += delta.y;
                bloco.SetVector("_Receiver", receiver);
                int dono = Mathf.RoundToInt(receiver.z) - 1;
                if (dono >= 0 && dono < oclusoresMin.Count)
                {
                    var min = oclusoresMin[dono];
                    var max = oclusoresMax[dono];
                    min.x += delta.x; min.y += delta.y;
                    max.x += delta.x; max.y += delta.y;
                    oclusoresMin[dono] = min;
                    oclusoresMax[dono] = max;
                }
            }
            if (receptor.temVento) bloco.SetVector("_Wind", receptor.vento);
            if (receptor.temTom) bloco.SetVector("_CanopyTone", receptor.tom);
            if (receptor.temSuperficie)
            {
                bloco.SetTexture("_PropSurface", receptor.superficie);
                bloco.SetFloat("_PropSurfaceEnabled", receptor.superficieAtiva);
            }
            if (receptor.temIgnorarSombra)
                bloco.SetFloat("_IgnorePlayerShadow", receptor.ignorarSombraJogador);
            sprite.SetPropertyBlock(bloco);
            bloco.Clear();
        }

        foreach (var sprite in dados.receptoresNevoa)
            if (sprite != null) ConfigurarNevoaDoCenario(sprite);
        foreach (var sombra in dados.silhuetas)
            if (sombra.sprite != null)
                RegistrarSilhuetaDoCentro(sombra.sprite, sombra.inclinacao,
                    sombra.profundidade, sombra.densidade, sombra.oclusor);

        var fonte = GameObject.Find("FontePublica")?.GetComponent<SpriteRenderer>();
        if (fonte != null)
            FontePracaAnimada.Instalar(fonte, controladorMarcus != null ? controladorMarcus.transform : null);
    }
}
