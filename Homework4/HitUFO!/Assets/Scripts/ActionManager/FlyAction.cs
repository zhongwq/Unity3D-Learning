using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyAction : SSAction {
	public float Vx;
	public float Vy;
	public Vector3 speed;
	public Vector3 gravity;

	private FlyAction() 
	{
	}

	public static FlyAction getAction(float vx, float vy) 
	{
		FlyAction action = ScriptableObject.CreateInstance<FlyAction> ();
		action.gravity = Vector3.zero;
		action.Vx = vx;
		action.Vy = vy;
		return action;
	}

	// Use this for initialization
	public override void Start () 
	{
		speed = new Vector3 (Vx, Vy, 0);
	}
	
	// Update is called once per frame
	public override void Update () 
	{
		if (2 * Vy + gravity.y > 0.00001) 
		{
			gravity.y -= 10 * Time.fixedDeltaTime;
			transform.Translate (speed * Time.fixedDeltaTime);
			transform.Translate (gravity * Time.fixedDeltaTime);
		}
		else 
		{
			destroy = true;
			callback.actionDone (this);
		}
	}
}
