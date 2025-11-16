using System.Globalization;

namespace FordelingAfDage;

public record Node(string Id);

public class Graph
{
    private readonly Dictionary<Node, HashSet<Node>> _edges = new();
    private readonly Node _source;
    private readonly Node _terminal;
    private readonly HashSet<Node> _personNodes;

    public Graph(MadklubSheet madklubSheet)
    {
        int n = madklubSheet.People.Length;
        var personNodes = new Node[n];

        for (var i = 0; i < madklubSheet.People.Length; i++)
        {
            string person = madklubSheet.People[i];
            var personNode = new Node(person);
            personNodes[i] = personNode;
            if (!_edges.ContainsKey(personNode))
            {
                _edges.Add(personNode, []);
            }
        }

        _personNodes = personNodes.ToHashSet();

        var dayNodes = new Node[n];
        for (var rowIndex = 0; rowIndex < madklubSheet.RowsOfWishes.Count; rowIndex++)
        {
            var dayNode = new Node(rowIndex.ToString());
            dayNodes[rowIndex] = dayNode;
            if (!_edges.ContainsKey(dayNode))
            {
                _edges.Add(dayNode, []);
            }

            bool[] row = madklubSheet.RowsOfWishes[rowIndex];
            for (var personIndex = 0; personIndex < row.Length; personIndex++)
            {
                bool hasWished = row[personIndex];
                if (hasWished)
                {
                    var personNode = personNodes[personIndex];
                    _edges[personNode].Add(dayNode);
                }
            }
        }

        // If a person has wished nothing, they wish everything
        foreach (var person in personNodes)
        {
            if (_edges[person].Count == 0)
            {
                foreach (var dayNode in dayNodes)
                {
                    _edges[person].Add(dayNode);
                }
            }
        }

        _source = new Node("Source");
        _edges.Add(_source, []);
        foreach (Node personNode in personNodes)
        {
            _edges[_source].Add(personNode);
        }

        _terminal = new Node("Terminal");
        _edges.Add(_terminal, []);
        foreach (Node dayNode in dayNodes)
        {
            _edges[dayNode].Add(_terminal);
        }
    }

    public void PrintGraph()
    {
        foreach (var (node, neighbors) in _edges.OrderBy(kvp => kvp.Key.Id))
        {
            Console.WriteLine($"Node {node.Id}: \n{string.Join("\n", neighbors.OrderBy(neighbor => neighbor.Id))}\n");
        }
    }

    public void PrintFlow()
    {
        const string firstDayOfMadklubString = "20/11/2025";
        DateTime firstDayOfMadklub = DateTime.ParseExact(firstDayOfMadklubString, "dd/MM/yyyy", CultureInfo.InvariantCulture);

        (int maxFlow, var flowEdges) = MaxFlow();
        Console.WriteLine($"People assigned: {maxFlow}\n"); // Max Flow
        foreach ((Node from, Node to) in flowEdges.OrderBy(edge =>
                     int.TryParse(edge.To.Id, out int id) ? id : int.MaxValue))
        {
            if (!_personNodes.Contains(from)) continue;
            var indexOfMadklubDay = int.Parse(to.Id);
            var day = firstDayOfMadklub.GetFutureDateSkippingWeekends(indexOfMadklubDay);
            Console.WriteLine($"{from.Id.GetNameFromRoomNumber(), -30} {from.Id} -> {day.ToShortDateString()}");
        }
    }

    public (int maxFlow, List<(Node From, Node To)> flowEdges) MaxFlow()
    {
        // construct residual graph
        var residual = new Dictionary<Node, HashSet<Node>>();
        foreach ((Node node, var neighbors) in _edges)
        {
            if (!residual.ContainsKey(node))
            {
                residual[node] = [];
            }

            foreach (Node neighbor in neighbors)
            {
                residual[node].Add(neighbor);
                if (!residual.ContainsKey(neighbor))
                {
                    residual[neighbor] = [];
                }
            }
        }

        var flowEdges = new HashSet<(Node From, Node To)>();
        var flow = 0;

        while (Bfs(residual, out var parent))
        {
            Node current = _terminal;
            while (current != _source)
            {
                Node next = parent[current];

                if (_personNodes.Contains(next) && flowEdges.Any(edge => edge.From == next))
                {
                    var edge = flowEdges.First(edge => edge.From == next);
                    flowEdges.Remove(edge);
                }

                if (_personNodes.Contains(next))
                {
                    Console.WriteLine($"Edge constructed from {next.Id} to {current.Id}");
                }
                
                flowEdges.Add((next, current));

                residual[next].Remove(current);
                residual[current].Add(next);
                current = next;
            }

            flow++;
        }

        return (flow, flowEdges.ToList());
    }

    private bool Bfs(Dictionary<Node, HashSet<Node>> residual, out Dictionary<Node, Node> parent)
    {
        parent = [];
        var visited = new HashSet<Node> { _source };
        var queue = new Queue<Node>();
        queue.Enqueue(_source);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var neighbor in residual[current])
            {
                if (visited.Contains(neighbor))
                {
                    continue;
                }

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
                parent[neighbor] = current;

                if (neighbor == _terminal)
                {
                    return true;
                }
            }
        }

        return false;
    }
}