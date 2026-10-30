using Eighth.Sim;
using Godot;

namespace EighthThrone.Demo;

// ตัวละครผู้เล่น: ทำท่าตามสถานะใน Sim (ไม่มีโมเดลจริง ใช้รูปทรงพื้นฐานต่อกัน)
public partial class PlayerModel : Node3D
{
    readonly Node3D _body;
    readonly Node3D _sword;
    readonly Node3D _legL, _legR;
    readonly MeshInstance3D _shield;
    readonly MeshInstance3D _aura;
    readonly StandardMaterial3D _armor;
    readonly StandardMaterial3D _shieldMat;
    readonly StandardMaterial3D _blade;
    float _yaw;
    float _walk;
    Vector3 _last;
    int _lastHp = -1;
    double _hurtAt = -10;

    const float Hip = 0.9f;

    public PlayerModel()
    {
        _armor = Art.Mat(new Color("2f7f86"), 0.45f, 0.5f);
        var dark = Art.Mat(new Color("1d2430"), 0.7f, 0.3f);
        var skin = Art.Mat(new Color("e0b48f"), 0.8f);
        var cape = Art.Mat(new Color("5a1622"), 0.9f);
        _blade = Art.Mat(new Color("dfe9f2"), 0.2f, 0.9f, new Color("8fd8ff"), 0.4f);

        _body = Art.Joint(this, new Vector3(0, Hip, 0));
        Art.Part(_body, Art.Capsule(0.3f, 1.0f), _armor, new Vector3(0, 0.25f, 0));
        Art.Part(_body, Art.Box(0.6f, 0.12f, 0.3f), dark, new Vector3(0, 0.62f, 0));
        Art.Part(_body, Art.Sphere(0.19f), skin, new Vector3(0, 0.92f, 0));
        Art.Part(_body, Art.Sphere(0.215f), dark, new Vector3(0, 0.99f, 0.04f), scale: new Vector3(1, 0.8f, 1));
        Art.Part(_body, Art.Prism(0.5f, 1.0f, 0.04f), cape, new Vector3(0, 0.1f, 0.27f), new Vector3(172, 0, 0));
        Art.Part(_body, Art.Capsule(0.08f, 0.62f), _armor, new Vector3(-0.38f, 0.32f, -0.05f), new Vector3(10, 0, -12));

        _sword = Art.Joint(_body, new Vector3(0.36f, 0.45f, -0.1f));
        Art.Part(_sword, Art.Capsule(0.08f, 0.5f), _armor, new Vector3(0, 0, -0.18f), new Vector3(90, 0, 0));
        Art.Part(_sword, Art.Box(0.24f, 0.05f, 0.06f), dark, new Vector3(0, 0, -0.42f));
        Art.Part(_sword, Art.Box(0.07f, 0.03f, 1.05f), _blade, new Vector3(0, 0, -0.98f));

        _legL = Art.Joint(this, new Vector3(-0.14f, Hip - 0.05f, 0));
        _legR = Art.Joint(this, new Vector3(0.14f, Hip - 0.05f, 0));
        foreach (var leg in new[] { _legL, _legR })
            Art.Part(leg, Art.Capsule(0.11f, 0.85f), dark, new Vector3(0, -0.42f, 0));

        _shieldMat = Art.Clear(new Color(0.48f, 0.77f, 1f, 0.35f));
        _shield = Art.Part(_body, Art.Cyl(0.62f, 0.62f, 0.03f, 32), _shieldMat, new Vector3(0, 0.25f, -0.62f),
            new Vector3(90, 0, 0), shadow: false);
        _aura = Art.Part(this, Art.Torus(0.6f, 0.75f), Art.Mat(new Color("f5c542"), 0.3f, 0, new Color("f5c542"), 3f),
            new Vector3(0, 0.04f, 0), scale: new Vector3(1, 0.2f, 1), shadow: false);
    }

