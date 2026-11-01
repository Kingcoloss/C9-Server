using Godot;
using GDict = Godot.Collections.Dictionary;

namespace EighthThrone.Demo;

// แสงเงาแบบอนิเมะ: เงาเป็นขั้นชัด สีเงาอมม่วงแทนสีดำ มีเส้นขอบรอบตัว (ตาม art bible)
// อ่านค่าสีเงาและความหนาเส้นขอบจากไฟล์ VRM (MToon) ถ้ามี
public static class Toon
{
    const string Body = @"
uniform sampler2D albedo_tex : source_color, filter_linear_mipmap, repeat_enable, hint_default_white;
uniform vec4 albedo : source_color = vec4(1.0);
uniform vec3 shade_color : source_color = vec3(0.72, 0.66, 0.84);
uniform float shade_shift : hint_range(-1.0, 1.0) = 0.0;
uniform float shade_toony : hint_range(0.0, 1.0) = 0.9;
uniform float light_scale = 0.85;
uniform vec3 rim_color : source_color = vec3(0.55, 0.62, 0.8);
uniform float rim_power = 4.0;
uniform float rim_strength = 0.35;
uniform vec3 emission_color : source_color = vec3(0.0);
uniform float alpha_cut = 0.5;
instance uniform vec4 flash = vec4(0.0);

void fragment() {
    vec4 c = texture(albedo_tex, UV) * albedo;
    ALBEDO = c.rgb;
    ALPHA_LINE
    ROUGHNESS = 1.0;
    METALLIC = 0.0;
    SPECULAR = 0.0;
    float nv = clamp(dot(NORMAL, VIEW), 0.0, 1.0);
    float rim = pow(1.0 - nv, rim_power) * rim_strength;
    EMISSION = emission_color + rim_color * rim * c.rgb + flash.rgb * flash.a;
}

void light() {
    float ndl = dot(NORMAL, LIGHT);
    float w = max(1.0 - shade_toony, 0.02) * 0.5;
    float band = smoothstep(shade_shift - w, shade_shift + w, ndl);
    if (LIGHT_IS_DIRECTIONAL) {
        float lit = band * smoothstep(0.25, 0.6, ATTENUATION);
        DIFFUSE_LIGHT += mix(shade_color, vec3(1.0), lit) * LIGHT_COLOR / PI * light_scale;
    } else {
        DIFFUSE_LIGHT += band * ATTENUATION * LIGHT_COLOR / PI * light_scale;
    }
}
";

    const string Outline = @"
shader_type spatial;
render_mode unshaded, cull_front, depth_draw_opaque, shadows_disabled, fog_disabled;
uniform vec3 outline_color : source_color = vec3(0.12, 0.08, 0.12);
uniform float width = 0.002;
uniform float min_px = 1.1;
uniform bool screen_mode = false;
uniform sampler2D albedo_tex : source_color, filter_linear_mipmap, hint_default_white;
uniform float alpha_cut = 0.5;
uniform bool use_cut = false;
instance uniform vec4 flash = vec4(0.0);

void vertex() {
    vec4 v = MODELVIEW_MATRIX * vec4(VERTEX, 1.0);
    vec3 n = normalize(mat3(MODELVIEW_MATRIX) * NORMAL);
    float depth = max(-v.z, 0.05);
    float px = 2.0 * depth / (PROJECTION_MATRIX[1][1] * VIEWPORT_SIZE.y);
    float w = screen_mode ? width * depth * 2.0 / PROJECTION_MATRIX[1][1] : width;
    v.xyz += n * max(w, min_px * px);
    POSITION = PROJECTION_MATRIX * v;
}

void fragment() {
    if (use_cut && texture(albedo_tex, UV).a < alpha_cut) discard;
    ALBEDO = outline_color + flash.rgb * flash.a * 0.5;
}
";

    static readonly Dictionary<string, Shader> Cache = new();
    static Shader? _outline;

    public enum Alpha { Opaque, Cutout, Blend }

    public static Shader BodyShader(Alpha alpha, bool doubleSided)
    {
        string key = $"{alpha}{doubleSided}";
        if (Cache.TryGetValue(key, out var s)) return s;
        string mode = alpha switch
        {
            Alpha.Blend => "blend_mix, depth_draw_always",
            _ => "depth_draw_opaque",
        };
        string alphaLine = alpha switch
        {
            Alpha.Cutout => "ALPHA = c.a; ALPHA_SCISSOR_THRESHOLD = alpha_cut;",
            Alpha.Blend => "ALPHA = c.a;",
            _ => "",
        };
        s = new Shader
        {
            Code = $"shader_type spatial;\nrender_mode {mode}, {(doubleSided ? "cull_disabled" : "cull_back")};\n" +
                   Body.Replace("ALPHA_LINE", alphaLine),
        };
        Cache[key] = s;
        return s;
    }

