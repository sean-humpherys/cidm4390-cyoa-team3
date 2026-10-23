using System;

public class Die
{
    // Property with a private setter so it can be updated via constructor or method,
    // or change to 'public set;' if you want direct assignment like die.Sides = 6;
    public int Sides { get; set; }

    /// <summary>
    /// Default constructor: defaults to a 20-sided die if no number is provided.
    /// </summary>
    public Die()
    {
        Sides = 20;
    }

    /// <summary>
    /// Overloaded constructor: lets other classes specify the side count when creating it.
    /// </summary>
    public Die(int sides)
    {
        if (sides < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(sides), "A die must have at least 1 side.");
        }
        Sides = sides;
    }

    /// <summary>
    /// Rolls the die and returns a random number between 1 and Sides.
    /// </summary>
    public int Roll()
    {
        return Random.Shared.Next(1, Sides + 1);
    }
}