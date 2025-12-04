namespace AdventOfCode.Days;

public class Day02 : IDay
{
    public override string InputFile => "Day02.txt";

    public override string Part1(string input)
    {
        var lines = GetInputFile();
        var line = lines[0];
        var ranges = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

        long total = 0;

        foreach (var range in ranges)
        {
            var bounds = range.Split('-', StringSplitOptions.RemoveEmptyEntries);
            long start = long.Parse(bounds[0]);
            int startLength = bounds[0].Length;
            long end = long.Parse(bounds[1]);
            int endLength = bounds[1].Length;

            var startHalf = startLength % 2 == 0 ? long.Parse(bounds[0][..(startLength / 2)]) : NthPowerOf10((startLength) / 2);
            var endHalf = endLength % 2 == 0 ? long.Parse(bounds[1][..(endLength / 2)]) : NthPowerOf10((endLength - 1) / 2) - 1;

            if (endHalf < startHalf) continue;

            for (long i = startHalf; i <= endHalf; i++)
            {
                var candidate = long.Parse(i + "" + i);
                if (candidate >= start && candidate <= end)
                {
                    total += candidate;
                }
            }
        }

        return total.ToString();
    }

    public override string Part2(string input)
    {
        // TODO: Implement Part 2
        return "Not implemented";
    }

    private static long NthPowerOf10(long n)
    {
        long result = 1;
        for (long i = 0; i < n; i++)
        {
            result *= 10;
        }
        return result;
    }
}
