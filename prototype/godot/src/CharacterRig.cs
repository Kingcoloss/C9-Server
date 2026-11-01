using Godot;
using GDict = Godot.Collections.Dictionary;
using GArray = Godot.Collections.Array;

namespace EighthThrone.Demo;

// ชื่อกระดูกกลางของร่างมนุษย์ ใช้จับคู่กระดูกจากไฟล์ต่างค่าย (Mixamo, VRoid/VRM, Godot)
public static class Humanoid
{
    public static readonly string[] Center = { "hips", "spine", "chest", "upperChest", "neck", "head" };
    static readonly string[] SideParts = { "Shoulder", "UpperArm", "LowerArm", "Hand", "UpperLeg", "LowerLeg", "Foot", "Toes" };
    static readonly string[] Fingers = { "Thumb", "Index", "Middle", "Ring", "Little" };
    static readonly Dictionary<string, string> Alias = new(StringComparer.OrdinalIgnoreCase);

    // กระดูกท่อนบน (ใช้ซ้อนท่ากันโล่หรือท่าฟันทับท่าเดิน) น้ำหนัก 0..1
    public static float UpperWeight(string key) => key switch
    {
        "hips" => 0f,
        "spine" => 0.5f,
        _ when key.Contains("Leg") || key.Contains("Foot") || key.Contains("Toes") => 0f,
        _ => 1f,
    };

    static Humanoid()
    {
        void A(string name, string key) => Alias[name] = key;
        A("Hips", "hips"); A("Spine", "spine"); A("Spine1", "chest"); A("Spine2", "upperChest");
        A("Neck", "neck"); A("Head", "head"); A("Chest", "chest"); A("UpperChest", "upperChest");
        foreach (var k in Center) A("J_Bip_C_" + char.ToUpperInvariant(k[0]) + k[1..], k);
        foreach (var (S, s) in new[] { ("Left", "left"), ("Right", "right") })
        {
            // Mixamo
            A(S + "Shoulder", s + "Shoulder"); A(S + "Arm", s + "UpperArm"); A(S + "ForeArm", s + "LowerArm");
            A(S + "Hand", s + "Hand"); A(S + "UpLeg", s + "UpperLeg"); A(S + "Leg", s + "LowerLeg");
            A(S + "Foot", s + "Foot"); A(S + "ToeBase", s + "Toes");
            foreach (var (mf, f) in new[] { ("Thumb", "Thumb"), ("Index", "Index"), ("Middle", "Middle"), ("Ring", "Ring"), ("Pinky", "Little") })
                for (int i = 1; i <= 3; i++) A($"{S}Hand{mf}{i}", $"{s}{f}{i}");
            // VRoid (J_Bip_L_UpperArm ...)
            string c = S[..1];
            foreach (var p in SideParts) A($"J_Bip_{c}_{(p == "Toes" ? "ToeBase" : p)}", s + p);
            foreach (var f in Fingers)
                for (int i = 1; i <= 3; i++) A($"J_Bip_{c}_{f}{i}", $"{s}{f}{i}");
            // Godot SkeletonProfileHumanoid (ไฟล์ที่ผ่าน retarget ในหน้าต่าง import มาแล้ว)
            foreach (var p in SideParts) A(S + p, s + p);
            A(S + "ThumbMetacarpal", s + "Thumb1"); A(S + "ThumbProximal", s + "Thumb2"); A(S + "ThumbDistal", s + "Thumb3");
            foreach (var f in new[] { "Index", "Middle", "Ring", "Little" })
            {
                A($"{S}{f}Proximal", $"{s}{f}1"); A($"{S}{f}Intermediate", $"{s}{f}2"); A($"{S}{f}Distal", $"{s}{f}3");
            }
        }
    }

    // ตัดคำนำหน้าแบบ "mixamorig:" "mixamorig1_" ทิ้ง
    static string Strip(string n)
    {
        if (n.StartsWith("mixamorig", StringComparison.OrdinalIgnoreCase))
        {
            int i = 9;
            while (i < n.Length && char.IsDigit(n[i])) i++;
            if (i < n.Length && (n[i] == ':' || n[i] == '_')) i++;
            return n[i..];
        }
        return n;
    }

