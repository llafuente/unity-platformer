using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Assertions;

namespace UnityPlatformer {
  /// <summary>
  /// Manage level loading.\n
  /// It's a singleton and must be included in every scene.\n
  /// We recommended you to prefab this.
  /// </summary>
  public class LevelManager : MBSingleton<LevelManager> {
    /// <summary>
    /// Index of the Main menu
    /// </summary>
    [Comment("Index of the main menu in the 'Scenes In Build'")]
    [Scene]
    public int mainMenu = 0;
    /// <summary>
    /// Index of the world map
    /// </summary>
    [Comment("Index of the world map in the 'Scenes In Build'")]
    [Scene]
    public int worldMap = 1;
    /// <summary>
    /// Index of the first scene\n
    /// This should be the main menu
    /// </summary>
    [Comment("Index of the first scene @levels")]
    public int firstScene = 0;
    /// <summary>
    /// All your level configuration
    /// </summary>
    public LevelData[] levels = new LevelData[1];
    /// <summary>
    /// Current level
    /// </summary>
    [HideInInspector]
    public LevelData currentLevel = null;
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
      int idx = SceneManager.GetActiveScene().buildIndex;
      if (idx == -1) {
        // this means unit-test or not in the build
        // try not fail...
        idx = 0;
      }

      for(int i = 0; i < levels.Length; ++i) {
        levels[i].locked = levels[i].startLocked;

        if (idx == levels[i].sceneIndex) {
          currentLevel = levels[i];
        }
      }

      Assert.IsNotNull(currentLevel, "(LevelManager) currentLevel not found");

      // load state from file
    }
    /// <summary>
    /// Load given scene if not locked
    /// </summary>
    /// <returns>will be loaded?</returns>
    public bool Load(int buildIndex) {
      LevelData ldata = GetByBuildIndex(buildIndex);

      Debug.LogFormat("locked? {1} - {0}", buildIndex, ldata.locked);

      if (ldata.locked) {
        return false;
      }
      currentLevel = ldata;

      return LoadScene(ldata.sceneIndex);
    }
    /// <summary>
    /// Load given scene regardless being locked
    /// </summary>
    /// <returns>Always true (can't check locked)</returns>
    public bool LoadScene(int buildIndex) {
      // REVIEW LoadSceneMode.Additive ??

      // maybe !Application.isEditor
      if (Application.isPlaying) {
        SceneManager.LoadScene (buildIndex, LoadSceneMode.Single);
      }

      if (onLevelStart != null) {
        onLevelStart();
      }
      return true;
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
    /// Unlock a scene at given index
    /// </summary>
    public void Unlock(int buildIndex) {
      GetByBuildIndex(buildIndex).locked = false;

      Debug.LogFormat("Unlock {1} - {0}", buildIndex, GetByBuildIndex(buildIndex).locked);
    }
    /// <summary>
    /// Unlock a scene at given index and load it
    /// </summary>
    public bool UnlockAndLoad(int buildIndex) {
      LevelData d = GetByBuildIndex(buildIndex);
      d.locked = false;
      return Load(buildIndex);
    }
    /// <summary>
    /// Lock a scene at given index
    /// </summary>
    public void Lock(int buildIndex) {
      GetByBuildIndex(buildIndex).locked = true;
    }
    /// <summary>
    /// Current level is cleared\n
    /// Unlock next levels and load next level (could be a menu or map)
    /// </summary>
    public void LevelCleared() {
      LevelData d = GetByBuildIndex(currentLevel.nextLevel);
      Assert.IsNotNull(d, "(LevelManager) Cannot find LevelData of " + currentLevel.nextLevel);

      if (currentLevel.unlock == null || currentLevel.unlock.Length == 0) {
        Unlock(currentLevel.nextLevel);
      } else {
        for (int i = 0; i < currentLevel.unlock.Length; ++i) {
          Unlock(currentLevel.unlock[i]);
        }
      }

      Assert.IsTrue(Load(currentLevel.nextLevel), "(LevelManager) Next level is locked, cannnot be loaded");

      if (onLevelEnd != null) {
        onLevelEnd();
      }
    }

    LevelData GetByBuildIndex(int buildIndex) {
      for (int i = 0; i < levels.Length; ++i) {
        if (buildIndex == levels[i].sceneIndex) {
          //Debug.LogFormat("buildIndex: {0} @{1}", buildIndex, i);
          return levels[i];
        }
      }

      return null;
    }
  }
}
