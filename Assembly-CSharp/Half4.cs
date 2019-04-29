using System;
using UnityEngine;

public struct Half4
{
	public ushort x;

	public ushort y;

	public ushort z;

	public ushort w;

	public Half4(Vector4 vec)
	{
		this.x = Mathf.FloatToHalf(vec.x);
		this.y = Mathf.FloatToHalf(vec.y);
		this.z = Mathf.FloatToHalf(vec.z);
		this.w = Mathf.FloatToHalf(vec.w);
	}

	public static explicit operator Vector4(Half4 vec)
	{
		return new Vector4(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z), Mathf.HalfToFloat(vec.w));
	}
}