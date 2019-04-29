using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class EnvironmentVolume : MonoBehaviour
{
	public bool StickyGizmos;

	public EnvironmentType Type = EnvironmentType.Underground;

	public Vector3 Center = Vector3.zero;

	public Vector3 Size = Vector3.one;

	public BoxCollider trigger
	{
		get;
		private set;
	}

	public EnvironmentVolume()
	{
	}

	protected virtual void Awake()
	{
		this.UpdateTrigger();
	}

	private void DrawGizmos()
	{
		Vector3 vector3 = base.transform.lossyScale;
		Quaternion quaternion = base.transform.rotation;
		Vector3 vector31 = base.transform.position + (quaternion * Vector3.Scale(vector3, this.Center));
		Vector3 vector32 = Vector3.Scale(vector3, this.Size);
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		GizmosUtil.DrawCube(vector31, vector32, quaternion);
		GizmosUtil.DrawWireCube(vector31, vector32, quaternion);
	}

	protected void OnDrawGizmos()
	{
		if (this.StickyGizmos)
		{
			this.DrawGizmos();
		}
	}

	protected void OnDrawGizmosSelected()
	{
		if (!this.StickyGizmos)
		{
			this.DrawGizmos();
		}
	}

	public void UpdateTrigger()
	{
		if (!this.trigger)
		{
			this.trigger = base.gameObject.GetComponent<BoxCollider>();
		}
		if (!this.trigger)
		{
			this.trigger = base.gameObject.AddComponent<BoxCollider>();
		}
		this.trigger.isTrigger = true;
		this.trigger.center = this.Center;
		this.trigger.size = this.Size;
	}
}