namespace AdventOfCode;

public class Day11
{
    const int Rounds = 10000;
    public async Task RunAsync()
    {
        var input = await File.ReadAllTextAsync("Day11Input.txt");
        var monkeyDeclarations = input.Split(new string[]{Environment.NewLine + Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
        var monkeys = monkeyDeclarations.Select(x => Parse(x)).ToList();

        var lcm = monkeys.Select(x => x.Test).Aggregate((acc, ele) => Lcm(acc, ele));

        for (var i = 0; i < Rounds; i++)
        {
            foreach (var monkey in monkeys)
            {
                foreach (var item in monkey.Items)
                {
                    var newItem = item % lcm;
                    newItem = monkey.WorryFunc(newItem);
                    var throwIndex = monkey.TestFunc(newItem);
                    monkeys[throwIndex].Items.Add(newItem);
                    monkey.TotalThrows++;
                }

                monkey.Items.RemoveAll(_ => true);
            }
        }

        var mostActive = monkeys.OrderByDescending(x => x.TotalThrows).ToList();
        var monkeyBusiness = mostActive[0].TotalThrows * mostActive[1].TotalThrows;
        Console.WriteLine(monkeyBusiness);
    }

    Monkey Parse(string input)
    {
        var lines = input.Split(Environment.NewLine, 6, StringSplitOptions.RemoveEmptyEntries);
        var itemTokens = lines[1].Substring(lines[1].IndexOf(":") + 1).Split(',', StringSplitOptions.RemoveEmptyEntries);
        var items = itemTokens.Select(x => long.Parse(x)).ToList();
        
        var operationTokens = lines[2].Substring(lines[2].IndexOf(":") + 1).Split(' ', 5, StringSplitOptions.RemoveEmptyEntries);

        var add = (long x, long y) => x + y;
        var mult = (long x, long y) => x * y;
        var op = operationTokens[3] == "+" ? add : mult;

        var worryFunc = (long old) => {
            var y = operationTokens[4] == "old" ? old : long.Parse(operationTokens[4]);
            return op(old, y);
        };
        
        var testTokens = lines[3].Substring(lines[3].IndexOf(":") + 1).Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
        var testDivisor = int.Parse(testTokens[2]);

        var trueTokens = lines[4].Substring(lines[4].IndexOf(":") + 1).Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
        var trueIndex = int.Parse(trueTokens[3]);

        var falseTokens = lines[5].Substring(lines[5].IndexOf(":") + 1).Split(' ', 4, StringSplitOptions.RemoveEmptyEntries);
        var falseIndex = int.Parse(falseTokens[3]);

        var testFunc = (long worry) => worry % testDivisor == 0 ? trueIndex : falseIndex;

        return new Monkey(items, worryFunc, testFunc, testDivisor);
    }

    record Monkey(
        List<long> Items,
        Func<long, long> WorryFunc,
        Func<long, int> TestFunc,
        long Test){
            public long TotalThrows {get;set;}
        }
    

    static long Gfc(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    static long Lcm(long a, long b)
    {
        return (a / Gfc(a, b)) * b;
    }
}