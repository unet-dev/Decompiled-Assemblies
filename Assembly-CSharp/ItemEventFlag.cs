using System;
using UnityEngine;
using UnityEngine.Events;

public class ItemEventFlag : MonoBehaviour, IItemUpdate
{
	public Item.Flag flag;

	public UnityEvent onEnabled = new UnityEvent();

	public UnityEvent onDisable = new UnityEvent();

	internal bool firstRun = true;

	internal bool lastState;

	public ItemEventFlag()
	{
	}

	public void OnItemUpdate(Item item)
	{
		bool flag = item.HasFlag(this.flag);
		if (!this.firstRun && flag == this.lastState)
		{
			return;
		}
		if (!flag)
		{
			this.onDisable.Invoke();
		}
		else
		{
			this.onEnabled.Invoke();
		}
		this.lastState = flag;
		this.firstRun = false;
	}
}