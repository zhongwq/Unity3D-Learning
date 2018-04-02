using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ICharacterController
{

    readonly GameObject character;
    readonly Move moveAction;
    readonly int type;//0: Priest, 1: Demon
    readonly ClickGUI clickGUI;

    bool onBoat;
    LandController landController;


    public ICharacterController(int chracterType, string name)
    {
        if (chracterType == 0)
        {
            this.character = Object.Instantiate(Resources.Load("Prefabs/Priest", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            this.type = 0;
        }
        else
        {
            this.character = Object.Instantiate(Resources.Load("Prefabs/Demon", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            this.type = 1;
        }
        character.name = name;
        moveAction = character.AddComponent(typeof(Move)) as Move;
        clickGUI = character.AddComponent(typeof(ClickGUI)) as ClickGUI;
        clickGUI.setController(this);
    }

    public string getName()
    {
        return character.name;
    }

    public void setPosition(Vector3 pos)
    {
        character.transform.position = pos;
    }

    public void MoveTo(Vector3 destination)
    {
        moveAction.moveTo(destination);
    }

    public int getType()
    {
        return type;
    }

    public void getOnBoat(BoatController boatController, int pos)
    {
        landController = null;
        if (pos == 0)
        {
            character.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
        }
        else
        {
            character.transform.rotation = Quaternion.AngleAxis(270, Vector3.up);
        }
        character.transform.parent = boatController.getBoat().transform;
        onBoat = true;
    }

    public void getOnLand(LandController land)
    {
        landController = land;
        if (land.getType() == -1)
        {
            character.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
        }
        else
        {
            character.transform.rotation = Quaternion.AngleAxis(270, Vector3.up);
        }
        character.transform.parent = null;
        onBoat = false;
    }

    public bool isOnBoat()
    {
        return onBoat;
    }

    public LandController getLandController()
    {
        return landController;
    }

    public void reset()
    {
        moveAction.reset();
        landController = (GameDirector.getInstance().currentSceneController as FirstController).rightLand;
        getOnLand(landController);
        setPosition(landController.getEmptyPosition());
        landController.getOnLand(this);
    }
}
