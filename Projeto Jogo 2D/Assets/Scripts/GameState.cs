using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EntradaDiario
{
    public string id;
    public string categoria; // "fato", "pista", "interpretacao", "item"
    public string titulo;
    public string descricao;
}

[Serializable]
public class ParNumero
{
    public string chave;
    public int valor;
}

[Serializable]
public class ParTexto
{
    public string chave;
    public string valor;
}

[Serializable]
public class GameStateSnapshot
{
    public int versao = 2;
    public string objetivoAtual = "Chegar até a casa do avô Adilio.";
    public List<string> flags = new List<string>();
    public List<ParNumero> numeros = new List<ParNumero>();
    public List<ParTexto> textos = new List<ParTexto>();
    public List<EntradaDiario> diario = new List<EntradaDiario>();
}

// Memória da partida em RAM durante a sessão.
// REGRA CANÔNICA R1: Progresso jamais é salvo automaticamente ao marcar flags,
// pegar pistas, trocar de cena ou vencer o boss.
// Somente o CheckpointSystem.Save() (acionado manualmente no Orelhão) serializa
// e grava o GameStateSnapshot de forma persistente.
public static class GameState
{
    private const string PrefixoLegado = "estado.";
    private const string ChaveIndiceLegado = "estado._chaves";

    private static readonly HashSet<string> flagsSessao = new HashSet<string>();
    private static readonly Dictionary<string, int> numerosSessao = new Dictionary<string, int>();
    private static readonly Dictionary<string, string> textosSessao = new Dictionary<string, string>();
    private static readonly List<EntradaDiario> diarioSessao = new List<EntradaDiario>();

    private static string objetivoAtualSessao = "Chegar até a casa do avô Adilio.";
    private static bool progressoNaoSalvo = false;

    public static event Action OnEstadoAlterado;

    public static bool TemProgressoNaoSalvo => progressoNaoSalvo;
    public static string ObjetivoAtual => objetivoAtualSessao;
    public static string ObterObjetivo() => objetivoAtualSessao;
    // Preferência em RAM, incluída somente no snapshot do orelhão. Saves antigos: apagada.
    public static bool LanternaLigada => Tem(Flags.LanternaDoCarroObtida) && Numero("equipamento.lanterna_ligada") == 1;
    public static void AlternarLanterna()
    {
        if (Tem(Flags.LanternaDoCarroObtida)) DefinirNumero("equipamento.lanterna_ligada", LanternaLigada ? 0 : 1);
    }

    public static void DefinirObjetivo(string novoObjetivo)
    {
        if (string.IsNullOrEmpty(novoObjetivo)) return;
        if (objetivoAtualSessao == novoObjetivo) return;
        objetivoAtualSessao = novoObjetivo;
        progressoNaoSalvo = true;
        OnEstadoAlterado?.Invoke();
    }

    // --- flags (em RAM) ----------------------------------------------

    public static bool Tem(string flag)
    {
        if (string.IsNullOrEmpty(flag)) return false;
        return flagsSessao.Contains(flag);
    }

    public static void Marcar(string flag, bool valor = true)
    {
        if (string.IsNullOrEmpty(flag)) return;

        bool alterou = valor ? flagsSessao.Add(flag) : flagsSessao.Remove(flag);
        if (alterou)
        {
            progressoNaoSalvo = true;
            SincronizarDiarioAutomaticoPorFlag(flag);
            OnEstadoAlterado?.Invoke();
        }
    }

    // --- contadores (em RAM) -----------------------------------------

    public static int Numero(string chave, int padrao = 0)
    {
        if (string.IsNullOrEmpty(chave)) return padrao;
        return numerosSessao.TryGetValue(chave, out int val) ? val : padrao;
    }

    public static void DefinirNumero(string chave, int valor)
    {
        if (string.IsNullOrEmpty(chave)) return;
        numerosSessao[chave] = valor;
        progressoNaoSalvo = true;
        OnEstadoAlterado?.Invoke();
    }

    // --- texto (em RAM) ----------------------------------------------

    public static string Texto(string chave, string padrao = "")
    {
        if (string.IsNullOrEmpty(chave)) return padrao;
        return textosSessao.TryGetValue(chave, out string val) ? val : padrao;
    }

    public static void DefinirTexto(string chave, string valor)
    {
        if (string.IsNullOrEmpty(chave)) return;
        textosSessao[chave] = valor ?? "";
        progressoNaoSalvo = true;
        OnEstadoAlterado?.Invoke();
    }

