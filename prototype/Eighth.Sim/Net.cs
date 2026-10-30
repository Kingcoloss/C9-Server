using System.Numerics;

namespace Eighth.Sim;

// ท่อส่งข้อมูลจำลองในเครื่องเดียว: หน่วงตามเวลาที่ตั้ง ไม่มีการสูญหาย
public sealed class DelayLink<T>
{
    struct Item
    {
        public double At;
        public long Seq;
        public T Value;
    }

    readonly List<Item> _q = new();
    readonly List<Item> _due = new();
    long _seq;

    public int InFlight => _q.Count;

    public void Send(double now, double delay, T value) =>
        _q.Add(new Item { At = now + delay, Seq = _seq++, Value = value });

    public void Deliver(double now, Action<T> sink)
    {
        if (_q.Count == 0) return;
        _due.Clear();
        for (int i = _q.Count - 1; i >= 0; i--)
        {
            if (_q[i].At > now) continue;
            _due.Add(_q[i]);
            _q.RemoveAt(i);
        }
        if (_due.Count == 0) return;
        _due.Sort((a, b) => a.At != b.At ? a.At.CompareTo(b.At) : a.Seq.CompareTo(b.Seq));
        foreach (var it in _due) sink(it.Value);
    }
}

// server: รับปุ่มตาม tick แล้วเดินโลก ส่ง snapshot กลับทุก tick
public sealed class GameServer
{
    public readonly World World;
    public int LateInputs { get; private set; }
    public int MissingInputs { get; private set; }

    readonly Dictionary<int, InputCmd> _inputs = new();
    InputCmd _last;

    public GameServer(uint seed = 7, int wolves = 3) => World = new World(seed, wolves);

    // แต่ละแพ็กเก็ตมีปุ่มของ tick ล่าสุดและ 3 tick ก่อนหน้า (กันแพ็กเก็ตหาย)
    public void Receive(InputCmd[] cmds)
    {
        if (cmds.Length == 0) return;
        if (cmds[^1].Tick <= World.Tick) LateInputs++;
        foreach (var c in cmds)
            if (c.Tick > World.Tick) _inputs[c.Tick] = c;
    }

    public Snapshot Step()
    {
        int t = World.Tick + 1;
        if (_inputs.Remove(t, out var cmd))
        {
            _last = cmd;
        }
        else
        {
            // ไม่มีปุ่มของ tick นี้: ใช้ทิศและการ์ดเดิม แต่ไม่กดซ้ำ
            cmd = _last;
            cmd.Tick = t;
            cmd.Press = Press.None;
            if (t > 1) MissingInputs++;
        }
        World.Step(cmd);
        return World.MakeSnapshot(t);
    }
}

public struct HeldInput
{
    public float MoveX, MoveY, Aim;
    public bool Guard;
}

public enum UiKind : byte
{
    Judgment,       // ผลที่เครื่องผู้เล่นทายไว้ทันทีที่เห็นท่าลง
    Verdict,        // ผลจริงจาก server เทียบกับที่ทาย
    LocalHit,       // เครื่องผู้เล่นเห็นว่าตีโดน (รอ server ยืนยัน)
    EnemyDamage,
    PlayerDamage,
    Stagger,
    Kill,
    Banner,
}

public struct UiEvent
{
    public UiKind Kind;
    public Vector2 Pos;
    public int EnemyId;
    public int Amount;
    public Judgment Local;
    public Judgment Server;
    public bool Mismatch;
    public bool Compensated;
    public int WaitTicks;     // server รอ input ที่ตรงกับภาพกี่ tick
    public string Text;
}

public struct EnemyRender
{
    public EnemyView V;       // สถานะแบบไม่ต่อเนื่องจาก snapshot ที่กำลังแสดง
    public Vector2 Pos;       // ตำแหน่งที่ interpolate แล้ว
    public float Facing;
    public float Alpha;
}

// client: ทำนายตัวเอง แสดงศัตรูย้อนหลัง 2 tick และเทียบผลกับ server
public sealed class GameClient : IFinishQuery
{
    const int Ring = 128;
    const int Redundancy = 4;

    public PlayerState Predicted;
    public HeldInput Held;
    public int ClientTick { get; private set; }
    public int LatestSnapshotTick => _latestTick;
    public Snapshot Latest => _snaps[^1];

