using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Cooldown helper
  /// </summary>
  public class Delay {
    /// <summary>
    /// Time since last reset
    /// </summary>
    protected float elapsedTime;
    /// <summary>
    /// Cooldown time in seconds
    /// </summary>
    protected float refTime;
    /// <summary>
    /// Boolean to keep track of when fire the callbacks
    /// </summary>
    protected bool wasReady = false;
    /// <summary>
    /// Callbacks
    /// </summary>
    public Action onReset;
    /// <summary>
    /// Callbacks
    /// </summary>
    public Action onFulfilled;

    public Delay(float timeInSeconds) {
      elapsedTime = refTime = timeInSeconds;
      Reset();
    }
    /// <summary>
    /// Is ready? cooldown expired?
    /// </summary>
    public bool Fulfilled() {
      return elapsedTime >= refTime;
    }
    /// <summary>
    /// Update cooldown
    /// </summary>
    public void Set(float timeInSeconds) {
      refTime = timeInSeconds;
    }
    /// <summary>
    /// Reset
    /// </summary>
    public void Reset(bool clearCallbacks = false) {
      elapsedTime = 0.0f;
      wasReady = false;
      if (onReset != null) {
        onReset();
      }

      if (clearCallbacks) {
        onReset = null;
        onFulfilled = null;
      }
    }
    /// <summary>
    /// Update with refTime, so it will be instantly fulfilled
    /// </summary>
    public void Clear() {
      Update(refTime);
    }

    public void Update(float delta) {
      elapsedTime += delta;
      if (elapsedTime >= refTime && !wasReady) {
        wasReady = true;
        if (onFulfilled != null) {
          onFulfilled();
        }
      }
    }
  }
}
