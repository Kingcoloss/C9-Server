using System.Numerics;

namespace Eighth.Sim;

public struct Rng
{
    uint _s;
    public Rng(uint seed) => _s = seed == 0 ? 0x9E3779B9u : seed;

    public uint Next()
    {
        _s ^= _s << 13;
        _s ^= _s >> 17;
        _s ^= _s << 5;
        return _s;
    }

    public int Range(int min, int maxExclusive) => min + (int)(Next() % (uint)(maxExclusive - min));
}

// โลกฝั่ง server: ตัดสินทุกอย่าง client ไม่เคยแก้ค่าในนี้ตรงๆ
public sealed class World : IFinishQuery
{
    public int Tick { get; private set; }
    public PlayerState Player;
    public readonly List<EnemyState> Enemies = new();
    public bool LagCompensation = true;
    public bool BossSpawned { get; private set; }
    public bool Over { get; private set; }

    readonly List<GameEvent> _events = new();
    readonly List<Pending> _pending = new();
    readonly HashSet<(int serial, int hit, int enemy)> _landed = new();
    readonly PlayerState[] _pHist = new PlayerState[Cfg.HistoryTicks];
    readonly int[] _pHistView = new int[Cfg.HistoryTicks];
    readonly int[] _pHistTick = new int[Cfg.HistoryTicks];
    readonly Dictionary<int, Vector2[]> _eHist = new();
    Rng _rng;
    int _nextId = 1;
    int _bossTimer = -1;
    int _viewTick;

    struct Pending
    {
        public int EnemyId, Serial, AttackTick;
        public AttackId Attack;
        public AttackShape Shape;
    }

    static readonly Vector2[] WolfSpawns = { new(0, -6), new(-5, -3), new(5, -3) };

    public World(uint seed = 7, int wolves = 3)
    {
        _rng = new Rng(seed);
        Player = PlayerState.Spawn(new Vector2(0, 7));
        for (int i = 0; i < Math.Min(wolves, WolfSpawns.Length); i++) SpawnWolf(WolfSpawns[i]);
        Array.Fill(_pHistTick, -1);
        Record(0);
    }

    public EnemyState SpawnWolf(Vector2 pos) => Add(new EnemyState
    {
        Kind = EKind.Wolf, NameTh = "หมาป่าหนาม", Pos = pos, Radius = 0.55f, Speed = 4.2f,
        Hp = 320, MaxHp = 320, Poise = 40, MaxPoise = 40, Facing = MathF.PI / 2,
    });

    public EnemyState SpawnBoss(Vector2 pos) => Add(new EnemyState
    {
        Kind = EKind.Boss, NameTh = "ราชาหมึก", Pos = pos, Radius = 1.2f, Speed = 2.6f,
        Hp = 5000, MaxHp = 5000, Poise = 300, MaxPoise = 300, Facing = MathF.PI / 2, Cooldown = 30,
    });

    EnemyState Add(EnemyState e)
    {
        e.Id = _nextId++;
        e.Phase = EPhase.Idle;
        Enemies.Add(e);
        var h = new Vector2[Cfg.HistoryTicks];
        Array.Fill(h, e.Pos);
        _eHist[e.Id] = h;
        return e;
    }

    public List<GameEvent> Step(in InputCmd cmd)
    {
        Tick++;
        _events.Clear();
        _viewTick = cmd.ViewTick;

        PlayerSim.Step(ref Player, cmd, Tick, this);
        Record(cmd.ViewTick);
        PlayerAttacks();

        foreach (var e in Enemies)
            if (e.Alive) StepEnemy(e);
        foreach (var e in Enemies)
        {
            _eHist[e.Id][Tick % Cfg.HistoryTicks] = e.Pos;
            if (e.Alive && e.Phase == EPhase.Active && e.PhaseTick == 0)
                _pending.Add(new Pending
                {
                    EnemyId = e.Id, Serial = e.AttackSerial, AttackTick = Tick,
                    Attack = e.Attack, Shape = AttackShape.Of(e),
                });
        }

        ResolvePending();
        Waves();
        return _events;
    }

    void Record(int viewTick)
    {
        int i = Tick % Cfg.HistoryTicks;
        _pHist[i] = Player;
        _pHistView[i] = viewTick;
        _pHistTick[i] = Tick;
    }

