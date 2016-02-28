using System;

public class PlayerMonitor : ControllerMonitor
{
	PlateformerPlayer player;

	override public void Start() {
		base.Start ();
		player = GetComponent<PlateformerPlayer> ();
	}
	override public  void OnGUI() {
		base.OnGUI ();
	}

	override public void Update() {
		base.Update ();
	}
}