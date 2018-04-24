using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class showSubCamera : MonoBehaviour {
	private float time = 0;
	private bool show = false;
	GameObject cameraObj;

	// Use this for initialization
	void Start () {
		show = false;
		cameraObj = (GameObject.Instantiate (Resources.Load ("Prefabs/SubCamera")) as GameObject);
		cameraObj.AddComponent<Skybox>().material = Resources.Load("Nostalgia 1") as Material;
		cameraObj.SetActive (false);
	}

	public void showCamera() {
		cameraObj.SetActive (true);
		show = true;
		time = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (show) {
			time -= Time.deltaTime;
			if (time <= 0) {
				show = false;
				cameraObj.SetActive (false);
			}
		}
	}
}
