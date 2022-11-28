using Map = System.Collections.Generic.Dictionary<Point2D, System.Collections.Generic.HashSet<Point2D>>;
abstract class RegexNode : ICloneable
{
  public RegexNode? Next {get; set;}

  public abstract void SetNext(RegexNode node);
  public override string ToString()
  {
    return "" + Next;
  }

  public object Clone()
  {
    return this.MemberwiseClone();
  }
}

class RegexNodeBranch : RegexNode
{
  private int expectedNumberOfBranches = 1;
  private bool isBranching = true;
  public List<RegexNode> Branches {get; set;} = new();
  public void AddNewBranch()
  {
    expectedNumberOfBranches++;
  }

  public void CloseBranches()
  {
    isBranching = false;
  }

  public override void SetNext(RegexNode node)
  {
    if(isBranching) 
    {
      Branches.Add(node);
    }
    else
    {
      Next = node;
    }
  }

  public override string ToString()
  {
    var finalBit = CanBeByPassed? "|)" : ")";
    return $"({string.Join("|", Branches.Select(b => b.ToString()))}{finalBit}" + Next?.ToString();
  }

  public RegexNodeBranch GetCopyWithNextInBranches()
  {
    var result = (RegexNodeBranch) this.Clone();
    result.Branches = Branches.Select(branch => {
      var firstBranch = (RegexNode) branch.Clone();
      var lastBranch = firstBranch;
      while(lastBranch.Next is not null)
      {
        var nextBranch = (RegexNode) lastBranch.Next.Clone();
        lastBranch.Next = nextBranch;
        lastBranch = nextBranch;
      }

      lastBranch.Next = this.Next;

      return firstBranch;
    }).ToList();
    if(result.CanBeByPassed && this.Next is not null)
    {
      result.Branches.Add(this.Next);
    }
    return result;
  }

  public bool CanBeByPassed => expectedNumberOfBranches > Branches.Count;
  
}

class RegexNodeLiteral : RegexNode
{
  public char Direction {get; set;}

  public override void SetNext(RegexNode node)
  {
    Next = node;
  }

  public override string ToString()
  {
    return "" + Direction + Next?.ToString();
  }
}

class Day20: IDayCommand
{

  private RegexNode BuildRegexTree(string input)
  {
    RegexNode Regex = new RegexNodeLiteral(){ Direction = input.First()};// Assumes the first one exists is not a branch

    var currentNode = Regex;
    Stack<RegexNodeBranch> stack = new();
    foreach(char c in input.Skip(1))
    {
      if("NSWE".Contains(c)) 
      {
        var nextRegex = new RegexNodeLiteral(){ Direction = c };
        currentNode.SetNext(nextRegex);
        currentNode = nextRegex;
      }
      if(c == '(')
      {
        var nextNode = new RegexNodeBranch();
        currentNode.SetNext(nextNode);
        currentNode = nextNode;
        stack.Push(nextNode);
      }
      if(c == '|')
      {
        var branchNode = stack.Peek();
        branchNode.AddNewBranch();
        currentNode = branchNode;
        continue;
      }
      if(c == ')')
      {
        var branchNode = stack.Pop();
        branchNode.CloseBranches();
        currentNode = branchNode;
      }
    }

    return Regex;
  }

  // I was using recursion before, but got an stackoverflow exception
  private Map BuildMapFromRegex(RegexNode regex)
  {
    var map = new Map();
    var currentPosition = new Point2D(0 ,0);
    var currentRegex = regex;

    var queue = new Queue<(RegexNode, Point2D)>();
    queue.Enqueue((regex, currentPosition));
    HashSet<(RegexNode, Point2D)> processed = new();

    while(queue.Count > 0)
    {
      (currentRegex, currentPosition) = queue.Dequeue();
      // Shortcut -> if a regex node falls under the same position, we don't need to reprocess it
      if(processed.Contains((currentRegex, currentPosition)))
      {
        continue;
      }
      processed.Add(((currentRegex, currentPosition)));
      // Literal
      if(currentRegex is RegexNodeLiteral)
      {
        var literal = (RegexNodeLiteral) currentRegex;
        var pointToAdd = literal.Direction switch
        {
          'N' => new Point2D(0, -1),
          'S' => new Point2D(0, 1),
          'W' => new Point2D(-1, 0),
          'E' => new Point2D(1, 0),
          _  => new Point2D(0, 0),
        };

        var newPosition = currentPosition + pointToAdd;
        map.TryAdd(currentPosition, new());
        map[currentPosition].Add(newPosition);
        
        map.TryAdd(newPosition, new());
        map[newPosition].Add(currentPosition);

        if(literal.Next is not null)
        {
          queue.Enqueue((literal.Next, newPosition));
        }
      }
      // Branch
      else
      {
        var branch = ((RegexNodeBranch) currentRegex).GetCopyWithNextInBranches();
        branch.Branches.ForEach(b => queue.Enqueue((b, currentPosition)));
      }
    }

    return map;
  }

  // Same solution as day 15
  public List<(Point2D target, List<Point2D> path)> FindPath(Point2D from, Map map)
  {
    Queue<Point2D> queue = new Queue<Point2D>();
    Dictionary<Point2D, Point2D> vertices = new();
    queue.Enqueue(from);
    vertices.Add(from, from);
    while (queue.Count > 0)
    {
      var currentPosition = queue.Dequeue();
      var adjacent = map[currentPosition];
      foreach (var adj in adjacent)
      {
        if (vertices.ContainsKey(adj))
        {
          continue;
        }
        queue.Enqueue(adj);
        vertices.Add(adj, currentPosition);
      }
    }

    List<Point2D> getPath(Point2D destiny)
    {
      var position = destiny;
      if (!vertices.ContainsKey(position)) return new();
      List<Point2D> path = new();
      while (position != from)
      {
        path.Add(position);
        position = vertices[position];
      }

      path.Reverse();
      return path;
    }
 
    return map.Keys.Select(t => (target: t, path: getPath(t)))
                   .Where(t => t.path.Count > 0)
                   .OrderByDescending(t => t.path.Count)
                   .ToList();
  }  

  public string Execute()
  {
    var regexString = new FileReader(20).Read().First().Replace("^", "").Replace("$", "");

    // Build the regex tree
    var regex = BuildRegexTree(regexString);
    
    // Build the map based on the regex
    var map = BuildMapFromRegex(regex);

    var paths = FindPath(new Point2D(0,0), map).ToList();

    var resultPart01 = paths.First();
    var resultPart02 = paths.Where(p => p.path.Count >= 1000).Count();
    
    return $"Further room requires passing through {resultPart01.path.Count} doors the number of runs whose shortest path go at least 1000 doors is {resultPart02}";
  }

}