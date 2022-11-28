using OperationLong = System.Collections.Generic.Dictionary<string, System.Action<long[], long, long, long>>;
using OperationInt = System.Collections.Generic.Dictionary<string, System.Action<int[], int, int, int>>;

class OperationBuilder
{
  public static OperationInt GetOperations()
  {
    return new OperationInt()
    {
      {"addr", (reg, a, b, c) => { reg[c] = reg[a] + reg[b];}},
      {"addi", (reg, a, b, c) => { reg[c] = reg[a] + b; }},
      {"mulr", (reg, a, b, c) => { reg[c] = reg[a] * reg[b]; }},
      {"muli", (reg, a, b, c) => { reg[c] = reg[a] * b; }},
      {"banr", (reg, a, b, c) => { reg[c] = reg[a] & reg[b]; }},
      {"bani", (reg, a, b, c) => { reg[c] = reg[a] & b; }},
      {"borr", (reg, a, b, c) => { reg[c] = reg[a] | reg[b]; }},
      {"bori", (reg, a, b, c) => { reg[c] = reg[a] | b; }},
      {"setr", (reg, a, b, c) => { reg[c] = reg[a]; }},
      {"seti", (reg, a, b, c) => { reg[c] = a; }},
      {"gtir", (reg, a, b, c) => { reg[c] = a > reg[b] ? 1 : 0; }},
      {"gtri", (reg, a, b, c) => { reg[c] = reg[a] > b ? 1 : 0; }},
      {"gtrr", (reg, a, b, c) => { reg[c] = reg[a] > reg[b] ? 1 : 0;}},
      {"eqir", (reg, a, b, c) => { reg[c] = a == reg[b] ? 1 : 0; }},
      {"eqri", (reg, a, b, c) => { reg[c] = reg[a] == b ? 1 : 0; }},
      {"eqrr", (reg, a, b, c) => { reg[c] = reg[a] == reg[b] ? 1 : 0; }},
    };
  }

  public static OperationLong GetOperationsLong()
  {
    return new OperationLong()
    {
      {"addr", (reg, a, b, c) => { reg[c] = reg[a] + reg[b];}},
      {"addi", (reg, a, b, c) => { reg[c] = reg[a] + b; }},
      {"mulr", (reg, a, b, c) => { reg[c] = reg[a] * reg[b]; }},
      {"muli", (reg, a, b, c) => { reg[c] = reg[a] * b; }},
      {"banr", (reg, a, b, c) => { reg[c] = reg[a] & reg[b]; }},
      {"bani", (reg, a, b, c) => { reg[c] = reg[a] & b; }},
      {"borr", (reg, a, b, c) => { reg[c] = reg[a] | reg[b]; }},
      {"bori", (reg, a, b, c) => { reg[c] = reg[a] | b; }},
      {"setr", (reg, a, b, c) => { reg[c] = reg[a]; }},
      {"seti", (reg, a, b, c) => { reg[c] = a; }},
      {"gtir", (reg, a, b, c) => { reg[c] = a > reg[b] ? 1 : 0; }},
      {"gtri", (reg, a, b, c) => { reg[c] = reg[a] > b ? 1 : 0; }},
      {"gtrr", (reg, a, b, c) => { reg[c] = reg[a] > reg[b] ? 1 : 0;}},
      {"eqir", (reg, a, b, c) => { reg[c] = a == reg[b] ? 1 : 0; }},
      {"eqri", (reg, a, b, c) => { reg[c] = reg[a] == b ? 1 : 0; }},
      {"eqrr", (reg, a, b, c) => { reg[c] = reg[a] == reg[b] ? 1 : 0; }},
    };
  }

}