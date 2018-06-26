using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour {
	public GameObject bulletPrefab;

	private Tank player;
	private MoveCtrl move;
	private Button btn;

	[SyncVar]
	public int identity;

	public override void OnStartLocalPlayer () {
		MeshRenderer[] tmp = GetComponentsInChildren<MeshRenderer> (true);
		for (int i = 0; i < tmp.Length; i++) {
			tmp [i].material.color = Color.red;
		}
	}

	// Use this for initialization
	void Start () {
		player = GetComponent<Tank> ();
		identity = GetInstanceID ();
		move = (MoveCtrl)FindObjectOfType (typeof(MoveCtrl));
		btn = GameObject.Find ("Attack").GetComponent<Button>();

		btn.GetComponent<Button>().onClick.AddListener (delegate() {
			this.attackHandler();
		});
	}
	
	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer)
			return;
		Camera.main.transform.position = new Vector3(player.transform.position.x, 30, player.transform.position.z);     
		if (player.getHp() <= 0)
			return;
		var x = Input.GetAxis("Horizontal") * 0.1f;
		var z = Input.GetAxis ("Vertical") * 0.1f;
		if (x != 0 || z != 0) {
			// 键盘控制
			transform.Translate(0, 0, z * 80.0f * Time.deltaTime);
			transform.Rotate(0, x * 900.0f * Time.deltaTime, 0);
		} else {
			// 摇杆控制
			float ver = move.Vertical + z;
			float hor = move.Horizontal;

			Vector3 direction = new Vector3 (hor, 0, ver);  

			if (direction != Vector3.zero) {  
				//控制转向  
				transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (direction), Time.deltaTime * 20);  
				//向前移动 
				transform.Translate (Vector3.forward * Time.deltaTime * 5);
			}
		}

		if (Input.GetKeyDown (KeyCode.Space))
			CmdAttack (identity);
	}

	void attackHandler() {
		if (!isLocalPlayer)
			return;
		if (player.getHp() <= 0)
			return;
		CmdAttack (identity);
	}

	[Command]
	void CmdAttack(int id) {
		var bullet = (GameObject)Instantiate (bulletPrefab, new Vector3 (transform.position.x, 1.5f, transform.position.z) +
			transform.forward * 1f, Quaternion.identity);
		bullet.GetComponent<Bullet> ().setFromID (id);
		bullet.transform.forward = transform.forward;
		bullet.GetComponent<Rigidbody>().velocity = transform.forward*20;

		NetworkServer.Spawn (bullet);
	}

}
