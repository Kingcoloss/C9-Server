using System.Text.RegularExpressions;
using Godot;
using GDict = Godot.Collections.Dictionary;

namespace EighthThrone.Demo;

// ตัวละครหนึ่งตัวที่พร้อมใช้: โมเดล + ท่าทั้งหมดที่แปลงแล้ว
// วางไฟล์ไว้ที่ characters/ (โมเดล .vrm .glb .fbx) และ characters/anims/ (ท่าจาก Mixamo)
public sealed partial class CharacterSet
{
    public static readonly string[] Roles =
    {
        "idle", "walk", "run", "guard",
        "light1", "light2", "light3", "light4", "heavy", "finisher", "skill1", "skill2",
        "dodge", "hit", "guardbreak", "death",
    };
    static readonly HashSet<string> LoopRoles = new() { "idle", "walk", "run", "guard" };

    public Node3D Template = null!;
    public Rig Rig = null!;
    public int[] Driven = null!;
    public string[] DrivenKeys = null!;
    public readonly Dictionary<string, RigClip> Clips = new();
    public float UnitScale;          // เมตรต่อหนึ่งหน่วยของโครงกระดูก
    public Transform3D ModelXform;   // วางโมเดลใต้จุดหมุน: หันหน้า -Z เท้าแตะพื้น
    public float HipHeight;          // เมตร
    public float WalkSpeed, RunSpeed;
    public string ModelPath = "";
    public readonly List<string> Log = new();
    public readonly List<(NodePath Mesh, int Shape, float Weight)> Blink = new(); // กะพริบตา (จาก VRM)
    public SpringSpec? Springs;      // ผมและกระโปรงที่แกว่งได้ (จาก VRM)

    public static string Dir = "res://characters";
    static CharacterSet? _cached;
    static bool _tried;

    public static CharacterSet? Get()
    {
        if (_tried) return _cached;
        _tried = true;
        ulong t0 = Time.GetTicksMsec();
        try { _cached = Load(Dir); }
        catch (Exception e)
        {
            GD.PushWarning($"โหลดตัวละครไม่สำเร็จ ใช้ตัวละครรูปทรงพื้นฐานแทน: {e.Message}");
            _cached = null;
        }
        if (_cached != null)
        {
            _cached.Log.Add($"โหลดเสร็จใน {Time.GetTicksMsec() - t0} ms");
            foreach (var l in _cached.Log) GD.Print("[character] " + l);
        }
        return _cached;
    }

    sealed class Spec
    {
        public string Source = "";
        public float Hit = -1, Start = -1, End = -1;
    }

    sealed record Candidate(string Name, LoadedScene Scene, Animation Anim);

