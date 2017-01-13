using System;
using UnityEngine;

namespace UnityPlatformer {
  /// <summary>
  /// Static Character
  /// This is a nice hack for breackable ropes. Just disable all 'Action' logic
  /// </summary>
  public class StaticCharacter : Character {
    /// <summary>
    /// Listen death event
    ///
    /// TODO do something...
    /// </summary>
    public override void OnDeath() {
      Debug.Log("stop enemy updating!");
      UpdateManager.Remove (this);
    }
    /// <summary>
    /// sync UpdateManager
    /// </summary>
    public override void OnEnable() {
      base.OnEnable();
      UpdateManager.Push(this, Configuration.instance.charactersPriority);
    }
    /// <summary>
    /// sync UpdateManager
    /// </summary>
    public override void OnDisable() {
      base.OnDisable();
      UpdateManager.Remove(this);
    }
    /// <summary>
    /// just do nothing!
    /// </summary>
    public override void PlatformerUpdate(float delta) {
    }
    /// <summary>
    /// just do nothing!
    /// </summary>
    public override void LatePlatformerUpdate(float delta) {
    }
  }
}
