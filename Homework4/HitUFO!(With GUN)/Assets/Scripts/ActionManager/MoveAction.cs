using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : SSAction
{
	public Vector3 target;
	public float speed;

	private MoveAction()
	{
	}

	public static MoveAction getAction(Vector3 target, float speed)
	{
		MoveAction action = ScriptableObject.CreateInstance<MoveAction>();
		action.target = target;
		action.speed = speed;
		return action;
	}

	// Use this for initialization
	public override void Start()
	{

	}

	// Update is called once per frame
	public override void Update()
	{
		transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
		if (transform.transform.position == target)
		{
			destroy = true;
			callback.actionDone(this);
		}
	}
}