    public static string? KeyOfName(string boneName) =>
        Alias.TryGetValue(Strip(boneName), out var k) ? k : null;

    // จับคู่กระดูกของโครงหนึ่งกับชื่อกลาง: ใช้ข้อมูล humanoid ในไฟล์ VRM ก่อน แล้วค่อยเดาจากชื่อ
    public static Dictionary<string, int> Map(Skeleton3D sk, LoadedScene? ls)
    {
        var map = new Dictionary<string, int>();
        if (ls != null) FromVrm(sk, ls, map);
        for (int i = 0; i < sk.GetBoneCount(); i++)
        {
            var key = KeyOfName(sk.GetBoneName(i));
            if (key != null && !map.ContainsKey(key) && !map.ContainsValue(i)) map[key] = i;
        }
        return map;
    }

    static void FromVrm(Skeleton3D sk, LoadedScene ls, Dictionary<string, int> map)
    {
        if (!ls.Json.TryGetValue("extensions", out var ev) || ev.VariantType != Variant.Type.Dictionary) return;
        var ext = ev.AsGodotDictionary();
        var nodes = ls.State.GetNodes();

        int BoneOfNode(Variant v)
        {
            int idx = v.AsInt32();
            if (idx < 0 || idx >= nodes.Count) return -1;
            var n = nodes[idx];
            int b = sk.FindBone(n.ResourceName);
            if (b < 0 && !string.IsNullOrEmpty(n.OriginalName)) b = sk.FindBone(n.OriginalName);
            return b;
        }

        if (ext.TryGetValue("VRMC_vrm", out var v1) && v1.VariantType == Variant.Type.Dictionary)
        {
            // VRM 1.0: humanBones = { "leftUpperArm": { "node": 12 }, ... }
            var hb = Dig(v1.AsGodotDictionary(), "humanoid", "humanBones");
            if (hb == null) return;
            foreach (var (name, val) in hb)
            {
                var key = VrmKey(name.AsString(), vrm1: true);
                if (key == null || val.VariantType != Variant.Type.Dictionary) continue;
                if (val.AsGodotDictionary().TryGetValue("node", out var nv) && BoneOfNode(nv) is var b and >= 0)
                    map[key] = b;
            }
        }
        else if (ext.TryGetValue("VRM", out var v0) && v0.VariantType == Variant.Type.Dictionary)
        {
            // VRM 0.x: humanBones = [ { "bone": "leftUpperArm", "node": 12 }, ... ]
            var hum = Dig(v0.AsGodotDictionary(), "humanoid");
            if (hum == null || !hum.TryGetValue("humanBones", out var arr) || arr.VariantType != Variant.Type.Array) return;
            foreach (var item in arr.AsGodotArray())
            {
                if (item.VariantType != Variant.Type.Dictionary) continue;
                var d = item.AsGodotDictionary();
                if (!d.TryGetValue("bone", out var bn) || !d.TryGetValue("node", out var nv)) continue;
                var key = VrmKey(bn.AsString(), vrm1: false);
                if (key != null && BoneOfNode(nv) is var b and >= 0) map[key] = b;
            }
        }
    }

    static string? VrmKey(string n, bool vrm1)
    {
        foreach (var k in Center)
            if (n == k) return k;
        foreach (var s in new[] { "left", "right" })
        {
            if (!n.StartsWith(s)) continue;
            var rest = n[s.Length..];
            foreach (var p in SideParts)
                if (rest == p) return s + p;
            if (rest.StartsWith("Thumb"))
            {
                var seg = rest[5..];
                int i = vrm1
                    ? seg switch { "Metacarpal" => 1, "Proximal" => 2, "Distal" => 3, _ => 0 }
                    : seg switch { "Proximal" => 1, "Intermediate" => 2, "Distal" => 3, _ => 0 };
                return i > 0 ? $"{s}Thumb{i}" : null;
            }
            foreach (var f in new[] { "Index", "Middle", "Ring", "Little" })
            {
                if (!rest.StartsWith(f)) continue;
                int i = rest[f.Length..] switch { "Proximal" => 1, "Intermediate" => 2, "Distal" => 3, _ => 0 };
                return i > 0 ? $"{s}{f}{i}" : null;
            }
        }
        return null;
    }

