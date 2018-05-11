using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAction : SSAction {
	private float time;
	private Animator animator;

	public static IdleAction GetIdleAction(float time, Animator animator) {
		IdleAction action = ScriptableObject.CreateInstance<IdleAction> ();
		action.time = time;
		action.animator = animator;
		return action;
	}

	public override void Start() {
		animator.SetFloat ("Speed", 0f);
	}

	public override void Update() {
		if (time == -1)
			return;
		time -= Time.deltaTime;

		if (time < 0) {
			this.destroy = true;
			this.callback.actionDone (this);
		}
	}
}
