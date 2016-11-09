using System;
using UnityEngine;

/// TODO handle lives / Game over
/// TODO handle character alignment
/// TODO handle Damage direction here or in the HitBox but must be done :)

namespace UnityPlatformer {
  /// <summary>
  /// Tracks character health and lives.
  ///
  /// Triggers character damage/death
  /// </summary>
  public class CharacterHealth : MonoBehaviour, IUpdateEntity {
    /// <summary>
    /// Character alignment
    /// </summary>
    public Alignment alignment = Alignment.None;
    /// <summary>
    /// Health the character will have when game starts
    /// </summary>
    [Comment("Health the character will have when game starts")]
    public int startingHealth = 1;
    /// <summary>
    /// Maximum health (-1 no maximum). NOTE if startingHealth == maxHealth will trigger onMaxHealth on Start.
    /// </summary>
    [Comment("Maximum health (-1 no maximum). NOTE if startingHealth == maxHealth will trigger onMaxHealth on Start.")]
    public int maxHealth = 1;
    /// <summary>
    /// Lives the character starts with (-1 no lives)
    /// </summary>
    [Comment("Lives the character starts with (-1 no lives)")]
    public int startingLives = 1;
    /// <summary>
    /// Maximum lives of the character. 2,147,483,647 is the maximum :)
    /// </summary>
    [Comment("Maximum lives of the character. 2,147,483,647 is the maximum :)")]
    public int maxLives = 1;
    /// <summary>
    /// After any Damage how much time the character will be invulnerable to any Damage (0 to disable)
    /// </summary>
    [Comment("After any Damage how much time the character will be invulnerable to any Damage (0 to disable)")]
    public float invulnerabilityTimeAfterDamage = 2.0f;
    /// <summary>
    /// List of damages that are ignored
    ///
    /// NOTE: this can give your character super powers! use it with caution!
    /// </summary>
    [Comment("List of damages that are ignored")]
    public DamageType immunity = 0;
    /// <summary>
    /// Fired when Character heal and now it's at maxHealth
    /// </summary>
    public Action onMaxHealth;
    /// <summary>
    /// Fired when character is damaged.
    ///
    /// Will be fired even it the character is inmmune
    /// </summary>
    public Action onDamage;
    /// <summary>
    /// Fired after onDamage and Character is inmmune to given Damage
    /// </summary>
    public Action onImmunity;
    /// <summary>
    /// Callback type for onHurt
    /// </summary>
    public delegate void HurtCallback(Damage dt, CharacterHealth to);
    /// <summary>
    /// This Character health is reduced, will fire after onDamage
    ///
    /// dt is the Damage dealed
    /// to is the CharacterHealth that hurt me, if possible, could be null
    /// </summary>
    public HurtCallback onInjured;
    /// <summary>
    /// This Character deal damage to other
    ///
    /// dt is the Damage dealed
    /// to is the CharacterHealth hurted
    /// </summary>
    public HurtCallback onHurt;
    /// <summary>
    /// Display some greenish starts floating around!
    /// </summary>
    public Action onHeal;
    /// <summary>
    /// Play death animation, turn off the music... those sort of things
    /// </summary>
    public Action onDeath;
    /// <summary>
    /// Credits...
    /// </summary>
    public Action onGameOver;
    /// <summary>
    /// Play that funky music! Quake-damage!
    ///
    /// NOTE this can be fired many times before onInvulnerabilityEnd
    /// </summary>
    public Action onInvulnerabilityStart;
    /// <summary>
    /// Stop that funky music!
    /// </summary>
    public Action onInvulnerabilityEnd;
    /// <summary>
    /// After death when there are lives player can respawn
    /// </summary>
    public Action onRespawn;
    // NOTE do not use setter/getter to trigger death, we need to preserve
    // logical Action dispacthing
    /// <summary>
    /// Character health
    /// </summary>
    [HideInInspector]
    public int health = 0;
    /// <summary>
    /// Character lives
    /// </summary>
    [HideInInspector]
    public int lives = 0;
    /// <summary>
    /// Character owner of this CharacterHealth
    /// </summary>
    [HideInInspector]
    public Character character;
    /// <summary>
    /// Time counter for invulnerability
    /// </summary>
    private float _invulnerable = 0;
    /// <summary>
    /// check missconfiguration and initialization
    /// </summary>
    public void Start() {
      if (startingHealth < maxHealth) {
        Debug.LogWarning(this.name + " startingHealth < maxHealth ?");
      }
      if (startingLives < maxLives) {
        Debug.LogWarning(this.name + " startingLives < maxLives ?");
      }

      character = GetComponent<Character>();
      Heal(startingHealth);
      lives = startingLives;
    }
    public void OnEnable() {
      UpdateManager.Push(this, Configuration.instance.charactersPriority);
    }
    /// <summary>
    /// invulnerability logic
    /// </summary>
    public virtual void PlatformerUpdate(float delta) {
      Debug.Log(delta);
      // NOTE do not use IsInvulnerable here...
      bool was_invulnerable = _invulnerable >= 0;

      _invulnerable -= delta;

      if (was_invulnerable && _invulnerable < 0) {
        if (onInvulnerabilityEnd != null) {
          onInvulnerabilityEnd();
        }
      }
    }
    public virtual void LatePlatformerUpdate(float delta) {}
    /// <summary>
    /// Turns a character invulnerable, but still can be killed using Kill
    ///
    /// NOTE use float.MaxValue for unlimited time
    /// </summary>
    public void SetInvulnerable(float time) {
      _invulnerable = time;

      if (_invulnerable > 0.0f) {
        if (onInvulnerabilityStart != null) {
          onInvulnerabilityStart();
        }
      }
    }
    /// <summary>
    /// disable invulnerability
    /// </summary>
    public void SetVulnerable() {
      bool was_invulnerable = _invulnerable >= 0;
      _invulnerable = -1; // any negative value works :D

      if (was_invulnerable && onInvulnerabilityEnd != null) {
        onInvulnerabilityEnd();
      }
    }
    /// <summary>
    /// Character is invulnerable?
    /// </summary>
    public bool IsInvulnerable() {
      // is death? leave him alone...
      return health <= 0 || _invulnerable > 0.0f;
    }
    /// <summary>
    /// Kill the character even if it's invulnerable
    /// </summary>
    public void Kill() {
      health = 0;
      Die();
    }
    /// <summary>
    /// Try to Damage the Character
    /// </summary>
    public void Damage(Damage dmg) {
      Debug.LogFormat("Object: {0} recieve damage {1} health {2} from: {3}",
        gameObject.name, dmg.amount, health, dmg.causer.gameObject.name);

      if (Damage(dmg.amount, dmg.type, dmg.causer)) {
        if (dmg.causer != null && dmg.causer.onHurt != null) {
          dmg.causer.onHurt(dmg, this);
        }
      }
    }
    /// <summary>
    /// Try to Damage the Character
    /// </summary>
    public bool Damage(int amount, DamageType dt, CharacterHealth causer = null) {
      Debug.LogFormat("immunity {0} DamageType {1}", immunity, dt);
      if ((immunity & dt) == dt) {
        Debug.LogFormat("Inmune to {0} attacks", dt);

        if (onDamage != null) {
          onDamage();
        }

        if (onImmunity != null) {
          onImmunity();
        }

        return false;
      }

      return Damage(amount, causer);
    }
    /// <summary>
    /// Try to Damage the Character
    ///
    /// triggers onDamage
    /// triggers onDeath
    /// NOTE this won't trigger onHurtCharacter
    /// </summary>
    public bool Damage(int amount = 1, CharacterHealth causer = null) {
      if (amount <= 0) {
        Debug.LogWarning("amount <= 0 ??");
      }

      if (IsInvulnerable()) {
        Debug.Log(this.name + " is invulnerable, ignore damage");

        if (onDamage != null) {
          onDamage();
        }

        if (onImmunity != null) {
          onImmunity();
        }

        return false;
      }

      health -= amount;

      // do not set invulnerable a dead Character
      if (health > 0) {
        SetInvulnerable(invulnerabilityTimeAfterDamage);
      }

      if (onDamage != null) {
        onDamage();
      }

      if (onInjured != null) {
        onInjured(null, causer);
      }

      if (health <= 0) {
        Die();
      }

      return true;

    }
    /// <summary>
    /// No healt
    /// </summary>
    public bool isDead() {
      return health <= 0;
    }
    /// <summary>
    /// increse health character if possible maxHealth not reached.
    /// Trigger onMaxHealth
    /// </summary>
    public void Heal(int amount = 1) {
      health += amount;
      if (onHeal != null) {
        onHeal();
      }

      if (maxHealth != -1 && health >= maxHealth) {
        health = maxHealth;
        if (onMaxHealth != null) {
          onMaxHealth();
        }
      }
    }
    /// <summary>
    /// Disable HitBox(es) and DamageType(s)
    /// Trigger onDeath
    /// </summary>
    public void Die() {
      --lives;

      if (onDeath != null) {
        Debug.Log(this.name + " died!");
        onDeath();
      }

      if (lives == 0) {
        // game over
        Debug.Log(this.name + " disable all HitBox(es)");
        var lch = GetComponentsInChildren<HitBox> ();
        foreach (var x in lch) {
           x.gameObject.SetActive(false);
        }

        Debug.Log(this.name + " disable all Damage(s)");
        var ldt = GetComponentsInChildren<Damage> ();
        foreach (var x in ldt) {
           x.gameObject.SetActive(false);
        }

        if (onGameOver != null) {
          Debug.Log(this.name + " game-over!");
          onGameOver();
        }
      } else {
        // disable invulnerability

        // respawn
        Heal(startingHealth);

        if (onRespawn != null) {
          onRespawn();
        }
      }
    }
  }
}
