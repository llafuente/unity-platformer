using System;
using UnityEngine;

namespace UnityPlatformer {
  [RequireComponent (typeof (AIInput))]
  public class Enemy : Character {
    protected AIInput input;
    public override void Start() {
      base.Start();

      input = GetComponent<AIInput>();
    }
    public override void OnDeath() {
      Debug.Log("stop enemy updating!");
      UpdateManager.instance.enemies.Remove (this);
    }
  }
}
