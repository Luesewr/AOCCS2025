using System.Text.RegularExpressions;

namespace AdventOfCode.Days
{
    internal class Day12 : IDay
    {
        public override string InputFile => "Day12.txt";
        public const int ShapeSize = 3;

        public override string Part1()
        {
            var lines = GetInputFile();

            var rawShapes = lines.TakeWhile(line => !line.Contains('x')).ToArray();
            var rawInputs = lines.SkipWhile(line => !line.Contains('x')).ToList();

            var originalShapes = Shape.FromRawList(rawShapes);
            var areaLookup = originalShapes.Select(s => s.CountFilledCells()).ToArray();

            var allShapes = new HashSet<Shape>();

            foreach (var shape in originalShapes)
            {
                var currentShape = shape;
                for (int i = 0; i < 4; i++)
                {
                    allShapes.Add(currentShape);
                    allShapes.Add(currentShape.GetFlipped());
                    currentShape = currentShape.GetRotated();
                }
            }

            var shapeList = allShapes.ToList();

            shapeList.ForEach(shape => shape.CalculateEliminationMask(shapeList));

            var shapeUndeterminedMask = new HashSet<int>[3, 3];

            for (int shapeIndex = 0; shapeIndex < shapeList.Count; shapeIndex++)
            {
                Shape? shape = shapeList[shapeIndex];
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (!shape.RowData[i, j]) continue;
                        shapeUndeterminedMask[2 - i, 2 - j] ??= [];
                        shapeUndeterminedMask[2 - i, 2 - j].Add(shapeIndex);
                    }
                }
            }

            int total = 0;

