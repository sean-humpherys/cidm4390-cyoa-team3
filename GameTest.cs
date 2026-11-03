using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class GameTest
{
    private StringWriter _consoleOutput = null!;
    private TextWriter _originalOutput = null!;
    private TextReader _originalInput = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _consoleOutput = new StringWriter();
        _originalOutput = Console.Out;
        _originalInput = Console.In;
        Console.SetOut(_consoleOutput);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        Console.SetOut(_originalOutput);
        Console.SetIn(_originalInput);
        _consoleOutput.Dispose();
    }

    private void SetConsoleInput(string input)
    {
        Console.SetIn(new StringReader(input));
    }

    private Game CreateInitializedGame(string languageCode = "en")
    {
        Game game = new Game();
        string messagesPath = Path.Combine(AppContext.BaseDirectory, "Dialogue.json");
        if (File.Exists(messagesPath))
        {
            game.Messages.ReadDictionary(messagesPath, languageCode);
        }
        return game;
    }

    [TestMethod]
    public void SetupDragon_InitializesDragonWithRandomNameAndRolledStats()
    {
        Game game = CreateInitializedGame();

        game.SetupDragon();

        Assert.IsNotNull(game.Dragon);
        Assert.IsFalse(string.IsNullOrWhiteSpace(game.Dragon.name));
        Assert.AreNotEqual("Dragon", game.Dragon.name, "Dragon name should be generated from pool, not hardcoded placeholder.");
        Assert.IsTrue(game.Dragon.health > 0);
        Assert.IsTrue(game.Dragon.strength > 0);
        Assert.IsTrue(game.Dragon.agility > 0);
    }

    [TestMethod]
    public void DisplayWelcomeAndMenu_InvalidChoiceThenCreate_RepromptsAndReturnsTrue()
    {
        Game game = CreateInitializedGame();
        // Input: 9 (invalid), then 1 (Create Character)
        SetConsoleInput($"9{Environment.NewLine}1{Environment.NewLine}");

        bool result = game.DisplayWelcomeAndMenu();

        string output = _consoleOutput.ToString();
        string expectedError = game.Messages.GetMessage("invalid_choice");

        Assert.IsTrue(result);
        StringAssert.Contains(output, expectedError);
    }

    [TestMethod]
    public void DisplayWelcomeAndMenu_ExitChoice_ReturnsFalse()
    {
        Game game = CreateInitializedGame();
        // Input: 2 (Exit)
        SetConsoleInput($"2{Environment.NewLine}");

        bool result = game.DisplayWelcomeAndMenu();

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void SetupPlayer_InvalidInputsThenValid_SetsUpPlayerWithFeedback()
    {
        Game game = CreateInitializedGame();

        // Inputs:
        // Race: 5 (invalid) -> 1 (Human)
        // Name: Arthur99 (invalid) -> Arthur
        // Occupation: 9 (invalid) -> 1 (Fighter)
        // Strength: roll
        // Agility: roll
        // Health: roll
        // Stats Review: 2 (Proceed)
        string simulatedInput = string.Join(Environment.NewLine, new[]
        {
            "5",
            "1",
            "Arthur99",
            "Arthur",
            "9",
            "1",
            "roll",
            "roll",
            "roll",
            "2"
        }) + Environment.NewLine;

        SetConsoleInput(simulatedInput);

        game.SetupPlayer();

        string output = _consoleOutput.ToString();

        Assert.IsNotNull(game.Player);
        Assert.AreEqual("Arthur", game.Player.Name);
        Assert.AreEqual(Race.Human, game.Player.Race);
        Assert.AreEqual(Occupation.Fighter, game.Player.Occupation);

        // Verification of reprompt errors
        StringAssert.Contains(output, game.Messages.GetMessage("invalid_choice"));
        StringAssert.Contains(output, game.Messages.GetMessage("invalid_name"));

        // Verification of immediate roll feedback
        StringAssert.Contains(output, $"Strength rolled: {game.Player.Strength}");
        StringAssert.Contains(output, $"Agility rolled: {game.Player.Agility}");
        StringAssert.Contains(output, $"Health Points rolled: {game.Player.Health}");
    }

    [TestMethod]
    public void DisplayEndGame_DragonDefeated_PrintsLocalizedVictoryMessage()
    {
        Game game = CreateInitializedGame();

        game.DisplayEndGame("dragon_defeated");

        string output = _consoleOutput.ToString();
        StringAssert.Contains(output, game.Messages.GetMessage("endgame_dragon_defeated"));
    }

    [TestMethod]
    public void DisplayEndGame_PlayerDefeated_PrintsLocalizedLossMessage()
    {
        Game game = CreateInitializedGame();

        game.DisplayEndGame("player_defeated");

        string output = _consoleOutput.ToString();
        StringAssert.Contains(output, game.Messages.GetMessage("endgame_player_defeated"));
    }

    [TestMethod]
    public void DisplayEndGame_Retreat_PrintsLocalizedRetreatMessage()
    {
        Game game = CreateInitializedGame();

        game.DisplayEndGame("retreat");

        string output = _consoleOutput.ToString();
        StringAssert.Contains(output, game.Messages.GetMessage("endgame_retreat"));
    }

    [TestMethod]
    public void PromptPlayAgain_InputNo_SetsIsRunningToFalse()
    {
        Game game = CreateInitializedGame();
        game.IsRunning = true;
        SetConsoleInput($"n{Environment.NewLine}");

        game.PromptPlayAgain();

        Assert.IsFalse(game.IsRunning);
    }

    [TestMethod]
    public void PromptPlayAgain_InvalidLetterThenYes_RepromptsAndKeepsRunning()
    {
        Game game = CreateInitializedGame();
        game.IsRunning = true;
        SetConsoleInput($"x{Environment.NewLine}y{Environment.NewLine}");

        game.PromptPlayAgain();

        string output = _consoleOutput.ToString();
        Assert.IsTrue(game.IsRunning);
        StringAssert.Contains(output, game.Messages.GetMessage("invalid_letter"));
    }
}