    public int Corrections { get; private set; }
    public int LocalJudged { get; private set; }
    public int ServerJudged { get; private set; }
    public int Mismatches { get; private set; }
    public int LastWaitTicks { get; private set; }

    readonly Queue<UiEvent> _ui = new();
    readonly List<Snapshot> _snaps = new();
    readonly List<GameEvent> _evQueue = new();
    readonly Dictionary<(int enemy, int serial), JudgeRec> _judg = new();
    readonly HashSet<(int serial, int hit, int enemy)> _localLanded = new();
    readonly InputCmd[] _inHist = new InputCmd[Ring];
    readonly int[] _inTick = new int[Ring];
    readonly PlayerState[] _stHist = new PlayerState[Ring];
    readonly int[] _stTick = new int[Ring];
    readonly List<InputCmd> _send = new(Redundancy);

    Press _pendingPress;
    int _latestTick = -1;
    double _latestAt;
    double _lastTickAt;
    double _lastUpdateAt;
    Vector2 _prevPos;
    Vector2 _errOffset;

    sealed class JudgeRec
    {
        public int AttackTick;
        public AttackId Attack;
        public AttackShape Shape;
        public bool LocalDone, ServerDone;
        public Judgment Local, Server;
        public bool Compensated;
        public int WaitTicks;
        public int SeenAt;
    }

    public GameClient(Snapshot welcome, double now = 0)
    {
        Array.Fill(_inTick, -1);
        Array.Fill(_stTick, -1);
        Predicted = welcome.Player;
        _prevPos = Predicted.Pos;
        ClientTick = welcome.Tick;
        _snaps.Add(welcome);
        _latestTick = welcome.Tick;
        _latestAt = now;
        _lastTickAt = now;
        _lastUpdateAt = now;
        _stHist[ClientTick % Ring] = Predicted;
        _stTick[ClientTick % Ring] = ClientTick;
    }

    public void QueuePress(Press p) => _pendingPress |= p;

    public bool TryDequeue(out UiEvent e) => _ui.TryDequeue(out e);

    // tick (ทศนิยม) ของโลกที่ผู้เล่นกำลังเห็นอยู่
    public float RenderTick(double now)
    {
        float frac = (float)Math.Clamp((now - _latestAt) * Cfg.TickRate, 0, 1);
        return _latestTick + frac - Cfg.InterpTicks;
    }

    public int ViewTick(double now) => Math.Max(0, (int)MathF.Floor(RenderTick(now)));

    // ---- รับ snapshot ----

    public void Receive(Snapshot s, double now)
    {
        if (s.Tick <= _latestTick) return;
        _snaps.Add(s);
        if (_snaps.Count > 48) _snaps.RemoveAt(0);
        _latestTick = s.Tick;
        _latestAt = now;
        _evQueue.AddRange(s.Events);

        foreach (var v in s.Enemies)
        {
            if (v.Phase != EPhase.Active) continue;
            var key = (v.Id, v.AttackSerial);
            if (!_judg.TryGetValue(key, out var rec))
                _judg[key] = rec = new JudgeRec { SeenAt = s.Tick };
            if (rec.Attack == AttackId.None)
            {
                rec.AttackTick = s.Tick - v.PhaseTick;
                rec.Attack = v.Attack;
                rec.Shape = AttackShape.Of(v);
            }
        }

        Reconcile(s);
    }

    // เอาสถานะจริงจาก server มาตั้งต้น แล้วจำลองปุ่มที่ยังไม่ได้รับการยืนยันซ้ำ
    void Reconcile(Snapshot s)
    {
        int t = s.Tick;
        var before = Predicted.Pos;
        if (t >= ClientTick)
        {
            Predicted = s.Player;
            ClientTick = t;
            Store(t, Predicted);
            return;
        }
        int i = t % Ring;
        if (_stTick[i] == t && !_stHist[i].NearlyEquals(s.Player)) Corrections++;

        var st = s.Player;
        Store(t, st);
        for (int k = t + 1; k <= ClientTick; k++)
        {
            int j = k % Ring;
            if (_inTick[j] != k) break;
            PlayerSim.Step(ref st, _inHist[j], k, this);
            Store(k, st);
        }
        Predicted = st;

        var err = before - Predicted.Pos;
        if (err.LengthSquared() < 1e-8f) return;
        if (err.LengthSquared() > 9f)
        {
            _errOffset = Vector2.Zero;
            _prevPos = Predicted.Pos;
            return;
        }
        _errOffset += err;
        _prevPos -= err;
    }

