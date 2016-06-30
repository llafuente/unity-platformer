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
    internal long frame = 0;

    struct ItemPrio {
      public IUpdateEntity entity;
      public int priority;
    }

    // Start with 10, and resize...
    [SerializeField]
    ItemPrio[] sortedList;
    int used = 0;

    struct Callback {
      public Action callback;
      public float time;
    }
    Callback[] callbacks;
    int usedCallbacks = 0;

    void LazyInit() {
      if (sortedList == null) {
        Array.Resize(ref sortedList, 10);
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
        if (used == sortedList.Length) {
          Array.Resize(ref sortedList, used + 10);
        }

        sortedList[used].entity = entity;
        sortedList[used].priority = priority;

        ++used;

        Array.Sort(sortedList, delegate(ItemPrio a, ItemPrio b) {
          return b.priority - a.priority;
        });

        return true;
      }

      return false;
    }

    public int IndexOf(IUpdateEntity entity) {
      LazyInit();

      for (int i = 0; i < used; ++i) {
        if (sortedList[i].entity == entity) {
          return i;
        }
      }
      return -1;
    }

    public bool Remove(IUpdateEntity entity) {
      LazyInit();

      int idx = IndexOf(entity);
      if (idx != -1) {
        // sortedList.Splice(idx, 1);
        for (int i = idx; i < used; ++i) {
          sortedList[i] = sortedList[i + 1];
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
      float delta = timeScale * Time.fixedDeltaTime;

      // update entities
      for (int i = 0; i < used; ++i) {
        sortedList[i].entity.ManagedUpdate(delta);
      }

      // call callbacks
      for (int i = 0; i < usedCallbacks; ++i) {
        callbacks[i].time -= delta;
        if (callbacks[i].time <= 0) {
          // trigger & 'splice'
          callbacks[i].callback();
          for (int j = i; j < usedCallbacks; ++j) {
            sortedList[j] = sortedList[j + 1];
          }
          --usedCallbacks;
          --i;
        }
      }
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
