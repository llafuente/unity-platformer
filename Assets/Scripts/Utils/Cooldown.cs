using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Climb a ladder
  /// TODO moveToCenterTime/Speed
  /// </summary>
  public class Cooldown {
    int limit;
    int counter;

    public Cooldown(float time) {
      Init(time);
    }

    public void Init(float time) {
      limit = UpdateManager.instance.GetFrameCount (time);
      Reset();
    }

    public bool Ready() {
      return counter >= limit;
    }

    public void Increment() {
      ++counter;
    }

    public void Reset() {
      counter = 0;
    }
  }
}
