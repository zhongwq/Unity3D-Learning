using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRecycle : MonoBehaviour {
	public ExplosionFactory factory;
	// Use this for initialization
	public void startTimer(float time) {
		Invoke("selfRecycle", time);
	}
	
	// Update is called once per frame
	private void selfRecycle () {
		factory.recycle (gameObject);
	}
}
