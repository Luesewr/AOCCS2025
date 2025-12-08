namespace AdventOfCode.Days
{
    internal class Day07 : IDay
    {
        public override string InputFile => "Day07.txt";
        public override string Part1()
        {
            var rows = GetInputFile();
            var startIndex = rows[0].IndexOf('S');
            var beamIndexes = new HashSet<int> { startIndex };

            int total = 0;

            foreach (var row in rows[1..])
            {
                var newBeamIndexes = new HashSet<int>();
                foreach (var beamIndex in beamIndexes)
                {
                    if (row[beamIndex] == '^')
                    {
                        newBeamIndexes.Add(beamIndex - 1);
                        newBeamIndexes.Add(beamIndex + 1);
                        total++;
                    } else
                    {
                        newBeamIndexes.Add(beamIndex);
                    }
                }
                beamIndexes = newBeamIndexes;
            }

            return total.ToString();
        }
        public override string Part2()
        {
            var rows = GetInputFile();
            var startIndex = rows[0].IndexOf('S');
            var beamCounts = new long[rows[0].Length];
            beamCounts[startIndex] = 1;

            foreach (var row in rows[1..])
            {
                var newBeamCounts = new long[rows[0].Length];
                for (int i = 0; i < beamCounts.Length; i++)
                {
                    long beamCount = beamCounts[i];
                    if (row[i] == '^')
                    {
                        newBeamCounts[i - 1] += beamCount;
                        newBeamCounts[i + 1] += beamCount;
                    }
                    else
                    {
                        newBeamCounts[i] += beamCount;
                    }
                }
                beamCounts = newBeamCounts;
            }

            long total = beamCounts.Sum();

            return total.ToString();
        }
    }
}
