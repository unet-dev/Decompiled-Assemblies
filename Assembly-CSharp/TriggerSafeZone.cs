using System;
using UnityEngine;

public class TriggerSafeZone : TriggerBase
{
	public TriggerSafeZone()
	{
	}

	public float GetSafeLevel(Vector3 pos)
	{
		return 1f;
	}
}