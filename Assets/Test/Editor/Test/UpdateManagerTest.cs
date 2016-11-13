using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

using UnityPlatformer;

namespace UnityPlatformer.Test {
  [TestFixture]
  [Category("UpdateManager")]
  class UpdateManagerTest {
    bool callbackCalled = false;
    bool callback2Called = false;
    bool callback3Called = false;

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

      ResetCallbacks();
    }

    void ResetCallbacks() {
      callbackCalled = false;
      callback2Called = false;
      callback3Called = false;
    }

    [Test]
    public void SetIntervalTest() {
      Fixture();
      UpdateManager.SetInterval(() => { callbackCalled = true; }, 1.0f);
      UpdateManager.SetInterval(() => { callback2Called = true; }, 2.0f);
      UpdateManager.SetInterval(() => { callback3Called = true; }, 3.0f);

      //1s
      umgr.forceFixedDeltaTime = 0.5f;
      umgr.FixedUpdate();
      umgr.FixedUpdate();

      Assert.That(callbackCalled, Is.EqualTo(true));
      Assert.That(callback2Called, Is.EqualTo(false));
      Assert.That(callback3Called, Is.EqualTo(false));

      ResetCallbacks();
      //2s
      umgr.FixedUpdate();
      umgr.FixedUpdate();

      Assert.That(callbackCalled, Is.EqualTo(true));
      Assert.That(callback2Called, Is.EqualTo(true));
      Assert.That(callback3Called, Is.EqualTo(false));

      ResetCallbacks();
      //3s
      umgr.FixedUpdate();
      umgr.FixedUpdate();

      Assert.That(callbackCalled, Is.EqualTo(true));
      Assert.That(callback2Called, Is.EqualTo(false));
      Assert.That(callback3Called, Is.EqualTo(true));

      ResetCallbacks();
      //4s
      umgr.FixedUpdate();
      umgr.FixedUpdate();

      Assert.That(callbackCalled, Is.EqualTo(true));
      Assert.That(callback2Called, Is.EqualTo(true));
      Assert.That(callback3Called, Is.EqualTo(false));

      ResetCallbacks();
      //5s
      umgr.FixedUpdate();
      umgr.FixedUpdate();

      Assert.That(callbackCalled, Is.EqualTo(true));
      Assert.That(callback2Called, Is.EqualTo(false));
      Assert.That(callback3Called, Is.EqualTo(false));

      ResetCallbacks();
      //6s
      umgr.FixedUpdate();
      umgr.FixedUpdate();

      Assert.That(callbackCalled, Is.EqualTo(true));
      Assert.That(callback2Called, Is.EqualTo(true));
      Assert.That(callback3Called, Is.EqualTo(true));

      ResetCallbacks();
    }
  }
}
