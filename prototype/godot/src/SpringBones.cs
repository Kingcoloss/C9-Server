using Godot;
using GDict = Godot.Collections.Dictionary;

namespace EighthThrone.Demo;

// ผมและกระโปรงแกว่งตามการเคลื่อนไหว (VRM spring bone แบบเดียวกับ UniVRM/three-vrm)
// อ่านค่าความแข็ง แรงโน้มถ่วง แรงหน่วง และทรงกลมกันทะลุจากไฟล์ VRM
public sealed class SpringSpec
{
    public sealed class Joint
    {
        public int Bone;
        public Vector3 Tail;      // ปลายกระดูกในแกนของกระดูกเอง (ท่าพัก)
        public float Stiff, Grav, Drag, Radius;
        public Vector3 GravDir;
    }

    public readonly List<List<Joint>> Chains = new();
    public readonly List<(int Bone, Vector3 Offset, float Radius)> Colliders = new();
    public int JointCount => Chains.Sum(c => c.Count);

    public static SpringSpec? Parse(LoadedScene ls, Skeleton3D sk)
    {
        var nodes = ls.State.GetNodes();
        int Bone(Variant v)
        {
            int i = v.AsInt32();
            return i >= 0 && i < nodes.Count ? sk.FindBone(nodes[i].ResourceName) : -1;
        }
        static Vector3 V(Variant v)
        {
            if (v.VariantType == Variant.Type.Array)
            {
                var a = v.AsGodotArray();
                return a.Count >= 3 ? new Vector3((float)a[0], (float)a[1], (float)a[2]) : Vector3.Zero;
            }
            var d = v.AsGodotDictionary();
            return new Vector3((float)d["x"], (float)d["y"], (float)d["z"]);
        }
        static float F(GDict d, string k, float def) => d.TryGetValue(k, out var v) ? (float)v : def;

        var spec = new SpringSpec();
        if (Humanoid.Dig(ls.Json, "extensions", "VRMC_springBone") is { } s1)
        {
            // VRM 1.0: springs[].joints[] เรียงจากโคนถึงปลาย ค่าของข้อต่อใช้กับท่อนถึงข้อถัดไป
            if (s1.TryGetValue("colliders", out var cols))
                foreach (var cv in cols.AsGodotArray())
                {
                    var c = cv.AsGodotDictionary();
                    int b = Bone(c["node"]);
                    var shape = Humanoid.Dig(c, "shape");
                    if (b < 0 || shape == null) continue;
                    if (Humanoid.Dig(shape, "sphere") is { } sp)
                        spec.Colliders.Add((b, sp.TryGetValue("offset", out var o) ? V(o) : Vector3.Zero, F(sp, "radius", 0)));
                    else if (Humanoid.Dig(shape, "capsule") is { } cp)
                    {
                        float r = F(cp, "radius", 0);
                        var a = cp.TryGetValue("offset", out var o) ? V(o) : Vector3.Zero;
                        var t = cp.TryGetValue("tail", out var tv) ? V(tv) : a;
                        for (int k = 0; k <= 3; k++) spec.Colliders.Add((b, a.Lerp(t, k / 3f), r));
                    }
                }
            if (s1.TryGetValue("springs", out var springs))
                foreach (var sv in springs.AsGodotArray())
                {
                    var joints = sv.AsGodotDictionary()["joints"].AsGodotArray();
                    var chain = new List<Joint>();
                    for (int i = 0; i + 1 < joints.Count; i++)
                    {
                        var j = joints[i].AsGodotDictionary();
                        int b = Bone(j["node"]), next = Bone(joints[i + 1].AsGodotDictionary()["node"]);
                        if (b < 0 || next < 0) continue;
                        chain.Add(new Joint
                        {
                            Bone = b,
                            Tail = sk.GetBoneRest(next).Origin,
                            Stiff = F(j, "stiffness", 1), Grav = F(j, "gravityPower", 0), Drag = F(j, "dragForce", 0.5f),
                            Radius = F(j, "hitRadius", 0),
                            GravDir = j.TryGetValue("gravityDir", out var g) ? V(g) : Vector3.Down,
                        });
                    }
                    if (chain.Count > 0) spec.Chains.Add(chain);
                }
        }
        else if (Humanoid.Dig(ls.Json, "extensions", "VRM", "secondaryAnimation") is { } s0)
        {
            // VRM 0.x: แต่ละกลุ่มระบุกระดูกโคน ลูกหลานทั้งหมดแกว่งด้วยค่าเดียวกัน (แกน Z กลับด้าน)
            if (s0.TryGetValue("colliderGroups", out var cgs))
                foreach (var gv in cgs.AsGodotArray())
                {
                    var g = gv.AsGodotDictionary();
                    int b = Bone(g["node"]);
                    if (b < 0) continue;
                    foreach (var cv in g["colliders"].AsGodotArray())
                    {
                        var c = cv.AsGodotDictionary();
                        var o = c.TryGetValue("offset", out var ov) ? V(ov) : Vector3.Zero;
                        spec.Colliders.Add((b, new Vector3(o.X, o.Y, -o.Z), F(c, "radius", 0)));
                    }
                }
            if (s0.TryGetValue("boneGroups", out var groups))
                foreach (var gv in groups.AsGodotArray())
                {
                    var g = gv.AsGodotDictionary();
                    float stiff = F(g, "stiffiness", 1), grav = F(g, "gravityPower", 0), drag = F(g, "dragForce", 0.4f);
                    float radius = F(g, "hitRadius", 0.02f);
                    var gdir = g.TryGetValue("gravityDir", out var gd) ? V(gd) : Vector3.Down;
                    gdir.Z = -gdir.Z;
                    foreach (var rv in g["bones"].AsGodotArray())
                    {
                        int root = Bone(rv);
                        if (root < 0) continue;
                        var chain = new List<Joint>();
                        void Walk(int b)
                        {
                            var kids = sk.GetBoneChildren(b);
                            var tail = kids.Length > 0 ? sk.GetBoneRest(kids[0]).Origin
                                : sk.GetBoneRest(b).Origin.Normalized() * 0.07f;
                            if (tail.LengthSquared() > 1e-8f)
                                chain.Add(new Joint { Bone = b, Tail = tail, Stiff = stiff, Grav = grav, Drag = drag, Radius = radius, GravDir = gdir });
                            foreach (int k in kids) Walk(k);
                        }
                        Walk(root);
                        if (chain.Count > 0) spec.Chains.Add(chain);
                    }
                }
        }
        return spec.Chains.Count > 0 ? spec : null;
    }
}

