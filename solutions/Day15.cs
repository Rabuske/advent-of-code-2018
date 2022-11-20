using Units = System.Collections.Generic.SortedDictionary<(int y, int x), (int hp, int attackPower, char type)>;
class Day15 : IDayCommand
{
  SortedDictionary<(int y, int x), char> deadUnits = new();
  Units elves = new();
  Units goblins = new();
  SortedSet<(int y, int x)> map = new();

  public List<(int y, int x)> FindOptimalPath((int y, int x) from, (int y, int x) to)
  {
    var alreadyVisited = new HashSet<(int y, int x)>();
    var paths = new PriorityQueue<(int y, int x)[], int>();

    var initialPaths = GetAdjacentPositions(from, true);

    paths.EnqueueRange(initialPaths.Select(s => new (int y, int x)[]{s}), 1);

    do
    {
      // There is no solution       
      if (alreadyVisited.Count() == map.Count())
      {
        return new();
      }
      var currentPath = paths.Dequeue();
      var currentPosition = currentPath.Last();
      if (currentPosition == to)
      {
        return currentPath.ToList();
      }
      if (alreadyVisited.Contains(currentPosition))
      {
        continue;
      }
      alreadyVisited.Add(currentPosition);

      var nextPositions = GetAdjacentPositions(currentPosition, true).Where(pos => !alreadyVisited.Contains(pos));
      foreach (var position in nextPositions)
      {
        var newPath = currentPath.Append(position).ToArray();
        paths.Enqueue(newPath, newPath.Length);
      }
    } while (paths.Count > 0);
    return new();
  }

  public bool ProcessTurn(int y, int x, Units allies, Units enemies)
  {
    var position = (y, x);
    if (!allies.ContainsKey(position))
    {
      return false;
    }
    var thisTurnUnit = allies[position];
    if (enemies.Count() == 0) return true;

    // Check if there is already a target next to the unit
    var thisUnitAdjacentPositions = GetAdjacentPositions(position, false);
    if (!thisUnitAdjacentPositions.Any(pos => enemies.ContainsKey(pos)))
    {
      // Move
      var possiblePositionsInRange = enemies.SelectMany(t => GetAdjacentPositions(t.Key, true)).ToList();
      var possiblePaths = possiblePositionsInRange.Select(pos => FindOptimalPath(position, pos))
                                                  .Where(path => path.Count > 0)
                                                  .OrderBy(path => path.Count)
                                                  .ThenBy(path => path.Last());
      if (possiblePaths.Count() > 0)
      {
        var path = possiblePaths.First();
        var newPosition = path.First();
        allies.Remove(position);
        allies.Add(newPosition, thisTurnUnit);
        position = newPosition;
      }
    }

    // Attack
    thisUnitAdjacentPositions = GetAdjacentPositions(position, false);
    var enemiesToAttack = enemies.Where(e => thisUnitAdjacentPositions.Contains(e.Key))
                                 .OrderBy(e => e.Value.hp)
                                 .ThenBy(e => e.Key);
    if (enemiesToAttack.Count() > 0)
    {
      var enemy = enemiesToAttack.First();
      if (enemy.Value.hp - thisTurnUnit.attackPower <= 0)
      {
        deadUnits.Add(enemy.Key, enemy.Value.type);
        enemies.Remove(enemy.Key);
      }
      else
      {
        enemies[enemy.Key] = (enemy.Value.hp - thisTurnUnit.attackPower, enemy.Value.attackPower, enemy.Value.type);
      }
    }

    return false;
  }

  public List<(int y, int x)> GetAdjacentPositions((int y, int x) pos, bool filterOutUnits)
  {
    var result = new List<(int y, int x)>();

    if (map.Contains((pos.y, pos.x + 1))) result.Add((pos.y, pos.x + 1));
    if (map.Contains((pos.y, pos.x - 1))) result.Add((pos.y, pos.x - 1));
    if (map.Contains((pos.y + 1, pos.x))) result.Add((pos.y + 1, pos.x));
    if (map.Contains((pos.y - 1, pos.x))) result.Add((pos.y - 1, pos.x));

    if (filterOutUnits)
    {
      result = result.Where(position => !elves.ContainsKey(position) && !goblins.ContainsKey(position)).ToList();
    }

    return result;
  }

  public string Execute()
  {
    var lines = new FileReader(15).Read().ToArray();
    // Just set everything in the maps
    for (int y = 0; y < lines.Count(); y++)
    {
      for (int x = 0; x < lines[y].Length; x++)
      {
        switch (lines[y][x])
        {
          case '.':
            map.Add((y, x));
            break;
          case 'G':
            map.Add((y, x));
            goblins.Add((y, x), (200, 3, 'G'));
            break;
          case 'E':
            map.Add((y, x));
            elves.Add((y, x), (200, 3, 'E'));
            break;
          default:
            continue;
        }
      }
    }

    var hasFinished = false;

    int numberOfRounds = 0;

    while (!hasFinished)
    {
      var allUnits = elves.ToList().Concat(goblins.ToList()).OrderBy(d => d.Key);

      foreach (var unit in allUnits)
      {
        var allies = unit.Value.type == 'G' ? goblins : elves;
        var enemies = unit.Value.type == 'G' ? elves : goblins;
        hasFinished = ProcessTurn(unit.Key.y, unit.Key.x, allies, enemies);
        if (hasFinished)
        {
          break;
        }
      }
      numberOfRounds++;
    }
    
    var winning = goblins.Count() > 0? goblins : elves;
    var remainingPoints = winning.Select(u => u.Value.hp).Sum();
    return $"Number of rounds {numberOfRounds - 1} remaining HP {remainingPoints} \n"
           + $"Part 01: {(numberOfRounds - 1) * remainingPoints}";
  }
}
