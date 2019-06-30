using System;
using UnityEngine;

public abstract class SingletonComponent<T> : SingletonComponent
where T : MonoBehaviour
{
	public static T Instance;

	static SingletonComponent()
	{
	}

	protected SingletonComponent()
	{
	}

	public override void SingletonClear()
	{
		if (SingletonComponent<T>.Instance == this)
		{
			SingletonComponent<T>.Instance = default(T);
		}
	}

	public override void SingletonSetup()
	{
		if (SingletonComponent<T>.Instance != this)
		{
			SingletonComponent<T>.Instance = (T)(this as T);
		}
	}
}