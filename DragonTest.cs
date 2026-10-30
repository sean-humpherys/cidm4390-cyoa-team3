using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

[TestClass]
public class DragonTest
{
    private const string FullTestDictionaryPath = "DragonTauntsFullSample.json";
    private const string PartialTestDictionaryPath = "DragonTauntsPartialSample.json";

    [TestInitialize]
    public void Setup()
    {
        // Full file containing both dragon messages
        string fullJson = """
         {
              "dragon_taunt": { "en": [
                "You swing like a sleepy goblin!",
                "A gust of wind poses more danger than you!",
                "Try harder, {characterRace}! You are testing my patience!",
                "Ha! Not even close, little {characterRace}!"
              ] },
              "dragon_defeated_false": { "en": [
                "Roar! That actually stung!",
                "Insolent mortal, you will pay for that strike!",
                "A lucky blow! It will be your last!",
                "My scales! You dare chip my armor?!"
              ] }
            }
        """;
        
            File.WriteAllText(FullTestDictionaryPath, fullJson);

        // Partial file with only dragon_taunt
        string partialJson = """
        {
              "dragon_taunt": { "en": [
                "Try harder, {characterRace}! You are testing my patience!",
                "Ha! Not even close, little {characterRace}!"
              ] }
            }
        """;
        File.WriteAllText(PartialTestDictionaryPath, partialJson);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(FullTestDictionaryPath))
        {
            File.Delete(FullTestDictionaryPath);
        }

        if (File.Exists(PartialTestDictionaryPath))
        {
            File.Delete(PartialTestDictionaryPath);
        }
    }

    [TestMethod]
    public void Constructor_SetsNameAndClawsWeapon()
    {
        var dragon = new Dragon("Smaug");

        Assert.AreEqual("Smaug", dragon.name);
        Assert.IsNotNull(dragon.weapon);
        Assert.AreEqual("claws", dragon.weapon.type);
        Assert.AreEqual(12, dragon.weapon.maxDamage);
        Assert.AreEqual("##", dragon.weapon.asciiArt);
    }

    [TestMethod]
    public void Constructor_ThrowsException_WhenNameIsEmpty()
    {
        try
        {
            _ = new Dragon("");
            Assert.Fail("Expected ArgumentException was not thrown when name is empty.");
        }
        catch (ArgumentException)
        {
            // Successfully caught expected exception
        }
    }

    [TestMethod]
    public void Constructor_ThrowsException_WhenNameIsWhitespace()
    {
        try
        {
            _ = new Dragon("   ");
            Assert.Fail("Expected ArgumentException was not thrown when name is whitespace.");
        }
        catch (ArgumentException)
        {
            // Successfully caught expected exception
        }
    }

    [TestMethod]
    public void Constructor_ThrowsException_WhenNameIsNull()
    {
        try
        {
            _ = new Dragon(null!);
            Assert.Fail("Expected ArgumentException was not thrown when name is null.");
        }
        catch (ArgumentException)
        {
            // Successfully caught expected exception
        }
    }

    [TestMethod]
    public void InitializeStats_WithDie_AssignsValidStatRanges()
    {
        var dragon = new Dragon("Balerion");
        var die = new Die();

        dragon.InitializeStats(die);

        Assert.IsTrue(dragon.strength >= 1 && dragon.strength <= 20);
        Assert.IsTrue(dragon.agility >= 1 && dragon.agility <= 20);
        Assert.IsTrue(dragon.health >= 1 && dragon.health <= 20);
    }

    [TestMethod]
    public void InitializeStats_ThrowsWhenDieIsNull()
    {
        try
        {
            var dragon = new Dragon("Balerion");
            dragon.InitializeStats(null!);
            Assert.Fail("Expected ArgumentNullException was not thrown when die is null.");
        }
        catch (ArgumentNullException)
        {
            // Successfully caught expected exception
        }
    }

    [TestMethod]
    public void InitializeStats_Overload_AssignsExactValues()
    {
        var dragon = new Dragon("Vermithrax");

        dragon.InitializeStats(18, 14, 20);

        Assert.AreEqual(18, dragon.strength);
        Assert.AreEqual(14, dragon.agility);
        Assert.AreEqual(20, dragon.health);
    }

    [TestMethod]
    public void MissedMeTaunt_UsesMessagesDictionaryAndReplacesRace()
    {
        var messages = new Messages();
        messages.ReadDictionary(FullTestDictionaryPath, "en");

        var dragon = new Dragon("Ignis", messages);
        string playerRace = "Elf";

        for (int i = 0; i < 20; i++)
        {
            string taunt = dragon.MissedMeTaunt(playerRace);
            Assert.IsFalse(string.IsNullOrWhiteSpace(taunt));
            Assert.IsFalse(taunt.Contains("{characterRace}"), "Placeholder {characterRace} was not replaced.");
        }
    }

    [TestMethod]
    public void WasHitTaunt_UsesMessagesDictionary()
    {
        var messages = new Messages();
        messages.ReadDictionary(FullTestDictionaryPath, "en");

        var dragon = new Dragon("Ignis", messages);

        for (int i = 0; i < 20; i++)
        {
            string taunt = dragon.WasHitTaunt();
            Assert.IsFalse(string.IsNullOrWhiteSpace(taunt));
            Assert.AreNotEqual("!", taunt);
        }
    }

    [TestMethod]
    public void Taunts_WithPartialDictionary_ReturnsFallbackForTheMissingMessage()
    {
        var messages = new Messages();
        messages.ReadDictionary(PartialTestDictionaryPath, "en");

        var dragon = new Dragon("Ancalagon", messages);
        string race = "Dwarf";

        for (int i = 0; i < 20; i++)
        {
            string missedTaunt = dragon.MissedMeTaunt(race);
            string hitTaunt = dragon.WasHitTaunt();

            // dragon_taunt exists, so a real taunt comes back
            Assert.IsFalse(string.IsNullOrWhiteSpace(missedTaunt));
            Assert.AreNotEqual("...", missedTaunt);
            Assert.IsFalse(missedTaunt.Contains("{characterRace}"));

            // dragon_defeated_false is missing, so the fallback comes back 
            Assert.AreEqual("!", hitTaunt);

            
        }
    }

    [TestMethod]
    public void Taunts_WhenMessagesIsNull_ReturnsLanguageNeutralFallbacks()
    {
        var dragon = new Dragon("Fafnir", null);

        string missedTaunt = dragon.MissedMeTaunt("Human");
        string hitTaunt = dragon.WasHitTaunt();

        Assert.AreEqual("...", missedTaunt);
        Assert.AreEqual("!", hitTaunt);
    }

    [TestMethod]
    public void Taunts_WhenDictionaryEmpty_CatchesAllExceptionsAndReturnsLanguageNeutralFallbacks()
    {
        var emptyMessages = new Messages();
        var dragon = new Dragon("Glaurung", emptyMessages);

        string missedTaunt = dragon.MissedMeTaunt("Halfling");
        string hitTaunt = dragon.WasHitTaunt();

        Assert.AreEqual("...", missedTaunt);
        Assert.AreEqual("!", hitTaunt);
    }

    [TestMethod]
    public void DisplayStats_IncludesAllStatsAndWeaponName()
    {
        var dragon = new Dragon("Draco");
        dragon.InitializeStats(15, 12, 19);

        string display = dragon.DisplayStats();

        StringAssert.Contains(display, "Draco");
        StringAssert.Contains(display, "15");
        StringAssert.Contains(display, "12");
        StringAssert.Contains(display, "19");
        StringAssert.Contains(display, "claws");
    }
}