using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFlyAction : SSAction {
	public int type; // 0: windAction, 1: shootAction
	public Vector3 speed;
	bool firstTime = false;

	private ArrowFlyAction() {}

	public static ArrowFlyAction GetSSAction(int type, Vector3 speed) {
		ArrowFlyAction arrowFlyAction = ScriptableObject.CreateInstance<ArrowFlyAction> ();
		arrowFlyAction.speed = speed;
		arrowFlyAction.firstTime = true;
		arrowFlyAction.type = type;
		return arrowFlyAction;
	}

	public override void Start() {}

	public override void Update() {
		if (firstTime) {
			if (type == 0) {
				// wind
				this.gameObject.GetComponent<Rigidbody> ().AddForce (speed, ForceMode.Force);
			} else {
				// shoot
				this.gameObject.GetComponent<Rigidbody> ().AddForce (speed, ForceMode.VelocityChange);
			}
			firstTime = false;
		}
		if (this.gameObject.transform.position.z > 50) {
			this.destroy = true;
			this.callback.actionDone (this);
		}
		if (this.gameObject.GetComponent<ArrowControl> ().arrowController.available == false) {
			this.destroy = true;
		}
	}
}
