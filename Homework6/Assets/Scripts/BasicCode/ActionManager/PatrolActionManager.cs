using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolActionManager : SSActionManager {
	public IdleAction toIdle(GameObject obj, Animator animator, ActionCallback callback) {
		IdleAction tmp = IdleAction.GetIdleAction (Random.Range (1, 2), animator);
		this.addAction (obj, tmp, callback);
		return tmp;
	}

	public WalkAction toLeft(GameObject obj, Animator animator, ActionCallback callback) {
		Vector3 target = Vector3.left * Random.Range(2, 4) + obj.transform.position;
		WalkAction tmp = WalkAction.GetWalkAction(target, 1.0f, animator);
		this.addAction (obj, tmp, callback);
		return tmp;
	}

	public WalkAction toRight(GameObject obj, Animator animator, ActionCallback callback) {
		Vector3 target = Vector3.right * Random.Range(2, 4) + obj.transform.position;
		WalkAction tmp = WalkAction.GetWalkAction(target, 1.0f, animator);
		this.addAction (obj, tmp, callback);
		return tmp;
	}

	public WalkAction toForward(GameObject obj, Animator animator, ActionCallback callback) {
		Vector3 target = Vector3.forward * Random.Range(2, 4) + obj.transform.position;
		WalkAction tmp = WalkAction.GetWalkAction(target, 1.0f, animator);
		this.addAction (obj, tmp, callback);
		return tmp;
	}

	public WalkAction toBack(GameObject obj, Animator animator, ActionCallback callback) {
		Vector3 target = Vector3.back * Random.Range(2, 4) + obj.transform.position;
		WalkAction tmp = WalkAction.GetWalkAction(target, 1.0f, animator);
		this.addAction (obj, tmp, callback);
		return tmp;
	}

	public RunAction getTarget(GameObject player, GameObject obj, Animator animator, ActionCallback callback) {
		RunAction tmp = RunAction.GetRunAction (player.transform, 2.0f, animator);
		this.addAction (obj, tmp, callback);
		return tmp;
	}



	public IdleAction Stop(GameObject obj, Animator animator, ActionCallback callback) {
		IdleAction tmp = IdleAction.GetIdleAction (-1f, animator);
		this.addAction (obj, tmp, callback);
		return tmp;
	}
}