    public static Shader OutlineShader => _outline ??= new Shader { Code = Outline };

    // วัสดุสีเรียบสำหรับอาวุธและของประกอบ
    public static ShaderMaterial Flat(Color c, Color shade, float outline = 0.004f, Color? emission = null)
    {
        var m = new ShaderMaterial { Shader = BodyShader(Alpha.Opaque, false) };
        m.SetShaderParameter("albedo", c);
        m.SetShaderParameter("shade_color", shade);
        if (emission is { } e) m.SetShaderParameter("emission_color", e);
        if (outline > 0)
        {
            var o = new ShaderMaterial { Shader = OutlineShader };
            o.SetShaderParameter("width", outline);
            o.SetShaderParameter("outline_color", new Color(0.1f, 0.07f, 0.1f));
            m.NextPass = o;
        }
        return m;
    }

    // เปลี่ยนวัสดุของทุก mesh ในโมเดลเป็นแบบอนิเมะ
    public static int Apply(Node root, GDict? json, float unitScale)
    {
        var mtoon = ReadMToon(json);
        int count = 0;
        var done = new Dictionary<Material, Material>();
        foreach (var mi in RigLoad.FindAll<MeshInstance3D>(root))
        {
            if (mi.Mesh == null) continue;
            for (int s = 0; s < mi.Mesh.GetSurfaceCount(); s++)
            {
                var src = mi.GetActiveMaterial(s);
                if (src == null) continue;
                if (!done.TryGetValue(src, out var dst))
                {
                    dst = Convert(src, mtoon.GetValueOrDefault(src.ResourceName), unitScale);
                    done[src] = dst;
                    count++;
                }
                mi.SetSurfaceOverrideMaterial(s, dst);
            }
        }
        return count;
    }

    sealed record MToonInfo(Color Shade, float Shift, float Toony, float Outline, bool ScreenOutline, Color OutlineColor,
        Color Rim, float RimPower);

    static Material Convert(Material src, MToonInfo? info, float unitScale)
    {
        Texture2D? tex = null;
        Color albedo = Colors.White;
        var alpha = Alpha.Opaque;
        float cut = 0.5f;
        bool twoSided = false;
        Color emission = Colors.Black;
        if (src is BaseMaterial3D b)
        {
            tex = b.AlbedoTexture;
            albedo = b.AlbedoColor;
            alpha = b.Transparency switch
            {
                BaseMaterial3D.TransparencyEnum.AlphaScissor or BaseMaterial3D.TransparencyEnum.AlphaHash => Alpha.Cutout,
                BaseMaterial3D.TransparencyEnum.Alpha or BaseMaterial3D.TransparencyEnum.AlphaDepthPrePass => Alpha.Blend,
                _ => Alpha.Opaque,
            };
            cut = b.AlphaScissorThreshold;
            twoSided = b.CullMode == BaseMaterial3D.CullModeEnum.Disabled;
            if (b.EmissionEnabled) emission = b.Emission * b.EmissionEnergyMultiplier;
        }
        string name = src.ResourceName.ToLowerInvariant();
        bool face = name.Contains("face") || name.Contains("eye") || name.Contains("brow") || name.Contains("mouth");

        var m = new ShaderMaterial { Shader = BodyShader(alpha, twoSided), ResourceName = src.ResourceName };
        if (tex != null) m.SetShaderParameter("albedo_tex", tex);
        m.SetShaderParameter("albedo", albedo);
        m.SetShaderParameter("alpha_cut", cut);
        m.SetShaderParameter("emission_color", emission);
        if (info != null)
        {
            m.SetShaderParameter("shade_color", info.Shade);
            m.SetShaderParameter("shade_shift", info.Shift);
            m.SetShaderParameter("shade_toony", info.Toony);
            if (info.Rim.R + info.Rim.G + info.Rim.B > 0.01f)
            {
                m.SetShaderParameter("rim_color", info.Rim);
                m.SetShaderParameter("rim_power", info.RimPower);
            }
        }
        // หน้าตาแบบอนิเมะมักไม่มีเงาแข็งบนหน้า: เลื่อนเส้นเงาให้หน้าสว่างเกือบตลอด
        if (face) m.SetShaderParameter("shade_shift", -0.6f);

        float width = info?.Outline ?? (face ? 0 : 0.0025f);
        if (alpha != Alpha.Blend && width > 0 && !(face && info == null))
        {
            var o = new ShaderMaterial { Shader = OutlineShader };
            o.SetShaderParameter("width", info?.ScreenOutline == true ? width : width * unitScale);
            o.SetShaderParameter("screen_mode", info?.ScreenOutline == true);
            o.SetShaderParameter("outline_color", info?.OutlineColor ?? new Color(0.12f, 0.08f, 0.12f));
            if (alpha == Alpha.Cutout && tex != null)
            {
                o.SetShaderParameter("use_cut", true);
                o.SetShaderParameter("albedo_tex", tex);
                o.SetShaderParameter("alpha_cut", cut);
            }
            m.NextPass = o;
        }
        return m;
    }

