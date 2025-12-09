using System.Numerics;

namespace AdventOfCode.Days
{
    internal class Day08 : IDay
    {
        public override string InputFile => "Day08.txt";
        public override string Part1()
        {
            var input = GetInputFile();
            var n = 1000;
            var positions = input.Select(line => line.Split(',').Select(int.Parse).ToArray()).Select(p => new Vector3(p[0], p[1], p[2]));
            var rankings = positions.Select((p, idx) => new { Position = p, Index = idx })
                .SelectMany(p => positions.Select((other, otherIdx) => new { Other = other, OtherIndex = otherIdx })
                    .Where(other => other.OtherIndex > p.Index)
                    .OrderBy(other => Vector3.DistanceSquared(p.Position, other.Other))
                    .Take(n)
                    .Select(o => Tuple.Create(p.Index, o.OtherIndex)))
                .OrderBy(entry => Vector3.DistanceSquared(positions.ElementAt(entry.Item1), positions.ElementAt(entry.Item2)))
                .Take(n);

            var unionFind = new UnionFind(positions.Count());

            foreach (var rank in rankings)
            {
                unionFind.Union(rank.Item1, rank.Item2);
            }

            var counts = new long[positions.Count()];
            for (int i = 0; i < positions.Count(); i++)
            {
                var root = unionFind.Find(i);
                counts[root]++;
            }

            var total = counts.OrderDescending().Take(3).Aggregate(1L, (acc, val) => acc * val);

            return total.ToString();
        }

        public override string Part2()
        {
            var input = GetInputFile();
            var positions = input.Select(line => line.Split(',').Select(int.Parse).ToArray()).Select(p => new Vector3(p[0], p[1], p[2]));
            var rankings = positions.Select((p, idx) => new { Position = p, Index = idx })
                .SelectMany(p => positions.Select((other, otherIdx) => new { Other = other, OtherIndex = otherIdx })
                    .Where(other => other.OtherIndex > p.Index)
                    .OrderBy(other => Vector3.DistanceSquared(p.Position, other.Other))
                    .Select(o => Tuple.Create(p.Index, o.OtherIndex)))
                .OrderBy(entry => Vector3.DistanceSquared(positions.ElementAt(entry.Item1), positions.ElementAt(entry.Item2)));

            var unionFind = new UnionFind(positions.Count());

            List<Tuple<Vector3, Vector3>> connections = [];

            foreach (var rank in rankings)
            {
                if (unionFind.Union(rank.Item1, rank.Item2))
                    connections.Add(Tuple.Create(positions.ElementAt(rank.Item1), positions.ElementAt(rank.Item2)));

                if (connections.Count >= positions.Count() - 1)
                    break;
            }

            var lastConnection = connections.Last();
            var total = (long)lastConnection.Item1.X * (long)lastConnection.Item2.X;

            return total.ToString();
        }
    }

    class UnionFind
    {
        private int[] parent;
        private int[] rank;
        public UnionFind(int size)
        {
            parent = new int[size];
            rank = new int[size];
            for (int i = 0; i < size; i++)
                parent[i] = i;
        }
        public int Find(int x)
        {
            if (parent[x] != x)
                parent[x] = Find(parent[x]);
            return parent[x];
        }
        public bool Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);
            if (rootX != rootY)
            {
                if (rank[rootX] > rank[rootY])
                    parent[rootY] = rootX;
                else if (rank[rootX] < rank[rootY])
                    parent[rootX] = rootY;
                else
                {
                    parent[rootY] = rootX;
                    rank[rootX]++;
                }
                return true;
            }
            return false;
        }
    }
}
