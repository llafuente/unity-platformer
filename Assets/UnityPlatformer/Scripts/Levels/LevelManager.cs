using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

#if CN_INPUT_MANAGER
  using CnControls;
#endif


namespace UnityPlatformer {
  /// <summary>
  /// Manage level loading.\n
  /// It's a singleton and must be included in every scene.\n
  /// We recommended you to prefab this.
  /// </summary>
  public class LevelManager : MBSingleton<LevelManager> {
    /// <summary>
    /// Index of the first scene\n
    /// This should be the main menu
    /// </summary>
    public int firstScene = 0;
    /// <summary>
    /// All your level configuration
    /// </summary>
    public LevelData[] levels;
    /// <summary>
    /// Current level
    /// </summary>
    [HideInInspector]
    public LevelData currentLevel;
    /// <summary>
    /// When level starts
    /// </summary>
    [HideInInspector]
    public Action onLevelStart;
    /// <summary>
    /// When level ends
    /// </summary>
    [HideInInspector]
    public Action onLevelEnd;
    public LevelManager() {}
    public void Start() {
      for(int i = 0; i < levels.Length; ++i) {
        levels[i].locked = levels[i].startLocked;
        levels[i].sceneId = i;
      }
      currentLevel = levels[0];
    }
    /// <summary>
    /// Load given scene
    /// </summary>
    /// <returns>If is going to be loaded (unlocked)</returns>
    public bool Load(LevelData ldata) {
      if (ldata.locked) {
        return false;
      }
      currentLevel = ldata;

      return LoadScene(ldata.sceneName);
    }
    /// <summary>
    /// Load given scene
    /// </summary>
    /// <returns>Always true (can't check locked)</returns>
    public bool LoadScene(string sceneName) {
      // REVIEW LoadSceneMode.Additive ??

      // maybe !Application.isEditor
      if (Application.isPlaying) {
        SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
      }

      if (onLevelStart != null) {
        onLevelStart();
      }
      return true;
    }
    /// <summary>
    /// Load at given scene
    /// </summary>
    /// <returns>If is going to be loaded (unlocked)</returns>
    public bool Load(int index) {
      return Load(levels[index]);
    }
    /// <summary>
    /// Unlock a scene at given index
    /// </summary>
    public void Unlock(int index) {
      levels[index].locked = false;
    }
    /// <summary>
    /// Unlock a scene at given index and load it
    /// </summary>
    public bool UnlockAndLoad(int index) {
      levels[index].locked = false;
      return Load(index);
    }
    /// <summary>
    /// Lock a scene at given index
    /// </summary>
    public void Lock(int index) {
      levels[index].locked = true;
    }
    /// <summary>
    /// Current level is cleared\n
    /// Unlock next levels and load next level (could be a menu or map)
    /// </summary>
    public void LevelCleared() {
      for (int i = 0; i < currentLevel.unlock.Length; ++i) {
        Unlock(currentLevel.unlock[i]);
      }

      int nextLevelId = currentLevel.nextLevel;
      if (nextLevelId == -1) {
        nextLevelId = currentLevel.sceneId + 1;
      }

      Debug.LogFormat("Loading level {0}", nextLevelId);

      if (!Load(levels[nextLevelId])) {
        Debug.LogWarning("Next level is locked, cannnot be loaded!");
      }

      if (onLevelEnd != null) {
        onLevelEnd();
      }
    }
  }
}
