using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreRecorder{
	private int score;

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

	private ScoreRecorder()
	{
		gameInfo = (GameObject.Instantiate(Resources.Load("Prefabs/ScoreInfo")) as GameObject).transform.Find("Text").GetComponent<Text>();
		gameInfo.text = "" + score;
	}

	public void record(int difficulty)
	{
		score += difficulty+1;
		gameInfo.text = "" + score;
	}

	public void reset() 
	{
		score = 0;
		gameInfo.text = "" + score;
	}

	public int getScore()
	{
		return score;
	}
}
