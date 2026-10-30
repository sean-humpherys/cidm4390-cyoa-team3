using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GameTest
{
    private TextReader _originalIn = null!;
    private TextWriter _originalOut = null!;
    private const string DummyEnglishFile = "English.txt";

    [TestInitialize]
    public void Setup()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;

        string[] sampleLines = new[]
        {
            "1=Welcome to Choose Your Adventure Game",
            "2=Choose; 1. Create Character or 2. Exit. Type '1' or '2'",
            "3=Invalid entry. Please try again.",
            "4==== CHARACTER CREATION ===",
            "5=Enter your hero's name: ",
            "6=Name cannot be blank.",
            "7=Choose your Race:\n1. Elf\n2. Human\n3. Dwarf\n4. Halfling",
            "8=Choose your Occupation:\n1. Fighter\n2. Magician\n3. Thief\n4. Archer",
            "9=Rolling your hero's attributes with a D20...",
            "10=Press any key to continue...",
            "11=You are walking in the mountains...Enter 'n' for North. Enter 's' for South. Enter 'e' for exit game",
            "12=You take the South path...still bored.",
            "13=You take the North path...The dragon has a hoard of gold behind it.",
            "14=The dragon says 'I am {0}. Leave or perish!'",
            "15=Your magic ring gives you the stats on the dragon.",
            "16=Do you wish to retreat or attack? Enter 'r' for retreat. Enter 'a' for attack.",
            "17=You quickly retreat. The End.",
            "18=Would you like to embark on another adventure? (Y/N): ",
            "19=Farewell, brave adventurer."
        };
        File.WriteAllLines(DummyEnglishFile, sampleLines);
    }

    [TestCleanup]
    public void Cleanup()
    {
        Console.SetIn(_originalIn);
        Console.SetOut(_originalOut);

        if (File.Exists(DummyEnglishFile))
        {
            File.Delete(DummyEnglishFile);
        }
    }

    [TestMethod]
    public void DefaultConstructor_InitializesWithDefaultValues()
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
    public void OverloadedConstructor_ValidArguments_AssignsProperties()
    {
        Messages messages = new Messages();
        Die die = new Die(12);

        Game game = new Game(messages, die);

        Assert.AreSame(messages, game.Messages);
        Assert.AreSame(die, game.Die);
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
    public void SetupDragon_InstantiatesDragonWithRolledStats()
    {
        Game game = new Game();
        game.SetupDragon();

        Assert.IsNotNull(game.Dragon);
        Assert.AreEqual("Dragon", game.Dragon.name);
        Assert.IsTrue(game.Dragon.strength >= 1 && game.Dragon.strength <= 20);
        Assert.IsTrue(game.Dragon.agility >= 1 && game.Dragon.agility <= 20);
        Assert.IsTrue(game.Dragon.health >= 1 && game.Dragon.health <= 20);
        Assert.IsNotNull(game.Dragon.weapon);
        Assert.AreEqual("claws", game.Dragon.weapon.type);
    }

    [TestMethod]
    public void DisplayWelcomeAndMenu_InputOne_ReturnsTrue()
    {
        Game game = new Game();
        game.Messages.ReadDictionary(DummyEnglishFile);

        using StringReader reader = new StringReader("1\n");
        using StringWriter writer = new StringWriter();
        Console.SetIn(reader);
        Console.SetOut(writer);

        bool result = game.DisplayWelcomeAndMenu();

        Assert.IsTrue(result);
        string output = writer.ToString();
        Assert.IsTrue(output.Contains("Welcome to Choose Your Adventure Game"));
    }

    [TestMethod]
    public void DisplayWelcomeAndMenu_InputTwo_ReturnsFalse()
    {
        Game game = new Game();
        game.Messages.ReadDictionary(DummyEnglishFile);

        using StringReader reader = new StringReader("2\n");
        using StringWriter writer = new StringWriter();
        Console.SetIn(reader);
        Console.SetOut(writer);

        bool result = game.DisplayWelcomeAndMenu();

        Assert.IsFalse(result);
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

        Assert.AreEqual("Welcome to Choose Your Adventure Game", game.Messages.GetMessage(1));
    }

    [TestMethod]
    public void PromptPlayAgain_InputYes_KeepsIsRunningTrue()
    {
        Game game = new Game { IsRunning = true };
        game.Messages.ReadDictionary(DummyEnglishFile);

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
        game.Messages.ReadDictionary(DummyEnglishFile);

        using StringReader reader = new StringReader("n\n");
        using StringWriter writer = new StringWriter();
        Console.SetIn(reader);
        Console.SetOut(writer);

        game.PromptPlayAgain();

        Assert.IsFalse(game.IsRunning);
    }
}