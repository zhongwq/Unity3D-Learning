# Homework5-adapter模式的打飞碟游戏

这周的任务主要就是通过给打飞碟游戏加上adapter模式让我们熟悉adapter模式，体会到Apapter模式给我们编程带来的作用，提升我们面向对象的设计技巧。

### 演示视频
视频演示网站: [Hit UFO!](http://v.youku.com/v_show/id_XMzU2MTk4NDE2MA==.html?spm=a2h3j.8428770.3416059.1) 
本周的游戏效果和上周差不多，就是多了一个右键点击鼠标切换动作管理器的功能。

### 完成的类
其实在上一周已完成作业的基础上，这一周并不用加什么特别的东西。只需要写出一个基本的ActionManagerTarget的接口(类似上图中的IActionManager), 然后实现一个供场景控制器调用的ActionManagerAdapter,然后实现我们这次需要实现的PhysicsActionManager, 把原来调用FirstSceneActionController的地方换成调用我们新实现的ActionManagerAdapter，那么这周的作业就完成了。下面我们分类来说说

#### ActionManagerTarget
这个接口里声明的就是我们动作管理器所需的基本函数，以及一个切换动作管理器的功能。通过使用这个统一的接口，我们可以在多个动作管理器之间无缝切换.

```C#
public interface ActionManagerTarget 
{
	int getMode();

	void switchActionManager();

	void addAction (GameObject ufo, float speed);

	void removeActionOf (GameObject ufo);
}

```


#### ActionManagerAdapter
这里就是对于我们声明的一个接口的实现，通过实现这个接口以供场景控制器调用，我们给游戏加上的Adapter模式的工作就完成一大半了，剩下的基本就是完成PhysicsActionManager了。

```C#
public class ActionManagerAdapter : ActionManagerTarget 
{
	FirstSceneActionManager firstSceneActionManager;
	PhysicsActionManager physicsActionManager;

	int mode = 0; // 0: firstSceneActionManager, 1: physicsActionManager

	public int getMode() 
	{
		return mode;
	}

	public void switchActionManager() 
	{
		mode = 1 - mode;
	}

	public ActionManagerAdapter(GameObject main) 
	{
		firstSceneActionManager = main.AddComponent < FirstSceneActionManager> ();
		physicsActionManager = main.AddComponent<PhysicsActionManager> ();
		mode = 0;
	}

	public void addAction(GameObject ufo, float speed) 
	{
		if (mode == 0)
		{
			firstSceneActionManager.addActionToUFO (ufo, speed);
		}
		else
		{
			physicsActionManager.addForceToObj (ufo, speed);
		}
	}

	public void removeActionOf(GameObject ufo) 
	{
		if (mode == 0) 
		{
			firstSceneActionManager.removeActionByObj (ufo);
		}
		else
		{
			physicsActionManager.removeForceOfObj (ufo);
		}
	}
}
```


#### PhysicsActionManage
在这里因为给我们的ufo加上了刚体，我们可以直接通过加上力、初速度，去掉力的方式，给动作加上对应的动作，设速度为0的操作实现动作的管理。

```C#
public class PhysicsActionManager : MonoBehaviour {
	public void addForceToObj (GameObject ufo, float speed) 
	{
		int position = (Random.Range (1, 3) < 2) ? -1 : 1;
		ufo.transform.position = new Vector3 (-10 * position, Random.Range (4, 6), 5);
		Vector3 speedVector = new Vector3 (Random.Range ((int)speed, (int)(1.5 * speed)) * position, Random.Range ((int)speed, (int)(1.2 * speed)), 0);
		ufo.GetComponent<Rigidbody> ().useGravity = true;
		ufo.GetComponent<Rigidbody> ().velocity = speedVector;
	}

	public void removeForceOfObj (GameObject ufo)
	{
	   ufo.GetComponent<Rigidbody> ().useGravity = false;
	   ufo.GetComponent<Rigidbody> ().velocity = Vector3.zero;
	}

}
```

实现了这些类之后，加上其他代码的一些微小的修整，即一些地方对于切换场景控制器功能的调用的设置(游戏场景内右键点击)以及一些加入了新的PhysicsActionManager后出了一些小Bug的修复和加了一些游戏的彩蛋(飞碟相撞会💥，你打得不快的话有时就打不到了，哈哈)，我们就完成这周的作业了，这周的这个作业相对前几周其实任务量并不大，所以，继续肝射箭小游戏去啦🌚!


