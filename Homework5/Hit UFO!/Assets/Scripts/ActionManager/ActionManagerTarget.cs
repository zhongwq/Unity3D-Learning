using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ActionManagerTarget 
{
	int getMode();

	void switchActionManager();

	void addAction (GameObject ufo, float speed);

	void removeActionOf (GameObject ufo);
}
