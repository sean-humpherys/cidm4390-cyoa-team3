using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class DragonTest
{
    private Messages _messages = null!;
    private static readonly string[] ValidDragonNames = new string[]
    {
        "Ignis",
        "Vermithrax",
        "Smaug",
        "Fafnir",
        "Balerion",
        "Pyre",
        "Cinder"
    };

    [TestInitialize]
    public void TestInitialize()
    {
        _messages = new Messages();
        string messagesPath = Path.Combine(AppContext.BaseDirectory, "Dialogue.json");
        if (File.Exists(messagesPath))
        {
            _messages.ReadDictionary(messagesPath, "en");
        }
    }

    [TestMethod]
    public void Constructor_WithoutName_AssignsRandomNameFromPoolAndInitializesWeapon()
    {
        Dragon dragon = new Dragon(_messages);

        Assert.IsNotNull(dragon.name);
        CollectionAssert.Contains(ValidDragonNames, dragon.name, "Dragon name should be chosen from the defined name pool.");
        Assert.IsNotNull(dragon.weapon);
        Assert.AreEqual("claws", dragon.weapon.type);
        Assert.AreEqual(12, dragon.weapon.maxDamage);
    }

    [TestMethod]
    public void Constructor_WithExplicitName_SetsNameAndInitializesWeapon()
    {
        Dragon dragon = new Dragon("Draco", _messages);

        Assert.AreEqual("Draco", dragon.name);
        Assert.IsNotNull(dragon.weapon);
        Assert.AreEqual("claws", dragon.weapon.type);
        Assert.AreEqual(12, dragon.weapon.maxDamage);
    }

    [TestMethod]
    public void Constructor_EmptyOrWhitespaceName_ThrowsArgumentException()
    {
        try
        {
        _ = new Dragon("   ", _messages);
        Assert.Fail("Expected ArgumentException was not thrown.");
        }
        catch (ArgumentException)
        {
        // Test passes: expected exception caught
        }
    }

    [TestMethod]
    public void InitializeStats_WithDie_RollsStatsAboveZero()
    {
        Dragon dragon = new Dragon("Draco", _messages);
        Die die = new Die(20);

        dragon.InitializeStats(die);

        Assert.IsTrue(dragon.strength >= 1 && dragon.strength <= 20);
        Assert.IsTrue(dragon.agility >= 1 && dragon.agility <= 20);
        Assert.IsTrue(dragon.health >= 1 && dragon.health <= 20);
    }

    [TestMethod]
    public void InitializeStats_ExplicitValues_SetsPropertiesCorrectly()
    {
        Dragon dragon = new Dragon("Draco", _messages);

        dragon.InitializeStats(15, 14, 18);

        Assert.AreEqual(15, dragon.strength);
        Assert.AreEqual(14, dragon.agility);
        Assert.AreEqual(18, dragon.health);
    }

    [TestMethod]
    public void MissedMeTaunt_ReplacesCharacterRacePlaceholder()
    {
        Dragon dragon = new Dragon("Draco", _messages);

        string taunt = dragon.MissedMeTaunt("Elf");

        Assert.IsFalse(string.IsNullOrWhiteSpace(taunt));
        Assert.IsFalse(taunt.Contains("{characterRace}"));
    }

    [TestMethod]
    public void WasHitTaunt_ReturnsValidString()
    {
        Dragon dragon = new Dragon("Draco", _messages);

        string taunt = dragon.WasHitTaunt();

        Assert.IsFalse(string.IsNullOrWhiteSpace(taunt));
    }

    [TestMethod]
    public void DisplayStats_ContainsNameAndKeyStats()
    {
        Dragon dragon = new Dragon("Balerion", _messages);
        dragon.InitializeStats(18, 12, 20);

        string display = dragon.DisplayStats();

        StringAssert.Contains(display, "Balerion");
        StringAssert.Contains(display, "18");
        StringAssert.Contains(display, "12");
        StringAssert.Contains(display, "20");
        StringAssert.Contains(display, "claws");
    }
}