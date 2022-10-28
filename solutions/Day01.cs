class Day01 : IDayCommand
{
  public string Part1(List<int> numbers) {
    return numbers.Sum().ToString();
  }

  public string Part2(List<int> numbers) {
    int index = 0; 
    int currentFrequency = 0;
    HashSet<int> frequencies = new HashSet<int>();
    while(true) {
      currentFrequency += numbers[index];
      if(frequencies.Contains(currentFrequency)) {
        return currentFrequency.ToString();
      }
      frequencies.Add(currentFrequency);
      index = ++index % numbers.Count;
    }
  }

  public string Execute()
  {
    List<int> numbers = new FileReader(1).Read().Select(line => int.Parse(line)).ToList();
    
    string part1 = Part1(numbers);
    string part2 = Part2(numbers);
    return $"Part 1: {part1} Part 2: {part2}";
  }
}