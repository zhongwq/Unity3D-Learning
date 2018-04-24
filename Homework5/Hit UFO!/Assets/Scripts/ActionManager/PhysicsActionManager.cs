using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsActionManager : MonoBehaviour {
	public void addForceToObj (GameObject ufo, float speed) 
	{
		int position = (Random.Range (1, 3) < 2) ? -1 : 1;
		ufo.transform.position = new Vector3 (-10 * position, Random.Range (4, 6), 5);
		Vector3 speedVector = new Vector3 (Random.Range ((int)speed, (int)(1.5 * speed)) * position, Random.Range ((int)speed, (int)(1.2 * speed)), 0);
		ufo.GetComponent<Rigidbody> ().useGravity = true;
		ufo.GetComponent<Rigidbody> ().velocity = speedVector;
	}

	public void removeForceOfObj (GameObject ufo)
	{
		ufo.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		ufo.GetComponent<Rigidbody> ().useGravity = false;
	}

}
