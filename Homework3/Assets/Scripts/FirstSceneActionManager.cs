using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSceneActionManager : SSActionManager {
	public void toggleBoat(BoatController boat) {
		MoveAction action = MoveAction.getAction (boat.getTarget (), boat.speed);
		this.addAction (boat.getBoat (), action, this);
		boat.toggle ();
	}

	public void moveCharacter(ICharacterController character, Vector3 target) {
		Vector3 nowPos = character.getPos ();
		Vector3 tmpPos = nowPos;
		if (target.y > nowPos.y) {
			tmpPos.y = target.y;
		} else {
			tmpPos.x = target.x;
		}
		SSAction action1 = MoveAction.getAction(tmpPos, character.speed);
		SSAction action2 = MoveAction.getAction(target, character.speed);
		SSAction sequenceAction = CCSequenceAction.getAction(1, 0, new List<SSAction>{action1, action2});
		this.addAction(character.getInstance(), sequenceAction, this);
	}
}
