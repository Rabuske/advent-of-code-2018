class Day10 : IDayCommand
{
  public string Execute()
  {

    var values = new FileReader(10).Read().Select(line =>
    {
      return new[] {
        line.Substring(10, 6).Trim(),
        line.Substring(17, 7).Trim(),
        line.Substring(36, 2).Trim(),
        line.Substring(40, 2).Trim(),
      };
    }).ToList();

    var lights = values.Select(coords =>
      (position: new Point2D(coords[0], coords[1]), velocity: new Point2D(coords[2], coords[3])))
      .ToList();

    var printed = false;
    long count = 0;

    while (true)
    {
      count++;
      lights = lights.Select(point => (position: point.position + point.velocity, point.velocity)).ToList();
      var minX = lights.Min(p => p.position.x);
      var maxX = lights.Max(p => p.position.x);
      var minY = lights.Min(p => p.position.y);
      var maxY = lights.Max(p => p.position.y);

      // Number 70 was improved after running the program for some times
      if (Math.Abs(maxX - minX) < 70 && Math.Abs(maxY - minY) < 70)
      {
        for (decimal y = minY - 1; y <= maxY + 1; y++)
        {
          for (decimal x = minX - 1; x <= maxX + 1; x++)
          {
            if (lights.Select(p => p.position).Contains(new Point2D(x, y)))
            {
              Console.Write("#");
            }
            else
            {
              Console.Write(".");
            }
          }
          Console.Write("\n");
        }
        Console.WriteLine($"-------- This was the iteration number {count}");
        printed = true;
      }
      else
      {
        if (printed) break;
      }
    }
    return "";
  }
}