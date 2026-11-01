using Godot;

namespace EighthThrone.Demo;

// พื้นผิวที่สร้างจาก noise (ไม่มีไฟล์ภาพจากที่อื่น) ใช้ร่วมกันทุกวัสดุเพื่อไม่ให้สร้างซ้ำ
public static class Tex
{
    static NoiseTexture2D? _stoneNormal, _stoneAlbedo, _tileNormal, _detailNormal, _roughness;
    static GradientTexture2D? _soft;

    static FastNoiseLite Noise(FastNoiseLite.NoiseTypeEnum type, float freq, int octaves, int seed) => new()
    {
        NoiseType = type,
        Frequency = freq,
        FractalOctaves = octaves,
        Seed = seed,
    };

    // หินผุ: ลายเล็กละเอียดสำหรับเสาและบัลลังก์
    public static NoiseTexture2D StoneNormal => _stoneNormal ??= new NoiseTexture2D
    {
        Width = 512, Height = 512, Seamless = true, AsNormalMap = true, BumpStrength = 9f,
        Noise = Noise(FastNoiseLite.NoiseTypeEnum.SimplexSmooth, 0.03f, 5, 3),
    };

    public static NoiseTexture2D StoneAlbedo => _stoneAlbedo ??= new NoiseTexture2D
    {
        Width = 512, Height = 512, Seamless = true,
        Noise = Noise(FastNoiseLite.NoiseTypeEnum.SimplexSmooth, 0.012f, 4, 5),
        ColorRamp = new Gradient
        {
            Offsets = new[] { 0f, 0.55f, 1f },
            Colors = new[] { new Color(0.62f, 0.62f, 0.66f), new Color(0.86f, 0.85f, 0.88f), new Color(1f, 0.98f, 0.95f) },
        },
    };

    // แผ่นหินปูพื้น: cellular noise ให้รอยต่อระหว่างแผ่น
    public static NoiseTexture2D TileNormal => _tileNormal ??= new NoiseTexture2D
    {
        Width = 1024, Height = 1024, Seamless = true, AsNormalMap = true, BumpStrength = 5f,
        Noise = new FastNoiseLite
        {
            NoiseType = FastNoiseLite.NoiseTypeEnum.Cellular,
            Frequency = 0.008f,
            CellularDistanceFunction = FastNoiseLite.CellularDistanceFunctionEnum.Euclidean,
            CellularReturnType = FastNoiseLite.CellularReturnTypeEnum.Distance2Sub,
            FractalType = FastNoiseLite.FractalTypeEnum.None,
            Seed = 11,
        },
    };

    public static NoiseTexture2D DetailNormal => _detailNormal ??= new NoiseTexture2D
    {
        Width = 256, Height = 256, Seamless = true, AsNormalMap = true, BumpStrength = 3f,
        Noise = Noise(FastNoiseLite.NoiseTypeEnum.Perlin, 0.08f, 3, 7),
    };

    // ความด้านของผิว 0.72..1 (หินแห้ง ไม่เงาเหมือนพื้นเปียก)
    public static NoiseTexture2D Roughness => _roughness ??= new NoiseTexture2D
    {
        Width = 256, Height = 256, Seamless = true,
        Noise = Noise(FastNoiseLite.NoiseTypeEnum.SimplexSmooth, 0.05f, 3, 9),
        ColorRamp = new Gradient
        {
            Offsets = new[] { 0f, 1f },
            Colors = new[] { new Color(0.72f, 0.72f, 0.72f), Colors.White },
        },
    };

    // จุดกลมขอบนุ่ม ใช้กับไฟ ฝุ่น และประกายไฟ
    public static GradientTexture2D Soft => _soft ??= new GradientTexture2D
    {
        Width = 64, Height = 64,
        Fill = GradientTexture2D.FillEnum.Radial,
        FillFrom = new Vector2(0.5f, 0.5f),
        FillTo = new Vector2(1f, 0.5f),
        Gradient = new Gradient
        {
            Offsets = new[] { 0f, 0.35f, 1f },
            Colors = new[] { Colors.White, new Color(1, 1, 1, 0.55f), new Color(1, 1, 1, 0) },
        },
    };

