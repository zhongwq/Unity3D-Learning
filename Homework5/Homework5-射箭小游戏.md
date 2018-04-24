# Homework5-射箭小游戏

### 游戏要求
1. 靶对象为 5 环，按环计分；
2. 箭对象，射中后要插在靶上
3. 增强要求：射中后，箭对象产生颤抖效果，到下一次射击 或 1秒以后（未实现）
4. 游戏仅一轮，无限 trials；
5. 增强要求：添加一个风向和强度标志，提高难度


### 实现效果
视频演示网站为[ArrowGame](http://v.youku.com/v_show/id_XMzU2MTk2NDQxMg==.html?spm=a2h3j.8428770.3416059.1)

下面是游戏的截图

![屏幕快照 2018-04-24 下午8.05.59](https://lh3.googleusercontent.com/--xyZKm6bxT0/Wt8f4GbaJjI/AAAAAAAAAG0/rxtIvjs5e801pgf6ORV0bjwqm0aBxUaVgCHMYCw/I/%255BUNSET%255D)
![屏幕快照 2018-04-24 下午8.06.13](https://lh3.googleusercontent.com/-lWwxc4qzO68/Wt8gAEA5q7I/AAAAAAAAAG4/ZoX8zNEeX1E2Os1Q1oMoCPVi6ppMil6NwCHMYCw/I/%255BUNSET%255D)
![屏幕快照 2018-04-24 下午8.06.32](https://lh3.googleusercontent.com/-z6pKnIKNl-I/Wt8gHzECD_I/AAAAAAAAAG8/E10SUYXmZc8_46kdGrJzDF-1BK2I_8RxACHMYCw/I/%255BUNSET%255D)



### UML图
![week7](https://lh3.googleusercontent.com/-n9RLBikE8L4/Wt8eL9jqYrI/AAAAAAAAAGU/g5xlhpIFsOsu1T1YJFtncmKAMSbQec4kQCHMYCw/I/week7.png)

### 游戏实现
做了这几次的作业以后，我发现基本架构的代码都几乎可以复用，所以我直接把一些基本的每次用到的代码弄了一个BasicCode文件夹，ActionManager也摆放在内，编写游戏的时候，在其基础上增加就好了。

根据我们画出的UML图，我们可以看到我们这次需要完成的类不算多, 基础的代码都有，不不不过这次作业中的PhysicsActionManager，我经过上一次作业的编辑，经过思考，觉得其应该继承我们的SSActionManager，这样不仅可以让我们减少了一个接口类的书写，还把其整合到整个ActionManager的系统中，拥有ActionCallback等的功能，动作的完成的判断可以直接在自身的Update里完成，比起上一次作业优化了挺多。

现在我们分类说说

#### ArrowFlyAction
这次作业最基本的肯定就是我们箭的动作了，这里我不仅包含了射箭出去的动作，还包含了风向对于箭产生的动作，至于为什么我会把风力对箭的影响实体化成动作在游戏中呢，因为通过使用ActionManager去管理风力，我们可以在场景控制器中减少相应的代码。

ArrowFlyAction实现如下

```C#
public class ArrowFlyAction : SSAction {
	public int type; // 0: windAction, 1: shootAction
	public Vector3 speed;
	bool firstTime = false;

	private ArrowFlyAction() {}

	public static ArrowFlyAction GetSSAction(int type, Vector3 speed) {
		ArrowFlyAction arrowFlyAction = ScriptableObject.CreateInstance<ArrowFlyAction> ();
		arrowFlyAction.speed = speed;
		arrowFlyAction.firstTime = true;
		arrowFlyAction.type = type;
		return arrowFlyAction;
	}

	public override void Start() {}

	public override void Update() {
		if (firstTime) {
			if (type == 0) {
				// wind
				this.gameObject.GetComponent<Rigidbody> ().AddForce (speed, ForceMode.Force);
			} else {
				// shoot
				this.gameObject.GetComponent<Rigidbody> ().AddForce (speed, ForceMode.VelocityChange);
			}
			firstTime = false;
		}
		if (this.gameObject.transform.position.z > 50) {
			this.destroy = true;
			this.callback.actionDone (this);
		}
		if (this.gameObject.GetComponent<ArrowControl> ().arrowController.available == false) {
			this.destroy = true;
		}
	}
}
```

#### PhysicsActionManager
和我一开始说的一样，我的PhysicsActionManager继承了SSActionManager类,并实现了ActionCallback接口以完成事件结束的处理。该类实现挺简单，这里不多BB

```C#
public class PhysicsActionManager : SSActionManager, ActionCallback {
	public FirstController firstController;

	public void Start () {
		firstController = (FirstController)GameDirector.getInstance ().currentSceneController;
		firstController.physicsActionManager = this;
	}

	public void Update() {
		base.Update ();
	}

	public void actionDone (SSAction source) {
		if (((ArrowFlyAction)source).type == 1) {
			firstController.arrowFactory.recycle (((ArrowFlyAction)source).gameObject.GetComponent<ArrowControl> ().arrowController);
			firstController.shootFinish = true;
		}
	}
}
```

到这里，基础的部分就差不多了，还剩下负责具体功能的各个类与我们的场景控制器。

#### CheckCrack
我们先来看看我用于检查物体碰撞以提供给计分器，并且使得箭体碰到靶子后插在靶上(停住)的类

这里主要用到了OnTriggerEnter来确定箭体碰到靶子这一事件，只要我们的CheckCrack Add到各个分数对应的GameObject中，我们的靶子就能获取箭射到的位置，交给计分员计分。

在这里，可能因为我靶子实现的不太好，我的箭在射中靶子后，插入靶子里面，从而也会触发后面靶子的触发器，所以这里可以看到，我的ArrowController里加入了一个available，这样保证箭一旦插入就不再处理对应的触发事件。

```C#
public class CheckCrack : MonoBehaviour {
	public FirstController firstController;
	public ScoreRecorder scoreRecorder;

	void Start() {
		firstController = (FirstController)GameDirector.getInstance ().currentSceneController;
		scoreRecorder = ScoreRecorder.getInstance ();
	}

	void OnTriggerEnter(Collider arrow) {
		if (arrow.gameObject.GetComponent<ArrowControl> ().arrowController.available == true) {
			arrow.gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			arrow.gameObject.GetComponent<Rigidbody> ().isKinematic = true;
			arrow.gameObject.GetComponent<ArrowControl> ().arrowController.available = false;
			arrow.gameObject.transform.GetComponent<Collider> ().enabled = false;
			firstController.shootFinish = true;
			firstController.showCamera.showCamera ();
			scoreRecorder.record (gameObject);
		}
	}
}
```

#### ScoreRecorder
计分员的实现和上次类似，不过这一次是根据GameObbject的名字来加分

```C#
public class ScoreRecorder {
	public int score = 0;

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

	private ScoreRecorder() {
		gameInfo = (GameObject.Instantiate (Resources.Load ("Prefabs/ScoreInfo")) as GameObject).transform.Find ("Text").GetComponent<Text> ();
		gameInfo.text = "" + score;
	}

	public void record(GameObject hitObj) {
		switch (hitObj.name) {
			case "1":
				score += 1;
				break;
			case "2":
				score += 2;
				break;
			case "3":
				score += 3;
				break;
			case "4":
				score += 4;
				break;
			case "5":
				score += 5;
				break;
		}
		gameInfo.text = "" + score;
	}


	public int getScore() {
		return score;
	}

	public void reset() {
		score = 0;
		gameInfo.text = "" + score;
	}
}
```

#### showSubCamera
这里我加了个子摄像头的功能，若玩家射中靶子，我会激活该摄像头，使玩家清晰看到箭射中的位置

```C#
public class showSubCamera : MonoBehaviour {
	private float time = 0;
	private bool show = false;
	GameObject cameraObj;

	// Use this for initialization
	void Start () {
		show = false;
		cameraObj = (GameObject.Instantiate (Resources.Load ("Prefabs/SubCamera")) as GameObject);
		cameraObj.SetActive (false);
	}

	public void showCamera() {
		cameraObj.SetActive (true);
		show = true;
		time = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (show) {
			time -= Time.deltaTime;
			if (time <= 0) {
				show = false;
				cameraObj.SetActive (false);
			}
		}
	}
}
```

#### WindMake
游戏中我直接实现了一个WindMake类来管理风的生成，每240帧数更换一个风向，并且把其实时的风向显示到游戏画面的左上角

```C#
public class WindMake : MonoBehaviour {
	public Vector3 Wind;
	Text gameInfo;
	int fpsCount = 0;

	void Awake() {
		gameInfo = (GameObject.Instantiate (Resources.Load ("Prefabs/WindInfo")) as GameObject).transform.Find ("Text").GetComponent<Text> ();
		makeWind ();
		fpsCount = 0;
	}

	void Update() {
		fpsCount++;
		if (fpsCount == 240) {
			Wind = makeWind ();
			fpsCount = 0;
		}// 240帧换一个风向
	}

	public Vector3 makeWind() {
		float y = Random.Range (-100, 100);
		float z = Random.Range (-30, 0);
		float x = Random.Range (-50, 50);
		gameInfo.text = "(" + x + "," + y + "," + z + ")";
		return new Vector3 (x, y, z);
	}

	public Vector3 getWind() {
		return Wind;
	}
}
```

说到这里，突然看到还有我们的ArrowFactory没有讲🌚

#### ArrowFactory
实现和基本的Factory类似，没有什么可讲的, 就是加上了对于刚体的初始化等的一些内容

```C#
public class ArrowFactory : MonoBehaviour {
	Queue<ArrowController> waitingQueue;
	List<ArrowController> runningList;
	 
	private FirstController firstController;


	GameObject basic;

	private void Awake() {
		waitingQueue = new Queue<ArrowController> ();
		runningList = new List<ArrowController> ();

		basic = Instantiate (Resources.Load ("Prefabs/Arrow", typeof(GameObject)) as GameObject);
		basic.SetActive (false);
	}

	public ArrowController getArrow() {
		ArrowController arrow;
		if (waitingQueue.Count == 0) {
			GameObject newArrow = GameObject.Instantiate (basic);
			arrow = new ArrowController (newArrow);
		} else {
			arrow = waitingQueue.Dequeue ();
		}
		arrow.getObject ().transform.parent = ((FirstController)GameDirector.getInstance ().currentSceneController).Bow.transform;
		arrow.getObject().GetComponent<Rigidbody> ().velocity = Vector3.zero;
		arrow.getObject ().transform.localPosition = new Vector3 (0, 2, 1);
		arrow.getObject ().GetComponent<Rigidbody> ().isKinematic = true;
		arrow.getObject ().transform.GetComponent<Collider> ().enabled = true;
		arrow.appear ();
		runningList.Add (arrow);
		return arrow;
	}

	public void recycle(ArrowController arrow) {
		arrow.disappear ();
		arrow.getObject ().transform.position = new Vector3 (0, 0, -8);
		arrow.getObject ().transform.GetComponent<Collider> ().enabled = false;
		runningList.Remove (arrow);
		waitingQueue.Enqueue (arrow);
	}

	public void recycleAll() {
		while (runningList.Count != 0)
		{
			recycle(runningList[0]);
		}
	}
}
```

#### ArrowController
这里为了方便Arrow的实现，我在这次作业中也一样实现了ArrowController类，加入了箭体的基本状态的信息，并方便了之后的拓展。

```C#
public class ArrowController{
	GameObject arrow;
	ArrowControl arrowControl;
	public bool available = false;

	public ArrowController (GameObject gameObject) {
		this.arrow = gameObject;
		arrowControl = gameObject.AddComponent<ArrowControl> ();
		arrowControl.arrowController = this;
	}

	public void appear()
	{
		available = true;
		arrow.SetActive(true);
	}

	public void disappear()
	{
		arrow.SetActive(false);
	}
		
	public GameObject getObject() {
		return arrow;
	}
}
```

#### ArrowControl类
这个类主要是用于GameObject获取其对应的Controller

```C#
public class ArrowControl : MonoBehaviour {
	public ArrowController arrowController;
}
```

#### FirstController类
写完各个功能的类后，就剩下我们的游戏场景控制器了,这里基本就是各个类的整合以及用户交互的实现，实现不难。

```C#
public class FirstController : MonoBehaviour, ISceneController {
	public PhysicsActionManager physicsActionManager;

	public GameDirector gameDirector;
	public GameObject Bow;
	public ArrowController arrow;
	public ScoreRecorder scoreRecorder;

	public ArrowFactory arrowFactory;
	public WindMake windMake;
	public showSubCamera showCamera;

	GUIStyle gameInfoStyle;
	public bool shootFinish = true; // whether arrow hit success

	void Awake() {
		arrowFactory = gameObject.AddComponent<ArrowFactory> ();
		windMake = gameObject.AddComponent<WindMake> ();
		physicsActionManager = gameObject.AddComponent<PhysicsActionManager> ();
		scoreRecorder = ScoreRecorder.getInstance ();
		showCamera = gameObject.AddComponent<showSubCamera> ();

		gameDirector = GameDirector.getInstance ();
		gameDirector.currentSceneController = this;
	}
		
	// Use this for initialization
	void Start () {
		loadResources ();
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		gameInfoStyle = new GUIStyle ();
		gameInfoStyle.fontSize = 25;
	}

	void OnGUI() {
		GUI.Label (new Rect (20, 80, 200, 50), "Press space to restart!", gameInfoStyle);
	}
	

	// Update is called once per frame
	void Update () {
		float offsetX = Input.GetAxis ("Mouse X");
		float offsetY = Input.GetAxis ("Mouse Y");
		moveBow (offsetX, offsetY);

		if (Input.GetKeyDown("escape")) {
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		if (Input.GetButton ("Fire1")) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			shootArrow ();
		}

		if (Input.GetKeyDown ("space")) {
			reset ();
		}

		if (shootFinish && arrow == null) {
			arrow = arrowFactory.getArrow ();
		}
		
	}

	public void moveBow(float offsetX, float offsetY) {
		float posy = Mathf.Clamp(Bow.transform.position.y + offsetY, 1, 30);
		Bow.transform.position = new Vector3 (Bow.transform.position.x + offsetX, posy, Bow.transform.position.z);
	}

	public void shootArrow() {
		if (arrow != null) {
			GameObject arrowObj = arrow.getObject ();
			arrowObj.GetComponent<Rigidbody> ().isKinematic = false;
			Vector3 dir = arrowObj.transform.up * -1;
			ArrowFlyAction arrowAction = ArrowFlyAction.GetSSAction (1, dir * 30);
			ArrowFlyAction windAction = ArrowFlyAction.GetSSAction (0, windMake.getWind());
			physicsActionManager.addAction (arrowObj, arrowAction, physicsActionManager);
			physicsActionManager.addAction (arrowObj, windAction, physicsActionManager);
			arrowObj.transform.parent = null;
			arrow = null;
			shootFinish = false;
		}
	}

	public void loadResources() {
		Instantiate(Resources.Load("Prefabs/Land"));
		Instantiate(Resources.Load("Prefabs/Target"));
		Bow = Instantiate(Resources.Load("Prefabs/Bow")) as GameObject;  
		arrow = arrowFactory.getArrow ();
	}

	void reset() {
		arrow = null;
		shootFinish = true;
		scoreRecorder.reset ();
		arrowFactory.recycleAll ();
	}
}
```

---
到这里这周的作业就完成了，难度不算太大，在学习了物理引擎的使用后，我们Action的使用更加简单，只需要加上对应的力就可以了，并不需要一些复杂的语句，而且有了之前几周完成的基础的游戏架构，我们基本就是完成负责游戏对象的创建功能的类，完成对象基本动作的类，以及游戏场景中对应功能解耦出来的类。只要有了这些基本的意识后，我们对于写游戏的上手还是很快的。

