using Dijkstra.NET.Graph.Simple;
using Dijkstra.NET.ShortestPath;

namespace AdventOfCode;

public class Day16
{
    public async Task RunAsync()
    {
        var input = await File.ReadAllLinesAsync("Day16Input.txt");

        Part1(input);
    }

    void Part1(string[] input)
    {
        var graph = new Graph();
        var valves = ParseValves(input, graph);
        ConnectTunnels(input, valves, graph);

        var start = valves.First(x => x.Name == "AA");
        var targets = valves.Where(x => x.FlowRate > 0).ToList();
        var startMove = new Move(30, 0);

        var score = FindHighestPressureReleased(start, targets, startMove, graph);
        Console.WriteLine(score);
    }

    List<Valve> ParseValves(string[] input, Graph graph)
    {
        var valves = input
            .Select(x => 
            {
                var valvePart = x.Split(';').First();
                var split = valvePart.Split(' ', 5, StringSplitOptions.RemoveEmptyEntries);
                var name = split[1];
                var flowRateTokens = split[4].Split('=').Last();
                var flowRate = int.Parse(flowRateTokens);

                var id = graph.AddNode();

                return new Valve(id, name, flowRate);
            })
            .ToList();

        return valves;
    }

    void ConnectTunnels(string[] input, List<Valve> valves, Graph graph)
    {
        var valveNameMap = valves.ToDictionary(x => x.Name);

        foreach (var (valve, text) in valves.Zip(input))
        {
            var tunnelsPart = text.Split(';').Last();
            var tunnelsText = tunnelsPart.Split(' ', 5, StringSplitOptions.RemoveEmptyEntries).Last();
            var tunnelTokens = tunnelsText.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var connection in tunnelTokens)
            {
                var toValve = valveNameMap[connection.Trim()];
                graph.Connect(valve.Id, toValve.Id, 1);
            }
        }
    }

    int FindHighestPressureReleased(Valve start, List<Valve> targets, Move previous, Graph graph)
    {
        var highestScore = 0;

        foreach (var target in targets)
        {
            var move = CalculateMove(start, target, previous, graph);
            if (move.TimeLeft <= 0)
                continue;
            
            if (move.Score > highestScore)
                highestScore = move.Score;
            
            var newTargets = targets.Where(x => x != target).ToList(); // 'target' is no longer a target, since we've turned on that valve
            var score = FindHighestPressureReleased(target, newTargets, move, graph);
            if (score > highestScore)
                highestScore = score;
        }

        return highestScore;
    }

    Move CalculateMove(Valve from, Valve to, Move previous, Graph graph)
    {
        var shortestPath = graph.Dijkstra(from.Id, to.Id);
        
        var timeLeft = previous.TimeLeft - shortestPath.Distance - 1; // - 1 since it costs 1 minute to turn on the valve
        var score = previous.Score + (to.FlowRate * timeLeft);

        return new Move(timeLeft, score);
    }

    public record Move(int TimeLeft, int Score);

    public record Valve(uint Id, string Name, int FlowRate);
    
    public record Tunnel(string FromName, List<string> ToNames);
}