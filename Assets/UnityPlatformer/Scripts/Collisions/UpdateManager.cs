using UnityEngine;
using System;
using UnityEngine.UI;

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
    /// Create a debug UI
    /// </summary>
    public bool debug = false;
    /// <summary>
    /// Time scale
    /// </summary>
    [HideInInspector]
    private float _timeScale = 1;
    /// <summary>
    /// Time Scale in the next frame\n
    /// This variable is for consistency, when anybody change timeScale
    /// the change is queued until next FixedUpdate
    /// </summary>
    private float nextFrameTimeScale = 1;
    /// <summary>
    /// Time Scale
    ///
    /// > 1 fast motion\n
    /// < 1 slow motion
    /// </summary>
    public float timeScale {
      get
      {
        return _timeScale;
      }
      set
      {
        nextFrameTimeScale = value;

        if (onTimeScaleChanged != null) {
          onTimeScaleChanged(value);
        }
      }
    }

    /// <summary>
    /// Time the app is running
    /// </summary>
    [HideInInspector]
    public float runningTime = 0;
    /// <summary>
    /// Current frame
    /// </summary>
    [HideInInspector]
    public long frame = 0;
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
    /// Time change callback
    /// </summary>
    public delegate void TimeChanged(float timeScale);
    /// <summary>
    /// notify when time is changed
    /// </summary>
    public TimeChanged onTimeScaleChanged;
    /// <summary>
    /// Initialize
    /// </summary>
    static void LazyInit() {
      // NOTE this need to be initialized with new before resize, docs are wrong!
      if (instance.frameListeners == null) {
        instance.frameListeners = new ItemPrio[10];
        instance.frameListenersCount = 0;
      }

      if (instance.callbacks == null) {
        instance.callbacks = new Callback[10];
        instance.callbacksCount = 0;
      }
    }
    /// <summary>
    /// LazyInit
    /// </summary>
    public void OnEnable() {
      LazyInit();

      if (instance.debug) {
        transform.GetChild(0).gameObject.SetActive(true);
        var slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener((value) => {
          UpdateManager.instance.timeScale = value;
        });
      }
    }
    /// <summary>
    /// Get current frame
    /// </summary>
    static public long GetCurrentFrame() {
      return instance.frame;
    }
    /// <summary>
    /// Get current frame
    /// </summary>
    public void SetTimeScale2(float ts) {
      Debug.Log(ts);
      timeScale = ts;
      if (onTimeScaleChanged != null) {
        onTimeScaleChanged(ts);
      }
    }
    /// <summary>
    /// Time to frame conversion
    ///
    /// NOTE you should listen to timeScale changes...
    /// </summary>
    static public int GetFrameCount(float time) {
      float frames = time / Time.fixedDeltaTime;
      int roundedFrames = Mathf.RoundToInt(frames);

      if (Mathf.Approximately(frames, roundedFrames)) {
        return roundedFrames;
      }

      return Mathf.RoundToInt(Mathf.CeilToInt(frames) / instance.timeScale);
    }
    /// <summary>
    /// Push a new entity to update loop
    /// </summary>
    static public bool Push(IUpdateEntity entity, int priority) {
      LazyInit();

      int idx = IndexOf(entity);
      if (idx == -1) {
        // resize before overflow!
        if (instance.frameListenersCount == instance.frameListeners.Length) {
          Array.Resize(ref instance.frameListeners, instance.frameListenersCount + 10);
        }

        instance.frameListeners[instance.frameListenersCount].entity = entity;
        instance.frameListeners[instance.frameListenersCount].priority = priority;

        ++instance.frameListenersCount;

        Array.Sort(instance.frameListeners, delegate(ItemPrio a, ItemPrio b) {
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
    static public int IndexOf(IUpdateEntity entity) {
      LazyInit();

      for (int i = 0; i < instance.frameListenersCount; ++i) {
        if (instance.frameListeners[i].entity == entity) {
          return i;
        }
      }
      return -1;
    }
    /// <summary>
    /// Remove given entity from frameListeners
    /// </summary>
    /// </returns>If it was removed</returns>
    static public bool Remove(IUpdateEntity entity) {
      if (_instance == null) {
        return false;
      }

      int idx = IndexOf(entity);
      if (idx != -1) {
        // frameListeners.Splice(idx, 1);
        for (int i = idx; i < instance.frameListenersCount; ++i) {
          instance.frameListeners[i] = instance.frameListeners[i + 1];
        }
        --instance.frameListenersCount;
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
    public void FixedUpdate() {
      Log.Verbose("FixedUpdate start: {0} listeners: {1} callbacks: {2}",
        frame, frameListenersCount, callbacksCount);

      _timeScale = nextFrameTimeScale;

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
    static public void SetTimeout(Action callback, float timeout) {
      if (instance.callbacksCount == instance.callbacks.Length) {
        Array.Resize(ref instance.callbacks, instance.callbacksCount + 10);
      }

      instance.callbacks[instance.callbacksCount].callback = callback;
      instance.callbacks[instance.callbacksCount].time = timeout;

      ++instance.callbacksCount;
    }
    /// <summary>
    /// Remove once given callback
    /// </summary>
    /// <returns>true if found, false if not found</returns>
    static public bool ClearTimeout(Action callback, float timeout) {
      for (int i = 0; i < instance.callbacksCount; ++i) {
        if (instance.callbacks[i].callback == callback) {
          for (int j = i; j < instance.callbacksCount - 1; ++j) {
            instance.callbacks[j] = instance.callbacks[j + 1];
          }
          --instance.callbacksCount;
          return true;
        }
      }

      return false;
    }
  }
}