    public void Sync(in PlayerState st, Vector3 pos, int tick, float alpha, double t, float dt)
    {
        Position = pos;
        _yaw = Mathf.LerpAngle(_yaw, Art.Yaw(st.Facing), Art.Smooth(22, dt));
        Rotation = new Vector3(0, _yaw, 0);

        float speed = dt > 0 ? (pos - _last).Length() / dt : 0;
        _last = pos;
        bool moving = st.Action == PAction.Idle && speed > 0.6f;
        _walk = moving ? _walk + dt * 11f : Mathf.Lerp(_walk, Mathf.Round(_walk / Mathf.Pi) * Mathf.Pi, Art.Smooth(10, dt));
        float swing = Mathf.Sin(_walk) * 0.75f;
        _legL.Rotation = new Vector3(swing, 0, 0);
        _legR.Rotation = new Vector3(-swing, 0, 0);

        var body = new Vector3(0, Hip + (moving ? Mathf.Abs(Mathf.Sin(_walk)) * 0.06f : 0), 0);
        var bodyRot = Vector3.Zero;
        var swordRot = new Vector3(-0.85f, 0.35f, 0);
        var swordPos = new Vector3(0.36f, 0.45f, -0.1f);
        bool guard = false;
        _blade.EmissionEnergyMultiplier = 0.4f;

        float k = st.ActionTick + alpha;
        switch (st.Action)
        {
            case PAction.Light:
            {
                var d = Moves.Light[st.Combo];
                float dir = st.Combo % 2 == 0 ? 1 : -1;
                if (st.Combo == 3) dir = 1;
                swordRot = new Vector3(st.Combo == 3 ? 0.25f : 0.05f, Swing(k, d, dir, 1.4f), 0);
                bodyRot.Y = -swordRot.Y * 0.25f;
                break;
            }
            case PAction.Heavy:
            case PAction.Finisher:
            {
                var d = st.Action == PAction.Heavy ? Moves.Heavy : Moves.Finisher;
                swordRot = new Vector3(Overhead(k, d), 0, 0);
                bodyRot.X = -0.25f * Mathf.Clamp((k - d.Startup) / Mathf.Max(1, d.Active), 0, 1);
                if (st.Action == PAction.Finisher)
                {
                    _blade.EmissionEnergyMultiplier = 4f;
                    body.Y += Mathf.Sin(Mathf.Clamp(k / d.Startup, 0, 1) * Mathf.Pi) * 0.7f;
                }
                break;
            }
            case PAction.Skill1:
            {
                // พุ่งแล้วแทงสามครั้งตามจังหวะโดน
                swordRot = new Vector3(0, 0, 0);
                float jab = 0;
                foreach (int h in Moves.Skill1HitTicks)
                    jab = Mathf.Max(jab, 1 - Mathf.Abs(k - h - 0.5f) / 2.5f);
                swordPos.Z -= jab * 0.5f;
                bodyRot.X = -0.3f;
                _blade.EmissionEnergyMultiplier = 1.5f;
                break;
            }
            case PAction.Skill2:
            {
                var d = Moves.Skill2;
                float wind = Mathf.Clamp(k / d.Startup, 0, 1);
                float spin = Mathf.Clamp((k - d.Startup) / (d.Active + 4f), 0, 1);
                swordRot = new Vector3(0.1f, 1.4f, 0);
                body.Y -= 0.25f * wind * (1 - spin);
                bodyRot.Y = Mathf.Tau * spin;
                _blade.EmissionEnergyMultiplier = 0.4f + 3f * wind;
                break;
            }
            case PAction.Dodge:
            {
                float p = Mathf.Clamp(k / Cfg.DodgeTicks, 0, 1);
                bodyRot.X = -Mathf.Tau * Mathf.Clamp(p * 1.25f, 0, 1);
                body.Y -= Mathf.Sin(p * Mathf.Pi) * 0.45f;
                break;
            }
            case PAction.HitStun:
                bodyRot.X = 0.4f * (1 - Mathf.Clamp(k / Mathf.Max(1, st.StunTicks), 0, 1));
                break;
            case PAction.GuardBreak:
                bodyRot.X = 0.55f;
                bodyRot.Z = Mathf.Sin((float)t * 25) * 0.08f;
                swordRot = new Vector3(-1.3f, 0.9f, 0);
                break;
            case PAction.Dead:
                bodyRot.Z = 1.45f;
                body.Y = 0.35f;
                break;
            case PAction.Idle when st.Guarding:
                guard = true;
                swordRot = new Vector3(0.25f, -1.25f, 0);
                break;
        }

        _body.Position = body;
        _body.Rotation = bodyRot;
        _sword.Position = swordPos;
        _sword.Rotation = swordRot;

        _shield.Visible = guard;
        if (guard)
        {
            bool fresh = tick - st.GuardStartTick <= Cfg.PerfectGuardWindow;
            _shieldMat.AlbedoColor = fresh ? new Color(1f, 0.82f, 0.3f, 0.55f) : new Color(0.48f, 0.77f, 1f, 0.35f);
        }
        _aura.Visible = tick <= st.CounterUntil;
        if (_aura.Visible) _aura.RotationDegrees = new Vector3(0, (float)t * 180, 0);

        if (_lastHp >= 0 && st.Hp < _lastHp) _hurtAt = t;
        _lastHp = st.Hp;
        float flash = Mathf.Clamp(1 - (float)(t - _hurtAt) / 0.18f, 0, 1);
        _armor.EmissionEnabled = flash > 0;
        _armor.Emission = new Color(1, 0.25f, 0.2f);
        _armor.EmissionEnergyMultiplier = flash * 2.5f;
    }

