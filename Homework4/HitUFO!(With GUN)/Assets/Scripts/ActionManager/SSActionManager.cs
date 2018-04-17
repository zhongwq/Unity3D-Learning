using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSActionManager : MonoBehaviour, ActionCallback
{
	private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
	private List<SSAction> waitingToAdd = new List<SSAction>();
	private List<int> waitingToDelete = new List<int>();

	protected void Update()
	{
		foreach (SSAction ac in waitingToAdd)
		{
			actions[ac.GetInstanceID()] = ac;
		}
		waitingToAdd.Clear();

		foreach (KeyValuePair<int, SSAction> kv in actions)
		{
			SSAction ac = kv.Value;
			if (ac.destroy)
			{
				waitingToDelete.Add(ac.GetInstanceID());
			}
			else if (ac.enable)
			{
				ac.Update();
			}
		}

		foreach (int key in waitingToDelete)
		{
			SSAction ac = actions[key];
			actions.Remove(key);
			DestroyObject(ac);
		}
		waitingToDelete.Clear();
	}

	public void addAction(GameObject gameObject, SSAction action, ActionCallback callback)
	{
		action.gameObject = gameObject;
		action.transform = gameObject.transform;
		action.callback = callback;
		waitingToAdd.Add(action);
		action.Start();
	}

	public void removeActionByObj(GameObject gameObject)
	{
		foreach (KeyValuePair<int, SSAction> kv in actions)
		{
			if (kv.Value.gameObject == gameObject)
			{
				kv.Value.destroy = true;
				kv.Value.enable = false;
			}
		}
	}

	public void actionDone(SSAction source)
	{

	}
}
