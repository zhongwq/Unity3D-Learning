using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsActionManager : SSActionManager, ActionCallback {
	public FirstController firstController;

	public void Start () {
		firstController = (FirstController)GameDirector.getInstance ().currentSceneController;
		firstController.physicsActionManager = this;
	}

	public void Update() {
		base.Update ();
	}

	public void actionDone (SSAction source) {
		if (((ArrowFlyAction)source).type == 1) {
			firstController.arrowFactory.recycle (((ArrowFlyAction)source).gameObject.GetComponent<ArrowControl> ().arrowController);
			firstController.shootFinish = true;
		}
	}
}
