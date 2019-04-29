using System;
using UnityEngine;

public abstract class ItemContainerSource : MonoBehaviour
{
	protected ItemContainerSource()
	{
	}

	public abstract ItemContainer GetItemContainer();
}