using System;
using UnityEngine;

[Serializable]
public struct LayerSelect
{
	[SerializeField]
	private int layer;

	public int Mask
	{
		get
		{
			return 1 << (this.layer & 31);
		}
	}

	public string Name
	{
		get
		{
			return LayerMask.LayerToName(this.layer);
		}
	}

	public LayerSelect(int layer)
	{
		this.layer = layer;
	}

	public static implicit operator Int32(LayerSelect layer)
	{
		return layer.layer;
	}

	public static implicit operator LayerSelect(int layer)
	{
		return new LayerSelect(layer);
	}
}