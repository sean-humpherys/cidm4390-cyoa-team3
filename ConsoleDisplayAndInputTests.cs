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
        // Arrange: Simulates player entering an empty value, a value with a white-space, a value with a numeric value, a sole numeric value, and "Hannah" text
        Console.SetIn(new StringReader("\n \nOlaf2\n23\nHannah\n"));

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

    [TestMethod]
    public void AskForChoice_ShowsErrorAndAsksAgain_AfterInvalidInput()
    {
        // Arrange: Simulates player entering a "9", then "2", and captures what is printed
        Console.SetIn(new StringReader("9\n2\n"));
        StringWriter output = new();
        Console.SetOut(output);

        // Act: Calls AskForChoice() with error message and question
        int result = ConsoleDisplayAndInput.AskForChoice(1, 4, "Invalid choice", "Pick a number");

        // Assert: Ensures 2 was returned and the error and question were printed
        Assert.AreEqual(2, result);
        StringAssert.Contains(output.ToString(), "Invalid choice");
        StringAssert.Contains(output.ToString(), "Pick a number");

    }


    [TestMethod]
    public void AskForLetter_ShowsErrorAndAsksAgain_AfterInvalidInput()
    {
        // Arrange: Simulates player entering "x" then "n" and captures what is printed
         Console.SetIn(new StringReader("x\nn\n"));
         StringWriter output = new();
         Console.SetOut(output);

        // Act: Calls AskForLetter() with error message and question
         char result = ConsoleDisplayAndInput.AskForLetter("nse", "Invalid letter", "Which way?");

        // Assert: Ensures "n" was returned and the error and question were printed
         Assert.AreEqual('n', result);
         StringAssert.Contains(output.ToString(), "Invalid letter");
         StringAssert.Contains(output.ToString(), "Which way?");
    }
    
    [TestMethod]
    public void AskForName_ShowsErrorAndAsksAgain_AfterNameWithInputs()
    {
        // Arrange: Simulates player entering "Olaf2" then "Hannah", and captures what is printed
         Console.SetIn(new StringReader("Olaf2\nHannah\n"));
         StringWriter output = new();
         Console.SetOut(output);

        // Act: Calls AskForName() with error message and question
         string result = ConsoleDisplayAndInput.AskForName("Invalid name", "What is your name?");

        // Assert: Ensures "Hannah" was returned and the error and question were printed
         Assert.AreEqual("Hannah", result);
         StringAssert.Contains(output.ToString(), "Invalid name");
    }

    [TestMethod]
    public void AskForName_ShowsErrorAndAsksAgain_AfterBlankName()
    {
        // Arrange: Simulates player entering only white-spaces, then "Hannah", then captures what is printed
        Console.SetIn(new StringReader("   \nHannah\n"));
        StringWriter output = new();
        Console.SetOut(output);

        // Act: Calls AskForName() with error message and question
        string result = ConsoleDisplayAndInput.AskForName("Invalid name", "What is your name?");

        // Assert: Ensures "Hannah" was returned and the error was printed
        Assert.AreEqual("Hannah", result);
        StringAssert.Contains(output.ToString(), "Invalid name");
    }

    public void WaitForRoll_ShowsErrorAndAsksAgain_AfterInvalidInput()
    {
        // Arrange: Simulates player entering "extra", "roll", and "jump", and captures what is printed
        StringReader reader1 = new StringReader("extra\nroll\njump\n");
        Console.SetIn(reader1);
        StringWriter output = new();
        Console.SetOut(output);

        // Act: Calls WaitForRoll() with error message and question
        ConsoleDisplayAndInput.WaitForRoll("Please type roll", "Type roll");


        // Assert: Ensures the method stopped at "roll" and the error and question were printed
        Assert.AreEqual("extra", reader1.ReadLine());
        StringAssert.Contains(output.ToString(), "Please type roll");
        StringAssert.Contains(output.ToString(), "Type roll");
    }

    [TestMethod]
    public void AskForChoice_PrintsNothing_WhenNoMessagesAreGiven()
    {
        // Arrange: Simulates player entering a "9", then "2", and captures what is printed
        Console.SetIn(new StringReader("9\n2\n"));
        StringWriter output = new();
        Console.SetOut(output);

        // Act: Calls AskForChoice() without error message or question
        ConsoleDisplayAndInput.AskForChoice(1,4);

        // Assert: Ensures nothing was printed
        Assert.AreEqual("", output.ToString());
    }

}
