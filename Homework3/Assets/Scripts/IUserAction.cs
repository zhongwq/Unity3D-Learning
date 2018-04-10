using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUserAction {
    void restart();
    void ToggleBoat();
    void ClickCharacter(ICharacterController chracter);
	void ChangeCamera();
}
