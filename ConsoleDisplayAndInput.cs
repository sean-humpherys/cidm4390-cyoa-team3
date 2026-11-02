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
    public static int AskForChoice(int min, int max, string invalidMessage = "", string prompt = "")
    {

        int number;
        bool isNumber;
        bool isValid;
   
        do
        {
            string? input = Console.ReadLine();
            // Tries to convert the answer into a number
            // isNumber returns true if answer can be converted into a number, and false if the answer is text or null (e.g, 'abc' would return false)
            isNumber = int.TryParse(input, out number);

            // Answer is valid only if it is a number that falls on or between min and max values
            isValid = isNumber && number >= min && number <= max;

            // Displays error message and asks question again if answer is invalid
            if (!isValid)
            {
                if (invalidMessage != "")
                {
                    PrintStoryGreen(invalidMessage);
                }

                if (prompt != "")
                {
                    PrintStoryGreen("\n" + prompt);
                    Console.Write("> ");
                }
            }

        // Player is asked again if their answer is not valid
        } while (!isValid);

        return number;
    }

    public static char AskForLetter(string allowedLetters, string invalidMessage = "", string prompt = "")
    {

       string? input;
       bool isValid;

        do
        {
            // Player answer is made lowercase so it is comparable to lowercase allowedLetters (skipped if the answer is null)
            input = Console.ReadLine()?.ToLower();
            // Answer is valid if input is not null, input is exactly one character, and it is one of the allowed letters
            isValid = input is not null && input.Length == 1 && allowedLetters.Contains(input);

            // Displays error message and asks question again if answer is invalid
            if (!isValid)
            {
                 if (invalidMessage != "")
                {
                    PrintStoryGreen(invalidMessage);
                }

                if (prompt != "")
                {
                    PrintStoryGreen("\n" + prompt);
                    Console.Write("> ");
                }
            }
            

         // Player is asked again if their answer is not valid
        } while(!isValid);

        // input is one letter, so that character is returned
        return input![0];
    }

    public static string AskForName(string invalidMessage = "", string prompt = "")
    {
        string? input;
        bool isValid;

        do
        {
            // White-space characters are removed from both ends of answer (skipped if answer is null)
            input = Console.ReadLine()?.Trim();
            // Answer is valid if answer is not null, an empty string, made up of white-space characters, and has any numeric characters
            isValid = !string.IsNullOrWhiteSpace(input) && !input.Any(char.IsDigit);


            // Displays error message and asks question again if answer is invalid
            if (!isValid)
            {
                 if (invalidMessage != "")
                {
                    PrintStoryGreen(invalidMessage);
                }

                if (prompt != "")
                {
                    PrintStoryGreen("\n" + prompt);
                    Console.Write("> ");
                }
            }
            
        // Player is asked again if their answer is not valid
        } while(!isValid);

        // input is non-blank and trimmed
        return input!;
    }

    public static void WaitForRoll(string invalidMessage = "", string prompt = "")
    {
        string? input;
        bool isValid;
        do
        {
            // White-space characters are removed from both ends of answer 
            // And answer is made lowercase so "roll", "Roll", and "ROLL" are all accepted (skipped if answer is null)
            input = Console.ReadLine()?.Trim().ToLower();

            // Answer is valid if input is any variation of "roll"
            isValid = input == "roll";

             // Displays error message and asks question again if answer is invalid
            if (!isValid)
            {
                 if (invalidMessage != "")
                {
                    PrintStoryGreen(invalidMessage);
                }

                if (prompt != "")
                {
                    PrintStoryGreen("\n" + prompt);
                    Console.Write("> ");
                }
            }

    
         // Player is asked again if answer is not "roll"
        } while(!isValid);
    }

}