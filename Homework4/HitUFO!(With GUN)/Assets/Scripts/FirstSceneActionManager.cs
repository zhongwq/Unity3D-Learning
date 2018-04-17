using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSceneActionManager : SSActionManager {
    Vector3 getRandomPosAroundPos(Vector3 pos)
    {
        return new Vector3(
            Random.Range(pos.x - 6, pos.x + 6),
            Random.Range(6, pos.y + 8),
            Random.Range(pos.z + 2, pos.z + 6));
    }

    public void addActionToUFO(GameObject ufo, float speed)
    {
        Vector3 ufoPos = ufo.transform.position;
        CCSequenceAction sequence = CCSequenceAction.getAction(-1, 0, new List<SSAction> {
                                                                    MoveAction.getAction(getRandomPosAroundPos(ufoPos), speed),
                                                                    MoveAction.getAction(getRandomPosAroundPos(ufoPos), speed)});
        addAction(ufo, sequence, this);
    }

    public void addActionToUFOs(UFOController[] ufoArr, float speed)
    {
        foreach(var ufo in ufoArr)
        {
            addActionToUFO(ufo.GetObject(), speed);
        }
    }
}
