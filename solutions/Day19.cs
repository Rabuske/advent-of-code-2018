class Day19 : IDayCommand
{
  public string Execute()
  {
    var lines = new FileReader(19).Read().ToList();
    var instructions = lines.Skip(1).Select(l => {
      var elements = l.Split(" ");
      return (name: elements[0], a: long.Parse(elements[1]), b: long.Parse(elements[2]), c: long.Parse(elements[3]));
    }).ToList();

    var program = new OperationProgram(instructions)
    {
      IndexOfInstructionPointer = int.Parse(lines[0].Split(" ")[1]),
    };


    // Part 01
    while(program.InstructionPointer >= 0 && program.InstructionPointer < instructions.Count)
    {
      program.ExecuteCycle();
    }
    var part01Register0 = program.Registers[0];

    // Part 02
    /*registers = Enumerable.Repeat((long)0, 6).ToArray();
    registers[0] = 1;
    instructionPointer = 0;
    while(instructionPointer >= 0 && instructionPointer < instructions.Count)
    {
      var instruction = instructions[instructionPointer];
      var operation = operations[instruction.name];
      registers[instructionPointerRegisterIndex] = instructionPointer; 
      var registersBefore = string.Join(",", registers);
      operation(registers, instruction.a, instruction.b, instruction.c);
      Console.WriteLine($"{instruction} [{registersBefore}] => [{string.Join(",", registers)}]");
      instructionPointer = (int) registers[instructionPointerRegisterIndex];
      instructionPointer++;
    }
    */
    // Via debugging I figured out that it was a program that calculated the factors of 10551267 and sum them :/
    var part02Register0 = new List<int>{1, 3, 9, 383, 1149, 3061, 3447, 9183, 27549, 1172363, 3517089, 10551267}.Sum();

    return $"The value of register 0 in part 01 is {part01Register0} and in part 02 is {part02Register0}";
  }
}
