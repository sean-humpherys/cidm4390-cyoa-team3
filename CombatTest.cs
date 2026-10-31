using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

[TestClass]
public class CombatTests
{
    // Combat rolls a 20-sided die for every attack and defense roll, and the
    // roll is always 1-20. That lets us make outcomes certain without mocking:
    //   strength / agility of 20 or more -> the check ALWAYS succeeds
    //   strength / agility of 0          -> the check ALWAYS fails
    // A weapon with maxDamage 1 always deals exactly 1 damage.
    private const int AlwaysSucceeds = 20;
    private const int AlwaysFails = 0;

    private StringWriter consoleOutput = new StringWriter();
    private TextWriter originalOut = Console.Out;
    private TextReader originalIn = Console.In;

    [TestInitialize]
    public void SetUp()
    {
        originalOut = Console.Out;
        originalIn = Console.In;
        consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        Console.SetIn(new StringReader(""));
    }

    [TestCleanup]
    public void TearDown()
    {
        Console.SetOut(originalOut);
        Console.SetIn(originalIn);
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    // Player.Strength and Player.Agility have private setters (they are set by
    // the dice rolls), so the tests use reflection to set exact values.
    private static void SetPrivate(object target, string propertyName, object value)
    {
        PropertyInfo? property = target.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        Assert.IsNotNull(property, $"Property {propertyName} was not found.");
        property.GetSetMethod(true)!.Invoke(target, new[] { value });
    }

    private static Player CreatePlayer(
        int strength,
        int agility,
        int health,
        Occupation occupation = Occupation.Fighter,
        int? maxDamage = null)
    {
        Player player = new Player("Aria", Race.Elf, occupation);

        SetPrivate(player, "Strength", strength);
        SetPrivate(player, "Agility", agility);
        player.Health = health;

        if (maxDamage.HasValue)
        {
            SetPrivate(player, "Weapon", new Weapon("test weapon", maxDamage.Value, "x"));
        }

        return player;
    }

    private static Dragon CreateDragon(
        int strength,
        int agility,
        int health,
        int maxDamage = 1)
    {
        Dragon dragon = new Dragon("Smaug");
        dragon.InitializeStats(strength, agility, health);
        dragon.weapon = new Weapon("claws", maxDamage, "##");
        return dragon;
    }

    // Supplies the keystrokes the player "types" (one per line).
    private static void SetInput(params string[] lines)
    {
        Console.SetIn(new StringReader(string.Join(Environment.NewLine, lines) + Environment.NewLine));
    }

    private string Output => consoleOutput.ToString();

    private static int CountOccurrences(string text, string value)
    {
        int count = 0;
        int index = 0;

        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += value.Length;
        }

        return count;
    }

