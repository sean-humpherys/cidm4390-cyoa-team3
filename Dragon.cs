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
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Dragon name cannot be null or empty.", nameof(name));
        }

        this.name = name.Trim();
        this.Messages = messages;
        this.weapon = new Weapon("claws", 12, "##");
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
        if (Messages == null)
        {
            return "...";
        }

        // Shuffle the IDs so attempts remain random while allowing fallbacks
        int[] shuffledIds = _missedTauntIds.OrderBy(_ => Random.Shared.Next()).ToArray();

        foreach (int id in shuffledIds)
        {
            try
            {
                string message = Messages.GetMessage(id);

                if (!string.IsNullOrWhiteSpace(message) && !message.StartsWith("["))
                {
                    return message;
                }
            }
            catch (KeyNotFoundException)
            {
                // Key not found in current language dictionary; retry next available ID
                continue;
            }
            catch (Exception)
            {
                // Any read failure or dictionary issue; attempt next ID
                continue;
            }
        }

        // Language-neutral fallback if all IDs fail or dictionary is empty
        return "...";
    }

    // Overload allowing race substitution if the dictionary string contains {race}.
    
    public string MissedMeTaunt(string race)
    {
        string raw = MissedMeTaunt();
        return string.IsNullOrEmpty(raw) ? raw : raw.Replace("{race}", race);
    }
    //Resolves a localized reaction when the dragon is hit by the player.
    // Retries alternate IDs if a key is missing; avoids hardcoded English fallbacks.  
    public string WasHitTaunt()
    {
        if (Messages == null)
        {
            return "!";
        }

        int[] shuffledIds = _wasHitTauntIds.OrderBy(_ => Random.Shared.Next()).ToArray();

        foreach (int id in shuffledIds)
        {
            try
            {
                string message = Messages.GetMessage(id);

                if (!string.IsNullOrWhiteSpace(message) && !message.StartsWith("["))
                {
                    return message;
                }
            }
            catch (KeyNotFoundException)
            {
                continue;
            }
            catch (Exception)
            {
                continue;
            }
        }

        // Language-neutral fallback if all hit IDs fail
        return "!";
    }

    public string DisplayStats()
    {
        return $"Dragon: {name} | Health: {health} | Strength: {strength} | Agility: {agility} | Weapon: {weapon.type} (Max Dmg: {weapon.maxDamage})";
    }
}