class Day05 : IDayCommand
{

  public bool isMatching(char one, char two) {    
    if(one == two) return false;
    return char.ToLower(one) == char.ToLower(two);    
  }
  
  public string reduce(string polymer) {
    Stack<char> reducingMedia = new ();
    foreach (char unit in polymer)
    {
      if(reducingMedia.Count > 0 && isMatching(unit, reducingMedia.Peek())) {
        reducingMedia.Pop();
      }
      else {
        reducingMedia.Push(unit);
      }
    }
    return string.Join("", reducingMedia);
  }

  public string Execute()
  {
    var polymer = new FileReader(5).Read().First();

    // Part 01
    var reduced = reduce(polymer);

    // Part 02
    var minimum = Enumerable.Range('a', 'z')
      .Select(letterInt => (char) letterInt)
      .Select(letter => polymer
        .Replace(letter.ToString(), "")
        .Replace(letter.ToString().ToUpper(), ""))
      .Select(p => reduce(p))
      .Select(reduced => reduced.Length)
      .Min();

    return $"Size of Reduce Polymer: {reduced.Length}, minimum is {minimum}";

  }
}