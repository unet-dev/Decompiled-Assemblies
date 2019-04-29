using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshPaintableSource : MonoBehaviour, IClientComponent
{
	public int texWidth = 256;

	public int texHeight = 128;

	public string replacementTextureName = "_DecalTexture";

	public float cameraFOV = 60f;

	public float cameraDistance = 2f;

	[NonSerialized]
	public Texture2D texture;

	public GameObject sourceObject;

	public Mesh collisionMesh;

	public Vector3 localPosition;

	public Vector3 localRotation;

	private static MaterialPropertyBlock block;

	static MeshPaintableSource()
	{
	}

	public MeshPaintableSource()
	{
	}

	public void Clear()
	{
		if (this.texture == null)
		{
			return;
		}
		this.texture.Clear(new Color(0f, 0f, 0f, 0f));
		this.texture.Apply(true, false);
	}

	public void Free()
	{
		if (this.texture)
		{
			UnityEngine.Object.Destroy(this.texture);
			this.texture = null;
		}
	}

	public void Init()
	{
		if (this.texture)
		{
			return;
		}
		this.texture = new Texture2D(this.texWidth, this.texHeight, TextureFormat.ARGB32, false)
		{
			name = string.Concat("MeshPaintableSource_", base.gameObject.name)
		};
		this.texture.Clear(Color.clear);
		if (MeshPaintableSource.block != null)
		{
			MeshPaintableSource.block.Clear();
		}
		else
		{
			MeshPaintableSource.block = new MaterialPropertyBlock();
		}
		MeshPaintableSource.block.SetTexture(this.replacementTextureName, this.texture);
		List<Renderer> list = Pool.GetList<Renderer>();
		base.transform.root.GetComponentsInChildren<Renderer>(true, list);
		foreach (Renderer renderer in list)
		{
			renderer.SetPropertyBlock(MeshPaintableSource.block);
		}
		Pool.FreeList<Renderer>(ref list);
	}

	public void Load(byte[] data)
	{
		this.Init();
		if (data != null)
		{
			this.texture.LoadImage(data);
			if (this.texture.width != this.texWidth || this.texture.height != this.texHeight)
			{
				this.texture.Resize(this.texWidth, this.texHeight);
			}
			this.texture.Apply(true, false);
		}
	}

	public void UpdateFrom(Texture2D input)
	{
		this.Init();
		this.texture.SetPixels32(input.GetPixels32());
		this.texture.Apply(true, false);
	}
}