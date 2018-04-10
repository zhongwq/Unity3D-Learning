# Homework3编程实践

## 牧师与魔鬼 动作分离版
在上一周的编程实践中，我们已经完成了一个具有MVC架构的牧师与魔鬼的版本，而我们这周的任务就是实现一个动作管理器，把我们上周写在GameObjectController和SceneController里的对于GameObject动作的处理写出来，通过一个SceneActionManager的实例来管理，让其专门负责GameObject的动作。下面我们来谈谈动作管理器。

---
### 建立动作管理器的意义
在上一个版本的简单的牧师与恶魔的游戏中，我为了避免FirstSceneController中对于物体移动操作的代码量过多，把物体的动作封装在GameObjectController里面，通过这种做法，FirstSceneController的代码量的确得到了一定的减少，但是这样的话我每写一个GameObjectController我就要多写一个物体运动的函数，一旦需要移动的GameObject多起来，可能冗余的代码量就会大量增多了，而且一旦我们需要修改Move，我们可能就每一个类都要进行修改了，显然这对于开发者非常不友好。

这个时候，动作管理器的意义就体现出来了，就以我们这次的作业为例子，Move的一类的动作抽象成一个动作类MoveAction，然后通过ActionManager对其进行管理。我们在需要物体进行移动时，只需要新建一个MoveAction，调用ActionManaer的addAction，把具体的参数传给它，ActionManager就会执行我们的动作，我们再也不用将Move这类的动作一个一个写在我们的GameObjController中，大大减少了我们的代码量，大大提高了我们代码的复用性，一旦动作方面的需求修改，我们也只需要修改该Action类的内容，以及相关调用创建该Action的地方，更加**利于我们的维护以及让程序更能适应需求变化**。

---

