using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {
	public float explosionRadius = 3f; //子弹的伤害半径
	public GameObject explosionPrefab;

	[SyncVar]
	private int fromId;

	void Update () {
		if (Mathf.Abs (transform.position.x) > 40 || Mathf.Abs (transform.position.z) > 40) {
			Destroy (this.gameObject); //超出距离回收子弹
		}
	}

	void OnCollisionEnter(Collision other) {
		bool flag = false;
		Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius); // 获取范围内碰撞体
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders [i].tag == "Player" && !(colliders [i].GetComponent<PlayerMove>().identity == fromId)) {
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
			if (isServer)
				RpcBulletExplosion ();
		}
	}

	public void setFromID(int id) {
		fromId = id;
	}

	[ClientRpc]
	void RpcBulletExplosion() {
		ParticleSystem explosion = Singleton<PSFactory>.Instance.getPs ();
		explosion.transform.position = transform.position; //设置粒子系统位置
		explosion.Play();
		Destroy(this.gameObject);
	}
}
