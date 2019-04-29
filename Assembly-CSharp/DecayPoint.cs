using System;
using UnityEngine;

public class DecayPoint : PrefabAttribute
{
	[Tooltip("If this point is occupied this will take this % off the power of the decay")]
	public float protection = 0.25f;

	public Socket_Base socket;

	public DecayPoint()
	{
	}

	protected override Type GetIndexedType()
	{
		return typeof(DecayPoint);
	}

	public bool IsOccupied(BaseEntity entity)
	{
		return entity.IsOccupied(this.socket);
	}
}