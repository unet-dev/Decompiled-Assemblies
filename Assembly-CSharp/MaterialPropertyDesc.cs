using System;
using UnityEngine;

public struct MaterialPropertyDesc
{
	public int nameID;

	public Type type;

	public MaterialPropertyDesc(string name, Type type)
	{
		this.nameID = Shader.PropertyToID(name);
		this.type = type;
	}
}