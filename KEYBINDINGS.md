# ⌨️ Keybindings: Ultimate Heroes

## 🎮 **Skill Keybindings**

Spieler können Skills über Keybindings aktivieren:

### **Commands für Keybindings**

```
!skill1    - Aktiviert Skill Slot 1
!skill2    - Aktiviert Skill Slot 2
!skill3    - Aktiviert Skill Slot 3
!ultimate  - Aktiviert Ultimate Skill
```

### **Wie man Keybindings setzt**

**In CS2 Console:**
```
bind f "css_skill1"
bind g "css_skill2"
bind h "css_skill3"
bind x "css_ultimate"
```

**Oder in config.cfg:**
```
bind "f" "css_skill1"
bind "g" "css_skill2"
bind "h" "css_skill3"
bind "x" "css_ultimate"
```

### **Beispiel Keybindings**

```
// Skills auf F, G, H
bind f "css_skill1"
bind g "css_skill2"
bind h "css_skill3"

// Ultimate auf X
bind x "css_ultimate"

// Oder auf Maus-Tasten
bind "mouse4" "css_skill1"
bind "mouse5" "css_skill2"
```

### **Wie es funktioniert**

1. Spieler aktiviert einen Build (`!activatebuild 1`)
2. Build hat 3 Skills (z.B. Fireball, Blink, Stealth)
3. `!skill1` aktiviert den ersten Skill (Fireball)
4. `!skill2` aktiviert den zweiten Skill (Blink)
5. `!skill3` aktiviert den dritten Skill (Stealth)
6. `!ultimate` aktiviert den Ultimate Skill (falls vorhanden)

### **Cooldowns**

- Skills zeigen Cooldown-Meldungen wenn nicht bereit
- Ultimate zeigt separaten Cooldown
- Passive Skills können nicht aktiviert werden

### **Hinweise**

- Keybindings funktionieren nur wenn ein Build aktiv ist
- Leere Slots zeigen Fehlermeldung
- Cooldowns werden respektiert

---

**Viel Spaß mit den Keybindings!** ⌨️
