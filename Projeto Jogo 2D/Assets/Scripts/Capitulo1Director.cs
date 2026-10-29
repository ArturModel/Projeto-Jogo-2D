using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public static bool EstaEmInterior =>
        instancia != null &&
        tabelaZonas.TryGetValue(instancia.zonaAtual, out InfoZona info) &&
        info.ehInterior;

    private GameObject raizMundoCapitulo1;
    private GameObject painelFechamentoCap1;
    private TextMeshProUGUI textoAvisoSaveFechamento;
    private float painelFechamentoAbertoEm;
    private PayphoneCheckpoint templatePayphone;
    private AudioSource musicaBairro;
    private AudioSource musicaBar;

    private const float VolumeBairro = 0.48f;
    private const float VolumeCasaAvo = 0.32f;
    private const float VolumeBar = 0.48f;
    private const float VolumeSinuca = 0.31f;
    private const float VelocidadeFadeMusica = 0.55f;

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

        var arteProducao = Resources.Load<Texture2D>("Art/Characters/" + nomeArquivo);
        if (arteProducao != null) { cacheTexturas[nomeArquivo] = arteProducao; return arteProducao; }
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
        var final = Resources.Load<Sprite>("Art/Characters/" + nomeArquivo);
        if (final == null) final = Resources.Load<Sprite>("Art/Buildings/" + nomeArquivo);
        if (final == null && (nomeArquivo == "Oak_Tree" || nomeArquivo == "Oak_Tree_Small"))
            final = Resources.Load<Sprite>("Art/World/" + (nomeArquivo == "Oak_Tree" ? "Tree_A" : "Tree_B"));
        if (final != null) return final;
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

        if (Application.isPlaying)
            PrepararMusicasDasZonas();

        if (cena == "EntradaDaCidade")
        {
            ConstruirMundoMultiZonas();
            AtualizarEstadoPortaoCentroNaCena();
        }
        else if (cena == "CasaDoAvo")
        {
            zonaAtual = ZonaId.Interior_CasaDoAvo;
            GarantirElementosCasaDoAvoCenaSeparada();
        }
        else if (cena == "BarDoZe")
        {
            zonaAtual = ZonaId.Interior_BarDoZe;
            GarantirElementosBarDoZeCenaSeparada();
        }
        SantaRitaPlaceDressing.ApplyC1();
        SantaRitaWorldLighting.Apply();
    }

    private void PrepararMusicasDasZonas()
    {
        if (musicaBairro == null)
            musicaBairro = CriarFonteMusical("Musica_BairroCasaAvo", "Cap1/Audio/Bairro_Casa_do_Avo_loop");
        if (musicaBar == null)
            musicaBar = CriarFonteMusical("Musica_BarDoZe", "Cap1/Audio/Bar_do_Ze_loop");
    }

    private AudioSource CriarFonteMusical(string nome, string caminho)
    {
        AudioClip clipe = Resources.Load<AudioClip>(caminho);
        if (clipe == null)
        {
            Debug.LogWarning("Música não encontrada: " + caminho);
            return null;
        }

        GameObject objetoFonte = new GameObject(nome);
        objetoFonte.transform.SetParent(transform, false);
        AudioSource fonte = objetoFonte.AddComponent<AudioSource>();
        fonte.clip = clipe;
        fonte.loop = true;
        fonte.playOnAwake = false;
        fonte.spatialBlend = 0f;
        fonte.volume = 0f;
        return fonte;
    }

    private void Update()
    {
        if (!Application.isPlaying) return;
        AtualizarTeclasPainelFechamento();
        SantaRitaBarAudio.DefinirContexto(zonaAtual == ZonaId.Interior_BarDoZe, SinucaMinigameUI.MaoEmCurso);

        // A música do bairro continua sem reiniciar ao entrar na casa; no bar
        // ela só toca no interior. O estado da partida controla a redução de volume.
        float alvoBairro = zonaAtual == ZonaId.Zona4_Bairro ? VolumeBairro :
            zonaAtual == ZonaId.Interior_CasaDoAvo ? VolumeCasaAvo : 0f;
        float alvoBar = zonaAtual == ZonaId.Interior_BarDoZe ?
            (SinucaMinigameUI.IsShowing ? VolumeSinuca : VolumeBar) : 0f;
        // O volume geral já é aplicado globalmente pelo AudioListener.
        float volumeUsuario = Mathf.Clamp01(PlayerPrefs.GetFloat("audio_musica", 1f));

        AtualizarFonteMusical(musicaBairro, alvoBairro * volumeUsuario);
        AtualizarFonteMusical(musicaBar, alvoBar * volumeUsuario);
    }

    private static void AtualizarFonteMusical(AudioSource fonte, float alvo)
    {
        if (fonte == null) return;
        if (alvo > 0f && !fonte.isPlaying) fonte.Play();
        fonte.volume = Mathf.MoveTowards(fonte.volume, alvo, VelocidadeFadeMusica * Time.unscaledDeltaTime);
        if (alvo <= 0f && fonte.volume <= 0f && fonte.isPlaying) fonte.Stop();
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

        // Ocultar apenas os objetos conhecidos do protótipo. Novas raízes de
        // autoria continuam ativas, mesmo sem fazer parte do montador de C1.
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
                // O objetivo só muda quando Marcus tiver algo concreto para perguntar ao Zé.
                GameState.Marcar(Flags.EntrouNoBarDoZe);
            }
        });
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

        // A arquitetura e a sinalização física apresentam o lugar durante a exploração.
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
            "Casa do vô destrancada",
            "FATO",
            "A porta da casa 40 estava destrancada, as luzes acesas, e o vô não respondeu."
        );
        if (!GameState.Tem(Flags.OuviuRelatoAtendente) && !GameState.Tem(Flags.ZeDerrotadoNaSinuca))
            GameState.DefinirObjetivo("Descobrir o que aconteceu na casa do vô.");
        StartCoroutine(NarracaoEntradaCasaAvoRoutine());
    }

    private IEnumerator NarracaoEntradaCasaAvoRoutine()
    {
        yield return new WaitForSeconds(0.8f);
        Legenda.Dizer("\"Vô? ...Vô Adílio?\"", 3.0f);
        yield return new WaitForSeconds(3.3f);
        Legenda.Dizer("\"A porta aberta. Tudo aceso. E ninguém.\"", 3.5f);
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
            case TilePiso.Grama: return "Grama_tiles";
            case TilePiso.Terra: return "Terra";
            case TilePiso.TerraRachada: return "Terra_Rachada";
            case TilePiso.AsfaltoH_Trinca: return "Asfalto_Trinca";
            case TilePiso.AsfaltoH_Poca: return "Asfalto_Poca";
            case TilePiso.AsfaltoH_Buraco: return "Asfalto_Buraco";
            case TilePiso.AsfaltoV: return "Estrada_Asfalto";
            case TilePiso.CalcadaNorte: return "Concreto";
            case TilePiso.CalcadaSul: return "Concreto";
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
            default: return "Grama_tiles";
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
        int cols = larguraUnidades;
        int rows = alturaUnidades;
        const int cellPx = 32;

        Texture2D texFinal = new Texture2D(cols * cellPx, rows * cellPx, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp,
            name = "Piso_" + nome
        };

        var cacheBlocos = new Dictionary<int, Color32[]>();

        for (int cy = 0; cy < rows; cy++)
        {
            for (int cx = 0; cx < cols; cx++)
            {
                int ux = cx;
                int uy = cy;
                TilePiso tipo = seletorTile(ux, uy);

                int variantKey=(int)tipo*16+(cx%4)+(cy%4)*4;
                if (!cacheBlocos.TryGetValue(variantKey, out Color32[] bloco))
                {
                    bloco = ExtrairBloco32(tipo,cx,cy);
                    cacheBlocos[variantKey] = bloco;
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

        // Sinalização nas ruas asfaltadas externas
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
                    bool eixo = Mathf.Abs(yUnidade - 9.95f) < 0.055f && (px / 80) % 2 == 0;
                    bool faixaPedestres = nome == "Zona3_Encruzilhada" &&
                        px >= 10 * cellPx && px < 14 * cellPx &&
                        yUnidade > 8.23f && yUnidade < 11.77f && (px / 14) % 2 == 0;
                    if (faixaPedestres || borda) texFinal.SetPixel(px, py, new Color(0.86f, 0.84f, 0.72f));
                    else if (eixo) texFinal.SetPixel(px, py, new Color(0.92f, 0.73f, 0.30f));
                }
            }
        }

        // Acabamento arquitetônico completo para os 4 Interiores (paredes reais com rodapé, lambril/azulejo, cornija e sombra projetada no chão)
        if (nome.StartsWith("Interior_"))
        {
            PintarAcabamentoArquitetonicoInterior(texFinal, nome, cols, rows, cellPx);
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

    private static void PintarAcabamentoArquitetonicoInterior(Texture2D tex, string nome, int cols, int rows, int cellPx)
    {
        int w = tex.width;
        int h = tex.height;
        int wallStartY = (rows - 2) * cellPx; // As 2 fileiras superiores formam a parede norte de 64px de altura

        bool isPosto = nome.Contains("Posto");
        bool isMercado = nome.Contains("Mercadinho");
        bool isCasaAvo = nome.Contains("CasaDoAvo");
        bool isBar = nome.Contains("BarDoZe");

        Color32 corRodapeEscuro = new Color32(54, 36, 24, 255);
        Color32 corRodapeLuz = new Color32(96, 66, 44, 255);
        Color32 corCornijaTopo = new Color32(42, 30, 22, 255);
        Color32 corMolduraMeio = isPosto ? new Color32(52, 108, 132, 255) :
                                 isMercado ? new Color32(62, 112, 78, 255) :
                                 new Color32(92, 58, 36, 255);

        // 1. Pintar a Parede Norte detalhada (64 pixels de altura: rodapé + lambril/azulejo + faixa + parede superior + cornija)
        for (int py = wallStartY; py < h; py++)
        {
            int wy = py - wallStartY; // 0..63
            for (int px = 0; px < w; px++)
            {
                int ruido = ((px * 13 + wy * 29 + px * wy) % 9) - 4;
                Color32 c;

                if (wy < 5)
                {
                    // Rodapé na base da parede norte
                    c = (wy == 4) ? corRodapeLuz : corRodapeEscuro;
                }
                else if (wy < 28)
                {
                    // Metade inferior da parede: Azulejo comercial (Posto/Mercado) ou Lambril de madeira (Casa do Avô / Bar)
                    if (isPosto || isMercado)
                    {
                        bool rejunte = (px % 16 == 0) || (wy % 8 == 0);
                        Color32 azulejoBase = isPosto ? new Color32(212, 220, 222, 255) : new Color32(224, 216, 198, 255);
                        Color32 rejunteCor = isPosto ? new Color32(148, 160, 166, 255) : new Color32(164, 152, 134, 255);
                        c = rejunte ? rejunteCor : AjustarBrilho(azulejoBase, ruido / 2);
                    }
                    else
                    {
                        // Lambril de madeira canelada vertical
                        int modX = px % 12;
                        bool friso = (modX == 0 || modX == 11);
                        Color32 madeiraBase = isCasaAvo ? new Color32(124, 82, 52, 255) : new Color32(92, 58, 38, 255);
                        c = friso ? new Color32(58, 36, 22, 255) : AjustarBrilho(madeiraBase, (modX == 2 ? 8 : 0) + ruido / 2);
                    }
                }
                else if (wy < 32)
                {
                    // Faixa divisória (Dado rail / Pastilha decorativa)
                    c = (wy == 31) ? AjustarBrilho(corMolduraMeio, 22) : corMolduraMeio;
                }
                else if (wy < 58)
                {
                    // Parte superior da parede: Pintura rebocada (Posto/Mercado), Papel de parede listrado clássico (Casa do Avô) ou Tijolo aparente/reboco rústico (Bar do Zé)
                    if (isCasaAvo)
                    {
                        bool listra = (px % 16 < 3);
                        Color32 papelBase = listra ? new Color32(198, 178, 142, 255) : new Color32(214, 196, 162, 255);
                        c = AjustarBrilho(papelBase, ruido / 2);
                    }
                    else if (isBar)
                    {
                        // Parede rústica de bar com tijolinhos aparentes em partes e reboco ocre
                        int brickRow = (wy - 32) / 6;
                        int brickX = (px + (brickRow % 2) * 8) % 16;
                        bool argamassa = ((wy - 32) % 6 == 0) || (brickX == 0);
                        bool areaTijolo = ((px / 32 + brickRow) % 3 == 0);
                        if (areaTijolo)
                        {
                            c = argamassa ? new Color32(118, 102, 86, 255) : AjustarBrilho(new Color32(146, 78, 54, 255), ruido);
                        }
                        else
                        {
                            c = AjustarBrilho(new Color32(158, 134, 104, 255), ruido);
                        }
                    }
                    else
                    {
                        Color32 tintaSuperior = isPosto ? new Color32(228, 224, 210, 255) : new Color32(234, 222, 194, 255);
                        c = AjustarBrilho(tintaSuperior, ruido / 2);
                    }
                }
                else
                {
                    // Cornija / Viga superior do teto
                    c = (wy == 58) ? corRodapeLuz : corCornijaTopo;
                }

                // Pilares de madeira/alvenaria a cada 6 unidades para quebrar a monotonia da parede
                if ((px < 12 || px >= w - 12 || (px % (6 * cellPx) < 6)) && wy >= 5 && wy < 58)
                {
                    c = AjustarBrilho(c, -18);
                }

                tex.SetPixel(px, py, c);
            }
        }

        // 2. Sombra suave projetada pela parede norte sobre o piso (6 pixels abaixo da parede)
        for (int py = wallStartY - 6; py < wallStartY; py++)
        {
            if (py < 0) continue;
            int dist = wallStartY - py; // 1..6
            int escurecer = (7 - dist) * 6;
            for (int px = 0; px < w; px++)
            {
                Color32 atual = tex.GetPixel(px, py);
                tex.SetPixel(px, py, AjustarBrilho(atual, -escurecer));
            }
        }

        // 3. Moldura de paredes laterais (Esquerda, Direita) e parede Sul (com abertura para a porta central!)
        int espessuraBorda = 10;
        int centroPortaPx = w / 2;
        int larguraPortaPx = 44; // Abertura da porta de saída no sul

        for (int py = 0; py < wallStartY; py++)
        {
            for (int px = 0; px < w; px++)
            {
                bool bordaEsq = px < espessuraBorda;
                bool bordaDir = px >= w - espessuraBorda;
                bool bordaSul = py < espessuraBorda && Mathf.Abs(px - centroPortaPx) > larguraPortaPx;

                if (bordaEsq || bordaDir || bordaSul)
                {
                    bool linhaInternaRodape = (bordaEsq && px >= espessuraBorda - 2) ||
                                              (bordaDir && px <= w - espessuraBorda + 1) ||
                                              (bordaSul && py >= espessuraBorda - 2);
                    tex.SetPixel(px, py, linhaInternaRodape ? corRodapeLuz : corCornijaTopo);
                }
                else if (px < espessuraBorda + 4 || px >= w - espessuraBorda - 4)
                {
                    // Sombra lateral discreta no chão junto ao rodapé
                    Color32 atual = tex.GetPixel(px, py);
                    tex.SetPixel(px, py, AjustarBrilho(atual, -14));
                }
            }
        }
    }

    private static Color32 AjustarBrilho(Color32 c, int delta)
    {
        return new Color32(
            (byte)Mathf.Clamp(c.r + delta, 0, 255),
            (byte)Mathf.Clamp(c.g + delta, 0, 255),
            (byte)Mathf.Clamp(c.b + delta, 0, 255),
            255
        );
    }

    private static Color32[] ExtrairBloco32(TilePiso tipo, int tileX=0, int tileY=0)
    {
        const int cellPx = 32;
        Color32[] result = new Color32[cellPx * cellPx];
        string material = tipo == TilePiso.Grama ? "Grama" : tipo == TilePiso.Terra || tipo == TilePiso.TerraRachada ? "Terra" :
            tipo == TilePiso.AsfaltoH_Trinca || tipo == TilePiso.AsfaltoH_Poca || tipo == TilePiso.AsfaltoH_Buraco || tipo == TilePiso.AsfaltoV || tipo == TilePiso.Cruzamento ? "Asfalto" :
            tipo == TilePiso.CalcadaNorte || tipo == TilePiso.CalcadaSul || tipo == TilePiso.Concreto ? "Calcada" :
            tipo == TilePiso.InteriorSmall ? "PisoVerde" : tipo == TilePiso.InteriorBeige ? "PisoCreme" :
            tipo == TilePiso.InteriorBar || tipo == TilePiso.InteriorPatterned ? "Madeira" : null;
        var macro=material!=null?Resources.Load<Texture2D>("Art/Macros/"+material+"_Macro"):null;
        if(macro!=null && macro.isReadable)
        {
            var pixels=macro.GetPixels((tileX%4)*32,(tileY%4)*32,32,32);
            for(int i=0;i<result.Length;i++)result[i]=pixels[i];return result;
        }
        var final = material != null ? Resources.Load<Texture2D>("Art/Tiles/" + material) : null;
        if (final != null && final.isReadable && final.width == 32 && final.height == 32)
        {
            result = final.GetPixels32();
            if (tipo == TilePiso.CalcadaNorte || tipo == TilePiso.CalcadaSul)
                for (int y=0;y<32;y++)for(int x=0;x<32;x++)
                    if ((tipo==TilePiso.CalcadaNorte && y<3)||(tipo==TilePiso.CalcadaSul && y>28))
                        result[y*32+x]=new Color32(151,148,133,255);
            return result;
        }

        // 1. GRAMA NATURAL CONTÍNUA (Sem bordas quadradas!) usando Grama_tiles.png
        if (tipo == TilePiso.Grama)
        {
            Texture2D texGrama = CarregarTextura("Grama_tiles");
            if (texGrama != null && texGrama.width >= 48 && texGrama.height >= 48)
            {
                for (int y = 0; y < cellPx; y++)
                {
                    for (int x = 0; x < cellPx; x++)
                    {
                        // Amostra a região central contínua de Grama_tiles.png (sem tocar nas bordas da folha)
                        int gx = 16 + (x % 32);
                        int gy = 16 + (y % 32);
                        Color32 gc = texGrama.GetPixel(gx, gy);
                        // Suaviza o tom para noite chuvosa de Santa Rita
                        result[y * cellPx + x] = new Color32(
                            (byte)Mathf.Clamp((int)(gc.r * 0.78f), 22, 95),
                            (byte)Mathf.Clamp((int)(gc.g * 0.82f), 42, 125),
                            (byte)Mathf.Clamp((int)(gc.b * 0.78f), 24, 85),
                            255);
                    }
                }
                return result;
            }
        }

        // 2. CALÇADA DE PEDRA / PASSEIO BRASILEIRO (Cinza de pedra com meio-fio, NUNCA verde!)
        if (tipo == TilePiso.CalcadaNorte || tipo == TilePiso.CalcadaSul || tipo == TilePiso.Concreto)
        {
            for (int y = 0; y < cellPx; y++)
            {
                for (int x = 0; x < cellPx; x++)
                {
                    int ruido = ((x * 19 + y * 37 + x * y * 5) % 11) - 5;
                    bool meioFioNorte = (tipo == TilePiso.CalcadaNorte && y < 4);
                    bool meioFioSul = (tipo == TilePiso.CalcadaSul && y >= cellPx - 4);

                    if (meioFioNorte || meioFioSul)
                    {
                        bool sarjetaEscura = (meioFioNorte && y == 0) || (meioFioSul && y == cellPx - 1);
                        Color32 corGuia = sarjetaEscura
                            ? new Color32(62, 64, 66, 255)
                            : new Color32(182, 178, 168, 255);
                        result[y * cellPx + x] = AjustarBrilho(corGuia, ruido / 2);
                        continue;
                    }

                    // Lajotas de pedra/concreto da calçada (16x16 px cada lajota)
                    int subX = x % 16;
                    int subY = y % 16;
                    bool junta = (subX == 0 || subY == 0);
                    bool biselLuz = (subX == 1 || subY == 15);
                    bool alterna = (((x / 16) + (y / 16)) % 2 == 0);

                    Color32 pedraBase = alterna
                        ? new Color32(138, 136, 130, 255)
                        : new Color32(130, 128, 122, 255);

                    int ajuste = ruido / 2 + (junta ? -24 : (biselLuz ? 8 : 0));
                    result[y * cellPx + x] = AjustarBrilho(pedraBase, ajuste);
                }
            }
            return result;
        }

        // 3. ASFALTO DA AVENIDA
        bool asfalto = tipo == TilePiso.AsfaltoH_Trinca || tipo == TilePiso.AsfaltoH_Poca ||
            tipo == TilePiso.AsfaltoH_Buraco || tipo == TilePiso.AsfaltoV || tipo == TilePiso.Cruzamento;
        if (asfalto)
        {
            Color32 baseAsfalto = new Color32(46, 50, 52, 255);
            for (int y = 0; y < cellPx; y++)
            {
                for (int x = 0; x < cellPx; x++)
                {
                    int ruido = ((x * 17 + y * 31 + x * y * 3) % 11) - 5;
                    result[y * cellPx + x] = AjustarBrilho(baseAsfalto, ruido / 2);
                }
            }
            return result;
        }

        // 4. PISOS INTERNOS REALISTAS (Cerâmica Comercial, Lajota de Mercearia, Assoalho de Tábua Corrida e Piso Rústico de Bar)
        if (tipo == TilePiso.InteriorSmall || tipo == TilePiso.InteriorBeige)
        {
            // Cerâmica esmaltada 16x16 com rejunte, bisel de brilho e leve variação entre peças
            bool posto = (tipo == TilePiso.InteriorSmall);
            for (int y = 0; y < cellPx; y++)
            {
                for (int x = 0; x < cellPx; x++)
                {
                    int tx = x % 16;
                    int ty = y % 16;
                    int qx = x / 16;
                    int qy = y / 16;
                    bool xadrez = ((qx + qy) % 2 == 0);
                    bool rejunte = (tx == 0 || ty == 0);
                    bool biselClaro = !rejunte && (tx == 1 || ty == 15);
                    bool biselSombra = !rejunte && (tx == 15 || ty == 1);
                    bool detalheCentro = (tx >= 7 && tx <= 8 && ty >= 7 && ty <= 8);

                    Color32 corPeca;
                    if (posto)
                    {
                        corPeca = xadrez
                            ? new Color32(196, 190, 176, 255)
                            : new Color32(178, 172, 158, 255);
                    }
                    else
                    {
                        corPeca = xadrez
                            ? new Color32(198, 168, 134, 255)
                            : new Color32(176, 142, 108, 255);
                    }

                    int ruido = ((x * 13 + y * 29) % 7) - 3;
                    int delta = ruido + (rejunte ? -38 : (biselClaro ? 12 : (biselSombra ? -10 : (detalheCentro ? -8 : 0))));
                    result[y * cellPx + x] = AjustarBrilho(corPeca, delta);
                }
            }
            return result;
        }

        if (tipo == TilePiso.InteriorPatterned || tipo == TilePiso.InteriorBar)
        {
            // Assoalho de tábua corrida de madeira (Casa do Avô Adílio e Bar do Zé)
            bool bar = (tipo == TilePiso.InteriorBar);
            for (int y = 0; y < cellPx; y++)
            {
                int tabuaIdx = y / 8; // 4 tábuas horizontais de 8px por bloco 32x32
                int pyTabua = y % 8;
                int offsetEmenda = (tabuaIdx % 2 == 0) ? 14 : 28;

                for (int x = 0; x < cellPx; x++)
                {
                    bool frestaHorizontal = (pyTabua == 0);
                    bool emendaVertical = (x == offsetEmenda);
                    bool prego = (pyTabua == 2 || pyTabua == 6) && (x == (offsetEmenda + 2) % 32);

                    Color32 corMadeiraBase;
                    if (bar)
                    {
                        corMadeiraBase = (tabuaIdx % 2 == 0)
                            ? new Color32(114, 76, 48, 255)
                            : new Color32(102, 66, 42, 255);
                    }
                    else
                    {
                        corMadeiraBase = (tabuaIdx % 2 == 0)
                            ? new Color32(154, 104, 64, 255)
                            : new Color32(140, 92, 56, 255);
                    }

                    // Veios longitudinais da madeira
                    int veio = Mathf.RoundToInt(Mathf.Sin((x + tabuaIdx * 9) * 0.45f + pyTabua * 0.9f) * 6f);
                    int delta = veio;
                    if (frestaHorizontal || emendaVertical) delta = -36;
                    else if (pyTabua == 7) delta += 8; // brilho na borda da tábua
                    else if (prego) delta = -28;

                    result[y * cellPx + x] = AjustarBrilho(corMadeiraBase, delta);
                }
            }
            return result;
        }

        if (tipo == TilePiso.ParedeAzulejo || tipo == TilePiso.ParedePapel || tipo == TilePiso.ParedeBar)
        {
            // Preenchimento base (sobrescrito em seguida por PintarAcabamentoArquitetonicoInterior)
            Color32 baseParede = new Color32(180, 168, 148, 255);
            for (int i = 0; i < result.Length; i++) result[i] = baseParede;
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
            int ty = Mathf.Clamp(sy * sh / cellPx, 0, sh - 1);
            for (int x = 0; x < cellPx; x++)
            {
                int tx = Mathf.Clamp(x * sw / cellPx, 0, sw - 1);
                Color c = src.GetPixel(tx, ty);
                if (c.a < 0.5f)
                {
                    c = new Color(0.36f, 0.34f, 0.31f, 1f);
                }
                result[y * cellPx + x] = c;
            }
        }

        return result;
    }

    private static GameObject ColocarArvore(Transform pai, string nome, Vector3 pos, bool pequena = false, bool comColisor = true)
    {
        string spriteName = pequena ? "Oak_Tree_Small" : "Oak_Tree";
        float escala = pequena ? 1.15f : 1.25f;
        Vector2 colSize = comColisor ? (pequena ? new Vector2(0.7f, 0.5f) : new Vector2(0.95f, 0.65f)) : Vector2.zero;
        Vector2 colOffset = comColisor ? new Vector2(0f, 0.35f) : Vector2.zero;
        return ColocarProp(pai, nome, pos, spriteName, 32f, escala, colSize, colOffset, 1);
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

        GameObject alvo=go;
        if(tipo==InteracaoCapitulo1.TipoInteracao.NpcFrentista||tipo==InteracaoCapitulo1.TipoInteracao.NpcAtendenteMercadinho)
        {
            alvo=new GameObject("ConversaPeloBalcao");alvo.transform.SetParent(go.transform,false);alvo.transform.localPosition=Vector3.down*1.5f;
        }
        InteracaoCapitulo1 inter = alvo.AddComponent<InteracaoCapitulo1>();
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
            // 1. Asfalto/Rua central (Y = 8..11)
            if (uy >= 8 && uy <= 11) return TilePiso.AsfaltoH_Trinca;
            // 2. Calçada de pedra contínua na margem Norte (Y = 12..13) e Sul (Y = 6..7)
            if (uy == 12) return TilePiso.CalcadaNorte;
            if (uy == 13) return TilePiso.Concreto;
            // Recuo pavimentado junto à calçada para o carro marrom de Marcus
            if (uy == 14 && ux >= 6 && ux <= 12) return TilePiso.Concreto;
            if (uy == 7) return TilePiso.CalcadaSul;
            if (uy == 6 && ux >= 18 && ux <= 26) return TilePiso.Terra; // acostamento de barro onde caiu a mala
            if (uy == 6) return TilePiso.Concreto;
            // 3. Atrás da calçada: Grama natural contínua (com árvores colocadas abaixo!)
            return TilePiso.Grama;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.5f);

        // Árvores na grama atrás da calçada norte e atrás da calçada sul (sequência: Rua -> Calçada -> Grama/Árvores)
        float[] arvoresNorteZ1 = { 103.0f, 115.2f, 118.0f, 123.5f, 128.2f, 132.0f };
        for (int i = 0; i < arvoresNorteZ1.Length; i++)
        {
            ColocarArvore(z.transform, "Arvore_Norte_Z1_" + i, new Vector3(arvoresNorteZ1[i], 15.2f + (i % 2) * 0.8f, 0f), pequena: (i % 3 == 1));
        }
        float[] arvoresSulZ1 = { 103.5f, 109.0f, 114.5f, 120.0f, 127.0f, 132.5f };
        for (int i = 0; i < arvoresSulZ1.Length; i++)
        {
            ColocarArvore(z.transform, "Arvore_Sul_Z1_" + i, new Vector3(arvoresSulZ1[i], 2.2f + (i % 2) * 0.9f, 0f), pequena: (i % 2 == 1));
        }

        // Carro de Marcus estacionado no recuo pavimentado da rodovia (nunca na grama!)
        ColocarProp(z.transform, "Carro_Marcus_Sprite", new Vector3(108.5f, 12.2f, 0f), "Sedan_Brown", 32f, 1.15f,
            new Vector2(2.6f, 1.2f), new Vector2(0f, 0.6f), 2);
        ColocarInteracao(z.transform, "Carro_Marcus_Interacao", new Vector3(108.5f, 11.4f, 0f),
            InteracaoCapitulo1.TipoInteracao.CarroMarcus, "");

        // Placa de Entrada de Santa Rita
        ColocarProp(z.transform, "Placa_SantaRita", new Vector3(113.5f, 13.4f, 0f), "PlacaEntrada", 16f, 0.9f,
            new Vector2(2.4f, 0.8f), new Vector2(0f, 0.4f), 2);

        // Abrigo / Parada de Ônibus na Calçada + Quadro de Avisos
        ColocarProp(z.transform, "Banco_Rodovia", new Vector3(119.5f, 13.2f, 0f), "Bench_Wood", 32f, 1.1f,
            new Vector2(1.6f, 0.6f), new Vector2(0f, 0.3f), 1);
        ColocarProp(z.transform, "Lixeira_Rodovia", new Vector3(121.2f, 13.2f, 0f), "TrashCan", 32f, 1.0f,
            new Vector2(0.6f, 0.5f), new Vector2(0f, 0.25f), 1);
        ColocarProp(z.transform, "Placa_Horarios_Rodovia", new Vector3(119.5f, 13.9f, 0f), "RoadSign_Blank", 32f, 1.1f,
            Vector2.zero, Vector2.zero, 1);
        ColocarInteracao(z.transform, "Interacao_PlacaRodovia", new Vector3(119.5f, 12.7f, 0f),
            InteracaoCapitulo1.TipoInteracao.PlacaRodoviaEntrada, "");

        // Orelhão #1 (Save Point da Entrada sobre a calçada)
        ColocarOrelhaoSave(z.transform, "Orelhao_Rodovia", "orelhao_entrada_rodovia", new Vector3(126.5f, 13.1f, 0f));

        // Pista na margem sul: Mala e caixotes abandonados
        ColocarProp(z.transform, "Caixotes_Barro_Sul", new Vector3(122.5f, 6.6f, 0f), "Caixotes", 16f, 0.85f,
            new Vector2(1.4f, 0.8f), new Vector2(0f, 0.4f), 1);
        ColocarInteracao(z.transform, "Interacao_MalaEstrada", new Vector3(122.5f, 7.3f, 0f),
            InteracaoCapitulo1.TipoInteracao.MalaAbandonadaEstrada, "");

        // Postes de luz e fiação sobre a calçada de pedra
        Color luzAmbar = new Color(1.0f, 0.82f, 0.52f, 1f);
        ColocarPosteComLuz(z.transform, "Poste_Z1_1", new Vector3(106.0f, 13.0f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z1_2", new Vector3(116.0f, 13.0f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z1_3", new Vector3(125.0f, 13.0f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z1_4", new Vector3(133.0f, 13.0f, 0f), luzAmbar);
        ColocarProp(z.transform, "UtilityPole_Z1_1", new Vector3(111.0f, 13.3f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);
        ColocarProp(z.transform, "UtilityPole_Z1_2", new Vector3(129.5f, 13.3f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);

        // Cerca de madeira entre a calçada sul e a mata sul
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
    // ZONA 2: POSTO DE GASOLINA & ANTIGA VENDA (Sequência: Rua -> Calçada -> Estabelecimentos -> Grama e Árvores!)
    // =========================================================================

    private void ConstruirZona2_Posto(Transform raiz)
    {
        GameObject z = new GameObject("Zona2_Posto");
        z.transform.SetParent(raiz, false);
        Vector2 min = new Vector2(200f, 0f);
        Vector2 max = new Vector2(238f, 22f);

        CriarPisoDeZona(z.transform, "Zona2_Posto", min, 38, 22, (ux, uy) =>
        {
            // 1. Avenida de Asfalto (Y = 8..11)
            if (uy >= 8 && uy <= 11) return TilePiso.AsfaltoH_Trinca;
            // 2. Calçada Sul de pedra (Y = 6..7) e Calçada Norte de pedra (Y = 12..13)
            if (uy == 7) return TilePiso.CalcadaSul;
            if (uy == 6) return TilePiso.Concreto;
            if (uy == 12) return TilePiso.CalcadaNorte;
            if (uy == 13) return TilePiso.Concreto;
            // 3. Base pavimentada sob o Posto (ux 7..16) e sob a Antiga Venda (ux 24..32)
            if (uy == 14 && ((ux >= 7 && ux <= 16) || (ux >= 24 && ux <= 32))) return TilePiso.Concreto;
            // 4. Atrás das construções e calçadas: Grama natural e árvores!
            return TilePiso.Grama;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.5f);

        // Árvores na grama atrás do Posto, entre o Posto e a Venda, e na margem sul (atrás da calçada)
        float[] arvoresNorteZ2 = { 203.0f, 206.0f, 218.5f, 221.5f, 234.5f, 236.5f };
        for (int i = 0; i < arvoresNorteZ2.Length; i++)
        {
            ColocarArvore(z.transform, "Arvore_Norte_Z2_" + i, new Vector3(arvoresNorteZ2[i], 16.5f + (i % 2) * 0.8f, 0f), pequena: (i % 2 == 0));
        }
        float[] arvoresSulZ2 = { 204.0f, 210.0f, 216.0f, 222.5f, 229.0f, 234.5f };
        for (int i = 0; i < arvoresSulZ2.Length; i++)
        {
            ColocarArvore(z.transform, "Arvore_Sul_Z2_" + i, new Vector3(arvoresSulZ2[i], 2.4f + (i % 2) * 0.9f, 0f), pequena: (i % 3 == 0));
        }

        // 1. POSTO DE GASOLINA (À esquerda na calçada norte)
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
            InteracaoCapitulo1.TipoInteracao.BombaCombustivelPosto, "");

        ColocarProp(z.transform, "Picape_No_Posto", new Vector3(207.8f, 13.4f, 0f), "Picape_Red", 32f, 1.15f,
            new Vector2(2.5f, 1.2f), new Vector2(0f, 0.6f), 2);
        ColocarInteracao(z.transform, "Picape_Posto_Interacao", new Vector3(207.8f, 12.6f, 0f),
            InteracaoCapitulo1.TipoInteracao.CarroEstacionadoPosto, "");

        // Carro estacionado corretamente no meio-fio do asfalto
        ColocarProp(z.transform, "Sedan_Avenida_Posto", new Vector3(220.5f, 11.4f, 0f), "Sedan_Black", 32f, 1.15f,
            new Vector2(2.5f, 1.1f), new Vector2(0f, 0.55f), 2);

        // 2. ANTIGA VENDA (À direita do Posto na mesma avenida)
        ColocarProp(z.transform, "Predio_Venda_Antiga", new Vector3(228.5f, 14.2f, 0f), "Venda", 16f, 0.90f,
            new Vector2(6.2f, 2.2f), new Vector2(0f, 1.5f), 2);
        ColocarProp(z.transform, "Caixotes_Venda", new Vector3(224.8f, 13.8f, 0f), "Caixotes", 16f, 0.85f,
            new Vector2(1.2f, 0.8f), new Vector2(0f, 0.4f), 2);
        ColocarProp(z.transform, "Banco_Venda", new Vector3(232.2f, 13.6f, 0f), "Bench_Wood", 32f, 1.05f,
            new Vector2(1.5f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarInteracao(z.transform, "Porta_Venda_Antiga", new Vector3(228.5f, 13.6f, 0f),
            InteracaoCapitulo1.TipoInteracao.VendaAntigaFechada, "");

        // Postes de luz e fiação na calçada
        Color luzAmbar = new Color(1.0f, 0.82f, 0.52f, 1f);
        ColocarPosteComLuz(z.transform, "Poste_Z2_1", new Vector3(204.5f, 13.0f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z2_2", new Vector3(217.5f, 13.0f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z2_3", new Vector3(224.0f, 13.0f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z2_4", new Vector3(234.0f, 13.0f, 0f), luzAmbar);
        ColocarProp(z.transform, "UtilityPole_Z2_1", new Vector3(216.0f, 13.4f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);
        ColocarProp(z.transform, "UtilityPole_Z2_2", new Vector3(233.0f, 13.4f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);

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

        // Porta de saída no sul da sala
        SantaRitaExitThreshold.Create(z.transform,"SoleiraSaida_Posto",new Vector2(208,-59.2f),"SaidaComercio",2.3f,-10);
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
            InteracaoCapitulo1.TipoInteracao.NpcFrentista, "", "Npc_Frentista", 32f, 1.0f, 2, solido: true);

        // Interações extras no balcão do Posto
        ColocarInteracao(z.transform, "Livro_Plantao_Posto", new Vector3(206.5f, -53.0f, 0f),
            InteracaoCapitulo1.TipoInteracao.LivroRegistroPosto, "");
        ColocarInteracao(z.transform, "TV_Portatil_Posto", new Vector3(209.6f, -53.0f, 0f),
            InteracaoCapitulo1.TipoInteracao.TvPortatilPosto, "");
    }

    // =========================================================================
    // ZONA 3: ENCRUZILHADA & MERCADINHO (Sequência: Rua -> Calçada -> Mercadinho/Casas -> Grama e Árvores!)
    // =========================================================================

    private void ConstruirZona3_Encruzilhada(Transform raiz)
    {
        GameObject z = new GameObject("Zona3_Encruzilhada");
        z.transform.SetParent(raiz, false);
        Vector2 min = new Vector2(300f, 0f);
        Vector2 max = new Vector2(340f, 26f);

        CriarPisoDeZona(z.transform, "Zona3_Encruzilhada", min, 40, 26, (ux, uy) =>
        {
            // Cruzamento central (X = 18..22, Y = 8..13)
            if (ux >= 18 && ux <= 22 && uy >= 8 && uy <= 13) return TilePiso.Cruzamento;
            // Subida de barro rumo ao Bairro das Casas (X = 17..23, Y >= 14)
            if (ux >= 18 && ux <= 22 && uy == 14) return TilePiso.AsfaltoParaTerra;
            if (ux >= 17 && ux <= 23 && uy >= 15) return (uy % 5 == 0) ? TilePiso.TerraRachada : TilePiso.Terra;
            // 1. Avenida de Asfalto Horizontal (Y = 8..11)
            if (uy >= 8 && uy <= 11) return TilePiso.AsfaltoH_Trinca;
            // 2. Calçada Sul de pedra (Y = 6..7) e Calçada Norte de pedra (Y = 12..13)
            if (uy == 7) return TilePiso.CalcadaSul;
            if (uy == 6) return TilePiso.Concreto;
            if (uy == 12) return TilePiso.CalcadaNorte;
            if (uy == 13) return TilePiso.Concreto;
            // 3. Base sob o Mercadinho (ux 6..15) e Casa da Esquina (ux 27..34)
            if (uy == 14 && ((ux >= 6 && ux <= 15) || (ux >= 27 && ux <= 34))) return TilePiso.Concreto;
            // 4. Atrás das calçadas e construções: Grama natural e árvores!
            return TilePiso.Grama;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.5f);

        // Árvores na grama atrás do Mercadinho, ao longo da subida do Bairro e atrás da calçada sul
        float[] arvoresNorteZ3 = { 303.0f, 306.0f, 314.8f, 325.2f, 336.5f };
        for (int i = 0; i < arvoresNorteZ3.Length; i++)
        {
            ColocarArvore(z.transform, "Arvore_Norte_Z3_" + i, new Vector3(arvoresNorteZ3[i], 18.2f + (i % 2) * 1.1f, 0f), pequena: (i % 2 == 1));
        }
        float[] arvoresSulZ3 = { 304.0f, 310.0f, 316.0f, 321.5f, 331.5f, 336.0f };
        for (int i = 0; i < arvoresSulZ3.Length; i++)
        {
            ColocarArvore(z.transform, "Arvore_Sul_Z3_" + i, new Vector3(arvoresSulZ3[i], 2.4f + (i % 2) * 0.8f, 0f), pequena: (i % 3 == 0));
        }

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

        // Engradados de bebida e lixeira ao lado do Mercadinho (sobre a calçada)
        ColocarProp(z.transform, "Engradado_Merc_1", new Vector3(306.5f, 13.5f, 0f), "BeerCrate_Blue", 32f, 1.1f, new Vector2(0.8f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Engradado_Merc_2", new Vector3(305.5f, 13.5f, 0f), "BeerCrate_Orange", 32f, 1.1f, new Vector2(0.8f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Lixeira_Mercadinho", new Vector3(315.4f, 13.5f, 0f), "TrashCan", 32f, 1.05f, new Vector2(0.6f, 0.5f), new Vector2(0f, 0.25f), 2);

        // Caminhonete estacionada junto ao meio-fio sul
        ColocarProp(z.transform, "Picape_Encruzilhada_Sprite", new Vector3(326.0f, 7.2f, 0f), "Picape_Brown", 32f, 1.15f,
            new Vector2(2.6f, 1.2f), new Vector2(0f, 0.6f), 2);
        ColocarInteracao(z.transform, "Picape_Encruzilhada_Interacao", new Vector3(326.0f, 6.4f, 0f),
            InteracaoCapitulo1.TipoInteracao.PicapeEncruzilhada, "");

        // 2. LADO DIREITO DA ENCRUZILHADA (Casa da esquina + Quadro de Avisos + Cerca)
        ColocarProp(z.transform, "Casa_Esquina_Encruzilhada", new Vector3(330.5f, 14.0f, 0f), "CasaSimples_B", 16f, 0.90f,
            new Vector2(5.6f, 2.2f), new Vector2(0f, 1.4f), 2);
        ColocarProp(z.transform, "Cerca_Esquina", new Vector3(335.5f, 14.0f, 0f), "CercaMadeira", 16f, 0.9f,
            new Vector2(3.4f, 0.6f), new Vector2(0f, 0.35f), 2);
        ColocarProp(z.transform, "Banco_Encruzilhada", new Vector3(326.5f, 13.3f, 0f), "Bench_Wood", 32f, 1.1f,
            new Vector2(1.5f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Mural_Encruzilhada", new Vector3(324.8f, 13.5f, 0f), "RoadSign_Blank", 32f, 1.15f,
            new Vector2(0.8f, 0.5f), new Vector2(0f, 0.25f), 2);
        ColocarInteracao(z.transform, "Quadro_Avisos_Interacao", new Vector3(324.8f, 12.9f, 0f),
            InteracaoCapitulo1.TipoInteracao.QuadroAvisosEncruzilhada, "");

        // Placa de Seta no início da subida de barro para o Bairro das Casas
        ColocarProp(z.transform, "Placa_Subida_Bairro", new Vector3(316.8f, 17.5f, 0f), "ArrowSign", 32f, 1.2f,
            new Vector2(0.6f, 0.5f), new Vector2(0f, 0.25f), 2);
        ColocarInteracao(z.transform, "Placa_Subida_Interacao", new Vector3(316.8f, 16.9f, 0f),
            InteracaoCapitulo1.TipoInteracao.PlacaSetaBairro, "");

        // Postes de luz e fiação na encruzilhada
        Color luzAmbar = new Color(1.0f, 0.82f, 0.52f, 1f);
        ColocarPosteComLuz(z.transform, "Poste_Z3_1", new Vector3(304.5f, 13.0f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z3_2", new Vector3(316.5f, 13.0f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z3_3", new Vector3(323.5f, 13.0f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z3_4", new Vector3(317.2f, 21.0f, 0f), luzAmbar);
        ColocarProp(z.transform, "UtilityPole_Z3_1", new Vector3(316.2f, 15.2f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);
        ColocarProp(z.transform, "UtilityPole_Z3_2", new Vector3(323.2f, 18.5f, 0f), "UtilityPole", 32f, 1.1f, new Vector2(0.4f, 0.4f), Vector2.zero, 2);

        // Barricada de obra fechando a saída leste para o Centro
        ColocarArteInterior(z.transform, "Interdicao_Centro_FaixaSul", new Vector2(336.5f, 7.8f),
            "BarricadaCentro", "WoodFence", 1f, new Vector2(4.4f, 1.8f));
        ColocarArteInterior(z.transform, "Interdicao_Centro_FaixaNorte", new Vector2(336.5f, 9.6f),
            "BarricadaCentro", "WoodFence", 1f, new Vector2(4.4f, 1.8f));
        ColocarArteInterior(z.transform, "Interdicao_Centro_Passeio", new Vector2(336.5f, 11.4f),
            "BarricadaCentro", "WoodFence", 1f, new Vector2(4.4f, 1.8f));
        ColocarInteracao(z.transform, "Interacao_Interdicao_Centro", new Vector3(333.6f, 10.1f, 0f),
            InteracaoCapitulo1.TipoInteracao.CentroBloqueioObra, "");

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

        // Porta de saída no sul
        SantaRitaExitThreshold.Create(z.transform,"SoleiraSaida_Mercadinho",new Vector2(309,-59.2f),"SaidaComercio",2.3f,-10);
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
            InteracaoCapitulo1.TipoInteracao.NpcAtendenteMercadinho, "", "Npc_Atendente", 32f, 1.0f, 2, solido: true);

        // Pistas dentro do Mercadinho: Caderno de entregas e Gôndola de suprimentos
        ColocarInteracao(z.transform, "Caderno_Mercadinho_Interacao", new Vector3(307.4f, -52.5f, 0f),
            InteracaoCapitulo1.TipoInteracao.CadernoFiadoMercadinho, "");
        ColocarInteracao(z.transform, "Prateleiras_Mercadinho_Interacao", new Vector3(314.5f, -55.0f, 0f),
            InteracaoCapitulo1.TipoInteracao.PrateleirasMercadinho, "");
        // Serviço do MESMO comércio: porta traseira só utilizável após Zé.
        var servico = new GameObject("Porta_Servico_Centro");
        servico.transform.SetParent(z.transform, false);
        servico.transform.position = new Vector3(316.3f, -50.0f, 0);
        var portal = servico.AddComponent<Capitulo2Portal>();
        portal.cenaDestino = "MercadinhoDeposito"; portal.entradaDestino = "loja";
        portal.textoPrompt = "Porta de serviço / depósito";
        portal.flagNecessaria = Flags.ZeDerrotadoNaSinuca;
        portal.avisoBloqueio = "A porta de serviço está fechada pelo lado do Centro. Preciso resolver a passagem da Rua Norte com Zé.";
        AtualizarSpawnPointExistente("Retorno_Deposito", "retorno_deposito", new Vector3(316.1f, -51.5f, 0));
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

            // Calçada / Passeio de pedra diante das casas (sequência: Rua de Barro -> Calçada -> Casas -> Grama/Árvores)
            if ((uy >= 11 && uy <= 12 && ux >= 6 && ux <= 36) || (uy == 23 && ux >= 6 && ux <= 41))
            {
                return TilePiso.Concreto;
            }

            return TilePiso.Grama;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.5f);

        // Árvores na grama atrás das casas do Bairro (Sul, Meio e Norte atrás da Casa do Avô Nº 40)
        float[] arvoresSulBairro = { 404.0f, 409.5f, 414.5f, 425.5f, 431.5f, 438.0f };
        for (int i = 0; i < arvoresSulBairro.Length; i++)
        {
            ColocarArvore(z.transform, "Arvore_Sul_Z4_" + i, new Vector3(arvoresSulBairro[i], 3.2f + (i % 2) * 1.1f, 0f), pequena: (i % 2 == 1));
        }
        float[] arvoresMeioBairro = { 404.5f, 413.5f, 425.0f, 436.5f };
        for (int i = 0; i < arvoresMeioBairro.Length; i++)
        {
            ColocarArvore(z.transform, "Arvore_Meio_Z4_" + i, new Vector3(arvoresMeioBairro[i], 16.2f + (i % 2) * 0.8f, 0f), pequena: (i % 2 == 0));
        }
        float[] arvoresNorteBairro = { 404.0f, 408.5f, 415.0f, 420.5f, 427.0f, 430.5f, 441.0f };
        for (int i = 0; i < arvoresNorteBairro.Length; i++)
        {
            ColocarArvore(z.transform, "Arvore_Norte_Z4_" + i, new Vector3(arvoresNorteBairro[i], 28.2f + (i % 2) * 1.2f, 0f), pequena: (i % 3 == 1));
        }

        // --- QUARTEIRÃO SUL DO BAIRRO (Exploração da vizinhança antes de chegar na nº 40) ---
        // Casa Nº 14 (Dona Lurdes) + Van na garagem de terra + Cercas de madeira (Imagem 5)
        ColocarProp(z.transform, "Bairro_Casa_14", new Vector3(410.5f, 12.6f, 0f), "CasaSimples_A", 16f, 0.90f,
            new Vector2(5.4f, 2.0f), new Vector2(0f, 1.3f), 2);
        ColocarProp(z.transform, "Cerca_Casa14", new Vector3(406.0f, 12.4f, 0f), "CercaMadeira", 16f, 0.85f,
            new Vector2(3.2f, 0.6f), new Vector2(0f, 0.35f), 2);
        ColocarProp(z.transform, "Van_Garagem_Casa14", new Vector3(415.0f, 12.2f, 0f), "Van_White", 32f, 1.1f,
            new Vector2(2.4f, 1.2f), new Vector2(0f, 0.6f), 2);
        ColocarInteracao(z.transform, "Porta_Casa_14_Lurdes", new Vector3(410.5f, 12.0f, 0f),
            InteracaoCapitulo1.TipoInteracao.VizinhaDonaLurdes, "");
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
            InteracaoCapitulo1.TipoInteracao.GalpaoBairroPista, "");

        // --- QUARTEIRÃO NORTE DO BAIRRO (Rua da Casa do Avô Adílio — Nº 40, Imagem 5!) ---
        // Casa Nº 32 (Esquerda) + Seu Ramiro na varanda
        ColocarProp(z.transform, "Bairro_Casa_32", new Vector3(410.5f, 24.4f, 0f), "CasaSimples_B", 16f, 0.90f,
            new Vector2(5.4f, 2.0f), new Vector2(0f, 1.3f), 2);
        ColocarProp(z.transform, "Cerca_Casa32", new Vector3(406.0f, 24.2f, 0f), "CercaMadeira", 16f, 0.85f,
            new Vector2(3.2f, 0.6f), new Vector2(0f, 0.35f), 2);
        ColocarProp(z.transform, "Jardim_Casa32", new Vector3(408.0f, 25.4f, 0f), "PlanterBox", 32f, 1.15f,
            Vector2.zero, Vector2.zero, 2);
        ColocarInteracao(z.transform, "NPC_SeuRamiro_Bairro", new Vector3(414.8f, 23.4f, 0f),
            InteracaoCapitulo1.TipoInteracao.NpcMoradorBairro, "", "Npc_Ramiro", 32f, 1.0f, 2, solido: true);

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
            InteracaoCapitulo1.TipoInteracao.CaixaCorreioAvo, "");

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
        SantaRitaExitThreshold.Create(z.transform,"SoleiraSaida_CasaAvo",new Vector2(409,-59.2f),"SaidaCasa",2.3f,-10);
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
            InteracaoCapitulo1.TipoInteracao.RetratoFamiliaCasaAvo, "");

        // 1. COZINHA (Canto Noroeste): Pia + Armário + PISTA 1 (Café passado hoje mesmo)
        ColocarArteInterior(z.transform, "Bancada_Cozinha_Avo", new Vector2(404.0f, -49.2f),
            "Interior_PiaAvo", "Sink", 1.0f, new Vector2(3.3f, 0.9f));
        ColocarInteracao(z.transform, "Pista_1_Cafe_Morno", new Vector3(404.2f, -50.2f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaCafeCasaAvo, "");

        // 2. SALA CENTRAL: Mesa de Madeira + Poltrona + TV + PISTA 2 (Cartas de paciência interrompidas)
        ColocarArteInterior(z.transform, "Mesa_Cartas_Avo", new Vector2(409.0f, -52.5f),
            "Interior_MesaCartas", "WoodTable", 0.78f, new Vector2(2.4f, 1.1f));
        ColocarArteInterior(z.transform, "Poltrona_Avo", new Vector2(405.8f, -52.5f),
            "Interior_PoltronaAvo", "Armchair", 0.78f, new Vector2(1.5f, 0.9f));
        ColocarProp(z.transform, "TV_Sala_Avo", new Vector3(403.2f, -53.8f, 0f), "TV_Old", 32f, 1.2f, new Vector2(1.2f, 0.9f), new Vector2(0f, 0.45f), 2);
        ColocarInteracao(z.transform, "Pista_2_Cartas_Mesa", new Vector3(409.0f, -53.3f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaCartasCasaAvo, "");

        // 3. QUARTO / ESCRITÓRIO (Lado Leste): Cama + Cômoda Escura + Rádio + Telefone + PISTA 3 (Nota Fiscal de 2004 e Bilhete do Bar do Zé)
        ColocarProp(z.transform, "Cama_Avo", new Vector3(415.0f, -50.2f, 0f), "Bed", 32f, 1.25f, new Vector2(1.8f, 2.2f), new Vector2(0f, 1.0f), 2);
        ColocarProp(z.transform, "Comoda_Avo", new Vector3(412.4f, -49.4f, 0f), "Cabinet_Dark", 32f, 1.25f, new Vector2(1.6f, 0.9f), new Vector2(0f, 0.45f), 2);
        ColocarProp(z.transform, "Radio_Avo", new Vector3(412.4f, -48.6f, 0f), "Radio", 32f, 1.0f, Vector2.zero, Vector2.zero, 3);
        ColocarInteracao(z.transform, "Pista_3_NotaFiscal_2004", new Vector3(412.4f, -50.4f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaNotaFiscal2004, "");
        ColocarInteracao(z.transform, "Interacao_Telefone_Avo", new Vector3(414.6f, -53.6f, 0f),
            InteracaoCapitulo1.TipoInteracao.TelefoneCasaAvo, "");

        // 4. ENTRADA DA CASA (Perto da porta sul): PISTA 4 (Banqueta caída e marcas de barro)
        ColocarProp(z.transform, "Banqueta_Caida_Avo", new Vector3(406.6f, -56.8f, 0f), "PlasticChair_Red", 32f, 1.05f, new Vector2(0.8f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarInteracao(z.transform, "Pista_4_Desordem_Porta", new Vector3(406.6f, -56.2f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaDesordemPorta, "");
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
            if (uy >= 8 && uy <= 11 && ux == 5) return TilePiso.AsfaltoParaTerra;
            if (uy == 8) return (ux % 6 == 0) ? TilePiso.AsfaltoH_Poca : TilePiso.AsfaltoH_Trinca;
            if (uy >= 9 && uy <= 11) return TilePiso.AsfaltoH_Trinca;
            // Calçada Sul de pedra (Y = 6..7) e Calçada Norte de pedra (Y = 12..13)
            if (uy == 7) return TilePiso.CalcadaSul;
            if (uy == 6) return TilePiso.Concreto;
            if (uy == 12) return TilePiso.CalcadaNorte;
            if (uy == 13) return TilePiso.Concreto;
            if (uy == 14 && ux >= 12 && ux <= 24) return TilePiso.Concreto;
            return TilePiso.Grama;
        });

        CriarParedesPerimetro(z.transform, min, max, 1.5f);

        // Árvores na grama atrás do Bar do Zé e atrás da calçada sul
        float[] arvoresNorteZ5 = { 504.0f, 509.0f, 524.5f, 529.5f, 534.5f };
        for (int i = 0; i < arvoresNorteZ5.Length; i++)
        {
            ColocarArvore(z.transform, "Arvore_Norte_Z5_" + i, new Vector3(arvoresNorteZ5[i], 16.5f + (i % 2) * 0.9f, 0f), pequena: (i % 2 == 1));
        }
        float[] arvoresSulZ5 = { 504.5f, 511.0f, 517.5f, 524.0f, 530.5f };
        for (int i = 0; i < arvoresSulZ5.Length; i++)
        {
            ColocarArvore(z.transform, "Arvore_Sul_Z5_" + i, new Vector3(arvoresSulZ5[i], 2.3f + (i % 2) * 0.9f, 0f), pequena: (i % 2 == 0));
        }

        // Caminhonete do Clube estacionada perto do Bar do Zé (Pista!)
        ColocarProp(z.transform, "Caminhonete_Clube_Bar", new Vector3(509.5f, 11.4f, 0f), "Picape_Black", 32f, 1.15f,
            new Vector2(2.6f, 1.2f), new Vector2(0f, 0.6f), 2);
        ColocarInteracao(z.transform, "Interacao_Caminhonete_Clube", new Vector3(509.5f, 10.6f, 0f),
            InteracaoCapitulo1.TipoInteracao.CaminhoneteClubeBar, "");

        // FACHADA DO BAR DO ZÉ
        ColocarProp(z.transform, "Predio_Fachada_BarDoZe", new Vector3(518.0f, 14.0f, 0f), "Fachada_BarDoZe", 16f, 0.94f,
            new Vector2(6.0f, 2.2f), new Vector2(0f, 1.5f), 2);
        AdicionarLuzPonto(z.transform, new Vector3(518.0f, 14.8f, 0f), new Color(1.0f, 0.72f, 0.38f, 1f), 7.8f, 1.45f);

        // Mesas de bar, cadeiras de plástico e engradados na calçada do Bar do Zé
        ColocarProp(z.transform, "Mesa_Calcada_Bar", new Vector3(513.5f, 13.4f, 0f), "RoundTable_Wood", 32f, 1.1f, new Vector2(1.0f, 0.8f), new Vector2(0f, 0.4f), 2);
        ColocarProp(z.transform, "Cadeira_Calcada_1", new Vector3(512.4f, 13.4f, 0f), "PlasticChair_Red", 32f, 1.05f, new Vector2(0.6f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Cadeira_Calcada_2", new Vector3(514.6f, 13.4f, 0f), "PlasticChair_Red", 32f, 1.05f, new Vector2(0.6f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Engradados_Bar_Ext", new Vector3(521.8f, 13.5f, 0f), "BeerCrate_Blue", 32f, 1.1f, new Vector2(0.8f, 0.6f), new Vector2(0f, 0.3f), 2);
        ColocarProp(z.transform, "Dumpster_Bar_Ext", new Vector3(523.2f, 13.8f, 0f), "Dumpster", 32f, 1.1f, new Vector2(1.4f, 0.9f), new Vector2(0f, 0.45f), 2);

        ColocarInteracao(z.transform, "Placa_Letreiro_BarZe", new Vector3(515.8f, 13.1f, 0f),
            InteracaoCapitulo1.TipoInteracao.PlacaBarDoZe, "");

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
        ColocarOrelhaoSave(z.transform, "Orelhao_BarDoZe", "orelhao_bar_do_ze", new Vector3(525.8f, 13.2f, 0f));

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
        ColocarPosteComLuz(z.transform, "Poste_Z5_1", new Vector3(505.5f, 13.0f, 0f), luzAmbar);
        ColocarPosteComLuz(z.transform, "Poste_Z5_2", new Vector3(527.5f, 13.0f, 0f), luzAmbar);

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

        // Porta de saída no sul do Bar
        SantaRitaExitThreshold.Create(z.transform,"SoleiraSaida_BarDoZe",new Vector2(509,-59.2f),"SaidaSociedade",2.3f,-10);
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
            InteracaoCapitulo1.TipoInteracao.BalcaoBarDoZe, "");

        // Quadro de Regras da Sinuca na parede
        ColocarProp(z.transform, "Quadro_Regras_Sprite", new Vector3(513.5f, -48.6f, 0f), "Quadro_Regras_Sinuca", 32f, 1.15f, Vector2.zero, Vector2.zero, 3);
        ColocarInteracao(z.transform, "Quadro_Regras_Interacao", new Vector3(513.5f, -49.8f, 0f),
            InteracaoCapitulo1.TipoInteracao.QuadroRegrasSinuca, "");

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
            InteracaoCapitulo1.TipoInteracao.NpcFrequentadorBar, "", "Npc_Frequentador", 32f, 1.0f, 2, solido: true);
    }

    // =========================================================================
    // SUPORTE ÀS CENAS SEPARADAS (Caso abertas diretamente no Editor)
    // =========================================================================

    private void GarantirElementosCasaDoAvoCenaSeparada()
    {
        AjustarLuzGlobal(true);
        ColocarInteracao(transform, "Pista_Cafe_Hoje", new Vector3(-2.6f, 1.3f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaCafeCasaAvo, "");
        ColocarInteracao(transform, "Pista_Cartas_Mesa", new Vector3(-0.65f, -4.15f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaCartasCasaAvo, "");
        ColocarInteracao(transform, "Pista_NotaFiscal_2004", new Vector3(2.8f, -0.2f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaNotaFiscal2004, "");
        ColocarInteracao(transform, "Pista_Desordem_Porta", new Vector3(-1.8f, -8.5f, 0f),
            InteracaoCapitulo1.TipoInteracao.PistaDesordemPorta, "");
    }

    private void GarantirElementosBarDoZeCenaSeparada()
    {
        AjustarLuzGlobal(true);
        GameState.Marcar(Flags.EntrouNoBarDoZe);

        ColocarInteracao(transform, "NPC_Ze", new Vector3(2.0f, -4.5f, 0f),
            InteracaoCapitulo1.TipoInteracao.NpcZeBar, "", "Npc_Ze", 32f, 1.0f, 2, solido: true);
        ColocarInteracao(transform, "NPC_Frequentador_Bar", new Vector3(-2.6f, -2.4f, 0f),
            InteracaoCapitulo1.TipoInteracao.NpcFrequentadorBar, "", "Npc_Frequentador", 32f, 1.0f, 2, solido: true);
        ColocarInteracao(transform, "Mesa_Sinuca_Trigger", new Vector3(0f, -4.8f, 0f),
            InteracaoCapitulo1.TipoInteracao.MesaSinucaBar, "");
        ColocarInteracao(transform, "Balcao_Bar_Trigger", new Vector3(-4.2f, -1.2f, 0f),
            InteracaoCapitulo1.TipoInteracao.BalcaoBarDoZe, "");
        ColocarInteracao(transform, "Quadro_Regras_Trigger", new Vector3(4.2f, 0.5f, 0f),
            InteracaoCapitulo1.TipoInteracao.QuadroRegrasSinuca, "");
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

    // Avança para o Centro (capítulo 2). Também é usado pelas verificações do capítulo 2.
    public static void MostrarTelaFechamentoCapitulo1()
    {
        if (!GameState.Tem(Flags.ZeDerrotadoNaSinuca)) return;
        SceneTransition.Ir("CentroSantaRita", "c1_portao_norte");
    }

    public static bool PainelFechamentoAberto =>
        instancia != null && instancia.painelFechamentoCap1 != null && instancia.painelFechamentoCap1.activeSelf;

    // Tela de fim do capítulo 1, na primeira vez que Marcus chega ao portão
    // aberto. O jogador escolhe seguir para o Centro ou ficar (e salvar antes).
    public static void MostrarPainelFimCapitulo1()
    {
        if (!GameState.Tem(Flags.ZeDerrotadoNaSinuca)) return;
        GameState.Marcar(Flags.Capitulo1Concluido);
        if (instancia == null)
        {
            MostrarTelaFechamentoCapitulo1();
            return;
        }
        instancia.AbrirPainelFechamento();
    }

    private void AbrirPainelFechamento()
    {
        if (painelFechamentoCap1 == null)
            ConstruirPainelFechamento();

        textoAvisoSaveFechamento.text = GameState.TemProgressoNaoSalvo
            ? "<color=#E8B068>O que você descobriu desde o último orelhão ainda não foi salvo. O orelhão fica ao lado do bar.</color>"
            : "<color=#98E898>Progresso salvo no orelhão.</color>";
        painelFechamentoCap1.SetActive(true);
        painelFechamentoAbertoEm = Time.unscaledTime;
    }

    private void FecharPainelFechamento(bool seguirParaCentro)
    {
        if (painelFechamentoCap1 == null || !painelFechamentoCap1.activeSelf) return;
        painelFechamentoCap1.SetActive(false);
        if (seguirParaCentro)
        {
            MostrarTelaFechamentoCapitulo1();
        }
        else
        {
            Legenda.Dizer("O portão fica aberto. O orelhão está ao lado do bar.", 4f);
        }
    }

    private void AtualizarTeclasPainelFechamento()
    {
        if (!PainelFechamentoAberto || Time.unscaledTime - painelFechamentoAbertoEm < 0.4f) return;
        var teclado = UnityEngine.InputSystem.Keyboard.current;
        if (teclado == null) return;
        if (teclado.digit1Key.wasPressedThisFrame || teclado.numpad1Key.wasPressedThisFrame)
            FecharPainelFechamento(true);
        else if (teclado.digit2Key.wasPressedThisFrame || teclado.numpad2Key.wasPressedThisFrame)
            FecharPainelFechamento(false);
    }

    private void ConstruirPainelFechamento()
    {
        Canvas canvas = UIFactory.CreateCanvas(transform, "FechamentoCapitulo1Canvas", 9600);
        canvas.gameObject.AddComponent<GraphicRaycaster>();
        painelFechamentoCap1 = canvas.gameObject;

        Image fundo = new GameObject("FundoEscuro", typeof(RectTransform)).AddComponent<Image>();
        fundo.transform.SetParent(canvas.transform, false);
        fundo.color = new Color(0.02f, 0.02f, 0.03f, 0.94f);
        UIFactory.Stretch(fundo.rectTransform);

        Image moldura = UIFactory.CreateBox(canvas.transform, "MolduraCapitulo1", "CaixaDialogo");
        RectTransform rt = moldura.rectTransform;
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(560f, 320f);
        rt.anchoredPosition = Vector2.zero;

        TextMeshProUGUI subtitulo = CriarTextoFechamento(rt, "Subtitulo", 14f, new Vector2(0f, 128f), new Vector2(500f, 18f));
        subtitulo.text = "SANTA RITA — A NOITE DAS 03:17";
        subtitulo.color = new Color32(170, 72, 64, 255);
        subtitulo.alignment = TextAlignmentOptions.Center;

        TextMeshProUGUI titulo = CriarTextoFechamento(rt, "Titulo", 24f, new Vector2(0f, 104f), new Vector2(520f, 28f));
        titulo.text = "FIM DO CAPÍTULO 1 — A MESA DO ZÉ";
        titulo.color = new Color(0.97f, 0.87f, 0.62f, 1f);
        titulo.alignment = TextAlignmentOptions.Center;

        TextMeshProUGUI corpo = CriarTextoFechamento(rt, "Resumo", 15f, new Vector2(0f, 6f), new Vector2(500f, 150f));
        corpo.text =
            "• Zé viu dois homens levarem o vô pelo portão do Centro. \"Ele não queria ir.\"\n" +
            "• Um deles é o homem de capa, que andou perguntando pela diretoria do Clube de 1990.\n" +
            "• Zé admitiu que sabia do piso do salão naquela noite, e mandou a música continuar.\n" +
            "• Você guardou a ficha do salão.";
        corpo.color = new Color(0.92f, 0.88f, 0.80f, 1f);
        corpo.alignment = TextAlignmentOptions.TopLeft;
        corpo.textWrappingMode = TextWrappingModes.Normal;
        corpo.lineSpacing = 6f;

        textoAvisoSaveFechamento = CriarTextoFechamento(rt, "AvisoSave", 13f, new Vector2(0f, -86f), new Vector2(500f, 32f));
        textoAvisoSaveFechamento.alignment = TextAlignmentOptions.Center;
        textoAvisoSaveFechamento.textWrappingMode = TextWrappingModes.Normal;

        CriarBotaoFechamento(rt, "[1] SEGUIR PARA O CENTRO", new Vector2(-122f, -126f), () => FecharPainelFechamento(true));
        CriarBotaoFechamento(rt, "[2] FICAR NA RUA NORTE", new Vector2(122f, -126f), () => FecharPainelFechamento(false));

        painelFechamentoCap1.SetActive(false);
    }

    private static TextMeshProUGUI CriarTextoFechamento(RectTransform pai, string nome, float tamanho, Vector2 pos, Vector2 tam)
    {
        TextMeshProUGUI t = UIFactory.CreateLabel(pai, nome, tamanho);
        RectTransform r = t.rectTransform;
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = tam;
        r.anchoredPosition = pos;
        return t;
    }

    private static void CriarBotaoFechamento(RectTransform pai, string texto, Vector2 pos, UnityEngine.Events.UnityAction acao)
    {
        GameObject go = new GameObject(texto, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(pai, false);
        RectTransform r = (RectTransform)go.transform;
        r.anchorMin = r.anchorMax = r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(224f, 32f);
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
