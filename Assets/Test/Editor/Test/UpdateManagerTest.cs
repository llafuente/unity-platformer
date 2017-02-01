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

    [Test]
    public void DelayTest() {
      Fixture();
      Delay d1 = UpdateManager.GetDelay(1.0f);
      Assert.That(umgr.delays[0], Is.EqualTo(d1));
      Delay d2 = UpdateManager.GetDelay(1.5f);
      Assert.That(umgr.delays[1], Is.EqualTo(d2));
      Delay d3 = UpdateManager.GetDelay(2.0f);
      Assert.That(umgr.delays[2], Is.EqualTo(d3));
      Assert.That(umgr.delaysCount, Is.EqualTo(3));

      umgr.forceFixedDeltaTime = 0.5f;

      umgr.timeScale = 1.0f;
      umgr.onTimeScaleChanged += (float timeScale) => {
        Assert.That(timeScale, Is.EqualTo(0.5f));
        callbackCalled = true;
      };


      // +1s
      umgr.FixedUpdate();

      // timeScale change is applied to the next frame, not current
      // do the change now, won't affect the first second FixedUpdate
      umgr.timeScale = 0.5f;
      Assert.That(callbackCalled, Is.EqualTo(true));
      ResetCallbacks();

      umgr.FixedUpdate();

      Assert.That(d1.Fulfilled(), Is.EqualTo(true));
      Assert.That(d2.Fulfilled(), Is.EqualTo(false));
      Assert.That(d3.Fulfilled(), Is.EqualTo(false));

      umgr.FixedUpdate(); // this just advance 0.25f total

      Assert.That(d1.Fulfilled(), Is.EqualTo(true));
      Assert.That(d2.Fulfilled(), Is.EqualTo(false));
      Assert.That(d3.Fulfilled(), Is.EqualTo(false));
      Assert.That(callbackCalled, Is.EqualTo(false));



      umgr.FixedUpdate(); // this just advance 0.25f total

      Assert.That(d1.Fulfilled(), Is.EqualTo(true));
      Assert.That(d2.Fulfilled(), Is.EqualTo(true));
      Assert.That(d3.Fulfilled(), Is.EqualTo(false));
      Assert.That(callbackCalled, Is.EqualTo(false));

      // d3 will never get fullfiled
      UpdateManager.DisposeDelay(d3);
      Assert.That(umgr.freeDelays[0], Is.EqualTo(d3));
      Assert.That(umgr.freeDelaysCount, Is.EqualTo(1));
      umgr.FixedUpdate();
      umgr.FixedUpdate();
      umgr.FixedUpdate();
      umgr.FixedUpdate();
      umgr.FixedUpdate();
      Assert.That(d3.Fulfilled(), Is.EqualTo(false));

      // Delay get reused
      Delay d4 = UpdateManager.GetDelay(1.0f);
      Assert.That(d4, Is.EqualTo(d3));
      Assert.That(umgr.freeDelaysCount, Is.EqualTo(0));
    }
  }
}
