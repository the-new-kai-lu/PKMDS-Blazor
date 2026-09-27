namespace Pkmds.Core.Utilities;

/// <summary>Bundled artwork overrides for species without a public sprite source.</summary>
public static class LocalPokemonSprites
{
    // Paths are relative to the shared sprite root. Add an entry and both PNGs to
    // Pkmds.Rcl/wwwroot/sprites/fakemon/ when introducing another local species.
    // The approved Fakemon designs currently share artwork across forms/genders.
    private static readonly IReadOnlyDictionary<ushort, string> SpeciesFiles = new Dictionary<ushort, string>
    {
        [1076] = "voltuff",
        [1077] = "surguenon",
        [1078] = "raijinque",
        [1079] = "embernewt",
        [1080] = "pyrovaran",
        [1081] = "magmalisk",
        [1082] = "rimevaran",
        [1083] = "fimbulisk",
        [1084] = "sedgling",
        [1085] = "cragaviar",
        [1086] = "ragnaroc"
    };

    /// <summary>Relative normal/shiny image path, or null to retain the standard sprite provider.</summary>
    public static string? GetRelativePath(ushort species, bool isShiny = false) =>
        SpeciesFiles.TryGetValue(species, out var file)
            ? $"fakemon/{(isShiny ? "shiny/" : string.Empty)}{file}.png"
            : null;

    /// <summary>Base-path-relative URL, including when PKMDS is hosted below a subdirectory.</summary>
    public static string? GetUrl(ushort species, bool isShiny = false) =>
        GetRelativePath(species, isShiny) is { } path ? $"{SpriteSource.BundledBaseUrl}{path}" : null;
}