    // อ่านค่า MToon จาก VRM 1.0 (VRMC_materials_mtoon) หรือ VRM 0.x (materialProperties)
    static Dictionary<string, MToonInfo> ReadMToon(GDict? json)
    {
        var res = new Dictionary<string, MToonInfo>();
        if (json == null) return res;
        static Color Lin(Variant v)
        {
            var a = v.AsGodotArray();
            return a.Count >= 3 ? new Color((float)a[0], (float)a[1], (float)a[2]).LinearToSrgb() : Colors.White;
        }

        if (json.TryGetValue("materials", out var mats) && mats.VariantType == Variant.Type.Array)
        {
            foreach (var mv in mats.AsGodotArray())
            {
                var md = mv.AsGodotDictionary();
                var e = Humanoid.Dig(md, "extensions", "VRMC_materials_mtoon");
                if (e == null) continue;
                string n = md.TryGetValue("name", out var nn) ? nn.AsString() : "";
                string mode = e.TryGetValue("outlineWidthMode", out var om) ? om.AsString() : "none";
                float ow = e.TryGetValue("outlineWidthFactor", out var w) ? (float)w : 0;
                res[n] = new MToonInfo(
                    e.TryGetValue("shadeColorFactor", out var sc) ? Lin(sc) : new Color(0.72f, 0.66f, 0.84f),
                    e.TryGetValue("shadingShiftFactor", out var sh) ? (float)sh : 0f,
                    e.TryGetValue("shadingToonyFactor", out var ty) ? (float)ty : 0.9f,
                    mode == "none" ? 0 : ow,
                    mode == "screenCoordinates",
                    e.TryGetValue("outlineColorFactor", out var oc) ? Lin(oc) : new Color(0.1f, 0.07f, 0.1f),
                    e.TryGetValue("parametricRimColorFactor", out var rc) ? Lin(rc) : Colors.Black,
                    e.TryGetValue("parametricRimFresnelPowerFactor", out var rp) ? (float)rp : 5f);
            }
        }

        var vrm0 = Humanoid.Dig(json, "extensions", "VRM");
        if (vrm0 != null && vrm0.TryGetValue("materialProperties", out var mp) && mp.VariantType == Variant.Type.Array)
        {
            foreach (var pv in mp.AsGodotArray())
            {
                var p = pv.AsGodotDictionary();
                string n = p.TryGetValue("name", out var nn) ? nn.AsString() : "";
                var fl = Humanoid.Dig(p, "floatProperties") ?? new GDict();
                var vec = Humanoid.Dig(p, "vectorProperties") ?? new GDict();
                float F(string k, float d) => fl.TryGetValue(k, out var v) ? (float)v : d;
                Color C(string k, Color d)
                {
                    if (!vec.TryGetValue(k, out var v)) return d;
                    var a = v.AsGodotArray();
                    return a.Count >= 3 ? new Color((float)a[0], (float)a[1], (float)a[2]) : d;
                }
                int mode = (int)F("_OutlineWidthMode", 0);
                res[n] = new MToonInfo(
                    C("_ShadeColor", new Color(0.72f, 0.66f, 0.84f)),
                    F("_ShadeShift", 0), F("_ShadeToony", 0.9f),
                    mode == 0 ? 0 : F("_OutlineWidth", 0) * (mode == 1 ? 0.01f : 0.01f),
                    mode == 2,
                    C("_OutlineColor", new Color(0.1f, 0.07f, 0.1f)),
                    C("_RimColor", Colors.Black), F("_RimFresnelPower", 5f));
            }
        }
        return res;
    }
}
