using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class D20Tests
{
    [TestMethod]
    public void Sides_DefaultValue_IsTwenty()
    {
        // Arrange
        var die = new D20();
        // Act
        int sides = die.Sides;
        // Assert
        Assert.AreEqual(20, sides);
    }

    [TestMethod]
    public void Roll_ReturnsValueWithinOneAndTwenty()
    {
        // Arrange
        var die = new D20();
        // Act & Assert
        // Run multiple rolls to verify bounds across a sample size
        for (int i = 0; i < 500; i++)
        {
            int result = die.Roll();
            Assert.IsTrue(result >= 1, $"Roll resulted in {result}, which is less than 1.");
            Assert.IsTrue(result <= 20, $"Roll resulted in {result}, which is greater than 20.");
        }
    }
}