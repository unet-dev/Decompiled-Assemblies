using System;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ConstructionPlaceholder : PrefabAttribute, IPrefabPreProcess
{
	public Mesh mesh;

	public Material material;

	public bool renderer;

	public bool collider;

	public ConstructionPlaceholder()
	{
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		if (clientside)
		{
			if (this.renderer)
			{
				MeshFilter component = rootObj.GetComponent<MeshFilter>();
				MeshRenderer meshRenderer = rootObj.GetComponent<MeshRenderer>();
				if (!component)
				{
					rootObj.AddComponent<MeshFilter>().sharedMesh = this.mesh;
				}
				if (!meshRenderer)
				{
					meshRenderer = rootObj.AddComponent<MeshRenderer>();
					meshRenderer.sharedMaterial = this.material;
					meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
				}
			}
			if (this.collider && !rootObj.GetComponent<MeshCollider>())
			{
				rootObj.AddComponent<MeshCollider>().sharedMesh = this.mesh;
			}
		}
	}

	protected override Type GetIndexedType()
	{
		return typeof(ConstructionPlaceholder);
	}
}