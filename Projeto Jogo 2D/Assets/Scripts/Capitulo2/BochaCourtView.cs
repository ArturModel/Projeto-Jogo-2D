using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum BochaCameraView { Lancamento, Alvo, Superior }

// Geometria 3D projetada no canvas do minigame. A câmera do mundo/URP 2D
// continua intacta. A mesma posição/altura física alimenta todas as vistas.
public sealed class BochaCourtView : MaskableGraphic, IPointerDownHandler, IDragHandler,
    IPointerUpHandler, IPointerMoveHandler, IScrollHandler
{
    public Action<PointerEventData> Pressed, Dragged, Released, Hovered;
    public Action<float> Scrolled;
    public BochaCameraView View { get; private set; }
    public float Zoom { get; private set; } = 1;
    public int TrackCount => tracks.Count;
    public float AimDistance { get; private set; } = 12.65f;
    private BochaSimulation simulation;
    private BochaShot preview;
    private bool aiming;
    private Texture2D atlas;
    private Vector3 eye, right, up, forward;
    private float focal, clock;
    private readonly List<Face> room = new List<Face>();
    private readonly List<Face> faces = new List<Face>(7000);
    private readonly List<Track> tracks = new List<Track>(720);
    private readonly Vector3[] previous = new Vector3[10];
    private readonly bool[] seen = new bool[10];
    private readonly V[] clipped = new V[6];
    private static readonly Vector2 WhiteUV = new Vector2(4f / 256, 60f / 128);
    private struct V { public Vector3 p; public Vector2 uv; public Color c; }
    private struct Face { public V a, b, c, d; public bool quad, roof; public float depth; }
    private struct Track { public Vector2 a, b; public float time; }
    public override Texture mainTexture => atlas != null ? atlas : Texture2D.whiteTexture;

    protected override void Awake()
    {
        base.Awake(); atlas = Resources.Load<Texture2D>("Bocha/CanchaAtlas"); raycastTarget = true; CriarSala();
    }
    public void SetState(BochaSimulation sim, BochaShot shot, bool canAim, float aimDistance = 12.65f)
    {
        if (simulation != sim) { tracks.Clear(); Array.Clear(seen, 0, seen.Length); clock = 0; }
        simulation = sim; preview = shot; aiming = canAim; AimDistance=Mathf.Clamp(aimDistance,.8f,19.15f);SetVerticesDirty();
    }
    public void SetView(BochaCameraView view) { View = view; Zoom = 1; SetVerticesDirty(); }
    public void ChangeZoom(float amount) { Zoom = Mathf.Clamp(Zoom + amount * .08f, .75f, 1.65f); SetVerticesDirty(); }
    public void Tick(float dt) { clock += dt; SetVerticesDirty(); }
    public void RecordContacts()
    {
        if (simulation == null) return;
        foreach (var b in simulation.bolas)
        {
            if (!b.valida || b.id < 0 || b.id >= previous.Length) continue;
            var now = new Vector3(b.pos.x, b.altura, b.pos.y);
            if (seen[b.id] && b.altura < .015f && previous[b.id].y < .015f &&
                (new Vector2(now.x, now.z) - new Vector2(previous[b.id].x, previous[b.id].z)).sqrMagnitude > .0012f)
            {
                tracks.Add(new Track { a = new Vector2(previous[b.id].x, previous[b.id].z), b = b.pos, time = clock });
                if (tracks.Count > 720) tracks.RemoveAt(0);
                previous[b.id] = now;
            }
            else if (!seen[b.id] || b.altura > .015f || previous[b.id].y > .015f) previous[b.id] = now;
            seen[b.id] = true;
        }
    }
    private void CameraBasis()
    {
        Vector3 target;
        if (View == BochaCameraView.Superior)
        { eye = new Vector3(0, 26, 10); target = new Vector3(0, 0, 10); up = Vector3.left; }
        else if (View == BochaCameraView.Alvo)
        {
            Vector2 p = simulation != null && simulation.bolas.Count > 0 ? simulation.bolas[0].pos : new Vector2(0, 13.3f);
            target = new Vector3(p.x * .6f, 0, p.y); eye = target + new Vector3(5.3f, 6.8f, -6.4f); up = Vector3.up;
        }
        else { eye = new Vector3(0, 2.7f, -4.6f); target = new Vector3(0, .3f, 10); up = Vector3.up; }
        forward = (target - eye).normalized; right = Vector3.Cross(up, forward).normalized; up = Vector3.Cross(forward, right);
        focal = rectTransform.rect.height * 1.07f * Zoom;
    }
    public Vector2 Project(Vector3 p)
    {
        CameraBasis(); return ProjectReady(p);
    }
    private Vector2 ProjectReady(Vector3 p)
    {
        Vector3 d = p - eye; float z = View == BochaCameraView.Superior ? 26 : Vector3.Dot(d, forward);
        return new Vector2(Vector3.Dot(d, right), Vector3.Dot(d, up)) * (focal / Mathf.Max(.15f, z));
    }
    public bool CourtPoint(Vector2 screen, Camera camera, out Vector2 point)
    {
        point = Vector2.zero; CameraBasis();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screen, camera, out var local)) return false;
        Vector3 origin = eye, ray;
        if (View == BochaCameraView.Superior) { origin += right * (local.x * 26 / focal) + up * (local.y * 26 / focal); ray = forward; }
        else ray = (forward + right * (local.x / focal) + up * (local.y / focal)).normalized;
        if (ray.y >= -.0001f) return false;
        float t = -origin.y / ray.y; if (t <= 0 || t > 160) return false;
        Vector3 hit = origin + ray * t; point = new Vector2(hit.x, hit.z); return true;
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); CameraBasis(); faces.Clear();
        foreach (var f in room) if (!(f.roof && View != BochaCameraView.Lancamento)) faces.Add(f);
        foreach (var t in tracks)
        {
            float alpha = Mathf.Clamp01(1 - (clock - t.time) / 22) * .48f;
            if (alpha > .01f) Line(new Vector3(t.a.x, .008f, t.a.y), new Vector3(t.b.x, .008f, t.b.y), .027f, new Color(.83f, .71f, .49f, alpha));
        }
        if (simulation != null)
        {
            foreach (var b in simulation.bolas) if (b.valida)
            {
                Disc(new Vector3(b.pos.x + .025f, .012f, b.pos.y), b.raio * (1.55f + b.altura * .12f), new Color(.025f, .025f, .02f, .18f / (1 + b.altura * .25f)));
                Disc(new Vector3(b.pos.x, .015f, b.pos.y), b.raio * 1.12f, new Color(.025f, .025f, .02f, .30f / (1 + b.altura)));
                Sphere(b.Centro, b.raio, BallColor(b.lado), b.rotacao, true);
            }
            if (simulation.bolas.Count > 0)
            {
                var p = simulation.bolas[0].pos;
                Ring(new Vector3(p.x, .018f, p.y), .28f, .018f, new Color(.94f, .84f, .56f, .75f));
            }
        }
        if (aiming)
        {
            var origin = new Vector3(preview.x, .025f, .65f);
            Vector2 direction = preview.velocidade.normalized;
            float length=AimDistance;
            // Termina nas guias, mesmo quando o cursor aponta para além delas.
            if(Mathf.Abs(direction.x)>.001f)length=Mathf.Min(length,((direction.x>0?2.8f:-2.8f)-preview.x)/direction.x);
            if(direction.y>.001f)length=Mathf.Min(length,19.8f/direction.y-.65f);
            for (float d = .38f; d < length; d += .30f)
            {
                var a = origin + new Vector3(direction.x, 0, direction.y) * d;
                Line(a, a + new Vector3(direction.x, 0, direction.y) * Mathf.Min(.19f,length-d), .023f, new Color(.96f, .87f, .62f, .75f));
            }
            Ring(origin+new Vector3(direction.x,0,direction.y)*length,.13f,.018f,new Color(.96f,.87f,.62f,.85f));
            Sphere(new Vector3(preview.x, preview.altura + .18f, .65f), .18f, BallColor(BochaSide.Marcus), Quaternion.identity, true);
            if (preview.tipo == BochaThrowType.Volo)
            {
                float flight = (preview.vertical + Mathf.Sqrt(preview.vertical * preview.vertical + 2 * BochaSimulation.Gravidade * preview.altura)) / BochaSimulation.Gravidade;
                Vector2 end = new Vector2(preview.x, .65f) + preview.velocidade * flight;
                end = new Vector2(Mathf.Clamp(end.x, -2.8f, 2.8f), Mathf.Clamp(end.y, .2f, 19.8f));
                Ring(new Vector3(end.x, .02f, end.y), .35f, .026f, new Color(.88f, .71f, .38f, .85f));
                for (int i = 1; i <= 15; i++)
                {
                    float t = flight * i / 16;
                    var p = new Vector3(preview.x + preview.velocidade.x * t, .18f + preview.altura + preview.vertical * t - .5f * BochaSimulation.Gravidade * t * t, .65f + preview.velocidade.y * t);
                    Sphere(p, .018f, new Color(.86f, .75f, .47f, .6f), Quaternion.identity, false);
                }
            }
        }
        for (int i = 0; i < faces.Count; i++)
        {
            var f = faces[i]; Vector3 center = (f.a.p + f.b.p + f.c.p + (f.quad ? f.d.p : f.c.p)) * .25f;
            f.depth = Vector3.Dot(center - eye, forward); faces[i] = f;
        }
        faces.Sort((a, b) => b.depth.CompareTo(a.depth));
        foreach (var f in faces) { Triangle(vh, f.a, f.b, f.c); if (f.quad) Triangle(vh, f.a, f.c, f.d); }
    }
    private void Triangle(VertexHelper vh, V a, V b, V c)
    {
        V prev = c; int count = 0;
        V current = a;
        for (int i = 0; i < 3; i++)
        {
            if (i == 1) current = b; else if (i == 2) current = c;
            float pd = Vector3.Dot(prev.p - eye, forward) - .18f, cd = Vector3.Dot(current.p - eye, forward) - .18f;
            if ((pd >= 0) != (cd >= 0))
            {
                float t = pd / (pd - cd);
                clipped[count++] = new V { p = Vector3.Lerp(prev.p, current.p, t), uv = Vector2.Lerp(prev.uv, current.uv, t), c = Color.Lerp(prev.c, current.c, t) };
            }
            if (cd >= 0) clipped[count++] = current;
            prev = current;
        }
        if (count < 3) return;
        int start = vh.currentVertCount;
        for (int i = 0; i < count; i++)
        { var v = clipped[i]; vh.AddVert(ProjectReady(v.p), v.c, v.uv); }
        for (int i = 1; i < count - 1; i++) vh.AddTriangle(start, start + i, start + i + 1);
    }
    public static Color BallColor(BochaSide side) => side == BochaSide.Marcus ? new Color(.15f, .53f, .59f) : side == BochaSide.Tonao ? new Color(.73f, .32f, .14f) : new Color(.91f, .85f, .63f);
    private void Sphere(Vector3 center, float radius, Color color, Quaternion rotation, bool light)
    {
        const int slices = 16, rings = 8;
        for (int y = 0; y < rings; y++) for (int x = 0; x < slices; x++)
        {
            Vector3 a = Unit(x, y), b = Unit(x + 1, y), c = Unit(x + 1, y + 1), d = Unit(x, y + 1);
            Vector3 n = (a + b + c + d).normalized;
            if (Vector3.Dot(n, eye - center) <= 0) continue;
            V Make(Vector3 normal)
            {
                Vector3 local = Quaternion.Inverse(rotation) * normal;
                bool stripe = light && (Mathf.Abs(local.y) < .065f || Mathf.Abs(local.x * .71f + local.z * .71f) < .04f);
                Color baseColor = stripe ? Color.Lerp(color, new Color(.94f, .9f, .75f), .72f) : color;
                float diffuse = .42f + .52f * Mathf.Max(0, Vector3.Dot(normal, new Vector3(-.35f, .8f, -.45f).normalized));
                float spec = Mathf.Pow(Mathf.Max(0, Vector3.Dot(normal, ((eye - center).normalized + new Vector3(-.35f, .8f, -.45f).normalized).normalized)), 28) * .35f;
                Color result = light ? baseColor * diffuse + new Color(spec, spec * .92f, spec * .75f, 0) : baseColor;
                result.a = color.a; return new V { p = center + normal * radius, uv = WhiteUV, c = result };
            }
            faces.Add(new Face { a = Make(a), b = Make(b), c = Make(c), d = Make(d), quad = true });
        }
    }
    private static Vector3 Unit(int x, int y)
    { float a = x * Mathf.PI * 2 / 16, p = y * Mathf.PI / 8; return new Vector3(Mathf.Sin(p) * Mathf.Cos(a), Mathf.Cos(p), Mathf.Sin(p) * Mathf.Sin(a)); }
    private void Disc(Vector3 p, float radius, Color color)
    {
        for (int i = 0; i < 20; i++)
        {
            float a = i * Mathf.PI / 10, b = (i + 1) * Mathf.PI / 10;
            faces.Add(new Face { a = SolidV(p, color), b = SolidV(p + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * radius, color), c = SolidV(p + new Vector3(Mathf.Cos(b), 0, Mathf.Sin(b)) * radius, color) });
        }
    }
    private void Ring(Vector3 p, float radius, float width, Color color)
    {
        for (int i = 0; i < 28; i++)
        { float a = i * Mathf.PI / 14, b = (i + 1) * Mathf.PI / 14; Line(p + new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a)) * radius, p + new Vector3(Mathf.Cos(b), 0, Mathf.Sin(b)) * radius, width, color); }
    }
    private static V SolidV(Vector3 p, Color c) => new V { p = p, c = c, uv = WhiteUV };
    private void Line(Vector3 a, Vector3 b, float width, Color color)
    {
        Vector3 n = Vector3.Cross(Vector3.up, b - a).normalized * width * .5f;
        faces.Add(new Face { a = SolidV(a - n, color), b = SolidV(a + n, color), c = SolidV(b + n, color), d = SolidV(b - n, color), quad = true });
    }
    private void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d, int tile, Color tint, bool roof = false)
    {
        float u = tile * .25f; var normal = Vector3.Cross(b - a, d - a).normalized;
        float diffuse = .68f + .28f * Mathf.Abs(Vector3.Dot(normal, new Vector3(-.2f, 1, -.3f).normalized));
        tint *= diffuse; tint.a = 1;
        room.Add(new Face { a = new V { p = a, uv = new Vector2(u, .5f), c = RoomLight(a,tint) }, b = new V { p = b, uv = new Vector2(u + .25f, .5f), c = RoomLight(b,tint) },
            c = new V { p = c, uv = new Vector2(u + .25f, 1), c = RoomLight(c,tint) }, d = new V { p = d, uv = new Vector2(u, 1), c = RoomLight(d,tint) }, quad = true, roof = roof });
    }
    private static Color RoomLight(Vector3 p,Color tint)
    {
        float glow=0;
        foreach(float z in new[]{3f,10f,17f})glow+=Mathf.Exp(-(p.x*p.x*.055f+(p.z-z)*(p.z-z)*.10f));
        float edge=Mathf.Clamp01((Mathf.Abs(p.x)-2.7f)*4)*Mathf.Exp(-p.y*3);
        float level=.68f+Mathf.Min(.37f,glow*.31f)-edge*.14f;
        return tint*new Color(level*1.035f,level*.99f,level*.88f,1);
    }
    private void Box(Vector3 p, Vector3 size, int tile, Color color, bool roof = false)
    {
        Vector3 l = p - size * .5f, h = p + size * .5f;
        Quad(new Vector3(l.x,h.y,l.z),new Vector3(h.x,h.y,l.z),new Vector3(h.x,h.y,h.z),new Vector3(l.x,h.y,h.z),tile,color,roof);
        Quad(new Vector3(l.x,l.y,l.z),new Vector3(l.x,l.y,h.z),new Vector3(l.x,h.y,h.z),new Vector3(l.x,h.y,l.z),tile,color,roof);
        Quad(new Vector3(h.x,l.y,h.z),new Vector3(h.x,l.y,l.z),new Vector3(h.x,h.y,l.z),new Vector3(h.x,h.y,h.z),tile,color,roof);
        Quad(new Vector3(h.x,l.y,l.z),new Vector3(l.x,l.y,l.z),new Vector3(l.x,h.y,l.z),new Vector3(h.x,h.y,l.z),tile,color,roof);
        Quad(new Vector3(l.x,l.y,h.z),new Vector3(h.x,l.y,h.z),new Vector3(h.x,h.y,h.z),new Vector3(l.x,h.y,h.z),tile,color,roof);
    }
    private void CriarSala()
    {
        room.Clear(); Color white = Color.white;
        for (float z = -3; z < 22; z += .5f) for (float x = -6; x < 6; x += .5f)
        {
            bool sand = Mathf.Abs(x + .25f) < 3 && z >= 0 && z < 20;
            Quad(new Vector3(x,0,z),new Vector3(x+.5f,0,z),new Vector3(x+.5f,0,z+.5f),new Vector3(x,0,z+.5f),sand?0:1,white);
            var floor=room[room.Count-1];float u=((sand?0:64)+(Mathf.RoundToInt((x+6)*32)%64))/256f;
            float v=1-(Mathf.RoundToInt((z+3)*32)%64)/128f;
            floor.a.uv=new Vector2(u,v);floor.b.uv=new Vector2(u+16f/256,v);
            floor.c.uv=new Vector2(u+16f/256,v-16f/128);floor.d.uv=new Vector2(u,v-16f/128);room[room.Count-1]=floor;
        }
        foreach (float x in new[] { -3.13f, 3.13f })
            for (float z = .5f; z < 20; z += 1) Box(new Vector3(x,.18f,z),new Vector3(.26f,.36f,1),1,white);
        foreach (float z in new[] { -.13f, 20.13f })
        {float h=z<0?.26f:.5f;Box(new Vector3(0,h*.5f,z),new Vector3(6.5f,h,.26f),1,white);}
        // Cantos escuros de contato e pequenos filetes, sem faixas físicas.
        foreach(float x in new[]{-2.99f,2.99f})
        {
            Box(new Vector3(x,.009f,10),new Vector3(.06f,.012f,20),3,new Color(.19f,.13f,.075f));
            Box(new Vector3(x*1.05f,.368f,10),new Vector3(.255f,.018f,20),1,new Color(.8f,.7f,.55f));
        }
        // Linhas físicas de lançamento e referência; não alteram atrito.
        Box(new Vector3(0,.004f,1.1f),new Vector3(5.9f,.006f,.035f),3,new Color(.72f,.66f,.45f));
        for (float z = -2; z < 22; z += 2) foreach (float x in new[] { -6f, 6f })
        {
            Box(new Vector3(x,2.25f,z+1),new Vector3(.22f,4.5f,2),2,white);
            Box(new Vector3(x*.975f,.6f,z+1),new Vector3(.16f,1.2f,2),1,white);
        }
        for (float x = -5; x < 6; x += 2)
        {
            Box(new Vector3(x,2.25f,22),new Vector3(2,4.5f,.2f),2,white);
            Box(new Vector3(x,.6f,21.85f),new Vector3(2,1.2f,.16f),1,white);
        }
        // Forro fechado em vez do vazio preto atrás das travessas. É retirado
        // apenas na câmera de consulta, como um corte da mesma sala.
        for(float z=-3;z<22;z+=2)for(float x=-6;x<6;x+=2)
            Quad(new Vector3(x,4.5f,z),new Vector3(x+2,4.5f,z),new Vector3(x+2,4.5f,z+2),new Vector3(x,4.5f,z+2),2,new Color(.69f,.65f,.52f),true);
        foreach (float z in new[] { 3f, 10f, 17f })
        {
            Box(new Vector3(0,4.15f,z),new Vector3(12.2f,.22f,.25f),1,new Color(.7f,.66f,.57f),true);
            foreach (float x in new[] { -5.65f, 5.65f }) Box(new Vector3(x,2,z),new Vector3(.22f,4,.22f),1,white);
            // Luminária industrial simples, com difusor quente.
            Box(new Vector3(0,3.8f,z),new Vector3(1.6f,.13f,.35f),3,new Color(.9f,.72f,.35f),true);
            Box(new Vector3(0,3.93f,z),new Vector3(1.8f,.1f,.45f),1,white,true);
        }
        foreach (float x in new[] { -4.6f, 4.6f }) foreach (float z in new[] { 5f, 11f, 17f })
        {
            Box(new Vector3(x,.47f,z),new Vector3(.64f,.14f,2.7f),1,white);
            Box(new Vector3(x+Mathf.Sign(x)*.26f,.84f,z),new Vector3(.1f,.65f,2.7f),1,white);
            foreach(float dz in new[]{-1f,1f}) Box(new Vector3(x,.23f,z+dz),new Vector3(.5f,.46f,.12f),1,white);
        }
        foreach (float x in new[] { -2.7f, 2.7f })
        {
            Box(new Vector3(x,2.5f,21.7f),new Vector3(1.65f,1.8f,.18f),1,white);
            Box(new Vector3(x,2.5f,21.58f),new Vector3(1.35f,1.5f,.04f),3,new Color(.08f,.15f,.15f));
            Box(new Vector3(x,2.5f,21.52f),new Vector3(.065f,1.5f,.04f),1,white);
            Box(new Vector3(x,2.5f,21.51f),new Vector3(1.35f,.065f,.04f),1,white);
            Box(new Vector3(x,1.7f,21.47f),new Vector3(1.85f,.10f,.32f),1,white);
        }
        Box(new Vector3(-4.7f,1.25f,21.6f),new Vector3(1.1f,2.5f,.18f),1,new Color(.65f,.62f,.54f));
        // Suporte das bochas e copa ficam fora do piso de jogo.
        Box(new Vector3(4.5f,1,21.5f),new Vector3(1.9f,2,.35f),1,white);
        for(int i=0;i<4;i++)Box(new Vector3(4.5f,.35f+i*.45f,21.2f),new Vector3(1.8f,.07f,.35f),1,white);
        Box(new Vector3(4.6f,.85f,.7f),new Vector3(1.2f,.14f,1.4f),1,white);
        foreach(float x in new[]{4.15f,5.05f})foreach(float z in new[]{.2f,1.2f})Box(new Vector3(x,.4f,z),new Vector3(.1f,.8f,.1f),1,white);
        Box(new Vector3(4.6f,1.18f,.8f),new Vector3(.23f,.54f,.23f),3,new Color(.15f,.24f,.19f));
    }
    public void OnPointerDown(PointerEventData e) => Pressed?.Invoke(e);
    public void OnDrag(PointerEventData e) => Dragged?.Invoke(e);
    public void OnPointerUp(PointerEventData e) => Released?.Invoke(e);
    public void OnPointerMove(PointerEventData e) => Hovered?.Invoke(e);
    public void OnScroll(PointerEventData e) => Scrolled?.Invoke(e.scrollDelta.y);
}

