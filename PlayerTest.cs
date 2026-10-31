using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

[TestClass]
public class CharacterCreationTests
{
    // Helper: creates a player and rolls strength, agility and health in the
    // same order the game uses (strength must be rolled before health).
    private static Player CreateRolledPlayer(Race race, Occupation occupation)
    {
        Player player = new Player("Aria", race, occupation);

        player.RollStrength();
        player.RollAgility();
        player.RollHealth();

        return player;
    }

    // ---------------------------------------------------------------
    // Constructors and identity
    // ---------------------------------------------------------------

    //Verifies that the Player constructor correctly stores character info(name, race, occupation, etc)
    //Also checks that stats have not been rolled.
    [TestMethod]
    public void PlayerConstructor_StoresCharacterChoices()
    {
        Player player = new Player(
            "Aria",
            Race.Elf,
            Occupation.Fighter);

        Assert.AreEqual("Aria", player.Name);
        Assert.AreEqual(Race.Elf, player.Race);
        Assert.AreEqual(Occupation.Fighter, player.Occupation);
        Assert.AreEqual(Weapon.LongSword, player.Weapon);
        Assert.IsFalse(player.HasRolledStrength);
        Assert.IsFalse(player.HasRolledAgility);
        Assert.IsFalse(player.HasRolledHealth);
        Assert.IsFalse(player.HasRolledStats);
    }

    //Verifies formatting for characrter name.
    [TestMethod]
    public void PlayerConstructor_TrimsName()
    {
        Player player = new Player(
            "  Aria  ",
            Race.Elf,
            Occupation.Fighter);

        Assert.AreEqual("Aria", player.Name);
    }

    //Verifies if a player does not add a name it throws an Exception.
    [TestMethod]
    public void PlayerConstructor_RejectsBlankName()
    {
        Assert.Throws<ArgumentException>(
            () => new Player(
                "   ",
                Race.Human,
                Occupation.Fighter));
    }

    //Verifies the parameterless constructor from the class diagram works
    //and starts with the Fighter's weapon (the default occupation).
    [TestMethod]
    public void PlayerParameterlessConstructor_HasDefaultWeapon()
    {
        Player player = new Player();

        Assert.IsNotNull(player.Weapon);
        Assert.AreEqual(Weapon.LongSword, player.Weapon);
        Assert.IsFalse(player.HasRolledStats);
    }

    //Verifies setting Occupation also assigns the matching weapon.
    [TestMethod]
    public void PlayerOccupationSetter_UpdatesWeapon()
    {
        Player player = new Player();

        player.Occupation = Occupation.Archer;
        Assert.AreEqual(Weapon.LongBow, player.Weapon);

        player.Occupation = Occupation.Thief;
        Assert.AreEqual(Weapon.Dagger, player.Weapon);
    }

    //Verifies the Name setter also rejects blank names.
    [TestMethod]
    public void PlayerNameSetter_RejectsBlankName()
    {
        Player player = new Player();

        Assert.Throws<ArgumentException>(
            () => player.Name = "   ");
    }

    // ---------------------------------------------------------------
    // Strength
    // ---------------------------------------------------------------

    //Verifies strength is generated, flagged, and within the expected range.
    //Human: d20 (1-20) + d4 (1-4) = 2-24.
    [TestMethod]
    public void RollStrength_Human_GeneratesValidValue()
    {
        for (int i = 0; i < 50; i++)
        {
            Player player = new Player(
                "Aria",
                Race.Human,
                Occupation.Fighter);

            player.RollStrength();

            Assert.IsTrue(player.HasRolledStrength);
            Assert.IsTrue(player.StrengthModifier >= 1 && player.StrengthModifier <= 4);
            Assert.IsTrue(player.Strength >= 2 && player.Strength <= 24);

            // Score minus the modifier must be a valid base d20 roll.
            int baseStrength = player.Strength - player.StrengthModifier;
            Assert.IsTrue(baseStrength >= 1 && baseStrength <= 20);
        }
    }

