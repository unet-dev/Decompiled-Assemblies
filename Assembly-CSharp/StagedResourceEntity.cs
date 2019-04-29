using Facepunch;
using Network;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StagedResourceEntity : ResourceEntity
{
	public List<StagedResourceEntity.ResourceStage> stages = new List<StagedResourceEntity.ResourceStage>();

	public int stage;

	public GameObjectRef changeStageEffect;

	public GameObject gibSourceTest;

	public StagedResourceEntity()
	{
	}

	private int FindBestStage()
	{
		float single = Mathf.InverseLerp(0f, this.MaxHealth(), this.Health());
		for (int i = 0; i < this.stages.Count; i++)
		{
			if (single >= this.stages[i].health)
			{
				return i;
			}
		}
		return this.stages.Count - 1;
	}

	public T GetStageComponent<T>()
	where T : Component
	{
		return this.stages[this.stage].instance.GetComponentInChildren<T>();
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.resource == null)
		{
			return;
		}
		int num = info.msg.resource.stage;
		if ((!info.fromDisk ? false : base.isServer))
		{
			this.health = this.startHealth;
			num = 0;
		}
		if (num != this.stage)
		{
			this.stage = num;
			this.UpdateStage();
		}
	}

	protected override void OnHealthChanged()
	{
		StagedResourceEntity stagedResourceEntity = this;
		base.Invoke(new Action(stagedResourceEntity.UpdateNetworkStage), 0.1f);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("StagedResourceEntity.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (info.msg.resource == null)
		{
			info.msg.resource = Pool.Get<BaseResource>();
		}
		info.msg.resource.health = this.Health();
		info.msg.resource.stage = this.stage;
	}

	protected virtual void UpdateNetworkStage()
	{
		if (this.FindBestStage() != this.stage)
		{
			this.stage = this.FindBestStage();
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			this.UpdateStage();
		}
	}

	private void UpdateStage()
	{
		if (this.stages.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.stages.Count; i++)
		{
			this.stages[i].instance.SetActive(i == this.stage);
		}
		GroundWatch.PhysicsChanged(base.gameObject);
	}

	[Serializable]
	public class ResourceStage
	{
		public float health;

		public GameObject instance;

		public ResourceStage()
		{
		}
	}
}