using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Observer {
	void notified (ActionType type, int position, GameObject actor);
}
