using System.Text;

namespace AdventOfCode;

public class Day13
{
    public async Task RunAsync()
    {
        var text = await File.ReadAllTextAsync("Day13Input.txt");
        var input = text.Split(new string[]{Environment.NewLine + Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        var pairs = input.Select(x => ParsePair(x)).ToList();

        var sumCorrectIndices = 0;
        for (var i = 0; i < pairs.Count; i++)
        {
            var pair = pairs[i];
            var result = Compare(pair.left, pair.right);
            if (result < 0)
                sumCorrectIndices += i + 1;
        }

        Console.WriteLine(sumCorrectIndices);
    }

    int Compare(IItem left, IItem right)
    {
        return (left, right) switch
        {
            (Integer leftInt, Integer rightInt) => Math.Sign(leftInt.Value - rightInt.Value),
            (List leftList, List rightList) => Compare(leftList, rightList),
            (Integer leftInt, List rightList) => Compare(new List(new List<IItem>{leftInt}), rightList),
            (List leftList, Integer rightInt) => Compare(leftList, new List(new List<IItem>{rightInt})),
            _ => throw new Exception()
        };
    }

    int Compare(List left, List right)
    {
        var leftEnum = left.Items.GetEnumerator();
        var rightEnum = right.Items.GetEnumerator();
        while (leftEnum.MoveNext() && rightEnum.MoveNext())
        {
            var result = Compare(leftEnum.Current, rightEnum.Current);
            if (result != 0)
                return result;
        }

        return Math.Sign(left.Items.Count - right.Items.Count);
    }

    Pair ParsePair(string input)
    {
        var tokens = input.Split(Environment.NewLine, 2, StringSplitOptions.RemoveEmptyEntries);
        var left = Parse(tokens[0]);
        var right = Parse(tokens[1]);

        return new Pair(left, right);
    }

    List Parse(string input)
    {
        return ParseList(new Queue<char>(input));
    }

    List ParseList(Queue<char> input)
    {
        var items = new List<IItem>();
        input.Dequeue(); // skip first [
        while (input.TryPeek(out var c) && c != ']')
        {
            if (c == '[')
                items.Add(ParseList(input));
            else if (char.IsDigit(c))
                items.Add(ParseInt(input));
            else
                input.Dequeue();
        }

        input.Dequeue();
        return new List(items);
    }

    Integer ParseInt(Queue<char> input)
    {
        var intTokens = new StringBuilder();
        while (char.IsDigit(input.Peek()))
            intTokens.Append(input.Dequeue());

        var value = int.Parse(intTokens.ToString());
        return new Integer(value);
    }
    
    public record Pair(IItem left, IItem right);
    public interface IItem {}
    record List(List<IItem> Items) : IItem;
    record Integer(int Value) : IItem;
}