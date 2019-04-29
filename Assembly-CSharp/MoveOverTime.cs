using System;
using UnityEngine;

public class MoveOverTime : MonoBehaviour
{
	[Range(-10f, 10f)]
	public float speed = 1f;

	public Vector3 position;

	public Vector3 rotation;

	public Vector3 scale;

	public MoveOverTime()
	{
	}

	private void Update()
	{
		Transform transforms = base.transform;
		Quaternion quaternion = base.transform.rotation;
		transforms.rotation = Quaternion.Euler(quaternion.eulerAngles + ((this.rotation * this.speed) * Time.deltaTime));
		Transform transforms1 = base.transform;
		transforms1.localScale = transforms1.localScale + ((this.scale * this.speed) * Time.deltaTime);
		Transform transforms2 = base.transform;
		transforms2.localPosition = transforms2.localPosition + ((this.position * this.speed) * Time.deltaTime);
	}
}