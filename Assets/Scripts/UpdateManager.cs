using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateManager : MonoBehaviour {
	List<PlateformerPlayer> players;
	List<PlatformController> movingPlatforms;

	// Use this for initialization
	void Start () {
		players = new List<PlateformerPlayer>();
		movingPlatforms = new List<PlatformController>();
		var objects = GameObject.FindGameObjectsWithTag(Controller2D.PLAYER_TAG);
		foreach (var obj in objects) {
			Debug.Log("Manage" + obj);
			if (obj.active) {
				players.Add (obj.GetComponent<PlateformerPlayer> ());
			}
		}

		objects = GameObject.FindGameObjectsWithTag(Controller2D.TROUGHT_TAG);
		foreach (var obj in objects) {
			Debug.Log("Manage" + obj);
		    movingPlatforms.Add(obj.GetComponent<PlatformController>());
		}

		objects = GameObject.FindGameObjectsWithTag(Controller2D.MOVINGPLATFORM_TAG);
		foreach (var obj in objects) {
			Debug.Log("Manage" + obj);
		    movingPlatforms.Add(obj.GetComponent<PlatformController>());
		}
	}

	// Update is called once per frame
	void Update () {
		foreach(var obj in movingPlatforms) {
			obj.ManagedUpdate();
		}
		foreach(var obj in players) {
			obj.ManagedUpdate();
		}
	}
}
