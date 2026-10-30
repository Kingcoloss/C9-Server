using Eighth.Sim;
using Godot;

namespace EighthThrone.Demo;

// แถบสถานะบนจอ: พลังชีวิต พลังกาย กระแส สกิล บอส แผงเครือข่าย และวิธีเล่น
public partial class Hud : Control
{
    public Main Main = null!;

    public override void _Draw()
    {
        var s = Main.Session;
        var c = s.Client;
        var size = GetViewportRect().Size;

        DrawPlayerPanel(c);
        DrawBoss(c, size);
        DrawNetPanel(s, size);
        if (Main.ShowHelp) DrawHelp(size);
        DrawBanner(size);
    }

    void DrawPlayerPanel(GameClient c)
    {
        var st = c.Predicted;
        var at = new Vector2(20, 20);
        DrawRect(new Rect2(at - new Vector2(10, 10), new Vector2(330, 132)), Palette.Panel);
        Text(Main.Bold, "ผู้สืบทอด", at + new Vector2(0, 16), 18, Palette.Text);

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
        Text(Main.Font, $"+{st.Flow * Cfg.FlowDamagePerStack * 100:0}% ดาเมจ", at + new Vector2(200, 88), 14, Palette.Muted);
        if (c.ClientTick <= st.CounterUntil)
            Text(Main.Bold, "สวนกลับ! ตีครั้งถัดไปแรงขึ้น 1.5 เท่า", at + new Vector2(0, 162), 16, Palette.Gold);

        Skill(at + new Vector2(0, 100), "Q", "หมัดสามดาว", (float)st.Cd1 / Cfg.Skill1Cooldown);
        Skill(at + new Vector2(160, 100), "E", "ดาวตกกระแทก", (float)st.Cd2 / Cfg.Skill2Cooldown);
    }

