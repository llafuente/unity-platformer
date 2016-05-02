using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Math behind the Jump
  /// </summary>
  public abstract class Jump {
    public Character character;
    public int ticks;

    public virtual void Reset() {
      ticks = 0;
    }

    public abstract void StartJump(ref Vector3 velocity);
    public abstract void EndJump(ref Vector3 velocity);
    public abstract bool IsBeforeApex();
    public abstract bool IsHanging();

    public virtual bool Jumping(ref Vector3 velocity) {
      ++ticks;

      return false;
    }
  }
}
