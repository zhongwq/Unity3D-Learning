using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : System.Object
{

    private static GameDirector _instance;

    public ISceneController currentSceneController { get; set; }
    public bool running { set; get; }

    public static GameDirector getInstance()
    {
        if (_instance == null)
        {
            _instance = new GameDirector();
            return _instance;
        }
        return _instance;
    }

    public int getFPS()
    {
        return Application.targetFrameRate;
    }

    public void setFPS(int fps)
    {
        Application.targetFrameRate = fps;
    }
}