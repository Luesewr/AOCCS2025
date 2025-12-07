using System.Text;

namespace AdventOfCode.Days;

public class Day03 : IDay
{
    public override string InputFile => "Day03.txt";

    public override string Part1()
    {
        string[] banks = GetInputFile();

        const int batteryAmount = 2;

        long total = 0;

        foreach (var bank in banks)
        {
            int previousIndex = -1;

            StringBuilder selectedBatteries = new();

            for (int i = 0; i < batteryAmount; i++)
            {
                var largest = -1;
                var largestIndex = -1;
                for (int j = previousIndex + 1, endIndex = bank.Length - batteryAmount + i; j <= endIndex; j++)
                {
                    var battery = bank[j] - '0';
                    if (battery > largest)
                    {
                        largest = battery;
                        largestIndex = j;
                        previousIndex = j;
                    }
                }

                selectedBatteries.Append(largest);
            }

            total += long.Parse(selectedBatteries.ToString());
        }

        return total.ToString();
    }

    public override string Part2()
    {
        string[] banks = GetInputFile();

        const int batteryAmount = 12;

        long total = 0;

        foreach (var bank in banks)
        {
            int previousIndex = -1;

            StringBuilder selectedBatteries = new();

            for (int i = 0; i < batteryAmount; i++)
            {
                var largest = -1;
                var largestIndex = -1;
                for (int j = previousIndex + 1, endIndex = bank.Length - batteryAmount + i; j <= endIndex; j++)
                {
                    var battery = bank[j] - '0';
                    if (battery > largest)
                    {
                        largest = battery;
                        largestIndex = j;
                        previousIndex = j;
                    }
                }

                selectedBatteries.Append(largest);
            }

            total += long.Parse(selectedBatteries.ToString());
        }

        return total.ToString();
    }
}