    // --- diário de investigação (fatos, pistas, interpretações, itens) ---

    public static void RegistrarNotaDiario(string id, string titulo, string categoria, string descricao)
    {
        string catLower = (categoria ?? "pista").Trim().ToLowerInvariant();
        if (catLower.Contains("hip") || catLower.Contains("interp")) catLower = "interpretacao";
        else if (catLower.Contains("fato")) catLower = "fato";
        else if (catLower.Contains("item") || catLower.Contains("ficha")) catLower = "item";
        else catLower = "pista";

        RegistrarDiario(id, catLower, titulo, descricao);
    }

    public static void RegistrarDiario(string id, string categoria, string titulo, string descricao)
    {
        if (string.IsNullOrEmpty(id)) return;
        for (int i = 0; i < diarioSessao.Count; i++)
        {
            if (diarioSessao[i].id == id)
            {
                if (diarioSessao[i].categoria == categoria && diarioSessao[i].titulo == titulo &&
                    diarioSessao[i].descricao == descricao) return;
                diarioSessao[i].categoria = categoria;
                diarioSessao[i].titulo = titulo;
                diarioSessao[i].descricao = descricao;
                progressoNaoSalvo = true;
                OnEstadoAlterado?.Invoke();
                return;
            }
        }

        diarioSessao.Add(new EntradaDiario
        {
            id = id,
            categoria = string.IsNullOrEmpty(categoria) ? "pista" : categoria,
            titulo = titulo ?? id,
            descricao = descricao ?? ""
        });
        progressoNaoSalvo = true;
        OnEstadoAlterado?.Invoke();
    }

    public static IReadOnlyList<EntradaDiario> ObterEntradasDiario()
    {
        return diarioSessao;
    }

    private static void SincronizarDiarioAutomaticoPorFlag(string flag)
    {
        if (flag == Flags.PrologoAbriuPortaLuvas)
        {
            RegistrarDiario("fato_endereco_adilio", "fato", "Endereço anotado",
                "No porta-luvas do carro havia um mapa dobrado de Santa Rita com a anotação de Adilio: Bairro das Casas, 40.");
        }
        else if (flag == Flags.FalouFrentista)
        {
            RegistrarDiario("fato_posto_frentista", "fato", "Relato no Posto",
                "O frentista reconheceu meu carro e confirmou que Adilio mora subindo para o Bairro das Casas.");
        }
        else if (flag == Flags.ChegouCasaAvo)
        {
            RegistrarDiario("fato_casa_destrancada", "fato", "Casa de Adilio vazia",
                "Adilio parou de responder hoje (2004). A porta estava destrancada e ele não está em casa.");
            DefinirObjetivo("Examinar os sinais na casa de Adilio.");
        }
        else if (flag == Flags.ExaminouCafeCasa || flag == "viu_chaleira")
        {
            RegistrarDiario("pista_cafe_passado", "pista", "Café passado hoje",
                "O bule ainda guarda calor e há café na xícara. Isso sugere uso recente; não permite saber quando Adílio saiu nem por quê.");
        }
        else if (flag == Flags.ExaminouCartasCasa)
        {
            RegistrarDiario("pista_cartas_mesa", "pista", "Correspondências na mesa",
                "As cartas de baralho ficaram sobre a mesa, no meio de uma partida de paciência. Não sei se Adílio pretendia continuar depois.");
        }
        else if (flag == Flags.ExaminouDesordemCasa)
        {
            RegistrarDiario("pista_desordem_localizada", "interpretacao", "Desordem localizada na sala",
                "O tapete está dobrado na ponta e a cadeira/sofá está levemente fora de lugar. Sem sangue ou objetos quebrados, mas sugere resistência ou saída às pressas.");
        }
        else if (flag == Flags.AchouReciboMercadinho)
        {
            RegistrarDiario("pista_recibo_mercadinho", "pista", "Cupom do Mercadinho (2004)",
                "Recibo emitido hoje no Mercadinho da encruzilhada. Alguém lá deve ter visto Adilio ou quem o procurava.");
            if (!Tem(Flags.OuviuRelatoAtendente))
                DefinirObjetivo("Levar o recibo ao Mercadinho na encruzilhada.");
        }
        else if (flag == Flags.OuviuRelatoAtendente)
        {
            RegistrarDiario("pista_relato_mercadinho", "pista", "Desconhecido no Mercadinho",
                "Ao apresentar o cupom, ouvi da atendente que uma caminhonete escura levava Adílio para a Rua Norte, perto do Bar do Zé. Ela não ouviu a conversa dentro do veículo nem sabe o que aconteceu depois.");
            if (!Tem(Flags.VenceuZeSinuca))
                DefinirObjetivo("Ir ao Bar do Zé na rua norte e questionar Zé.");
        }
        else if (flag == Flags.VenceuZeSinuca)
        {
            RegistrarDiario("pista_relato_ze", "interpretacao", "O que Zé deixou escapar",
                "Após perder duas mãos na sinuca, Zé admitiu que o homem que perguntou por Adilio não veio a passeio e que meu avô jamais sairia de casa à noite deixando o café na mesa por vontade própria.");
            RegistrarDiario(Flags.FichaZeObtida, "item", "Ficha do Clube (Origem: Zé)",
                "Ficha metálica com a insígnia do Clube de Santa Rita entregue por Zé após a série de sinuca.");
            DefinirObjetivo("Seguir para o Centro da cidade (ou salvar no Orelhão do bar antes de avançar).");
        }
    }

