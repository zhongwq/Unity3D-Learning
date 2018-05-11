using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolFactory : MonoBehaviour {
	List<GameObject> waitingList = new List<GameObject>();
	List<GameObject> runningList = new List<GameObject>();

	public GameObject getObject(Vector3 pos, Quaternion quaternion) {
		if (waitingList.Count == 0) {
			GameObject newGameObj = Instantiate (Resources.Load ("Prefabs/Patrolman"), pos, quaternion) as GameObject;
			runningList.Add (newGameObj);
		} else {
			runningList.Add (waitingList [0]);
			waitingList.RemoveAt (0);
			runningList [runningList.Count - 1].SetActive (true);
			runningList [runningList.Count - 1].transform.position = pos;
			runningList [runningList.Count - 1].transform.localRotation = quaternion;
			runningList [runningList.Count - 1].GetComponent<PatrolCtrl> ().initial ();
		}
		return runningList [runningList.Count - 1];
	}

	public void freeObj (GameObject obj) {
		obj.GetComponent<PatrolCtrl> ().removeAction ();
		runningList.Remove (obj);
		waitingList.Add (obj);
	}

	public void freeAllObj () {
		while (runningList.Count != 0) {
			freeObj (runningList [0]);
		}
	}
}
