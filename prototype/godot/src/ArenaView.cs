using Eighth.Sim;
using Godot;

namespace EighthThrone.Demo;

// วาดสนาม สัญญาณเตือนท่า ศัตรู ผู้เล่น และเอฟเฟกต์ในพิกัดโลก (กล้องเลื่อนตามผู้เล่น)
public partial class ArenaView : Node2D
{
    public Main Main = null!;
    const float P = Main.Ppm;

    public override void _Draw()
    {
        var s = Main.Session;
        var c = s.Client;
        double now = s.Now;
        var me = Main.ToPx(c.PlayerRenderPos(now));

        DrawArena();

        var enemies = c.Enemies(now);
        foreach (var e in enemies) DrawTelegraph(e);
        foreach (var e in enemies) DrawEnemy(e, me, c.Predicted);
        DrawPlayer(c, me);
        if (Main.ShowServer) DrawServerTruth(s);
        DrawFx();
    }

    void DrawArena()
    {
        float r = Cfg.ArenaRadius * P;
        DrawCircle(Vector2.Zero, r + 18, Palette.Bg);
        DrawCircle(Vector2.Zero, r, Palette.Arena);
        for (int i = 1; i < 3; i++) DrawArc(Vector2.Zero, r * i / 3f, 0, Mathf.Tau, 96, Palette.Grid, 2, true);
        for (int i = 0; i < 8; i++)
        {
            var d = Vector2.FromAngle(i * Mathf.Tau / 8);
            DrawLine(d * r * 0.12f, d * r, Palette.Grid, 1.5f, true);
        }
        DrawArc(Vector2.Zero, r, 0, Mathf.Tau, 128, Palette.Rim, 4, true);
    }

    // ---- สัญญาณเตือนท่าของศัตรู ----

    void DrawTelegraph(in EnemyRender e)
    {
        var v = e.V;
        if (v.Attack == AttackId.None || (v.Phase != EPhase.Windup && v.Phase != EPhase.Active)) return;
        var def = Attacks.Get(v.Attack);
        var shape = Telegraph.Preview(e);
        var col = Palette.Tele(def.Tele);
        var center = Main.ToPx(shape.Center);
        float r = shape.Radius * P;

        if (v.Phase == EPhase.Active)
        {
            FillShape(shape.Kind, center, r, shape.Facing, shape.HalfArc, Palette.A(col, 0.55f));
            OutlineShape(shape.Kind, center, r, shape.Facing, shape.HalfArc, Palette.A(col, 1f), 3);
            return;
        }

        float p = Telegraph.Progress(e);
        FillShape(shape.Kind, center, r, shape.Facing, shape.HalfArc, Palette.A(col, 0.10f));
        FillShape(shape.Kind, center, r * p, shape.Facing, shape.HalfArc, Palette.A(col, 0.22f + 0.2f * p));
        OutlineShape(shape.Kind, center, r, shape.Facing, shape.HalfArc, Palette.A(col, 0.5f + 0.5f * p), p >= 1f ? 3 : 2);

        var head = Main.ToPx(e.Pos) + new Vector2(0, -(v.Radius * P + 44));
        string hint = def.Tele switch
        {
            Tele.Red => "  กันไม่ได้",
            Tele.Purple => "  รับพริ้วไม่ได้",
            _ => "",
        };
        CenterText(Main.Bold, def.NameTh.Replace(" (หลอก)", "") + hint, head, 17, col);
    }

    void FillShape(ShapeKind kind, Vector2 c, float r, float facing, float half, Color col)
    {
        if (r < 1f) return;
        if (kind == ShapeKind.Sector && half < Mathf.Pi - 0.01f) DrawColoredPolygon(Sector(c, r, facing, half), col);
        else DrawCircle(c, r, col, true, -1, true);
    }

    void OutlineShape(ShapeKind kind, Vector2 c, float r, float facing, float half, Color col, float w)
    {
        if (kind == ShapeKind.Sector && half < Mathf.Pi - 0.01f)
        {
            var pts = Sector(c, r, facing, half);
            var closed = new Vector2[pts.Length + 1];
            pts.CopyTo(closed, 0);
            closed[^1] = pts[0];
            DrawPolyline(closed, col, w, true);
        }
        else DrawArc(c, r, 0, Mathf.Tau, 64, col, w, true);
    }

