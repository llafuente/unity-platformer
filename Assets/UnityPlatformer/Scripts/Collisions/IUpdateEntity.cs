using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
  /// <summary>
  /// Interface to interact with UpdateManager
  /// </summary>
  public interface IUpdateEntity {
    /// <summary>
    /// Event like FixedUpdate
    /// </summary>
    void PlatformerUpdate(float delta);
    /// <summary>
    /// Event fired after every entity recieve PlatformerUpdate
    /// </summary>
    void LatePlatformerUpdate(float delta);
  }
}