    // --- snapshots para o CheckpointSystem ---------------------------

    public static void ConfirmarSaveManual()
    {
        progressoNaoSalvo = false;
    }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    internal static void RestaurarIndicadorDeProgresso(bool pendente) => progressoNaoSalvo = pendente;
#endif

    public static GameStateSnapshot ExportarSnapshot()
    {
        var snap = new GameStateSnapshot
        {
            versao = 2,
            objetivoAtual = objetivoAtualSessao,
            flags = new List<string>(flagsSessao),
            numeros = new List<ParNumero>(),
            textos = new List<ParTexto>(),
            diario = new List<EntradaDiario>()
        };

        foreach (var kv in numerosSessao)
            snap.numeros.Add(new ParNumero { chave = kv.Key, valor = kv.Value });

        foreach (var kv in textosSessao)
            snap.textos.Add(new ParTexto { chave = kv.Key, valor = kv.Value });

        for (int i = 0; i < diarioSessao.Count; i++)
        {
            var d = diarioSessao[i];
            snap.diario.Add(new EntradaDiario
            {
                id = d.id,
                categoria = d.categoria,
                titulo = d.titulo,
                descricao = d.descricao
            });
        }

        return snap;
    }

    public static void RestaurarSnapshot(GameStateSnapshot snap) => ImportarSnapshot(snap);

    public static void ImportarSnapshot(GameStateSnapshot snap)
    {
        ResetarMemoriaSilencioso();
        if (snap == null) return;

        if (!string.IsNullOrEmpty(snap.objetivoAtual))
            objetivoAtualSessao = snap.objetivoAtual;

        if (snap.flags != null)
        {
            for (int i = 0; i < snap.flags.Count; i++)
            {
                if (!string.IsNullOrEmpty(snap.flags[i]))
                    flagsSessao.Add(snap.flags[i]);
            }
        }

        if (snap.numeros != null)
        {
            for (int i = 0; i < snap.numeros.Count; i++)
            {
                if (!string.IsNullOrEmpty(snap.numeros[i].chave))
                    numerosSessao[snap.numeros[i].chave] = snap.numeros[i].valor;
            }
        }

        if (snap.textos != null)
        {
            for (int i = 0; i < snap.textos.Count; i++)
            {
                if (!string.IsNullOrEmpty(snap.textos[i].chave))
                    textosSessao[snap.textos[i].chave] = snap.textos[i].valor ?? "";
            }
        }

        if (snap.diario != null)
        {
            for (int i = 0; i < snap.diario.Count; i++)
            {
                var d = snap.diario[i];
                if (d != null && !string.IsNullOrEmpty(d.id))
                {
                    diarioSessao.Add(new EntradaDiario
                    {
                        id = d.id,
                        categoria = d.categoria,
                        titulo = d.titulo,
                        descricao = d.descricao
                    });
                }
            }
        }

        progressoNaoSalvo = false;
        OnEstadoAlterado?.Invoke();
    }

    private static void ResetarMemoriaSilencioso()
    {
        flagsSessao.Clear();
        numerosSessao.Clear();
        textosSessao.Clear();
        diarioSessao.Clear();
        objetivoAtualSessao = "Chegar até a casa do avô Adilio.";
        progressoNaoSalvo = false;
    }

