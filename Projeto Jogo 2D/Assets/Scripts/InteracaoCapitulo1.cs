using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Cadeia de pistas do Capítulo 1. Cada pessoa sabe uma parte só, e cada
// objeto mostra o que está nele; quem junta as partes é o jogador.
//
// Linha do tempo interna (terça, 19/10/2004), para os vestígios não se contradizerem:
//   qui 14/10, 16h  A atendente entrega engradados no Bar do Zé e vê um homem
//                   de capa na mesa dos fundos com Zé. Os dois param de falar.
//   seg 18/10       O homem de capa pergunta no mercadinho onde Adílio mora.
//   07h40           Adílio compra querosene no posto, a pé, como sempre.
//   à tarde         O homem de capa volta ao mercadinho e pergunta a rua.
//   18h10           Adílio compra café e velas. A atendente escreve um aviso
//                   no verso do cupom.
//   ~20h30          Três batidas fortes na casa 40 (Dona Lurdes). Perto da
//                   porta o banco cai e o fone sai do gancho: a linha passa a
//                   dar ocupado (Marcus, na estrada).
//   ~20h40          Farol parado na frente da casa por dez minutos; depois o
//                   motor sobe a ladeira (Seu Ramiro).
//   ~20h50          Nos fundos do bar, Zé vê dois homens levarem Adílio pelo
//                   portão do Centro; um abre o cadeado com chave. Zé tranca a
//                   porta dos fundos (o freguês nota "lá pelas nove").
//   ~21h30          Marcus chega. O carro afoga na baixada da entrada.
public class InteracaoCapitulo1 : Interactable
{
    public enum TipoInteracao
    {
        CarroMarcus,
        PlacaRodoviaEntrada,
        MalaAbandonadaEstrada,
        BombaCombustivelPosto,
        CarroEstacionadoPosto,
        VendaAntigaFechada,
        NpcFrentista,
        LivroRegistroPosto,
        TvPortatilPosto,
        PicapeEncruzilhada,
        QuadroAvisosEncruzilhada,
        PlacaSetaBairro,
        NpcAtendenteMercadinho,
        CadernoFiadoMercadinho,
        PrateleirasMercadinho,
        VizinhaDonaLurdes,
        GalpaoBairroPista,
        NpcMoradorBairro,
        CaixaCorreioAvo,
        PistaCafeCasaAvo,
        PistaCartasCasaAvo,
        PistaNotaFiscal2004,
        PistaDesordemPorta,
        RetratoFamiliaCasaAvo,
        TelefoneCasaAvo,
        CaminhoneteClubeBar,
        PlacaBarDoZe,
        NpcZeBar,
        MesaSinucaBar,
        BalcaoBarDoZe,
        QuadroRegrasSinuca,
        LimiarCentro,
        PortaEntradaBarDoZe,
        NpcFrequentadorBar,
        CentroBloqueioObra
    }

    // Pistas novas do capítulo 1 (as antigas continuam em Flags, no GameState).
    public const string Pista_QuerosenePosto = "pista_querosene_posto";
    public const string Pista_BatidasLurdes = "pista_batidas_lurdes";
    public const string Pista_FarolRamiro = "pista_farol_ramiro";
    public const string Pista_PneusBairro = "pista_pneus_bairro";
    public const string Pista_RastroPortao = "pista_rastro_portao";
    public const string Pista_FundosTrancados = "pista_fundos_trancados";
    public const string Pista_FotoSalao1990 = "pista_foto_salao_1990";
    public const string ZeDesconversou = "ze_desconversou";
    public const string ConheceuFrentista = "c1_frentista_reconheceu";

    private const string EtiquetaNova = "Pista · anotada no diário";
    private const string EtiquetaRevisita = "Pista";

    [SerializeField] private TipoInteracao tipo;
    [SerializeField] private string promptCustomizado = "";

    public TipoInteracao Tipo
    {
        get => tipo;
        set => tipo = value;
    }

    public void Configurar(TipoInteracao novoTipo, string novoPrompt = "")
    {
        tipo = novoTipo;
        promptCustomizado = novoPrompt;
    }

    public override string Prompt
    {
        get
        {
            if (!string.IsNullOrEmpty(promptCustomizado))
            {
                return promptCustomizado;
            }

            bool zeDerrotado = GameState.Tem(Flags.ZeDerrotadoNaSinuca);
            bool desafioAceito = GameState.Tem(Flags.FalouComZeAntesDaSinuca);

            switch (tipo)
            {
                case TipoInteracao.CarroMarcus:
                    return GameState.Tem(Flags.MapaDoCarroObtido) ? "Olhar o carro" : "Abrir o porta-luvas";
                case TipoInteracao.PlacaRodoviaEntrada:
                    return "Ler o quadro da parada";
                case TipoInteracao.MalaAbandonadaEstrada:
                    return "Olhar a mala no barro";
                case TipoInteracao.BombaCombustivelPosto:
                    return "Olhar a bomba";
                case TipoInteracao.CarroEstacionadoPosto:
                    return "Olhar a picape";
                case TipoInteracao.VendaAntigaFechada:
                    return "Ler a tabuleta da porta";
                case TipoInteracao.NpcFrentista:
                    return "Falar com o frentista";
                case TipoInteracao.LivroRegistroPosto:
                    return "Ler o caderno do balcão";
                case TipoInteracao.TvPortatilPosto:
                    return "Olhar a TV";
                case TipoInteracao.PicapeEncruzilhada:
                    return "Olhar a picape de entrega";
                case TipoInteracao.QuadroAvisosEncruzilhada:
                    return "Ler o mural";
                case TipoInteracao.PlacaSetaBairro:
                    return "Ler a placa";
                case TipoInteracao.NpcAtendenteMercadinho:
                    return "Falar com a atendente";
                case TipoInteracao.CadernoFiadoMercadinho:
                    return "Ler o caderno de vendas";
                case TipoInteracao.PrateleirasMercadinho:
                    return "Olhar as prateleiras";
                case TipoInteracao.VizinhaDonaLurdes:
                    return "Bater na porta (nº 14)";
                case TipoInteracao.GalpaoBairroPista:
                    return "Olhar as marcas no barro";
                case TipoInteracao.NpcMoradorBairro:
                    return "Falar com o vizinho";
                case TipoInteracao.CaixaCorreioAvo:
                    return "Olhar a caixa de correio";
                case TipoInteracao.PistaCafeCasaAvo:
                    return "Olhar o bule e a xícara";
                case TipoInteracao.PistaCartasCasaAvo:
                    return "Olhar as cartas na mesa";
                case TipoInteracao.PistaNotaFiscal2004:
                    return "Olhar o papel amassado";
                case TipoInteracao.PistaDesordemPorta:
                    return "Olhar perto da porta";
                case TipoInteracao.RetratoFamiliaCasaAvo:
                    return "Olhar o porta-retrato";
                case TipoInteracao.TelefoneCasaAvo:
                    return "Olhar o telefone";
                case TipoInteracao.CaminhoneteClubeBar:
                    return "Olhar a caminhonete";
                case TipoInteracao.PlacaBarDoZe:
                    return "Ler o letreiro";
                case TipoInteracao.NpcZeBar:
                    return !zeDerrotado && desafioAceito ? "Jogar a melhor de três" : "Falar com Zé";
                case TipoInteracao.MesaSinucaBar:
                    return !zeDerrotado && desafioAceito ? "Jogar a melhor de três" : "Olhar a mesa de sinuca";
                case TipoInteracao.BalcaoBarDoZe:
                    return "Olhar atrás do balcão";
                case TipoInteracao.QuadroRegrasSinuca:
                    return "Ler as regras da casa";
                case TipoInteracao.LimiarCentro:
                    return zeDerrotado ? "Atravessar o portão" : "Olhar o portão";
                case TipoInteracao.CentroBloqueioObra:
                    return "Ler o aviso da obra";
                case TipoInteracao.PortaEntradaBarDoZe:
                    return "Entrar no bar";
                case TipoInteracao.NpcFrequentadorBar:
                    return "Falar com o freguês";
                default:
                    return "Olhar";
            }
        }
    }

