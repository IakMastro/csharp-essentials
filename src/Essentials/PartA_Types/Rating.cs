namespace Essentials;

/// A concert rating from 1 to 5. A value type: copying it copies the value.
public readonly record struct Rating
{
    public int Value { get; }

    /// Throws ArgumentOutOfRangeException (ParamName "value") unless 1 <= value <= 5.
    public Rating(int value)
    {
        if (value < 1 || value > 5)
            throw new ArgumentOutOfRangeException(nameof(value), value, "Rating must be between 1 and 5.");
        Value = value;
    }

    /// "★★★☆☆" style: Value filled stars, then (5 - Value) empty stars.
    public override string ToString()
    {
        return new string('★', Value) + new string('☆', 5 - Value);
    }
}
