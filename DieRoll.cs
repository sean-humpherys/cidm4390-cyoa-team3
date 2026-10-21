using System;

public class D20
{
    // Property to store the number of sides
    public int Sides { get; } = 20;

    /// <summary>
    /// Rolls the die and returns a random number between 1 and 20.
    /// </summary>
    public int Roll()
    {
        // Random.Shared generates random numbers safely without extra setup
        return Random.Shared.Next(1, Sides + 1);
    }
}