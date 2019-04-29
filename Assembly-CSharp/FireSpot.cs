using System;
using UnityEngine;

public class FireSpot : BaseEntity
{
	public GameObject flameEffect;

	public FireSpot()
	{
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}
}