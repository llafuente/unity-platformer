using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityPlatformer {
  /// <summary>
  /// Custom update loop. This will avoid most of the problems of who is
  /// updated first in exchange of some manual work
  /// TODO REVIEW use a priority queue to handle everything? put priority @Configuration
  /// </summary>
  public class UpdateManager : MBSingleton<UpdateManager> {
    // to scale up/down
    public float timeScale = 1;

    [HideInInspector]
    public List<Character> characters = new List<Character>();
    [HideInInspector]
    public List<MovingPlatform> movingPlatforms = new List<MovingPlatform>();
    [HideInInspector]
    public List<Enemy> enemies = new List<Enemy>();
    [HideInInspector]
    public List<Projectile> projectiles = new List<Projectile>();
    [HideInInspector]
    public List<IUpdateEntity> others = new List<IUpdateEntity>();


    public int GetFrameCount(float time) {
      float frames = time / Time.fixedDeltaTime;
      int roundedFrames = Mathf.RoundToInt(frames);

      if (Mathf.Approximately(frames, roundedFrames)) {
        return roundedFrames;
      }

      return Mathf.RoundToInt(Mathf.CeilToInt(frames) / timeScale);
    }

    /// <summary>
    /// Update those object we manage in order: MovingPlatdorms - characters
    /// </summary>
    void FixedUpdate() {
      float delta = timeScale * Time.fixedDeltaTime;

      foreach(var obj in movingPlatforms) {
        obj.ManagedUpdate(delta);
      }
      foreach(var obj in characters) {
        obj.ManagedUpdate(delta);
      }
      foreach(var obj in enemies) {
        obj.ManagedUpdate(delta);
      }
      foreach(var obj in projectiles) {
        obj.ManagedUpdate(delta);
      }
      foreach(var obj in others) {
        obj.ManagedUpdate(delta);
      }

    }
  }
}
