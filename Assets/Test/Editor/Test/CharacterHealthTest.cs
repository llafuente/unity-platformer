using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

using UnityPlatformer;

namespace UnityPlatformer.Test {
  [TestFixture]
  [Category("Character")]
  class CharacterHealthTest {
    bool onHealCalled = false;
    bool onDamageCalled = false;
    bool onImmunityCalled = false;
    bool onMaxHealthCalled = false;
    bool onInjuredCalled = false;
    bool onHurtCalled = false;
    bool onDeathCalled = false;
    bool onGameOverCalled = false;
    bool onInvulnerabilityStartCalled = false;
    bool onInvulnerabilityEndCalled = false;
    bool onRespawnCalled = false;

    Configuration config;
    UpdateManager umgr;

    CharacterHealth FixtureCreateHealth(string characterName) {
      Configuration.ClearInstance();
      UpdateManager.ClearInstance();
      System.GC.Collect();

      var obj = new GameObject();
      obj.name = characterName;
      config = obj.AddComponent<Configuration>();
      Assert.NotNull(config);
      umgr = obj.AddComponent<UpdateManager>();
      Assert.NotNull(umgr);


      var objx = new GameObject();
      Character character = objx.AddComponent<Character>();
      Assert.NotNull(character);
      CharacterHealth health = objx.GetComponent<CharacterHealth>();
      Assert.NotNull(health);
      PlatformerCollider2D col2d = objx.GetComponent<PlatformerCollider2D>();
      Assert.NotNull(col2d);

      return health;
    }

    void AttachEvents(CharacterHealth health) {
      health.onHeal += () => { onHealCalled = true; };
      health.onDamage += () => { onDamageCalled = true; };
      health.onImmunity += () => { onImmunityCalled = true; };
      health.onMaxHealth += () => { onMaxHealthCalled = true; };
      health.onInjured += (Damage dt, CharacterHealth to) => { onInjuredCalled = true; };
      health.onHurt += (Damage dt, CharacterHealth to) => { onHurtCalled = true; };
      health.onDeath += () => { onDeathCalled = true; };
      health.onGameOver += () => { onGameOverCalled = true; };
      health.onInvulnerabilityStart += () => { onInvulnerabilityStartCalled = true; };
      health.onInvulnerabilityEnd += () => { onInvulnerabilityEndCalled = true; };
      health.onRespawn += () => { onRespawnCalled = true; };

      ResetCallbacks();
    }

    void ResetCallbacks() {
      onHealCalled = false;
      onDamageCalled = false;
      onImmunityCalled = false;
      onMaxHealthCalled = false;
      onInjuredCalled = false;
      onHurtCalled = false;
      onDeathCalled = false;
      onGameOverCalled = false;
      onInvulnerabilityStartCalled = false;
      onInvulnerabilityEndCalled = false;
      onRespawnCalled = false;
    }

    [Test]
    public void DamageTest() {
      CharacterHealth health = FixtureCreateHealth("testchar");
      AttachEvents(health);

      health.startingHealth = 2;
      health.maxHealth = 2;

      health.Start();
      Assert.That(health.lives, Is.EqualTo(1));
      Assert.That(health.health, Is.EqualTo(2));
      Assert.That(health.IsInvulnerable(), Is.EqualTo(false));

      Assert.That(onHealCalled, Is.EqualTo(true));
      Assert.That(onDamageCalled, Is.EqualTo(false));
      Assert.That(onImmunityCalled, Is.EqualTo(false));
      Assert.That(onMaxHealthCalled, Is.EqualTo(true));
      Assert.That(onInjuredCalled, Is.EqualTo(false));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(false));
      Assert.That(onGameOverCalled, Is.EqualTo(false));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(false));
      Assert.That(onRespawnCalled, Is.EqualTo(false));

      ResetCallbacks();

      health.Damage(1, null);

