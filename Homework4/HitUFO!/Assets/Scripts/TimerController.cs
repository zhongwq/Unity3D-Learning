using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerController: MonoBehaviour {
	private int time;
	private float tmpSecond;

	Text nowTime;

	public void setTime(int time)
	{
		this.time = time;
	}

	void Start()
	{
		time = 0;
		nowTime = (GameObject.Instantiate(Resources.Load("Prefabs/Timer")) as GameObject).transform.Find("Text").GetComponent<Text>();
		nowTime.text = "";
	}

	void Update()
	{
		if (time == 0)
		{
			nowTime.text = "";
			tmpSecond = 0;
		}
		else
		{
			tmpSecond += Time.deltaTime;
			if (tmpSecond >= 1)//每过1秒执行一次
			{
				time--;
				nowTime.text = "" + time;
				tmpSecond = 0;
			}
		}
	}
}
