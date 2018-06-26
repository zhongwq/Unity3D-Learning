using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSFactory : MonoBehaviour {
	private List<ParticleSystem> psContainer;
	private List<ParticleSystem> tankPsContainer;

	void Awake() {
		psContainer = new List<ParticleSystem>();
		tankPsContainer = new List<ParticleSystem> ();
	}
		
	public ParticleSystem getPs() {
		for (int i = 0; i < psContainer.Count; i++) {
			if (!psContainer[i].isPlaying) {
				return psContainer[i];
			}
		}
		GameObject explore = Instantiate (Resources.Load ("Prefabs/ShellExplosion")) as GameObject;
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
		ParticleSystem newPs = explore.GetComponent<ParticleSystem> ();
		tankPsContainer.Add(newPs);
		return newPs;
	}
}
