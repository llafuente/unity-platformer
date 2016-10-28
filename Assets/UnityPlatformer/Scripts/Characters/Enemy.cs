using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Base class for Enemies
  /// </summary>
  public class Enemy : Character {
    public AIInput input;
    /// <summary>
    /// Listen death event
    ///
    /// TODO do something...
    /// </summary>
    public override void OnDeath() {
      Debug.Log("stop enemy updating!");
      UpdateManager.instance.Remove (this);
    }
    /// <summary>
    /// check missconfiguration
    /// </summary>
    public virtual void Start() {
      if (input == null) {
        Debug.LogWarning("input cannot be null", this);
      }
    }
    /// <summary>
    /// sync UpdateManager
    /// </summary>
    public override void OnEnable() {
      base.OnEnable();
      UpdateManager.instance.Push(this, Configuration.instance.enemiesPriority);
    }
    /// <summary>
    /// sync UpdateManager
    /// </summary>
    public override void OnDisable() {
      base.OnDisable();
      UpdateManager.instance.Remove(this);
    }
  }
}
