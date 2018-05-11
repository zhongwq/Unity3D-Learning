using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUserAction {
	void move (int dir);
	void changeDirection(float offsetX);
	void resetGame();
}
