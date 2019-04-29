using System;
using UnityEngine;

public class HideIfOwnerFirstPerson : EntityComponent<BaseEntity>, IClientComponent, IViewModeChanged
{
	public GameObject[] disableGameObjects;

	public bool worldModelEffect;

	public HideIfOwnerFirstPerson()
	{
	}
}