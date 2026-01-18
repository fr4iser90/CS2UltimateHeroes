# ⚖️ Plan: Rules Engine (Erweitert)

## 📋 Zweck

Die Rules Engine erweitert den BuildValidator um:
- Tag-based Rules
- Combination Rules
- Diminishing Returns
- Configurable Rules

## 🔗 Abhängigkeiten

- `BuildValidator` (Application/BuildValidator.cs) ✅
- `ISkill` (Domain/Skills/ISkill.cs) ✅
- `IHero` (Domain/Heroes/IHero.cs) ✅

## 📐 Rules Engine Struktur

### Rule Definition

```csharp
namespace UltimateHeroes.Application.Rules
{
    public class Rule
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public RuleType Type { get; set; }
        public RuleCondition Condition { get; set; }
        public RuleAction Action { get; set; }
        public bool IsBlocking { get; set; } // Blockiert Build oder nur Warning
    }
    
    public enum RuleType
    {
        TagLimit,        // Max X Skills mit Tag Y
        Combination,     // Tag X + Tag Y = Effect
        Diminishing,    // Diminishing Returns
        PowerBudget      // Power Budget Check
    }
    
    public class RuleCondition
    {
        public RuleConditionType Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
    
    public enum RuleConditionType
    {
        TagCount,        // Count(SkillTag) > X
        TagCombination,  // Has(SkillTag.X) && Has(SkillTag.Y)
        PowerWeight,     // Total PowerWeight > X
        SkillTypeCount   // Count(SkillType) > X
    }
    
    public class RuleAction
    {
        public RuleActionType Type { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
    
    public enum RuleActionType
    {
        Block,          // Blockiert Build
        Warning,        // Warning nur
        ApplyModifier,  // Wendet Modifier an (z.B. Cooldown +25%)
        ReducePower     // Reduziert Power Weight
    }
}
```

## 🎯 Implementierung

### Datei: `Application/Rules/RulesEngine.cs`

