class OperationProgram
{
  public OperationProgram(List<(string name, long a, long b, long c)> instructions)
  {
    Instructions = instructions;
  }

  public int IndexOfInstructionPointer {get; set;}

  public int InstructionPointer {get; set;}

  public long[] Registers {get; set;} = Enumerable.Repeat(0L, 6).ToArray();

  public static Dictionary<string, Action<long[], long, long, long>>  Operations {get; set;} = OperationBuilder.GetOperationsLong();

  public List<(string name, long a, long b, long c)> Instructions {get; init;} = new();

  public void ExecuteCycle()
  {
    var instruction = Instructions[InstructionPointer];
    var operation = Operations[instruction.name];
    Registers[IndexOfInstructionPointer] = InstructionPointer; 
    operation(Registers, instruction.a, instruction.b, instruction.c);
    InstructionPointer = (int) Registers[IndexOfInstructionPointer];
    InstructionPointer++;
  }

  public override string ToString()
  {
    var nextInstruction = Instructions[InstructionPointer];
    return $" Next: {nextInstruction} | Registers: [{string.Join(",", Registers)}]";
  }

}