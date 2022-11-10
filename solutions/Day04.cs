class Guard
{
  public int Id { init; get; }
  public List<(DateTime begin, DateTime end)> SleepingTimes { init; get; }
  private DateTime previousBeginning;

  public Guard(int id)
  {
    this.Id = id;
    this.SleepingTimes = new List<(DateTime begin, DateTime end)>();
  }

  public void AddAction(DateTime dateTime, string action)
  {
    if (action.StartsWith("falls"))
    {
      previousBeginning = dateTime;
    }

    if (action.StartsWith("wakes"))
    {
      SleepingTimes?.Add((previousBeginning, dateTime));
    }
  }

  public double GetNumberOfSleepingMinutes()
  {
    return this.SleepingTimes.Select(times => (times.end - times.begin).TotalMinutes).Sum();
  }

  public (int sleptMinute, int timesSlept) MostSleptMinute()
  {
    // There must be a smarter way to do this
    Dictionary<int, int> sleptMinutes = Enumerable.Range(0, 60).ToDictionary(i => i, i => 0);
    foreach (var sleptInterval in SleepingTimes)
    {
      var initialMinute = sleptInterval.begin.Minute;
      var finalMinute = sleptInterval.end.Minute;
      for (int i = initialMinute; i < finalMinute; i++)
      {
        sleptMinutes[i] += 1;
      }
    }
    var mostSleptMinute = sleptMinutes.OrderByDescending(sm => sm.Value).First();
    return (mostSleptMinute.Key, mostSleptMinute.Value);
  }

}

class Day04 : IDayCommand
{
  public (int guardId, int minute) Part02(Dictionary<int, Guard> guards)
  {
    var guardsAndMostSleptMinute = guards.Values.Select(guard => (guard.Id, guard.MostSleptMinute())).OrderByDescending(result => result.Item2.timesSlept).First();
    return (guardsAndMostSleptMinute.Id, guardsAndMostSleptMinute.Item2.sleptMinute);
  }

  public string Execute()
  {
    var lines = new FileReader(4).Read().Select(l => l.Replace("[", "").Replace("] ", "]").Replace("#", ""));
    var separatedLines = lines.Select(l => l.Split("]"));
    var timeline = new SortedList<DateTime, string>();
    foreach (var line in separatedLines)
    {
      var dateTime = DateTime.Parse(line[0]);
      timeline.Add(dateTime, line[1]);
    }

    var guards = new Dictionary<int, Guard>();
    var currentGuard = new Guard(0);
    foreach (var entry in timeline)
    {
      if (entry.Value.StartsWith("Guard"))
      {
        var id = int.Parse(entry.Value.Split(" ")[1]);
        currentGuard = guards.ContainsKey(id) ? guards[id] : new Guard(id);
        guards.TryAdd(id, currentGuard);
      }
      else
      {
        currentGuard.AddAction(entry.Key, entry.Value);
      }
    }

    var tiredGuard = guards.Values.OrderByDescending(g => g.GetNumberOfSleepingMinutes()).First();
    var mostSleptMinute = tiredGuard.MostSleptMinute().sleptMinute;
    var mostFrequentSleeper = Part02(guards);

    return $"The tired guard is {tiredGuard.Id} and the most slept minute is {mostSleptMinute}. The result of part 1 is {tiredGuard.Id * mostSleptMinute}\n" +
           $"The result of part 02 is {mostFrequentSleeper.guardId * mostFrequentSleeper.minute}";
  }
}