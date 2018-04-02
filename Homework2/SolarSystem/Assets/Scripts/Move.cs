using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
	public float speed;

	private Vector3 normalPlane;
	
	// Use this for initialization
	void Start () {
		normalPlane.Set (0, Random.Range (0, 10), Random.Range (0, 2));
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.RotateAround (this.transform.parent.position, normalPlane, speed);
	}
}
