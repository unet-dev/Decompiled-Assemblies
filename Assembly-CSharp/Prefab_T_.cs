using System;
using UnityEngine;

public class Prefab<T> : Prefab, IComparable<Prefab<T>>
where T : UnityEngine.Component
{
	public T Component;

	public Prefab(string name, GameObject prefab, T component, GameManager manager, PrefabAttribute.Library attribute) : base(name, prefab, manager, attribute)
	{
		this.Component = component;
	}

	public int CompareTo(Prefab<T> that)
	{
		return base.CompareTo(that);
	}
}