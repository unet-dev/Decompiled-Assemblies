using System;
using UnityEngine;

public class HideUntilMobile : EntityComponent<BaseEntity>
{
	public GameObject[] visuals;

	private Vector3 startPos;

	public HideUntilMobile()
	{
	}
}