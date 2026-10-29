using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChegadaEmSantaRita : MonoBehaviour
{
    [SerializeField] private float esperaAntesDaFala = 2.0f;
    [SerializeField] private float esperaAntesDoObjetivo = 1.4f;

    [TextArea]
    [SerializeField] private string comSabendoOEndereco = "Bairro das Casas, 40. Preciso seguir pela estrada de asfalto e subir na encruzilhada.";

    [TextArea]
    [SerializeField] private string semSaberOEndereco = "Faz tempo. Acho que a casa dele era subindo.";

    [TextArea]
    [SerializeField] private string objetivo = "Pegar a lanterna no carro.";

    private IEnumerator Start()
    {
        Capitulo1Director.EnsureForCurrentScene();

        // A chegada não explica nada: o jogador fica parado na chuva, ouve a
        // cidade e só então Marcus lembra da lanterna. A tecla E flutuando
        // sobre o carro é o único tutorial.
        if (!GameState.Tem(Flags.MapaDoCarroObtido))
        {
            GameState.DefinirObjetivo("Pegar a lanterna no carro.");
        }
        else if (!GameState.Tem(Flags.ChegouCasaAvo) && string.IsNullOrEmpty(GameState.ObterObjetivo()))
        {
            GameState.DefinirObjetivo("Ir até a casa do vô, no Bairro das Casas, nº 40.");
        }

        if (GameState.Tem(Flags.ChegouCasaAvo))
        {
            yield break;
        }

        yield return new WaitForSeconds(esperaAntesDaFala);

        if (!GameState.Tem(Flags.MapaDoCarroObtido))
        {
            if (!GameMessageUI.IsShowing)
                Legenda.Dizer("\"Escuro demais. A lanterna ficou no porta-luvas.\"", 4f);
            yield break;
        }

        Legenda.Dizer(
            GameState.Tem(Flags.PrologoAbriuPortaLuvas)
                ? comSabendoOEndereco
                : semSaberOEndereco,
            3.8f
        );
    }
}

/// <summary>
/// Gatilho de transição entre as Zonas da Cidade ("quadrados/cenas") e Interiores,
/// usando corte de câmera com fade-to-black (estilo Pokémon FireRed / OMORI / Corpse Party).
/// </summary>
