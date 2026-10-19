using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// A primeira coisa que acontece depois do prólogo (§8 do documento).
public class ChegadaEmSantaRita : MonoBehaviour
{
    [SerializeField] private float esperaAntesDaFala = 2.0f;
    [SerializeField] private float esperaAntesDoObjetivo = 1.4f;

    [TextArea]
    [SerializeField] private string comSabendoOEndereco = "Bairro das Casas, 40. Preciso seguir pela estrada de asfalto e subir na encruzilhada.";

    [TextArea]
    [SerializeField] private string semSaberOEndereco = "Faz seis anos... Preciso seguir pela estrada à direita e perguntar no Posto onde fica a casa do vô Adílio.";

    [TextArea]
    [SerializeField] private string objetivo = "Seguir pela estrada para a direita (Leste) até o Posto de Gasolina. (Pressione M para abrir o Mapa)";

    private IEnumerator Start()
    {
        Capitulo1Director.EnsureForCurrentScene();

        if (!GameState.Tem(Flags.ChegouCasaAvo) && string.IsNullOrEmpty(GameState.ObterObjetivo()))
        {
            GameState.DefinirObjetivo("Seguir pela rodovia para a direita até o Posto de Gasolina e procurar pela casa de Adílio (nº 40).");
        }

        if (!GameState.Tem(Flags.PrologoConcluido) || GameState.Tem(Flags.ChegouCasaAvo))
        {
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

        Legenda.Dizer(objetivo, 4.5f);
    }
}

/// <summary>
/// Gatilho de transição entre as Zonas da Cidade ("quadrados/cenas") e Interiores,
/// usando corte de câmera com fade-to-black (estilo Pokémon FireRed / OMORI / Corpse Party).
/// </summary>
public class TransicaoDeZonaTrigger : Interactable
{
    private Capitulo1Director.ZonaId zonaDestino;
    private Vector2 posicaoDestino;
    private int direcaoDestino;
    private string promptTexto = "Avançar";
    private bool automaticoAoEncostar;
    private bool exigirCasaAvoVisitada;
    private float cooldownAte;

