using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class DragonTest
{
    // Mock to verify Dragon queries GetMessage with valid ID ranges
    private class MockMessages : Messages
    {
        public int LastRequestedId { get; private set; }

        public override string GetMessage(int whichMessage)
        {
            LastRequestedId = whichMessage;
            return $"Message {whichMessage}";
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
        var d20 = new D20();

        dragon.InitializeStats(d20);

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
            // Expected
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
    public void MissedMeTaunt_DelegatesToMessagesService()
    {
        var mockMessages = new MockMessages();
        var dragon = new Dragon("Ignis", mockMessages);

        string taunt = dragon.MissedMeTaunt();

        Assert.IsTrue(mockMessages.LastRequestedId >= 101 && mockMessages.LastRequestedId <= 104);
        StringAssert.StartsWith(taunt, "Message ");
    }

    [TestMethod]
    public void WasHitTaunt_DelegatesToMessagesService()
    {
        var mockMessages = new MockMessages();
        var dragon = new Dragon("Ignis", mockMessages);

        string taunt = dragon.WasHitTaunt();

        Assert.IsTrue(mockMessages.LastRequestedId >= 201 && mockMessages.LastRequestedId <= 204);
        StringAssert.StartsWith(taunt, "Message ");
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