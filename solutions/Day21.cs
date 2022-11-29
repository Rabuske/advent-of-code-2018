class Day21 : IDayCommand
{
  public string Execute()
  {
    // My Program:
    /*
    #ip 1
    0 | seti 123 0 3      | Initializes reg[3] with 123
    1 | bani 3 456 3      | reg[3] = reg[3] && 456
    2 | eqri 3 72 3       | !!! reg[3] = reg[3] == 72 if(reg[3] == 72) go to 5 else go to 1
    3 | addr 3 1 1        | // Pointer control -> jump to 5 
    4 | seti 0 0 1        | // Pointer control -> jump to 1
    5 | seti 0 1 3        | reg[3] = 0
    6 | bori 3 65536 2    | reg[2] = reg[3] || 65536
    7 | seti 1505483 6 3  | reg[3] = 1505483
    8 | bani 2 255 4      | reg[4] = reg[2] && 255
    9 | addr 3 4 3        | reg[3] = reg[3] + reg[4]
   10 | bani 3 16777215 3 | reg[3] = reg[3] && 16777215
   11 | muli 3 65899 3    | reg[3] = reg[3] * 65899
   12 | bani 3 16777215 3 | reg[3] = reg[3] && 16777215
   13 | gtir 256 2 4      | reg[4] = 256 > reg[2] ? 1 : 0
   14 | addr 4 1 1        | // Pointer control -> skip next line if 256 > reg[2]
   15 | addi 1 1 1        | // Pointer control -> skip next line
   16 | seti 27 6 1       | // Pointer control -> jump to 28
   17 | seti 0 3 4        | reg[4] = 0
   18 | addi 4 1 5        | reg[5] = reg[4] + 1 // Start of one loop while reg[4] * 256 < reg[2]; reg[4]++
   19 | muli 5 256 5      | reg[5] = reg[5] * 256 
   20 | gtrr 5 2 5        | reg[5] = reg[5] > reg[2]? 1 : 0
   21 | addr 5 1 1        | // Pointer control -> skip next line if reg[5] > reg[2]
   22 | addi 1 1 1        | // Pointer control -> skip next line
   23 | seti 25 4 1       | // Pointer control -> jump to 26
   24 | addi 4 1 4        | reg[4] = reg[4] + 1
   25 | seti 17 3 1       | // Pointer control -> jump to 18
   26 | setr 4 1 2        | reg[2] = reg[4]
   27 | seti 7 4 1        | // Pointer control -> jump to 8
   28 | eqrr 3 0 4        | reg[4] = reg[3] == reg[0]
   29 | addr 4 1 1        | // Pointer control -> halt program if reg[3] == reg[0]
   30 | seti 5 9 1        | // Pointer control -> jump to 6

    */

    HashSet<long> seen = new();
    long firstValue = 0;
    long previous = 0;
    long A, B, C, D, E = 0;
    A = 0;
    do
    {
      C = 123;
    L1:
      C &= 456;
      C = C == 72? 1 : 0;
      if(C == 0) goto L1;
      C = 0;
    L6:
      B = C | 65536;
      C = 1505483;
    L8:      
      D = B & 255;
      C += D;
      C &= 16777215;
      C *= 65899;
      C &= 16777215;
      D = 256 > B? 1 : 0;
      if(D == 1) goto L28;
      D = 0;
    L18:
      E = (D + 1) * 256;
      if(E > B) goto L26;
      D += 1;
      goto L18;
    L26:      
      B = D;
      goto L8;
    L28:
      D = C == A? 1 : 0; 
      if(seen.Count == 0) firstValue = C;
      if(seen.Contains(C)) break;
      seen.Add(C);
      previous = C;
      if(D == 0) goto L6;
    }while(A != C);

    return $"The value for the least is {firstValue} and for the most is {previous}";
  }
}