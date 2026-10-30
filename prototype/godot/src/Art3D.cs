using Godot;
using NVec = System.Numerics.Vector2;

namespace EighthThrone.Demo;

// ตัวช่วยสร้างวัสดุและชิ้นส่วนจากรูปทรงพื้นฐาน (ยังไม่มีโมเดลจากศิลปิน)
public static class Art
{
    public static StandardMaterial3D Mat(Color albedo, float rough = 0.8f, float metal = 0f, Color? emit = null, float energy = 1f)
    {
        var m = new StandardMaterial3D { AlbedoColor = albedo, Roughness = rough, Metallic = metal };
        if (emit is { } e)
        {
            m.EmissionEnabled = true;
            m.Emission = e;
            m.EmissionEnergyMultiplier = energy;
        }
        return m;
    }

    public static StandardMaterial3D Clear(Color c) => new()
    {
        AlbedoColor = c,
        Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
        ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
        CullMode = BaseMaterial3D.CullModeEnum.Disabled,
    };

    public static MeshInstance3D Part(Node3D parent, Mesh mesh, Material mat, Vector3 pos,
        Vector3? rotDeg = null, Vector3? scale = null, bool shadow = true)
    {
        var mi = new MeshInstance3D { Mesh = mesh, MaterialOverride = mat, Position = pos };
        if (rotDeg is { } r) mi.RotationDegrees = r;
        if (scale is { } s) mi.Scale = s;
        if (!shadow) mi.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
        parent.AddChild(mi);
        return mi;
    }

    public static Node3D Joint(Node3D parent, Vector3 pos)
    {
        var n = new Node3D { Position = pos };
        parent.AddChild(n);
        return n;
    }

    public static CapsuleMesh Capsule(float r, float h) => new() { Radius = r, Height = Mathf.Max(h, 2 * r) };
    public static SphereMesh Sphere(float r) => new() { Radius = r, Height = 2 * r, RadialSegments = 24, Rings = 12 };
    public static BoxMesh Box(float x, float y, float z) => new() { Size = new Vector3(x, y, z) };
    public static PrismMesh Prism(float x, float y, float z) => new() { Size = new Vector3(x, y, z) };

    public static CylinderMesh Cyl(float top, float bottom, float h, int seg = 32) =>
        new() { TopRadius = top, BottomRadius = bottom, Height = h, RadialSegments = seg };

    public static TorusMesh Torus(float inner, float outer) =>
        new() { InnerRadius = inner, OuterRadius = outer, Rings = 64, RingSegments = 12 };

    // พิกัดพื้นของ Sim (x, y) -> โลก 3D (x, สูง, z)
    public static Vector3 V3(NVec v, float y = 0) => new(v.X, y, v.Y);

    // มุมใน Sim (ทิศ cos, sin บนพื้น) -> การหมุนรอบแกน Y ของ node ที่หันหน้าไปทาง -Z
    public static float Yaw(float simAngle) => Mathf.Atan2(-Mathf.Cos(simAngle), -Mathf.Sin(simAngle));

    // ทิศหน้าของกล้อง (หมุนรอบ Y) -> มุมใน Sim
    public static float SimAngleOfYaw(float yaw) => Mathf.Atan2(-Mathf.Cos(yaw), -Mathf.Sin(yaw));

    public static float Smooth(float rate, float dt) => 1f - Mathf.Exp(-rate * dt);
}

