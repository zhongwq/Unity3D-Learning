using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController {
    readonly GameObject boat;
    readonly Move moveAction;
    readonly Vector3 right = new Vector3(3.5f, 0, 0);
    readonly Vector3 left = new Vector3(-3.5f, 0, 0);
    readonly Vector3[] right_positions;
    readonly Vector3[] left_positions;

    int status;// 0: left, 1: right
    ICharacterController[] characterOnBoat = new ICharacterController[2];

    public BoatController()
    {
        status = 1;
        right_positions = new Vector3[] { new Vector3(2.5F, 0.2F, 0), new Vector3(4.5F, 0.2F, 0) };
        left_positions = new Vector3[] { new Vector3(-4.5F, 0.2F, 0), new Vector3(-2.5F, 0.2F, 0) };
        boat = Object.Instantiate(Resources.Load("Prefabs/Boat", typeof(GameObject)), right, Quaternion.identity, null) as GameObject;
        boat.name = "boat";
        moveAction = boat.AddComponent(typeof(Move)) as Move;
        boat.AddComponent(typeof(ClickGUI));
    }

    public void Move()
    {
		if (moveAction.getStatus () != 0)
			return;
        if (status == 1)
        {
            moveAction.moveTo(left);
        }
        else
        {
            moveAction.moveTo(right);
        }
        status = 1 - status;
    }

    public int getEmptyIndex()
    {
        for (int i = 0; i < characterOnBoat.Length; i++)
        {
            if (characterOnBoat[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    public Vector3 getEmptyPosition()
    {
        int index = getEmptyIndex();
        if (status == 0)
        {
            return left_positions[index];
        }
        else
        {
            return right_positions[index];
        }
    }

    public bool isEmpty()
    {
        for (int i = 0; i < characterOnBoat.Length; i++)
        {
            if (characterOnBoat[i] != null)
            {
                return false;
            }
        }
        return true;
    }

    public void getOnBoat(ICharacterController character)
    {
        characterOnBoat[getEmptyIndex()] = character;
    }

    public ICharacterController getOffBoat(string name)
    {
        for (int i = 0; i < characterOnBoat.Length; i++)
        {
            if (characterOnBoat[i] != null && characterOnBoat[i].getName() == name)
            {
                ICharacterController character = characterOnBoat[i];
                characterOnBoat[i] = null;
                return character;
            }
        }
        return null;
    }

    public GameObject getBoat()
    {
        return boat;
    }

    public int getBoatPos()
    {
        return status;
    }

    public int[] getBoatStatus()
    {
        int[] boatStatus = { 0, 0 };
        for (int i = 0; i < characterOnBoat.Length; i++)
        {
            if (characterOnBoat[i] == null)
                continue;
            boatStatus[characterOnBoat[i].getType()]++;
        }
        return boatStatus;
    }// 0: Priests, 1: Demon

    public bool available()
    {
        return (moveAction.getStatus() == 0);
    }

    public void reset()
    {
        moveAction.reset();
        if (status == 0)
        {
            Move();
        }
        characterOnBoat = new ICharacterController[2];
    }
}
