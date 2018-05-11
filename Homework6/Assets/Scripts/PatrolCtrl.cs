using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolCtrl : MonoBehaviour, ActionCallback, Observer {
	public enum ActionStatus: int { IDLE, TOLEFT, TOFORWARD, TORIGHT, TOBACK }

	private Animator animator;
	private SSAction currentAction;
	private ActionStatus status;
	private PatrolActionManager patrolActionManager;

	// Use this for initialization
	void Start () {
		animator = this.gameObject.GetComponent<Animator> ();
		patrolActionManager = Singleton<PatrolActionManager>.Instance;
		Publisher publisher = Publisher.getInstance ();
		publisher.add (this);

		status = ActionStatus.IDLE;

		currentAction = patrolActionManager.toIdle (gameObject, animator, this);
	}

	void FixedUpdate () {
		if (transform.localEulerAngles.x != 0 || transform.localEulerAngles.z != 0) {
			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
		}
		if (transform.position.y != 0) {
			transform.position = new Vector3(transform.position.x, 0, transform.position.z);
		}
		gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
	}

	public void initial () {
		status = ActionStatus.IDLE;
		currentAction = patrolActionManager.toIdle (gameObject, animator, this);
	}

	public void removeAction () {
		if (currentAction)
			currentAction.destroy = true;
	}

	// When action done do next action
	public void actionDone (SSAction source) {
		status = status == ActionStatus.TOBACK ? ActionStatus.TOLEFT: (ActionStatus)((int)status + 1);
		switch (status) {
			case ActionStatus.TOLEFT:
				currentAction = patrolActionManager.toLeft (gameObject, animator, this);
				break;
			case ActionStatus.TORIGHT:
				currentAction = patrolActionManager.toRight (gameObject, animator, this);
				break;
			case ActionStatus.TOFORWARD:
				currentAction = patrolActionManager.toForward (gameObject, animator, this);
				break;
			case ActionStatus.TOBACK:
				currentAction = patrolActionManager.toBack (gameObject, animator, this);
				break;
		}
	}

	private void turn () {
		currentAction.destroy = true;
		switch (status) {
			case ActionStatus.TOLEFT:
				status = ActionStatus.TORIGHT;
				currentAction = patrolActionManager.toRight (gameObject, animator, this);
				break;
			case ActionStatus.TORIGHT:
				status = ActionStatus.TOLEFT;
				currentAction = patrolActionManager.toLeft (gameObject, animator, this);;
				break;
			case ActionStatus.TOFORWARD:
				status = ActionStatus.TOBACK;
				currentAction = patrolActionManager.toBack (gameObject, animator, this);
				break;
			case ActionStatus.TOBACK:
				status = ActionStatus.TOFORWARD;
				currentAction = patrolActionManager.toForward (gameObject, animator, this);
				break;
		}
	}

	private void OnCollisionEnter (Collision collision) {
		if (collision.gameObject.CompareTag ("Wall"))
			turn ();
	}

	private void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag ("Door"))
			turn ();
	}

	public void notified (ActionType type, int position, GameObject actor) {
		if (type == ActionType.ENTER) {
			if (position == this.gameObject.name [this.gameObject.name.Length - 1] - '0') {
				currentAction.destroy = true;
				currentAction = patrolActionManager.getTarget (actor, gameObject, animator, this);
			}
		} else if (type == ActionType.DEAD) {
			currentAction = patrolActionManager.Stop (gameObject, animator, this);
		}
	}
}
