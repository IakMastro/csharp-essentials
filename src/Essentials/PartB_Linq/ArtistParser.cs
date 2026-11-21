namespace Essentials;

public static class ArtistParser
{
    /// Splits a sheet's artist cell on commas, trims each name, drops empty entries,
    /// keeps the original order (headliner first).
    public static IReadOnlyList<string> Parse(string cell) => cell.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                 .ToList()
                 .AsReadOnly();
}
