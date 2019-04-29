using System;
using UnityEngine;
using UnityEngine.Events;

public class EntityFlag_Toggle : EntityComponent<BaseEntity>, IOnPostNetworkUpdate, IOnSendNetworkUpdate, IPrefabPreProcess
{
	public bool runClientside = true;

	public bool runServerside = true;

	public BaseEntity.Flags flag;

	[SerializeField]
	private UnityEvent onFlagEnabled = new UnityEvent();

	[SerializeField]
	private UnityEvent onFlagDisabled = new UnityEvent();

	internal bool hasRunOnce;

	internal bool lastHasFlag;

	public EntityFlag_Toggle()
	{
	}

	public void DoUpdate(BaseEntity entity)
	{
		bool flag = entity.HasFlag(this.flag);
		if (this.hasRunOnce && flag == this.lastHasFlag)
		{
			return;
		}
		this.hasRunOnce = true;
		this.lastHasFlag = flag;
		if (flag)
		{
			this.onFlagEnabled.Invoke();
			return;
		}
		this.onFlagDisabled.Invoke();
	}

	public void OnPostNetworkUpdate(BaseEntity entity)
	{
		if (base.baseEntity != entity)
		{
			return;
		}
		if (!this.runClientside)
		{
			return;
		}
		this.DoUpdate(entity);
	}

	public void OnSendNetworkUpdate(BaseEntity entity)
	{
		if (!this.runServerside)
		{
			return;
		}
		this.DoUpdate(entity);
	}

	public void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		bool flag;
		if (!clientside || !this.runClientside)
		{
			flag = (!serverside ? false : this.runServerside);
		}
		else
		{
			flag = true;
		}
		if (!flag)
		{
			process.RemoveComponent(this);
		}
	}
}