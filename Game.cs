using System;
using System.Collections.Generic;
using System.IO;

public class Game
{
    public Messages Messages { get; set; }
    public Player? Player { get; set; }
    public Dragon? Dragon { get; set; }
    public Die Die { get; set; }
    public bool IsRunning { get; set; }

    private static readonly string MessagesFilePath = Path.Combine(
        AppContext.BaseDirectory,
        "Dialogue.json"
    );

    /// <summary>
    /// Default constructor initializing core dependencies.
    /// </summary>
    public Game()
    {
        Messages = new Messages();
        Die = new Die(20);
        IsRunning = true;
    }

    /// <summary>
    /// Overloaded constructor to support dependency injection.
    /// </summary>
    public Game(Messages messages, Die die)
    {
        Messages = messages ?? throw new ArgumentNullException(nameof(messages));
        Die = die ?? throw new ArgumentNullException(nameof(die));
        IsRunning = true;
    }

    /// <summary>
    /// Top-level adventure loop coordinating all lifecycle stages.
    /// </summary>
    public void Start()
    {
        try
        {
            Console.Title = "Welcome to Choose Your Adventure Game";
        }
        catch (Exception)
        {
            // Suppress console title errors in non-interactive/redirected environments
        }

        while (IsRunning)
        {
            SetupLanguage();

            // 1. Instantiate Dragon passing Messages service and initialize stats via Die
            SetupDragon();

            // 2. Display Title & Main Menu
            bool createCharacter = DisplayWelcomeAndMenu();
            if (!createCharacter)
            {
                IsRunning = false;
                break;
            }

            // 3. Branch: Create Character Workflow
            SetupPlayer();

            // 4. Branch: Mountain Exploration & Dragon Encounter Flow
            bool continueAdventure = ExploreMountains();
            if (!continueAdventure)
            {
                IsRunning = false;
                break;
            }

            // 5. Replay Decision (only reached when a run concludes)
            PromptPlayAgain();
        }

        ConsoleDisplayAndInput.PrintStoryGreen("\nFarewell, brave adventurer.");
    }

    /// <summary>
    /// Prompts for language and loads the dictionary from Dialogue.json.
    /// </summary>
    public void SetupLanguage()
    {
        SafeClear();
        ConsoleDisplayAndInput.PrintStoryGreen("==================================================");
        ConsoleDisplayAndInput.PrintStoryGreen("      SELECT LANGUAGE / ELIGE IDIOMA / LANGUE     ");
        ConsoleDisplayAndInput.PrintStoryGreen("==================================================");
        
        string langPrompt = "1. English\n2. Español\n3. Français\n\nEnter choice / Elija opción / Choix (1-3): ";
        Console.Write(langPrompt);

        int choice = ConsoleDisplayAndInput.AskForChoice(
            1, 
            3, 
            "Invalid choice. / Opción no válida. / Choix invalide.", 
            langPrompt
        );

        string languageCode = choice switch
        {
            2 => "es",
            3 => "fr",
            _ => "en"
        };

        try
        {
            Messages.ReadDictionary(MessagesFilePath, languageCode);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[Failed to load {MessagesFilePath}]: {ex.Message}");
            Console.ResetColor();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Displays title and main menu. Passes invalid_choice and choice_create_exit for re-prompting.
    /// </summary>
    public bool DisplayWelcomeAndMenu()
    {
        SafeClear();
        ConsoleDisplayAndInput.PrintStoryGreen(Messages.GetMessage("intro") + "\n");

        string menuPrompt = Messages.GetMessage("choice_create_exit");
        ConsoleDisplayAndInput.PrintStoryGreen(menuPrompt);
        Console.Write("> ");

        int choice = ConsoleDisplayAndInput.AskForChoice(
            1, 
            2, 
            Messages.GetMessage("invalid_choice"), 
            menuPrompt
        );

        return choice == 1;
    }

    /// <summary>
    /// Instantiates Dragon with default weapon ("claws", 12) and rolls initial stats with a D20.
    /// </summary>
    public void SetupDragon()
    {
        Dragon = new Dragon("Dragon", Messages);
        Dragon.InitializeStats(Die);
    }

    /// <summary>
    /// Coordinates character creation: Name, Race, Occupation, Stat Rolls, and Review.
    /// Passes localized error messages and questions into each console input call.
    /// </summary>
    public void SetupPlayer()
    {
        SafeClear();

        // 1. Race Selection ("character_initial")
        Race chosenRace = PromptRace();

        // 2. Name input with validation ("character_name")
        string namePrompt = Messages.GetMessage("character_name");
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + namePrompt);
        Console.Write("> ");
        string name = ConsoleDisplayAndInput.AskForName(
            Messages.GetMessage("invalid_name"), 
            namePrompt
        );

        // 3. Occupation Selection ("character_occupation")
        Occupation chosenOccupation = PromptOccupation();

        // 4. Instantiate Player
        Player = new Player(name, chosenRace, chosenOccupation, Messages);

        // 5. Roll stats interactively with validation & re-prompting
        string strPrompt = Messages.GetMessage("character_strength");
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + strPrompt);
        Console.Write("> ");
        ConsoleDisplayAndInput.WaitForRoll(
            Messages.GetMessage("invalid_roll"), 
            strPrompt
        );
        Player.RollStrength();
        Console.WriteLine($"Strength rolled: {Player.Strength} (Modifier: {Player.StrengthModifier})");

        string agiPrompt = Messages.GetMessage("character_agility");
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + agiPrompt);
        Console.Write("> ");
        ConsoleDisplayAndInput.WaitForRoll(
            Messages.GetMessage("invalid_roll"), 
            agiPrompt
        );
        Player.RollAgility();
        Console.WriteLine($"Agility rolled: {Player.Agility} (Modifier: {Player.AgilityModifier})");

