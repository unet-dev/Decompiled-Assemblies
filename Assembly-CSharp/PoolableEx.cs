using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class PoolableEx
{
	public static void AwakeFromInstantiate(this GameObject gameObject)
	{
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
			return;
		}
		gameObject.GetComponent<Poolable>().SetBehaviourEnabled(true);
	}

	public static bool SupportsPooling(this GameObject gameObject)
	{
		Poolable component = gameObject.GetComponent<Poolable>();
		if (component == null)
		{
			return false;
		}
		return component.prefabID != 0;
	}
}