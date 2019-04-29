using System;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshRendererInfo : ComponentInfo<MeshRenderer>
{
	public ShadowCastingMode shadows;

	public Material material;

	public Mesh mesh;

	public MeshRendererInfo()
	{
	}

	public override void Reset()
	{
		this.component.shadowCastingMode = this.shadows;
		if (this.material)
		{
			this.component.sharedMaterial = this.material;
		}
		this.component.GetComponent<MeshFilter>().sharedMesh = this.mesh;
	}

	public override void Setup()
	{
		this.shadows = this.component.shadowCastingMode;
		this.material = this.component.sharedMaterial;
		this.mesh = this.component.GetComponent<MeshFilter>().sharedMesh;
	}
}