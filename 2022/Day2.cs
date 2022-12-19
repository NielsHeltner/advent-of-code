namespace AdventOfCode;

public class Day2
{
    public async Task RunAsync()
    {
        var input = await File.ReadAllLinesAsync("Day2Input.txt");
        var mappedInputs = input.Select(x => 
        {
            var tokenized = x.Split(" ", 2);
            var parsed = MapInput(tokenized[0], tokenized[1]);
            return parsed;
        });

        var scores = mappedInputs.Select(x => CalculateScore(x));

        Console.WriteLine(scores.Sum());

        var mappedInputs2 = input.Select(x => 
        {
            var tokenized = x.Split(" ", 2);
            var parsed = MapInput2(tokenized[0], tokenized[1]);
            return parsed;
        });

        var scores2 = mappedInputs2.Select(x => CalculateScore2(x));

        Console.WriteLine(scores2.Sum());
    }

    private int CalculateScore(Input input)
    {
        var weaponScore = CalculateWeaponScore(input.Me);
        var outcome = CalculateOutcome(input);
        var outcomeScore = CalculateOutcomeScore(outcome);

        return weaponScore + outcomeScore;
    }

    private int CalculateScore2(Input2 input)
    {
        var meWeapon = CalculateWeapon(input);

        var weaponScore = CalculateWeaponScore(meWeapon);
        var outcomeScore = CalculateOutcomeScore(input.Outcome);

        return weaponScore + outcomeScore;
    }

    private Outcome CalculateOutcome(Input input)
    {
        if (input.Opponent == input.Me)
            return Outcome.Draw;
        
        return input switch
        {
            (Weapon.Rock, Weapon.Scissors) => Outcome.Opponent,
            (Weapon.Scissors, Weapon.Paper) => Outcome.Opponent,
            (Weapon.Paper, Weapon.Rock) => Outcome.Opponent,
            _ => Outcome.Me
        };
    }

    private Weapon CalculateWeapon(Input2 input)
    {
        if (input.Outcome == Outcome.Draw)
            return input.Opponent;
        
        return input switch
        {
            (Weapon.Rock, Outcome.Opponent) => Weapon.Scissors,
            (Weapon.Paper, Outcome.Me) => Weapon.Scissors,
            (Weapon.Paper, Outcome.Opponent) => Weapon.Rock,
            (Weapon.Scissors, Outcome.Me) => Weapon.Rock,
            _ => Weapon.Paper
        };
    }

    private int CalculateOutcomeScore(Outcome outcome)
    {
        return outcome switch
        {
            Outcome.Draw => 3,
            Outcome.Me => 6,
            Outcome.Opponent => 0,
            _ => throw new ArgumentException(nameof(outcome))
        };
    }

    private int CalculateWeaponScore(Weapon weapon)
    {
        return weapon switch
        {
            Weapon.Rock => 1,
            Weapon.Paper => 2,
            Weapon.Scissors => 3,
            _ => throw new ArgumentException(nameof(weapon))
        };
    }

    private Input MapInput(string opponent, string me)
    {
        var opponentWeapon = opponent switch
        {
            "A" => Weapon.Rock,
            "B" => Weapon.Paper,
            "C" => Weapon.Scissors,
            _ => throw new ArgumentException(opponent)
        };

        var meWeapon = me switch
        {
            "X" => Weapon.Rock,
            "Y" => Weapon.Paper,
            "Z" => Weapon.Scissors,
            _ => throw new ArgumentException(me)
        };

        return new Input(opponentWeapon, meWeapon);
    }

    private Input2 MapInput2(string opponent, string outcome)
    {
        var opponentWeapon = opponent switch
        {
            "A" => Weapon.Rock,
            "B" => Weapon.Paper,
            "C" => Weapon.Scissors,
            _ => throw new ArgumentException(opponent)
        };

        var outc = outcome switch
        {
            "X" => Outcome.Opponent,
            "Y" => Outcome.Draw,
            "Z" => Outcome.Me,
            _ => throw new ArgumentException(outcome)
        };

        return new Input2(opponentWeapon, outc);
    }

    internal record Input(Weapon Opponent, Weapon Me);

    internal record Input2(Weapon Opponent, Outcome Outcome);

    internal enum Weapon
    {
        Rock,
        Paper,
        Scissors
    }

    internal enum Outcome
    {
        Draw,
        Me,
        Opponent
    }
}