```csharp
namespace UltimateHeroes.Application.Rules
{
    public class RulesEngine
    {
        private readonly List<Rule> _rules = new();
        
        public RulesEngine()
        {
            LoadDefaultRules();
        }
        
        public void AddRule(Rule rule)
        {
            _rules.Add(rule);
        }
        
        public RuleResult ValidateBuild(IHero hero, List<ISkill> skills)
        {
            var result = new RuleResult
            {
                IsValid = true,
                Errors = new List<string>(),
                Warnings = new List<string>(),
                Modifiers = new Dictionary<string, float>()
            };
            
            foreach (var rule in _rules)
            {
                if (EvaluateCondition(rule.Condition, hero, skills))
                {
                    ApplyAction(rule.Action, rule, result);
                    
                    if (rule.IsBlocking && result.Errors.Count > 0)
                    {
                        result.IsValid = false;
                    }
                }
            }
            
            return result;
        }
        
        private bool EvaluateCondition(RuleCondition condition, IHero hero, List<ISkill> skills)
        {
            return condition.Type switch
            {
                RuleConditionType.TagCount => EvaluateTagCount(condition, skills),
                RuleConditionType.TagCombination => EvaluateTagCombination(condition, skills),
                RuleConditionType.PowerWeight => EvaluatePowerWeight(condition, hero, skills),
                RuleConditionType.SkillTypeCount => EvaluateSkillTypeCount(condition, skills),
                _ => false
            };
        }
        
        private bool EvaluateTagCount(RuleCondition condition, List<ISkill> skills)
        {
            var tag = (SkillTag)condition.Parameters["tag"];
            var maxCount = (int)condition.Parameters["maxCount"];
            
            var count = skills.Count(s => s.Tags.Contains(tag));
            return count > maxCount;
        }
        
        private bool EvaluateTagCombination(RuleCondition condition, List<ISkill> skills)
        {
            var tag1 = (SkillTag)condition.Parameters["tag1"];
            var tag2 = (SkillTag)condition.Parameters["tag2"];
            
            var hasTag1 = skills.Any(s => s.Tags.Contains(tag1));
            var hasTag2 = skills.Any(s => s.Tags.Contains(tag2));
            
            return hasTag1 && hasTag2;
        }
        
        private bool EvaluatePowerWeight(RuleCondition condition, IHero hero, List<ISkill> skills)
        {
            var maxPower = (int)condition.Parameters["maxPower"];
            var totalPower = hero.PowerWeight + skills.Sum(s => s.PowerWeight);
            return totalPower > maxPower;
        }
        
        private bool EvaluateSkillTypeCount(RuleCondition condition, List<ISkill> skills)
        {
            var skillType = (SkillType)condition.Parameters["skillType"];
            var maxCount = (int)condition.Parameters["maxCount"];
            
            var count = skills.Count(s => s.Type == skillType);
            return count > maxCount;
        }
        
        private void ApplyAction(RuleAction action, Rule rule, RuleResult result)
        {
            switch (action.Type)
            {
                case RuleActionType.Block:
                    result.Errors.Add(rule.Description);
                    break;
                    
                case RuleActionType.Warning:
                    result.Warnings.Add(rule.Description);
                    break;
                    
                case RuleActionType.ApplyModifier:
                    var modifierKey = (string)action.Parameters["modifierKey"];
                    var modifierValue = (float)action.Parameters["modifierValue"];
                    result.Modifiers[modifierKey] = modifierValue;
                    break;
                    
                case RuleActionType.ReducePower:
                    // Not implemented yet
                    break;
            }
        }
        
        private void LoadDefaultRules()
        {
            // Max 1 Ultimate
            AddRule(new Rule
            {
                Id = "max_ultimate",
                Name = "Max 1 Ultimate",
                Type = RuleType.TagLimit,
                IsBlocking = true,
                Condition = new RuleCondition
                {
                    Type = RuleConditionType.SkillTypeCount,
                    Parameters = new Dictionary<string, object>
                    {
                        { "skillType", SkillType.Ultimate },
                        { "maxCount", 1 }
                    }
                },
                Action = new RuleAction
                {
                    Type = RuleActionType.Block,
                    Parameters = new Dictionary<string, object>()
                }
            });
            
            // Max 2 Mobility
            AddRule(new Rule
            {
                Id = "max_mobility",
                Name = "Max 2 Mobility Skills",
                Type = RuleType.TagLimit,
                IsBlocking = true,
                Condition = new RuleCondition
                {
                    Type = RuleConditionType.TagCount,
                    Parameters = new Dictionary<string, object>
                    {
                        { "tag", SkillTag.Mobility },
                        { "maxCount", 2 }
                    }
                },
                Action = new RuleAction
                {
                    Type = RuleActionType.Block,
                    Parameters = new Dictionary<string, object>()
                }
            });
            
            // CC + Stealth = Cooldown Malus
            AddRule(new Rule
            {
                Id = "cc_stealth_combo",
                Name = "CC + Stealth Combo",
                Type = RuleType.Combination,
                IsBlocking = false,
                Condition = new RuleCondition
                {
                    Type = RuleConditionType.TagCombination,
                    Parameters = new Dictionary<string, object>
                    {
                        { "tag1", SkillTag.CrowdControl },
                        { "tag2", SkillTag.Stealth }
                    }
                },
                Action = new RuleAction
                {
                    Type = RuleActionType.ApplyModifier,
                    Parameters = new Dictionary<string, object>
                    {
                        { "modifierKey", "cooldown_multiplier" },
                        { "modifierValue", 1.25f } // +25% Cooldown
                    }
                }
            });
            
            // 3+ Damage Skills = Diminishing Returns
            AddRule(new Rule
            {
                Id = "damage_diminishing",
                Name = "Damage Diminishing Returns",
                Type = RuleType.Diminishing,
                IsBlocking = false,
                Condition = new RuleCondition
                {
                    Type = RuleConditionType.TagCount,
                    Parameters = new Dictionary<string, object>
                    {
                        { "tag", SkillTag.Damage },
                        { "maxCount", 2 } // 3+ Damage Skills
                    }
                },
                Action = new RuleAction
                {
                    Type = RuleActionType.ApplyModifier,
                    Parameters = new Dictionary<string, object>
                    {
                        { "modifierKey", "damage_multiplier" },
                        { "modifierValue", 0.9f } // -10% Damage
                    }
                }
            });
        }
    }
    
    public class RuleResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public Dictionary<string, float> Modifiers { get; set; } = new();
    }
}
```

## 🔄 Integration

1. **BuildValidator**: Nutzt RulesEngine für erweiterte Validation
2. **BuildService**: Validiert Builds mit RulesEngine
3. **Skill Activation**: Wendet Modifiers an

## ✅ Tests

- Rules werden korrekt evaluiert
- Blocking Rules blockieren Builds
- Warning Rules zeigen nur Warnings
- Modifiers werden korrekt angewendet
- Custom Rules können hinzugefügt werden

## 📝 Nächste Schritte

1. ✅ Rule, RuleCondition, RuleAction definieren
2. ✅ RulesEngine.cs implementieren
3. ✅ Default Rules laden
4. ✅ Integration mit BuildValidator
5. ✅ Configurable Rules (später)
