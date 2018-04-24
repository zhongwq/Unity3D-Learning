using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController{
	GameObject arrow;
	ArrowControl arrowControl;
	public bool available = false;

	public ArrowController (GameObject gameObject) {
		this.arrow = gameObject;
		arrowControl = gameObject.AddComponent<ArrowControl> ();
		arrowControl.arrowController = this;
	}

	public void appear()
	{
		available = true;
		arrow.SetActive(true);
	}

	public void disappear()
	{
		arrow.SetActive(false);
	}
		
	public GameObject getObject() {
		return arrow;
	}
}
