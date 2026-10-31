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

    private const string MessagesFilePath = "game_messages.json";

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

            // 5. Replay Decision
            PromptPlayAgain();
        }

        ConsoleDisplayAndInput.PrintStoryGreen("\nFarewell, brave adventurer.");
    }

    /// <summary>
    /// Prompts for language and loads the dictionary from game_messages.json.
    /// </summary>
    public void SetupLanguage()
    {
        SafeClear();
        ConsoleDisplayAndInput.PrintStoryGreen("==================================================");
        ConsoleDisplayAndInput.PrintStoryGreen("     SELECT LANGUAGE / ELIGE IDIOMA / LANGUE      ");
        ConsoleDisplayAndInput.PrintStoryGreen("==================================================");
        ConsoleDisplayAndInput.PrintStoryGreen("1. English");
        ConsoleDisplayAndInput.PrintStoryGreen("2. Español");
        ConsoleDisplayAndInput.PrintStoryGreen("3. Français");
        Console.Write("\nEnter choice / Elija opción / Choix (1-3): ");

        int choice = ConsoleDisplayAndInput.AskForChoice(1, 3);

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
            ConsoleDisplayAndInput.PrintStoryGreen($"\n[Notice: Language dictionary not loaded ({ex.Message}). Continuing with defaults.]");
        }
    }

    /// <summary>
    /// Displays title and main menu. Returns true to create character; false to exit game.
    /// </summary>
    public bool DisplayWelcomeAndMenu()
    {
        SafeClear();
        ConsoleDisplayAndInput.PrintStoryGreen(Messages.GetMessage("intro") + "\n");
        ConsoleDisplayAndInput.PrintStoryGreen(Messages.GetMessage("choice_create_exit"));
        Console.Write("> ");

        int choice = ConsoleDisplayAndInput.AskForChoice(1, 2);
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
    /// </summary>
    public void SetupPlayer()
    {
        SafeClear();

        // 1. Race Selection ("character_initial")
        Race chosenRace = PromptRace();

        // 2. Name input with validation ("character_name")
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("character_name"));
        Console.Write("> ");
        string name = ConsoleDisplayAndInput.AskForName();

        // 3. Occupation Selection ("character_occupation")
        Occupation chosenOccupation = PromptOccupation();

        // 4. Instantiate Player
        Player = new Player(name, chosenRace, chosenOccupation);

        // 5. Roll stats interactively with prompts from JSON
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("character_strength"));
        Console.Write("> ");
        ConsoleDisplayAndInput.WaitForRoll();

        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("character_agility"));
        Console.Write("> ");
        ConsoleDisplayAndInput.WaitForRoll();

        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("character_health"));
        Console.Write("> ");
        ConsoleDisplayAndInput.WaitForRoll();

        Player.RollInitialStats();

        // 6. Post-roll review & branch ("choice_stats_adventure")
        while (true)
        {
            ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("choice_stats_adventure"));
            Console.Write("> ");
            int postChoice = ConsoleDisplayAndInput.AskForChoice(1, 2);

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

    private Race PromptRace()
    {
        ConsoleDisplayAndInput.PrintStoryGreen(Messages.GetMessage("character_initial"));
        ConsoleDisplayAndInput.PrintStoryGreen("1. Human | 2. Elf | 3. Dwarf | 4. Halfling");
        Console.Write("> ");

        int choice = ConsoleDisplayAndInput.AskForChoice(1, 4);

        return choice switch
        {
            2 => Race.Elf,
            3 => Race.Dwarf,
            4 => Race.Halfling,
            _ => Race.Human
        };
    }

    private Occupation PromptOccupation()
    {
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("character_occupation"));
        ConsoleDisplayAndInput.PrintStoryGreen("1. Fighter | 2. Magician | 3. Thief | 4. Archer");
        Console.Write("> ");

        int choice = ConsoleDisplayAndInput.AskForChoice(1, 4);

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
            ConsoleDisplayAndInput.PrintStoryGreen(Messages.GetMessage("choice_cardinal"));
            Console.Write("> ");

            char pathChoice = ConsoleDisplayAndInput.AskForLetter("nse");

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
                break;
            }
        }

        // Dragon Threat in Red ("dragon_intro")
        string dragonIntroTemplate = Messages.GetMessage("dragon_intro");
        string dragonWarning = dragonIntroTemplate.Replace("{dragonName}", Dragon.name);
        ConsoleDisplayAndInput.PrintDragonRed(dragonWarning);

        // Magic Ring gives dragon's stats ("ring")
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("ring"));
        ConsoleDisplayAndInput.PrintStoryGreen(Dragon.DisplayStats());

        // Encounter Choice: Retreat or Attack ("choice_retreat_attack")
        while (true)
        {
            ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("choice_retreat_attack"));
            Console.Write("> ");

            char combatChoice = ConsoleDisplayAndInput.AskForLetter("ra");

            if (combatChoice == 'r')
            {
                ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage("endgame_retreat"));
                return true;
            }

            if (combatChoice == 'a')
            {
                Combat combat = new Combat(Player, Dragon);
                combat.StartBattle();
                return true;
            }
        }
    }

    /// <summary>
    /// Prompts the player to restart or exit.
    /// </summary>
    public void PromptPlayAgain()
    {
        ConsoleDisplayAndInput.PrintStoryGreen("\nWould you like to embark on another adventure? (Y/N): ");
        Console.Write("> ");

        char choice = ConsoleDisplayAndInput.AskForLetter("yn");

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