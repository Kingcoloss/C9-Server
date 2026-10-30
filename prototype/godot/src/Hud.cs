using Eighth.Sim;
using Godot;

namespace EighthThrone.Demo;

// แถบสถานะบนจอ: พลังชีวิต พลังกาย กระแส สกิล บอส แผงเครือข่าย และวิธีเล่น
public partial class Hud : Control
{
    public IGameHost Host = null!;

    public override void _Draw()
    {
        var s = Host.Session;
        var c = s.Client;
        var size = GetViewportRect().Size;

        DrawPlayerPanel(c);
        DrawBoss(c, size);
        if (Host.ShowNet) DrawNetPanel(s, size);
        else DrawNetLine(s, size);
        if (Host.ShowHelp) DrawHelp(size);
        DrawBanner(size);
    }

    void DrawPlayerPanel(GameClient c)
    {
        var st = c.Predicted;
        var at = new Vector2(20, 20);
        DrawRect(new Rect2(at - new Vector2(10, 10), new Vector2(330, 132)), Palette.Panel);
        Text(Host.Bold, "ผู้สืบทอด", at + new Vector2(0, 16), 18, Palette.Text);

        Label("พลังชีวิต", at + new Vector2(0, 42));
        Bar(at + new Vector2(80, 30), 220, 14, (float)st.Hp / Cfg.PlayerMaxHp, Palette.Hp, $"{st.Hp}");
        Label("พลังกาย", at + new Vector2(0, 64));
        Bar(at + new Vector2(80, 52), 220, 10, st.Stamina / Cfg.StaminaMax, Palette.Stamina, "");

        Label("กระแส", at + new Vector2(0, 88));
        for (int i = 0; i < Cfg.MaxFlow; i++)
        {
            var p = at + new Vector2(88 + i * 22, 82);
            DrawCircle(p, 7, new Color(0, 0, 0, 0.6f), true, -1, true);
            if (i < st.Flow) DrawCircle(p, 6, new Color("ffb347"), true, -1, true);
        }
        Text(Host.Font, $"+{st.Flow * Cfg.FlowDamagePerStack * 100:0}% ดาเมจ", at + new Vector2(200, 88), 14, Palette.Muted);
        if (c.ClientTick <= st.CounterUntil)
            Text(Host.Bold, "สวนกลับ! ตีครั้งถัดไปแรงขึ้น 1.5 เท่า", at + new Vector2(0, 162), 16, Palette.Gold);

        Skill(at + new Vector2(0, 100), "Q", "หมัดสามดาว", (float)st.Cd1 / Cfg.Skill1Cooldown);
        Skill(at + new Vector2(160, 100), "E", "ดาวตกกระแทก", (float)st.Cd2 / Cfg.Skill2Cooldown);
    }

    void Skill(Vector2 at, string key, string name, float cd)
    {
        var box = new Rect2(at, new Vector2(26, 26));
        DrawRect(box, new Color(1, 1, 1, 0.12f));
        if (cd > 0) DrawRect(new Rect2(at, new Vector2(26, 26 * cd)), new Color(0, 0, 0, 0.7f));
        Text(Host.Bold, key, at + new Vector2(8, 19), 15, cd > 0 ? Palette.Muted : Palette.Text);
        Text(Host.Font, name, at + new Vector2(34, 19), 14, cd > 0 ? Palette.Muted : Palette.Text);
    }

    void DrawBoss(GameClient c, Vector2 size)
    {
        foreach (var e in c.Latest.Enemies)
        {
            if (e.Kind != EKind.Boss || e.Phase == EPhase.Dead) continue;
            // วางกลางช่องว่างระหว่างแผงผู้เล่น (ซ้าย) กับแผงเครือข่าย (ขวา)
            float left = 350, right = size.X - 420;
            float w = Mathf.Clamp(right - left - 20, 200, 480);
            var at = new Vector2(left + (right - left - w) / 2, 34);
            Text(Host.Bold, "ราชาหมึก", at + new Vector2(0, -6), 18, Palette.Text);
            Bar(at, w, 14, (float)e.Hp / e.MaxHp, Palette.Hp, $"{e.Hp} / {e.MaxHp}");
            Bar(at + new Vector2(0, 17), w, 5, e.Poise / e.MaxPoise, Palette.Poise, "");
        }
    }

