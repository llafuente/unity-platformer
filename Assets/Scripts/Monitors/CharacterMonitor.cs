using System;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Monitors {
  public class CharacterMonitor : ControllerMonitor {

    Character character;

    override public void Start() {
      base.Start ();
      character = GetComponent<Character> ();
    }
    override public  void OnGUI() {
      base.OnGUI ();
    }

    override public void Update() {
      base.Update ();
      text += string.Format(
        "Area: {0}\n"+
        "State: {1}\n"+
        "Ladder: {2}\n" +
        "Platform: {3}\n",
        character.state.ToString(),
        character.area.ToString(),
        character.ladder,
        character.platform
      );
    }
  }
}
