using System;
using UnityEngine;

public class Occludee : MonoBehaviour
{
	public float minTimeVisible = 0.1f;

	public bool isStatic = true;

	public bool autoRegister;

	public bool stickyGizmos;

	public OccludeeState state;

	protected int occludeeId = -1;

	protected Vector3 center;

	protected float radius;

	protected Renderer renderer;

	protected Collider collider;

	public Occludee()
	{
	}

	protected virtual void Awake()
	{
		this.renderer = base.GetComponent<Renderer>();
		this.collider = base.GetComponent<Collider>();
	}

	public void OnDisable()
	{
		if (this.autoRegister && this.occludeeId >= 0)
		{
			this.Unregister();
		}
	}

	public void OnEnable()
	{
		if (this.autoRegister && this.collider != null)
		{
			this.Register();
		}
	}

	protected virtual void OnVisibilityChanged(bool visible)
	{
		if (this.renderer != null)
		{
			this.renderer.enabled = visible;
		}
	}

	public void Register()
	{
		Bounds bound = this.collider.bounds;
		this.center = bound.center;
		bound = this.collider.bounds;
		float single = bound.extents.x;
		bound = this.collider.bounds;
		float single1 = Mathf.Max(single, bound.extents.y);
		bound = this.collider.bounds;
		this.radius = Mathf.Max(single1, bound.extents.z);
		Occludee occludee = this;
		this.occludeeId = OcclusionCulling.RegisterOccludee(this.center, this.radius, this.renderer.enabled, this.minTimeVisible, this.isStatic, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(occludee.OnVisibilityChanged));
		if (this.occludeeId < 0)
		{
			Debug.LogWarning(string.Concat("[OcclusionCulling] Occludee registration failed for ", base.name, ". Too many registered."));
		}
		this.state = OcclusionCulling.GetStateById(this.occludeeId);
	}

	public void Unregister()
	{
		OcclusionCulling.UnregisterOccludee(this.occludeeId);
	}
}