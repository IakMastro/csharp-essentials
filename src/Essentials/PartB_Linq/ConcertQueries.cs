namespace Essentials;

public static class ConcertQueries
{
    /// Deferred: returns a query, not a list. Concerts in the given year, in source order.
    public static IEnumerable<Concert> InYear(IEnumerable<Concert> concerts, int year) => concerts.Where(c => c.Date.Year == year);

    /// Materialised: same filter, but a snapshot taken at call time.
    public static IReadOnlyList<Concert> InYearSnapshot(IEnumerable<Concert> concerts, int year)
        => InYear(concerts, year).ToList().AsReadOnly();

    /// Titles of concerts rated >= minRating, newest first; ties by Title (ordinal).
    public static IReadOnlyList<string> TopRated(IEnumerable<Concert> concerts, int minRating)
    {
        if (minRating <= 0 || minRating > 5)
            throw new ArgumentOutOfRangeException(nameof(minRating), "minRating must be between 1 and 5");
        return concerts.Where(c => c.Rating.Value >= minRating)
                       .OrderByDescending(c => c.Date)
                       .ThenBy(c => c.Title, StringComparer.Ordinal)
                       .Select(c => c.Title)
                       .ToList()
                       .AsReadOnly();
    }

    /// Number of concerts per venue.
    public static IReadOnlyDictionary<string, int> CountByVenue(IEnumerable<Concert> concerts)
        => concerts.CountBy(c => c.Venue).ToDictionary();

    /// The `top` artists I've seen most often, as (Artist, Count), by Count desc then Artist (ordinal).
    public static IReadOnlyList<(string Artist, int Count)> MostSeenArtists(IEnumerable<Concert> concerts, int top)
    {
        if (top <= 0)
            throw new ArgumentOutOfRangeException(nameof(top), "top must be a positive integer");
        return concerts.SelectMany(c => c.Artists.Distinct())
                       .GroupBy(artist => artist)
                       .Select(g => (Artist: g.Key, Count: g.Count()))
                       .OrderByDescending(ac => ac.Count)
                       .ThenBy(ac => ac.Artist, StringComparer.Ordinal)
                       .Take(top)
                       .ToList()
                       .AsReadOnly();
    }
}