    static Vector2[] Sector(Vector2 c, float r, float facing, float half)
    {
        const int n = 28;
        var pts = new Vector2[n + 2];
        pts[0] = c;
        for (int i = 0; i <= n; i++)
        {
            float a = facing - half + 2 * half * i / n;
            pts[i + 1] = c + Vector2.FromAngle(a) * r;
        }
        return pts;
    }

    // ---- ศัตรู ----

    void DrawEnemy(in EnemyRender e, Vector2 me, in PlayerState player)
    {
        var v = e.V;
        var pos = Main.ToPx(e.Pos);
        float r = v.Radius * P;
        bool boss = v.Kind == EKind.Boss;

        if (v.Phase == EPhase.Dead)
        {
            DrawCircle(pos, r, Palette.A(Palette.Muted, 0.18f), true, -1, true);
            return;
        }

        var body = boss ? Palette.Boss : Palette.Wolf;
        if (v.Phase == EPhase.Flinch) body = body.Lerp(Colors.White, 0.5f);
        if (v.Phase == EPhase.Staggered) body = body.Darkened(0.35f);

        DrawCircle(pos + new Vector2(0, r * 0.25f), r * 1.05f, new Color(0, 0, 0, 0.35f), true, -1, true);
        DrawCircle(pos, r, body, true, -1, true);
        DrawArc(pos, r, 0, Mathf.Tau, 48, body.Lightened(0.35f), 2, true);

        var dir = Vector2.FromAngle(e.Facing);
        var side = new Vector2(-dir.Y, dir.X);
        var tip = pos + dir * (r + 10);
        DrawColoredPolygon(new[] { tip, pos + dir * (r - 4) + side * 9, pos + dir * (r - 4) - side * 9 },
            body.Lightened(0.5f));
        if (boss)
        {
            // หนวดหกเส้นรอบตัว
            for (int i = 0; i < 6; i++)
            {
                float a = e.Facing + Mathf.Pi + (i - 2.5f) * 0.38f;
                float wave = Mathf.Sin((float)Main.T * 3 + i) * 0.15f;
                var a0 = pos + Vector2.FromAngle(a) * r * 0.9f;
                var a1 = pos + Vector2.FromAngle(a + wave) * r * 1.6f;
                DrawLine(a0, a1, body.Lightened(0.15f), 6, true);
            }
        }

        // หลอดพลังชีวิตและหลอดการทรงตัว
        float w = boss ? 0 : Mathf.Max(56, r * 2 + 16);
        if (!boss)
        {
            var top = pos + new Vector2(-w / 2, -r - 20);
            Bar(top, w, 6, (float)v.Hp / v.MaxHp, Palette.Hp);
            Bar(top + new Vector2(0, 8), w, 3, v.Poise / v.MaxPoise, Palette.Poise);
        }

        if (v.Phase == EPhase.Staggered)
        {
            float pulse = 0.5f + 0.5f * Mathf.Sin((float)Main.T * 8);
            DrawArc(pos, r + 8 + pulse * 4, 0, Mathf.Tau, 48, Palette.Gold, 3, true);
            float dist = (Main.ToPx(e.Pos) - me).Length() / P - v.Radius;
            if (dist <= Cfg.FinisherRange)
                CenterText(Main.Bold, "F  ปิดฉาก", pos + new Vector2(0, r + 34), 20, Palette.Gold);
        }
    }

    void Bar(Vector2 at, float w, float h, float frac, Color col)
    {
        DrawRect(new Rect2(at, new Vector2(w, h)), new Color(0, 0, 0, 0.6f));
        DrawRect(new Rect2(at, new Vector2(w * Mathf.Clamp(frac, 0, 1), h)), col);
    }

    // ---- ผู้เล่น ----

