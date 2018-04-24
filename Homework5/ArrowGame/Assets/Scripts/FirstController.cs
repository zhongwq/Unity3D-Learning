using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstController : MonoBehaviour, ISceneController {
	public PhysicsActionManager physicsActionManager;

	public GameDirector gameDirector;
	public GameObject Bow;
	public ArrowController arrow;
	public ScoreRecorder scoreRecorder;

	public ArrowFactory arrowFactory;
	public WindMake windMake;
	public showSubCamera showCamera;

	GUIStyle gameInfoStyle;
	public bool shootFinish = true; // whether arrow hit success

	void Awake() {
		arrowFactory = gameObject.AddComponent<ArrowFactory> ();
		windMake = gameObject.AddComponent<WindMake> ();
		physicsActionManager = gameObject.AddComponent<PhysicsActionManager> ();
		scoreRecorder = ScoreRecorder.getInstance ();
		showCamera = gameObject.AddComponent<showSubCamera> ();

		gameDirector = GameDirector.getInstance ();
		gameDirector.currentSceneController = this;
	}
		
	// Use this for initialization
	void Start () {
		loadResources ();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		gameInfoStyle = new GUIStyle ();
		gameInfoStyle.fontSize = 25;
	}

	void OnGUI() {
		GUI.Label (new Rect (20, 80, 200, 50), "Press space to restart!", gameInfoStyle);
	}
	

	// Update is called once per frame
	void Update () {
		float offsetX = Input.GetAxis ("Mouse X");
		float offsetY = Input.GetAxis ("Mouse Y");
		moveBow (offsetX, offsetY);

		if (Input.GetKeyDown("escape")) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		if (Input.GetButton ("Fire1")) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			shootArrow ();
		}

		if (Input.GetKeyDown ("space")) {
			reset ();
		}

		if (shootFinish && arrow == null) {
			arrow = arrowFactory.getArrow ();
		}
		
	}

	public void moveBow(float offsetX, float offsetY) {
		float posy = Mathf.Clamp(Bow.transform.position.y + offsetY, 1, 30);
		Bow.transform.position = new Vector3 (Bow.transform.position.x + offsetX, posy, Bow.transform.position.z);
	}

	public void shootArrow() {
		if (arrow != null) {
			GameObject arrowObj = arrow.getObject ();
			arrowObj.GetComponent<Rigidbody> ().isKinematic = false;
			Vector3 dir = arrowObj.transform.up * -1;
			ArrowFlyAction arrowAction = ArrowFlyAction.GetSSAction (1, dir * 30);
			ArrowFlyAction windAction = ArrowFlyAction.GetSSAction (0, windMake.getWind());
			physicsActionManager.addAction (arrowObj, arrowAction, physicsActionManager);
			physicsActionManager.addAction (arrowObj, windAction, physicsActionManager);
			arrowObj.transform.parent = null;
			arrow = null;
			shootFinish = false;
		}
	}

	public void loadResources() {
		Instantiate(Resources.Load("Prefabs/Land"));
		Instantiate(Resources.Load("Prefabs/Target"));
		Bow = Instantiate(Resources.Load("Prefabs/Bow")) as GameObject;  
		arrow = arrowFactory.getArrow ();
	}

	void reset() {
		arrow = null;
		shootFinish = true;
		scoreRecorder.reset ();
		arrowFactory.recycleAll ();
	}
}
