using Eighth.Sim;
using Godot;

namespace EighthThrone.Demo;

public sealed class Floater3D
{
    public string Text = "";
    public Vector3 Pos;
    public Color Color;
    public int Size;
    public double Born, Life;
}

// ฉากหลักแบบ 3D: กล้องหลังตัวละคร ใช้ Sim และ netcode จำลองชุดเดียวกับมุมมอง 2D
public partial class Main3D : Node3D, IGameHost
{
    public LocalSession Session { get; private set; } = null!;
    public Font Font { get; private set; } = null!;
    public Font Bold { get; private set; } = null!;
    public List<FeedLine> Feed { get; } = new();
    public bool ShowServer { get; set; }
    public bool ShowHelp { get; private set; } = true;
    public bool ShowNet { get; private set; }
    public string Banner { get; private set; } = "";
    public double BannerUntil { get; private set; }
    public float BannerY => 0.24f; // ตัวละครอยู่กลางล่างของจอ จึงขึ้นป้ายไว้ด้านบน
    public double T { get; private set; }
    public int HelpHighlightRow => 3;

    public string[] HelpRows { get; } =
    {
        "เมาส์ = หมุนกล้อง (คลิกในหน้าต่างเพื่อจับเมาส์ · Esc ปล่อย) · WASD เดินตามทิศกล้อง · ลูกศรซ้ายขวา หมุนกล้อง",
        "คลิกซ้าย/J = ตีเบา (ต่อได้ 4 จังหวะ) · คลิกขวา/K = ตีหนัก · Space = หลบ · Shift ค้าง = กัน",
        "Q หมัดสามดาว · E ดาวตกกระแทก · F ปิดฉากศัตรูที่เสียหลัก · หลบ/กันตอนวงเตือนเต็มพอดี = พริ้ว",
        "ทอง = หลบ/รับพริ้วได้ · แดง = กันไม่ได้ ต้องหลบ · ม่วง = กันได้ แต่รับพริ้วไม่ได้",
        "1-4 ping · C ชดเชยความหน่วง · N แผงเครือข่าย · G ตำแหน่งบน server · B บอทเล่นแทน · F2 มุมมอง 2D · R เริ่มใหม่ · H ซ่อน",
    };

    public Camera3D Camera { get; private set; } = null!;
    public readonly List<Floater3D> Floaters = new();
    public int SoftTarget = -1;
    public double PerfectAt = -10;
    public bool MouseCaptured => Input.MouseMode == Input.MouseModeEnum.Captured;
    public bool BotPlaying => _auto != null || _bot != null;

    readonly Dictionary<int, Node3D> _enemies = new();
    readonly Dictionary<int, MeshInstance3D> _ghosts = new();
    readonly List<(MeshInstance3D mesh, StandardMaterial3D mat, double born)> _flashes = new();
    List<Node3D> _crystals = new();
    PlayerModel _player = null!;
    GroundFx _ground = null!;
    Overlay3D _overlay = null!;
    Hud _hud = null!;
    Node3D _actors = null!;
    Autopilot? _auto;
    FightBot? _bot;
    int _botFrame;
    int _ping = 80;
    bool _comp = true;

    float _yaw;
    float _pitch = -0.32f;
    float _dist = 6.2f;
    Vector3 _camTarget;
    float _shake;
    double _slowUntil = -1;

    public override void _Ready()
    {
        DisplayServer.WindowSetTitle("บัลลังก์ที่แปด · demo การต่อสู้ 3D");
        Font = Shared.LoadFont("NotoSans-Regular", "NotoSansThai-Regular");
        Bold = Shared.LoadFont("NotoSans-Bold", "NotoSansThai-Bold");
        ThemeDB.FallbackFont = Font;

        BuildEnvironment();
        _crystals = Arena3D.Build(this);
        _actors = new Node3D();
        AddChild(_actors);
        _ground = new GroundFx();
        AddChild(_ground);
        Camera = new Camera3D { Fov = 62, Near = 0.1f, Far = 300 };
        AddChild(Camera);
        Camera.MakeCurrent();

        var layer = new CanvasLayer();
        AddChild(layer);
        _overlay = new Overlay3D { Host = this, MouseFilter = Control.MouseFilterEnum.Ignore };
        layer.AddChild(_overlay);
        _overlay.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _hud = new Hud { Host = this, MouseFilter = Control.MouseFilterEnum.Ignore };
        layer.AddChild(_hud);
        _hud.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

        var args = OS.GetCmdlineUserArgs();
        if (args.Contains("--autoplay"))
        {
            var shots = args.FirstOrDefault(a => a.StartsWith("--shots="))?["--shots=".Length..];
            _auto = new Autopilot(this, shots);
            ShowHelp = false;
        }
        Restart();
    }

