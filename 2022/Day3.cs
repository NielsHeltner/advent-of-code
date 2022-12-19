namespace AdventOfCode;

public class Day3
{
    public async Task RunAsync()
    {
        var input = await File.ReadAllLinesAsync("Day3Input.txt");
        var parsed = input.Select(x =>
        {
            var half = x.Length / 2;
            var left = x.Substring(0, half);
            var right = x.Substring(half);

            return new Rucksack(left, right);
        });

        var scores = parsed.Select(x => 
        {
            var left = x.left.ToHashSet();
            var duplicated = x.right.First(y => left.Contains(y));

            if (char.IsLower(duplicated))
            {
                var score = (duplicated - 'a') + 1;
                return score;
            }

            return (duplicated - 'A') + 27;
        });

        Console.WriteLine(scores.Sum());

        var scores2 = parsed.Select((x, i) => (x, i)).GroupBy(x => x.i / 3).Select(r => 
        {
            var rucksacks = r.Select(x => x.x.left + x.x.right).ToList();

            var overlaps = rucksacks.Aggregate((acc, ele) => string.Concat(acc.Intersect(ele)));
            if (overlaps.Length != 1)
                throw new Exception(overlaps);

            var duplicated = char.Parse(overlaps);

            if (char.IsLower(duplicated))
            {
                var score = (duplicated - 'a') + 1;
                return score;
            }

            return (duplicated - 'A') + 27;
        });

        Console.WriteLine(scores2.Sum());
    }

    internal record Rucksack(string left, string right);
}