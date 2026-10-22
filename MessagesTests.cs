using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class MessagesTests
{
    [TestMethod]
    public void GetMessage_ReturnsHello_ForKey1()
    {
        // Arrange
        Messages messages = new Messages();

        // Act
        messages.ReadDictionary("Sample.txt");

        // Assert
        Assert.AreEqual("Hello", messages.GetMessage(1));
    }

    [TestMethod]
    public void GetMessage_ReturnsGoodbye_ForKey2()
    {
        // Arrange
        Messages messages = new Messages();

        // Act
        messages.ReadDictionary("Sample.txt");

        // Assert
        Assert.AreEqual("Goodbye", messages.GetMessage(2));
    }
}