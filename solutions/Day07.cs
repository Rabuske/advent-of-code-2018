
class Node
{
  public char Id { init; get; }
  public List<Node> Prerequisites { set; get; } = new List<Node>();

  public bool IsProcessed { set; get; } = false;

  public bool CanBeProcessed => !IsProcessed && (Prerequisites.Count == 0 || Prerequisites.All(node => node.IsProcessed));

  public int ProcessingTime => -4 + (int)Id;

}

class Day07 : IDayCommand
{

  public Dictionary<char, Node> GeneratedNodes(List<string> lines)
  {
    Dictionary<char, Node> nodes = new();
    foreach (var line in lines)
    {
      char step = line[5];
      char nextStep = line[36];

      if (!nodes.ContainsKey(step)) nodes.Add(step, new Node() { Id = step });
      if (!nodes.ContainsKey(nextStep)) nodes.Add(nextStep, new Node() { Id = nextStep });

      nodes[nextStep].Prerequisites.Add(nodes[step]);
    }
    return nodes;
  }

  public string Execute()
  {

    Dictionary<char, Node> nodes = GeneratedNodes(new FileReader(7).Read().ToList());
    List<Node> processedNodes = new List<Node>();
    int totalProcessingTime = 0;
    List<(int, Node)> workQueue = new();

    while (processedNodes.Count < nodes.Count)
    {
      var nodesToProcess = nodes.Values.Where(node => node.CanBeProcessed && !workQueue.Select(item => item.Item2).Contains(node)).OrderBy(node => node.Id).ToList();
      var nextNode = nodesToProcess.Count > 0 ? nodesToProcess[0] : null;

      // Distribute Work while possible
      if (nextNode is not null && workQueue.Count < 5)
      {
        workQueue.Add((nextNode.ProcessingTime, nextNode));
        continue;
      }

      // "Process" the next event in the work queue
      var nextProcess = workQueue.OrderBy(item => item.Item1).First();
      workQueue.Remove(nextProcess);
      totalProcessingTime += nextProcess.Item1;

      // Update elapsing times of workers
      workQueue = workQueue.Select(item => ((item.Item1 - nextProcess.Item1), item.Item2)).ToList();
      processedNodes.Add(nextProcess.Item2);
      nextProcess.Item2.IsProcessed = true;
    }

    var order = string.Join("", processedNodes.Select(node => node.Id));

    return $"The execution order is {order} and the total elapsed time is {totalProcessingTime}";

  }
}
