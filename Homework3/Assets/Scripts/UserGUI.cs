using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour {
    public int status = 0;

    private IUserAction action;

    GUIStyle headerStyle;
    GUIStyle buttonStyle;

	// Use this for initialization
	void Start () {
        action = GameDirector.getInstance().currentSceneController as IUserAction;

        headerStyle = new GUIStyle();
        headerStyle.fontSize = 40;
        headerStyle.alignment = TextAnchor.MiddleCenter;

        buttonStyle = new GUIStyle("button");
        buttonStyle.fontSize = 30;
    }
	
	// Update is called once per frame
	void OnGUI () {
		if (GUI.Button (new Rect (30, 30, 140, 50), "Change Angle"))
			action.ChangeCamera ();
        GUI.Label(new Rect(Screen.width / 2 - 100, 10, 200, 50), "Priests & Demons", headerStyle);
        if (status == 1)
        {
            GUI.Label(new Rect(Screen.width / 2 - 45, Screen.height / 2 - 90, 100, 50), "Gameover!", headerStyle);
            if (GUI.Button(new Rect(Screen.width / 2 - 65, Screen.height / 2, 140, 70), "Restart", buttonStyle))
            {
                status = 0;
                action.restart();
            }
        }
        else if (status == 2)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 90, 100, 50), "Win!", headerStyle);
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 140, 70), "Restart", buttonStyle))
            {
                status = 0;
                action.restart();
            }
        }
    }
}
