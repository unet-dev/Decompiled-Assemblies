using System;
using UnityEngine;

public class DiveSite : JunkPile
{
	public Transform bobber;

	public DiveSite()
	{
	}

	public override float TimeoutPlayerCheckRadius()
	{
		return 40f;
	}
}