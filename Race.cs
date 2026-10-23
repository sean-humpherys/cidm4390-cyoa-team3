using System;

public enum Race
{
    Elf,
    Human,
    Dwarf,
    Halfling
}

public class RaceRules
{
    private readonly Die d4 = new Die(4);

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