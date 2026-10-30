using System.Numerics;

namespace Eighth.Sim;

public static class Geo
{
    public static Vector2 Dir(float angle) => new(MathF.Cos(angle), MathF.Sin(angle));

    public static float Angle(Vector2 v) => MathF.Atan2(v.Y, v.X);

    public static float Wrap(float a)
    {
        while (a > MathF.PI) a -= MathF.Tau;
        while (a < -MathF.PI) a += MathF.Tau;
        return a;
    }

    public static bool InCircle(Vector2 center, float radius, Vector2 p, float pr) =>
        Vector2.DistanceSquared(center, p) <= (radius + pr) * (radius + pr);

    // ทรงพัด: จุดยอด ทิศ รัศมี และครึ่งมุม (เรเดียน) เทียบกับวงกลมรัศมี pr
    public static bool InSector(Vector2 apex, float facing, float radius, float halfArc, Vector2 p, float pr)
    {
        var d = p - apex;
        float dist = d.Length();
        if (dist - pr > radius) return false;
        if (halfArc >= MathF.PI - 0.001f || dist < 0.0001f) return true;
        float diff = MathF.Abs(Wrap(Angle(d) - facing));
        return diff <= halfArc + MathF.Asin(MathF.Min(1f, pr / dist));
    }

    public static Vector2 ClampToArena(Vector2 p, float r)
    {
        float max = Cfg.ArenaRadius - r;
        return p.LengthSquared() > max * max ? Vector2.Normalize(p) * max : p;
    }
}

public interface IFinishQuery
{
    bool TryFinisherTarget(Vector2 pos, out int enemyId);
}

// ตรรกะการเคลื่อนที่และท่าของผู้เล่น ไม่มีการสุ่ม client ใช้ทำนาย server ใช้ตัดสิน
public static class PlayerSim
{
    public static bool IsAttack(PAction a) =>
        a is PAction.Light or PAction.Heavy or PAction.Skill1 or PAction.Skill2 or PAction.Finisher;

    public static MoveDef Def(in PlayerState s) => s.Action switch
    {
        PAction.Light => Moves.Light[s.Combo],
        PAction.Heavy => Moves.Heavy,
        PAction.Skill1 => Moves.Skill1,
        PAction.Skill2 => Moves.Skill2,
        PAction.Finisher => Moves.Finisher,
        _ => default,
    };

    public static bool InRecovery(in PlayerState s)
    {
        if (!IsAttack(s.Action)) return false;
        if (s.Action == PAction.Skill1) return s.ActionTick >= Moves.Skill1HitTicks[^1] + 2;
        var d = Def(s);
        return s.ActionTick >= d.Startup + d.Active;
    }

    public static void Step(ref PlayerState s, in InputCmd c, int tick, IFinishQuery q)
    {
        if (s.Cd1 > 0) s.Cd1--;
        if (s.Cd2 > 0) s.Cd2--;
        if (s.StaminaDelay > 0) s.StaminaDelay--;
        else s.Stamina = MathF.Min(Cfg.StaminaMax, s.Stamina + Cfg.StaminaRegenPerSec * Cfg.Dt);

        if (s.Action == PAction.Dead) return;

        if (c.Press != Press.None)
        {
            s.Buffered |= c.Press;
            s.BufferedTick = tick;
        }
        if (tick - s.BufferedTick > Cfg.InputBufferTicks) s.Buffered = Press.None;

        if (s.Action != PAction.Idle)
        {
            s.ActionTick++;
            bool done = s.Action switch
            {
                PAction.HitStun or PAction.GuardBreak => s.ActionTick >= s.StunTicks,
                PAction.Dodge => s.ActionTick >= Cfg.DodgeTicks,
                _ => s.ActionTick >= Def(s).Total,
            };
            if (done) ToIdle(ref s, tick);
        }

        TryStart(ref s, c, tick, q);
        Move(ref s, c, tick);
        s.Pos = Geo.ClampToArena(s.Pos, Cfg.PlayerRadius);
    }

    static void ToIdle(ref PlayerState s, int tick)
    {
        if (s.Action == PAction.Light) s.ComboResetTick = tick + Cfg.ComboResetTicks;
        s.Action = PAction.Idle;
        s.ActionTick = 0;
    }

