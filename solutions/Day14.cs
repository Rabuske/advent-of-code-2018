class Day14 : IDayCommand
{
  public string Execute()
  {
    int input = int.Parse(new FileReader(14).Read().First());
    List<int> recipes = new(){3,7};

    int elf1Index = 0;
    int elf2Index = 1;
    
    // Part 01
    while(recipes.Count < input + 10)
    {
      var elf1CurrRec = recipes[elf1Index];
      var elf2CurrRec = recipes[elf2Index];
      var resulting = elf1CurrRec + elf2CurrRec;
      recipes.AddRange(resulting.ToString().Select(c => (int)char.GetNumericValue(c)));
      elf1Index = (elf1Index + 1 + elf1CurrRec) % recipes.Count;
      elf2Index = (elf2Index + 1 + elf2CurrRec) % recipes.Count;
    }
    var part01 = string.Join("", recipes.TakeLast(10).Select(l => l.ToString()));

    // Part 02
    recipes = new(){3,7};

    elf1Index = 0;
    elf2Index = 1;    

    var inputAsString = input.ToString();
    string lastRelevantRecipe = string.Join("", Enumerable.Repeat(" ", inputAsString.Length - 2)) + "37";

    while(lastRelevantRecipe != inputAsString)
    {
      var elf1CurrRec = recipes[elf1Index];
      var elf2CurrRec = recipes[elf2Index];
      var resulting = elf1CurrRec + elf2CurrRec;
      foreach (var rec in resulting.ToString())
      {
        var asInt = (int)char.GetNumericValue(rec);
        recipes.Add(asInt);
        lastRelevantRecipe = lastRelevantRecipe.Substring(1) + asInt;
        if(lastRelevantRecipe == inputAsString) break;
      }
      elf1Index = (elf1Index + 1 + elf1CurrRec) % recipes.Count;
      elf2Index = (elf2Index + 1 + elf2CurrRec) % recipes.Count;
    }    

    var part02 = recipes.Count - inputAsString.Length;

    return $"Part 01: {part01} Part 02: {part02}";

  }
}