    public static GDict? Dig(GDict d, params string[] path)
    {
        var cur = d;
        foreach (var p in path)
        {
            if (!cur.TryGetValue(p, out var v) || v.VariantType != Variant.Type.Dictionary) return null;
            cur = v.AsGodotDictionary();
        }
        return cur;
    }
}

// ข้อมูลโครงกระดูกหนึ่งชุดในท่ายืนพัก พร้อมแกนกลาง (หันหน้า +Z, ซ้ายมือ +X, หัวขึ้น +Y)
public sealed class Rig
{
    public readonly Skeleton3D Sk;
    public readonly Dictionary<string, int> Map;
    public readonly string?[] KeyOf;
    public readonly int[] Parent;
    public readonly int[] Order;          // พ่อมาก่อนลูกเสมอ
    public readonly Quaternion[] RestL, RestG;
    public readonly Vector3[] RestLP, RestGP;
    public readonly Quaternion Mq, MqInv; // แกนโครงกระดูก -> แกนกลาง
    public readonly Dictionary<string, Quaternion> Fix = new(); // หมุนแขนขาให้เป็นท่า T ตรงเป๊ะ (แกนกลาง)
    public readonly int Hips;
    public readonly float HipHeight;      // หน่วยของโครงกระดูก

    public Rig(Skeleton3D sk, Dictionary<string, int> map)
    {
        Sk = sk;
        Map = map;
        int n = sk.GetBoneCount();
        KeyOf = new string?[n];
        foreach (var (k, i) in map) KeyOf[i] = k;
        Parent = new int[n];
        RestL = new Quaternion[n]; RestG = new Quaternion[n];
        RestLP = new Vector3[n]; RestGP = new Vector3[n];
        for (int i = 0; i < n; i++)
        {
            Parent[i] = sk.GetBoneParent(i);
            var r = sk.GetBoneRest(i);
            RestL[i] = r.Basis.Orthonormalized().GetRotationQuaternion();
            RestLP[i] = r.Origin;
            var g = sk.GetBoneGlobalRest(i);
            RestG[i] = g.Basis.Orthonormalized().GetRotationQuaternion();
            RestGP[i] = g.Origin;
        }
        var depth = new int[n];
        for (int i = 0; i < n; i++)
            for (int p = Parent[i]; p >= 0; p = Parent[p]) depth[i]++;
        Order = Enumerable.Range(0, n).OrderBy(i => depth[i]).ToArray();

        Hips = map.GetValueOrDefault("hips", -1);
        if (Hips < 0) throw new InvalidOperationException("ไม่พบกระดูกสะโพก (hips)");

        // แกนกลางจากตำแหน่งกระดูก: ไหล่ซ้าย-ขวา บอกด้านข้าง สะโพกถึงหัวบอกด้านบน
        Vector3 P(string k) => RestGP[map[k]];
        Vector3 side = map.ContainsKey("leftUpperArm") && map.ContainsKey("rightUpperArm")
            ? P("leftUpperArm") - P("rightUpperArm")
            : P("leftUpperLeg") - P("rightUpperLeg");
        string top = new[] { "head", "neck", "upperChest", "chest", "spine" }.First(map.ContainsKey);
        Vector3 up = P(top) - P("hips");
        var L = side.Normalized();
        var U = (up - L * up.Dot(L)).Normalized();
        var F = L.Cross(U);
        var toSkel = new Basis(L, U, F);           // แกนกลาง -> โครงกระดูก
        MqInv = toSkel.GetRotationQuaternion();
        Mq = MqInv.Inverse();

        // แขนขาที่ไม่ได้กางตรง (เช่นท่า A) จะถูกคำนวณให้เทียบกับท่า T ตรง
        foreach (var s in new[] { "left", "right" })
        {
            var outward = s == "left" ? Vector3.Right : Vector3.Left; // +X คือซ้ายมือของตัวละคร
            AddFix(s + "Shoulder", s + "UpperArm", outward);
            AddFix(s + "UpperArm", s + "LowerArm", outward);
            AddFix(s + "LowerArm", s + "Hand", outward);
            if (!AddFix(s + "Hand", s + "Middle1", outward) && Fix.TryGetValue(s + "LowerArm", out var la))
                Fix[s + "Hand"] = la;
            AddFix(s + "UpperLeg", s + "LowerLeg", Vector3.Down);
            AddFix(s + "LowerLeg", s + "Foot", Vector3.Down);
        }

        float footY = float.MaxValue;
        foreach (var f in new[] { "leftFoot", "rightFoot" })
            if (map.TryGetValue(f, out var fi)) footY = Math.Min(footY, Canon(RestGP[fi]).Y);
        float hipY = Canon(RestGP[Hips]).Y;
        HipHeight = footY < float.MaxValue ? hipY - footY : hipY;
    }

