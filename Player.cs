using System;

public class Player
{
    public string Name { get; private set; }
    public Race Race { get; private set; }
    public Occupation Occupation { get; private set; }

    public int Strength { get; private set; }
    public int Agility { get; private set; }
    public int Health { get; private set; }

    // The racial modifier roll is always 1-4 after stats are rolled.
    public int StrengthModifier { get; private set; }
    public int AgilityModifier { get; private set; }

    public Weapon Weapon { get; private set; }
    public bool HasRolledStats { get; private set; }

    // Stores the signed racial adjustment applied to the stats.
    // Positive for Human, Elf, and Dwarf; negative for Halfling.
    private int strengthAdjustment;

    private readonly Die d20 = new Die(20);
    private readonly RaceRules raceRules = new RaceRules();
    private readonly OccupationRules occupationRules =
        new OccupationRules();

    public Player(string name, Race race, Occupation occupation)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Character name cannot be empty.",
                nameof(name));
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
            throw new InvalidOperationException(
                "Initial stats have already been rolled.");
        }

        // Roll strength and apply the racial adjustment.
        int baseStrength = d20.Roll();

        strengthAdjustment =
            raceRules.RollStrengthModifier(Race);

        // Store the modifier roll as a positive value from 1 to 4.
        StrengthModifier = Math.Abs(strengthAdjustment);

        Strength = Math.Max(
            1,
            baseStrength + strengthAdjustment
        );

        // Roll agility and apply the racial adjustment.
        int baseAgility = d20.Roll();

        AgilityModifier =
            raceRules.RollAgilityModifier(Race);

        Agility = Math.Max(
            1,
            baseAgility + AgilityModifier
        );

        // Roll health and reuse the strength racial adjustment.
        int baseHealth = d20.Roll();

        Health = Math.Max(
            1,
            baseHealth + strengthAdjustment
        );

        HasRolledStats = true;
    }

    public string DisplayStats()
    {
        if (!HasRolledStats)
        {
            return "Stats have not been rolled. Type 'roll' first.";
        }

        return
            "===== CHARACTER STATS =====\n" +
            $"Name: {Name}\n" +
            $"Race: {Race}\n" +
            $"Occupation: {Occupation}\n" +
            $"Strength: {Strength}\n" +
            $"Strength Modifier (1-4 roll): {StrengthModifier}\n" +
            $"Strength Adjustment Applied: {strengthAdjustment}\n" +
            $"Agility: {Agility}\n" +
            $"Agility Modifier: {AgilityModifier}\n" +
            $"Health: {Health}\n" +
            $"Weapon: {Weapon.asciiArt} {Weapon.type}\n" +
            $"Weapon Max Damage: {Weapon.maxDamage}\n" +
            "============================";
    }
}