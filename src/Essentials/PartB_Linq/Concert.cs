namespace Essentials;

/// One row of my concerts history.
public record Concert(string Title, DateOnly Date, string Venue, IReadOnlyList<string> Artists, Rating Rating);
