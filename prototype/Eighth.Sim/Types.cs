using System.Numerics;

namespace Eighth.Sim;

[Flags]
public enum Press : byte
{
    None = 0,
    Light = 1,
    Heavy = 2,
    Dodge = 4,
    Skill1 = 8,
    Skill2 = 16,
    Finisher = 32,
}

// สิ่งที่ client ส่งไป server ต่อ tick: ปุ่มเท่านั้น ไม่มีตำแหน่งหรือดาเมจ
public struct InputCmd
{
    public int Tick;
    public float MoveX, MoveY;   // -1..1 บนพื้น (y = ลงจอ)
    public float Aim;            // มุมเล็งเป็นเรเดียน
    public bool Guard;
    public Press Press;
    public int ViewTick;         // tick ของโลกที่ผู้เล่นเห็นอยู่ตอนกด
}

public enum PAction : byte { Idle, Light, Heavy, Skill1, Skill2, Finisher, Dodge, HitStun, GuardBreak, Dead }

// สถานะผู้เล่นทั้งหมดเป็น struct เพื่อย้อนและจำลองซ้ำได้ (rollback)
public struct PlayerState
{
    public Vector2 Pos;
    public float Facing;
    public int Hp;
    public float Stamina;
    public int StaminaDelay;
    public PAction Action;
    public int ActionTick;
    public int ActionSerial;
    public int Combo;
    public int ComboResetTick;
    public bool Guarding;
    public int GuardStartTick;
    public int DodgeStartTick;
    public Vector2 DodgeDir;
    public int Flow;
    public PAction LastFlowKind;
    public int Cd1, Cd2;
    public int CounterUntil;
    public int StunTicks;
    public Press Buffered;
    public int BufferedTick;
    public int FinisherTarget;

    public static PlayerState Spawn(Vector2 pos) => new()
    {
        Pos = pos,
        Facing = -MathF.PI / 2,
        Hp = Cfg.PlayerMaxHp,
        Stamina = Cfg.StaminaMax,
        Action = PAction.Idle,
        GuardStartTick = -1000,
        DodgeStartTick = -1000,
        LastFlowKind = PAction.Idle,
        BufferedTick = -1000,
        CounterUntil = -1,
        FinisherTarget = -1,
    };

    public readonly bool Invulnerable =>
        (Action == PAction.Dodge && ActionTick < Cfg.DodgeIFrames) || Action == PAction.Finisher;

    public readonly bool SuperArmor => Action == PAction.Skill2 && ActionTick < Moves.Skill2.Startup;

    public readonly bool NearlyEquals(in PlayerState o) =>
        Vector2.DistanceSquared(Pos, o.Pos) < 0.0004f &&
        Action == o.Action && ActionTick == o.ActionTick && Hp == o.Hp &&
        MathF.Abs(Stamina - o.Stamina) < 0.5f && Flow == o.Flow && Guarding == o.Guarding;
}

public enum EKind : byte { Wolf, Boss }

public enum EPhase : byte { Idle, Chase, Windup, Active, Recover, Flinch, Staggered, Dead }

public sealed class EnemyState
{
    public int Id;
    public EKind Kind;
    public string NameTh = "";
    public Vector2 Pos;
    public float Facing;
    public float Radius;
    public float Speed;
    public int Hp, MaxHp;
    public float Poise, MaxPoise;
    public int PoiseRegenDelay;
    public EPhase Phase;
    public int PhaseTick;
    public int PhaseLength;
    public AttackId Attack;
    public int AttackSerial;
    public Vector2 Target;
    public int Cooldown;
    // รูปการโจมตี ถ่ายไว้ตอนเริ่มจังหวะโดน
    public Vector2 ShapeCenter;
    public float ShapeRadius;
    public float ShapeFacing;
    public float ShapeHalfArc;

    public bool Armor => Kind == EKind.Boss ||
        (Phase == EPhase.Windup && PhaseTick >= PhaseLength - 8) || Phase == EPhase.Active;

    public bool Alive => Phase != EPhase.Dead;
}

// สิ่งที่ client ได้รู้เกี่ยวกับศัตรูหนึ่งตัวใน snapshot
public struct EnemyView
{
    public int Id;
    public EKind Kind;
    public Vector2 Pos;
    public float Facing;
    public float Radius;
    public int Hp, MaxHp;
    public float Poise, MaxPoise;
    public EPhase Phase;
    public int PhaseTick;
    public int PhaseLength;
    public AttackId Attack;
    public int AttackSerial;
    public Vector2 Target;
    public Vector2 ShapeCenter;
    public float ShapeRadius, ShapeFacing, ShapeHalfArc;
    public bool Armor;
}

public enum Judgment : byte { None, Miss, Evade, PerfectDodge, Guard, PerfectGuard, GuardBreak, Hit }

public enum EvKind : byte
{
    EnemyDamaged, EnemyStaggered, EnemyKilled,
    Defense,        // ผลการตัดสินท่าของศัตรูที่โจมตีผู้เล่น
    BossSpawned, Victory, Defeat,
}

public struct GameEvent
{
    public EvKind Kind;
    public int Tick;
    public int EnemyId;
    public int AttackSerial;
    public int Amount;
    public Vector2 Pos;
    public Judgment Judgment;
    public int AttackTick;   // tick ที่ท่าของศัตรูลงจริงบน server
    public int JudgedAt;     // tick ของผู้เล่นที่ใช้ตัดสิน
    public bool Compensated;
}

public sealed class Snapshot
{
    public int Tick;
    public int LastInputTick;
    public PlayerState Player;
    public EnemyView[] Enemies = Array.Empty<EnemyView>();
    public GameEvent[] Events = Array.Empty<GameEvent>();
}
