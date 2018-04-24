using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCrack : MonoBehaviour {
	public FirstController firstController;
	public ScoreRecorder scoreRecorder;

	void Start() {
		firstController = (FirstController)GameDirector.getInstance ().currentSceneController;
		scoreRecorder = ScoreRecorder.getInstance ();
	}

	void OnTriggerEnter(Collider arrow) {
		if (arrow.gameObject.GetComponent<ArrowControl> ().arrowController.available == true) {
			arrow.gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			arrow.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
			arrow.gameObject.GetComponent<ArrowControl> ().arrowController.available = false;
			arrow.gameObject.transform.GetComponent<Collider> ().enabled = false;
			firstController.shootFinish = true;
			firstController.showCamera.showCamera ();
			scoreRecorder.record (gameObject);
		}
	}
}
