using System;
using UnityEngine;

[Serializable]
public class MetabolismAttribute
{
	public float startMin;

	public float startMax;

	public float min;

	public float max;

	public float @value;

	public float lastValue;

	internal float lastGreatFraction;

	private const float greatInterval = 0.1f;

	public float greatFraction
	{
		get
		{
			return Mathf.Floor(this.Fraction() / 0.1f) / 10f;
		}
	}

	public MetabolismAttribute()
	{
	}

	public void Add(float val)
	{
		this.@value = Mathf.Clamp(this.@value + val, this.min, this.max);
	}

	public float Fraction()
	{
		return Mathf.InverseLerp(this.min, this.max, this.@value);
	}

	public bool HasChanged()
	{
		bool flag = this.lastValue != this.@value;
		this.lastValue = this.@value;
		return flag;
	}

	public bool HasGreatlyChanged()
	{
		float single = this.greatFraction;
		bool flag = this.lastGreatFraction != single;
		this.lastGreatFraction = single;
		return flag;
	}

	public void Increase(float fTarget)
	{
		fTarget = Mathf.Clamp(fTarget, this.min, this.max);
		if (fTarget <= this.@value)
		{
			return;
		}
		this.@value = fTarget;
	}

	public float InverseFraction()
	{
		return 1f - this.Fraction();
	}

	public void MoveTowards(float fTarget, float fRate)
	{
		if (fRate == 0f)
		{
			return;
		}
		this.@value = Mathf.Clamp(Mathf.MoveTowards(this.@value, fTarget, fRate), this.min, this.max);
	}

	public void Reset()
	{
		this.@value = Mathf.Clamp(UnityEngine.Random.Range(this.startMin, this.startMax), this.min, this.max);
	}

	public void SetValue(float newValue)
	{
		this.@value = newValue;
	}

	public void Subtract(float val)
	{
		this.@value = Mathf.Clamp(this.@value - val, this.min, this.max);
	}

	public enum Type
	{
		Calories,
		Hydration,
		Heartrate,
		Poison,
		Radiation,
		Bleeding,
		Health,
		HealthOverTime
	}
}