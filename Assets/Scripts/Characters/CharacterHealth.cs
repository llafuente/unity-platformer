using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Tracks character health and lives.
  /// Triggers character death
  /// TODO handle lives / Game over
  /// </summary>
  public class CharacterHealth : MonoBehaviour {

    #region public

    [Comment("Health the character will have when Start")]
    public int startingHealth = 1;
    [Comment("Max health. if startingHealth == maxHealth will trigger onMaxHealth on Start")]
    public int maxHealth = 1;
    [Comment("Lives the character starts with. -1 disable lives")]
    public int startingLives = 1;
    [Comment("Maximum lives of the character. 2,147,483,647 is the maximum :)")]
    public int maxLives = 1;
    [Comment("After any Damage how much time the character will be invulnerable to any Damage. -1 to disable")]
    public float invulnerabilityTimeAfterDamage = 2.0f;

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

    #endregion

    #region ~private

    // NOTE do not use setter/getter to trigger death, we need to preserve
    // logical Action dispacthing
    [HideInInspector]
    public int health = 0;
    [HideInInspector]
    public int lives = 0;

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

      health = startingHealth;
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
    public CharacterHealth SetInvulnerable(float time) {
      _invulnerable = time;

      if (_invulnerable > 0.0f) {
        if (onInvulnerabilityStart != null) {
          onInvulnerabilityStart();
        }
      }

      return this;
    }
    /// <summary>
    /// disable invulnerability
    /// </summary>
    public CharacterHealth SetVulnerable() {
      _invulnerable = -1; // any negative value works :D

      return this;
    }

    public bool IsInvulnerable() {
      // is death? leave him alone...
      return health <= 0 || _invulnerable > 0.0f;

    }
    /// <summary>
    /// Kill the character even if it's invulnerable
    /// </summary>
    public CharacterHealth Kill() {
      health = 0;
      Die();

      return this;
    }
    /// <summary>
    /// Kill the character even if it's invulnerable
    /// TODO handle direction here or in the Hitbox but must be done :)
    /// </summary>
    public void Damage(DamageType dt) {
      if (Damage(dt.amount)) {
        if (dt.causer != null && dt.causer.onHurtCharacter != null) {
          causer.onHurtCharacter(dt, GetComponent<Character>());
        }
      }
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

      Debug.Log(this.name + " recieve damage " + amount);
      health -= amount;
      Debug.Log(this.name + " remaining health " + health);

      SetInvulnerable(invulnerabilityTimeAfterDamage);

      if (onDamage != null) {
        onDamage();
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
    /// Disable Hitbox(es) and DamageType(s)
    /// Trigger onDeath
    /// </summary>
    public void Die() {
      Debug.Log(this.name + " disable all Hitbox");
  	  var lch = GetComponentsInChildren<Hitbox> ();
      foreach (var x in lch) {
  	     x.gameObject.SetActive(false);
      }

      Debug.Log(this.name + " disable all DamageType");
      var ldt = GetComponentsInChildren<DamageType> ();
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
