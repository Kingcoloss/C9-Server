using Eighth.Sim;
using Godot;

namespace EighthThrone.Demo;

// ตัวละครผู้เล่นที่ฉากใช้ มีสองแบบ: รูปทรงพื้นฐาน (PlayerModel) กับโมเดลจริงพร้อมท่า (AvatarModel)
public abstract partial class PlayerVisual : Node3D
{
    public bool Stepped { get; protected set; } // เท้าแตะพื้นในเฟรมนี้ (ใช้เล่นเสียงเท้า)
    public abstract void Sync(in PlayerState st, Vector3 pos, int tick, float alpha, double t, float dt);
}

// ท่าทางหนึ่งชุด: การหมุนของกระดูกที่ขยับได้ + ตำแหน่งสะโพก
public sealed class Pose
{
    public readonly Quaternion[] Q;
    public Vector3 Hip;
    public Pose(int n) { Q = new Quaternion[n]; }

    public void CopyFrom(Pose o) { Array.Copy(o.Q, Q, Q.Length); Hip = o.Hip; }

    public void Blend(Pose a, Pose b, float w)
    {
        for (int i = 0; i < Q.Length; i++) Q[i] = a.Q[i].Slerp(b.Q[i], w);
        Hip = a.Hip.Lerp(b.Hip, w);
    }

    // ท่อนบนจาก b ท่อนล่างจาก a (เช่น เดินไปพร้อมยกโล่)
    public void Layer(Pose a, Pose b, float[] upper, float w)
    {
        for (int i = 0; i < Q.Length; i++) Q[i] = a.Q[i].Slerp(b.Q[i], upper[i] * w);
        Hip = a.Hip;
    }
}

public partial class AvatarModel : PlayerVisual
{
    readonly CharacterSet _cs;
    readonly Node3D _pivot;
    readonly Node3D _model;
    readonly Skeleton3D _sk;
    readonly int[] _driven;
    readonly int[] _slotOf;       // กระดูก -> ช่องใน Pose (-1 ถ้าไม่ขยับ)
    readonly float[] _upper;
    readonly Dictionary<string, int> _slot = new();
    readonly Pose _out, _from, _tgt, _loco, _a, _b, _c, _rest;
    readonly Quaternion[] _fk;
    readonly List<GeometryInstance3D> _geoms = new();
    readonly ShaderMaterial _bladeMat;
    readonly MeshInstance3D _guard;
    readonly StandardMaterial3D _guardMat;
    readonly MeshInstance3D _aura;

    float _yaw, _speed, _phase, _legYaw, _bank;
    double _steps;
    double _idleT;
    long _key = long.MinValue;
    float _fade = 1, _fadeDur = 0.1f;
    readonly List<(MeshInstance3D Mesh, int Shape, float Weight)> _blink = new();
    readonly SpringSim? _springs;
    double _nextBlink = 2;
    int _lastHp = -1;
    double _hurtAt = -10, _deadAt = -1;

