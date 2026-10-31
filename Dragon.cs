using System;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

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

    //Returns a defense taunt from the "dragon_taunt" message
    //or returns a default fallback if Messages is not configured.
    
    public string MissedMeTaunt()
    {
        if (Messages == null)
        {
            return "...";
        }

        string message = Messages.GetMessage("dragon_taunt");

        // GetMessage returns a [Missing message ...] placeholder when the name is not found
        // so language=neutral fallback is returned instead

        if (string.IsNullOrWhiteSpace(message) || message.StartsWith("["))
        {
            return "...";
        }

        return message;
    }

    // Overload allowing race substitution if the dictionary string contains {race}.
    
    public string MissedMeTaunt(string race)
    {
        string raw = MissedMeTaunt();
        return string.IsNullOrEmpty(raw) ? raw : raw.Replace("{characterRace}", race);
    }
    //Resolves a localized reaction from the "dragon_defeated_false" message
    // Avoids hardcoded English fallbacks  
    public string WasHitTaunt()
    {
        if (Messages == null)
        {
            return "!";
        }

       string message = Messages.GetMessage("dragon_defeated_false");

       // Language-neutral fallback if message name is not found
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