using System.Reflection;

namespace C9.Portal.Services;

/// <summary>Finds <see cref="IPortalModule"/> implementations in the host and in C9.Portal.*.dll next to it.</summary>
public sealed class ModuleCatalog
{
    private ModuleCatalog(IReadOnlyList<IPortalModule> modules)
    {
        Modules = modules;
        Assemblies = modules.Select(m => m.GetType().Assembly).Where(a => a != typeof(ModuleCatalog).Assembly).Distinct().ToArray();
    }

    public IReadOnlyList<IPortalModule> Modules { get; }
    /// <summary>Module assemblies other than the host, for the Blazor router.</summary>
    public IReadOnlyList<Assembly> Assemblies { get; }

    public static ModuleCatalog Discover(ILogger? logger = null)
    {
        List<Assembly> candidates = [typeof(ModuleCatalog).Assembly];
        foreach (string file in Directory.EnumerateFiles(AppContext.BaseDirectory, "C9.Portal.*.dll"))
        {
            string name = Path.GetFileNameWithoutExtension(file);
            if (name is "C9.Portal.Abstractions") continue;
            try { candidates.Add(Assembly.Load(new AssemblyName(name))); }
            catch (Exception error) when (error is FileNotFoundException or FileLoadException or BadImageFormatException)
            {
                logger?.LogWarning("Skipped portal module assembly {Name}: {Message}", name, error.Message);
            }
        }
        List<IPortalModule> modules = [];
        foreach (Type type in candidates.Distinct().SelectMany(a => a.GetExportedTypes()))
        {
            if (!type.IsClass || type.IsAbstract || !typeof(IPortalModule).IsAssignableFrom(type) || type.GetConstructor(Type.EmptyTypes) is null) continue;
            modules.Add((IPortalModule)Activator.CreateInstance(type)!);
        }
        string? duplicate = modules.GroupBy(m => m.Id, StringComparer.Ordinal).FirstOrDefault(g => g.Count() > 1)?.Key;
        if (duplicate is not null) throw new InvalidOperationException($"Two portal modules use the id '{duplicate}'");
        foreach (IPortalModule module in modules)
            if (module.Id.Length is 0 or > 32 || !module.Id.All(c => char.IsAsciiLetterLower(c) || char.IsAsciiDigit(c) || c == '-'))
                throw new InvalidOperationException($"Portal module id '{module.Id}' must be lower-case letters, digits or '-'");
        return new ModuleCatalog(modules.OrderBy(m => m.Order).ThenBy(m => m.Id, StringComparer.Ordinal).ToArray());
    }
}
