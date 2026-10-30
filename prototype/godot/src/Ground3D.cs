using Eighth.Sim;
using Godot;

namespace EighthThrone.Demo;

// วาดวงเตือนท่าบนพื้นและแนวฟันของผู้เล่นใหม่ทุกเฟรม (สามเหลี่ยมสีโปร่งแสง)
public partial class GroundFx : MeshInstance3D
{
    readonly ImmediateMesh _mesh = new();
    readonly List<Vector3> _v = new();
    readonly List<Color> _c = new();

    public GroundFx()
    {
        Mesh = _mesh;
        CastShadow = ShadowCastingSetting.Off;
        MaterialOverride = new StandardMaterial3D
        {
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            VertexColorUseAsAlbedo = true,
            CullMode = BaseMaterial3D.CullModeEnum.Disabled,
        };
    }

    public void Begin()
    {
        _v.Clear();
        _c.Clear();
    }

    public void End()
    {
        _mesh.ClearSurfaces();
        if (_v.Count == 0) return;
        _mesh.SurfaceBegin(Mesh.PrimitiveType.Triangles);
        for (int i = 0; i < _v.Count; i++)
        {
            _mesh.SurfaceSetColor(_c[i]);
            _mesh.SurfaceAddVertex(_v[i]);
        }
        _mesh.SurfaceEnd();
    }

    void Tri(Vector3 a, Vector3 b, Vector3 c, Color col)
    {
        _v.Add(a); _c.Add(col);
        _v.Add(b); _c.Add(col);
        _v.Add(c); _c.Add(col);
    }

    static Vector3 Dir(float simAngle) => new(Mathf.Cos(simAngle), 0, Mathf.Sin(simAngle));

    // พื้นที่ทรงพัดหรือวงกลม (half >= PI คือวงกลมเต็ม) ระหว่างรัศมี r0..r1
    public void Fill(Vector3 c, float r0, float r1, float facing, float half, Color col, float y)
    {
        if (r1 <= r0 + 0.001f) return;
        bool full = half >= Mathf.Pi - 0.01f;
        if (full)
        {
            facing = 0;
            half = Mathf.Pi;
        }
        int n = Mathf.Max(8, (int)(half * 2 * 12));
        c.Y = y;
        for (int i = 0; i < n; i++)
        {
            float a0 = facing - half + 2 * half * i / n;
            float a1 = facing - half + 2 * half * (i + 1) / n;
            var o0 = c + Dir(a0) * r1;
            var o1 = c + Dir(a1) * r1;
            if (r0 <= 0.001f)
            {
                Tri(c, o0, o1, col);
                continue;
            }
            var i0 = c + Dir(a0) * r0;
            var i1 = c + Dir(a1) * r0;
            Tri(i0, o0, o1, col);
            Tri(i0, o1, i1, col);
        }
    }

    // เส้นขอบ: วงนอกและขอบสองข้างของพัด
    public void Outline(Vector3 c, float r, float facing, float half, Color col, float w, float y)
    {
        Fill(c, Mathf.Max(0, r - w), r, facing, half, col, y);
        if (half >= Mathf.Pi - 0.01f) return;
        foreach (float a in new[] { facing - half, facing + half })
        {
            var d = Dir(a);
            var side = new Vector3(-d.Z, 0, d.X) * (w / 2);
            var p0 = new Vector3(c.X, y, c.Z);
            var p1 = p0 + d * r;
            Tri(p0 - side, p0 + side, p1 + side, col);
            Tri(p0 - side, p1 + side, p1 - side, col);
        }
    }

    public void Shape(in AttackShape s, float scale, Color col, float y) =>
        Fill(new Vector3(s.Center.X, 0, s.Center.Y), 0, s.Radius * scale,
            s.Facing, s.Kind == ShapeKind.Sector ? s.HalfArc : Mathf.Pi, col, y);

    public void ShapeOutline(in AttackShape s, Color col, float w, float y) =>
        Outline(new Vector3(s.Center.X, 0, s.Center.Y), s.Radius,
            s.Facing, s.Kind == ShapeKind.Sector ? s.HalfArc : Mathf.Pi, col, w, y);

    // แนวฟันของผู้เล่น: ริบบิ้นโค้งระดับเอวตามระยะและมุมของท่า
    public void Slash(Vector3 at, float r, float facing, float half, Color col, float height)
    {
        bool full = half >= Mathf.Pi - 0.01f;
        if (full)
        {
            facing = 0;
            half = Mathf.Pi;
        }
        int n = Mathf.Max(8, (int)(half * 2 * 10));
        for (int i = 0; i < n; i++)
        {
            float a0 = facing - half + 2 * half * i / n;
            float a1 = facing - half + 2 * half * (i + 1) / n;
            var b0 = at + Dir(a0) * r + Vector3.Up * (height - 0.25f);
            var b1 = at + Dir(a1) * r + Vector3.Up * (height - 0.25f);
            var t0 = b0 + Vector3.Up * 0.5f;
            var t1 = b1 + Vector3.Up * 0.5f;
            Tri(b0, t0, t1, col);
            Tri(b0, t1, b1, col);
            var m0 = at + Dir(a0) * (r * 0.35f) + Vector3.Up * height;
            var m1 = at + Dir(a1) * (r * 0.35f) + Vector3.Up * height;
            Tri(m0, b0 + Vector3.Up * 0.25f, b1 + Vector3.Up * 0.25f, Palette.A(col, col.A * 0.35f));
            Tri(m0, b1 + Vector3.Up * 0.25f, m1, Palette.A(col, col.A * 0.35f));
        }
    }
}
