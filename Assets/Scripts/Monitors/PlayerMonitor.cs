﻿using System;
using UnityPlatformer.Characters;

namespace UnityPlatformer.Monitors {
	public class PlayerMonitor : ControllerMonitor {
		Character player;

		override public void Start() {
			base.Start ();
			player = GetComponent<Character> ();
		}
		override public  void OnGUI() {
			base.OnGUI ();
		}

		override public void Update() {
			base.Update ();
		}
	}
}