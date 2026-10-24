using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransicaoDeZonaTrigger : Interactable
{
    private Capitulo1Director.ZonaId zonaDestino;
    private Vector2 posicaoDestino;
    private int direcaoDestino;
    private string promptTexto = "Avançar";
    private bool automaticoAoEncostar;
    private bool exigirCasaAvoVisitada;
    private float cooldownAte;

    public override string Prompt => automaticoAoEncostar ? "Seguir" : promptTexto.StartsWith("Sair") ? "Sair" : "Abrir porta";

    public void Configurar(
        Capitulo1Director.ZonaId destino,
        Vector2 posDestino,
        int dirDestino,
        string prompt,
        bool automatico,
        bool exigirCasaAvo = false)
    {
        zonaDestino = destino;
        posicaoDestino = posDestino;
        direcaoDestino = dirDestino;
        promptTexto = prompt;
        automaticoAoEncostar = automatico;
        exigirCasaAvoVisitada = exigirCasaAvo;
    }

    public override void Interact()
    {
        TentarTransicionar();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!automaticoAoEncostar || Time.unscaledTime < cooldownAte)
        {
            return;
        }

        if (other.GetComponent<PlayerController>() != null)
        {
            TentarTransicionar();
        }
    }

    private void TentarTransicionar()
    {
        if (Time.unscaledTime < cooldownAte)
        {
            return;
        }

        if (zonaDestino == Capitulo1Director.ZonaId.Zona2_Posto &&
            Capitulo1Director.ZonaAtual == Capitulo1Director.ZonaId.Zona1_Rodovia &&
            !GameState.Tem(Flags.MapaDoCarroObtido))
        {
            cooldownAte = Time.unscaledTime + 2f;
            GameMessageUI.Instance.Show("O mapa e a lanterna ficaram no meu carro. Preciso pegá-los antes de seguir.", "MARCUS", "marcus");
            return;
        }

        if (exigirCasaAvoVisitada && !GameState.Tem(Flags.ChegouCasaAvo))
        {
            cooldownAte = Time.unscaledTime + 2.0f;
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("MARCUS", "A casa do vô Adílio (nº 40) fica bem aqui neste bairro, na rua de barro logo abaixo. Preciso verificar se ele está em casa antes de subir para a Rua Norte.", "marcus")
            });
            return;
        }

        cooldownAte = Time.unscaledTime + 1.2f;
        if (Capitulo1Director.Instance != null)
        {
            Capitulo1Director.Instance.TransicionarPara(zonaDestino, posicaoDestino, direcaoDestino);
        }
    }
}

/// <summary>
/// Componente de interação narrativa do Capítulo 1: cobre todas as 5 Zonas Externas
/// e os 4 Interiores (Conveniência do Posto, Mercadinho, Casa do Avô e Bar do Zé).
/// </summary>
