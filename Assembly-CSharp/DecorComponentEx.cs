using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class DecorComponentEx
{
	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components, ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		for (int i = 0; i < (int)components.Length; i++)
		{
			components[i].Apply(ref pos, ref rot, ref scale);
		}
	}

	public static void ApplyDecorComponents(this Transform transform, DecorComponent[] components)
	{
		Vector3 vector3 = transform.position;
		Quaternion quaternion = transform.rotation;
		Vector3 vector31 = transform.localScale;
		transform.ApplyDecorComponents(components, ref vector3, ref quaternion, ref vector31);
		transform.position = vector3;
		transform.rotation = quaternion;
		transform.localScale = vector31;
	}

	public static void ApplyDecorComponentsScaleOnly(this Transform transform, DecorComponent[] components)
	{
		Vector3 vector3 = transform.position;
		Quaternion quaternion = transform.rotation;
		Vector3 vector31 = transform.localScale;
		transform.ApplyDecorComponents(components, ref vector3, ref quaternion, ref vector31);
		transform.localScale = vector31;
	}
}