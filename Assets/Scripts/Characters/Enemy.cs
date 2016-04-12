using System;
using UnityEngine;

namespace UnityPlatformer.Characters {
	[RequireComponent (typeof (CharacterHealth))]
	public class Enemy : Character {
		public override void OnDeath() {
			Debug.Log("stop enemy updating!");
			UpdateManager.enemies.Remove (this);
		}
	}
}