    bool AddFix(string bone, string child, Vector3 ideal)
    {
        if (!Map.TryGetValue(bone, out var b) || !Map.TryGetValue(child, out var c)) return false;
        var d = Canon(RestGP[c] - RestGP[b]);
        if (d.LengthSquared() < 1e-10f) return false;
        Fix[bone] = new Quaternion(d.Normalized(), ideal);
        return true;
    }

    public Vector3 Canon(Vector3 skelVec) => Mq * skelVec;
    public Vector3 Skel(Vector3 canonVec) => MqInv * canonVec;

    // ทิศพักในแกนกลางหลังปรับเป็นท่า T แล้ว
    public Quaternion IdealRest(string key) =>
        Fix.GetValueOrDefault(key, Quaternion.Identity) * Mq * RestG[Map[key]];
}

// ท่าหนึ่งท่าที่แปลงลงโครงกระดูกเป้าหมายแล้ว เก็บเป็นเฟรม 30 fps
public sealed class RigClip
{
    public string Name = "";
    public float Length;
    public float Fps = 30;
    public int Frames;
    public int Bones;                 // จำนวนกระดูกที่ขยับ (ตาม Driven ของโมเดล)
    public Quaternion[] Rot = System.Array.Empty<Quaternion>(); // [frame * Bones + j]
    public Vector3[] Hip = System.Array.Empty<Vector3>();
    public float NaturalSpeed;        // ความเร็วเดินในคลิปก่อนตัด root motion (หน่วยโครงเป้าหมาย/วินาที)
    public float LeftStrike;          // เฟส 0..1 ที่เท้าซ้ายก้าวไปหน้าสุด
    public float Hit = -1;            // วินาทีที่ดาบฟันแรงสุด (เดาจากความเร็วมือขวา)
    public float Start, End;
    public float[] HandSpeed = System.Array.Empty<float>();

    // จังหวะที่มือขวาเร็วที่สุดในช่วง [Start, End]
    public float FindHit()
    {
        int a = Math.Max(0, Mathf.FloorToInt(Start * Fps)), b = Math.Min(Frames - 1, Mathf.CeilToInt(End * Fps));
        int best = -1;
        for (int f = a; f <= b && f < HandSpeed.Length; f++)
            if (best < 0 || HandSpeed[f] > HandSpeed[best]) best = f;
        return best >= 0 && HandSpeed[best] > 0 ? best / Fps : Start + (End - Start) * 0.4f;
    }

    public void Sample(float t, bool loop, Quaternion[] q, out Vector3 hip)
    {
        float f = t * Fps;
        int last = Frames - 1;
        if (loop)
        {
            // เฟรมสุดท้ายของคลิปวนซ้ำมักเหมือนเฟรมแรก จึงวนในช่วง [0, last)
            float span = Math.Max(1, last);
            f = Mathf.PosMod(f, span);
        }
        else f = Mathf.Clamp(f, 0, last);
        int i0 = Mathf.FloorToInt(f);
        int i1 = Math.Min(i0 + 1, last);
        float w = f - i0;
        int a = i0 * Bones, b = i1 * Bones;
        for (int j = 0; j < Bones; j++) q[j] = Rot[a + j].Slerp(Rot[b + j], w);
        hip = Hip[i0].Lerp(Hip[i1], w);
    }
}

public enum RootMode { Lock, Detrend }

