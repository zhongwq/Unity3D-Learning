using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum tankType:int {Player, Enemy}

public class AllFactory : MonoBehaviour {
	public GameObject player; //玩家坦克
	public GameObject bullet; //子弹
	public Transform imageTarget;

	private Dictionary<int, GameObject> usingTanks;
	private Dictionary<int, GameObject> freeTanks;
	private Dictionary<int, GameObject> usingBullets;
	private Dictionary<int, GameObject> freeBullets;

	private List<ParticleSystem> psContainer;
	private List<ParticleSystem> tankPsContainer;



	void Awake() {
		usingTanks = new Dictionary<int, GameObject>();
		freeTanks = new Dictionary<int, GameObject>();
		usingBullets = new Dictionary<int, GameObject>();
		freeBullets = new Dictionary<int, GameObject>();
		psContainer = new List<ParticleSystem>();
		tankPsContainer = new List<ParticleSystem> ();
	}

	void Start() {
	}

	public GameObject getPlayer() {//获取玩家坦克
		return player;
	}

	public GameObject getTank() { 
		if (freeTanks.Count == 0) {
			GameObject newTank = Instantiate (Resources.Load ("Prefabs/Enemy")) as GameObject;
			usingTanks.Add(newTank.GetInstanceID(), newTank);
			newTank.transform.parent = imageTarget;
			newTank.transform.localScale = new Vector3 (0.02f, 0.02f, 0.02f);
			newTank.transform.localPosition = new Vector3(((float)Random.Range(-4, 4))/10, 0, ((float)Random.Range(-4, 4))/10);
			return newTank;
		}
		foreach (KeyValuePair<int, GameObject> pair in freeTanks) {
			pair.Value.SetActive(true);
			freeTanks.Remove(pair.Key);
			usingTanks.Add(pair.Key, pair.Value);
			pair.Value.transform.parent = imageTarget;
			pair.Value.transform.localPosition = new Vector3(((float)Random.Range(-4, 4))/10, 0, ((float)Random.Range(-4, 4))/10);
			pair.Value.GetComponent<Enemy> ().init ();
			return pair.Value;
		}
		return null;
	}   

	public GameObject getBullet(tankType type) {
		if (freeBullets.Count == 0) {
			GameObject newBullet = Instantiate (Resources.Load ("Prefabs/Bullet")) as GameObject;
			newBullet.GetComponent<Bullet>().setTankType(type);
			usingBullets.Add(newBullet.GetInstanceID(), newBullet);
			newBullet.transform.parent = imageTarget;
			return newBullet;
		}
		foreach (KeyValuePair<int, GameObject> pair in freeBullets) {
			pair.Value.SetActive(true);
			pair.Value.GetComponent<Bullet>().setTankType(type);
			freeBullets.Remove(pair.Key);
			usingBullets.Add(pair.Key, pair.Value);
			pair.Value.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			return pair.Value;
		}
		return null;
	}

	public ParticleSystem getPs() {
		for (int i = 0; i < psContainer.Count; i++) {
			if (!psContainer[i].isPlaying) {
				return psContainer[i];
			}
		}
		GameObject explore = Instantiate (Resources.Load ("Prefabs/ShellExplosion")) as GameObject;
		explore.transform.parent = imageTarget;
		ParticleSystem newPs = explore.GetComponent<ParticleSystem> ();
		psContainer.Add(newPs);
		return newPs;
	}

	public ParticleSystem getTankPs() {
		for (int i = 0; i < tankPsContainer.Count; i++) {
			if (!tankPsContainer[i].isPlaying) {
				return tankPsContainer[i];
			}
		}
		GameObject explore = Instantiate (Resources.Load ("Prefabs/TankExplosion")) as GameObject;
		explore.transform.parent = imageTarget;
		ParticleSystem newPs = explore.GetComponent<ParticleSystem> ();
		tankPsContainer.Add(newPs);
		return newPs;
	}


	public void recycleTank(GameObject tank) {
		tank.SetActive (false);
		usingTanks.Remove(tank.GetInstanceID());
		freeTanks.Add(tank.GetInstanceID(), tank);
		ParticleSystem explosion = getTankPs (); //获取爆炸的粒子系统
		explosion.transform.position = tank.transform.position; //设置粒子系统位置
		explosion.Play();
	}

	public void recycleBullet(GameObject bullet) {
		usingBullets.Remove(bullet.GetInstanceID());
		freeBullets.Add(bullet.GetInstanceID(), bullet);
		bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
		bullet.SetActive(false);
	}

	public void recycleAllTanks() {
		List<int> keyList = new List<int>(usingTanks.Keys);
		foreach (int key in keyList) {
			if (usingTanks.ContainsKey (key)) {
				usingTanks [key].SetActive (false);
				freeTanks.Add(key, usingTanks [key]);
				usingTanks.Remove(key);
			}
		}
	}

	public void recycleAllBullets() {
		List<int> keyList = new List<int>(usingBullets.Keys);
		foreach (int key in keyList) {
			if (usingBullets.ContainsKey (key)) {
				usingBullets [key].SetActive (false);
				freeBullets.Add(key, usingBullets [key]);
				usingBullets.Remove(key);
			}
		}
	}

	public void reset() {
		recycleAllTanks ();
		recycleAllBullets ();
		player.GetComponent<Player> ().init ();
	}
}
