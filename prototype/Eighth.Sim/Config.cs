namespace Eighth.Sim;

// ค่าตั้งต้นจากตาราง "ค่าตั้งต้นสำหรับปรับสมดุล" ใน design bible
// เวลาเป็นจำนวน tick ที่ 30 Hz (1 tick = 33.3 ms) ปัดให้ใกล้ค่าในตารางที่สุด
public static class Cfg
{
    public const int TickRate = 30;
    public const float Dt = 1f / TickRate;

    public const float ArenaRadius = 15f;

    // ผู้เล่น
    public const int PlayerMaxHp = 1000;
    public const float PlayerRadius = 0.45f;
    public const float PlayerSpeed = 6f;
    public const float GuardMoveFactor = 0.4f;
    public const float StaminaMax = 100f;
    public const float DodgeCost = 30f;
    public const float StaminaRegenPerSec = 60f;
    public const int StaminaRegenDelay = 15;     // 0.5 วิ
    public const int DodgeTicks = 12;            // 0.4 วิ
    public const int DodgeIFrames = 8;           // 0.27 วิ (ตาราง 0.25)
    public const float DodgeDistance = 4.5f;
    public const int PerfectDodgeWindow = 5;     // 0.17 วิ (ตาราง 0.15)
    public const int PerfectGuardWindow = 4;     // 0.13 วิ (ตาราง 0.12)
    public const float GuardDamageFactor = 0.2f;
    public const float GuardStaminaCost = 20f;
    public const int HitStunTicks = 11;
    public const int GuardBreakTicks = 24;
    public const int InputBufferTicks = 6;
    public const int ComboResetTicks = 10;
    public const int CounterTicks = 36;          // ช่วงสวนกลับหลังหลบพริ้ว
    public const int MaxFlow = 5;
    public const float FlowDamagePerStack = 0.03f;
    public const int Skill1Cooldown = 120;
    public const int Skill2Cooldown = 210;
    public const float FinisherRange = 3f;

    // netcode
    public const int InterpTicks = 2;            // ภาพของคนอื่นช้ากว่าจริง 2 snapshot
    public const int DefenseWaitCapTicks = 10;   // server รอ input ที่ตรงกับภาพได้ไม่เกิน 333 ms
    public const int AttackRewindCapTicks = 8;   // ย้อนตำแหน่งศัตรูให้ผู้ตีได้ไม่เกิน 267 ms
    public const int HistoryTicks = 64;
}

public readonly record struct MoveDef(
    int Startup, int Active, int Recover,
    int Damage, int Poise,
    float Radius, float ArcDeg, float Lunge)
{
    public int Total => Startup + Active + Recover;
}

public static class Moves
{
    public static readonly MoveDef[] Light =
    {
        new(4, 2, 7, 40, 8, 1.9f, 110, 0.5f),
        new(4, 2, 7, 40, 8, 1.9f, 110, 0.5f),
        new(5, 2, 8, 52, 10, 2.0f, 120, 0.6f),
        new(7, 3, 11, 85, 22, 2.3f, 140, 0.9f),
    };

    public static readonly MoveDef Heavy = new(10, 3, 13, 125, 35, 2.4f, 150, 1.0f);

    // หมัดสามดาว: พุ่งไปข้างหน้าแล้วต่อย 3 ครั้ง
    public static readonly MoveDef Skill1 = new(0, 0, 26, 45, 12, 1.9f, 110, 3.2f);
    public static readonly int[] Skill1HitTicks = { 5, 11, 17 };
    public const int Skill1DashTicks = 18;

    // ดาวตกกระแทก: ทนแรงตอนง้าง แล้วกระแทกรอบตัว
    public static readonly MoveDef Skill2 = new(12, 2, 14, 150, 45, 3.3f, 360, 0f);

    // ท่าปิดฉาก: ใช้กับศัตรูที่เสียหลัก ไม่รับดาเมจตลอดท่า
    public static readonly MoveDef Finisher = new(6, 2, 16, 320, 0, 2.8f, 140, 0.8f);
}

public enum Tele : byte { Gold, Red, Purple }

public enum ShapeKind : byte { CircleFront, Sector, CircleAtTarget }

public enum AttackId : byte { None, WolfLunge, BossSlam, BossFeint, BossSweep, BossInkPool }

public readonly record struct AttackDef(
    AttackId Id, string NameTh, Tele Tele,
    int Windup, int Active, int Recover,
    int Damage, ShapeKind Shape, float Radius, float Offset, float HalfArcDeg,
    bool Guardable, bool Parryable, float Lunge, int FeintHold, int PoiseOnParry);

public static class Attacks
{
    public static readonly AttackDef WolfLunge = new(AttackId.WolfLunge, "กระโจน", Tele.Gold,
        18, 3, 16, 70, ShapeKind.CircleFront, 1.3f, 1.4f, 0, true, true, 1.5f, 0, 40);

    public static readonly AttackDef BossSlam = new(AttackId.BossSlam, "หมึกทุบ", Tele.Gold,
        24, 3, 26, 150, ShapeKind.CircleFront, 2.4f, 2.4f, 0, true, true, 0f, 0, 110);

    // ท่าหลอก: หน้าตาเหมือนหมึกทุบ แต่ค้างก่อนลง 10 tick
    public static readonly AttackDef BossFeint = new(AttackId.BossFeint, "หมึกทุบ (หลอก)", Tele.Gold,
        34, 3, 26, 150, ShapeKind.CircleFront, 2.4f, 2.4f, 0, true, true, 0f, 10, 110);

    public static readonly AttackDef BossSweep = new(AttackId.BossSweep, "หนวดกวาด", Tele.Red,
        30, 4, 24, 190, ShapeKind.Sector, 4.4f, 0f, 100, false, false, 0f, 0, 0);

    public static readonly AttackDef BossInkPool = new(AttackId.BossInkPool, "บ่อหมึก", Tele.Purple,
        36, 3, 20, 120, ShapeKind.CircleAtTarget, 3.0f, 0f, 0, true, false, 0f, 0, 0);

    public static AttackDef Get(AttackId id) => id switch
    {
        AttackId.WolfLunge => WolfLunge,
        AttackId.BossSlam => BossSlam,
        AttackId.BossFeint => BossFeint,
        AttackId.BossSweep => BossSweep,
        AttackId.BossInkPool => BossInkPool,
        _ => throw new ArgumentOutOfRangeException(nameof(id)),
    };
}