    // หิน: สีพื้น x ลาย noise + normal map แบบ triplanar (ไม่ต้อง UV ก็ไม่ยืด)
    public static StandardMaterial3D Stone(Color tint, float scale, bool tiles = false)
    {
        var m = new StandardMaterial3D
        {
            AlbedoColor = tint,
            AlbedoTexture = StoneAlbedo,
            NormalEnabled = true,
            NormalTexture = tiles ? TileNormal : StoneNormal,
            NormalScale = tiles ? 0.7f : 1f,
            Roughness = 1f,
            RoughnessTexture = Roughness,
            RoughnessTextureChannel = BaseMaterial3D.TextureChannel.Red,
            Uv1Triplanar = true,
            Uv1WorldTriplanar = true,
            Uv1Scale = Vector3.One * scale,
            TextureFilter = BaseMaterial3D.TextureFilterEnum.LinearWithMipmapsAnisotropic,
        };
        return m;
    }

    // เพิ่มรายละเอียดผิว (ผ้า ขน เกราะ) ให้วัสดุที่มีอยู่
    public static void Detail(StandardMaterial3D m, float scale, float strength = 0.6f)
    {
        m.NormalEnabled = true;
        m.NormalTexture = DetailNormal;
        m.NormalScale = strength;
        m.Uv1Triplanar = true;
        m.Uv1Scale = Vector3.One * scale;
    }

    public static StandardMaterial3D Additive(Color c) => new()
    {
        ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
        BlendMode = BaseMaterial3D.BlendModeEnum.Add,
        Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
        BillboardMode = BaseMaterial3D.BillboardModeEnum.Particles,
        VertexColorUseAsAlbedo = true,
        AlbedoColor = c,
        AlbedoTexture = Soft,
        CullMode = BaseMaterial3D.CullModeEnum.Disabled,
        NoDepthTest = false,
    };
}

// เตาไฟรอบสนาม: เปลวไฟ ประกายไฟลอยขึ้น และแสงที่สั่นไหว
public partial class Brazier : Node3D
{
    OmniLight3D _light = null!;
    readonly FastNoiseLite _flicker = new() { Frequency = 2.5f };
    float _t;
    public bool CastShadow;

    public override void _Ready()
    {
        var stone = Tex.Stone(new Color("4a4b55"), 0.9f);
        Art.Part(this, Art.Cyl(0.32f, 0.45f, 1.4f, 16), stone, new Vector3(0, 0.7f, 0));
        Art.Part(this, Art.Cyl(0.7f, 0.42f, 0.38f, 20), stone, new Vector3(0, 1.55f, 0));
        Art.Part(this, Art.Cyl(0.6f, 0.6f, 0.06f, 20), Art.Mat(new Color("1a0d08"), 1f, 0, new Color("ff6a1a"), 1.2f), new Vector3(0, 1.72f, 0), shadow: false);

        AddChild(Flames(new Vector3(0, 1.75f, 0)));
        AddChild(Embers(new Vector3(0, 1.9f, 0)));
        _light = new OmniLight3D
        {
            Position = new Vector3(0, 2.4f, 0),
            LightColor = new Color("ff9a45"),
            LightEnergy = 2.6f,
            OmniRange = 11f,
            OmniAttenuation = 1.4f,
            ShadowEnabled = CastShadow,
            LightVolumetricFogEnergy = 1.5f,
        };
        AddChild(_light);
        _t = GD.Randf() * 100;
    }

    public override void _Process(double delta)
    {
        _t += (float)delta;
        _light.LightEnergy = 2.3f + 0.6f * _flicker.GetNoise1D(_t * 10);
    }

