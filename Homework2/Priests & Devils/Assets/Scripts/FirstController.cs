using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstController : MonoBehaviour, ISceneController, IUserAction {
    readonly Vector3 riverPosition = new Vector3(0, -0.25f, 0);

    UserGUI userGUI;

    public LandController rightLand;
    public LandController leftLand;
    public BoatController boat;
    public ICharacterController[] characters;

    void Awake()
    {
        GameDirector director = GameDirector.getInstance();
        director.currentSceneController = this;
        userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;
        genGameObjects();
    }

    public void genGameObjects()
    {
        characters = new ICharacterController[6];
        GameObject river = Instantiate(Resources.Load("Prefabs/River", typeof(GameObject)), riverPosition, Quaternion.identity, null) as GameObject;
        river.name = "river";

        boat = new BoatController();
        leftLand = new LandController(-1);
        rightLand = new LandController(1);

        for (int i = 0; i < 3; i++)
        {
            ICharacterController priest = new ICharacterController(0, "priest" + i);
            priest.setPosition(rightLand.getEmptyPosition());
            priest.getOnLand(rightLand);
            rightLand.getOnLand(priest);
            characters[i] = priest;
        }

        for (int i = 0; i < 3; i++)
        {
            ICharacterController demon = new ICharacterController(1, "demon" + i);
            demon.setPosition(rightLand.getEmptyPosition());
            demon.getOnLand(rightLand);
            rightLand.getOnLand(demon);
            characters[i+3] = demon;
        }
    }


    public void ClickCharacter(ICharacterController character)
    {
        if (userGUI.status != 0 || !boat.available())
        {
            return;
        }
        if (character.isOnBoat()) {
            LandController land;
            if (boat.getBoatPos() == 0)
            {
                land = leftLand;
            }
            else
            {
                land = rightLand;
            }
            boat.getOffBoat(character.getName());
            character.MoveTo(land.getEmptyPosition());
            character.getOnLand(land);
            land.getOnLand(character);
        }
        else
        {
            LandController land = character.getLandController();
            if (boat.getEmptyIndex() == -1)
                return;
            int landPos = land.getType(), boatPos = (boat.getBoatPos() == 0) ? -1 : 1;
            if (landPos != boatPos)
                return;
            land.getOffLand(character.getName());
            character.MoveTo(boat.getEmptyPosition());
            character.getOnBoat(boat, boat.getEmptyIndex());
            boat.getOnBoat(character);
        }
        userGUI.status = checkResult();
    }

    public void ToggleBoat()
    {
        if (userGUI.status != 0 || boat.isEmpty())
            return;
        boat.Move();
        userGUI.status = checkResult();
    }

    int checkResult()
    {
        int leftPriests = 0;
        int rightPriests = 0;
        int leftDemons = 0;
        int rightDemons = 0;

        int[] leftStatus = leftLand.getStatus();
        leftPriests += leftStatus[0];
        leftDemons += leftStatus[1];

        if (leftPriests + leftDemons == 6)
            return 2;

        int[] rightStatus = rightLand.getStatus();
        rightPriests += rightStatus[0];
        rightDemons += rightStatus[1];

        int[] boatStatus = boat.getBoatStatus();
        if (boat.getBoatPos() == 0)
        {
            leftPriests += boatStatus[0];
            leftDemons += boatStatus[1];
        }
        else
        {
            rightPriests += boatStatus[0];
            rightDemons += boatStatus[1];
        }

        if (leftPriests > 0 && leftPriests < leftDemons)
            return 1;
        if (rightPriests > 0 && rightPriests < rightDemons)
            return 1;

        return 0;
    }

    public void restart()
    {
        boat.reset();
        leftLand.reset();
        rightLand.reset();
        for (int i = 0; i < characters.Length; i++)
            characters[i].reset();
    }
}