    void Store(int tick, in PlayerState st)
    {
        int i = tick % Ring;
        _stHist[i] = st;
        _stTick[i] = tick;
    }

    // ---- tick ของ client ----

    public void TickTo(int target, double now, Action<InputCmd[]> send)
    {
        if (ClientTick >= target) return;
        int view = ViewTick(now);
        while (ClientTick < target)
        {
            ClientTick++;
            var cmd = new InputCmd
            {
                Tick = ClientTick,
                MoveX = Held.MoveX,
                MoveY = Held.MoveY,
                Aim = Held.Aim,
                Guard = Held.Guard,
                Press = _pendingPress,
                ViewTick = view,
            };
            _pendingPress = Press.None;

            _prevPos = Predicted.Pos;
            PlayerSim.Step(ref Predicted, cmd, ClientTick, this);
            int i = ClientTick % Ring;
            _inHist[i] = cmd;
            _inTick[i] = ClientTick;
            Store(ClientTick, Predicted);

            _send.Clear();
            for (int k = Math.Max(1, ClientTick - Redundancy + 1); k <= ClientTick; k++)
                if (_inTick[k % Ring] == k) _send.Add(_inHist[k % Ring]);
            send(_send.ToArray());

            JudgeLocally(view, now);
            PredictHits(now);
        }
        _lastTickAt = now;
    }

    // เมื่อภาพของผู้เล่นเห็นท่าลงแล้ว ทายผลด้วยกติกาเดียวกับ server
    readonly List<(int, int)> _ready = new();

    void JudgeLocally(int view, double now)
    {
        _ready.Clear();
        foreach (var (key, rec) in _judg)
        {
            if (rec.LocalDone || rec.Attack == AttackId.None || view < rec.AttackTick) continue;
            rec.LocalDone = true;
            rec.Local = Judging.Judge(Predicted, ClientTick, rec.Shape, Attacks.Get(rec.Attack));
            if (rec.Local == Judgment.None) continue;
            LocalJudged++;
            _ui.Enqueue(new UiEvent
            {
                Kind = UiKind.Judgment, Pos = Predicted.Pos, EnemyId = key.enemy, Local = rec.Local,
            });
            if (rec.ServerDone) _ready.Add(key);
        }
        foreach (var key in _ready) Verdict(key, _judg[key]);
    }

    void PredictHits(double now)
    {
        if (!PlayerSim.ActiveHit(Predicted, out var def, out int hit)) return;
        float half = def.ArcDeg * 0.5f * MathF.PI / 180f;
        foreach (var e in Enemies(now))
        {
            if (e.V.Phase == EPhase.Dead) continue;
            var key = (Predicted.ActionSerial, hit, e.V.Id);
            if (_localLanded.Contains(key)) continue;
            if (!Geo.InSector(Predicted.Pos, Predicted.Facing, def.Radius, half, e.Pos, e.V.Radius)) continue;
            _localLanded.Add(key);
            _ui.Enqueue(new UiEvent { Kind = UiKind.LocalHit, Pos = e.Pos, EnemyId = e.V.Id });
        }
        if (_localLanded.Count > 512) _localLanded.Clear();
    }

    void Verdict((int enemy, int serial) key, JudgeRec rec)
    {
        bool mismatch = rec.Local != rec.Server;
        if (mismatch) Mismatches++;
        _ui.Enqueue(new UiEvent
        {
            Kind = UiKind.Verdict, Pos = Predicted.Pos, EnemyId = key.enemy,
            Local = rec.Local, Server = rec.Server, Mismatch = mismatch,
            Compensated = rec.Compensated, WaitTicks = rec.WaitTicks,
        });
        _judg.Remove(key);
    }

    // ---- ต่อเฟรม: ปล่อยเหตุการณ์ตามเวลาที่ผู้เล่นเห็น ----

