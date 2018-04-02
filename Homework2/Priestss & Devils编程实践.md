# Homework2-编程实践

### 题目要求

* 阅读以下游戏脚本

> Priests and Devils
Priests and Devils is a puzzle game in which you will help the Priests and Devils to cross the river within the time limit. There are 3 priests and 3 devils at one side of the river. They all want to get to the other side of this river, but there is only one boat and this boat can only carry two persons each time. And there must be one person steering the boat from one side to the other side. In the flash game, you can click on them to move them and click the go button to move the boat to the other direction. If the priests are out numbered by the devils on either side of the river, they get killed and the game is over. You can try it in many > ways. Keep all priests alive! Good luck ！

##### 简答题：

* play the game ( http://www.flash-game.net/game/2535/priests-and-devils.html )
* 列出游戏中提及的事物（Objects）
    游戏中提及的事物有
    * 魔鬼
    * 牧师
    * 船
    * 两边的岸
    * 河
    
* 用表格列出玩家动作表（规则表），注意，动作越少越好

| 行为 | 行为条件 |
| --- | --- |
| 牧师／恶魔上船 | 船上有空位且船在该岸 |
| 开船 | 船上有人 |
| 牧师/恶魔下船 | 船有人且靠在岸边 |
| 游戏胜利 | 6个人都到了对岸 |
| 游戏失败 | 有一边岸上人数不均 |

##### 程序限制

* 请将游戏中对象做成预制
* 在 GenGameObjects 中创建 长方形、正方形、球 及其色彩代表游戏中的对象。
* 使用 C# 集合类型 有效组织对象
* 整个游戏仅 主摄像机 和 一个 Empty 对象， 其他对象必须代码动态生成！！！整个游戏不许出现 Find 游戏对象， SendMessage 这类突破程序结构的 通讯耦合 语句。 违背本条准则，不给分
* 请使用课件架构图编程，不接受非 MVC 结构程序
* 注意细节，例如：船未靠岸，牧师与魔鬼上下船运动中，均不能接受用户事件！

---
### Unity中的MVC架构
想使用一个架构进行开发，我们肯定需要先了解MVC架构如何在基于Unity的游戏的开发中使用。仔细分析Unity中的MVC结构，结合老师上课时所讲的内容，它其实和平时在WEB中常用MVC结构在本质上并没有太大的区别，都是关注点分离(SOC)的体现，是一种成熟的软件设计规范。使用MVC架构去开发我们的软件，可以使得我们在拓展软件功能、改进用户交互的时候，不需要重写大量的业务逻辑代码，减少了我们的代码量，方便了软件的拓展，就如老师所说的：“稳固的MVC框架经得起任意拓展”。
    
下面我们来谈谈Unity中的MVC架构:

* M(Model): Unity中，Model就是游戏场景中GameObject、和它们的关系以及后台的数据模型(比如用户信息等)，它们都受到相应的Controller的控制
* V(View): View这部分在游戏中起到的是与用户进行交互的功能，它们负责向用户战术游戏结果，处理GUI事件并通过IUserAction接口来与Controller进行交互。
* C(Controller): Controller控制着游戏场景中所有的对象，其负责接收来自View的用户的指令并根据用户输入的指令，更新Models以及更新View的状态，其可以被认为是连接Model与View的桥梁。

---
### 项目UML图

在了解了Unity中MVC的基本架构之后，我们就可以开始进行下面的开发了

按照题目要求的MVC结构，分析游戏的基本结构，得到基本的UML图的结构如下:
![UML](https://zhongwq.github.io/img/GameStructure-hw2.png)

## 对各模块分块进行开发
---
### GameDirector
首先，我们来开发担负着众多职责的GameDirector，在游戏里面，导演(Director)的职责大致如下:

* 获取当前游戏的场景
* 控制场景运行、切换、入栈与出栈，暂停、恢复、退出
* 管理游戏全局状态
* 设定游戏的配置
* 设定游戏全局视图

由上我们可知，GameDirector对于一个游戏是多么的重要，但是，在我们这次的小项目中，它的职责没有那么多，它在这次的项目中主要负责的是获取当前游戏场景，其代码如下:

```c#
public class GameDirector : System.Object {
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
```
Director在运行时只会有一个实例在运行，在上面的代码中，我们可以看到其应用了单例模式保证只有一个实例生成，方便了类之间的事件通信，方便了MVC的实现。

Director作为游戏最为高层的Controller，其不具体的控制游戏中的任一对象，其通过对currentSceneController的维护，把控制游戏对象的任务交给不同的SceneController。

---
### ISceneController
ISceneController的代码如下

```c#
//interface of Scene

public interface ISceneController
{
    void genGameObjects();
}
```
ISCeneController是游戏中的场景管理器，俗称场记，其主要职责如下:

* 管理游戏在该游戏场景中的所有的游戏对象
* 协调游戏对象、游戏对象管理器(GameObjectControllers)之间的通信
* 响应外部输入事件
* 管理该游戏场景的所有的规则
* 管理一些其他的东西

该统一的接口的实现正是GameDirector对各个游戏场景管理的关键，他是导演控制场景的渠道，GameDirector通过对于currentSceneController的维护，可以简单实现游戏场景的各种管理

---

### IUserAction
IUserAction中体现了游戏编程的门面模式，游戏外部与一个子系统的通信必须通过一个统一的门面对象进行，而我们现在实现的IUserAction就是这样的一个对象,GUI类通过IUserAction来与Controller进行通信，我们通过实现IUserAction接口来定义Controller与GUI的交互，这样我们在实现Controller类时只需要实现IUserAction对应的接口，他就可以与任意使用IUserAction接口的View即GUI类调用，GUI类也是只需调用接口中的方法即可实现与Controller的通信, 方便了我们MVC架构的实现。

其定义如下:

```C#
public interface IUserAction {
    void restart();
    void ToggleBoat();
    void ClickCharacter(ICharacterController chracter);
}
```
---
### UserGUI
UserGUI的代码如下:

```C#
public class UserGUI : MonoBehaviour {
    public int status = 0;
    private IUserAction action;
    GUIStyle headerStyle;
    GUIStyle buttonStyle;

	// Use this for initialization
	void Start () {
        action = GameDirector.getInstance().currentSceneController as IUserAction;
        headerStyle = new GUIStyle();
        headerStyle.fontSize = 40;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle = new GUIStyle("button");
        buttonStyle.fontSize = 30;
    }
	
	// Update is called once per frame
	void OnGUI () {
        GUI.Label(new Rect(Screen.width / 2 - 100, 10, 200, 50), "Priests & Demons", headerStyle);
        if (status == 1)
        {
            GUI.Label(new Rect(Screen.width / 2 - 45, Screen.height / 2 - 90, 100, 50), "Gameover!", headerStyle);
            if (GUI.Button(new Rect(Screen.width / 2 - 65, Screen.height / 2, 140, 70), "Restart", buttonStyle))
            {
                status = 0;
                action.restart();
            }
        }
        else if (status == 2)
        {
            GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2 - 90, 100, 50), "Win!", headerStyle);
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 140, 70), "Restart", buttonStyle))
            {
                status = 0;
                action.restart();
            }
        }
    }
}
```
由上我们可以看到，UserGUI只是简单地根据游戏状态，对游戏信息等进行展示，然后通过IUserAction与Controller进行交互，这也是我们在上面所提到的，我们只需要在GUI中调用IUserAction中的方法就可以迅速实现对游戏场景的控制，下面的ClickGUI也是一样

---
### ClickGUI
该类的代码如下:

```c#
public class ClickGUI : MonoBehaviour {
    IUserAction action;
    ICharacterController character;

    public void setController(ICharacterController characterController)
    {
        character = characterController;
    }

    void Start()
    {
        action = GameDirector.getInstance().currentSceneController as IUserAction;
    }

    void OnMouseDown()
    {
        if (gameObject.name == "boat")
        {
            action.ToggleBoat();
        }
        else
        {
            action.ClickCharacter(character);
        }
    }
}
```
在ClickGUI中，我们同样用到了IUserAction, ClickGUI监听事件的发生，然后通过IUserAction提供的类似移动船，游戏角色上下船的函数，把用户指令传给Controller进行执行。从而实现View和Controller之间的交互。

---
### FirstController
在该游戏中，只有一个游戏场景，所以

其代码实现如下:

```C#
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
        if (userGUI.status != 0 || boat.isEmptty())
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

```

FirstController继承了ISceneController和IUserAction这两个接口，并实现了其里面的函数，其既可以通过IUserAction与View进行交互，也可以在LoadResources后，通过GameObject的Controller对场景中的各个GameObject进行控制。

---
下面我们来介绍游戏中游戏对象的Controller的书写

首先，贯穿着我们这整一个游戏的就是GameObject的移动啦！所以我先说一下抽象出来的Move类，其代码如下：

```C#
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
```
为了让物体不要直接穿过land的墙体，当character上船时，先把物体移动到destination的对应的x坐标，然后再把物体移动到对应的位置，当charcter下船时，先把物体移动到destination的对应的y坐标，然后再把物体移动到对应的位置,这样保证了物体不会穿越墙体进行移动，且实现方式较为简单。

### ICharacterController
因为牧师与恶魔的动作等都是一样的，就只是外形、类型不一样，所以我把其抽象为一个Controller,通过type区分它们
其代码如下:

```C#
public class ICharacterController
{
    readonly GameObject character;
    readonly Move moveAction;
    readonly int type;//0: Priest, 1: Demon
    readonly ClickGUI clickGUI;

    bool onBoat;
    LandController landController;

    public ICharacterController(int chracterType, string name)
    {
        if (chracterType == 0)
        {
            this.character = Object.Instantiate(Resources.Load("Prefabs/Priest", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            this.type = 0;
        }
        else
        {
            this.character = Object.Instantiate(Resources.Load("Prefabs/Demon", typeof(GameObject)), Vector3.zero, Quaternion.identity, null) as GameObject;
            this.type = 1;
        }
        character.name = name;
        moveAction = character.AddComponent(typeof(Move)) as Move;
        clickGUI = character.AddComponent(typeof(ClickGUI)) as ClickGUI;
        clickGUI.setController(this);
    }

    public string getName()
    {
        return character.name;
    }

    public void setPosition(Vector3 pos)
    {
        character.transform.position = pos;
    }

    public void MoveTo(Vector3 destination)
    {
        moveAction.moveTo(destination);
    }

    public int getType()
    {
        return type;
    }

    public void getOnBoat(BoatController boatController, int pos)
    {
        landController = null;
        if (pos == 0)
        {
            character.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
        }
        else
        {
            character.transform.rotation = Quaternion.AngleAxis(270, Vector3.up);
        }
        character.transform.parent = boatController.getBoat().transform;
        onBoat = true;
    }

    public void getOnLand(LandController land)
    {
        landController = land;
        if (land.getType() == -1)
        {
            character.transform.rotation = Quaternion.AngleAxis(90, Vector3.up);
        }
        else
        {
            character.transform.rotation = Quaternion.AngleAxis(270, Vector3.up);
        }
        character.transform.parent = null;
        onBoat = false;
    }

    public bool isOnBoat()
    {
        return onBoat;
    }

    public LandController getLandController()
    {
        return landController;
    }

    public void reset()
    {
        moveAction.reset();
        landController = (GameDirector.getInstance().currentSceneController as FirstController).rightLand;
        getOnLand(landController);
        setPosition(landController.getEmptyPosition());
        landController.getOnLand(this);
    }
}
```
CharcterController类的实现十分简单，就是根据type导入预制，然后再实现上船，上岸，重置以及一些其他的get, set函数就完成了，具体可以看以上代码的实现。

### BoatController
BoatController类的代码如下:

```C#
public class BoatController {
    readonly GameObject boat;
    readonly Move moveAction;
    readonly Vector3 right = new Vector3(3.5f, 0, 0);
    readonly Vector3 left = new Vector3(-3.5f, 0, 0);
    readonly Vector3[] right_positions;
    readonly Vector3[] left_positions;

    int status;// 0: left, 1: right
    ICharacterController[] characterOnBoat = new ICharacterController[2];

    public BoatController()
    {
        status = 1;
        right_positions = new Vector3[] { new Vector3(2.5F, 0.2F, 0), new Vector3(4.5F, 0.2F, 0) };
        left_positions = new Vector3[] { new Vector3(-4.5F, 0.2F, 0), new Vector3(-2.5F, 0.2F, 0) };
        boat = Object.Instantiate(Resources.Load("Prefabs/Boat", typeof(GameObject)), right, Quaternion.identity, null) as GameObject;
        boat.name = "boat";
        moveAction = boat.AddComponent(typeof(Move)) as Move;
        boat.AddComponent(typeof(ClickGUI));
    }

    public void Move()
    {
        if (status == 1)
        {
            moveAction.moveTo(left);
        }
        else
        {
            moveAction.moveTo(right);
        }
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

    public bool isEmptty()
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

    public bool available()
    {
        return (moveAction.getStatus() == 0);
    }

    public void reset()
    {
        moveAction.reset();
        if (status == 0)
        {
            Move();
        }
        characterOnBoat = new ICharacterController[2];
    }
}
```
Boat的实现也差不多，只需要实现初始化、移动(ToggleBoat),character上船、下船相关的函数就可以实现了。

### LandController
LandController类的代码如下:

```C#
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
```
landController的实现和上面的Controller也类似，也是实现初始化，上岸，下岸的相关函数就完成了。

---
## 总结（对MVC架构开发游戏的体会）


MVC架构作为一种经典的开发模式，是关注点分离的一个十分经典的体现。我们在之前的课程中，在完成一些团队项目时也有使用其作为基本架构进行开发。

项目使用MVC架构分离的设计，**给每一个部分赋予单一的职责**，使得我们在开发一个模块时不用太多地关注所使用到的模块的内部实现，我们只需要去调用相应的接口去实现自己想实现的东西，**使得我们的游戏有着更低的耦合度，更方便Debug，更方便开发、迭代**，就以我们上面的程序为例，在开发FirstSceneController时，我们不用怎么关注GameObject的Controller内部是怎么实现的，我们只需要知道，它给了我们什么接口，这个接口有着什么功能，知道了之后我们在FirstSceneController中直接调用接口就可以了，这不仅让个人开发更有条理，还可以使多人协作开发更加便利，难以想象一个高耦合度的中大型软件是怎么进行多人协作开发的，这样的软件一个人开发都难受，何况是多人。

总之，在一些中大型项目中，使用MVC架构进行开发

* 软件耦合度低
* 模块易复用
* 生产效率高(对于中大型软件) 
* 易于维护、迭代
* 有利软件工程化管理

> 但是，MVC架构不是普适的，在一些较小规模的游戏中，严格使用MVC架构对于开发的效率是有一定影响的，就像我们这次的作业🌚(逃。不过，通过这一次严格使用MVC架构开发的作业，我们对于MVC有了基本的了解，方便了我们之后使用MVC架构对其他一些更大的游戏进行开发。


