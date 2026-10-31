using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class MessagesTests
{
    private const string SampleJson = """
    {
      "intro": { "en": "Welcome", "fr": "Bienvenue" },
      "dragon_taunt": { "en": ["Taunt one", "Taunt two"], "fr": ["Un", "Deux"] }
    }
    """;

    private readonly List<string> tempFiles = new();

    // Writes JSON to a temporary file and returns its path
    private string WriteTempJson(string json)
    {
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        File.WriteAllText(path, json);
        tempFiles.Add(path);
        return path;
    }

    [TestCleanup]
    public void DeleteTempFiles()
    {
        foreach (string path in tempFiles)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }

    [TestMethod]
    public void ReadDictionary_LoadsTheChosenLanguage()
    {
        // Arrange
        Messages messages = new();
        string path = WriteTempJson(SampleJson);

        // Act and Assert (English, then French loaded afterward)
        messages.ReadDictionary(path, "en");
        Assert.AreEqual("Welcome", messages.GetMessage("intro"));

        messages.ReadDictionary(path, "fr");
        Assert.AreEqual("Bienvenue", messages.GetMessage("intro"));
    }

    [TestMethod]
    public void GetMessage_ReturnsOneOfTheLines_ForAListMessage()
    {
        // Arrange
        Messages messages = new();
        messages.ReadDictionary(WriteTempJson(SampleJson), "en");
        string[] options = { "Taunt one", "Taunt two" };

        // Act and Assert (run several times because the choice is random)
        for (int i = 0; i < 20; i++)
        {
            CollectionAssert.Contains(options, messages.GetMessage("dragon_taunt"));
        }
    }

    [TestMethod]
    public void GetMessage_ReturnsPlaceholderText_ForMissingMessageName()
    {
        // Arrange
        Messages messages = new();
        messages.ReadDictionary(WriteTempJson(SampleJson), "en");

        // Act
        string result = messages.GetMessage("nothing_here");

        // Assert
        Assert.AreEqual("[Missing message nothing_here]", result);
    }

    [TestMethod]
    public void ReadDictionary_ThrowsFileNotFound_ForMissingFile()
    {
        // Arrange
        Messages messages = new();
        string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");

        // Act and Assert
        Assert.ThrowsExactly<FileNotFoundException>(() => messages.ReadDictionary(path, "en"));
    }

    [TestMethod]
    public void ReadDictionary_ThrowsFormatException_ForInvalidJson()
    {
        // Arrange
        Messages messages = new();
        string path = WriteTempJson("{ this is not json");

        // Act and Assert
        Assert.ThrowsExactly<FormatException>(() => messages.ReadDictionary(path, "en"));
    }

    [TestMethod]
    public void ReadDictionary_ThrowsFormatException_WhenLanguageIsMissing()
    {
        // Arrange (the file has no Spanish)
        Messages messages = new();
        string path = WriteTempJson(SampleJson);

        // Act and Assert
        Assert.ThrowsExactly<FormatException>(() => messages.ReadDictionary(path, "es"));
    }

    [TestMethod]
    public void ReadDictionary_ThrowsFormatException_WhenMessageIsNotTextOrList()
    {
        // Arrange (5 is a number, not text)
        Messages messages = new();
        string path = WriteTempJson("""{ "intro": { "en": 5 } }""");

        // Act and Assert
        Assert.ThrowsExactly<FormatException>(() => messages.ReadDictionary(path, "en"));
    }

    [TestMethod]
    public void ReadDictionary_KeepsOldMessages_WhenNewFileIsInvalid()
    {
        // Arrange
        Messages messages = new();
        messages.ReadDictionary(WriteTempJson(SampleJson), "en");
        string badPath = WriteTempJson("{ this is not json");

        // Act
        Assert.ThrowsExactly<FormatException>(() => messages.ReadDictionary(badPath, "en"));

        // Assert
        Assert.AreEqual("Welcome", messages.GetMessage("intro"));
    }
}