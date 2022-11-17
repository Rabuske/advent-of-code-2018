using Pots = System.Collections.Generic.SortedDictionary<long, char>;
class Day12 : IDayCommand
{

  private Dictionary<string, char> generationPatterns = new();
  public void NormalizeInput(Pots input)
  {
    var firstPotIndex = input.First(pair => pair.Value == '#').Key;
    var lastPotIndex = input.Last(pair => pair.Value == '#').Key;

    var filtered = input.Where(pair => pair.Key < firstPotIndex || pair.Key > lastPotIndex);
    filtered.ToList().ForEach(element => input.Remove(element.Key));
    Enumerable.Range(1, 5).ToList().ForEach(index =>
    {
      input.Add(firstPotIndex - index, '.');
      input.Add(lastPotIndex + index, '.');
    });
  }

  public string GetNeighborhood(long potIndex, Pots input)
  {
    var result = "";
    for (long i = potIndex - 2; i <= potIndex + 2; i++)
    {
      if (input.ContainsKey(i))
      {
        result += input[i];
      }
      else
      {
        result += '.';
      }
    }
    return result;
  }

  public Pots NextGeneration(Pots pots, long generationNumber)
  {
    var input = string.Join("", pots.Values);
    var resultingPots = new Pots();
    foreach (var pot in pots)
    {
      string neighborhood = GetNeighborhood(pot.Key, pots);
      var result = generationPatterns[neighborhood];
      resultingPots.Add(pot.Key, result);
    }
    NormalizeInput(resultingPots);
    return resultingPots;
  }

  public Pots ProcessGenerations(Pots pots, long numberOfGenerations)
  {
    var currentPots = pots;
    for (long i = 0; i < numberOfGenerations; i++)
    {
      var previous = String.Join("", currentPots.Values);
      var previousFirstIndex = currentPots.First(pair => pair.Value == '#').Key;
      currentPots = NextGeneration(currentPots, i);
      var next = String.Join("", currentPots.Values);
      var nextFirstIndex = currentPots.First(pair => pair.Value == '#').Key;
      // Detected stable pattern
      if (previous == next)
      {
        var missingProcessing = numberOfGenerations - i - 1;
        var differenceBetweenIndexes = (nextFirstIndex - previousFirstIndex) * missingProcessing;
        var result = new Pots();
        foreach (var pot in currentPots)
        {
          result.Add(pot.Key + differenceBetweenIndexes, pot.Value);
        }
        return result;
      }
    }
    return currentPots;
  }

  public string Execute()
  {
    var lines = new FileReader(12).Read();
    Pots pots = new();
    var index = 0;
    foreach (var pot in lines.First().Substring(15))
    {
      pots.Add(index++, pot);
    }
    NormalizeInput(pots);

    generationPatterns = lines.Skip(2).Select(line => line.Replace("=> ", "").Split(" ")).ToDictionary(v => v[0], v => v[1][0]);

    // Part 01
    var part01Pots = ProcessGenerations(pots, 20);

    // Part 02
    var part02Pots = ProcessGenerations(pots, 50000000000);

    var potsWithPlantsSum01 = part01Pots.Where(pair => pair.Value == '#').Sum(pair => pair.Key);
    var potsWithPlantsSum02 = part02Pots.Where(pair => pair.Value == '#').Sum(pair => pair.Key);
    return $"The sum of pots with plants after 20 generations is {potsWithPlantsSum01} and after 5 billion is {potsWithPlantsSum02}";

  }
}
