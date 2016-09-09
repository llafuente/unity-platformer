using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Tracks character health and lives.
  /// Triggers character death
  /// TODO handle lives / Game over
  /// TODO handle alignment
  /// </summary>
  public class CharacterHealth : MonoBehaviour {

    #region public

    public Alignment alignment = Alignment.None;
    [Comment("Health the character will have when game starts")]
    public int startingHealth = 1;
    [Comment("Maximum health (-1 no maximum). NOTE if startingHealth == maxHealth will trigger onMaxHealth on Start.")]
    public int maxHealth = 1;
    [Comment("Lives the character starts with (-1 no lives)")]
    public int startingLives = 1;
    [Comment("Maximum lives of the character. 2,147,483,647 is the maximum :)")]
    public int maxLives = 1;
    [Comment("After any Damage how much time the character will be invulnerable to any Damage (0 to disable)")]
    public float invulnerabilityTimeAfterDamage = 2.0f;
    [Comment("List of damages that are ignored")]
    public DamageType immunity = 0;
    ///
    /// Actions
    ///

    /// <summary>
    /// Give your character super powers!
    /// </summary>
    public Action onMaxHealth;
    /// <summary>
    /// Flash it!
    /// </summary>
    public Action onDamage;
    public delegate void HurtCallback(Damage dt, CharacterHealth to);
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
    /// Play that funky music!
    /// NOTE this can be fired many times before onInvulnerabilityEnd
    /// </summary>
    public Action onInvulnerabilityStart;
    /// <summary>
    /// Stop that funky music!
    /// </summary>
    public Action onInvulnerabilityEnd;
    /// <summary>
    /// Stop that funky music!
    /// </summary>
    public Action onImmunity;
    #endregion

    #region ~private

    // NOTE do not use setter/getter to trigger death, we need to preserve
    // logical Action dispacthing
    internal int health = 0;
    internal int lives = 0;
    internal Character character;

    #endregion

    #region private

    float _invulnerable = 0;

    #endregion

    void Start() {
      // check missconfiguration
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

    void LateUpdate() {
      // NOTE do not use IsInvulnerable here...
      bool was_invulnerable = _invulnerable >= 0;

      _invulnerable -= Time.deltaTime;

      if (was_invulnerable && _invulnerable < 0) {
        if (onInvulnerabilityEnd != null) {
          onInvulnerabilityEnd();
        }
      }
    }

    /// <summary>
    /// Turns a character invulnerable, but still can be killed using Kill
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
    /// Kill the character even if it's invulnerable
    /// TODO handle direction here or in the HitBox but must be done :)
    /// </summary>
    public void Damage(Damage dmg) {
      Debug.LogFormat("Object: {0} recieve damage {1} health {2} from: {3}",
        gameObject.name, dmg.amount, health, dmg.causer.gameObject.name);

      if (Damage(dmg.amount, dmg.type)) {
        if (dmg.causer != null && dmg.causer.onHurt != null) {
          dmg.causer.onHurt(dmg, this);
        }
      }
    }

    public bool Damage(int amount, DamageType dt) {
      Debug.LogFormat("immunity {0} DamageType {1}", immunity, dt);
      if ((immunity & dt) == dt) {
        Debug.LogFormat("Inmune to {0} attacks", dt);
        return false;
      }

      return Damage(amount);
    }

    /// <summary>
    /// triggers onDamage
    /// triggers onDeath
    /// NOTE this won't trigger onHurtCharacter
    /// </summary>
    public bool Damage(int amount = 1) {
      if (amount <= 0) {
        Debug.LogWarning("amount <= 0 ??");
      }

      if (IsInvulnerable()) {
        Debug.Log(this.name + " is invulnerable, ignore damage");
        return false;
      }

      health -= amount;

      SetInvulnerable(invulnerabilityTimeAfterDamage);

      if (onDamage != null) {
        onDamage();
      }

      if (onHurt != null) {
        onHurt(null, this);
      }

      if (health <= 0) {
        Die();
      }

      return true;

    }

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

      //Destroy(gameObject);

      if (onDeath != null) {
        Debug.Log(this.name + " died!");
        onDeath();
      }
    }
  }
}
