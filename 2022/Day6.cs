namespace AdventOfCode;

public class Day6
{
    public async Task RunAsync()
    {
        var input = await File.ReadAllTextAsync("Day6Input.txt");

        var part1 = SlidingWindowUniqueChars(input, 4);
        Console.WriteLine(part1);

        var part2 = SlidingWindowUniqueChars(input, 14);
        Console.WriteLine(part2);
    }

    private static int SlidingWindowUniqueChars(string input, int length)
    {
        var checkBuffer = new HashSet<char>();
        var recentChars = new Queue<char>();

        for (var i = 0; i < length - 1; i++)
        {
            recentChars.Enqueue(input[i]);
        }

        for (var i = length - 1; i < input.Length; i++)
        {
            recentChars.Enqueue(input[i]);
            if (recentChars.All(x => checkBuffer.Add(x)))
            {
                return i + 1;
            }
            else
            {
                recentChars.Dequeue();
                checkBuffer.Clear();
            }
        }

        return -1;
    }
}