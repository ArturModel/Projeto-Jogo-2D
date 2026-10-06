using System.Collections.Generic;
using UnityEngine;

// Memória da partida. O CheckpointSystem guarda ONDE o jogador estava; o
// GameState guarda O QUE já aconteceu.
//
// Quase tudo em Santa Rita depende disso: uma fala muda porque você tentou
// ligar para o avô no carro, o orelhão sabe quantas vezes você já atendeu,
// uma porta só abre depois de achar o fragmento certo da fita. Sem um lugar
// único para esses fatos, cada script inventaria a própria chave de
// PlayerPrefs e nada conseguiria ser limpo de forma confiável em "Novo jogo".
public static class GameState
{
    private const string Prefixo = "estado.";
    private const string ChaveIndice = "estado._chaves";

    // Toda chave escrita entra neste índice. É o que permite Limpar() apagar
    // exatamente o que a partida criou, sem varrer o PlayerPrefs inteiro.
    private static HashSet<string> indice;

    private static HashSet<string> Indice
    {
        get
        {
            if (indice == null)
            {
                indice = new HashSet<string>();
                string bruto = PlayerPrefs.GetString(ChaveIndice, string.Empty);

                foreach (string chave in bruto.Split(';'))
                {
                    if (!string.IsNullOrEmpty(chave))
                    {
                        indice.Add(chave);
                    }
                }
            }

            return indice;
        }
    }

    private static void Registrar(string chave)
    {
        if (Indice.Add(chave))
        {
            PlayerPrefs.SetString(ChaveIndice, string.Join(";", Indice));
        }
    }

    // --- flags -------------------------------------------------------

    public static bool Tem(string flag)
    {
        return PlayerPrefs.GetInt(Prefixo + flag, 0) == 1;
    }

    public static void Marcar(string flag, bool valor = true)
    {
        Registrar(flag);
        PlayerPrefs.SetInt(Prefixo + flag, valor ? 1 : 0);
        PlayerPrefs.Save();
    }

    // --- contadores --------------------------------------------------

    public static int Numero(string chave, int padrao = 0)
    {
        return PlayerPrefs.GetInt(Prefixo + chave, padrao);
    }

    public static void DefinirNumero(string chave, int valor)
    {
        Registrar(chave);
        PlayerPrefs.SetInt(Prefixo + chave, valor);
        PlayerPrefs.Save();
    }

    // --- texto -------------------------------------------------------

    public static string Texto(string chave, string padrao = "")
    {
        return PlayerPrefs.GetString(Prefixo + chave, padrao);
    }

    public static void DefinirTexto(string chave, string valor)
    {
        Registrar(chave);
        PlayerPrefs.SetString(Prefixo + chave, valor);
        PlayerPrefs.Save();
    }

    // --- ciclo de vida -----------------------------------------------

    public static void Limpar()
    {
        foreach (string chave in Indice)
        {
            PlayerPrefs.DeleteKey(Prefixo + chave);
        }

        Indice.Clear();
        PlayerPrefs.DeleteKey(ChaveIndice);
        PlayerPrefs.Save();
    }
}

// Nomes canônicos das flags, vindos do documento de roteiro. Ficam aqui como
// constantes para que um erro de digitação vire erro de compilação em vez de
// uma flag que nunca liga.
public static class Flags
{
    // Prólogo — §11 do documento do prólogo
    public const string PrologoOlhouCelular = "prologo_olhou_celular";
    public const string PrologoTentouLigarAvo = "prologo_tentou_ligar_avo";
    public const string PrologoOuviuRadio = "prologo_ouviu_radio";
    public const string PrologoAbriuPortaLuvas = "prologo_abriu_porta_luvas";
    public const string PrologoViuFotoAvo = "prologo_viu_foto_avo";
    public const string PrologoConcluido = "prologo_concluido";

    // Ato I
    public const string ChegouCasaAvo = "chegou_casa_avo";

    // Contadores
    public const string OrelhaoVozIndice = "orelhao_voz_indice";
    public const string OrelhaoUsos = "orelhao_usos";
}
