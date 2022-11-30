class Day23 : IDayCommand
{
  public string Execute()
  {
    var lines = new FileReader(23).Read().ToList();
    var nanobots = lines.Select(l => l.Replace("pos=<", "").Replace(">", "").Replace(" r=","").Split(","))
                        .Select(n => (position: new Point3D(int.Parse(n[0]), int.Parse(n[1]), int.Parse(n[2])), radius: int.Parse(n[3])))
                        .ToList();


    // Part 01
    var largestRadius = nanobots.OrderByDescending(n => n.radius).First();
    var nanobotsWithinRadius = nanobots.Where(n => n.position.ManhattanDistance(largestRadius.position) <= largestRadius.radius).Count();

    // Part 02:
    /* 
      Shamelessly I've copied the solution from EriiKKo on Reddit. I don't think I would be able to come up with something this clever in a short period of time
      This method calculates the manhattan distance between each point and the origin and then, considering the radius, adds them to a list of overlapping segments
      The -1 serves to remove this segment from being considered
      The last bit checks for the point where the most segments overlap at the same time, giving us the solution  
    */
    var queue = new List<(decimal distance, int count)>();
    nanobots.ForEach(bot => {
      var distanceToOrigin = bot.position.ManhattanDistance(new Point3D(0 , 0, 0));
      queue.Add((Math.Max(0, distanceToOrigin - bot.radius), 1)); 
      queue.Add((1+ distanceToOrigin + bot.radius, -1));
    });

    queue.Sort();

    var maxCount = 0;
    var count = 0;
    decimal result = 0;
    foreach((var distance, var localCount) in queue)
    {
      count += localCount;
      if(count > maxCount)
      {
        result = distance;
        maxCount = count;
      }
    }

    return $"Nanobots in range of the largest radius: {nanobotsWithinRadius}, the point in range of the bots in the origin has a {result} distance over (0,0,0)";
  }
}
