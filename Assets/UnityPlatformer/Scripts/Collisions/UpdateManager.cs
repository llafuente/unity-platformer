using UnityEngine;
using System;

namespace UnityPlatformer {
  /// <summary>
  /// Custom update loop.
  ///
  /// The purpose behind this class/style is to be precise about the update
  /// order of all entities.\n
  /// Also handle Timeout Actions, do not use corountines.\n
  /// NOTE Use a sorted array based on the priorities defined at Configuration\n
  /// NOTE executionOrder should be -50
  /// </summary>
  public class UpdateManager : MBSingleton<UpdateManager> {
    /// <summary>
    /// Time Scale
    ///
    /// > 1 fast motion\n
    /// < 1 slow motion
    /// </summary>
    public float timeScale = 1;
    /// <summary>
    /// Time the app is running
    /// </summary>
    internal float runningTime = 0;
    /// <summary>
    /// Current frame
    /// </summary>
    internal long frame = 0;
    /// <summary>
    /// Priority queue type
    /// </summary>
    struct ItemPrio {
      public IUpdateEntity entity;
      public int priority;
    }
    /// <summary>
    /// Priority queue frame listeners
    /// </summary>
    [SerializeField]
    ItemPrio[] frameListeners;
    /// <summary>
    /// frameListeners in use
    /// </summary>
    int frameListenersCount;
    /// <summary>
    /// Callback type
    /// </summary>
    struct Callback {
      public Action callback;
      public float time;
    }
    /// <summary>
    /// List of callbacks
    /// </summary>
    Callback[] callbacks;
    /// <summary>
    /// Scheduled callbacks
    /// </summary>
    int callbacksCount;
    /// <summary>
    /// Initialize
    /// </summary>
    void LazyInit() {
      // NOTE this need to be initialized with new before resize, docs are wrong!
      if (frameListeners == null) {
        frameListeners = new ItemPrio[10];
        frameListenersCount = 0;
      }

      if (callbacks == null) {
        callbacks = new Callback[10];
        callbacksCount = 0;
      }
    }
    /// <summary>
    /// LazyInit
    /// </summary>
    public void OnEnable() {
      LazyInit();
    }
    /// <summary>
    /// FPS
    /// </summary>
    public int GetFrameCount(float time) {
      float frames = time / Time.fixedDeltaTime;
      int roundedFrames = Mathf.RoundToInt(frames);

      if (Mathf.Approximately(frames, roundedFrames)) {
        return roundedFrames;
      }

      return Mathf.RoundToInt(Mathf.CeilToInt(frames) / timeScale);
    }
    /// <summary>
    /// Push a new entity to update loop
    /// </summary>
    public bool Push(IUpdateEntity entity, int priority) {
      LazyInit();

      int idx = IndexOf(entity);
      if (idx == -1) {
        // resize before overflow!
        if (frameListenersCount == frameListeners.Length) {
          Array.Resize(ref frameListeners, frameListenersCount + 10);
        }

        frameListeners[frameListenersCount].entity = entity;
        frameListeners[frameListenersCount].priority = priority;

        ++frameListenersCount;

        Array.Sort(frameListeners, delegate(ItemPrio a, ItemPrio b) {
          return b.priority - a.priority;
        });

        return true;
      }

      return false;
    }
    /// <summary>
    /// Index of given entity in the frameListeners
    /// </summary>
    /// </returns>>= 0 if found (index). -1 if not found</returns>
    public int IndexOf(IUpdateEntity entity) {
      LazyInit();

      for (int i = 0; i < frameListenersCount; ++i) {
        if (frameListeners[i].entity == entity) {
          return i;
        }
      }
      return -1;
    }
    /// <summary>
    /// Remove given entity from frameListeners
    /// </summary>
    /// </returns>If it was removed</returns>
    public bool Remove(IUpdateEntity entity) {
      LazyInit();

      int idx = IndexOf(entity);
      if (idx != -1) {
        // frameListeners.Splice(idx, 1);
        for (int i = idx; i < frameListenersCount; ++i) {
          frameListeners[i] = frameListeners[i + 1];
        }
        --frameListenersCount;
        return true;
      }

      return false;
    }
    /// <summary>
    /// Increment frame count
    /// </summary>
    void Update() {
      ++frame;
    }

    /// <summary>
    /// Run update loop
    ///
    /// First call PlatformerUpdate\n
    /// Then call LatePlatformerUpdate\n
    /// NOTE FixedUpdate can be called multiple times each frame
    /// </summary>
    void FixedUpdate() {
      Log.Verbose("FixedUpdate start: {0}", frame);

      float delta = timeScale * Time.fixedDeltaTime;
      runningTime += delta;

      // update entities
      for (int i = 0; i < frameListenersCount; ++i) {
        frameListeners[i].entity.PlatformerUpdate(delta);
      }
      for (int i = 0; i < frameListenersCount; ++i) {
        frameListeners[i].entity.LatePlatformerUpdate(delta);
      }

      // call callbacks
      for (int i = 0; i < callbacksCount; ++i) {
        callbacks[i].time -= delta;

        if (callbacks[i].time <= 0) {
          // trigger & 'splice'
          callbacks[i].callback();
          for (int j = i; j < callbacksCount - 1; ++j) {
            callbacks[j] = callbacks[j + 1];
          }
          --callbacksCount;
          --i;
        }
      }

      Log.Verbose("FixedUpdate end: {0}", frame);
    }
    /// <summary>
    /// Call given callback in given timeout
    ///
    /// NOTE Do not use corountines because the can't hotswap
    /// Also corountines don't know if you modify timeScale this do.
    /// </summary>
    public void SetTimeout(Action callback, float timeout) {
      if (callbacksCount == callbacks.Length) {
        Array.Resize(ref callbacks, callbacksCount + 10);
      }

      callbacks[callbacksCount].callback = callback;
      callbacks[callbacksCount].time = timeout;

      ++callbacksCount;
    }
  }
}