        string hpPrompt = Messages.GetMessage("character_health");
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + hpPrompt);
        Console.Write("> ");
        ConsoleDisplayAndInput.WaitForRoll(
            Messages.GetMessage("invalid_roll"), 
            hpPrompt
        );
        Player.RollHealth();
        Console.WriteLine($"Health Points rolled: {Player.Health}");

        // 6. Post-roll review & branch ("choice_stats_adventure")
        string postPrompt = Messages.GetMessage("choice_stats_adventure");
        while (true)
        {
            ConsoleDisplayAndInput.PrintStoryGreen("\n" + postPrompt);
            Console.Write("> ");

            int postChoice = ConsoleDisplayAndInput.AskForChoice(
                1, 
                2, 
                Messages.GetMessage("invalid_choice"), 
                postPrompt
            );

            if (postChoice == 1)
            {
                ConsoleDisplayAndInput.PrintStoryGreen("\n" + Player.DisplayStats());
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// Prompts for race selection and reprompts with options list on invalid input.
    /// </summary>
    private Race PromptRace()
    {
        string racePrompt = Messages.GetMessage("character_initial") + "\n1. Human | 2. Elf | 3. Dwarf | 4. Halfling";
        ConsoleDisplayAndInput.PrintStoryGreen(racePrompt);
        Console.Write("> ");

        int choice = ConsoleDisplayAndInput.AskForChoice(
            1, 
            4, 
            Messages.GetMessage("invalid_choice"), 
            racePrompt
        );

        return choice switch
        {
            2 => Race.Elf,
            3 => Race.Dwarf,
            4 => Race.Halfling,
            _ => Race.Human
        };
    }

    /// <summary>
    /// Prompts for occupation selection and reprompts with options list on invalid input.
    /// </summary>
    private Occupation PromptOccupation()
    {
        string occPrompt = Messages.GetMessage("character_occupation") + "\n1. Fighter | 2. Magician | 3. Thief | 4. Archer";
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + occPrompt);
        Console.Write("> ");

        int choice = ConsoleDisplayAndInput.AskForChoice(
            1, 
            4, 
            Messages.GetMessage("invalid_choice"), 
            occPrompt
        );

        return choice switch
        {
            2 => Occupation.Magician,
            3 => Occupation.Thief,
            4 => Occupation.Archer,
            _ => Occupation.Fighter
        };
    }

    /// <summary>
    /// Executes Mountain Exploration path, Dragon dialogue, Ring check, and Encounter branch.
    /// Preserves player state when retreating mid-combat and reprompts on invalid letters.
    /// </summary>
    private bool ExploreMountains()
    {
        if (Player == null || Dragon == null)
        {
            return false;
        }

        SafeClear();

        // Path selection loop: North, South, or Exit
        while (true)
        {
            string pathPrompt = Messages.GetMessage("choice_cardinal");
            ConsoleDisplayAndInput.PrintStoryGreen(pathPrompt);
            Console.Write("> ");

            char pathChoice = ConsoleDisplayAndInput.AskForLetter(
                "nse", 
                Messages.GetMessage("invalid_letter"), 
                pathPrompt
            );

            if (pathChoice == 'e')
            {
                return false;
            }

            if (pathChoice == 's')
            {
                ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("choice_cardinal_south") + "\n");
                continue;
            }

            if (pathChoice == 'n')
            {
                ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("choice_cardinal_north") + "\n");

                // Dragon Threat in Red ("dragon_intro")
                string dragonIntroTemplate = Messages.GetMessage("dragon_intro");
                string dragonWarning = dragonIntroTemplate.Replace("{dragonName}", Dragon.name);
                ConsoleDisplayAndInput.PrintDragonRed(dragonWarning);

                // Magic Ring gives dragon's stats ("ring")
                ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("ring"));
                ConsoleDisplayAndInput.PrintStoryGreen(Dragon.DisplayStats());

                // Pre-Combat Choice: Retreat or Attack
                bool backToPaths = false;
                string combatPrompt = Messages.GetMessage("choice_retreat_attack");

                // Encounter Choice: Retreat or Attack ("choice_retreat_attack")
                while (true)
                {
                    ConsoleDisplayAndInput.PrintStoryGreen("\n" + combatPrompt);
                    Console.Write("> ");

                    char combatChoice = ConsoleDisplayAndInput.AskForLetter(
                        "ra", 
                        Messages.GetMessage("invalid_letter"), 
                        combatPrompt
                    );

                    if (combatChoice == 'r')
                    {
                        // Pre-fight retreat ends this run cleanly
                        DisplayEndGame("retreat");
                        return true; // Signals run ended cleanly -> advances to PromptPlayAgain()
                    }

                    if (combatChoice == 'a')
                    {
                        Combat combat = new Combat(Player, Dragon, Messages);
                        combat.PlayerAttacksDragonSequence();

                        // Check post-combat health states:
                        if (Player.Health > 0 && Dragon.health > 0)
                        {
                            // Both alive -> Player chose 'r' mid-combat.
                            // Break out of encounter loop and return to the path fork.
                            backToPaths = true;
                            break;
                        }

                        if (Dragon.health <= 0)
                        {
                            DisplayEndGame("dragon_defeated");
                            return true;
                        }

                        if (Player.Health <= 0)
                        {
                            DisplayEndGame("player_defeated");
                            return true;
                        }
                    }
                }

                if (backToPaths)
                {
                    continue; // Loops back to choice_cardinal while keeping current Player instance and health
                }
            }
        }
    }

    /// <summary>
    /// Displays the localized end game condition per the End Game Activity Diagram.
    /// </summary>
    public void DisplayEndGame(string condition)
    {
        Console.WriteLine();
        switch (condition)
        {
            case "dragon_defeated":
                ConsoleDisplayAndInput.PrintStoryGreen(Messages.GetMessage("endgame_dragon_defeated"));
                break;

            case "player_defeated":
                ConsoleDisplayAndInput.PrintDragonRed(Messages.GetMessage("endgame_player_defeated"));
                break;

            case "retreat":
            default:
                ConsoleDisplayAndInput.PrintStoryGreen(Messages.GetMessage("endgame_retreat"));
                break;
        }
    }

    /// <summary>
    /// Prompts the player to restart or exit. Passes invalid_letter for validation.
    /// </summary>
    public void PromptPlayAgain()
    {
        string playAgainPrompt = "Would you like to embark on another adventure? (Y/N): ";
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + playAgainPrompt);
        Console.Write("> ");

        char choice = ConsoleDisplayAndInput.AskForLetter(
            "yn", 
            Messages.GetMessage("invalid_letter"), 
            playAgainPrompt
        );

        if (choice != 'y')
        {
            IsRunning = false;
        }
    }

    /// <summary>
    /// Safely clears the console without throwing an IOException when executed in redirected/test environments.
    /// </summary>
    private static void SafeClear()
    {
        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
            // Ignored during automated tests where stdout has no buffer handle
        }
    }
}