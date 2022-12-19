namespace AdventOfCode;

public class Day5
{
    public async Task RunAsync()
    {
        var input = await File.ReadAllTextAsync("Day5Input.txt");
        var splitInput = input.Split(new string[]{Environment.NewLine + Environment.NewLine}, 2, StringSplitOptions.RemoveEmptyEntries);
        var stacksInput = splitInput[0].Split(Environment.NewLine);
        var movesInput = splitInput[1].Split(Environment.NewLine);

        var stacks = ParseStacks(stacksInput);
        var moves = movesInput.Select(x => ParseMove(x)).ToList();

        foreach (var move in moves)
        {
            var fromStack = stacks[move.FromStack - 1];
            var toStack = stacks[move.ToStack - 1];

            var toBeMoved = new List<string>();
            for (var amount = 0; amount < move.Amount; amount++)
            {
                var crate = fromStack.Pop();
                toBeMoved.Add(crate);
            }
            foreach (var crate in toBeMoved.AsEnumerable().Reverse())
            {
                toStack.Push(crate);
            }
        }

        Console.WriteLine(string.Join("", stacks.Select(x => x.Peek().Trim(new[]{'[', ']'}))));
    }

    const int CrateLength = 4;
    private List<Stack<string>> ParseStacks(string[] stacksInput)
    {
        var reversed = stacksInput.Reverse().ToList();
        var stacks = reversed[0]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => new Stack<string>())
            .ToList();

        foreach (var line in reversed.Skip(1))
        {
            var lineTokens = line.Chunk(CrateLength).Select(x => new string(x).Trim());
            foreach (var (crate, i) in lineTokens.Select((x, i) => (x, i)))
            {
                if (!string.IsNullOrWhiteSpace(crate))
                    stacks[i].Push(crate);
            }
        }

        return stacks;
    }

    private Move ParseMove(string moveInput)
    {
        var tokens = moveInput.Split(' ', 6, StringSplitOptions.RemoveEmptyEntries);
        return new Move(int.Parse(tokens[1]), int.Parse(tokens[3]), int.Parse(tokens[5]));
    }

    record Move(int Amount, int FromStack, int ToStack);
}