    //Verifies Halfling strength SUBTRACTS the 1-4 modifier.
    //Halfling: d20 (1-20) - d4 (1-4) = -3 to 19 (no minimum applied).
    [TestMethod]
    public void RollStrength_Halfling_SubtractsModifier()
    {
        for (int i = 0; i < 50; i++)
        {
            Player player = new Player(
                "Pip",
                Race.Halfling,
                Occupation.Thief);

            player.RollStrength();

            Assert.IsTrue(player.StrengthModifier >= 1 && player.StrengthModifier <= 4);
            Assert.IsTrue(player.Strength >= -3 && player.Strength <= 19);

            // Score plus the modifier must be a valid base d20 roll.
            int baseStrength = player.Strength + player.StrengthModifier;
            Assert.IsTrue(baseStrength >= 1 && baseStrength <= 20);
        }
    }

    //Does not allow strength to be rolled again.
    [TestMethod]
    public void RollStrength_CannotBeCalledTwice()
    {
        Player player = new Player(
            "Aria",
            Race.Human,
            Occupation.Fighter);

        player.RollStrength();

        Assert.Throws<InvalidOperationException>(
            () => player.RollStrength());
    }

    // ---------------------------------------------------------------
    // Agility
    // ---------------------------------------------------------------

    //Verifies agility is generated, flagged, and within the expected range.
    [TestMethod]
    public void RollAgility_GeneratesValidValue()
    {
        Player player = new Player(
            "Aria",
            Race.Human,
            Occupation.Fighter);

        player.RollAgility();

        Assert.IsTrue(player.HasRolledAgility);
        Assert.IsTrue(player.Agility >= 1 && player.Agility <= 20);
    }

    //Does not allow agility to be rolled again.
    [TestMethod]
    public void RollAgility_CannotBeCalledTwice()
    {
        Player player = new Player(
            "Aria",
            Race.Human,
            Occupation.Fighter);

        player.RollAgility();

        Assert.Throws<InvalidOperationException>(
            () => player.RollAgility());
    }

    // Verifies that Elves and Halflings receive an agility modifier
    // and that the final agility score is consistent with that modifier.
    [TestMethod]
    [DataRow(Race.Elf)]
    [DataRow(Race.Halfling)]
    public void RollAgility_ElfAndHalflingApplyAgilityModifier(
        Race race)
    {
        for (int i = 0; i < 50; i++)
        {
            Player player = new Player(
                "Aria",
                race,
                Occupation.Fighter);

            player.RollAgility();

            // The agility modifier must be between 1 and 4.
            Assert.IsTrue(
                player.AgilityModifier >= 1 &&
                player.AgilityModifier <= 4,
                "Agility modifier should be between 1 and 4.");

            // Both races receive a positive agility adjustment.
            Assert.IsTrue(
                player.Agility >= 2 &&
                player.Agility <= 24,
                "Agility should include the racial adjustment.");

            // The score minus its modifier should correspond
            // to a valid base d20 roll.
            int baseAgility =
                player.Agility - player.AgilityModifier;

            Assert.IsTrue(
                baseAgility >= 1 && baseAgility <= 20,
                "Base agility should be between 1 and 20.");
        }
    }

    // Verifies Humans and Dwarves get no agility modifier.
    [TestMethod]
    [DataRow(Race.Human)]
    [DataRow(Race.Dwarf)]
    public void RollAgility_HumanAndDwarfHaveNoModifier(Race race)
    {
        for (int i = 0; i < 50; i++)
        {
            Player player = new Player(
                "Aria",
                race,
                Occupation.Fighter);

            player.RollAgility();

            Assert.AreEqual(0, player.AgilityModifier);
            Assert.IsTrue(player.Agility >= 1 && player.Agility <= 20);
        }
    }

    // ---------------------------------------------------------------
    // Health
    // ---------------------------------------------------------------

