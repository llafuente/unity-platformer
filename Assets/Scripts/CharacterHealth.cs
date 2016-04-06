using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <>
  /// Tracks character health and lives.
  /// Triggers character death
  /// TODO handle lives
  /// <>
  public class CharacterHealth : MonoBehaviour
  {
    /// properties
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

    /// internal but public

    // NOTE do not use setter/getter to trigger death, we need to preserve
    // logical Action dispacthing
    [HideInInspector]
    public int health = 0;
    [HideInInspector]
    public int lives = 0;

    ///
    /// Actions
    ///

    public Action onMaxHealth;
    public Action onDamage;
    public Action onDeath;
    public Action onGameOver;
    // NOTE this can be fired many times before onInvulnerabilityEnd
    public Action onInvulnerabilityStart;
    public Action onInvulnerabilityEnd;

    /// private

    float _invulnerable = 0;

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
      bool was_invulnerable = _invulnerable >= 0;

      _invulnerable -= Time.deltaTime;

      if (was_invulnerable && _invulnerable < 0) {
        if (onInvulnerabilityEnd != null) {
          onInvulnerabilityEnd();
        }
      }
    }

    public CharacterHealth SetInvulnerable(float time) {
      _invulnerable = time;

      if (_invulnerable > 0) {
        if (onInvulnerabilityStart != null) {
          onInvulnerabilityStart();
        }
      }

      return this;
    }

    public CharacterHealth SetVulnerable() {
      _invulnerable = -1; // any negative value works :D

      return this;
    }

    public CharacterHealth Kill() {
      health = 0;
      Die();

      return this;
    }

    public CharacterHealth Damage(DamageType dt) {
      return Damage(dt.amount);
    }

    public CharacterHealth Damage(int amount = 1) {
      if (amount <= 0) {
        Debug.LogWarning("amount <= 0 ??");
      }

      if (_invulnerable > 0) {
        Debug.Log(this.name + " is invulnerable, ignore damage");

        return this;
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

      return this;
    }

    public bool isDead() {
      return health <= 0;
    }

    virtual public void Heal(int amount = 1) {
      health += amount;

      if (maxHealth != -1 && health >= maxHealth) {
        health = maxHealth;
        if (onMaxHealth != null) {
          onMaxHealth();
        }
      }
    }

    // TODO
    virtual public void Die() {
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
        Debug.Log(this.name + " ????");
        onDeath();
      }
    }
  }
}