    public AvatarModel(CharacterSet cs)
    {
        _cs = cs;
        _pivot = new Node3D { Position = new Vector3(0, cs.HipHeight, 0) };
        AddChild(_pivot);
        _model = (Node3D)cs.Template.Duplicate();
        _model.Transform = cs.ModelXform;
        _model.Position += new Vector3(0, -cs.HipHeight, 0);
        _pivot.AddChild(_model);
        _sk = RigLoad.FindFirst<Skeleton3D>(_model)!;
        _sk.ResetBonePoses();

        _driven = cs.Driven;
        _slotOf = Enumerable.Repeat(-1, _sk.GetBoneCount()).ToArray();
        _upper = new float[_driven.Length];
        for (int j = 0; j < _driven.Length; j++)
        {
            _slotOf[_driven[j]] = j;
            _slot[cs.DrivenKeys[j]] = j;
            _upper[j] = Humanoid.UpperWeight(cs.DrivenKeys[j]);
        }
        int n = _driven.Length;
        _out = new Pose(n); _from = new Pose(n); _tgt = new Pose(n); _loco = new Pose(n);
        _a = new Pose(n); _b = new Pose(n); _c = new Pose(n); _rest = new Pose(n);
        for (int j = 0; j < n; j++) _rest.Q[j] = cs.Rig.RestL[_driven[j]];
        _rest.Hip = cs.Rig.RestLP[cs.Rig.Hips];
        _out.CopyFrom(_rest);
        _fk = new Quaternion[_sk.GetBoneCount()];

        foreach (var g in RigLoad.FindAll<GeometryInstance3D>(_model)) _geoms.Add(g);
        if (cs.Springs is { } sp) _springs = new SpringSim(_sk, sp);
        foreach (var (path, shape, weight) in cs.Blink)
            if (_model.GetNodeOrNull<MeshInstance3D>(path) is { } mi) _blink.Add((mi, shape, weight));

        // ดาบมือขวา โล่แขนซ้าย (ไฟล์ Mixamo ชุดดาบโล่ไม่มีอาวุธมาให้)
        _bladeMat = Toon.Flat(new Color("dfe9f2"), new Color(0.7f, 0.78f, 0.9f), 0.003f, new Color(0.25f, 0.5f, 0.65f));
        var steel = Toon.Flat(new Color("39424f"), new Color(0.6f, 0.6f, 0.75f));
        var teal = Toon.Flat(new Color("2f7f86"), new Color(0.62f, 0.62f, 0.85f));
        var gold = Toon.Flat(new Color("c9a24a"), new Color(0.75f, 0.62f, 0.7f));
        if (Attach("rightHand", "rightMiddle1", 0.55f, -0.025f, out var sword))
        {
            Part(sword, Art.Capsule(0.025f, 0.2f), steel, new Vector3(0, 0, 0), new Vector3(90, 0, 0));
            Part(sword, Art.Box(0.24f, 0.035f, 0.05f), gold, new Vector3(0, 0, -0.11f));
            Part(sword, Art.Box(0.06f, 0.016f, 0.9f), _bladeMat, new Vector3(0, 0, -0.58f));
            Part(sword, Art.Sphere(0.03f), gold, new Vector3(0, 0, 0.11f));
        }
        if (Attach("leftLowerArm", "leftHand", 0.55f, 0.07f, out var shield))
        {
            Part(shield, Art.Cyl(0.3f, 0.3f, 0.035f, 32), teal, Vector3.Zero);
            Part(shield, Art.Torus(0.27f, 0.32f), gold, new Vector3(0, 0.012f, 0), scale: new Vector3(1, 0.5f, 1));
            Part(shield, Art.Sphere(0.06f), gold, new Vector3(0, 0.03f, 0), scale: new Vector3(1, 0.5f, 1));
        }

        _guardMat = Art.Clear(new Color(0.48f, 0.77f, 1f, 0.3f));
        _guard = Art.Part(this, Art.Cyl(0.62f, 0.62f, 0.03f, 32), _guardMat,
            new Vector3(0, cs.HipHeight + 0.35f, -0.62f), new Vector3(90, 0, 0), shadow: false);
        _aura = Art.Part(this, Art.Torus(0.6f, 0.75f), Art.Mat(new Color("f5c542"), 0.3f, 0, new Color("f5c542"), 3f),
            new Vector3(0, 0.04f, 0), scale: new Vector3(1, 0.2f, 1), shadow: false);
    }

    void Part(Node3D parent, Mesh mesh, Material mat, Vector3 pos, Vector3? rot = null, Vector3? scale = null)
    {
        var mi = Art.Part(parent, mesh, mat, pos, rot, scale);
        _geoms.Add(mi);
    }

    // ติดของเข้ากับกระดูก: ตำแหน่งระหว่างกระดูกกับลูก (t) แล้วยกขึ้นทางหลังมือ (lift เมตร)
    // ทิศของในท่า T: ใบดาบชี้ไปหน้าตัว โล่หันหน้าขึ้น (หลังแขน) ซึ่งตอนยกกันจะหันออกหน้า
    bool Attach(string bone, string toward, float t, float lift, out Node3D holder)
    {
        holder = null!;
        var rig = _cs.Rig;
        if (!rig.Map.TryGetValue(bone, out var b)) return false;
        var ideal = rig.IdealRest(bone); // กระดูก -> แกนกลาง (ท่า T)
        var p0 = rig.Canon(rig.RestGP[b]);
        var p1 = rig.Map.TryGetValue(toward, out var c) ? rig.Canon(rig.RestGP[c]) : p0;
        float unit = _cs.UnitScale;
        var at = p0.Lerp(p1, t) + Vector3.Up * (lift / unit) - p0;
        var basisC = bone == "rightHand" ? new Basis(Vector3.Up, Mathf.Pi) : Basis.Identity;
        var inv = new Basis(ideal.Inverse());
        var att = new BoneAttachment3D { BoneName = _sk.GetBoneName(b) };
        _sk.AddChild(att);
        holder = new Node3D
        {
            Transform = new Transform3D(inv * basisC * Basis.FromScale(Vector3.One / unit), inv * at),
        };
        att.AddChild(holder);
        return true;
    }

