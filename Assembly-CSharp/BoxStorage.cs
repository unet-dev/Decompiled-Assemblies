using System;
using UnityEngine;

public class BoxStorage : StorageContainer
{
	public BoxStorage()
	{
	}

	public override Vector3 GetDropPosition()
	{
		return base.ClosestPoint(base.GetDropPosition() + (base.LastAttackedDir * 10f));
	}
}