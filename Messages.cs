public class Messages
{
    private string currentLanguage= "";
    private Dictionary<int,string> languageDictionary = new Dictionary<int, string>();


    public void ReadDictionary(string languageFilePath)
    {
        // Clears dictionary contents in case another language file is loaded later
        languageDictionary.Clear();
        // Foreach loop reads through every line in the file
        foreach (string line in File.ReadAllLines(languageFilePath))
        {
            // Split cuts a line at the first "=" and creates two pieces 
            // (parts[0] is the int, parts[1] is the text)
            string[] parts = line.Split('=', 2);

            languageDictionary.Add(int.Parse(parts[0]), parts[1]);
        }

    }

    public string GetMessage(int whichMessage)
    {
        // Dictionary takes int input (the key) and returns the matching text (the value)
        return languageDictionary[whichMessage];
    
    }
}