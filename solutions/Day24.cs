using System.Text.Json;
using System.Text.Json.Serialization;

class Attack : ICloneable
{
  [JsonPropertyName("damage")]
  public int AttackDamage { get; set; }

  [JsonPropertyName("type")]
  public string AttackType { get; set; } = String.Empty;

  [JsonPropertyName("initiative")]
  public int Initiative { get; set; }

  public object Clone()
  {
    return this.MemberwiseClone();
  }
}

class Group : ICloneable
{
  public char Type { get; set; }

  [JsonPropertyName("units")]
  public int Units { get; set; }

  [JsonPropertyName("hp")]
  public int HitPoints { get; set; }

  [JsonPropertyName("immune")]
  public List<string> Immunities { get; set; } = new();

  [JsonPropertyName("weak")]
  public List<string> Weakness { get; set; } = new();

  [JsonPropertyName("attack")]
  public Attack Attack { get; set; } = new();

  public int EffectivePower => Units * Attack.AttackDamage;

  public int CalculateDamage(Group attackingGroup)
  {
    if (this.Immunities.Contains(attackingGroup.Attack.AttackType))
    {
      return 0;
    }

    if (this.Weakness.Contains(attackingGroup.Attack.AttackType))
    {
      return attackingGroup.EffectivePower * 2;
    }

    return attackingGroup.EffectivePower;
  }

  public object Clone()
  {
    var clone = this.MemberwiseClone();
    ((Group)clone).Attack = (Attack)Attack.Clone();
    return clone;
  }

  public void ReceiveDamage(Group attackingGroup)
  {
    var damage = CalculateDamage(attackingGroup);
    var unitsToRemove = damage / HitPoints;
    Units = Math.Max(0, Units - unitsToRemove);
  }
}

class Day24 : IDayCommand
{

  public string Execute()
  {
    var file = new FileReader(24).Read().Aggregate("", (c, n) => c + n);
    var document = JsonDocument.Parse(file);

    var immuneSystemGroups = document.RootElement.GetProperty("immuneSystem").Deserialize<List<Group>>();
    var infectionGroups = document.RootElement.GetProperty("infection").Deserialize<List<Group>>();

    if (immuneSystemGroups is null || infectionGroups is null)
    {
      return "Error";
    }

    immuneSystemGroups.ForEach(g => g.Type = 'M');
    infectionGroups.ForEach(g => g.Type = 'F');
    var allGroups = immuneSystemGroups.Concat(infectionGroups).ToList();

    // Part 01
    var validGroups = allGroups.Select(g => (Group)g.Clone()).ToList();
    validGroups = RunBattle(validGroups);
    var numberOfWinningUnitsPart01 = validGroups.Sum(g => g.Units);

    // Part 02
    int boost = 0;
    while (!validGroups.All(g => g.Type == 'M'))
    {
      boost += 1;
      validGroups = allGroups.Select(g =>
      {
        var clone = (Group)g.Clone();
        if (clone.Type == 'M')
        {
          clone.Attack.AttackDamage += boost;
        }
        return clone;
      }).ToList();
      validGroups = RunBattle(validGroups);
    }
    var numberOfWinningUnitsPart02 = validGroups.Sum(g => g.Units);

    return $"Number of winning units {numberOfWinningUnitsPart01} after boost {numberOfWinningUnitsPart02}";

  }

  private static List<Group> RunBattle(List<Group> groups)
  {
    bool HasWinner(List<Group> groups) => groups.All(g => g.Type == 'M') || groups.All(g => g.Type == 'F');
    var numberOfUnits = groups.Sum(g => g.Units);
    while (!HasWinner(groups))
    {
      // In decreasing order of effective power, groups choose their targets; in a tie, the group with the higher initiative chooses first
      groups = groups.OrderByDescending(g => g.EffectivePower)
                               .ThenByDescending(g => g.Attack.Initiative)
                               .ToList();

      // Target Selection Phase
      Dictionary<Group, Group> targetSelection = new();
      foreach (var attacker in groups)
      {
        // The attacking group chooses to target the group in the enemy army to which it would deal the most damage 
        // (after accounting for weaknesses and immunities, but not accounting for whether the defending group has enough units to actually receive all of that damage).
        // If an attacking group is considering two defending groups to which it would deal equal damage, it chooses to target the defending group with the largest effective power;
        // if there is still a tie, it chooses the defending group with the highest initiative
        // Defending groups can only be chosen as a target by one attacking group
        var enemy = groups.Where(d => d.Type != attacker.Type)
                               .Where(d => !targetSelection.Values.Contains(d))
                               .Where(d => d.CalculateDamage(attacker) > 0) // Even though not described in the text, ignore enemies that are immune to the attack
                               .OrderByDescending(d => d.CalculateDamage(attacker))
                               .ThenByDescending(d => d.EffectivePower)
                               .ThenByDescending(d => d.Attack.Initiative)
                               .FirstOrDefault();

        if (enemy is not null)
        {
          targetSelection.Add(attacker, enemy);
        }
      }

      // Attack Phase
      // Groups attack in decreasing order of initiative, regardless of whether they are part of the infection or the immune system.
      var attackers = targetSelection.Keys.OrderByDescending(g => g.Attack.Initiative).ToList();
      foreach (var attacker in attackers)
      {
        var defender = targetSelection[attacker];
        if (attacker.EffectivePower == 0) continue; // This unit has been eliminated
        defender.ReceiveDamage(attacker);
        if (defender.Units == 0) // Unit is dead
        {
          groups.Remove((defender));
        }
      }

      var newNumberOfUnits = groups.Sum(g => g.Units);
      // No changes in the battle scene
      if (numberOfUnits == newNumberOfUnits)
      {
        break;
      }
      numberOfUnits = newNumberOfUnits;
    }

    return groups;
  }
}
