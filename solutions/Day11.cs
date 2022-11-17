class Day11 : IDayCommand
{
  public string Execute()
  {
    long gridSerialNumber = int.Parse(new FileReader(11).Read().First());
    var fuelCells = new int[300,300];
    long total = 0;

    for (int y = 1; y <= 300; y++)
    {
      for (int x = 1; x <= 300; x++)
      {
        long rackId = x + 10;
        long powerLevel = rackId * y;
        powerLevel = powerLevel + gridSerialNumber;
        powerLevel = powerLevel * rackId;
        powerLevel = (powerLevel % 1000) / 100;
        powerLevel = powerLevel - 5;
        fuelCells[x - 1, y - 1] = (int)powerLevel;
        total += powerLevel;
      }
    }
    
    // Part 01
    var power3x3 = new List<(int x, int y, int totalPower)>();
    for (int y = 0; y < 298; y++)
    {
      for (int x = 0; x < 298; x++)
      {
        var totalPower = fuelCells[x, y + 0] + fuelCells[x + 1, y + 0] + fuelCells[x + 2, y + 0] +
                         fuelCells[x, y + 1] + fuelCells[x + 1, y + 1] + fuelCells[x + 2, y + 1] +
                         fuelCells[x, y + 2] + fuelCells[x + 1, y + 2] + fuelCells[x + 2, y + 2];
        power3x3.Add((x + 1, y + 1, totalPower));
      }
    }
    
    // Part 02 brute force
    var powers = new List<(int x, int y, int totalPower, int size)>();
    for (int y = 0; y < 300; y++)
    {
      for (int x = 0; x < 300; x++)
      {
        var calculatedPower = 0;
        var maxSquareSize = 300 - Math.Max(x, y);
        for(int squareSize = 1; squareSize <= maxSquareSize; squareSize++) 
        {
          for (int i = 0; i < squareSize; i++)
          {
            if(i != squareSize - 1) 
            {
              calculatedPower += fuelCells[x + squareSize - 1, y + i];              
            }
            calculatedPower += fuelCells[x + i, y + squareSize - 1];
          }
          powers.Add((x + 1, y + 1, calculatedPower, squareSize));
        }
      }
    }
    
    var maxPower3x3 = power3x3.OrderByDescending(x => x.totalPower).First();
    var maxPower = powers.OrderByDescending(x => x.totalPower).First();
    return $"The coords for the 3x3 cell is ({maxPower3x3.x},{maxPower3x3.y}), the other one is ({maxPower.x},{maxPower.y}) size {maxPower.size}";

  }
}