public sealed class BochaMapGraphic : MaskableGraphic
{
    public BochaSimulation Simulation { get; set; }
    public Vector2 MapPoint(Vector2 world)
    {
        var r=rectTransform.rect;
        return new Vector2(r.x+5+(world.x+3)/6*(r.width-10),r.y+7+world.y/20*(r.height-14));
    }
    public void Refresh() => SetVerticesDirty();
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear(); var r = rectTransform.rect; var inner = new Rect(r.x+5,r.y+7,r.width-10,r.height-14);
        Quad(vh,r,new Color(.33f,.25f,.14f));Quad(vh,inner,new Color(.18f,.16f,.105f));
        if(Simulation==null)return;
        foreach(var b in Simulation.bolas)if(b.valida)
        {
            Vector2 p=MapPoint(b.pos);
            float radius=b.lado==BochaSide.Nenhum?2.1f:2.6f;
            Circle(vh,p,radius+1,new Color(.025f,.025f,.02f));Circle(vh,p,radius,BochaCourtView.BallColor(b.lado));
            if(b.altura>.05f)Circle(vh,p,.8f,Color.white);
        }
    }
    private static void Quad(VertexHelper vh,Rect r,Color c)
    {int i=vh.currentVertCount;vh.AddVert(new Vector2(r.xMin,r.yMin),c,Vector2.zero);vh.AddVert(new Vector2(r.xMax,r.yMin),c,Vector2.zero);vh.AddVert(new Vector2(r.xMax,r.yMax),c,Vector2.zero);vh.AddVert(new Vector2(r.xMin,r.yMax),c,Vector2.zero);vh.AddTriangle(i,i+1,i+2);vh.AddTriangle(i,i+2,i+3);}
    private static void Circle(VertexHelper vh,Vector2 p,float r,Color c)
    {int start=vh.currentVertCount;vh.AddVert(p,c,Vector2.zero);for(int i=0;i<=16;i++){float a=i*Mathf.PI/8;vh.AddVert(p+new Vector2(Mathf.Cos(a),Mathf.Sin(a))*r,c,Vector2.zero);if(i>0)vh.AddTriangle(start,start+i,start+i+1);}}
}
