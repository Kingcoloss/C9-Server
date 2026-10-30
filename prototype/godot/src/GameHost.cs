using Eighth.Sim;
using Godot;

namespace EighthThrone.Demo;

// สิ่งที่ HUD และโหมดสาธิตใช้จากฉากหลัก ใช้ร่วมกันทั้งฉาก 3D และมุมมองตรวจ netcode แบบ 2D
public interface IGameHost
{
    LocalSession Session { get; }
    Font Font { get; }
    Font Bold { get; }
    List<FeedLine> Feed { get; }
    bool ShowServer { get; set; }
    bool ShowHelp { get; }
    bool ShowNet { get; }
    string Banner { get; }
    double BannerUntil { get; }
    float BannerY { get; }
    double T { get; }
    string[] HelpRows { get; }
    int HelpHighlightRow { get; }
    void SetPing(int i);
    void SetComp(bool on);
    Viewport GetViewport();
    SceneTree GetTree();
}

public readonly record struct FeedLine(string Text, Color Color, double At);

public static class Shared
{
    public static readonly int[] Pings = { 0, 80, 150, 250 };

    public static Font LoadFont(string latin, string thai)
    {
        var main = GD.Load<FontFile>($"res://fonts/{latin}.ttf");
        var th = GD.Load<FontFile>($"res://fonts/{thai}.ttf");
        if (main == null) return ThemeDB.FallbackFont;
        if (th != null) main.Fallbacks = new Godot.Collections.Array<Font> { th };
        return main;
    }

    // ข้อความผลตัดสินสำหรับ feed และข้อความลอยใต้ตัวละคร (ถ้ามี)
    public static bool DescribeVerdict(in UiEvent ui, out FeedLine line, out string? floatText, double now)
    {
        floatText = null;
        line = default;
        if (ui.Server == Judgment.Miss && !ui.Mismatch) return false;
        string wait = ui.Compensated ? $"server รอ {ui.WaitTicks} tick" : "server ไม่รอ";
        if (ui.EarlyTicks > 0) wait = $"server ตัดสินก่อนภาพของเรา {ui.EarlyTicks * 1000 / Cfg.TickRate} ms";
        if (ui.Mismatch)
        {
            line = new FeedLine($"เครื่องทาย {Judging.Th(ui.Local)} » server ตัดสิน {Judging.Th(ui.Server)}  ({wait})",
                Palette.Hurt, now);
            floatText = $"server: {Judging.Th(ui.Server)}";
        }
        else
        {
            line = new FeedLine($"{Judging.Th(ui.Server)}  เครื่อง = server  ({wait})", Palette.Judgment(ui.Server), now);
            if (ui.EarlyTicks > 2 && !Judging.Avoided(ui.Server)) floatText = "server ตัดสินไปก่อนที่เราเห็นท่าลง";
        }
        return true;
    }

    public static void Push(List<FeedLine> feed, FeedLine line)
    {
        feed.Add(line);
        if (feed.Count > 6) feed.RemoveAt(0);
    }
}
