using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    internal class Day10 : IDay
    {
        public override string InputFile => "Day10.txt";
        public override string Part1()
        {
            var input = GetInputFile();
            Regex seperator = new(@"\[(.*)\] (.*) \{(.*)\}");

            int total = 0;
            foreach (var line in input)
            {
                var match = seperator.Match(line);
                var indicatorLength = match.Groups[1].Length;
                var indicators = Convert.ToInt32(new string(match.Groups[1].Value.Select(c => (c == '#') ? '1' : '0').ToArray()), 2);
                var buttons = match.Groups[2].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(button =>
                {
                    var buttonNumber = 0;
                    foreach (var buttonElement in button.TrimStart('(').TrimEnd(')').Split(",", StringSplitOptions.RemoveEmptyEntries))
                    {
                        var i = int.Parse(buttonElement);
                        buttonNumber |= (1 << (indicatorLength - i - 1));
                    }
                    return buttonNumber;
                }).ToArray();

                int[] sequences = GetValidButtonSequences(0, 0, indicators, buttons);
                var minButtons = sequences.Min(seq => Convert.ToString(seq, 2).Count(c => c == '1'));

                total += minButtons;
            }
            return total.ToString();
        }

        public override string Part2()
        {
            var input = GetInputFile();
            Regex seperator = new(@"\[(.*)\] (.*) \{(.*)\}");

            int total = 0;
            foreach (var line in input)
            {
                var match = seperator.Match(line);
                var joltageRequirements = match.Groups[3].Value.Split(",").Select(int.Parse).ToArray();
                var buttons = match.Groups[2].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(button =>
                {
                    return button.TrimStart('(').TrimEnd(')').Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                }).ToArray();

                var adapterMentionCount = new int[joltageRequirements.Length];

                for (int i = 0; i < buttons.Length; i++)
                {
                    for (int j = 0; j < buttons[i].Length; j++)
                    {
                        adapterMentionCount[buttons[i][j]]++;
                    }
                }

                buttons = buttons.OrderBy(button => button.Min(adapterIndex => adapterMentionCount[adapterIndex])).OrderByDescending(button => button.Length).ToArray();

                adapterMentionCount = new int[joltageRequirements.Length];
                var lastMentioned = new int[joltageRequirements.Length];

                for (int i = 0; i < buttons.Length; i++)
                {
                    for (int j = 0; j < buttons[i].Length; j++)
                    {
                        adapterMentionCount[buttons[i][j]]++;
                        lastMentioned[buttons[i][j]] = i;
                    }
                }

                var requirementsFulfilledByCount = new int[buttons.Length];

                for (int i = 0; i < adapterMentionCount.Length; i++)
                {
                    requirementsFulfilledByCount[lastMentioned[i]]++;
                }

                var requirementsFulfilledBy = new int[buttons.Length][];

                for (int i = 0; i < buttons.Length; i++)
                {
                    requirementsFulfilledBy[i] = new int[requirementsFulfilledByCount[i]];
                }

                for (int i = 0; i < adapterMentionCount.Length; i++)
                {
                    var adapterIndex = lastMentioned[i];
                    requirementsFulfilledBy[adapterIndex][--requirementsFulfilledByCount[adapterIndex]] = i;
                }

                //Console.WriteLine(string.Join(", ", lastMentioned));
                //Console.WriteLine("Requirements Fulfilled By:");
                //foreach (var group in requirementsFulfilledBy)
                //{
                //    Console.WriteLine(string.Join(", ", group));
                //}

                int[] sequence = GetValidJoltageSequence(new int[joltageRequirements.Length], [], joltageRequirements, buttons, requirementsFulfilledBy)!;

                //int[] mergedSequence = sequence.Zip(baseSequence, (a, b) => a + b).ToArray();

                var minButtons = sequence.Sum();

                total += minButtons;
                Console.WriteLine(total);
            }
            return total.ToString();
        }

        public int[] GetValidButtonSequences(int total, int sequence, int indicators, int[] buttons)
        {
            if (total == indicators)
            {
                return [sequence];
            }
            if (buttons.Length == 0)
            {
                return [];
            }
            var with = GetValidButtonSequences(total ^ buttons[0], (sequence << 1) | 1, indicators, buttons[1..]);
            var without = GetValidButtonSequences(total, sequence << 1, indicators, buttons[1..]);
            return with.Concat(without).ToArray();
        }

        public int[]? GetValidJoltageSequence(int[] total, int[] sequence, int[] joltageRequirement, int[][] joltageAdapters, int[][] fulfillRequirements)
        {
            if (joltageRequirement.SequenceEqual(total))
            {
                return sequence;
            }

            if (joltageAdapters.Length == 0) return null;

            var maxCanAdd = joltageAdapters[0].Min(adapterIndex => joltageRequirement[adapterIndex] - total[adapterIndex]);
            var minCanAdd = fulfillRequirements[0].Length > 0 ? fulfillRequirements[0].Max(adapterIndex => joltageRequirement[adapterIndex] - total[adapterIndex]) : 0;
            //Console.WriteLine(minCanAdd + ", " + maxCanAdd);
            var newTotal = (int[])total.Clone();
            for (int i = 0; i < joltageAdapters[0].Length; i++)
            {
                newTotal[joltageAdapters[0][i]] += maxCanAdd + 1;
            }
            var newAdapters = joltageAdapters[1..];
            var newFulfillRequirements = fulfillRequirements[1..];
            var newSequence = sequence.Append(maxCanAdd + 1).ToArray();

            int[]? bestResult = null;
            int bestCount = int.MaxValue;

            bool nextSameSize = joltageAdapters.Length > 1 && joltageAdapters[1].Length == joltageAdapters[0].Length;

            for (int count = maxCanAdd; count >= minCanAdd; count--)
            {
                for (int i = 0; i < joltageAdapters[0].Length; i++)
                {
                    newTotal[joltageAdapters[0][i]]--;
                }

                newSequence[^1] = count;

                var result = GetValidJoltageSequence(newTotal, newSequence, joltageRequirement, newAdapters, newFulfillRequirements);
                var resultCount = result?.Sum();
                if (result != null && resultCount < bestCount)
                {
                    bestResult = result;
                    bestCount = resultCount.Value;
                }
            }

            return bestResult;
        }
    }
}
