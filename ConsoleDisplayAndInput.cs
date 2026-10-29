using System.Reflection.Metadata.Ecma335;

public static class ConsoleDisplayAndInput
{
    public static void PrintStoryGreen(string text)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(text);
    }

    public static void PrintDragonRed(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        // Returns text color to green after dragon dialogue is finished
        Console.ForegroundColor = ConsoleColor.Green;
    }

    // Min and max represent the lowest and highest options numbers on the menu 
    // (e.g., 1-4 representing each race choice)
    public static int AskForChoice(int min, int max)
    {

        int number;
        bool isNumber;
   
        do
        {
            string? input = Console.ReadLine();
            // Tries to convert the answer into a number
            // isNumber returns true if answer can be converted into a number, and false if the answer is text or null (e.g, 'abc' would return false)
            isNumber = int.TryParse(input, out number);

        // Player is asked again if their answer is not an int, or does not fall on or between max or min
        } while (!isNumber || number < min || number > max);

        return number;
    }

    public static char AskForLetter(string allowedLetters)
    {

       string? input;

        do
        {
            // Player answer is made lowercase so it is comparable to lowercase allowedLetters (skipped if the answer is null)
            input = Console.ReadLine()?.ToLower();
            

        // Player is asked again if their answer is null, is not exactly one character, or is not one of allowedLetters
        } while(input is null || input.Length != 1 || !allowedLetters.Contains(input));

        // input is one letter, so that character is returned
        return input[0];
    }

    public static string AskForName()
    {
        string? input;

        do
        {
            // White-space characters are removed from both ends of answer (skipped if answer is null)
            input = Console.ReadLine()?.Trim();
            
        // Player is asked again if their answer is null, an empty string, or made up of white-space characters
        } while(string.IsNullOrWhiteSpace(input));

        // input is non-blank and trimmed
        return input;
    }

    public static void WaitForRoll()
    {
        string? input;
        do
        {
            // White-space characters are removed from both ends of answer 
            // And answer is made lowercase so "roll", "Roll", and "ROLL" are all accepted (skipped if answer is null)
            input = Console.ReadLine()?.Trim().ToLower();
        
         // Player is asked again if answer is not "roll"
        } while(input != "roll");
    }

}