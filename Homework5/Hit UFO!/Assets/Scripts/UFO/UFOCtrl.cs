using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOCtrl : MonoBehaviour {
	public UFOController ufoController;

	void Start()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject obj = transform.GetChild(i).gameObject;
			obj.AddComponent<UFOCtrl>().ufoController = ufoController;
		}
	}

	public void OnCollisionEnter (Collision collision) 
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer ("UFO")) 
		{
			UFOCtrl ufo1 = collision.gameObject.GetComponent<UFOCtrl> ();
			((FirstController)GameDirector.getInstance ().currentSceneController).ufoCrash (ufo1.ufoController, this.ufoController);
		}
		else if (collision.gameObject.layer == LayerMask.NameToLayer ("Land")) 
		{
			ufoController.FinishAction ();
		}
	}
} //用于获取UFO的Controller
