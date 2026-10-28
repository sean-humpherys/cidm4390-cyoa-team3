using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

[TestClass]
public class DragonTest
{
    private const string FullTestDictionaryPath = "DragonTauntsFullSample.txt";
    private const string PartialTestDictionaryPath = "DragonTauntsPartialSample.txt";

    [TestInitialize]
    public void Setup()
    {
        // Full dictionary containing all IDs: 101-104 and 201-204
        string[] fullLines = new string[]
        {
            "101=You swing like a sleepy goblin!",
            "102=A gust of wind poses more danger than you!",
            "103=Try harder, {race}! You are testing my patience!",
            "104=Ha! Not even close, little {race}!",
            "201=Roar! That actually stung!",
            "202=Insolent mortal, you will pay for that strike!",
            "203=A lucky blow! It will be your last!",
            "204=My scales! You dare chip my armor?!"
        };
        File.WriteAllLines(FullTestDictionaryPath, fullLines);

        // Partial dictionary missing IDs 101, 102, 201, 202 to verify retry logic
        string[] partialLines = new string[]
        {
            "103=Try harder, {race}! You are testing my patience!",
            "104=Ha! Not even close, little {race}!",
            "203=A lucky blow! It will be your last!",
            "204=My scales! You dare chip my armor?!"
        };
        File.WriteAllLines(PartialTestDictionaryPath, partialLines);
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
        messages.ReadDictionary(FullTestDictionaryPath);

        var dragon = new Dragon("Ignis", messages);
        string playerRace = "Elf";

        for (int i = 0; i < 20; i++)
        {
            string taunt = dragon.MissedMeTaunt(playerRace);
            Assert.IsFalse(string.IsNullOrWhiteSpace(taunt));
            Assert.IsFalse(taunt.Contains("{race}"), "Placeholder {race} was not replaced.");
        }
    }

    [TestMethod]
    public void WasHitTaunt_UsesMessagesDictionary()
    {
        var messages = new Messages();
        messages.ReadDictionary(FullTestDictionaryPath);

        var dragon = new Dragon("Ignis", messages);

        for (int i = 0; i < 20; i++)
        {
            string taunt = dragon.WasHitTaunt();
            Assert.IsFalse(string.IsNullOrWhiteSpace(taunt));
            Assert.AreNotEqual("!", taunt);
        }
    }

    [TestMethod]
    public void Taunts_WithPartialDictionary_SuccessfullyRetriesAndResolvesAvailableKey()
    {
        var messages = new Messages();
        messages.ReadDictionary(PartialTestDictionaryPath);

        var dragon = new Dragon("Ancalagon", messages);
        string race = "Dwarf";

        for (int i = 0; i < 20; i++)
        {
            string missedTaunt = dragon.MissedMeTaunt(race);
            string hitTaunt = dragon.WasHitTaunt();

            Assert.IsFalse(string.IsNullOrWhiteSpace(missedTaunt));
            Assert.AreNotEqual("...", missedTaunt);
            Assert.IsFalse(missedTaunt.Contains("{race}"));

            Assert.IsFalse(string.IsNullOrWhiteSpace(hitTaunt));
            Assert.AreNotEqual("!", hitTaunt);
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