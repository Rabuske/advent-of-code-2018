enum Tool {
  TORCH, 
  CLIMBING_GEAR,
  NEITHER,
}


class Day22 : IDayCommand
{

  public Point2D Target { get; set; } = new Point2D(0, 0);
  public Dictionary<Point2D, (int erosionLevel, int geologicIndex, int riskLevel)> Cave = new();
  public int Depth {get; set;}

  public (int erosionLevel, int geologicIndex, int riskLevel) GenerateRegion(Point2D pos)
  {
    if (Cave.ContainsKey(pos)) return Cave[pos];
    var erosionLevel = 0;
    var geologicIndex = 0;
    if (pos != Target)
    {
      if (pos.y == 0)
      {
        geologicIndex = (int) pos.x * 16807;
      }
      else if (pos.x == 0)
      {
        geologicIndex = (int) pos.y * 48271;
      }
      else
      {
        geologicIndex = GenerateRegion(pos + new Point2D(-1,0)).erosionLevel * GenerateRegion(pos + new Point2D(0,-1)).erosionLevel;
      }
    }
    erosionLevel = (geologicIndex + Depth) % 20183;
    int riskLevel = (erosionLevel % 3);
    Cave.Add(pos, (erosionLevel, geologicIndex, riskLevel));
    return (erosionLevel, geologicIndex, riskLevel);
  }

  public List<Tool> GetValidTools(int riskLevel)
  {
    return riskLevel switch {
      0 => new(){Tool.CLIMBING_GEAR, Tool.TORCH},
      1 => new(){Tool.CLIMBING_GEAR, Tool.NEITHER},
      2 => new(){Tool.TORCH, Tool.NEITHER},
      _ => new(),
    };
  }
  
  public List<(Point2D position, int costToMove, Tool tool)> GetAdjacent(Point2D currentPosition, Tool currentTool)
  {
    var adjacentWithCost = new List<(Point2D position, int costToMove, Tool tool)>();
    var adjacent = new List<Point2D>(){
      currentPosition + new Point2D(+1, +0),
      currentPosition + new Point2D(-1, +0),
      currentPosition + new Point2D(+0, +1),
      currentPosition + new Point2D(+0, -1),
    }.Where(p => p.x >= 0 && p.y >= 0)
     .Select(p => (position: p, region: GenerateRegion(p)));

    var currentRegion = GenerateRegion(currentPosition);     
    var validToolsCurrent = GetValidTools(currentRegion.riskLevel);

    foreach(var adj in adjacent)
    {
      var validToolsAdjacent = GetValidTools(adj.region.riskLevel);

      // Target needs to have torch
      if(adj.position == Target)
      {
        validToolsAdjacent = new(){Tool.TORCH};
      } 
      // Never change tools if the region is the same type
      else if(currentRegion.riskLevel == adj.region.riskLevel) 
      {
        validToolsAdjacent = new(){currentTool};        
      }
      var totalValidTools = validToolsAdjacent.Intersect(validToolsCurrent);
      foreach (var tool in totalValidTools)
      {
        var timeSpent = tool == currentTool? 1 : 8;
        adjacentWithCost.Add((position: adj.position, timeSpent, tool));
      }      
    }
    return adjacentWithCost;
  }

  public int ReachTarget(Point2D target)
  {
    PriorityQueue<(Point2D position, Tool tool, int timeSpent),int> queue = new();
    HashSet<(Point2D, Tool)> visited = new();
    queue.Enqueue((new Point2D(0,0), Tool.TORCH, 0), 0);

    while(queue.Count > 0)
    {
      var nextRegion = queue.Dequeue();
      if(nextRegion.position == target)
      {
        return nextRegion.timeSpent;
      }
      if(visited.Contains((nextRegion.position, nextRegion.tool)))
      {
        continue;
      }
      visited.Add((nextRegion.position, nextRegion.tool));
      var adjacent = GetAdjacent(nextRegion.position, nextRegion.tool);
      queue.EnqueueRange(adjacent.Select( adj => {
        var cost = adj.costToMove + nextRegion.timeSpent;
        var toEnqueue = (position: adj.position, tool: adj.tool, timeSpent: cost);
        return (toEnqueue, toEnqueue.timeSpent);
      }));
    }

    return 0;
  }

  public string Execute()
  {
    var lines = new FileReader(22).Read().ToList();
    Depth = int.Parse(lines.First().Replace("depth: ", ""));
    var targetCoordinates = lines[1].Replace("target: ", "").Split(",").Select(n => int.Parse(n)).ToArray();
    Target = new Point2D(targetCoordinates[0], targetCoordinates[1]);

    // Part 01
    var totalRiskLevel = Enumerable.Range(0, (int)Target.y + 1)
                                   .SelectMany(y => Enumerable.Range(0, (int)Target.x + 1).Select(x => GenerateRegion(new Point2D(x,y)).riskLevel))
                                   .Sum();

    var timeToTarget = ReachTarget(Target);

    return $"The total risk level up until the target is {totalRiskLevel} and the time to go to target is {timeToTarget}";
  }
}