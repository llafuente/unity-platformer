using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace UnityPlatformer {
  /// <summary>
  /// Custom update loop.
  ///
  /// The purpose behind this class/style is to be precise about the update
  /// order of all entities.<para />
  /// Also handle Timeout Actions, do not use corountines.<para />
  /// NOTE Use a sorted array based on the priorities defined at Configuration<para />
  /// NOTE executionOrder should be -50<para />
  /// NOTE Do not call UpdateManager at Awake
  /// </summary>
  public class UpdateManager : MBSingleton<UpdateManager> {
    /// <summary>
    /// Create a debug UI
    /// </summary>
    public bool debug = false;
    /// <summary>
    /// Ignore Unity Time.fixedDeltaTime use this value\n
    /// this is meant for testing, because setting Time.fixedDeltaTime
    /// may mess your configuration
    /// </summary>
    [HideInInspector]
    public float forceFixedDeltaTime = 0.0f;
    /// <summary>
    /// Time scale
    /// </summary>
    [HideInInspector]
    protected float _timeScale = 1;
    /// <summary>
    /// Time Scale in the next frame\n
    /// This variable is for consistency, when anybody change timeScale
    /// the change is queued until next FixedUpdate
    /// </summary>
    protected float nextFrameTimeScale = 1;
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
      /// <summary>
      /// the callback itself
      /// </summary>
      public Action callback;
      /// <summary>
      /// Time to call callback. Start at initial time and when reach <= 0 fire!
      /// </summary>
      public float time;
      /// <summary>
      /// Initial time to repeat, if zero callback will be executed once
      /// </summary>
      public float repeat;
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
    /// Get current delta
    /// </summary>
    static public float GetFixedDeltaTime() {
      return instance.forceFixedDeltaTime != 0.0f ? instance.forceFixedDeltaTime : Time.fixedDeltaTime;
    }
    /// <summary>
    /// Time to frame conversion
    ///
    /// NOTE you should listen to timeScale changes...
    /// </summary>
    static public int GetFrameCount(float time) {
      float frames = time / GetFixedDeltaTime();
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
      Assert.IsNotNull(entity, "(UpdateManager) Push(entity = null)");

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
        for (int i = idx; i < instance.frameListenersCount - 1; ++i) {
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
    static public float GetCurrentDelta() {
      return instance.timeScale * GetFixedDeltaTime();
    }
    /// <summary>
    /// Run update loop
    ///
    /// First call PlatformerUpdate\n
    /// Then call LatePlatformerUpdate\n
    /// NOTE FixedUpdate can be called multiple times each frame
    /// </summary>
    public void FixedUpdate() {
      float delta = GetCurrentDelta();
      _timeScale = nextFrameTimeScale;

      runningTime += delta;

      Log.Verbose("start: {0} listeners: {1} callbacks: {2} delta {3} runningTime {4}",
      frame, frameListenersCount, callbacksCount, delta, runningTime);

      //Debug.LogFormat("FixedUpdate start: {0} listeners: {1} callbacks: {2} delta {3} runningTime {4} -- {5}",
      //frame, frameListenersCount, callbacksCount, delta, runningTime, frameListeners);

      // update entities
      for (int i = 0; i < frameListenersCount; ++i) {
        /*
        Debug.LogFormat("{0} {1}", i, frameListeners[i].entity);
        if (frameListeners[i].entity == null) {
          for (int j = i; j < frameListenersCount; ++j) {
            frameListeners[j] = frameListeners[j + 1];
          }
          --frameListenersCount;
          --i;
          continue;
        }
        */

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
          if (callbacks[i].repeat == 0.0f) {
            for (int j = i; j < callbacksCount - 1; ++j) {
              callbacks[j] = callbacks[j + 1];
            }
            --callbacksCount;
            --i;
          } else {
            callbacks[i].time += callbacks[i].repeat;
          }
        }
      }

      Log.Verbose("end: {0}", frame);
    }
    /// <summary>
    /// Call given callback in given timeout
    /// if timeout is &lt;= 0, will be called now.
    ///
    /// NOTE Do not use corountines because the can't hotswap
    /// Also corountines don't know if you modify timeScale this do.
    /// </summary>
    static public void SetTimeout(Action callback, float timeout) {
      if (timeout <= 0.0f) {
        callback();
        return;
      }

      LazyInit();

      if (instance.callbacksCount == instance.callbacks.Length) {
        Array.Resize(ref instance.callbacks, instance.callbacksCount + 10);
      }

      instance.callbacks[instance.callbacksCount].callback = callback;
      instance.callbacks[instance.callbacksCount].time = timeout;
      instance.callbacks[instance.callbacksCount].repeat = 0.0f;

      ++instance.callbacksCount;
    }
    /// <summary>
    /// Call given callback in given timeout
    ///
    /// NOTE Do not use corountines because the can't hotswap
    /// Also corountines don't know if you modify timeScale this do.
    /// </summary>
    static public void SetInterval(Action callback, float timeout) {
      LazyInit();

      if (instance.callbacksCount == instance.callbacks.Length) {
        Array.Resize(ref instance.callbacks, instance.callbacksCount + 10);
      }

      instance.callbacks[instance.callbacksCount].callback = callback;
      instance.callbacks[instance.callbacksCount].time = timeout;
      instance.callbacks[instance.callbacksCount].repeat = timeout;

      ++instance.callbacksCount;
    }
    /// <summary>
    /// Remove once given callback
    /// </summary>
    /// <returns>true if found, false if not found</returns>
    static public bool ClearTimeout(Action callback) {
      LazyInit();

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
