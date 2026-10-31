using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GameTest
{
    private TextReader _originalIn = null!;
    private TextWriter _originalOut = null!;
    private const string MessagesPath = "game_messages.json";

    private const string SampleJson = @"{
      ""intro"": {
        ""en"": ""Welcome to Choose Your Adventure Game"",
        ""es"": ""Bienvenido al juego «Elige tu aventura»"",
        ""fr"": ""Bienvenue dans votre jeu d'aventure""
      },
      ""choice_create_exit"": {
        ""en"": ""Choose; 1. Create Character or 2. Exit. Type '1' or '2'"",
        ""es"": ""Elige: 1. Crear personaje o 2. Salir. Escribe '1' o '2'"",
        ""fr"": ""Choisissez : 1. Créer un personnage ou 2. Quitter. Tapez '1' ou '2'""
      },
      ""character_initial"": {
        ""en"": ""What race of character do you want to be? Human, Elf, Dwarf or Halfling?"",
        ""es"": ""¿Qué raza de personaje quieres ser? ¿Humano, Elfo, Enano o Mediano?"",
        ""fr"": ""Quelle race de personnage voulez-vous être ? Humain, Elfe, Nain ou Halfelin ?""
      },
      ""character_name"": {
        ""en"": ""What is the name of your character?"",
        ""es"": ""¿Cuál es el nombre de tu personaje?"",
        ""fr"": ""Quel est le nom de votre personnage ?""
      },
      ""character_occupation"": {
        ""en"": ""What is the occupation of your character? Fighter, magician, thief, or archer?"",
        ""es"": ""¿Cuál es la ocupación de tu personaje? ¿Guerrero, mago, ladrón o arquero?"",
        ""fr"": ""Quelle est la profession de votre personnage ? Combattant, magicien, voleur ou archer ?""
      },
      ""character_strength"": {
        ""en"": ""Next, we are going to roll for strength. Type 'roll'"",
        ""es"": ""A continuación, vamos a tirar los dados para la fuerza. Escribe 'roll'"",
        ""fr"": ""Ensuite, nous allons lancer les dés pour la force. Tapez 'roll'""
      },
      ""character_agility"": {
        ""en"": ""Next, we will roll for agility. Type 'roll'"",
        ""es"": ""A continuación, vamos a tirar los dados para la agilidad. Escribe 'roll'"",
        ""fr"": ""Ensuite, nous allons lancer les dés pour l'agilité. Tapez 'roll'""
      },
      ""character_health"": {
        ""en"": ""Next, we will roll for health points. Type 'roll'"",
        ""es"": ""A continuación, vamos a tirar los dados para los puntos de salud. Escribe 'roll'"",
        ""fr"": ""Ensuite, nous allons lancer les dés pour les points de vie. Tapez 'roll'""
      },
      ""choice_stats_adventure"": {
        ""en"": ""What would you like to do next? 1. List character stats. 2. Proceed on an adventure. Type '1' or '2'"",
        ""es"": ""¿Qué te gustaría hacer a continuación? 1. Listar las estadísticas del personaje. 2. Proceder a una aventura. Escribe '1' o '2'"",
        ""fr"": ""Que voulez-vous faire ensuite ? 1. Lister les statistiques du personnage. 2. Partir à l'aventure. Tapez '1' ou '2'""
      }
    }";

    [TestInitialize]
    public void Setup()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
        File.WriteAllText(MessagesPath, SampleJson);
    }

    [TestCleanup]
    public void Cleanup()
    {
        Console.SetIn(_originalIn);
        Console.SetOut(_originalOut);

        if (File.Exists(MessagesPath))
        {
            File.Delete(MessagesPath);
        }
    }

    [TestMethod]
    public void DefaultConstructor_InitializesDependenciesAndState()
    {
        Game game = new Game();

        Assert.IsNotNull(game.Messages);
        Assert.IsNotNull(game.Die);
        Assert.AreEqual(20, game.Die.Sides);
        Assert.IsTrue(game.IsRunning);
        Assert.IsNull(game.Player);
        Assert.IsNull(game.Dragon);
    }

    [TestMethod]
    public void OverloadedConstructor_ValidParameters_AssignsProperties()
    {
        Messages customMessages = new Messages();
        Die customDie = new Die(10);

        Game game = new Game(customMessages, customDie);

        Assert.AreSame(customMessages, game.Messages);
        Assert.AreSame(customDie, game.Die);
        Assert.AreEqual(10, game.Die.Sides);
        Assert.IsTrue(game.IsRunning);
    }

    [TestMethod]
    public void OverloadedConstructor_NullMessages_ThrowsArgumentNullException()
    {
        Die die = new Die(20);

        try
        {
            new Game(null!, die);
            Assert.Fail("Expected ArgumentNullException was not thrown for null Messages.");
        }
        catch (ArgumentNullException ex)
        {
            Assert.AreEqual("messages", ex.ParamName);
        }
    }

    [TestMethod]
    public void OverloadedConstructor_NullDie_ThrowsArgumentNullException()
    {
        Messages messages = new Messages();

        try
        {
            new Game(messages, null!);
            Assert.Fail("Expected ArgumentNullException was not thrown for null Die.");
        }
        catch (ArgumentNullException ex)
        {
            Assert.AreEqual("die", ex.ParamName);
        }
    }

    [TestMethod]
    public void SetupLanguage_SelectEnglish_LoadsDictionary()
    {
        Game game = new Game();

        using StringReader reader = new StringReader("1\n");
        using StringWriter writer = new StringWriter();
        Console.SetIn(reader);
        Console.SetOut(writer);

        game.SetupLanguage();

        Assert.AreEqual("Welcome to Choose Your Adventure Game", game.Messages.GetMessage("intro"));
    }

    [TestMethod]
    public void SetupDragon_InstantiatesDragonWithRolledStats()
    {
        Game game = new Game();

        game.SetupDragon();

        Assert.IsNotNull(game.Dragon);
        Assert.AreEqual("Dragon", game.Dragon.name);
        Assert.AreEqual("claws", game.Dragon.weapon.type);
        Assert.AreEqual(12, game.Dragon.weapon.maxDamage);
        Assert.IsTrue(game.Dragon.strength >= 1 && game.Dragon.strength <= 20);
        Assert.IsTrue(game.Dragon.agility >= 1 && game.Dragon.agility <= 20);
        Assert.IsTrue(game.Dragon.health >= 1 && game.Dragon.health <= 20);
    }

    [TestMethod]
    public void DisplayWelcomeAndMenu_InputOne_ReturnsTrue()
    {
        Game game = new Game();
        game.Messages.ReadDictionary(MessagesPath, "en");

        using StringReader reader = new StringReader("1\n");
        using StringWriter writer = new StringWriter();
        Console.SetIn(reader);
        Console.SetOut(writer);

        bool result = game.DisplayWelcomeAndMenu();

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void DisplayWelcomeAndMenu_InputTwo_ReturnsFalse()
    {
        Game game = new Game();
        game.Messages.ReadDictionary(MessagesPath, "en");

        using StringReader reader = new StringReader("2\n");
        using StringWriter writer = new StringWriter();
        Console.SetIn(reader);
        Console.SetOut(writer);

        bool result = game.DisplayWelcomeAndMenu();

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void SetupPlayer_CompletesCreationWorkflow()
    {
        Game game = new Game();
        game.Messages.ReadDictionary(MessagesPath, "en");

        // 1 (Human) -> Aragorn -> 1 (Fighter) -> roll -> roll -> roll -> 2 (Proceed)
        string inputSequence = "1\nAragorn\n1\nroll\nroll\nroll\n2\n";

        using StringReader reader = new StringReader(inputSequence);
        using StringWriter writer = new StringWriter();
        Console.SetIn(reader);
        Console.SetOut(writer);

        game.SetupPlayer();

        Assert.IsNotNull(game.Player);
        Assert.AreEqual("Aragorn", game.Player.Name);
        Assert.AreEqual(Race.Human, game.Player.Race);
        Assert.AreEqual(Occupation.Fighter, game.Player.Occupation);
        Assert.AreEqual("long sword", game.Player.Weapon.type);
        Assert.IsTrue(game.Player.HasRolledStats);
        Assert.IsTrue(game.Player.Strength >= 1);
        Assert.IsTrue(game.Player.Agility >= 1);
        Assert.IsTrue(game.Player.Health >= 1);
    }

    [TestMethod]
    public void PromptPlayAgain_InputYes_KeepsIsRunningTrue()
    {
        Game game = new Game { IsRunning = true };

        using StringReader reader = new StringReader("y\n");
        using StringWriter writer = new StringWriter();
        Console.SetIn(reader);
        Console.SetOut(writer);

        game.PromptPlayAgain();

        Assert.IsTrue(game.IsRunning);
    }

    [TestMethod]
    public void PromptPlayAgain_InputNo_SetsIsRunningFalse()
    {
        Game game = new Game { IsRunning = true };

        using StringReader reader = new StringReader("n\n");
        using StringWriter writer = new StringWriter();
        Console.SetIn(reader);
        Console.SetOut(writer);

        game.PromptPlayAgain();

        Assert.IsFalse(game.IsRunning);
    }
}