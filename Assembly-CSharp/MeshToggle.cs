using System;
using UnityEngine;

public class MeshToggle : MonoBehaviour
{
	public Mesh[] RendererMeshes;

	public Mesh[] ColliderMeshes;

	public MeshToggle()
	{
	}

	public void SwitchAll(int index)
	{
		this.SwitchRenderer(index);
		this.SwitchCollider(index);
	}

	public void SwitchAll(float factor)
	{
		this.SwitchRenderer(factor);
		this.SwitchCollider(factor);
	}

	public void SwitchCollider(int index)
	{
		if (this.ColliderMeshes.Length == 0)
		{
			return;
		}
		MeshCollider component = base.GetComponent<MeshCollider>();
		if (!component)
		{
			return;
		}
		component.sharedMesh = this.ColliderMeshes[Mathf.Clamp(index, 0, (int)this.ColliderMeshes.Length - 1)];
	}

	public void SwitchCollider(float factor)
	{
		int num = Mathf.RoundToInt(factor * (float)((int)this.ColliderMeshes.Length));
		this.SwitchCollider(num);
	}

	public void SwitchRenderer(int index)
	{
		if (this.RendererMeshes.Length == 0)
		{
			return;
		}
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (!component)
		{
			return;
		}
		component.sharedMesh = this.RendererMeshes[Mathf.Clamp(index, 0, (int)this.RendererMeshes.Length - 1)];
	}

	public void SwitchRenderer(float factor)
	{
		int num = Mathf.RoundToInt(factor * (float)((int)this.RendererMeshes.Length));
		this.SwitchRenderer(num);
	}
}