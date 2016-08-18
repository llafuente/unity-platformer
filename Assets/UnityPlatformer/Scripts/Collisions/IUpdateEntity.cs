using UnityEngine;
using System.Collections;

namespace UnityPlatformer {
	public interface IUpdateEntity {
    void PlatformerUpdate(float delta);
    void LatePlatformerUpdate(float delta);
  }
}