    void BuildEnvironment()
    {
        var sky = new ProceduralSkyMaterial
        {
            SkyTopColor = new Color("141a33"),
            SkyHorizonColor = new Color("5b4a6e"),
            GroundBottomColor = new Color("0c0e14"),
            GroundHorizonColor = new Color("3a3048"),
            SunAngleMax = 20,
        };
        var env = new Godot.Environment
        {
            BackgroundMode = Godot.Environment.BGMode.Sky,
            Sky = new Sky { SkyMaterial = sky },
            AmbientLightSource = Godot.Environment.AmbientSource.Sky,
            AmbientLightEnergy = 0.45f,
            TonemapMode = Godot.Environment.ToneMapper.Filmic,
            GlowEnabled = true,
            GlowIntensity = 0.7f,
            GlowBloom = 0.08f,
            FogEnabled = true,
            FogLightColor = new Color("2c2a40"),
            FogDensity = 0.012f,
        };
        AddChild(new WorldEnvironment { Environment = env });
        var sun = new DirectionalLight3D
        {
            LightColor = new Color("ffe2b8"),
            LightEnergy = 1.35f,
            ShadowEnabled = true,
            DirectionalShadowMaxDistance = 45,
            RotationDegrees = new Vector3(-52, 28, 0),
        };
        AddChild(sun);
        var rim = new DirectionalLight3D
        {
            LightColor = new Color("7fa8ff"),
            LightEnergy = 0.35f,
            RotationDegrees = new Vector3(-25, 200, 0),
        };
        AddChild(rim);
    }

    void Restart()
    {
        Session = new LocalSession(_ping, _comp, seed: GD.Randi() | 1u);
        foreach (var n in _enemies.Values) n.QueueFree();
        _enemies.Clear();
        foreach (var g in _ghosts.Values) g.QueueFree();
        _ghosts.Clear();
        _player?.QueueFree();
        _player = new PlayerModel();
        _actors.AddChild(_player);
        Floaters.Clear();
        Feed.Clear();
        _yaw = Art.Yaw(Session.Client.Predicted.Facing);
        _camTarget = Art.V3(Session.Client.Predicted.Pos, 1.3f);
        ShowBanner("ล้มหมาป่าหนาม 3 ตัว แล้วราชาหมึกจะออกมา", 4);
    }

