using System.Globalization;

namespace Essentials;

public static class SheetDate
{
    /// Parses the sheet's M/D/YYYY dates with the invariant culture, e.g. "6/25/2023" -> 2023-06-25.
    /// Throws FormatException: "Invalid date '<text>': expected M/D/YYYY."
    public static DateOnly Parse(string text)
    {
        try
        {
            return DateOnly.ParseExact(text, "M/d/yyyy", CultureInfo.InvariantCulture);
        }
        catch (FormatException)
        {
            throw new FormatException($"Invalid date '{text}': expected M/D/YYYY.");
        }
    }
}
