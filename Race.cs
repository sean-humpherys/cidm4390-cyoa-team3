using System;

public enum Race
{
    Elf,
    Human,
    Dwarf,
    Halfling
}

public class D4
{
    public int Sides {get;} = 4;

    public int Roll()
    {
        return Random.Shared.Next(1, Sides + 1);
    }
}

public class RaceRules
{
    private readonly D4 d4 = new D4();

    public int RollStrengthModifier(Race race)
    {
        switch (race)
        {
            case Race.Human:
            case Race.Elf:
            case Race.Dwarf:
                return d4.Roll();
            case Race.Halfling:
                return -d4.Roll();
            default:
                throw new ArgumentOutOfRangeException(nameof(race));
        }
    }

    public int RollAgilityModifier (Race race)
    {
        switch (race)
        {
            case Race.Elf:
            case Race.Halfling:
                return d4.Roll();
            case Race.Human:
            case Race.Dwarf:
                return 0;
            default:
                throw new ArgumentOutOfRangeException(nameof(race));
        }
    }
}