    RigClip? Clip(string role) => _cs.Clips.GetValueOrDefault(role);

    public override void Sync(in PlayerState st, Vector3 pos, int tick, float alpha, double t, float dt)
    {
        Position = pos;
        float targetYaw = Art.Yaw(st.Facing);
        float yawRate = dt > 0 ? Mathf.Wrap(targetYaw - _yaw, -Mathf.Pi, Mathf.Pi) * 22f : 0;
        _yaw = Mathf.LerpAngle(_yaw, targetYaw, Art.Smooth(22, dt));
        Rotation = new Vector3(0, _yaw, 0);
        _speed = Mathf.Lerp(_speed, st.Action == PAction.Idle ? st.Vel.Length() : 0, Art.Smooth(14, dt));
        _idleT += dt;

        Locomotion(st, dt, out bool stepped);
        Stepped = stepped;

        float k = st.ActionTick + alpha;
        long key = ((long)st.Action << 32) | (uint)st.ActionSerial;
        float fadeIn = 0.07f;
        var pivotPos = new Vector3(0, _cs.HipHeight, 0);
        var pivotRot = Vector3.Zero;
        float tuck = 0, flinch = 0, swing = 0;
        Blade(0.25f);

        switch (st.Action)
        {
            case PAction.Idle:
                _tgt.CopyFrom(_loco);
                key = st.Guarding ? 1 : 0;
                fadeIn = _key is 0 or 1 ? 0.12f : 0.18f;
                break;
            case PAction.Light:
                swing = Attack(st.Combo switch { 0 => "light1", 1 => "light2", 2 => "light3", _ => "light4" },
                    Moves.Light[st.Combo], k);
                break;
            case PAction.Heavy:
                swing = Attack("heavy", Moves.Heavy, k);
                break;
            case PAction.Finisher:
            {
                var d = Moves.Finisher;
                swing = Attack("finisher", d, k);
                pivotPos.Y += Mathf.Sin(Mathf.Clamp(k / d.Startup, 0, 1) * Mathf.Pi) * 0.7f;
                Blade(4f);
                break;
            }
            case PAction.Skill1:
            {
                int seg = k < 8 ? 0 : k < 14 ? 1 : 2;
                key = (key << 2) | (uint)seg;
                fadeIn = 0.05f;
                var l = Moves.Light[0];
                float anchor = l.Startup + l.Active * 0.5f;
                float kk = k - (Moves.Skill1HitTicks[seg] - anchor);
                var c = Clip(seg == 1 ? "light2" : "light1");
                if (k < Moves.Skill1DashTicks)
                {
                    float dash = Moves.Skill1.Lunge / (Moves.Skill1DashTicks * Cfg.Dt);
                    var run = Clip("run") ?? Clip("walk");
                    if (run != null)
                    {
                        _phase = Mathf.PosMod(_phase + dash * dt / Math.Max(0.5f, _cs.RunSpeed * run.Length), 1f);
                        run.Sample((_phase + run.LeftStrike) * run.Length, true, _a.Q, out _a.Hip);
                    }
                    else _a.CopyFrom(_loco);
                }
                else _a.CopyFrom(_loco);
                if (c != null)
                {
                    c.Sample(AttackTime(c, l, kk), false, _b.Q, out _b.Hip);
                    _tgt.Layer(_a, _b, _upper, 1f);
                }
                else
                {
                    _tgt.CopyFrom(_a);
                    swing = Mathf.Sin(kk * 0.5f);
                }
                Blade(1.5f);
                break;
            }
            case PAction.Skill2:
            {
                var d = Moves.Skill2;
                float wind = Mathf.Clamp(k / d.Startup, 0, 1);
                float spin = Mathf.Clamp((k - d.Startup) / (d.Active + 4f), 0, 1);
                swing = Attack("skill2", d, k);
                pivotPos.Y -= 0.22f * wind * (1 - spin);
                pivotRot.Y = Mathf.Tau * spin;
                Blade(0.4f + 3f * wind);
                break;
            }
            case PAction.Dodge:
            {
                fadeIn = 0.05f;
                float p = Mathf.Clamp(k / Cfg.DodgeTicks, 0, 1);
                if (Clip("dodge") is { } c)
                {
                    c.Sample(Mathf.Lerp(c.Start, c.End, p), false, _tgt.Q, out _tgt.Hip);
                }
                else
                {
                    // ไม่มีท่ากลิ้ง: ม้วนตัวแล้วตีลังกาไปข้างหน้า
                    _tgt.CopyFrom(_loco);
                    tuck = Mathf.Sin(Mathf.Clamp(p * 1.15f, 0, 1) * Mathf.Pi);
                    pivotRot.X = -Mathf.Tau * Mathf.Clamp(p * 1.25f, 0, 1);
                    pivotPos.Y -= tuck * 0.35f;
                }
                break;
            }
            case PAction.HitStun:
            {
                fadeIn = 0.05f;
                float stun = Math.Max(6, st.StunTicks);
                if (Clip("hit") is { } c)
                {
                    float speed = Mathf.Clamp(c.Length / (stun * Cfg.Dt), 1f, 1.8f);
                    c.Sample(c.Start + k * Cfg.Dt * speed, false, _tgt.Q, out _tgt.Hip);
                }
                else
                {
                    _tgt.CopyFrom(_loco);
                    flinch = 1 - Mathf.Clamp(k / stun, 0, 1);
                }
                break;
            }
            case PAction.GuardBreak:
            {
                if ((Clip("guardbreak") ?? Clip("hit")) is { } c)
                    c.Sample(c.Start + k * Cfg.Dt * 0.8f, false, _tgt.Q, out _tgt.Hip);
                else
                {
                    _tgt.CopyFrom(_loco);
                    flinch = 1;
                }
                pivotRot.Z = Mathf.Sin((float)t * 25) * 0.05f;
                break;
            }
            case PAction.Dead:
            {
                fadeIn = 0.1f;
                key = -2;
                if (_deadAt < 0) _deadAt = t;
                float since = (float)(t - _deadAt);
                if (Clip("death") is { } c) c.Sample(c.Start + since, false, _tgt.Q, out _tgt.Hip);
                else
                {
                    _tgt.CopyFrom(_loco);
                    float f = Mathf.Clamp(since / 0.5f, 0, 1);
                    pivotRot.Z = 1.45f * f;
                    pivotPos.Y = Mathf.Lerp(_cs.HipHeight, 0.3f, f);
                }
                break;
            }
        }
        if (st.Action != PAction.Dead) _deadAt = -1;

        if (key != _key)
        {
            _from.CopyFrom(_out);
            _fade = 0;
            _fadeDur = fadeIn;
            _key = key;
        }
        _fade = Math.Min(_fade + dt, _fadeDur);
        float w = _fadeDur > 0 ? _fade / _fadeDur : 1;
        _out.Blend(_from, _tgt, w * w * (3 - 2 * w));

        // ปรับท่าเพิ่มด้วยโปรแกรม แล้วส่งให้โครงกระดูก
        for (int j = 0; j < _driven.Length; j++) _sk.SetBonePoseRotation(_driven[j], _out.Q[j]);
        _sk.SetBonePosePosition(_cs.Rig.Hips, _out.Hip);
        Procedural(tuck, flinch, swing);

        // เอียงเข้าโค้งตอนวิ่งเลี้ยว
        float runW = Mathf.Clamp(_speed / Cfg.PlayerSpeed, 0, 1);
        _bank = Mathf.Lerp(_bank, Mathf.Clamp(yawRate * runW * 0.04f, -0.2f, 0.2f), Art.Smooth(8, dt));
        pivotRot.Z += _bank;
        _pivot.Position = pivotPos;
        _pivot.Rotation = pivotRot;
        if (_springs != null)
        {
            // ลมจากความเร็ววิ่ง (แกนของตัวละคร: หน้าคือ -Z) พัดผมไปทางหลัง
            var vel = st.Action == PAction.Dodge ? st.DodgeDir * 11f : st.Vel;
            var local = Basis.Inverse() * new Vector3(vel.X, 0, vel.Y);
            _springs.Step(dt, GlobalTransform, -local * 0.12f);
        }

        // โล่เรืองตอนกัน (ทองช่วงรับพริ้ว), วงแสงตอนสวนกลับได้, ตัวกระพริบแดงตอนโดน
        bool guard = st.Action == PAction.Idle && st.Guarding;
        _guard.Visible = guard;
        if (guard)
        {
            bool fresh = tick - st.GuardStartTick <= Cfg.PerfectGuardWindow;
            _guardMat.AlbedoColor = fresh ? new Color(1f, 0.82f, 0.3f, 0.5f) : new Color(0.48f, 0.77f, 1f, 0.28f);
        }
        _aura.Visible = tick <= st.CounterUntil;
        if (_aura.Visible) _aura.RotationDegrees = new Vector3(0, (float)t * 180, 0);
        if (_lastHp >= 0 && st.Hp < _lastHp) _hurtAt = t;
        _lastHp = st.Hp;
        float flash = Mathf.Clamp(1 - (float)(t - _hurtAt) / 0.18f, 0, 1);
        var fc = new Color(1, 0.25f, 0.2f, flash * 0.9f);
        foreach (var g in _geoms) g.SetInstanceShaderParameter("flash", fc);

        // กะพริบตาทุก 2-5 วินาที หลับตาตอนตาย
        if (_blink.Count > 0)
        {
            if (t > _nextBlink + 0.16) _nextBlink = t + 2 + GD.Randf() * 3;
            float since = (float)(t - _nextBlink);
            float eye = st.Action == PAction.Dead ? 1 : since is >= 0 and < 0.16f ? Mathf.Sin(since / 0.16f * Mathf.Pi) : 0;
            foreach (var (mi, shape, weight) in _blink) mi.SetBlendShapeValue(shape, eye * weight);
        }
    }

