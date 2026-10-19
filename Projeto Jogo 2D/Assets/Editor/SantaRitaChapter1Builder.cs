#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SantaRitaChapter1Builder
{
    private const string SpritesDir = "Assets/Sprites/Capitulo1";
    private const string Cap1ResourcesDir = "Assets/Resources/Cap1";
    private const string PortraitsDir = "Assets/Resources/UI/Portraits";
    private const string BarDoZeScenePath = "Assets/Scenes/Cidade/BarDoZe.unity";
    private const string EntradaScenePath = "Assets/Scenes/Cidade/EntradaDaCidade.unity";
    private const string CasaDoAvoScenePath = "Assets/Scenes/Cidade/CasaDoAvo.unity";
    private const string PrologoScenePath = "Assets/Scenes/Prologo.unity";
    private const string TriggerVersion = "v3_cap1_r1";

    static SantaRitaChapter1Builder()
    {
        TryRunAutoSetup();
        EditorApplication.delayCall += TryRunAutoSetup;
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.delayCall += TryRunAutoSetup;
            }
        };
    }

    private static void TryRunAutoSetup()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            return;

        string resultPath = Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/cap1_builder_result.txt"));
        if (!File.Exists(resultPath) || !File.ReadAllText(resultPath).Contains($"SUCCESS {TriggerVersion}"))
        {
            try
            {
                ExecuteSetupAndTests(buildPlayer: false);
                File.WriteAllText(resultPath, $"SUCCESS {TriggerVersion} at {DateTime.Now:O}\nScenes updated, sprites configured, EditorBuildSettings updated, and all R1 automated tests passed.");
            }
            catch (Exception ex)
            {
                File.WriteAllText(resultPath, $"ERROR {TriggerVersion} at {DateTime.Now:O}\n{ex}");
                Debug.LogError($"[SANTA RITA] Erro no auto-setup: {ex}");
            }
        }
    }

    [MenuItem("Santa Rita/Build & Validate Capítulo 1 Vertical Slice")]
    public static void BuildAndValidateAll()
    {
        ExecuteSetupAndTests(buildPlayer: true);
    }

    public static void ExecuteSetupAndTests(bool buildPlayer)
    {
        Debug.Log("=== [SANTA RITA] Iniciando Build & Validação do Capítulo 1 (Vertical Slice R1 v3) ===");

        string cenaOriginal = SceneManager.GetActiveScene().path;

        EnsureDirectories();
        GenerateInteriorAndPropSprites();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        ConfigureSpriteImporters();

        UpdatePrologoScene();
        CreateOrUpdateBarDoZeScene();
        UpdateCasaDoAvoScene();
        UpdateEntradaDaCidadeScene();
        UpdateBuildSettings();

        if (!string.IsNullOrEmpty(cenaOriginal) && File.Exists(cenaOriginal))
        {
            EditorSceneManager.OpenScene(cenaOriginal, OpenSceneMode.Single);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RunAutomatedVerification();
        if (buildPlayer)
        {
            BuildWindows64Player();
        }

        Debug.Log("=== [SANTA RITA] Build & Validação v3 Concluídos com Sucesso! ===");
    }

    private static void EnsureDirectories()
    {
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Sprites/Capitulo1"));
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources/Cap1"));
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "Resources/UI/Portraits"));
    }

    private static void GenerateInteriorAndPropSprites()
    {
        // Preserva os sprites de alta qualidade gerados a partir de Heroi.png e Venda.png/CasaAvo.png!
        // Gera apenas os props de interior da sinuca, pistas e portão:
        SavePoolTablePng(Path.Combine(SpritesDir, "Mesa_Sinuca_TopDown.png"));
        SaveBarCounterPng(Path.Combine(SpritesDir, "Balcao_Bar.png"));
        SaveRulesBoardPng(Path.Combine(SpritesDir, "Quadro_Regras_Sinuca.png"));
        SaveIronGatePng(Path.Combine(SpritesDir, "Portao_Centro.png"));
        SaveIronGatePng(Path.Combine(Cap1ResourcesDir, "Portao_Centro.png"));

        SaveCardsCluePng(Path.Combine(SpritesDir, "Cartas_Mesa.png"));
        SaveReceiptCluePng(Path.Combine(SpritesDir, "Nota_Fiscal_2004.png"));
        SaveOverturnedStoolPng(Path.Combine(SpritesDir, "Banco_Caido.png"));
    }

    private static void ConfigureSpriteImporters()
    {
        string[] dirs = { SpritesDir, Cap1ResourcesDir, PortraitsDir };
        foreach (string dir in dirs)
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), dir)))
                continue;

            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { dir });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti == null) continue;

                string fileName = Path.GetFileNameWithoutExtension(path);
                ti.textureType = TextureImporterType.Sprite;
                ti.spriteImportMode = SpriteImportMode.Single;
                ti.textureCompression = TextureImporterCompression.Uncompressed;

                if (fileName == "MapaSantaRitaArte")
                {
                    ti.filterMode = FilterMode.Bilinear;
                    ti.maxTextureSize = 2048;
                    ti.spritePixelsPerUnit = 100f;
                }
                else if (fileName.StartsWith("Npc_"))
                {
                    ti.filterMode = FilterMode.Point;
                    ti.spritePixelsPerUnit = 32f;
                    TextureImporterSettings tis = new TextureImporterSettings();
                    ti.ReadTextureSettings(tis);
                    tis.spriteAlignment = (int)SpriteAlignment.Custom;
                    tis.spritePivot = new Vector2(0.5f, 0.125f);
                    ti.SetTextureSettings(tis);
                }
                else if (fileName == "CasaAvo" || fileName == "Venda" || fileName.StartsWith("CasaSimples_") ||
                         fileName == "CercaMadeira" || fileName == "Fachada_BarDoZe" || fileName == "Posto_Cobertura")
                {
                    ti.filterMode = FilterMode.Point;
                    ti.spritePixelsPerUnit = 16f;
                    TextureImporterSettings tis = new TextureImporterSettings();
                    ti.ReadTextureSettings(tis);
                    tis.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                    tis.spritePivot = new Vector2(0.5f, 0f);
                    ti.SetTextureSettings(tis);
                }
                else
                {
                    ti.filterMode = FilterMode.Point;
                    ti.spritePixelsPerUnit = 32f;
                }

                ti.SaveAndReimport();
            }
        }
    }

    private static void UpdatePrologoScene()
    {
        Scene scene = EditorSceneManager.OpenScene(PrologoScenePath, OpenSceneMode.Single);
        bool dirty = false;

        PrologoDirector director = UnityEngine.Object.FindAnyObjectByType<PrologoDirector>();
        if (director != null)
        {
            SerializedObject so = new SerializedObject(director);
            SerializedProperty falasProp = so.FindProperty("falas");
            if (falasProp != null && falasProp.isArray && falasProp.arraySize >= 2)
            {
                falasProp.GetArrayElementAtIndex(0).FindPropertyRelative("texto").stringValue = "\"Desde cedo sem atender.\"";
                falasProp.GetArrayElementAtIndex(1).FindPropertyRelative("texto").stringValue = "\"O vô Adílio nunca deixa o telefone tocar até cair.\"";
                if (falasProp.arraySize >= 3)
                    falasProp.GetArrayElementAtIndex(2).FindPropertyRelative("texto").stringValue = "\"Talvez eu esteja exagerando... mas não dava pra esperar até amanhã.\"";
                if (falasProp.arraySize >= 4)
                    falasProp.GetArrayElementAtIndex(3).FindPropertyRelative("texto").stringValue = "\"Seis anos sem pisar em Santa Rita.\"";
            }
            SerializedProperty recProp = so.FindProperty("falaAoReconhecer");
            if (recProp != null) recProp.stringValue = "\"Não mudou tanto desde noventa e oito...\"";
            SerializedProperty finProp = so.FindProperty("falaFinal");
            if (finProp != null) finProp.stringValue = "\"Certo... vamos ver por que o vô não atendeu hoje.\"";
            so.ApplyModifiedPropertiesWithoutUndo();
            dirty = true;
        }

        PrologoVideo video = UnityEngine.Object.FindAnyObjectByType<PrologoVideo>(FindObjectsInactive.Include);
        if (video != null)
        {
            SerializedObject so = new SerializedObject(video);
            SerializedProperty falasProp = so.FindProperty("falas");
            if (falasProp != null && falasProp.isArray && falasProp.arraySize >= 2)
            {
                falasProp.GetArrayElementAtIndex(0).FindPropertyRelative("texto").stringValue = "\"Desde cedo sem atender.\"";
                falasProp.GetArrayElementAtIndex(1).FindPropertyRelative("texto").stringValue = "\"O vô Adílio nunca deixa o telefone tocar até cair.\"";
                if (falasProp.arraySize >= 3)
                    falasProp.GetArrayElementAtIndex(2).FindPropertyRelative("texto").stringValue = "\"Seis anos sem voltar pra Santa Rita.\"";
                if (falasProp.arraySize >= 4)
                    falasProp.GetArrayElementAtIndex(3).FindPropertyRelative("texto").stringValue = "\"Certo... vamos ver por que ele não atendeu hoje.\"";
            }
            so.ApplyModifiedPropertiesWithoutUndo();
            dirty = true;
        }

        if (dirty)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }

    private static void CreateOrUpdateBarDoZeScene()
    {
        if (File.Exists(BarDoZeScenePath))
        {
            AssetDatabase.DeleteAsset(BarDoZeScenePath);
        }
        AssetDatabase.CopyAsset(CasaDoAvoScenePath, BarDoZeScenePath);
        AssetDatabase.Refresh();

        Scene scene = EditorSceneManager.OpenScene(BarDoZeScenePath, OpenSceneMode.Single);

        DentroDaCasaDoAvo dentroAvo = UnityEngine.Object.FindAnyObjectByType<DentroDaCasaDoAvo>();
        if (dentroAvo != null)
        {
            UnityEngine.Object.DestroyImmediate(dentroAvo.gameObject);
        }

        DoorTrigger porta = UnityEngine.Object.FindAnyObjectByType<DoorTrigger>();
        if (porta != null)
        {
            SerializedObject soPorta = new SerializedObject(porta);
            soPorta.FindProperty("cenaDestino").stringValue = "EntradaDaCidade";
            soPorta.FindProperty("entrada").stringValue = "saida_bar_ze";
            soPorta.FindProperty("textoDoPrompt").stringValue = "Sair para a Rua Norte";
            soPorta.ApplyModifiedPropertiesWithoutUndo();
        }

        SpawnPoint spawn = UnityEngine.Object.FindAnyObjectByType<SpawnPoint>();
        if (spawn != null)
        {
            SerializedObject soSpawn = new SerializedObject(spawn);
            soSpawn.FindProperty("id").stringValue = "entrada_bar";
            soSpawn.FindProperty("direcaoInicial").intValue = 1; // DirUp
            soSpawn.ApplyModifiedPropertiesWithoutUndo();
        }

        string[] moveisCasa = { "Cama", "Guarda_Roupa", "Xicara", "Bilhete", "Telefone", "Cruz", "Retrato", "Poltrona", "Mesa",
                                "Pista_Cartas_Mesa", "Pista_NotaFiscal_2004", "Pista_Desordem_Porta" };
        foreach (string nomeMovel in moveisCasa)
        {
            GameObject go = GameObject.Find(nomeMovel);
            if (go != null)
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
        }

        GameObject chao = GameObject.Find("Chao");
        if (chao != null)
        {
            SpriteRenderer srChao = chao.GetComponent<SpriteRenderer>();
            if (srChao != null)
            {
                srChao.color = new Color(0.72f, 0.56f, 0.42f, 1f);
            }
        }

        Transform moveisParent = GameObject.Find("Moveis")?.transform;

        // Mesa de Sinuca no centro do salão (0, -4.8)
        Sprite mesaSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(SpritesDir, "Mesa_Sinuca_TopDown.png"));
        GameObject mesaGo = CreateSceneSpriteObject("Mesa_Sinuca_Trigger", new Vector3(0f, -4.8f, 0f), mesaSprite, 4, moveisParent);
        BoxCollider2D mesaCol = mesaGo.AddComponent<BoxCollider2D>();
        mesaCol.size = new Vector2(2.6f, 1.3f);
        mesaCol.offset = new Vector2(0f, 0.7f);
        InteracaoCapitulo1 intMesa = mesaGo.AddComponent<InteracaoCapitulo1>();
        intMesa.Configurar(InteracaoCapitulo1.TipoInteracao.MesaSinucaBar, "");

        // NPC Zé ao lado da mesa de sinuca (2.0, -4.5)
        Sprite zeSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(SpritesDir, "Npc_Ze.png"));
        GameObject zeGo = CreateSceneSpriteObject("NPC_Ze", new Vector3(2.0f, -4.5f, 0f), zeSprite, 6, moveisParent);
        BoxCollider2D zeCol = zeGo.AddComponent<BoxCollider2D>();
        zeCol.size = new Vector2(0.8f, 0.5f);
        zeCol.offset = new Vector2(0f, 0.25f);
        InteracaoCapitulo1 intZe = zeGo.AddComponent<InteracaoCapitulo1>();
        intZe.Configurar(InteracaoCapitulo1.TipoInteracao.NpcZeBar, "");

        // Frequentador do Bar (-2.6, -2.4)
        Sprite freqSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(SpritesDir, "Npc_Frequentador.png"));
        GameObject freqGo = CreateSceneSpriteObject("NPC_Frequentador_Bar", new Vector3(-2.6f, -2.4f, 0f), freqSprite, 6, moveisParent);
        BoxCollider2D freqCol = freqGo.AddComponent<BoxCollider2D>();
        freqCol.size = new Vector2(0.8f, 0.5f);
        freqCol.offset = new Vector2(0f, 0.25f);
        freqGo.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.NpcFrequentadorBar, "Conversar com o Frequentador");

        // Balcão do Bar (-4.2, -1.2)
        Sprite balcaoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(SpritesDir, "Balcao_Bar.png"));
        GameObject balcaoGo = CreateSceneSpriteObject("Balcao_Bar_Trigger", new Vector3(-4.2f, -1.2f, 0f), balcaoSprite, 3, moveisParent);
        BoxCollider2D balcaoCol = balcaoGo.AddComponent<BoxCollider2D>();
        balcaoCol.size = new Vector2(2.8f, 0.8f);
        balcaoCol.offset = new Vector2(0f, 0.4f);
        InteracaoCapitulo1 intBalcao = balcaoGo.AddComponent<InteracaoCapitulo1>();
        intBalcao.Configurar(InteracaoCapitulo1.TipoInteracao.BalcaoBarDoZe, "Examinar balcão do bar");

        // Quadro de Regras na parede (4.2, 0.5)
        Sprite quadroSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(SpritesDir, "Quadro_Regras_Sinuca.png"));
        GameObject quadroGo = CreateSceneSpriteObject("Quadro_Regras_Trigger", new Vector3(4.2f, 0.5f, 0f), quadroSprite, 3, moveisParent);
        InteracaoCapitulo1 intQuadro = quadroGo.AddComponent<InteracaoCapitulo1>();
        intQuadro.Configurar(InteracaoCapitulo1.TipoInteracao.QuadroRegrasSinuca, "Ler regras da mesa");

        if (UnityEngine.Object.FindAnyObjectByType<Capitulo1Director>() == null)
        {
            new GameObject("Capitulo1Director").AddComponent<Capitulo1Director>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void UpdateCasaDoAvoScene()
    {
        Scene scene = EditorSceneManager.OpenScene(CasaDoAvoScenePath, OpenSceneMode.Single);
        Transform moveisParent = GameObject.Find("Moveis")?.transform;

        GameObject bilheteGo = GameObject.Find("Bilhete");
        if (bilheteGo != null)
        {
            ItemDeCena itemBilhete = bilheteGo.GetComponent<ItemDeCena>();
            if (itemBilhete != null)
            {
                SerializedObject so = new SerializedObject(itemBilhete);
                SerializedProperty falas = so.FindProperty("falas");
                falas.ClearArray();
                falas.InsertArrayElementAtIndex(0);
                falas.GetArrayElementAtIndex(0).stringValue = "Um recado rabiscado com a letra firme do vô Adílio: <i>\"Rua Norte — Bar do Zé. Não aceitar recado do Clube.\"</i>";
                falas.InsertArrayElementAtIndex(1);
                falas.GetArrayElementAtIndex(1).stringValue = "A tinta da caneta é de hoje. Ele saiu às pressas, ou alguém veio cobrar essa conversa.";
                so.FindProperty("flagAoExaminar").stringValue = Flags.CasaAvo_LeuBilhete;
                so.FindProperty("flagAoTerminar").stringValue = Flags.Pista_RumoBarDoZe;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        GameObject xicaraGo = GameObject.Find("Xicara");
        if (xicaraGo != null)
        {
            ItemDeCena itemXicara = xicaraGo.GetComponent<ItemDeCena>();
            if (itemXicara != null)
            {
                UnityEngine.Object.DestroyImmediate(itemXicara);
            }
            InteracaoCapitulo1 intCafe = xicaraGo.GetComponent<InteracaoCapitulo1>();
            if (intCafe == null) intCafe = xicaraGo.AddComponent<InteracaoCapitulo1>();
            intCafe.Configurar(InteracaoCapitulo1.TipoInteracao.PistaCafeCasaAvo, "Examinar xícara de café");
        }

        RemoveIfExists("Pista_Cartas_Mesa");
        Sprite cartasSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(SpritesDir, "Cartas_Mesa.png"));
        GameObject cartasGo = CreateSceneSpriteObject("Pista_Cartas_Mesa", new Vector3(-0.65f, -4.15f, 0f), cartasSprite, 2, moveisParent);
        cartasGo.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.PistaCartasCasaAvo, "Examinar cartas na mesa");

        RemoveIfExists("Pista_NotaFiscal_2004");
        Sprite notaSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(SpritesDir, "Nota_Fiscal_2004.png"));
        GameObject notaGo = CreateSceneSpriteObject("Pista_NotaFiscal_2004", new Vector3(2.8f, -0.2f, 0f), notaSprite, 2, moveisParent);
        notaGo.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.PistaNotaFiscal2004, "Ler nota fiscal (2004)");

        RemoveIfExists("Pista_Desordem_Porta");
        Sprite bancoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(SpritesDir, "Banco_Caido.png"));
        GameObject bancoGo = CreateSceneSpriteObject("Pista_Desordem_Porta", new Vector3(-1.8f, -8.5f, 0f), bancoSprite, 2, moveisParent);
        bancoGo.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.PistaDesordemPorta, "Examinar banco caído");

        if (UnityEngine.Object.FindAnyObjectByType<Capitulo1Director>() == null)
        {
            new GameObject("Capitulo1Director").AddComponent<Capitulo1Director>();
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void UpdateEntradaDaCidadeScene()
    {
        Scene scene = EditorSceneManager.OpenScene(EntradaScenePath, OpenSceneMode.Single);

        // 0. Garante que o Tilemap Ruas esteja ativo na cena!
        GameObject grid = GameObject.Find("Grid");
        if (grid != null)
        {
            Transform ruasTr = grid.transform.Find("Ruas");
            if (ruasTr != null)
            {
                ruasTr.gameObject.SetActive(true);
            }
        }

        PayphoneCheckpoint[] payphones = UnityEngine.Object.FindObjectsByType<PayphoneCheckpoint>(FindObjectsSortMode.None);
        foreach (PayphoneCheckpoint pp in payphones)
        {
            SerializedObject soPp = new SerializedObject(pp);
            SerializedProperty idProp = soPp.FindProperty("payphoneId");
            if (idProp != null && string.IsNullOrEmpty(idProp.stringValue))
            {
                idProp.stringValue = "orelhao_encruzilhada";
            }
            SerializedProperty linesProp = soPp.FindProperty("falas");
            if (linesProp != null && linesProp.isArray)
            {
                linesProp.ClearArray();
                string[] novasLinhas =
                {
                    "O gancho está gelado. A linha chia com a tempestade lá fora.",
                    "Discar para fora da cidade só devolve sinal de ocupado: o tronco da rodovia caiu com a chuva.",
                    "Anotado no caderno. Se eu perder o rumo esta noite, retomo daqui."
                };
                for (int i = 0; i < novasLinhas.Length; i++)
                {
                    linesProp.InsertArrayElementAtIndex(i);
                    linesProp.GetArrayElementAtIndex(i).stringValue = novasLinhas[i];
                }
            }
            soPp.ApplyModifiedPropertiesWithoutUndo();
        }

        // Remove objetos antigos para recriar com posicionamento e sprites perfeitos
        string[] nomesCap1 =
        {
            "Posto_Cobertura", "Posto_Frentista", "Carro_Marcus_Interacao",
            "Predio_Venda_Mercadinho", "Mercadinho_Atendente",
            "Predio_CasaDoAvo", "Cerca_CasaAvo_Esq", "Cerca_CasaAvo_Dir",
            "Bairro_Casa_A1", "Bairro_Cerca_1", "Bairro_Casa_B1", "Bairro_Casa_C1", "Bairro_Morador_Ramiro",
            "Fachada_BarDoZe", "Placa_BarDoZe", "Porta_BarDoZe",
            "Spawn_SaidaBarDoZe", "Payphone_BarDoZe", "Portao_Centro",
            "Barreira_Limiar_Centro", "Limiar_Centro_Trigger", "Capitulo1Director"
        };
        foreach (string n in nomesCap1) RemoveIfExists(n);

        // 1. Carro de Marcus perto de CarroDoProtagonista (15.5, 15.1)
        GameObject carroInt = new GameObject("Carro_Marcus_Interacao");
        carroInt.transform.position = new Vector3(15.5f, 15.1f, 0f);
        carroInt.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.CarroMarcus, "Examinar meu carro");

        // 2. Posto de Gasolina na chegada (22.2, 16.0) e Frentista (21.2, 14.8)
        Sprite postoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(Cap1ResourcesDir, "Posto_Cobertura.png"));
        GameObject postoGo = CreateBuildingObject("Posto_Cobertura", new Vector3(22.2f, 16.0f, 0f), postoSprite, 0.82f, new Vector2(5.8f, 1.8f), new Vector2(0f, 1.2f));

        Sprite frentistaSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(Cap1ResourcesDir, "Npc_Frentista.png"));
        GameObject frentistaGo = CreateNpcObject("Posto_Frentista", new Vector3(21.2f, 14.8f, 0f), frentistaSprite);
        frentistaGo.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.NpcFrentista, "Conversar com o Frentista");

        // 3. Mercadinho / Venda na Avenida Sul (40.5, 16.0) e Atendente (40.5, 14.8)
        Sprite vendaSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(Cap1ResourcesDir, "Venda.png"));
        CreateBuildingObject("Predio_Venda_Mercadinho", new Vector3(40.5f, 16.0f, 0f), vendaSprite, 0.82f, new Vector2(6.0f, 2.0f), new Vector2(0f, 1.3f));

        Sprite atendenteSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(Cap1ResourcesDir, "Npc_Atendente.png"));
        GameObject atendenteGo = CreateNpcObject("Mercadinho_Atendente", new Vector3(40.5f, 14.8f, 0f), atendenteSprite);
        atendenteGo.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.NpcAtendenteMercadinho, "Conversar com a Atendente");

        // 4. CASA DO AVÔ ADÍLIO (Nº 40) exatamente onde estão PortaCasaAvo (28.5, 38.6) e CaixaCorreio (32.8, 38.2)!
        Sprite casaAvoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(Cap1ResourcesDir, "CasaAvo.png"));
        CreateBuildingObject("Predio_CasaDoAvo", new Vector3(28.5f, 38.3f, 0f), casaAvoSprite, 0.82f, new Vector2(6.2f, 2.2f), new Vector2(0f, 1.6f));

        Sprite cercaSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(Cap1ResourcesDir, "CercaMadeira.png"));
        CreateBuildingObject("Cerca_CasaAvo_Esq", new Vector3(23.2f, 38.3f, 0f), cercaSprite, 0.9f, new Vector2(3.6f, 0.6f), new Vector2(0f, 0.4f));
        CreateBuildingObject("Cerca_CasaAvo_Dir", new Vector3(34.8f, 38.3f, 0f), cercaSprite, 0.9f, new Vector2(3.6f, 0.6f), new Vector2(0f, 0.4f));

        // 5. Casas vizinhas do Bairro das Casas e Morador (Seu Ramiro)
        Sprite casaASprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(Cap1ResourcesDir, "CasaSimples_A.png"));
        Sprite casaBSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(Cap1ResourcesDir, "CasaSimples_B.png"));
        Sprite casaCSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(Cap1ResourcesDir, "CasaSimples_C.png"));
        CreateBuildingObject("Bairro_Casa_A1", new Vector3(12.0f, 36.4f, 0f), casaASprite, 0.82f, new Vector2(5.4f, 2.0f), new Vector2(0f, 1.3f));
        CreateBuildingObject("Bairro_Cerca_1", new Vector3(16.2f, 36.4f, 0f), cercaSprite, 0.85f, new Vector2(3.4f, 0.6f), new Vector2(0f, 0.4f));
        CreateBuildingObject("Bairro_Casa_B1", new Vector3(20.0f, 36.4f, 0f), casaBSprite, 0.82f, new Vector2(5.4f, 2.0f), new Vector2(0f, 1.3f));
        CreateBuildingObject("Bairro_Casa_C1", new Vector3(38.5f, 36.4f, 0f), casaCSprite, 0.82f, new Vector2(5.4f, 2.0f), new Vector2(0f, 1.3f));

        Sprite moradorSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(Cap1ResourcesDir, "Npc_Frequentador.png"));
        GameObject moradorGo = CreateNpcObject("Bairro_Morador_Ramiro", new Vector3(24.8f, 36.0f, 0f), moradorSprite);
        moradorGo.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.NpcMoradorBairro, "Conversar com Seu Ramiro");

        // 6. Exterior do Bar do Zé na Rua Norte (45.5, 36.4)
        Vector3 posBarExterior = new Vector3(45.5f, 36.4f, 0f);
        Sprite barFacadeSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(Cap1ResourcesDir, "Fachada_BarDoZe.png"));
        CreateBuildingObject("Fachada_BarDoZe", posBarExterior, barFacadeSprite, 0.82f, new Vector2(5.6f, 2.0f), new Vector2(0f, 1.4f));

        GameObject placaBar = new GameObject("Placa_BarDoZe");
        placaBar.transform.position = new Vector3(43.5f, 35.8f, 0f);
        placaBar.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.PlacaBarDoZe, "Ler letreiro do Bar do Zé");

        GameObject portaBar = new GameObject("Porta_BarDoZe");
        portaBar.transform.position = new Vector3(45.5f, 36.1f, 0f);
        BoxCollider2D trigBar = portaBar.AddComponent<BoxCollider2D>();
        trigBar.isTrigger = true;
        trigBar.size = new Vector2(1.4f, 1.0f);
        DoorTrigger dtBar = portaBar.AddComponent<DoorTrigger>();
        SerializedObject soDtBar = new SerializedObject(dtBar);
        soDtBar.FindProperty("cenaDestino").stringValue = "BarDoZe";
        soDtBar.FindProperty("entrada").stringValue = "entrada_bar";
        soDtBar.FindProperty("textoDoPrompt").stringValue = "Entrar no Bar do Zé";
        soDtBar.ApplyModifiedPropertiesWithoutUndo();

        GameObject spawnBar = new GameObject("Spawn_SaidaBarDoZe");
        spawnBar.transform.position = new Vector3(45.5f, 34.8f, 0f);
        SpawnPoint spBar = spawnBar.AddComponent<SpawnPoint>();
        SerializedObject soSpBar = new SerializedObject(spBar);
        soSpBar.FindProperty("id").stringValue = "saida_bar_ze";
        soSpBar.FindProperty("direcaoInicial").intValue = 0; // DirDown
        soSpBar.ApplyModifiedPropertiesWithoutUndo();

        if (payphones.Length > 0)
        {
            GameObject novoOrelhao = UnityEngine.Object.Instantiate(payphones[0].gameObject);
            novoOrelhao.name = "Payphone_BarDoZe";
            novoOrelhao.transform.position = new Vector3(49.5f, 36.2f, 0f);
            PayphoneCheckpoint ppBar = novoOrelhao.GetComponent<PayphoneCheckpoint>();
            if (ppBar != null)
            {
                SerializedObject soPpBar = new SerializedObject(ppBar);
                SerializedProperty idBarProp = soPpBar.FindProperty("payphoneId");
                if (idBarProp != null) idBarProp.stringValue = "orelhao_bar_do_ze";
                soPpBar.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // 7. Portão / Limiar do Centro no fim leste da Rua Norte (52.5, 34.5)
        Vector3 posLimiarCentro = new Vector3(52.5f, 34.5f, 0f);
        Sprite portaoSprite = AssetDatabase.LoadAssetAtPath<Sprite>(Path.Combine(SpritesDir, "Portao_Centro.png"));
        CreateSceneSpriteObject("Portao_Centro", posLimiarCentro, portaoSprite, 6, null);

        GameObject barreiraCentro = new GameObject("Barreira_Limiar_Centro");
        barreiraCentro.transform.position = posLimiarCentro;
        BoxCollider2D colBarreira = barreiraCentro.AddComponent<BoxCollider2D>();
        colBarreira.size = new Vector2(2.2f, 3.6f);
        colBarreira.offset = new Vector2(0f, 0.5f);

        GameObject limiarTrigger = new GameObject("Limiar_Centro_Trigger");
        limiarTrigger.transform.position = posLimiarCentro + new Vector3(-0.8f, 0f, 0f);
        limiarTrigger.AddComponent<InteracaoCapitulo1>().Configurar(InteracaoCapitulo1.TipoInteracao.LimiarCentro, "");

        // 8. Diretor do Capítulo 1
        new GameObject("Capitulo1Director").AddComponent<Capitulo1Director>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static GameObject CreateBuildingObject(string name, Vector3 pos, Sprite sprite, float scale, Vector2 colSize, Vector2 colOffset)
    {
        GameObject go = CreateSceneSpriteObject(name, pos, sprite, 0, null);
        go.transform.localScale = new Vector3(scale, scale, 1f);
        if (colSize.x > 0.01f && colSize.y > 0.01f)
        {
            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.size = colSize;
            col.offset = colOffset;
        }
        return go;
    }

    private static GameObject CreateNpcObject(string name, Vector3 pos, Sprite sprite)
    {
        GameObject go = CreateSceneSpriteObject(name, pos, sprite, 0, null);
        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.75f, 0.45f);
        col.offset = new Vector2(0f, 0.22f);
        return go;
    }

    private static void UpdateBuildSettings()
    {
        var scenes = EditorBuildSettings.scenes.ToList();
        if (!scenes.Any(s => s.path == BarDoZeScenePath))
        {
            scenes.Add(new EditorBuildSettingsScene(BarDoZeScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"[SANTA RITA] Cena '{BarDoZeScenePath}' registrada no EditorBuildSettings.");
        }
    }

    private static void RunAutomatedVerification()
    {
        Debug.Log("=== [SANTA RITA] Executando Testes Automatizados de Invariantes R1 ===");

        CheckpointSystem.Clear();
        GameState.Limpar();
        GameState.Marcar(Flags.ChegouCasaAvo);
        GameState.RegistrarNotaDiario("teste_ram", "Nota em RAM", "FATO", "Apenas na memória da sessão.");

        if (!GameState.TemProgressoNaoSalvo)
            throw new Exception("Falha no Teste 1A: GameState deveria acusar progresso não salvo em RAM.");
        if (CheckpointSystem.HasCheckpoint)
            throw new Exception("Falha no Teste 1B: CheckpointSystem não deveria ter save manual antes de usar um orelhão.");

        CheckpointSystem.Save("EntradaDaCidade", new Vector2(46.5f, 15.0f), "orelhao_encruzilhada");
        if (!CheckpointSystem.HasCheckpoint || GameState.TemProgressoNaoSalvo)
            throw new Exception("Falha no Teste 1C: Save manual no orelhão não persistiu corretamente.");

        GameState.Marcar(Flags.ZeDerrotadoNaSinuca);
        CheckpointSystem.RequestSpawnAtCheckpoint();
        if (GameState.Tem(Flags.ZeDerrotadoNaSinuca))
            throw new Exception("Falha no Teste 1D: Recarregar o save do orelhão não descartou flags marcadas após o save.");
        if (!GameState.Tem(Flags.ChegouCasaAvo))
            throw new Exception("Falha no Teste 1E: Recarregar o save do orelhão perdeu flag salva antes do orelhão.");

        CheckpointSystem.Clear();
        GameState.Limpar();

        GameObject hostSinuca = new GameObject("TesteSinucaHost");
        SinucaMinigameUI sinuca = hostSinuca.AddComponent<SinucaMinigameUI>();
        sinuca.IniciarDuelo(null, null);

        if (!sinuca.SimularTesteDeInvariantes(out string relatorioSinuca))
        {
            UnityEngine.Object.DestroyImmediate(hostSinuca);
            throw new Exception($"Falha no Teste 2 (Sinuca): {relatorioSinuca}");
        }
        Debug.Log($"[TESTE SINUCA PASS] {relatorioSinuca}");
        sinuca.Fechar();
        UnityEngine.Object.DestroyImmediate(hostSinuca);

        Debug.Log("=== [SANTA RITA] Todos os Testes Automatizados Passaram! ===");
    }

    private static void BuildWindows64Player()
    {
        string buildDir = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Build"));
        Directory.CreateDirectory(buildDir);
        string exePath = Path.Combine(buildDir, "SantaRita.exe");

        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        BuildPlayerOptions opts = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = exePath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(opts);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new Exception($"Falha ao gerar build Windows x64: {report.summary.result} ({report.summary.totalErrors} erros)");
        }

        Debug.Log($"[SANTA RITA] Build Windows x64 gerado com sucesso em: {exePath} ({report.summary.totalSize} bytes)");
    }

    private static void RemoveIfExists(string name)
    {
        GameObject go = GameObject.Find(name);
        while (go != null)
        {
            UnityEngine.Object.DestroyImmediate(go);
            go = GameObject.Find(name);
        }
    }

    private static GameObject CreateSceneSpriteObject(string name, Vector3 pos, Sprite sprite, int sortingOrder, Transform parent)
    {
        GameObject go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);
        go.transform.position = pos;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = sortingOrder;
        Material litMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Settings/Sprite-Lit-Default.mat");
        if (litMat == null)
        {
            string[] matGuids = AssetDatabase.FindAssets("Sprite-Lit-Default t:Material");
            if (matGuids.Length > 0)
                litMat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(matGuids[0]));
        }
        if (litMat != null) sr.sharedMaterial = litMat;
        return go;
    }

    private static void SavePoolTablePng(string assetPath)
    {
        Texture2D tex = new Texture2D(96, 56, TextureFormat.RGBA32, false);
        ClearTex(tex);

        Color woodDark = new Color(0.24f, 0.13f, 0.07f);
        Color woodRail = new Color(0.38f, 0.21f, 0.11f);
        Color felt = new Color(0.10f, 0.42f, 0.24f);
        Color pocket = new Color(0.04f, 0.04f, 0.05f);

        FillRect(tex, 2, 4, 92, 48, woodDark);
        FillRect(tex, 5, 7, 86, 42, woodRail);
        FillRect(tex, 10, 12, 76, 32, felt);

        int[] px = { 10, 48, 85 };
        int[] py = { 12, 43 };
        foreach (int x in px)
            foreach (int y in py)
                FillRect(tex, x - 3, y - 3, 6, 6, pocket);

        WritePng(tex, assetPath);
    }

    private static void SaveBarCounterPng(string assetPath)
    {
        Texture2D tex = new Texture2D(96, 36, TextureFormat.RGBA32, false);
        ClearTex(tex);
        FillRect(tex, 2, 0, 92, 28, new Color(0.32f, 0.18f, 0.10f));
        FillRect(tex, 0, 24, 96, 8, new Color(0.46f, 0.28f, 0.15f));
        FillRect(tex, 18, 28, 4, 7, new Color(0.22f, 0.58f, 0.28f));
        FillRect(tex, 26, 28, 4, 8, new Color(0.72f, 0.46f, 0.18f));
        FillRect(tex, 68, 28, 5, 7, new Color(0.62f, 0.20f, 0.18f));
        WritePng(tex, assetPath);
    }

    private static void SaveRulesBoardPng(string assetPath)
    {
        Texture2D tex = new Texture2D(32, 32, TextureFormat.RGBA32, false);
        ClearTex(tex);
        FillRect(tex, 2, 4, 28, 24, new Color(0.38f, 0.24f, 0.12f));
        FillRect(tex, 4, 6, 24, 20, new Color(0.10f, 0.22f, 0.14f));
        FillRect(tex, 7, 20, 18, 2, new Color(0.88f, 0.86f, 0.78f));
        FillRect(tex, 7, 15, 14, 2, new Color(0.88f, 0.32f, 0.28f));
        FillRect(tex, 7, 10, 16, 2, new Color(0.88f, 0.86f, 0.78f));
        WritePng(tex, assetPath);
    }

    private static void SaveIronGatePng(string assetPath)
    {
        Texture2D tex = new Texture2D(112, 48, TextureFormat.RGBA32, false);
        ClearTex(tex);
        Color iron = new Color(0.18f, 0.20f, 0.24f);
        Color pillar = new Color(0.28f, 0.26f, 0.25f);
        FillRect(tex, 0, 0, 14, 44, pillar);
        FillRect(tex, 98, 0, 14, 44, pillar);
        FillRect(tex, 14, 10, 84, 3, iron);
        FillRect(tex, 14, 34, 84, 3, iron);
        for (int x = 18; x < 96; x += 8)
        {
            FillRect(tex, x, 4, 3, 38, iron);
        }
        WritePng(tex, assetPath);
    }

    private static void SaveCardsCluePng(string assetPath)
    {
        Texture2D tex = new Texture2D(24, 24, TextureFormat.RGBA32, false);
        ClearTex(tex);
        FillRect(tex, 4, 6, 7, 10, new Color(0.94f, 0.91f, 0.84f));
        FillRect(tex, 12, 8, 7, 10, new Color(0.94f, 0.91f, 0.84f));
        FillRect(tex, 8, 4, 7, 10, new Color(0.76f, 0.22f, 0.20f));
        WritePng(tex, assetPath);
    }

    private static void SaveReceiptCluePng(string assetPath)
    {
        Texture2D tex = new Texture2D(24, 24, TextureFormat.RGBA32, false);
        ClearTex(tex);
        FillRect(tex, 6, 3, 12, 17, new Color(0.92f, 0.88f, 0.74f));
        FillRect(tex, 8, 15, 8, 1, new Color(0.25f, 0.22f, 0.20f));
        FillRect(tex, 8, 12, 7, 1, new Color(0.25f, 0.22f, 0.20f));
        FillRect(tex, 8, 8, 8, 2, new Color(0.68f, 0.20f, 0.18f));
        WritePng(tex, assetPath);
    }

    private static void SaveOverturnedStoolPng(string assetPath)
    {
        Texture2D tex = new Texture2D(28, 24, TextureFormat.RGBA32, false);
        ClearTex(tex);
        Color wood = new Color(0.42f, 0.25f, 0.14f);
        FillRect(tex, 4, 6, 12, 12, wood);
        FillRect(tex, 16, 7, 8, 3, wood * 0.8f);
        FillRect(tex, 16, 14, 8, 3, wood * 0.8f);
        WritePng(tex, assetPath);
    }

    private static void ClearTex(Texture2D tex)
    {
        Color c = new Color(0f, 0f, 0f, 0f);
        for (int y = 0; y < tex.height; y++)
            for (int x = 0; x < tex.width; x++)
                tex.SetPixel(x, y, c);
    }

    private static void FillRect(Texture2D tex, int x0, int y0, int w, int h, Color c)
    {
        for (int y = Mathf.Max(0, y0); y < Mathf.Min(tex.height, y0 + h); y++)
            for (int x = Mathf.Max(0, x0); x < Mathf.Min(tex.width, x0 + w); x++)
                tex.SetPixel(x, y, c);
    }

    private static void WritePng(Texture2D tex, string assetPath)
    {
        tex.Apply();
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
        File.WriteAllBytes(fullPath, tex.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(tex);
    }
}
#endif
