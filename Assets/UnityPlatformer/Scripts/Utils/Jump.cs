using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Abstract class for Jumping logic.
  /// </summary>
  public abstract class Jump {
    /// <summary>
    /// Target Character
    /// </summary>
    public Character character;
    /// <summary>
    /// Ticks since jump start
    /// </summary>
    public int ticks;
    /// <summary>
    /// Reset jump data for next jump
    /// </summary>
    public virtual void Reset() {
      ticks = 0;
    }
    /// <summary>
    /// Set initial velocity
    /// </summary>
    public abstract void StartJump(ref Vector3 velocity);
    /// <summary>
    /// Set final velocity
    /// </summary>
    public abstract void EndJump(ref Vector3 velocity);
    /// <summary>
    /// Did apex reached?
    /// </summary>
    public abstract bool IsBeforeApex();
    /// <summary>
    /// Did apex reached?
    /// </summary>
    public abstract bool IsHanging();
    /// <summary>
    /// Set velocity while jumping
    /// </summary>
    public virtual bool Jumping(ref Vector3 velocity, float delta) {
      ++ticks;

      return false;
    }
  }
}
