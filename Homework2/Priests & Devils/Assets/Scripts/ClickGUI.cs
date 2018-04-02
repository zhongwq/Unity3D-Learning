using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickGUI : MonoBehaviour {
    IUserAction action;
    ICharacterController character;

    public void setController(ICharacterController characterController)
    {
        character = characterController;
    }

    void Start()
    {
        action = GameDirector.getInstance().currentSceneController as IUserAction;
    }

    void OnMouseDown()
    {
        if (gameObject.name == "boat")
        {
            action.ToggleBoat();
        }
        else
        {
            action.ClickCharacter(character);
        }
    }
}
