using UnityEngine;
using System.Collections.Generic;
using System;

#if CN_INPUT_MANAGER
  using CnControls;
#endif


namespace UnityPlatformer {
  public class LevelData {
    /// <summary>
    /// World number, to keep your data tide.
    /// </summary>
    public int world;
    /// <summary>
    /// Level number, to keep your data tide.
    /// </summary>
    public int level;
    /// <summary>
    /// Is a bonud level?
    /// </summary>
    public bool isBonusLevel;
    /// <summary>
    /// Is a menu?
    /// </summary>
    public bool isMenu;
    /// <summary>
    /// Index of the next level\n
    /// -1 means next index
    /// </summary>
    public int nextLevel = -1;
    /// <summary>
    /// List of LevelData Indexes (@LevelManager) that will be unlocked
    /// </summary>
    public int[] unlock;
    /// <summary>
    /// Start locked?\n
    /// NOTE main menu should be false
    /// </summary>
    public bool startLocked = true;
    /// <summary>
    /// Scene name
    /// </summary>
    public string sceneName;
    /// <summary>
    /// Is currently locked?\n
    /// TODO persist
    /// </summary>
    [HideInInspector]
    public int sceneId = 0;
    /// <summary>
    /// Is currently locked?\n
    /// TODO persist
    /// </summary>
    [HideInInspector]
    public bool locked = false;
    public LevelData() {}
  }
}
