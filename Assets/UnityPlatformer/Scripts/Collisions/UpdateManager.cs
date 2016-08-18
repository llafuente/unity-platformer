using UnityEngine;
using System;

namespace UnityPlatformer {
  /// <summary>
  /// Custom update loop. This will avoid most of the problems of who is
  /// updated first in exchange of some manual work
  /// NOTE Use a sorted array based on the priorities @Configuration
  /// </summary>
  public class UpdateManager : MBSingleton<UpdateManager> {
    // to scale up/down
    public float timeScale = 1;
    internal float runningTime = 0;
    internal long frame = 0;

    struct ItemPrio {
      public IUpdateEntity entity;
      public int priority;
    }

    // Start with 10, and resize...
    [SerializeField]
    ItemPrio[] frameListeners;
    int used;

    struct Callback {
      public Action callback;
      public float time;
    }
    Callback[] callbacks;
    int usedCallbacks;

    void LazyInit() {
      // NOTE this need to be initialized with new before resize, docs are wrong!
      if (frameListeners == null) {
        frameListeners = new ItemPrio[10];
        used = 0;
      }

      if (callbacks == null) {
        callbacks = new Callback[10];
        usedCallbacks = 0;
      }
    }

    public void OnEnable() {
      LazyInit();
    }

    public int GetFrameCount(float time) {
      float frames = time / Time.fixedDeltaTime;
      int roundedFrames = Mathf.RoundToInt(frames);

      if (Mathf.Approximately(frames, roundedFrames)) {
        return roundedFrames;
      }

      return Mathf.RoundToInt(Mathf.CeilToInt(frames) / timeScale);
    }

    public bool Push(IUpdateEntity entity, int priority) {
      LazyInit();

      int idx = IndexOf(entity);
      if (idx == -1) {
        // resize before overflow!
        if (used == frameListeners.Length) {
          Array.Resize(ref frameListeners, used + 10);
        }

        frameListeners[used].entity = entity;
        frameListeners[used].priority = priority;

        ++used;

        Array.Sort(frameListeners, delegate(ItemPrio a, ItemPrio b) {
          return b.priority - a.priority;
        });

        return true;
      }

      return false;
    }

    public int IndexOf(IUpdateEntity entity) {
      LazyInit();

      for (int i = 0; i < used; ++i) {
        if (frameListeners[i].entity == entity) {
          return i;
        }
      }
      return -1;
    }

    public bool Remove(IUpdateEntity entity) {
      LazyInit();

      int idx = IndexOf(entity);
      if (idx != -1) {
        // frameListeners.Splice(idx, 1);
        for (int i = idx; i < used; ++i) {
          frameListeners[i] = frameListeners[i + 1];
        }
        --used;
        return true;
      }

      return false;
    }

    void Update() {
      ++frame;
    }

    /// <summary>
    /// Update object in order
    /// </summary>
    void FixedUpdate() {
      Log.Verbose("FixedUpdate start: {0}", frame);

      float delta = timeScale * Time.fixedDeltaTime;
      runningTime += delta;

      // update entities
      for (int i = 0; i < used; ++i) {
        frameListeners[i].entity.PlatformerUpdate(delta);
      }
      for (int i = 0; i < used; ++i) {
        frameListeners[i].entity.LatePlatformerUpdate(delta);
      }

      // call callbacks
      for (int i = 0; i < usedCallbacks; ++i) {
        callbacks[i].time -= delta;

        if (callbacks[i].time <= 0) {
          // trigger & 'splice'
          callbacks[i].callback();
          for (int j = i; j < usedCallbacks - 1; ++j) {
            callbacks[j] = callbacks[j + 1];
          }
          --usedCallbacks;
          --i;
        }
      }

      Log.Verbose("FixedUpdate end: {0}", frame);
    }

    public void SetTimeout(Action method, float timeout) {
      if (usedCallbacks == callbacks.Length) {
        Array.Resize(ref callbacks, usedCallbacks + 10);
      }

      callbacks[usedCallbacks].callback = method;
      callbacks[usedCallbacks].time = timeout;

      ++usedCallbacks;
    }
  }
}
