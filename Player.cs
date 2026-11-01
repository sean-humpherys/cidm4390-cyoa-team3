using System;

public class Player
{
    // ---- Identity (settable per the class diagram, validated on set) ----
    private string name = string.Empty;
    private Race race;
    private Occupation occupation;

    public string Name
    {
        get => name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(
                    "Character name cannot be empty.",
                    nameof(value));
            }
            name = value.Trim();
        }
    }

    public Race Race
    {
        get => race;
        set
        {
            if (!Enum.IsDefined(typeof(Race), value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            race = value;
        }
    }

    // Setting the occupation also sets the matching weapon:
    // Fighter = long sword (12), Magician = lightning bolt spell (12),
    // Thief = dagger (6), Archer = long bow (8).
    public Occupation Occupation
    {
        get => occupation;
        set
        {
            if (!Enum.IsDefined(typeof(Occupation), value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
            occupation = value;
            Weapon = occupationRules.GetWeapon(value);
        }
    }

    // ---- Stats ----
    public int Strength { get; private set; }
    public int Agility { get; private set; }
    public int Health { get; set; }

    // The racial modifier roll is always 1-4.
    public int StrengthModifier { get; private set; }
    public int AgilityModifier { get; private set; }

    // Defaults to the Fighter's weapon because the default Occupation is
    // Fighter (the first enum value). It is replaced whenever Occupation is set.
    public Weapon Weapon { get; private set; } = Weapon.LongSword;

    // Tracks which stats have been rolled so far.
    public bool HasRolledStrength { get; private set; }
    public bool HasRolledAgility { get; private set; }
    public bool HasRolledHealth { get; private set; }
    public bool HasRolledStats =>
        HasRolledStrength && HasRolledAgility && HasRolledHealth;

    // Signed racial adjustment applied to strength.
    // Positive for Human, Elf, and Dwarf; negative for Halfling.
    private int strengthAdjustment;

    // Localized text (Dialogue.json) used for the taunts. Optional.
    public Messages? Messages { get; set; }

    private readonly Die d20 = new Die(20);
    private readonly RaceRules raceRules = new RaceRules();
    private readonly OccupationRules occupationRules =
        new OccupationRules();

    // Parameterless constructor from the class diagram.
    // Set Name, Race and Occupation afterwards before rolling stats.
    public Player()
    {
    }

    public Player(string name, Race race, Occupation occupation, Messages? messages = null)
    {
        Name = name;
        Race = race;
        Occupation = occupation;
        Messages = messages;
    }

    // Step 1: Randomly select 1-20 for strength, then apply the racial
    // modifier (Human, Elf, Dwarf: add 1-4. Halfling: subtract 1-4).
    public void RollStrength()
    {
        if (HasRolledStrength)
        {
            throw new InvalidOperationException(
                "Strength has already been rolled.");
        }

        int baseStrength = d20.Roll();

        strengthAdjustment = raceRules.RollStrengthModifier(Race);
        StrengthModifier = Math.Abs(strengthAdjustment);

        Strength = baseStrength + strengthAdjustment;

        HasRolledStrength = true;
    }

    // Step 2: Randomly select 1-20 for agility, then apply the racial
    // modifier (Elf, Halfling: add 1-4. Human, Dwarf: no modifier).
    public void RollAgility()
    {
        if (HasRolledAgility)
        {
            throw new InvalidOperationException(
                "Agility has already been rolled.");
        }

        int baseAgility = d20.Roll();

        AgilityModifier = raceRules.RollAgilityModifier(Race);

        Agility = baseAgility + AgilityModifier;

        HasRolledAgility = true;
    }

    // Step 3: Randomly select 1-20 for health points. Human, Elf and Dwarf
    // add the prior strength modifier. Halfling adds nothing.
    // Strength must be rolled first.
    public void RollHealth()
    {
        if (!HasRolledStrength)
        {
            throw new InvalidOperationException(
                "Strength must be rolled before health points.");
        }

        if (HasRolledHealth)
        {
            throw new InvalidOperationException(
                "Health points have already been rolled.");
        }

        int baseHealth = d20.Roll();

        int healthBonus = (Race == Race.Halfling) ? 0 : StrengthModifier;

        Health = baseHealth + healthBonus;

        HasRolledHealth = true;
    }

    // Random g-rated taunt ("player_taunt" in Dialogue.json) used when an
    // opponent's attack fails against the player.
    // Returns a language-neutral fallback if Messages is not configured.
    public string MissedMeTaunt()
    {
        if (Messages == null)
        {
            return "...";
        }

        string message = Messages.GetMessage("player_taunt");

        // GetMessage returns a [Missing message ...] placeholder when the name is not found
        if (string.IsNullOrWhiteSpace(message) || message.StartsWith("["))
        {
            return "...";
        }

        return message;
    }

    // Random sarcastic reply ("player_defeated_false" in Dialogue.json) used
    // when the player is hit but still alive.
    // Returns a language-neutral fallback if Messages is not configured.
    public string WasHitTaunt()
    {
        if (Messages == null)
        {
            return "!";
        }

        string message = Messages.GetMessage("player_defeated_false");

        if (string.IsNullOrWhiteSpace(message) || message.StartsWith("["))
        {
            return "!";
        }

        return message;
    }

    // Character summary: exactly the eight items in the appendix.
    public string DisplayStats()
    {
        if (!HasRolledStats)
        {
            return "Stats have not been fully rolled yet.";
        }

        return
            $"Name: {Name}\n" +
            $"Race: {Race}\n" +
            $"Occupation: {Occupation}\n" +
            $"Strength: {Strength}\n" +
            $"Agility: {Agility}\n" +
            $"Health Points: {Health}\n" +
            $"Weapon: {Weapon.type}\n" +
            $"Weapon's Maximum Damage Points: {Weapon.maxDamage}";
    }
}