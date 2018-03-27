using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chess : MonoBehaviour {
	private int[,] gameBoxStatus = new int[3, 3]; //用于保存棋盘数据
	private int gameStatus = 0; //记录游戏状态
	private int term = 1; //记录当前轮到哪个玩家
	private int oscore = 0;
	private int xscore = 0;
	private int xpos = 420;
	private int ypos = 150;
	private int Winner = 0;

	// Use this for in itialization
	void Start () {
		initialAll ();
	}

	void initialAll () {
		oscore = 0;
		xscore = 0;
		initialGameInfo ();
	}

	void initialGameInfo () {
		gameStatus = 0;
		Winner = 0;
		term = 1;
		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++)
				gameBoxStatus[i,j] = 0;
	}

	void OnGUI () {
		GUIStyle headerStyle = new GUIStyle ();
		headerStyle.normal.textColor = new Color (255, 255, 255);
		headerStyle.fontSize = 40;
		GUIStyle gameInfoStyle = new GUIStyle ();
		gameInfoStyle.normal.textColor = new Color (255, 255, 255);
		gameInfoStyle.fontSize = 25;
		GUIStyle gameBox = new GUIStyle ();
		gameBox.fontSize = 40;
		GUI.Label(new Rect(300,60,200,100), "Simple Chess Game", headerStyle);
		GUI.Label (new Rect (xpos-280, ypos, 100, 50), "O Score", gameInfoStyle);
		GUI.Box (new Rect (xpos - 250, ypos + 60, 100, 50), oscore.ToString(), gameBox);
		GUI.Label (new Rect (xpos + 330, ypos, 100, 50), "X Score", gameInfoStyle);
		GUI.Box (new Rect (xpos + 360, ypos + 60, 100, 50), xscore.ToString(), gameBox);
		if (gameStatus == 0) {
			GUI.Label (new Rect (xpos+20, ypos, 100, 50), "Who first?", gameInfoStyle);
			if (GUI.Button (new Rect (xpos+20, ypos + 60, 100, 50), "O")) {
				term = 1;
				gameStatus = 1;
			}
			if (GUI.Button (new Rect (xpos+20, ypos + 120, 100, 50), "X")) {
				term = 2;
				gameStatus = 1;
			}
		} else if (gameStatus == 1) {
			if (GUI.Button (new Rect (xpos+25, ypos + 180, 100, 50), "Reset"))
				initialGameInfo ();
			int result = Winner;
			if (Winner == 0) {
				result = checkWin ();
				if (result == 1) {
					Winner = 1;
					oscore += 1;
					gameStatus = 2;
				} else if (result == 2) {
					Winner = 2;
					xscore += 1;
					gameStatus = 2;
				} else if (result == 3) {
					Winner = 3;
					gameStatus = 2;
				}
			}
			for (int i = 0; i < 3; i++)
				for (int j = 0; j < 3; j++) {
					if (gameBoxStatus [i, j] == 1)
						GUI.Button (new Rect (xpos + i * 50, ypos + j * 50, 50, 50), "O");
					if (gameBoxStatus [i, j] == 2)
						GUI.Button (new Rect (xpos + i * 50, ypos + j * 50, 50, 50), "X");
					if (GUI.Button (new Rect (xpos + i * 50, ypos + j * 50, 50, 50), "")) {  
						if (result == 0) {  
							gameBoxStatus [i, j] = term;  
							term = (term == 2) ? 1 : 2;
						}  
					}  
				}
		} else {
			if (Winner == 3) {
				GUI.Label (new Rect (xpos, ypos, 100, 50), "No one wins!", gameInfoStyle);
			} else if (Winner == 1) {
				GUI.Label (new Rect (xpos+30, ypos, 100, 50), "O wins!", gameInfoStyle);
			} else if (Winner == 2) {
				GUI.Label (new Rect (xpos+30, ypos, 100, 50), "X wins!", gameInfoStyle);
			}
			if (GUI.Button (new Rect (xpos+10, ypos+60, 120, 50), "Play again!"))
				initialGameInfo ();
			if (GUI.Button (new Rect (xpos+10, ypos+120, 120, 50), "Reset"))
				initialAll ();
		}
	}

	int checkWin () {
		for (int i = 0; i < 3; i++) {
			if (gameBoxStatus [0, i] != 0 && gameBoxStatus [0, i] == gameBoxStatus [1, i] && gameBoxStatus [1, i] == gameBoxStatus [2, i])
				return gameBoxStatus [0, i];
			if (gameBoxStatus [i, 0] != 0 && gameBoxStatus [i, 0] == gameBoxStatus [i, 1] && gameBoxStatus [i, 1] == gameBoxStatus [i, 2])
				return gameBoxStatus [i, 0];
		}
		if ((gameBoxStatus [1, 1] != 0 && gameBoxStatus [0, 0] == gameBoxStatus [1, 1] && gameBoxStatus [1, 1] == gameBoxStatus [2, 2]) ||
			(gameBoxStatus [0, 2] == gameBoxStatus [1, 1] && gameBoxStatus [1, 1] == gameBoxStatus [2, 0]))
			return gameBoxStatus [1, 1];
		bool flag = true;
		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++) {
				if (gameBoxStatus[i,j] == 0) {
					flag = false;
				}
			}
		return (flag) ? 3 : 0;
	}
}
