namespace Essentials.Tests;

// Part C: nulls and errors. Specs: exercise §6.1 (AC-14..AC-17), §6.2 (F-05, F-06) and §6.3 (W-01..W-03).
public class PartC_NullsAndErrorsTests
{
    private static readonly Concert CrushTheWorld = new(
        "Crush the World Tour",
        new DateOnly(2024, 6, 14),
        "Πλατεία Νερού",
        ["Megadeth", "Blind Guardian", "Grand Magus"],
        new Rating(4));

    private static readonly Concert[] Concerts =
    [
        new("Burn 50 Year Tour", new DateOnly(2024, 5, 24), "Floyd - Live Music Venue", ["Glenn Hughes"], new Rating(5)),
        CrushTheWorld,
    ];

    // Checks every field of a concert. Record equality can't be used directly here:
    // Artists is a list, and two different lists with the same names are not equal by reference.
    private static void AssertConcert(
        string title, DateOnly date, string venue, string[] artists, int rating, Concert actual)
    {
        Assert.Equal(title, actual.Title);
        Assert.Equal(date, actual.Date);
        Assert.Equal(venue, actual.Venue);
        Assert.Equal(artists, actual.Artists);
        Assert.Equal(rating, actual.Rating.Value);
    }

    // ---- Must pass ----

    [Fact]
    public void AC14_FindByTitle_matches_ignoring_case()
    {
        var found = ConcertLookup.FindByTitle(Concerts, "crush the world tour");

        Assert.Same(CrushTheWorld, found);
    }

    [Fact]
    public void AC15_FindByTitle_returns_null_when_missing_and_Describe_handles_it()
    {
        var found = ConcertLookup.FindByTitle(Concerts, "Nope");

        Assert.Null(found);
        Assert.Equal("Unknown venue", ConcertLookup.Describe(found));
    }

    [Theory]
    [InlineData("6/25/2023", 2023, 6, 25)]
    [InlineData("12/8/2023", 2023, 12, 8)]
    public void AC16_SheetDate_parses_M_D_YYYY(string text, int year, int month, int day)
    {
        var date = SheetDate.Parse(text);

        Assert.Equal(new DateOnly(year, month, day), date);
    }

    [Fact]
    public void AC17_SheetRowParser_parses_the_sample_rows()
    {
        // The header row is not passed in, so the first row here is row 2 (§3.5).
        IEnumerable<string[]> rows =
        [
            ["Crush the World Tour", "Megadeth, Blind Guardian, Grand Magus", "Πλατεία Νερού", "Release Athens", "6/14/2024", "4", "notes"],
            ["Burn 50 Year Tour", "Glenn Hughes", "Floyd - Live Music Venue", "", "5/24/2024", "5"],
            ["Bad Rating Night", "Some Band", "Κύτταρο", "", "1/5/2025", "0"],
            ["Typo Night", "Opeth, opeth, Mothers of Million", "Universe Multivenue", "", "6/25/2025", "5", ""],
            ["", "", "", "", "", "0"],
        ];

        var result = SheetRowParser.Parse(rows);

        Assert.Collection(
            result.Concerts,
            c => AssertConcert("Crush the World Tour", new DateOnly(2024, 6, 14), "Πλατεία Νερού",
                ["Megadeth", "Blind Guardian", "Grand Magus"], 4, c),
            c => AssertConcert("Burn 50 Year Tour", new DateOnly(2024, 5, 24), "Floyd - Live Music Venue",
                ["Glenn Hughes"], 5, c),
            c => AssertConcert("Typo Night", new DateOnly(2025, 6, 25), "Universe Multivenue",
                ["Opeth", "Mothers of Million"], 5, c));
        Assert.Equal(
            [
                "Row 4: rating '0' is outside 1–5; row skipped.",
                "Row 5: duplicate artist 'opeth' ignored.",
            ],
            result.Warnings);
    }

    // ---- Must fail cleanly ----

    [Theory]
    [InlineData("13/45/2024")]
    [InlineData("2024-06-14")]
    [InlineData("")]
    public void F05_SheetDate_rejects_anything_but_M_D_YYYY(string text)
    {
        var ex = Assert.Throws<FormatException>(() => SheetDate.Parse(text));

        Assert.Equal($"Invalid date '{text}': expected M/D/YYYY.", ex.Message);
    }

    [Fact]
    public void F06_SheetRowParser_throws_only_when_rows_itself_is_null()
    {
        IEnumerable<string[]>? rows = null;

        Assert.Throws<ArgumentNullException>(() => SheetRowParser.Parse(rows!));
    }

    // ---- Must warn ----

    [Fact]
    public void W01_Rating_outside_1_to_5_skips_the_row_with_a_warning()
    {
        IEnumerable<string[]> rows = [["Bad Rating Night", "Some Band", "Κύτταρο", "", "1/5/2025", "0"]];

        var result = SheetRowParser.Parse(rows);

        Assert.Empty(result.Concerts);
        Assert.Equal(["Row 2: rating '0' is outside 1–5; row skipped."], result.Warnings);
    }

    [Fact]
    public void W02_Duplicate_artist_is_dropped_with_a_warning()
    {
        IEnumerable<string[]> rows =
            [["Typo Night", "Opeth, opeth, Mothers of Million", "Universe Multivenue", "", "6/25/2025", "5", ""]];

        var result = SheetRowParser.Parse(rows);

        var concert = Assert.Single(result.Concerts);
        Assert.Equal(["Opeth", "Mothers of Million"], concert.Artists);
        Assert.Equal(["Row 2: duplicate artist 'opeth' ignored."], result.Warnings);
    }

    [Fact]
    public void W03_Short_row_and_invalid_date_are_skipped_with_warnings()
    {
        IEnumerable<string[]> rows =
        [
            ["Short", "X", "Y"],
            ["Good", "A", "V", "", "13/1/2025", "3"],
        ];

        var result = SheetRowParser.Parse(rows);

        Assert.Empty(result.Concerts);
        Assert.Equal(
            [
                "Row 2: expected 7 cells, got 3; row skipped.",
                "Row 3: invalid date '13/1/2025'; row skipped.",
            ],
            result.Warnings);
    }
}
