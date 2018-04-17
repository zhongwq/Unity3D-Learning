using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOController {
	public UFOData objData;
	GameObject gameObject;
	UFOCtrl ufoCtrl;

	public UFOController(GameObject gameObject)
	{
		this.gameObject = gameObject;
		ufoCtrl = gameObject.AddComponent<UFOCtrl>();
		ufoCtrl.ufoController = this;
	}

	public void appear()
	{
		gameObject.SetActive(true);
	}

	public void disappear()
	{
		gameObject.SetActive(false);
	}

	public GameObject GetObject()
	{
		return gameObject;
	}

	public void setAttribute(UFOData data)
	{
		objData = data;
		gameObject.transform.localScale.Set (data.size, data.size, data.size);
		foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>())
		{
			renderer.material.color = data.color;
		}
	}
}
