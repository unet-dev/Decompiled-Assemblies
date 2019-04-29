using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ArticulatedOccludee : BaseMonoBehaviour
{
	private const float UpdateBoundsFadeStart = 20f;

	private const float UpdateBoundsFadeLength = 1000f;

	private const float UpdateBoundsMaxFrequency = 15f;

	private const float UpdateBoundsMinFrequency = 0.5f;

	private LODGroup lodGroup;

	public List<Collider> colliders = new List<Collider>();

	private OccludeeSphere localOccludee = new OccludeeSphere(-1);

	private List<Renderer> renderers = new List<Renderer>();

	private bool isVisible = true;

	private Action TriggerUpdateVisibilityBoundsDelegate;

	public bool IsVisible
	{
		get
		{
			return this.isVisible;
		}
	}

	public ArticulatedOccludee()
	{
	}

	private void ApplyVisibility(bool vis)
	{
		object obj;
		if (this.lodGroup != null)
		{
			if (vis)
			{
				obj = null;
			}
			else
			{
				obj = 100000;
			}
			float single = (float)obj;
			if (single != this.lodGroup.localReferencePoint.x)
			{
				this.lodGroup.localReferencePoint = new Vector3(single, single, single);
			}
		}
	}

	protected virtual bool CheckVisibility()
	{
		if (this.localOccludee.state == null)
		{
			return true;
		}
		return this.localOccludee.state.isVisible;
	}

	public void ClearVisibility()
	{
		if (this.lodGroup != null)
		{
			this.lodGroup.localReferencePoint = Vector3.zero;
			this.lodGroup.RecalculateBounds();
			this.lodGroup = null;
		}
		if (this.renderers != null)
		{
			this.renderers.Clear();
		}
		this.localOccludee = new OccludeeSphere(-1);
	}

	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.UnregisterFromCulling();
		this.ClearVisibility();
	}

	protected virtual void OnVisibilityChanged(bool visible)
	{
		if (MainCamera.mainCamera != null && this.localOccludee.IsRegistered)
		{
			float single = Vector3.Distance(MainCamera.mainCamera.transform.position, base.transform.position);
			this.VisUpdateUsingCulling(single, visible);
			this.ApplyVisibility(this.isVisible);
		}
	}

	public void ProcessVisibility(LODGroup lod)
	{
		this.lodGroup = lod;
		if (lod != null)
		{
			this.renderers = new List<Renderer>(16);
			LOD[] lODs = lod.GetLODs();
			for (int i = 0; i < (int)lODs.Length; i++)
			{
				Renderer[] rendererArray = lODs[i].renderers;
				for (int j = 0; j < (int)rendererArray.Length; j++)
				{
					Renderer renderer = rendererArray[j];
					if (renderer != null)
					{
						this.renderers.Add(renderer);
					}
				}
			}
		}
		this.UpdateCullingBounds();
	}

	private void RegisterForCulling(OcclusionCulling.Sphere sphere, bool visible)
	{
		if (this.localOccludee.IsRegistered)
		{
			this.UnregisterFromCulling();
		}
		ArticulatedOccludee articulatedOccludee = this;
		int num = OcclusionCulling.RegisterOccludee(sphere.position, sphere.radius, visible, 0.25f, false, base.gameObject.layer, new OcclusionCulling.OnVisibilityChanged(articulatedOccludee.OnVisibilityChanged));
		if (num >= 0)
		{
			this.localOccludee = new OccludeeSphere(num, this.localOccludee.sphere);
			return;
		}
		this.localOccludee.Invalidate();
		Debug.LogWarning(string.Concat("[OcclusionCulling] Occludee registration failed for ", base.name, ". Too many registered."));
	}

	public virtual void TriggerUpdateVisibilityBounds()
	{
		float single;
		if (base.enabled)
		{
			Vector3 vector3 = base.transform.position - MainCamera.mainCamera.transform.position;
			float single1 = vector3.sqrMagnitude;
			if (single1 >= 400f)
			{
				float single2 = Mathf.Clamp01((Mathf.Sqrt(single1) - 20f) * 0.001f);
				float single3 = Mathf.Lerp(0.06666667f, 2f, single2);
				single = UnityEngine.Random.Range(single3, single3 + 0.06666667f);
			}
			else
			{
				single = 1f / UnityEngine.Random.Range(5f, 25f);
			}
			this.UpdateVisibility(single);
			this.ApplyVisibility(this.isVisible);
			if (this.TriggerUpdateVisibilityBoundsDelegate == null)
			{
				ArticulatedOccludee articulatedOccludee = this;
				this.TriggerUpdateVisibilityBoundsDelegate = new Action(articulatedOccludee.TriggerUpdateVisibilityBounds);
			}
			base.Invoke(this.TriggerUpdateVisibilityBoundsDelegate, single);
		}
	}

	private void UnregisterFromCulling()
	{
		if (this.localOccludee.IsRegistered)
		{
			OcclusionCulling.UnregisterOccludee(this.localOccludee.id);
			this.localOccludee.Invalidate();
		}
	}

	public void UpdateCullingBounds()
	{
		Vector3 vector3 = Vector3.zero;
		Vector3 vector31 = Vector3.zero;
		bool flag = false;
		int num = (this.renderers != null ? this.renderers.Count : 0);
		int num1 = (this.colliders != null ? this.colliders.Count : 0);
		if (num > 0 && (num1 == 0 || num < num1))
		{
			for (int i = 0; i < this.renderers.Count; i++)
			{
				if (this.renderers[i].isVisible)
				{
					Bounds item = this.renderers[i].bounds;
					Vector3 vector32 = item.min;
					Vector3 vector33 = item.max;
					if (flag)
					{
						vector3.x = (vector3.x < vector32.x ? vector3.x : vector32.x);
						vector3.y = (vector3.y < vector32.y ? vector3.y : vector32.y);
						vector3.z = (vector3.z < vector32.z ? vector3.z : vector32.z);
						vector31.x = (vector31.x > vector33.x ? vector31.x : vector33.x);
						vector31.y = (vector31.y > vector33.y ? vector31.y : vector33.y);
						vector31.z = (vector31.z > vector33.z ? vector31.z : vector33.z);
					}
					else
					{
						vector3 = vector32;
						vector31 = vector33;
						flag = true;
					}
				}
			}
		}
		if (!flag && num1 > 0)
		{
			flag = true;
			Bounds bound = this.colliders[0].bounds;
			vector3 = bound.min;
			bound = this.colliders[0].bounds;
			vector31 = bound.max;
			for (int j = 1; j < this.colliders.Count; j++)
			{
				Bounds item1 = this.colliders[j].bounds;
				Vector3 vector34 = item1.min;
				Vector3 vector35 = item1.max;
				vector3.x = (vector3.x < vector34.x ? vector3.x : vector34.x);
				vector3.y = (vector3.y < vector34.y ? vector3.y : vector34.y);
				vector3.z = (vector3.z < vector34.z ? vector3.z : vector34.z);
				vector31.x = (vector31.x > vector35.x ? vector31.x : vector35.x);
				vector31.y = (vector31.y > vector35.y ? vector31.y : vector35.y);
				vector31.z = (vector31.z > vector35.z ? vector31.z : vector35.z);
			}
		}
		if (flag)
		{
			Vector3 vector36 = vector31 - vector3;
			Vector3 vector37 = vector3 + (vector36 * 0.5f);
			float single = Mathf.Max(Mathf.Max(vector36.x, vector36.y), vector36.z) * 0.5f;
			OcclusionCulling.Sphere sphere = new OcclusionCulling.Sphere(vector37, single);
			if (this.localOccludee.IsRegistered)
			{
				OcclusionCulling.UpdateDynamicOccludee(this.localOccludee.id, sphere.position, sphere.radius);
				this.localOccludee.sphere = sphere;
				return;
			}
			bool flag1 = true;
			if (this.lodGroup != null)
			{
				flag1 = this.lodGroup.enabled;
			}
			this.RegisterForCulling(sphere, flag1);
		}
	}

	private void UpdateVisibility(float delay)
	{
	}

	private void VisUpdateUsingCulling(float dist, bool visibility)
	{
	}
}