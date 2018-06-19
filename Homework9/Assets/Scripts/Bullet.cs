using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	public float explosionRadius = 3f; //子弹的伤害半径
	private tankType type; //发射子弹的坦克类型

	void Update () {
		if (Mathf.Abs (transform.position.x) > 26 || Mathf.Abs (transform.position.z) > 24) {
			Singleton<AllFactory>.Instance.recycleBullet(this.gameObject); //超出距离回收子弹
		}
	}

	void OnCollisionEnter(Collision other) {
		bool flag = false;
		AllFactory allFactory = Singleton<AllFactory>.Instance;
		ParticleSystem explosion = allFactory.getPs(); //获取爆炸的粒子系统
		explosion.transform.position = transform.position; //设置粒子系统位置
		Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius); // 获取范围内碰撞体
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].tag == "Player" && this.type == tankType.Enemy 
				|| colliders[i].tag == "Enemy" && this.type == tankType.Player) {//对同类坦克不产生对应效果
				float distance = Vector3.Distance(colliders[i].transform.position, transform.position);//取击中坦克与爆炸中心的距离
				float hurt = 100f / distance; //伤害根据距离变化
				float current = colliders[i].GetComponent<Tank>().getHp(); 
				colliders[i].GetComponent<Tank>().setHp(current - hurt); //计算hp
				flag = true;
			}
			if (colliders [i].tag == "Building")
				flag = true; 
		}
		if (flag == true && this.gameObject.activeSelf) {
			explosion.Play ();
			allFactory.recycleBullet(this.gameObject); //爆炸后回收子弹
		}
	}

	public void setTankType(tankType type) {
		this.type = type;
	}
}
