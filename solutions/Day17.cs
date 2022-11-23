
enum WaterType 
{
  STILL,
  MOVING
}
class Day17 : IDayCommand
{
  public string Execute()
  {
    var clay = new FileReader(17).Read().SelectMany(line =>
    {
      var parts = line.Split(", ");
      var startingAxis = parts[0][0];
      var startingCoord = int.Parse(parts[0].Substring(2));

      var movingAxis = parts[1][0];
      var movingCoords = parts[1].Substring(2).Split("..").Select(n => int.Parse(n)).ToArray();

      var clayPoints = new List<Point2D>();
      for (int i = movingCoords[0]; i <= movingCoords[1]; i++)
      {
        if (startingAxis == 'x')
        {
          clayPoints.Add(new Point2D(startingCoord, i));
        }
        else
        {
          clayPoints.Add(new Point2D(i, startingCoord));
        }
      }
      return clayPoints;
    }).ToHashSet();

    // Fill water
    var waterPoints = new Dictionary<Point2D, WaterType>();
    var movingWater = new List<Point2D>();
    var generationPath = new Dictionary<Point2D, Point2D>();
    
    movingWater.Add(new Point2D(500, 0));

    var lowerPoint = clay.Max(p => p.y);
    var highestPoint = clay.Min(p => p.y);

    bool isStillWater(Point2D point) => waterPoints.ContainsKey(point) && waterPoints[point] == WaterType.STILL;
    bool isFree(Point2D point) => !waterPoints.ContainsKey(point) && !clay.Contains(point);
    bool isFreeOrMoving(Point2D point) => !clay.Contains(point) && (!waterPoints.ContainsKey(point) || waterPoints[point] == WaterType.MOVING);

    while (movingWater.Count > 0)
    {
      var currWater = movingWater.Last();
      var down  = currWater + new Point2D(0, 1);
      var left  = currWater + new Point2D(-1, 0);
      var right = currWater + new Point2D(1, 0);

      // Add this point to the water points (as moving water for now)
      waterPoints.TryAdd(currWater, WaterType.MOVING);
      
      //Print(clay, waterPoints);

      // If down is bellow the limit, it means this flow is infinite - prevent all previous sources from being processed
      if(down.y > lowerPoint)
      {
        while(currWater != null) 
        {
          movingWater.Remove(currWater);
          currWater = generationPath.TryGetValue(currWater, out currWater)? currWater : null;
        }
        continue;
      }

      // If down is free - add a new down point
      if(isFree(down))
      {
        movingWater.Add(down);
        generationPath.Add(down, currWater);
        continue;
      }      
      
      // If down is blocked, or there is still water, try to move left or right
      if(clay.Contains(down) || isStillWater(down))
      {
        var newWaterPointsLeft = new List<Point2D>();
        var newWaterPointsRight = new List<Point2D>();

        // Check if left could exist
        GetNextPoints(clay, waterPoints, left, newWaterPointsLeft, new Point2D(-1, 0));

        // Check if right could exist
        GetNextPoints(clay, waterPoints, right, newWaterPointsRight, new Point2D(1, 0));

        // First Point
        var firstPoint = newWaterPointsLeft.LastOrDefault();
        var isFlowingLeft = firstPoint is not null;
        if(firstPoint is not null)
        {
          var firstPointDown = firstPoint + new Point2D(0 ,1);
          var firstPointLeft = firstPoint + new Point2D(-1 ,0);
          isFlowingLeft = isFreeOrMoving(firstPointDown) || isFreeOrMoving(firstPointLeft);
        }

        // Last Point
        var lastPoint = newWaterPointsRight.LastOrDefault();
        var isFlowingRight = lastPoint is not null;
        if(lastPoint is not null)
        {
          var lastPointDown = lastPoint + new Point2D(0 ,1);
          var lastPointRight = lastPoint + new Point2D(1 ,0);
          isFlowingRight = isFreeOrMoving(lastPointDown) || isFreeOrMoving(lastPointRight);
        }

        // Add or update existing points
        newWaterPointsLeft.Concat(newWaterPointsRight).ToList().ForEach(wp => {
          var waterType = isFlowingLeft || isFlowingRight? WaterType.MOVING : WaterType.STILL;
          generationPath.TryAdd(wp, currWater);
          waterPoints.TryAdd(wp, waterType);
          waterPoints[wp] = waterType;
        });

        // Update the "worklist"
        if(!isFlowingLeft && !isFlowingRight)
        {
          waterPoints[currWater] = WaterType.STILL;
          movingWater.Remove(currWater);
        }

        if(isFlowingLeft && firstPoint is not null && isFree(firstPoint + new Point2D(0, 1)))
        {
          movingWater.Add(firstPoint);
        }

        if(isFlowingRight && lastPoint is not null && isFree(lastPoint + new Point2D(0, 1)))
        {
          movingWater.Add(lastPoint);
        }
        if(movingWater.Last() == currWater)
        {
          movingWater.Remove(currWater);
        }
      }
      movingWater.Remove(currWater);
    }

    //Print(clay, waterPoints);

    var tilesWithWater = waterPoints.Where(w => w.Key.y >= highestPoint && w.Key.y <= lowerPoint).Count(); // -1 because of the source
    var tilesWithStillWater = waterPoints.Where(w => w.Key.y >= highestPoint && w.Key.y <= lowerPoint && w.Value == WaterType.STILL).Count();

    return $"Number of tiles with water: {tilesWithWater}, number of tiles with still water {tilesWithStillWater}";
  }

  private void GetNextPoints(HashSet<Point2D> clay, 
                                    Dictionary<Point2D, WaterType> waterPoints, 
                                    Point2D point, 
                                    List<Point2D> newWaterPoints, 
                                    Point2D nextDifferencePoint)
  {
    var currPoint = point;
    while (!clay.Contains(currPoint))
    {
      newWaterPoints.Add(currPoint);
      var currPointDown = currPoint + new Point2D(0, 1);
      var waterPointDownIsMovingOrEmpty = !waterPoints.ContainsKey(currPointDown) || waterPoints[currPointDown] == WaterType.MOVING;
      if (!clay.Contains(currPointDown) && waterPointDownIsMovingOrEmpty)
      {
        return;
      }
      currPoint = currPoint + nextDifferencePoint;
    }

    return;
  }

  private static void Print(HashSet<Point2D> clay, Dictionary<Point2D, WaterType> waterPoints)
  {
    var minX = Math.Min(clay.Min(c => c.x), waterPoints.Keys.Min(w => w.x));
    var maxX = Math.Max(clay.Max(c => c.x), waterPoints.Keys.Max(w => w.x));
    var minY = Math.Min(clay.Min(c => c.y), waterPoints.Keys.Min(w => w.y));
    var maxY = Math.Max(clay.Max(c => c.y), waterPoints.Keys.Max(w => w.y));

    for (var y = minY - 1; y <= maxY + 1; y++)
    {
      for (var x = minX - 1; x <= maxX + 1; x++)
      {
        var point = new Point2D(x, y);
        if (clay.Contains(point))
        {
          Console.Write("#");
          continue;
        }
        if (waterPoints.ContainsKey(point))
        {
          Console.Write(waterPoints[point] == WaterType.MOVING? '|' : '~');
          continue;
        }
        Console.Write(".");
      }
      Console.WriteLine();
    }
    Console.WriteLine("------------------------------");
  }
}