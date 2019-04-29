using System;
using UnityEngine;

public struct Half3
{
	public ushort x;

	public ushort y;

	public ushort z;

	public Half3(Vector3 vec)
	{
		this.x = Mathf.FloatToHalf(vec.x);
		this.y = Mathf.FloatToHalf(vec.y);
		this.z = Mathf.FloatToHalf(vec.z);
	}

	public static explicit operator Vector3(Half3 vec)
	{
		return new Vector3(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z));
	}
}