public sealed class SpringSim
{
    readonly Skeleton3D _sk;
    readonly SpringSpec _spec;
    readonly Vector3[][] _cur, _prev;
    readonly Vector3[] _colC;
    readonly float[] _colR;
    bool _init;

    public SpringSim(Skeleton3D sk, SpringSpec spec)
    {
        _sk = sk;
        _spec = spec;
        _cur = spec.Chains.Select(c => new Vector3[c.Count]).ToArray();
        _prev = spec.Chains.Select(c => new Vector3[c.Count]).ToArray();
        _colC = new Vector3[spec.Colliders.Count];
        _colR = new float[spec.Colliders.Count];
    }

    public void Reset() => _init = false;

    // คำนวณในแกนของตัวละคร (center) การเดินและหมุนตัวทั้งตัวจึงไม่เหวี่ยงผมจนผิดธรรมชาติ
    // แต่ท่าของร่างกาย (วิ่ง ฟัน ตีลังกา) ยังทำให้ผมแกว่ง และมีลมจากความเร็วพัดผมไปด้านหลัง
    public void Step(float dt, Transform3D center, Vector3 wind)
    {
        if (dt <= 0) return;
        dt = Math.Min(dt, 1f / 30f);
        var w = center.AffineInverse() * _sk.GlobalTransform;
        var down = (center.Basis.Inverse() * Vector3.Down).Normalized();
        float scale = w.Basis.Scale.X;
        for (int i = 0; i < _colC.Length; i++)
        {
            var (b, off, r) = _spec.Colliders[i];
            _colC[i] = w * (_sk.GetBoneGlobalPose(b) * off);
            _colR[i] = r * scale;
        }
        var wq = w.Basis.Orthonormalized().GetRotationQuaternion();
        var wqi = wq.Inverse();

        for (int c = 0; c < _spec.Chains.Count; c++)
        {
            var chain = _spec.Chains[c];
            for (int i = 0; i < chain.Count; i++)
            {
                var j = chain[i];
                int p = _sk.GetBoneParent(j.Bone);
                var parentG = p >= 0 ? _sk.GetBoneGlobalPose(p) : Transform3D.Identity;
                var restG = parentG * _sk.GetBoneRest(j.Bone);
                var head = w * restG.Origin;
                var restTail = w * (restG * j.Tail);
                float len = (restTail - head).Length();
                if (len < 1e-6f) continue;
                var restDir = (restTail - head) / len;

                if (!_init || (_cur[c][i] - head).Length() > len * 4)
                {
                    _cur[c][i] = _prev[c][i] = restTail;
                }
                var cur = _cur[c][i];
                var gdir = j.GravDir.IsEqualApprox(Vector3.Down) ? down : j.GravDir;
                var next = cur + (cur - _prev[c][i]) * (1 - j.Drag)
                           + restDir * (j.Stiff * dt)
                           + gdir * (j.Grav * dt) + wind * dt;
                next = head + (next - head).Normalized() * len;
                float hr = j.Radius * scale;
                for (int k = 0; k < _colC.Length; k++)
                {
                    var d = next - _colC[k];
                    float min = _colR[k] + hr;
                    if (d.LengthSquared() < min * min && d.LengthSquared() > 1e-10f)
                    {
                        next = _colC[k] + d.Normalized() * min;
                        next = head + (next - head).Normalized() * len;
                    }
                }
                _prev[c][i] = cur;
                _cur[c][i] = next;

                var to = (next - head).Normalized();
                var delta = restDir.Dot(to) > 0.99999f ? Quaternion.Identity : new Quaternion(restDir, to);
                var restRot = restG.Basis.Orthonormalized().GetRotationQuaternion();
                var g = (wqi * delta * wq * restRot).Normalized();
                var parentRot = parentG.Basis.Orthonormalized().GetRotationQuaternion();
                _sk.SetBonePoseRotation(j.Bone, (parentRot.Inverse() * g).Normalized());
            }
        }
        _init = true;
    }
}

