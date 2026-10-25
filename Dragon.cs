using System;

public class Dragon
{
    // Properties strictly matching UML Class Diagram
    public string name { get; set; }
    public int strength { get; set; }
    public int health { get; set; }
    public int agility { get; set; }
    public Weapon weapon { get; set; }

    // Reference to the localization messages dictionary
    public Messages? Messages { get; set; }

    // Dictionary IDs representing defense taunts in language files
    // Example: 101-104 correspond to the 4 defensive lines in the dictionary
    private readonly int[] _missedTauntIds = new int[] { 101, 102, 103, 104 };

    // Dictionary IDs representing hit reaction lines in language files
    // Example: 201-204 correspond to the 4 hit reaction lines in the dictionary
    private readonly int[] _wasHitTauntIds = new int[] { 201, 202, 203, 204 };

    
    // Initializes Dragon with default weapon ("claws", 12, "##") and optional Messages service.
    public Dragon(string name, Messages? messages = null)
    {
        this.name = name;
        this.weapon = new Weapon("claws", 12, "##");
        this.Messages = messages;
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

    //Returns a defense taunt by delegating to Messages.GetMessage(),
    //or returns a default fallback if Messages is not configured.
    
    public string MissedMeTaunt()
    {
        if (Messages != null)
        {
            int selectedId = _missedTauntIds[Random.Shared.Next(_missedTauntIds.Length)];
            return Messages.GetMessage(selectedId);
        }

        return "Missed me, breath of a gnat!";
    }

    // Overload allowing race substitution if the dictionary string contains {race}.
    
    public string MissedMeTaunt(string race)
    {
        string raw = MissedMeTaunt();
        return raw.Replace("{race}", race);
    }

    // Returns a hit reaction taunt by delegating to Messages.GetMessage(),
    // or returns a default fallback if Messages is not configured.
    
    public string WasHitTaunt()
    {
        if (Messages != null)
        {
            int selectedId = _wasHitTauntIds[Random.Shared.Next(_wasHitTauntIds.Length)];
            return Messages.GetMessage(selectedId);
        }

        return "Ow! That actually stung a bit!";
    }

    public string DisplayStats()
    {
        return $"Dragon: {name} | Health: {health} | Strength: {strength} | Agility: {agility} | Weapon: {weapon.type} (Max Dmg: {weapon.maxDamage})";
    }
}