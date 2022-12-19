using System.Text;

namespace AdventOfCode;

public class Day10
{
    public async Task RunAsync()
    {
        var input = await File.ReadAllLinesAsync("Day10Input.txt");
        var operations = new Queue<IOperation>(input
            .Select(x => Parse(x)));
        
        var signalStrengthSum = 0;

        var crtOutput = new StringBuilder();
        var crtX = 0;
        var crtY = 0;
        var x = 1;
        var cycle = 1;
        IOperation? running = null;
        while (running is not null || operations.Any())
        {
            if (cycle == 20 || (cycle > 0 && (cycle - 20) % 40 == 0))
            {
                signalStrengthSum += cycle * x;
                //Console.WriteLine($"Cycle {cycle} signal strength {cycle * x}");
            }

            if (running is null && operations.TryDequeue(out var operation))
            {
                running = operation;
            }

            var dx = Math.Abs(crtX - x);
            if (dx < 2)
            {
                crtOutput.Append("#");
            }
            else
            {
                crtOutput.Append(".");
            }

            if (running is not null)
            {
                running.CyclesLeft--;
                if (running.CyclesLeft <= 0)
                {
                    Execute(running, ref x);
                    running = null;
                }
            }

            crtX++;
            if (crtX >= CrtWdith)
            {
                crtX = 0;
                crtY++;
                crtOutput.AppendLine();
            }

            cycle++;
        }

        Console.WriteLine(crtOutput.ToString());
    }

    private void Execute(IOperation operation, ref int x)
    {
        if (operation is Add add)
        {
            x += add.V;
        }
    }

    IOperation Parse(string input)
    {
        var tokenized = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (tokenized[0] == "addx")
        {
            return new Add(){V=int.Parse(tokenized[1]), CyclesLeft = 2};
        }

        return new Noop() {CyclesLeft = 1};
    }

    const int CrtWdith = 40;

    interface IOperation{int CyclesLeft {get;set;}}
    class Add : IOperation {public int V {get;set;} public int CyclesLeft {get;set; }}
    class Noop : IOperation {public int CyclesLeft {get;set; }}
}