public static class Retarget
{
    // แปลงท่าจากโครงต้นทางไปโครงเป้าหมาย โดยเทียบการหมุนของกระดูกแต่ละท่อนในแกนกลาง
    // ทั้งสองโครงถูกปรับเป็นท่า T ตรงก่อน ท่าที่ได้จึงไม่เพี้ยนแม้ท่ายืนพักของสองไฟล์ต่างกัน
    public static RigClip Build(Rig src, Animation anim, Rig dst, int[] driven, RootMode root, string name)
    {
        int ns = src.Sk.GetBoneCount();
        var rotTrack = Enumerable.Repeat(-1, ns).ToArray();
        var posTrack = Enumerable.Repeat(-1, ns).ToArray();
        for (int t = 0; t < anim.GetTrackCount(); t++)
        {
            var type = anim.TrackGetType(t);
            if (type != Animation.TrackType.Rotation3D && type != Animation.TrackType.Position3D) continue;
            var path = anim.TrackGetPath(t);
            if (path.GetSubNameCount() == 0) continue;
            int b = src.Sk.FindBone(path.GetSubName(0));
            if (b < 0) continue;
            if (type == Animation.TrackType.Rotation3D) rotTrack[b] = t; else posTrack[b] = t;
        }

        var clip = new RigClip { Name = name, Length = (float)anim.Length, Bones = driven.Length };
        clip.Frames = Math.Max(2, Mathf.CeilToInt(clip.Length * clip.Fps) + 1);
        clip.Rot = new Quaternion[clip.Frames * driven.Length];
        clip.Hip = new Vector3[clip.Frames];
        clip.End = clip.Length;

        // ส่วนที่คำนวณครั้งเดียว: การหมุนพักในท่า T ของทั้งสองฝั่ง
        int nd = dst.Sk.GetBoneCount();
        var srcIdealInv = new Dictionary<string, Quaternion>();
        var dstIdeal = new Dictionary<string, Quaternion>();
        foreach (var (k, sb) in src.Map)
        {
            if (!dst.Map.ContainsKey(k)) continue;
            srcIdealInv[k] = src.IdealRest(k).Inverse();
            dstIdeal[k] = dst.IdealRest(k);
        }

        float scale = src.HipHeight > 1e-5f ? dst.HipHeight / src.HipHeight : 1f;
        var gs = new Quaternion[ns];
        var gp = new Vector3[ns];
        var gt = new Quaternion[nd];
        var lt = new Quaternion[nd];
        var hipOff = new Vector3[clip.Frames];
        var lFoot = new float[clip.Frames];
        var rHand = new Vector3[clip.Frames];
        Vector3 hipRest = src.Canon(src.RestGP[src.Hips]);
        int sLF = src.Map.GetValueOrDefault("leftFoot", -1);
        int sRF = src.Map.GetValueOrDefault("rightFoot", -1);
        int sRH = src.Map.GetValueOrDefault("rightHand", -1);

        for (int f = 0; f < clip.Frames; f++)
        {
            double time = Math.Min(f / (double)clip.Fps, anim.Length);
            foreach (int i in src.Order)
            {
                var lq = rotTrack[i] >= 0 ? anim.RotationTrackInterpolate(rotTrack[i], time) : src.RestL[i];
                var lp = posTrack[i] >= 0 ? anim.PositionTrackInterpolate(posTrack[i], time) : src.RestLP[i];
                int p = src.Parent[i];
                gs[i] = p < 0 ? lq.Normalized() : (gs[p] * lq).Normalized();
                gp[i] = p < 0 ? lp : gp[p] + gs[p] * lp;
            }

            foreach (int i in dst.Order)
            {
                int p = dst.Parent[i];
                var parentG = p < 0 ? Quaternion.Identity : gt[p];
                var key = dst.KeyOf[i];
                if (key != null && srcIdealInv.TryGetValue(key, out var inv))
                {
                    var delta = (src.Mq * gs[src.Map[key]]) * inv;      // การหมุนจากท่า T ของต้นทาง (แกนกลาง)
                    gt[i] = (dst.MqInv * delta * dstIdeal[key]).Normalized();
                    lt[i] = (parentG.Inverse() * gt[i]).Normalized();
                }
                else
                {
                    lt[i] = dst.RestL[i];
                    gt[i] = (parentG * lt[i]).Normalized();
                }
            }
            for (int j = 0; j < driven.Length; j++) clip.Rot[f * driven.Length + j] = lt[driven[j]];

            var hc = src.Canon(gp[src.Hips]);
            hipOff[f] = (hc - hipRest) * scale;
            if (sLF >= 0) lFoot[f] = src.Canon(gp[sLF] - gp[src.Hips]).Z;
            if (sRH >= 0) rHand[f] = src.Canon(gp[sRH] - gp[src.Hips]) * scale;
        }

        // ความเร็วเดินในคลิป แล้วตัดการเคลื่อนที่ไปข้างหน้าทิ้ง (ตำแหน่งจริงมาจาก Sim)
        var drift = hipOff[^1] - hipOff[0];
        drift.Y = 0;
        clip.NaturalSpeed = clip.Length > 0 ? drift.Length() / clip.Length : 0;
        int hp = dst.Parent[dst.Hips];
        var parentRest = hp < 0 ? Transform3D.Identity : dst.Sk.GetBoneGlobalRest(hp);
        for (int f = 0; f < clip.Frames; f++)
        {
            var d = hipOff[f];
            if (root == RootMode.Lock) { d.X = 0; d.Z = 0; }
            else
            {
                float u = clip.Frames > 1 ? f / (float)(clip.Frames - 1) : 0;
                var trend = hipOff[0] + (hipOff[^1] - hipOff[0]) * u;
                d.X -= trend.X; d.Z -= trend.Z;
            }
            var global = dst.RestGP[dst.Hips] + dst.Skel(d);
            clip.Hip[f] = parentRest.AffineInverse() * global;
        }

        // เฟสที่เท้าซ้ายอยู่หน้าสุด (ใช้จัดจังหวะเดินกับวิ่งให้ตรงกัน และเล่นเสียงเท้า)
        if (sLF >= 0)
        {
            int best = 0;
            for (int f = 1; f < clip.Frames - 1; f++) if (lFoot[f] > lFoot[best]) best = f;
            clip.LeftStrike = best / (float)Math.Max(1, clip.Frames - 1);
        }

        // ความเร็วมือขวาเทียบสะโพก ใช้หาจังหวะฟัน
        clip.HandSpeed = new float[clip.Frames];
        if (sRH >= 0)
            for (int f = 2; f < clip.Frames - 2; f++)
                clip.HandSpeed[f] = (rHand[f + 1] - rHand[f - 1]).Length() + 0.5f * (rHand[f + 2] - rHand[f - 2]).Length();
        clip.Hit = clip.FindHit();
        return clip;
    }
}

