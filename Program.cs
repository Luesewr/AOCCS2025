using AdventOfCode.Days;

if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run <day>");
    Console.WriteLine("Example: dotnet run 1");
    return;
}

if (!int.TryParse(args[0], out int dayNumber) || dayNumber < 1 || dayNumber > 25)
{
    Console.WriteLine("Please provide a valid day number (1-25)");
    return;
}

IDay? day = dayNumber switch
{
    1 => new Day01(),
    2 => new Day02(),
    3 => new Day03(),
    4 => new Day04(),
    5 => new Day05(),
    6 => new Day06(),
    7 => new Day07(),
    8 => new Day08(),
    9 => new Day09(),
    10 => new Day10(),
    _ => null
};

if (day == null)
{
    Console.WriteLine($"Day {dayNumber:D2} is not implemented yet.");
    return;
}


Console.WriteLine($"--- Day {dayNumber:D2} ---");
Console.WriteLine($"Part 1: {day.Part1()}");
Console.WriteLine($"Part 2: {day.Part2()}");
