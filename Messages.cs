using System.Text.Json;

public class Messages
{
    // Dictionary holds the messages for one language (e.g.,key = message name, value = list of lines) 
    // (most messages have one line, taunts have several)
    private Dictionary<string, List<string>> languageDictionary = new();

    // Picks a random line when a message has several (e.g., taunts)
    private readonly Random random = new();

    // Loads one language ("en", "fr" or "es") from the JSON file
    public void ReadDictionary(string languageFilePath, string languageCode)
    {
        // Stops early with an error if the JSON file is not found
        if (!File.Exists(languageFilePath))
        {
            throw new FileNotFoundException($"Language file not found: {languageFilePath}", languageFilePath);
        }

        // Temporary dictionary so old messages are kept if this file is bad
        Dictionary<string, List<string>> loaded = new();

        try
        {
            // Reads the JSON file
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(languageFilePath));

            // Loop reads through every message in the file
            foreach (JsonProperty message in document.RootElement.EnumerateObject())
            {
                // Finds the chosen language inside the message (e.g, finding "en" inside of "intro")
                if (!message.Value.TryGetProperty(languageCode, out JsonElement text))
                {
                    throw new FormatException($"Message \"{message.Name}\" has no \"{languageCode}\" text in {languageFilePath}");
                }

                // Empty list for the message
                List<string> lines = new();

                // A message is either one text or a list of texts
                if (text.ValueKind == JsonValueKind.String)
                {
                    lines.Add(text.GetString()!);
                }
                else if (text.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement item in text.EnumerateArray())
                    {
                        lines.Add(item.GetString()!);
                    }
                }

                // Rejects anything that is not text or a list of texts
                if (lines.Count == 0)
                {
                    throw new FormatException($"Message \"{message.Name}\" ({languageCode}) must be text or a list of text.");
                }

                // Each message's name (key) and its lines (value) are paired and saved in the temporary dictionary
                loaded[message.Name] = lines;
            }
        }
        catch (Exception ex) when (ex is JsonException || ex is InvalidOperationException)
        {
            // Bad JSON or the wrong structure in the file
            throw new FormatException($"Language file is not valid (expected message name -> language -> text): {languageFilePath}. {ex.Message}", ex);
        }

        // Only replaces the old messages once the whole file loaded without errors
        languageDictionary = loaded;
    }

    // Takes the message name (the key) and returns its lines (the value)
    public string GetMessage(string key)
    {
        // If the message name is found, it returns one of its lines (random when its several)
        if (languageDictionary.TryGetValue(key, out List<string>? lines))
        {
            return lines[random.Next(lines.Count)];
        }
        // If the message name is not found, a placeholder returns for that name
        else
        {
            return $"[Missing message {key}]";
        }
    }
}