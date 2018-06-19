using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Player : Tank {
	public void init() {
		this.gameObject.SetActive (true);
		setHp(300f);
	}

	void Start() {
		setHp(300f);
	}

	void Update () {
		if (getHp() <= 0 && gameObject.activeSelf) {
			this.gameObject.SetActive(false);
			ParticleSystem explosion = Singleton<AllFactory>.Instance.getTankPs (); //获取爆炸的粒子系统
			explosion.transform.position = transform.position; //设置粒子系统位置
			explosion.Play();
			((FirstSceneController)GameDirector.getInstance ().currentSceneController).gameOver = true;
		}
	}
}