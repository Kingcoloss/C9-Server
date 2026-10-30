using System.Numerics;
using Eighth.Sim;

// ทดสอบ netcode แบบไม่มีหน้าจอ: บอทกดปุ่มตามภาพที่ client เห็น เหมือนคนเล่นจริง
int failed = 0;

void Check(string name, bool ok, string detail = "")
{
    Console.WriteLine($"{(ok ? "PASS" : "FAIL")}  {name}  {detail}");
    if (!ok) failed++;
}

Console.WriteLine("== หลบพริ้ว: กดหลบ 2 tick ก่อนท่าลงตามภาพที่เห็น ==");
foreach (int ping in new[] { 0, 80, 150, 250 })
{
    var r = Trials.Defend(ping, comp: true, guard: false);
    Check($"ping {ping,3} ชดเชยเปิด", r.Server == Judgment.PerfectDodge && !r.Mismatch, r.ToString());
}
foreach (int ping in new[] { 80, 150, 250 })
{
    var r = Trials.Defend(ping, comp: false, guard: false);
    Check($"ping {ping,3} ชดเชยปิด (server ต้องไม่ให้หลบพริ้ว)", r.Server != Judgment.PerfectDodge, r.ToString());
}

Console.WriteLine("== รับพริ้ว: ยกการ์ด 2 tick ก่อนท่าลง ==");
foreach (int ping in new[] { 0, 150, 250 })
{
    var r = Trials.Defend(ping, comp: true, guard: true);
    Check($"ping {ping,3} ชดเชยเปิด", r.Server == Judgment.PerfectGuard && !r.Mismatch, r.ToString());
}
{
    var r = Trials.Defend(150, comp: false, guard: true);
    Check("ping 150 ชดเชยปิด (server ต้องไม่ให้รับพริ้ว)", r.Server != Judgment.PerfectGuard, r.ToString());
}

Console.WriteLine("== ผลเหมือนเดิมทุกครั้ง (deterministic) ==");
{
    string a = Trials.Scripted(150, 900), b = Trials.Scripted(150, 900);
    Check("ปุ่มชุดเดียวกัน ได้โลกเดียวกัน", a == b, a);
}

Console.WriteLine("== บอทเล่นจนจบด่าน ==");
foreach (int ping in new[] { 0, 80, 150, 250 })
{
    var f = Trials.Fight(ping, comp: true);
    Check($"ping {ping,3} เล่นจบไม่มี error, ผลทาย≠server ไม่เกิน 10%",
        f.Finished && f.Mismatches * 10 <= Math.Max(10, f.Judged), f.ToString());
}
{
    var f = Trials.Fight(150, comp: false);
    Console.WriteLine($"INFO  ping 150 ชดเชยปิด  {f}");
}

Console.WriteLine(failed == 0 ? "ผ่านทั้งหมด" : $"ไม่ผ่าน {failed} ข้อ");
return failed == 0 ? 0 : 1;

record DefendResult(Judgment Local, Judgment Server, bool Mismatch, int WaitTicks)
{
    public override string ToString() =>
        $"[เครื่องผู้เล่น: {Judging.Th(Local)} / server: {Judging.Th(Server)} / server รอ {WaitTicks} tick]";
}

record FightResult(bool Finished, string Outcome, double Seconds, int Judged, int Mismatches, int Corrections,
    int LocalHits, int ServerHits, int Late)
{
    public override string ToString() =>
        $"[{Outcome} {Seconds:0}s, ตัดสิน {Judged}, ไม่ตรง {Mismatches}, แก้ตำแหน่ง {Corrections}, " +
        $"ตีโดน(เครื่อง/server) {LocalHits}/{ServerHits}, ปุ่มมาช้า {Late}]";
}

static class Trials
{
    const double Frame = 1.0 / 60;

    public static DefendResult Defend(int ping, bool comp, bool guard, int react = 2)
    {
        var s = new LocalSession(ping, comp, seed: 11, wolves: 1);
        var c = s.Client;
        int handled = -1, clearAt = -1;
        for (int f = 0; f < 60 * 30; f++)
        {
            foreach (var e in c.Enemies(s.Now))
            {
                if (e.V.Phase == EPhase.Dead) continue;
                var to = e.Pos - c.Predicted.Pos;
                c.Held.Aim = Geo.Angle(to);
                if (e.V.Phase != EPhase.Windup || e.V.AttackSerial == handled) continue;
                if (e.V.PhaseLength - e.V.PhaseTick > react) continue;
                handled = e.V.AttackSerial;
                if (guard)
                {
                    c.Held.Guard = true;
                    continue;
                }
                var side = Vector2.Normalize(new Vector2(-to.Y, to.X));
                c.Held.MoveX = side.X;
                c.Held.MoveY = side.Y;
                c.QueuePress(Press.Dodge);
                clearAt = c.ClientTick + 3;
            }
            if (clearAt >= 0 && c.ClientTick >= clearAt)
            {
                c.Held.MoveX = c.Held.MoveY = 0;
                clearAt = -1;
            }
            s.Advance(Frame);
            while (c.TryDequeue(out var ui))
                if (ui.Kind == UiKind.Verdict)
                    return new DefendResult(ui.Local, ui.Server, ui.Mismatch, ui.WaitTicks);
        }
        return new DefendResult(Judgment.None, Judgment.None, true, -1);
    }

