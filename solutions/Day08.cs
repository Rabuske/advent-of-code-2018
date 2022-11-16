
class Day08Node
{
  public List<Day08Node> Children { init; get; } = new();
  public List<int> Metadata { init; get; } = new();

  public List<int> TotalMetadata()
  {
    return Enumerable.Concat(Metadata, Children.SelectMany(c => c.TotalMetadata())).ToList();
  }

  public int GetValue()
  {
    if (Children.Count == 0)
    {
      return Metadata.Sum();
    }

    var value = 0;
    foreach (var childIndex in Metadata)
    {
      if (childIndex > Children.Count) continue;
      value += Children[childIndex - 1].GetValue();
    }
    return value;
  }
}

class Day08 : IDayCommand
{

  public Day08Node ReadNode(List<int> numbers)
  {
    var node = new Day08Node();

    var numberOfChildren = numbers[0];
    var numberOfMetadata = numbers[1];
    numbers.RemoveAt(0);
    numbers.RemoveAt(0);

    node.Children.AddRange(Enumerable.Range(0, numberOfChildren).Select(i => ReadNode(numbers)));
    node.Metadata.AddRange(Enumerable.Range(0, numberOfMetadata).Select(i =>
    {
      var metadata = numbers[0];
      numbers.RemoveAt(0);
      return metadata;
    }));

    return node;
  }

  public string Execute()
  {
    var numbers = new FileReader(8).Read().First().Split(" ").Select(c => int.Parse(c)).ToList();

    var root = ReadNode(numbers);

    var sumOfMetadatas = root.TotalMetadata().Sum();
    var valueOfRoot = root.GetValue();

    return $"The sum of metadata is {sumOfMetadatas}. The value of the root node is {valueOfRoot}";
  }
}