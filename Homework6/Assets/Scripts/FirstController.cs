using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstController : MonoBehaviour, ISceneController, Observer {
	private PatrolFactory factory;
	public ScoreRecorder scoreRecorder;

	private static float[] posx = {2, 2, -2, -2};
	private static float[] posz = {2, -2, -2, 2};
	private int gameStatus;
	private GUIStyle gameInfoStyle;
	private GUIStyle buttonStyle;

	private GameObject man;

	// Use this for initialization
	void Start () {
		factory = Singleton<PatrolFactory>.Instance;
		scoreRecorder = ScoreRecorder.getInstance ();
		Publisher publish = Publisher.getInstance ();
		publish.add (this);
		gameStatus = 0;
		gameInfoStyle = new GUIStyle ();
		gameInfoStyle.fontSize = 25;
		buttonStyle = new GUIStyle("button");
		buttonStyle.fontSize = 30;
		loadResources ();
	}

	void OnGUI () {
		if (gameStatus != 0) {
			GUI.Label(new Rect(Screen.width / 2 - 45, Screen.height / 2 - 90, 100, 50), "Gameover!", gameInfoStyle);
			if (GUI.Button(new Rect(Screen.width / 2 - 65, Screen.height / 2, 140, 70), "Restart", buttonStyle)){
				reset ();
				gameStatus = 0;
			}
		}
	}

	public void loadResources() {
		man = Instantiate (Resources.Load ("Prefabs/Man"), new Vector3 (-6, 0, 10), Quaternion.Euler (new Vector3 (0, 180, 0))) as GameObject;
		for (int i = 0; i < 4; i++) {
			GameObject patrol = factory.getObject (new Vector3 (posx [i], 0, posz [i]), Quaternion.Euler (new Vector3 (0, 180, 0)));
			patrol.name = "Patrol" + (i + 1);
		}
	}

	private void reset() {
		man.transform.position = new Vector3 (-6, 0, 10);
		man.transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
		for (int i = 0; i < 4; i++) {
			GameObject patrol = factory.getObject (new Vector3 (posx [i], 0, posz [i]), Quaternion.Euler (new Vector3 (0, 180, 0)));
			patrol.name = "Patrol" + (i + 1);
		}
		scoreRecorder.reset ();
		man.GetComponent<Animator> ().SetBool ("live", true);
	}

	public void notified (ActionType type, int position, GameObject actor) {
		if (type == ActionType.DEAD) {
			gameStatus = 1;
			factory.freeAllObj ();
		}
	}
}
