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
    [SerializeField] private string semSaberOEndereco = "Faz seis anos... Primeiro preciso pegar a lanterna e o mapa no meu carro marrom logo ao lado [E].";

    [TextArea]
    [SerializeField] private string objetivo = "Examine seu carro marrom estacionado logo ao lado [Tecla E] para pegar a Lanterna e o Mapa de Santa Rita.";

    private IEnumerator Start()
    {
        Capitulo1Director.EnsureForCurrentScene();

        if (!GameState.Tem(Flags.MapaDoCarroObtido))
        {
            GameState.DefinirObjetivo(objetivo);
        }
        else if (!GameState.Tem(Flags.ChegouCasaAvo) && string.IsNullOrEmpty(GameState.ObterObjetivo()))
        {
            GameState.DefinirObjetivo("Seguir pela rodovia para a direita até o Posto de Gasolina e procurar pela casa de Adílio (nº 40).");
        }

        if (GameState.Tem(Flags.ChegouCasaAvo))
        {
            yield break;
        }

        yield return new WaitForSeconds(0.6f);

        if (!GameState.Tem(Flags.MapaDoCarroObtido))
        {
            Legenda.Dizer("Preciso examinar meu carro marrom logo ao lado [Tecla E] para pegar a lanterna e o mapa de Santa Rita.", 6.0f);

            yield return new WaitForSeconds(0.8f);

            if (!GameState.Tem(Flags.MapaDoCarroObtido) && !GameMessageUI.IsShowing)
            {
                GameMessageUI.Instance.Show(
                    "\"Cheguei à entrada de Santa Rita. A avenida está escura demais.\nAntes de seguir em frente, preciso olhar meu carro marrom estacionado logo aqui ao lado [Tecla E] para pegar a Lanterna e o Mapa da cidade.\"",
                    "Marcus",
                    "marcus"
                );
            }
            yield break;
        }

        yield return new WaitForSeconds(esperaAntesDaFala);

        Legenda.Dizer(
            GameState.Tem(Flags.PrologoAbriuPortaLuvas)
                ? comSabendoOEndereco
                : semSaberOEndereco,
            3.8f
        );

        yield return new WaitForSeconds(esperaAntesDoObjetivo);

        Legenda.Dizer("Seguir pela estrada para a direita (Leste) até o Posto de Gasolina. (Pressione M para abrir o Mapa)", 4.5f);
    }
}

/// <summary>
/// Gatilho de transição entre as Zonas da Cidade ("quadrados/cenas") e Interiores,
/// usando corte de câmera com fade-to-black (estilo Pokémon FireRed / OMORI / Corpse Party).
/// </summary>
