using System;
using UnityEngine;

public class HitNumber : MonoBehaviour
{
	public HitNumber.HitType hitType;

	public HitNumber()
	{
	}

	public int ColorToMultiplier(HitNumber.HitType type)
	{
		switch (type)
		{
			case HitNumber.HitType.Yellow:
			{
				return 1;
			}
			case HitNumber.HitType.Green:
			{
				return 3;
			}
			case HitNumber.HitType.Blue:
			{
				return 5;
			}
			case HitNumber.HitType.Purple:
			{
				return 10;
			}
			case HitNumber.HitType.Red:
			{
				return 20;
			}
		}
		return 0;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(base.transform.position, 0.025f);
	}

	public enum HitType
	{
		Yellow,
		Green,
		Blue,
		Purple,
		Red
	}
}