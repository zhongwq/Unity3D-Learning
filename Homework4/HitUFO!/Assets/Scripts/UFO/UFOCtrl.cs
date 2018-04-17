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
} //用于获取UFO的Controller
