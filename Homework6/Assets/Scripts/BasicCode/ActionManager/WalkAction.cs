using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkAction : SSAction {
	private float speed;
	private Vector3 target;
	private Animator animator;

	public static WalkAction GetWalkAction(Vector3 target, float speed, Animator animator) {
		WalkAction tmp = ScriptableObject.CreateInstance<WalkAction> ();
		tmp.speed = speed;
		tmp.animator = animator;
		tmp.target = target;
		return tmp;
	}

	public override void Start() {
		animator.SetFloat ("Speed", 0.6f);
	}

	public override void Update() {
		Quaternion rotation = Quaternion.LookRotation(target - transform.position);
		if (transform.rotation != rotation) 
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * speed * 5);
		this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed * Time.deltaTime);
		if (this.transform.position == target) {
			this.destroy = true;
			this.callback.actionDone(this);
		}
	}
}
