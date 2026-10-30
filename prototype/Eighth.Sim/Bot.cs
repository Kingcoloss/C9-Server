using System.Numerics;

namespace Eighth.Sim;

// บอทเล่นแทนคน ดูเฉพาะสิ่งที่ client เห็น (ภาพศัตรูย้อนหลัง) แล้วกดปุ่มผ่าน GameClient เหมือนผู้เล่น
// ใช้ในโหมดสาธิต (--autoplay) และในการทดสอบ
public sealed class FightBot
{
    public int ReactTicks = 2;      // กดหลบหรือยกการ์ดก่อนท่าลงตามภาพกี่ tick
    public bool UseGuard = true;    // สลับระหว่างหลบกับรับพริ้วสำหรับท่าที่รับพริ้วได้

    Rng _rng;
    int _handled = -1;
    int _clearAt = -1;
    int _guardUntil = -1;
    int _defenses;

    public FightBot(uint seed = 1234) => _rng = new Rng(seed);

    public void Step(GameClient c, double now, int frame)
    {
        var me = c.PlayerRenderPos(now);
        EnemyRender target = default;
        bool hasTarget = false;
        bool threat = false;
        float best = float.MaxValue;

        foreach (var e in c.Enemies(now))
        {
            if (e.V.Phase == EPhase.Dead) continue;
            float d = Vector2.Distance(me, e.Pos);
            if (d < best)
            {
                best = d;
                target = e;
                hasTarget = true;
            }
            if (e.V.Phase != EPhase.Windup || e.V.Attack == AttackId.None) continue;
            var shape = Telegraph.Preview(e);
            if (!shape.Contains(me, Cfg.PlayerRadius + 0.8f)) continue;
            // เห็นวงเตือนครอบตัว: หยุดออกท่าใหม่ จะได้หลบทันตอนวงเต็ม
            threat = true;
            int key = e.V.Id * 10000 + e.V.AttackSerial;
            if (key == _handled || e.V.PhaseLength - e.V.PhaseTick > ReactTicks) continue;
            _handled = key;
            Defend(c, e, shape, me);
        }

        if (c.ClientTick <= _guardUntil) return;
        if (c.Held.Guard)
        {
            c.Held.Guard = false;
        }
        if (_clearAt >= 0)
        {
            if (c.ClientTick < _clearAt) return;
            c.Held.MoveX = c.Held.MoveY = 0;
            _clearAt = -1;
        }
        if (!hasTarget)
        {
            c.Held.MoveX = c.Held.MoveY = 0;
            return;
        }

        var to = target.Pos - me;
        c.Held.Aim = Geo.Angle(to);
        float reach = target.V.Radius + 1.4f;
        if (best > reach)
        {
            var dir = Vector2.Normalize(to);
            c.Held.MoveX = dir.X;
            c.Held.MoveY = dir.Y;
            return;
        }
        c.Held.MoveX = c.Held.MoveY = 0;
        if (threat) return;
        if (target.V.Phase == EPhase.Staggered) c.QueuePress(Press.Finisher);
        else if (c.Predicted.Cd1 == 0 && _rng.Range(0, 40) == 0) c.QueuePress(Press.Skill1);
        else if (c.Predicted.Cd2 == 0 && _rng.Range(0, 50) == 0) c.QueuePress(Press.Skill2);
        else if (frame % 6 == 0) c.QueuePress(_rng.Range(0, 5) == 0 ? Press.Heavy : Press.Light);
    }

    void Defend(GameClient c, in EnemyRender e, in AttackShape shape, Vector2 me)
    {
        var def = Attacks.Get(e.V.Attack);
        bool guard = UseGuard && def.Guardable && def.Parryable && _defenses++ % 2 == 1;
        c.Held.Aim = Geo.Angle(e.Pos - me);
        if (guard)
        {
            c.Held.MoveX = c.Held.MoveY = 0;
            c.Held.Guard = true;
            _guardUntil = c.ClientTick + 10;
            return;
        }
        var away = me - shape.Center;
        if (away.LengthSquared() < 0.01f) away = new Vector2(1, 0);
        away = Vector2.Normalize(away);
        c.Held.Guard = false;
        c.Held.MoveX = away.X;
        c.Held.MoveY = away.Y;
        c.QueuePress(Press.Dodge);
        _clearAt = c.ClientTick + 3;
    }
}
