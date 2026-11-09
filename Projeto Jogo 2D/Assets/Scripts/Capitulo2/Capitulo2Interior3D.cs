using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[DefaultExecutionOrder(50)]
internal sealed partial class Capitulo2Interior3D : MonoBehaviour
{
    private const int RoomLayer = 27;
    private const int RoomMask = 1 << RoomLayer;
    private readonly List<Object> owned = new();
    private readonly Dictionary<Color, Material> surfaces = new();
    private readonly Dictionary<Texture2D, Material> spriteMaterials = new();
    private readonly Dictionary<Sprite, Mesh> spriteMeshes = new();
    private readonly List<(SpriteRenderer source, MeshFilter mesh, MeshRenderer renderer)> actors = new();
    private Transform world;
    private Camera vista, baseCamera;
    private int previousMask;
    private CameraClearFlags previousClear;
    private PlayerController player;
    private Light lantern;
    private Vector3 focus;

    internal static Capitulo2Interior3D Atual { get; private set; }

    internal void Montar(string scene, PlayerController jogador)
    {
        if (jogador == null || Camera.main == null) return;
        player = jogador;
        baseCamera = Camera.main;
        previousMask = baseCamera.cullingMask;
        previousClear = baseCamera.clearFlags;
        world = new GameObject(scene + " 3D").transform;
        world.SetParent(transform, false);

        switch (scene)
        {
            case "IgrejaMatriz": MontarIgreja(); break;
            case "PrefeituraSantaRita": MontarPrefeitura(); break;
            case "RodoviariaSantaRita": MontarRodoviaria(); break;
            case "MercadinhoDeposito": MontarDeposito(); break;
            case "OficinaRadio": MontarOficina(); break;
            case "CasaCostureira": MontarCostureira(); break;
            case "AbrigoCemiterio": MontarAbrigo(); break;
            case "SociedadeBeiraRio": MontarSociedade(); break;
            default: return;
        }

        var cameraObject = new GameObject("Câmera de exploração 3D", typeof(Camera));
        cameraObject.transform.SetParent(world, false);
        vista = cameraObject.GetComponent<Camera>();
        vista.depth = baseCamera.depth + 1;
        vista.cullingMask = RoomMask;
        vista.clearFlags = CameraClearFlags.SolidColor;
        vista.backgroundColor = new Color(.023f, .027f, .030f);
        vista.fieldOfView = 38f;
        vista.nearClipPlane = .06f;
        vista.farClipPlane = 65f;
        vista.allowHDR = false;
        var urp = cameraObject.AddComponent<UniversalAdditionalCameraData>();
        urp.SetRenderer(1);
        urp.volumeLayerMask = 0;
        urp.renderPostProcessing = false;

        foreach (var sprite in FindObjectsByType<SpriteRenderer>(FindObjectsSortMode.None))
        {
            if (sprite.gameObject.scene != gameObject.scene || sprite.sprite == null) continue;
            if (sprite.GetComponentInParent<PlayerController>() != player &&
                !sprite.name.StartsWith("NPC_") && !sprite.name.Contains("Frequentador") &&
                sprite.name != "Motorista_Escala" && sprite.name != "Passageira_Espera") continue;
            var model = MeshSprite(sprite.name + " 3D", sprite.sprite, sprite.transform.position, 1);
            actors.Add((sprite, model.GetComponent<MeshFilter>(), model.GetComponent<MeshRenderer>()));
        }

        lantern = Lamp("Feixe da lanterna de Marcus", player.transform.position, .86f,
            new Color(1f, .87f, .64f), 8f, 7f).GetComponent<Light>();
        lantern.type = LightType.Spot;
        lantern.spotAngle = 48f;
        lantern.innerSpotAngle = 26f;
        focus = ToWorld(player.transform.position);
        baseCamera.cullingMask = 0;
        baseCamera.clearFlags = CameraClearFlags.Nothing;
        Atual = this;
        AtualizarVista(1);
    }

    private void LateUpdate()
    {
        if (vista == null || player == null) return;
        // A apresentação do minigame possui câmera e iluminação próprias.
        vista.enabled = !BochaMinigameUI.IsShowing;
        if (!vista.enabled) return;
        AtualizarVista(1f - Mathf.Exp(-Time.unscaledDeltaTime * 9));
    }