    public bool TryFinisherTarget(Vector2 pos, out int enemyId)
    {
        enemyId = -1;
        float best = float.MaxValue;
        foreach (var e in Enemies)
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

    // ---- ผู้เล่นตีศัตรู: ย้อนตำแหน่งศัตรูไปตามภาพที่ผู้เล่นเห็น ----

    void PlayerAttacks()
    {
        if (!PlayerSim.ActiveHit(Player, out var def, out int hit)) return;
        int at = LagCompensation ? Math.Max(_viewTick, Tick - Cfg.AttackRewindCapTicks) : Tick;
        float half = def.ArcDeg * 0.5f * MathF.PI / 180f;
        foreach (var e in Enemies)
        {
            if (!e.Alive) continue;
            var key = (Player.ActionSerial, hit, e.Id);
            if (_landed.Contains(key)) continue;
            var pos = PosAt(e, at);
            if (!Geo.InSector(Player.Pos, Player.Facing, def.Radius, half, pos, e.Radius)) continue;
            _landed.Add(key);
            DamageEnemy(e, def);
        }
        if (_landed.Count > 512) _landed.Clear();
    }

    Vector2 PosAt(EnemyState e, int tick)
    {
        if (tick >= Tick || tick <= Tick - Cfg.HistoryTicks || tick < 0) return e.Pos;
        return _eHist[e.Id][tick % Cfg.HistoryTicks];
    }

    void DamageEnemy(EnemyState e, in MoveDef def)
    {
        float mul = 1f + Player.Flow * Cfg.FlowDamagePerStack;
        float poiseMul = 1f;
        if (Tick <= Player.CounterUntil)
        {
            mul *= 1.5f;
            poiseMul = 2f;
            Player.CounterUntil = -1;
        }
        if (e.Phase == EPhase.Staggered) mul *= 1.5f;
        int dmg = (int)MathF.Round(def.Damage * mul);
        e.Hp -= dmg;
        Emit(new GameEvent { Kind = EvKind.EnemyDamaged, EnemyId = e.Id, Amount = dmg, Pos = e.Pos });
        if (e.Hp <= 0)
        {
            e.Hp = 0;
            e.Phase = EPhase.Dead;
            Emit(new GameEvent { Kind = EvKind.EnemyKilled, EnemyId = e.Id, Pos = e.Pos });
            return;
        }
        if (e.Phase == EPhase.Staggered)
        {
            // ท่าปิดฉากใช้ได้ครั้งเดียวต่อการเสียหลักหนึ่งครั้ง ศัตรูลุกขึ้นทันทีหลังโดน
            if (Player.Action != PAction.Finisher) return;
            e.Poise = e.MaxPoise;
            SetPhase(e, EPhase.Recover, 20);
            return;
        }
        e.Poise -= def.Poise * poiseMul;
        e.PoiseRegenDelay = 60;
        if (e.Poise <= 0) Stagger(e);
        else if (!e.Armor) SetPhase(e, EPhase.Flinch, 8);
    }

    void Stagger(EnemyState e)
    {
        e.Poise = 0;
        SetPhase(e, EPhase.Staggered, e.Kind == EKind.Boss ? 150 : 90);
        Emit(new GameEvent { Kind = EvKind.EnemyStaggered, EnemyId = e.Id, Pos = e.Pos });
    }

    static void SetPhase(EnemyState e, EPhase phase, int length)
    {
        e.Phase = phase;
        e.PhaseTick = 0;
        e.PhaseLength = length;
    }

    // ---- AI ศัตรู ----

    void StepEnemy(EnemyState e)
    {
        e.PhaseTick++;
        if (e.Cooldown > 0) e.Cooldown--;
        if (e.PoiseRegenDelay > 0) e.PoiseRegenDelay--;
        else if (e.Phase != EPhase.Staggered) e.Poise = MathF.Min(e.MaxPoise, e.Poise + e.MaxPoise * 0.15f * Cfg.Dt);

        var toP = Player.Pos - e.Pos;
        float dist = toP.Length();
        bool playerUp = Player.Action != PAction.Dead;

        switch (e.Phase)
        {
            case EPhase.Idle:
            case EPhase.Chase:
            {
                if (!playerUp) { e.Phase = EPhase.Idle; break; }
                if (dist > 0.01f) e.Facing = Geo.Angle(toP);
                if (TryAttack(e, dist)) break;
                float stop = e.Kind == EKind.Boss ? 2.8f : 1.7f;
                if (dist > stop) e.Pos += Vector2.Normalize(toP) * MathF.Min(e.Speed * Cfg.Dt, dist - stop);
                e.Phase = EPhase.Chase;
                break;
            }
            case EPhase.Windup:
            {
                var def = Attacks.Get(e.Attack);
                if (e.PhaseTick < def.Windup * 2 / 3 && dist > 0.01f && def.Shape != ShapeKind.CircleAtTarget)
                    e.Facing = Geo.Angle(toP);
                if (e.PhaseTick >= def.Windup)
                {
                    SetPhase(e, EPhase.Active, def.Active);
                    var dir = Geo.Dir(e.Facing);
                    e.ShapeFacing = e.Facing;
                    e.ShapeRadius = def.Radius;
                    e.ShapeHalfArc = def.HalfArcDeg * MathF.PI / 180f;
                    e.ShapeCenter = def.Shape switch
                    {
                        ShapeKind.CircleFront => e.Pos + dir * (def.Offset + def.Lunge),
                        ShapeKind.CircleAtTarget => e.Target,
                        _ => e.Pos,
                    };
                }
                break;
            }
            case EPhase.Active:
            {
                var def = Attacks.Get(e.Attack);
                if (def.Lunge > 0) e.Pos += Geo.Dir(e.Facing) * (def.Lunge / def.Active);
                if (e.PhaseTick >= def.Active)
                {
                    SetPhase(e, EPhase.Recover, def.Recover);
                }
                break;
            }
            case EPhase.Recover:
                if (e.PhaseTick >= e.PhaseLength)
                {
                    e.Phase = EPhase.Chase;
                    e.Cooldown = e.Kind == EKind.Boss ? _rng.Range(15, 40) : _rng.Range(40, 80);
                }
                break;
            case EPhase.Flinch:
                if (e.PhaseTick >= e.PhaseLength) e.Phase = EPhase.Chase;
                break;
            case EPhase.Staggered:
                if (e.PhaseTick >= e.PhaseLength)
                {
                    e.Poise = e.MaxPoise;
                    e.Phase = EPhase.Chase;
                    e.Cooldown = 20;
                }
                break;
        }

        Separate(e);
        e.Pos = Geo.ClampToArena(e.Pos, e.Radius);
    }

    bool TryAttack(EnemyState e, float dist)
    {
        if (e.Cooldown > 0) return false;
        if (e.Kind == EKind.Wolf)
        {
            if (dist > 2.7f) return false;
            StartWindup(e, AttackId.WolfLunge);
            return true;
        }
        if (dist < 4.4f)
        {
            int roll = _rng.Range(0, 100);
            var pick = roll < 35 ? AttackId.BossSlam
                : roll < 65 ? AttackId.BossSweep
                : roll < 80 ? AttackId.BossFeint
                : AttackId.BossInkPool;
            StartWindup(e, pick);
            return true;
        }
        if (dist < 11f && _rng.Range(0, 100) < 4)
        {
            StartWindup(e, AttackId.BossInkPool);
            return true;
        }
        return false;
    }

    void StartWindup(EnemyState e, AttackId id)
    {
        var def = Attacks.Get(id);
        SetPhase(e, EPhase.Windup, def.Windup);
        e.Attack = id;
        e.AttackSerial++;
        e.Target = Player.Pos;
    }

    void Separate(EnemyState e)
    {
        foreach (var o in Enemies)
        {
            if (o == e || !o.Alive) continue;
            var d = e.Pos - o.Pos;
            float min = e.Radius + o.Radius;
            float len = d.Length();
            if (len < min && len > 0.0001f) e.Pos += d / len * (min - len) * 0.5f;
        }
        var dp = e.Pos - Player.Pos;
        float pmin = e.Radius + Cfg.PlayerRadius;
        float plen = dp.Length();
        if (plen < pmin && plen > 0.0001f) e.Pos += dp / plen * (pmin - plen);
    }

    // ---- ท่าของศัตรูลงใส่ผู้เล่น ----

    void ResolvePending()
    {
        for (int i = _pending.Count - 1; i >= 0; i--)
        {
            var p = _pending[i];
            int judgeTick;
            if (!LagCompensation)
            {
                judgeTick = p.AttackTick;
            }
            else
            {
                judgeTick = FindViewMatch(p.AttackTick);
                if (judgeTick < 0)
                {
                    if (Tick - p.AttackTick < Cfg.DefenseWaitCapTicks) continue;
                    judgeTick = Tick;
                }
            }
            _pending.RemoveAt(i);
            var state = HistoryAt(judgeTick);
            var def = Attacks.Get(p.Attack);
            var j = Judging.Judge(state, judgeTick, p.Shape, def);
            if (j == Judgment.None) continue;
            int dmg = Apply(j, def, p.EnemyId);
            Emit(new GameEvent
            {
                Kind = EvKind.Defense, EnemyId = p.EnemyId, AttackSerial = p.Serial, Judgment = j,
                Amount = dmg, Pos = Player.Pos, AttackTick = p.AttackTick, JudgedAt = judgeTick,
                Compensated = LagCompensation,
            });
        }
    }

    // tick แรกที่ผู้เล่นเห็นโลกถึง tick ที่ท่าลงแล้ว
    int FindViewMatch(int attackTick)
    {
        for (int t = Math.Max(attackTick, Tick - Cfg.HistoryTicks + 1); t <= Tick; t++)
        {
            int i = t % Cfg.HistoryTicks;
            if (_pHistTick[i] == t && _pHistView[i] >= attackTick) return t;
        }
        return -1;
    }

    PlayerState HistoryAt(int tick)
    {
        int i = tick % Cfg.HistoryTicks;
        return _pHistTick[i] == tick ? _pHist[i] : Player;
    }

    int Apply(Judgment j, in AttackDef def, int enemyId)
    {
        switch (j)
        {
            case Judgment.PerfectDodge:
                Player.CounterUntil = Tick + Cfg.CounterTicks;
                Player.Stamina = MathF.Min(Cfg.StaminaMax, Player.Stamina + 15f);
                return 0;
            case Judgment.PerfectGuard:
            {
                var e = Enemies.Find(x => x.Id == enemyId);
                if (e is { Alive: true } && e.Phase != EPhase.Staggered)
                {
                    e.Poise -= def.PoiseOnParry;
                    e.PoiseRegenDelay = 60;
                    if (e.Poise <= 0) Stagger(e);
                }
                return 0;
            }
            case Judgment.Guard:
            {
                int dmg = (int)(def.Damage * Cfg.GuardDamageFactor);
                Player.Stamina = MathF.Max(0, Player.Stamina - Cfg.GuardStaminaCost);
                Player.StaminaDelay = Cfg.StaminaRegenDelay;
                Hurt(dmg, stun: 0);
                return dmg;
            }
            case Judgment.GuardBreak:
            {
                int dmg = def.Damage / 2;
                Player.Stamina = 0;
                Player.StaminaDelay = Cfg.StaminaRegenDelay;
                Hurt(dmg, Cfg.GuardBreakTicks, PAction.GuardBreak);
                return dmg;
            }
            case Judgment.Hit:
                Player.Flow = Math.Max(0, Player.Flow - 2);
                Hurt(def.Damage, Cfg.HitStunTicks);
                return def.Damage;
            default:
                return 0;
        }
    }

    void Hurt(int dmg, int stun, PAction stunAction = PAction.HitStun)
    {
        if (Player.Action == PAction.Dead) return;
        Player.Hp -= dmg;
        if (Player.Hp <= 0)
        {
            Player.Hp = 0;
            Player.Action = PAction.Dead;
            Player.ActionTick = 0;
            Player.Guarding = false;
            return;
        }
        if (stun <= 0 || Player.SuperArmor || Player.Action == PAction.Finisher) return;
        Player.Action = stunAction;
        Player.ActionTick = 0;
        Player.StunTicks = stun;
        Player.Guarding = false;
        Player.Buffered = Press.None;
    }

    void Waves()
    {
        if (Over) return;
        if (Player.Action == PAction.Dead)
        {
            Over = true;
            Emit(new GameEvent { Kind = EvKind.Defeat, Pos = Player.Pos });
            return;
        }
        bool anyAlive = Enemies.Exists(e => e.Alive);
        if (!BossSpawned && !anyAlive)
        {
            if (_bossTimer < 0) _bossTimer = 45;
            else if (--_bossTimer == 0)
            {
                var boss = SpawnBoss(new Vector2(0, -8));
                BossSpawned = true;
                Emit(new GameEvent { Kind = EvKind.BossSpawned, EnemyId = boss.Id, Pos = boss.Pos });
            }
        }
        else if (BossSpawned && !anyAlive)
        {
            Over = true;
            Emit(new GameEvent { Kind = EvKind.Victory, Pos = Player.Pos });
        }
    }

    void Emit(GameEvent e)
    {
        e.Tick = Tick;
        _events.Add(e);
    }

    public Snapshot MakeSnapshot(int lastInputTick)
    {
        var views = new EnemyView[Enemies.Count];
        for (int i = 0; i < Enemies.Count; i++)
        {
            var e = Enemies[i];
            views[i] = new EnemyView
            {
                Id = e.Id, Kind = e.Kind, Pos = e.Pos, Facing = e.Facing, Radius = e.Radius,
                Hp = e.Hp, MaxHp = e.MaxHp, Poise = e.Poise, MaxPoise = e.MaxPoise,
                Phase = e.Phase, PhaseTick = e.PhaseTick, PhaseLength = e.PhaseLength,
                Attack = e.Attack, AttackSerial = e.AttackSerial, Target = e.Target,
                ShapeCenter = e.ShapeCenter, ShapeRadius = e.ShapeRadius, ShapeFacing = e.ShapeFacing,
                ShapeHalfArc = e.ShapeHalfArc, Armor = e.Armor,
            };
        }
        return new Snapshot
        {
            Tick = Tick,
            LastInputTick = lastInputTick,
            Player = Player,
            Enemies = views,
            Events = _events.ToArray(),
        };
    }
}
