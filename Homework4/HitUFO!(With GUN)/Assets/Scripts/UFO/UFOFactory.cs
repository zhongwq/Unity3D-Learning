using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOFactory : MonoBehaviour {
    Queue<UFOController> waitingQueue;
    List<UFOController> runningList;

    GameObject basic;

    private void Awake()
    {
        waitingQueue = new Queue<UFOController>();
        runningList = new List<UFOController>();

        basic = Instantiate(Resources.Load("Prefabs/UFO", typeof(GameObject))) as GameObject;
        basic.SetActive(false);
    }

    public UFOController GetUFO(UFOData data)
    {
        UFOController ufo;
        if (waitingQueue.Count > 0)
            ufo = waitingQueue.Dequeue();
        else
        {
            GameObject newUFO = GameObject.Instantiate(basic);
            ufo = new UFOController(newUFO);
            newUFO.transform.position += Vector3.forward * Random.value * 10;
        }
        ufo.setAttribute(data);
        runningList.Add(ufo);
        ufo.appear();
        return ufo;
    }

    public List<UFOController> GetRunningList()
    {
        return this.runningList;
    }

    public UFOController[] initAll (UFOData data, int num)
    {
        UFOController[] list = new UFOController[num];
        for (int i = 0; i < num; i++)
        {
            list[i] = GetUFO(data);
        }
        return list;
    }

    public void recycle(UFOController ufo)
    {
        ufo.disappear();
        waitingQueue.Enqueue(ufo);
        runningList.Remove(ufo);
    }

    public void recycleAll()
    {
        while (runningList.Count != 0)
        {
            recycle(runningList[0]);
        }
    }
}
