using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstSceneController : MonoBehaviour, ISceneController {
	public MoveCtrl move;
	public GameObject tank;
	public Button btn;

	public bool gameOver = false;
	private AllFactory allFactory;
	GUIStyle headerStyle;

	private int Count = 0;
	private int Score = 0;

	// Use this for initialization
	void Awake () {
		GameDirector director = GameDirector.getInstance ();
		director.currentSceneController = this;
		allFactory = Singleton<AllFactory>.Instance;
		tank = allFactory.getPlayer ();
		btn.GetComponent<Button>().onClick.AddListener (delegate() {
			this.attackHandler();
		});
	}

	void Start () {
		Count = 0;
		headerStyle = new GUIStyle();
		headerStyle.fontSize = 100;
		headerStyle.alignment = TextAnchor.MiddleCenter;
	}

	public void loadResources() {
	}

	public void GetScore() {
		++Score;
	}

	void GameOver () {
		this.gameOver = true;
	}

	public Vector3 getPlayerPos() {
		return tank.transform.position;
	}

	// Update is called once per frame
	void Update () {
		if (gameOver)
			return;
		if (Count++ % 120 == 0) {
			allFactory.getTank ();
		}
		float ver = -move.Horizontal;  
		float hor = move.Vertical;

		Vector3 direction = new Vector3 (hor, 0, ver);  


		if (direction != Vector3.zero) {  
			//控制转向  
			tank.transform.rotation = Quaternion.Lerp (tank.transform.rotation, Quaternion.LookRotation (direction), Time.deltaTime * 20);  
			//向前移动  
			tank.transform.Translate (Vector3.forward * Time.deltaTime * 5);
		}
	}

	void OnGUI() {
		GUI.Label(new Rect(Screen.width - 500, 80, 400, 110), "Score: " + Score, headerStyle);
		if (gameOver) {
			GUI.Label (new Rect (Screen.width / 2 - 200, Screen.height / 2 - 100, 400, 110), "Game Over!", headerStyle);
			if (GUI.Button (new Rect (Screen.width / 2 - 150, Screen.height / 2 + 30, 300, 120), "Replay")) {
				allFactory.reset ();
				Score = 0;
				Count = 0;
				gameOver = false;
			}
		}
	}

	void attackHandler() {
		if (gameOver)
			return;
		GameObject bullet = allFactory.getBullet(tankType.Player);//获取子弹，设置其发出坦克的类型
		bullet.transform.position = new Vector3(tank.transform.position.x, 1.5f, tank.transform.position.z) +
		tank.transform.forward * 1.5f;
		bullet.transform.forward = tank.transform.forward;
		Rigidbody rb = bullet.GetComponent<Rigidbody>();
		rb.AddForce(bullet.transform.forward * 20, ForceMode.Impulse);//给子弹一个向前的力，实现子弹向前移动
	}
}