    void Blade(float energy) =>
        _bladeMat.SetShaderParameter("emission_color", new Color(0.25f, 0.55f, 0.75f) * energy * 0.6f);

    // ---- เดิน/วิ่ง: จังหวะก้าวเดินตามระยะทางจริง เท้าจึงไม่ไถล ----
    void Locomotion(in PlayerState st, float dt, out bool stepped)
    {
        stepped = false;
        var idle = Clip("idle");
        var walk = Clip("walk") ?? Clip("run");
        var run = Clip("run") ?? walk;
        float s = _speed;
        float vW = _cs.WalkSpeed, vR = _cs.RunSpeed;

        // เดินถอยหลังหรือเดินเฉียงตอนยกโล่ (หันหน้าหาศัตรูแต่เดินไปทางอื่น)
        float rel = 0;
        bool back = false;
        if (s > 0.3f && st.Vel.LengthSquared() > 0.01f)
        {
            rel = Mathf.Wrap(MathF.Atan2(st.Vel.Y, st.Vel.X) - st.Facing, -Mathf.Pi, Mathf.Pi);
            if (Math.Abs(rel) > 1.75f)
            {
                back = true;
                rel = Mathf.Wrap(rel - Mathf.Pi, -Mathf.Pi, Mathf.Pi);
            }
            rel = Mathf.Clamp(rel, -1.2f, 1.2f);
        }
        _legYaw = Mathf.LerpAngle(_legYaw, rel, Art.Smooth(10, dt));

        if (idle != null) idle.Sample(idle.Start + (float)_idleT, true, _a.Q, out _a.Hip);
        else if (walk != null) walk.Sample(0, true, _a.Q, out _a.Hip);
        else _a.CopyFrom(_rest);

        if (walk == null || run == null)
        {
            _loco.CopyFrom(_a);
        }
        else
        {
            float u = s <= vW ? 0 : Mathf.Clamp((s - vW) / Math.Max(0.1f, vR - vW), 0, 1);
            float stride = Mathf.Lerp(vW * walk.Length, vR * run.Length, u);
            float dp = (back ? -1 : 1) * s * dt / Math.Max(0.2f, stride);
            _phase = Mathf.PosMod(_phase + dp, 1f);
            // เสียงเท้า: ทุกครึ่งรอบก้าว (เท้าซ้ายหรือขวาไปหน้าสุด)
            double before = _steps;
            _steps += Math.Abs(dp);
            stepped = s > 0.8f && Math.Floor(_steps * 2) != Math.Floor(before * 2);
            walk.Sample((_phase + walk.LeftStrike) * walk.Length, true, _b.Q, out _b.Hip);
            run.Sample((_phase + run.LeftStrike) * run.Length, true, _c.Q, out _c.Hip);
            _loco.Blend(_b, _c, u);
            float wIdle = s < vW ? 1 - s / vW : 0;
            if (s < 0.05f) wIdle = 1;
            _b.CopyFrom(_loco);
            _loco.Blend(_b, _a, wIdle);

        }

        if (st.Guarding && Clip("guard") is { } g)
        {
            g.Sample(g.Start + (float)_idleT, true, _b.Q, out _b.Hip);
            _c.CopyFrom(_loco);
            _loco.Layer(_c, _b, _upper, 1f);
        }
    }

