using UnityEngine;
using UnityEngine.Rendering;
using System.IO;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityPlatformer {
  public enum LogLevel {
    Error = 1,
    Warning = 2,
    Info = 3,
    Verbose = 4,
    Debug = 5,
    Silly = 6
  }

  /// <summary>
  /// Shortcut to create a Trigger2D, configure: BoxCollider2D and Rigidbody2D.
  /// *NOTE* BoxCollider2D and Rigidbody2D will be hidden.
  /// </summary>
  public class Log {
    private static Log _instance;

    public static Log instance {
      get {
        if (_instance == null) {
            _instance = new Log();
        }
        return _instance;
      }
    }

    static StreamWriter streamWriter = new StreamWriter("./log");
    static public LogLevel level = LogLevel.Info;

    static public void Write(LogLevel lvl, string s) {
      if (lvl <= level) {
        streamWriter.WriteLine(s);
      }
    }

    static public void Error(string format, params System.Object[] values) {
      string s = String.Format(format, values);

      Write(LogLevel.Error, s);
    }


    static public void Warning(string format, params System.Object[] values) {
      string s = String.Format(format, values);

      Write(LogLevel.Warning, s);
    }

    static public void Info(string format, params System.Object[] values) {
      string s = String.Format(format, values);

      Write(LogLevel.Info, s);
    }

    static public void Debug(string format, params System.Object[] values) {
      string s = String.Format(format, values);

      Write(LogLevel.Debug, s);
    }

    static public void Verbose(string format, params System.Object[] values) {
      string s = String.Format(format, values);
      Write(LogLevel.Verbose, s);
    }

    static public void Silly(string format, params System.Object[] values) {
      string s = String.Format(format, values);
      Write(LogLevel.Silly, s);
    }

  }
}
