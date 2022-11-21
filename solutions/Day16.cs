class Day16 : IDayCommand
{
  public string Execute()
  {
    List<(int[] before, int[] operation, int[] after)> samples = new();

    var lines = new FileReader(16).Read().ToList();
    int i = 0;
    for (i = 0; i < lines.Count; i += 4)
    {
      if (!lines[i].StartsWith("Before:"))
      {
        break;
      }

      var before = lines[i].Replace("Before: [", "")
                           .Replace("]", "")
                           .Split(", ")
                           .Select(e => int.Parse(e))
                           .ToArray();

      var operation = lines[i + 1].Split(" ")
                                  .Select(e => int.Parse(e))
                                  .ToArray();

      var after = lines[i + 2].Replace("After:  [", "")
                              .Replace("]", "")
                              .Split(", ")
                              .Select(e => int.Parse(e))
                              .ToArray();
      samples.Add((before, operation, after));
    }

    // Read the program
    var program = lines.Skip(i)
                       .Where(s => !string.IsNullOrEmpty(s))
                       .Select(line => line.Split(" ")
                       .Select(n => int.Parse(n))
                       .ToArray()).ToList();

    var operations = OperationBuilder.GetOperations();
    var codesAndOper = new Dictionary<int, List<List<string>>>();
    foreach (var sample in samples)
    {
      var operationsThatWork = new List<string>();
      foreach ((var name, var op) in operations)
      {
        var copyOfInput = sample.before.Select(i => i).ToArray();
        op(copyOfInput, sample.operation[1], sample.operation[2], sample.operation[3]);
        if (Enumerable.SequenceEqual(copyOfInput, sample.after))
        {
          operationsThatWork.Add(name);
        }
      }
      codesAndOper.TryAdd(sample.operation[0], new());
      codesAndOper[sample.operation[0]].Add(operationsThatWork);
    }

    // Part 1
    var numberOfSamplesThatBehaveLike3OPCode = codesAndOper.Values.Select(v => v.Where(o => o.Count >= 3).Count()).Sum();

    // Part 2
    var reducedOperations = codesAndOper.Select(codeAndOper =>
    {
      var reducedResults = codeAndOper.Value.FirstOrDefault() ?? new();
      reducedResults = codeAndOper.Value.Skip(1)
                                  .Aggregate((reducedResults, nextResult) => reducedResults
                                    .Intersect(nextResult).ToList())
                                  .ToList();
      return new KeyValuePair<int, List<string>>(codeAndOper.Key, reducedResults);
    }).ToDictionary(kv => kv.Key, kv => kv.Value);

    var operationsMappingToOpCodes = new Dictionary<int, string>();

    // Reduce the possibilities to one each:
    while (reducedOperations.Values.Any(l => l.Count > 1))
    {
      foreach ((var code, var operationName) in operationsMappingToOpCodes.ToList())
      {
        reducedOperations.Where(kv => kv.Value.Contains(operationName) && kv.Key != code)
                         .ToList().ForEach(opers => opers.Value.Remove(operationName));

      }
      reducedOperations.Where(kv => kv.Value.Count == 1).ToList().ForEach(kv => operationsMappingToOpCodes.TryAdd(kv.Key, kv.Value.First()));
    }

    var registers = Enumerable.Repeat(0, 4).ToArray();
    program.ForEach(instruction =>
    {
      var operationName = operationsMappingToOpCodes[instruction[0]];
      var operationAction = operations[operationName];
      operationAction(registers, instruction[1], instruction[2], instruction[3]);
    });

    return $"Number of Samples that work is {numberOfSamplesThatBehaveLike3OPCode} and the program produces the number {registers[0]} in register 0";
  }
}