    public void Update(double now)
    {
        float dt = (float)Math.Max(0, now - _lastUpdateAt);
        _lastUpdateAt = now;
        _errOffset *= MathF.Exp(-12f * dt);

        float rt = RenderTick(now);
        for (int i = 0; i < _evQueue.Count; i++)
        {
            var ev = _evQueue[i];
            if (ev.Tick > rt) continue;
            _evQueue.RemoveAt(i--);
            Release(ev, now);
        }

        // ล้างการตัดสินที่ค้าง (เช่นผู้เล่นตายก่อน)
        if (_judg.Count > 64)
        {
            var stale = _judg.Where(kv => kv.Value.SeenAt < _latestTick - 300).Select(kv => kv.Key).ToList();
            foreach (var k in stale) _judg.Remove(k);
        }
    }

    void Release(in GameEvent ev, double now)
    {
        switch (ev.Kind)
        {
            case EvKind.Defense:
            {
                var key = (ev.EnemyId, ev.AttackSerial);
                if (!_judg.TryGetValue(key, out var rec)) _judg[key] = rec = new JudgeRec { SeenAt = ev.Tick };
                rec.ServerDone = true;
                rec.Server = ev.Judgment;
                rec.Compensated = ev.Compensated;
                rec.WaitTicks = ev.Tick - ev.AttackTick;
                LastWaitTicks = rec.WaitTicks;
                ServerJudged++;
                if (ev.Amount > 0)
                    _ui.Enqueue(new UiEvent { Kind = UiKind.PlayerDamage, Pos = Predicted.Pos, Amount = ev.Amount });
                if (rec.LocalDone) Verdict(key, rec);
                break;
            }
            case EvKind.EnemyDamaged:
                _ui.Enqueue(new UiEvent { Kind = UiKind.EnemyDamage, Pos = EnemyPos(ev.EnemyId, ev.Pos, now), EnemyId = ev.EnemyId, Amount = ev.Amount });
                break;
            case EvKind.EnemyStaggered:
                _ui.Enqueue(new UiEvent { Kind = UiKind.Stagger, Pos = EnemyPos(ev.EnemyId, ev.Pos, now), EnemyId = ev.EnemyId });
                break;
            case EvKind.EnemyKilled:
                _ui.Enqueue(new UiEvent { Kind = UiKind.Kill, Pos = ev.Pos, EnemyId = ev.EnemyId });
                break;
            case EvKind.BossSpawned:
                _ui.Enqueue(new UiEvent { Kind = UiKind.Banner, Text = "ราชาหมึก ปรากฏตัว", Pos = ev.Pos });
                break;
            case EvKind.Victory:
                _ui.Enqueue(new UiEvent { Kind = UiKind.Banner, Text = "ชนะ! กด R เพื่อเล่นใหม่", Pos = ev.Pos });
                break;
            case EvKind.Defeat:
                _ui.Enqueue(new UiEvent { Kind = UiKind.Banner, Text = "พ่ายแพ้ กด R เพื่อเล่นใหม่", Pos = ev.Pos });
                break;
        }
    }

    Vector2 EnemyPos(int id, Vector2 fallback, double now)
    {
        foreach (var e in Enemies(now))
            if (e.V.Id == id) return e.Pos;
        return fallback;
    }

    // ---- ข้อมูลสำหรับวาดภาพ ----

    public Vector2 PlayerRenderPos(double now)
    {
        float a = (float)Math.Clamp((now - _lastTickAt) * Cfg.TickRate, 0, 1);
        return Vector2.Lerp(_prevPos, Predicted.Pos, a) + _errOffset;
    }

    readonly List<EnemyRender> _render = new();

    public List<EnemyRender> Enemies(double now)
    {
        _render.Clear();
        float rt = RenderTick(now);
        int baseTick = (int)MathF.Floor(rt);
        int ai = 0;
        for (int i = _snaps.Count - 1; i >= 0; i--)
        {
            if (_snaps[i].Tick <= baseTick)
            {
                ai = i;
                break;
            }
        }
        var a = _snaps[ai];
        var b = ai + 1 < _snaps.Count ? _snaps[ai + 1] : a;
        float alpha = b.Tick > a.Tick ? Math.Clamp((rt - a.Tick) / (b.Tick - a.Tick), 0f, 1f) : 0f;

        foreach (var v in a.Enemies)
        {
            var pos = v.Pos;
            var facing = v.Facing;
            foreach (var w in b.Enemies)
            {
                if (w.Id != v.Id) continue;
                pos = Vector2.Lerp(v.Pos, w.Pos, alpha);
                facing = v.Facing + Geo.Wrap(w.Facing - v.Facing) * alpha;
                break;
            }
            _render.Add(new EnemyRender { V = v, Pos = pos, Facing = facing, Alpha = alpha });
        }
        return _render;
    }