    void Skill(Vector2 at, string key, string name, float cd)
    {
        var box = new Rect2(at, new Vector2(26, 26));
        DrawRect(box, new Color(1, 1, 1, 0.12f));
        if (cd > 0) DrawRect(new Rect2(at, new Vector2(26, 26 * cd)), new Color(0, 0, 0, 0.7f));
        Text(Main.Bold, key, at + new Vector2(8, 19), 15, cd > 0 ? Palette.Muted : Palette.Text);
        Text(Main.Font, name, at + new Vector2(34, 19), 14, cd > 0 ? Palette.Muted : Palette.Text);
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
            Text(Main.Bold, "ราชาหมึก", at + new Vector2(0, -6), 18, Palette.Text);
            Bar(at, w, 14, (float)e.Hp / e.MaxHp, Palette.Hp, $"{e.Hp} / {e.MaxHp}");
            Bar(at + new Vector2(0, 17), w, 5, e.Poise / e.MaxPoise, Palette.Poise, "");
        }
    }

    void DrawNetPanel(LocalSession s, Vector2 size)
    {
        var c = s.Client;
        const float w = 380;
        var at = new Vector2(size.X - w - 20, 20);
        int lines = 8 + Main.Feed.Count;
        DrawRect(new Rect2(at - new Vector2(10, 10), new Vector2(w + 10, 24 + lines * 21)), Palette.Panel);

        int viewLag = c.ClientTick - c.ViewTick(s.Now);
        float y = at.Y + 16;
        void Line(string text, Color col, Font? f = null, int fs = 15)
        {
            Text(f ?? Main.Font, text, new Vector2(at.X, y), fs, col);
            y += 21;
        }

        Line("เครือข่าย (จำลองในเครื่อง)", Palette.Text, Main.Bold, 17);
        string pings = string.Join("  ", Main.Pings.Select((p, i) => p == s.PingMs ? $"[{i + 1}] {p}" : $"{i + 1}: {p}"));
        Line($"Ping {s.PingMs} ms     {pings}", Palette.Text);
        Line($"ชดเชยความหน่วง (C): {(s.LagCompensation ? "เปิด" : "ปิด")}",
            s.LagCompensation ? Palette.Stamina : Palette.Hurt);
        Line($"ตัวเราเดินนำ server {s.Lead} tick ({s.Lead * 1000 / Cfg.TickRate} ms)", Palette.Muted);
        Line($"ภาพศัตรูช้ากว่าตัวเรา {viewLag} tick ({viewLag * 1000 / Cfg.TickRate} ms)", Palette.Muted);
        Line($"server รอ input ล่าสุด {c.LastWaitTicks} tick (เพดาน {Cfg.DefenseWaitCapTicks})", Palette.Muted);
        Line($"ผลที่เครื่องทาย ไม่ตรง server: {c.Mismatches} / {c.ServerJudged}",
            c.Mismatches > 0 ? Palette.Hurt : Palette.Muted);
        Line($"แก้ตำแหน่งตัวเองตาม server: {c.Corrections} ครั้ง", Palette.Muted);
        foreach (var f in Main.Feed)
        {
            float age = (float)(Main.T - f.At);
            Line(f.Text, Palette.A(f.Color, age < 6 ? 1f : 0.55f), Main.Font, 14);
        }
    }

    void DrawHelp(Vector2 size)
    {
        string[] rows =
        {
            "WASD เดิน · เมาส์เล็ง · คลิกซ้าย หรือ J = ตีเบา · คลิกขวา หรือ K = ตีหนัก",
            "Space = หลบ · Shift ค้าง = กัน · Q / E = สกิล · F = ปิดฉากศัตรูที่เสียหลัก",
            "หลบหรือยกการ์ดตอนวงเตือนเต็มพอดี = พริ้ว (ได้จังหวะสวนกลับ)",
            "ทอง = หลบ/รับพริ้วได้ · แดง = กันไม่ได้ ต้องหลบ · ม่วง = กันได้ แต่รับพริ้วไม่ได้",
            "1-4 เปลี่ยน ping · C เปิด/ปิดการชดเชย · G ดูตำแหน่งจริงบน server · B ให้บอทเล่นแทน · R เริ่มใหม่ · H ซ่อน",
        };
        float h = rows.Length * 22 + 16;
        var at = new Vector2(20, size.Y - h - 14);
        DrawRect(new Rect2(at - new Vector2(10, 0), new Vector2(720, h)), Palette.Panel);
        for (int i = 0; i < rows.Length; i++)
            Text(Main.Font, rows[i], at + new Vector2(0, 26 + i * 22), 15, i == 3 ? Palette.Gold : Palette.Text);
    }

    void DrawBanner(Vector2 size)
    {
        if (Main.T > Main.BannerUntil || Main.Banner.Length == 0) return;
        const int fs = 34;
        float w = Main.Bold.GetStringSize(Main.Banner, HorizontalAlignment.Left, -1, fs).X;
        var p = new Vector2((size.X - w) / 2, size.Y * 0.68f);
        DrawRect(new Rect2(p - new Vector2(24, 44), new Vector2(w + 48, 64)), Palette.Panel);
        DrawString(Main.Bold, p, Main.Banner, HorizontalAlignment.Left, -1, fs, Palette.Text);
    }

    void Label(string text, Vector2 at) => Text(Main.Font, text, at, 14, Palette.Muted);

    void Text(Font f, string text, Vector2 at, int size, Color col) =>
        DrawString(f, at, text, HorizontalAlignment.Left, -1, size, col);

    void Bar(Vector2 at, float w, float h, float frac, Color col, string label)
    {
        DrawRect(new Rect2(at, new Vector2(w, h)), new Color(0, 0, 0, 0.6f));
        DrawRect(new Rect2(at, new Vector2(w * Mathf.Clamp(frac, 0, 1), h)), col);
        if (label.Length > 0) Text(Main.Bold, label, at + new Vector2(6, h - 2), (int)Mathf.Max(11, h - 2), Palette.Text);
    }
}
