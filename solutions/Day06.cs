class Day06 : IDayCommand
{
  public IEnumerable<Point2D> GetEdge(Point2D min, Point2D max) {
    for (decimal x = min.x - 1; x <= max.x + 1; x++)
    {
      yield return new Point2D(x, min.y - 1);
    }
    for (decimal x = min.x - 1; x <= max.x + 1; x++)
    {
      yield return new Point2D(x, max.y + 1);
    }
    for (decimal y = min.y; y <= max.y; y++)
    {
      yield return new Point2D(min.x - 1, y);
    }
    for (decimal y = min.y; y <= max.y; y++)
    {
      yield return new Point2D(max.x + 1, y);
    }    
  }

  public IEnumerable<Point2D> GetInsideArea(Point2D min, Point2D max) 
  {
    for(decimal x = min.x; x <= max.x; x ++) 
    {
      for (decimal y = min.y; y <= max.y; y++)
      {
        yield return new Point2D(x, y);
      }
    }
  }

  public List<Point2D> GetClosest(List<Point2D> points, Point2D evalPoint)
  {
    var ordered = points.OrderBy(p => p.ManhattanDistance(evalPoint));
    if(ordered.ElementAt(0) == ordered.ElementAt(1)) return new List<Point2D>(); // If points are tied, it is counted for nothing
    return new List<Point2D> {ordered.ElementAt(0)};
  }

  public bool IsNearAllPoints(List<Point2D> points, Point2D evalPoint, decimal threshold) 
  {
    decimal sumOfDistances = 0;
    foreach (var point in points)
    {
      sumOfDistances += evalPoint.ManhattanDistance(point);
      if(sumOfDistances >= threshold) return false;
    }
    return true;
  }

  public string Execute()
  {
    var points = new FileReader(6).Read().Select(line => line.Replace(",",""))
      .Select(line => line.Split(" "))
      .Select(p => new Point2D(decimal.Parse(p[0]), decimal.Parse(p[1])))
      .ToList();

    // Initial Idea: 
    // 1) find the delimited area formed by the edge of the points
    // 2) evaluate the area immediately outside it => these are the areas that extend to infinity
    // 3) Calculate the areas inside the delimited area and count the ones closest to the points that don't extend to infinity

    var lowestPoint = new Point2D(
      points.Min(p => p.x),
      points.Min(p => p.y)
    );

    var highestPoint = new Point2D(
      points.Max(p => p.x),
      points.Max(p => p.y)
    );

    var edge = GetEdge(lowestPoint, highestPoint);
    var pointsWithInfiniteArea = edge.SelectMany(point => GetClosest(points, point))
                                     .ToList();

    var insideArea = GetInsideArea(lowestPoint, highestPoint).ToList();
    var pointsAndAreas = insideArea.SelectMany(point => GetClosest(points, point))
                                   .GroupBy(p => p)
                                   .ToDictionary(p => p.Key, p => p.Count());

    var largestArea = pointsAndAreas.Where(pointAndArea => !pointsWithInfiniteArea.Contains(pointAndArea.Key)).Max(pointsAndArea => pointsAndArea.Value);

    var regionNearAllPoints = insideArea.Where(p => IsNearAllPoints(points, p, 10000)).ToList();

    return $"Largest Area is {largestArea} region near everything has {regionNearAllPoints.Count} points";

  }
}