using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

            switch (tipo)
            {
                case TipoInteracao.CarroMarcus:
                    return "Examinar meu carro";
                case TipoInteracao.PlacaRodoviaEntrada:
                    return "Ler quadro da parada de ônibus";
                case TipoInteracao.MalaAbandonadaEstrada:
                    return "Examinar mala caída no barro";
                case TipoInteracao.BombaCombustivelPosto:
                    return "Examinar bomba de combustível";
                case TipoInteracao.CarroEstacionadoPosto:
                    return "Examinar picape no posto";
                case TipoInteracao.VendaAntigaFechada:
                    return "Examinar antiga Venda";
                case TipoInteracao.NpcFrentista:
                    return "Conversar com o Frentista";
                case TipoInteracao.LivroRegistroPosto:
                    return "Ler livro de plantão do Posto";
                case TipoInteracao.TvPortatilPosto:
                    return "Olhar TV portátil";
                case TipoInteracao.PicapeEncruzilhada:
                    return "Examinar caminhonete no meio-fio";
                case TipoInteracao.QuadroAvisosEncruzilhada:
                    return "Ler quadro de avisos da comunidade";
                case TipoInteracao.PlacaSetaBairro:
                    return "Ler placa de direção";
                case TipoInteracao.NpcAtendenteMercadinho:
                    return "Conversar com a Atendente";
                case TipoInteracao.CadernoFiadoMercadinho:
                    return "Ler caderno de entregas no balcão";
                case TipoInteracao.PrateleirasMercadinho:
                    return "Examinar prateleiras do Mercadinho";
                case TipoInteracao.VizinhaDonaLurdes:
                    return "Bater na porta (Casa nº 14)";
                case TipoInteracao.GalpaoBairroPista:
                    return "Examinar rastros na lama (Casa nº 22)";
                case TipoInteracao.NpcMoradorBairro:
                    return "Conversar com Seu Ramiro";
                case TipoInteracao.CaixaCorreioAvo:
                    return "Examinar caixa de correio (Nº 40)";
                case TipoInteracao.PistaCafeCasaAvo:
                    return "Examinar bule e xícara de café";
                case TipoInteracao.PistaCartasCasaAvo:
                    return "Examinar cartas na mesa";
                case TipoInteracao.PistaNotaFiscal2004:
                    return "Ler nota fiscal (2004) e bilhete";
                case TipoInteracao.PistaDesordemPorta:
                    return "Examinar banco caído e pegadas";
                case TipoInteracao.RetratoFamiliaCasaAvo:
                    return "Examinar retrato de família (1998)";
                case TipoInteracao.TelefoneCasaAvo:
                    return "Examinar telefone fixo";
                case TipoInteracao.CaminhoneteClubeBar:
                    return "Examinar caminhonete escura";
                case TipoInteracao.PlacaBarDoZe:
                    return "Ler letreiro do Bar do Zé";
                case TipoInteracao.NpcZeBar:
                    return GameState.Tem(Flags.ZeDerrotadoNaSinuca) ? "Falar com Zé" : "Desafiar Zé";
                case TipoInteracao.MesaSinucaBar:
                    return GameState.Tem(Flags.ZeDerrotadoNaSinuca) ? "Examinar mesa de sinuca" : "Jogar Sinuca contra Zé";
                case TipoInteracao.BalcaoBarDoZe:
                    return "Examinar balcão do bar";
                case TipoInteracao.QuadroRegrasSinuca:
                    return "Ler regras da mesa";
                case TipoInteracao.LimiarCentro:
                    return GameState.Tem(Flags.ZeDerrotadoNaSinuca) ? "Avançar para o Centro" : "Examinar portão do Centro";
                case TipoInteracao.CentroBloqueioObra:
                    return "Ler interdição da avenida";
                case TipoInteracao.PortaEntradaBarDoZe:
                    return "Entrar no Bar do Zé";
                case TipoInteracao.NpcFrequentadorBar:
                    return "Conversar com o Frequentador";
                default:
                    return "Examinar";
            }
        }
    }

    public override void Interact()
    {
        switch (tipo)
        {
            case TipoInteracao.CarroMarcus:
                InteragirCarroMarcus();
                break;
            case TipoInteracao.PlacaRodoviaEntrada:
                InteragirPlacaRodovia();
                break;
            case TipoInteracao.MalaAbandonadaEstrada:
                InteragirMalaAbandonada();
                break;
            case TipoInteracao.BombaCombustivelPosto:
                InteragirBombaPosto();
                break;
            case TipoInteracao.CarroEstacionadoPosto:
                InteragirCarroPosto();
                break;
            case TipoInteracao.VendaAntigaFechada:
                InteragirVendaAntiga();
                break;
            case TipoInteracao.NpcFrentista:
                InteragirFrentista();
                break;
            case TipoInteracao.LivroRegistroPosto:
                InteragirLivroPosto();
                break;
            case TipoInteracao.TvPortatilPosto:
                InteragirTvPosto();
                break;
            case TipoInteracao.PicapeEncruzilhada:
                InteragirPicapeEncruzilhada();
                break;
            case TipoInteracao.QuadroAvisosEncruzilhada:
                InteragirQuadroEncruzilhada();
                break;
            case TipoInteracao.PlacaSetaBairro:
                InteragirPlacaSetaBairro();
                break;
            case TipoInteracao.NpcAtendenteMercadinho:
                InteragirAtendenteMercadinho();
                break;
            case TipoInteracao.CadernoFiadoMercadinho:
                InteragirCadernoMercadinho();
                break;
            case TipoInteracao.PrateleirasMercadinho:
                InteragirPrateleirasMercadinho();
                break;
            case TipoInteracao.VizinhaDonaLurdes:
                InteragirVizinhaLurdes();
                break;
            case TipoInteracao.GalpaoBairroPista:
                InteragirGalpaoBairro();
                break;
            case TipoInteracao.NpcMoradorBairro:
                InteragirMoradorBairro();
                break;
            case TipoInteracao.CaixaCorreioAvo:
                InteragirCaixaCorreioAvo();
                break;
            case TipoInteracao.PistaCafeCasaAvo:
                InteragirPistaCafe();
                break;
            case TipoInteracao.PistaCartasCasaAvo:
                InteragirPistaCartas();
                break;
            case TipoInteracao.PistaNotaFiscal2004:
                InteragirPistaNotaFiscal();
                break;
            case TipoInteracao.PistaDesordemPorta:
                InteragirPistaDesordem();
                break;
            case TipoInteracao.RetratoFamiliaCasaAvo:
                InteragirRetratoFamilia();
                break;
            case TipoInteracao.TelefoneCasaAvo:
                InteragirTelefoneCasaAvo();
                break;
            case TipoInteracao.CaminhoneteClubeBar:
                InteragirCaminhoneteClube();
                break;
            case TipoInteracao.PlacaBarDoZe:
                InteragirPlacaBar();
                break;
            case TipoInteracao.NpcZeBar:
            case TipoInteracao.MesaSinucaBar:
                InteragirZeOuSinuca();
                break;
            case TipoInteracao.BalcaoBarDoZe:
                InteragirBalcaoBar();
                break;
            case TipoInteracao.QuadroRegrasSinuca:
                InteragirQuadroRegras();
                break;
            case TipoInteracao.LimiarCentro:
                InteragirLimiarCentro();
                break;
            case TipoInteracao.CentroBloqueioObra:
                GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
                {
                    new DialogoLinha("MARCUS", "A avenida para o Centro está interditada para reparo do pavimento. A placa orienta o desvio pela rua de barro do Bairro das Casas.", "marcus"),
                    new DialogoLinha("MARCUS", "Preciso chegar à casa do vô primeiro. Depois posso seguir pela Rua Norte.", "marcus")
                });
                break;
            case TipoInteracao.PortaEntradaBarDoZe:
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
                break;
            case TipoInteracao.NpcFrequentadorBar:
                InteragirFrequentadorBar();
                break;
        }
    }

    private void InteragirCarroMarcus()
    {
        bool jaViuPortaLuvas = GameState.Tem(Flags.MapaDoCarroObtido);
        GameState.Marcar(Flags.PrologoAbriuPortaLuvas);
        GameState.Marcar(Flags.MapaDoCarroObtido);
        GameState.Marcar(Flags.LanternaDoCarroObtida);
        SantaRitaSound.Play("pickup", .13f);
        GameState.RegistrarNotaDiario(
            "porta_luvas_endereco",
            "Endereço de Adílio (Porta-luvas)",
            "PISTA",
            "Mapa dobrado no porta-luvas com a letra do avô Adílio: 'Bairro das Casas, nº 40 (seguindo o asfalto até a encruzilhada e subindo a rua de barro)'."
        );

        var linhas = new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "Meu carro. Estacionei no acostamento logo na entrada de Santa Rita; o motor ainda estala de quente na chuva.", "marcus"),
            new DialogoLinha("MARCUS", jaViuPortaLuvas
                ? "O mapa e a lanterna já estão comigo. Bairro das Casas, nº 40."
                : "Peguei o mapa dobrado no porta-luvas e a lanterna no banco. Bairro das Casas, nº 40. Posso abrir o mapa com M.", "marcus")
        };
        GameMessageUI.Instance.ShowDialogue(linhas);
    }

    private void InteragirPlacaRodovia()
    {
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "Um abrigo de ônibus velho na entrada da cidade. O cartaz da prefeitura avisa: 'Linhas noturnas suspensas. Em caso de queda de linha telefônica no Bairro das Casas, utilize os orelhões públicos da avenida.'", "marcus"),
            new DialogoLinha("MARCUS", "À direita na avenida já dá para ver a claridade do Posto de Gasolina.", "marcus")
        });
    }

    private void InteragirMalaAbandonada()
    {
        GameState.RegistrarNotaDiario(
            "mala_estrada_1998",
            "Mala abandonada na beira da rodovia",
            "PISTA",
            "Uma mala caída no barro da entrada continha um recorte plastificado de outubro de 1998 sobre o silêncio em torno da noite das 03:17."
        );

        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "Uma mala velha caída no barro da sarjeta, como se alguém tivesse saído da cidade às pressas.", "marcus"),
            new DialogoLinha("MARCUS", "Dentro há um recorte plastificado do jornal de Santa Rita de 1998: 'Relógio da praça parou às 03:17 — diretoria do Clube nega incidentes'.", "marcus")
        });
    }

    private void InteragirBombaPosto()
    {
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "O marcador mecânico da bomba parou em 45 litros. O bico ainda cheira a diesel fresco.", "marcus"),
            new DialogoLinha("MARCUS", "A luz da loja de conveniência do posto está acesa. O frentista deve estar lá dentro se abrigando da chuva.", "marcus")
        });
    }

    private void InteragirCarroPosto()
    {
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "Uma caminhonete vermelha estacionada sob a cobertura do posto. Os pneus estão limpos — quem dirige isso não subiu pro barro do bairro hoje.", "marcus")
        });
    }

    private void InteragirVendaAntiga()
    {
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "A antiga Venda de madeira da entrada está fechada com cadeado.", "marcus"),
            new DialogoLinha("MARCUS", "Há uma tabuleta pregada na porta: 'Mudamos para o prédio de dois andares do MERCADINHO, seguindo reto pela avenida de asfalto até a Encruzilhada'.", "marcus")
        });
    }

    private void InteragirFrentista()
    {
        bool primeiraVez = !GameState.Tem(Flags.FalouComFrentista);
        GameState.Marcar(Flags.FalouComFrentista);

        GameState.RegistrarNotaDiario(
            "frentista_posto",
            "Relato do Frentista na Conveniência do Posto",
            "PISTA",
            "Dentro da conveniência do Posto, o frentista confirmou que Adílio mora no Bairro das Casas, nº 40 (seguindo o asfalto para a direita até a Encruzilhada do Mercadinho e subindo a rua de barro)."
        );

        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("MARCUS", "Zé abriu o portão do Centro. Vou seguir por lá procurando o vô.", "marcus"),
                new DialogoLinha("FRENTISTA", "Então vai com cuidado. Daqui eu só vi o movimento da avenida; não sei como está mais adiante.", "frentista"),
                new DialogoLinha("MARCUS", "Não vou parar até encontrar o meu avô.", "marcus")
            });
            return;
        }

        if (GameState.Tem(Flags.Pista_CasaAvoCompleta))
        {
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("MARCUS", "A casa do meu avô estava destrancada, o café ainda morno e um banco caído na porta. Parece que interromperam alguma coisa.", "marcus"),
                new DialogoLinha("FRENTISTA", "Com uma chuva dessas, seu Adílio jamais deixaria a porta aberta... Fala com a atendente dentro do Mercadinho na Encruzilhada ou sobe direto pro Bar do Zé na Rua Norte.", "frentista")
            });
            return;
        }

        if (primeiraVez)
        {
            GameState.DefinirObjetivo("Sair do Posto e seguir pela avenida para a direita até a Encruzilhada do Mercadinho.");
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("FRENTISTA", "Boa noite, chefia! Entra aí, sacode esse guarda-chuva que lá fora o temporal não está pra brincadeira.", "frentista"),
                new DialogoLinha("MARCUS", "Boa noite. Estou procurando meu avô, o seu Adílio. Liguei o dia inteiro e o telefone dele só dá ocupado.", "marcus"),
                new DialogoLinha("FRENTISTA", "Adílio da casa quarenta? Conheço das entregas. Hoje não veio aqui. Só estou acabando de fechar o caixa antes de baixar a porta.", "frentista"),
                new DialogoLinha("MARCUS", "É ele. Vou tentar na casa. A subida é depois do mercado?", "marcus"),
                new DialogoLinha("FRENTISTA", "Isso, segue o asfalto até a encruzilhada e pega a rua de barro. Vai com cuidado na curva; uma caminhonete do Clube passou levantando água ali mais cedo.", "frentista")
            });
        }
        else
        {
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("FRENTISTA", "Segue pela avenida de asfalto para a direita até a Encruzilhada do Mercadinho. Subindo a rua de barro você chega no Bairro das Casas (nº 40).", "frentista")
            });
        }
    }

    private void InteragirLivroPosto()
    {
        GameState.RegistrarNotaDiario(
            "livro_plantao_posto",
            "Registro de abastecimento no Posto",
            "PISTA",
            "O caderno do balcão do posto registra que uma caminhonete escura ligada ao Clube abasteceu ao anoitecer e subiu em direção ao Bairro das Casas."
        );

        GameMessageUI.Instance.ShowInspection("LIVRO DE PLANTÃO", "Bilhete",
            "21:30 — Caminhonete preta. Clube Santa Rita. Pagamento em dinheiro. Seguiu para o Bairro das Casas.");
    }

    private void InteragirTvPosto()
    {
        GameMessageUI.Instance.ShowInspection("TV PORTÁTIL DO POSTO", "TV_Old",
            "Uma TV pequena de tubo em cima do balcão. O sinal regional oscila a cada trovão lá fora.");
    }

    private void InteragirPicapeEncruzilhada()
    {
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "Uma caminhonete estacionada no meio-fio em frente ao Mercadinho. Os paralamas estão carregados com o barro avermelhado da subida do bairro.", "marcus")
        });
    }

    private void InteragirQuadroEncruzilhada()
    {
        GameMessageUI.Instance.ShowInspection("MURAL DA ENCRUZILHADA", "RoadSign_Blank",
            "Avisos de missa e contas atrasadas.\n\nA carvão: 'Depois da meia-noite, o portão do Centro só abre com Ficha do Clube'.");
    }

    private void InteragirPlacaSetaBairro()
    {
        GameMessageUI.Instance.ShowInspection("PLACA DE DIREÇÃO", "ArrowSign",
            "↖ BAIRRO DAS CASAS\nNúmeros 10 a 50\n\n↑ RUA NORTE\nBar do Zé");
    }

    private void InteragirAtendenteMercadinho()
    {
        if (GameState.Tem(Capitulo2Director.Vitoria))
        {
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("MARCUS", "Tonão guardou um recado do vô. Ainda não sei onde ele está. Se aparecer notícia, posso receber pela Rodoviária.", "marcus"),
                new DialogoLinha("ATENDENTE", "Está bem. Se alguém vier perguntar por ele, vou pedir que deixe o recado lá.", "atendente")
            });
            return;
        }
        GameState.Marcar(Flags.FalouComAtendenteMercadinho);

        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("MARCUS", "Falei com Zé. Ele me deu esta ficha e disse que levaram o vô pelo portão do Centro.", "marcus"),
                new DialogoLinha("ATENDENTE", "Essa ficha é da mesa dele. Eu não vi o portão, só a passagem aqui pela rua. Se souber do Adílio, deixa notícia, por favor.", "atendente")
            });
            return;
        }

        if (GameState.Tem(Flags.ChegouCasaAvo) || GameState.Tem(Flags.Pista_NotaFiscal2004) || GameState.Tem(Flags.Pista_CasaAvoCompleta))
        {
            GameState.Marcar(Flags.Pista_RumoBarDoZe);
            GameState.RegistrarNotaDiario(
                "atendente_bar_do_ze",
                "Testemunho da Atendente no Mercadinho",
                "PISTA",
                "Dentro do Mercadinho, a atendente confirmou o cupom de hoje e contou que viu Adílio subindo para os lados do Bar do Zé na Rua Norte cercado por dois homens do Clube."
            );
            GameState.DefinirObjetivo("Subir pelo Bairro das Casas até a Rua Norte e confrontar Zé no Bar do Zé.");

            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("MARCUS", "Sou Marcus, neto do Adílio. Estive na casa dele agora. A porta estava destrancada e achei esta nota fiscal daqui com um aviso sobre o Bar do Zé.", "marcus"),
                new DialogoLinha("ATENDENTE", "Fala baixo, Marcus... Seu Adílio comprou esse café comigo hoje à tarde. Ele estava tenso, disse que tinham mexido nos arquivos da noite de 1998.", "atendente"),
                new DialogoLinha("ATENDENTE", "Pouco antes do temporal desabar, vi a caminhonete escura do Clube descer do Bairro das Casas levando ele pro alto da Rua Norte, bem na porta do Bar do Zé.", "atendente"),
                new DialogoLinha("MARCUS", "Então é no Bar do Zé que eu vou achar a resposta. Fica subindo até o fim do Bairro das Casas, à direita, não é?", "marcus"),
                new DialogoLinha("ATENDENTE", "Isso mesmo. Mas cuidado: o Zé controla o portão pro Centro e não abre a boca pra ninguém sem apostar na mesa de sinuca dele.", "atendente")
            });
        }
        else
        {
            GameState.RegistrarNotaDiario(
                "atendente_rotina_hoje",
                "Compras de Adílio hoje no Mercadinho",
                "FATO",
                "A atendente do Mercadinho contou que Adílio esteve na loja hoje mesmo comprando café e velas antes de subir a estrada de barro de volta para o Bairro das Casas (nº 40)."
            );
            GameState.DefinirObjetivo("Sair do Mercadinho e subir a estrada de barro na Encruzilhada até o Bairro das Casas (Casa nº 40).");

            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("ATENDENTE", "Boa noite. Pode entrar, ainda estou conferindo as entregas. Está precisando de alguma coisa?", "atendente"),
                new DialogoLinha("MARCUS", "Sou Marcus, neto do Adílio. Você viu meu avô hoje? Liguei desde cedo e ele não atende.", "marcus"),
                new DialogoLinha("ATENDENTE", "Vi sim! Ele esteve aqui hoje mesmo, levou pó de café e um maço de velas. Subiu pela rua de terra aqui do lado de volta pro Bairro das Casas.", "atendente"),
                new DialogoLinha("ATENDENTE", "Sobe pela estrada de barro ali fora na encruzilhada até a casa dele, no número 40. Se achar qualquer coisa estranha lá, volta aqui e me avisa!", "atendente")
            });
        }
    }

    private void InteragirCadernoMercadinho()
    {
        GameState.RegistrarNotaDiario(
            "caderno_entregas_mercadinho",
            "Caderno de Vendas do Mercadinho (2004)",
            "PISTA",
            "O livro-caixa do Mercadinho comprova que Adílio fez compras hoje (Outubro de 2004) e registra remessas de bebidas para o Bar do Zé na Rua Norte."
        );

        GameMessageUI.Instance.ShowInspection("CADERNO DO MERCADINHO", "Bilhete",
            "19/10/2004 — Adílio, Bairro das Casas, 40. Café torrado e velas: pago.\n\nRemessa de engradados — Bar do Zé, Rua Norte.");
    }

    private void InteragirPrateleirasMercadinho()
    {
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "Prateleiras abastecidas com enlatados, café, pilhas e velas. O cheiro de café moído aqui dentro é idêntico ao que o vô Adílio sempre comprava.", "marcus")
        });
    }

    private void InteragirVizinhaLurdes()
    {
        GameState.RegistrarNotaDiario(
            "vizinha_lurdes_relato",
            "Relato da Vizinha (Casa nº 14)",
            "PISTA",
            "Dona Lurdes, moradora da entrada do Bairro das Casas, ouviu batidas pesadas e vozes perto da casa nº 40 antes do temporal apertar."
        );

        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("DONA LURDES (ATRÁS DA PORTA)", "Quem está batendo a essa hora?! ...Ah, é o neto do seu Adílio!", "atendente"),
            new DialogoLinha("MARCUS", "Dona Lurdes, desculpe incomodar. A senhora viu o meu avô hoje?", "marcus"),
            new DialogoLinha("DONA LURDES (ATRÁS DA PORTA)", "Menino, mais cedo ouvi um carro pesado subir essa lama toda e parar lá na rua de cima, perto da casa 40. Depois ouvi vozes grossas e passos subindo pro lado do Bar do Zé!", "atendente"),
            new DialogoLinha("DONA LURDES (ATRÁS DA PORTA)", "Segue subindo esta rua de barro que a casa 40 do teu avô fica logo no quarteirão de cima, ao lado do Seu Ramiro!", "atendente")
        });
    }

    private void InteragirGalpaoBairro()
    {
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "O barro aqui na esquina está revirado por pneus largos de caminhonete. O rastro sobe direto para a rua da casa do vô Adílio e continua para o nordeste.", "marcus")
        });
    }

    private void InteragirMoradorBairro()
    {
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "Boa noite. O senhor conhece Adílio, da casa quarenta? Sou o neto dele.", "marcus"),
            new DialogoLinha("SEU RAMIRO", "Conheço, somos vizinhos. Ramiro. Estava recolhendo a lenha antes da chuva apertar. A casa dele fica logo ali, depois da cerca.", "adilio"),
            new DialogoLinha("MARCUS", "O senhor viu se alguém esteve na casa dele hoje?", "marcus"),
            new DialogoLinha("SEU RAMIRO", "Vi uma caminhonete escura parar aí na frente da casa 40 mais cedo. Depois ouvi passos apressados subindo a ladeira de barro lá da direita, que dá na Rua Norte e no Bar do Zé.", "adilio")
        });
    }

    private void InteragirCaixaCorreioAvo()
    {
        GameMessageUI.Instance.ShowInspection("CAIXA DE CORREIO", "CaixaCorreio",
            "Família Adílio\nBairro das Casas, nº 40\n\nA madeira está úmida da chuva. É aqui.");
    }

    private void InteragirPistaCafe()
    {
        GameState.Marcar(Flags.Pista_CafeAindaQuente);
        GameState.RegistrarNotaDiario(
            "pista_cafe_hoje",
            "Café passado hoje mesmo",
            "PISTA",
            "A garrafa térmica e a xícara na cozinha de Adílio ainda guardam calor e cheiro de café fresco. A rotina foi interrompida hoje, poucas horas antes da minha chegada."
        );

        GameMessageUI.Instance.ShowInspection("CAFÉ AINDA MORNO", "Interior_PiaAvo",
            "A xícara está pela metade e o bule ainda está morno. Adílio passou café hoje mesmo: ele estava em casa até poucas horas antes da minha chegada.",
            VerificarConclusaoPistasCasaAvo);
    }

    private void InteragirPistaCartas()
    {
        GameState.Marcar(Flags.Pista_CartasMesa);
        GameState.RegistrarNotaDiario(
            "pista_cartas_mesa",
            "Paciência interrompida na mesa",
            "PISTA",
            "As cartas de baralho de Adílio ficaram espalhadas no meio de uma partida solitária, indicando que alguém bateu à porta e o fez levantar no meio do jogo."
        );

        GameMessageUI.Instance.ShowInspection("PACIÊNCIA INTERROMPIDA", "Interior_MesaCartas",
            "O baralho de Adílio ficou espalhado no meio de uma partida. Ele nunca deixava as cartas fora da caixa antes de sair. Alguém o interrompeu.",
            VerificarConclusaoPistasCasaAvo);
    }

    private void InteragirPistaNotaFiscal()
    {
        GameState.Marcar(Flags.Pista_NotaFiscal2004);
        GameState.Marcar(Flags.CasaAvo_LeuBilhete);
        GameState.RegistrarNotaDiario(
            "pista_nota_2004",
            "Nota Fiscal de 2004 e anotação do Bar do Zé",
            "PISTA",
            "Cupom do Mercadinho Santa Rita datado de hoje (2004). No verso, a letra firme de Adílio: 'Rua Norte — Bar do Zé. Não aceitar recado do Clube'."
        );

        GameMessageUI.Instance.ShowInspection("NOTA E BILHETE", "Bilhete",
            "Cupom do Mercadinho Santa Rita, 19/10/2004.\n\nNo verso, a letra de Adílio: 'Rua Norte — Bar do Zé. Não aceitar recado do Clube'.",
            VerificarConclusaoPistasCasaAvo);
    }

    private void InteragirPistaDesordem()
    {
        GameState.Marcar(Flags.Pista_DesordemPorta);
        GameState.RegistrarNotaDiario(
            "pista_desordem_porta",
            "Desordem localizada junto à porta",
            "PISTA",
            "A casa está arrumada, exceto perto da entrada: um banco derrubado, marcas de lama recente e o casaco ainda no cabide."
        );

        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "O resto da casa está em ordem, mas aqui perto da porta o banco tombou e há pegadas de barro molhado que não são do calçado dele.", "marcus"),
            new DialogoLinha("MARCUS", "O casaco de chuva dele ficou no prego. Ele não saiu voluntariamente.", "marcus")
        }, VerificarConclusaoPistasCasaAvo);
    }

    private void InteragirRetratoFamilia()
    {
        GameMessageUI.Instance.ShowInspection("RETRATO DE FAMÍLIA", "FramedPhoto",
            "Adílio, minha mãe e eu, antes de 1998. Atrás da moldura: uma chave enferrujada e '03:17' escrito a lápis.",
            VerificarConclusaoPistasCasaAvo);
    }

    private void InteragirTelefoneCasaAvo()
    {
        GameState.Marcar(Flags.CasaAvo_OlhouTelefone);
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "O fone está caído ao lado da base, emitindo um tom de ocupado constante. Ele tentou discar para alguém quando bateram na porta.", "marcus")
        }, VerificarConclusaoPistasCasaAvo);
    }

    public static void VerificarConclusaoPistasCasaAvo()
    {
        int cont = 0;
        if (GameState.Tem(Flags.Pista_CafeAindaQuente)) cont++;
        if (GameState.Tem(Flags.Pista_CartasMesa)) cont++;
        if (GameState.Tem(Flags.Pista_NotaFiscal2004)) cont++;
        if (GameState.Tem(Flags.Pista_DesordemPorta)) cont++;
        if (GameState.Tem(Flags.CasaAvo_LeuBilhete)) cont++;
        if (GameState.Tem(Flags.CasaAvo_OlhouTelefone)) cont++;

        if (cont >= 3 && !GameState.Tem(Flags.Pista_CasaAvoCompleta))
        {
            GameState.Marcar(Flags.Pista_CasaAvoCompleta);
            GameState.Marcar(Flags.Pista_RumoBarDoZe);
            GameState.RegistrarNotaDiario(
                "hipotese_casa_adilio",
                "Rotina interrompida — bilhete para a Rua Norte",
                "HIPÓTESE",
                "O café morno, a nota fiscal de hoje (2004), as cartas interrompidas e o banco caído sugerem uma saída inesperada. Ainda não sei o motivo. O bilhete aponta para o Bar do Zé, na Rua Norte."
            );
            GameState.DefinirObjetivo("Sair da casa de Adílio e subir pela saída nordeste do Bairro até a Rua Norte / Bar do Zé (ou confirmar no Mercadinho).");
            Legenda.Dizer("O bilhete fala da Rua Norte. Talvez alguém tenha visto o vô por lá.", 4f, Legenda.PrioridadeCritica);
        }
    }

    private void InteragirCaminhoneteClube()
    {
        GameState.RegistrarNotaDiario(
            "caminhonete_clube_bar_ze",
            "Caminhonete do Clube em frente ao Bar do Zé",
            "PISTA",
            "A mesma caminhonete escura vista no Posto e no Bairro das Casas esteve parada na Rua Norte, junto ao Bar do Zé e ao Portão do Centro."
        );

        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "Uma caminhonete escura estacionada na Rua Norte, bem perto do Bar do Zé. No vidro há um selo metálico do Clube de Santa Rita.", "marcus"),
            new DialogoLinha("MARCUS", "Os pneus têm barro vermelho, como o da rua do vô. Pode ter passado por lá. Isso sozinho não me diz quem estava dentro.", "marcus")
        });
    }

    private void InteragirPlacaBar()
    {
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "'BAR DO ZÉ — SINUCA E BEBIDAS'. A luz amarela lá dentro está acesa e dá pra ouvir o estalo seco das bolas de bilhar.", "marcus")
        });
    }

    private void InteragirBalcaoBar()
    {
        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "Garrafas empoeiradas, um cinzeiro cheio e um caderno de fiado onde metade dos nomes foi riscada a caneta preta.", "marcus")
        });
    }

    private void InteragirQuadroRegras()
    {
        GameMessageUI.Instance.ShowInspection("REGRAS DA MESA", "Quadro_Regras_Sinuca",
            "MELHOR DE 3\n7 bolas na mesa. Quem fizer 4 leva a mão.\n\nA partir da 2ª mão, o dono marca uma bola [X]: ESSA FICA NA MESA.");
    }

    private void InteragirFrequentadorBar()
    {
        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("FREQUENTADOR", "Nunca achei que ia ver alguém limpar a mesa do Zé respeitando a bola marcada. O velho Adílio ia ficar orgulhoso.", "adilio")
            });
            return;
        }

        GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
        {
            new DialogoLinha("FREQUENTADOR", "Cuidado com a segunda mão na mesa do Zé, rapaz. Ele sempre escolhe uma bola pra marcar com giz. Se você derrubar a bola marcada, perde a vez e dá ponto pra ele.", "adilio"),
            new DialogoLinha("MARCUS", "Obrigado pelo aviso. Só preciso que ele abra o bico sobre o meu avô.", "marcus")
        });
    }

    private void InteragirZeOuSinuca()
    {
        if (GameState.Tem("c2_registro_manutencao"))
        {
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha> {
                new DialogoLinha("MARCUS", "A cópia de manutenção registra outras tentativas. O padre ouviu Júlia responder.", "marcus"),
                new DialogoLinha("ZÉ", "Eu queria o salão funcionando. Não vou dizer que vi cada tentativa. Sobre teu avô, contei a passagem pelo portão; não sei o que aconteceu depois dali.", "ze")
            });
            return;
        }
        GameState.Marcar(Flags.EntrouNoBarDoZe);

        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("ZÉ", "Já te entreguei a Ficha do Clube e já destranquei o portão pro Centro lá fora na Rua Norte, garoto. Não me pede pra ir junto.", "ze"),
                new DialogoLinha("MARCUS", "Seu Adílio esteve aqui hoje e foi levado porque não aceitou calar a boca sobre 1998. Eu vou até o fim.", "marcus"),
                new DialogoLinha("ZÉ", "Então guarda bem essa ficha no bolso e usa o orelhão lá fora antes de cruzar pro Centro.", "ze")
            });
            return;
        }

        GameState.Marcar(Flags.FalouComZeAntesDaSinuca);
        GameState.RegistrarNotaDiario(
            "confronto_ze_bar",
            "O Desafio do Bar do Zé",
            "FATO",
            "Zé admitiu que Adílio esteve no bar hoje, mas exige uma partida Melhor de 3 na sinuca (com a regra 'A Bola Marcada / Essa Fica na Mesa') antes de entregar a ficha e abrir passagem para o Centro."
        );

        var linhasPreDuelo = new List<DialogoLinha>
        {
            new DialogoLinha("ZÉ", "Olha só quem a chuva trouxe de volta pra Santa Rita. O neto do seu Adílio.", "ze"),
            new DialogoLinha("MARCUS", "Onde está o meu avô, Zé? Na casa dele o café ainda estava morno, o banco perto da porta tombou e encontrei o teu nome anotado num cupom de hoje.", "marcus"),
            new DialogoLinha("ZÉ", "Teu avô apareceu aqui hoje mais cedo, sim. Veio fazer pergunta que ninguém nesta cidade quer ouvir desde aquela madrugada de noventa e oito.", "ze"),
            new DialogoLinha("MARCUS", "Quem levou ele daqui? Ele não saiu daquela casa por vontade própria.", "marcus"),
            new DialogoLinha("ZÉ", "Quer que eu abra o portão do Centro e te diga quem carregou o velho? Então pega o taco, garoto. Melhor de três na minha mesa.", "ze"),
            new DialogoLinha("ZÉ", "Sete bolas, quatro pontos levam a mão. Mas a partir da segunda mão vale a regra da casa: quando eu marcar uma bola, ESSA FICA NA MESA. Encaçapou a marcada, entrega o ponto e a bola volta pro pano.", "ze")
        };

        GameMessageUI.Instance.ShowDialogue(linhasPreDuelo, IniciarPartidaSinucaComZe);
    }

    private void IniciarPartidaSinucaComZe()
    {
        SinucaMinigameUI.Instance.IniciarDuelo(
            aoVencer: AoVencerZeNaSinuca,
            aoPerder: () =>
            {
                Legenda.Dizer("Zé continua guardando o caminho para o Centro.", 3.5f, Legenda.PrioridadeCritica);
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
            "Ficha do Clube e Confissão de Zé",
            "PISTA",
            "Derrotei Zé na sinuca (Melhor de 3). Ele entregou a Ficha do Clube #01 e confirmou que Adílio não saiu voluntariamente: homens ligados ao Clube o levaram pelo portão do Centro porque ele voltou a mexer na noite de 1998."
        );

        GameState.DefinirObjetivo("Sair do Bar do Zé e atravessar o portão no fim da Rua Norte rumo ao Centro.");

        var linhasPosVitoria = new List<DialogoLinha>
        {
            new DialogoLinha("ZÉ", "...Você tem o pulso firme do velho Adílio. Palavra dada na mesa não volta atrás.", "ze"),
            new DialogoLinha("ZÉ", "Pega. Essa é uma Ficha do Clube — `ficha_clube_ze_01`. Sem esse metal no bolso, as portas lá pra cima nem te enxergam.", "ze"),
            new DialogoLinha("MARCUS", "Agora fala a verdade, Zé. O que fizeram com o meu avô hoje?", "marcus"),
            new DialogoLinha("ZÉ", "Teu avô não saiu de casa porque quis, Marcus. Ele descobriu que alguém mexeu nos registros antigos da estrada — daquela noite das 03:17 em noventa e oito.", "ze"),
            new DialogoLinha("ZÉ", "Dois homens desceram do Centro hoje à tarde e levaram ele passando pelo portão aqui do lado. Eu já destranquei a grade lá fora. O resto é entre você e o que sobrou desta cidade.", "ze")
        };

        GameMessageUI.Instance.ShowDialogue(linhasPosVitoria, () =>
        {
            Legenda.Dizer("Recebido: Ficha do Clube (Zé). Portão do Centro desbloqueado! Pressione M para ver o Mapa/Diário.", 5f, Legenda.PrioridadeCritica);
            Capitulo1Director.AtualizarEstadoPortaoCentroNaCena();
        });
    }

    private void InteragirLimiarCentro()
    {
        if (!GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("MARCUS", "O portão de ferro que dá acesso ao Centro de Santa Rita está trancado com corrente.", "marcus"),
                new DialogoLinha("MARCUS", "A fechadura tem o mesmo emblema do letreiro do Bar do Zé aqui ao lado. Preciso resolver isso com o Zé lá dentro primeiro.", "marcus")
            });
            return;
        }

        GameState.Marcar(Flags.LimiarCentroDesbloqueado);
        GameState.Marcar(Flags.Capitulo1Concluido);
        GameState.RegistrarNotaDiario(
            "capitulo1_concluido_limiar",
            "Limiar do Centro Atravessado",
            "FATO",
            "Com a Ficha do Clube conquistada no Bar do Zé e a confirmação de que Adílio foi levado contra a vontade, cruzei o portão rumo ao Centro de Santa Rita."
        );

        var linhasFechamento = new List<DialogoLinha>
        {
            new DialogoLinha("MARCUS", "A corrente cedeu. Do outro lado da neblina começam os casarões do Centro e a subida para o Clube.", "marcus"),
            new DialogoLinha("MARCUS", "Eu tenho a primeira Ficha do Clube e sei que o vô Adílio foi levado hoje porque chegou perto demais da verdade sobre 1998.", "marcus")
        };

        GameMessageUI.Instance.ShowDialogue(linhasFechamento, () =>
        {
            Capitulo1Director.MostrarTelaFechamentoCapitulo1();
        });
    }
}

/// <summary>
/// Diretor do Capítulo 1: Constrói e gerencia as 5 Zonas Externas ("cenas/quadrados" separados por cortes de fade)
/// e os 4 Interiores Exploráveis (Conveniência do Posto, Mercadinho, Casa do Avô Adílio nº 40 e Bar do Zé),
/// fiéis às Imagens 3, 4 e 5 e ao Mapa Oficial (`MapaSantaRitaArte.jpg`).
/// </summary>