    static GpuParticles3D Flames(Vector3 at)
    {
        var pm = new ParticleProcessMaterial
        {
            EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Sphere,
            EmissionSphereRadius = 0.28f,
            Direction = Vector3.Up,
            Spread = 12,
            InitialVelocityMin = 0.8f,
            InitialVelocityMax = 1.6f,
            Gravity = new Vector3(0, 1.2f, 0),
            ScaleMin = 0.7f,
            ScaleMax = 1.2f,
            ScaleCurve = Curve01(1f, 0.15f),
            ColorRamp = new GradientTexture1D
            {
                Gradient = new Gradient
                {
                    Offsets = new[] { 0f, 0.25f, 0.6f, 1f },
                    Colors = new[] { new Color(1f, 0.9f, 0.55f, 0.9f), new Color(1f, 0.55f, 0.15f, 0.8f), new Color(0.8f, 0.2f, 0.05f, 0.4f), new Color(0.2f, 0.05f, 0.02f, 0f) },
                },
            },
        };
        return new GpuParticles3D
        {
            Position = at,
            Amount = 48,
            Lifetime = 0.75f,
            ProcessMaterial = pm,
            DrawPass1 = new QuadMesh { Size = new Vector2(0.55f, 0.55f), Material = Tex.Additive(new Color(1, 1, 1)) },
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            VisibilityAabb = new Aabb(new Vector3(-1, -0.5f, -1), new Vector3(2, 3, 2)),
        };
    }

    static GpuParticles3D Embers(Vector3 at)
    {
        var pm = new ParticleProcessMaterial
        {
            EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Sphere,
            EmissionSphereRadius = 0.3f,
            Direction = Vector3.Up,
            Spread = 25,
            InitialVelocityMin = 1.2f,
            InitialVelocityMax = 2.6f,
            Gravity = new Vector3(0.3f, 0.6f, 0),
            TurbulenceEnabled = true,
            TurbulenceNoiseStrength = 1.5f,
            TurbulenceNoiseScale = 3f,
            ScaleMin = 0.4f,
            ScaleMax = 1f,
            ColorRamp = new GradientTexture1D
            {
                Gradient = new Gradient
                {
                    Offsets = new[] { 0f, 0.7f, 1f },
                    Colors = new[] { new Color(1f, 0.75f, 0.3f, 1f), new Color(1f, 0.4f, 0.1f, 0.8f), new Color(1f, 0.3f, 0.1f, 0f) },
                },
            },
        };
        return new GpuParticles3D
        {
            Position = at,
            Amount = 14,
            Lifetime = 2.2f,
            ProcessMaterial = pm,
            DrawPass1 = new QuadMesh { Size = new Vector2(0.06f, 0.06f), Material = Tex.Additive(new Color(1, 1, 1)) },
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            VisibilityAabb = new Aabb(new Vector3(-3, -1, -3), new Vector3(6, 8, 6)),
        };
    }

    public static CurveTexture Curve01(float from, float to)
    {
        var c = new Curve { MaxValue = Mathf.Max(1f, Mathf.Max(from, to)) };
        c.AddPoint(new Vector2(0, from));
        c.AddPoint(new Vector2(1, to));
        return new CurveTexture { Curve = c };
    }
}

public static class Vfx
{
    // ฝุ่นละอองลอยทั่วสนาม เห็นชัดในแสงหมอก
    public static GpuParticles3D Dust(float radius)
    {
        var pm = new ParticleProcessMaterial
        {
            EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Box,
            EmissionBoxExtents = new Vector3(radius, 3f, radius),
            Direction = new Vector3(1, 0.2f, 0.3f),
            Spread = 180,
            InitialVelocityMin = 0.05f,
            InitialVelocityMax = 0.25f,
            Gravity = Vector3.Zero,
            TurbulenceEnabled = true,
            TurbulenceNoiseStrength = 0.6f,
            TurbulenceNoiseScale = 6f,
            ScaleMin = 0.5f,
            ScaleMax = 1.2f,
            ColorRamp = new GradientTexture1D
            {
                Gradient = new Gradient
                {
                    Offsets = new[] { 0f, 0.3f, 0.7f, 1f },
                    Colors = new[] { new Color(1, 0.95f, 0.85f, 0), new Color(1, 0.95f, 0.85f, 0.35f), new Color(1, 0.95f, 0.85f, 0.35f), new Color(1, 0.95f, 0.85f, 0) },
                },
            },
        };
        return new GpuParticles3D
        {
            Position = new Vector3(0, 3f, 0),
            Amount = 260,
            Lifetime = 9f,
            Preprocess = 9f,
            ProcessMaterial = pm,
            DrawPass1 = new QuadMesh { Size = new Vector2(0.035f, 0.035f), Material = Tex.Additive(new Color(1, 1, 1)) },
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
            VisibilityAabb = new Aabb(new Vector3(-radius - 2, -4, -radius - 2), new Vector3(2 * radius + 4, 10, 2 * radius + 4)),
        };
    }

