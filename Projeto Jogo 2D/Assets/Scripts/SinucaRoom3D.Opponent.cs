using System.Collections.Generic;
using UnityEngine;

public sealed partial class SinucaRoom3D
{
    private Actor opponent;
    private Transform opponentCue;
    private readonly List<Vector2> opponentPath = new List<Vector2>();
    private readonly Sprite[] opponentSprites = new Sprite[24];
    private int opponentFacing;
    // World heading is independent of the billboard and survives camera movement.
    private Vector2 opponentHeading = Vector2.down;
    private int opponentWalkFrame, opponentShotPose;
    private bool opponentShooting;
    private Vector2 opponentBall, opponentDirection;
    private float opponentStride;
    private int opponentWaypoint;
    public bool OpponentAtShot => opponentWaypoint >= opponentPath.Count;

    // A separate cue stays in the room layer; Marcus's first-person hands never drive it.
    private void BuildOpponent()
    {
        opponent = actors.Find(a => a.definition.id == "NPC_Ze_Interior");
        if (opponent == null) return;
        opponentCue = BuildCue(dynamic);
        opponentCue.name = "TacoZe";
        opponentCue.gameObject.SetActive(false);
        var sheet = Resources.Load<Texture2D>("Art/Characters/Npc_Ze_Sinuca");
        if (sheet == null) { Debug.LogWarning("Sinuca: folha nova do Zé ausente."); return; }
        float cell = sheet.width / 4f;
        for (int i = 0; i < opponentSprites.Length; i++)
        {
            var sprite = Sprite.Create(sheet, new Rect(i % 4 * cell, (sheet.height / cell - 1 - i / 4) * cell, cell, cell),
                new Vector2(.5f, .12f), cell / 2f, 0, SpriteMeshType.FullRect);
            sprite.name = "ZeSinuca_" + i; opponentSprites[i] = sprite; owned.Add(sprite); NativeMesh(sprite);
        }
        opponent.rest = opponentSprites[0];
        opponent.filter.sharedMesh = NativeMesh(opponent.rest);
        opponent.renderer.sharedMaterial = PixelMaterial(opponent.rest);
        PixelSources[opponent.definition.id] = opponent.rest;
    }

    private bool OpponentWalkable(Vector2 p)
    {
        if (p.x < -5.05f || p.x > 5.05f || p.y < -7.05f || p.y > 3.4f ||
            (Mathf.Abs(p.x) < 1.85f && Mathf.Abs(p.y) < 1.13f)) return false;
        foreach (var prop in SantaRitaBarLayout.Props)
        {
            if (prop.id == "NPC_Ze_Interior" || prop.kind == SantaRitaBarLayout.Kind.Wall ||
                prop.kind == SantaRitaBarLayout.Kind.Decor || prop.kind == SantaRitaBarLayout.Kind.Chair) continue;
            var at = Plan(prop.position - Vector2.up * prop.supportOffset);
            float radius = prop.kind == SantaRitaBarLayout.Kind.Actor ? .42f : prop.width * .5f + .24f;
            if (prop.kind == SantaRitaBarLayout.Kind.Counter)
            { if (Mathf.Abs(p.x - at.x) < radius && Mathf.Abs(p.y - at.z) < .69f) return false; }
            else if (Vector2.Distance(p, new Vector2(at.x, at.z)) < radius) return false;
        }
        return true;
    }

    private bool OpponentSegment(Vector2 from, Vector2 to)
    {
        int count = Mathf.Max(1, Mathf.CeilToInt(Vector2.Distance(from, to) / .04f));
        for (int i = 0; i <= count; i++) if (!OpponentWalkable(Vector2.Lerp(from, to, (float)i / count))) return false;
        return true;
    }

    public bool PrepareOpponentShot(Vector2 ball, Vector2 direction)
    {
        CancelOpponentShot();
        if (opponent == null || opponentSprites[0] == null || direction.sqrMagnitude < .001f) return false;
        opponentBall = ball * MetresPerUnit; opponentDirection = direction.normalized;
        // Intersect the backwards shot ray with the player's clearance rectangle.
        var back = -opponentDirection;
        float tx = Mathf.Abs(back.x) > .0001f ? ((back.x > 0 ? 1.94f : -1.94f) - opponentBall.x) / back.x : float.PositiveInfinity;
        float ty = Mathf.Abs(back.y) > .0001f ? ((back.y > 0 ? 1.22f : -1.22f) - opponentBall.y) / back.y : float.PositiveInfinity;
        var target = opponentBall + back * Mathf.Min(tx, ty);
        if (!OpponentWalkable(target)) return false;
        var start = new Vector2(opponent.root.localPosition.x, opponent.root.localPosition.z);
        // Grid search includes furniture, table clearance and both exact endpoint segments.
        const float cell = .12f; const int width = 85, height = 88;
        var parent = new int[width * height]; for (int i = 0; i < parent.Length; i++) parent[i] = -2;
        var queue = new Queue<int>(); int found = -1;
        for (int y = 0; y < height; y++) for (int x = 0; x < width; x++)
        {
            var p = new Vector2(-5.04f + x * cell, -7.04f + y * cell); int id = y * width + x;
            if (Vector2.Distance(p, start) <= .18f && OpponentSegment(start, p)) { parent[id] = -1; queue.Enqueue(id); }
        }
        int[] steps = { -1, 1, -width, width };
        while (queue.Count > 0)
        {
            int id = queue.Dequeue(), x = id % width, y = id / width;
            var p = new Vector2(-5.04f + x * cell, -7.04f + y * cell);
            if (Vector2.Distance(p, target) <= .18f && OpponentSegment(p, target)) { found = id; break; }
            foreach (int step in steps)
            {
                int n = id + step;
                if (n < 0 || n >= parent.Length || parent[n] != -2 || Mathf.Abs(n % width - x) + Mathf.Abs(n / width - y) != 1) continue;
                var next = new Vector2(-5.04f + n % width * cell, -7.04f + n / width * cell);
                if (!OpponentSegment(p, next)) continue;
                parent[n] = id; queue.Enqueue(n);
            }
        }
        if (found < 0) return false;
        for (int id = found; id >= 0; id = parent[id]) opponentPath.Add(new Vector2(-5.04f + id % width * cell, -7.04f + id / width * cell));
        opponentPath.Reverse(); opponentPath.Add(target);
        // Remove the grid's stair steps when there is unobstructed floor between points.
        for (int i = 0; i + 2 < opponentPath.Count;)
            if (OpponentSegment(opponentPath[i], opponentPath[i + 2])) opponentPath.RemoveAt(i + 1); else i++;
        opponentCue.gameObject.SetActive(true); return true;
    }

