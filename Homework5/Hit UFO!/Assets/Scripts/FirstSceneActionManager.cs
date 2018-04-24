using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSceneActionManager : SSActionManager {
	public void addActionToUFO(GameObject ufo, float speed)
	{
		int position = (Random.Range (1, 3) < 2) ? -1 : 1;
		ufo.transform.position = new Vector3 (-10 * position, Random.Range (4, 6), 5);
		SSAction flyAction = FlyAction.getAction (Random.Range ((int)speed, (int)(1.5 * speed)) * position, Random.Range ((int)speed, (int)(1.2 * speed)));
		addAction(ufo, flyAction, this);
	}

	public void addActionToUFOs(UFOController[] ufoArr, float speed)
	{
		foreach(var ufo in ufoArr)
		{
			addActionToUFO(ufo.GetObject(), speed);
		}
	}

	public override void actionDone(SSAction source) {
		(GameDirector.getInstance ().currentSceneController as FirstController).ufoFactory.recycle (source.gameObject.GetComponent<UFOCtrl>().ufoController);
	}
}
