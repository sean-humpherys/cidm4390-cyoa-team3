using System;

public class Game
{
    public Messages Messages { get; set; }
    public Player? Player { get; set; }
    public Dragon? Dragon { get; set; }
    public Die Die { get; set; }
    public bool IsRunning { get; set; }

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
    /// Top-level adventure loop coordinating all lifecycle stages per the Start Game Activity Diagram.
    /// </summary>
    public void Start()
    {
        Console.Title = "Welcome to Choose Your Adventure Game";

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

        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage(19));
    }

    /// <summary>
    /// Prompts for language and loads the dictionary via Messages.
    /// </summary>
    public void SetupLanguage()
    {
        Console.Clear();
        ConsoleDisplayAndInput.PrintStoryGreen("==================================================");
        ConsoleDisplayAndInput.PrintStoryGreen("     SELECT LANGUAGE / ELIGE IDIOMA / LANGUE      ");
        ConsoleDisplayAndInput.PrintStoryGreen("==================================================");
        ConsoleDisplayAndInput.PrintStoryGreen("1. English");
        ConsoleDisplayAndInput.PrintStoryGreen("2. Español");
        ConsoleDisplayAndInput.PrintStoryGreen("3. Français");
        Console.Write("\nEnter choice / Elija opción / Choix (1-3): ");

        int choice = ConsoleDisplayAndInput.AskForChoice(1, 3);

        string filePath = choice switch
    {
        2 => "Spanish.txt",
        3 => "French.txt",
        _ => "English.txt"
    };

        try
        {
            Messages.ReadDictionary(filePath);
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
        Console.Clear();
        ConsoleDisplayAndInput.PrintStoryGreen(Messages.GetMessage(1) + "\n");
        ConsoleDisplayAndInput.PrintStoryGreen(Messages.GetMessage(2));
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
        Console.Clear();
        ConsoleDisplayAndInput.PrintStoryGreen(Messages.GetMessage(4));

        // 1. Name input with validation
        Console.Write(Messages.GetMessage(5));
        string name = ConsoleDisplayAndInput.AskForName();

        // 2. Race Selection
        Race chosenRace = PromptRace();

        // 3. Occupation Selection
        Occupation chosenOccupation = PromptOccupation();

        // 4. Instantiate Player (auto-assigns weapon via OccupationRules)
        Player = new Player(name, chosenRace, chosenOccupation);

        // 5. Roll stats interactively
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage(9));
        Player.RollInitialStats();

        // 6. Review stats before journey
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Player.DisplayStats());
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage(10));
        Console.ReadKey(true);
    }

    private Race PromptRace()
    {
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage(7).Replace("\\n", "\n"));
        Console.Write("> ");

        int choice = ConsoleDisplayAndInput.AskForChoice(1, 4);

        return choice switch
        {
            1 => Race.Elf,
            2 => Race.Human,
            3 => Race.Dwarf,
            4 => Race.Halfling,
            _ => Race.Human
        };
    }

    private Occupation PromptOccupation()
    {
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage(8).Replace("\\n", "\n"));
        Console.Write("> ");

        int choice = ConsoleDisplayAndInput.AskForChoice(1, 4);

        return choice switch
        {
            1 => Occupation.Fighter,
            2 => Occupation.Magician,
            3 => Occupation.Thief,
            4 => Occupation.Archer,
            _ => Occupation.Fighter
        };
    }

    /// <summary>
    /// Executes the Mountain Exploration path, Dragon dialogue, Ring check, and Encounter branch.
    /// </summary>
    private bool ExploreMountains()
    {
        if (Player == null || Dragon == null)
        {
            return false;
        }

        Console.Clear();

        // Path selection loop: North, South, or Exit
        while (true)
        {
            ConsoleDisplayAndInput.PrintStoryGreen(Messages.GetMessage(11));
            Console.Write("> ");

            char pathChoice = ConsoleDisplayAndInput.AskForLetter("nse");

            if (pathChoice == 'e')
            {
                // 'e' -> Exit Game
                return false;
            }

            if (pathChoice == 's')
            {
                // 's' -> South path narrative, loops back
                ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage(12) + "\n");
                continue;
            }

            if (pathChoice == 'n')
            {
                // 'n' -> North path leads directly to the dragon
                ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage(13) + "\n");
                break;
            }
        }

        // Dragon Threat: Red Foreground Text
        ConsoleDisplayAndInput.PrintDragonRed(string.Format(Messages.GetMessage(14), Dragon.name));

        // Magic Ring gives dragon's stats
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage(15));
        ConsoleDisplayAndInput.PrintStoryGreen(Dragon.DisplayStats());

        // Encounter Choice: Retreat or Attack
        while (true)
        {
            ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage(16));
            Console.Write("> ");

            char combatChoice = ConsoleDisplayAndInput.AskForLetter("ra");

            if (combatChoice == 'r')
            {
                // 'r' -> Retreat End Game narrative
                ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage(17));
                return true;
            }

            if (combatChoice == 'a')
            {
                // 'a' -> Hand off to Combat class
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
        ConsoleDisplayAndInput.PrintStoryGreen("\n" + Messages.GetMessage(18));
        Console.Write("> ");

        char choice = ConsoleDisplayAndInput.AskForLetter("yn");

        if (choice != 'y')
        {
            IsRunning = false;
        }
    }
}