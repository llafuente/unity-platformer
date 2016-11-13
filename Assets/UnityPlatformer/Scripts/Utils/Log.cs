using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
using System;

namespace UnityPlatformer {
  /// <summary>
  /// Log levels
  /// </summary>
  public enum LogLevel {
    Error = 1,
    Warning = 2,
    Info = 3,
    Verbose = 4,
    Debug = 5,
    Silly = 6
  }

  /// <summary>
  /// Log to file
  /// </summary>
  public class Log {
    /// <summary>
    /// real instance
    /// </summary>
    private static Log _instance;
    /// <summary>
    /// Singleton
    /// </summary>
    public static Log instance {
      get {
        if (_instance == null) {
            _instance = new Log();
        }
        return _instance;
      }
    }
    /// <summary>
    /// file instance
    /// </summary>
    public StreamWriter streamWriter = null;
    /// <summary>
    /// Current log level
    /// </summary>
    public LogLevel level = LogLevel.Silly;
    /// <summary>
    /// protected constructor for Singleton usage
    /// </summary>
    protected Log() {
      if (Application.isPlaying) {
        streamWriter = new StreamWriter("./log");
      }
    }
    static public void SetLevel(LogLevel lvl) {
      instance.level = lvl;
    }
    /// <summary>
    /// Write to file
    /// </summary>
    static public void Write(LogLevel lvl, string s) {
      if (instance.streamWriter != null && lvl <= instance.level) {
        instance.streamWriter.WriteLine(s);
      }
    }
    /// <summary>
    /// Log an error message
    /// </summary>
    static public void Error(string format, params System.Object[] values) {
      string s = String.Format(format, values);

      Write(LogLevel.Error, s);
    }
    /// <summary>
    /// Log a warning message
    /// </summary>
    static public void Warning(string format, params System.Object[] values) {
      string s = String.Format(format, values);

      Write(LogLevel.Warning, s);
    }
    /// <summary>
    /// Log an info message
    /// </summary>
    static public void Info(string format, params System.Object[] values) {
      string s = String.Format(format, values);

      Write(LogLevel.Info, s);
    }
    /// <summary>
    /// Log debug message
    /// </summary>
    static public void Debug(string format, params System.Object[] values) {
      string s = String.Format(format, values);

      Write(LogLevel.Debug, s);
    }
    /// <summary>
    /// Log verbose
    /// </summary>
    static public void Verbose(string format, params System.Object[] values) {
      string s = String.Format(format, values);
      Write(LogLevel.Verbose, s);
    }
    /// <summary>
    /// Log silly
    /// </summary>
    static public void Silly(string format, params System.Object[] values) {
      string s = String.Format(format, values);
      Write(LogLevel.Silly, s);
    }
  }
}
