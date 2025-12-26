namespace AdventOfCode.Days
{
    internal class Day11 : IDay
    {
        public override string InputFile => "Day11.txt";

        public override string Part1()
        {
            var lines = GetInputFile();
            var lookup = lines.Select(line => line.Split(": "))
                              .ToDictionary(parts => parts[0], parts => parts[1].Split(" ").ToHashSet());
            var reverseLookup = lookup.SelectMany(kvp => kvp.Value.Select(value => (value, kvp.Key)))
                                      .GroupBy(pair => pair.value)
                                      .ToDictionary(g => g.Key, g => g.Select(pair => pair.Key).ToHashSet());
            var valid = GetValidSet(reverseLookup, "out");

            var currentCounts = lookup.Keys.Concat(reverseLookup.Keys).Distinct().ToDictionary(key => key, key => 0L);
            currentCounts["you"] = 1L;
            var outCount = 0L;

            while (currentCounts.Values.Any(value => value > 0))
            {
                var (current, count) = currentCounts.First(kvp => kvp.Value > 0);
                currentCounts[current] = 0;
                if (current == "out")
                {
                    outCount += count;
                    continue;
                }
                foreach (var output in lookup[current])
                {
                    if (!valid.Contains(output))
                        continue;

                    currentCounts[output] += count;
                }
            }

            return outCount.ToString();
        }

        public override string Part2()
        {
            var lines = GetInputFile();
            var lookup = lines.Select(line => line.Split(": "))
                              .ToDictionary(parts => parts[0], parts => parts[1].Split(" ").ToHashSet());
            var reverseLookup = lookup.SelectMany(kvp => kvp.Value.Select(value => (value, kvp.Key)))
                                      .GroupBy(pair => pair.value)
                                      .ToDictionary(g => g.Key, g => g.Select(pair => pair.Key).ToHashSet());
            var outValid = GetValidSet(reverseLookup, "out");
            var dacValid = GetValidSet(reverseLookup, "dac");
            var fftValid = GetValidSet(reverseLookup, "fft");
            var defaultValid = dacValid.Intersect(fftValid).ToHashSet();

            var allowedSets = new HashSet<string>[]
            {
                defaultValid,
                dacValid,
                fftValid,
                outValid,
            };

            var currentCounts = lookup.Keys.Concat(reverseLookup.Keys).Distinct().ToDictionary(key => key, key => new long[4] { 0L, 0L, 0L, 0L });
            currentCounts["svr"][0] = 1L;
            var outCount = 0L;

            while (currentCounts.Values.Any(values => values.Any(value => value > 0)))
            {
                var (current, counts) = currentCounts.First(kvp => kvp.Value.Any(value => value > 0));
                currentCounts[current] = [0L, 0L, 0L, 0L];

                if (current == "out")
                {
                    outCount += counts[3];
                } else if (current == "fft")
                {
                    counts[1] = counts[0];
                    counts[3] = counts[2];
                    counts[0] = 0;
                    counts[2] = 0;
                } else if (current == "dac")
                {
                    counts[2] = counts[0];
                    counts[3] = counts[1];
                    counts[0] = 0;
                    counts[1] = 0;
                }

                foreach (var output in lookup.GetValueOrDefault(current, []))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (!allowedSets[i].Contains(output) || counts[i] == 0) continue;
                        currentCounts[output][i] += counts[i];
                    }
                }
            }
            return outCount.ToString();
        }

        internal static HashSet<string> GetValidSet(Dictionary<string, HashSet<string>> lookup, string component)
        {
            var valid = new HashSet<string>();
            var queue = new Queue<string>([component]);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (valid.Contains(current))
                    continue;

                valid.Add(current);
                if (lookup.TryGetValue(current, out HashSet<string>? value))
                {
                    foreach (var dependent in value)
                    {
                        queue.Enqueue(dependent);
                    }
                }
            }
            return valid;
        }
    }
}
