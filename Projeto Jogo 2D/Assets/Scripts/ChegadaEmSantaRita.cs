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
#pragma warning disable CS0414
    [SerializeField] private float esperaAntesDoObjetivo = 1.4f;
#pragma warning restore CS0414

    [TextArea]
    [SerializeField] private string comSabendoOEndereco = "Bairro das Casas, 40. Preciso seguir pela estrada de asfalto e subir na encruzilhada.";

    [TextArea]
    [SerializeField] private string semSaberOEndereco = "Faz tempo. Acho que a casa dele era subindo.";

#pragma warning disable CS0414
    [TextArea]
    [SerializeField] private string objetivo = "Pegar a lanterna no carro.";
#pragma warning restore CS0414

    private IEnumerator Start()
    {
        Capitulo1Director.EnsureForCurrentScene();

        // A chegada não explica nada: o jogador fica parado na chuva, ouve a
        // cidade e só então Marcus lembra da lanterna. A tecla E flutuando
        // sobre o carro é o único tutorial.
        if (!GameState.Tem(Flags.LanternaDoCarroObtida))
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

        bool tinhaLanterna = GameState.Tem(Flags.LanternaDoCarroObtida);
        yield return new WaitForSeconds(esperaAntesDaFala);
        if (tinhaLanterna != GameState.Tem(Flags.LanternaDoCarroObtida)) yield break;

        if (!tinhaLanterna)
            SantaRitaFalasEntrada.DizerEspontanea("Escuro demais. A lanterna ficou no porta-luvas.", 2.2f);
        else
            SantaRitaFalasEntrada.DizerEspontanea(
                GameState.Tem(Flags.PrologoAbriuPortaLuvas) ? comSabendoOEndereco : semSaberOEndereco, 2.2f);
    }
}

/// <summary>
/// Gatilho de transição entre as Zonas da Cidade ("quadrados/cenas") e Interiores,
/// usando corte de câmera com fade-to-black (estilo Pokémon FireRed / OMORI / Corpse Party).
/// </summary>
