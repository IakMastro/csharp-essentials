namespace Essentials;

public static class SheetRowParser
{
    /// Each row has 7 cells in sheet order: Event, Artists, Venue, Promoter, Date, Rating, Notes.
    /// Row numbers in warnings are 1-based and count the header as row 1 (first data row is row 2).
    public static ParseResult Parse(IEnumerable<string[]> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        var concerts = new List<Concert>();
        var warnings = new List<string>();
        int rowNum = 1; // header row

        foreach (var cells in rows)
        {
            rowNum++;
            if (cells.Length < 6)
            {
                warnings.Add($"Row {rowNum}: expected 7 cells, got {cells.Length}; row skipped.");
                continue;
            }

            string title = cells[0];

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var artists = new List<string>();
            var duplicatedArtists = new List<string>();
            foreach (var artist in ArtistParser.Parse(cells[1]))
            {
                if (seen.Add(artist))
                    artists.Add(artist);
                else
                    duplicatedArtists.Add(artist);
            }

            if (string.IsNullOrWhiteSpace(title) && artists.Count == 0)
                continue;

            int rating;
            if (int.TryParse(cells[5], out int r) && r >= 1 && r <= 5)
                rating = r;
            else
            {
                warnings.Add($"Row {rowNum}: rating '{cells[5]}' is outside 1–5; row skipped.");
                continue;
            }

            string venue = cells[2];
            DateOnly date;
            try
            {
                date = SheetDate.Parse(cells[4]);
            }
            catch (FormatException)
            {
                warnings.Add($"Row {rowNum}: invalid date '{cells[4]}'; row skipped.");
                continue;
            }

            foreach (var artist in duplicatedArtists)
                warnings.Add($"Row {rowNum}: duplicate artist '{artist}' ignored.");

            concerts.Add(new Concert(title, date, venue, artists, new Rating(rating)));
        }
        return new ParseResult(concerts.AsReadOnly(), warnings.AsReadOnly());
    }
}