    public void TickOpponentWalk(float dt)
    {
        if (opponent == null || OpponentAtShot || dt <= 0) return;
        var from = new Vector2(opponent.root.localPosition.x, opponent.root.localPosition.z);
        var to = Vector2.MoveTowards(from, opponentPath[opponentWaypoint], 1.15f * Mathf.Min(dt, .05f));
        float distance = Vector2.Distance(from, to); opponentStride += distance;
        opponent.root.localPosition = new Vector3(to.x, 0, to.y);
        int phase = (int)(opponentStride / .16f) % 4;
        opponentWalkFrame = phase == 3 ? 2 : phase + 1;
        if ((to - from).sqrMagnitude > .000001f) opponentHeading = (to - from).normalized;
        UpdateOpponentView();
        opponentCue.localPosition = opponent.root.localPosition + Vector3.up * 1.0f;
        opponentCue.localRotation = Quaternion.Euler(-80, 0, 0);
        if (Vector2.Distance(to, opponentPath[opponentWaypoint]) < .001f) opponentWaypoint++;
        if (OpponentAtShot) { opponentWalkFrame = 0; opponentHeading = opponentDirection; UpdateOpponentView(); }
    }

    // progress=0..1: settle, feather the cue, pull back, contact at .82, recover.
    public void PoseOpponentShot(float progress)
    {
        if (opponent == null) return;
        int pose = progress < .12f || progress > .98f ? 0 : progress < .28f || progress > .92f ? 1 : progress < .82f ? 2 : 3;
        opponentShooting = true;
        opponentShotPose = pose;
        opponentHeading = opponentDirection;
        UpdateOpponentView();
        var axis = new Vector3(opponentDirection.x, 0, opponentDirection.y);
        float lift = BridgeRailLift(opponentBall / MetresPerUnit, opponentDirection);
        axis = (axis - Vector3.up * (lift / .25f)).normalized;
        float pull = progress < .65f ? .12f + Mathf.Sin(progress * 32) * .035f :
            progress < .76f ? Mathf.Lerp(.12f, .24f, (progress - .65f) / .11f) : Mathf.Lerp(.24f, -.087f, Mathf.Clamp01((progress - .76f) / .06f));
        opponentCue.localPosition = new Vector3(opponentBall.x, TableY + .047f, opponentBall.y) - axis * pull;
        opponentCue.localRotation = Quaternion.LookRotation(axis);
    }

    // Called on every camera update, including while Zé is standing or paused mid-pose.
    private void UpdateOpponentView()
    {
        if (opponent == null || opponentSprites[0] == null) return;
        var delta = camera.transform.localPosition - opponent.root.localPosition;
        var viewer = new Vector2(delta.x, delta.z);
        if (viewer.sqrMagnitude > .0001f)
        {
            viewer.Normalize();
            float alignment = Vector2.Dot(opponentHeading, viewer);
            // Hysteresis prevents flickering at front/profile/back boundaries.
            float frontLimit = opponentFacing == 0 ? .60f : .72f;
            float backLimit = opponentFacing == 1 ? -.60f : -.72f;
            opponentFacing = alignment > frontLimit ? 0 : alignment < backLimit ? 1 : 2;
            var screenRight = new Vector2(-viewer.y, viewer.x);
            float side = Vector2.Dot(opponentHeading, screenRight);
            var scale = opponent.root.localScale;
            scale.x = Mathf.Abs(scale.x) * (opponentFacing == 2 && side < 0 ? -1 : 1);
            opponent.root.localScale = scale;
        }
        int frame = opponentShooting
            ? (opponentFacing == 0 ? 16 : opponentFacing == 1 ? 20 : 12) + opponentShotPose
            : opponentFacing * 4 + opponentWalkFrame;
        opponent.filter.sharedMesh = NativeMesh(opponentSprites[frame]);
    }

    public void CancelOpponentShot()
    {
        opponentPath.Clear(); opponentWaypoint = 0; opponentShooting = false; opponentWalkFrame = 0;
        if (opponentCue != null) opponentCue.gameObject.SetActive(false);
        UpdateOpponentView();
    }
}
