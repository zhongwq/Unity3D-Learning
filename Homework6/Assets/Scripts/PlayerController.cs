using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Camera subCamera;
	private Animator animator;
	private AnimatorStateInfo stateInfo;

	private Vector3 velocity;
	private int angle;

	private float rotateSpeed = 15f;
	private float rotateSpeed_Sub = 60f;
	private float runSpeed = 3f;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
		angle = 1;
	}

	void OnTriggerEnter(Collider other) {
		// 进入被监控的Area则发布信息
		if (other.gameObject.CompareTag ("Area")) {
			Publisher publish = Publisher.getInstance ();
			int areaIndex = other.gameObject.name[other.gameObject.name.Length-1]-'0';
			publish.notify (ActionType.ENTER, areaIndex, this.gameObject);
		}
	}

	void OnTriggerExit(Collider other) {
		// 逃离时发布信息
		if (other.gameObject.CompareTag ("Area") && animator.GetBool ("live")) {
			Publisher publish = Publisher.getInstance ();
			publish.notify (ActionType.EXIT, -1, this.gameObject);
		}
	}

	void OnCollisionEnter(Collision collision) {
		// 与Patrol碰撞后发布信息
		if (collision.gameObject.CompareTag ("Patrol") && animator.GetBool ("live")) {
			animator.SetBool ("live", false);
			Publisher publish = Publisher.getInstance ();
			publish.notify (ActionType.DEAD, 0, null);
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		// 当用户按下空格，改变游戏视角，通过改变子摄像头的深度实现
		if (Input.GetKeyDown ("space")) {
			if (angle == 1) {
				subCamera.depth = -1;
				angle = 3;
			} else {
				angle = 1;
				subCamera.depth = -2;
			}
		}
		if (!animator.GetBool ("live"))
			return;
		float x = Input.GetAxis ("Horizontal");
		float z = Input.GetAxis ("Vertical");

		if (x == 0 && z == 0) {
			animator.SetBool ("Run", false);
			return;
		} else {
			if (angle == 1) {
				// 俯视视角的交互方式（WSAD分别为上下左右）
				animator.SetBool ("Run", true);
				velocity = new Vector3 (x, 0, z);
				if (x != 0 || z != 0) {
					Quaternion rotation = Quaternion.LookRotation (velocity);
					if (transform.rotation != rotation)
						transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.fixedDeltaTime * rotateSpeed);
				}

				this.transform.position += velocity * Time.fixedDeltaTime * runSpeed;
			} else {
				// 第三人称视角的交互方式(WS前进、后退, AD左右旋转方向)
				if (z != 0)
					animator.SetBool ("Run", true);
				transform.Translate(0, 0, z * runSpeed * Time.fixedDeltaTime);
				transform.Rotate(0, x * rotateSpeed_Sub * Time.fixedDeltaTime, 0);
			}
		}
		// 避免碰撞带来的影响
		if (transform.localEulerAngles.x != 0 || transform.localEulerAngles.z != 0) {
			transform.localEulerAngles = new Vector3 (0, transform.localEulerAngles.y, 0);
		} 
		if (transform.position.y != 0) {
			transform.position = new Vector3 (transform.position.x, 0, transform.position.z);
		}
	}
}
