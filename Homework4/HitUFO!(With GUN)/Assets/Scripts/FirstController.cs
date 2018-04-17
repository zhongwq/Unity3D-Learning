using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstController : MonoBehaviour, ISceneController {
    GameDirector gameDirector;
    FirstSceneActionManager actionManager;
    UFOFactory ufoFactory;
	ExplosionFactory explosionFactory;
	FirstCharacterController firstCharacterController;

    ScoreRecorder scoreRecorder;
    TimerController timerController;
    DifficultyController difficulty;

	int gameStatus;
    float playTime = 0;
    bool gaming = false;
	int round = 1;
	GUIStyle headerStyle;
	Text roundText;

    void Awake()
    {
        gameDirector = GameDirector.getInstance();
        gameDirector.currentSceneController = this;
        actionManager = gameObject.AddComponent<FirstSceneActionManager>();
        ufoFactory = gameObject.AddComponent<UFOFactory>();
		explosionFactory = gameObject.AddComponent<ExplosionFactory> ();
        difficulty = DifficultyController.getInstance();
        timerController = gameObject.AddComponent<TimerController>();
        scoreRecorder = ScoreRecorder.getInstance();

        loadResources();
    }

    public void Start()
    {
		gameStatus = 0;
		difficulty.setDifficulty(0);
		round = 1;
		roundText = (GameObject.Instantiate(Resources.Load("Prefabs/RoundInfo")) as GameObject).transform.Find("Text").GetComponent<Text>();
		roundText.text = "" + round;
		headerStyle = new GUIStyle();
		headerStyle.fontSize = 40;
		headerStyle.alignment = TextAnchor.MiddleCenter;
		startRound ();
    }

	void OnGUI() 
	{
		if (gameStatus == 1) 
		{
			GUI.Label(new Rect(Screen.width / 2 - 45, Screen.height / 2 - 90, 100, 50), "You get "+ scoreRecorder.getScore()+" points in this game!", headerStyle);
			GUI.Label(new Rect(Screen.width / 2 - 45, Screen.height / 2, 100, 50), "Enter to play again!", headerStyle);
		}
	}

    void Update()
    {
		if (gameStatus == 0)
		{
 	        if (gaming == true)
	        {
	            playTime += Time.deltaTime;
	        }

	        if (gaming && checkNoUFO())
	        {
				if (round == 10) 
				{
					gameStatus = 1;
					gaming = false;
					return;
				}
	            gaming = false;
	            Invoke("startRound", 3);
	            timerController.setTime(3);
	            difficulty.setDifficultyByScore(scoreRecorder.getScore());
	        }
	        else if (gaming && timeout())
	        {
				if (round == 10) 
				{
					gameStatus = 1;
					gaming = false;
					return;
				}
	            gaming = false;
	            foreach (UFOController ufo in ufoFactory.GetRunningList())
	            {
	                actionManager.removeActionByObj(ufo.GetObject());
	            }
	            ufoFactory.recycleAll();
	            Invoke("startRound", 3);
	            timerController.setTime(3);
	            difficulty.setDifficultyByScore(scoreRecorder.getScore());
	        }
		}
		else
		{
			if (Input.GetKeyDown ("enter") || Input.GetKeyDown ("return"))
				replay ();
		}
    }

    public void loadResources()
    {
		firstCharacterController = new FirstCharacterController();
        Instantiate(Resources.Load("Prefabs/Land"));
    }

    void startRound()
    {
		roundText.text = "" + round++;
        gaming = true;
        playTime = 0;
        UFOController[] ufoCtrlArr = ufoFactory.initAll(difficulty.getUFOAttributes(), difficulty.UFONumber);
        foreach (var ufo in ufoCtrlArr)
        {
            ufo.appear();
        }
        actionManager.addActionToUFOs(ufoCtrlArr, difficulty.getUFOAttributes().speed);
    }

    bool timeout()
    {
        return (playTime > difficulty.currentSendInterval);
    }

    bool checkNoUFO()
    {
        return ufoFactory.GetRunningList().Count == 0;
    }

    public void shootUFO(UFOController ufo)
    {
        scoreRecorder.record(difficulty.getDifficulty());
        actionManager.removeActionByObj(ufo.GetObject());
		explosionFactory.explode (ufo.GetObject ().transform.position);
        ufoFactory.recycle(ufo);
    }

	public void shootGround(Vector3 pos) 
	{
		explosionFactory.explode (pos);
	}

	public void replay() 
	{
		gameStatus = 0;
		ufoFactory.recycleAll();
		round = 1;
		roundText.text = "" + round;
		Invoke("startRound", 3);
		timerController.setTime(3);
		difficulty.resetDifficulty ();
		scoreRecorder.reset ();
	}
}
