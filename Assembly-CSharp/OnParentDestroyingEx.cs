using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class OnParentDestroyingEx
{
	public static void BroadcastOnParentDestroying(this GameObject go)
	{
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponentsInChildren<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
	}

	public static void SendOnParentDestroying(this GameObject go)
	{
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponents<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
	}
}