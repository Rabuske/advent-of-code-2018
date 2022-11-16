class Day09 : IDayCommand
{
  public string Execute()
  {
    var input = new FileReader(9).Read().First().Split(" ");
    var numberOfPlayers = int.Parse(input[0]);
    var scores = Enumerable.Range(0, numberOfPlayers).Select(i => (long)0).ToArray();
    var lastMarble = int.Parse(input[6]);
    long part01HighestScore = 0;

    LinkedList<int> marbles = new();
    LinkedListNode<int> currentNode = marbles.AddFirst(0);

    for (int marbleNumber = 1; marbleNumber <= lastMarble * 100; marbleNumber++)
    {
      if (marbleNumber % 23 == 0)
      {
        for (int e = 0; e < 6; e++)
        {
          currentNode = currentNode.Previous ?? marbles.Last ?? new LinkedListNode<int>(0);
        }
        var nodeToBeDeleted = currentNode.Previous ?? marbles.Last ?? new LinkedListNode<int>(0);

        var currentPlayer = marbleNumber % numberOfPlayers;
        scores[currentPlayer] += marbleNumber;
        scores[currentPlayer] += nodeToBeDeleted.Value;

        marbles.Remove(nodeToBeDeleted);
      }
      else
      {
        currentNode = currentNode.Next ?? marbles.First ?? new LinkedListNode<int>(0);
        marbles.AddAfter(currentNode, new LinkedListNode<int>(marbleNumber));
        currentNode = currentNode.Next ?? marbles.First ?? new LinkedListNode<int>(0);
      }

      if (marbleNumber == lastMarble)
      {
        part01HighestScore = scores.Max();
      }
    }


    return $"The highest score is {part01HighestScore}, after 100 times more iterations is {scores.Max()}";

  }
}