namespace AdventOfCode.Days;

public class Day01 : IDay
{
    public override string InputFile => "Day01.txt";

    public override string Part1()
    {
        var lines = GetInputFile();

        var dial = 50;
        var dialCount = 0;

        foreach (var line in lines)
        {
            char direction = line[0];
            int distance = int.Parse(line[1..]) % 100;
            dial = direction switch
            {
                'L' => dial - distance,
                'R' => dial + distance,
                _ => throw new InvalidDataException(),
            };

            dial = (dial + 100) % 100;

            if (dial == 0) dialCount++;
        }

        return dialCount.ToString();
    }

    public override string Part2()
    {
        var lines = GetInputFile();

        var dial = 50;
        var dialCount = 0;

        foreach (var line in lines)
        {
            char direction = line[0];
            int distance = int.Parse(line[1..]);

            dialCount += distance / 100;
            distance %= 100;

            switch (direction)
            {
                case 'L':
                    if (dial != 0 && dial - distance <= 0)
                        dialCount++;
                    dial -= distance;
                    break;
                case 'R':
                    if (dial != 0 && dial + distance >= 100)
                        dialCount++;
                    dial += distance;
                    break;
            };

            dial = (dial + 100) % 100;
        }

        return dialCount.ToString();
    }
}
