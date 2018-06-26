using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Tank : NetworkBehaviour {
	GUIStyle healthStyle;
	GUIStyle backStyle;

	public const float maxHp = 100;

	[SyncVar]
	public float hp = maxHp;

	void Update () {
		if (getHp() <= 0 && gameObject.activeSelf) {
			hp = maxHp;
			RpcRespawn();
		}
	}

	public float getHp() {
		return hp;
	}

	public void setHp(float hp) {
		if (!isServer)
			return;
		this.hp = hp; 
	}

	void OnGUI()
	{
		InitStyles();

		// Draw a Health Bar

		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

		// draw health bar background
		GUI.color = Color.grey;
		GUI.backgroundColor = Color.grey;
		GUI.Box(new Rect(pos.x-26, Screen.height - pos.y + 20, 45, 7), ".", backStyle);

		// draw health bar amount
		GUI.color = Color.green;
		GUI.backgroundColor = Color.green;
		GUI.Box(new Rect(pos.x-25, Screen.height - pos.y + 21, hp/maxHp * 45, 5), ".", healthStyle);
	}

	void InitStyles()
	{
		if( healthStyle == null )
		{
			healthStyle = new GUIStyle( GUI.skin.box );
			healthStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 1f, 0f, 1.0f ) );
		}

		if( backStyle == null )
		{
			backStyle = new GUIStyle( GUI.skin.box );
			backStyle.normal.background = MakeTex( 2, 2, new Color( 0f, 0f, 0f, 1.0f ) );
		}
	}

	Texture2D MakeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}

	[ClientRpc]
	void RpcRespawn() {
		ParticleSystem explosion = Singleton<PSFactory>.Instance.getTankPs ();
		explosion.transform.position = transform.position; //设置粒子系统位置
		explosion.Play();
		if (isLocalPlayer) {
			// move back to zero location
			transform.position = Vector3.zero;
		}
	}
}