    void DrawNetPanel(LocalSession s, Vector2 size)
    {
        var c = s.Client;
        const float w = 380;
        var at = new Vector2(size.X - w - 20, 20);
        int lines = 8 + Host.Feed.Count;
        DrawRect(new Rect2(at - new Vector2(10, 10), new Vector2(w + 10, 24 + lines * 21)), Palette.Panel);

        int viewLag = c.ClientTick - c.ViewTick(s.Now);
        float y = at.Y + 16;
        void Line(string text, Color col, Font? f = null, int fs = 15)
        {
            Text(f ?? Host.Font, text, new Vector2(at.X, y), fs, col);
            y += 21;
        }

        Line("เครือข่าย (จำลองในเครื่อง)", Palette.Text, Host.Bold, 17);
        string pings = string.Join("  ", Shared.Pings.Select((p, i) => p == s.PingMs ? $"[{i + 1}] {p}" : $"{i + 1}: {p}"));
        Line($"Ping {s.PingMs} ms     {pings}", Palette.Text);
        Line($"ชดเชยความหน่วง (C): {(s.LagCompensation ? "เปิด" : "ปิด")}",
            s.LagCompensation ? Palette.Stamina : Palette.Hurt);
        Line($"ตัวเราเดินนำ server {s.Lead} tick ({s.Lead * 1000 / Cfg.TickRate} ms)", Palette.Muted);
        Line($"ภาพศัตรูช้ากว่าตัวเรา {viewLag} tick ({viewLag * 1000 / Cfg.TickRate} ms)", Palette.Muted);
        Line($"server รอ input ล่าสุด {c.LastWaitTicks} tick (เพดาน {Cfg.DefenseWaitCapTicks})", Palette.Muted);
        Line($"ผลที่เครื่องทาย ไม่ตรง server: {c.Mismatches} / {c.ServerJudged}",
            c.Mismatches > 0 ? Palette.Hurt : Palette.Muted);
        Line($"แก้ตำแหน่งตัวเองตาม server: {c.Corrections} ครั้ง", Palette.Muted);
        foreach (var f in Host.Feed)
        {
            float age = (float)(Host.T - f.At);
            Line(f.Text, Palette.A(f.Color, age < 6 ? 1f : 0.55f), Host.Font, 14);
        }
    }

    // แบบย่อ: บรรทัดเดียวมุมขวาบน กับผลตัดสินล่าสุด
    void DrawNetLine(LocalSession s, Vector2 size)
    {
        string text = $"Ping {s.PingMs} ms · ชดเชยความหน่วง {(s.LagCompensation ? "เปิด" : "ปิด")} · N ดูรายละเอียด";
        float w = Host.Font.GetStringSize(text, HorizontalAlignment.Left, -1, 15).X;
        FeedLine? last = Host.Feed.Count > 0 && Host.T - Host.Feed[^1].At < 4 ? Host.Feed[^1] : null;
        if (last is { } l) w = Mathf.Max(w, Host.Font.GetStringSize(l.Text, HorizontalAlignment.Left, -1, 14).X);
        var at = new Vector2(size.X - w - 30, 20);
        DrawRect(new Rect2(at - new Vector2(10, 10), new Vector2(w + 20, 32 + (last != null ? 21 : 0))), Palette.Panel);
        Text(Host.Font, text, at + new Vector2(0, 16), 15, s.LagCompensation ? Palette.Text : Palette.Hurt);
        if (last is { } f) Text(Host.Font, f.Text, at + new Vector2(0, 37), 14, f.Color);
    }

    void DrawHelp(Vector2 size)
    {
        var rows = Host.HelpRows;
        float h = rows.Length * 22 + 16;
        float w = 0;
        foreach (var r in rows) w = Mathf.Max(w, Host.Font.GetStringSize(r, HorizontalAlignment.Left, -1, 15).X);
        var at = new Vector2(20, size.Y - h - 14);
        DrawRect(new Rect2(at - new Vector2(10, 0), new Vector2(w + 24, h)), Palette.Panel);
        for (int i = 0; i < rows.Length; i++)
            Text(Host.Font, rows[i], at + new Vector2(0, 26 + i * 22), 15, i == Host.HelpHighlightRow ? Palette.Gold : Palette.Text);
    }

    void DrawBanner(Vector2 size)
    {
        if (Host.T > Host.BannerUntil || Host.Banner.Length == 0) return;
        const int fs = 34;
        float w = Host.Bold.GetStringSize(Host.Banner, HorizontalAlignment.Left, -1, fs).X;
        var p = new Vector2((size.X - w) / 2, size.Y * Host.BannerY);
        DrawRect(new Rect2(p - new Vector2(24, 44), new Vector2(w + 48, 64)), Palette.Panel);
        DrawString(Host.Bold, p, Host.Banner, HorizontalAlignment.Left, -1, fs, Palette.Text);
    }

    void Label(string text, Vector2 at) => Text(Host.Font, text, at, 14, Palette.Muted);

    void Text(Font f, string text, Vector2 at, int size, Color col) =>
        DrawString(f, at, text, HorizontalAlignment.Left, -1, size, col);

    void Bar(Vector2 at, float w, float h, float frac, Color col, string label)
    {
        DrawRect(new Rect2(at, new Vector2(w, h)), new Color(0, 0, 0, 0.6f));
        DrawRect(new Rect2(at, new Vector2(w * Mathf.Clamp(frac, 0, 1), h)), col);
        if (label.Length > 0) Text(Host.Bold, label, at + new Vector2(6, h - 2), (int)Mathf.Max(11, h - 2), Palette.Text);
    }
}