    // ฟันแนวนอน: ง้าง -> ฟันผ่าน -> คืนท่า
    static float Swing(float k, in MoveDef d, float dir, float arc)
    {
        if (k < d.Startup) return dir * arc * (k / d.Startup);
        if (k < d.Startup + d.Active) return dir * arc * (1 - 2 * (k - d.Startup) / d.Active);
        float r = Mathf.Clamp((k - d.Startup - d.Active) / Mathf.Max(1, d.Recover), 0, 1);
        return -dir * arc * (1 - r) + 0.35f * r;
    }

    // ฟันจากเหนือหัวลงพื้น
    static float Overhead(float k, in MoveDef d)
    {
        if (k < d.Startup) return Mathf.Lerp(-0.85f, 2.3f, k / d.Startup);
        if (k < d.Startup + d.Active) return Mathf.Lerp(2.3f, -0.9f, (k - d.Startup) / d.Active);
        return -0.9f;
    }
}

// ดาวหมุนเหนือหัวตอนเสียหลัก
public partial class Stars : Node3D
{
    public Stars()
    {
        var m = Art.Mat(new Color("ffe066"), 0.3f, 0, new Color("ffe066"), 3f);
        for (int i = 0; i < 3; i++)
        {
            float a = i * Mathf.Tau / 3;
            Art.Part(this, Art.Prism(0.18f, 0.18f, 0.06f), m, new Vector3(Mathf.Cos(a) * 0.45f, 0, Mathf.Sin(a) * 0.45f), shadow: false);
        }
    }

    public void Spin(double t) => Rotation = new Vector3(0, (float)t * 4, 0);
}

// หมาป่าหนาม
public partial class WolfModel : Node3D
{
    readonly Node3D _body;
    readonly Node3D[] _legs = new Node3D[4];
    readonly StandardMaterial3D _eyes;
    readonly StandardMaterial3D _fur;
    readonly Stars _stars;
    float _yaw;
    int _lastHp = -1;
    double _hurtAt = -10;
    double _deadAt = -1;

