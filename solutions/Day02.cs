// Naive Implementation of Part 2
class Day02 : IDayCommand
{

  public int Part1 (List<Dictionary<char,int>> countedLetters) {
    int linesWithTwoOfTheSameLetters = countedLetters.Where(line => line.Values.Contains(2)).Count();
    int linesWithThreeOfTheSameLetters = countedLetters.Where(line => line.Values.Contains(3)).Count();
    int checkSum = linesWithTwoOfTheSameLetters * linesWithThreeOfTheSameLetters;
    return checkSum;
  }

  Func<string, string, bool> hasASingleDifference = (line1, line2) => {    
    bool hasDifference = false;
    
    for(int i = 0; i < line1.Length; i++) {
      if(line1[i] != line2[i]) {
        if(hasDifference) return false;
        hasDifference = true;
      }
    }
    
    return true;
  };

  public string GetDifference(string line1, string line2) {
    string s = "";
    
    for(int i = 0; i < line1.Length; i++) {
      if(line1[i] == line2[i]) {
        s += line1[i];
      }
    }
    return s;
  }

  public string Part2 (List<string> lines) {
    for(int i = 0; i < lines.Count - 1; i++) {
      var linesWithSingleDifference = lines.Skip(i + 1).FirstOrDefault(l => hasASingleDifference(l, lines[i]));
      if(!string.IsNullOrEmpty(linesWithSingleDifference)) {        
        return GetDifference(linesWithSingleDifference, lines[i]);
      }
    }
    return "";
  }

  public string Execute()
  {
    var lines = new FileReader(2).Read().ToList();
    var countedLetters = lines.Select(line => line.GroupBy(c => c).ToDictionary(c => c.Key, g => g.Count())).ToList();

    return $"Part 1: {Part1(countedLetters)}  Part 2: {Part2(lines)}";
  }
}