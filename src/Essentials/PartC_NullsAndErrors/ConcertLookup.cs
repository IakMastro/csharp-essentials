namespace Essentials;

public static class ConcertLookup
{
    /// Case-insensitive (ordinal ignore case) title match; null when not found.
    public static Concert? FindByTitle(IEnumerable<Concert> concerts, string title)
        => concerts.FirstOrDefault(c => string.Equals(c.Title, title, StringComparison.OrdinalIgnoreCase));

    /// "<Venue> on <yyyy-MM-dd>" for a concert, "Unknown venue" for null.
    public static string Describe(Concert? concert)
        => concert is null
            ? "Unknown venue"
            : $"{concert.Venue} on {concert.Date:yyyy-MM-dd}";
}
