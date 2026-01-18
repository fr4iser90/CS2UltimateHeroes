# 🔍 Reflection erklärt

## Was ist Reflection?

**Reflection** ist eine C#-Funktion, die es ermöglicht, zur **Laufzeit** (nicht zur Compile-Zeit) Informationen über Typen, Properties, Methoden etc. zu erhalten und diese zu nutzen.

### Einfaches Beispiel:

```csharp
// OHNE Reflection (hardcoded):
var hero = new Vanguard();
heroService.RegisterHero(hero);

// MIT Reflection (automatisch):
var heroType = typeof(Vanguard);
var hero = (IHero)Activator.CreateInstance(heroType)!;
heroService.RegisterHero(hero);
```

### Warum verwenden wir Reflection?

**Problem ohne Reflection:**
- Jedes Mal wenn du einen neuen Hero/Skill hinzufügst, musst du die Liste in `UltimateHeroes.cs` manuell erweitern
- Du musst jeden Skill manuell mit `EffectManager` verbinden
- Viel Code-Duplikation und Fehleranfälligkeit

**Lösung mit Reflection:**
- Das System findet **automatisch** alle Heroes/Skills im Assembly
- Keine manuellen Listen mehr nötig
- Neue Klassen werden automatisch erkannt

## 🎯 Wo verwenden wir Reflection?

### 1. **Hero Registration** (Auto-Discovery)
```csharp
// Findet ALLE Klassen die IHero implementieren
var heroTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => typeof(IHero).IsAssignableFrom(t) && !t.IsAbstract);

// Erstellt Instanzen automatisch
foreach (var type in heroTypes)
{
    var hero = (IHero)Activator.CreateInstance(type)!;
    _heroes[hero.Id] = hero;
}
```

### 2. **Skill Registration** (Auto-Discovery)
```csharp
// Findet ALLE Klassen die ISkill implementieren
var skillTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => typeof(ISkill).IsAssignableFrom(t) && !t.IsAbstract);

// Erstellt Instanzen automatisch
foreach (var type in skillTypes)
{
    var skill = (ISkill)Activator.CreateInstance(type)!;
    _skills[skill.Id] = skill;
}
```

### 3. **EffectManager/SpawnService Injection** (Auto-Set)
```csharp
// Findet ALLE Skills
var skillTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => typeof(ISkill).IsAssignableFrom(t));

foreach (var type in skillTypes)
{
    // Prüft ob Skill eine statische "EffectManager" Property hat
    var effectManagerProperty = type.GetProperty("EffectManager", 
        BindingFlags.Public | BindingFlags.Static | BindingFlags.SetProperty);
    
    if (effectManagerProperty != null)
    {
        // Setzt den EffectManager automatisch
        effectManagerProperty.SetValue(null, _effectManager);
    }
}
```

### 4. **Buff Handler Registration** (Auto-Discovery)
```csharp
// Findet ALLE Klassen die IBuffHandler implementieren
var handlerTypes = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(t => typeof(IBuffHandler).IsAssignableFrom(t) && !t.IsAbstract);

// Registriert Handler automatisch
foreach (var type in handlerTypes)
{
    var handler = (IBuffHandler)Activator.CreateInstance(type)!;
    _handlerRegistry.RegisterHandler(handler);
}
```

## ✅ Vorteile

1. **Keine manuellen Listen mehr** - Neue Heroes/Skills werden automatisch erkannt
2. **Weniger Fehler** - Keine vergessenen Registrierungen
3. **Wartbarer Code** - Änderungen an einer Stelle
4. **Erweiterbar** - Neue Features fügen sich automatisch ein

## ⚠️ Nachteile

1. **Langsamer** - Reflection ist langsamer als direkte Aufrufe (aber nur beim Start, nicht während des Spiels)
2. **Schwerer zu debuggen** - Fehler werden zur Laufzeit erkannt, nicht zur Compile-Zeit
3. **Komplexer** - Code ist schwerer zu verstehen

## 🎯 In unserem Projekt

Wir verwenden Reflection **nur beim Plugin-Start** (einmalig), nicht während des Spiels. Das ist ein guter Kompromiss zwischen Flexibilität und Performance.
