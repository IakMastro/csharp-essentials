namespace Essentials;

/// My shelf of albums. A reference type: two variables can point to the same shelf.
public class Shelf
{
    private readonly List<Album> _albums = [];
    public IReadOnlyList<Album> Albums => _albums.AsReadOnly();

    /// Adds an album. Throws InvalidOperationException if an equal album is already on the shelf.
    public void Add(Album album)
    {
        if (_albums.Contains(album))
            throw new InvalidOperationException($"Album '{album.Title}' by {album.Artist} is already on the shelf.");
        _albums.Add(album);
    }

    /// Albums of the given format, ordered by Artist, then Year.
    public IReadOnlyList<Album> ByFormat(MediaFormat format)
    {
        return _albums.Where(a => a.Format == format)
                     .OrderBy(a => a.Artist)
                     .ThenBy(a => a.Year)
                     .ToList()
                     .AsReadOnly();
    }
}
