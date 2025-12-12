namespace AdventOfCode.Days
{
    internal class Day09 : IDay
    {
        public override string InputFile => "Day09.txt";
        public override string Part1()
        {
            var input = GetInputFile();
            var coordinates = input.Select(line => 
            {
                var parts = line.Split(',', StringSplitOptions.TrimEntries);
                return (X: long.Parse(parts[0]), Y: long.Parse(parts[1]));
            }).ToList();

            var largest = coordinates
                .SelectMany(coordinate => coordinates
                    .Where(other => !coordinate.Equals(other))
                    .Select(other => (Start: coordinate, End: other)))
                .Select(entry =>
                    (Math.Abs(entry.Start.X - entry.End.X) + 1) *
                    (Math.Abs(entry.Start.Y - entry.End.Y) + 1))
                .Max();
            return largest.ToString();
        }
        public override string Part2()
        {
            // Detect the horizontal and vertical points of conflict, eliminate adjacent parrallel points of conflict
            var input = GetInputFile();
            var coordinates = input.Select(line =>
            {
                var parts = line.Split(',', StringSplitOptions.TrimEntries);
                return (X: long.Parse(parts[0]), Y: long.Parse(parts[1]));
            }).ToList();
            var filteredCoordinates = coordinates
                .Where(coordinate => !coordinates.Any(other =>
                    other.X == coordinate.X &&
                    Math.Abs(other.Y - coordinate.Y) == 1) &&
                    !coordinates.Any(other =>
                    other.Y == coordinate.Y &&
                    Math.Abs(other.X - coordinate.X) == 1));
            var uniqueX = coordinates.Select(c => c.X).Distinct().ToList();
            var uniqueY = coordinates.Select(c => c.Y).Distinct().ToList();
            var segments = coordinates.Select((coordinate, index) => (Start: coordinate, End: coordinates[(index + 1) % coordinates.Count])).ToList();
            var horizontalConflicts = segments
                .Where(segment => segment.Start.Y == segment.End.Y)
                .SelectMany(segment => uniqueX
                    .Where(x => x > Math.Min(segment.Start.X, segment.End.X) && x < Math.Max(segment.Start.X, segment.End.X))
                    .Select(x => (X: x, segment.Start.Y))
                ).ToArray();
            var verticalConflicts = segments
                .Where(segment => segment.Start.X == segment.End.X)
                .SelectMany(segment => uniqueY
                    .Where(y => y > Math.Min(segment.Start.Y, segment.End.Y) && y < Math.Max(segment.Start.Y, segment.End.Y))
                    .Select(y => (segment.Start.X, Y: y))
                ).ToArray();
            horizontalConflicts = horizontalConflicts
                .Where(point => !horizontalConflicts.Any(other =>
                    other.X == point.X &&
                    Math.Abs(other.Y - point.Y) == 1))
                .ToArray();
            verticalConflicts = verticalConflicts
                .Where(point => !verticalConflicts.Any(other =>
                    other.Y == point.Y &&
                    Math.Abs(other.X - point.X) == 1))
                .ToArray();

            var normalizedCoordinateDirection = segments
                .Select((segment, index) => {
                    var previousSegment = segments[(index - 1 + segments.Count) % segments.Count];

                    var directions = (s1: (
                        X: Math.Sign(previousSegment.End.X - previousSegment.Start.X),
                        Y: Math.Sign(previousSegment.End.Y - previousSegment.Start.Y)
                    ), s2: (
                        X: Math.Sign(segment.End.X - segment.Start.X),
                        Y: Math.Sign(segment.End.Y - segment.Start.Y)
                    ));

                    return (Coordinate: coordinates[index], Directions: directions);
                })
                .Select(entry =>
                {
                    var direction = (X: entry.Directions.s2.X - entry.Directions.s1.X, Y: entry.Directions.s2.Y - entry.Directions.s1.Y);
                    var crossProduct = entry.Directions.s1.X * entry.Directions.s2.Y - entry.Directions.s1.Y * entry.Directions.s2.X;
                    var inverted = crossProduct < 0;
                    return (entry.Coordinate, Direction: direction, Inverted: inverted);
                })
                .ToArray();

            var largest = coordinates
                .SelectMany((coordinate, currentIndex) => coordinates
                    .Where((other, otherIndex) => otherIndex > currentIndex)
                    .Select(other => (Start: coordinate, End: other)))
                    .Where(entry =>
                    {
                        var (_, currentDirection, currentInverted) = normalizedCoordinateDirection.First(ncd => ncd.Coordinate.Equals(entry.Start));
                        var (_, otherDirection, otherInverted) = normalizedCoordinateDirection.First(ncd => ncd.Coordinate.Equals(entry.End));
                        var xCurrentDelta = Math.Sign(entry.End.X - entry.Start.X);
                        var yCurrentDelta = Math.Sign(entry.End.Y - entry.Start.Y);
                        var currentDirectionConflict = (currentDirection.X == xCurrentDelta && currentDirection.Y == yCurrentDelta) == currentInverted;
                        var xOtherDelta = Math.Sign(entry.Start.X - entry.End.X);
                        var yOtherDelta = Math.Sign(entry.Start.Y - entry.End.Y);
                        var otherDirectionConflict = (otherDirection.X == xOtherDelta && otherDirection.Y == yOtherDelta) == otherInverted;
                        return !(currentDirectionConflict || otherDirectionConflict);
                    })
                    .Where(entry => !filteredCoordinates.Any(filteredCoordinates =>
                        filteredCoordinates.X > Math.Min(entry.Start.X, entry.End.X) &&
                         filteredCoordinates.X < Math.Max(entry.Start.X, entry.End.X) &&
                         filteredCoordinates.Y > Math.Min(entry.Start.Y, entry.End.Y) &&
                         filteredCoordinates.Y < Math.Max(entry.Start.Y, entry.End.Y)))
                    .Where(entry => !horizontalConflicts.Any(conflict =>
                        conflict.X >= Math.Min(entry.Start.X, entry.End.X) &&
                        conflict.X <= Math.Max(entry.Start.X, entry.End.X) &&
                        conflict.Y > Math.Min(entry.Start.Y, entry.End.Y) &&
                        conflict.Y < Math.Max(entry.Start.Y, entry.End.Y)))
                    .Where(entry => !verticalConflicts.Any(conflict =>
                        conflict.Y >= Math.Min(entry.Start.Y, entry.End.Y) &&
                        conflict.Y <= Math.Max(entry.Start.Y, entry.End.Y) &&
                        conflict.X > Math.Min(entry.Start.X, entry.End.X) &&
                        conflict.X < Math.Max(entry.Start.X, entry.End.X)))
                .Select(entry =>
                    (Math.Abs(entry.Start.X - entry.End.X) + 1) *
                    (Math.Abs(entry.Start.Y - entry.End.Y) + 1))
                .Max();
            return largest.ToString();
        }
    }
}
