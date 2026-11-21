namespace Essentials.Tests;

// Part B: LINQ. Specs: exercise §6.1 (AC-07..AC-13) and §6.2 (F-04).
public class PartB_LinqTests
{
    private static Concert Make(
        string title,
        DateOnly date,
        string venue = "Gazarte",
        int rating = 3,
        string[]? artists = null)
        => new(title, date, venue, artists ?? ["Some Band"], new Rating(rating));

    // ---- Must pass ----

    [Fact]
    public void AC07_ArtistParser_splits_trims_and_keeps_order()
    {
        var artists = ArtistParser.Parse("Megadeth, Blind Guardian,  Grand Magus ");

        Assert.Equal(["Megadeth", "Blind Guardian", "Grand Magus"], artists);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AC08_ArtistParser_returns_an_empty_list_for_a_blank_cell(string cell)
    {
        var artists = ArtistParser.Parse(cell);

        Assert.NotNull(artists);
        Assert.Empty(artists);
    }

    [Fact]
    public void AC09_InYear_is_deferred_and_sees_concerts_added_after_the_call()
    {
        var list = new List<Concert> { Make("First", new DateOnly(2025, 3, 1)) };
        var q = ConcertQueries.InYear(list, 2025);

        list.Add(Make("Second", new DateOnly(2025, 9, 1)));

        Assert.Equal(2, q.Count());
    }

    [Fact]
    public void AC10_InYearSnapshot_is_not_affected_by_later_changes()
    {
        var list = new List<Concert> { Make("First", new DateOnly(2025, 3, 1)) };
        var snap = ConcertQueries.InYearSnapshot(list, 2025);

        list.Add(Make("Second", new DateOnly(2025, 9, 1)));

        Assert.Single(snap);
    }

    [Fact]
    public void AC11_TopRated_orders_newest_first_then_by_title_ordinal()
    {
        Concert[] concerts =
        [
            Make("Run for Your Lives Tour", new DateOnly(2026, 5, 23), rating: 5),
            Make("M72 World Tour", new DateOnly(2026, 5, 23), rating: 5),
            Make("December Show", new DateOnly(2025, 12, 1), rating: 4),
            Make("Summer Show", new DateOnly(2026, 7, 19), rating: 2),
        ];

        var titles = ConcertQueries.TopRated(concerts, 5);

        Assert.Equal(["M72 World Tour", "Run for Your Lives Tour"], titles);
    }

    [Fact]
    public void AC12_CountByVenue_counts_concerts_per_venue()
    {
        Concert[] concerts =
        [
            Make("A", new DateOnly(2024, 1, 1), venue: "Κύτταρο"),
            Make("B", new DateOnly(2024, 2, 1), venue: "Gazarte"),
            Make("C", new DateOnly(2024, 3, 1), venue: "Κύτταρο"),
            Make("D", new DateOnly(2024, 4, 1), venue: "Κύτταρο"),
        ];

        var counts = ConcertQueries.CountByVenue(concerts);

        Assert.Equal(2, counts.Count);
        Assert.Equal(3, counts["Κύτταρο"]);
        Assert.Equal(1, counts["Gazarte"]);
    }

    [Fact]
    public void AC13_MostSeenArtists_counts_each_artist_once_per_concert()
    {
        Concert[] concerts =
        [
            Make("A", new DateOnly(2024, 1, 1), artists: ["Megadeth", "Sepultura"]),
            Make("B", new DateOnly(2024, 2, 1), artists: ["Megadeth"]),
            Make("C", new DateOnly(2024, 3, 1), artists: ["Dream Theater"]),
            Make("D", new DateOnly(2024, 4, 1), artists: ["Dream Theater", "Megadeth"]),
        ];

        var top = ConcertQueries.MostSeenArtists(concerts, 2);

        Assert.Equal([("Megadeth", 3), ("Dream Theater", 2)], top);
    }

    // ---- Must fail cleanly ----

    [Fact]
    public void F04_TopRated_and_MostSeenArtists_reject_out_of_range_arguments()
    {
        Concert[] concerts = [Make("A", new DateOnly(2024, 1, 1))];

        var topRated = Assert.Throws<ArgumentOutOfRangeException>(() => ConcertQueries.TopRated(concerts, 0));
        var mostSeen = Assert.Throws<ArgumentOutOfRangeException>(() => ConcertQueries.MostSeenArtists(concerts, 0));

        Assert.Equal("minRating", topRated.ParamName);
        Assert.Equal("top", mostSeen.ParamName);
    }
}
