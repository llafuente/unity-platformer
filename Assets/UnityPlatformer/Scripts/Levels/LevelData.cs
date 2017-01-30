using UnityEngine;
using System.Collections.Generic;
using System;

#if CN_INPUT_MANAGER
  using CnControls;
#endif


namespace UnityPlatformer {
  [Serializable]
  public class LevelData {
    /// <summary>
    /// Scene name
    /// </summary>
    [Comment("Index of the scene in the 'Scenes In Build'")]
    [Scene]
    public int sceneIndex = 0;
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
    /// Index of the next level
    /// </summary>
    [Scene]
    public int nextLevel = 0;
    /// <summary>
    /// levels that will be unlocked\n
    /// Leave it empty to unlock the next level
    /// </summary>
    [Comment("Levels that will be unlocked. Leave it empty to unlock the next level")]
    [Scene]
    public int[] unlock;
    /// <summary>
    /// Start locked?
    /// </summary>
    public bool startLocked = true;
    /// <summary>
    /// Is currently locked?\n
    /// TODO persist
    /// </summary>
    [HideInInspector]
    public bool locked = false;
    public LevelData() {}
  }
}