    void DrawPlayer(GameClient c, Vector2 pos)
    {
        var st = c.Predicted;
        float r = Cfg.PlayerRadius * P;
        var col = Palette.Player;
        int tick = c.ClientTick;

        if (st.Action == PAction.Dead) col = Palette.Muted;
        if (st.Action == PAction.HitStun) col = col.Lerp(Palette.Hurt, 0.6f);
        if (st.Action == PAction.GuardBreak) col = col.Lerp(new Color("ff9f43"), 0.6f);

        // เงาตามตอนหลบ
        if (st.Action == PAction.Dodge)
        {
            var back = new Vector2(-st.DodgeDir.X, -st.DodgeDir.Y);
            for (int i = 1; i <= 3; i++)
                DrawCircle(pos + back * (i * 14), r, Palette.A(col, 0.22f / i), true, -1, true);
            if (st.Invulnerable) DrawArc(pos, r + 6, 0, Mathf.Tau, 32, Palette.A(Colors.White, 0.7f), 2, true);
        }

        // วงท่าโจมตีของผู้เล่น
        if (PlayerSim.IsAttack(st.Action))
        {
            var def = PlayerSim.Def(st);
            bool active = PlayerSim.ActiveHit(st, out _, out _);
            float half = def.ArcDeg * 0.5f * Mathf.Pi / 180f;
            var tint = st.Action == PAction.Finisher ? Palette.Gold : Colors.White;
            if (half >= Mathf.Pi - 0.01f)
                DrawCircle(pos, def.Radius * P, Palette.A(tint, active ? 0.3f : 0.07f), true, -1, true);
            else
                DrawColoredPolygon(Sector(pos, def.Radius * P, st.Facing, half), Palette.A(tint, active ? 0.3f : 0.07f));
        }

        DrawCircle(pos + new Vector2(0, r * 0.3f), r * 1.05f, new Color(0, 0, 0, 0.35f), true, -1, true);
        DrawCircle(pos, r, col, true, -1, true);
        var dir = Vector2.FromAngle(st.Facing);
        DrawLine(pos, pos + dir * (r + 12), new Color("e6fffb"), 4, true);

        if (st.Guarding)
        {
            bool fresh = tick - st.GuardStartTick <= Cfg.PerfectGuardWindow;
            DrawArc(pos, r + 9, st.Facing - 1.1f, st.Facing + 1.1f, 20,
                fresh ? Palette.Gold : Palette.Guard, fresh ? 6 : 4, true);
        }

        if (tick <= st.CounterUntil)
        {
            float pulse = 0.5f + 0.5f * Mathf.Sin((float)Main.T * 14);
            DrawArc(pos, r + 14 + pulse * 3, 0, Mathf.Tau, 40, Palette.Gold, 3, true);
            CenterText(Main.Bold, "สวนกลับ!", pos + new Vector2(0, r + 34), 18, Palette.Gold);
        }
    }

    // ---- ตำแหน่งจริงบน server (กด G) ----

    void DrawServerTruth(LocalSession s)
    {
        var w = s.Server.World;
        foreach (var e in w.Enemies)
        {
            if (!e.Alive) continue;
            DrawArc(Main.ToPx(e.Pos), e.Radius * P, 0, Mathf.Tau, 40, Palette.A(Palette.Ghost, 0.85f), 2, true);
        }
        var pp = Main.ToPx(w.Player.Pos);
        DrawArc(pp, Cfg.PlayerRadius * P, 0, Mathf.Tau, 32, Palette.A(Palette.Ghost, 0.85f), 2, true);
        CenterText(Main.Font, "server ตอนนี้", pp + new Vector2(0, -Cfg.PlayerRadius * P - 10), 14, Palette.Ghost);
    }

    // ---- เอฟเฟกต์ ----

    void DrawFx()
    {
        double t = Main.T;
        foreach (var sp in Main.Fx.Sparks)
        {
            float k = (float)((t - sp.Born) / 0.18);
            for (int i = 0; i < 6; i++)
            {
                var d = Vector2.FromAngle(i * Mathf.Tau / 6 + 0.3f);
                DrawLine(sp.Pos + d * (6 + 20 * k), sp.Pos + d * (14 + 26 * k), Palette.A(Colors.White, 1 - k), 3, true);
            }
        }
        foreach (var f in Main.Fx.Floaters)
        {
            float k = (float)((t - f.Born) / f.Life);
            var at = f.Pos + new Vector2(0, -36 * k);
            CenterText(Main.Bold, f.Text, at, f.Size, Palette.A(f.Color, 1 - k * k));
        }
    }

    void CenterText(Font font, string text, Vector2 at, int size, Color col)
    {
        float w = font.GetStringSize(text, HorizontalAlignment.Left, -1, size).X;
        var p = at - new Vector2(w / 2, 0);
        DrawStringOutline(font, p, text, HorizontalAlignment.Left, -1, size, 5, new Color(0, 0, 0, col.A * 0.8f));
        DrawString(font, p, text, HorizontalAlignment.Left, -1, size, col);
    }
}