    // --- ciclo de vida -----------------------------------------------

    public static void Limpar()
    {
        ResetarMemoriaSilencioso();
        if (CheckpointSystem.EmArmazenamentoDeTeste)
        {
            OnEstadoAlterado?.Invoke();
            return;
        }

        // Limpa eventuais chaves legadas que versões antigas gravaram direto no PlayerPrefs
        string bruto = PlayerPrefs.GetString(ChaveIndiceLegado, string.Empty);
        foreach (string chave in bruto.Split(';'))
        {
            if (!string.IsNullOrEmpty(chave))
                PlayerPrefs.DeleteKey(PrefixoLegado + chave);
        }
        PlayerPrefs.DeleteKey(ChaveIndiceLegado);
        PlayerPrefs.DeleteKey(PrefixoLegado + "sabe_que_avo_saiu_sozinho");
        PlayerPrefs.Save();

        OnEstadoAlterado?.Invoke();
    }
}

// Nomes canônicos das flags do Capítulo 1 (Revisão R1).
public static class Flags
{
    // Prólogo
    public const string PrologoOlhouCelular = "prologo_olhou_celular";
    public const string PrologoTentouLigarAvo = "prologo_tentou_ligar_avo";
    public const string PrologoOuviuRadio = "prologo_ouviu_radio";
    public const string PrologoAbriuPortaLuvas = "prologo_abriu_porta_luvas";
    public const string PrologoViuFotoAvo = "prologo_viu_foto_avo";
    public const string PrologoConcluido = "prologo_concluido";
    public const string MapaDoCarroObtido = "mapa_do_carro_obtido";
    public const string LanternaDoCarroObtida = "lanterna_do_carro_obtida";

    // Capítulo 1 — Entrada, Posto, Casa de Adilio, Mercadinho e Bar do Zé
    public const string FalouFrentista = "falou_frentista";
    public const string FalouComFrentista = FalouFrentista;

    public const string ChegouCasaAvo = "chegou_casa_avo";

    public const string ExaminouCafeCasa = "pista_cafe_casa";
    public const string Pista_CafeAindaQuente = ExaminouCafeCasa;

    public const string ExaminouCartasCasa = "pista_cartas_casa";
    public const string Pista_CartasMesa = ExaminouCartasCasa;

    public const string ExaminouDesordemCasa = "pista_desordem_casa";
    public const string Pista_DesordemPorta = ExaminouDesordemCasa;

    public const string AchouReciboMercadinho = "pista_recibo_mercadinho";
    public const string Pista_NotaFiscal2004 = AchouReciboMercadinho;

    public const string CasaAvo_LeuBilhete = "casa_bilhete";
    public const string CasaAvo_OlhouTelefone = "casa_telefone_fora_do_gancho";
    public const string Pista_CasaAvoCompleta = "pista_casa_avo_completa";
    public const string Pista_RumoBarDoZe = "pista_rumo_bar_do_ze";

    public const string FalouAtendenteMercadinho = "falou_atendente_mercadinho";
    public const string FalouComAtendenteMercadinho = FalouAtendenteMercadinho;
    public const string ApresentouReciboAtendente = "apresentou_recibo_atendente";
    public const string OuviuRelatoAtendente = "ouviu_relato_atendente";

    public const string EntrouNoBarDoZe = "entrou_no_bar_do_ze";
    public const string ConheceuZeBar = "conheceu_ze_bar";
    public const string FalouComZeAntesDaSinuca = ConheceuZeBar;

    public const string VenceuZeSinuca = "venceu_ze_sinuca";
    public const string ZeDerrotadoNaSinuca = VenceuZeSinuca;

    public const string FichaZeObtida = "ficha_clube_ze_01";
    public const string RecebeuFichaClubeZe = FichaZeObtida;

    public const string Pista_AdilioNaoSaiuVoluntariamente = "pista_adilio_nao_saiu_voluntariamente";

    public const string CentroLiberado = "centro_liberado";
    public const string LimiarCentroDesbloqueado = CentroLiberado;
    public const string Capitulo1Concluido = "capitulo_1_concluido";

    // Contadores de Orelhão
    public const string OrelhaoVozIndice = "orelhao_voz_indice";
    public const string OrelhaoUsos = "orelhao_usos";
    public const string OrelhaoUsado = "orelhao_usado";
}