// ตรวจทิศของทรงกลมกันทะลุ: ในท่าพัก ปลายผมไม่ควรจมอยู่ในทรงกลม (เรียกด้วย --spring-test=ไฟล์.vrm)
public static class SpringTest
{
    public static void Run(string path)
    {
        var ls = RigLoad.Load(path);
        if (ls?.Skeleton is not { } sk) return;
        var spec = SpringSpec.Parse(ls, sk);
        if (spec == null) { GD.Print("ไม่มี spring bone"); return; }
        foreach (float flip in new[] { 1f, -1f })
        {
            int inside = 0, total = 0;
            float worst = 0;
            foreach (var chain in spec.Chains)
                foreach (var j in chain)
                {
                    var tail = sk.GetBoneGlobalRest(j.Bone) * j.Tail;
                    total++;
                    foreach (var (b, off, r) in spec.Colliders)
                    {
                        var o = new Vector3(off.X, off.Y, off.Z * flip);
                        var c = sk.GetBoneGlobalRest(b) * o;
                        float pen = r + j.Radius - (tail - c).Length();
                        if (pen > 0) { inside++; worst = Math.Max(worst, pen); break; }
                    }
                }
            GD.Print($"{path.GetFile()} z×{flip}: ปลายจมในทรงกลม {inside}/{total} ลึกสุด {worst:0.000}");
        }
    }
}