    //Health depends on the strength modifier, so strength must be rolled first.
    [TestMethod]
    public void RollHealth_RequiresStrengthFirst()
    {
        Player player = new Player(
            "Aria",
            Race.Human,
            Occupation.Fighter);

        Assert.Throws<InvalidOperationException>(
            () => player.RollHealth());
    }

    //Does not allow health to be rolled again.
    [TestMethod]
    public void RollHealth_CannotBeCalledTwice()
    {
        Player player = new Player(
            "Aria",
            Race.Human,
            Occupation.Fighter);

        player.RollStrength();
        player.RollHealth();

        Assert.Throws<InvalidOperationException>(
            () => player.RollHealth());
    }

    //Human, Elf and Dwarf ADD the strength modifier to their health roll.
    //Health = d20 (1-20) + modifier (1-4) = 2-24.
    [TestMethod]
    [DataRow(Race.Human)]
    [DataRow(Race.Elf)]
    [DataRow(Race.Dwarf)]
    public void RollHealth_HumanElfDwarf_AddStrengthModifier(Race race)
    {
        for (int i = 0; i < 50; i++)
        {
            Player player = new Player(
                "Aria",
                race,
                Occupation.Fighter);

            player.RollStrength();
            player.RollHealth();

            Assert.IsTrue(player.HasRolledHealth);
            Assert.IsTrue(player.Health >= 2 && player.Health <= 24);

            // Health minus the strength modifier must be a valid base d20 roll.
            int baseHealth = player.Health - player.StrengthModifier;
            Assert.IsTrue(baseHealth >= 1 && baseHealth <= 20);
        }
    }

    //Halfling health gets NO strength modifier (neither added nor subtracted).
    //Health = plain d20 roll = 1-20. If the modifier were subtracted,
    //health could drop to 0 or below.
    [TestMethod]
    public void RollHealth_Halfling_GetsNoModifier()
    {
        for (int i = 0; i < 200; i++)
        {
            Player player = new Player(
                "Pip",
                Race.Halfling,
                Occupation.Thief);

            player.RollStrength();
            player.RollHealth();

            Assert.IsTrue(
                player.Health >= 1 && player.Health <= 20,
                "Halfling health should be a plain d20 roll (1-20).");
        }
    }

    // ---------------------------------------------------------------
    // Full set of rolls
    // ---------------------------------------------------------------

    //HasRolledStats is only true once strength, agility and health are all rolled.
    [TestMethod]
    public void HasRolledStats_TrueOnlyAfterAllThreeRolls()
    {
        Player player = new Player(
            "Aria",
            Race.Human,
            Occupation.Fighter);

        Assert.IsFalse(player.HasRolledStats);

        player.RollStrength();
        Assert.IsFalse(player.HasRolledStats);

        player.RollAgility();
        Assert.IsFalse(player.HasRolledStats);

        player.RollHealth();
        Assert.IsTrue(player.HasRolledStats);
    }

    // ---------------------------------------------------------------
    // DisplayStats
    // ---------------------------------------------------------------

    //Before all stats are rolled, no summary is shown.
    [TestMethod]
    public void DisplayStats_BeforeRolling_DoesNotShowSummary()
    {
        Player player = new Player(
            "Aria",
            Race.Elf,
            Occupation.Fighter);

        string output = player.DisplayStats();

        StringAssert.Contains(output, "not been fully rolled");
        Assert.IsFalse(output.Contains("Aria"));
    }

    //Verifies that all eight summary items from the appendix are displayed.
    [TestMethod]
    public void DisplayStats_IncludesCharacterInformation()
    {
        Player player = CreateRolledPlayer(Race.Elf, Occupation.Fighter);

        string output = player.DisplayStats();

        StringAssert.Contains(output, "Name: Aria");
        StringAssert.Contains(output, "Race: Elf");
        StringAssert.Contains(output, "Occupation: Fighter");
        StringAssert.Contains(output, $"Strength: {player.Strength}");
        StringAssert.Contains(output, $"Agility: {player.Agility}");
        StringAssert.Contains(output, $"Health Points: {player.Health}");
        StringAssert.Contains(output, "Weapon: long sword");
        StringAssert.Contains(output, "Weapon's Maximum Damage Points: 12");
    }