    static CharacterSet? Load(string dir)
    {
        if (!DirAccess.DirExistsAbsolute(dir)) return null;
        var cfg = ReadJson(dir.PathJoin("anim_map.json"));
        var files = ListFiles(dir);
        string? modelName = cfg != null && cfg.TryGetValue("model", out var mv) ? mv.AsString() : null;
        string? modelPath = modelName != null
            ? dir.PathJoin(modelName)
            : files.OrderBy(f => f.GetFile().StartsWith("player", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(f => f.GetExtension().ToLowerInvariant() switch { "vrm" => 0, "glb" => 1, "gltf" => 2, _ => 3 })
                .FirstOrDefault();
        if (modelPath == null) return null;

        var model = RigLoad.Load(modelPath);
        if (model?.Skeleton == null) throw new InvalidOperationException($"{modelPath.GetFile()} ไม่มีโครงกระดูก");
        var cs = new CharacterSet { ModelPath = modelPath, Template = model.Root };
        var map = Humanoid.Map(model.Skeleton, model);
        cs.Rig = new Rig(model.Skeleton, map);
        cs.Driven = cs.Rig.Order.Where(i => cs.Rig.KeyOf[i] != null).ToArray();
        cs.DrivenKeys = cs.Driven.Select(i => cs.Rig.KeyOf[i]!).ToArray();
        cs.Log.Add($"โมเดล {modelPath.GetFile()}: กระดูก {model.Skeleton.GetBoneCount()} ท่อน จับคู่ได้ {map.Count}");

        // ท่าทั้งหมดที่หาได้ (ในโมเดลเองและในโฟลเดอร์ anims)
        var cands = new List<Candidate>();
        void AddFrom(LoadedScene ls)
        {
            var usable = ls.Animations.Where(a => a.Anim.GetTrackCount() > 0).ToList();
            foreach (var (name, anim) in usable)
            {
                string stem = ls.Path.GetFile().GetBaseName();
                cands.Add(new Candidate(usable.Count == 1 ? stem : $"{stem}#{name}", ls, anim));
            }
        }
        AddFrom(model);
        foreach (var f in ListFiles(dir.PathJoin("anims")))
            if (RigLoad.Load(f) is { } ls) AddFrom(ls);
        cs.Log.Add($"พบท่า {cands.Count} ท่า");

        var specs = ChooseRoles(cands.Select(c => c.Name).ToList(), cfg, cs.Log);
        var srcRigs = new Dictionary<LoadedScene, Rig?>();
        foreach (var (role, spec) in specs)
        {
            var cand = cands.FirstOrDefault(c => string.Equals(c.Name, spec.Source, StringComparison.OrdinalIgnoreCase));
            if (cand == null)
            {
                cs.Log.Add($"  {role}: ไม่พบ '{spec.Source}'");
                continue;
            }
            if (!srcRigs.TryGetValue(cand.Scene, out var srcRig))
            {
                srcRig = cand.Scene.Skeleton == null ? null
                    : new Rig(cand.Scene.Skeleton, Humanoid.Map(cand.Scene.Skeleton, cand.Scene));
                srcRigs[cand.Scene] = srcRig;
            }
            if (srcRig == null) continue;
            var clip = Retarget.Build(srcRig, cand.Anim, cs.Rig, cs.Driven,
                LoopRoles.Contains(role) ? RootMode.Detrend : RootMode.Lock, cand.Name);
            if (spec.Start >= 0) clip.Start = Math.Min(spec.Start, clip.Length);
            if (spec.End > 0) clip.End = Math.Clamp(spec.End, clip.Start, clip.Length);
            clip.Hit = spec.Hit >= 0 ? Math.Clamp(spec.Hit, clip.Start, clip.End) : clip.FindHit();
            cs.Clips[role] = clip;
        }

        cs.ReadBlink(model);
        cs.Springs = SpringSpec.Parse(model, model.Skeleton);
        if (cs.Springs != null)
        {
            var driven = new HashSet<int>(cs.Driven);
            foreach (var chain in cs.Springs.Chains) chain.RemoveAll(j => driven.Contains(j.Bone));
            cs.Springs.Chains.RemoveAll(c => c.Count == 0);
            cs.Log.Add($"ผม/กระโปรงแกว่งได้ {cs.Springs.Chains.Count} สาย {cs.Springs.JointCount} ข้อ ทรงกลมกันทะลุ {cs.Springs.Colliders.Count}");
        }
        cs.Place(cfg);
        Toon.Apply(cs.Template, model.Json, cs.UnitScale);
        foreach (var ap in RigLoad.FindAll<AnimationPlayer>(cs.Template)) ap.Free();
        foreach (var other in srcRigs.Keys) if (other != model) other.Root.Free();

        float hipScale = cs.HipHeight / 0.95f;
        cs.WalkSpeed = Speed("walk", 1.4f * hipScale);
        cs.RunSpeed = Speed("run", 4.2f * hipScale);
        if (cs.RunSpeed < cs.WalkSpeed + 0.5f) cs.RunSpeed = cs.WalkSpeed + 0.5f;
        float Speed(string role, float fallback)
        {
            float v = cs.Clips.TryGetValue(role, out var c) ? c.NaturalSpeed * cs.UnitScale : 0;
            return v > 0.3f ? v : fallback;
        }

        foreach (var (role, c) in cs.Clips.OrderBy(kv => Array.IndexOf(Roles, kv.Key)))
            cs.Log.Add($"  {role,-10} <- {c.Name} ({c.Start:0.00}-{c.End:0.00}s, ฟันที่ {c.Hit:0.00}s, เร็ว {c.NaturalSpeed * cs.UnitScale:0.0} m/s)");
        cs.Log.Add($"สูง {cs.HipHeight:0.00} m ถึงสะโพก, หน่วย {cs.UnitScale:0.####} m, เดิน {cs.WalkSpeed:0.0} วิ่ง {cs.RunSpeed:0.0} m/s");
        return cs;
    }

    // วางโมเดล: หันหน้า -Z (แบบเดียวกับตัวละครเดิม) เท้าแตะพื้น สะโพกตรงจุดกำเนิด
    void Place(GDict? cfg)
    {
        var sk = Rig.Sk;
        var w = RigLoad.RelativeTo(sk, Template);
        float sw = w.Basis.Scale.X;
        var rot = new Basis(Vector3.Up, Mathf.Pi) * new Basis(Rig.Mq);

        Transform3D Make(float s) =>
            new Transform3D(Basis.FromScale(Vector3.One * s) * rot, Vector3.Zero) * w.AffineInverse();

        Aabb Bounds(Transform3D x)
        {
            Aabb? box = null;
            foreach (var mi in RigLoad.FindAll<MeshInstance3D>(Template))
            {
                if (mi.Mesh == null) continue;
                var b = x * RigLoad.RelativeTo(mi, Template) * mi.Mesh.GetAabb();
                box = box?.Merge(b) ?? b;
            }
            return box ?? new Aabb(Vector3.Zero, Vector3.One);
        }

        var x0 = Make(sw);
        var box0 = Bounds(x0);
        float h = Math.Max(box0.Size.Y, 0.01f);
        float want = cfg != null && cfg.TryGetValue("height", out var hv) ? (float)hv : 0;
        float rel = want > 0 ? want / h : (h < 1.3f || h > 2.3f ? 1.7f / h : 1f);
        UnitScale = sw * rel;
        ModelXform = Make(UnitScale);
        var hip = ModelXform * w * Rig.RestGP[Rig.Hips];
        float minY = box0.Position.Y * rel;
        ModelXform.Origin -= new Vector3(hip.X, minY, hip.Z);
        HipHeight = hip.Y - minY;
        Log.Add($"ความสูงโมเดล {h * rel:0.00} m");
    }

    // ท่ากะพริบตาในไฟล์ VRM: 0.x อยู่ใน blendShapeMaster, 1.0 อยู่ใน expressions.preset.blink
    void ReadBlink(LoadedScene ls)
    {
        var nodes = ls.State.GetNodes();
        void Add(int nodeIdx, int shape, float weight)
        {
            if (nodeIdx < 0 || nodeIdx >= nodes.Count) return;
            string name = nodes[nodeIdx].ResourceName;
            var mi = RigLoad.FindAll<MeshInstance3D>(Template).FirstOrDefault(m => m.Name == name);
            if (mi?.Mesh is ArrayMesh am && shape < am.GetBlendShapeCount())
                Blink.Add((Template.GetPathTo(mi), shape, weight));
        }
        if (Humanoid.Dig(ls.Json, "extensions", "VRMC_vrm", "expressions", "preset", "blink") is { } b1 &&
            b1.TryGetValue("morphTargetBinds", out var binds))
        {
            foreach (var v in binds.AsGodotArray())
            {
                var d = v.AsGodotDictionary();
                Add(d["node"].AsInt32(), d["index"].AsInt32(), d.TryGetValue("weight", out var w) ? (float)w : 1f);
            }
        }
        else if (Humanoid.Dig(ls.Json, "extensions", "VRM", "blendShapeMaster") is { } bm &&
                 bm.TryGetValue("blendShapeGroups", out var groups))
        {
            foreach (var gv in groups.AsGodotArray())
            {
                var g = gv.AsGodotDictionary();
                if (!g.TryGetValue("presetName", out var pn) || pn.AsString() != "blink") continue;
                foreach (var v in g["binds"].AsGodotArray())
                {
                    var d = v.AsGodotDictionary();
                    int mesh = d["mesh"].AsInt32();
                    for (int i = 0; i < nodes.Count; i++)
                        if (nodes[i].Mesh == mesh) Add(i, d["index"].AsInt32(), (float)d["weight"] / 100f);
                }
            }
        }
        if (Blink.Count > 0) Log.Add("กะพริบตาได้");
    }

    // ---- เลือกไฟล์ท่าให้แต่ละบทบาท: ตามไฟล์ anim_map.json ก่อน แล้วค่อยเดาจากชื่อไฟล์ ----
    static Dictionary<string, Spec> ChooseRoles(List<string> names, GDict? cfg, List<string> log)
    {
        var res = new Dictionary<string, Spec>();
        var clipsCfg = cfg != null ? Humanoid.Dig(cfg, "clips") : null;
        if (clipsCfg != null)
        {
            foreach (var (k, v) in clipsCfg)
            {
                var spec = new Spec();
                if (v.VariantType == Variant.Type.String) spec.Source = v.AsString();
                else if (v.VariantType == Variant.Type.Dictionary)
                {
                    var d = v.AsGodotDictionary();
                    spec.Source = d.TryGetValue("file", out var f) ? f.AsString() : "";
                    if (d.TryGetValue("anim", out var a)) spec.Source += "#" + a.AsString();
                    if (d.TryGetValue("hit", out var h)) spec.Hit = (float)h;
                    if (d.TryGetValue("start", out var s)) spec.Start = (float)s;
                    if (d.TryGetValue("end", out var e)) spec.End = (float)e;
                }
                // ใส่ชื่อไฟล์มาพร้อมนามสกุลก็ได้
                var src = spec.Source;
                int hash = src.IndexOf('#');
                string file = hash >= 0 ? src[..hash] : src, anim = hash >= 0 ? src[hash..] : "";
                spec.Source = file.GetBaseName() + anim;
                if (spec.Source.Length > 0) res[k.AsString()] = spec;
            }
        }

        var used = new HashSet<string>(res.Values.Select(s => s.Source), StringComparer.OrdinalIgnoreCase);
        var sorted = names.OrderBy(NaturalKey).ToList();
        foreach (var role in Roles)
        {
            if (res.ContainsKey(role)) continue;
            var ranked = sorted.Select(n => (n, s: Score(role, n))).Where(p => p.s >= 0)
                .OrderByDescending(p => p.s).ToList();
            if (ranked.Count == 0) continue;
            // ท่าโจมตีพยายามไม่ใช้ไฟล์ซ้ำกัน ถ้ามีให้เลือกพอ
            var pick = role is "idle" or "walk" or "run" or "guard"
                ? ranked[0].n
                : ranked.FirstOrDefault(p => !used.Contains(p.n)).n ?? ranked[0].n;
            used.Add(pick);
            res[role] = new Spec { Source = pick };
        }

        // บทบาทที่ไม่มีไฟล์: ยืมท่าใกล้เคียง
        (string role, string[] from)[] borrow =
        {
            ("walk", new[] { "run" }), ("run", new[] { "walk" }),
            ("light1", new[] { "light2", "heavy" }), ("light2", new[] { "light1" }), ("light3", new[] { "light1", "light2" }),
            ("light4", new[] { "heavy", "light3", "light1" }), ("heavy", new[] { "light4", "light1" }),
            ("finisher", new[] { "heavy", "light4" }), ("skill1", new[] { "light1" }), ("skill2", new[] { "heavy", "light4" }),
            ("guardbreak", new[] { "hit" }),
        };
        foreach (var (role, from) in borrow)
            if (!res.ContainsKey(role) && from.FirstOrDefault(res.ContainsKey) is { } f)
                res[role] = new Spec { Source = res[f].Source, Hit = res[f].Hit, Start = res[f].Start, End = res[f].End };

        var missing = Roles.Where(r => !res.ContainsKey(r)).ToList();
        if (missing.Count > 0) log.Add("ไม่มีท่า (ใช้ท่าที่โปรแกรมขยับให้แทน): " + string.Join(", ", missing));
        return res;
    }

    static int Score(string role, string name)
    {
        string n = " " + Regex.Replace(name.ToLowerInvariant(), @"[_\-#().]", " ") + " ";
        bool Has(params string[] ws) => ws.Any(n.Contains);
        return role switch
        {
            "idle" => !Has("idle") ? -1 : Has("block", "crouch", "cast", "power", "sad", "look", "talk", "turn", " to ") ? 2 : 20,
            "walk" => !Has("walk") ? -1 : Has("back", "left", "right", "strafe", "crouch", "block", "turn", "stop", "start") ? 2 : 20,
            "run" => !Has(" run", "sprint", "jog") ? -1 : Has("back", "left", "right", "strafe", "crouch", "jump", "turn", "stop", "start", " to ") ? 2 : 20,
            "guard" => !Has("block", "guard", "defen") ? -1 : Has("impact", "hit", "react") ? 3 : Has("idle") ? 25 : 15,
            "light1" or "light2" or "light3" or "light4" =>
                Has("jump", "power", "run", "kick", "cast") ? -1 : Has("slash") ? 20 : Has("attack") ? 12 : -1,
            "heavy" => Has("jump", "run") ? -1 : Has("attack") ? (Has("power", "heavy", "strong") ? 25 : 18) : Has("slash") ? 8 : -1,
            "finisher" => Has("jump attack", "jumping attack") ? 25 : Has("power") && Has("attack", "slash") ? 22
                : Has("attack") ? 10 : Has("slash") ? 5 : -1,
            "skill1" => Has("kick", "punch", "bash", "combo") ? 20 : Has("slash") ? 6 : Has("attack") ? 5 : -1,
            "skill2" => Has("spin", "360", "whirl", "sweep") ? 25 : Has("jump attack") ? 15 : Has("attack") ? 8 : -1,
            "dodge" => Has("roll", "dodge", "evade", "dive", "tumble") ? 20 : -1,
            "hit" => !Has("impact", " hit", "react", "flinch", "damage") ? -1 : Has("block") ? 8 : 20,
            "guardbreak" => Has("block") && Has("impact", " hit", "react") ? 25 : Has("impact", " hit", "react") ? 10 : -1,
            "death" => Has("death", "dying", " die", "dead") ? 20 : -1,
            _ => -1,
        };
    }

    // "slash (2)" มาก่อน "slash (10)"
    static string NaturalKey(string s) => NumRe().Replace(s.ToLowerInvariant(), m => m.Value.PadLeft(6, '0'));

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumRe();

    static List<string> ListFiles(string dir)
    {
        var res = new List<string>();
        using var d = DirAccess.Open(dir);
        if (d == null) return res;
        foreach (var f in d.GetFiles())
        {
            var ext = f.GetExtension().ToLowerInvariant();
            if (ext is "vrm" or "glb" or "gltf" or "fbx") res.Add(dir.PathJoin(f));
        }
        res.Sort(StringComparer.OrdinalIgnoreCase);
        return res;
    }

    static GDict? ReadJson(string path)
    {
        if (!Godot.FileAccess.FileExists(path)) return null;
        var v = Json.ParseString(Godot.FileAccess.GetFileAsString(path));
        return v.VariantType == Variant.Type.Dictionary ? v.AsGodotDictionary() : null;
    }
}