    private void AtualizarVista(float follow)
    {
        Vector2 at = player.transform.position;
        focus = Vector3.Lerp(focus, ToWorld(at, -SantaRitaShallowStairs.ElevationAt(at)), follow);
        if (!SantaRitaFalasEntrada.UsaCamera(vista)) vista.transform.SetPositionAndRotation(focus + new Vector3(0, 9.1f, -8.3f),
            Quaternion.Euler(46f, 0, 0));
        foreach (var actor in actors)
        {
            if (actor.source == null) continue;
            var sprite = actor.source.sprite;
            actor.renderer.enabled = actor.source.enabled && actor.source.gameObject.activeInHierarchy && sprite != null;
            if (!actor.renderer.enabled) continue;
            Vector2 feet = actor.source.transform.position;
            actor.mesh.transform.position = ToWorld(feet, -SantaRitaShallowStairs.ElevationAt(feet));
            actor.mesh.transform.localScale = new Vector3(actor.source.flipX ? -1f : 1f, 1, 1);
            actor.mesh.sharedMesh = MeshFor(sprite);
            actor.renderer.sharedMaterial = SpriteMaterial(sprite.texture);
        }

        lantern.enabled = player.ObterFeixeLanterna(out var lens, out var direction);
        if (lantern.enabled)
        {
            lantern.transform.position = ToWorld(new Vector2(lens.x, at.y), focus.y + .87f);
            lantern.transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, -.22f, direction.y));
        }
    }

    internal static Vector3 PendenteNoEcra(Camera fallback) =>
        Atual != null && Atual.vista != null
            ? Atual.vista.WorldToScreenPoint(new Vector3(9.4f, 2.37f, 12.25f))
            : fallback.WorldToScreenPoint(SantaRitaBochaAmbiente.PendenteDaEntrada);

    internal static bool TryFocus(Interactable target, out Camera camera, out Bounds bounds)
    {
        camera = null;
        bounds = default;
        if (Atual == null || Atual.vista == null || target == null) return false;
        string id = target is Capitulo2Interactable cue ? cue.interactionId : target.name;
        var model = Atual.world.Find(id);
        var render = model != null ? model.GetComponentInChildren<Renderer>() : null;
        Vector2 point = target.transform.position;
        bounds = render != null ? render.bounds :
            new Bounds(ToWorld(point, .80f - SantaRitaShallowStairs.ElevationAt(point)), new Vector3(.75f, .75f, .38f));
        camera = Atual.vista;
        return true;
    }

    private static Vector3 ToWorld(Vector2 point, float height = 0) => new Vector3(point.x, height, point.y);

    private GameObject Box(string name, Vector2 position, Vector3 size, float height, Color color)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Destroy(cube.GetComponent<Collider>());
        cube.name = name;
        cube.layer = RoomLayer;
        cube.transform.SetParent(world, false);
        cube.transform.position = ToWorld(position, height + size.y * .5f);
        cube.transform.localScale = size;
        if (!surfaces.TryGetValue(color, out var material))
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit")) { color = color };
            material.SetFloat("_Smoothness", .16f);
            surfaces.Add(color, material);
            owned.Add(material);
        }
        cube.GetComponent<MeshRenderer>().sharedMaterial = material;
        return cube;
    }

    private GameObject SpriteProp(string name, string resource, Vector2 position, float width)
    {
        var sprite = Resources.Load<Sprite>(resource);
        if (sprite == null && !resource.StartsWith("Art/"))
            sprite = Resources.Load<Sprite>("Art/Props/" + resource) ?? Resources.Load<Sprite>("Art/World/" + resource);
        if (sprite == null)
        {
            Debug.LogWarning("Arte do interior C2 ausente: " + resource, this);
            var empty = new GameObject(name);
            empty.transform.SetParent(world, false);
            empty.transform.position = ToWorld(position);
            return empty;
        }
        return MeshSprite(name, sprite, position, width / Mathf.Max(.01f, sprite.bounds.size.x));
    }

    private GameObject MeshSprite(string name, Sprite sprite, Vector2 position, float scale)
    {
        var obj = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
        obj.layer = RoomLayer;
        obj.transform.SetParent(world, false);
        obj.transform.position = ToWorld(position);
        obj.transform.localScale = Vector3.one * scale;
        obj.GetComponent<MeshFilter>().sharedMesh = MeshFor(sprite);
        obj.GetComponent<MeshRenderer>().sharedMaterial = SpriteMaterial(sprite.texture);
        return obj;
    }

    private Mesh MeshFor(Sprite sprite)
    {
        if (spriteMeshes.TryGetValue(sprite, out var mesh)) return mesh;
        var points = sprite.vertices;
        var vertices = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++) vertices[i] = new Vector3(points[i].x, points[i].y, 0);
        mesh = new Mesh { name = sprite.name, vertices = vertices, uv = sprite.uv };
        mesh.triangles = System.Array.ConvertAll(sprite.triangles, x => (int)x);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        spriteMeshes.Add(sprite, mesh);
        owned.Add(mesh);
        return mesh;
    }

    private Material SpriteMaterial(Texture2D texture)
    {
        if (spriteMaterials.TryGetValue(texture, out var material)) return material;
        material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.SetTexture("_BaseMap", texture);
        material.SetFloat("_AlphaClip", 1);
        material.SetFloat("_Cull", 0);
        material.SetFloat("_Cutoff", .39f);
        material.SetFloat("_Smoothness", .05f);
        material.EnableKeyword("_ALPHATEST_ON");
        spriteMaterials.Add(texture, material);
        owned.Add(material);
        return material;
    }

    private GameObject Lamp(string name, Vector2 position, float height, Color color, float intensity, float range)
    {
        var obj = new GameObject(name, typeof(Light));
        obj.layer = RoomLayer;
        obj.transform.SetParent(world, false);
        obj.transform.position = ToWorld(position, height);
        var light = obj.GetComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.cullingMask = RoomMask;
        return obj;
    }

    private void OnDestroy()
    {
        if (baseCamera != null)
        {
            baseCamera.cullingMask = previousMask;
            baseCamera.clearFlags = previousClear;
        }
        if (Atual == this) Atual = null;
        foreach (var item in owned) Destroy(item);
    }
}

