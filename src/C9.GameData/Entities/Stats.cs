namespace C9.GameData.Entities;

/// <summary>
/// One character stat the server knows how to send to the Client. Keys are fixed in code (<see cref="Services.StatKeys"/>);
/// operators change the label, whether players may spend points on it and how much one point adds.
/// </summary>
public sealed class StatDefinition
{
    public required string Key { get; set; }
    /// <summary>English label taken from the Client message table (ENG_GameData.c9t).</summary>
    public required string Name { get; set; }
    public short Sort { get; set; }
    public bool Upgradable { get; set; } = true;
    /// <summary>Raw units added to the stat for each spent point.</summary>
    public int PerPoint { get; set; } = 1;
}

/// <summary>Base value of one stat for one base class; advanced jobs use their base class (<see cref="Services.StatService.ClassOf"/>).</summary>
public sealed class ClassStatBase
{
    /// <summary>0 Fighter, 1 Hunter, 2 Shaman, 3 WitchBlade, 4 Mistic (Client template order).</summary>
    public short ClassCode { get; set; }
    public required string StatKey { get; set; }
    public int Value { get; set; }
}

/// <summary>Stat upgrade points granted when a character reaches <see cref="Level"/>. Rows cover levels 1..max without gaps.</summary>
public sealed class StatPointLevel
{
    public short Level { get; set; }
    public int Points { get; set; }
}

/// <summary>Points a player spent on one stat of one character (through the L9 launcher).</summary>
public sealed class CharacterStat
{
    public long CharacterId { get; set; }
    public required string StatKey { get; set; }
    public int Points { get; set; }
}
