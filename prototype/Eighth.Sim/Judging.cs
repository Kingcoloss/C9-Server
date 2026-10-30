using System.Numerics;

namespace Eighth.Sim;

public struct AttackShape
{
    public ShapeKind Kind;
    public Vector2 Center;
    public float Radius;
    public float Facing;
    public float HalfArc;

    public readonly bool Contains(Vector2 p, float pr) => Kind == ShapeKind.Sector
        ? Geo.InSector(Center, Facing, Radius, HalfArc, p, pr)
        : Geo.InCircle(Center, Radius, p, pr);

    public static AttackShape Of(EnemyState e) => new()
    {
        Kind = Attacks.Get(e.Attack).Shape,
        Center = e.ShapeCenter,
        Radius = e.ShapeRadius,
        Facing = e.ShapeFacing,
        HalfArc = e.ShapeHalfArc,
    };

    public static AttackShape Of(in EnemyView e) => new()
    {
        Kind = Attacks.Get(e.Attack).Shape,
        Center = e.ShapeCenter,
        Radius = e.ShapeRadius,
        Facing = e.ShapeFacing,
        HalfArc = e.ShapeHalfArc,
    };
}

// กติกาตัดสินท่าของศัตรูที่ลงใส่ผู้เล่น ใช้ฟังก์ชันเดียวกันทั้ง server และ client
// tick = tick ของผู้เล่นที่ใช้ตัดสิน (state ของผู้เล่น ณ tick นั้น)
public static class Judging
{
    const float NearMargin = 1.5f;

    public static Judgment Judge(in PlayerState p, int tick, in AttackShape shape, in AttackDef def)
    {
        if (p.Action == PAction.Dead) return Judgment.None;
        bool inside = shape.Contains(p.Pos, Cfg.PlayerRadius);
        bool near = shape.Contains(p.Pos, Cfg.PlayerRadius + NearMargin);
        bool freshDodge = p.Action == PAction.Dodge && tick - p.DodgeStartTick <= Cfg.PerfectDodgeWindow;

        if (freshDodge && near) return Judgment.PerfectDodge;
        if (!inside) return Judgment.Miss;
        if (p.Invulnerable) return Judgment.Evade;
        if (p.Guarding && def.Guardable)
        {
            if (def.Parryable && tick - p.GuardStartTick <= Cfg.PerfectGuardWindow) return Judgment.PerfectGuard;
            return p.Stamina >= Cfg.GuardStaminaCost ? Judgment.Guard : Judgment.GuardBreak;
        }
        return Judgment.Hit;
    }

    public static bool Avoided(Judgment j) => j is Judgment.Miss or Judgment.Evade or Judgment.PerfectDodge;

    public static string Th(Judgment j) => j switch
    {
        Judgment.Miss => "พลาด",
        Judgment.Evade => "หลบ",
        Judgment.PerfectDodge => "หลบพริ้ว",
        Judgment.Guard => "กัน",
        Judgment.PerfectGuard => "รับพริ้ว",
        Judgment.GuardBreak => "การ์ดแตก",
        Judgment.Hit => "โดน",
        _ => "-",
    };
}
