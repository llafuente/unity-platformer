using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Cooldown helper
  /// @deprecated use Delay instead
  /// </summary>
  public class Cooldown : IUpdateEntity {
    /// <summary>
    /// time since last reset
    /// </summary>
    protected float counter;
    /// <summary>
    /// cooldown time in seconds
    /// </summary>
    protected float seconds;
    /// <summary>
    /// boolean to keep track of when fire the callbacks
    /// </summary>
    protected bool wasReady = false;
    /// <summary>
    /// callbacks
    /// </summary>
    public Action onReset;
    /// <summary>
    /// callbacks
    /// </summary>
    public Action onReady;
    /// <summary>
    /// constructor
    /// </summary>
    public Cooldown(float timeInSeconds) {
      counter = seconds = timeInSeconds;
      wasReady = true;

      UpdateManager.Push(this, Configuration.instance.cooldownsPriority);
    }
    /// <summary>
    /// Is ready? cooldown expired?
    /// </summary>
    public bool Ready() {
      return counter >= seconds;
    }
    /// <summary>
    /// Update cooldown
    /// </summary>
    public void Set(float timeInSeconds) {
      seconds = timeInSeconds;
    }
    /// <summary>
    /// Reset
    /// </summary>
    public void Reset() {
      counter = 0.0f;
      wasReady = false;
      if (seconds > 0.0f && onReset != null) {
        onReset();
      }
    }
    /// <summary>
    /// Clear cooldown -> ready
    /// </summary>
    public void Clear() {
      counter = seconds + 1;
    }

    public void PlatformerUpdate(float delta) {
      counter += delta;
      if (counter >= seconds && !wasReady) {
        wasReady = true;
        if (onReady != null) {
          onReady();
        }
      }
    }

    public void LatePlatformerUpdate(float delta) {}
  }
}
