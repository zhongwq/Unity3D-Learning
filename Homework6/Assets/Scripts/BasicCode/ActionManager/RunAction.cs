using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAction : SSAction {
	private float speed;
	private Transform target;
	private Animator animator;

	public static RunAction GetRunAction(Transform target, float speed, Animator animator) {
		RunAction currentAction = ScriptableObject.CreateInstance<RunAction>();
		currentAction.speed = speed;
		currentAction.target = target;
		currentAction.animator = animator;
		return currentAction;
	}

	public override void Start() {
		animator.SetFloat ("Speed", 1.1f);
	}

	public override void Update() {
		Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
		if (transform.rotation != rotation) 
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * speed * 5);

		this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, speed * Time.deltaTime);
		if (Vector3.Distance(this.transform.position, target.position) < 0.5) {
			this.destroy = true;
			this.callback.actionDone(this);
		}
	}
}
