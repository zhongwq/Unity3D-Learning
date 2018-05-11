using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRecorder: Observer {
	public int score = 0;

	private int status = 0; // 使进出配对，避免Reset时加多一分
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
		status = 0;
		Publisher publish = Publisher.getInstance ();
		publish.add (this);
	}

	public void addScore() {
		score += 1;
		gameInfo.text = "" + score;
	}


	public int getScore() {
		return score;
	}

	public void reset() {
		score = 0;
		status = 0;
		gameInfo.text = "" + score;
	}

	public void notified (ActionType type, int position, GameObject actor) {
		if (type == ActionType.ENTER) {
			status = 1;
		} else if (type == ActionType.EXIT && status == 1) {
			addScore ();
		}
	}
}
