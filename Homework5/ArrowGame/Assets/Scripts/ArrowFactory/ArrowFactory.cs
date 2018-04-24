using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFactory : MonoBehaviour {
	Queue<ArrowController> waitingQueue;
	List<ArrowController> runningList;
	 
	private FirstController firstController;


	GameObject basic;

	private void Awake() {
		waitingQueue = new Queue<ArrowController> ();
		runningList = new List<ArrowController> ();

		basic = Instantiate (Resources.Load ("Prefabs/Arrow", typeof(GameObject)) as GameObject);
		basic.SetActive (false);
	}

	public ArrowController getArrow() {
		ArrowController arrow;
		if (waitingQueue.Count == 0) {
			GameObject newArrow = GameObject.Instantiate (basic);
			arrow = new ArrowController (newArrow);
		} else {
			arrow = waitingQueue.Dequeue ();
		}
		arrow.getObject ().transform.parent = ((FirstController)GameDirector.getInstance ().currentSceneController).Bow.transform;
		arrow.getObject().GetComponent<Rigidbody> ().velocity = Vector3.zero;
		arrow.getObject ().transform.localPosition = new Vector3 (0, 2, 1);
		arrow.getObject ().GetComponent<Rigidbody> ().isKinematic = true;
		arrow.getObject ().transform.GetComponent<Collider> ().enabled = true;
		arrow.appear ();
		runningList.Add (arrow);
		return arrow;
	}

	public void recycle(ArrowController arrow) {
		arrow.disappear ();
		arrow.getObject ().transform.position = new Vector3 (0, 0, -8);
		arrow.getObject ().transform.GetComponent<Collider> ().enabled = false;
		runningList.Remove (arrow);
		waitingQueue.Enqueue (arrow);
	}

	public void recycleAll() {
		while (runningList.Count != 0)
		{
			recycle(runningList[0]);
		}
	}
}
