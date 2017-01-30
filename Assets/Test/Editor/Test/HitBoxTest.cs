using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

using UnityPlatformer;

namespace UnityPlatformer.Test {
  [TestFixture]
  [Category("HitBox")]
  class HitBoxTest {
    TestCharacter char1;
    TestCharacter char2;

    Configuration config;
    UpdateManager umgr;

    void Fixture() {
      Configuration.ClearInstance();
      UpdateManager.ClearInstance();
      System.GC.Collect();

      var obj = new GameObject();
      config = obj.AddComponent<Configuration>();
      Assert.NotNull(config);
      umgr = obj.AddComponent<UpdateManager>();
      Assert.NotNull(umgr);

      char1 = new TestCharacter("char1");
      char1.health.alignment = Alignment.Allied;
      char2 = new TestCharacter("char2");
      char2.health.alignment = Alignment.Enemy;
    }

    [Test]
    public void DamageTest() {
      Log.Debug("Start test");
      Fixture();

      char1.dealHitBox.OnTriggerEnter2D(char2.recieveBox2D);

      Assert.That(char1.onInjuredCalled, Is.EqualTo(false));
      Assert.That(char1.onHurtCalled, Is.EqualTo(true));
      Assert.That(char2.onInjuredCalled, Is.EqualTo(true));
      Assert.That(char2.onHurtCalled, Is.EqualTo(false));

      char1.ResetCallbacks();
      char2.ResetCallbacks();
      char1.recieveHitBox.OnTriggerEnter2D(char2.dealBox2D);

      Assert.That(char1.onInjuredCalled, Is.EqualTo(false));
      Assert.That(char1.onHurtCalled, Is.EqualTo(false));
      Assert.That(char2.onInjuredCalled, Is.EqualTo(false));
      Assert.That(char2.onHurtCalled, Is.EqualTo(false));
    }
  }
}