    // ประกายไฟตอนฟันโดน (ยิงครั้งเดียวแล้วลบตัวเอง)
    public static void Sparks(Node3D parent, Vector3 at, Color color, int amount = 26, float speed = 6f)
    {
        var pm = new ParticleProcessMaterial
        {
            EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Sphere,
            EmissionSphereRadius = 0.15f,
            Direction = Vector3.Up,
            Spread = 180,
            InitialVelocityMin = speed * 0.5f,
            InitialVelocityMax = speed,
            Gravity = new Vector3(0, -12, 0),
            DampingMin = 2,
            DampingMax = 4,
            ScaleMin = 0.6f,
            ScaleMax = 1.4f,
            ScaleCurve = Brazier.Curve01(1f, 0f),
            Color = color,
        };
        var p = new GpuParticles3D
        {
            Position = at,
            Amount = amount,
            Lifetime = 0.45f,
            OneShot = true,
            Explosiveness = 1f,
            ProcessMaterial = pm,
            DrawPass1 = new QuadMesh { Size = new Vector2(0.09f, 0.09f), Material = Tex.Additive(new Color(1, 1, 1)) },
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
        };
        parent.AddChild(p);
        p.Emitting = true;
        p.Finished += p.QueueFree;
    }

    // ฝุ่นฟุ้งที่พื้น (ตอนหลบ ตอนบอสทุบ)
    public static void Puff(Node3D parent, Vector3 at, float size = 1f)
    {
        var pm = new ParticleProcessMaterial
        {
            EmissionShape = ParticleProcessMaterial.EmissionShapeEnum.Ring,
            EmissionRingRadius = 0.4f * size,
            EmissionRingInnerRadius = 0.1f * size,
            EmissionRingHeight = 0.05f,
            EmissionRingAxis = Vector3.Up,
            Direction = new Vector3(0, 0.3f, 0),
            Spread = 180,
            InitialVelocityMin = 1.2f * size,
            InitialVelocityMax = 2.4f * size,
            Gravity = new Vector3(0, 0.4f, 0),
            DampingMin = 3,
            DampingMax = 5,
            ScaleMin = 1f,
            ScaleMax = 1.8f,
            ScaleCurve = Brazier.Curve01(0.6f, 1.4f),
            ColorRamp = new GradientTexture1D
            {
                Gradient = new Gradient
                {
                    Offsets = new[] { 0f, 1f },
                    Colors = new[] { new Color(0.75f, 0.72f, 0.7f, 0.45f), new Color(0.75f, 0.72f, 0.7f, 0f) },
                },
            },
        };
        var mat = new StandardMaterial3D
        {
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            BillboardMode = BaseMaterial3D.BillboardModeEnum.Particles,
            VertexColorUseAsAlbedo = true,
            AlbedoTexture = Tex.Soft,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
        };
        var p = new GpuParticles3D
        {
            Position = at + Vector3.Up * 0.1f,
            Amount = (int)(16 * size),
            Lifetime = 0.8f,
            OneShot = true,
            Explosiveness = 0.9f,
            ProcessMaterial = pm,
            DrawPass1 = new QuadMesh { Size = new Vector2(0.6f, 0.6f) * size, Material = mat },
            CastShadow = GeometryInstance3D.ShadowCastingSetting.Off,
        };
        parent.AddChild(p);
        p.Emitting = true;
        p.Finished += p.QueueFree;
    }
}
