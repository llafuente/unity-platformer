using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

namespace UnityPlatformer {
  public class NextLevelTrigger: BoxTrigger2D {
    public float delay = 0.0f;
    public void Start() {
      onTriggerEnter2D += (Collider2D o) => {
        Debug.Log(o);
        UpdateManager.SetTimeout(NextLevel, delay);
        //LevelManager.instance.
      };
    }

    void NextLevel() {
      LevelManager.instance.LevelCleared();
    }
  }
}
