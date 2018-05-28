using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingOfParticle : MonoBehaviour {
	public class particlePos {
		public float r = 0.0f; //初始化半径
		public float combine_r = 0.0f; //集合后的半径
		public float cur_r = 0.0f; //记录粒子当前时刻半径
		public float angle = 0.0f;

		public particlePos(float radiu, float angle, float combine) {
			r = radiu;
			this.angle = angle;
			combine_r = combine;
			cur_r = radiu;
		}
	}
		
	public ParticleSystem particleSystem;
	public int particleNum = 10000;
	public float outMinR = 5.0f;
	public float outMaxR = 10.0f;
	public float inMinR = 6.0f;
	public float inMaxR = 9.0f;

	public float speed = 0.1f;

	public int flag;


	private ParticleSystem.Particle[] particleArray;
	private particlePos[] posArray;

	void OnGUI() {
		if (GUI.Button(new Rect(0, 15, 100, 30), "Change")) {
			flag = (flag == -1)? 0: 1 - flag;
		}
	}

	// Use this for initialization
	void Start () {
		flag = -1;
		posArray = new particlePos[particleNum];
		particleArray = new ParticleSystem.Particle[particleNum];
		particleSystem.maxParticles = particleNum;
		particleSystem.Emit(particleNum);
		particleSystem.GetParticles(particleArray);

		for (int i = 0; i < particleNum; i++) {   
			float randomAngle; // 设置粒子的随机角度
			float maxR, minR; // 最大最小半径供随机

			if(i < particleNum * 1 / 2) { //将前半部分用于较宽的那个环 
				maxR = outMaxR;
				minR = outMinR;
				randomAngle = Random.Range(0.0f, 360.0f);
			} else { //后半部分用于窄环,根据I Remember,窄环带缺口，因此我们设置一半向0度集中、一半向180度集中，使得90度和-90度形成两个对称缺口
				maxR = inMaxR;
				minR = inMinR;
				float minAngle = Random.Range(-90f, 0.0f);
				float maxAngle = Random.Range(0.0f, 90f);
				float angle = Random.Range(minAngle, maxAngle);

				randomAngle = i % 2 == 0 ? angle : angle - 180; //利用对称来设置另一半粒子
			}

			float midRadius = (maxR + minR) / 2;
			float min = Random.Range(minR, midRadius);
			float max = Random.Range(midRadius, maxR);
			float randomRadius = Random.Range(min, max);
			float combineRadius;

			// 平均半径以外的粒子集合半径调小，使缩小时移动的距离少一些
			if (randomRadius > midRadius)
				combineRadius = randomRadius - (randomRadius - midRadius) / 2;
			else
				combineRadius = randomRadius - (randomRadius - midRadius) * 3 / 4;

			// 设置粒子的属性
			posArray[i] = new particlePos(randomRadius, randomAngle, combineRadius);
			particleArray[i].position = new Vector3(randomRadius * Mathf.Cos(randomAngle), randomRadius * Mathf.Sin(randomAngle), 0.0f);
		}
		particleSystem.SetParticles(particleArray, particleNum);
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < particleNum; i++)
		{
			// 小圈转动速度更快
			if (i > particleNum * 1 / 2)
				speed = 0.1f;
			else
				speed = 0.05f;
			posArray[i].angle -=  speed;
			posArray[i].angle = posArray[i].angle % 360;
			float rad = posArray[i].angle / 180 * Mathf.PI;

			// 根据flag, 判断粒子的动态，改变粒子的现在的半径
			if(flag == 0) { //粒子向中间收缩 
				if (posArray[i].cur_r > posArray[i].combine_r + 0.05f) {
					//两层环的收缩速度不同
					if(i < particleNum * 1 / 2)
						posArray[i].cur_r -=  2.0f * Time.deltaTime;
					else
						posArray[i].cur_r -= Time.deltaTime;
				} else if(posArray[i].cur_r < posArray[i].combine_r - 0.05f) {
					if (i < particleNum * 1 / 2)
						posArray[i].cur_r += 2.0f * Time.deltaTime;
					else
						posArray[i].cur_r += Time.deltaTime;
				}
			} else if(flag == 1) { //粒子范围扩大 
				if (posArray[i].cur_r < posArray[i].r - 0.05f) {
					if (i < particleNum * 1 / 2)
						posArray[i].cur_r += 2.0f * Time.deltaTime;
					else
						posArray[i].cur_r += Time.deltaTime;
				} else if (posArray[i].cur_r > posArray[i].r + 0.05f) {
					if (i < particleNum * 1 / 2)
						posArray[i].cur_r -= 2.0f* Time.deltaTime;
					else
						posArray[i].cur_r -= Time.deltaTime;
				}
			}

			// 通过curR和新的角度设置粒子的位置
			particleArray[i].position = new Vector3(posArray[i].cur_r * Mathf.Cos(rad), posArray[i].cur_r * Mathf.Sin(rad), 0f);
		}
		particleSystem.SetParticles(particleArray, particleNum);
	}
}
