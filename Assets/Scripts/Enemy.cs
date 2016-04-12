using System;
using UnityEngine;

namespace UnityPlatformer {
	[RequireComponent (typeof (CharacterHealth))]
	public class Enemy : Character {
		public override void OnDeath() {
			Debug.Log("stop enemy updating!");
			UpdateManager.enemies.Remove (this);
		}
	}
}
