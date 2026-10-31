using System;

public class Combat
{
    // ---- Properties (from the class diagram) ----
    private Player player { get; set; }
    private Dragon dragon { get; set; }

    // Both attack sequences use a 20-sided die
    private const int DieSides = 20;

    // ---- Constructor ----
    public Combat(Player player, Dragon dragon)
    {
        this.player = player;
        this.dragon = dragon;
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
            Console.WriteLine($"You prepare yourself to attack the dragon with your weapon. " +
                              $"As a {player.Occupation} you withdraw your {player.Weapon.type}. You attack.");

            DisplayWeaponImage();

            int attackRoll = RollDie(DieSides);

            if (attackRoll <= player.Strength)
            {
                // Player's attack hits
                Console.WriteLine($"You hit {dragon.name}.");

                int defenseRoll = RollDie(DieSides);

                if (defenseRoll <= dragon.agility)
                {
                    // Dragon defends successfully
                    Console.WriteLine($"{dragon.name} successfully defends your attack.");
                    Console.WriteLine(dragon.MissedMeTaunt(player.Race.ToString()));
                }
                else
                {
                    // Dragon fails to defend: apply damage
                    int damage = RollDie(player.Weapon.maxDamage);
                    dragon.health -= damage;

                    Console.WriteLine($"The dragon failed to defend your attack. " +
                                      $"Your weapon delivers {damage} damage points. " +
                                      $"The dragon's health points is now {dragon.health}.");

                    if (dragon.health <= 0)
                    {
                        Console.WriteLine("With the last words of 'Arggg!', you have defeated the dragon.");
                        // Game should transition to the "End Game" activity.
                        return;
                    }

                    // Dragon still alive: sarcastic reply
                    Console.WriteLine(dragon.WasHitTaunt());
                }
            }
            else
            {
                // Player's attack misses
                Console.WriteLine($"You missed {dragon.name}.");
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
        Console.WriteLine("You hastily retreat licking your wounds.");
        // Control returns to Game, which displays the main menu.
    }

    // =====================================================================
    // DRAGON ATTACKS PLAYER SEQUENCE
    // Same logic as the player's attack, with the roles reversed.
    // =====================================================================
    public void DragonAttacksPlayerSequence()
    {
        Console.WriteLine();
        Console.WriteLine("It is the dragon's turn to attack.");

        int attackRoll = RollDie(DieSides);

        if (attackRoll <= dragon.strength)
        {
            // Dragon's attack hits
            Console.WriteLine($"{dragon.name} hits you.");

            int defenseRoll = RollDie(DieSides);

            if (defenseRoll <= player.Agility)
            {
                // Player defends successfully
                Console.WriteLine("You successfully defend the attack.");
                Console.WriteLine(player.MissedMeTaunt());
            }
            else
            {
                // Player fails to defend: apply damage
                int damage = RollDie(dragon.weapon.maxDamage);
                player.Health -= damage;

                Console.WriteLine($"The dragon's attack delivers {damage} damage points. " +
                                  $"Your health points is now {player.Health}.");

                if (player.Health <= 0)
                {
                    Console.WriteLine("With the last words of 'Oh no!', you have been defeated by the dragon.");
                    // Game should transition to the "End Game" activity.
                    return;
                }

                // Player still alive: sarcastic reply
                Console.WriteLine(player.WasHitTaunt());
            }
        }
        else
        {
            // Dragon's attack misses
            Console.WriteLine("The dragon missed you.");
        }
    }

    // =====================================================================
    // Helper methods
    // =====================================================================

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
    private bool AskPlayerToContinue()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Enter 'r' to retreat or 'a' to attack.");
            string? choice = Console.ReadLine()?.Trim().ToLower();

            if (choice == "a") return true;
            if (choice == "r") return false;

            Console.WriteLine("Invalid choice. Please try again.");
        }
    }
}