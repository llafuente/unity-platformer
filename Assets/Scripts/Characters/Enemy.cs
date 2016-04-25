using System;
using UnityEngine;

namespace UnityPlatformer {
  public class Enemy : Character {
    public AIInput input;

    public override void OnDeath() {
      Debug.Log("stop enemy updating!");
      UpdateManager.instance.enemies.Remove (this);
    }

    public override void OnEnable() {
      UpdateManager.instance.enemies.Add(this);
    }

    public override void OnDisable() {
      UpdateManager.instance.enemies.Remove(this);
    }
  }
}
