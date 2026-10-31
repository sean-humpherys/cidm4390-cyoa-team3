using System;

// ======================================================================
// Create Character Activity (per Appendix C and the Create Character diagram)
// Order: race -> name -> occupation -> roll strength -> roll agility
//        -> roll health -> menu (1. list stats / 2. proceed on adventure)
// ======================================================================

// ---------- Race ----------
Race race = AskForEnum<Race>(
    "What race of character do you want to be? Human, Elf, Dwarf or Halfling?");

// ---------- Name ----------
string name;
do
{
    Console.WriteLine("\nWhat is the name of your character?");
    name = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Please enter a name.");
    }
}
while (string.IsNullOrWhiteSpace(name));

// ---------- Occupation (this also sets the weapon) ----------
Occupation occupation = AskForEnum<Occupation>(
    "What is the occupation of your character? Fighter, magician, thief, or archer?");

Player player = new Player(name, race, occupation);

// ---------- Roll for strength, agility, then health ----------
WaitForRoll("Next, we are going to roll for strength. Type 'roll'");
player.RollStrength();

WaitForRoll("Next, we will roll for agility. Type 'roll'");
player.RollAgility();

// Health must come after strength because it uses the strength modifier.
WaitForRoll("Next, we will roll for health points. Type 'roll'");
player.RollHealth();

// ---------- Post-creation menu ----------
bool creatingCharacter = true;

while (creatingCharacter)
{
    Console.WriteLine(
        "\nWhat would you like to do next? " +
        "1. List character stats. 2. Proceed on an adventure. Type 1 or 2");

    string choice = (Console.ReadLine() ?? "").Trim();

    switch (choice)
    {
        case "1":
            Console.WriteLine();
            Console.WriteLine(player.DisplayStats());
            break;

        case "2":
            // TODO: hand off to the Start Adventure activity (Game class).
            // Placeholder: show the main adventure menu from Appendix D.
            Console.WriteLine(
                "\nYou are walking in the mountains. You see two paths, " +
                "one leading north and higher into the mountains. " +
                "The other path leads south and into the woods. " +
                "Which do you take? Enter 'n' for North. Enter 's' for South. " +
                "Enter 'e' for exit game.");
            creatingCharacter = false;
            break;

        default:
            Console.WriteLine("Please type 1 or 2.");
            break;
    }
}

// ======================================================================
// Helper methods
// ======================================================================

// Shows the prompt and waits until the player types 'roll'.
static void WaitForRoll(string prompt)
{
    while (true)
    {
        Console.WriteLine($"\n{prompt}");

        string input = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

        if (input == "roll")
        {
            return;
        }

        Console.WriteLine("Please type 'roll'.");
    }
}

// Asks until the player types the name of one of the enum values
// (case-insensitive). Numbers such as "1" are not accepted.
static T AskForEnum<T>(string prompt)
    where T : struct, Enum
{
    while (true)
    {
        Console.WriteLine($"\n{prompt}");

        string input = (Console.ReadLine() ?? "").Trim();

        foreach (string enumName in Enum.GetNames(typeof(T)))
        {
            if (string.Equals(enumName, input, StringComparison.OrdinalIgnoreCase))
            {
                return Enum.Parse<T>(enumName);
            }
        }

        Console.WriteLine("That choice is not valid. Try again.");
    }
}