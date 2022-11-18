class Cart
{
  public int X { get; set; }
  public int Y { get; set; }
  public char Direction { get; set; }
  private int intersectionControlNumber = 0;

  public int AbsolutePosition(int xSize) => Y * xSize + X;

  private static Dictionary<string, char> transitionMapping = new() {
    {"^|", '^'},
    {"v|", 'v'},
    {">-", '>'},
    {"<-", '<'},
    {"^/", '>'},
    {"^\\", '<'},
    {"v/", '<'},
    {"v\\", '>'},
    {">/", '^'},
    {">\\", 'v'},
    {"</", 'v'},
    {"<\\", '^'},
  };

  private static Dictionary<string, char> intersectionMapping = new() {
    {"^0", '<'},
    {"^1", '^'},
    {"^2", '>'},
    {"v0", '>'},
    {"v1", 'v'},
    {"v2", '<'},
    {">0", '^'},
    {">1", '>'},
    {">2", 'v'},
    {"<0", 'v'},
    {"<1", '<'},
    {"<2", '^'},
  };

  public Cart Move(char[][] map)
  {
    var nextPosition = GetNextPosition();
    var nextTrack = map[nextPosition.y][nextPosition.x];
    var nextDirection = GetNextDirection(nextTrack);
    return new Cart()
    {
      Direction = nextDirection,
      X = nextPosition.x,
      Y = nextPosition.y,
      intersectionControlNumber = intersectionControlNumber,
    };
  }

  private (int x, int y) GetNextPosition()
  {
    return Direction switch
    {
      '^' => (X, Y - 1),
      'v' => (X, Y + 1),
      '>' => (X + 1, Y),
      '<' => (X - 1, Y),
      _ => (X, Y)
    };
  }

  private char GetNextDirection(char nextTrack)
  {
    if (nextTrack == '+')
    {
      var currentKey = "" + Direction + intersectionControlNumber;
      intersectionControlNumber += 1;
      intersectionControlNumber %= 3;
      return intersectionMapping[currentKey];
    }

    return transitionMapping["" + Direction + nextTrack];
  }
}

class Day13 : IDayCommand
{
  public string Execute()
  {
    char[][] map = new FileReader(13).Read().Select(line => line.ToCharArray()).ToArray();
    SortedDictionary<int, Cart> carts = new();
    var xSize = map[0].Length;

    // Find all carts on the map and separate them
    for (int y = 0; y < map.Length; y++)
    {
      for (int x = 0; x < xSize; x++)
      {
        if ("^v><".Contains(map[y][x]))
        {
          var cart = new Cart()
          {
            Direction = map[y][x],
            X = x,
            Y = y
          };
          map[y][x] = "^v".Contains(cart.Direction) ? '|' : '-';
          carts.Add(cart.AbsolutePosition(xSize), cart);
        }
      }
    }

    SortedDictionary<int, Cart> alreadyProcessedCarts = new();
    var result = "";
    // Move carts
    while (carts.Count > 1)
    {
      while (carts.Count > 0)
      {
        var nextToProcess = carts.First();
        carts.Remove(nextToProcess.Key);
        var processed = nextToProcess.Value.Move(map);
        // Collision?
        if (carts.ContainsKey(processed.AbsolutePosition(xSize)) || alreadyProcessedCarts.ContainsKey(processed.AbsolutePosition(xSize)))
        {
          result = string.IsNullOrEmpty(result) ? $" First collision at ({processed.X}, {processed.Y}) " : result;
          carts.Remove(processed.AbsolutePosition(xSize));
          alreadyProcessedCarts.Remove(processed.AbsolutePosition(xSize));
        }
        else
        {
          alreadyProcessedCarts.Add(processed.AbsolutePosition(xSize), processed);
        }
      }
      carts = alreadyProcessedCarts;
      alreadyProcessedCarts = new();
    }

    var lastCart = carts.First().Value;

    return result + $"and the last cart stands at ({lastCart.X},{lastCart.Y})";
  }
}
