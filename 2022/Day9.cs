namespace AdventOfCode;

public class Day9
{
    public async Task RunAsync()
    {
        var input = await File.ReadAllLinesAsync("Day9Input.txt");
        var commands = input.Select(x => x.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)).ToList();

        var knots = new List<Knot>();
        for (var i = 0; i < 10; i++)
        {
            var knot = new Knot
            {
                X = 0,
                Y = 0
            };
            knots.Add(knot);
        }

        var tailPositions = new HashSet<Position>();

        foreach (var command in commands)
        {
            var direction = command[0];
            var steps = int.Parse(command[1]);

            for (var i = 0; i < steps; i++)
            {
                var head = knots.First();
                if (direction == "L")
                    head.X--;
                if (direction == "R")
                    head.X++;
                if (direction == "U")
                    head.Y++;
                if (direction == "D")
                    head.Y--;
                
                for (var j = 1; j < knots.Count; j++)
                {
                    var current = knots[j];
                    var previous = knots[j - 1];
                    var dx = previous.X - current.X;
                    var dy = previous.Y - current.Y;

                    if (Math.Abs(dx) > 1 || Math.Abs(dy) > 1)
                    {
                        current.X += Math.Sign(dx);
                        current.Y += Math.Sign(dy);
                    }
                }
                
                var tail = knots.Last();
                tailPositions.Add(new Position(tail.X, tail.Y));
            }
        }

        Console.WriteLine(tailPositions.Count);
    }

    record Knot
    {
        public int X {get;set;}
        public int Y {get;set;}
    }
    record Position(int X, int Y);
}