// ตรวจการแปลงท่าด้วยตัวเลข: ทิศของแขนขาบนโครงเป้าหมายต้องตรงกับต้นทาง (เรียกด้วย --rig-test=โมเดล|ไฟล์ท่า)
public static class RigTest
{
    static readonly (string a, string b)[] Segs =
    {
        ("leftUpperArm", "leftLowerArm"), ("leftLowerArm", "leftHand"),
        ("rightUpperArm", "rightLowerArm"), ("rightLowerArm", "rightHand"),
        ("leftUpperLeg", "leftLowerLeg"), ("leftLowerLeg", "leftFoot"),
        ("rightUpperLeg", "rightLowerLeg"), ("rightLowerLeg", "rightFoot"),
        ("hips", "neck"), ("neck", "head"), ("leftShoulder", "leftUpperArm"), ("rightShoulder", "rightUpperArm"),
        ("rightLittle1", "rightIndex1"), ("leftLittle1", "leftIndex1"), ("rightHand", "rightMiddle1"),
    };

    public static void Run(string spec)
    {
        var parts = spec.Split('|');
        var model = RigLoad.Load(parts[0]);
        string animFile = parts[1], animName = "";
        int hash = animFile.IndexOf('#');
        if (hash >= 0) { animName = animFile[(hash + 1)..]; animFile = animFile[..hash]; }
        var src = RigLoad.Load(animFile);
        if (model?.Skeleton == null || src?.Skeleton == null) { GD.Print("โหลดไม่ได้"); return; }
        var dst = new Rig(model.Skeleton, Humanoid.Map(model.Skeleton, model));
        var srig = new Rig(src.Skeleton, Humanoid.Map(src.Skeleton, src));
        var driven = dst.Order.Where(i => dst.KeyOf[i] != null).ToArray();
        var (name, anim) = src.Animations.First(a => a.Anim.GetTrackCount() > 0 && (animName == "" || a.Name == animName));
        var clip = Retarget.Build(srig, anim, dst, driven, RootMode.Detrend, name);
        GD.Print($"test {parts[0].GetFile()} <- {animFile.GetFile()}#{name}: frames={clip.Frames} hipH dst={dst.HipHeight:0.###} src={srig.HipHeight:0.###} speed={clip.NaturalSpeed:0.###} hit={clip.Hit:0.00} strike={clip.LeftStrike:0.00}");
        GD.Print($"  mapped dst={dst.Map.Count} src={srig.Map.Count} fixes dst=[{string.Join(", ", dst.Fix.Select(f => $"{f.Key}:{Mathf.RadToDeg(f.Value.GetAngle()):0}"))}]");

        var slot = new Dictionary<int, int>();
        for (int j = 0; j < driven.Length; j++) slot[driven[j]] = j;
        int nd = dst.Sk.GetBoneCount(), ns = src.Skeleton.GetBoneCount();
        var q = new Quaternion[driven.Length];
        var errs = Segs.ToDictionary(s => s.a + ">" + s.b, _ => new List<float>());
        var rotTrack = new int[ns]; var posTrack = new int[ns];
        Array.Fill(rotTrack, -1); Array.Fill(posTrack, -1);
        for (int t = 0; t < anim.GetTrackCount(); t++)
        {
            var p = anim.TrackGetPath(t);
            if (p.GetSubNameCount() == 0) continue;
            int b = src.Skeleton.FindBone(p.GetSubName(0));
            if (b < 0) continue;
            if (anim.TrackGetType(t) == Animation.TrackType.Rotation3D) rotTrack[b] = t;
            else if (anim.TrackGetType(t) == Animation.TrackType.Position3D) posTrack[b] = t;
        }
        for (double time = 0; time <= anim.Length; time += Math.Max(anim.Length / 12.0, 0.05))
        {
            clip.Sample((float)time, false, q, out var hip);
            var gq = new Quaternion[nd]; var gp = new Vector3[nd];
            foreach (int i in dst.Order)
            {
                int p = dst.Parent[i];
                var lq = slot.TryGetValue(i, out var j) ? q[j] : dst.RestL[i];
                var lp = i == dst.Hips ? hip : dst.RestLP[i];
                gq[i] = (p < 0 ? lq : gq[p] * lq).Normalized();
                gp[i] = p < 0 ? lp : gp[p] + gq[p] * lp;
            }
            var sq = new Quaternion[ns]; var sp = new Vector3[ns];
            foreach (int i in srig.Order)
            {
                int p = srig.Parent[i];
                var lq = rotTrack[i] >= 0 ? anim.RotationTrackInterpolate(rotTrack[i], time) : srig.RestL[i];
                var lp = posTrack[i] >= 0 ? anim.PositionTrackInterpolate(posTrack[i], time) : srig.RestLP[i];
                sq[i] = (p < 0 ? lq : sq[p] * lq).Normalized();
                sp[i] = p < 0 ? lp : sp[p] + sq[p] * lp;
            }
            foreach (var (a, b) in Segs)
            {
                if (!dst.Map.ContainsKey(a) || !dst.Map.ContainsKey(b) || !srig.Map.ContainsKey(a) || !srig.Map.ContainsKey(b)) continue;
                var dd = dst.Canon(gp[dst.Map[b]] - gp[dst.Map[a]]).Normalized();
                var ds = srig.Canon(sp[srig.Map[b]] - sp[srig.Map[a]]).Normalized();
                errs[a + ">" + b].Add(Mathf.RadToDeg(dd.AngleTo(ds)));
                if (a == "rightLittle1" && Math.Abs(time - 1.0) < 0.11) GD.Print($"  t={time:0.00} knuckle dst={dd} src={ds}");
            }
        }
        foreach (var (k, v) in errs)
            if (v.Count > 0) GD.Print($"  {k,-28} max {v.Max(),5:0.0}°  mean {v.Average(),5:0.0}°");
    }
}
