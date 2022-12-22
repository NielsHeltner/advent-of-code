namespace AdventOfCode;

public class Day14
{
    public async Task RunAsync()
    {
        var input = await File.ReadAllLinesAsync("Day14Input.txt");
        var rockLines = input.Select(x => ParseRocks(x)).ToList();

        var cave = new Dictionary<int, Dictionary<int, Element>>();
        foreach (var rocks in rockLines)
        {
            for (var i = 1; i < rocks.Count; i++)
            {
                var previous = rocks[i - 1];
                var current = rocks[i];

                var dx = current.X - previous.X;
                var dy = current.Y - previous.Y;
                var signX = Math.Sign(dx);
                var signY = Math.Sign(dy);

                for (var x = 0; x <= Math.Abs(dx); x++)
                    Insert(cave, previous.X + (x * signX), previous.Y, new Rock());
                
                for (var y = 0; y <= Math.Abs(dy); y++)
                    Insert(cave, previous.X, previous.Y + (y * signY), new Rock());
            }
        }

        var height = cave.Values.SelectMany(x => x.Keys).Max();
        var cave2 = cave.ToDictionary(x => x.Key, x => x.Value.ToDictionary(y => y.Key, y => y.Value));

        var part1 = SimulateSand(cave, height, false);
        Console.WriteLine(part1);

        var part2 = SimulateSand(cave2, height, true);
        Console.WriteLine(part2);
    }

    const int sandX = 500;

    int SimulateSand(Dictionary<int, Dictionary<int, Element>> cave, int height, bool part2)
    {
        var moves = new List<Position>
        {
            new Position(0, 1),
            new Position(-1, 1),
            new Position(1, 1)
        };

        var countSand = 0;
        var sand = new Position(sandX, 0);
        while (part2 || sand.Y < height)
        {
            var newPos = MoveSand(moves, sand, cave, height);
            if (newPos != null)
                sand = newPos;
            else
            {
                Insert(cave, sand.X, sand.Y, new Sand());
                countSand++;
                if (sand.Y <= 0)
                    break;
                sand = new Position(sandX, 0);
            }
        }

        return countSand;
    }

    Position? MoveSand(List<Position> moves, Position sand, Dictionary<int, Dictionary<int, Element>> cave, int height)
    {
        foreach (var move in moves)
        {
            var newPos = sand with { X = sand.X + move.X, Y = sand.Y + move.Y};
            if (!IsOccupied(cave, newPos.X, newPos.Y, height))
                return newPos;
        }

        return null;
    }

    void Insert(Dictionary<int, Dictionary<int, Element>> cave, int x, int y, Element element)
    {
        if (!cave.TryGetValue(x, out var xLayer))
            cave[x] = new Dictionary<int, Element>();
        
        cave[x][y] = element;
    }

    bool IsOccupied(Dictionary<int, Dictionary<int, Element>> cave, int x, int y, int height)
    {
        var isOccupied = cave.TryGetValue(x, out var xLayer) && xLayer.ContainsKey(y);
        var isFloor = y >= height + 2;
        return isOccupied || isFloor;
    }
    
    List<Position> ParseRocks(string input)
    {
        var tokens = input.Split("->", StringSplitOptions.RemoveEmptyEntries);
        var rocks = tokens.Select(x => ParseRock(x)).ToList();
        return rocks;
    }

    Position ParseRock(string input)
    {
        var tokens = input.Split(',', 2, StringSplitOptions.RemoveEmptyEntries);
        var x = int.Parse(tokens[0]);
        var y = int.Parse(tokens[1]);

        return new Position(x, y);
    }

    internal abstract record Element();
    internal record Rock() : Element();
    internal record Sand() : Element();
    internal record Position(int X, int Y);
}