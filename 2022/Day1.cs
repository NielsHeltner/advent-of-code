namespace AdventOfCode;

public class Day1
{
    public async Task RunAsync()
    {
        var input = await File.ReadAllTextAsync("Day1Input.txt");
        var elvesInput = input.Split(new string[]{Environment.NewLine + Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

        var elvesWithCalories = elvesInput.Select(x => x.Split(Environment.NewLine));

        var elvesWithCaloriesInt = elvesWithCalories.Select(x => x.Select(y => int.Parse(y)));

        var carryingMost = elvesWithCaloriesInt.Select(x => x.Sum()).OrderByDescending(x => x).Take(3).Sum();

        Console.WriteLine(carryingMost);
    }
}