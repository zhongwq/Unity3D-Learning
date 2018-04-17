using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyController {
    private int currentDifficulty = 0;  // 0-2
    public float currentSendInterval;

    public readonly int UFONumber = 10;
    float[] sendUFOInterval = { 10, 9, 8 };
    float[] UFOSize = { 1F, 0.9F, 0.8F };
    float[] UFOSpeed = { 5, 6, 7 };
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
        currentSendInterval = sendUFOInterval[0];
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
            currentSendInterval = sendUFOInterval[currentDifficulty];
            difficultyInfo.text = "" + currentDifficulty;
        }
    }

    public int getDifficulty()
    {
        return currentDifficulty;
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
            currentSendInterval = sendUFOInterval[currentDifficulty];
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
        currentSendInterval = sendUFOInterval[currentDifficulty];
		difficultyInfo.text = "" + currentDifficulty;
    }
}
