using System;
using UnityEngine;

namespace UnityPlatformer {
  public class Enemy : Character {
    public AIInput input;

    public override void OnDeath() {
      Debug.Log("stop enemy updating!");
      UpdateManager.instance.Remove (this);
    }

    public override void OnEnable() {
      base.OnEnable();
      UpdateManager.instance.Push(this, Configuration.instance.enemiesPriority);
    }

    public override void OnDisable() {
      base.OnDisable();
      UpdateManager.instance.Remove(this);
    }
  }
}
