using Eighth.Sim;
using Godot;
using NVec = System.Numerics.Vector2;

namespace EighthThrone.Demo;

// ฉากหลัก: สร้างทุกอย่างด้วยโค้ด รับปุ่ม เดินเวลาให้ server/client จำลอง แล้วสั่งวาด
public partial class Main : Node2D, IGameHost
{
    public const float Ppm = 42f;   // พิกเซลต่อเมตร

    public LocalSession Session { get; private set; } = null!;
    public Font Font { get; private set; } = null!;
    public Font Bold { get; private set; } = null!;
    public readonly Fx Fx = new();
    public List<FeedLine> Feed { get; } = new();
    public bool ShowServer { get; set; }
    public bool ShowHelp { get; private set; } = true;
    public bool ShowNet => true;
    public string Banner { get; private set; } = "";
    public double BannerUntil { get; private set; }
    public float BannerY => 0.68f;
    public double T { get; private set; }   // เวลาจริงตั้งแต่เปิดเกม ใช้ทำแอนิเมชัน
    public int HelpHighlightRow => 3;

    public string[] HelpRows { get; } =
    {
        "มุมมองตรวจ netcode (F2 กลับไปฉาก 3D) · WASD เดิน · เมาส์เล็ง · คลิกซ้าย/J ตีเบา · คลิกขวา/K ตีหนัก",
        "Space = หลบ · Shift ค้าง = กัน · Q / E = สกิล · F = ปิดฉากศัตรูที่เสียหลัก",
        "หลบหรือยกการ์ดตอนวงเตือนเต็มพอดี = พริ้ว (ได้จังหวะสวนกลับ)",
        "ทอง = หลบ/รับพริ้วได้ · แดง = กันไม่ได้ ต้องหลบ · ม่วง = กันได้ แต่รับพริ้วไม่ได้",
        "1-4 เปลี่ยน ping · C เปิด/ปิดการชดเชย · G ดูตำแหน่งจริงบน server · B ให้บอทเล่นแทน · R เริ่มใหม่ · H ซ่อน",
    };

    Camera2D _cam = null!;
    ArenaView _arena = null!;
    Hud _hud = null!;
    Autopilot? _auto;
    FightBot? _bot;
    int _botFrame;
    int _ping = 80;
    bool _comp = true;

    public override void _Ready()
    {
        DisplayServer.WindowSetTitle("บัลลังก์ที่แปด · demo การต่อสู้");
        RenderingServer.SetDefaultClearColor(new Color("0b0f16"));
        Font = Shared.LoadFont("NotoSans-Regular", "NotoSansThai-Regular");
        Bold = Shared.LoadFont("NotoSans-Bold", "NotoSansThai-Bold");
        ThemeDB.FallbackFont = Font;

        _arena = new ArenaView { Main = this };
        AddChild(_arena);
        _cam = new Camera2D { PositionSmoothingEnabled = true, PositionSmoothingSpeed = 6f };
        AddChild(_cam);
        _cam.MakeCurrent();

        var layer = new CanvasLayer();
        AddChild(layer);
        _hud = new Hud { Host = this, MouseFilter = Control.MouseFilterEnum.Ignore };
        layer.AddChild(_hud);
        _hud.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

        var args = OS.GetCmdlineUserArgs();
        if (args.Contains("--autoplay"))
        {
            var shots = args.FirstOrDefault(a => a.StartsWith("--shots="))?["--shots=".Length..];
            _auto = new Autopilot(this, shots);
        }

        Restart();
    }

    void Restart()
    {
        Session = new LocalSession(_ping, _comp, seed: GD.Randi() | 1u);
        Fx.Clear();
        Feed.Clear();
        ShowBanner("ล้มหมาป่าหนาม 3 ตัว แล้วราชาหมึกจะออกมา", 4);
    }

    public void ShowBanner(string text, double seconds)
    {
        Banner = text;
        BannerUntil = T + seconds;
    }

    // ---- ปุ่ม ----

