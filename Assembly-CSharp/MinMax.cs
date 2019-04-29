using System;
using UnityEngine;

[Serializable]
public class MinMax
{
	public float x;

	public float y = 1f;

	public MinMax(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	public float Lerp(float t)
	{
		return Mathf.Lerp(this.x, this.y, t);
	}

	public float Lerp(float a, float b, float t)
	{
		return Mathf.Lerp(this.x, this.y, Mathf.InverseLerp(a, b, t));
	}

	public float Random()
	{
		return UnityEngine.Random.Range(this.x, this.y);
	}
}