    public WolfModel()
    {
        _fur = Art.Mat(new Color("4b413b"), 0.95f);
        var belly = Art.Mat(new Color("2f2925"), 0.95f);
        var bone = Art.Mat(new Color("d9cfb8"), 0.6f);
        _eyes = Art.Mat(new Color("ffcf4d"), 0.3f, 0, new Color("ffcf4d"), 1.5f);

        _body = Art.Joint(this, new Vector3(0, 0.62f, 0));
        Art.Part(_body, Art.Capsule(0.3f, 1.25f), _fur, Vector3.Zero, new Vector3(90, 0, 0));
        Art.Part(_body, Art.Capsule(0.24f, 0.9f), belly, new Vector3(0, -0.08f, 0.05f), new Vector3(90, 0, 0));
        for (int i = 0; i < 4; i++)
            Art.Part(_body, Art.Prism(0.16f, 0.32f - i * 0.04f, 0.14f), bone, new Vector3(0, 0.33f, -0.3f + i * 0.22f), new Vector3(-20, 0, 0));
        Art.Part(_body, Art.Sphere(0.24f), _fur, new Vector3(0, 0.16f, -0.72f));
        Art.Part(_body, Art.Box(0.2f, 0.16f, 0.32f), _fur, new Vector3(0, 0.08f, -0.98f));
        Art.Part(_body, Art.Prism(0.1f, 0.2f, 0.08f), _fur, new Vector3(-0.12f, 0.42f, -0.68f));
        Art.Part(_body, Art.Prism(0.1f, 0.2f, 0.08f), _fur, new Vector3(0.12f, 0.42f, -0.68f));
        Art.Part(_body, Art.Sphere(0.045f), _eyes, new Vector3(-0.1f, 0.22f, -0.9f), shadow: false);
        Art.Part(_body, Art.Sphere(0.045f), _eyes, new Vector3(0.1f, 0.22f, -0.9f), shadow: false);
        Art.Part(_body, Art.Capsule(0.07f, 0.6f), _fur, new Vector3(0, 0.15f, 0.82f), new Vector3(60, 0, 0));

        float[] xs = { -0.17f, 0.17f, -0.17f, 0.17f };
        float[] zs = { -0.42f, -0.42f, 0.4f, 0.4f };
        for (int i = 0; i < 4; i++)
        {
            _legs[i] = Art.Joint(this, new Vector3(xs[i], 0.5f, zs[i]));
            Art.Part(_legs[i], Art.Capsule(0.075f, 0.55f), belly, new Vector3(0, -0.24f, 0));
        }
        _stars = new Stars { Position = new Vector3(0, 1.35f, -0.5f), Visible = false };
        AddChild(_stars);
    }

    public void Sync(in EnemyRender e, double t, float dt)
    {
        var v = e.V;
        Position = Art.V3(e.Pos);
        _yaw = Mathf.LerpAngle(_yaw, Art.Yaw(e.Facing), Art.Smooth(14, dt));
        Rotation = new Vector3(0, _yaw, 0);

        var body = new Vector3(0, 0.62f, 0);
        var rot = Vector3.Zero;
        float gait = 0;
        float eyes = 1.5f;
        _stars.Visible = false;

        switch (v.Phase)
        {
            case EPhase.Chase:
                gait = 1;
                body.Y += Mathf.Abs(Mathf.Sin((float)t * 13)) * 0.06f;
                break;
            case EPhase.Windup:
            {
                float p = Telegraph.Progress(e);
                body.Y -= 0.16f * p;
                body.Z += 0.15f * p;
                rot.X = 0.22f * p;
                eyes = 1.5f + 5f * p;
                break;
            }
            case EPhase.Active:
                rot.X = -0.22f;
                gait = 2;
                eyes = 6;
                break;
            case EPhase.Flinch:
                body.X += Mathf.Sin((float)t * 70) * 0.06f;
                rot.X = 0.15f;
                break;
            case EPhase.Staggered:
                rot.Z = 1.1f;
                body.Y = 0.42f;
                eyes = 0.3f;
                _stars.Visible = true;
                _stars.Spin(t);
                break;
            case EPhase.Dead:
                if (_deadAt < 0) _deadAt = t;
                rot.Z = 1.5f;
                body.Y = 0.38f - Mathf.Clamp((float)(t - _deadAt - 1.0) * 0.4f, 0, 1.2f);
                eyes = 0;
                break;
        }

        _body.Position = body;
        _body.Rotation = rot;
        for (int i = 0; i < 4; i++)
        {
            float ph = (float)t * 13 + (i is 0 or 3 ? 0 : Mathf.Pi);
            _legs[i].Rotation = new Vector3(gait == 2 ? (i < 2 ? -0.9f : 0.9f) : Mathf.Sin(ph) * 0.7f * gait, 0, 0);
            _legs[i].Visible = v.Phase is not (EPhase.Dead or EPhase.Staggered);
        }
        _eyes.EmissionEnergyMultiplier = eyes;

        if (_lastHp >= 0 && v.Hp < _lastHp) _hurtAt = t;
        _lastHp = v.Hp;
        float flash = Mathf.Clamp(1 - (float)(t - _hurtAt) / 0.12f, 0, 1);
        _fur.EmissionEnabled = flash > 0;
        _fur.Emission = Colors.White;
        _fur.EmissionEnergyMultiplier = flash * 1.5f;
    }
}

