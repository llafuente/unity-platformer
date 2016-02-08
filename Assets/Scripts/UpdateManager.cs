using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Custom update loop. This will avoid most of the problems of who is updated first
/// in exchange of some manual work
/// </summary>
public class UpdateManager : MonoBehaviour {
	List<PlateformerPlayer> players;
	List<PlatformController> movingPlatforms;

	/// <summary>
	/// Gather all stuff that need to be updated. Object must be tagged appropriately
	/// </summary>
	void Start () {
		players = new List<PlateformerPlayer>();
		movingPlatforms = new List<PlatformController>();
		var objects = GameObject.FindGameObjectsWithTag(Controller2D.PLAYER_TAG);
		foreach (var obj in objects) {
			Debug.Log("Manage" + obj);
			if (obj.activeInHierarchy) {
				players.Add (obj.GetComponent<PlateformerPlayer> ());
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

	/// <summary>
	/// Update those object we manage in order: MovingPlatdorms - players
	/// </summary>
	void Update () {
		foreach(var obj in movingPlatforms) {
			obj.ManagedUpdate();
		}
		foreach(var obj in players) {
			obj.ManagedUpdate();
		}
	}
}
