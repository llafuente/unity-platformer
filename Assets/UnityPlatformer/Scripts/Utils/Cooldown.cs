using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Cooldown helper
  /// </summary>
  public class Cooldown {
    /// <summary>
    /// frames to cooldown
    /// </summary>
    int limit;
    /// <summary>
    /// frames since las reset
    /// </summary>
    int counter;
    /// <summary>
    /// Constructor
    /// </summary>
    public Cooldown(float time) {
      Init(time);
    }
    /// <summary>
    /// Initialize max time
    /// </summary>
    public void Init(float time) {
      limit = UpdateManager.instance.GetFrameCount (time);
      Reset();
    }
    /// <summary>
    /// Increment and check
    /// </summary>
    public bool IncReady() {
      Increment();
      return Ready();
    }
    /// <summary>
    /// Is ready, cooldown expired
    /// </summary>
    public bool Ready() {
      return counter >= limit;
    }
    /// <summary>
    /// Increment counter
    /// </summary>
    public void Increment() {
      ++counter;
    }
    /// <summary>
    /// Reset
    /// </summary>
    public void Reset() {
      counter = 0;
    }
  }
}
