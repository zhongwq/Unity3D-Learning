using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {
    readonly float speed = 20;

    int status;//0: 静止, 1: 处于前段移动, 2: 处于后段移动
    Vector3 middle;
    Vector3 destination;

    void Update()
    {
        if (status == 1)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, middle, Time.deltaTime * speed);
            if (transform.position == middle)
            {
                status = 2;
            }
        }
        else if (status == 2)
        {
            this.transform.position = Vector3.MoveTowards(this.transform.position, destination, Time.deltaTime * speed);
            if (this.transform.position == destination)
            {
                status = 0;
            }
        }
    }

    public int getStatus()
    {
        return status;
    }

    public  void moveTo(Vector3 _destination)
    {
        destination = _destination;
        middle = _destination;
        if (_destination.y == this.transform.position.y)
        {
            status = 2;
            return;
        }
        else if (_destination.y < this.transform.position.y)
        {
            middle.y = transform.position.y;
        }
        else
        {
            middle.x = transform.position.x;
        }
        status = 1;
    }

    public void reset()
    {
        status = 0;
    }
}