    public override void _UnhandledInput(InputEvent e)
    {
        var c = Session.Client;
        if (e is InputEventMouseButton { Pressed: true } mb)
        {
            if (mb.ButtonIndex == MouseButton.Left) c.QueuePress(Press.Light);
            else if (mb.ButtonIndex == MouseButton.Right) c.QueuePress(Press.Heavy);
            return;
        }
        if (e is not InputEventKey { Pressed: true, Echo: false } k) return;
        switch (k.PhysicalKeycode)
        {
            case Key.Space: c.QueuePress(Press.Dodge); break;
            case Key.J: c.QueuePress(Press.Light); break;
            case Key.K: c.QueuePress(Press.Heavy); break;
            case Key.Q: c.QueuePress(Press.Skill1); break;
            case Key.E: c.QueuePress(Press.Skill2); break;
            case Key.F: c.QueuePress(Press.Finisher); break;
            case Key.Key1: SetPing(0); break;
            case Key.Key2: SetPing(1); break;
            case Key.Key3: SetPing(2); break;
            case Key.Key4: SetPing(3); break;
            case Key.C: SetComp(!_comp); break;
            case Key.G: ShowServer = !ShowServer; break;
            case Key.B:
                _bot = _bot == null ? new FightBot() : null;
                ShowBanner(_bot != null ? "บอทเล่นแทน: เปิด (กด B อีกครั้งเพื่อเล่นเอง)" : "บอทเล่นแทน: ปิด", 2);
                break;
            case Key.H: ShowHelp = !ShowHelp; break;
            case Key.R: Restart(); break;
            case Key.F2: GetTree().ChangeSceneToFile("res://main3d.tscn"); break;
            case Key.Escape: GetTree().Quit(); break;
            default: return;
        }
        GetViewport().SetInputAsHandled();
    }

    public void SetPing(int i)
    {
        _ping = Shared.Pings[i];
        Session.PingMs = _ping;
        ShowBanner($"Ping {_ping} ms", 1.5);
    }

    public void SetComp(bool on)
    {
        _comp = on;
        Session.LagCompensation = on;
        ShowBanner(on ? "ชดเชยความหน่วง: เปิด" : "ชดเชยความหน่วง: ปิด", 1.5);
    }

    static bool Down(Key a, Key b) => Input.IsPhysicalKeyPressed(a) || Input.IsPhysicalKeyPressed(b);

    // ---- ต่อเฟรม ----

    public override void _Process(double delta)
    {
        T += delta;
        var c = Session.Client;
        if (_auto != null) _auto.Step();
        else if (_bot != null) _bot.Step(c, Session.Now, ++_botFrame);
        else ReadHeld(c);

        Session.Advance(delta);
        Drain();
        Fx.Update(T);

        _cam.Position = ToPx(c.PlayerRenderPos(Session.Now));
        _arena.QueueRedraw();
        _hud.QueueRedraw();
    }

    void ReadHeld(GameClient c)
    {
        float mx = (Down(Key.D, Key.Right) ? 1 : 0) - (Down(Key.A, Key.Left) ? 1 : 0);
        float my = (Down(Key.S, Key.Down) ? 1 : 0) - (Down(Key.W, Key.Up) ? 1 : 0);
        var me = ToPx(c.PlayerRenderPos(Session.Now));
        var aim = GetGlobalMousePosition() - me;
        c.Held = new HeldInput
        {
            MoveX = mx,
            MoveY = my,
            Aim = aim.LengthSquared() > 1f ? aim.Angle() : c.Held.Aim,
            Guard = Down(Key.Shift, Key.L),
        };
    }

    void Drain()
    {
        var c = Session.Client;
        while (c.TryDequeue(out var ui))
        {
            _auto?.OnUi(ui);
            var at = ToPx(ui.Pos);
            switch (ui.Kind)
            {
                case UiKind.Judgment:
                    if (ui.Local == Judgment.Miss) break;
                    Fx.Text(Judging.Th(ui.Local) + (ui.Local is Judgment.PerfectDodge or Judgment.PerfectGuard ? "!" : ""),
                        at + new Vector2(0, -46), Palette.Judgment(ui.Local), 30, T, 1.1);
                    break;
                case UiKind.Verdict:
                    AddVerdict(ui, at);
                    break;
                case UiKind.LocalHit:
                    Fx.Spark(at, T);
                    break;
                case UiKind.EnemyDamage:
                    Fx.Text(ui.Amount.ToString(), at + new Vector2(GD.Randf() * 24 - 12, -30), Palette.Damage, 22, T, 0.8);
                    break;
                case UiKind.PlayerDamage:
                    Fx.Text($"-{ui.Amount}", at + new Vector2(18, -20), Palette.Hurt, 24, T, 0.9);
                    break;
                case UiKind.Stagger:
                    Fx.Text("เสียหลัก! กด F", at + new Vector2(0, -60), Palette.Gold, 24, T, 1.4);
                    break;
                case UiKind.Kill:
                    Fx.Text("ล้ม", at + new Vector2(0, -40), Palette.Muted, 22, T, 1.0);
                    break;
                case UiKind.Banner:
                    bool final = ui.Text.Contains("กด R");
                    ShowBanner(ui.Text, final ? double.MaxValue : 3);
                    break;
            }
        }
    }

    void AddVerdict(in UiEvent ui, Vector2 at)
    {
        if (!Shared.DescribeVerdict(ui, out var line, out var floatText, T)) return;
        Shared.Push(Feed, line);
        if (floatText != null) Fx.Text(floatText, at + new Vector2(0, 30), Palette.Hurt, 18, T, 1.6);
    }

    public static Vector2 ToPx(NVec v) => new(v.X * Ppm, v.Y * Ppm);
}
