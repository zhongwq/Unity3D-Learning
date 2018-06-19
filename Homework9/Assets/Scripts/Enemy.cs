using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Tank {
	private Vector3 target;

	private bool gameover; //根据FirstSceneController中的gameOver，判断游戏是否结束
	private AllFactory allFactory;

	public void init() {
		setHp (100f);//设置初始生命值  
		StopAllCoroutines ();//停止所有协程
		StartCoroutine(shoot());//开始射击的协程
	}

	void Start() {
		setHp(100f);//设置初始生命值  
		allFactory = Singleton<AllFactory>.Instance;
		StartCoroutine(shoot());//开始射击的协程
	}

	void Update () {
		gameover = ((FirstSceneController)GameDirector.getInstance ().currentSceneController).gameOver;
		if (!gameover) {
			target = ((FirstSceneController)GameDirector.getInstance().currentSceneController).getPlayerPos();  // 感知     
			if (getHp() <= 0 && gameObject.activeSelf) { //思考
				allFactory.recycleTank (this.gameObject);
				((FirstSceneController)GameDirector.getInstance ().currentSceneController).GetScore ();
				StopCoroutine(shoot());
			} else {
				NavMeshAgent agent = GetComponent<NavMeshAgent>();
				agent.SetDestination(target);
			}
		} else {
			NavMeshAgent agent = GetComponent<NavMeshAgent>();
			agent.velocity = Vector3.zero;
			agent.ResetPath();
		}

	}

	IEnumerator shoot() { // 行为
		while(!gameover && getHp() > 0) {
			for (float i = 2; i > 0; i -= Time.deltaTime) {
				yield return 0; 
			}
			if (Vector3.Distance(transform.position, target) < 15) { //距离判断是否攻击
				GameObject bullet = allFactory.getBullet(tankType.Enemy);
				bullet.transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z) +
					transform.forward * 1.5f;
				bullet.transform.forward = transform.forward;
				Rigidbody rb = bullet.GetComponent<Rigidbody>();
				rb.AddForce(bullet.transform.forward * 20, ForceMode.Impulse);
			}
		}
	}
}
