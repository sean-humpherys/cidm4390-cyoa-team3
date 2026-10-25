using System;

Console.WriteLine("=== Character Creator ===");

string name;
do
{
    Console.Write("Name Your Character: ");
    name = Console.ReadLine() ?? "";

    if (string.IsNullOrWhiteSpace(name))
    {
        Console.WriteLine("Please enter a name.");
    }
}
while (string.IsNullOrWhiteSpace(name));

Race race = AskForEnum<Race>(
    "Choose your race: Elf, Human, Dwarf, Halfling"
);

Occupation occupation = AskForEnum<Occupation>(
    "Choose your occupation: Fighter, Magician, Thief, Archer"
);

Player player = new Player(name, race, occupation);

Console.WriteLine($"\nCharacter Created:{player.Name}");
Console.WriteLine($"Race: {player.Race} | Occupation: {player.Occupation}");

while(true)
{
    Console.WriteLine("\nCommands: roll | stats | Continue | quit");
    Console.Write("Enter a command: ");

    string command = (Console.ReadLine() ?? "")
        .Trim()
        .ToLowerInvariant();

    switch (command)
    {
        case "roll":
            if (player.HasRolledStats)
            {
                Console.WriteLine(
                    "You have already rolled your stats. " +
                    "Use 'stats' to view them.");
            }
            else
            {
                player.RollInitialStats();
                Console.WriteLine(player.DisplayStats());
            }
            break;

        case "stats":
            Console.WriteLine(player.DisplayStats());
            break;

        case "continue":
            if (!player.HasRolledStats)
            {
                Console.WriteLine(
                    "Roll your initial stats before continuing.");
            }
            else
            {
                Console.WriteLine(
                    "Your character is ready for the next game step.");

            }
            break;

        case "quit":
            Console.WriteLine("Goodbye!");
            return;

        default:
            Console.WriteLine(
                "Unknown command. Enter roll, stats, continue, or quit.");
            break;
    }
}

static T AskForEnum<T>(string prompt)
    where T : struct, Enum
{
    while (true)
    {
        Console.WriteLine($"\n{prompt}");
        Console.Write("Your choice: ");

        string input = Console.ReadLine() ?? "";

        if (Enum.TryParse<T>(input, true, out T result) &&
            Enum.IsDefined(typeof(T), result))
        {
            return result;
        }

        Console.WriteLine("That choice is not valid. Try again.");
    }
}