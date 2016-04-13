using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityPlatformer.Characters;
using UnityPlatformer.Tiles;

namespace UnityPlatformer {
	/// <summary>
	/// Custom update loop. This will avoid most of the problems of who is
	/// updated first in exchange of some manual work / tagging
	/// </summary>
	public class UpdateManager : MonoBehaviour {
		// to scale up/down
		public float timeScale = 1;

		[HideInInspector]
		public static List<Character> players;
		[HideInInspector]
		public static List<MovingPlatform> movingPlatforms;
		[HideInInspector]
		public static List<Enemy> enemies;
		[HideInInspector]
		public static List<Projectile> projectiles;

		/// <summary>
		/// Gather all stuff that need to be updated. Object must be tagged appropriately
		/// </summary>
		void Start () {
			players = new List<Character>();
			movingPlatforms = new List<MovingPlatform>();
			enemies =  new List<Enemy>();
			projectiles =  new List<Projectile>();

			var objects = GameObject.FindGameObjectsWithTag(Configuration.instance.playerTag);
			Character pp;
			foreach (var obj in objects) {
				Debug.Log("Manage" + obj);
				if (obj.activeInHierarchy) {
					pp = obj.GetComponent<Character> ();
					if (!pp) {
						Debug.LogWarning("Invalid Character: " + obj);
					}
					players.Add (pp);
					pp.Attach (this);
				}
			}

			objects = GameObject.FindGameObjectsWithTag(Configuration.instance.enemyTag);
			Enemy eny;
			foreach (var obj in objects) {
				Debug.Log("Manage" + obj);
				if (obj.activeInHierarchy) {
					eny = obj.GetComponent<Enemy> ();
					if (!eny) {
						Debug.LogWarning("Invalid Enemy: " + obj);
					}
					enemies.Add (eny);
					//c2d.Attach (this);
				}
			}

			objects = GameObject.FindGameObjectsWithTag(Configuration.instance.movingPlatformThroughTag);
			foreach (var obj in objects) {
				Debug.Log("Manage" + obj);
				if (obj.activeInHierarchy) {
					movingPlatforms.Add (obj.GetComponent<MovingPlatform> ());
				}
			}

			objects = GameObject.FindGameObjectsWithTag(Configuration.instance.movingPlatformTag);
			foreach (var obj in objects) {
				Debug.Log("Manage" + obj);
				if (obj.activeInHierarchy) {
					movingPlatforms.Add (obj.GetComponent<MovingPlatform> ());
				}
			}

			objects = GameObject.FindGameObjectsWithTag(Configuration.instance.projectileTag);
			foreach (var obj in objects) {
				Debug.Log("Manage" + obj);
				Projectile projectile = obj.GetComponent<Projectile> ();
				if (obj.activeInHierarchy) {
					projectiles.Add (projectile);
				}
			}
		}


		public int GetFrameCount(float time) {
			float frames = time / Time.fixedDeltaTime;
			int roundedFrames = Mathf.RoundToInt(frames);

			if (Mathf.Approximately(frames, roundedFrames)) {
				return roundedFrames;
			}

			return Mathf.RoundToInt(Mathf.CeilToInt(frames) / timeScale);
		}

		/// <summary>
		/// Update those object we manage in order: MovingPlatdorms - players
		/// </summary>
		void FixedUpdate() {
			foreach(var obj in movingPlatforms) {
				obj.ManagedUpdate(Time.fixedDeltaTime);
			}
			foreach(var obj in players) {
				obj.ManagedUpdate(Time.fixedDeltaTime);
			}
			foreach(var obj in enemies) {
				obj.ManagedUpdate(Time.fixedDeltaTime);
			}
			foreach(var obj in projectiles) {
				obj.ManagedUpdate(Time.fixedDeltaTime);
			}
		}
	}
}
