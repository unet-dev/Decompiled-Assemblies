using Rust;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class LoadBalancer : SingletonComponent<LoadBalancer>
{
	public static bool Paused;

	private const float MinMilliseconds = 1f;

	private const float MaxMilliseconds = 100f;

	private const int MinBacklog = 1000;

	private const int MaxBacklog = 100000;

	private Queue<DeferredAction>[] queues = new Queue<DeferredAction>[] { new Queue<DeferredAction>(), new Queue<DeferredAction>(), new Queue<DeferredAction>(), new Queue<DeferredAction>(), new Queue<DeferredAction>() };

	private Stopwatch watch = Stopwatch.StartNew();

	static LoadBalancer()
	{
	}

	public LoadBalancer()
	{
	}

	public static int Count()
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			return 0;
		}
		Queue<DeferredAction>[] instance = SingletonComponent<LoadBalancer>.Instance.queues;
		int count = 0;
		for (int i = 0; i < (int)instance.Length; i++)
		{
			count += instance[i].Count;
		}
		return count;
	}

	private static void CreateInstance()
	{
		GameObject gameObject = new GameObject()
		{
			name = "LoadBalancer"
		};
		gameObject.AddComponent<LoadBalancer>();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	public static void Enqueue(DeferredAction action)
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			LoadBalancer.CreateInstance();
		}
		SingletonComponent<LoadBalancer>.Instance.queues[action.Index].Enqueue(action);
	}

	protected void LateUpdate()
	{
		if (Rust.Application.isReceiving)
		{
			return;
		}
		if (Rust.Application.isLoading)
		{
			return;
		}
		if (LoadBalancer.Paused)
		{
			return;
		}
		int num = LoadBalancer.Count();
		float single = Mathf.InverseLerp(1000f, 100000f, (float)num);
		float single1 = Mathf.SmoothStep(1f, 100f, single);
		this.watch.Reset();
		this.watch.Start();
		for (int i = 0; i < (int)this.queues.Length; i++)
		{
			Queue<DeferredAction> deferredActions = this.queues[i];
			while (deferredActions.Count > 0)
			{
				deferredActions.Dequeue().Action();
				if (this.watch.Elapsed.TotalMilliseconds <= (double)single1)
				{
					continue;
				}
				return;
			}
		}
	}

	public static void ProcessAll()
	{
		if (!SingletonComponent<LoadBalancer>.Instance)
		{
			LoadBalancer.CreateInstance();
		}
		Queue<DeferredAction>[] instance = SingletonComponent<LoadBalancer>.Instance.queues;
		for (int i = 0; i < (int)instance.Length; i++)
		{
			Queue<DeferredAction> deferredActions = instance[i];
			while (deferredActions.Count > 0)
			{
				deferredActions.Dequeue().Action();
			}
		}
	}
}