在了解了动作管理器的意义之后，我们就可以开始我们动作管理器的编写了，我们先画一个动作管理器的UML图理清一下思路
![Action](https://lh3.googleusercontent.com/-YSWanKlD7HY/Wsw_kCP9C4I/AAAAAAAAAFI/RZdcRwTrgR0MseQbnE7_2RHb9B6ybCi7QCHMYCw/I/Action.png)
然后与上一节课的UML图结合起来变成整个程序的UML图，画出来UML图之后，我们程序的架构就很清晰了。
![complete](https://lh3.googleusercontent.com/-_2oLcaG3GAs/Wsw_tw7Z1TI/AAAAAAAAAFM/EBRScTUY26ohx7cOcOW6rbDorQQUXWoxwCHMYCw/I/complete.png)


下面我们分类进行描述

### ActionCallBack

这个方法主要是提供一个让动作完成时调用的接口，一旦动作完成，该接口对应实现的方法就会被调用，动作管理器可以对动作的完成进行响应。

```C#
public interface ActionCallback {
	void actionDone (SSAction source);
}
```

---

### SSAction
SSAction这个类就是所有动作对象类抽象出来的一个不需要绑定 GameObject 对象的可编程基类，其动作的实现由其子类实现Update中的内容实现，这个在后面的MoveAction就有体现，所有的SSAction对象受ActionManager管理。

```C#
public class SSAction : ScriptableObject {
	public bool enable = true;
	public bool destroy = false;

	public GameObject gameObject;
	public Transform transform;
	public ActionCallback callback;

	public virtual void Start()
	{
		throw new System.NotImplementedException();
	}

	public virtual void Update()
	{
		throw new System.NotImplementedException();
	}
}
```

---

### MoveAction

MoveAction就是上面SSAction的一个子类，它的作用就是根据用户所提供的目的地和速度，使得物体完成一定速度的向某一目的地的直线运动。其通过Update的实现来完成逐帧的对于物体位置的变动，实现一个所谓的物体的"动作"。

```C#
public class MoveAction : SSAction {
	public Vector3 target;
	public float speed;

	private MoveAction(){
	}

	public static MoveAction getAction(Vector3 target, float speed) {
		MoveAction action = ScriptableObject.CreateInstance<MoveAction> ();
		action.target = target;
		action.speed = speed;
		return action;
	}

	// Use this for initialization
	public override void Start () {
		
	}
	
	// Update is called once per frame
	public override void Update () {
		transform.position = Vector3.MoveTowards(transform.position, target, speed*Time.deltaTime);
		if (transform.transform.position == target) {
			destroy = true;
			callback.actionDone (this);
		}
	}
}
```

---

### CCSequenceAction
这是一个动作顺序执行序列的是SSAction的子类，其通过一个Action的List，和actionDone对于List的维护以及Update对于调用哪个Action的Update方法的控制，实现了一个动作顺序执行的效果。十分方便了我们对于一个连续动作的编程。

```C#
public class CCSequenceAction : SSAction, ActionCallback {
	public List<SSAction> sequence;
	public int repeat = 1; // 1->only do it for once, -1->repeat forever
	public int currentActionIndex = 0;

	public static CCSequenceAction getAction(int repeat, int currentActionIndex, List<SSAction> sequence) {
		CCSequenceAction action = ScriptableObject.CreateInstance<CCSequenceAction>();
		action.sequence = sequence;
		action.repeat = repeat;
		action.currentActionIndex = currentActionIndex;
		return action;
	}

	public override void Update() {
		if (sequence.Count == 0)return;
		if (currentActionIndex < sequence.Count) {
			sequence[currentActionIndex].Update();
		}
	}

	public void actionDone(SSAction source) {
		source.destroy = false;
		this.currentActionIndex++;
		if (this.currentActionIndex >= sequence.Count) {
			this.currentActionIndex = 0;
			if (repeat > 0) repeat--;
			if (repeat == 0) {
				this.destroy = true;
				this.callback.actionDone(this);
			}
		}
	}

	public override void Start() {
		foreach(SSAction action in sequence) {
			action.gameObject = this.gameObject;
			action.transform = this.transform;
			action.callback = this;
			action.Start();
		}
	}

	void OnDestroy() {
		foreach(SSAction action in sequence) {
			DestroyObject(action);
		}
	}
}
```

---

### SSActionManager

这个就是我们这一期的主角ActionManager了，其负责了action的增加、删除、执行。它通过在Update中调用SSAction的Update方法，实现对于动作的一个调度，管理动作的自动执行。

```C#
public class SSActionManager : MonoBehaviour, ActionCallback {
	private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
	private List<SSAction> waitingToAdd = new List<SSAction>();
	private List<int> watingToDelete = new List<int>();

	protected void Update() {
		foreach(SSAction ac in waitingToAdd) {
			actions[ac.GetInstanceID()] = ac;
		}
		waitingToAdd.Clear();

		foreach(KeyValuePair<int, SSAction> kv in actions) {
			SSAction ac = kv.Value;
			if (ac.destroy) {
				watingToDelete.Add(ac.GetInstanceID());
			} else if (ac.enable) {
				ac.Update();
			}
		}

		foreach(int key in watingToDelete) {
			SSAction ac = actions[key];
			actions.Remove(key);
			DestroyObject(ac);
		}
		watingToDelete.Clear();
	}

	public void addAction(GameObject gameObject, SSAction action, ActionCallback callback) {
		action.gameObject = gameObject;
		action.transform = gameObject.transform;
		action.callback = callback;
		waitingToAdd.Add(action);
		action.Start();
	}

	public void actionDone(SSAction source) {

	}
}

```

---

# FirstSceneActionManager
FirstSceneActionManager就是我们上面SSActionManager的一个子类，对于我们场景中具体的动作进行了封装，我们只需要在FirstSceneController中调用该方法，就可以实现我们之前的Move了，十分方便了我们的编程。

```C#
public class FirstSceneActionManager : SSActionManager {
	public void toggleBoat(BoatController boat) {
		MoveAction action = MoveAction.getAction (boat.getTarget (), boat.speed);
		this.addAction (boat.getBoat (), action, this);
		boat.toggle ();
	}

	public void moveCharacter(ICharacterController character, Vector3 target) {
		Vector3 nowPos = character.getPos ();
		Vector3 tmpPos = nowPos;
		if (target.y > nowPos.y) {
			tmpPos.y = target.y;
		} else {
			tmpPos.x = target.x;
		}
		SSAction action1 = MoveAction.getAction(tmpPos, character.speed);
		SSAction action2 = MoveAction.getAction(target, character.speed);
		SSAction sequenceAction = CCSequenceAction.getAction(1, 0, new List<SSAction>{action1, action2});
		this.addAction(character.getInstance(), sequenceAction, this);
	}
}
```

### FirstController
在完成了FirstSceneActionManager后，我们把GameObjectController中以及FirstController中一些关于动作的部分删除，然后在原来动作的部分调用FirstSceneActionManager提供新的方法就可以了。(不过因为一开始设计架构的时候没有考虑加入动作管理器，加之最近时间较紧，没有太多时间对游戏进行重构，所以FirstController中仍然有着部分关于判断物体运动的逻辑)

```C#
public class FirstController : MonoBehaviour, ISceneController, IUserAction {
    UserGUI userGUI;

    public LandController rightLand;
    public LandController leftLand;
    public BoatController boat;
    public ICharacterController[] characters;
	private FirstSceneActionManager actionManager;

    void Awake()
    {
        GameDirector director = GameDirector.getInstance();
        director.currentSceneController = this;
        userGUI = gameObject.AddComponent<UserGUI>() as UserGUI;
        genGameObjects();
    }

	void Start() {
		actionManager = GetComponent<FirstSceneActionManager> ();
	}

    public void genGameObjects()
    {
        characters = new ICharacterController[6];
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
			actionManager.moveCharacter (character, land.getEmptyPosition ());
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
			actionManager.moveCharacter (character, boat.getEmptyPosition ());
            character.getOnBoat(boat, boat.getEmptyIndex());
            boat.getOnBoat(character);
        }
        userGUI.status = checkResult();
    }

    public void ToggleBoat()
    {
		if (userGUI.status != 0 || boat.isEmpty() || !boat.available())
            return;
		actionManager.toggleBoat (boat);
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
```

---
### 改进

* 根据上课学到的内容，给Camera加上了SkyBox，美化了程序的UI
* 给游戏添加了切换视角的功能，玩家可以根据自己喜好在全局视角以及船只跟随视角之间任意切换，游戏的体验更佳

---
到这里，这周的作业就完成啦，不说了，先去肝现操了(逃🌚

感谢阅读我的博客！😋


