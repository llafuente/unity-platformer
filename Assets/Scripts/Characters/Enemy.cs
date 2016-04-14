using System;
using UnityEngine;

namespace UnityPlatformer.Characters {
	public class Enemy : Character {
		public override void OnDeath() {
			Debug.Log("stop enemy updating!");
			UpdateManager.instance.enemies.Remove (this);
		}
	}
}
