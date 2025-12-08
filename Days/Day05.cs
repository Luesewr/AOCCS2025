using System.Text.Unicode;

namespace AdventOfCode.Days
{
    internal class Day05 : IDay
    {
        public override string InputFile => "Day05.txt";

        public override string Part1()
        {
            var input = GetInputFile();

            var ranges = input.TakeWhile(line => !string.IsNullOrWhiteSpace(line))
                              .Select(line => line.Split("-", StringSplitOptions.TrimEntries))
                              .Select(parts => (Start: long.Parse(parts[0]), End: long.Parse(parts[1])))
                              .ToList();
            var ids = input.Skip(ranges.Count + 1)
                                   .Select(line => long.Parse(line.Trim()))
                                   .ToHashSet();

            var freshIds = ids.Where(id =>
            {
                return ranges.Any(range => id >= range.Start && id <= range.End);
            });

            return freshIds.Count().ToString();
        }

        public override string Part2()
        {
            var input = GetInputFile();

            var ranges = input.TakeWhile(line => !string.IsNullOrWhiteSpace(line))
                              .Select(line => line.Split("-", StringSplitOptions.TrimEntries))
                              .Select(parts => (Start: long.Parse(parts[0]), End: long.Parse(parts[1])))
                              .ToList();

            for (int i = 0; i < ranges.Count; i++)
            {
                for (int j = i + 1; j < ranges.Count; j++)
                {
                    if ((ranges[i].Start >= ranges[j].Start && ranges[i].Start <= ranges[j].End) || 
                        (ranges[i].End >= ranges[j].Start && ranges[i].End <= ranges[j].End) || 
                        (ranges[i].Start <= ranges[j].Start && ranges[i].End >= ranges[j].End))
                    {
                        var newStart = Math.Min(ranges[i].Start, ranges[j].Start);
                        var newEnd = Math.Max(ranges[i].End, ranges[j].End);
                        ranges[i] = (newStart, newEnd);
                        ranges.RemoveAt(j);
                        i = -1;
                        break;
                    }
                }
            }

            var totalIds = ranges.Sum(range => range.End - range.Start + 1);

            return totalIds.ToString();
        }
    }
}
