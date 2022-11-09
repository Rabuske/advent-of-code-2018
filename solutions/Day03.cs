class Day03 : IDayCommand
{
  public string Execute()
  {
    var lines = new FileReader(3).Read();
    var separatedLines = lines.Select(line => line.Replace(",", " ").Replace(":", "").Replace("x", " ").Split(" "));
    var listOfClaims = new List<string>();

    // Dumb first idea: keep track of all the points and the number of entries that add to them
    Dictionary<Point2D, HashSet<string>> points = new Dictionary<Point2D, HashSet<string>>();
    foreach (var line in separatedLines)
    {
      var startingPointX = int.Parse(line[2]);
      var startingPointY = int.Parse(line[3]);
      var sizeX = int.Parse(line[4]);
      var sizeY = int.Parse(line[5]);
      var claimNumber = line[0];
      listOfClaims.Add(claimNumber);
      var startPoint = new Point2D(startingPointX, startingPointY);
      for (int i = 0; i < sizeX; i++)
      {
        for (int j = 0; j < sizeY; j++)
        {
          var newPoint = startPoint + new Point2D(i, j);
          var exists = points.TryGetValue(newPoint, out var numberOfEntries);
          if (exists)
          {
            points[newPoint].Add(claimNumber);
          }
          else
          {
            points.Add(newPoint, new HashSet<string> { claimNumber });
          }
        }
      }
    }

    var claimsWithOverlappingSquares = new HashSet<string>();
    var numberOfSquaresWithMoreThanOneClaim = points.Values.Where(v => v.Count > 1).Count(claims =>
    {
      foreach (var claim in claims)
      {
        claimsWithOverlappingSquares.Add(claim);
      }
      return true;
    });

    listOfClaims = listOfClaims.Except(claimsWithOverlappingSquares).ToList();
    return $"Number of squares with more than one claim {numberOfSquaresWithMoreThanOneClaim} and the only claim that is not duplicated is {listOfClaims[0]}";

  }
}