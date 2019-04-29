using System;
using UnityEngine;

public struct OccludeeSphere
{
	public int id;

	public OccludeeState state;

	public OcclusionCulling.Sphere sphere;

	public bool IsRegistered
	{
		get
		{
			return this.id >= 0;
		}
	}

	public OccludeeSphere(int id)
	{
		OccludeeState stateById;
		this.id = id;
		if (id < 0)
		{
			stateById = null;
		}
		else
		{
			stateById = OcclusionCulling.GetStateById(id);
		}
		this.state = stateById;
		this.sphere = new OcclusionCulling.Sphere(Vector3.zero, 0f);
	}

	public OccludeeSphere(int id, OcclusionCulling.Sphere sphere)
	{
		OccludeeState stateById;
		this.id = id;
		if (id < 0)
		{
			stateById = null;
		}
		else
		{
			stateById = OcclusionCulling.GetStateById(id);
		}
		this.state = stateById;
		this.sphere = sphere;
	}

	public void Invalidate()
	{
		this.id = -1;
		this.state = null;
		this.sphere = new OcclusionCulling.Sphere();
	}
}