// สนามประลองกลางซากวิหาร มีบัลลังก์ว่างตั้งอยู่ด้านเหนือ
public static class Arena3D
{
    public static List<Node3D> Build(Node3D root)
    {
        var floating = new List<Node3D>();
        float r = Eighth.Sim.Cfg.ArenaRadius;
        var stone = Art.Mat(new Color("3b3f4a"), 0.92f);
        var darkStone = Art.Mat(new Color("23262e"), 0.95f);
        var gold = Art.Mat(new Color("c9a24a"), 0.35f, 0.8f, new Color("f5c542"), 0.6f);
        var rune = Art.Mat(new Color("1b1f2a"), 0.6f, 0f, new Color("4fb8e8"), 0.55f);

        Art.Part(root, new PlaneMesh { Size = new Vector2(160, 160) }, Art.Mat(new Color("161a22"), 1f), new Vector3(0, -0.62f, 0));
        Art.Part(root, Art.Cyl(r, r + 0.8f, 0.6f, 96), Art.Mat(new Color("34373f"), 0.9f), new Vector3(0, -0.3f, 0));
        Art.Part(root, Art.Torus(r - 0.25f, r + 0.35f), darkStone, new Vector3(0, 0.02f, 0), scale: new Vector3(1, 0.5f, 1));

        // วงอักขระบนพื้น
        foreach (float rr in new[] { 4.5f, 9.5f })
            Art.Part(root, Art.Torus(rr - 0.04f, rr + 0.04f), rune, new Vector3(0, 0.005f, 0), scale: new Vector3(1, 0.08f, 1), shadow: false);
        for (int i = 0; i < 8; i++)
        {
            float a = i * Mathf.Tau / 8;
            var mid = new Vector3(Mathf.Cos(a) * 7f, 0.006f, Mathf.Sin(a) * 7f);
            Art.Part(root, Art.Box(0.06f, 0.01f, 4.8f), rune, mid, new Vector3(0, -Mathf.RadToDeg(a) + 90, 0), shadow: false);
        }

        // เสาหินรอบสนาม บางต้นหัก บนยอดมีผลึกลอย
        var rng = new RandomNumberGenerator { Seed = 8 };
        for (int i = 0; i < 14; i++)
        {
            float a = i * Mathf.Tau / 14 + 0.11f;
            if (Mathf.Abs(Mathf.Wrap(a - 1.5f * Mathf.Pi, -Mathf.Pi, Mathf.Pi)) < 0.35f) continue; // เว้นทางไปบัลลังก์
            var p = new Vector3(Mathf.Cos(a) * (r + 2.6f), 0, Mathf.Sin(a) * (r + 2.6f));
            float h = rng.RandfRange(2.5f, 7.5f);
            bool broken = rng.Randf() < 0.35f;
            if (broken) h *= 0.45f;
            Art.Part(root, Art.Cyl(0.55f, 0.7f, h, 12), stone, p + new Vector3(0, h / 2 - 0.6f, 0));
            Art.Part(root, Art.Box(1.7f, 0.35f, 1.7f), darkStone, p + new Vector3(0, -0.45f, 0));
            if (broken)
            {
                Art.Part(root, Art.Cyl(0.5f, 0.55f, 2.2f, 12), stone,
                    p + new Vector3(rng.RandfRange(-1.5f, 1.5f), -0.15f, rng.RandfRange(-1.5f, 1.5f)),
                    new Vector3(90, rng.RandfRange(0, 180), 0));
                continue;
            }
            Art.Part(root, Art.Box(1.3f, 0.3f, 1.3f), darkStone, p + new Vector3(0, h - 0.45f, 0));
            var crystal = new Node3D { Position = p + new Vector3(0, h + 0.9f, 0) };
            root.AddChild(crystal);
            var tint = i % 3 == 0 ? new Color("b48cff") : new Color("6fd3ff");
            Art.Part(crystal, Art.Prism(0.5f, 1.1f, 0.5f), Art.Mat(tint, 0.2f, 0f, tint, 2.5f), Vector3.Zero, shadow: false);
            Art.Part(crystal, Art.Prism(0.5f, 0.6f, 0.5f), Art.Mat(tint, 0.2f, 0f, tint, 2.5f), new Vector3(0, -0.85f, 0), new Vector3(180, 0, 0), shadow: false);
            floating.Add(crystal);
        }

        // บัลลังก์ที่แปด: ว่างเปล่า ตั้งอยู่นอกสนามด้านเหนือ
        var throne = new Node3D { Position = new Vector3(0, 0, -(r + 9)) };
        root.AddChild(throne);
        for (int i = 0; i < 4; i++)
            Art.Part(throne, Art.Box(9 - i * 1.6f, 0.6f, 6 - i * 1.0f), i % 2 == 0 ? darkStone : stone, new Vector3(0, -0.3f + i * 0.6f, 0));
        var seatY = 2.1f;
        Art.Part(throne, Art.Box(2.6f, 0.5f, 2.2f), stone, new Vector3(0, seatY + 0.25f, 0));
        Art.Part(throne, Art.Box(2.8f, 5.5f, 0.5f), darkStone, new Vector3(0, seatY + 2.9f, 1.0f));
        Art.Part(throne, Art.Box(0.4f, 1.2f, 2.2f), stone, new Vector3(-1.4f, seatY + 0.9f, 0));
        Art.Part(throne, Art.Box(0.4f, 1.2f, 2.2f), stone, new Vector3(1.4f, seatY + 0.9f, 0));
        for (int i = 0; i < 8; i++)
        {
            float x = -1.05f + i * 0.3f;
            float h = 0.6f + (i is 3 or 4 ? 0.9f : i is 2 or 5 ? 0.5f : 0.2f);
            Art.Part(throne, Art.Prism(0.26f, h, 0.3f), gold, new Vector3(x, seatY + 5.65f + h / 2, 1.0f));
        }
        Art.Part(throne, Art.Torus(0.55f, 0.7f), gold, new Vector3(0, seatY + 4.2f, 0.72f), new Vector3(90, 0, 0), shadow: false);
        var glow = new OmniLight3D { Position = new Vector3(0, seatY + 4.2f, -0.5f), LightColor = new Color("f5c542"), LightEnergy = 2.2f, OmniRange = 9f };
        throne.AddChild(glow);
        return floating;
    }
}
