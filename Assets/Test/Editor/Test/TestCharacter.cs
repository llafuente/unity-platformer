using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

using UnityPlatformer;

namespace UnityPlatformer.Test {
  class TestCharacter {
    public GameObject charObj;
    public Character character;
    public CharacterHealth health;

    public HitBox enterHitBox;
    public BoxCollider2D enterBox2D;

    public HitBox dealHitBox;
    public BoxCollider2D dealBox2D;
    public Damage damage;

    public HitBox recieveHitBox;
    public BoxCollider2D recieveBox2D;

    // health callbacks
    public bool onHealCalled = false;
    public bool onDamageCalled = false;
    public bool onImmunityCalled = false;
    public bool onMaxHealthCalled = false;
    public bool onInjuredCalled = false;
    public bool onHurtCalled = false;
    public bool onDeathCalled = false;
    public bool onGameOverCalled = false;
    public bool onInvulnerabilityStartCalled = false;
    public bool onInvulnerabilityEndCalled = false;
    public bool onRespawnCalled = false;

    public TestCharacter(string name) {
      charObj = new GameObject();
      charObj.name = name;
      character = charObj.AddComponent<Character>();
      Assert.NotNull(character);
      health = charObj.GetComponent<CharacterHealth>();
      Assert.NotNull(health);
      health.Start();

      GameObject enterGO = charObj.CreateChild("enterAreas");
      enterGO.AddComponent<BoxCollider2D>();
      enterHitBox = enterGO.AddComponent<HitBox>();
      enterHitBox.type = HitBoxType.EnterAreas;
      enterHitBox.gameObject.layer = 1;

      enterBox2D = enterHitBox.gameObject.AddComponent<BoxCollider2D>();
      enterBox2D.size = new Vector2(1, 1);
      enterHitBox.owner = health;
      enterHitBox.Start();

      GameObject dealGO = charObj.CreateChild("dealHitBox");
      dealGO.AddComponent<BoxCollider2D>();
      dealHitBox = dealGO.AddComponent<HitBox>();
      Assert.NotNull(dealHitBox);

      dealHitBox.type = HitBoxType.DealDamage;
      dealHitBox.useGlobalMask = false;
      dealHitBox.collisionMask = (1 << 2); // | (1 << ?)
      dealHitBox.gameObject.layer = 2;
      damage = dealHitBox.gameObject.AddComponent<Damage>();
      damage.causer = health;
      damage.Start();
      dealHitBox.owner = health;

      dealBox2D = dealHitBox.gameObject.AddComponent<BoxCollider2D>();
      dealBox2D.size = new Vector2(1, 1);
      dealHitBox.Start();

      GameObject recieveGO = charObj.CreateChild("recieveHitBox");
      recieveGO.AddComponent<BoxCollider2D>();
      recieveHitBox = recieveGO.AddComponent<HitBox>();
      Assert.NotNull(recieveHitBox);

      recieveHitBox.type = HitBoxType.RecieveDamage;
      recieveHitBox.useGlobalMask = false;
      recieveHitBox.collisionMask = (1 << 2);
      recieveHitBox.gameObject.layer = 2;
      recieveHitBox.owner = health;

      recieveBox2D = recieveHitBox.gameObject.AddComponent<BoxCollider2D>();
      recieveBox2D.size = new Vector2(1, 1);
      recieveHitBox.Start();

      // init health callbacks
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

    }
    public void ResetCallbacks() {
      // health callbacks
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
  }
}