// ราชาหมึก: ตัวหมึกยักษ์สวมมงกุฎ มีหนวดแปดเส้น
public partial class BossModel : Node3D
{
    const int Arms = 8;
    const int Segs = 5;
    const float SegLen = 0.62f;

    readonly Node3D _mantle;
    readonly Node3D[] _armRoot = new Node3D[Arms];
    readonly Node3D[][] _joints = new Node3D[Arms][];
    readonly StandardMaterial3D _skin;
    readonly StandardMaterial3D _eyes;
    readonly Stars _stars;
    float _yaw;
    int _lastHp = -1;
    double _hurtAt = -10;
    double _deadAt = -1;
    float _raise, _sweep, _glow;

    public BossModel()
    {
        _skin = Art.Mat(new Color("4a2a78"), 0.5f, 0.1f);
        var under = Art.Mat(new Color("7d4fb0"), 0.6f);
        var gold = Art.Mat(new Color("d8b04c"), 0.3f, 0.9f, new Color("f5c542"), 0.5f);
        _eyes = Art.Mat(new Color("ffe680"), 0.2f, 0, new Color("ffd84d"), 4f);

        _mantle = Art.Joint(this, new Vector3(0, 2.5f, 0));
        Art.Part(_mantle, Art.Sphere(1f), _skin, Vector3.Zero, scale: new Vector3(1.3f, 1.55f, 1.3f));
        Art.Part(_mantle, Art.Torus(0.9f, 1.35f), under, new Vector3(0, -1.0f, 0), scale: new Vector3(1, 0.6f, 1));
        Art.Part(_mantle, Art.Sphere(0.17f), _eyes, new Vector3(-0.42f, -0.2f, -1.15f), shadow: false);
        Art.Part(_mantle, Art.Sphere(0.17f), _eyes, new Vector3(0.42f, -0.2f, -1.15f), shadow: false);
        Art.Part(_mantle, Art.Torus(0.55f, 0.7f), gold, new Vector3(0, 1.42f, 0), scale: new Vector3(1, 0.5f, 1));
        for (int i = 0; i < 5; i++)
        {
            float a = i * Mathf.Tau / 5;
            Art.Part(_mantle, Art.Prism(0.22f, 0.5f, 0.22f), gold,
                new Vector3(Mathf.Cos(a) * 0.62f, 1.72f, Mathf.Sin(a) * 0.62f));
        }

        for (int i = 0; i < Arms; i++)
        {
            float a = i * Mathf.Tau / Arms;
            var root = Art.Joint(this, new Vector3(0, 1.45f, 0));
            root.Rotation = new Vector3(0, a, 0);
            _armRoot[i] = root;
            var j = new Node3D[Segs];
            Node3D parent = Art.Joint(root, new Vector3(0, 0, -0.85f));
            for (int k = 0; k < Segs; k++)
            {
                j[k] = parent;
                float r = 0.24f - k * 0.04f;
                Art.Part(j[k], Art.Capsule(r, SegLen + r), k % 2 == 0 ? _skin : under, new Vector3(0, 0, -SegLen / 2), new Vector3(90, 0, 0));
                parent = Art.Joint(j[k], new Vector3(0, 0, -SegLen));
            }
            _joints[i] = j;
        }
        _stars = new Stars { Position = new Vector3(0, 5.0f, 0), Visible = false, Scale = Vector3.One * 2 };
        AddChild(_stars);
    }