    public override void Interact()
    {
        switch (tipo)
        {
            case TipoInteracao.CarroMarcus: InteragirCarroMarcus(); break;
            case TipoInteracao.PlacaRodoviaEntrada: InteragirPlacaRodovia(); break;
            case TipoInteracao.MalaAbandonadaEstrada: InteragirMalaAbandonada(); break;
            case TipoInteracao.BombaCombustivelPosto: InteragirBombaPosto(); break;
            case TipoInteracao.CarroEstacionadoPosto: InteragirCarroPosto(); break;
            case TipoInteracao.VendaAntigaFechada: InteragirVendaAntiga(); break;
            case TipoInteracao.NpcFrentista: InteragirFrentista(); break;
            case TipoInteracao.LivroRegistroPosto: InteragirLivroPosto(); break;
            case TipoInteracao.TvPortatilPosto: InteragirTvPosto(); break;
            case TipoInteracao.PicapeEncruzilhada: InteragirPicapeEncruzilhada(); break;
            case TipoInteracao.QuadroAvisosEncruzilhada: InteragirQuadroEncruzilhada(); break;
            case TipoInteracao.PlacaSetaBairro: InteragirPlacaSetaBairro(); break;
            case TipoInteracao.NpcAtendenteMercadinho: InteragirAtendenteMercadinho(); break;
            case TipoInteracao.CadernoFiadoMercadinho: InteragirCadernoMercadinho(); break;
            case TipoInteracao.PrateleirasMercadinho: InteragirPrateleirasMercadinho(); break;
            case TipoInteracao.VizinhaDonaLurdes: InteragirVizinhaLurdes(); break;
            case TipoInteracao.GalpaoBairroPista: InteragirMarcasBarro(); break;
            case TipoInteracao.NpcMoradorBairro: InteragirMoradorBairro(); break;
            case TipoInteracao.CaixaCorreioAvo: InteragirCaixaCorreioAvo(); break;
            case TipoInteracao.PistaCafeCasaAvo: InteragirPistaCafe(); break;
            case TipoInteracao.PistaCartasCasaAvo: InteragirPistaCartas(); break;
            case TipoInteracao.PistaNotaFiscal2004: InteragirPistaNotaFiscal(); break;
            case TipoInteracao.PistaDesordemPorta: InteragirPistaDesordem(); break;
            case TipoInteracao.RetratoFamiliaCasaAvo: InteragirRetratoFamilia(); break;
            case TipoInteracao.TelefoneCasaAvo: InteragirTelefoneCasaAvo(); break;
            case TipoInteracao.CaminhoneteClubeBar: InteragirCaminhoneteBar(); break;
            case TipoInteracao.PlacaBarDoZe: InteragirPlacaBar(); break;
            case TipoInteracao.NpcZeBar: InteragirZe(); break;
            case TipoInteracao.MesaSinucaBar: InteragirMesaSinuca(); break;
            case TipoInteracao.BalcaoBarDoZe: InteragirBalcaoBar(); break;
            case TipoInteracao.QuadroRegrasSinuca: InteragirQuadroRegras(); break;
            case TipoInteracao.LimiarCentro: InteragirLimiarCentro(); break;
            case TipoInteracao.CentroBloqueioObra: InteragirAvisoObra(); break;
            case TipoInteracao.PortaEntradaBarDoZe: EntrarNoBar(); break;
            case TipoInteracao.NpcFrequentadorBar: InteragirFrequentadorBar(); break;
        }
    }

    // =========================================================================
    // Utilidades
    // =========================================================================

    private static DialogoLinha Marcus(string texto) => new DialogoLinha("MARCUS", texto, "marcus");

    // Pensamento de Marcus: mesma caixa, em itálico, para não parecer fala.
    private static DialogoLinha Pensa(string texto) => new DialogoLinha("MARCUS", "<i>" + texto + "</i>", "marcus");

    private static void Falar(params DialogoLinha[] linhas) => Falar(null, linhas);

