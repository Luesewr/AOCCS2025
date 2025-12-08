using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    internal class Day06 : IDay
    {
        public override string InputFile => "Day06.txt";
        public override string Part1()
        {
            var rows = GetInputFile();
            var operatorRow = rows.Last();
            var operatorIndexes = new List<int>();
            for (int i = 0; i < operatorRow.Length; i++)
            {
                if (operatorRow[i] != ' ')
                {
                    operatorIndexes.Add(i);
                }
            }

            var columns = new List<List<long>>();
            for (int i = 0; i < rows.Length - 1; i++)
            {
                var row = rows[i];
                for (int j = 0; j < operatorIndexes.Count; j++)
                {
                    if (i == 0)
                    {
                        columns.Add([]);
                    }

                    var start = operatorIndexes[j];
                    var end = j + 1 < operatorIndexes.Count ? operatorIndexes[j + 1] : row.Length;
                    
                    var number = int.Parse(row[start..end].Trim());
                    columns[j].Add(number);
                }
            }

            long total = 0;
            
            for (int i = 0; i < columns.Count; i++)
            {
                var op = operatorRow[operatorIndexes[i]];
                if (op == '+')
                {
                    var sum = columns[i].Sum();
                    total += sum;
                }
                else if (op == '*')
                {
                    var product = columns[i].Aggregate(1L, (acc, val) => acc * val);
                    total += product;
                }
            }
            return total.ToString();
        }
        public override string Part2()
        {
            var rows = GetInputFile();
            var operatorRow = rows.Last();
            var operatorIndexes = new List<int>();
            for (int i = 0; i < operatorRow.Length; i++)
            {
                if (operatorRow[i] != ' ')
                {
                    operatorIndexes.Add(i);
                }
            }

            var columns = new List<List<long>>();

            for (int i = 0; i < operatorIndexes.Count; i++)
            {
                var block = new string[rows.Length - 1];

                for (int j = 0; j < rows.Length - 1; j++)
                {
                    var row = rows[j];

                    var start = operatorIndexes[i];
                    var end = i + 1 < operatorIndexes.Count ? operatorIndexes[i + 1] : row.Length;

                    var segment = row[start..end];
                    block[j] = segment;
                }

                var tiltedBlock = new string[block[0].Length];
                for (int r = 0; r < block.Length; r++)
                {
                    for (int c = 0; c < block[r].Length; c++)
                    {
                        tiltedBlock[c] += block[r][c];
                    }
                }

                columns.Add([]);
                for (int j = 0; j < tiltedBlock.Length; j++)
                {
                    if (tiltedBlock[j].Trim().Length == 0)
                        continue;

                    columns[i].Add(long.Parse(tiltedBlock[j].Trim()));
                }
            }

            long total = 0;

            for (int i = 0; i < columns.Count; i++)
            {
                var op = operatorRow[operatorIndexes[i]];
                if (op == '+')
                {
                    var sum = columns[i].Sum();
                    total += sum;
                }
                else if (op == '*')
                {
                    var product = columns[i].Aggregate(1L, (acc, val) => acc * val);
                    total += product;
                }
            }
            return total.ToString();
        }
    }
}
