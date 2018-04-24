using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRecorder {
	public int score = 0;

	Text gameInfo;

	private static ScoreRecorder instance;
	public static ScoreRecorder getInstance()
	{
		if (instance == null)
		{
			instance = new ScoreRecorder();
		}
		return instance;
	}

	private ScoreRecorder() {
		gameInfo = (GameObject.Instantiate (Resources.Load ("Prefabs/ScoreInfo")) as GameObject).transform.Find ("Text").GetComponent<Text> ();
		gameInfo.text = "" + score;
	}

	public void record(GameObject hitObj) {
		switch (hitObj.name) {
			case "1":
				score += 1;
				break;
			case "2":
				score += 2;
				break;
			case "3":
				score += 3;
				break;
			case "4":
				score += 4;
				break;
			case "5":
				score += 5;
				break;
		}
		gameInfo.text = "" + score;
	}


	public int getScore() {
		return score;
	}

	public void reset() {
		score = 0;
		gameInfo.text = "" + score;
	}
}