    // ===============================================================
    // PlayerAttacksDragonSequence: intro text and weapon art
    // ===============================================================

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_DisplaysIntroWithOccupationAndWeapon()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);
        SetInput("r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        StringAssert.Contains(
            Output,
            "You prepare yourself to attack the dragon with your weapon. " +
            "As a Fighter you withdraw your long sword. You attack.");
    }

    [TestMethod]
    [DataRow(Occupation.Fighter, "-)=====>")]
    [DataRow(Occupation.Magician, "zap~~~~~~")]
    [DataRow(Occupation.Thief, "-)==>")]
    [DataRow(Occupation.Archer, "}     -->")]
    [Timeout(5000)]
    public void PlayerAttack_DisplaysWeaponImageForOccupation(
        Occupation occupation,
        string expectedArt)
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50, occupation);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);
        SetInput("r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        StringAssert.Contains(Output, expectedArt);
    }

    // ===============================================================
    // PlayerAttacksDragonSequence: the player's attack roll
    // ===============================================================

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_Miss_DisplaysMissAndLeavesDragonUnharmed()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);
        SetInput("r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        StringAssert.Contains(Output, "You missed Smaug.");
        Assert.IsFalse(Output.Contains("You hit"));
        Assert.AreEqual(50, dragon.health);
    }

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_HitAndDragonDefends_ShowsDefenseAndTaunt()
    {
        Player player = CreatePlayer(AlwaysSucceeds, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysSucceeds, 50);
        SetInput("r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        StringAssert.Contains(Output, "You hit Smaug.");
        StringAssert.Contains(Output, "Smaug successfully defends your attack.");
        StringAssert.Contains(Output, dragon.MissedMeTaunt(player.Race.ToString()));
        Assert.IsFalse(Output.Contains("failed to defend"));
        Assert.AreEqual(50, dragon.health);
    }

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_HitAndDragonFailsToDefend_AppliesDamage()
    {
        Player player = CreatePlayer(AlwaysSucceeds, AlwaysFails, 50, maxDamage: 1);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);
        SetInput("r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        StringAssert.Contains(Output, "You hit Smaug.");
        StringAssert.Contains(
            Output,
            "The dragon failed to defend your attack. " +
            "Your weapon delivers 1 damage points. " +
            "The dragon's health points is now 49.");
        Assert.AreEqual(49, dragon.health);
    }

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_DragonSurvivesHit_ShowsDragonSarcasticReply()
    {
        Player player = CreatePlayer(AlwaysSucceeds, AlwaysFails, 50, maxDamage: 1);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);
        SetInput("r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        StringAssert.Contains(Output, dragon.WasHitTaunt());
        Assert.IsFalse(Output.Contains("you have defeated the dragon"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_DamageNeverExceedsWeaponMaxDamage()
    {
        const int attempts = 50;
        const int maxDamage = 12;

        Player player = CreatePlayer(AlwaysSucceeds, AlwaysFails, 50, maxDamage: maxDamage);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 100000);
        Combat combat = new Combat(player, dragon);

        // One "r" per call so each sequence ends by retreating.
        SetInput(Enumerable.Repeat("r", attempts).ToArray());

        for (int i = 0; i < attempts; i++)
        {
            int before = dragon.health;

            combat.PlayerAttacksDragonSequence();

            int damage = before - dragon.health;
            Assert.IsTrue(
                damage >= 1 && damage <= maxDamage,
                $"Damage {damage} was outside 1-{maxDamage}.");
        }
    }

    // ===============================================================
    // PlayerAttacksDragonSequence: defeating the dragon
    // ===============================================================

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_DragonDefeated_DisplaysVictoryAndStopsCombat()
    {
        Player player = CreatePlayer(AlwaysSucceeds, AlwaysFails, 50, maxDamage: 1);
        Dragon dragon = CreateDragon(AlwaysSucceeds, AlwaysFails, 1);
        SetInput("r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        StringAssert.Contains(
            Output,
            "With the last words of 'Arggg!', you have defeated the dragon.");
        Assert.IsTrue(dragon.health <= 0);

        // The fight is over: no dragon turn, no retreat prompt, player unharmed.
        Assert.IsFalse(Output.Contains("dragon's turn"));
        Assert.IsFalse(Output.Contains("Enter 'r' to retreat"));
        Assert.AreEqual(50, player.Health);
    }

    // ===============================================================
    // PlayerAttacksDragonSequence: the dragon's turn and the player's defeat
    // ===============================================================

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_DragonTakesATurnAfterThePlayer()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);
        SetInput("r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        StringAssert.Contains(Output, "It is the dragon's turn to attack.");
        StringAssert.Contains(Output, "The dragon missed you.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_PlayerDefeatedByDragon_EndsWithoutRetreatPrompt()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 1);
        Dragon dragon = CreateDragon(AlwaysSucceeds, AlwaysFails, 50, maxDamage: 1);
        SetInput("r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        StringAssert.Contains(
            Output,
            "With the last words of 'Oh no!', you have been defeated by the dragon.");
        Assert.IsTrue(player.Health <= 0);
        Assert.IsFalse(Output.Contains("Enter 'r' to retreat"));
        Assert.IsFalse(Output.Contains("You hastily retreat"));
    }

    // ===============================================================
    // PlayerAttacksDragonSequence: retreat / attack prompt
    // ===============================================================

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_PromptsForRetreatOrAttack()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);
        SetInput("r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        StringAssert.Contains(Output, "Enter 'r' to retreat or 'a' to attack.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_Retreat_DisplaysRetreatMessageAndStops()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);
        SetInput("r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        StringAssert.Contains(Output, "You hastily retreat licking your wounds.");
        Assert.AreEqual(1, CountOccurrences(Output, "You prepare yourself to attack"));
    }

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_ChoosingAttack_RepeatsTheAttackSequence()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);
        SetInput("a", "a", "r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        Assert.AreEqual(3, CountOccurrences(Output, "You prepare yourself to attack"));
        StringAssert.Contains(Output, "You hastily retreat licking your wounds.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_InvalidChoice_PromptsAgain()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);
        SetInput("x", "", "r");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        Assert.AreEqual(2, CountOccurrences(Output, "Invalid choice. Please try again."));
        StringAssert.Contains(Output, "You hastily retreat licking your wounds.");
    }

    [TestMethod]
    [Timeout(5000)]
    public void PlayerAttack_ChoiceIsNotCaseSensitiveOrSpaceSensitive()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);
        SetInput("  A  ", "  R  ");

        new Combat(player, dragon).PlayerAttacksDragonSequence();

        Assert.AreEqual(2, CountOccurrences(Output, "You prepare yourself to attack"));
        StringAssert.Contains(Output, "You hastily retreat licking your wounds.");
    }

    // ===============================================================
    // DragonAttacksPlayerSequence
    // ===============================================================

    [TestMethod]
    public void DragonAttack_Miss_DisplaysMissAndLeavesPlayerUnharmed()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysFails, AlwaysFails, 50);

        new Combat(player, dragon).DragonAttacksPlayerSequence();

        StringAssert.Contains(Output, "It is the dragon's turn to attack.");
        StringAssert.Contains(Output, "The dragon missed you.");
        Assert.IsFalse(Output.Contains("hits you"));
        Assert.AreEqual(50, player.Health);
    }

    [TestMethod]
    public void DragonAttack_HitAndPlayerDefends_ShowsDefenseAndTaunt()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysSucceeds, 50);
        Dragon dragon = CreateDragon(AlwaysSucceeds, AlwaysFails, 50);

        new Combat(player, dragon).DragonAttacksPlayerSequence();

        StringAssert.Contains(Output, "Smaug hits you.");
        StringAssert.Contains(Output, "You successfully defend the attack.");
        StringAssert.Contains(Output, player.MissedMeTaunt());
        Assert.AreEqual(50, player.Health);
    }

    [TestMethod]
    public void DragonAttack_HitAndPlayerFailsToDefend_AppliesDamage()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysSucceeds, AlwaysFails, 50, maxDamage: 1);

        new Combat(player, dragon).DragonAttacksPlayerSequence();

        StringAssert.Contains(Output, "Smaug hits you.");
        StringAssert.Contains(
            Output,
            "The dragon's attack delivers 1 damage points. " +
            "Your health points is now 49.");
        StringAssert.Contains(Output, player.WasHitTaunt());
        Assert.AreEqual(49, player.Health);
    }

    [TestMethod]
    public void DragonAttack_PlayerDefeated_DisplaysDefeatMessage()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 1);
        Dragon dragon = CreateDragon(AlwaysSucceeds, AlwaysFails, 50, maxDamage: 1);

        new Combat(player, dragon).DragonAttacksPlayerSequence();

        StringAssert.Contains(
            Output,
            "With the last words of 'Oh no!', you have been defeated by the dragon.");
        Assert.IsTrue(player.Health <= 0);
        Assert.IsFalse(Output.Contains(player.WasHitTaunt()));
    }

    [TestMethod]
    public void DragonAttack_DamageNeverExceedsDragonWeaponMaxDamage()
    {
        const int attempts = 100;
        const int maxDamage = 12;

        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 100000);
        Dragon dragon = CreateDragon(AlwaysSucceeds, AlwaysFails, 50, maxDamage);
        Combat combat = new Combat(player, dragon);

        for (int i = 0; i < attempts; i++)
        {
            int before = player.Health;

            combat.DragonAttacksPlayerSequence();

            int damage = before - player.Health;
            Assert.IsTrue(
                damage >= 1 && damage <= maxDamage,
                $"Damage {damage} was outside 1-{maxDamage}.");
        }
    }

    [TestMethod]
    public void DragonAttack_DoesNotChangeDragonHealth()
    {
        Player player = CreatePlayer(AlwaysFails, AlwaysFails, 50);
        Dragon dragon = CreateDragon(AlwaysSucceeds, AlwaysFails, 50, maxDamage: 1);

        new Combat(player, dragon).DragonAttacksPlayerSequence();

        Assert.AreEqual(50, dragon.health);
    }
}