using UnityEngine;
using System.Collections;

public class FPSControl : MonoBehaviour
{
	public enum RotationAxes
	{
		MouseXAndY = 0,
		MouseX = 1,
		MouseY = 2
	}

	public RotationAxes axes = RotationAxes.MouseXAndY;
	public float sensitivityHor = 8.0f;//水平旋转的速度
	public float sensitivityVert = 8.0f;//垂直旋转的速度
	public float minimumVert = -90.0f;//垂直旋转的最小角度
	public float maximumVert = 90.0f;//垂直旋转的最大角度

	private float _rotationX = 0;//为垂直角度声明一个私有变量

	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update()
	{
		if (axes == RotationAxes.MouseX)
		{
			//水平旋转的操作
			transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityHor, 0);
		}
		else if (axes == RotationAxes.MouseY)
		{
			//垂直旋转的操作
			_rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
			_rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);

			float rotationY = transform.localEulerAngles.y;
			transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);
		}
		else
		{
			//水平且垂直旋转的操作
			_rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
			_rotationX = Mathf.Clamp(_rotationX, minimumVert, maximumVert);//限制角度大小

			float delta = Input.GetAxis("Mouse X") * sensitivityHor;//设置水平旋转的变化量
			float rotationY = transform.localEulerAngles.y + delta;//原来的角度加上变化量
			transform.localEulerAngles = new Vector3(_rotationX, rotationY, 0);//相对于全局坐标空间的角度
		}

		//当点击esc后，解除CursorLockMode
		if (Input.GetKeyDown("escape"))
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
		if (Input.GetButtonDown("Fire1"))
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}