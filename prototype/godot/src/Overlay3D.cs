using Eighth.Sim;
using Godot;

namespace EighthThrone.Demo;

// สิ่งที่ลอยตามตำแหน่งในโลก 3D: หลอดเลือดศัตรู ชื่อท่า ตัวเลขดาเมจ และเป้าที่ล็อกอยู่
public partial class Overlay3D : Control
{
    public Main3D Host = null!;

    public override void _Draw()
    {
        var cam = Host.Camera;
        var s = Host.Session;
        var c = s.Client;
        double now = s.Now;
        var size = GetViewportRect().Size;
        var me = c.PlayerRenderPos(now);

        // ขอบจอเรืองทองตอนพริ้ว
        float pf = 1 - (float)((Host.T - Host.PerfectAt) / 0.5);
        if (pf > 0)
        {
            var g = Palette.A(Palette.Gold, 0.45f * pf);
            float b = 26;
            DrawRect(new Rect2(0, 0, size.X, b), g);
            DrawRect(new Rect2(0, size.Y - b, size.X, b), g);
            DrawRect(new Rect2(0, 0, b, size.Y), g);
            DrawRect(new Rect2(size.X - b, 0, b, size.Y), g);
        }

        foreach (var e in c.Enemies(now))
        {
            var v = e.V;
            if (v.Phase == EPhase.Dead) continue;
            bool boss = v.Kind == EKind.Boss;
            if (!Project(cam, Art.V3(Host.VisualPos(e), boss ? 5.0f : 1.55f), out var head)) continue;
            if (boss) head.Y = Mathf.Max(head.Y, 100); // อย่าให้ชื่อท่าทับหลอดเลือดบอสด้านบน

            if (!boss)
            {
                const float w = 64;
                Bar(head + new Vector2(-w / 2, -8), w, 6, (float)v.Hp / v.MaxHp, Palette.Hp);
                Bar(head + new Vector2(-w / 2, 0), w, 3, v.Poise / v.MaxPoise, Palette.Poise);
            }

            if (v.Phase == EPhase.Windup && v.Attack != AttackId.None)
            {
                var def = Attacks.Get(v.Attack);
                string hint = def.Tele switch { Tele.Red => "  กันไม่ได้", Tele.Purple => "  รับพริ้วไม่ได้", _ => "" };
                CenterText(Host.Bold, def.NameTh.Replace(" (หลอก)", "") + hint, head + new Vector2(0, -18), 18, Palette.Tele(def.Tele));
            }

            if (v.Phase == EPhase.Staggered)
            {
                float dist = System.Numerics.Vector2.Distance(e.Pos, me) - v.Radius;
                if (dist <= Cfg.FinisherRange)
                    CenterText(Host.Bold, "F  ปิดฉาก", head + new Vector2(0, -20), 22, Palette.Gold);
            }

            if (v.Id == Host.SoftTarget && !Host.BotPlaying)
            {
                var t = head + new Vector2(0, -44 + Mathf.Sin((float)Host.T * 6) * 3);
                DrawColoredPolygon(new[] { t + new Vector2(-8, -8), t + new Vector2(8, -8), t }, Palette.A(Colors.White, 0.85f));
            }
        }

        foreach (var f in Host.Floaters)
        {
            float k = (float)((Host.T - f.Born) / f.Life);
            if (!Project(cam, f.Pos, out var p)) continue;
            CenterText(Host.Bold, f.Text, p + new Vector2(0, -40 * k), f.Size, Palette.A(f.Color, 1 - k * k));
        }

        if (!Host.MouseCaptured && !Host.BotPlaying)
            CenterText(Host.Font, "คลิกในหน้าต่างเพื่อควบคุมกล้องด้วยเมาส์  ·  หรือกด B ให้บอทเล่นให้ดู",
                new Vector2(size.X / 2, size.Y * 0.6f), 18, Palette.Text);
    }

    static bool Project(Camera3D cam, Vector3 world, out Vector2 screen)
    {
        screen = default;
        if (cam.IsPositionBehind(world)) return false;
        screen = cam.UnprojectPosition(world);
        return true;
    }

    void Bar(Vector2 at, float w, float h, float frac, Color col)
    {
        DrawRect(new Rect2(at, new Vector2(w, h)), new Color(0, 0, 0, 0.6f));
        DrawRect(new Rect2(at, new Vector2(w * Mathf.Clamp(frac, 0, 1), h)), col);
    }

    void CenterText(Font font, string text, Vector2 at, int size, Color col)
    {
        float w = font.GetStringSize(text, HorizontalAlignment.Left, -1, size).X;
        var p = at - new Vector2(w / 2, 0);
        DrawStringOutline(font, p, text, HorizontalAlignment.Left, -1, size, 6, new Color(0, 0, 0, col.A * 0.85f));
        DrawString(font, p, text, HorizontalAlignment.Left, -1, size, col);
    }
}
