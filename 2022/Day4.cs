namespace AdventOfCode;

public sealed class Day4
{
    public async Task RunAsync()
    {
        var input = await File.ReadAllLinesAsync("Day4Input.txt");
        var parsed = input.Select(x => 
        {
            var ranges = x.Split(",", 2);
            var left = ParseRange(ranges[0]);
            var right = ParseRange(ranges[1]);

            return new ElfPair(left, right);
        }).ToList();

        var contains = parsed.Where(x => x.left.Contains(x.right) || x.right.Contains(x.left)).ToList();
        
        Console.WriteLine(contains.Count);

        var overlaps = parsed.Where(x => x.left.Overlaps(x.right) || x.right.Overlaps(x.left)).ToList();
        
        Console.WriteLine(overlaps.Count);
    }

    private Range ParseRange(string input)
    {
        var tokenized = input.Split("-", 2);
        var from = int.Parse(tokenized[0]);
        var to = int.Parse(tokenized[1]);
        return new Range(from, to);
    }

    record Range(int from, int to)
    {
        public bool Contains(Range other)
        {
            return from <= other.from && to >= other.to;
        }

        public bool Overlaps(Range other)
        {
            return (to >= other.from && to <= other.to) || (from <= other.to && from >= other.from);
        }
    }

    record ElfPair(Range left, Range right);
}
