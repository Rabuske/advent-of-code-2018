using Units = System.Collections.Generic.Dictionary<(int line, int column), (int hp, int attackPower, char type)>;
class Day15 : IDayCommand
{
  Units units = new();
  SortedSet<(int line, int column)> map = new();

  public List<((int line, int column) target, List<(int line, int column)> path)> FindOptimalPath((int line, int column) from, List<(int line, int column)> inRange)
  {
    Queue<(int line, int column)> queue = new Queue<(int line, int column)>();
    Dictionary<(int fromLine, int fromColumn), (int toLine, int toColumn)> vertices = new();
    queue.Enqueue(from);
    vertices.Add(from, (-1, -1));
    while (queue.Count > 0)
    {
      var currentPosition = queue.Dequeue();
      var adjacent = GetAdjacentPositions(currentPosition, true);
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

    List<(int x, int y)> getPath(int destLine, int destColumn)
    {
      var position = (destLine, destColumn);
      if (!vertices.ContainsKey(position)) return new();
      List<(int line, int column)> path = new();
      while (position != from)
      {
        path.Add(position);
        position = vertices[position];
      }

      path.Reverse();
      return path;
    }
 
    return inRange.Select(t => (target: t, path: getPath(t.line, t.column)))
                  .Where(t => t.path.Count > 0)
                  .OrderBy(t => t.path.Count)
                  .ThenBy(t => t.target)
                  .ToList();
  }

  public bool ProcessTurn(int line, int column, bool stopOnElfDeath)
  {
    var position = (line, column);
    if (!units.ContainsKey(position))
    {
      return false;
    }
    var thisTurnUnit = units[position];
    var enemies = units.Where(u => u.Value.type != thisTurnUnit.type).ToDictionary(u => u.Key, u => u.Value);
    if (enemies.Count() == 0) return true;

    // Check if there is already a target next to the unit
    var thisUnitAdjacentPositions = GetAdjacentPositions(position, false);
    if (!thisUnitAdjacentPositions.Any(pos => enemies.ContainsKey(pos)))
    {
      // Move
      var positionsInRange = enemies.SelectMany(t => GetAdjacentPositions(t.Key, true)).ToList();
      var bestPaths = FindOptimalPath(position, positionsInRange);
      var bestPath = bestPaths.FirstOrDefault().path;

      if (bestPath is not null)
      {
        var newPosition = bestPath[0];
        units.Remove(position);
        units.Add(newPosition, thisTurnUnit);
        position = newPosition;
      }
      else
      {
        return false;
      }
    }

    // Attack
    thisUnitAdjacentPositions = GetAdjacentPositions(position, false);
    var enemiesToAttack = enemies.Where(e => thisUnitAdjacentPositions.Contains(e.Key))
                                 .OrderBy(e => e.Value.hp)
                                 .ThenBy(e => e.Key)
                                 .ToList();
    if (enemiesToAttack.Count() > 0)
    {
      var enemy = enemiesToAttack.First();
      if (enemy.Value.hp - thisTurnUnit.attackPower <= 0)
      {
        units.Remove(enemy.Key);
        if(enemy.Value.type == 'E' && stopOnElfDeath)
        {
          return true;
        }
      }
      else
      {
        units[enemy.Key] = (enemy.Value.hp - thisTurnUnit.attackPower, enemy.Value.attackPower, enemy.Value.type);
      }
    }

    return false;
  }

  public List<(int line, int column)> GetAdjacentPositions((int line, int column) pos, bool filterOutUnits)
  {
    var result = new List<(int line, int column)>();

    if (map.Contains((pos.line, pos.column + 1))) result.Add((pos.line, pos.column + 1));
    if (map.Contains((pos.line, pos.column - 1))) result.Add((pos.line, pos.column - 1));
    if (map.Contains((pos.line + 1, pos.column))) result.Add((pos.line + 1, pos.column));
    if (map.Contains((pos.line - 1, pos.column))) result.Add((pos.line - 1, pos.column));

    if (filterOutUnits)
    {
      result = result.Where(position => !units.ContainsKey(position)).ToList();
    }
    return result.OrderBy(r => r).ToList();
  }

  private int PlayTheGame(bool stopOnElfDeath)
  {
    int fullRoundsCompleted = 0;
    var hasFinished = false;
    for (fullRoundsCompleted = 0; !hasFinished; fullRoundsCompleted++)
    {
      //Print(lines.Count(), lines[0].Length, fullRoundsCompleted);     
      var unitsToProcess = units.OrderBy(u => u.Key).ToList();
      foreach (var unit in unitsToProcess)
      {
        hasFinished = ProcessTurn(unit.Key.line, unit.Key.column, stopOnElfDeath: stopOnElfDeath);
        if (hasFinished)
        {
          return fullRoundsCompleted;
        }
      }
    }
    return 0;
  }

  private void Print(int maxY, int maxX, int numberOfRounds)
  {
    Console.WriteLine($"--------------- Turn {numberOfRounds}");
    for (int line = 0; line < maxY; line++)
    {
      for (int column = 0; column < maxX; column++)
      {
        var position = (line, column);
        if (units.ContainsKey(position))
        {
          Console.Write(units[position].type);
          continue;
        }
        if (map.Contains(position))
        {
          Console.Write(".");
          continue;
        }
        Console.Write("#");
      }
      Console.WriteLine();
    }
    foreach (var unit in units)
    {
      Console.WriteLine($"{unit.Value.type}({unit.Key.line},{unit.Key.column})({unit.Value.hp})");
    }
  }

  public string Execute()
  {
    var lines = new FileReader(15).Read().ToArray();
    // Just set everything in the maps
    for (int line = 0; line < lines.Count(); line++)
    {
      for (int column = 0; column < lines[line].Length; column++)
      {
        switch (lines[line][column])
        {
          case '.':
            map.Add((line, column));
            break;
          case 'G':
            map.Add((line, column));
            units.Add((line, column), (200, 3, 'G'));
            break;
          case 'E':
            map.Add((line, column));
            units.Add((line, column), (200, 3, 'E'));
            break;
          default:
            continue;
        }
      }
    }
    
    Units originalUnits = units.ToDictionary(u => u.Key, u => u.Value);

    // Part 01
    int fullRoundsCompletedPart01 = PlayTheGame(stopOnElfDeath: false);
    var remainingPointsPart01 = units.Select(u => u.Value.hp).Sum();

    // Part 02
    int fullRoundsCompletedPart02 = 0;
    var currentElfPower = 4;
    do
    {
      units = originalUnits.ToDictionary(u => u.Key, u => (u.Value.hp, u.Value.type == 'E'? currentElfPower : u.Value.attackPower, u.Value.type));
      fullRoundsCompletedPart02 = PlayTheGame(true);    
      currentElfPower++;
    } while(units.Where(u => u.Value.type == 'E').Count() != originalUnits.Where(u => u.Value.type == 'E').Count());

    var remainingPointsPart02 = units.Select(u => u.Value.hp).Sum();

    return   $"Part 01: Number of rounds {fullRoundsCompletedPart01} remaining HP {remainingPointsPart01} \n"
           + $"Part 01: {(fullRoundsCompletedPart01) * remainingPointsPart01} \n"
           + $"Part 02: Number of rounds {fullRoundsCompletedPart02} remaining HP {remainingPointsPart02} \n"
           + $"Part 02: {(fullRoundsCompletedPart02) * remainingPointsPart02}";
  }  
}
