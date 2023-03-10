namespace AdventOfCode;

public class Day12
{
    public async Task RunAsync()
    {
        var input = await File.ReadAllLinesAsync("Day12Input.txt");

        Part1(input);

        Part2(input);
    }

    void Part2(string[] input)
    {
        var nodes = ParseNodes(input);

        var minSteps = int.MaxValue;

        var starts = nodes.SelectMany(x => x.Select(y => y)).Where(x => x.Value == 'a' || x.Value == 'S').ToList();
        var end = nodes.SelectMany(x => x.Select(y => y)).First(x => x.Value == 'E');
        foreach (var start in starts)
        {
            DijkstraSearch(start, end);
            var steps = GetStepCount(end);
            if (steps <= 0)
                continue;

            minSteps = Math.Min(minSteps, steps);
            foreach (var nodeList in nodes)
                foreach (var node in nodeList)
                {
                    node.PathToStart = null;
                    node.MinCostToStart = null;
                    node.Visited = false;
                }
        }

        Console.WriteLine(minSteps);
    }

    void Part1(string[] input)
    {
        var nodes = ParseNodes(input);

        var start = nodes.SelectMany(x => x.Select(y => y)).First(x => x.Value == 'S');
        var end = nodes.SelectMany(x => x.Select(y => y)).First(x => x.Value == 'E');

        DijkstraSearch(start, end);

        Console.WriteLine(GetStepCount(end));
    }

    int GetStepCount(Node end)
    {
        var shortestPath = new List<Node>();
        var current = end;
        while (current != null)
        {
            shortestPath.Add(current);
            current = current.PathToStart;
        }

        return shortestPath.Count - 1;
    }

    List<List<Node>> ParseNodes(string[] input)
    {
        var nodes = input.Select(x => x.Select(y => new Node(y)).ToList()).ToList();

        for (var rowI = 0; rowI < nodes.Count; rowI++)
        {
            var row = nodes[rowI];
            for (var colI = 0; colI < row.Count; colI++)
            {
                var node = row[colI];
                var height = ParseHeight(node.Value);

                var up = GetEdge(height, nodes, rowI, colI - 1);
                if (up != null)
                    node.Edges.Add(up);
                
                var down = GetEdge(height, nodes, rowI, colI + 1);
                if (down != null)
                    node.Edges.Add(down);
                
                var left = GetEdge(height, nodes, rowI - 1, colI);
                if (left != null)
                    node.Edges.Add(left);
                
                var right = GetEdge(height, nodes, rowI + 1, colI);
                if (right != null)
                    node.Edges.Add(right);
            }
        }

        return nodes;
    }

    void DijkstraSearch(Node start, Node end)
    {
        start.MinCostToStart = 0;
        
        var prioQueue = new List<Node>();
        prioQueue.Add(start);
        while (prioQueue.Any())
        {
            prioQueue = prioQueue.OrderBy(x => x.MinCostToStart).ToList();
            var node = prioQueue.First();
            prioQueue.Remove(node);
            foreach (var edge in node.Edges.OrderBy(x => x.Cost))
            {
                var childNode = edge.Node;
                if (childNode.Visited)
                    continue;
                
                var costToChildNode = node.MinCostToStart + edge.Cost;
                
                if (childNode.MinCostToStart == null ||
                    childNode.MinCostToStart >= costToChildNode)
                {
                    childNode.MinCostToStart = costToChildNode;
                    childNode.PathToStart = node;

                    if (!prioQueue.Contains(childNode))
                        prioQueue.Add(childNode);
                }
            }

            node.Visited = true;

            if (node == end)
                break;
        }
    }

    Edge? GetEdge(int height, List<List<Node>> nodes, int rowI, int colI)
    {
        if (rowI < 0 || rowI >= nodes.Count)
            return null;
        
        var row = nodes[rowI];
        if (colI < 0 || colI >= row.Count)
            return null;
        
        var node = row[colI];
        var cost = ParseHeight(node.Value) - height;
        if (cost > 1) // we cannot move up more than 1
            return null;
        
        return new Edge(cost, node);
    }

    int ParseHeight(char input)
    {
        if (input == 'S')
            return 'a' - 'a';
        
        if (input == 'E')
            return 'z' - 'a';

        return input - 'a';
    }

    class Node
    {
        public bool Visited{get;set;}
        public char Value {get;}
        public List<Edge> Edges {get;}
        public int? MinCostToStart {get;set;}
        public Node? PathToStart {get;set;}

        public Node(char value)
        {
            Value = value;
            Edges = new List<Edge>();
        }
    }

    class Edge
    {
        public int Cost {get;}
        public Node Node {get;}

        public Edge(
            int cost,
            Node node)
        {
            Cost = cost;
            Node = node;
        }
    }
}