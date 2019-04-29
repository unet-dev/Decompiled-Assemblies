using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class InvokeHandler : SingletonComponent<InvokeHandler>
{
	private ListDictionary<InvokeAction, float> curList = new ListDictionary<InvokeAction, float>(2048);

	private ListHashSet<InvokeAction> addList = new ListHashSet<InvokeAction>(1024);

	private ListHashSet<InvokeAction> delList = new ListHashSet<InvokeAction>(1024);

	private int nullIndex;

	private const int nullChecks = 50;

	public InvokeHandler()
	{
	}

	private void ApplyAdds()
	{
		InvokeAction[] buffer = this.addList.Values.Buffer;
		int count = this.addList.Count;
		float single = Time.time;
		for (int i = 0; i < count; i++)
		{
			InvokeAction invokeAction = buffer[i];
			this.curList.Remove(invokeAction);
			this.curList.Add(invokeAction, single + invokeAction.initial);
		}
		this.addList.Clear();
	}

	private void ApplyRemoves()
	{
		InvokeAction[] buffer = this.delList.Values.Buffer;
		int count = this.delList.Count;
		for (int i = 0; i < count; i++)
		{
			InvokeAction invokeAction = buffer[i];
			this.curList.Remove(invokeAction);
		}
		this.delList.Clear();
	}

	public static void CancelInvoke(Behaviour sender, Action action)
	{
		if (SingletonComponent<InvokeHandler>.Instance)
		{
			SingletonComponent<InvokeHandler>.Instance.QueueRemove(new InvokeAction(sender, action, 0f, -1f, 0f));
		}
	}

	private bool Contains(InvokeAction invoke)
	{
		if (this.delList.Contains(invoke))
		{
			return false;
		}
		if (this.curList.Contains(invoke))
		{
			return true;
		}
		return this.addList.Contains(invoke);
	}

	public static int Count()
	{
		if (!SingletonComponent<InvokeHandler>.Instance)
		{
			return 0;
		}
		return SingletonComponent<InvokeHandler>.Instance.curList.Count;
	}

	private static void CreateInstance()
	{
		GameObject gameObject = new GameObject()
		{
			name = "InvokeHandler"
		};
		gameObject.AddComponent<InvokeHandler>();
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	private void DoTick()
	{
		float[] buffer = this.curList.Values.Buffer;
		InvokeAction[] invokeActionArray = this.curList.Keys.Buffer;
		int count = this.curList.Count;
		float single = Time.time;
		for (int i = 0; i < count; i++)
		{
			if (single >= buffer[i])
			{
				InvokeAction invokeAction = invokeActionArray[i];
				if (!invokeAction.sender || this.delList.Contains(invokeAction))
				{
					this.QueueRemove(invokeAction);
				}
				else
				{
					if (invokeAction.repeat < 0f)
					{
						this.QueueRemove(invokeAction);
					}
					else
					{
						float single1 = single + invokeAction.repeat;
						if (invokeAction.random > 0f)
						{
							single1 += UnityEngine.Random.Range(-invokeAction.random, invokeAction.random);
						}
						buffer[i] = single1;
					}
					TimeWarning.BeginSample(invokeAction.action.Method.Name);
					invokeAction.action();
					TimeWarning.EndSample();
				}
			}
		}
	}

	public static void FindInvokes(Behaviour sender, List<InvokeAction> list)
	{
		if (!SingletonComponent<InvokeHandler>.Instance)
		{
			return;
		}
		InvokeAction[] buffer = SingletonComponent<InvokeHandler>.Instance.curList.Keys.Buffer;
		int count = SingletonComponent<InvokeHandler>.Instance.curList.Count;
		for (int i = 0; i < count; i++)
		{
			InvokeAction invokeAction = buffer[i];
			if (invokeAction.sender == sender)
			{
				list.Add(invokeAction);
			}
		}
	}

	public static void Invoke(Behaviour sender, Action action, float time)
	{
		if (!SingletonComponent<InvokeHandler>.Instance)
		{
			InvokeHandler.CreateInstance();
		}
		SingletonComponent<InvokeHandler>.Instance.QueueAdd(new InvokeAction(sender, action, time, -1f, 0f));
	}

	public static void InvokeRandomized(Behaviour sender, Action action, float time, float repeat, float random)
	{
		if (!SingletonComponent<InvokeHandler>.Instance)
		{
			InvokeHandler.CreateInstance();
		}
		SingletonComponent<InvokeHandler>.Instance.QueueAdd(new InvokeAction(sender, action, time, repeat, random));
	}

	public static void InvokeRepeating(Behaviour sender, Action action, float time, float repeat)
	{
		if (!SingletonComponent<InvokeHandler>.Instance)
		{
			InvokeHandler.CreateInstance();
		}
		SingletonComponent<InvokeHandler>.Instance.QueueAdd(new InvokeAction(sender, action, time, repeat, 0f));
	}

	public static bool IsInvoking(Behaviour sender, Action action)
	{
		if (!SingletonComponent<InvokeHandler>.Instance)
		{
			return false;
		}
		return SingletonComponent<InvokeHandler>.Instance.Contains(new InvokeAction(sender, action, 0f, -1f, 0f));
	}

	protected void LateUpdate()
	{
		this.ApplyRemoves();
		this.ApplyAdds();
		this.DoTick();
		this.RemoveExpired();
		this.ApplyRemoves();
		this.ApplyAdds();
	}

	private void QueueAdd(InvokeAction invoke)
	{
		this.delList.Remove(invoke);
		this.addList.Remove(invoke);
		this.addList.Add(invoke);
	}

	private void QueueRemove(InvokeAction invoke)
	{
		this.delList.Remove(invoke);
		this.addList.Remove(invoke);
		this.delList.Add(invoke);
	}

	private void RemoveExpired()
	{
		InvokeAction[] buffer = this.curList.Keys.Buffer;
		int count = this.curList.Count;
		if (this.nullIndex >= count)
		{
			this.nullIndex = 0;
		}
		int num = Mathf.Min(this.nullIndex + 50, count);
		while (this.nullIndex < num)
		{
			InvokeAction invokeAction = buffer[this.nullIndex];
			if (!invokeAction.sender)
			{
				this.QueueRemove(invokeAction);
			}
			this.nullIndex++;
		}
	}
}