    // กดปุ่มสุ่มแบบกำหนด seed แล้วสรุปสถานะโลกเป็นข้อความ
    public static string Scripted(int ping, int frames)
    {
        var s = new LocalSession(ping, true, seed: 3);
        var rng = new Rng(99);
        var c = s.Client;
        for (int f = 0; f < frames; f++)
        {
            if (f % 10 == 0)
            {
                c.Held.MoveX = rng.Range(-1, 2);
                c.Held.MoveY = rng.Range(-1, 2);
                c.Held.Aim = rng.Range(0, 628) / 100f;
                c.Held.Guard = rng.Range(0, 6) == 0;
                c.QueuePress((Press)(1 << rng.Range(0, 5)));
            }
            s.Advance(Frame);
            while (c.TryDequeue(out _)) { }
        }
        var w = s.Server.World;
        var parts = w.Enemies.Select(e => $"{e.Id}:{e.Hp}@{e.Pos.X:0.000},{e.Pos.Y:0.000}");
        return $"t{w.Tick} hp{w.Player.Hp} pos{w.Player.Pos.X:0.000},{w.Player.Pos.Y:0.000} " + string.Join(" ", parts);
    }

    // บอทเดินเข้าหาศัตรูที่ใกล้ที่สุด ตีต่อเนื่อง ใช้สกิล หลบตามภาพ และใช้ท่าปิดฉากเมื่อศัตรูเสียหลัก
    public static FightResult Fight(int ping, bool comp)
    {
        var s = new LocalSession(ping, comp, seed: 5);
        var c = s.Client;
        int handled = -1, clearAt = -1, localHits = 0, serverHits = 0, staggers = 0;
        double bossAt = 0;
        var dmgLog = new List<string>();
        string outcome = "หมดเวลา";
        var rng = new Rng(1234);
        for (int f = 0; f < 60 * 600; f++)
        {
            EnemyRender? target = null;
            float best = float.MaxValue;
            var me = c.PlayerRenderPos(s.Now);
            foreach (var e in c.Enemies(s.Now))
            {
                if (e.V.Phase == EPhase.Dead) continue;
                float d = Vector2.Distance(me, e.Pos);
                if (d < best) { best = d; target = e; }

                if (e.V.Phase != EPhase.Windup || handled == e.V.Id * 10000 + e.V.AttackSerial) continue;
                var shape = Telegraph.Preview(e);
                if (!shape.Contains(me, Cfg.PlayerRadius + 0.8f)) continue;
                if (e.V.PhaseLength - e.V.PhaseTick > 2) continue;
                handled = e.V.Id * 10000 + e.V.AttackSerial;
                var away = me - shape.Center;
                if (away.LengthSquared() < 0.01f) away = new Vector2(1, 0);
                away = Vector2.Normalize(away);
                c.Held.MoveX = away.X;
                c.Held.MoveY = away.Y;
                c.QueuePress(Press.Dodge);
                clearAt = c.ClientTick + 3;
            }

            if (clearAt < 0 && target is { } t)
            {
                var to = t.Pos - me;
                c.Held.Aim = Geo.Angle(to);
                float reach = t.V.Radius + 1.4f;
                if (best > reach)
                {
                    var dir = Vector2.Normalize(to);
                    c.Held.MoveX = dir.X;
                    c.Held.MoveY = dir.Y;
                }
                else
                {
                    c.Held.MoveX = c.Held.MoveY = 0;
                    if (t.V.Phase == EPhase.Staggered) c.QueuePress(Press.Finisher);
                    else if (c.Predicted.Cd1 == 0 && rng.Range(0, 30) == 0) c.QueuePress(Press.Skill1);
                    else if (c.Predicted.Cd2 == 0 && rng.Range(0, 40) == 0) c.QueuePress(Press.Skill2);
                    else if (f % 4 == 0) c.QueuePress(rng.Range(0, 5) == 0 ? Press.Heavy : Press.Light);
                }
            }
            if (clearAt >= 0 && c.ClientTick >= clearAt)
            {
                c.Held.MoveX = c.Held.MoveY = 0;
                clearAt = -1;
            }

            s.Advance(Frame);
            while (c.TryDequeue(out var ui))
            {
                if (ui.Kind == UiKind.LocalHit) localHits++;
                if (ui.Kind == UiKind.EnemyDamage) { serverHits++; if (bossAt > 0) dmgLog.Add($"{ui.Amount}:{c.Predicted.Action}"); }
                if (ui.Kind == UiKind.Banner && ui.Text.StartsWith("ราชาหมึก")) bossAt = s.Now;
                if (ui.Kind == UiKind.Stagger) staggers++;
                if (ui.Kind == UiKind.Banner && ui.Text.StartsWith("ชนะ")) outcome = "ชนะ";
                if (ui.Kind == UiKind.Banner && ui.Text.StartsWith("พ่ายแพ้")) outcome = "แพ้";
            }
            if (outcome != "หมดเวลา") break;
        }
        if (Environment.GetEnvironmentVariable("FIGHT_DEBUG") == "1")
            Console.WriteLine($"      boss {bossAt:0.0}s -> end {s.Now:0.0}s, staggers {staggers}, hp {s.Server.World.Player.Hp}\n      {string.Join(" ", dmgLog)}");
        return new FightResult(outcome != "หมดเวลา", outcome, s.Now, c.ServerJudged, c.Mismatches, c.Corrections,
            localHits, serverHits, s.Server.LateInputs);
    }
}
