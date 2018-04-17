using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyController {
    private int currentDifficulty = 0;  // 0-2
    public float currentSendTime;

    public readonly int UFONumber = 10;
    int[] sendUfoTime = { 30, 20, 10 };
	int[] succesScore = { 24, 16, 6 };
    float[] UFOSize = { 1F, 0.8F, 0.5F };
    float[] UFOSpeed = { 10, 12, 15 };
	Color[] UFOColor = { Color.blue, Color.gray, Color.red };

    public Text difficultyInfo;

    private static DifficultyController instance;
    public static DifficultyController getInstance()
    {
        if (instance == null)
        {
            instance = new DifficultyController();
        }
        return instance;
    }

    private DifficultyController()
    {
        currentDifficulty = 0;
        currentSendTime = sendUfoTime[0];
        difficultyInfo = (GameObject.Instantiate(Resources.Load("Prefabs/DifficultInfo")) as GameObject).transform.Find("Text").GetComponent<Text>();
        difficultyInfo.text = "" + currentDifficulty;
    }

    public UFOData getUFOAttributes()
    {
        return new UFOData(UFOSize[currentDifficulty], UFOSpeed[currentDifficulty], UFOColor[currentDifficulty]);
    }

    public void increaseDifficulty()
    {
        if (currentDifficulty < 2)
        {
            currentDifficulty++;
            currentSendTime = sendUfoTime[currentDifficulty];
            difficultyInfo.text = "" + currentDifficulty;
        }
    }

    public int getDifficulty()
    {
        return currentDifficulty;
    }

	public int getEmitNum()
	{
		return sendUfoTime [currentDifficulty];
	}	

	public int getSuccessScore()
	{
		return succesScore [currentDifficulty];
	}

    public void setDifficulty(int difficulty)
    {
        if (currentDifficulty != difficulty)
        {
            if (difficulty > 2)
            {
                Debug.Log(difficulty);
                throw new System.Exception("difficulty is out of range!");
            }

            currentDifficulty = difficulty;
            currentSendTime = sendUfoTime[currentDifficulty];
            difficultyInfo.text = "" + currentDifficulty;
        }
    }

    public void setDifficultyByScore(int currentScore)
    {
        if (currentScore > 30)
        {
            setDifficulty(2);
        }
        else if (currentScore > 15)
        {
            setDifficulty(1);
        }
        else
        {
            setDifficulty(0);
        }
    }

    public void resetDifficulty()
    {
        currentDifficulty = 0;
        currentSendTime = sendUfoTime[currentDifficulty];
		difficultyInfo.text = "" + currentDifficulty;
    }
}
