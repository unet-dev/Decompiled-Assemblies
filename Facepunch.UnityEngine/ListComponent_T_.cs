using System;
using UnityEngine;

public abstract class ListComponent<T> : ListComponent
where T : MonoBehaviour
{
	public static ListHashSet<T> InstanceList;

	static ListComponent()
	{
		ListComponent<T>.InstanceList = new ListHashSet<T>(8);
	}

	protected ListComponent()
	{
	}

	public override void Clear()
	{
		ListComponent<T>.InstanceList.Remove((T)(this as T));
	}

	public override void Setup()
	{
		if (!ListComponent<T>.InstanceList.Contains((T)(this as T)))
		{
			ListComponent<T>.InstanceList.Add((T)(this as T));
		}
	}
}