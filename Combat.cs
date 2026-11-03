using System;

// How a battle ended. Game.cs uses this to decide what happens next.
public enum CombatResult
{
    DragonDefeated,   // -> End Game (victory)
    PlayerDefeated,   // -> End Game (defeat)
    PlayerRetreated   // -> back to the mountain exploration loop / main menu
}

public class Combat
{
    // ---- Properties (from the class diagram) ----
    private Player player { get; set; }
    private Dragon dragon { get; set; }

    // Localized text (Dialogue.json).
    // Messages object is used. If neither exists, English fallback text is shown.
    private Messages? messages { get; set; }

    // Both attack sequences use a 20-sided die
    private const int DieSides = 20;

    // ---- Constructor ----
    public Combat(Player player, Dragon dragon, Messages? messages = null)
    {
        this.player = player;
        this.dragon = dragon;
        this.messages = messages ?? dragon.Messages;
    }

    // BATTLE
    public CombatResult Battle()
    {
        return PlayerAttacksDragonSequence();
    }

    // PLAYER ATTACKS DRAGON SEQUENCE
    // Runs the full loop: player attacks -> dragon attacks -> retreat/attack
    // prompt. Returns a CombatResult when the dragon is defeated, the player
    // is defeated, or the player retreats.
    public CombatResult PlayerAttacksDragonSequence()
    {
        while (true)
        {
            // ---------- Player's turn ----------
            Console.WriteLine();
            Console.WriteLine(GetText(
                "attack_intro",
                "You prepare yourself to attack the dragon with your weapon. " +
                "As a {characterOccupation} you withdraw your {characterWeapon}. You attack.",
                ("{characterOccupation}", player.Occupation.ToString()),
                ("{characterWeapon}", player.Weapon.type)));

            DisplayWeaponImage();

            int attackRoll = RollDie(DieSides);

            if (attackRoll <= player.Strength)
            {
                // Player's attack hits
                Console.WriteLine(GetText(
                    "attack_hit",
                    "You hit {dragonName}.",
                    ("{dragonName}", dragon.name)));

                int defenseRoll = RollDie(DieSides);

                if (defenseRoll <= dragon.agility)
                {
                    // Dragon defends successfully
                    Console.WriteLine(GetText(
                        "dragon_defend_success",
                        "{dragonName} successfully defends your attack.",
                        ("{dragonName}", dragon.name)));

                    // Dragon taunt is rendered in red ({characterRace} is filled in by Dragon)
                    ConsoleDisplayAndInput.PrintDragonRed(
                        dragon.MissedMeTaunt(player.Race.ToString()));
                }
                else
                {
                    // Dragon fails to defend: apply damage
                    int damage = RollDie(player.Weapon.maxDamage);
                    dragon.health -= damage;

                    Console.WriteLine(GetText(
                        "dragon_defend_fail",
                        "The dragon failed to defend your attack. " +
                        "Your weapon delivers {combatDamage} damage points. " +
                        "The dragon's health points is now {combatDragonHealth}.",
                        ("{combatDamage}", damage.ToString()),
                        ("{combatDragonHealth}", dragon.health.ToString())));

                    if (dragon.health <= 0)
                    {
                        // Final exclamation. Game prints the ending.
                        Console.WriteLine(GetText(
                            "dragon_defeated_true",
                            "With the last words of 'Arggg!', you have defeated the dragon."));


                        return CombatResult.DragonDefeated;
                    }

                    // Dragon still alive: random sarcastic reply, in red
                    ConsoleDisplayAndInput.PrintDragonRed(dragon.WasHitTaunt());
                }
            }
            else
            {
                // Player's attack misses
                Console.WriteLine(GetText(
                    "attack_miss",
                    "You missed {dragonName}.",
                    ("{dragonName}", dragon.name)));
            }

            // ---------- Dragon's turn ----------
            DragonAttacksPlayerSequence();

            if (player.Health <= 0)
            {
                return CombatResult.PlayerDefeated;
            }

            // ---------- Retreat or keep attacking? ----------
            if (!AskPlayerToContinue())
            {
                // Player retreated: no changes to player health or stats.
                ConsoleDisplayAndInput.PrintStoryGreen(GetText(
                    "player_retreat",
                    "You hastily retreat licking your wounds."));

                return CombatResult.PlayerRetreated;
            }
        }
    }

    // DRAGON ATTACKS PLAYER SEQUENCE
    // Same logic as the player's attack, with the roles reversed.
    public void DragonAttacksPlayerSequence()
    {
        Console.WriteLine();

        int attackRoll = RollDie(DieSides);

        if (attackRoll <= dragon.strength)
        {
            // Dragon's attack hits
            Console.WriteLine(GetText(
                "dragon_hit_success",
                "{dragonName} hits you.",
                ("{dragonName}", dragon.name)));

            int defenseRoll = RollDie(DieSides);

            if (defenseRoll <= player.Agility)
            {
                // Player defends successfully
                Console.WriteLine(GetText(
                    "player_defend_true",
                    "You successfully defend the attack."));

                // Player's random taunt
                Console.WriteLine(player.MissedMeTaunt());
            }
            else
            {
                // Player fails to defend: apply damage
                int damage = RollDie(dragon.weapon.maxDamage);
                player.Health -= damage;

                Console.WriteLine(GetText(
                    "player_defend_false",
                    "The dragon's attack delivers {combatDamage} damage points. " +
                    "Your health points is now {combatPlayerHealth}.",
                    ("{combatDamage}", damage.ToString()),
                    ("{combatPlayerHealth}", player.Health.ToString())));

                if (player.Health <= 0)
                {
                    // Final exclamation. Game prints the ending.
                    Console.WriteLine(GetText(
                        "player_defeated_true",
                        "With the last words of 'Oh no!', you have been defeated by the dragon."));

                    return;
                }

                // Player still alive: random sarcastic reply
                Console.WriteLine(player.WasHitTaunt());
            }
        }
        else
        {
            // Dragon's attack misses
            Console.WriteLine(GetText(
                "dragon_hit_fail",
                "The dragon missed you."));
        }
    }

    // Helper methods
    // Gets a message from Dialogue.json (through Messages)
    private string GetText(
        string key,
        string englishFallback,
        params (string placeholder, string value)[] values)
    {
        string text = messages != null
            ? messages.GetMessage(key)
            : englishFallback;

        foreach ((string placeholder, string value) in values)
        {
            text = text.Replace(placeholder, value);
        }

        return text;
    }

    private int RollDie(int sides)
    {
        Die die = new Die(sides);
        return die.Roll();
    }

    private void DisplayWeaponImage()
    {
        switch (player.Occupation.ToString().ToLower())
        {
            case "fighter":
                Console.WriteLine("-)=====>");
                break;
            case "magician":
                Console.WriteLine("zap~~~~~~");
                break;
            case "thief":
                Console.WriteLine("-)==>");
                break;
            case "archer":
                Console.WriteLine("}     -->");
                break;
        }
    }

    // Prompts for 'r' (retreat) or 'a' (attack). Returns true to keep attacking.
    // Any other input shows the prompt again. If the input ends (null),
    // it counts as a retreat so the game can never get stuck in this loop.
    private bool AskPlayerToContinue()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine(GetText(
                "choice_retreat_attack",
                "Do you wish to retreat or attack? Enter 'r' for retreat. Enter 'a' for attack."));

            string? choice = Console.ReadLine();

            if (choice == null) return false;

            choice = choice.Trim().ToLower();

            if (choice == "a") return true;
            if (choice == "r") return false;
        }
    }
}