            foreach (var rawInput in rawInputs)
            {
                Regex regex = new(@"(\d+)x(\d+): (.+)");
                var match = regex.Match(rawInput);
                if (!match.Success) throw new InvalidOperationException("Invalid input line: " + rawInput);

                int width = int.Parse(match.Groups[1].Value);
                int height = int.Parse(match.Groups[2].Value);
                int[] shapeCounts = match.Groups[3].Value.Split(' ').Select(int.Parse).ToArray();
                int totalArea = shapeCounts.Select((count, index) => count * areaLookup[index]).Sum();

                var visited = new HashSet<GridState>();

                var gridState = GridState.DefaultGrid(width, height, shapeList, shapeCounts);

                for (int shapeIndex = 0; shapeIndex < shapeCounts.Length; shapeIndex++)
                {
                    if (shapeCounts[shapeIndex] > 0) continue;
                    var childShapeIndeces = shapeList
                        .Select((s, index) => (s, index))
                        .Where(t => t.s.ShapeIndex == shapeIndex)
                        .Select(t => t.index)
                        .ToHashSet();

                    for (int yy = 0; yy < gridState.Height; yy++)
                    {
                        for (int xx = 0; xx < gridState.Width; xx++)
                        {
                            if (gridState.AllowedShapes[yy, xx].Intersect(childShapeIndeces).Any())
                            {
                                var newAllowed = new HashSet<int>(gridState.AllowedShapes[yy, xx]);
                                newAllowed.ExceptWith(childShapeIndeces);
                                gridState.AllowedShapes[yy, xx] = newAllowed;
                            }
                        }
                    }

                    gridState.UpdateAreaDeterminationsFull(shapeUndeterminedMask);
                }

                Stack<(GridState gridState, int[] remainingShapes, int remainingArea, int x, int y)> pq = new();
                pq.Push((gridState, shapeCounts, totalArea, 1, 1));
                var allPlaced = false;

                while (pq.Count > 0 && !allPlaced)
                {
                    var (currentGrid, remainingShapes, remainingArea, x, y) = pq.Pop();

                    if (visited.Contains(currentGrid))
                    {
                        continue;
                    }
                       
                    visited.Add(currentGrid);

                    foreach (var shapeIndex in currentGrid.AllowedShapes[y, x])
                    {
                        int actualShapeIndex = shapeList[shapeIndex].ShapeIndex;
                        var newGrid = currentGrid.WithShape(shapeList[shapeIndex], x, y);
                        newGrid.UpdateAreaDeterminationsAfterShape(x, y, shapeUndeterminedMask);

                        var newArea = remainingArea - areaLookup[actualShapeIndex];

                        var newRemainingShapes = (int[])remainingShapes.Clone();
                        newRemainingShapes[actualShapeIndex]--;

                        if (!newGrid.CanStillFit(totalArea, newArea))
                            continue;

                        if (newRemainingShapes[actualShapeIndex] <= 0)
                        {
                            allPlaced = newRemainingShapes.All(shapeCount => shapeCount <= 0);

                            if (allPlaced)
                            {
                                total++;
                                Console.WriteLine("Found solution:" + total);
                                break;
                            }

                            var childShapeIndeces = shapeList
                                .Select((s, index) => (s, index))
                                .Where(t => t.s.ShapeIndex == actualShapeIndex)
                                .Select(t => t.index)
                                .ToHashSet();

                            for (int yy = 0; yy < newGrid.Height; yy++)
                            {
                                for (int xx = 0; xx < newGrid.Width; xx++)
                                {
                                    if (newGrid.AllowedShapes[yy, xx].Intersect(childShapeIndeces).Any())
                                    {
                                        var newAllowed = new HashSet<int>(newGrid.AllowedShapes[yy, xx]);
                                        newAllowed.ExceptWith(childShapeIndeces);
                                        newGrid.AllowedShapes[yy, xx] = newAllowed;
                                    }
                                }
                            }

                            newGrid.UpdateAreaDeterminationsFull(shapeUndeterminedMask);
                        }

                        for (int yy = newGrid.Height - 1; yy >= 0; yy--)
                        {
                            for (int xx = newGrid.Width - 1; xx >= 0; xx--)
                            {
                                if (yy <= 0 || yy >= newGrid.Height - 1 || xx <= 0 || xx >= newGrid.Width - 1)
                                    continue;

                                var shapeSetSize = newGrid.AllowedShapes[yy, xx].Count;
                                if (shapeSetSize > 0)
                                {
                                    pq.Push((newGrid, newRemainingShapes, newArea, xx, yy));
                                }
                            }
                        }
                    }
                }
            }
            return total.ToString();
        }

        public override string Part2()
        {

            return "Result for Part 2";
        }
    }

    internal class GridState(int width, int height, int shapeCount)
    {
        public static HashSet<int> EmptySet = [];
        public static Dictionary<HashSet<int>, HashSet<int>> setLookup = [];

        public int Width { get; set; } = width;
        public int Height { get; set; } = height;
        public int ShapeCount { get; set; } = shapeCount;
        public bool[,] Data { get; set; } = new bool[height, width];
        public HashSet<int>[,] AllowedShapes { get; set; } = new HashSet<int>[height, width];
        public bool[,] Determined { get; set; } = new bool[height, width];
        public int DeterminedEmptyCount { get; set; } = 0;
        public HashSet<(int x, int y, int shapeIndex)> PlacedShapes { get; set; } = [];

        public GridState WithShape(Shape shape, int xCenter, int yCenter)
        {
            var newState = new GridState(Width, Height, ShapeCount);
            Array.Copy(Data, newState.Data, Data.Length);
            Array.Copy(AllowedShapes, newState.AllowedShapes, AllowedShapes.Length);
            Array.Copy(Determined, newState.Determined, Determined.Length);

            newState.PlacedShapes = [.. PlacedShapes, (xCenter, yCenter, shape.ShapeIndex)];

            int size = shape.RowData.GetLength(0);

            int topLeftX = xCenter - (size / 2);
            int topLeftY = yCenter - (size / 2);

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    if (shape.RowData[r, c])
                    {
                        int gridR = r + topLeftY;
                        int gridC = c + topLeftX;
                        if (gridR >= 0 && gridR < Height && gridC >= 0 && gridC < Width)
                        {
                            newState.Data[gridR, gridC] = true;
                            newState.Determined[gridR, gridC] = true;
                        }
                    }
                }
            }

            for (int yOffset = -size + 1; yOffset < size; yOffset++)
            {
                for (int xOffset = -size + 1; xOffset < size; xOffset++)
                {
                    int elimY = yCenter + yOffset;
                    int elimX = xCenter + xOffset;
                    if (elimY >= 0 && elimY < Height && elimX >= 0 && elimX < Width)
                    {
                        var elimSet = shape.EliminationMask[yOffset + size - 1, xOffset + size - 1];
                        if (elimSet != null && elimSet.Intersect(newState.AllowedShapes[elimY, elimX]).Any())
                        {
                            newState.AllowedShapes[elimY, elimX] = newState.AllowedShapes[elimY, elimX].Except(elimSet).ToHashSet();
                        }
                    }
                }
            }

            return newState;
        }

        public bool UpdateDetermined(int x, int y, HashSet<int>[,] UndeterminedMask)
        {
            if (Determined[y, x]) return false;

            bool determined = true;

            for (int i = 0; i < 3 && determined; i++)
            {
                for (int j = 0; j < 3 && determined; j++)
                {
                    int gridX = x + j - 1;
                    int gridY = y + i - 1;
                    if (gridX < 0 || gridX >= Width || gridY < 0 || gridY >= Height)
                        continue;

                    if (AllowedShapes[gridY, gridX].Intersect(UndeterminedMask[i, j]).Any())
                    {
                        determined = false;
                        break;
                    }
                }
            }

            Determined[y, x] = determined;
            if (determined && !Data[y, x])
            {
                DeterminedEmptyCount++;
            }

            return determined;
        }

        public void UpdateAreaDeterminationsAfterShape(int x, int y, HashSet<int>[,] UndeterminedMask)
        {
            var ToProcess = GetEffectiveRangeSet(x, y);

            while (ToProcess.Count > 0)
            {
                var pos = ToProcess.First();
                ToProcess.Remove(pos);
                if (!Determined[pos.y, pos.x] && UpdateDetermined(pos.x, pos.y, UndeterminedMask))
                {
                    var neighbors = GetEffectiveRangeSet(pos.x, pos.y);
                    neighbors.Remove(pos);
                    
                    ToProcess.UnionWith(neighbors);
                }
            }
        }

        public void UpdateAreaDeterminationsFull(HashSet<int>[,] UndeterminedMask)
        {
            var ToProcess = Enumerable
                .Range(0, Width)
                .SelectMany(x => Enumerable.Range(0, Height).Select(y => (x, y)))
                .Where(pos => !Determined[pos.y, pos.x])
                .ToHashSet();
            while (ToProcess.Count > 0)
            {
                var pos = ToProcess.First();
                ToProcess.Remove(pos);
                if (!Determined[pos.y, pos.x] && UpdateDetermined(pos.x, pos.y, UndeterminedMask))
                {
                    var neighbors = GetEffectiveRangeSet(pos.x, pos.y);
                    neighbors.Remove(pos);
                    ToProcess.UnionWith(neighbors);
                }
            }
        }

        public bool CanStillFit(int totalArea, int remainingArea)
        {
            int allowedEmptyCells = Width * Height - totalArea;
            return DeterminedEmptyCount <= allowedEmptyCells;
        }

        //public bool CanStillFit(int totalArea, int remainingArea)
        //{
        //    int filledCells = totalArea - remainingArea;
        //    int totalDetermined = DeterminedEmptyCount + filledCells;
        //    return (totalDetermined * totalArea / filledCells) <= Width * Height;
        //}

        public HashSet<(int x, int y)> GetEffectiveRangeSet(int xCenter, int yCenter)
        {
            return Enumerable
                .Range(xCenter - 2, 5)
                .SelectMany(x => Enumerable.Range(yCenter - 2, 5).Select(y => (x, y)))
                .Where(pos => pos.x >= 0 && pos.x < Width && pos.y >= 0 && pos.y < Height)
                .Where(pos => !Determined[pos.y, pos.x])
                .ToHashSet();
        }

        public static GridState DefaultGrid(int width, int height, List<Shape> shapes, int[] shapeCounts)
        {
            var gridState = new GridState(width, height, shapes.Count);

            var allShapesSet = new HashSet<int>(shapes.Select((item, index) => index).Where(index => shapeCounts[shapes[index].ShapeIndex] > 0));

            for (int y = 0; y < height; y++)
            {
                gridState.AllowedShapes[y, 0] = EmptySet;
                gridState.AllowedShapes[y, width - 1] = EmptySet;
            }

            for (int x = 0; x < width; x++)
            {
                gridState.AllowedShapes[0, x] = EmptySet;
                gridState.AllowedShapes[height - 1, x] = EmptySet;
            }

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    gridState.AllowedShapes[y, x] = allShapesSet;
                }
            }

            return gridState;
        }

        public override bool Equals(object? obj)
        {
            return obj is GridState other && PlacedShapes.SetEquals(other.PlacedShapes);
        }

        public override int GetHashCode()
        {
            int hash = 0;
            foreach (var (x, y, shapeIndex) in PlacedShapes)
                hash ^= HashCode.Combine(x, y, shapeIndex);
            return hash;
        }

        public override string ToString()
        {
            var lines = new List<string>();
            for (int r = 0; r < Height; r++)
            {
                var lineChars = new char[Width];
                for (int c = 0; c < Width; c++)
                {
                    lineChars[c] = Data[r, c] ? '#' : '.';
                }
                lines.Add(new string(lineChars));
            }
            return string.Join(Environment.NewLine, lines);
        }
    }

    internal class Shape(int shapeIndex, bool[,] rowData, bool[,] columnData)
    {
        public int ShapeIndex { get; set; } = shapeIndex;
        public bool[,] RowData { get; set; } = rowData;
        public bool[,] ColumnData { get; set; } = columnData;
        public HashSet<int>[,] EliminationMask { get; set; } = new HashSet<int>[5, 5];

        public static Shape FromRaw(string[] raw, int index)
        {
            var rawData = raw.Skip(1);
            var data = rawData.Select(line => line.Select(c => c == '#').ToArray()).ToArray();
            var rowData = new bool[data.Length, data.Length];
            var columnData = new bool[data.Length, data.Length];
            for (int col = 0; col < data.Length; col++)
            {
                for (int row = 0; row < data.Length; row++)
                {
                    rowData[row, col] = data[row][col];
                    columnData[col, row] = data[row][col];
                }
            }
            return new Shape(index, rowData, columnData);
        }

        public static Shape[] FromRawList(string[] rawList)
        {
            return rawList.Aggregate(new List<List<string>> { new() },
                static (acc, item) =>
                {
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        acc.Add([]);
                    }
                    else
                    {
                        acc[^1].Add(item);
                    }
                    return acc;
                }).Where(list => list.Count > 0).Select(list => list.ToArray()).Select(FromRaw).ToArray();
        }

        public bool Intersects(Shape other, int xOffset, int yOffset)
        {
            int size = RowData.GetLength(0);
            if (size == 0 || other.RowData.GetLength(0) == 0) return false;

            for (int row = 0; row < size; row++)
            {
                int otherRow = row - yOffset;
                if (otherRow < 0 || otherRow >= other.RowData.GetLength(0)) continue;

                for (int col = 0; col < size; col++)
                {
                    int otherCol = col - xOffset;
                    if (otherCol < 0 || otherCol >= size) continue;

                    if (RowData[row, col] && other.RowData[otherRow, otherCol])
                        return true;
                }
            }

            for (int col = 0; col < size; col++)
            {
                int otherCol = col - xOffset;
                if (otherCol < 0 || otherCol >= other.ColumnData.GetLength(0)) continue;

                for (int row = 0; row < size; row++)
                {
                    int otherRow = row - yOffset;
                    if (otherRow < 0 || otherRow >= size) continue;

                    if (ColumnData[col, row] && other.ColumnData[otherCol, otherRow])
                        return true;
                }
            }

            return false;
        }

        public void CalculateEliminationMask(List<Shape> allShapes)
        {
            for (int shapeIndex = 0; shapeIndex < allShapes.Count; shapeIndex++)
            {
                Shape? other = allShapes[shapeIndex];
                int size = RowData.GetLength(0);
                for (int yOffset = -size + 1; yOffset < size; yOffset++)
                {
                    for (int xOffset = -size + 1; xOffset < size; xOffset++)
                    {
                        if (Intersects(other, xOffset, yOffset))
                        {
                            EliminationMask[yOffset + size - 1, xOffset + size - 1] ??= [];
                            EliminationMask[yOffset + size - 1, xOffset + size - 1].Add(shapeIndex);
                        }
                    }
                }
            }
        }

        public Shape GetRotated()
        {
            int size = RowData.GetLength(0);
            var newRowData = new bool[size, size];
            var newColumnData = new bool[size, size];

            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    newRowData[r, c] = RowData[size - 1 - c, r];
                }
            }

            for (int c = 0; c < size; c++)
            {
                for (int r = 0; r < size; r++)
                {
                    newColumnData[c, r] = RowData[size - 1 - c, r];
                }
            }

            return new Shape(ShapeIndex, newRowData, newColumnData);
        }

        public Shape GetFlipped()
        {
            int size = RowData.GetLength(0);
            var newRowData = new bool[size, size];
            var newColumnData = new bool[size, size];
            for (int r = 0; r < size; r++)
            {
                for (int c = 0; c < size; c++)
                {
                    newRowData[r, c] = RowData[r, size - 1 - c];
                }
            }

            for (int c = 0; c < size; c++)
            {
                for (int r = 0; r < size; r++)
                {
                    newColumnData[c, r] = newRowData[r, c];
                }
            }
            return new Shape(ShapeIndex, newRowData, newColumnData);
        }

        public int CountFilledCells()
        {
            int count = 0;
            for (int row = 0; row < RowData.GetLength(0); row++)
            {
                for (int col = 0; col < RowData.GetLength(1); col++)
                {
                    if (RowData[row, col])
                        count++;
                }
            }
            return count;
        }

        public override string ToString()
        {
            var size = RowData.GetLength(0);
            var lines = new List<string>();
            for (int row = 0; row < size; row++)
            {
                var lineChars = new char[size];
                for (int col = 0; col < size; col++)
                {
                    lineChars[col] = RowData[row, col] ? '#' : '.';
                }
                lines.Add(new string(lineChars));
            }
            return string.Join(Environment.NewLine, lines);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (var row in RowData)
            {
                hash = hash * 31 + row.GetHashCode();
            }
            return hash;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Shape other || other.RowData.GetLength(0) != RowData.GetLength(0) || other.RowData.GetLength(1) != RowData.GetLength(1))
                return false;
            for (int i = 0; i < RowData.GetLength(0); i++)
            {
                for (int j = 0; j < RowData.GetLength(1); j++)
                {
                    if (RowData[i, j] != other.RowData[i, j])
                        return false;
                }
            }
            return true;
        }
    }
}