    static void TryStart(ref PlayerState s, in InputCmd c, int tick, IFinishQuery q)
    {
        var b = s.Buffered;
        if (b == Press.None) return;
        bool free = s.Action == PAction.Idle;
        bool canCancel = free || InRecovery(s);
        if (!canCancel) return;

        if (b.HasFlag(Press.Dodge) && s.Stamina >= Cfg.DodgeCost)
        {
            var m = new Vector2(c.MoveX, c.MoveY);
            var dir = m.LengthSquared() > 0.01f ? Vector2.Normalize(m) : Geo.Dir(s.Facing);
            s.Action = PAction.Dodge;
            s.ActionTick = 0;
            s.ActionSerial++;
            s.DodgeDir = dir;
            s.Facing = Geo.Angle(dir);
            s.Stamina -= Cfg.DodgeCost;
            s.StaminaDelay = Cfg.StaminaRegenDelay;
            s.DodgeStartTick = tick;
            s.Guarding = false;
            s.Buffered = Press.None;
            return;
        }
        if (b.HasFlag(Press.Finisher) && q.TryFinisherTarget(s.Pos, out int target))
        {
            s.FinisherTarget = target;
            StartAttack(ref s, PAction.Finisher, c);
            return;
        }
        if (b.HasFlag(Press.Skill1) && s.Cd1 == 0)
        {
            StartAttack(ref s, PAction.Skill1, c);
            s.Cd1 = Cfg.Skill1Cooldown;
            return;
        }
        if (b.HasFlag(Press.Skill2) && s.Cd2 == 0)
        {
            StartAttack(ref s, PAction.Skill2, c);
            s.Cd2 = Cfg.Skill2Cooldown;
            return;
        }
        if (b.HasFlag(Press.Heavy))
        {
            StartAttack(ref s, PAction.Heavy, c);
            return;
        }
        if (b.HasFlag(Press.Light))
        {
            int next = s.Action == PAction.Light || (free && tick <= s.ComboResetTick)
                ? (s.Combo + 1) % Moves.Light.Length
                : 0;
            StartAttack(ref s, PAction.Light, c);
            s.Combo = next;
        }
    }

    static void StartAttack(ref PlayerState s, PAction a, in InputCmd c)
    {
        // กระแส: ต่อท่าต่างชนิดได้ชั้นเพิ่ม ใช้ท่าเดิมซ้ำไม่เพิ่ม
        if (a != s.LastFlowKind) s.Flow = Math.Min(Cfg.MaxFlow, s.Flow + 1);
        s.LastFlowKind = a;
        if (a != PAction.Light) s.ComboResetTick = -1;
        s.Action = a;
        s.ActionTick = 0;
        s.ActionSerial++;
        s.Facing = c.Aim;
        s.Guarding = false;
        s.Buffered = Press.None;
    }

    static void Move(ref PlayerState s, in InputCmd c, int tick)
    {
        switch (s.Action)
        {
            case PAction.Idle:
            {
                var m = new Vector2(c.MoveX, c.MoveY);
                if (m.LengthSquared() > 1f) m = Vector2.Normalize(m);
                if (c.Guard && !s.Guarding) s.GuardStartTick = tick;
                s.Guarding = c.Guard;
                float speed = Cfg.PlayerSpeed * (s.Guarding ? Cfg.GuardMoveFactor : 1f);
                s.Pos += m * speed * Cfg.Dt;
                if (s.Guarding) s.Facing = c.Aim;
                else if (m.LengthSquared() > 0.01f) s.Facing = Geo.Angle(m);
                return;
            }
            case PAction.Dodge:
            {
                float n = Cfg.DodgeTicks;
                float f0 = Ease(s.ActionTick / n), f1 = Ease((s.ActionTick + 1) / n);
                s.Pos += s.DodgeDir * Cfg.DodgeDistance * (f1 - f0);
                break;
            }
            case PAction.Skill1:
                if (s.ActionTick < Moves.Skill1DashTicks)
                    s.Pos += Geo.Dir(s.Facing) * (Moves.Skill1.Lunge / Moves.Skill1DashTicks);
                break;
            case PAction.Light or PAction.Heavy or PAction.Skill2 or PAction.Finisher:
            {
                var d = Def(s);
                int window = d.Startup + d.Active;
                if (d.Lunge > 0 && s.ActionTick < window)
                    s.Pos += Geo.Dir(s.Facing) * (d.Lunge / window);
                break;
            }
        }
        s.Guarding = false;
    }

    static float Ease(float x)
    {
        x = Math.Clamp(x, 0f, 1f);
        return 1f - (1f - x) * (1f - x);
    }

    // tick นี้ท่าของผู้เล่นอยู่ในจังหวะโดนไหม hitIndex ใช้กันโดนซ้ำในท่าเดียว
    public static bool ActiveHit(in PlayerState s, out MoveDef def, out int hitIndex)
    {
        def = Def(s);
        hitIndex = 0;
        if (!IsAttack(s.Action)) return false;
        if (s.Action == PAction.Skill1)
        {
            for (int i = 0; i < Moves.Skill1HitTicks.Length; i++)
            {
                int h = Moves.Skill1HitTicks[i];
                if (s.ActionTick >= h && s.ActionTick < h + 2)
                {
                    hitIndex = i;
                    return true;
                }
            }
            return false;
        }
        return s.ActionTick >= def.Startup && s.ActionTick < def.Startup + def.Active;
    }
}