    public bool TryFinisherTarget(Vector2 pos, out int enemyId)
    {
        enemyId = -1;
        float best = float.MaxValue;
        foreach (var e in Latest.Enemies)
        {
            if (e.Phase != EPhase.Staggered) continue;
            float d = Vector2.Distance(pos, e.Pos) - e.Radius;
            if (d <= Cfg.FinisherRange && d < best)
            {
                best = d;
                enemyId = e.Id;
            }
        }
        return enemyId >= 0;
    }
}

// รวม server กับ client ไว้ในโปรแกรมเดียว แล้วหน่วงข้อความระหว่างกันตาม ping ที่ตั้ง
public sealed class LocalSession
{
    public const double SubStep = 1.0 / 240;

    public GameServer Server { get; }
    public GameClient Client { get; }
    public double Now { get; private set; }
    public int PingMs { get; set; }

    public bool LagCompensation
    {
        get => Server.World.LagCompensation;
        set => Server.World.LagCompensation = value;
    }

    public double OneWay => PingMs / 2000.0;

    // client เดินนำ server กี่ tick เพื่อให้ปุ่มไปถึงทันพอดี
    public int Lead => (int)Math.Ceiling((OneWay + SubStep) * Cfg.TickRate) + 1;

    readonly DelayLink<InputCmd[]> _up = new();
    readonly DelayLink<Snapshot> _down = new();
    double _nextTickAt = Cfg.Dt;

    public LocalSession(int pingMs = 80, bool lagCompensation = true, uint seed = 7, int wolves = 3)
    {
        PingMs = pingMs;
        Server = new GameServer(seed, wolves);
        LagCompensation = lagCompensation;
        Client = new GameClient(Server.World.MakeSnapshot(0));
    }

    public void Advance(double dt)
    {
        double end = Now + Math.Clamp(dt, 0, 0.25);
        while (Now < end - 1e-9)
        {
            Now = Math.Min(end, Now + SubStep);
            _up.Deliver(Now, Server.Receive);
            _down.Deliver(Now, s => Client.Receive(s, Now));
            while (Now >= _nextTickAt - 1e-9)
            {
                _nextTickAt += Cfg.Dt;
                var snap = Server.Step();
                _down.Send(Now, OneWay, snap);
                Client.TickTo(Server.World.Tick + Lead, Now, p => _up.Send(Now, OneWay, p));
            }
        }
        Client.Update(Now);
    }
}

// รูปร่างและความคืบหน้าของสัญญาณเตือนท่า สำหรับวาดบนจอ
public static class Telegraph
{
    public static AttackShape Preview(in EnemyRender e)
    {
        var v = e.V;
        if (v.Phase == EPhase.Active) return AttackShape.Of(v);
        var def = Attacks.Get(v.Attack);
        return new AttackShape
        {
            Kind = def.Shape,
            Radius = def.Radius,
            Facing = e.Facing,
            HalfArc = def.HalfArcDeg * MathF.PI / 180f,
            Center = def.Shape switch
            {
                ShapeKind.CircleFront => e.Pos + Geo.Dir(e.Facing) * (def.Offset + def.Lunge),
                ShapeKind.CircleAtTarget => v.Target,
                _ => e.Pos,
            },
        };
    }

    // 0..1 ของช่วงง้าง ท่าหลอกจะเต็มเร็วเท่าท่าจริงแล้วค้างไว้
    public static float Progress(in EnemyRender e)
    {
        var v = e.V;
        if (v.Phase != EPhase.Windup || v.Attack == AttackId.None) return 0f;
        var def = Attacks.Get(v.Attack);
        float shown = def.Windup - def.FeintHold;
        return Math.Clamp((v.PhaseTick + e.Alpha) / shown, 0f, 1f);
    }
}
