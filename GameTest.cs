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
        game.Messages.ReadDictionary(messagesPath, languageCode);
        return game;
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
        StringAssert.Contains(output, expectedError, "Should display localized invalid_choice error.");
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
    public void SetupPlayer_InvalidNameAndRolls_RepromptsAndPrintsImmediateFeedback()
    {
        Game game = CreateInitializedGame();

        // 1: Human race
        // Conan123: Invalid name (contains digits)
        // Conan: Valid name
        // 1: Fighter occupation
        // badroll -> roll (Strength)
        // badroll -> roll (Agility)
        // badroll -> roll (Health)
        // 2: Proceed to adventure (skip stats review)
        string simulatedInput = string.Join(Environment.NewLine, new[]
        {
            "1",
            "Conan123",
            "Conan",
            "1",
            "badroll",
            "roll",
            "badroll",
            "roll",
            "badroll",
            "roll",
            "2"
        }) + Environment.NewLine;

        SetConsoleInput(simulatedInput);

        game.SetupPlayer();

        string output = _consoleOutput.ToString();
        string invalidNameError = game.Messages.GetMessage("invalid_name");
        string invalidRollError = game.Messages.GetMessage("invalid_roll");

        // Assert player entity state
        Assert.IsNotNull(game.Player);
        Assert.AreEqual("Conan", game.Player.Name);
        Assert.AreEqual(Race.Human, game.Player.Race);
        Assert.AreEqual(Occupation.Fighter, game.Player.Occupation);
        Assert.IsTrue(game.Player.Health > 0);

        // Assert invalid input handling
        StringAssert.Contains(output, invalidNameError, "Should show invalid_name error when digits are entered.");
        StringAssert.Contains(output, invalidRollError, "Should show invalid_roll error when not entering 'roll'.");

        // Assert immediate stat roll feedback
        StringAssert.Contains(output, $"Strength rolled: {game.Player.Strength}", "Should print Strength feedback.");
        StringAssert.Contains(output, $"Agility rolled: {game.Player.Agility}", "Should print Agility feedback.");
        StringAssert.Contains(output, $"Health Points rolled: {game.Player.Health}", "Should print Health feedback.");
    }

    [TestMethod]
    public void DisplayEndGame_DragonDefeated_PrintsLocalizedVictoryMessage()
    {
        Game game = CreateInitializedGame();

        game.DisplayEndGame("dragon_defeated");

        string output = _consoleOutput.ToString();
        string expectedVictory = game.Messages.GetMessage("endgame_dragon_defeated");

        StringAssert.Contains(output, expectedVictory);
    }

    [TestMethod]
    public void DisplayEndGame_PlayerDefeated_PrintsLocalizedLossMessage()
    {
        Game game = CreateInitializedGame();

        game.DisplayEndGame("player_defeated");

        string output = _consoleOutput.ToString();
        string expectedLoss = game.Messages.GetMessage("endgame_player_defeated");

        StringAssert.Contains(output, expectedLoss);
    }

    [TestMethod]
    public void DisplayEndGame_Retreat_PrintsLocalizedRetreatMessage()
    {
        Game game = CreateInitializedGame();

        game.DisplayEndGame("retreat");

        string output = _consoleOutput.ToString();
        string expectedRetreat = game.Messages.GetMessage("endgame_retreat");

        StringAssert.Contains(output, expectedRetreat);
    }

    [TestMethod]
    public void PromptPlayAgain_InputNo_SetsIsRunningToFalse()
    {
        Game game = CreateInitializedGame();
        game.IsRunning = true;
        // Input: 'n'
        SetConsoleInput($"n{Environment.NewLine}");

        game.PromptPlayAgain();

        Assert.IsFalse(game.IsRunning);
    }

    [TestMethod]
    public void PromptPlayAgain_InvalidLetterThenYes_RepromptsAndKeepsRunning()
    {
        Game game = CreateInitializedGame();
        game.IsRunning = true;
        // Input: 'x' (invalid), then 'y'
        SetConsoleInput($"x{Environment.NewLine}y{Environment.NewLine}");

        game.PromptPlayAgain();

        string output = _consoleOutput.ToString();
        string expectedError = game.Messages.GetMessage("invalid_letter");

        Assert.IsTrue(game.IsRunning);
        StringAssert.Contains(output, expectedError, "Should display invalid_letter error on invalid play-again answer.");
    }
}