    public override string Prompt => promptTexto;

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
                new DialogoLinha("FRENTISTA", "Ouvi dizer que você limpou a mesa do Zé lá na Rua Norte. Se ele abriu o portão do Centro, vai com cuidado, rapaz.", "frentista"),
                new DialogoLinha("MARCUS", "Não vou parar até encontrar o meu avô.", "marcus")
            });
            return;
        }

        if (GameState.Tem(Flags.Pista_CasaAvoCompleta))
        {
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("MARCUS", "A casa do meu avô estava destrancada, o café ainda morno e um banco caído na porta. Ele não saiu por vontade própria.", "marcus"),
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
                new DialogoLinha("FRENTISTA", "Seu Adílio? Homem pontual daquele jeito? Hoje mais cedo passou uma caminhonete escura do Clube abastecendo aqui e subiu pro lado do bairro dele.", "frentista"),
                new DialogoLinha("FRENTISTA", "Faz o seguinte: sai aqui na avenida e segue para a direita até a Encruzilhada do Mercadinho. Lá na encruzilhada sobe a estrada de barro à esquerda que você cai direto no Bairro das Casas. A casa dele é a número 40!", "frentista")
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
        GameState.Marcar(Flags.FalouComAtendenteMercadinho);

        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            GameMessageUI.Instance.ShowDialogue(new List<DialogoLinha>
            {
                new DialogoLinha("ATENDENTE", "Você ganhou do Zé na própria mesa dele?! Fazia anos que ninguém tirava uma Ficha do Clube da mão daquele homem...", "atendente"),
                new DialogoLinha("MARCUS", "Ele confessou que levaram o vô Adílio pelo portão do Centro. Vou atrás deles agora.", "marcus")
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
                new DialogoLinha("MARCUS", "Estive na casa do meu avô agora. A porta estava destrancada, o café ainda morno e achei esta nota fiscal daqui de hoje com um aviso sobre o Bar do Zé.", "marcus"),
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
                new DialogoLinha("ATENDENTE", "Boa noite! Entre, estava quase baixando as portas por causa da chuva... Espera, você não é o Marcus, neto do seu Adílio?", "atendente"),
                new DialogoLinha("MARCUS", "Sou eu mesmo. Você viu meu avô hoje? Liguei desde cedo e ele não atende.", "marcus"),
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
            new DialogoLinha("SEU RAMIRO", "Boa noite, guri. Se veio procurar o Adílio, a casa dele é essa grande logo aqui à direita, número 40, com cerca de madeira e chaminé.", "adilio"),
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
                "Adílio foi levado hoje — Rumo ao Bar do Zé",
                "HIPÓTESE",
                "O café morno, a nota fiscal de hoje (2004), as cartas interrompidas e o banco caído provam que Adílio não saiu por conta própria. O bilhete aponta para o Bar do Zé na Rua Norte (subindo a rua à direita no Bairro das Casas)."
            );
            GameState.DefinirObjetivo("Sair da casa de Adílio e subir pela saída nordeste do Bairro até a Rua Norte / Bar do Zé (ou confirmar no Mercadinho).");
            Legenda.Dizer("Encontrei as pistas! Agora posso subir pela Rua Norte (nordeste do Bairro) até o Bar do Zé. [M] Mapa", 5f, Legenda.PrioridadeCritica);
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
            new DialogoLinha("MARCUS", "Os pneus têm o mesmo barro vermelho da frente da casa 40. Eles trouxeram o vô Adílio por aqui.", "marcus")
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
public class Capitulo1Director : MonoBehaviour
{
    public enum ZonaId
    {
        Zona1_Rodovia = 0,
        Zona2_Posto = 1,
        Interior_Posto = 2,
        Zona3_Encruzilhada = 3,
        Interior_Mercadinho = 4,
        Zona4_Bairro = 5,
        Interior_CasaDoAvo = 6,
        Zona5_RuaNorteBar = 7,
        Interior_BarDoZe = 8
    }

    public struct InfoZona
    {
        public ZonaId id;
        public string titulo;
        public string subtitulo;
        public Vector2 min;
        public Vector2 max;
        public float orthoSize;
        public bool ehInterior;
        public Vector2 uvMinMapa;
        public Vector2 uvMaxMapa;
    }

    private static Capitulo1Director instancia;
    public static Capitulo1Director Instance => instancia;

    private ZonaId zonaAtual = ZonaId.Zona1_Rodovia;
    public static ZonaId ZonaAtual => instancia != null ? instancia.zonaAtual : ZonaId.Zona1_Rodovia;

    private GameObject raizMundoCapitulo1;
    private GameObject painelFechamentoCap1;
    private PayphoneCheckpoint templatePayphone;

    private static readonly Dictionary<string, Texture2D> cacheTexturas = new Dictionary<string, Texture2D>();
    private static readonly Dictionary<string, Sprite> cacheSprites = new Dictionary<string, Sprite>();
    private static readonly Dictionary<ZonaId, InfoZona> tabelaZonas = new Dictionary<ZonaId, InfoZona>
    {
        {
            ZonaId.Zona1_Rodovia,
            new InfoZona
            {
                id = ZonaId.Zona1_Rodovia,
                titulo = "RODOVIA DE ENTRADA — SANTA RITA",
                subtitulo = "Chegada pela BR • Acostamento Sul",
                min = new Vector2(100f, 0f),
                max = new Vector2(136f, 20f),
                orthoSize = 5.4f,
                ehInterior = false,
                uvMinMapa = new Vector2(0.09f, 0.14f),
                uvMaxMapa = new Vector2(0.16f, 0.21f)
            }
        },
        {
            ZonaId.Zona2_Posto,
            new InfoZona
            {
                id = ZonaId.Zona2_Posto,
                titulo = "AVENIDA SUL — POSTO & VENDA ANTIGA",
                subtitulo = "Entrada da Cidade • Asfalto Principal",
                min = new Vector2(200f, 0f),
                max = new Vector2(238f, 22f),
                orthoSize = 5.5f,
                ehInterior = false,
                uvMinMapa = new Vector2(0.16f, 0.19f),
                uvMaxMapa = new Vector2(0.24f, 0.25f)
            }
        },
        {
            ZonaId.Interior_Posto,
            new InfoZona
            {
                id = ZonaId.Interior_Posto,
                titulo = "CONVENIÊNCIA DO POSTO",
                subtitulo = "Interior • Atendimento da Estrada",
                min = new Vector2(200f, -60f),
                max = new Vector2(216f, -48f),
                orthoSize = 4.6f,
                ehInterior = true,
                uvMinMapa = new Vector2(0.17f, 0.20f),
                uvMaxMapa = new Vector2(0.19f, 0.22f)
            }
        },
        {
            ZonaId.Zona3_Encruzilhada,
            new InfoZona
            {
                id = ZonaId.Zona3_Encruzilhada,
                titulo = "ENCRUZILHADA DO MERCADINHO",
                subtitulo = "Cruzamento Principal • Subida pro Bairro",
                min = new Vector2(300f, 0f),
                max = new Vector2(340f, 26f),
                orthoSize = 5.6f,
                ehInterior = false,
                uvMinMapa = new Vector2(0.24f, 0.22f),
                uvMaxMapa = new Vector2(0.32f, 0.29f)
            }
        },
        {
            ZonaId.Interior_Mercadinho,
            new InfoZona
            {
                id = ZonaId.Interior_Mercadinho,
                titulo = "MERCADINHO SANTA RITA",
                subtitulo = "Interior • Mercearia & Entregas",
                min = new Vector2(300f, -60f),
                max = new Vector2(318f, -47f),
                orthoSize = 4.7f,
                ehInterior = true,
                uvMinMapa = new Vector2(0.25f, 0.23f),
                uvMaxMapa = new Vector2(0.27f, 0.25f)
            }
        },
        {
            ZonaId.Zona4_Bairro,
            new InfoZona
            {
                id = ZonaId.Zona4_Bairro,
                titulo = "BAIRRO DAS CASAS",
                subtitulo = "Comunidade Residencial • Ruas de Barro (Nº 10 ao 50)",
                min = new Vector2(400f, 0f),
                max = new Vector2(444f, 34f),
                orthoSize = 5.8f,
                ehInterior = false,
                uvMinMapa = new Vector2(0.17f, 0.29f),
                uvMaxMapa = new Vector2(0.29f, 0.43f)
            }
        },
        {
            ZonaId.Interior_CasaDoAvo,
            new InfoZona
            {
                id = ZonaId.Interior_CasaDoAvo,
                titulo = "CASA DO AVÔ ADÍLIO — Nº 40",
                subtitulo = "Interior • Bairro das Casas",
                min = new Vector2(400f, -60f),
                max = new Vector2(418f, -46f),
                orthoSize = 4.7f,
                ehInterior = true,
                uvMinMapa = new Vector2(0.23f, 0.37f),
                uvMaxMapa = new Vector2(0.25f, 0.39f)
            }
        },
        {
            ZonaId.Zona5_RuaNorteBar,
            new InfoZona
            {
                id = ZonaId.Zona5_RuaNorteBar,
                titulo = "RUA NORTE — EXTERIOR DO BAR DO ZÉ",
                subtitulo = "Alto do Bairro • Limiar do Centro",
                min = new Vector2(500f, 0f),
                max = new Vector2(538f, 22f),
                orthoSize = 5.5f,
                ehInterior = false,
                uvMinMapa = new Vector2(0.29f, 0.39f),
                uvMaxMapa = new Vector2(0.41f, 0.44f)
            }
        },
        {
            ZonaId.Interior_BarDoZe,
            new InfoZona
            {
                id = ZonaId.Interior_BarDoZe,
                titulo = "BAR DO ZÉ — SINUCA & BEBIDAS",
                subtitulo = "Interior • Desafio da Mesa",
                min = new Vector2(500f, -60f),
                max = new Vector2(518f, -46f),
                orthoSize = 4.7f,
                ehInterior = true,
                uvMinMapa = new Vector2(0.34f, 0.40f),
                uvMaxMapa = new Vector2(0.36f, 0.42f)
            }
        }
    };

    public static bool TryObterInfoZona(ZonaId id, out InfoZona info)
    {
        return tabelaZonas.TryGetValue(id, out info);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatic()
    {
        instancia = null;
        cacheTexturas.Clear();
        cacheSprites.Clear();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AoCarregarCenaRuntime()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        EnsureForCurrentScene();
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureForCurrentScene();
    }

    public static Capitulo1Director EnsureForCurrentScene()
    {
        string cena = SceneManager.GetActiveScene().name;
        if (cena != "EntradaDaCidade" && cena != "CasaDoAvo" && cena != "BarDoZe")
        {
            return null;
        }

        if (instancia == null)
        {
            instancia = FindAnyObjectByType<Capitulo1Director>();
        }

        if (instancia == null)
        {
            GameObject go = new GameObject("Capitulo1Director");
            instancia = go.AddComponent<Capitulo1Director>();
        }

        instancia.ConfigurarCenaAtual(cena);
        return instancia;
    }

    private static Texture2D CarregarTextura(string nomeArquivo)
    {
        if (cacheTexturas.TryGetValue(nomeArquivo, out Texture2D cached) && cached != null)
        {
            return cached;
        }

        string[] pastas =
        {
            "Resources/Cap1/",
            "Sprites/Capitulo1/",
            "Art/Tiles Cidade/",
            "Art/Cidade/",
            "Art/Props (Generated)/Exterior/",
            "Art/Props (Generated)/Interior/",
            "Art/Veiculos/"
        };

        for (int i = 0; i < pastas.Length; i++)
        {
            string full = Path.Combine(Application.dataPath, pastas[i] + nomeArquivo + ".png");
            if (File.Exists(full))
            {
                byte[] bytes = File.ReadAllBytes(full);
                Texture2D texDisk = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (ImageConversion.LoadImage(texDisk, bytes))
                {
                    texDisk.filterMode = FilterMode.Point;
                    texDisk.wrapMode = TextureWrapMode.Clamp;
                    cacheTexturas[nomeArquivo] = texDisk;
                    return texDisk;
                }
            }
        }

        Texture2D texRes = Resources.Load<Texture2D>("Cap1/" + nomeArquivo);
        if (texRes != null)
        {
            texRes.filterMode = FilterMode.Point;
            cacheTexturas[nomeArquivo] = texRes;
            return texRes;
        }

        return null;
    }

    private static Sprite CarregarSprite(string nomeArquivo, float ppu, Vector2 pivot)
    {
        string chave = $"{nomeArquivo}_{ppu}_{pivot.x:F2}_{pivot.y:F2}";
        if (cacheSprites.TryGetValue(chave, out Sprite cached) && cached != null)
        {
            return cached;
        }

        Texture2D tex = CarregarTextura(nomeArquivo);
        if (tex != null)
        {
            // Os veículos de 300×300 são folhas 3×3 de ângulos, não um carro.
            // A primeira célula é a vista lateral própria para estacionar na avenida.
            bool folhaVeiculo = nomeArquivo.StartsWith("Sedan_") || nomeArquivo.StartsWith("Picape_") ||
                nomeArquivo.StartsWith("Van_") || nomeArquivo.StartsWith("Perua_") ||
                nomeArquivo.StartsWith("Hatch_") || nomeArquivo.StartsWith("Caminhao_") ||
                nomeArquivo.StartsWith("Onibus_");
            Rect recorte = folhaVeiculo && tex.width >= 300 && tex.height >= 300
                ? new Rect(0, tex.height - 100, 100, 100)
                : new Rect(0, 0, tex.width, tex.height);
            Sprite spr = Sprite.Create(tex, recorte, pivot, ppu);
            cacheSprites[chave] = spr;
            return spr;
        }

        return null;
    }

    private void ConfigurarCenaAtual(string cena)
    {
        MapaInterativoUI.EnsureInstance();

        if (cena == "EntradaDaCidade")
        {
            ConstruirMundoMultiZonas();
            AtualizarEstadoPortaoCentroNaCena();
        }
        else if (cena == "CasaDoAvo")
        {
            GarantirElementosCasaDoAvoCenaSeparada();
        }
        else if (cena == "BarDoZe")
        {
            GarantirElementosBarDoZeCenaSeparada();
        }
    }

    // =========================================================================
    // SISTEMA MULTI-ZONAS & INTERIORES (Fiel às Imagens 3, 4 e 5)
    // =========================================================================

    private void ConstruirMundoMultiZonas()
    {
        if (raizMundoCapitulo1 != null)
        {
            return;
        }

        // 1. Guarda referência ao Orelhão original antes de ocultar o cenário antigo
        GameObject orelhaoOrig = GameObject.Find("Orelhao");
        if (orelhaoOrig != null)
        {
            templatePayphone = orelhaoOrig.GetComponent<PayphoneCheckpoint>();
        }

        // A cena serializada ainda contém a cidade protótipo. Os carros dela
        // ficavam desenhados sobre os da cidade construída abaixo.
        string[] raizesPreservadas =
        {
            "Player", "Global Light 2D", "Main Camera", "Flashlight",
            "MenuPausa", "Chuva", "Capitulo1Director", "Ato I",
            "Saida do Carro", "Entrada da Casa", "Spawn_SaidaBarDoZe"
        };
        foreach (GameObject raizLegada in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (Array.IndexOf(raizesPreservadas, raizLegada.name) < 0)
                raizLegada.SetActive(false);
        }

        // Compatibilidade com cenas anteriores que tinham parte do cenário em filhos.
        string[] objetosAntigosParaOcultar =
        {
            "Grid",
            "Mata",
            "Cenario",
            "Limites",
            "PortaCasaAvo",
            "Orelhao",
            "Luz",
            "Posto_Cobertura",
            "Posto_Frentista",
            "Predio_Venda_Mercadinho",
            "Mercadinho_Atendente",
            "Predio_CasaDoAvo",
            "Cerca_CasaAvo_Esq",
            "Cerca_CasaAvo_Dir",
            "Bairro_Casa_A1",
            "Bairro_Casa_B1",
            "Bairro_Casa_C1",
            "Bairro_Cerca_1",
            "Bairro_Morador_Ramiro",
            "Fachada_BarDoZe",
            "Placa_BarDoZe",
            "Porta_BarDoZe",
            "Payphone_BarDoZe",
            "Portao_Centro",
            "Barreira_Limiar_Centro",
            "Limiar_Centro_Trigger",
            "Carro_Marcus_Interacao", "CarroDoProtagonista", "PicapeNaVenda", "PeruaNoBairro"
        };

        for (int i = 0; i < objetosAntigosParaOcultar.Length; i++)
        {
            GameObject go = GameObject.Find(objetosAntigosParaOcultar[i]);
            if (go != null)
            {
                go.SetActive(false);
            }
        }

        // 3. Ajusta os SpawnPoints para que o prólogo e retornos de cena caiam nas Zonas corretas
        AtualizarSpawnPointExistente("Saida do Carro", "saida_carro", new Vector3(112.0f, 10.2f, 0f));
        AtualizarSpawnPointExistente("Entrada da Casa", "entrada_casa_avo", new Vector3(423.5f, 22.8f, 0f));
        AtualizarSpawnPointExistente("Spawn_SaidaBarDoZe", "saida_bar_ze", new Vector3(518.0f, 11.8f, 0f));

        // 4. Ajusta a iluminação global para que o asfalto, o barro e os prédios fiquem nítidos à noite
        AjustarLuzGlobal(false);

        raizMundoCapitulo1 = new GameObject("MundoSantaRita_Cap1");

        // 5. Constrói as 5 Zonas Externas e os 4 Interiores
        ConstruirZona1_Rodovia(raizMundoCapitulo1.transform);
        ConstruirZona2_Posto(raizMundoCapitulo1.transform);
        ConstruirInterior1_Posto(raizMundoCapitulo1.transform);
        ConstruirZona3_Encruzilhada(raizMundoCapitulo1.transform);
        ConstruirInterior2_Mercadinho(raizMundoCapitulo1.transform);
        ConstruirZona4_Bairro(raizMundoCapitulo1.transform);
        ConstruirInterior3_CasaDoAvo(raizMundoCapitulo1.transform);
        ConstruirZona5_RuaNorteBar(raizMundoCapitulo1.transform);
        ConstruirInterior4_BarDoZe(raizMundoCapitulo1.transform);

        // 6. Posiciona o jogador na Zona inicial correta no próximo início de frame
        StartCoroutine(PosicionarJogadorInicialRoutine());
    }

    private IEnumerator PosicionarJogadorInicialRoutine()
    {
        yield return null;

        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player == null)
        {
            yield break;
        }

        Vector2 pos = player.transform.position;
        ZonaId zonaDetectada = ZonaId.Zona1_Rodovia;

        // Se o jogador ainda estava nas coordenadas antigas (0..64), move para a Zona 1 (ao lado do carro)
        if (pos.x < 85f)
        {
            player.TeleportarPara(new Vector2(112.0f, 10.2f), 2);
            zonaDetectada = ZonaId.Zona1_Rodovia;
        }
        else
        {
            foreach (var kv in tabelaZonas)
            {
                InfoZona z = kv.Value;
                if (pos.x >= z.min.x - 2f && pos.x <= z.max.x + 2f &&
                    pos.y >= z.min.y - 2f && pos.y <= z.max.y + 2f)
                {
                    zonaDetectada = z.id;
                    break;
                }
            }
        }

        AplicarEstadoDaZona(zonaDetectada, mostrarBanner: true);
    }

    public void TransicionarPara(ZonaId destino, Vector2 posDestino, int direcaoDestino)
    {
        if (!tabelaZonas.TryGetValue(destino, out InfoZona info))
        {
            return;
        }

        SceneFader.Instance.TransicionarParaZona(() =>
        {
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player != null)
            {
                player.TeleportarPara(posDestino, direcaoDestino);
            }

            AplicarEstadoDaZona(destino, mostrarBanner: false);

            if (destino == ZonaId.Interior_CasaDoAvo)
            {
                DispararChegadaCasaDoAvo();
            }
            else if (destino == ZonaId.Interior_BarDoZe)
            {
                GameState.Marcar(Flags.EntrouNoBarDoZe);
                if (!GameState.Tem(Flags.ZeDerrotadoNaSinuca))
                {
                    GameState.DefinirObjetivo("Confrontar Zé na mesa de sinuca (Melhor de 3 — 'A Bola Marcada').");
                }
            }
        }, info.titulo, info.subtitulo);
    }

    private void AplicarEstadoDaZona(ZonaId id, bool mostrarBanner)
    {
        zonaAtual = id;
        if (!tabelaZonas.TryGetValue(id, out InfoZona info))
        {
            return;
        }

        AjustarLuzGlobal(info.ehInterior);

        Chuva chuva = FindAnyObjectByType<Chuva>();
        if (chuva != null)
        {
            chuva.DefinirModoInterior(info.ehInterior);
        }

        CameraFollow camFollow = FindAnyObjectByType<CameraFollow>();
        if (camFollow != null)
        {
            camFollow.DefinirLimitesRegiao(info.min, info.max, info.orthoSize, snapImediato: true);
        }

        if (mostrarBanner && SceneFader.Instance != null)
        {
            SceneFader.Instance.MostrarBannerArea(info.titulo, info.subtitulo);
        }
    }

    private void DispararChegadaCasaDoAvo()
    {
        bool primeiraVez = !GameState.Tem(Flags.ChegouCasaAvo);
        GameState.Marcar(Flags.ChegouCasaAvo);

        if (!primeiraVez)
        {
            return;
        }

        GameState.RegistrarNotaDiario(
            "casa_adilio_entrada",
            "Casa de Adílio destrancada (Nº 40)",
            "FATO",
            "A porta da frente da casa nº 40 estava destrancada e Adílio não respondeu ao chamado. A rotina parece ter sido interrompida hoje mesmo."
        );
        GameState.DefinirObjetivo("Examinar as pistas dentro da casa de Adílio (café, cartas, nota fiscal de 2004 e banco caído).");
        StartCoroutine(NarracaoEntradaCasaAvoRoutine());
    }

    private IEnumerator NarracaoEntradaCasaAvoRoutine()
    {
        yield return new WaitForSeconds(0.8f);
        Legenda.Dizer("\"A porta estava destrancada... Vô Adílio?\"", 3.2f);
        yield return new WaitForSeconds(3.5f);
        Legenda.Dizer("Examine os objetos da casa para descobrir o que aconteceu hoje.", 4.0f);
    }

    private static void AjustarLuzGlobal(bool emInterior)
    {
        GameObject goGlobalLight = GameObject.Find("Global Light 2D");
        if (goGlobalLight != null)
        {
            Light2D l2d = goGlobalLight.GetComponent<Light2D>();
            if (l2d != null)
            {
                l2d.intensity = emInterior ? 0.72f : 0.46f;
                l2d.color = emInterior
                    ? new Color(1.0f, 0.94f, 0.84f, 1f)
                    : new Color(0.76f, 0.82f, 0.94f, 1f);
            }
        }
    }

    private static void AtualizarSpawnPointExistente(string objName, string id, Vector3 pos)
    {
        GameObject go = GameObject.Find(objName);
        if (go == null)
        {
            go = new GameObject(objName);
            SpawnPoint sp = go.AddComponent<SpawnPoint>();
            sp.Configurar(id, 0);
        }
        go.transform.position = pos;
    }

    // =========================================================================
    // GERADOR DE PISO PIXEL-ART POR ZONA / INTERIOR (1 Textura Composta por Área)
    // =========================================================================

    private enum TilePiso
    {
        Grama,
        Terra,
        TerraRachada,
        AsfaltoH_Trinca,
        AsfaltoH_Poca,
        AsfaltoH_Buraco,
        AsfaltoV,
        CalcadaNorte,
        CalcadaSul,
        Concreto,
        Cruzamento,
        AsfaltoParaTerra,
        InteriorSmall,
        InteriorBeige,
        InteriorPatterned,
        InteriorBar,
        ParedeAzulejo,
        ParedePapel,
        ParedeBar
    }

    private static string NomeArquivoTile(TilePiso t)
    {
        switch (t)
        {
            case TilePiso.Grama: return "Grama";
            case TilePiso.Terra: return "Terra";
            case TilePiso.TerraRachada: return "Terra_Rachada";
            case TilePiso.AsfaltoH_Trinca: return "Asfalto_Trinca";
            case TilePiso.AsfaltoH_Poca: return "Asfalto_Poca";
            case TilePiso.AsfaltoH_Buraco: return "Asfalto_Buraco";
            case TilePiso.AsfaltoV: return "Estrada_Asfalto";
            case TilePiso.CalcadaNorte: return "Calcada_Faixa";
            case TilePiso.CalcadaSul: return "Calcada_Faixa";
            case TilePiso.Concreto: return "Concreto";
            case TilePiso.Cruzamento: return "Cruzamento_A";
            case TilePiso.AsfaltoParaTerra: return "Asfalto_ParaTerra";
            case TilePiso.InteriorSmall: return "FloorTile_Small";
            case TilePiso.InteriorBeige: return "FloorTile_Beige";
            case TilePiso.InteriorPatterned: return "FloorTile_Patterned";
            case TilePiso.InteriorBar: return "FloorTile_Patterned";
            case TilePiso.ParedeAzulejo: return "WallTile_BlueBeige";
            case TilePiso.ParedePapel: return "WallPaper_Stained";
            case TilePiso.ParedeBar: return "WallPaper_Stained";
            default: return "Grama";
        }
    }

    private static void CriarPisoDeZona(
        Transform pai,
        string nome,
        Vector2 min,
        int larguraUnidades,
        int alturaUnidades,
        Func<int, int, TilePiso> seletorTile)
    {
        // Um tile de 32 px equivale a uma unidade. Antes cada pequeno piso
        // ocupava duas unidades, produzindo o xadrez gigante visto no jogo.
        int cols = larguraUnidades;
        int rows = alturaUnidades;
        const int cellPx = 32;

        Texture2D texFinal = new Texture2D(cols * cellPx, rows * cellPx, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            name = "Piso_" + nome
        };

        var cacheBlocos = new Dictionary<TilePiso, Color32[]>();

        for (int cy = 0; cy < rows; cy++)
        {
            for (int cx = 0; cx < cols; cx++)
            {
                int ux = cx;
                int uy = cy;
                TilePiso tipo = seletorTile(ux, uy);

                if (!cacheBlocos.TryGetValue(tipo, out Color32[] bloco))
                {
                    bloco = ExtrairBloco32(tipo);
                    cacheBlocos[tipo] = bloco;
                }

                int baseX = cx * cellPx;
                int baseY = cy * cellPx;
                for (int py = 0; py < cellPx; py++)
                {
                    for (int px = 0; px < cellPx; px++)
                    {
                        texFinal.SetPixel(baseX + px, baseY + py, bloco[py * cellPx + px]);
                    }
                }
            }
        }

        // Sinalização no pavimento, pintada só sobre o asfalto. O cruzamento
        // continua livre para o jogador distinguir as duas vias.
        if (nome.StartsWith("Zona1_") || nome.StartsWith("Zona2_") ||
            nome.StartsWith("Zona3_") || nome.StartsWith("Zona5_"))
        {
            for (int px = 0; px < texFinal.width; px++)
            {
                int ux = px / cellPx;
                for (int py = 0; py < texFinal.height; py++)
                {
                    int uy = py / cellPx;
                    TilePiso piso = seletorTile(ux, uy);
                    if (piso != TilePiso.AsfaltoH_Trinca && piso != TilePiso.AsfaltoH_Poca &&
                        piso != TilePiso.Cruzamento) continue;
                    float yUnidade = py / 32f;
                    bool borda = Mathf.Abs(yUnidade - 8.08f) < 0.045f || Mathf.Abs(yUnidade - 11.92f) < 0.045f;
                    bool eixo = Mathf.Abs(yUnidade - 9.5f) < 0.055f && (px / 96) % 2 == 0;
                    // Travessia de pedestres diante do mercadinho, na avenida da encruzilhada.
                    bool faixaPedestres = nome == "Zona3_Encruzilhada" &&
                        px >= 10 * cellPx && px < 14 * cellPx &&
                        yUnidade > 8.23f && yUnidade < 11.77f && (px / 14) % 2 == 0;
                    if (faixaPedestres || borda) texFinal.SetPixel(px, py, new Color(0.86f, 0.84f, 0.72f));
                    else if (eixo) texFinal.SetPixel(px, py, new Color(0.92f, 0.73f, 0.30f));
                }
            }
        }
        texFinal.Apply(false, false);

        GameObject goPiso = new GameObject("Piso_" + nome);
        goPiso.transform.SetParent(pai, false);
        goPiso.transform.position = new Vector3(min.x, min.y, 0f);

        SpriteRenderer sr = goPiso.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(
            texFinal,
            new Rect(0, 0, texFinal.width, texFinal.height),
            Vector2.zero,
            32f,
            0,
            SpriteMeshType.FullRect
        );
        sr.sortingOrder = -20;
    }

    private static Color32[] ExtrairBloco32(TilePiso tipo)
    {
        const int cellPx = 32;
        Color32[] result = new Color32[cellPx * cellPx];
        bool asfalto = tipo == TilePiso.AsfaltoH_Trinca || tipo == TilePiso.AsfaltoH_Poca ||
            tipo == TilePiso.AsfaltoH_Buraco || tipo == TilePiso.AsfaltoV || tipo == TilePiso.Cruzamento;
        bool interior = tipo == TilePiso.InteriorSmall || tipo == TilePiso.InteriorBeige ||
            tipo == TilePiso.InteriorPatterned || tipo == TilePiso.InteriorBar ||
            tipo == TilePiso.ParedeAzulejo || tipo == TilePiso.ParedePapel || tipo == TilePiso.ParedeBar;
        if (asfalto || interior)
        {
            Color32 baseCor = asfalto ? new Color32(47, 51, 52, 255) :
                tipo == TilePiso.InteriorSmall ? new Color32(72, 81, 81, 255) :
                tipo == TilePiso.InteriorBeige ? new Color32(116, 91, 68, 255) :
                tipo == TilePiso.InteriorPatterned ? new Color32(91, 68, 57, 255) :
                tipo == TilePiso.InteriorBar ? new Color32(67, 51, 43, 255) :
                tipo == TilePiso.ParedeAzulejo ? new Color32(121, 139, 135, 255) :
                tipo == TilePiso.ParedeBar ? new Color32(80, 59, 46, 255) :
                new Color32(157, 132, 97, 255);
            for (int y = 0; y < cellPx; y++)
                for (int x = 0; x < cellPx; x++)
                {
                    int ruido = ((x * 17 + y * 31 + x * y * 3) % 11) - 5;
                    bool junta = interior && (x == 0 || y == 0);
                    int brilho = ruido / 3 - (junta ? 17 : 0);
                    result[y * cellPx + x] = new Color32(
                        (byte)Mathf.Clamp(baseCor.r + brilho, 0, 255),
                        (byte)Mathf.Clamp(baseCor.g + brilho, 0, 255),
                        (byte)Mathf.Clamp(baseCor.b + brilho, 0, 255), 255);
                }
            return result;
        }
        Texture2D src = CarregarTextura(NomeArquivoTile(tipo));
        bool flipY = (tipo == TilePiso.CalcadaSul);

        if (src == null)
        {
            Color32 fallback = new Color32(45, 52, 42, 255);
            for (int i = 0; i < result.Length; i++) result[i] = fallback;
            return result;
        }

        int sw = src.width;
        int sh = src.height;
        for (int y = 0; y < cellPx; y++)
        {
            int sy = flipY ? (cellPx - 1 - y) : y;
            int ty = Mathf.Clamp( sy * sh / cellPx, 0, sh - 1);
            for (int x = 0; x < cellPx; x++)
            {
                int tx = Mathf.Clamp(x * sw / cellPx, 0, sw - 1);
                Color c = src.GetPixel(tx, ty);
                if (c.a < 0.5f)
                {
                    // Se o tile tiver canto transparente (ex.: Cruzamento_A), preenche com concreto/terra
                    c = new Color(0.36f, 0.34f, 0.31f, 1f);
                }
                result[y * cellPx + x] = c;
            }
        }

        return result;
    }

    private static void CriarParedesPerimetro(Transform pai, Vector2 min, Vector2 max, float espessura = 1.2f)
    {
        GameObject go = new GameObject("ColisoresPerimetro");
        go.transform.SetParent(pai, false);

        float w = max.x - min.x;
        float h = max.y - min.y;
        Vector2 centro = (min + max) * 0.5f;

        // Sul
        AdicionarBoxCollider(go, new Vector2(centro.x, min.y + espessura * 0.5f), new Vector2(w + 4f, espessura));
        // Norte
        AdicionarBoxCollider(go, new Vector2(centro.x, max.y - espessura * 0.5f), new Vector2(w + 4f, espessura));
        // Oeste
        AdicionarBoxCollider(go, new Vector2(min.x + espessura * 0.5f, centro.y), new Vector2(espessura, h + 4f));
        // Leste
        AdicionarBoxCollider(go, new Vector2(max.x - espessura * 0.5f, centro.y), new Vector2(espessura, h + 4f));
    }

    private static BoxCollider2D AdicionarBoxCollider(GameObject go, Vector2 centroMundo, Vector2 tamanho, bool isTrigger = false)
    {
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.offset = centroMundo - (Vector2)go.transform.position;
        col.size = tamanho;
        col.isTrigger = isTrigger;
        return col;
    }

    private static GameObject ColocarProp(
        Transform pai,
        string nome,
        Vector3 pos,
        string spriteName,
        float ppu,
        float escala,
        Vector2 colSize,
        Vector2 colOffset,
        int sortingOrder = 0,
        Vector2? pivotCustom = null)
    {
        Vector2 pivot = pivotCustom ?? new Vector2(0.5f, 0f);
        Sprite spr = CarregarSprite(spriteName, ppu, pivot);

        GameObject go = new GameObject(nome);
        go.transform.SetParent(pai, false);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(escala, escala, 1f);

        if (spr != null)
        {
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr;
            sr.sortingOrder = sortingOrder;
        }

        if (colSize.x > 0.01f && colSize.y > 0.01f)
        {
            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.size = colSize;
            col.offset = colOffset;
            col.isTrigger = false;
        }

        return go;
    }

    private enum MovelLoja { CaixaPosto, CaixaMercado, Geladeira, Prateleira, Hortifruti }

    private static GameObject ColocarArteInterior(Transform pai, string nome, Vector2 pos,
        string arquivoArte, string spriteReserva, float escala, Vector2 colisaoMundo)
    {
        string caminho = Path.Combine(Application.streamingAssetsPath, arquivoArte + ".png");
        if (!File.Exists(caminho))
            return ColocarProp(pai, nome, pos, spriteReserva, 32f, escala,
                colisaoMundo / escala, new Vector2(0f, colisaoMundo.y / (2f * escala)), 2);

        Texture2D arte = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!ImageConversion.LoadImage(arte, File.ReadAllBytes(caminho)))
            return ColocarProp(pai, nome, pos, spriteReserva, 32f, escala,
                colisaoMundo / escala, new Vector2(0f, colisaoMundo.y / (2f * escala)), 2);
        arte.filterMode = FilterMode.Point;
        GameObject go = new GameObject(nome);
        go.transform.SetParent(pai, false);
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * escala;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(arte, new Rect(0, 0, arte.width, arte.height),
            new Vector2(0.5f, 0f), 32f, 0, SpriteMeshType.FullRect);
        sr.sortingOrder = 2;
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = colisaoMundo / escala;
        col.offset = new Vector2(0f, colisaoMundo.y / (2f * escala));
        return go;
    }

    private static void ColocarTapete(Transform pai, string nome, Vector2 pos,
        Vector2 tamanho, Color32 borda, Color32 centro)
    {
        const int w = 64, h = 40;
        Color32[] trama = new Color32[w * h];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                bool moldura = x < 4 || x >= w - 4 || y < 4 || y >= h - 4 ||
                    x == 7 || x == w - 8 || y == 7 || y == h - 8;
                trama[y * w + x] = moldura ? borda : centro;
            }
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point, name = nome + "_Trama"
        };
        tex.SetPixels32(trama);
        tex.Apply(false, false);
        GameObject go = new GameObject(nome);
        go.transform.SetParent(pai, false);
        go.transform.position = pos;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), w / tamanho.x);
        go.transform.localScale = new Vector3(1f, tamanho.y / (h * tamanho.x / w), 1f);
        sr.sortingOrder = -10;
    }

    private static void ColocarMovelLoja(Transform pai, string nome, Vector2 pos,
        MovelLoja tipo, float larguraMundo = 3.4f)
    {
        string arquivoArte = tipo == MovelLoja.CaixaPosto ? "Loja_CaixaPosto" :
            tipo == MovelLoja.CaixaMercado ? "Loja_CaixaMercado" :
            tipo == MovelLoja.Geladeira ? "Loja_Geladeira" :
            tipo == MovelLoja.Prateleira ? "Loja_Prateleira" : "Loja_Hortifruti";
        if (!string.IsNullOrEmpty(arquivoArte))
        {
            string caminho = Path.Combine(Application.streamingAssetsPath, arquivoArte + ".png");
            if (File.Exists(caminho))
            {
                Texture2D arte = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                if (ImageConversion.LoadImage(arte, File.ReadAllBytes(caminho)))
                {
                    arte.filterMode = FilterMode.Point;
                    GameObject movel = new GameObject(nome);
                    movel.transform.SetParent(pai, false);
                    movel.transform.position = pos;
                    movel.transform.localScale = Vector3.one * (larguraMundo / 3.4f);
                    SpriteRenderer render = movel.AddComponent<SpriteRenderer>();
                    render.sprite = Sprite.Create(arte, new Rect(0, 0, arte.width, arte.height),
                        new Vector2(0.5f, 0f), 32f, 0, SpriteMeshType.FullRect);
                    render.sortingOrder = 2;
                    BoxCollider2D colisao = movel.AddComponent<BoxCollider2D>();
                    colisao.size = new Vector2(render.sprite.bounds.size.x * 0.86f, 0.85f);
                    colisao.offset = new Vector2(0f, 0.43f);
                    return;
                }
            }
        }

        const int w = 96, h = 64;
        Color32[] pixels = new Color32[w * h];
        void Pintar(int x0, int y0, int dx, int dy, Color32 cor)
        {
            for (int y = Mathf.Max(0, y0); y < Mathf.Min(h, y0 + dy); y++)
                for (int x = Mathf.Max(0, x0); x < Mathf.Min(w, x0 + dx); x++)
                    pixels[y * w + x] = cor;
        }
        Color32 escuro = new Color32(30, 30, 28, 255);
        Color32 madeira = new Color32(104, 70, 46, 255);
        Color32 claro = new Color32(207, 174, 119, 255);
        if (tipo == MovelLoja.CaixaPosto || tipo == MovelLoja.CaixaMercado)
        {
            bool posto = tipo == MovelLoja.CaixaPosto;
            Pintar(2, 4, 92, 31, escuro);
            Pintar(5, 7, 86, 24, posto ? new Color32(44, 83, 83, 255) : madeira);
            for (int x = 9; x < 89; x += 25) Pintar(x, 9, 3, 18, escuro);
            Pintar(3, 32, 90, 9, posto ? new Color32(174, 190, 177, 255) : claro);
            Pintar(6, 41, 84, 3, new Color32(242, 225, 174, 255));
            // Registradora, bobina e produtos ao lado do caixa.
            Pintar(61, 44, 23, 12, escuro);
            Pintar(64, 51, 16, 5, new Color32(111, 144, 130, 255));
            Pintar(13, 44, 8, 9, new Color32(202, 87, 56, 255));
            Pintar(25, 44, 8, 9, new Color32(220, 180, 67, 255));
            Pintar(37, 44, 8, 9, new Color32(76, 143, 93, 255));
        }
        else if (tipo == MovelLoja.Geladeira)
        {
            Pintar(5, 3, 86, 60, escuro);
            Pintar(9, 7, 78, 51, new Color32(114, 157, 155, 255));
            Pintar(12, 10, 72, 45, new Color32(31, 60, 64, 255));
            for (int y = 15; y < 50; y += 15)
            {
                Pintar(12, y - 2, 72, 3, new Color32(176, 198, 184, 255));
                for (int x = 17; x < 79; x += 12)
                {
                    Pintar(x, y + 2, 7, 10, (x + y) % 3 == 0 ?
                        new Color32(198, 73, 52, 255) : new Color32(213, 188, 84, 255));
                    Pintar(x + 1, y + 9, 5, 2, new Color32(234, 227, 188, 255));
                }
            }
            Pintar(47, 8, 3, 48, new Color32(194, 212, 201, 255));
            Pintar(83, 25, 3, 14, new Color32(222, 222, 201, 255));
        }
        else
        {
            bool fruta = tipo == MovelLoja.Hortifruti;
            Pintar(3, 4, 90, 52, escuro);
            Pintar(7, 8, 82, 44, madeira);
            for (int y = 14; y < 48; y += 13)
            {
                Pintar(9, y, 78, 4, claro);
                for (int x = 12; x < 84; x += 10)
                    Pintar(x, y + 4, 7, 7, fruta ?
                        ((x + y) % 3 == 0 ? new Color32(186, 69, 49, 255) : new Color32(105, 156, 71, 255)) :
                        ((x + y) % 3 == 0 ? new Color32(188, 73, 55, 255) : new Color32(193, 174, 96, 255)));
            }
            Pintar(5, 54, 86, 5, claro);
        }

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp, name = nome + "_Arte"
        };
        tex.SetPixels32(pixels);
        tex.Apply(false, false);
        GameObject go = new GameObject(nome);
        go.transform.SetParent(pai, false);
        go.transform.position = pos;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0f), w / larguraMundo,
            0, SpriteMeshType.FullRect);
        sr.sortingOrder = 2;
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(larguraMundo * 0.88f, 0.85f);
        col.offset = new Vector2(0f, 0.43f);
    }

    private static GameObject ColocarPosteComLuz(Transform pai, string nome, Vector3 pos, Color corLuz, float raio = 5.5f, float intensidade = 1.15f)
    {
        GameObject go = ColocarProp(pai, nome, pos, "StreetLamp", 32f, 1.0f, new Vector2(0.35f, 0.35f), new Vector2(0f, 0.2f), 2);
        AdicionarLuzPonto(go.transform, new Vector3(0f, 2.2f, 0f), corLuz, raio, intensidade);
        return go;
    }

    private static void AdicionarLuzPonto(Transform pai, Vector3 localPos, Color cor, float raioExterno, float intensidade)
    {
        GameObject luzGo = new GameObject("PointLight2D");
        luzGo.transform.SetParent(pai, false);
        luzGo.transform.localPosition = localPos;

        Light2D luz = luzGo.AddComponent<Light2D>();
        luz.lightType = Light2D.LightType.Point;
        luz.color = cor;
        luz.intensity = intensidade;
        luz.pointLightInnerRadius = raioExterno * 0.25f;
        luz.pointLightOuterRadius = raioExterno;
        luz.falloffIntensity = 0.55f;
    }

    private static InteracaoCapitulo1 ColocarInteracao(
        Transform pai,
        string nome,
        Vector3 pos,
        InteracaoCapitulo1.TipoInteracao tipo,
        string prompt = "",
        string spriteName = null,
        float ppu = 32f,
        float escala = 1f,
        int sortingOrder = 0,
        bool solido = true)
    {
        GameObject go = new GameObject(nome);
        go.transform.SetParent(pai, false);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(escala, escala, 1f);

        if (!string.IsNullOrEmpty(spriteName))
        {
            Sprite spr = CarregarSprite(spriteName, ppu, new Vector2(0.5f, 0.08f));
            if (spr != null)
            {
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = spr;
                sr.sortingOrder = sortingOrder;
            }
        }

        if (solido && !string.IsNullOrEmpty(spriteName))
        {
            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.85f, 0.55f);
            col.offset = new Vector2(0f, 0.28f);
        }

        InteracaoCapitulo1 inter = go.AddComponent<InteracaoCapitulo1>();
        inter.Configurar(tipo, prompt);
        return inter;
    }

    private static TransicaoDeZonaTrigger ColocarGatilhoTransicao(
        Transform pai,
        string nome,
        Vector3 pos,
        Vector2 triggerSize,
        ZonaId destino,
        Vector2 posDestino,
        int dirDestino,
        string prompt,
        bool automatico,
        bool exigirCasaAvo = false)
    {
        GameObject go = new GameObject(nome);
        go.transform.SetParent(pai, false);
        go.transform.position = pos;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = triggerSize;
        col.isTrigger = true;

        TransicaoDeZonaTrigger trig = go.AddComponent<TransicaoDeZonaTrigger>();
        trig.Configurar(destino, posDestino, dirDestino, prompt, automatico, exigirCasaAvo);
        return trig;
    }

    private void ColocarOrelhaoSave(Transform pai, string nome, string checkpointId, Vector3 pos)
    {
        if (templatePayphone != null)
        {
            GameObject clone = Instantiate(templatePayphone.gameObject, pai);
            clone.name = nome;
            clone.SetActive(true);
            clone.transform.position = pos;
            PayphoneCheckpoint pp = clone.GetComponent<PayphoneCheckpoint>();
            if (pp != null)
            {
                pp.ConfigurarId(checkpointId);
            }
            return;
        }

        ColocarProp(pai, nome, pos, "PhoneBooth", 32f, 1.0f, new Vector2(0.9f, 0.6f), new Vector2(0f, 0.3f), 1);
    }

    // =========================================================================
    // ZONA 1: RODOVIA DE ENTRADA (Chegada de Marcus, Asfalto + Barro na Borda)
    // =========================================================================

    private void ConstruirZona1_Rodovia(Transform raiz)
    {
        GameObject z = new GameObject("Zona1_Rodovia");
        z.transform.SetParent(raiz, false);
        Vector2 min = new Vector2(100f, 0f);
        Vector2 max = new Vector2(136f, 20f);

        CriarPisoDeZona(z.transform, "Zona1_Rodovia", min, 36, 20, (ux, uy) =>
        {
            // Rodovia de asfalto central (Y = 8..11)
            if (uy >= 8 && uy <= 11) return TilePiso.AsfaltoH_Trinca;
            // Recuo pavimentado para o carro inteiro, fora da faixa de rolamento.
            if (uy >= 12 && uy <= 15 && ux >= 6 && ux <= 12) return TilePiso.Concreto;
            if (uy == 12) return TilePiso.CalcadaNorte;
            // Acostamento de barro e lama nas margens da rodovia (como pedido pelo usuário)
            if (uy == 6 || uy == 14) return (ux % 4 == 0) ? TilePiso.TerraRachada : TilePiso.Terra;
            return TilePiso.Grama;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.5f);

        // Carro de Marcus estacionado no recuo pavimentado da rodovia (nunca na grama!)
        ColocarProp(z.transform, "Carro_Marcus_Sprite", new Vector3(108.5f, 12.2f, 0f), "Sedan_Brown", 32f, 1.15f,
            new Vector2(2.6f, 1.2f), new Vector2(0f, 0.6f), 2);
        ColocarInteracao(z.transform, "Carro_Marcus_Interacao", new Vector3(108.5f, 11.4f, 0f),
            InteracaoCapitulo1.TipoInteracao.CarroMarcus, "Examinar meu carro");

        // Placa de Entrada de Santa Rita
        ColocarProp(z.transform, "Placa_SantaRita", new Vector3(113.5f, 13.4f, 0f), "PlacaEntrada", 16f, 0.9f,
            new Vector2(2.4f, 0.8f), new Vector2(0f, 0.4f), 2);

        // Abrigo / Parada de Ônibus na Rodovia + Quadro de Avisos
        ColocarProp(z.transform, "Banco_Rodovia", new Vector3(119.5f, 13.4f, 0f), "Bench_Wood", 32f, 1.1f,
            new Vector2(1.6f, 0.6f), new Vector2(0f, 0.3f), 1);
        ColocarProp(z.transform, "Lixeira_Rodovia", new Vector3(121.2f, 13.4f, 0f), "TrashCan", 32f, 1.0f,
            new Vector2(0.6f, 0.5f), new Vector2(0f, 0.25f), 1);
        ColocarProp(z.transform, "Placa_Horarios_Rodovia", new Vector3(119.5f, 14.1f, 0f), "RoadSign_Blank", 32f, 1.1f,
            Vector2.zero, Vector2.zero, 1);
        ColocarInteracao(z.transform, "Interacao_PlacaRodovia", new Vector3(119.5f, 12.9f, 0f),
            InteracaoCapitulo1.TipoInteracao.PlacaRodoviaEntrada, "Ler quadro da parada de ônibus");

        // Orelhão #1 (Save Point da Entrada)
        ColocarOrelhaoSave(z.transform, "Orelhao_Rodovia", "orelhao_entrada_rodovia", new Vector3(126.5f, 13.3f, 0f));

        // Pista na margem sul de barro: Mala e caixotes abandonados
        ColocarProp(z.transform, "Caixotes_Barro_Sul", new Vector3(122.5f, 6.6f, 0f), "Caixotes", 16f, 0.85f,
            new Vector2(1.4f, 0.8f), new Vector2(0f, 0.4f), 1);
        ColocarInteracao(z.transform, "Interacao_MalaEstrada", new Vector3(122.5f, 7.3f, 0f),
            InteracaoCapitulo1.TipoInteracao.MalaAbandonadaEstrada, "Examinar mala caída no barro");

        // Postes de luz e fiação ao longo da rodovia
        Color luzAmbar = new Color(1.0f, 0.82f, 0.52f, 1f);
        ColocarPosteComLuz(z.transform, "Poste_Z1_1", new Vector3(106.0f, 13.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z1_2", new Vector3(116.0f, 13.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z1_3", new Vector3(125.0f, 13.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z1_4", new Vector3(133.0f, 13.2f, 0f), luzAmbar);
        ColocarProp(z.transform, "UtilityPole_Z1_1", new Vector3(111.0f, 13.5f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);
        ColocarProp(z.transform, "UtilityPole_Z1_2", new Vector3(129.5f, 13.5f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);

        // Cerca de madeira na margem sul e seta indicando a saída Leste para o Posto (Zona 2)
        for (int x = 104; x <= 130; x += 6)
        {
            ColocarProp(z.transform, "CercaSul_" + x, new Vector3(x, 5.4f, 0f), "WoodFence", 32f, 1.1f, new Vector2(2.2f, 0.5f), new Vector2(0f, 0.25f), 1);
        }
        ColocarProp(z.transform, "Seta_Para_Posto", new Vector3(133.5f, 12.8f, 0f), "ArrowSign", 32f, 1.15f, Vector2.zero, Vector2.zero, 2);

        // Gatilho de corte de cena (Leste -> Zona 2: Posto de Gasolina)
        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Z1_Para_Z2",
            new Vector3(134.4f, 10.0f, 0f),
            new Vector2(2.2f, 5.5f),
            ZonaId.Zona2_Posto,
            new Vector2(203.0f, 10.0f),
            3,
            "Seguir pela Avenida -> Posto de Gasolina",
            automatico: true
        );
    }

    // =========================================================================
    // ZONA 2: POSTO DE GASOLINA & ANTIGA VENDA (Idêntico à Imagem 3!)
    // =========================================================================

    private void ConstruirZona2_Posto(Transform raiz)
    {
        GameObject z = new GameObject("Zona2_Posto");
        z.transform.SetParent(raiz, false);
        Vector2 min = new Vector2(200f, 0f);
        Vector2 max = new Vector2(238f, 22f);

        CriarPisoDeZona(z.transform, "Zona2_Posto", min, 38, 22, (ux, uy) =>
        {
            // Avenida de Asfalto (Y = 8..11)
            if (uy >= 8 && uy <= 11) return TilePiso.AsfaltoH_Trinca;
            // Calçada Sul (Y = 6)
            if (uy == 6) return TilePiso.CalcadaSul;
            // Calçada Norte e Pátio de Concreto do Posto e da Venda (Y = 12..16)
            if (uy == 12) return TilePiso.CalcadaNorte;
            if (uy >= 14 && uy <= 16 && ux >= 6 && ux <= 32) return TilePiso.Concreto;
            return TilePiso.Grama;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.5f);

        // 1. POSTO DE GASOLINA (À esquerda na calçada norte, como na Imagem 3)
        ColocarProp(z.transform, "Predio_Posto", new Vector3(212.0f, 14.2f, 0f), "Posto_Cobertura", 16f, 0.92f,
            new Vector2(6.4f, 2.2f), new Vector2(0f, 1.6f), 2);
        AdicionarLuzPonto(z.transform, new Vector3(212.0f, 15.2f, 0f), new Color(1.0f, 0.88f, 0.65f, 1f), 7.0f, 1.35f);

        // Porta para entrar no INTERIOR DA CONVENIÊNCIA DO POSTO (Frentista está lá dentro!)
        ColocarGatilhoTransicao(
            z.transform,
            "Porta_Entrar_Posto",
            new Vector3(211.6f, 13.8f, 0f),
            new Vector2(1.8f, 1.4f),
            ZonaId.Interior_Posto,
            new Vector2(208.0f, -57.2f),
            1,
            "Entrar na Conveniência do Posto",
            automatico: false
        );

        // Bomba de combustível e picape estacionada no pátio de concreto do posto
        ColocarInteracao(z.transform, "Bomba_Posto_Interacao", new Vector3(214.0f, 13.8f, 0f),
            InteracaoCapitulo1.TipoInteracao.BombaCombustivelPosto, "Examinar bomba de combustível");

        ColocarProp(z.transform, "Picape_No_Posto", new Vector3(207.8f, 13.6f, 0f), "Picape_Red", 32f, 1.15f,
            new Vector2(2.5f, 1.2f), new Vector2(0f, 0.6f), 2);
        ColocarInteracao(z.transform, "Picape_Posto_Interacao", new Vector3(207.8f, 12.8f, 0f),
            InteracaoCapitulo1.TipoInteracao.CarroEstacionadoPosto, "Examinar picape no posto");

        // Carro estacionado corretamente no meio-fio do asfalto
        ColocarProp(z.transform, "Sedan_Avenida_Posto", new Vector3(220.5f, 11.4f, 0f), "Sedan_Black", 32f, 1.15f,
            new Vector2(2.5f, 1.1f), new Vector2(0f, 0.55f), 2);

        // 2. ANTIGA VENDA (À direita do Posto na mesma avenida, idêntico à Imagem 3!)
        ColocarProp(z.transform, "Predio_Venda_Antiga", new Vector3(228.5f, 14.2f, 0f), "Venda", 16f, 0.90f,
            new Vector2(6.2f, 2.2f), new Vector2(0f, 1.5f), 2);
        ColocarProp(z.transform, "Caixotes_Venda", new Vector3(224.8f, 14.0f, 0f), "Caixotes", 16f, 0.85f,
            new Vector2(1.2f, 0.8f), new Vector2(0f, 0.4f), 2);
        ColocarProp(z.transform, "Banco_Venda", new Vector3(232.2f, 13.8f, 0f), "Bench_Wood", 32f, 1.05f,
            new Vector2(1.5f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarInteracao(z.transform, "Porta_Venda_Antiga", new Vector3(228.5f, 13.6f, 0f),
            InteracaoCapitulo1.TipoInteracao.VendaAntigaFechada, "Examinar antiga Venda");

        // Postes de luz e fiação na calçada (Imagem 3)
        Color luzAmbar = new Color(1.0f, 0.82f, 0.52f, 1f);
        ColocarPosteComLuz(z.transform, "Poste_Z2_1", new Vector3(204.5f, 13.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z2_2", new Vector3(217.5f, 13.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z2_3", new Vector3(224.0f, 13.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z2_4", new Vector3(234.0f, 13.2f, 0f), luzAmbar);
        ColocarProp(z.transform, "UtilityPole_Z2_1", new Vector3(216.0f, 13.6f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);
        ColocarProp(z.transform, "UtilityPole_Z2_2", new Vector3(233.0f, 13.6f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);

        // Gatilhos de Transição Oeste (Zona 1) e Leste (Zona 3: Encruzilhada & Mercadinho)
        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Z2_Para_Z1",
            new Vector3(201.4f, 10.0f, 0f),
            new Vector2(2.0f, 5.5f),
            ZonaId.Zona1_Rodovia,
            new Vector2(132.8f, 10.0f),
            2,
            "<- Voltar para a Rodovia de Entrada",
            automatico: true
        );

        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Z2_Para_Z3",
            new Vector3(236.5f, 10.0f, 0f),
            new Vector2(2.0f, 5.5f),
            ZonaId.Zona3_Encruzilhada,
            new Vector2(303.0f, 10.5f),
            3,
            "Seguir pela Avenida -> Encruzilhada & Mercadinho",
            automatico: true
        );
    }

    // =========================================================================
    // INTERIOR 1: CONVENIÊNCIA DO POSTO (Com o Frentista lá dentro!)
    // =========================================================================

    private void ConstruirInterior1_Posto(Transform raiz)
    {
        GameObject z = new GameObject("Interior1_Posto");
        z.transform.SetParent(raiz, false);
        Vector2 min = new Vector2(200f, -60f);
        Vector2 max = new Vector2(216f, -48f);

        CriarPisoDeZona(z.transform, "Interior_Posto", min, 16, 12, (ux, uy) =>
        {
            if (uy >= 10) return TilePiso.ParedeAzulejo;
            return TilePiso.InteriorSmall;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.0f);
        AdicionarBoxCollider(z, new Vector2(208f, -49.2f), new Vector2(16f, 1.6f));

        // Iluminação interna quente
        AdicionarLuzPonto(z.transform, new Vector3(208.0f, -53.5f, 0f), new Color(1.0f, 0.94f, 0.80f, 1f), 10.0f, 1.1f);
        ColocarTapete(z.transform, "Capacho_Posto", new Vector2(208f, -57.6f),
            new Vector2(2.6f, 1.2f), new Color32(37, 45, 43, 255), new Color32(104, 115, 99, 255));

        // Porta de saída no sul da sala
        ColocarProp(z.transform, "PortaFrame_Posto", new Vector3(208.0f, -59.2f, 0f), "DoorFrame", 32f, 1.2f, Vector2.zero, Vector2.zero, 1);
        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Interior_Posto",
            new Vector3(208.0f, -58.4f, 0f),
            new Vector2(2.2f, 1.4f),
            ZonaId.Zona2_Posto,
            new Vector2(211.6f, 12.6f),
            0,
            "Sair para o Posto de Gasolina",
            automatico: false
        );

        // Conveniência compacta: geladeira de bebidas, salgadinhos e caixa.
        // O corredor da porta até o frentista permanece livre.
        ColocarMovelLoja(z.transform, "Geladeira_Bebidas_Posto", new Vector2(203.1f, -51.2f), MovelLoja.Geladeira);
        ColocarMovelLoja(z.transform, "Prateleira_Salgadinhos_Posto", new Vector2(212.9f, -51.2f), MovelLoja.Prateleira);
        ColocarMovelLoja(z.transform, "Prateleira_Viagem_Posto", new Vector2(212.9f, -54.1f), MovelLoja.Prateleira, 2.5f);
        ColocarProp(z.transform, "Caixas_Posto", new Vector3(213.4f, -55.5f, 0f), "CardboardBoxes_Stack", 32f, 1.15f, new Vector2(1.2f, 0.8f), new Vector2(0f, 0.4f), 2);
        ColocarProp(z.transform, "Refrigerantes_Posto", new Vector3(203.2f, -55.3f, 0f), "BottlesCrate", 32f, 1.25f,
            new Vector2(1.1f, 0.7f), new Vector2(0f, 0.35f), 2);
        ColocarProp(z.transform, "Ventilador_Posto", new Vector3(211.8f, -49.0f, 0f), "Fan_Wall", 32f, 1.1f,
            Vector2.zero, Vector2.zero, 3);
        ColocarProp(z.transform, "Relogio_Posto", new Vector3(208.0f, -49.2f, 0f), "WallClock", 32f, 1.1f, Vector2.zero, Vector2.zero, 3);

        // Balcão de atendimento + TV
        ColocarMovelLoja(z.transform, "Caixa_Conveniência_Posto", new Vector2(208.0f, -52.8f), MovelLoja.CaixaPosto, 3.8f);
        ColocarProp(z.transform, "TV_Posto", new Vector3(209.6f, -52.1f, 0f), "TV_Old", 32f, 1.0f, Vector2.zero, Vector2.zero, 3);

        // FRENTISTA dentro da loja de conveniência atrás do balcão!
        ColocarInteracao(z.transform, "NPC_Frentista_Interior", new Vector3(208.0f, -51.5f, 0f),
            InteracaoCapitulo1.TipoInteracao.NpcFrentista, "Conversar com o Frentista", "Npc_Frentista", 32f, 1.0f, 2, solido: true);

        // Interações extras no balcão do Posto
        ColocarInteracao(z.transform, "Livro_Plantao_Posto", new Vector3(206.5f, -53.0f, 0f),
            InteracaoCapitulo1.TipoInteracao.LivroRegistroPosto, "Ler livro de plantão do Posto");
        ColocarInteracao(z.transform, "TV_Portatil_Posto", new Vector3(209.6f, -53.0f, 0f),
            InteracaoCapitulo1.TipoInteracao.TvPortatilPosto, "Olhar TV portátil");
    }

    // =========================================================================
    // ZONA 3: ENCRUZILHADA & MERCADINHO (Idêntico à Imagem 4!)
    // =========================================================================

    private void ConstruirZona3_Encruzilhada(Transform raiz)
    {
        GameObject z = new GameObject("Zona3_Encruzilhada");
        z.transform.SetParent(raiz, false);
        Vector2 min = new Vector2(300f, 0f);
        Vector2 max = new Vector2(340f, 26f);

        CriarPisoDeZona(z.transform, "Zona3_Encruzilhada", min, 40, 26, (ux, uy) =>
        {
            // Cruzamento central (X = 18..24, Y = 8..12)
            if (ux >= 18 && ux <= 22 && uy >= 8 && uy <= 13) return TilePiso.Cruzamento;
            // O ramal de barro encontra o asfalto sem faixas de grama entre os dois.
            if (ux >= 18 && ux <= 22 && uy == 14) return TilePiso.AsfaltoParaTerra;
            if (ux >= 17 && ux <= 23 && uy >= 15) return (uy % 5 == 0) ? TilePiso.TerraRachada : TilePiso.Terra;
            // Avenida de Asfalto Horizontal (Y = 8..11)
            if (uy >= 8 && uy <= 11) return TilePiso.AsfaltoH_Trinca;
            // Calçada Sul (Y = 6) e Calçada Norte / Pátio do Mercadinho (Y = 12..16)
            if (uy == 6) return TilePiso.CalcadaSul;
            if (uy == 12) return TilePiso.CalcadaNorte;
            if (uy >= 14 && uy <= 16 && (ux <= 16 || ux >= 24)) return TilePiso.Concreto;
            return TilePiso.Grama;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.5f);

        // 1. MERCADINHO DE 2 ANDARES COM TOLDO LISTRADO AZUL E BRANCO (Imagem 4!)
        ColocarProp(z.transform, "Predio_Mercadinho", new Vector3(311.0f, 13.8f, 0f), "Fachada_Mercadinho", 16f, 0.95f,
            new Vector2(7.2f, 2.4f), new Vector2(0f, 1.6f), 2);
        AdicionarLuzPonto(z.transform, new Vector3(311.0f, 14.5f, 0f), new Color(1.0f, 0.88f, 0.62f, 1f), 7.2f, 1.3f);

        // Porta para entrar no INTERIOR DO MERCADINHO (Atendente está lá dentro!)
        ColocarGatilhoTransicao(
            z.transform,
            "Porta_Entrar_Mercadinho",
            new Vector3(311.0f, 13.4f, 0f),
            new Vector2(2.0f, 1.4f),
            ZonaId.Interior_Mercadinho,
            new Vector2(309.0f, -57.2f),
            1,
            "Entrar no Mercadinho Santa Rita",
            automatico: false
        );

        // Engradados de bebida e lixeira ao lado do Mercadinho (Imagem 4)
        ColocarProp(z.transform, "Engradado_Merc_1", new Vector3(306.5f, 13.8f, 0f), "BeerCrate_Blue", 32f, 1.1f, new Vector2(0.8f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Engradado_Merc_2", new Vector3(305.5f, 13.8f, 0f), "BeerCrate_Orange", 32f, 1.1f, new Vector2(0.8f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Lixeira_Mercadinho", new Vector3(315.4f, 13.8f, 0f), "TrashCan", 32f, 1.05f, new Vector2(0.6f, 0.5f), new Vector2(0f, 0.25f), 2);

        // A caminhonete ocupa uma vaga ao sul da avenida; a frente da venda
        // fica livre para circulação e leitura da fachada.
        ColocarProp(z.transform, "Picape_Encruzilhada_Sprite", new Vector3(326.0f, 7.0f, 0f), "Picape_Brown", 32f, 1.15f,
            new Vector2(2.6f, 1.2f), new Vector2(0f, 0.6f), 2);
        ColocarInteracao(z.transform, "Picape_Encruzilhada_Interacao", new Vector3(326.0f, 6.2f, 0f),
            InteracaoCapitulo1.TipoInteracao.PicapeEncruzilhada, "Examinar caminhonete estacionada");

        // 2. LADO DIREITO DA ENCRUZILHADA (Casa da esquina + Quadro de Avisos + Cerca, Imagem 4)
        ColocarProp(z.transform, "Casa_Esquina_Encruzilhada", new Vector3(330.5f, 14.0f, 0f), "CasaSimples_B", 16f, 0.90f,
            new Vector2(5.6f, 2.2f), new Vector2(0f, 1.4f), 2);
        ColocarProp(z.transform, "Cerca_Esquina", new Vector3(335.5f, 14.0f, 0f), "CercaMadeira", 16f, 0.9f,
            new Vector2(3.4f, 0.6f), new Vector2(0f, 0.35f), 2);
        ColocarProp(z.transform, "Banco_Encruzilhada", new Vector3(326.5f, 13.6f, 0f), "Bench_Wood", 32f, 1.1f,
            new Vector2(1.5f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Mural_Encruzilhada", new Vector3(324.8f, 13.8f, 0f), "RoadSign_Blank", 32f, 1.15f,
            new Vector2(0.8f, 0.5f), new Vector2(0f, 0.25f), 2);
        ColocarInteracao(z.transform, "Quadro_Avisos_Interacao", new Vector3(324.8f, 13.1f, 0f),
            InteracaoCapitulo1.TipoInteracao.QuadroAvisosEncruzilhada, "Ler quadro de avisos da comunidade");

        // Placa de Seta no início da subida de barro para o Bairro das Casas
        ColocarProp(z.transform, "Placa_Subida_Bairro", new Vector3(316.8f, 17.5f, 0f), "ArrowSign", 32f, 1.2f,
            new Vector2(0.6f, 0.5f), new Vector2(0f, 0.25f), 2);
        ColocarInteracao(z.transform, "Placa_Subida_Interacao", new Vector3(316.8f, 16.9f, 0f),
            InteracaoCapitulo1.TipoInteracao.PlacaSetaBairro, "Ler placa de direção");

        // Postes de luz e fiação na encruzilhada (Imagem 4)
        Color luzAmbar = new Color(1.0f, 0.82f, 0.52f, 1f);
        ColocarPosteComLuz(z.transform, "Poste_Z3_1", new Vector3(304.5f, 13.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z3_2", new Vector3(316.5f, 13.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z3_3", new Vector3(323.5f, 13.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z3_4", new Vector3(317.2f, 21.0f, 0f), luzAmbar);
        ColocarProp(z.transform, "UtilityPole_Z3_1", new Vector3(316.2f, 15.2f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);
        ColocarProp(z.transform, "UtilityPole_Z3_2", new Vector3(323.2f, 18.5f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);

        // Dois módulos de obra, com cones e seta para a subida do bairro,
        // fecham a largura inteira da avenida em vez de uma parede invisível.
        ColocarArteInterior(z.transform, "Interdicao_Centro_FaixaSul", new Vector2(336.5f, 7.8f),
            "BarricadaCentro", "WoodFence", 1f, new Vector2(4.4f, 1.8f));
        ColocarArteInterior(z.transform, "Interdicao_Centro_FaixaNorte", new Vector2(336.5f, 9.6f),
            "BarricadaCentro", "WoodFence", 1f, new Vector2(4.4f, 1.8f));
        ColocarArteInterior(z.transform, "Interdicao_Centro_Passeio", new Vector2(336.5f, 11.4f),
            "BarricadaCentro", "WoodFence", 1f, new Vector2(4.4f, 1.8f));
        ColocarInteracao(z.transform, "Interacao_Interdicao_Centro", new Vector3(333.6f, 10.1f, 0f),
            InteracaoCapitulo1.TipoInteracao.CentroBloqueioObra, "Ler interdição da avenida");

        // Gatilhos de Transição: Oeste (-> Zona 2: Posto) e Noroeste pela estrada de terra (-> Zona 4: Bairro das Casas)
        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Z3_Para_Z2",
            new Vector3(301.4f, 10.5f, 0f),
            new Vector2(2.0f, 5.5f),
            ZonaId.Zona2_Posto,
            new Vector2(234.8f, 10.0f),
            2,
            "<- Voltar para a Avenida do Posto",
            automatico: true
        );

        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Z3_Para_Z4_Bairro",
            new Vector3(319.5f, 24.4f, 0f),
            new Vector2(6.5f, 2.2f),
            ZonaId.Zona4_Bairro,
            new Vector2(420.0f, 3.0f),
            1,
            "Subir pela Estrada de Barro -> Bairro das Casas",
            automatico: true
        );
    }

    // =========================================================================
    // INTERIOR 2: INTERIOR DO MERCADINHO SANTA RITA (Com a Atendente lá dentro!)
    // =========================================================================

    private void ConstruirInterior2_Mercadinho(Transform raiz)
    {
        GameObject z = new GameObject("Interior2_Mercadinho");
        z.transform.SetParent(raiz, false);
        Vector2 min = new Vector2(300f, -60f);
        Vector2 max = new Vector2(318f, -47f);

        CriarPisoDeZona(z.transform, "Interior_Mercadinho", min, 18, 13, (ux, uy) =>
        {
            if (uy >= 11) return TilePiso.ParedePapel;
            return TilePiso.InteriorBeige;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.0f);
        AdicionarBoxCollider(z, new Vector2(309f, -48.2f), new Vector2(18f, 1.6f));

        // Iluminação interna quente de mercearia
        AdicionarLuzPonto(z.transform, new Vector3(309.0f, -53.0f, 0f), new Color(1.0f, 0.93f, 0.78f, 1f), 11.0f, 1.15f);
        ColocarTapete(z.transform, "Capacho_Mercadinho", new Vector2(309f, -57.6f),
            new Vector2(3.2f, 1.2f), new Color32(100, 51, 42, 255), new Color32(156, 105, 70, 255));

        // Porta de saída no sul
        ColocarProp(z.transform, "PortaFrame_Mercadinho", new Vector3(309.0f, -59.2f, 0f), "DoorFrame", 32f, 1.2f, Vector2.zero, Vector2.zero, 1);
        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Interior_Mercadinho",
            new Vector3(309.0f, -58.4f, 0f),
            new Vector2(2.4f, 1.4f),
            ZonaId.Zona3_Encruzilhada,
            new Vector2(311.0f, 12.2f),
            0,
            "Sair para a Encruzilhada",
            automatico: false
        );

        // A mercearia tem duas alas, hortifruti e corredor central de acesso ao caixa.
        ColocarMovelLoja(z.transform, "Secos_Mercado_Noroeste", new Vector2(303.3f, -50.3f), MovelLoja.Prateleira);
        ColocarMovelLoja(z.transform, "Secos_Mercado_Nordeste", new Vector2(314.7f, -50.3f), MovelLoja.Prateleira);
        ColocarMovelLoja(z.transform, "Hortifruti_Mercado", new Vector2(303.3f, -53.8f), MovelLoja.Hortifruti);
        ColocarMovelLoja(z.transform, "Bebidas_Mercado", new Vector2(314.7f, -53.8f), MovelLoja.Geladeira);
        ColocarProp(z.transform, "Caixas_Merc_Esq", new Vector3(302.8f, -55.2f, 0f), "CardboardBoxes_Stack", 32f, 1.2f, new Vector2(1.2f, 0.8f), new Vector2(0f, 0.4f), 2);
        ColocarProp(z.transform, "Bebidas_Merc", new Vector3(314.3f, -56.2f, 0f), "BeerCrate_Orange", 32f, 1.2f,
            new Vector2(1.1f, 0.7f), new Vector2(0f, 0.35f), 2);

        // Balcão central do Mercadinho + Rádio
        ColocarMovelLoja(z.transform, "Caixa_Mercadinho", new Vector2(309.0f, -52.2f), MovelLoja.CaixaMercado, 3.8f);
        ColocarProp(z.transform, "Radio_Mercadinho", new Vector3(310.6f, -51.5f, 0f), "Radio", 32f, 1.0f, Vector2.zero, Vector2.zero, 3);

        // ATENDENTE dentro do Mercadinho atrás do balcão!
        ColocarInteracao(z.transform, "NPC_Atendente_Interior", new Vector3(309.0f, -50.9f, 0f),
            InteracaoCapitulo1.TipoInteracao.NpcAtendenteMercadinho, "Conversar com a Atendente", "Npc_Atendente", 32f, 1.0f, 2, solido: true);

        // Pistas dentro do Mercadinho: Caderno de entregas e Gôndola de suprimentos
        ColocarInteracao(z.transform, "Caderno_Mercadinho_Interacao", new Vector3(307.4f, -52.5f, 0f),
            InteracaoCapitulo1.TipoInteracao.CadernoFiadoMercadinho, "Ler caderno de entregas no balcão");
        ColocarInteracao(z.transform, "Prateleiras_Mercadinho_Interacao", new Vector3(314.5f, -55.0f, 0f),
            InteracaoCapitulo1.TipoInteracao.PrateleirasMercadinho, "Examinar prateleiras do Mercadinho");
    }

    // =========================================================================
    // ZONA 4: BAIRRO DAS CASAS & CASA DO AVÔ ADÍLIO Nº 40 (Idêntico à Imagem 5!)
    // =========================================================================

    private void ConstruirZona4_Bairro(Transform raiz)
    {
        GameObject z = new GameObject("Zona4_Bairro");
        z.transform.SetParent(raiz, false);
        Vector2 min = new Vector2(400f, 0f);
        Vector2 max = new Vector2(444f, 34f);

        // SOMENTE RUAS DE BARRO E TERRA (Zero asfalto no Bairro das Casas, exatamente como na Imagem 5!)
        CriarPisoDeZona(z.transform, "Zona4_Bairro", min, 44, 34, (ux, uy) =>
        {
            // Entrada sul de barro (X = 18..22, Y = 0..10)
            bool entradaSul = (ux >= 18 && ux <= 22 && uy <= 10);
            // Rua Sul do Bairro (Y = 8..10, X = 6..36)
            bool ruaSul = (uy >= 8 && uy <= 10 && ux >= 6 && ux <= 36);
            // Avenida Central de Barro subindo (X = 16..20, Y = 10..22)
            bool subidaCentral = (ux >= 16 && ux <= 20 && uy >= 10 && uy <= 22);
            // Rua Principal da Casa do Avô Nº 40 (Y = 20..22, X = 6..38)
            bool ruaCasaAvo = (uy >= 20 && uy <= 22 && ux >= 6 && ux <= 38);
            // Subida Nordeste de Barro para a Rua Norte / Bar do Zé (X = 32..38, Y = 22..34)
            bool subidaNordeste = (ux >= 32 && ux <= 38 && uy >= 22);

            if (entradaSul || ruaSul || subidaCentral || ruaCasaAvo || subidaNordeste)
            {
                return ((ux + uy) % 4 == 0) ? TilePiso.TerraRachada : TilePiso.Terra;
            }

            // Passeios estreitos de concreto entre o barro da rua e os jardins.
            if ((uy == 12 && ux >= 6 && ux <= 36) || (uy == 24 && ux >= 6 && ux <= 38))
            {
                return TilePiso.Concreto;
            }

            return TilePiso.Grama;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.5f);

        // --- QUARTEIRÃO SUL DO BAIRRO (Exploração da vizinhança antes de chegar na nº 40) ---
        // Casa Nº 14 (Dona Lurdes) + Van na garagem de terra + Cercas de madeira (Imagem 5)
        ColocarProp(z.transform, "Bairro_Casa_14", new Vector3(410.5f, 12.6f, 0f), "CasaSimples_A", 16f, 0.90f,
            new Vector2(5.4f, 2.0f), new Vector2(0f, 1.3f), 2);
        ColocarProp(z.transform, "Cerca_Casa14", new Vector3(406.0f, 12.4f, 0f), "CercaMadeira", 16f, 0.85f,
            new Vector2(3.2f, 0.6f), new Vector2(0f, 0.35f), 2);
        ColocarProp(z.transform, "Van_Garagem_Casa14", new Vector3(415.0f, 12.2f, 0f), "Van_White", 32f, 1.1f,
            new Vector2(2.4f, 1.2f), new Vector2(0f, 0.6f), 2);
        ColocarInteracao(z.transform, "Porta_Casa_14_Lurdes", new Vector3(410.5f, 12.0f, 0f),
            InteracaoCapitulo1.TipoInteracao.VizinhaDonaLurdes, "Bater na porta (Casa nº 14)");
        ColocarProp(z.transform, "Jardim_Casa14", new Vector3(407.5f, 13.6f, 0f), "PlanterBox", 32f, 1.15f,
            Vector2.zero, Vector2.zero, 2);

        // Casa Nº 22 (Lado direito da Rua Sul) + Rastros de pneu na lama
        ColocarProp(z.transform, "Bairro_Casa_22", new Vector3(429.5f, 12.6f, 0f), "CasaSimples_C", 16f, 0.90f,
            new Vector2(5.4f, 2.0f), new Vector2(0f, 1.3f), 2);
        ColocarProp(z.transform, "Cerca_Casa22", new Vector3(434.2f, 12.4f, 0f), "CercaMadeira", 16f, 0.85f,
            new Vector2(3.2f, 0.6f), new Vector2(0f, 0.35f), 2);
        ColocarProp(z.transform, "Caixotes_Casa22", new Vector3(425.8f, 12.2f, 0f), "Caixotes", 16f, 0.85f,
            new Vector2(1.2f, 0.8f), new Vector2(0f, 0.4f), 2);
        ColocarProp(z.transform, "Jardim_Casa22", new Vector3(432.5f, 13.6f, 0f), "PlanterBox", 32f, 1.15f,
            Vector2.zero, Vector2.zero, 2);
        ColocarInteracao(z.transform, "Pista_Rastros_Bairro", new Vector3(425.8f, 11.4f, 0f),
            InteracaoCapitulo1.TipoInteracao.GalpaoBairroPista, "Examinar rastros na lama (Casa nº 22)");

        // --- QUARTEIRÃO NORTE DO BAIRRO (Rua da Casa do Avô Adílio — Nº 40, Imagem 5!) ---
        // Casa Nº 32 (Esquerda) + Seu Ramiro na varanda
        ColocarProp(z.transform, "Bairro_Casa_32", new Vector3(410.5f, 24.4f, 0f), "CasaSimples_B", 16f, 0.90f,
            new Vector2(5.4f, 2.0f), new Vector2(0f, 1.3f), 2);
        ColocarProp(z.transform, "Cerca_Casa32", new Vector3(406.0f, 24.2f, 0f), "CercaMadeira", 16f, 0.85f,
            new Vector2(3.2f, 0.6f), new Vector2(0f, 0.35f), 2);
        ColocarProp(z.transform, "Jardim_Casa32", new Vector3(408.0f, 25.4f, 0f), "PlanterBox", 32f, 1.15f,
            Vector2.zero, Vector2.zero, 2);
        ColocarInteracao(z.transform, "NPC_SeuRamiro_Bairro", new Vector3(414.8f, 23.4f, 0f),
            InteracaoCapitulo1.TipoInteracao.NpcMoradorBairro, "Conversar com Seu Ramiro", "Npc_Frequentador", 32f, 1.0f, 2, solido: true);

        // CASA DO AVÔ ADÍLIO — Nº 40 (Destaque central no Bairro das Casas!)
        ColocarProp(z.transform, "Predio_CasaDoAvo_40", new Vector3(423.5f, 24.4f, 0f), "CasaAvo", 16f, 0.96f,
            new Vector2(6.6f, 2.4f), new Vector2(0f, 1.6f), 2);
        AdicionarLuzPonto(z.transform, new Vector3(423.5f, 25.0f, 0f), new Color(1.0f, 0.86f, 0.56f, 1f), 7.5f, 1.35f);

        ColocarProp(z.transform, "Cerca_Avo_Esq", new Vector3(418.2f, 24.2f, 0f), "CercaMadeira", 16f, 0.9f,
            new Vector2(3.4f, 0.6f), new Vector2(0f, 0.35f), 2);
        ColocarProp(z.transform, "Cerca_Avo_Dir", new Vector3(428.8f, 24.2f, 0f), "CercaMadeira", 16f, 0.9f,
            new Vector2(3.4f, 0.6f), new Vector2(0f, 0.35f), 2);
        ColocarProp(z.transform, "Jardim_Avo_40", new Vector3(420.0f, 25.5f, 0f), "PlanterBox", 32f, 1.2f,
            Vector2.zero, Vector2.zero, 2);

        // Caixa de Correio Nº 40
        ColocarProp(z.transform, "CaixaCorreio_40_Sprite", new Vector3(426.6f, 23.6f, 0f), "CaixaCorreio", 16f, 0.9f,
            new Vector2(0.7f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarInteracao(z.transform, "CaixaCorreio_40_Interacao", new Vector3(426.6f, 23.0f, 0f),
            InteracaoCapitulo1.TipoInteracao.CaixaCorreioAvo, "Examinar caixa de correio (Nº 40)");

        // Porta para entrar no INTERIOR DA CASA DO AVÔ ADÍLIO (Nº 40)
        ColocarGatilhoTransicao(
            z.transform,
            "Porta_Entrar_CasaDoAvo",
            new Vector3(423.5f, 23.8f, 0f),
            new Vector2(2.0f, 1.5f),
            ZonaId.Interior_CasaDoAvo,
            new Vector2(409.0f, -57.2f),
            1,
            "Entrar na Casa do Avô Adílio (Nº 40)",
            automatico: false
        );

        // Casa Nº 48 na curva nordeste rumo ao Bar do Zé
        ColocarProp(z.transform, "Bairro_Casa_48", new Vector3(439.5f, 24.4f, 0f), "CasaSimples_A", 16f, 0.88f,
            new Vector2(5.2f, 2.0f), new Vector2(0f, 1.3f), 2);
        ColocarProp(z.transform, "Seta_Subida_BarZe", new Vector3(433.2f, 26.5f, 0f), "ArrowSign", 32f, 1.15f, Vector2.zero, Vector2.zero, 2);

        // Postes de madeira e iluminação de comunidade (Imagem 5)
        Color luzAmbar = new Color(1.0f, 0.82f, 0.52f, 1f);
        ColocarPosteComLuz(z.transform, "Poste_Z4_1", new Vector3(416.5f, 11.4f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z4_2", new Vector3(423.5f, 11.4f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z4_3", new Vector3(416.5f, 23.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z4_4", new Vector3(431.5f, 23.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z4_5", new Vector3(434.2f, 29.5f, 0f), luzAmbar);

        // Gatilhos de Transição: Sul (-> Zona 3: Encruzilhada) e Nordeste (-> Zona 5: Rua Norte & Bar do Zé)
        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Z4_Para_Z3",
            new Vector3(420.0f, 1.4f, 0f),
            new Vector2(6.0f, 2.0f),
            ZonaId.Zona3_Encruzilhada,
            new Vector2(319.5f, 22.4f),
            0,
            "Descer pela Estrada de Barro -> Encruzilhada & Mercadinho",
            automatico: true
        );

        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Z4_Para_Z5_Bar",
            new Vector3(435.5f, 32.4f, 0f),
            new Vector2(6.5f, 2.2f),
            ZonaId.Zona5_RuaNorteBar,
            new Vector2(503.2f, 10.0f),
            3,
            "Subir para a Rua Norte -> Bar do Zé",
            automatico: true,
            exigirCasaAvo: true
        );
    }

    // =========================================================================
    // INTERIOR 3: CASA DO AVÔ ADÍLIO — Nº 40 (Sem tela azul, com móveis e 4 pistas!)
    // =========================================================================

    private void ConstruirInterior3_CasaDoAvo(Transform raiz)
    {
        GameObject z = new GameObject("Interior3_CasaDoAvo");
        z.transform.SetParent(raiz, false);
        Vector2 min = new Vector2(400f, -60f);
        Vector2 max = new Vector2(418f, -46f);

        CriarPisoDeZona(z.transform, "Interior_CasaDoAvo", min, 18, 14, (ux, uy) =>
        {
            if (uy >= 12) return TilePiso.ParedePapel;
            if (ux <= 6 && uy >= 6) return TilePiso.InteriorSmall; // Cozinha à esquerda
            return TilePiso.InteriorPatterned;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.0f);
        AdicionarBoxCollider(z, new Vector2(409f, -47.2f), new Vector2(18f, 1.6f));

        // Iluminação interna aconchegante/melancólica da casa de Adílio
        AdicionarLuzPonto(z.transform, new Vector3(409.0f, -52.5f, 0f), new Color(1.0f, 0.88f, 0.68f, 1f), 11.0f, 1.1f);
        ColocarTapete(z.transform, "Tapete_Sala_Avo", new Vector2(409f, -53.3f),
            new Vector2(5.0f, 3.0f), new Color32(94, 49, 41, 255), new Color32(145, 101, 73, 255));

        // Porta de saída no sul da casa
        ColocarProp(z.transform, "PortaFrame_CasaAvo", new Vector3(409.0f, -59.2f, 0f), "DoorFrame", 32f, 1.2f, Vector2.zero, Vector2.zero, 1);
        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Interior_CasaAvo",
            new Vector3(409.0f, -58.4f, 0f),
            new Vector2(2.4f, 1.4f),
            ZonaId.Zona4_Bairro,
            new Vector2(423.5f, 22.6f),
            0,
            "Sair para o Bairro das Casas",
            automatico: false
        );

        // Janelas, relógio e retrato de família (1998) na parede norte
        ColocarProp(z.transform, "Janela_Avo_Esq", new Vector3(404.5f, -47.4f, 0f), "Window_Curtain", 32f, 1.2f, Vector2.zero, Vector2.zero, 2);
        ColocarProp(z.transform, "Janela_Avo_Dir", new Vector3(413.5f, -47.4f, 0f), "Window_Curtain", 32f, 1.2f, Vector2.zero, Vector2.zero, 2);
        ColocarProp(z.transform, "Relogio_Avo", new Vector3(407.0f, -47.4f, 0f), "WallClock", 32f, 1.1f, Vector2.zero, Vector2.zero, 2);
        ColocarProp(z.transform, "Foto_Familia_Avo", new Vector3(409.0f, -47.5f, 0f), "FramedPhoto", 32f, 1.2f, Vector2.zero, Vector2.zero, 2);
        ColocarInteracao(z.transform, "Interacao_Retrato_Avo", new Vector3(409.0f, -48.6f, 0f),
            InteracaoCapitulo1.TipoInteracao.RetratoFamiliaCasaAvo, "Examinar retrato de família (1998)");

        // 1. COZINHA (Canto Noroeste): Pia + Armário + PISTA 1 (Café passado hoje mesmo)
        ColocarArteInterior(z.transform, "Bancada_Cozinha_Avo", new Vector2(404.0f, -49.2f),
            "Interior_PiaAvo", "Sink", 1.0f, new Vector2(3.3f, 0.9f));
        ColocarInteracao(z.transform, "Pista_1_Cafe_Morno", new Vector3(404.2f, -50.2f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaCafeCasaAvo, "Examinar bule e xícara de café");

        // 2. SALA CENTRAL: Mesa de Madeira + Poltrona + TV + PISTA 2 (Cartas de paciência interrompidas)
        ColocarArteInterior(z.transform, "Mesa_Cartas_Avo", new Vector2(409.0f, -52.5f),
            "Interior_MesaCartas", "WoodTable", 0.78f, new Vector2(2.4f, 1.1f));
        ColocarArteInterior(z.transform, "Poltrona_Avo", new Vector2(405.8f, -52.5f),
            "Interior_PoltronaAvo", "Armchair", 0.78f, new Vector2(1.5f, 0.9f));
        ColocarProp(z.transform, "TV_Sala_Avo", new Vector3(403.2f, -53.8f, 0f), "TV_Old", 32f, 1.2f, new Vector2(1.2f, 0.9f), new Vector2(0f, 0.45f), 2);
        ColocarInteracao(z.transform, "Pista_2_Cartas_Mesa", new Vector3(409.0f, -53.3f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaCartasCasaAvo, "Examinar cartas na mesa");

        // 3. QUARTO / ESCRITÓRIO (Lado Leste): Cama + Cômoda Escura + Rádio + Telefone + PISTA 3 (Nota Fiscal de 2004 e Bilhete do Bar do Zé)
        ColocarProp(z.transform, "Cama_Avo", new Vector3(415.0f, -50.2f, 0f), "Bed", 32f, 1.25f, new Vector2(1.8f, 2.2f), new Vector2(0f, 1.0f), 2);
        ColocarProp(z.transform, "Comoda_Avo", new Vector3(412.4f, -49.4f, 0f), "Cabinet_Dark", 32f, 1.25f, new Vector2(1.6f, 0.9f), new Vector2(0f, 0.45f), 2);
        ColocarProp(z.transform, "Radio_Avo", new Vector3(412.4f, -48.6f, 0f), "Radio", 32f, 1.0f, Vector2.zero, Vector2.zero, 3);
        ColocarInteracao(z.transform, "Pista_3_NotaFiscal_2004", new Vector3(412.4f, -50.4f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaNotaFiscal2004, "Ler nota fiscal (2004) e bilhete");
        ColocarInteracao(z.transform, "Interacao_Telefone_Avo", new Vector3(414.6f, -53.6f, 0f),
            InteracaoCapitulo1.TipoInteracao.TelefoneCasaAvo, "Examinar telefone fixo");

        // 4. ENTRADA DA CASA (Perto da porta sul): PISTA 4 (Banqueta caída e marcas de barro)
        ColocarProp(z.transform, "Banqueta_Caida_Avo", new Vector3(406.6f, -56.8f, 0f), "PlasticChair_Red", 32f, 1.05f, new Vector2(0.8f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarInteracao(z.transform, "Pista_4_Desordem_Porta", new Vector3(406.6f, -56.2f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaDesordemPorta, "Examinar banco caído e pegadas");
    }

    // =========================================================================
    // ZONA 5: RUA NORTE — EXTERIOR DO BAR DO ZÉ & PORTÃO DO CENTRO
    // =========================================================================

    private void ConstruirZona5_RuaNorteBar(Transform raiz)
    {
        GameObject z = new GameObject("Zona5_RuaNorteBar");
        z.transform.SetParent(raiz, false);
        Vector2 min = new Vector2(500f, 0f);
        Vector2 max = new Vector2(538f, 22f);

        CriarPisoDeZona(z.transform, "Zona5_RuaNorteBar", min, 38, 22, (ux, uy) =>
        {
            // Transição do barro do Bairro para o calçamento da Rua Norte (X = 0..6, Y = 8..11)
            if (uy >= 8 && uy <= 11 && ux <= 4) return TilePiso.TerraRachada;
            if (uy >= 8 && uy <= 11 && ux == 6) return TilePiso.AsfaltoParaTerra;
            if (uy == 8) return (ux % 6 == 0) ? TilePiso.AsfaltoH_Poca : TilePiso.AsfaltoH_Trinca;
            if (uy >= 9 && uy <= 11) return TilePiso.AsfaltoH_Trinca;
            if (uy == 6) return TilePiso.CalcadaSul;
            if (uy == 12) return TilePiso.CalcadaNorte;
            if (uy >= 14 && uy <= 16 && ux >= 8 && ux <= 30) return TilePiso.Concreto;
            return TilePiso.Grama;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.5f);

        // Caminhonete do Clube estacionada perto do Bar do Zé (Pista!)
        ColocarProp(z.transform, "Caminhonete_Clube_Bar", new Vector3(509.5f, 11.4f, 0f), "Picape_Black", 32f, 1.15f,
            new Vector2(2.6f, 1.2f), new Vector2(0f, 0.6f), 2);
        ColocarInteracao(z.transform, "Interacao_Caminhonete_Clube", new Vector3(509.5f, 10.6f, 0f),
            InteracaoCapitulo1.TipoInteracao.CaminhoneteClubeBar, "Examinar caminhonete escura");

        // FACHADA DO BAR DO ZÉ
        ColocarProp(z.transform, "Predio_Fachada_BarDoZe", new Vector3(518.0f, 14.0f, 0f), "Fachada_BarDoZe", 16f, 0.94f,
            new Vector2(6.0f, 2.2f), new Vector2(0f, 1.5f), 2);
        AdicionarLuzPonto(z.transform, new Vector3(518.0f, 14.8f, 0f), new Color(1.0f, 0.72f, 0.38f, 1f), 7.8f, 1.45f);

        // Mesas de bar, cadeiras de plástico e engradados na calçada do Bar do Zé
        ColocarProp(z.transform, "Mesa_Calcada_Bar", new Vector3(513.5f, 13.8f, 0f), "RoundTable_Wood", 32f, 1.1f, new Vector2(1.0f, 0.8f), new Vector2(0f, 0.4f), 2);
        ColocarProp(z.transform, "Cadeira_Calcada_1", new Vector3(512.4f, 13.8f, 0f), "PlasticChair_Red", 32f, 1.05f, new Vector2(0.6f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Cadeira_Calcada_2", new Vector3(514.6f, 13.8f, 0f), "PlasticChair_Red", 32f, 1.05f, new Vector2(0.6f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Engradados_Bar_Ext", new Vector3(521.8f, 13.8f, 0f), "BeerCrate_Blue", 32f, 1.1f, new Vector2(0.8f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Dumpster_Bar_Ext", new Vector3(523.2f, 14.2f, 0f), "Dumpster", 32f, 1.1f, new Vector2(1.4f, 0.9f), new Vector2(0f, 0.45f), 2);

        ColocarInteracao(z.transform, "Placa_Letreiro_BarZe", new Vector3(515.8f, 13.2f, 0f),
            InteracaoCapitulo1.TipoInteracao.PlacaBarDoZe, "Ler letreiro do Bar do Zé");

        // Porta para entrar no INTERIOR DO BAR DO ZÉ
        ColocarGatilhoTransicao(
            z.transform,
            "Porta_Entrar_BarDoZe",
            new Vector3(518.0f, 13.4f, 0f),
            new Vector2(2.0f, 1.4f),
            ZonaId.Interior_BarDoZe,
            new Vector2(509.0f, -57.2f),
            1,
            "Entrar no Bar do Zé",
            automatico: false
        );

        // Orelhão #2 ao lado do Bar do Zé (Save Point antes do Boss!)
        ColocarOrelhaoSave(z.transform, "Orelhao_BarDoZe", "orelhao_bar_do_ze", new Vector3(525.8f, 13.4f, 0f));

        // PORTÃO DO CENTRO no final leste da Rua Norte
        Vector3 posPortao = new Vector3(532.5f, 10.0f, 0f);
        ColocarProp(z.transform, "Portao_Centro", posPortao, "Portao_Centro", 32f, 1.15f, Vector2.zero, Vector2.zero, 6);

        GameObject barreiraCentro = new GameObject("Barreira_Limiar_Centro");
        barreiraCentro.transform.SetParent(z.transform, false);
        barreiraCentro.transform.position = posPortao;
        BoxCollider2D colBarreira = barreiraCentro.AddComponent<BoxCollider2D>();
        colBarreira.size = new Vector2(2.2f, 4.5f);
        colBarreira.offset = new Vector2(0f, 0.5f);

        ColocarInteracao(z.transform, "Limiar_Centro_Trigger", posPortao + new Vector3(-1.0f, 0f, 0f),
            InteracaoCapitulo1.TipoInteracao.LimiarCentro, "");

        // Postes de luz na Rua Norte
        Color luzAmbar = new Color(1.0f, 0.82f, 0.52f, 1f);
        ColocarPosteComLuz(z.transform, "Poste_Z5_1", new Vector3(505.5f, 13.2f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z5_2", new Vector3(527.5f, 13.2f, 0f), luzAmbar);

        // Gatilho de Transição Oeste (-> Zona 4: Bairro das Casas)
        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Z5_Para_Z4_Bairro",
            new Vector3(501.4f, 10.0f, 0f),
            new Vector2(2.0f, 5.5f),
            ZonaId.Zona4_Bairro,
            new Vector2(435.5f, 29.8f),
            0,
            "<- Descer para o Bairro das Casas",
            automatico: true
        );
    }

    // =========================================================================
    // INTERIOR 4: BAR DO ZÉ (Sem tela azul, com Mesa de Sinuca, Zé, Frequentador!)
    // =========================================================================

    private void ConstruirInterior4_BarDoZe(Transform raiz)
    {
        GameObject z = new GameObject("Interior4_BarDoZe");
        z.transform.SetParent(raiz, false);
        Vector2 min = new Vector2(500f, -60f);
        Vector2 max = new Vector2(518f, -46f);

        CriarPisoDeZona(z.transform, "Interior_BarDoZe", min, 18, 14, (ux, uy) =>
        {
            if (uy >= 12) return TilePiso.ParedeBar;
            return TilePiso.InteriorBar;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.0f);
        AdicionarBoxCollider(z, new Vector2(509f, -47.2f), new Vector2(18f, 1.6f));

        // Iluminação quente de sinuca e bar
        AdicionarLuzPonto(z.transform, new Vector3(509.0f, -52.8f, 0f), new Color(1.0f, 0.84f, 0.52f, 1f), 11.5f, 1.25f);
        ColocarTapete(z.transform, "Capacho_Bar", new Vector2(509f, -57.6f),
            new Vector2(2.9f, 1.2f), new Color32(70, 44, 30, 255), new Color32(119, 86, 51, 255));

        // Porta de saída no sul do Bar
        ColocarProp(z.transform, "PortaFrame_BarDoZe", new Vector3(509.0f, -59.2f, 0f), "DoorFrame", 32f, 1.2f, Vector2.zero, Vector2.zero, 1);
        ColocarGatilhoTransicao(
            z.transform,
            "Saida_Interior_BarDoZe",
            new Vector3(509.0f, -58.4f, 0f),
            new Vector2(2.4f, 1.4f),
            ZonaId.Zona5_RuaNorteBar,
            new Vector2(518.0f, 12.0f),
            0,
            "Sair para a Rua Norte",
            automatico: false
        );

        // Balcão do Bar, prateleiras de garrafas, mesas laterais e engradados
        ColocarProp(z.transform, "Estante_Garrafas_1", new Vector3(503.5f, -49.0f, 0f), "Cabinet_Dark", 32f, 1.25f, new Vector2(1.6f, 0.9f), new Vector2(0f, 0.45f), 2);
        ColocarProp(z.transform, "Estante_Garrafas_2", new Vector3(506.0f, -49.0f, 0f), "BoxesShelf", 32f, 1.25f, new Vector2(1.8f, 0.9f), new Vector2(0f, 0.45f), 2);
        ColocarArteInterior(z.transform, "Balcao_Bar_Sprite", new Vector2(504.8f, -50.8f),
            "Interior_BalcaoBar", "Balcao_Bar", 0.96f, new Vector2(3.8f, 1.0f));
        ColocarInteracao(z.transform, "Balcao_Bar_Interacao", new Vector3(504.8f, -51.6f, 0f),
            InteracaoCapitulo1.TipoInteracao.BalcaoBarDoZe, "Examinar balcão do bar");

        // Quadro de Regras da Sinuca na parede
        ColocarProp(z.transform, "Quadro_Regras_Sprite", new Vector3(513.5f, -48.6f, 0f), "Quadro_Regras_Sinuca", 32f, 1.15f, Vector2.zero, Vector2.zero, 3);
        ColocarInteracao(z.transform, "Quadro_Regras_Interacao", new Vector3(513.5f, -49.8f, 0f),
            InteracaoCapitulo1.TipoInteracao.QuadroRegrasSinuca, "Ler regras da mesa");

        // Mesas laterais e engradados no canto do bar
        ColocarProp(z.transform, "Mesa_Bar_Esq", new Vector3(503.2f, -55.0f, 0f), "RoundTable_Wood", 32f, 1.15f, new Vector2(1.1f, 0.8f), new Vector2(0f, 0.4f), 2);
        ColocarProp(z.transform, "Cadeira_Bar_Esq", new Vector3(504.4f, -55.0f, 0f), "PlasticChair_Red", 32f, 1.05f, new Vector2(0.6f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Engradados_Bar_Int", new Vector3(515.2f, -55.5f, 0f), "BottlesCrate", 32f, 1.15f, new Vector2(1.0f, 0.7f), new Vector2(0f, 0.35f), 2);

        // MESA DE SINUCA CENTRAL
        ColocarProp(z.transform, "Mesa_Sinuca_Central", new Vector3(509.0f, -53.2f, 0f), "Mesa_Sinuca_TopDown", 32f, 1.15f,
            new Vector2(3.2f, 1.6f), new Vector2(0f, 0.8f), 2);
        ColocarInteracao(z.transform, "Mesa_Sinuca_Trigger", new Vector3(509.0f, -53.8f, 0f),
            InteracaoCapitulo1.TipoInteracao.MesaSinucaBar, "");

        // ZÉ (Boss do Cap. 1) ao lado da mesa de sinuca
        ColocarInteracao(z.transform, "NPC_Ze_Interior", new Vector3(511.6f, -52.8f, 0f),
            InteracaoCapitulo1.TipoInteracao.NpcZeBar, "", "Npc_Ze", 32f, 1.0f, 2, solido: true);

        // FREQUENTADOR perto do balcão
        ColocarInteracao(z.transform, "NPC_Frequentador_Interior", new Vector3(505.8f, -52.4f, 0f),
            InteracaoCapitulo1.TipoInteracao.NpcFrequentadorBar, "Conversar com o Frequentador", "Npc_Frequentador", 32f, 1.0f, 2, solido: true);
    }

    // =========================================================================
    // SUPORTE ÀS CENAS SEPARADAS (Caso abertas diretamente no Editor)
    // =========================================================================

    private void GarantirElementosCasaDoAvoCenaSeparada()
    {
        AjustarLuzGlobal(true);
        ColocarInteracao(transform, "Pista_Cafe_Hoje", new Vector3(-2.6f, 1.3f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaCafeCasaAvo, "Examinar bule de café");
        ColocarInteracao(transform, "Pista_Cartas_Mesa", new Vector3(-0.65f, -4.15f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaCartasCasaAvo, "Examinar cartas na mesa");
        ColocarInteracao(transform, "Pista_NotaFiscal_2004", new Vector3(2.8f, -0.2f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaNotaFiscal2004, "Ler nota fiscal (2004)");
        ColocarInteracao(transform, "Pista_Desordem_Porta", new Vector3(-1.8f, -8.5f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaDesordemPorta, "Examinar banco caído");
    }

    private void GarantirElementosBarDoZeCenaSeparada()
    {
        AjustarLuzGlobal(true);
        GameState.Marcar(Flags.EntrouNoBarDoZe);
        if (!GameState.Tem(Flags.ZeDerrotadoNaSinuca))
        {
            GameState.DefinirObjetivo("Confrontar Zé e vencê-lo na Melhor de 3 da Sinuca ('A Bola Marcada').");
        }

        ColocarInteracao(transform, "NPC_Ze", new Vector3(2.0f, -4.5f, 0f),
            InteracaoCapitulo1.TipoInteracao.NpcZeBar, "", "Npc_Ze", 32f, 1.0f, 2, solido: true);
        ColocarInteracao(transform, "NPC_Frequentador_Bar", new Vector3(-2.6f, -2.4f, 0f),
            InteracaoCapitulo1.TipoInteracao.NpcFrequentadorBar, "Conversar com o Frequentador", "Npc_Frequentador", 32f, 1.0f, 2, solido: true);
        ColocarInteracao(transform, "Mesa_Sinuca_Trigger", new Vector3(0f, -4.8f, 0f),
            InteracaoCapitulo1.TipoInteracao.MesaSinucaBar, "");
        ColocarInteracao(transform, "Balcao_Bar_Trigger", new Vector3(-4.2f, -1.2f, 0f),
            InteracaoCapitulo1.TipoInteracao.BalcaoBarDoZe, "Examinar balcão");
        ColocarInteracao(transform, "Quadro_Regras_Trigger", new Vector3(4.2f, 0.5f, 0f),
            InteracaoCapitulo1.TipoInteracao.QuadroRegrasSinuca, "Ler regras da mesa");
    }

    public static void AtualizarEstadoPortaoCentroNaCena()
    {
        GameObject barreiraFisica = GameObject.Find("Barreira_Limiar_Centro");
        if (barreiraFisica != null)
        {
            Collider2D col = barreiraFisica.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = !GameState.Tem(Flags.ZeDerrotadoNaSinuca);
            }
        }
    }

    public static void MostrarTelaFechamentoCapitulo1()
    {
        EnsureForCurrentScene();
        if (instancia != null)
        {
            instancia.AbrirPainelFechamento();
        }
    }

    private void AbrirPainelFechamento()
    {
        if (painelFechamentoCap1 != null)
        {
            painelFechamentoCap1.SetActive(true);
            return;
        }

        Canvas canvas = UIFactory.CreateCanvas(transform, "FechamentoCapitulo1Canvas", 9600);
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        painelFechamentoCap1 = canvas.gameObject;

        Image fundo = new GameObject("FundoEscuro", typeof(RectTransform)).AddComponent<Image>();
        fundo.transform.SetParent(canvas.transform, false);
        fundo.color = new Color(0.03f, 0.03f, 0.04f, 0.96f);
        UIFactory.Stretch(fundo.rectTransform);

        Image moldura = UIFactory.CreateBox(canvas.transform, "MolduraCapitulo1", "DialogueBox");
        moldura.color = new Color(0.94f, 0.89f, 0.79f, 0.99f);
        RectTransform rt = moldura.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(540f, 310f);
        rt.anchoredPosition = Vector2.zero;

        TextMeshProUGUI subtitulo = UIFactory.CreateLabel(rt, "Subtitulo", 14f);
        subtitulo.text = "SANTA RITA — A NOITE DAS 03:17";
        subtitulo.color = new Color(0.58f, 0.22f, 0.20f, 1f);
        subtitulo.alignment = TextAlignmentOptions.Center;
        subtitulo.rectTransform.anchorMin = subtitulo.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        subtitulo.rectTransform.sizeDelta = new Vector2(480f, 24f);
        subtitulo.rectTransform.anchoredPosition = new Vector2(0f, 118f);

        TextMeshProUGUI titulo = UIFactory.CreateLabel(rt, "Titulo", 24f);
        titulo.text = "FIM DO CAPÍTULO 1 — A MESA DO ZÉ";
        titulo.color = new Color(0.18f, 0.14f, 0.10f, 1f);
        titulo.alignment = TextAlignmentOptions.Center;
        titulo.rectTransform.anchorMin = titulo.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        titulo.rectTransform.sizeDelta = new Vector2(490f, 32f);
        titulo.rectTransform.anchoredPosition = new Vector2(0f, 88f);

        TextMeshProUGUI corpo = UIFactory.CreateLabel(rt, "Resumo", 15f);
        corpo.text =
            "• FATO CONFIRMADO: Adílio não saiu de casa voluntariamente hoje. O café morno e o banco tombado provam que sua rotina foi interrompida à força.\n\n" +
            "• RECOMPENSA OBTIDA: Ficha do Clube #01 (ficha_clube_ze_01), conquistada ao derrotar Zé na Melhor de 3 sob a regra 'A Bola Marcada'.\n\n" +
            "• PRÓXIMO PASSO: Com o portão norte destrancado, Marcus avança para o Centro de Santa Rita em direção aos quatro cantos da cidade e ao Clube no topo.";
        corpo.color = new Color(0.22f, 0.18f, 0.14f, 1f);
        corpo.alignment = TextAlignmentOptions.TopLeft;
        corpo.textWrappingMode = TextWrappingModes.Normal;
        corpo.rectTransform.anchorMin = corpo.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        corpo.rectTransform.sizeDelta = new Vector2(468f, 146f);
        corpo.rectTransform.anchoredPosition = new Vector2(0f, -2f);

        CriarBotaoFechamento(rt, "CONTINUAR EXPLORANDO", new Vector2(-115f, -114f), () =>
        {
            painelFechamentoCap1.SetActive(false);
        });

        CriarBotaoFechamento(rt, "VOLTAR AO MENU", new Vector2(115f, -114f), () =>
        {
            painelFechamentoCap1.SetActive(false);
            SceneFader.Instance.LoadScene("MainMenu");
        });
    }

    private static void CriarBotaoFechamento(RectTransform pai, string texto, Vector2 pos, UnityEngine.Events.UnityAction acao)
    {
        GameObject go = new GameObject(texto, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(pai, false);
        RectTransform r = (RectTransform)go.transform;
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(200f, 34f);
        r.anchoredPosition = pos;

        Image img = go.GetComponent<Image>();
        img.color = new Color(0.22f, 0.16f, 0.12f, 0.95f);

        TextMeshProUGUI lbl = UIFactory.CreateLabel(r, "Texto", 16f);
        lbl.text = texto;
        lbl.color = new Color(0.95f, 0.86f, 0.68f, 1f);
        lbl.alignment = TextAlignmentOptions.Center;
        UIFactory.Stretch(lbl.rectTransform);

        Button btn = go.GetComponent<Button>();
        btn.onClick.AddListener(acao);
    }
}
