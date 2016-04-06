using System;
using UnityEngine;

namespace UnityPlatformer {
	[RequireComponent (typeof (CharacterHealth))]
	public class Enemy : MonoBehaviour, UpdateEntity {
		public virtual void Start() {
			CharacterHealth ch = GetComponent<CharacterHealth>();
			ch.onDeath += OnDeath;
		}

		public void OnDeath() {
			Debug.Log("stop enemy updating!");
			UpdateManager.enemies.Remove (this);
		}

		public virtual void ManagedUpdate(float delta) {

		}
	}
}
