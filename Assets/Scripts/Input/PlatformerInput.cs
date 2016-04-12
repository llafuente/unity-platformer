using UnityEngine;
using System.Collections.Generic;
using System;

#if CN_INPUT_MANAGER
  using CnControls;
#endif


namespace UnityPlatformer {
  /// <summary>
  /// </summary>
  public abstract class PlatformerInput : MonoBehaviour
  {
    public abstract bool IsActionDown(string action);
    public abstract bool IsActionButtonDown(string action);
    public abstract bool IsLeftDown();
    public abstract bool IsRightDown();
    public abstract bool IsUpDown();
    public abstract bool IsDownDown();
    public abstract float GetAxisRawX();
    public abstract float GetAxisRawY();
    public abstract Vector2 GetAxisRaw();
  }
}