    // เวลาในคลิปท่าโจมตี: ช่วงง้างถูกบีบให้ฟันตรงจังหวะที่ Sim ตัดสิน แล้วเล่นท่าตามต่อ
    static float AttackTime(RigClip c, in MoveDef d, float k)
    {
        float anchor = d.Startup + d.Active * 0.5f;
        float pre = anchor * Cfg.Dt;
        float s0 = Math.Max(c.Start, c.Hit - pre * 3f);
        if (k < anchor) return Mathf.Lerp(s0, c.Hit, Mathf.Clamp(k / Math.Max(anchor, 0.01f), 0, 1));
        float tail = Math.Max(1, d.Total - anchor) * Cfg.Dt;
        float post = Mathf.Clamp((c.End - c.Hit) / tail, 1f, 2f);
        return Math.Min(c.End, c.Hit + (k - anchor) * Cfg.Dt * post);
    }

    float Attack(string role, in MoveDef d, float k)
    {
        if (Clip(role) is { } c)
        {
            c.Sample(AttackTime(c, d, k), false, _tgt.Q, out _tgt.Hip);
            return 0;
        }
        _tgt.CopyFrom(_loco);
        if (k < d.Startup) return k / d.Startup;
        if (k < d.Startup + d.Active) return 1 - 2 * (k - d.Startup) / d.Active;
        return -(1 - Mathf.Clamp((k - d.Startup - d.Active) / Math.Max(1, d.Recover), 0, 1));
    }

