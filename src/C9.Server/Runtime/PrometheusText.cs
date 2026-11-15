using System.Globalization;
using System.Text;

namespace C9.Server;

/// <summary>Minimal Prometheus text exposition (format 0.0.4) without external packages.</summary>
public sealed class PrometheusText
{
    public const string ContentType = "text/plain; version=0.0.4; charset=utf-8";
    private readonly StringBuilder builder = new();
    private readonly HashSet<string> declared = [];

    public PrometheusText Family(string name, string type, string help)
    {
        if (!declared.Add(name)) return this;
        builder.Append("# HELP ").Append(name).Append(' ').Append(help.Replace("\\", "\\\\").Replace("\n", "\\n")).Append('\n');
        builder.Append("# TYPE ").Append(name).Append(' ').Append(type).Append('\n');
        return this;
    }

    public PrometheusText Sample(string name, double value, params (string Name, string Value)[] labels)
    {
        builder.Append(name);
        if (labels.Length > 0)
        {
            builder.Append('{');
            for (int i = 0; i < labels.Length; i++)
            {
                if (i > 0) builder.Append(',');
                builder.Append(labels[i].Name).Append("=\"").Append(Escape(labels[i].Value)).Append('"');
            }
            builder.Append('}');
        }
        builder.Append(' ').Append(Format(value)).Append('\n');
        return this;
    }

    private static string Escape(string value) => value.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");

    private static string Format(double value) =>
        double.IsPositiveInfinity(value) ? "+Inf" : double.IsNegativeInfinity(value) ? "-Inf" : double.IsNaN(value) ? "NaN" :
        value.ToString("G17", CultureInfo.InvariantCulture);

    public override string ToString() => builder.ToString();
}
