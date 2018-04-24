using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManagerAdapter : ActionManagerTarget 
{
	FirstSceneActionManager firstSceneActionManager;
	PhysicsActionManager physicsActionManager;

	int mode = 0; // 0: firstSceneActionManager, 1: physicsActionManager

	public int getMode() 
	{
		return mode;
	}

	public void switchActionManager() 
	{
		mode = 1 - mode;
	}

	public ActionManagerAdapter(GameObject main) 
	{
		firstSceneActionManager = main.AddComponent < FirstSceneActionManager> ();
		physicsActionManager = main.AddComponent<PhysicsActionManager> ();
		mode = 0;
	}

	public void addAction(GameObject ufo, float speed) 
	{
		if (mode == 0)
		{
			firstSceneActionManager.addActionToUFO (ufo, speed);
		}
		else
		{
			physicsActionManager.addForceToObj (ufo, speed);
		}
	}

	public void removeActionOf(GameObject ufo) 
	{
		if (mode == 0) 
		{
			firstSceneActionManager.removeActionByObj (ufo);
		}
		else
		{
			physicsActionManager.removeForceOfObj (ufo);
		}
	}
}