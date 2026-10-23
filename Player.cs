using System;

public class Player
{
    public string Name {get; private set;}
    public Race Race{get; private set;}
    public Occupation Occupation {get; private set;}

    public int Strength{get; private set;}
    public int Agility{get; private set;}
    public int Health{get; private set;}

    public Weapon Weapon{get; private set;}
    public bool HasRolledStats{get; private set;}

    public int StrengthModifier =>
        HasRolledStats
            ? CalculateStrengthModifier(Strength)
            : 0;

    private readonly Die d20 = new Die(20);
    private readonly RaceRules raceRules = new RaceRules();
    private readonly OccupationRules occupationRules = 
        new OccupationRules();

    public Player(string name, Race race, Occupation occupation)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Character name cannot be empty.", nameof(name));
        }

        if (!Enum.IsDefined(typeof(Race), race))
        {
            throw new ArgumentOutOfRangeException(nameof(race));
        }
        if (!Enum.IsDefined(typeof(Occupation), occupation))
        {
            throw new ArgumentOutOfRangeException(nameof(occupation));
        }

        Name = name.Trim();
        Race = race;
        Occupation = occupation;
        Weapon = occupationRules.GetWeapon(occupation);
    }

    public void RollInitialStats()
    {
        if (HasRolledStats)
        {
            throw new InvalidOperationException("Initial stats have already been rolled.");
        }

        int baseStength = d20.Roll();
        int RaceStrengthModifier = raceRules.RollStrengthModifier(Race);
        
        Strength = Math.Max(
            1, 
            baseStength + RaceStrengthModifier
        );

        int baseAgility = d20.Roll();
        int AgilityModifier = raceRules.RollAgilityModifier(Race);
        
        Agility = Math.Max(
            1,
            baseAgility + AgilityModifier
        );

        int baseHealth = d20.Roll();
        int StrengthModifier = CalculateStrengthModifier(Strength);

        Health = Math.Max(
            1,
            baseHealth + StrengthModifier
        );

        HasRolledStats = true;
    }

    private static int CalculateStrengthModifier(int strength)
    {
        return (int)Math.Floor((strength - 10)/2.0);
    }

    public string DisplayStats()
    {
        if (!HasRolledStats)
        {
            return "Stats has not been rolled. Type 'roll' first.";
        }

        return
            "===== CHARACTER STATS ===== \n" +
            $"Name: {Name}\n" +
            $"Race: {Race}\n" +
            $"Occupation: {Occupation}\n" +
            $"Strength: {Strength}\n" +
            $"Strength Modifier: {StrengthModifier}\n" +
            $"Agility: {Agility}\n" +
            $"Health: {Health}\n" +
            $"Weapon: {Weapon.asciiArt} {Weapon.type}\n" +
            $"Weapon Max Damage: {Weapon.maxDamage}\n" +
            "============================";
    }
}
