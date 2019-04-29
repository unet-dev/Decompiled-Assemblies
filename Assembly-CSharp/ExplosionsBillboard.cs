using System;
using UnityEngine;

public class ExplosionsBillboard : MonoBehaviour
{
	public UnityEngine.Camera Camera;

	public bool Active = true;

	public bool AutoInitCamera = true;

	private GameObject myContainer;

	private Transform t;

	private Transform camT;

	private Transform contT;

	public ExplosionsBillboard()
	{
	}

	private void Awake()
	{
		if (this.AutoInitCamera)
		{
			this.Camera = UnityEngine.Camera.main;
			this.Active = true;
		}
		this.t = base.transform;
		Vector3 vector3 = this.t.parent.transform.localScale;
		vector3.z = vector3.x;
		this.t.parent.transform.localScale = vector3;
		this.camT = this.Camera.transform;
		Transform transforms = this.t.parent;
		this.myContainer = new GameObject()
		{
			name = string.Concat("Billboard_", this.t.gameObject.name)
		};
		this.contT = this.myContainer.transform;
		this.contT.position = this.t.position;
		this.t.parent = this.myContainer.transform;
		this.contT.parent = transforms;
	}

	private void Update()
	{
		if (this.Active)
		{
			this.contT.LookAt(this.contT.position + (this.camT.rotation * Vector3.back), this.camT.rotation * Vector3.up);
		}
	}
}