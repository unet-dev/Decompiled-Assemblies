using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SkinnedMeshRendererInfo : ComponentInfo<SkinnedMeshRenderer>
{
	public ShadowCastingMode shadows;

	public Material material;

	public Mesh mesh;

	public Bounds bounds;

	public Mesh cachedMesh;

	public SkinnedMeshRendererCache.RigInfo cachedRig;

	private Transform root;

	private Transform[] bones;

	public SkinnedMeshRendererInfo()
	{
	}

	public void BuildRig()
	{
		this.RefreshCache();
		Vector3 vector3 = base.transform.position;
		Quaternion quaternion = base.transform.rotation;
		base.transform.rotation = Quaternion.identity;
		base.transform.position = Vector3.zero;
		Transform[] transformArrays = new Transform[(int)this.cachedRig.transforms.Length];
		for (int i = 0; i < (int)this.cachedRig.transforms.Length; i++)
		{
			GameObject gameObject = new GameObject(this.cachedRig.bones[i]);
			gameObject.transform.position = this.cachedRig.transforms[i].MultiplyPoint(Vector3.zero);
			gameObject.transform.rotation = Quaternion.LookRotation(this.cachedRig.transforms[i].GetColumn(2), this.cachedRig.transforms[i].GetColumn(1));
			gameObject.transform.SetParent(base.transform, true);
			transformArrays[i] = gameObject.transform;
		}
		GameObject gameObject1 = new GameObject("root");
		gameObject1.transform.position = this.cachedRig.rootTransform.MultiplyPoint(Vector3.zero);
		gameObject1.transform.rotation = Quaternion.LookRotation(this.cachedRig.rootTransform.GetColumn(2), this.cachedRig.rootTransform.GetColumn(1));
		gameObject1.transform.SetParent(base.transform, true);
		this.component.rootBone = gameObject1.transform;
		this.component.bones = transformArrays;
		base.transform.rotation = quaternion;
		base.transform.position = vector3;
	}

	private void RefreshCache()
	{
		if (this.cachedMesh != this.component.sharedMesh)
		{
			if (this.cachedMesh != null)
			{
				SkinnedMeshRendererCache.Add(this.cachedMesh, this.cachedRig);
			}
			this.cachedMesh = this.component.sharedMesh;
			this.cachedRig = SkinnedMeshRendererCache.Get(this.component);
		}
	}

	public override void Reset()
	{
		if (this.component == null)
		{
			return;
		}
		this.component.shadowCastingMode = this.shadows;
		if (this.material)
		{
			this.component.sharedMaterial = this.material;
		}
		this.component.sharedMesh = this.mesh;
		this.component.localBounds = this.bounds;
	}

	public override void Setup()
	{
		this.shadows = this.component.shadowCastingMode;
		this.material = this.component.sharedMaterial;
		this.mesh = this.component.sharedMesh;
		this.bounds = this.component.localBounds;
		this.RefreshCache();
	}
}