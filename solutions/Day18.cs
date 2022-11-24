class Day18 : IDayCommand
{

  public const char OPEN = '.';
  public const char TREE = '|';
  public const char LUMBERYARD = '#';
  public Dictionary <string, string> processed = new(); 
  public List<char> GetAdjacent(string map, int numberOfColumns, int numberOfLines, int index)
  {
    List <(int line, int column)> differences = new (){
      ( 1,  0), ( 1,  1), ( 1, -1), ( 0,  1), ( 0, -1), (-1,  0), (-1,  1), (-1, -1),
    };

    (var line, var column) = Get2by2Coords(index, numberOfColumns);

    return differences.Where(
      d => line + d.line >= 0 && 
      line + d.line < numberOfLines && 
      column + d.column >= 0 
      && column + d.column < numberOfColumns
    ).Select(d => 
      map[GetIndex(line + d.line, column + d.column, numberOfColumns)]
    ).ToList();
  }

  public (int line, int column) Get2by2Coords(int index, int numberOfColumns)
  {
    return (index / numberOfColumns, index % numberOfColumns);
  }

  public int GetIndex(int line, int column, int numberOfColumns)
  {
    return line * numberOfColumns + column;
  }

  private char ApplyChanges(string map, int numberOfLines, int numberOfColumns, int index, char acre)
  {
    var adjacent = GetAdjacent(map, numberOfLines, numberOfColumns, index);
    return acre switch 
    {
      OPEN => adjacent.Count(c => c == TREE) >= 3? TREE : OPEN,
      TREE => adjacent.Count(c => c == LUMBERYARD) >= 3? LUMBERYARD : TREE,
      LUMBERYARD => adjacent.Count(c => c == LUMBERYARD) >= 1 && adjacent.Count(c => c == TREE) >= 1? LUMBERYARD : OPEN, 
      _ => acre,
    };
  }

  public string Process(string map, int numberOfLines, int numberOfColumns, out bool containsChain)
  {
    if(processed.ContainsKey(map)) 
    {
      containsChain = false;
      return processed[map];
    }
    var result = string.Join("", map.Select((acre, index) => ApplyChanges(map, numberOfLines, numberOfColumns, index, acre)));
    containsChain = processed.ContainsKey(result);
    processed.Add(map, result);
    return result;
  }

  public string Execute()
  {
    var lines = new FileReader(18).Read().ToList(); 
    var numberOfLines = lines.Count();
    var numberOfColumns = lines[0].Count();
    string map = lines.Aggregate("", (curr, next) => curr + next);

    // Part 01
    for (int i = 0; i < 10; i++)
    {
      map = Process(map, numberOfLines, numberOfColumns, out var containsChain);
    }
    var countPart01 = map.Select(c => c).GroupBy(c => c).ToDictionary(c => c.Key, c => c.Count());

    // Part 02
    const int remainingToProcess = 1000000000 - 10;
    for (int i = 0; i < remainingToProcess; i++)
    {
      map = Process(map, numberOfLines, numberOfColumns, out var containsChain);
      if(containsChain)
      {
        // Find the size of the chain
        List<string> chain = new List<string>();
        var currMap = map;
        while(!chain.Contains(currMap))
        {
          chain.Add(currMap);
          currMap = processed[currMap];
        } 

        // Advance to the correct index
        var index = (remainingToProcess - i - 1) % chain.Count();
        map = chain[index];
        break;
      }
    }
    var countPart02 = map.Select(c => c).GroupBy(c => c).ToDictionary(c => c.Key, c => c.Count());


    return $"resource value after 10 min is {countPart01[TREE] * countPart01[LUMBERYARD]} and after 1000000000 min is {countPart02[TREE] * countPart02[LUMBERYARD]}";
  }

}
