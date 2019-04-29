using System;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Armor Properties")]
public class ArmorProperties : ScriptableObject
{
	[InspectorFlags]
	public HitArea area;

	public ArmorProperties()
	{
	}

	public bool Contains(HitArea hitArea)
	{
		return (int)(this.area & hitArea) != 0;
	}
}