    public void ShowBanner(string text, double seconds)
    {
        Banner = text;
        BannerUntil = T + seconds;
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

    // ---- ปุ่ม ----

    public override void _UnhandledInput(InputEvent e)
    {
        var c = Session.Client;
        if (e is InputEventMouseMotion mm && MouseCaptured)
        {
            _yaw -= mm.Relative.X * 0.0045f;
            _pitch = Mathf.Clamp(_pitch - mm.Relative.Y * 0.0035f, -1.1f, 0.15f);
            return;
        }
        if (e is InputEventMouseButton { Pressed: true } mb)
        {
            if (mb.ButtonIndex == MouseButton.WheelUp) _dist = Mathf.Max(3.5f, _dist - 0.4f);
            else if (mb.ButtonIndex == MouseButton.WheelDown) _dist = Mathf.Min(11f, _dist + 0.4f);
            else if (!MouseCaptured && !BotPlaying) Input.MouseMode = Input.MouseModeEnum.Captured;
            else if (mb.ButtonIndex == MouseButton.Left) c.QueuePress(Press.Light);
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
            case Key.N: ShowNet = !ShowNet; break;
            case Key.H: ShowHelp = !ShowHelp; break;
            case Key.B:
                _bot = _bot == null ? new FightBot() : null;
                if (_bot != null) Input.MouseMode = Input.MouseModeEnum.Visible;
                ShowBanner(_bot != null ? "บอทเล่นแทน: เปิด (กด B อีกครั้งเพื่อเล่นเอง)" : "บอทเล่นแทน: ปิด", 2);
                break;
            case Key.R: Restart(); break;
            case Key.F2:
                Input.MouseMode = Input.MouseModeEnum.Visible;
                GetTree().ChangeSceneToFile("res://main.tscn");
                break;
            case Key.Escape: Input.MouseMode = Input.MouseModeEnum.Visible; break;
            default: return;
        }
        GetViewport().SetInputAsHandled();
    }

    static bool Down(Key k) => Input.IsPhysicalKeyPressed(k);

    // ---- ต่อเฟรม ----

    public override void _Process(double delta)
    {
        float dt = (float)delta;
        T += delta;
        var c = Session.Client;

        if (_auto != null) _auto.Step();
        else if (_bot != null) _bot.Step(c, Session.Now, ++_botFrame);
        else ReadHeld(c, dt);

        // หลบหรือรับพริ้วแล้วเวลาช้าลงครู่หนึ่ง (ทั้ง server และ client จำลองช้าลงเท่ากัน)
        Session.Advance(delta * (T < _slowUntil ? 0.45 : 1.0));
        Drain();
        SyncWorld(dt);
        UpdateCamera(dt);
        _overlay.QueueRedraw();
        _hud.QueueRedraw();
    }

    void ReadHeld(GameClient c, float dt)
    {
        if (Down(Key.Left)) _yaw += 2.2f * dt;
        if (Down(Key.Right)) _yaw -= 2.2f * dt;
        float w = (Down(Key.W) ? 1 : 0) - (Down(Key.S) ? 1 : 0);
        float d = (Down(Key.D) ? 1 : 0) - (Down(Key.A) ? 1 : 0);
        var fwd = new Vector2(-Mathf.Sin(_yaw), -Mathf.Cos(_yaw));
        var right = new Vector2(Mathf.Cos(_yaw), -Mathf.Sin(_yaw));
        var move = fwd * w + right * d;
        float camAngle = Mathf.Atan2(fwd.Y, fwd.X);
        c.Held = new HeldInput
        {
            MoveX = move.X,
            MoveY = move.Y,
            Aim = SoftAim(c, camAngle),
            Guard = Down(Key.Shift) || Down(Key.L),
        };
    }

    // ล็อกเป้าแบบอ่อน: ถ้ามีศัตรูใกล้และอยู่ในมุมกล้อง ให้หันไปหาตอนออกท่า
    float SoftAim(GameClient c, float camAngle)
    {
        var me = c.PlayerRenderPos(Session.Now);
        SoftTarget = -1;
        float best = float.MaxValue, aim = camAngle;
        foreach (var e in c.Enemies(Session.Now))
        {
            if (e.V.Phase == EPhase.Dead) continue;
            var to = e.Pos - me;
            float dist = to.Length() - e.V.Radius;
            if (dist > 6f) continue;
            float ang = MathF.Atan2(to.Y, to.X);
            float diff = Mathf.Abs(Mathf.AngleDifference(camAngle, ang));
            if (diff > 1.25f) continue;
            float score = dist + diff * 2.5f;
            if (score >= best) continue;
            best = score;
            aim = ang;
            SoftTarget = e.V.Id;
        }
        return aim;
    }

    void Drain()
    {
        var c = Session.Client;
        var me = Art.V3(c.PlayerRenderPos(Session.Now));
        while (c.TryDequeue(out var ui))
        {
            _auto?.OnUi(ui);
            switch (ui.Kind)
            {
                case UiKind.Judgment:
                    if (ui.Local == Judgment.Miss) break;
                    bool perfect = ui.Local is Judgment.PerfectDodge or Judgment.PerfectGuard;
                    Float(Judging.Th(ui.Local) + (perfect ? "!" : ""), me + Vector3.Up * 2.3f, Palette.Judgment(ui.Local), perfect ? 34 : 28, 1.1);
                    if (perfect)
                    {
                        PerfectAt = T;
                        _slowUntil = T + 0.35;
                    }
                    break;
                case UiKind.Verdict:
                    if (!Shared.DescribeVerdict(ui, out var line, out var floatText, T)) break;
                    Shared.Push(Feed, line);
                    if (floatText != null) Float(floatText, me + Vector3.Up * 1.0f, Palette.Hurt, 18, 1.6);
                    break;
                case UiKind.LocalHit:
                    Flash(EnemyPoint(ui.EnemyId, ui.Pos));
                    _shake = Mathf.Max(_shake, 0.06f);
                    break;
                case UiKind.EnemyDamage:
                    Float(ui.Amount.ToString(), EnemyPoint(ui.EnemyId, ui.Pos) + new Vector3(GD.Randf() * 0.6f - 0.3f, 0.6f, 0),
                        Palette.Damage, 24, 0.8);
                    break;
                case UiKind.PlayerDamage:
                    Float($"-{ui.Amount}", me + new Vector3(0.4f, 1.8f, 0), Palette.Hurt, 26, 0.9);
                    _shake = Mathf.Max(_shake, 0.2f);
                    break;
                case UiKind.Stagger:
                    Float("เสียหลัก! กด F", EnemyPoint(ui.EnemyId, ui.Pos) + Vector3.Up * 1.2f, Palette.Gold, 26, 1.4);
                    break;
                case UiKind.Kill:
                    Float("ล้ม", EnemyPoint(ui.EnemyId, ui.Pos) + Vector3.Up * 0.8f, Palette.Muted, 22, 1.0);
                    break;
                case UiKind.Banner:
                    ShowBanner(ui.Text, ui.Text.Contains("กด R") ? double.MaxValue : 3);
                    break;
            }
        }
    }

    void Float(string text, Vector3 at, Color col, int size, double life) =>
        Floaters.Add(new Floater3D { Text = text, Pos = at, Color = col, Size = size, Born = T, Life = life });

    // ศัตรูบนจอแสดงย้อนหลัง ส่วนตัวเราแสดงล่วงหน้า เดินชนกันแล้วตัวเราจะจมเข้าไปในศัตรูบนจอ
    // จึงดันโมเดลศัตรูออกเฉพาะภาพ ตำแหน่งที่ใช้ตัดสินยังเป็นของ server เหมือนเดิม (ดูได้ด้วยปุ่ม G)
    public System.Numerics.Vector2 VisualPos(in EnemyRender e)
    {
        if (e.V.Phase == EPhase.Dead) return e.Pos;
        var me = Session.Client.PlayerRenderPos(Session.Now);
        var d = e.Pos - me;
        float min = e.V.Radius + Cfg.PlayerRadius + 0.1f;
        float len = d.Length();
        return len >= min || len < 0.0001f ? e.Pos : me + d / len * min;
    }

    Vector3 EnemyPoint(int id, System.Numerics.Vector2 fallback)
    {
        foreach (var e in Session.Client.Enemies(Session.Now))
            if (e.V.Id == id) return Art.V3(VisualPos(e), e.V.Kind == EKind.Boss ? 2.6f : 1.0f);
        return Art.V3(fallback, 1.0f);
    }

    void Flash(Vector3 at)
    {
        var mat = Art.Clear(new Color(1f, 0.95f, 0.8f, 0.9f));
        var mesh = Art.Part(this, Art.Sphere(0.5f), mat, at, shadow: false);
        _flashes.Add((mesh, mat, T));
    }

    void SyncWorld(float dt)
    {
        var s = Session;
        var c = s.Client;
        double now = s.Now;
        float alpha = c.TickAlpha(now);

        _player.Sync(c.Predicted, Art.V3(c.PlayerRenderPos(now)), c.ClientTick, alpha, T, dt);

        _ground.Begin();
        foreach (var e in c.Enemies(now))
        {
            if (!_enemies.TryGetValue(e.V.Id, out var node))
            {
                node = e.V.Kind == EKind.Boss ? new BossModel() : new WolfModel();
                _actors.AddChild(node);
                _enemies[e.V.Id] = node;
            }
            var shown = e;
            shown.Pos = VisualPos(e);
            if (node is WolfModel w) w.Sync(shown, T, dt);
            else if (node is BossModel b) b.Sync(shown, T, dt);
            Telegraph3D(e);
        }

        // แนวฟันของผู้เล่นตอนท่าอยู่ในจังหวะโดน
        var st = c.Predicted;
        if (PlayerSim.ActiveHit(st, out var def, out _))
        {
            var col = st.Action == PAction.Finisher ? Palette.A(Palette.Gold, 0.5f) : new Color(0.85f, 0.95f, 1f, 0.32f);
            float half = def.ArcDeg * 0.5f * Mathf.Pi / 180f;
            _ground.Slash(Art.V3(c.PlayerRenderPos(now)), def.Radius, st.Facing, half, col, 1.05f);
        }
        _ground.End();

        foreach (var cr in _crystals)
            cr.Rotation = new Vector3(0, (float)T * 0.6f, 0);

        for (int i = _flashes.Count - 1; i >= 0; i--)
        {
            var (mesh, mat, born) = _flashes[i];
            float k = (float)((T - born) / 0.16);
            if (k >= 1)
            {
                mesh.QueueFree();
                _flashes.RemoveAt(i);
                continue;
            }
            mesh.Scale = Vector3.One * (0.3f + 1.2f * k);
            mat.AlbedoColor = new Color(1f, 0.95f, 0.8f, 0.9f * (1 - k));
        }
        Floaters.RemoveAll(f => T - f.Born > f.Life);
        SyncGhosts();
    }

    void Telegraph3D(in EnemyRender e)
    {
        var v = e.V;
        if (v.Attack == AttackId.None || (v.Phase != EPhase.Windup && v.Phase != EPhase.Active)) return;
        var adef = Attacks.Get(v.Attack);
        var col = Palette.Tele(adef.Tele);
        var shape = Telegraph.Preview(e);
        if (v.Phase == EPhase.Active)
        {
            _ground.Shape(shape, 1f, Palette.A(col, 0.55f), 0.035f);
            _ground.ShapeOutline(shape, Palette.A(col, 1f), 0.12f, 0.045f);
            return;
        }
        float p = Telegraph.Progress(e);
        _ground.Shape(shape, 1f, Palette.A(col, 0.12f), 0.03f);
        _ground.Shape(shape, p, Palette.A(col, 0.25f + 0.25f * p), 0.035f);
        _ground.ShapeOutline(shape, Palette.A(col, 0.55f + 0.45f * p), p >= 1 ? 0.14f : 0.09f, 0.045f);
    }

    void SyncGhosts()
    {
        foreach (var g in _ghosts.Values) g.Visible = ShowServer;
        if (!ShowServer) return;
        var w = Session.Server.World;
        Ghost(0, Art.V3(w.Player.Pos, 0.9f), 0.45f, 1.8f);
        foreach (var e in w.Enemies)
        {
            if (!e.Alive)
            {
                if (_ghosts.TryGetValue(e.Id, out var dead)) dead.Visible = false;
                continue;
            }
            Ghost(e.Id, Art.V3(e.Pos, e.Kind == EKind.Boss ? 2.2f : 0.6f), e.Radius, e.Kind == EKind.Boss ? 4.4f : 1.2f);
        }
    }

    void Ghost(int id, Vector3 at, float r, float h)
    {
        if (!_ghosts.TryGetValue(id, out var g))
        {
            g = Art.Part(this, Art.Capsule(r, h), Art.Clear(Palette.A(Palette.Ghost, 0.28f)), at, shadow: false);
            _ghosts[id] = g;
        }
        g.Position = at;
        g.Visible = true;
    }

    void UpdateCamera(float dt)
    {
        var c = Session.Client;
        var me = Art.V3(c.PlayerRenderPos(Session.Now), 1.3f);
        if (BotPlaying)
        {
            // บอทเล่นอยู่: กล้องอยู่หลังตัวละครและหันไปทางศัตรูที่ใกล้ที่สุด (บอสมาก่อน)
            // จะได้เห็นทั้งตัวละครและท่าของศัตรู แม้ตัวละครจะหลบไปทางอื่น
            var mePos = c.PlayerRenderPos(Session.Now);
            float aim = c.Predicted.Facing, best = float.MaxValue;
            bool boss = false;
            foreach (var e in c.Enemies(Session.Now))
            {
                if (e.V.Phase == EPhase.Dead) continue;
                var d = e.Pos - mePos;
                float score = d.Length() - (e.V.Kind == EKind.Boss ? 100 : 0);
                if (score >= best || d.Length() < 0.4f) continue;
                best = score;
                aim = MathF.Atan2(d.Y, d.X);
                boss = e.V.Kind == EKind.Boss;
            }
            _yaw = Mathf.LerpAngle(_yaw, Art.Yaw(aim), Art.Smooth(1.6f, dt));
            _pitch = Mathf.Lerp(_pitch, boss ? -0.6f : -0.42f, Art.Smooth(2, dt));
            _dist = Mathf.Lerp(_dist, boss ? 9.5f : 7.5f, Art.Smooth(2, dt));
        }
        _camTarget = _camTarget.Lerp(me, Art.Smooth(12, dt));
        var rot = Basis.FromEuler(new Vector3(_pitch, _yaw, 0));
        var pos = _camTarget + rot * new Vector3(0, 0, _dist);
        if (pos.Y < 0.4f) pos.Y = 0.4f;
        _shake = Mathf.Max(0, _shake - dt * 0.9f);
        var jitter = new Vector3(GD.Randf() - 0.5f, GD.Randf() - 0.5f, GD.Randf() - 0.5f) * _shake;
        Camera.Position = pos + jitter;
        Camera.LookAt(_camTarget + jitter * 0.5f, Vector3.Up);
    }
}