    public void Sync(in EnemyRender e, double t, float dt)
    {
        var v = e.V;
        Position = Art.V3(e.Pos);
        _yaw = Mathf.LerpAngle(_yaw, Art.Yaw(e.Facing), Art.Smooth(6, dt));
        Rotation = new Vector3(0, _yaw, 0);

        float raise = 0, sweep = 0, glow = 0;
        var mantleRot = Vector3.Zero;
        float mantleY = 2.5f + Mathf.Sin((float)t * 1.6f) * 0.08f;
        float eyes = 4;
        _stars.Visible = false;

        if (v.Phase == EPhase.Windup && v.Attack != AttackId.None)
        {
            float p = Telegraph.Progress(e);
            switch (v.Attack)
            {
                case AttackId.BossSweep: sweep = p; break;
                case AttackId.BossInkPool: glow = p; break;
                default: raise = p; break;
            }
            eyes = 4 + 6 * p;
        }
        else if (v.Phase == EPhase.Active)
        {
            raise = v.Attack is AttackId.BossSlam or AttackId.BossFeint ? -1 : 0;
            sweep = v.Attack == AttackId.BossSweep ? -1 : 0;
            glow = v.Attack == AttackId.BossInkPool ? 1.3f : 0;
            eyes = 10;
        }
        else if (v.Phase == EPhase.Staggered)
        {
            mantleRot.X = 0.35f;
            mantleY -= 0.6f;
            eyes = 0.5f;
            _stars.Visible = true;
            _stars.Spin(t);
        }
        else if (v.Phase == EPhase.Dead)
        {
            if (_deadAt < 0) _deadAt = t;
            float d = Mathf.Clamp((float)(t - _deadAt) / 2.5f, 0, 1);
            mantleY -= 2.6f * d;
            mantleRot.X = 0.6f * d;
            eyes = 4 * (1 - d);
        }

        _raise = Mathf.Lerp(_raise, raise, Art.Smooth(raise < 0 ? 40 : 10, dt));
        _sweep = Mathf.Lerp(_sweep, sweep, Art.Smooth(sweep < 0 ? 25 : 8, dt));
        _glow = Mathf.Lerp(_glow, glow, Art.Smooth(10, dt));

        _mantle.Position = new Vector3(0, mantleY, 0);
        _mantle.Rotation = mantleRot;
        _eyes.EmissionEnergyMultiplier = eyes;
        bool hurt = t - _hurtAt < 0.1;
        _skin.EmissionEnabled = _glow > 0.02f || hurt;
        _skin.Emission = hurt ? new Color("ffd9f0") : new Color("7a2fd6");
        _skin.EmissionEnergyMultiplier = hurt ? 0.3f : _glow * 0.9f;

        for (int i = 0; i < Arms; i++)
        {
            float a = i * Mathf.Tau / Arms;
            float front = Mathf.Max(0, Mathf.Cos(a));      // หนวดที่อยู่ด้านหน้า (แกน -Z)
            float right = Mathf.Max(0, Mathf.Sin(a));
            _armRoot[i].Rotation = new Vector3(0, a + _sweep * 0.9f * right, 0);
            float basePitch = -1.0f + front * (_raise > 0 ? 2.3f * _raise : 0.9f * _raise);
            if (v.Phase == EPhase.Dead || v.Phase == EPhase.Staggered) basePitch = -1.05f;
            for (int k = 0; k < Segs; k++)
            {
                float wave = Mathf.Sin((float)t * 2.2f + i * 0.9f + k * 0.7f) * 0.13f;
                float pitch = k == 0 ? basePitch : 0.28f + wave - (front * _raise > 0.3f ? 0.2f : 0);
                _joints[i][k].Rotation = new Vector3(pitch, wave * 0.6f, 0);
            }
        }

        if (_lastHp >= 0 && v.Hp < _lastHp) _hurtAt = t;
        _lastHp = v.Hp;
    }
}
