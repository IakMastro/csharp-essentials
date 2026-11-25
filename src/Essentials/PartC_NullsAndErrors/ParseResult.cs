namespace Essentials;

/// What the parser returns: the concerts it could build, plus warnings for rows it skipped or changed.
public record ParseResult(IReadOnlyList<Concert> Concerts, IReadOnlyList<string> Warnings);
