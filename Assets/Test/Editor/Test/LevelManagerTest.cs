using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

using UnityPlatformer;

namespace UnityPlatformer.Test {
  [TestFixture]
  [Category("LevelManager")]
  class LevelManagerTest {
    [Test]
    public void CreateLevelManagerTest() {
      var obj = new GameObject();
      var lmgr = obj.AddComponent<LevelManager>();
      Assert.NotNull(lmgr);

      lmgr.levels = new LevelData[3];
      // 0 - main menu
      lmgr.levels[0] = new LevelData();
      lmgr.levels[0].unlock = new int[1];
      lmgr.levels[0].unlock[0] = 1;
      lmgr.levels[0].startLocked = false;
      lmgr.levels[0].isMenu = true;

      // 1-1 - level
      lmgr.levels[1] = new LevelData();
      lmgr.levels[1].unlock = new int[1];
      lmgr.levels[1].unlock[0] = 2;
      lmgr.levels[1].startLocked = true;

      // 1-2 - level
      lmgr.levels[2] = new LevelData();
      lmgr.levels[2].unlock = null;
      lmgr.levels[2].startLocked = true;

      lmgr.Start(); // startLocked -> locked
      Assert.That(lmgr.levels[0].locked, Is.EqualTo(false));
      Assert.That(lmgr.levels[1].locked, Is.EqualTo(true));
      Assert.That(lmgr.levels[2].locked, Is.EqualTo(true));


      Assert.That(lmgr.currentLevel.sceneId, Is.EqualTo(0));

      lmgr.LevelCleared(); // nice hack to start :)
      Assert.That(lmgr.currentLevel.sceneId, Is.EqualTo(1));
      Assert.That(lmgr.currentLevel.locked, Is.EqualTo(false));

      lmgr.LevelCleared();
      Assert.That(lmgr.currentLevel.sceneId, Is.EqualTo(2));
      Assert.That(lmgr.currentLevel.locked, Is.EqualTo(false));
    }
  }
}
/*

        [Test]
        public void ParameterizedTest([Values(1, 2, 3)] int a)
        {
            Assert.Pass();
        }
        [Test]
        public void RangeTest([NUnit.Framework.Range(1, 10, 3)] int x)
        {
            Assert.Pass();
        }

        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "expected message")]
        public void ExpectedExceptionTest()
        {
            throw new ArgumentException("expected message");
        }

        [Datapoint]
        public double zero = 0;
*/
