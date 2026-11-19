namespace Essentials.Tests;

// Part A: types. Specs: exercise §6.1 (AC-01..AC-06) and §6.2 (F-01..F-03).
public class PartA_TypesTests
{
    private static Album Octavarium() => new("Octavarium", "Dream Theater", 2005, MediaFormat.Cd);

    // ---- Must pass ----

    [Fact]
    public void AC01_Albums_with_same_values_are_equal()
    {
        var a = Octavarium();
        var b = Octavarium();

        Assert.True(a == b);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void AC02_With_returns_a_changed_copy_and_leaves_the_original_unchanged()
    {
        var a = Octavarium();

        var b = a with { Format = MediaFormat.Vinyl };

        Assert.Equal(MediaFormat.Vinyl, b.Format);
        Assert.Equal(MediaFormat.Cd, a.Format);
        Assert.True(a != b);
    }

    [Fact]
    public void AC03_Rating_ToString_shows_filled_then_empty_stars()
    {
        var rating = new Rating(3);

        Assert.Equal("★★★☆☆", rating.ToString());
    }

    [Fact]
    public void AC04_Changing_a_copied_SeatPosition_leaves_the_original_unchanged()
    {
        var s1 = new SeatPosition(10, 4);
        var s2 = s1;

        s2 = s2 with { Seat = 5 };

        Assert.Equal(4, s1.Seat);
        Assert.Equal(5, s2.Seat);
        Assert.True(new SeatPosition(10, 4) == s1);
    }

    [Fact]
    public void AC05_Two_variables_pointing_to_one_Shelf_see_the_same_albums()
    {
        var shelf = new Shelf();
        var same = shelf;

        same.Add(Octavarium());

        Assert.Single(shelf.Albums);
    }

    [Fact]
    public void AC06_ByFormat_orders_by_Artist_then_Year()
    {
        var ghostReveries = new Album("Ghost Reveries", "Opeth", 2005, MediaFormat.Vinyl);
        var magma = new Album("Magma", "Gojira", 2016, MediaFormat.Vinyl);
        var blackwaterPark = new Album("Blackwater Park", "Opeth", 2001, MediaFormat.Vinyl);
        var shelf = new Shelf();
        shelf.Add(ghostReveries);
        shelf.Add(magma);
        shelf.Add(blackwaterPark);
        shelf.Add(Octavarium()); // the one CD: must not appear

        var vinyl = shelf.ByFormat(MediaFormat.Vinyl);

        Assert.Equal([magma, blackwaterPark, ghostReveries], vinyl);
    }

    // ---- Must fail cleanly ----

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void F01_Rating_outside_1_to_5_throws_ArgumentOutOfRange(int value)
    {
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Rating(value));

        Assert.Equal("value", ex.ParamName);
        Assert.Contains("Rating must be between 1 and 5", ex.Message);
    }

    [Fact]
    public void F02_Adding_an_equal_album_twice_throws_and_leaves_the_shelf_unchanged()
    {
        var shelf = new Shelf();
        shelf.Add(Octavarium());

        var ex = Assert.Throws<InvalidOperationException>(() => shelf.Add(Octavarium()));

        Assert.Contains("already on the shelf", ex.Message);
        Assert.Single(shelf.Albums);
    }

    [Fact]
    public void F03_Albums_cannot_be_cast_back_to_modify_the_shelf()
    {
        var shelf = new Shelf();
        shelf.Add(Octavarium());
        var extra = new Album("Magma", "Gojira", 2016, MediaFormat.Vinyl);

        var albums = shelf.Albums;

        // Casting to List<Album> must fail: Albums must not be a List<Album>.
        Assert.IsNotType<List<Album>>(albums);
        // If it is an IList<Album>, Add must be refused.
        if (albums is IList<Album> list)
        {
            Assert.Throws<NotSupportedException>(() => list.Add(extra));
        }
        Assert.Single(shelf.Albums);
    }
}
