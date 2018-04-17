using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionFactory : MonoBehaviour {
	Queue<GameObject> waitingQueue;
	List<GameObject> runningList;

	GameObject basic;

	private void Awake()
	{
		waitingQueue = new Queue<GameObject>();
		runningList = new List<GameObject>();

		basic = Instantiate(Resources.Load("Prefabs/Explosion", typeof(GameObject))) as GameObject;
		basic.SetActive(false);
	}

	public void explode(Vector3 pos)
	{
		GameObject explosion;
		if (waitingQueue.Count == 0) {
			explosion = GameObject.Instantiate (basic);
			explosion.AddComponent<SelfRecycle> ().factory = this;
		} 
		else 
		{
			explosion = waitingQueue.Dequeue ();
		}
		runningList.Add (explosion);
		explosion.GetComponent<SelfRecycle> ().startTimer (0.2f);

		explosion.SetActive (true);
		explosion.transform.position = pos;
	}

	public void recycle(GameObject explosion)
	{
		explosion.SetActive (false);
		waitingQueue.Enqueue(explosion);
		runningList.Remove(explosion);
	}
}
