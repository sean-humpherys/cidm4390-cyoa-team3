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
        ""fr"": ""Bienvenue dans votre jeu d'aventure"",
        ""es"": ""Bienvenido al juego «Elige tu aventura»""
      },
      ""choice_create_exit"": {
        ""en"": ""Choose; 1. Create Character or 2. Exit. Type '1' or '2'"",
        ""fr"": ""Choisissez : 1. Créer un personnage ou 2. Quitter. Tapez '1' ou '2'"",
        ""es"": ""Elige: 1. Crear personaje o 2. Salir. Escribe '1' o '2'""
      },
      ""choice_cardinal"": {
        ""en"": ""You are walking in the mountains... Enter 'n' for North. Enter 's' for South. Enter 'e' for exit game"",
        ""fr"": ""Vous marchez dans les montagnes... Entrez « n » pour le nord. Entrez « s » pour le sud. Entrez « e » pour quitter le jeu."",
        ""es"": ""Estás caminando por las montañas... Introduce 'n' para Norte. Introduce 's' para Sur. Introduce 'e' para salir del juego.""
      },
      ""choice_cardinal_south"": {
        ""en"": ""You take the South path... still bored"",
        ""fr"": ""Tu prends le chemin du Sud... tu t'ennuies."",
        ""es"": ""Tomas el camino del sur... sigues aburrido""
      },
      ""choice_cardinal_north"": {
        ""en"": ""You take the North path... The dragon has a hoard of gold behind it."",
        ""fr"": ""Vous prenez le sentier du Nord... Le dragon a un trésor d'or derrière lui."",
        ""es"": ""Tomas el camino del norte... El dragón tiene un tesoro de oro detrás.""
      },
      ""dragon_intro"": {
        ""en"": ""The dragon says 'I am {dragonName}. Leave or perish!'"",
        ""fr"": ""Le dragon dit : « Je suis {dragonName}. Partez ou périssez ! »"",
        ""es"": ""El dragón dice: 'Soy {dragonName}. ¡Vete o perece!'""
      },
      ""ring"": {
        ""en"": ""Your magic ring gives you the stats on the dragon."",
        ""fr"": ""Votre bague magique vous donne les statistiques du dragon."",
        ""es"": ""Tu anillo mágico te da las estadísticas del dragón.""
      },
      ""choice_retreat_attack"": {
        ""en"": ""Do you wish to retreat or attack? Enter 'r' for retreat. Enter 'a' for attack."",
        ""fr"": ""Souhaitez-vous battre en retraite ou attaquer ? Entrez « r » pour battre en retraite. Entrez « a » pour attaquer."",
        ""es"": ""¿Deseas retirarte o atacar? Introduce 'r' para retirarte. Introduce 'a' para atacar.""
      },
      ""character_initial"": {
        ""en"": ""What race of character do you want to be? Human, Elf, Dwarf or Halfling?"",
        ""fr"": ""Quelle race de personnage voulez-vous être ? Humain, Elfe, Nain ou Halfelin ?"",
        ""es"": ""¿Qué raza de personaje quieres ser? ¿Humano, Elfo, Enano o Mediano?""
      },
      ""character_name"": {
        ""en"": ""What is the name of your character?"",
        ""fr"": ""Quel est le nom de votre personnage ?"",
        ""es"": ""¿Cuál es el nombre de tu personaje?""
      },
      ""character_occupation"": {
        ""en"": ""What is the occupation of your character? Fighter, magician, thief, or archer?"",
        ""fr"": ""Quelle est la profession de votre personnage ? Combattant, magicien, voleur ou archer ?"",
        ""es"": ""¿Cuál es la ocupación de tu personaje? ¿Guerrero, mago, ladrón o arquero?""
      },
      ""character_strength"": {
        ""en"": ""Next, we are going to roll for strength. Type 'roll'"",
        ""fr"": ""Ensuite, nous allons lancer les dés pour la force. Tapez 'roll'"",
        ""es"": ""A continuación, vamos a tirar los dados para la fuerza. Escribe 'roll'""
      },
      ""character_agility"": {
        ""en"": ""Next, we will roll for agility. Type 'roll'"",
        ""fr"": ""Ensuite, nous allons lancer les dés pour l'agilité. Tapez 'roll'"",
        ""es"": ""A continuación, vamos a tirar los dados para la agilidad. Escribe 'roll'""
      },
      ""character_health"": {
        ""en"": ""Next, we will roll for health points. Type 'roll'"",
        ""fr"": ""Ensuite, nous allons lancer les dés pour les points de vie. Tapez 'roll'"",
        ""es"": ""A continuación, vamos a tirar los dados para los puntos de salud. Escribe 'roll'""
      },
      ""choice_stats_adventure"": {
        ""en"": ""What would you like to do next? 1. List character stats. 2. Proceed on an adventure. Type '1' or '2'"",
        ""fr"": ""Que voulez-vous faire ensuite ? 1. Lister les statistiques du personnage. 2. Partir à l'aventure. Tapez '1' ou '2'"",
        ""es"": ""¿Qué te gustaría hacer a continuación? 1. Listar las estadísticas del personaje. 2. Proceder a una aventura. Escribe '1' o '2'""
      },
      ""endgame_retreat"": {
        ""en"": ""You quickly retreat... The End."",
        ""fr"": ""Vous vous retirez rapidement... La Fin."",
        ""es"": ""Te retiras rápidamente... El Fin.""
      },
      ""endgame_dragon_defeated"": {
        ""en"": ""With the dragon vanquished... The End."",
        ""fr"": ""Avec le dragon vaincu... La Fin."",
        ""es"": ""Con el dragón vencido... El Fin.""
      },
      ""endgame_player_defeated"": {
        ""en"": ""The dragon laughs as your eyesight fades... The End."",
        ""fr"": ""Le dragon rit alors que votre vue s'estompe... La Fin."",
        ""es"": ""El dragón se ríe mientras tu vista se desvanece... El Fin.""
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
    public void SetupPlayer_CompletesCreationWorkflowAndAllStatRolls()
    {
        Game game = new Game();
        game.Messages.ReadDictionary(MessagesPath, "en");

        // Input sequence: 1 (Human) -> Aragorn -> 1 (Fighter) -> roll -> roll -> roll -> 2 (Proceed)
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

        Assert.IsTrue(game.Player.HasRolledStrength, "Strength was not recorded as rolled.");
        Assert.IsTrue(game.Player.HasRolledAgility, "Agility was not recorded as rolled.");
        Assert.IsTrue(game.Player.HasRolledHealth, "Health was not recorded as rolled.");
        Assert.IsTrue(game.Player.HasRolledStats, "Composite HasRolledStats should be true.");

        Assert.IsTrue(game.Player.Strength >= 1);
        Assert.IsTrue(game.Player.Agility >= 1);
        Assert.IsTrue(game.Player.Health >= 1);
    }

    [TestMethod]
    public void DisplayEndGame_DragonDefeated_PrintsDragonVanquishedMessage()
    {
        Game game = new Game();
        game.Messages.ReadDictionary(MessagesPath, "en");

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        game.DisplayEndGame("dragon_defeated");

        string output = writer.ToString();
        Assert.IsTrue(output.Contains("With the dragon vanquished... The End."));
    }

    [TestMethod]
    public void DisplayEndGame_PlayerDefeated_PrintsEyesightFadesMessage()
    {
        Game game = new Game();
        game.Messages.ReadDictionary(MessagesPath, "en");

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        game.DisplayEndGame("player_defeated");

        string output = writer.ToString();
        Assert.IsTrue(output.Contains("The dragon laughs as your eyesight fades... The End."));
    }

    [TestMethod]
    public void DisplayEndGame_CharacterRetreats_PrintsRetreatMessage()
    {
        Game game = new Game();
        game.Messages.ReadDictionary(MessagesPath, "en");

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        game.DisplayEndGame("retreat");

        string output = writer.ToString();
        Assert.IsTrue(output.Contains("You quickly retreat... The End."));
    }

    [TestMethod]
    public void DisplayEndGame_MissingOrNullMessages_UsesFallbackText()
    {
        // Instantiates Game without reading JSON dictionary to verify fallback strings
        Game game = new Game();

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        game.DisplayEndGame("dragon_defeated");

        string output = writer.ToString();
        Assert.IsTrue(output.Contains("With the dragon vanquished... The End."));
    }
}