namespace AdventOfCode;

public class Day8
{
    public async Task RunAsync()
    {
        var text = await File.ReadAllLinesAsync("Day8Input.txt");
        var input = text.Select(x => x.Select(y => int.Parse(y.ToString())).ToList()).ToList();

        var countVisible = 0;
        var bestScenicScore = 0;

        for (var rowI = 0; rowI < input.Count; rowI++)
        {
            var row = input[rowI];
            for (var colI = 0; colI < row.Count; colI++)
            {
                var tree = input[rowI][colI];

                var left = row.Take(colI).ToList();
                var right = row.Skip(colI + 1).ToList();
                var up = new List<int>();
                var down = new List<int>();

                for (var rowJ = 0; rowJ < rowI; rowJ++)
                {
                    up.Add(input[rowJ][colI]);
                }
                for (var rowK = rowI + 1; rowK < input.Count; rowK++)
                {
                    down.Add(input[rowK][colI]);
                }

                if ((left.All(x => x < tree)) ||
                    right.All(x => x < tree) ||
                    up.All(x => x < tree) ||
                    down.All(x => x < tree))
                    countVisible++;
                
                var scenicScore =
                    CountTrees(left.AsEnumerable().Reverse().ToList(), tree) * 
                    CountTrees(right, tree) * 
                    CountTrees(up.AsEnumerable().Reverse().ToList(), tree) * 
                    CountTrees(down, tree);
                if (scenicScore > bestScenicScore)
                    bestScenicScore = scenicScore;
            }
        }

        Console.WriteLine(countVisible);
        Console.WriteLine(bestScenicScore);
    }

    int CountTrees(List<int> trees, int tree)
    {
        var count = 0;
        foreach (var t in trees)
        {
            count++;
            if (t >= tree)
                break;
        }

        return count;
    }
}