using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WindMake : MonoBehaviour {
	public Vector3 Wind;
	Text gameInfo;
	int fpsCount = 0;

	void Awake() {
		gameInfo = (GameObject.Instantiate (Resources.Load ("Prefabs/WindInfo")) as GameObject).transform.Find ("Text").GetComponent<Text> ();
		makeWind ();
		fpsCount = 0;
	}

	void Update() {
		fpsCount++;
		if (fpsCount == 240) {
			Wind = makeWind ();
			fpsCount = 0;
		}// 240帧换一个风向
	}

	public Vector3 makeWind() {
		float y = Random.Range (-100, 100);
		float z = Random.Range (-30, 0);
		float x = Random.Range (-50, 50);
		gameInfo.text = "(" + x + "," + y + "," + z + ")";
		return new Vector3 (x, y, z);
	}

	public Vector3 getWind() {
		return Wind;
	}
}
