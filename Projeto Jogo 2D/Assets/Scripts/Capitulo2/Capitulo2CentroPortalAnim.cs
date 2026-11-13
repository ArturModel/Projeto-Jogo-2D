using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

internal static class Capitulo2CentroPortalAnim
{
    private const int RoomLayer = 27;
    private const int ExteriorLayer = 26;
    private const int Hidden3DMask = (1 << RoomLayer) | (1 << ExteriorLayer);
    private const float DuracaoEntrada2D = 1.35f;
    private const float DuracaoSaida2D = 1.12f;

    private static bool centroEmEspera;
    private static Scene cenaCentroEmEspera;
    private static readonly List<GameObject> raizesAtivasCentro = new List<GameObject>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        centroEmEspera = false;
        cenaCentroEmEspera = default;
        raizesAtivasCentro.Clear();
    }

    private readonly struct DoorDef
    {
        public readonly string Cena;
        public readonly string Fachada;
        public readonly int PxX;
        public readonly int PxYBottom;
        public readonly int PxW;
        public readonly int PxH;
        public readonly bool DoubleDoor;
        public readonly bool HingeLeft;
        public readonly float ArchTop;
        public readonly bool OpenBay;
        public readonly Color WarmColor;

        public DoorDef(string cena, string fachada, int pxX, int pxYBottom, int pxW, int pxH,
            bool doubleDoor, bool hingeLeft, float archTop, bool openBay, Color warmColor)
        {
            Cena = cena;
            Fachada = fachada;
            PxX = pxX;
            PxYBottom = pxYBottom;
            PxW = pxW;
            PxH = pxH;
            DoubleDoor = doubleDoor;
            HingeLeft = hingeLeft;
            ArchTop = archTop;
            OpenBay = openBay;
            WarmColor = warmColor;
        }
    }

    private static readonly Dictionary<string, DoorDef> Portas = new Dictionary<string, DoorDef>
    {
        ["CasaCostureira"] = new DoorDef("CasaCostureira", "CasaCostura", 94, 24, 20, 52, false, false, 0f, false, new Color(1.00f, .76f, .44f, 1f)),
        ["OficinaRadio"] = new DoorDef("OficinaRadio", "OficinaRadio", 34, 25, 24, 57, false, true, 0f, false, new Color(.95f, .78f, .48f, 1f)),
        ["PrefeituraSantaRita"] = new DoorDef("PrefeituraSantaRita", "Prefeitura", 109, 29, 38, 54, true, true, 0f, false, new Color(1.00f, .82f, .55f, 1f)),
        ["IgrejaMatriz"] = new DoorDef("IgrejaMatriz", "Igreja", 109, 24, 49, 58, true, true, .22f, false, new Color(1.00f, .70f, .34f, 1f)),
        ["MercadinhoDeposito"] = new DoorDef("MercadinhoDeposito", "Fundos_MesmoMercadinho", 112, 54, 32, 52, false, false, 0f, false, new Color(.92f, .82f, .60f, 1f)),
        ["SociedadeBeiraRio"] = new DoorDef("SociedadeBeiraRio", "Sociedade", 126, 27, 47, 54, true, true, 0f, false, new Color(1.00f, .74f, .38f, 1f)),
        ["RodoviariaSantaRita"] = new DoorDef("RodoviariaSantaRita", "Rodoviaria", 118, 22, 46, 62, false, true, 0f, true, new Color(.82f, .90f, 1.00f, 1f)),
    };

    private sealed class DoorRig
    {
        public GameObject Root;
        public Material PortalMaterial;
        public Material SpillMaterial;
        public Light2D Light2D;
        public readonly List<Material> OwnedMaterials = new List<Material>();
        public readonly List<Mesh> OwnedMeshes = new List<Mesh>();
        public readonly List<LeafData> Leaves = new List<LeafData>();
        public Vector3 DoorCenter;
        public Vector2 DoorSize;
        public float BottomY;
        public int BaseOrder;
        public float UMin, UMax, VMin, VMax;

        public void Dispose()
        {
            if (Root != null) UnityEngine.Object.Destroy(Root);
            for (int i = 0; i < OwnedMaterials.Count; i++)
                if (OwnedMaterials[i] != null) UnityEngine.Object.Destroy(OwnedMaterials[i]);
            for (int i = 0; i < OwnedMeshes.Count; i++)
                if (OwnedMeshes[i] != null) UnityEngine.Object.Destroy(OwnedMeshes[i]);
        }
    }

    private sealed class LeafData
    {
        public Transform Pivot;
        public MeshFilter MeshFilter;
        public bool HingeLeft;
        public float Width;
        public float Height;
        public float Sign;
    }

    internal static bool SuportaCena(string sceneName) =>
        !string.IsNullOrEmpty(sceneName) && Portas.ContainsKey(sceneName);

    internal static void Entrar(Capitulo2Portal portal, string cenaDestino, string entradaDestino)
    {
        if (portal == null || string.IsNullOrEmpty(cenaDestino)) return;
        if (!SuportaCena(cenaDestino))
        {
            SceneTransition.Ir(cenaDestino, entradaDestino);
            return;
        }
        var fader = SceneFader.Instance;
        if (fader == null || !fader.IniciarTransicaoExterna(cenaDestino)) return;
        SceneTransition.PrepararEntrada(entradaDestino);
        fader.ExecutarTransicaoExterna(RotinaEntrada(fader, portal, Portas[cenaDestino], cenaDestino));
    }

    internal static void Sair(Capitulo2Interior3D interior, Capitulo2Portal portal)
    {
        if (interior == null || portal == null) return;
        string origem = interior.gameObject.scene.name;
        if (!SuportaCena(origem))
        {
            SceneFader.Instance.TransicionarPortaPosto(
                Capitulo2Interior3D.FocoDaPorta,
                () => SceneTransition.PrepararEntrada(portal.entradaDestino),
                () => Vector2.one * .5f,
                animarAntes: Capitulo2Interior3D.AnimarSaida,
                antesDaNevoa: Capitulo2Interior3D.SaidaAntesDaNevoa,
                cenaDestino: portal.cenaDestino);
            return;
        }
        var fader = SceneFader.Instance;
        if (fader == null || !fader.IniciarTransicaoExterna(portal.cenaDestino)) return;
        SceneTransition.PrepararEntrada(portal.entradaDestino);
        fader.ExecutarTransicaoExterna(RotinaSaida(fader, interior, Portas[origem], portal.cenaDestino));
    }

    private static IEnumerator RotinaEntrada(SceneFader fader, Capitulo2Portal portal, DoorDef def, string cenaDestino)
    {
        DoorRig rig = null;
        GameObject previewHost = null;
        Capitulo2Interior3D preview3D = null;
        Camera cam = Camera.main;
        CameraFollow follow = cam != null ? cam.GetComponent<CameraFollow>() : null;
        bool followWas = follow != null && follow.enabled;
        int prevMask = cam != null ? cam.cullingMask : -1;
        float prevOrtho = cam != null ? cam.orthographicSize : 5.5f;
        Vector3 startCamPos = cam != null ? cam.transform.position : Vector3.zero;

        PlayerController player = UnityEngine.Object.FindAnyObjectByType<PlayerController>();
        Chuva chuvaRua = UnityEngine.Object.FindAnyObjectByType<Chuva>();
        AsyncOperation preload = null;

        try
        {
            if (cam == null)
            {
                Capitulo2Interior3D.EntradaIntegradaPendente = true;
                SceneFader.CarregarCenaSegura(cenaDestino);
                yield break;
            }

            Application.backgroundLoadingPriority = ThreadPriority.Normal;
            cam.cullingMask = prevMask & ~Hidden3DMask;

            Vector3 fallbackPos = portal != null ? portal.transform.position + Vector3.up * .4f : Vector3.zero;
            rig = ConstruirDoorRig(def, fallbackPos, null);

            if (follow != null) follow.enabled = false;
            InteractionPrompt.Instance?.SetTarget(null);

            Vector3 startPlayerPos = player != null ? player.transform.position : rig.DoorCenter;
            Vector3 stopOutside = new Vector3(rig.DoorCenter.x, rig.BottomY + .05f, startPlayerPos.z);
            Vector3 insideThreshold = new Vector3(rig.DoorCenter.x, rig.BottomY + .16f, startPlayerPos.z);

            if (player != null)
                player.DefinirCaminhadaRoteirizada(true, PlayerController.DirUp, rig.BaseOrder + 2, 1f);

            float zoomX = (2f * prevOrtho * cam.aspect) / Mathf.Max(.35f, rig.DoorSize.x);
            float zoomY = (2f * prevOrtho) / Mathf.Max(.35f, rig.DoorSize.y);
            float targetZoom = Mathf.Clamp(Mathf.Max(zoomX * 1.24f, zoomY * 1.18f), 6f, 34f);

            bool tocouPorta = false;
            int passoInit = 0;
            Vector3 camAlvo = new Vector3(rig.DoorCenter.x, rig.DoorCenter.y, startCamPos.z);

            for (float t = 0f; t < DuracaoEntrada2D; t += Mathf.Min(Time.unscaledDeltaTime, .04f))
            {
                // Dilui montagem 3D, warm-up da RT e carga assíncrona nos primeiros passos com a porta ainda fechada.
                if (passoInit == 1)
                {
                    previewHost = new GameObject("CentroPortalPreview3D_" + cenaDestino);
                    preview3D = previewHost.AddComponent<Capitulo2Interior3D>();
                    preview3D.MontarPreview(cenaDestino, cam);
                    if (rig.PortalMaterial != null)
                        rig.PortalMaterial.SetTexture("_PortalTex", preview3D.TargetTexture);
                }
                else if (passoInit == 2 && preview3D != null && preview3D.Camera3D != null)
                {
                    preview3D.AtualizarPreview(0f);
                    preview3D.Camera3D.Render();
                }
                else if (passoInit == 3)
                {
                    preload = SceneFader.PrepararCargaAssincrona(cenaDestino, LoadSceneMode.Additive);
                }
                else if (t >= .40f)
                {
                    Application.backgroundLoadingPriority = ThreadPriority.High;
                }
                passoInit++;

                float pWalk1 = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, .44f, t));
                float pOpen = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.18f, .66f, t));
                float pWalk2 = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.46f, .80f, t));
                float pFadeOut = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.50f, .78f, t));
                float pZoomT = Mathf.InverseLerp(.04f, DuracaoEntrada2D - .04f, t);
                float zoomEase = pZoomT * pZoomT * (3f - 2f * pZoomT);
                float zoom = Mathf.Pow(targetZoom, Mathf.Pow(zoomEase, 1.35f));
                float pAudioEntrada = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.24f, DuracaoEntrada2D - .03f, t));
                SceneFader.DefinirEstadoAudioPortal(true, true, pAudioEntrada, pAudioEntrada * .72f);
                if (chuvaRua != null) chuvaRua.DefinirOclusaoPortaBar(Mathf.Lerp(0f, .42f, pAudioEntrada));

                if (!tocouPorta && t >= .18f)
                {
                    tocouPorta = true;
                    if (!def.OpenBay) SantaRitaSound.Play("door", .22f);
                }

                if (player != null)
                {
                    Vector3 pos = t < .46f
                        ? Vector3.Lerp(startPlayerPos, stopOutside, pWalk1)
                        : Vector3.Lerp(stopOutside, insideThreshold, pWalk2);
                    player.transform.position = pos;
                    player.DefinirCaminhadaRoteirizada(true, PlayerController.DirUp, rig.BaseOrder + 2, 1f - pFadeOut);
                }

                AtualizarDoorRig(rig, def, pOpen, pDive: Mathf.InverseLerp(.55f, DuracaoEntrada2D, t));
                if (preview3D != null)
                    preview3D.AtualizarPreview(Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.35f, DuracaoEntrada2D, t)));

                float camCenterT = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.02f, .70f, pZoomT));
                cam.transform.position = Vector3.Lerp(startCamPos, camAlvo, camCenterT);
                cam.orthographicSize = prevOrtho;
                AplicarZoomProjecao(cam, zoom);

                yield return null;
            }

            if (preview3D == null)
            {
                previewHost = new GameObject("CentroPortalPreview3D_" + cenaDestino);
                preview3D = previewHost.AddComponent<Capitulo2Interior3D>();
                preview3D.MontarPreview(cenaDestino, cam);
                if (rig.PortalMaterial != null)
                    rig.PortalMaterial.SetTexture("_PortalTex", preview3D.TargetTexture);
            }
            if (preload == null) preload = SceneFader.PrepararCargaAssincrona(cenaDestino, LoadSceneMode.Additive);

            AtualizarDoorRig(rig, def, 1f, 1f);
            SceneFader.DefinirEstadoAudioPortal(true, true, 1f, .72f);
            if (chuvaRua != null) chuvaRua.DefinirOclusaoPortaBar(.42f);
            preview3D.AtualizarPreview(1f);
            cam.transform.position = camAlvo;
            AplicarZoomProjecao(cam, targetZoom);

            if (preview3D.Camera3D != null) preview3D.Camera3D.Render();
            fader.CapturarBridge(preview3D.TargetTexture);

            if (previewHost != null) UnityEngine.Object.Destroy(previewHost);
            if (rig != null) rig.Dispose();
            rig = null;
            previewHost = null;

            cam.cullingMask = prevMask;
            cam.ResetProjectionMatrix();
            if (follow != null) follow.enabled = followWas;
            if (player != null) player.DefinirCaminhadaRoteirizada(false);

            Scene centroAtual = SceneManager.GetActiveScene();
            Capitulo2Interior3D.EntradaIntegradaPendente = true;
            Application.backgroundLoadingPriority = ThreadPriority.High;
            if (preload != null)
            {
                float esperaPreload = 0f;
                while (preload.progress < .89f && esperaPreload < .5f)
                {
                    esperaPreload += Time.unscaledDeltaTime;
                    yield return null;
                }

                raizesAtivasCentro.Clear();
                centroEmEspera = false;
                if (centroAtual.IsValid() && centroAtual.name == "CentroSantaRita")
                {
                    fader.DefinirFallbackAudioListener(true);
                    foreach (var root in centroAtual.GetRootGameObjects())
                    {
                        if (root != null && root.activeSelf)
                        {
                            raizesAtivasCentro.Add(root);
                            root.SetActive(false);
                        }
                    }
                    cenaCentroEmEspera = centroAtual;
                    centroEmEspera = true;
                }

                preload.completed += _ => fader.DefinirFallbackAudioListener(false);
                preload.allowSceneActivation = true;
                while (!preload.isDone) yield return null;
                fader.DefinirFallbackAudioListener(false);

                Scene cenaCarregada = SceneManager.GetSceneByName(cenaDestino);
                if (cenaCarregada.IsValid() && cenaCarregada.isLoaded)
                    SceneManager.SetActiveScene(cenaCarregada);
            }
            else
            {
                centroEmEspera = false;
                raizesAtivasCentro.Clear();
                SceneFader.CarregarCenaSegura(cenaDestino);
            }
            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            float espera = 0f;
            while ((Capitulo2Interior3D.Atual == null || Capitulo2Interior3D.Atual.TargetTexture == null ||
                    SceneManager.GetActiveScene().name != cenaDestino) && espera < 6f)
            {
                espera += Time.unscaledDeltaTime;
                yield return null;
            }

            if (Capitulo2Interior3D.Atual != null && Capitulo2Interior3D.Atual.Camera3D != null)
                Capitulo2Interior3D.Atual.Camera3D.Render();
            fader.DefinirAlphaBridge(0f);

            for (float t = 0f; t < Capitulo2Interior3D.DuracaoEntradaIntegrada; t += Mathf.Min(Time.unscaledDeltaTime, .04f))
            {
                float pFecho = Mathf.SmoothStep(0f, 1f, t / Capitulo2Interior3D.DuracaoEntradaIntegrada);
                SceneFader.DefinirEstadoAudioPortal(true, true, 0f, Mathf.Lerp(.72f, 1f, pFecho));
                Capitulo2Interior3D.AnimarEntradaIntegrada(t);
                yield return null;
            }
            SceneFader.DefinirEstadoAudioPortal(true, true, 0f, 1f);
            Capitulo2Interior3D.AnimarEntradaIntegrada(Capitulo2Interior3D.DuracaoEntradaIntegrada);
        }
        finally
        {
            Application.backgroundLoadingPriority = ThreadPriority.Normal;
            if (rig != null) rig.Dispose();
            if (previewHost != null) UnityEngine.Object.Destroy(previewHost);
            if (cam != null && SceneManager.GetActiveScene().name == "CentroSantaRita")
            {
                cam.cullingMask = prevMask;
                cam.ResetProjectionMatrix();
                if (follow != null) follow.enabled = followWas;
            }
            if (player != null && SceneManager.GetActiveScene().name == "CentroSantaRita")
                player.DefinirCaminhadaRoteirizada(false);
            fader.ConcluirTransicaoExterna();
        }
    }

    private static IEnumerator RotinaSaida(SceneFader fader, Capitulo2Interior3D interior, DoorDef def, string cenaRua)
    {
        DoorRig rig = null;
        Camera cam = null;
        CameraFollow follow = null;
        bool followWas = false;
        int prevMask = -1;
        PlayerController player = null;
        AsyncOperation preload = null;

        bool centroInstantaneo = centroEmEspera && cenaCentroEmEspera.IsValid() &&
            cenaCentroEmEspera.isLoaded && cenaCentroEmEspera.name == cenaRua;

        try
        {
            Application.backgroundLoadingPriority = ThreadPriority.Normal;
            int passoSaida = 0;

            for (float t = 0f; t < Capitulo2Interior3D.DuracaoSaidaIntegrada; t += Mathf.Min(Time.unscaledDeltaTime, .04f))
            {
                if (interior == null) break;
                if (!centroInstantaneo)
                {
                    if (passoSaida == 1)
                        preload = SceneFader.PrepararCargaAssincrona(cenaRua);
                    else if (t >= .32f)
                        Application.backgroundLoadingPriority = ThreadPriority.High;
                }
                passoSaida++;
                float pAudioSaida = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.24f, Capitulo2Interior3D.DuracaoSaidaIntegrada - .02f, t));
                float pMusicaSaida = Mathf.SmoothStep(0f, .52f, Mathf.InverseLerp(.12f, Capitulo2Interior3D.DuracaoSaidaIntegrada, t));
                SceneFader.DefinirEstadoAudioPortal(true, false, pAudioSaida, pMusicaSaida);
                Capitulo2Interior3D.AnimarSaidaIntegrada(t);
                yield return null;
            }
            SceneFader.DefinirEstadoAudioPortal(true, false, 1f, .52f);
            if (!centroInstantaneo && preload == null)
                preload = SceneFader.PrepararCargaAssincrona(cenaRua);
            if (interior != null)
            {
                Capitulo2Interior3D.AnimarSaidaIntegrada(Capitulo2Interior3D.DuracaoSaidaIntegrada);
                interior.PrepararCapturaBridgeSaida();
                fader.CapturarBridge(interior.TargetTexture);

                interior.PrepararCapturaBackdropInterior();
                fader.CapturarPortalBackdrop(interior.TargetTexture);
            }

            if (centroInstantaneo)
            {
                Scene cenaInterior = interior != null ? interior.gameObject.scene : SceneManager.GetActiveScene();
                if (cenaInterior.IsValid())
                {
                    foreach (var root in cenaInterior.GetRootGameObjects())
                        if (root != null) root.SetActive(false);
                }

                for (int i = 0; i < raizesAtivasCentro.Count; i++)
                {
                    if (raizesAtivasCentro[i] != null)
                        raizesAtivasCentro[i].SetActive(true);
                }
                raizesAtivasCentro.Clear();
                centroEmEspera = false;

                SceneManager.SetActiveScene(cenaCentroEmEspera);
                Capitulo2Director.RestaurarParaCena(cenaCentroEmEspera);

                player = UnityEngine.Object.FindAnyObjectByType<PlayerController>();
                if (player != null)
                {
                    player.enabled = true;
                    if (SceneTransition.ConsumirEntrada(out Vector2 posEntrada, out int dirEntrada))
                        player.TeleportarPara(posEntrada, dirEntrada);
                }
                var chuva = UnityEngine.Object.FindAnyObjectByType<Chuva>();
                if (chuva != null)
                {
                    chuva.DefinirModoInterior(false);
                    chuva.DefinirOclusaoPortaBar(0f);
                }
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (cenaInterior.IsValid() && cenaInterior != cenaCentroEmEspera)
                    SceneManager.UnloadSceneAsync(cenaInterior);

                yield return null;
            }
            else
            {
                centroEmEspera = false;
                raizesAtivasCentro.Clear();
                Application.backgroundLoadingPriority = ThreadPriority.High;
                if (preload != null)
                {
                    preload.allowSceneActivation = true;
                    while (!preload.isDone) yield return null;
                }
                else
                {
                    SceneFader.CarregarCenaSegura(cenaRua);
                }
                Application.backgroundLoadingPriority = ThreadPriority.Normal;

                float espera = 0f;
                while (SceneManager.GetActiveScene().name != cenaRua && espera < 6f)
                {
                    espera += Time.unscaledDeltaTime;
                    yield return null;
                }

                // Aguarda Capitulo2Director.Start (yield return null) e Player.Start concluírem o posicionamento.
                yield return null;
                yield return null;
            }
            SceneFader.DefinirEstadoAudioPortal(true, false, 0f, .52f);

            player = UnityEngine.Object.FindAnyObjectByType<PlayerController>();
            cam = Camera.main;
            follow = cam != null ? cam.GetComponent<CameraFollow>() : null;
            followWas = follow == null || follow.enabled || centroInstantaneo;
            prevMask = cam != null ? cam.cullingMask : -1;

            if (cam != null && player != null)
            {
                float prevOrtho = cam.orthographicSize;

                Vector3 destinoRua = player.transform.position;
                Vector3 endCamPos = follow != null
                    ? follow.ObterPosicaoSeguraPara(destinoRua)
                    : cam.transform.position;

                rig = ConstruirDoorRig(def, destinoRua + Vector3.up * 1.4f, fader.PortalBackdropTexture);

                if (follow != null) follow.enabled = false;

                Vector3 inicioSoleira = new Vector3(rig.DoorCenter.x, rig.BottomY + .06f, destinoRua.z);
                Vector3 camZoomPos = new Vector3(rig.DoorCenter.x, rig.DoorCenter.y - .08f, endCamPos.z);

                float zoomX = (2f * prevOrtho * cam.aspect) / Mathf.Max(.35f, rig.DoorSize.x);
                float startZoom = Mathf.Clamp(zoomX * .48f, 4.8f, 11.5f);
                AtualizarDoorRig(rig, def, 1f, 0f);
                player.transform.position = inicioSoleira;
                player.DefinirCaminhadaRoteirizada(true, PlayerController.DirDown, rig.BaseOrder + 2, 1f);
                cam.transform.position = camZoomPos;
                AplicarZoomProjecao(cam, startZoom);

                // Garante 1 frame renderizado atrás do BridgeFrame antes de dissolver para a rua 2D.
                yield return null;

                bool tocouFecho = false;
                for (float t = 0f; t < DuracaoSaida2D; t += Mathf.Min(Time.unscaledDeltaTime, .04f))
                {
                    float pMusicaRua = Mathf.SmoothStep(0f, 1f, t / DuracaoSaida2D);
                    SceneFader.DefinirEstadoAudioPortal(true, false, 0f, Mathf.Lerp(.52f, 1f, pMusicaRua));
                    float bridgeFade = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, .34f, t));
                    fader.DefinirAlphaBridge(bridgeFade);

                    float pWalk = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, .68f, t));
                    Vector3 pAtual = Vector3.Lerp(inicioSoleira, destinoRua, pWalk);
                    player.transform.position = pAtual;
                    int sortMarcus = pWalk > .28f ? (1000 - Mathf.RoundToInt(pAtual.y * 10)) : (rig.BaseOrder + 2);
                    player.DefinirCaminhadaRoteirizada(true, PlayerController.DirDown, sortMarcus, 1f);

                    float pClose = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.42f, .94f, t));
                    float openVal = 1f - pClose;
                    if (!tocouFecho && t >= .86f)
                    {
                        tocouFecho = true;
                        if (!def.OpenBay) SantaRitaSound.Play("door", .15f, .88f);
                    }

                    AtualizarDoorRig(rig, def, openVal, 0f);

                    float pZoomOut = Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, DuracaoSaida2D, t));
                    float zoom = Mathf.Pow(startZoom, 1f - pZoomOut);
                    cam.transform.position = Vector3.Lerp(camZoomPos, endCamPos, pZoomOut);
                    cam.orthographicSize = prevOrtho;
                    AplicarZoomProjecao(cam, zoom);

                    yield return null;
                }

                fader.DefinirAlphaBridge(0f);
                player.transform.position = destinoRua;
                cam.transform.position = endCamPos;
                cam.ResetProjectionMatrix();
                if (follow != null)
                {
                    follow.enabled = followWas;
                    follow.SnapImediatoParaAlvo();
                }
            }
        }
        finally
        {
            Application.backgroundLoadingPriority = ThreadPriority.Normal;
            if (rig != null) rig.Dispose();
            if (cam != null)
            {
                if (prevMask != -1) cam.cullingMask = prevMask;
                cam.ResetProjectionMatrix();
                if (follow != null) follow.enabled = followWas;
            }
            if (player != null) player.DefinirCaminhadaRoteirizada(false);
            fader.ConcluirTransicaoExterna();
        }
    }

    private static void AplicarZoomProjecao(Camera cam, float zoom)
    {
        cam.ResetProjectionMatrix();
        Matrix4x4 proj = cam.projectionMatrix;
        proj.m00 *= zoom;
        proj.m11 *= zoom;
        cam.projectionMatrix = proj;
    }

    private static SpriteRenderer EncontrarFachada(string nomeFachada)
    {
        if (string.IsNullOrEmpty(nomeFachada)) return null;
        foreach (var sr in UnityEngine.Object.FindObjectsByType<SpriteRenderer>())
        {
            if (sr != null && sr.name == nomeFachada && sr.sprite != null &&
                sr.gameObject.scene.name == "CentroSantaRita")
                return sr;
        }
        return null;
    }

    private static DoorRig ConstruirDoorRig(DoorDef def, Vector3 fallbackCenter, RenderTexture portalRt)
    {
        var rig = new DoorRig();
        SpriteRenderer facade = EncontrarFachada(def.Fachada);

        Vector3 bl, tr;
        if (facade != null && facade.sprite != null)
        {
            Sprite sp = facade.sprite;
            float ppu = Mathf.Max(1f, sp.pixelsPerUnit);
            Vector2 pivot = sp.pivot;
            float x0 = (def.PxX - pivot.x) / ppu;
            float x1 = (def.PxX + def.PxW - pivot.x) / ppu;
            float y0 = (def.PxYBottom - pivot.y) / ppu;
            float y1 = (def.PxYBottom + def.PxH - pivot.y) / ppu;
            bl = facade.transform.TransformPoint(new Vector3(x0, y0, 0f));
            tr = facade.transform.TransformPoint(new Vector3(x1, y1, 0f));
            if (bl.x > tr.x) { float tmp = bl.x; bl.x = tr.x; tr.x = tmp; }
            if (bl.y > tr.y) { float tmp = bl.y; bl.y = tr.y; tr.y = tmp; }
        }
        else
        {
            bl = new Vector3(fallbackCenter.x - .42f, fallbackCenter.y - .72f, 0f);
            tr = new Vector3(fallbackCenter.x + .42f, fallbackCenter.y + .72f, 0f);
        }

        float w = Mathf.Max(.25f, tr.x - bl.x);
        float h = Mathf.Max(.35f, tr.y - bl.y);
        rig.DoorCenter = new Vector3((bl.x + tr.x) * .5f, (bl.y + tr.y) * .5f, 0f);
        rig.DoorSize = new Vector2(w, h);
        rig.BottomY = bl.y;
        rig.BaseOrder = 2100;

        rig.Root = new GameObject("CentroPortalRig_" + def.Cena);
        rig.Root.transform.position = rig.DoorCenter;

        int sortingLayer = facade != null ? facade.sortingLayerID : 0;

        // 1. Quad do Portal 3D dentro do vão da porta
        Shader portalShader = Shader.Find("SantaRita/Cap2/PortalCentro3D");
        if (portalShader != null)
        {
            var portalGo = new GameObject("PortalCentro3D", typeof(MeshFilter), typeof(MeshRenderer));
            portalGo.transform.SetParent(rig.Root.transform, false);
            Mesh quadMesh = CriarQuadCentrado(w, h);
            rig.OwnedMeshes.Add(quadMesh);
            portalGo.GetComponent<MeshFilter>().sharedMesh = quadMesh;

            Material mat = new Material(portalShader);
            mat.SetTexture("_PortalTex", portalRt);
            mat.SetFloat("_PixelAmount", .78f);
            mat.SetVector("_PixelGrid", new Vector4(512f, 288f, 0f, 0f));
            mat.SetFloat("_ArchTop", def.ArchTop);
            mat.SetColor("_WarmRim", def.WarmColor);
            mat.SetFloat("_Alpha", 0f);
            mat.SetFloat("_FrameShadow", .55f);
            rig.OwnedMaterials.Add(mat);
            rig.PortalMaterial = mat;

            var mr = portalGo.GetComponent<MeshRenderer>();
            mr.sharedMaterial = mat;
            mr.sortingLayerID = sortingLayer;
            mr.sortingOrder = rig.BaseOrder;
        }

        // 2. Luz derramada na soleira da calçada
        Shader spillShader = Shader.Find("SantaRita/Cap2/LuzSoleira2D");
        if (spillShader != null)
        {
            var spillGo = new GameObject("LuzSoleira2D", typeof(MeshFilter), typeof(MeshRenderer));
            spillGo.transform.SetParent(rig.Root.transform, false);
            float spillH = Mathf.Clamp(h * 1.15f, 1.35f, 2.35f);
            float spillW = w * 1.95f;
            spillGo.transform.localPosition = new Vector3(0f, -h * .5f - spillH * .5f + .03f, 0f);
            Mesh spillMesh = CriarQuadCentrado(spillW, spillH);
            rig.OwnedMeshes.Add(spillMesh);
            spillGo.GetComponent<MeshFilter>().sharedMesh = spillMesh;

            Material matSpill = new Material(spillShader);
            matSpill.SetColor("_Color", new Color(def.WarmColor.r, def.WarmColor.g, def.WarmColor.b, .48f));
            matSpill.SetFloat("_Open", 0f);
            rig.OwnedMaterials.Add(matSpill);
            rig.SpillMaterial = matSpill;

            var mrSpill = spillGo.GetComponent<MeshRenderer>();
            mrSpill.sharedMaterial = matSpill;
            mrSpill.sortingLayerID = sortingLayer;
            mrSpill.sortingOrder = rig.BaseOrder;
        }

        // 3. Light2D quente vazando pela porta aberta na fachada e em Marcus
        var lightGo = new GameObject("LuzPorta2D", typeof(Light2D));
        lightGo.transform.SetParent(rig.Root.transform, false);
        lightGo.transform.localPosition = new Vector3(0f, -h * .22f, 0f);
        var l2d = lightGo.GetComponent<Light2D>();
        l2d.lightType = Light2D.LightType.Point;
        l2d.color = def.WarmColor;
        l2d.intensity = 0f;
        l2d.pointLightOuterRadius = 3.6f;
        l2d.pointLightInnerRadius = .28f;
        l2d.falloffIntensity = .62f;
        rig.Light2D = l2d;

        // 4. Folha(s) 2D da porta recortadas com exatidão da própria textura da fachada
        if (!def.OpenBay && facade != null && facade.sprite != null && facade.sprite.texture != null)
        {
            Texture2D tex = facade.sprite.texture;
            Rect r = facade.sprite.rect;
            float uMin = (r.x + def.PxX) / tex.width;
            float uMax = (r.x + def.PxX + def.PxW) / tex.width;
            float vMin = (r.y + def.PxYBottom) / tex.height;
            float vMax = (r.y + def.PxYBottom + def.PxH) / tex.height;
            rig.UMin = uMin;
            rig.UMax = uMax;
            rig.VMin = vMin;
            rig.VMax = vMax;

            Material baseMat = facade.sharedMaterial;
            Material leafMat = baseMat != null ? new Material(baseMat) : new Material(Shader.Find("Sprites/Default"));
            leafMat.mainTexture = tex;
            rig.OwnedMaterials.Add(leafMat);

            if (def.DoubleDoor)
            {
                float uMid = (uMin + uMax) * .5f;
                float halfW = w * .5f;
                AdicionarFolha(rig, leafMat, sortingLayer, true, -w * .5f, halfW, h, uMin, uMid, vMin, vMax);
                AdicionarFolha(rig, leafMat, sortingLayer, false, w * .5f, halfW, h, uMid, uMax, vMin, vMax);
            }
            else
            {
                float hingeX = def.HingeLeft ? -w * .5f : w * .5f;
                AdicionarFolha(rig, leafMat, sortingLayer, def.HingeLeft, hingeX, w, h, uMin, uMax, vMin, vMax);
            }
        }

        return rig;
    }

    private static void AdicionarFolha(DoorRig rig, Material mat, int sortingLayer, bool hingeLeft,
        float hingeLocalX, float leafW, float leafH, float uMin, float uMax, float vMin, float vMax)
    {
        var leafGo = new GameObject(hingeLeft ? "FolhaPortaEsq" : "FolhaPortaDir", typeof(MeshFilter), typeof(MeshRenderer));
        leafGo.transform.SetParent(rig.Root.transform, false);
        leafGo.transform.localPosition = new Vector3(hingeLocalX, -leafH * .5f, 0f);

        Mesh mesh = new Mesh { name = "DoorLeafMesh" };
        rig.OwnedMeshes.Add(mesh);
        var mf = leafGo.GetComponent<MeshFilter>();
        mf.sharedMesh = mesh;

        var mr = leafGo.GetComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        mr.sortingLayerID = sortingLayer;
        mr.sortingOrder = rig.BaseOrder + 1;

        var leaf = new LeafData
        {
            Pivot = leafGo.transform,
            MeshFilter = mf,
            HingeLeft = hingeLeft,
            Width = leafW,
            Height = leafH,
            Sign = hingeLeft ? 1f : -1f,
        };
        AtualizarFolhaMesh(leaf, 0f, uMin, uMax, vMin, vMax);
        rig.Leaves.Add(leaf);
    }

    private static void AtualizarFolhaMesh(LeafData leaf, float openT, float uMin, float uMax, float vMin, float vMax)
    {
        // Simula rotação em perspectiva para dentro do cômodo numa câmera ortográfica 2D:
        // a largura aparente reduz por cos(angle), a borda livre afunila levemente e entra na sombra.
        float angle = Mathf.Clamp01(openT) * (92f * Mathf.Deg2Rad);
        float cosA = Mathf.Clamp(Mathf.Cos(angle), .02f, 1f);
        float edgeX = leaf.Sign * leaf.Width * cosA;
        float topDrop = openT * leaf.Height * .13f;
        float botLift = openT * leaf.Height * .045f;

        Vector3 v0 = new Vector3(0f, 0f, 0f);
        Vector3 v1 = new Vector3(edgeX, botLift, 0f);
        Vector3 v2 = new Vector3(edgeX, leaf.Height - topDrop, 0f);
        Vector3 v3 = new Vector3(0f, leaf.Height, 0f);

        float uHinge = leaf.HingeLeft ? uMin : uMax;
        float uFree = leaf.HingeLeft ? uMax : uMin;

        Color cHinge = Color.Lerp(Color.white, new Color(.45f, .40f, .38f, 1f), openT * .7f);
        Color cFree = Color.Lerp(Color.white, new Color(.14f, .12f, .11f, 1f), openT * .92f);

        Mesh m = leaf.MeshFilter.sharedMesh;
        m.SetVertices(new[] { v0, v1, v2, v3 });
        m.SetUVs(0, new[]
        {
            new Vector2(uHinge, vMin),
            new Vector2(uFree, vMin),
            new Vector2(uFree, vMax),
            new Vector2(uHinge, vMax),
        });
        m.SetColors(new[] { cHinge, cFree, cFree, cHinge });
        m.SetTriangles(leaf.HingeLeft ? new[] { 0, 2, 1, 0, 3, 2 } : new[] { 0, 1, 2, 0, 2, 3 }, 0);
        m.RecalculateBounds();
    }

    private static void AtualizarDoorRig(DoorRig rig, DoorDef def, float openT, float pDive)
    {
        if (rig == null) return;
        float clampedOpen = Mathf.Clamp01(openT);
        float portalReveal = def.OpenBay ? Mathf.Clamp01(clampedOpen * 1.4f + .15f) : Mathf.Clamp01(clampedOpen * 2.1f);

        if (rig.PortalMaterial != null)
        {
            rig.PortalMaterial.SetFloat("_Alpha", portalReveal);
            rig.PortalMaterial.SetFloat("_PixelAmount", Mathf.Lerp(.85f, .12f, Mathf.Clamp01(pDive)));
            rig.PortalMaterial.SetFloat("_FrameShadow", Mathf.Lerp(.55f, 0f, Mathf.Clamp01(pDive * 1.2f)));
        }

        if (rig.SpillMaterial != null)
            rig.SpillMaterial.SetFloat("_Open", clampedOpen);

        if (rig.Light2D != null)
            rig.Light2D.intensity = 1.45f * clampedOpen * (1f - Mathf.Clamp01(pDive) * .35f);

        if (rig.Leaves.Count == 2)
        {
            float uMid = (rig.UMin + rig.UMax) * .5f;
            AtualizarFolhaMesh(rig.Leaves[0], clampedOpen, rig.UMin, uMid, rig.VMin, rig.VMax);
            AtualizarFolhaMesh(rig.Leaves[1], clampedOpen, uMid, rig.UMax, rig.VMin, rig.VMax);
        }
        else if (rig.Leaves.Count == 1)
        {
            AtualizarFolhaMesh(rig.Leaves[0], clampedOpen, rig.UMin, rig.UMax, rig.VMin, rig.VMax);
        }
    }

    private static Mesh CriarQuadCentrado(float w, float h)
    {
        float hw = w * .5f;
        float hh = h * .5f;
        Mesh m = new Mesh { name = "QuadCentrado" };
        m.vertices = new[]
        {
            new Vector3(-hw, -hh, 0f),
            new Vector3(hw, -hh, 0f),
            new Vector3(hw, hh, 0f),
            new Vector3(-hw, hh, 0f),
        };
        m.uv = new[]
        {
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 1f),
            new Vector2(0f, 1f),
        };
        m.triangles = new[] { 0, 2, 1, 0, 3, 2 };
        m.RecalculateBounds();
        return m;
    }
}
