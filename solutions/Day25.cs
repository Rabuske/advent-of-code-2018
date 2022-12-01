class Day25 : IDayCommand
{

  public int ManhattanDistance((int, int, int, int) a, (int, int, int, int) b)
  {
    return Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2) + Math.Abs(a.Item3 - b.Item3) + Math.Abs(a.Item4 - b.Item4);
  }

  public string Execute()
  {
    var points = new FileReader(25).Read()
                                   .Select(l => l.Split(",").Select(p => int.Parse(p)).ToArray())
                                   .Select(p => (p[0], p[1], p[2], p[3]))
                                   .ToList();

    List<List<(int, int, int, int)>> constellations = new();
    HashSet<(int, int, int, int)> seen = new();

    while(seen.Count < points.Count)
    {
      // Start a new constellation
      var next = points.Where(p => !seen.Contains(p)).First();
      var constellation = new List<(int, int, int, int)>();
      constellations.Add(constellation);

      // Build the constellation
      BuildConstellation(next, constellation, seen, points);
    }
    
    return $"The number of constellations is {constellations.Count}";
  }

  private void BuildConstellation((int, int, int, int) next, List<(int, int, int, int)> constellation, HashSet<(int, int, int, int)> seen, List<(int, int, int, int)> points)
  {
    if(seen.Contains(next)) return;
    seen.Add(next);
    constellation.Add(next);
    var closePoints = points.Where(p => !seen.Contains(p) && ManhattanDistance(p, next) <= 3);
    foreach(var closePoint in closePoints)
    {
      BuildConstellation(closePoint, constellation, seen, points);
    }
  }
}
