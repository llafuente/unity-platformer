using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Custom update loop. This will avoid most of the problems of who is updated first
/// in exchange of some manual work
/// </summary>
public class UpdateManager : MonoBehaviour {
	// to scale up/down
	public float timeScale = 1;

	[HideInInspector]
	public static List<PlateformerPlayer> players;
	[HideInInspector]
	public static List<PlatformController> movingPlatforms;
	[HideInInspector]
	public static List<Enemy> enemies;

	/// <summary>
	/// Gather all stuff that need to be updated. Object must be tagged appropriately
	/// </summary>
	void Start () {
		players = new List<PlateformerPlayer>();
		movingPlatforms = new List<PlatformController>();
		enemies =  new List<Enemy>();

		var objects = GameObject.FindGameObjectsWithTag(Controller2D.PLAYER_TAG);
		PlateformerPlayer pp;
		foreach (var obj in objects) {
			Debug.Log("Manage" + obj);
			if (obj.activeInHierarchy) {
				pp = obj.GetComponent<PlateformerPlayer> ();
				if (!pp) {
					Debug.LogWarning("Invalid PlateformerPlayer: " + obj);
				}
				players.Add (pp);
				pp.Attach (this);
			}
		}

		objects = GameObject.FindGameObjectsWithTag(Controller2D.ENEMY_TAG);
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

		objects = GameObject.FindGameObjectsWithTag(Controller2D.TROUGHT_TAG);
		foreach (var obj in objects) {
			Debug.Log("Manage" + obj);
			if (obj.activeInHierarchy) {
				movingPlatforms.Add (obj.GetComponent<PlatformController> ());
			}
		}

		objects = GameObject.FindGameObjectsWithTag(Controller2D.MOVINGPLATFORM_TAG);
		foreach (var obj in objects) {
			Debug.Log("Manage" + obj);
			if (obj.activeInHierarchy) {
				movingPlatforms.Add (obj.GetComponent<PlatformController> ());
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
	}
}
