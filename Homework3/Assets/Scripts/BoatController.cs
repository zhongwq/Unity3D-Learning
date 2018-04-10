using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController {
    readonly GameObject boat;
	public readonly float speed = 10;
    readonly Vector3 right = new Vector3(3.5f, 0, 0);
    readonly Vector3 left = new Vector3(-3.5f, 0, 0);
    readonly Vector3[] right_positions;
    readonly Vector3[] left_positions;

	private GameObject cameraObj;

    int status;// 0: left, 1: right
    ICharacterController[] characterOnBoat = new ICharacterController[2];

    public BoatController()
    {
        status = 1;
        right_positions = new Vector3[] { new Vector3(2.5F, 0.2F, 0), new Vector3(4.5F, 0.2F, 0) };
        left_positions = new Vector3[] { new Vector3(-4.5F, 0.2F, 0), new Vector3(-2.5F, 0.2F, 0) };
        boat = Object.Instantiate(Resources.Load("Prefabs/Boat", typeof(GameObject)), right, Quaternion.identity, null) as GameObject;
        boat.name = "boat";
        boat.AddComponent(typeof(ClickGUI));
		attachCamera ();
    }

	private void attachCamera() {
		cameraObj = new GameObject("BoatCamera");
		cameraObj.transform.parent = boat.transform;
		cameraObj.transform.localPosition = new Vector3(0, 7, -10);
		cameraObj.transform.localRotation = Quaternion.Euler(10, 0, 0);
		cameraObj.AddComponent<Camera>();
		Camera cameraComp = cameraObj.GetComponent<Camera>();
		cameraComp.fieldOfView = 40;
		cameraObj.AddComponent<Skybox>().material = Resources.Load("Nostalgia 1") as Material;
	}

	public void toggleCamera() {
		Camera cameraComp = cameraObj.GetComponent<Camera> ();
		if (cameraComp.depth == 0)
			cameraComp.depth = -2;
		else
			cameraComp.depth = 0;
	}

	public Vector3 getTarget()
    {
        if (status == 1)
        {
            return left;
        }
        else
        {
           	return right;
        }
    }

	public void toggle() {
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

	public bool available () {
		return ((status == 0) ? left : right) == boat.transform.position;
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

    public void reset()
    {
        if (status == 0)
        {
			toggle ();
        }
		boat.transform.position = right;
        characterOnBoat = new ICharacterController[2];
    }
}