      Assert.That(onHealCalled, Is.EqualTo(false));
      Assert.That(onDamageCalled, Is.EqualTo(true));
      Assert.That(onImmunityCalled, Is.EqualTo(false));
      Assert.That(onMaxHealthCalled, Is.EqualTo(false));
      Assert.That(onInjuredCalled, Is.EqualTo(true));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(false));
      Assert.That(onGameOverCalled, Is.EqualTo(false));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(true));
      Assert.That(onRespawnCalled, Is.EqualTo(false));

      // after recieve damage time to be invulnerable
      Assert.That(health.IsInvulnerable(), Is.EqualTo(true));

      ResetCallbacks();

      // TEST add time, wait until invulnerability ends
      umgr.forceFixedDeltaTime = 0.25f;
      umgr.FixedUpdate();
      Assert.That(health.IsInvulnerable(), Is.EqualTo(true));

      umgr.forceFixedDeltaTime = 2.0f;
      umgr.FixedUpdate();
      Assert.That(health.IsInvulnerable(), Is.EqualTo(false));
      Assert.That(onInvulnerabilityEndCalled, Is.EqualTo(true));

      ResetCallbacks();

      health.immunity = DamageType.Water;

      // TEST invulnerable to recieved damage
      var obj3 = new GameObject();
      var damage = obj3.AddComponent<Damage>();
      var character2 = obj3.AddComponent<Character>();
      Assert.NotNull(damage);
      damage.type = DamageType.Water;
      damage.causer = character2.GetComponent<CharacterHealth>();
      damage.causer.alignment = Alignment.Enemy;

      health.Damage(damage);
      Assert.That(onHealCalled, Is.EqualTo(false));
      Assert.That(onDamageCalled, Is.EqualTo(true));
      Assert.That(onImmunityCalled, Is.EqualTo(true));
      Assert.That(onMaxHealthCalled, Is.EqualTo(false));
      Assert.That(onInjuredCalled, Is.EqualTo(false));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(false));
      Assert.That(onGameOverCalled, Is.EqualTo(false));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(false));
      Assert.That(onRespawnCalled, Is.EqualTo(false));

      ResetCallbacks();
      /// damage again, kill character

      health.Damage(1, null);
      Assert.That(onHealCalled, Is.EqualTo(false));
      Assert.That(onDamageCalled, Is.EqualTo(true));
      Assert.That(onImmunityCalled, Is.EqualTo(false));
      Assert.That(onMaxHealthCalled, Is.EqualTo(false));
      Assert.That(onInjuredCalled, Is.EqualTo(true));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(true));
      Assert.That(onGameOverCalled, Is.EqualTo(true));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(false));
      Assert.That(onRespawnCalled, Is.EqualTo(false));
    }

    [Test]
    public void DamageUpdateManagerKillTest() {
      CharacterHealth health = FixtureCreateHealth("testchar");
      AttachEvents(health);

      health.startingLives = 2;
      health.maxLives = 2;
      health.startingHealth = 2;
      health.maxHealth = 2;

      health.Start();
      Assert.That(health.lives, Is.EqualTo(2));
      Assert.That(health.health, Is.EqualTo(2));
      Assert.That(health.IsInvulnerable(), Is.EqualTo(false));

      Assert.That(onHealCalled, Is.EqualTo(true));
      Assert.That(onDamageCalled, Is.EqualTo(false));
      Assert.That(onImmunityCalled, Is.EqualTo(false));
      Assert.That(onMaxHealthCalled, Is.EqualTo(true));
      Assert.That(onInjuredCalled, Is.EqualTo(false));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(false));
      Assert.That(onGameOverCalled, Is.EqualTo(false));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(false));
      Assert.That(onRespawnCalled, Is.EqualTo(false));

      ResetCallbacks();

      health.Damage(1, null);

      Assert.That(onHealCalled, Is.EqualTo(false));
      Assert.That(onDamageCalled, Is.EqualTo(true));
      Assert.That(onImmunityCalled, Is.EqualTo(false));
      Assert.That(onMaxHealthCalled, Is.EqualTo(false));
      Assert.That(onInjuredCalled, Is.EqualTo(true));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(false));
      Assert.That(onGameOverCalled, Is.EqualTo(false));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(true));
      Assert.That(onRespawnCalled, Is.EqualTo(false));

      // after recieve damage time to be invulnerable
      Assert.That(health.IsInvulnerable(), Is.EqualTo(true));

      ResetCallbacks();

      umgr.forceFixedDeltaTime = 0.25f;
      // TEST add time, wait until invulnerability ends
      umgr.FixedUpdate();
      Assert.That(health.IsInvulnerable(), Is.EqualTo(true));
      umgr.forceFixedDeltaTime = 2.0f;
      umgr.FixedUpdate();
      Assert.That(health.IsInvulnerable(), Is.EqualTo(false));
      Assert.That(onInvulnerabilityEndCalled, Is.EqualTo(true));

      ResetCallbacks();

      health.immunity = DamageType.Water;

      // TEST invulnerable to recieved damage
      var obj3 = new GameObject();
      obj3.name = "Character3";
      var damage = obj3.AddComponent<Damage>();
      var character2 = obj3.AddComponent<Character>();
      Assert.NotNull(damage);
      damage.type = DamageType.Water;
      damage.causer = character2.GetComponent<CharacterHealth>();
      damage.causer.alignment = Alignment.Enemy;
      health.Damage(damage);
      Assert.That(onHealCalled, Is.EqualTo(false));
      Assert.That(onDamageCalled, Is.EqualTo(true));
      Assert.That(onImmunityCalled, Is.EqualTo(true));
      Assert.That(onMaxHealthCalled, Is.EqualTo(false));
      Assert.That(onInjuredCalled, Is.EqualTo(false));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(false));
      Assert.That(onGameOverCalled, Is.EqualTo(false));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(false));
      Assert.That(onRespawnCalled, Is.EqualTo(false));

      ResetCallbacks();

      /// damage again, kill character

      health.Damage(1, null);
      Assert.That(onHealCalled, Is.EqualTo(true));
      Assert.That(onDamageCalled, Is.EqualTo(true));
      Assert.That(onImmunityCalled, Is.EqualTo(false));
      Assert.That(onMaxHealthCalled, Is.EqualTo(true));
      Assert.That(onInjuredCalled, Is.EqualTo(true));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(true));
      Assert.That(onGameOverCalled, Is.EqualTo(false));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(false));
      Assert.That(onRespawnCalled, Is.EqualTo(true));

      ResetCallbacks();

      Assert.That(health.lives, Is.EqualTo(1));
      Assert.That(health.health, Is.EqualTo(2));

      // kill it!
      health.Kill();
      Assert.That(onHealCalled, Is.EqualTo(false));
      Assert.That(onDamageCalled, Is.EqualTo(false));
      Assert.That(onImmunityCalled, Is.EqualTo(false));
      Assert.That(onMaxHealthCalled, Is.EqualTo(false));
      Assert.That(onInjuredCalled, Is.EqualTo(false));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(true));
      Assert.That(onGameOverCalled, Is.EqualTo(true));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(false));
      Assert.That(onRespawnCalled, Is.EqualTo(false));
    }

    [Test]
    public void AlignamentTest() {
      CharacterHealth health = FixtureCreateHealth("testchar");
      AttachEvents(health);
      ResetCallbacks();

      health.startingLives = 2;
      health.maxLives = 2;
      health.startingHealth = 2;
      health.maxHealth = 2;
      health.alignment = Alignment.Enemy;

      health.Start();

      CharacterHealth health2 = FixtureCreateHealth("testchar2");
      health2.startingLives = 2;
      health2.maxLives = 2;
      health2.startingHealth = 2;
      health2.maxHealth = 2;
      health2.alignment = Alignment.Enemy;
      Damage damage = health2.gameObject.AddComponent<Damage>();

      health2.Start();


      //------------------------------------------
      // DAMAGE NOT DEALT
      // cannot recieve damage from friends
      health.friendlyFire = false;
      ResetCallbacks();

      Assert.That(health.lives, Is.EqualTo(2));
      Assert.That(health.health, Is.EqualTo(2));

      health.Damage(damage);

      Assert.That(health.lives, Is.EqualTo(2));
      Assert.That(health.health, Is.EqualTo(2));

      Assert.That(onHealCalled, Is.EqualTo(false));
      Assert.That(onDamageCalled, Is.EqualTo(false));
      Assert.That(onImmunityCalled, Is.EqualTo(false));
      Assert.That(onMaxHealthCalled, Is.EqualTo(false));
      Assert.That(onInjuredCalled, Is.EqualTo(false));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(false));
      Assert.That(onGameOverCalled, Is.EqualTo(false));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(false));
      Assert.That(onRespawnCalled, Is.EqualTo(false));

      //------------------------------------------
      // DAMAGE NOT DEALT
      // can recieve damage but the damage is not meant for friends
      damage.friendlyFire = false;
      health.friendlyFire = true;

      health.Damage(damage);

      Assert.That(health.lives, Is.EqualTo(2));
      Assert.That(health.health, Is.EqualTo(2));

      Assert.That(onHealCalled, Is.EqualTo(false));
      Assert.That(onDamageCalled, Is.EqualTo(false));
      Assert.That(onImmunityCalled, Is.EqualTo(false));
      Assert.That(onMaxHealthCalled, Is.EqualTo(false));
      Assert.That(onInjuredCalled, Is.EqualTo(false));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(false));
      Assert.That(onGameOverCalled, Is.EqualTo(false));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(false));
      Assert.That(onRespawnCalled, Is.EqualTo(false));


      //------------------------------------------
      // DAMAGE DEALT
      // we can recieve damage from friends and damage is unfrienly!
      damage.friendlyFire = true;
      health.friendlyFire = true;

      Assert.That(health.lives, Is.EqualTo(2));
      Assert.That(health.health, Is.EqualTo(2));

      health.Damage(damage);

      Assert.That(health.lives, Is.EqualTo(2));
      Assert.That(health.health, Is.EqualTo(1));

      Assert.That(onHealCalled, Is.EqualTo(false));
      Assert.That(onDamageCalled, Is.EqualTo(true));
      Assert.That(onImmunityCalled, Is.EqualTo(false));
      Assert.That(onMaxHealthCalled, Is.EqualTo(false));
      Assert.That(onInjuredCalled, Is.EqualTo(true));
      Assert.That(onHurtCalled, Is.EqualTo(false));
      Assert.That(onDeathCalled, Is.EqualTo(false));
      Assert.That(onGameOverCalled, Is.EqualTo(false));
      Assert.That(onInvulnerabilityStartCalled, Is.EqualTo(true));
      Assert.That(onRespawnCalled, Is.EqualTo(false));
    }
  }
}
