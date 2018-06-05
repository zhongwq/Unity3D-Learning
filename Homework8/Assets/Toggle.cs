using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Toggle : MonoBehaviour {
	private Button yourButton;  
	public Text text;  
	private Animator animator;

	// Use this for initialization  
	void Start() {  
		Button btn = this.gameObject.GetComponent<Button>();  
		animator = text.GetComponent<Animator> ();
		btn.onClick.AddListener(ClickBtn);  
	}  

	void ClickBtn() {  
		if (animator.GetBool ("State")) {
			animator.SetTrigger ("ToggleIn");
			animator.SetBool ("State", false);
		} else {
			animator.SetTrigger ("ToggleOut");
			animator.SetBool ("State", true);
		}
	}
}
