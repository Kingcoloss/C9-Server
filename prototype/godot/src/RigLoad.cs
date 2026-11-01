using Godot;
using Godot.Collections;

namespace EighthThrone.Demo;

// โหลดโมเดลหรือไฟล์ท่าทาง (.vrm .glb .gltf .fbx) ตอนเกมรัน ไม่ต้องผ่านหน้าต่าง import ของ Godot
public sealed class LoadedScene
{
    public required string Path;
    public required Node3D Root;
    public required GltfState State;
    public Skeleton3D? Skeleton;
    public readonly List<(string Name, Animation Anim)> Animations = new();

    // JSON ของไฟล์ (มีส่วนขยาย VRM อยู่ในนี้ ถ้าเป็นไฟล์ .vrm)
    public Dictionary Json => State.Json;
}

public static class RigLoad
{
    public static LoadedScene? Load(string path)
    {
        string ext = path.GetExtension().ToLowerInvariant();
        GltfDocument doc;
        GltfState state;
        if (ext == "fbx")
        {
            doc = new FbxDocument();
            state = new FbxState();
        }
        else if (ext is "glb" or "gltf" or "vrm")
        {
            doc = new GltfDocument();
            state = new GltfState();
        }
        else return null;

        // ไฟล์ .vrm คือ glb ที่เปลี่ยนนามสกุล อ่านเป็น buffer จะไม่ติดเรื่องนามสกุล
        Error err;
        if (ext == "vrm")
        {
            var bytes = Godot.FileAccess.GetFileAsBytes(path);
            if (bytes.Length == 0)
            {
                GD.PushWarning($"อ่านไฟล์ไม่ได้: {path}");
                return null;
            }
            err = doc.AppendFromBuffer(bytes, path.GetBaseDir(), state);
        }
        else err = doc.AppendFromFile(path, state);

        if (err != Error.Ok)
        {
            GD.PushWarning($"โหลด {path} ไม่สำเร็จ: {err}");
            return null;
        }
        if (doc.GenerateScene(state) is not Node3D root)
        {
            GD.PushWarning($"สร้างฉากจาก {path} ไม่สำเร็จ");
            return null;
        }

        var ls = new LoadedScene { Path = path, Root = root, State = state };
        ls.Skeleton = FindFirst<Skeleton3D>(root);
        foreach (var ap in FindAll<AnimationPlayer>(root))
            foreach (var lib in ap.GetAnimationLibraryList())
                foreach (var name in ap.GetAnimationLibrary(lib).GetAnimationList())
                    ls.Animations.Add((name.ToString(), ap.GetAnimationLibrary(lib).GetAnimation(name)));
        return ls;
    }

    public static T? FindFirst<T>(Node n) where T : Node
    {
        if (n is T t) return t;
        foreach (var c in n.GetChildren())
            if (FindFirst<T>(c) is { } r) return r;
        return null;
    }

    public static List<T> FindAll<T>(Node n, List<T>? into = null) where T : Node
    {
        into ??= new List<T>();
        if (n is T t) into.Add(t);
        foreach (var c in n.GetChildren()) FindAll(c, into);
        return into;
    }

    // ตำแหน่งและทิศของโหนดเทียบกับราก (ใช้ได้ก่อนโหนดเข้าฉาก)
    public static Transform3D RelativeTo(Node3D n, Node root)
    {
        var x = Transform3D.Identity;
        for (Node? p = n; p != null && p != root; p = p.GetParent())
            if (p is Node3D p3) x = p3.Transform * x;
        return x;
    }

    // พิมพ์โครงสร้างไฟล์ไว้ดูตอนตั้งค่า (เรียกด้วย --rig-probe=<ไฟล์>)
    public static void Probe(string path)
    {
        var ls = Load(path);
        if (ls == null) return;
        GD.Print($"== {path}");
        PrintTree(ls.Root, ls.Root, 0);
        if (ls.Skeleton is { } sk)
        {
            GD.Print($"skeleton {sk.Name} bones={sk.GetBoneCount()} xform={RelativeTo(sk, ls.Root)}");
            for (int i = 0; i < sk.GetBoneCount(); i++)
            {
                var r = sk.GetBoneRest(i);
                var g = sk.GetBoneGlobalRest(i);
                GD.Print($"  {i,3} {sk.GetBoneName(i),-34} parent={sk.GetBoneParent(i),3} " +
                         $"restQ={r.Basis.GetRotationQuaternion()} pos={g.Origin}");
            }
        }
        foreach (var (name, a) in ls.Animations)
        {
            GD.Print($"anim '{name}' len={a.Length:0.00} tracks={a.GetTrackCount()} loop={a.LoopMode}");
            for (int t = 0; t < Math.Min(a.GetTrackCount(), 6); t++)
                GD.Print($"    {a.TrackGetType(t)} {a.TrackGetPath(t)} keys={a.TrackGetKeyCount(t)}");
        }
        ls.Root.Free();
    }

    static void PrintTree(Node n, Node root, int depth)
    {
        if (depth > 4) return;
        string extra = n is Node3D n3 ? $" {n3.Transform}" : "";
        GD.Print($"{new string(' ', depth * 2)}{n.Name} ({n.GetClass()}){extra}");
        foreach (var c in n.GetChildren())
            if (c is not Skeleton3D || depth < 4) PrintTree(c, root, depth + 1);
    }
}
