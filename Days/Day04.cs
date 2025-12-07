namespace AdventOfCode.Days
{
    internal class Day04 : IDay
    {
        public override string InputFile => "Day04.txt";

        public override string Part1(string input)
        {
            string[] rows = GetInputFile();
            int[,] counts = new int[rows.Length, rows[0].Length];

            for (int i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                for (int j = 0; j < row.Length; j++)
                {
                    if (row[j] != '@') continue;

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0) continue;

                            int newX = i + x;
                            int newY = j + y;
                            if (newX >= 0 && newX < rows.Length && newY >= 0 && newY < row.Length)
                            {
                                counts[newX, newY]++;
                            }
                        }
                    }
                }
            }

            var total = 0;

            for (int i = 0; i < rows.Length; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    if (rows[i][j] == '@' && counts[i, j] < 4)
                    {
                        total++;
                    }
                }
            }

            return total.ToString();
        }
        public override string Part2(string input)
        {
            string[] rows = GetInputFile();
            int totalRemoved = 0;
            int removed;
            do
            {
                removed = 0;

                int[,] counts = new int[rows.Length, rows[0].Length];

                for (int i = 0; i < rows.Length; i++)
                {
                    var row = rows[i];
                    for (int j = 0; j < row.Length; j++)
                    {
                        if (row[j] != '@') continue;

                        for (int x = -1; x <= 1; x++)
                        {
                            for (int y = -1; y <= 1; y++)
                            {
                                if (x == 0 && y == 0) continue;

                                int newX = i + x;
                                int newY = j + y;
                                if (newX >= 0 && newX < rows.Length && newY >= 0 && newY < row.Length)
                                {
                                    counts[newX, newY]++;
                                }
                            }
                        }
                    }
                }

                var newRows = rows.ToArray();

                for (int i = 0; i < rows.Length; i++)
                {
                    for (int j = 0; j < rows[i].Length; j++)
                    {
                        if (rows[i][j] == '@' && counts[i, j] < 4)
                        {
                            newRows[i] = newRows[i].Remove(j, 1).Insert(j, ".");
                            removed++;
                        }
                    }
                }

                rows = newRows;
                totalRemoved += removed;
            } while (removed > 0);

            return totalRemoved.ToString();
        }
    }
}