    private static void Falar(Action aoFechar, params DialogoLinha[] linhas)
    {
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>(linhas), aoFechar);
    }

    private static bool JaNoDiario(string id)
    {
        var entradas = GameState.ObterEntradasDiario();
        for (int i = 0; i < entradas.Count; i++)
        {
            if (entradas[i].id == id) return true;
        }
        return false;
    }

    // Registra a nota e diz se ela é nova (a tela de inspeção avisa "anotada no diário").
    private static bool Anotar(string id, string titulo, string categoria, string descricao)
    {
        bool nova = !JaNoDiario(id);
        GameState.RegistrarNotaDiario(id, titulo, categoria, descricao);
        return nova;
    }

    private static string Etiqueta(bool nova) => nova ? EtiquetaNova : EtiquetaRevisita;

    // =========================================================================
    // Zona 1: rodovia de entrada
    // =========================================================================

    private void InteragirCarroMarcus()
    {
        bool primeiraVez = !GameState.Tem(Flags.MapaDoCarroObtido);
        GameState.Marcar(Flags.PrologoAbriuPortaLuvas);
        GameState.Marcar(Flags.MapaDoCarroObtido);
        GameState.Marcar(Flags.LanternaDoCarroObtida);
        SantaRitaSound.Play(primeiraVez ? "door" : "inspect", .13f);

        if (!primeiraVez)
        {
            Falar(Marcus("Não pega. Afogou na baixada e vai ficar aí até a água baixar."));
            return;
        }

        if (!GameState.Tem(Flags.ChegouCasaAvo))
            GameState.DefinirObjetivo("Ir até a casa do vô, no Bairro das Casas, nº 40.");

        Anotar(
            "porta_luvas_endereco",
            "Carro afogado na entrada",
            "FATO",
            "O motor afogou na baixada alagada da entrada. Sigo a pé. Levei a lanterna e o mapa da cidade que o vô me deu quando fui embora: 'Bairro das Casas, 40. Segue o asfalto e sobe no barro.'"
        );

        Falar(() =>
        {
            DicaDeTecla.Mostrar("F", "Lanterna", () => GameState.LanternaLigada);
            DicaDeTecla.Mostrar("M", "Mapa", () => MapaInterativoUI.IsShowing, 8f);
        },
            Marcus("Afogou de vez na baixada. Daqui eu sigo a pé."),
            Marcus("A lanterna estava no porta-luvas, embaixo do mapa da cidade que o vô me deu quando eu fui embora."),
            Marcus("No verso, a letra dele: \"Bairro das Casas, 40. Segue o asfalto e sobe no barro.\"")
        );
    }

    private void InteragirPlacaRodovia()
    {
        GameMessageUI.Instance.ShowInspection("QUADRO DA PARADA", "RoadSign_Blank",
            "PARADA SANTA RITA — ENTRADA\n\nÚltimo ônibus: 19h40.\nLinhas noturnas suspensas durante a obra da avenida do Centro.\n\nColado no canto, um adesivo quase apagado:\n\"SANTA RITA JAMAIS ABANDONA OS SEUS\"",
            pensamento: "Esse adesivo está em todo canto desde que eu era guri.");
    }

    private void InteragirMalaAbandonada()
    {
        Falar(
            Marcus("Uma mala velha atolada no barro, aberta e vazia."),
            Marcus("A etiqueta da rodoviária desmanchou com a chuva. Só sobrou o ano: 98."),
            Pensa("Alguém saiu daqui com pressa. Ou nunca chegou a sair.")
        );
    }

    // =========================================================================
    // Zona 2 e interior: posto
    // =========================================================================

    private void InteragirBombaPosto()
    {
        GameMessageUI.Instance.ShowInspection("BOMBA DE DIESEL", null,
            "O mostrador mecânico parou em 45,0 litros. O bico ainda pinga.",
            pensamento: "Alguém encheu um tanque grande hoje. Caminhão, talvez.");
    }

    private void InteragirCarroPosto()
    {
        Falar(Marcus("A picape do posto. Galões vazios e uma lona encharcada na caçamba."));
    }

    private void InteragirVendaAntiga()
    {
        Falar(
            Marcus("A venda onde o vô comprava fumo de corda quando eu era guri. Cadeado na porta."),
            Marcus("Tem uma tabuleta pregada: \"MUDAMOS PRO MERCADINHO — na encruzilhada, seguindo a avenida\".")
        );
    }

    private void InteragirFrentista()
    {
        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            Falar(
                Marcus("O Zé viu levarem o vô pelo portão do Centro. Hoje, perto das nove."),
                new DialogoLinha("FRENTISTA", "...Eu não sei o suficiente pra me meter nisso, Marcus. Nunca sei.", "frentista"),
                new DialogoLinha("FRENTISTA", "Vai com cuidado.", "frentista")
            );
            return;
        }

        if (!GameState.Tem(ConheceuFrentista))
        {
            GameState.Marcar(ConheceuFrentista);
            GameState.Marcar(Flags.FalouComFrentista);
            GameState.Marcar(Pista_QuerosenePosto);
            Anotar(
                "frentista_posto",
                "Frentista: o vô passou cedo",
                "PISTA",
                "O frentista me reconheceu. Viu o vô às sete e pouco, a pé, comprando querosene pro lampião. O vô disse que ia faltar luz com o temporal."
            );
            AtualizarLinhaDoTempo();

            Falar(
                new DialogoLinha("FRENTISTA", "Boa noite. Tô fechando o caixa, mas se for só gasolina...", "frentista"),
                new DialogoLinha("FRENTISTA", "Espera. Tu é o neto do Adílio. O Marcus.", "frentista"),
                Marcus("Sou eu. Faz tempo."),
                new DialogoLinha("FRENTISTA", "Seis anos, por aí. Desde aquela... desde que tu foi embora.", "frentista"),
                Marcus("O vô não atende o telefone desde cedo. De noite começou a dar ocupado. Você viu ele hoje?"),
                new DialogoLinha("FRENTISTA", "Cedo, sim. Sete e pouco, a pé, como sempre. Levou um litro de querosene pro lampião.", "frentista"),
                new DialogoLinha("FRENTISTA", "Disse que ia faltar luz com esse temporal. Depois disso não passou mais aqui.", "frentista"),
                Marcus("Meu carro afogou na baixada da entrada."),
                new DialogoLinha("FRENTISTA", "Com essa água, só amanhã. Deixa lá que ninguém mexe. Vai ver o teu avô, guri.", "frentista")
            );
            return;
        }

        if (GameState.Tem(Flags.Pista_CasaAvoCompleta) && !GameState.Tem(Flags.AchouReciboMercadinho))
        {
            Falar(
                Marcus("A casa do vô estava aberta. Café pela metade, e nem sinal dele."),
                new DialogoLinha("FRENTISTA", "Aberta? Com essa chuva?", "frentista"),
                new DialogoLinha("FRENTISTA", "Olha, eu só vi ele de manhã. Ele faz as compras no mercadinho todo fim de tarde. Se alguém viu ele depois de mim, foi lá.", "frentista")
            );
            return;
        }

        Falar(new DialogoLinha("FRENTISTA", "Sete e pouco, um litro de querosene. Foi a última vez que eu vi o teu avô.", "frentista"));
    }

    private void InteragirLivroPosto()
    {
        GameState.Marcar(Pista_QuerosenePosto);
        bool nova = Anotar(
            "livro_plantao_posto",
            "Caderno do posto: querosene às 07h40",
            "PISTA",
            "O caderno de plantão do posto registra: '07:40 — Querosene 1 L — Seu Adílio, nº 40 (pago)'. Ele se preparou para passar a noite em casa sem luz."
        );
        AtualizarLinhaDoTempo();

        GameMessageUI.Instance.ShowInspection("CADERNO DE PLANTÃO", "Bilhete",
            "TERÇA, 19/10/2004\n\n06:55  Leite e pão — D. Cida (fiado)\n07:40  Querosene 1 L — Seu Adílio, nº 40 (pago)\n12:10  Gasolina 20 L — picape da obra\n17:30  Diesel 45 L — caminhão de entrega\n19:05  Bomba 2 desligada (chuva)",
            pensamento: "Querosene pro lampião. O vô se preparou pra passar a noite em casa.",
            etiqueta: Etiqueta(nova));
    }

    private void InteragirTvPosto()
    {
        GameMessageUI.Instance.ShowInspection("TV DO BALCÃO", "TV_Old",
            "Jornal regional, imagem chiando a cada trovão:\n\n\"...temporal na serra derrubou linhas telefônicas. Interurbano instável até amanhã...\"",
            pensamento: "Por isso o celular não pega aqui.");
    }

    // =========================================================================
    // Zona 3 e interior: encruzilhada e mercadinho
    // =========================================================================

    private void InteragirPicapeEncruzilhada()
    {
        GameMessageUI.Instance.ShowInspection("PICAPE DE ENTREGA", null,
            "Picape do mercadinho, engradados vazios amarrados na caçamba.\n\nNo painel, um bloco de anotações:\n\"Qui — Bar do Zé (R. Norte), 6 engr.\"",
            pensamento: "O mercadinho entrega até no bar lá de cima.");
    }

    private void InteragirQuadroEncruzilhada()
    {
        GameMessageUI.Instance.ShowInspection("MURAL DA ENCRUZILHADA", "RoadSign_Blank",
            "AVISO DA PREFEITURA\nObra na avenida do Centro. O portão da Rua Norte fica trancado das 20h às 6h. Chave reserva no Bar do Zé.\n\nMissa de sétimo dia — sexta, 19h.\n\nVende-se fogão a lenha. Tratar no nº 22.");
    }

    private void InteragirPlacaSetaBairro()
    {
        GameMessageUI.Instance.ShowInspection("PLACA DE DIREÇÃO", "ArrowSign",
            "SUBIDA DE BARRO\nBairro das Casas — nº 10 a 50\n\nPELO ALTO DO BAIRRO\nRua Norte");
    }

    private void InteragirAvisoObra()
    {
        Falar(
            Marcus("\"AVENIDA INTERDITADA — OBRA. Pedestres para o Centro: portão da Rua Norte.\""),
            Pensa("O caminho pro Centro é pelo alto. E de noite fica trancado.")
        );
    }

    private void InteragirAtendenteMercadinho()
    {
        if (GameState.Tem(Capitulo2Director.Vitoria))
        {
            Falar(
                Marcus("Tonão guardou um recado do vô. Ainda não sei onde ele está. Se aparecer notícia, posso receber pela Rodoviária."),
                new DialogoLinha("ATENDENTE", "Está bem. Se alguém vier perguntar por ele, vou pedir que deixe o recado lá.", "atendente")
            );
            return;
        }
        GameState.Marcar(Flags.FalouComAtendenteMercadinho);

        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            Falar(
                Marcus("O Zé viu. Levaram o vô pelo portão do Centro, e o homem de capa estava junto."),
                new DialogoLinha("ATENDENTE", "Meu Deus... Eu devia ter falado alguma coisa antes. Pra alguém.", "atendente"),
                new DialogoLinha("ATENDENTE", "Se achar o seu Adílio, volta aqui e me conta. Por favor.", "atendente")
            );
            return;
        }

        if (GameState.Tem(Flags.OuviuRelatoAtendente))
        {
            Falar(new DialogoLinha("ATENDENTE", "Já te contei tudo o que eu sei. O homem de capa, a mesa dos fundos do Zé. Não vi mais nada depois que fechei, às sete.", "atendente"));
            return;
        }

        if (GameState.Tem(Flags.AchouReciboMercadinho))
        {
            // Só quem leu o aviso no verso do cupom ganha a confiança dela.
            Falar(ConfirmarTestemunhoAtendente,
                Marcus("Isto estava no chão da casa do meu avô. O aviso no verso é seu?"),
                new DialogoLinha("ATENDENTE", "...É. Fui eu que escrevi. Ele está bem?", "atendente"),
                Marcus("A casa estava aberta e ele não está lá. Quem é o homem de capa?"),
                new DialogoLinha("ATENDENTE", "Não sei o nome. Entrou aqui ontem perguntando onde o seu Adílio morava. Eu disse que não sabia.", "atendente"),
                new DialogoLinha("ATENDENTE", "Hoje à tarde voltou e perguntou de novo, a rua dessa vez. Não tirou o capuz. Aí eu avisei o seu Adílio no cupom.", "atendente"),
                Marcus("Você já tinha visto esse homem antes?"),
                new DialogoLinha("ATENDENTE", "Uma vez. Quinta-feira, quando fui levar os engradados no Bar do Zé. Ele estava na mesa dos fundos, conversando com o Zé.", "atendente"),
                new DialogoLinha("ATENDENTE", "Quando eu entrei, os dois pararam de falar.", "atendente"),
                Marcus("E o meu avô, depois das seis?"),
                new DialogoLinha("ATENDENTE", "Não vi mais. Fechei às sete.", "atendente")
            );
            return;
        }

        bool primeiraVez = !JaNoDiario("atendente_rotina_hoje");
        Anotar(
            "atendente_rotina_hoje",
            "Mercadinho: o vô passou no fim da tarde",
            "FATO",
            "A atendente disse que o vô passou no fim da tarde, levou o de sempre e foi pra casa. Hesitou quando perguntei se havia algo diferente."
        );

        if (!primeiraVez)
        {
            Falar(new DialogoLinha("ATENDENTE", "Já disse, moço. Ele passou no fim da tarde, como todo dia. Vou fechar.", "atendente"));
            return;
        }

        Falar(
            new DialogoLinha("ATENDENTE", "Boa noite. Já vou fechar.", "atendente"),
            Marcus("Sou o Marcus, neto do seu Adílio, da casa 40. Você viu ele hoje?"),
            new DialogoLinha("ATENDENTE", "Neto...? Passou aqui no fim da tarde, como todo dia. Levou o de sempre e foi pra casa.", "atendente"),
            Marcus("Notou alguma coisa diferente?"),
            new DialogoLinha("ATENDENTE", "...Não. Boa noite, moço.", "atendente"),
            Pensa("Ela pensou antes de responder. Não confia em mim.")
        );
    }

    // Só o fechamento da conversa que apresenta o cupom transmite o relato.
    public static void ConfirmarTestemunhoAtendente()
    {
        if (!GameState.Tem(Flags.AchouReciboMercadinho)) return;
        GameState.Marcar(Flags.ApresentouReciboAtendente);
        GameState.Marcar(Flags.OuviuRelatoAtendente);
        GameState.Marcar(Flags.Pista_RumoBarDoZe);
        GameState.RegistrarNotaDiario(
            "pista_relato_mercadinho",
            "O homem de capa",
            "PISTA",
            "A atendente escreveu o aviso do cupom. Um homem de capa, que não tira o capuz, perguntou ontem e hoje onde o vô morava. Ela já o tinha visto uma vez: quinta-feira, na mesa dos fundos do Bar do Zé, conversando com o Zé. Os dois pararam de falar quando ela entrou."
        );
        GameState.DefinirObjetivo("Descobrir quem é o homem de capa.");
    }

    private void InteragirCadernoMercadinho()
    {
        GameMessageUI.Instance.ShowInspection("CADERNO DE VENDAS", "Bilhete",
            "OUTUBRO\n\nQui 14 — Bar do Zé (R. Norte): 6 engradados. Entregue às 16h.\nSeg 18 — Pão, leite, fumo.\nTer 19 — Adílio (nº 40): café, velas, fósforo. Pago. 18h10.",
            pensamento: "Seis e dez. A última compra do dia.");
    }

    private void InteragirPrateleirasMercadinho()
    {
        Falar(Marcus("Café Santa Rita, o mesmo pacote amarelo que o vô deixava em cima da geladeira."));
    }

    // =========================================================================
    // Zona 4: bairro
    // =========================================================================

    private void InteragirVizinhaLurdes()
    {
        if (GameState.Tem(Pista_BatidasLurdes))
        {
            Falar(new DialogoLinha("DONA LURDES", "Já disse o que eu ouvi, menino. Oito e meia, três batidas. Vai com Deus.", "lurdes"));
            return;
        }

        GameState.Marcar(Pista_BatidasLurdes);
        Anotar(
            "vizinha_lurdes_relato",
            "Dona Lurdes: batidas às 20h30",
            "PISTA",
            "Dona Lurdes (nº 14) ouviu três batidas fortes na porta da casa 40 por volta das oito e meia. Depois a chuva apertou e ela não ouviu mais nada. Não viu quem era."
        );
        AtualizarLinhaDoTempo();

        Falar(
            new DialogoLinha("DONA LURDES", "Quem é? Eu não abro porta de noite.", "lurdes"),
            Marcus("Sou o Marcus, neto do seu Adílio, da casa 40."),
            new DialogoLinha("DONA LURDES", "O neto... Ai, menino. Bateram lá na casa dele hoje. Umas oito e meia, que eu olhei o relógio da cozinha.", "lurdes"),
            new DialogoLinha("DONA LURDES", "Três batidas, forte, de quem não tem paciência. Depois a chuva apertou e eu não ouvi mais nada.", "lurdes"),
            Marcus("A senhora viu quem era?"),
            new DialogoLinha("DONA LURDES", "Eu não abro a cortina de noite, meu filho. Nem a cortina, nem a porta.", "lurdes")
        );
    }

    private void InteragirMarcasBarro()
    {
        GameState.Marcar(Pista_PneusBairro);
        bool nova = Anotar(
            "pista_pneus_bairro",
            "Pneus de carro pesado no barro",
            "PISTA",
            "Na esquina do nº 22, marcas fundas de pneu largo, ainda com água parada dentro. Um carro pesado subiu a rua do vô hoje à noite."
        );

        GameMessageUI.Instance.ShowInspection("MARCAS NO BARRO", null,
            "Marcas de pneu largo, fundas, ainda com água parada dentro. A chuva não teve tempo de apagar.\n\nO rastro sobe a rua, na direção da casa do vô.",
            pensamento: "Carro pesado. E passou aqui hoje à noite.",
            etiqueta: Etiqueta(nova));
    }

    private void InteragirMoradorBairro()
    {
        if (GameState.Tem(Pista_FarolRamiro))
        {
            Falar(new DialogoLinha("SEU RAMIRO", "Farol parado na frente da casa, uns dez minutos, e depois o motor subindo a ladeira. Foi o que eu vi.", "ramiro"));
            return;
        }

        GameState.Marcar(Pista_FarolRamiro);
        Anotar(
            "ramiro_farol",
            "Seu Ramiro: farol parado às 20h40",
            "PISTA",
            "Seu Ramiro, vizinho do vô, viu um farol parado na frente da casa 40 lá pelas nove menos vinte, por uns dez minutos. Depois ouviu o motor forçando na ladeira, subindo. Não viu ninguém."
        );
        AtualizarLinhaDoTempo();

        Falar(
            new DialogoLinha("VIZINHO", "Boa noite. Tu não é daqui... ou é?", "ramiro"),
            Marcus("Marcus. Neto do Adílio, da casa 40."),
            new DialogoLinha("SEU RAMIRO", "Ramiro. Sou vizinho do teu avô faz trinta anos. Ele fala de ti.", "ramiro"),
            Marcus("O senhor viu alguém na casa dele hoje?"),
            new DialogoLinha("SEU RAMIRO", "Gente não vi. Eu estava no galpão, recolhendo a lenha. Lá pelas nove menos vinte vi um farol parado na frente da casa dele. Coisa de dez minutos.", "ramiro"),
            new DialogoLinha("SEU RAMIRO", "Depois ouvi o motor forçando na ladeira, subindo. Teu avô não recebe visita de noite.", "ramiro")
        );
    }

    private void InteragirCaixaCorreioAvo()
    {
        Falar(Marcus("\"ADÍLIO — Nº 40\", pintado à mão. É aqui."));
    }

    // =========================================================================
    // Interior: casa de Adílio
    // =========================================================================

    private void InteragirPistaCafe()
    {
        GameState.Marcar(Flags.Pista_CafeAindaQuente);
        bool nova = Anotar(
            "pista_cafe_hoje",
            "Café pela metade, ainda morno",
            "PISTA",
            "A xícara está pela metade e o bule ainda guarda calor. O vô nunca deixa café na xícara."
        );

        GameMessageUI.Instance.ShowInspection("BULE E XÍCARA", "Interior_PiaAvo",
            "Xícara pela metade, ainda morna por fora. O bule também guarda um resto de calor.",
            VerificarConclusaoPistasCasaAvo,
            pensamento: "O vô nunca deixa café pela metade. Ele estava aqui há pouco tempo.",
            etiqueta: Etiqueta(nova));
    }

    private void InteragirPistaCartas()
    {
        GameState.Marcar(Flags.Pista_CartasMesa);
        bool nova = Anotar(
            "pista_cartas_mesa",
            "Paciência largada no meio",
            "PISTA",
            "A paciência ficou no meio, com um ás fora do monte. O vô sempre termina o jogo antes de levantar."
        );

        GameMessageUI.Instance.ShowInspection("PACIÊNCIA NA MESA", "Interior_MesaCartas",
            "Partida de paciência no meio. Um ás largado fora do monte, virado pra cima.",
            VerificarConclusaoPistasCasaAvo,
            pensamento: "Ele sempre termina a paciência antes de levantar. Alguma coisa fez ele largar o jogo.",
            etiqueta: Etiqueta(nova));
    }

    private void InteragirPistaNotaFiscal()
    {
        GameState.Marcar(Flags.Pista_NotaFiscal2004);
        bool nova = Anotar(
            "pista_nota_2004",
            "Cupom com um aviso no verso",
            "PISTA",
            "Cupom do Mercadinho Santa Rita, hoje às 18h10: café, velas e fósforo. No verso, a lápis, letra de mulher: 'Seu Adílio, aquele homem de capa voltou a perguntar onde o senhor mora. Não falei nada. Cuidado.'"
        );
        AtualizarLinhaDoTempo();

        GameMessageUI.Instance.ShowInspection("CUPOM AMASSADO", "Bilhete",
            "MERCADINHO SANTA RITA\n19/10/2004 — 18:10\nCafé torrado 500 g\nVelas (maço)\nFósforo\nTOTAL R$ 7,40\n\nNo verso, a lápis:\n\"Seu Adílio, aquele homem de capa voltou a perguntar onde o senhor mora. Não falei nada. Cuidado.\"",
            VerificarConclusaoPistasCasaAvo,
            pensamento: "Letra de mulher. Alguém no mercadinho tentou avisar o vô.",
            etiqueta: Etiqueta(nova));
    }

    private void InteragirPistaDesordem()
    {
        GameState.Marcar(Flags.Pista_DesordemPorta);
        bool nova = Anotar(
            "pista_desordem_porta",
            "Pegadas de bota e casaco no prego",
            "PISTA",
            "Perto da porta: banco tombado e barro de dois pares de bota de sola grossa, maiores que o chinelo do vô. Entram e saem. O casaco de chuva dele continua no prego."
        );

        GameMessageUI.Instance.ShowInspection("PERTO DA PORTA", null,
            "O banco está tombado. No piso, barro molhado: dois pares de pegadas de bota, sola grossa, maiores que o chinelo do vô. Vão da porta até aqui e voltam.\n\nO casaco de chuva dele continua no prego.",
            VerificarConclusaoPistasCasaAvo,
            pensamento: "Ninguém sai nesse temporal sem casaco. A não ser que não tenha tido tempo de pegar.",
            etiqueta: Etiqueta(nova));
    }

    private void InteragirRetratoFamilia()
    {
        GameMessageUI.Instance.ShowInspection("PORTA-RETRATO", "FramedPhoto",
            "Natal de 1997. O vô, a mãe e eu na varanda desta casa.\n\nAtrás, a letra dele: \"Que o guri volte.\"",
            pensamento: "Fui embora no ano seguinte. Não voltei.");
    }

    private void InteragirTelefoneCasaAvo()
    {
        GameState.Marcar(Flags.CasaAvo_OlhouTelefone);
        bool nova = Anotar(
            "telefone_casa_adilio",
            "Fone fora do gancho",
            "PISTA",
            "O fone da sala está pendurado pelo fio. Na estrada, a ligação começou a dar ocupado por volta das oito e meia: foi quando o fone caiu."
        );
        AtualizarLinhaDoTempo();

        GameMessageUI.Instance.ShowInspection("TELEFONE DA SALA", "TelefoneForaDoGancho",
            "O fone está pendurado pelo fio, quase no chão. O tom de ocupado sai baixinho do aparelho.",
            VerificarConclusaoPistasCasaAvo,
            pensamento: "Começou a dar ocupado lá pelas oito e meia, quando eu ainda estava na estrada. Foi a essa hora que o fone caiu.",
            etiqueta: Etiqueta(nova));
    }

    public static void VerificarConclusaoPistasCasaAvo()
    {
        int cont = 0;
        if (GameState.Tem(Flags.Pista_CafeAindaQuente)) cont++;
        if (GameState.Tem(Flags.Pista_CartasMesa)) cont++;
        if (GameState.Tem(Flags.Pista_DesordemPorta)) cont++;
        if (GameState.Tem(Flags.CasaAvo_OlhouTelefone)) cont++;
        if (GameState.Tem(Flags.Pista_NotaFiscal2004) || GameState.Tem(Flags.CasaAvo_LeuBilhete)) cont++;

        if (cont >= 3 && !GameState.Tem(Flags.Pista_CasaAvoCompleta))
        {
            GameState.Marcar(Flags.Pista_CasaAvoCompleta);
            GameState.RegistrarNotaDiario(
                "hipotese_casa_adilio",
                "A noite do vô foi interrompida",
                "HIPÓTESE",
                "Café pela metade, paciência largada, banco caído, fone fora do gancho. Ele estava em casa, sem pressa, e alguma coisa interrompeu a noite. Ainda não sei o quê, nem quem."
            );
        }

        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca) || GameState.Tem(Flags.OuviuRelatoAtendente)) return;

        if (GameState.Tem(Flags.Pista_NotaFiscal2004))
            GameState.DefinirObjetivo("Descobrir quem escreveu o aviso no verso do cupom.");
        else if (GameState.Tem(Flags.Pista_CasaAvoCompleta))
            GameState.DefinirObjetivo("Descobrir o que interrompeu a noite do vô.");
    }

    // Junta os horários que o jogador já descobriu. Só aparece com dois ou mais.
    public static void AtualizarLinhaDoTempo()
    {
        var linhas = new List<string>();
        if (GameState.Tem(Pista_QuerosenePosto))
            linhas.Add("07h40 — Compra querosene no posto, a pé.");
        if (GameState.Tem(Flags.Pista_NotaFiscal2004))
            linhas.Add("18h10 — Compra café e velas no mercadinho. Alguém o avisa sobre o homem de capa.");
        if (GameState.Tem(Pista_BatidasLurdes))
            linhas.Add("~20h30 — Três batidas fortes na porta da casa 40 (Dona Lurdes).");
        if (GameState.Tem(Flags.CasaAvo_OlhouTelefone))
            linhas.Add("~20h30 — O telefone começa a dar ocupado: o fone caiu do gancho.");
        if (GameState.Tem(Pista_FarolRamiro))
            linhas.Add("~20h40 — Farol parado na frente da casa por dez minutos; o motor sobe a ladeira (Seu Ramiro).");
        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca))
            linhas.Add("~20h50 — Zé vê dois homens levarem o vô pelo portão do Centro.");
        else if (GameState.Tem(Pista_FundosTrancados))
            linhas.Add("~21h — Zé tranca a porta dos fundos do bar (freguês).");

        if (linhas.Count < 2) return;

        GameState.RegistrarNotaDiario(
            "linha_do_tempo_adilio",
            "Linha do tempo do dia do vô",
            "HIPÓTESE",
            string.Join("\n", linhas)
        );
    }

    // =========================================================================
    // Zona 5: Rua Norte e portão do Centro
    // =========================================================================

    private void InteragirCaminhoneteBar()
    {
        Falar(
            Marcus("Uma caminhonete velha com engradados vazios na caçamba. Deve ser do bar."),
            Marcus("O chão embaixo dela está seco. Ela não saiu daqui desde antes da chuva.")
        );
    }

    private void InteragirPlacaBar()
    {
        Falar(Marcus("\"BAR DO ZÉ — SINUCA E BEBIDAS\". Luz amarela lá dentro e o estalo seco das bolas."));
    }

    private void EntrarNoBar()
    {
        if (Capitulo1Director.Instance != null)
        {
            Capitulo1Director.Instance.TransicionarPara(
                Capitulo1Director.ZonaId.Interior_BarDoZe,
                new Vector2(509.0f, -57.2f),
                1
            );
        }
        else
        {
            SceneTransition.Ir("BarDoZe", "entrada_bar");
        }
    }

    private void InteragirLimiarCentro()
    {
        if (!GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            GameState.Marcar(Pista_RastroPortao);
            Anotar(
                "pista_rastro_portao",
                "Rastro de pneu atravessa o portão do Centro",
                "PISTA",
                "O portão da Rua Norte tem corrente nova e cadeado da prefeitura. Marcas de pneu largo passam rente a ele e seguem do outro lado. Alguém abriu esse portão hoje à noite."
            );
            Falar(
                Marcus("Portão de ferro. Corrente nova, cadeado da prefeitura."),
                Marcus("No barro, marcas de pneu largo passam rente ao portão e continuam do outro lado."),
                Pensa("Alguém abriu isso hoje à noite. E fechou de novo.")
            );
            return;
        }

        // Depois do capítulo 2 começar, ou depois da tela de fim, o portão é só passagem.
        if (GameState.Tem("c2_iniciado") || GameState.Tem(Flags.Capitulo1Concluido))
        {
            Falar(Capitulo1Director.MostrarTelaFechamentoCapitulo1, Marcus("O cadeado continua aberto."));
            return;
        }

        Falar(Capitulo1Director.MostrarPainelFimCapitulo1,
            Marcus("O cadeado está aberto. Do outro lado, a avenida do Centro some na neblina."),
            Marcus("As marcas de pneu seguem por ali.")
        );
    }

    // =========================================================================
    // Interior: Bar do Zé
    // =========================================================================

    private void InteragirBalcaoBar()
    {
        GameState.Marcar(Pista_FotoSalao1990);
        bool nova = Anotar(
            "pista_foto_salao_1990",
            "Foto do salão do Clube, 1990",
            "PISTA",
            "Atrás do balcão do Zé, uma foto do salão do Clube lotado, faixa 'NOITE DE JOGOS — 1990'. Zé aparece de avental, servindo."
        );

        GameMessageUI.Instance.ShowInspection("ATRÁS DO BALCÃO", "FramedPhoto",
            "Entre as garrafas, uma foto desbotada: o salão do Clube lotado, faixa \"NOITE DE JOGOS — 1990\". No canto, Zé de avental, servindo.\n\nEmbaixo, um caderno de fiado com metade dos nomes riscados.",
            pensamento: "Noventa. Eu estava nessa noite.",
            etiqueta: Etiqueta(nova));
    }

    private void InteragirQuadroRegras()
    {
        GameMessageUI.Instance.ShowInspection("REGRAS DA CASA", "Quadro_Regras_Sinuca",
            "MELHOR DE TRÊS.\n\nSete bolas na mesa. Quem fizer quatro leva a mão.\n\nDa segunda mão em diante, o dono marca uma bola com giz [X]: ESSA FICA NA MESA.\n\nDerrubou a marcada: a tacada não vale, as bolas voltam e a branca vai pra mão do outro.");
    }

    private void InteragirFrequentadorBar()
    {
        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            Falar(new DialogoLinha("FREGUÊS", "Ninguém ganha do Zé na mesa dele. Vai com cuidado lá fora, guri.", "frequentador"));
            return;
        }

        var linhas = new List<DialogoLinha>();
        if (!GameState.Tem(Pista_FundosTrancados))
        {
            GameState.Marcar(Pista_FundosTrancados);
            Anotar(
                "frequentador_fundos",
                "Zé trancou os fundos por volta das 21h",
                "PISTA",
                "Um freguês do bar contou que, lá pelas nove, o Zé foi nos fundos, voltou branco e passou a tranca na porta de trás com o bar aberto. Nunca tinha visto ele fazer isso."
            );
            AtualizarLinhaDoTempo();

            linhas.Add(new DialogoLinha("FREGUÊS", "Tu é o neto do Adílio? Tem a cara dele.", "frequentador"));
            linhas.Add(Marcus("Viu meu avô hoje?"));
            linhas.Add(new DialogoLinha("FREGUÊS", "Aqui não. Mas olha... lá pelas nove o Zé foi nos fundos ver não sei o quê.", "frequentador"));
            linhas.Add(new DialogoLinha("FREGUÊS", "Voltou branco feito cal e passou a tranca na porta de trás. Com o bar aberto. Nunca vi ele fazer isso.", "frequentador"));
            linhas.Add(new DialogoLinha("FREGUÊS", "Não diz que fui eu que falei.", "frequentador"));
        }
        else if (!GameState.Tem(Flags.FalouComZeAntesDaSinuca))
        {
            linhas.Add(new DialogoLinha("FREGUÊS", "Fala baixo. O Zé tá olhando pra cá.", "frequentador"));
        }

        if (GameState.Tem(Flags.FalouComZeAntesDaSinuca))
        {
            linhas.Add(new DialogoLinha("FREGUÊS", "Vai jogar com ele? Cuidado na segunda mão. Ele marca uma bola com giz.", "frequentador"));
            linhas.Add(new DialogoLinha("FREGUÊS", "Às vezes vale mais deixar a branca difícil pra ele do que arriscar perto da marcada.", "frequentador"));
        }

        Falar(linhas.ToArray());
    }

    private void InteragirMesaSinuca()
    {
        if (!GameState.Tem(Flags.ZeDerrotadoNaSinuca) && GameState.Tem(Flags.FalouComZeAntesDaSinuca))
        {
            InteragirZe();
            return;
        }

        Falar(Marcus("Feltro remendado, tabela gasta. O Zé não tira o olho de mim."));
    }

    private void InteragirZe()
    {
        if (GameState.Tem("c2_registro_manutencao"))
        {
            Falar(
                Marcus("A cópia de manutenção registra outras tentativas. O padre ouviu Júlia responder."),
                new DialogoLinha("ZÉ", "Eu queria o salão funcionando. Não vou dizer que vi cada tentativa. Sobre teu avô, contei a passagem pelo portão; não sei o que aconteceu depois dali.", "ze")
            );
            return;
        }
        GameState.Marcar(Flags.EntrouNoBarDoZe);

        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            Falar(
                new DialogoLinha("ZÉ", "Já te disse o que eu vi. O portão tá aberto.", "ze"),
                new DialogoLinha("ZÉ", "Se tu tem juízo, usa o orelhão lá fora antes de atravessar.", "ze")
            );
            return;
        }

        if (GameState.Tem(Flags.FalouComZeAntesDaSinuca))
        {
            Falar(IniciarPartidaSinucaComZe, new DialogoLinha("ZÉ", "A mesa tá pronta. Pega o taco.", "ze"));
            return;
        }

        if (!GameState.Tem(Flags.OuviuRelatoAtendente))
        {
            ZeDesconversa();
            return;
        }

        // Marcus agora tem algo concreto: o homem de capa sentou na mesa do Zé.
        GameState.Marcar(Flags.FalouComZeAntesDaSinuca);
        Anotar(
            "confronto_ze_bar",
            "Zé aceitou falar, do jeito dele",
            "FATO",
            "Quando citei o homem de capa, Zé não negou. Disse que só conta o que viu hoje se eu ganhar dele na sinuca, melhor de três."
        );
        GameState.DefinirObjetivo("Vencer Zé na sinuca (melhor de três).");

        var linhas = new List<DialogoLinha>();
        if (!GameState.Tem(ZeDesconversou))
            linhas.Add(new DialogoLinha("ZÉ", "Olha quem a chuva trouxe. O neto do Adílio.", "ze"));
        linhas.Add(Marcus("Quinta-feira um homem de capa sentou aqui com você. Hoje ele andou perguntando onde o vô morava."));
        linhas.Add(new DialogoLinha("ZÉ", "Muita gente senta na minha mesa.", "ze"));
        if (GameState.Tem(Pista_FundosTrancados))
        {
            linhas.Add(Marcus("E hoje, lá pelas nove, você trancou os fundos com o bar aberto."));
            linhas.Add(new DialogoLinha("ZÉ", "...Tu andou conversando bastante pra quem acabou de chegar.", "ze"));
        }
        linhas.Add(Marcus("Meu avô sumiu hoje à noite. Eu não saio daqui sem resposta."));
        linhas.Add(new DialogoLinha("ZÉ", "Então pega um taco. Aqui dentro a conversa é do meu jeito.", "ze"));
        linhas.Add(new DialogoLinha("ZÉ", "Melhor de três. Ganhou, eu te conto o que eu vi hoje. Perdeu, tu pega teu carro e volta pra capital.", "ze"));
        linhas.Add(Marcus("O carro afogou na entrada."));
        linhas.Add(new DialogoLinha("ZÉ", "Então volta a pé.", "ze"));
        linhas.Add(new DialogoLinha("ZÉ", "Sete bolas, quatro pontos levam a mão. Da segunda em diante eu marco uma com giz: essa fica na mesa.", "ze"));
        linhas.Add(new DialogoLinha("ZÉ", "Derrubou a marcada, a tacada não vale, as bolas voltam e a branca vai pra mão do outro. Vale pra mim também.", "ze"));

        Falar(IniciarPartidaSinucaComZe, linhas.ToArray());
    }

    private void ZeDesconversa()
    {
        if (GameState.Tem(ZeDesconversou))
        {
            if (GameState.Tem(Pista_FundosTrancados))
            {
                Falar(
                    Marcus("Lá pelas nove você trancou os fundos com o bar aberto. O que viu lá fora?"),
                    new DialogoLinha("ZÉ", "Tranquei porque chove pra dentro. Próxima pergunta.", "ze"),
                    Pensa("Não basta. Preciso saber quem andou rondando o vô antes de hoje.")
                );
                return;
            }

            Falar(new DialogoLinha("ZÉ", "Já te falei. Bebe ou vai embora.", "ze"));
            return;
        }

        GameState.Marcar(ZeDesconversou);
        Anotar(
            "ze_desconversou",
            "Zé desconversou",
            "FATO",
            "Contei ao Zé que o vô sumiu. Ele não perguntou o que aconteceu. Só mandou eu beber ou ir embora."
        );

        Falar(
            new DialogoLinha("ZÉ", "Olha quem a chuva trouxe. O neto do Adílio.", "ze"),
            Marcus("O vô sumiu. A casa está aberta, o café pela metade. Você viu ele hoje?"),
            new DialogoLinha("ZÉ", "Aqui é bar, guri. Eu vejo copo e mesa. Quer beber, senta. Quer delegado, é no Centro, e o Centro tá fechado.", "ze"),
            Pensa("Ele nem perguntou o que aconteceu. Preciso de alguma coisa concreta antes de insistir.")
        );
    }

    private void IniciarPartidaSinucaComZe()
    {
        SinucaMinigameUI.Instance.IniciarDuelo(
            aoVencer: AoVencerZeNaSinuca,
            aoPerder: () =>
            {
                Legenda.Dizer("Zé recolhe as bolas. A conversa acabou, por enquanto.", 3.5f, Legenda.PrioridadeCritica);
            }
        );
    }

    private void AoVencerZeNaSinuca()
    {
        GameState.Marcar(Flags.ZeDerrotadoNaSinuca);
        GameState.Marcar(Flags.RecebeuFichaClubeZe);
        GameState.Marcar(Flags.Pista_AdilioNaoSaiuVoluntariamente);
        GameState.Marcar(Flags.LimiarCentroDesbloqueado);

        GameState.RegistrarNotaDiario(
            "vitoria_ze_ficha_clube",
            "Zé viu levarem o vô",
            "PISTA",
            "Lá pelas 20h50, da porta dos fundos do bar, Zé viu uma caminhonete parar no portão do Centro. Dois homens tiraram o vô da cabine e o levaram pelo portão; um deles abriu o cadeado com chave. Zé: 'Ele não queria ir.' Depois trancou a porta dos fundos."
        );
        GameState.RegistrarNotaDiario(
            "pista_relato_mercadinho",
            "O homem de capa",
            "PISTA",
            "Perguntou ontem e hoje no mercadinho onde o vô morava. Na quinta, sentou com o Zé, quis saber quem ainda está vivo da diretoria do Clube de 1990 e perguntou do vô. Hoje estava entre os que levaram o vô pelo portão do Centro."
        );
        GameState.RegistrarNotaDiario(
            "ze_salao_1990",
            "O que o Zé sabia em 1990",
            "FATO",
            "Zé admitiu que a diretoria sabia que o piso do salão estava podre sobre a cisterna. Consertar era fechar o salão. Naquela noite de 1990, ele mandou a música continuar."
        );
        GameState.RegistrarNotaDiario(
            Flags.FichaZeObtida,
            "Ficha do salão #01 (Zé)",
            "ITEM",
            "Ficha de latão do salão do Clube, número 01 riscado à mão. Zé me deu depois da sinuca. Não abre porta nenhuma: mostra a quem é do Clube que eu sentei na mesa dele."
        );
        AtualizarLinhaDoTempo();

        GameState.DefinirObjetivo("Atravessar o portão do Centro. Antes, posso salvar no orelhão da Rua Norte.");

        Falar(() =>
        {
            Legenda.Dizer("Recebido: Ficha do salão. O portão do Centro está destrancado.", 5f, Legenda.PrioridadeCritica);
            Capitulo1Director.AtualizarEstadoPortaoCentroNaCena();
        },
            new DialogoLinha("ZÉ", "...Tu segura o taco igual o teu avô. Palavra dada na mesa não volta.", "ze"),
            Marcus("Então fala. O que você viu?"),
            new DialogoLinha("ZÉ", "Fui nos fundos lá pelas nove menos dez. Chuva entrando pela porta. Vi uma caminhonete parar no portão do Centro, farol baixo.", "ze"),
            new DialogoLinha("ZÉ", "Dois homens tiraram o teu avô da cabine. Um deles abriu o cadeado com chave. Eu vi levarem ele. Ele não queria ir.", "ze"),
            Marcus("E você trancou a porta."),
            new DialogoLinha("ZÉ", "Tranquei.", "ze"),
            Marcus("Quem eram?"),
            new DialogoLinha("ZÉ", "Um eu nunca vi. O outro, de capa, sentou nessa mesa quinta-feira. Quis saber quem ainda está vivo da diretoria do Clube de noventa. E perguntou do Adílio.", "ze"),
            Marcus("Por que alguém ia querer saber de noventa?"),
            new DialogoLinha("ZÉ", "Porque tem gente que não esquece.", "ze"),
            new DialogoLinha("ZÉ", "O piso do salão estava podre fazia dois invernos. Infiltração, a cisterna embaixo. A diretoria sabia. Eu sabia.", "ze"),
            new DialogoLinha("ZÉ", "Arrumar era fechar o salão. Fechar o salão era matar a única coisa que ainda juntava esta cidade. Então, naquela noite, eu mandei a música continuar.", "ze"),
            new DialogoLinha("ZÉ", "Uma cidade inteira não pode acabar por causa de uma decisão, guri. ...Tu estava lá também. Não me olha assim.", "ze"),
            Marcus("..."),
            new DialogoLinha("ZÉ", "Pega. Ficha do salão. Quem for do Clube vai saber que tu sentou na minha mesa.", "ze"),
            new DialogoLinha("ZÉ", "A chave do portão fica aqui desde a obra. Vou abrir pra ti. O que tem do outro lado, eu não sei. Isso é tudo que eu vi.", "ze")
        );
    }
}
