using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class DragonTest
{
    private const string TestDictionaryPath = "DragonTauntsSample.txt";

    [TestInitialize]
    public void Setup()
    {
        // Write the sample test dictionary lines needed for testing
        File.WriteAllLines(TestDictionaryPath, new[]
        {
            "101=Missed me, breath of a gnat!",
            "102=Is that all you've got? I've seen kittens with more fire in their paws!",
            "103=Try harder, {race}! You're making me yawn!",
            "104=Maybe if you squinted harder, you'd actually hit something!",
            "201=Ow! That actually stung a bit!",
            "202=Impressive! You might actually be worth toasting...",
            "203=That...was unexpected. But I assure you, it won't happen again.",
            "204=Well, I'll be! You have more bite than I thought!"
        });
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Remove the temporary file after tests finish
        if (File.Exists(TestDictionaryPath))
        {
            File.Delete(TestDictionaryPath);
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
    public void InitializeStats_WithD20_AssignsValidStatRanges()
    {
        var dragon = new Dragon("Balerion");
        var die = new Die();

        dragon.InitializeStats(die);

        Assert.IsTrue(dragon.strength >= 1 && dragon.strength <= 20);
        Assert.IsTrue(dragon.agility >= 1 && dragon.agility <= 20);
        Assert.IsTrue(dragon.health >= 1 && dragon.health <= 20);
    }

    [TestMethod]
    public void InitializeStats_ThrowsWhenD20IsNull()
    {
        var dragon = new Dragon("Balerion");

        try
        {
            dragon.InitializeStats(null!);
            Assert.Fail("Expected ArgumentNullException was not thrown.");
        }
        catch (ArgumentNullException)
        {
            // Expected exception caught
        }
    }

    [TestMethod]
    public void InitializeStats_Overload_AssignsExactValues()
    {
        var dragon = new Dragon("Balerion");

        dragon.InitializeStats(18, 14, 20);

        Assert.AreEqual(18, dragon.strength);
        Assert.AreEqual(14, dragon.agility);
        Assert.AreEqual(20, dragon.health);
    }

    [TestMethod]
    public void MissedMeTaunt_UsesMessagesDictionaryAndReplacesRace()
    {
        var messages = new Messages();
        messages.ReadDictionary(TestDictionaryPath);

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
        messages.ReadDictionary(TestDictionaryPath);

        var dragon = new Dragon("Ignis", messages);

        for (int i = 0; i < 20; i++)
        {
            string taunt = dragon.WasHitTaunt();
            Assert.IsFalse(string.IsNullOrWhiteSpace(taunt));
        }
    }

    [TestMethod]
    public void DisplayStats_IncludesAllStatsAndWeaponName()
    {
        var dragon = new Dragon("Fafnir")
        {
            strength = 15,
            agility = 12,
            health = 25
        };

        string output = dragon.DisplayStats();

        StringAssert.Contains(output, "Fafnir");
        StringAssert.Contains(output, "15");
        StringAssert.Contains(output, "12");
        StringAssert.Contains(output, "25");
        StringAssert.Contains(output, "claws");
    }
}