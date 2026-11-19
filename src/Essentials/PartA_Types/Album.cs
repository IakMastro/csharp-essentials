namespace Essentials;

/// An album I own. Immutable; two albums with the same values are equal.
public record Album(string Title, string Artist, int Year, MediaFormat Format);