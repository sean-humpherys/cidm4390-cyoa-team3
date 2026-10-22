using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class DieTests
{
    [TestMethod]
    public void Sides_DefaultValue_IsTwenty()
    {
        // Arrange
        var die = new Die();

        // Act
        int sides = die.Sides;

        // Assert
        Assert.AreEqual(20, sides);
    }

    [TestMethod]
    public void Sides_CustomValue_SetsCorrectly()
    {
        // Arrange
        var die = new Die(6);

        // Act
        int sides = die.Sides;

        // Assert
        Assert.AreEqual(6, sides);
    }

    [TestMethod]
    public void Roll_ReturnsValueWithinOneAndSides()
    {
        // Arrange
        var die = new Die(6);

        // Act & Assert
        for (int i = 0; i < 500; i++)
        {
            int result = die.Roll();
            Assert.IsTrue(result >= 1, $"Roll resulted in {result}, which is less than 1.");
            Assert.IsTrue(result <= 6, $"Roll resulted in {result}, which is greater than 6.");
        }
    }
}