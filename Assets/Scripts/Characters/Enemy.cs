using System;
using UnityEngine;

namespace UnityPlatformer {
  public class Enemy : Character {
    public AIInput input;

    public override void OnDeath() {
      Debug.Log("stop enemy updating!");
      UpdateManager.instance.enemies.Remove (this);
    }
  }
}
