using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType { ENTER, EXIT, DEAD }

public class Publisher {
	private delegate void ActionUpdate(ActionType type, int position, GameObject actor);
	private ActionUpdate updateList;

	private static Publisher _instance;
	public static Publisher getInstance() {
		if (_instance == null)
			_instance = new Publisher();
		return _instance;
	}

	public void notify(ActionType type, int position, GameObject actor)
	{
		if (updateList != null) 
			updateList(type, position, actor);
	}

	public void add(Observer observer)
	{
		updateList += observer.notified;
	}

	public void delete(Observer observer)
	{
		updateList -= observer.notified;
	}
}
