using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandController {
    readonly GameObject land;
    readonly Vector3 leftPos = new Vector3(-8.5f, 0f, 0f);
    readonly Vector3 rightPos = new Vector3(8.5f, 0f, 0f);
    readonly Vector3[] landPositions;
    readonly int type;// 1:right, -1: left

    ICharacterController[] characterOnLand;

    public LandController(int _type)
    {
        landPositions = new Vector3[] { new Vector3(6F,0.5F,0), new Vector3(7F,0.5F,0), new Vector3(8F,0.5F,0),
                new Vector3(9F,0.5F,0), new Vector3(10F,0.5F,0), new Vector3(11F,0.5F,0) };

        characterOnLand = new ICharacterController[6];

        type = _type;
        if (type == 1)
        {
            land = Object.Instantiate(Resources.Load("Prefabs/Land", typeof(GameObject)), rightPos, Quaternion.identity, null) as GameObject;
            land.name = "rightLand";
        }
        else
        {
            land = Object.Instantiate(Resources.Load("Prefabs/Land", typeof(GameObject)), leftPos, Quaternion.identity, null) as GameObject;
            land.name = "leftLand";
        }
    }

    public Vector3 getEmptyPosition()
    {
        Vector3 pos = landPositions[getEmptyIndex()];
        pos.x *= type;
        return pos;
    }

    public int getEmptyIndex()
    {
        for (int i = 0; i < characterOnLand.Length; i++)
        {
            if (characterOnLand[i] == null)
                return i;
        }
        return -1;
    }

    public void getOnLand(ICharacterController chracter)
    {
        characterOnLand[getEmptyIndex()] = chracter;
    }

    public ICharacterController getOffLand(string name)
    {
        for (int i = 0; i < characterOnLand.Length; i++)
        {
            if (characterOnLand[i] != null && characterOnLand[i].getName() == name)
            {
                ICharacterController tmp = characterOnLand[i];
                characterOnLand[i] = null;
                return tmp;
            }
        }
        return null;
    }

    public int getType()
    {
        return type;
    }

    public int[] getStatus()
    {
        int[] status = { 0, 0 };
        for (int i = 0; i < characterOnLand.Length; i++)
        {
            if (characterOnLand[i] == null)
                continue;
            status[characterOnLand[i].getType()]++;
        }
        return status;
    }// 0: priests, 1: Demon

    public void reset()
    {
        characterOnLand = new ICharacterController[6];
    }
}
