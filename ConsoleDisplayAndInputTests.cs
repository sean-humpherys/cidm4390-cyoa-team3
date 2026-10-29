using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ConsoleDisplayAndInputTests
{


    [TestMethod]
    public void AskForChoice_ReturnsNumber_AfterInvalidInputs()
    {
        // Arrange: Simulates player entering an empty value, a value with a white-space, "abc", "9", and "2"
        Console.SetIn(new StringReader("\n \nabc\n9\n2\n"));
        // Act: Calls AskForChoice(int min, int max)
        int result = ConsoleDisplayAndInput.AskForChoice(1,4);

        // Assert: Ensures invalid values were skipped, and 2 was returned
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void AskForLetter_ReturnsLetter_AfterInvalidInputs()
    {
        // Arrange: Simulates player entering an empty value, a value with a white-space, "abc", "x", and "N"
        Console.SetIn(new StringReader("\n \nabc\nx\nN\n"));

        // Act: Calls AskForLetter(string allowedLetters)
        char result = ConsoleDisplayAndInput.AskForLetter("nse");

        // Assert: Ensures invalid values were skipped, and  "N" came back as the lowercase 'n'
        Assert.AreEqual('n', result);
    }

    [TestMethod]
    public void AskForName_ReturnsText_AfterInvalidInputs()
    {
        // Arrange: Simulates player entering an empty value, a value with a white-space, and "Hannah" text
        Console.SetIn(new StringReader("\n \nHannah\n"));

        // Act: Calls AskForName()
        string result = ConsoleDisplayAndInput.AskForName();

        // Assert: Ensures invalid values were skipped, and "Hannah" was returned
        Assert.AreEqual("Hannah", result);
    }

    [TestMethod]
    public void WaitForRoll_Finishes_AfterInvalidInputsAndDifferentCapitalizations()
    {
        // Arrange: Simulates player entering an empty value, a value with a white-space, "ROLL", and "extra"
        StringReader reader1 = new StringReader("\n \nROLL\nextra\n");
        Console.SetIn(reader1);
        // Act: Calls WaitForRoll()
        ConsoleDisplayAndInput.WaitForRoll();
         // Assert: Ensures invalid values were skipped and that the method stopped correctly
        Assert.AreEqual("extra", reader1.ReadLine());

        // Arrange: Simulates player entering an empty value, a value with a white-space, "Roll", and "extra"
        StringReader reader2 = new StringReader("\n \nRoll\nextra\n");
        Console.SetIn(reader2);
        // Act: Calls WaitForRoll()
        ConsoleDisplayAndInput.WaitForRoll();
         // Assert: Ensures invalid values were skipped and that the method stopped correctly
        Assert.AreEqual("extra", reader2.ReadLine());

        // Arrange: Simulates player entering an empty value, a value with a white-space, "roll", and "extra"
        StringReader reader3 = new StringReader("\n \nroll\nextra\n");
        Console.SetIn(reader3);
        // Act: Calls WaitForRoll()
        ConsoleDisplayAndInput.WaitForRoll();
         // Assert: Ensures invalid values were skipped and that the method stopped correctly
        Assert.AreEqual("extra", reader3.ReadLine());

    }
}
