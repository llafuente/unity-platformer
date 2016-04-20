using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Tiles {
  [CustomEditor(typeof(MovingPlatform))]
  class MovingPlatformEditor : Editor {
    /* TODO FIXME i want this to be displayed all the time not just when selected.
    void OnEnable(){
      SceneView.onSceneGUIDelegate += CustomSceneGUI;
    }
    void OnDisable(){
      SceneView.onSceneGUIDelegate -= CustomSceneGUI;
    }
    void CustomSceneGUI(UnityEditor.SceneView sceneView) {
    */


    void CustomSceneGUI() {
      // Do your drawing here using Handles.
      //Handles.BeginGUI();

      MovingPlatform mp = target as MovingPlatform;

      if (mp.localWaypoints != null) {
        Handles.color = Color.red;
        float size = .3f;

        for (int i =0; i < mp.localWaypoints.Length; i ++) {
          Vector3 globalWaypointPos = (Application.isPlaying) ? mp.globalWaypoints[i] : mp.localWaypoints[i] + mp.transform.position;
          Handles.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
          Handles.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
          Handles.Label(globalWaypointPos + new Vector3(-0.1f, 0.8f, 0), "" + i);;
        }
      }
      // Do your drawing here using GUI.
      //Handles.EndGUI();
    }
  }
}
