# Homework4 HitUFO！ 游戏编程实践

### 作业要求
* 内容要求
    * 游戏有 n 个 round，每个 round 都包括10 次 trial
    * 每个 trial 的飞碟的色彩、大小、发射位置、速度、角度、同时出现的个数都可能不同。它们由该 round 的 ruler 控制
    * 每个 trial 的飞碟有随机性，总体难度随 round 上升
    * 鼠标点中得分，得分规则按色彩、大小、速度不同计算，规则可自由设定
* 游戏要求
    * 使用带缓存的工厂模式管理不同飞碟的生产与回收，该工厂必须是场景单实例的！具体实现见参考资源 Singleton 模板类
    * 尽可能使用前面 MVC 结构实现人机交互与游戏模型分离


### 作业实现
本周的内容在有了之前MVC架构以及动作管理器的基础之上，其实本次作业的实现较为简单。按照之前的惯例，为了编程的条理性，我们先设计出我们这次作业的UML图

#### 视频演示
* [Hit UFO!](http://new-play.tudou.com/v/886361316.html?spm=a2hzp.8244740.0.0)
* [Hit UFO! (With GUN)](http://new-play.tudou.com/v/886354731.html?spm=a2hzp.8244740.0.0)

---
#### UML图

![week6](https://lh3.googleusercontent.com/-WDA88p802Pg/WtXqkJ9wbNI/AAAAAAAAAGE/nAt63khQThY0V3pmiWKaZ62YADuab87ZACHMYCw/I/week6.png)

> 其中FirstCharacterController类是为GUN版本创建的。

---

#### 分类介绍

首先，我们来介绍一下这次作业的重中之重，工厂类的实现，这次的作业一共有两个工厂类，一个是UFO生成的工厂，一个是爆炸生成的工厂，**通过带缓存的工厂对象的实现，我们减少了游戏对象的创建以及销毁的次数，可以大大减少游戏的开销，并且游戏对象的创建和销毁由一个对象统一管理，使得业务逻辑更加明确，从而使我们的程序更加容易扩展。**



下面我们先来看看这两个类

##### UFOFactory

```C#
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
```

##### ExplosionFactory

```C#
public class ExplosionFactory : MonoBehaviour {
	Queue<GameObject> waitingQueue;
	List<GameObject> runningList;

	GameObject basic;

	private void Awake()
	{
		waitingQueue = new Queue<GameObject>();
		runningList = new List<GameObject>();

		basic = Instantiate(Resources.Load("Prefabs/Explosion", typeof(GameObject))) as GameObject;
		basic.SetActive(false);
	}

	public void explode(Vector3 pos)
	{
		GameObject explosion;
		if (waitingQueue.Count == 0) {
			explosion = GameObject.Instantiate (basic);
			explosion.AddComponent<SelfRecycle> ().factory = this;
		} 
		else 
		{
			explosion = waitingQueue.Dequeue ();
		}
		runningList.Add (explosion);
		explosion.GetComponent<SelfRecycle> ().startTimer (0.2f);

		explosion.SetActive (true);
		explosion.transform.position = pos;
	}

	public void recycle(GameObject explosion)
	{
		explosion.SetActive (false);
		waitingQueue.Enqueue(explosion);
		runningList.Remove(explosion);
	}
}
```

##### FlyAction
在这里我使用的是我们第一个星期所写的抛物线运动的代码，外部通过getAction提供UFO的x, y的初速度，生成物体的速度向量，然后每一帧都给物体加上对应重力的移动和初速度的移动即可得到物体的抛物线。

```C#
public class FlyAction : SSAction {
	public float Vx;
	public float Vy;
	public Vector3 speed;
	public Vector3 gravity;

	private FlyAction() 
	{
	}

	public static FlyAction getAction(float vx, float vy) 
	{
		FlyAction action = ScriptableObject.CreateInstance<FlyAction> ();
		action.gravity = Vector3.zero;
		action.Vx = vx;
		action.Vy = vy;
		return action;
	}

	// Use this for initialization
	public override void Start () 
	{
		speed = new Vector3 (Vx, Vy, 0);
	}
	
	// Update is called once per frame
	public override void Update () 
	{
		if (2 * Vy + gravity.y > 0.00001) 
		{
			gravity.y -= 10 * Time.fixedDeltaTime;
			transform.Translate (speed * Time.fixedDeltaTime);
			transform.Translate (gravity * Time.fixedDeltaTime);
		}
		else 
		{
			destroy = true;
			callback.actionDone (this);
		}
	}
}
```

##### Fire
Fire类是游戏的提供开火操作的类，通过该类，我们可以获取用户的输入，并且对于用户操作进行相应的处理，我们本次实验主要用到了Ray来实现用户的射击操作，通过Raycast我们可以获取射线击中的物体，并且把其通过相应的接口传给我们的场记(SceneControll)，完成相应的操作。

```C#
public class Fire : MonoBehaviour {
    public new Camera camera;
    public FirstController sceneController;
    LayerMask layer;

    void Awake()
    {
        layer = LayerMask.GetMask("UFO", "Terrian");
    }

    // Use this for initialization
    void Start () {
        camera = Camera.main;
        sceneController = GameDirector.getInstance().currentSceneController as FirstController;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButton("Fire1"))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity ,layer))
            {
                if (hit.transform.gameObject.layer == 8)
                {
                    sceneController.shootUFO(hit.transform.gameObject.GetComponent<UFOCtrl>().ufoController);
                }
                else if (hit.transform.gameObject.layer == 9)
                {
					sceneController.shootGround (hit.point);
                }
            }
        }
	}
}
```

##### ScoreRecorder

ScoreRecorder的作用就是我们游戏的计分员，我们积分数据维护没有放在场记里面，而是把其分离了出来做了一个记分员，由记分员来维护我们的积分

```C#
public class ScoreRecorder{
    private int score;

    Text gameInfo;

    private static ScoreRecorder instance;
    public static ScoreRecorder getInstance()
    {
        if (instance == null)
        {
            instance = new ScoreRecorder();
        }
        return instance;
    }

    private ScoreRecorder()
    {
        gameInfo = (GameObject.Instantiate(Resources.Load("Prefabs/ScoreInfo")) as GameObject).transform.Find("Text").GetComponent<Text>();
        gameInfo.text = "" + score;
    }

    public void record(int difficulty)
    {
        score += difficulty+1;
        gameInfo.text = "" + score;
    }

    public int getScore()
    {
        return score;
    }
}
```

##### FirstController
FirstController类就是我们这次游戏场景的场记了，在之前学习的基础之上，这一次游戏架构的设计更加合理，场记相比上一次的游戏更加简洁易看，可读性更强。

```C#
public class FirstController : MonoBehaviour, ISceneController {
    GameDirector gameDirector;
    FirstSceneActionManager actionManager;
    public UFOFactory ufoFactory;
	ExplosionFactory explosionFactory;

    ScoreRecorder scoreRecorder;
    TimerController timerController;
    DifficultyController difficulty;

	int gameStatus;
	int round = 1;
	int emitNum;
	int scoreInRound;
	int fpsCount;
	bool running;
	GUIStyle headerStyle;
	GUIStyle buttonStyle;
	Text roundText;

    void Awake()
    {
        gameDirector = GameDirector.getInstance();
        gameDirector.currentSceneController = this;
        actionManager = gameObject.AddComponent<FirstSceneActionManager>();
        ufoFactory = gameObject.AddComponent<UFOFactory>();
		explosionFactory = gameObject.AddComponent<ExplosionFactory> ();
        difficulty = DifficultyController.getInstance();
        timerController = gameObject.AddComponent<TimerController>();
        scoreRecorder = ScoreRecorder.getInstance();

        loadResources();
    }

    public void Start()
    {
		gameStatus = 0;
		difficulty.setDifficulty(0);
		round = 1;
		roundText = (GameObject.Instantiate(Resources.Load("Prefabs/RoundInfo")) as GameObject).transform.Find("Text").GetComponent<Text>();
		roundText.text = "" + round;
		fpsCount = 0;
		headerStyle = new GUIStyle();
		headerStyle.fontSize = 40;
		headerStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle = new GUIStyle("button");
		buttonStyle.fontSize = 30;
		running = false;
    }

	void OnGUI() 
	{
		if (gameStatus == 0) 
		{
			GUI.Label(new Rect(Screen.width / 2 - 45, Screen.height / 2 - 90, 100, 50), "Hit the UFO!", headerStyle);
			if (GUI.Button(new Rect(Screen.width / 2 - 45, Screen.height / 2, 100, 50), "Play!",buttonStyle))
			{
				replay ();	
			}
				
		}
		else if (gameStatus == 2) 
		{
			GUI.Label(new Rect(Screen.width / 2 - 45, Screen.height / 2 - 90, 100, 50), "You get "+ scoreRecorder.getScore()+" points in this game!", headerStyle);
			if (GUI.Button(new Rect(Screen.width / 2 - 90, Screen.height / 2, 200, 50), "Play Again",buttonStyle))
			{
				replay ();	
			}
		}
	}

    void Update()
    {
		if (running && gameStatus == 1)
		{
			fpsCount++;
			if (fpsCount == 60)
			{
				if (emitNum < difficulty.getEmitNum ()) {
					emitUFO ();
					fpsCount = 0;
					emitNum++;
				} else if (emitNum == difficulty.getEmitNum ()) {
					if (scoreInRound < difficulty.getSuccessScore ()) {
						gameStatus = 2;
					}
					else
					{
						Invoke("startRound", 3);
						timerController.setTime(3);
						difficulty.setDifficultyByScore (scoreRecorder.getScore ());
					}
				}
			}
		}
		else
		{
			if (Input.GetKeyDown ("enter") || Input.GetKeyDown ("return"))
				replay ();
		}
    }

    public void loadResources()
    {
        Instantiate(Resources.Load("Prefabs/Land"));
    }

    void startRound()
    {
		running = true;
		roundText.text = "" + round++;
		emitNum = 0;
		scoreInRound = 0;
		fpsCount = 0;
    }

	void emitUFO()
	{
		UFOController ufoCtrl = ufoFactory.GetUFO(difficulty.getUFOAttributes());
		ufoCtrl.appear ();
		actionManager.addActionToUFO(ufoCtrl.GetObject(), difficulty.getUFOAttributes().speed);
	}

    public void shootUFO(UFOController ufo)
    {
		scoreInRound++;
        scoreRecorder.record(difficulty.getDifficulty());
        actionManager.removeActionByObj(ufo.GetObject());
		explosionFactory.explode (ufo.GetObject ().transform.position);
        ufoFactory.recycle(ufo);
    }

	public void shootGround(Vector3 pos) 
	{
		explosionFactory.explode (pos);
	}

	public void replay() 
	{
		gameStatus = 1;
		ufoFactory.recycleAll();
		round = 1;
		roundText.text = "" + round++;
		Invoke("startRound", 3);
		timerController.setTime(3);
		difficulty.resetDifficulty ();
		scoreRecorder.reset ();
	}
}
```
---
### 游戏效果图
![屏幕快照 2018-04-17 下午8.32.43](https://lh3.googleusercontent.com/-7TfSCbR2GtQ/WtXpqzeUWCI/AAAAAAAAAF4/Kf0d2rzCM0kammsJh_gUj2IGozBBUR1RgCHMYCw/I/%255BUNSET%255D)

![屏幕快照 2018-04-17 下午8.33.09](https://lh3.googleusercontent.com/-uxqOdTXxGZE/WtXpoUfnvVI/AAAAAAAAAF0/NXUJLM53T1ExFylHgshTUEaCpC5QiYllgCHMYCw/I/%255BUNSET%255D)


---
### 总结
通过完成这一次的游戏，我体会到了之前我们所写的MVC、动作管理器的巨大作用，有了这些基本的架构，我们游戏的实现更加简易，基本只用实现我们的游戏物体的Controller、各个游戏功能"负责人"的Controller，然后通过场记对于各个Controller进行整合，我们就可以得到我们的整个游戏，只要我们设计好了我们的UML图，我们很快就可以实现出我们的整个游戏。