    // ---------------------------------------------------------------
    // RaceRules
    // ---------------------------------------------------------------

    //Verifies the specific modifier for Halfling is properly applied.
    [TestMethod]
    public void HalflingStrengthAdjustment_IsNegativeD4()
    {
        RaceRules rules = new RaceRules();

        for (int i = 0; i < 50; i++)
        {
            int result = rules.RollStrengthModifier(Race.Halfling);

            Assert.IsTrue(result >= -4 && result <= -1);
        }
    }

    //Verified modifier for Elf is applied.
    [TestMethod]
    public void ElfAgilityAdjustment_IsD4()
    {
        RaceRules rules = new RaceRules();

        for (int i = 0; i < 50; i++)
        {
            int result = rules.RollAgilityModifier(Race.Elf);

            Assert.IsTrue(result >= 1 && result <= 4);
        }
    }

    //Verifies no modifier is applied to the correct races.
    [TestMethod]
    public void HumanAndDwarfAgilityAdjustment_IsZero()
    {
        RaceRules rules = new RaceRules();

        Assert.AreEqual(
            0,
            rules.RollAgilityModifier(Race.Human));

        Assert.AreEqual(
            0,
            rules.RollAgilityModifier(Race.Dwarf));
    }

    // ---------------------------------------------------------------
    // Die
    // ---------------------------------------------------------------

    //Verifies Die without an argument defaults to 20.
    [TestMethod]
    public void Die_DefaultConstructorUsesTwentySides()
    {
        Die die = new Die();

        Assert.AreEqual(20, die.Sides);
    }

    //Ensures that values rolled between 1-4 stay between that range.
    [TestMethod]
    public void DieFourSided_ReturnsValuesBetweenOneAndFour()
    {
        Die die = new Die(4);

        for (int i = 0; i < 100; i++)
        {
            int result = die.Roll();

            Assert.IsTrue(result >= 1 && result <= 4);
        }
    }

    //Similar to before test but with values 1-20
    [TestMethod]
    public void DieTwentySided_ReturnsValuesBetweenOneAndTwenty()
    {
        Die die = new Die(20);

        for (int i = 0; i < 100; i++)
        {
            int result = die.Roll();

            Assert.IsTrue(result >= 1 && result <= 20);
        }
    }

    //Ensures Die cannot have 0 sides.
    [TestMethod]
    public void Die_RejectsZeroSides()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Die(0));
    }
}

//Verifies each occupation recieves the correct information
[TestClass]
public class OccupationTests
{
    // Tests the weapon assigned to each occupation.
    // Each DataRow supplies a different occupation and its
    // expected weapon properties
    [TestMethod]
    [DataRow(Occupation.Fighter, "long sword", 12, "-)=====>")]
    [DataRow(Occupation.Magician, "lightning bolt spell", 12, "zap~~~~~~")]
    [DataRow(Occupation.Thief, "dagger", 6, "-)==>")]
    [DataRow(Occupation.Archer, "long bow", 8, "}    -->")]
    public void GetWeapon_ReturnsCorrectWeapon(
        Occupation occupation,
        string expectedType,
        int expectedDamage,
        string expectedIcon)
    {
        OccupationRules rules = new OccupationRules();

        // Retrieve the weapon associated with the selected occupation.
        Weapon weapon = rules.GetWeapon(occupation);

        //Verifies weapon types match the expected values.
        Assert.AreEqual(expectedType, weapon.type);
        Assert.AreEqual(expectedDamage, weapon.maxDamage);
        Assert.AreEqual(expectedIcon, weapon.asciiArt);
    }
}