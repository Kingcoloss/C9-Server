using Eighth.Sim;
using Godot;

namespace EighthThrone.Demo;

public static class Palette
{
    public static readonly Color Bg = new("0b0f16");
    public static readonly Color Arena = new("151c28");
    public static readonly Color Rim = new("3a4862");
    public static readonly Color Grid = new("1d2636");
    public static readonly Color Gold = new("f5c542");
    public static readonly Color Red = new("ff4d5e");
    public static readonly Color Purple = new("a77bff");
    public static readonly Color Player = new("3dd6c6");
    public static readonly Color Wolf = new("a39383");
    public static readonly Color Boss = new("6a4aa3");
    public static readonly Color Hp = new("e5484d");
    public static readonly Color Stamina = new("46c27a");
    public static readonly Color Poise = new("f5c542");
    public static readonly Color Guard = new("7cc4ff");
    public static readonly Color Damage = new("fff3d6");
    public static readonly Color Hurt = new("ff6b6b");
    public static readonly Color Text = new("e6edf3");
    public static readonly Color Muted = new("8b98a9");
    public static readonly Color Panel = new(0.05f, 0.07f, 0.1f, 0.78f);
    public static readonly Color Ghost = new("ff6bd5");

    public static Color Tele(Tele t) => t switch
    {
        Eighth.Sim.Tele.Red => Red,
        Eighth.Sim.Tele.Purple => Purple,
        _ => Gold,
    };

    public static Color Judgment(Judgment j) => j switch
    {
        Eighth.Sim.Judgment.PerfectDodge or Eighth.Sim.Judgment.PerfectGuard => Gold,
        Eighth.Sim.Judgment.Guard => Guard,
        Eighth.Sim.Judgment.GuardBreak => new Color("ff9f43"),
        Eighth.Sim.Judgment.Hit => Hurt,
        _ => Muted,
    };

    public static Color A(Color c, float a)
    {
        c.A = a;
        return c;
    }
}

// ตัวเลขดาเมจ ข้อความลอย และประกายตอนตีโดน
public sealed class Fx
{
    public sealed class Floater
    {
        public string Text = "";
        public Vector2 Pos;
        public Color Color;
        public int Size;
        public double Born, Life;
    }

    public readonly struct SparkFx
    {
        public readonly Vector2 Pos;
        public readonly double Born;
        public SparkFx(Vector2 pos, double born)
        {
            Pos = pos;
            Born = born;
        }
    }

    public readonly List<Floater> Floaters = new();
    public readonly List<SparkFx> Sparks = new();

    public void Clear()
    {
        Floaters.Clear();
        Sparks.Clear();
    }

    public void Text(string text, Vector2 pos, Color color, int size, double now, double life) =>
        Floaters.Add(new Floater { Text = text, Pos = pos, Color = color, Size = size, Born = now, Life = life });

    public void Spark(Vector2 pos, double now) => Sparks.Add(new SparkFx(pos, now));

    public void Update(double now)
    {
        Floaters.RemoveAll(f => now - f.Born > f.Life);
        Sparks.RemoveAll(s => now - s.Born > 0.18);
    }
}
