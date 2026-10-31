// Combat.cs
// No namespace needed: this project uses top-level statements,
// so every .cs file in the root folder can see the other classes.

using System;

public class Combat
{
    // ---- Properties (from the class diagram) ----
    private Player player { get; set; }
    private Dragon dragon { get; set; }

    // Localized text (Dialogue.json). If none is passed in, the dragon's
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

    // =====================================================================
    // PLAYER ATTACKS DRAGON SEQUENCE
    // Runs the full loop: player attacks -> dragon attacks -> retreat/attack
    // prompt. Returns when the dragon is defeated, the player is defeated,
    // or the player retreats.
    // =====================================================================
    public void PlayerAttacksDragonSequence()
    {
        bool keepFighting = true;

        while (keepFighting)
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

                    // Dragon's random taunt ({characterRace} is filled in by Dragon)
                    Console.WriteLine(dragon.MissedMeTaunt(player.Race.ToString()));
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
                        Console.WriteLine(GetText(
                            "dragon_defeated_true",
                            "With the last words of 'Arggg!', you have defeated the dragon."));
                        // Game should transition to the "End Game" activity.
                        return;
                    }

                    // Dragon still alive: random sarcastic reply
                    Console.WriteLine(dragon.WasHitTaunt());
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
                // Game should transition to the "End Game" activity.
                return;
            }

            // ---------- Retreat or keep attacking? ----------
            keepFighting = AskPlayerToContinue();
        }

        // Player retreated
        Console.WriteLine(GetText(
            "player_retreat",
            "You hastily retreat licking your wounds."));
        // Control returns to Game, which displays the main menu.
    }

    // =====================================================================
    // DRAGON ATTACKS PLAYER SEQUENCE
    // Same logic as the player's attack, with the roles reversed.
    // =====================================================================
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
                    Console.WriteLine(GetText(
                        "player_defeated_true",
                        "With the last words of 'Oh no!', you have been defeated by the dragon."));
                    // Game should transition to the "End Game" activity.
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

    // =====================================================================
    // Helper methods
    // =====================================================================

    // Gets a message from Dialogue.json (through Messages) and fills in its
    // {placeholders}. If there is no Messages object, the English fallback
    // text is used instead so Combat still works on its own.
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

    // Die (DieRoll.cs) takes the side count in its constructor and Roll() has
    // no parameters, so create a Die with the sides we need and roll it.
    private int RollDie(int sides)
    {
        Die die = new Die(sides);
        return die.Roll();
    }

    // Displays the ASCII weapon image that matches the player's occupation.
    // Occupation is an enum, so convert it to text before comparing.
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