using System;

public class Dragon
{
    private static readonly string[] DragonNames = new string[]
    {
        "Ignis",
        "Vermithrax",
        "Smaug",
        "Fafnir",
        "Balerion",
        "Pyre",
        "Cinder"
    };

    // Properties strictly matching UML Class Diagram
    public string name { get; set; }
    public int strength { get; set; }
    public int health { get; set; }
    public int agility { get; set; }
    public Weapon weapon { get; set; }

    // Reference to the localization messages dictionary
    public Messages? Messages { get; set; }

    /// <summary>
    /// Constructor that automatically picks a random name from DragonNames
    /// and chains to the main constructor to ensure weapon initialization.
    /// </summary>
    public Dragon(Messages? messages = null)
        : this(GetRandomName(), messages)
    {
    }

    /// <summary>
    /// Authoritative constructor initializing Dragon name, weapon, and Messages service.
    /// </summary>
    public Dragon(string name, Messages? messages = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Dragon name cannot be null or empty.", nameof(name));
        }

        this.name = name.Trim();
        this.Messages = messages;
        this.weapon = new Weapon("claws", 12, "##");
    }

    private static string GetRandomName()
    {
        Random random = new Random();
        return DragonNames[random.Next(DragonNames.Length)];
    }

    public void InitializeStats(Die die)
    {
        ArgumentNullException.ThrowIfNull(die);

        strength = die.Roll();
        agility = die.Roll();
        health = die.Roll();
    }

    public void InitializeStats(int str, int agi, int hp)
    {
        strength = str;
        agility = agi;
        health = hp;
    }

    // Returns a defense taunt from the "dragon_taunt" message
    // or returns a default fallback if Messages is not configured.
    public string MissedMeTaunt()
    {
        if (Messages == null)
        {
            return "...";
        }

        string message = Messages.GetMessage("dragon_taunt");

        // GetMessage returns a [Missing message ...] placeholder when the name is not found
        if (string.IsNullOrWhiteSpace(message) || message.StartsWith("["))
        {
            return "...";
        }

        return message;
    }

    // Overload allowing race substitution if the dictionary string contains {characterRace}.
    public string MissedMeTaunt(string race)
    {
        string raw = MissedMeTaunt();
        return string.IsNullOrEmpty(raw) ? raw : raw.Replace("{characterRace}", race);
    }

    // Resolves a localized reaction from the "dragon_defeated_false" message
    public string WasHitTaunt()
    {
        if (Messages == null)
        {
            return "!";
        }

        string message = Messages.GetMessage("dragon_defeated_false");

        if (string.IsNullOrWhiteSpace(message) || message.StartsWith("["))
        {
            return "!";
        }

        return message;
    }

    public string DisplayStats()
    {
        return $"Dragon: {name} | Health: {health} | Strength: {strength} | Agility: {agility} | Weapon: {weapon.type} (Max Dmg: {weapon.maxDamage})";
    }
}