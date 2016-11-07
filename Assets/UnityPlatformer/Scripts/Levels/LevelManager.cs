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
    internal LevelData currentLevel;
    /// <summary>
    /// When level starts
    /// </summary>
    internal Action onLevelStart;
    /// <summary>
    /// When level ends
    /// </summary>
    internal Action onLevelEnd;
    /// <summary>
    /// Load given scene
    /// </summary>
    /// <returns>If is going to be loaded (unlocked)</returns>
    bool Load(LevelData ldata) {
      if (!ldata.unlocked) {
        return false;
      }

      return LoadScene(ldata.sceneName);
    }
    /// <summary>
    /// Load given scene
    /// </summary>
    /// <returns>Always true (can't check locked)</returns>
    bool LoadScene(string sceneName) {
      // LoadSceneMode.Additive ??
      SceneManager.LoadScene (sceneName, LoadSceneMode.Single);

      if (onLevelStart != null) {
        onLevelStart();
      }
      return true;
    }
    /// <summary>
    /// Load at given scene
    /// </summary>
    /// <returns>If is going to be loaded (unlocked)</returns>
    bool LoadByIndex(int index) {
      return Load(levels[index]);
    }
    /// <summary>
    /// Unlock a scene at given index
    /// </summary>
    void Unlock(int index) {
      levels[index].unlocked = true;
    }
    /// <summary>
    /// Lock a scene at given index
    /// </summary>
    void Lock(int index) {
      levels[index].unlocked = false;
    }
    /// <summary>
    /// Current level is cleared\n
    /// Unlock next levels and load next level (could be a menu or map)
    /// </summary>
    void LevelCleared() {
      for (int i = 0; i < currentLevel.unlock.Length; ++i) {
        Unlock(currentLevel.unlock[i]);
      }

      if (!Load(levels[currentLevel.nextLevel])) {
        Debug.LogWarning("Next level is locked, cannnot be loaded!");
      }

      if (onLevelEnd != null) {
        onLevelEnd();
      }
    }
  }
}