    // ---- ขยับเพิ่มด้วยโปรแกรม (หมุนกระดูกในแกนกลาง: +X ซ้ายมือ, +Y ขึ้น, +Z หน้า) ----
    void Procedural(float tuck, float flinch, float swing)
    {
        if (Math.Abs(_legYaw) > 0.01f && Rot("hips", Vector3.Up, -_legYaw))
            Rot("spine", Vector3.Up, _legYaw);
        if (tuck > 0.01f)
        {
            Rot("spine", Vector3.Right, 0.5f * tuck);
            Rot("chest", Vector3.Right, 0.3f * tuck);
            foreach (var s in new[] { "left", "right" })
            {
                Rot(s + "UpperLeg", Vector3.Right, -1.75f * tuck);
                Rot(s + "LowerLeg", Vector3.Right, 2.3f * tuck);
                Rot(s + "UpperArm", Vector3.Back, (s == "left" ? -1.1f : 1.1f) * tuck);
            }
        }
        if (flinch > 0.01f)
        {
            Rot("spine", Vector3.Right, -0.35f * flinch);
            Rot("head", Vector3.Right, -0.25f * flinch);
        }
        if (Math.Abs(swing) > 0.01f)
        {
            Rot("spine", Vector3.Up, 0.5f * swing);
            Rot("rightUpperArm", Vector3.Right, -1.3f);
            Rot("rightUpperArm", Vector3.Up, 1.2f * swing);
        }
    }

    // หมุนกระดูกหนึ่งท่อนรอบแกนในแกนกลาง (ลูก ๆ หมุนตาม)
    bool Rot(string key, Vector3 axisCanon, float angle)
    {
        if (!_slot.TryGetValue(key, out var j)) return false;
        int b = _driven[j];
        var rig = _cs.Rig;
        int p = rig.Parent[b];
        var parentG = p < 0 ? Quaternion.Identity : GlobalRot(p);
        var r = new Quaternion(rig.Skel(axisCanon).Normalized(), angle);
        var local = _sk.GetBonePoseRotation(b);
        _sk.SetBonePoseRotation(b, (parentG.Inverse() * r * parentG * local).Normalized());
        return true;
    }

    Quaternion GlobalRot(int b)
    {
        var q = _sk.GetBonePoseRotation(b);
        for (int p = _cs.Rig.Parent[b]; p >= 0; p = _cs.Rig.Parent[p]) q = _sk.GetBonePoseRotation(p) * q;
        return q.Normalized();
    }
}
