using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StabilityEntity : DecayEntity
{
	public bool grounded;

	[NonSerialized]
	public float cachedStability;

	[NonSerialized]
	public int cachedDistanceFromGround = 2147483647;

	private List<StabilityEntity.Support> supports;

	private int stabilityStrikes;

	private bool dirty;

	public static StabilityEntity.StabilityCheckWorkQueue stabilityCheckQueue;

	public static StabilityEntity.UpdateSurroundingsQueue updateSurroundingsQueue;

	static StabilityEntity()
	{
		StabilityEntity.stabilityCheckQueue = new StabilityEntity.StabilityCheckWorkQueue();
		StabilityEntity.updateSurroundingsQueue = new StabilityEntity.UpdateSurroundingsQueue();
	}

	public StabilityEntity()
	{
	}

	public int CachedDistanceFromGround(StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1;
		}
		if (this.supports == null)
		{
			return 1;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		int num = 2147483647;
		for (int i = 0; i < this.supports.Count; i++)
		{
			StabilityEntity stabilityEntity = this.supports[i].SupportEntity(ignoreEntity);
			if (stabilityEntity != null)
			{
				int num1 = stabilityEntity.cachedDistanceFromGround;
				if (num1 != 2147483647)
				{
					num = Mathf.Min(num, num1 + 1);
				}
			}
		}
		return num;
	}

	public float CachedSupportValue(StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1f;
		}
		if (this.supports == null)
		{
			return 1f;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		float single = 0f;
		for (int i = 0; i < this.supports.Count; i++)
		{
			StabilityEntity.Support item = this.supports[i];
			StabilityEntity stabilityEntity = item.SupportEntity(ignoreEntity);
			if (stabilityEntity != null)
			{
				float single1 = stabilityEntity.cachedStability;
				if (single1 != 0f)
				{
					single = single + single1 * item.factor;
				}
			}
		}
		return Mathf.Clamp01(single);
	}

	protected void DebugNudge()
	{
		this.StabilityCheck();
	}

	public int DistanceFromGround(StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1;
		}
		if (this.supports == null)
		{
			return 1;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		int num = 2147483647;
		for (int i = 0; i < this.supports.Count; i++)
		{
			StabilityEntity stabilityEntity = this.supports[i].SupportEntity(ignoreEntity);
			if (stabilityEntity != null)
			{
				int num1 = stabilityEntity.CachedDistanceFromGround(ignoreEntity);
				if (num1 != 2147483647)
				{
					num = Mathf.Min(num, num1 + 1);
				}
			}
		}
		return num;
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.UpdateSurroundingEntities();
	}

	public void InitializeSupports()
	{
		this.supports = new List<StabilityEntity.Support>();
		if (this.grounded)
		{
			return;
		}
		List<EntityLink> entityLinks = base.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink item = entityLinks[i];
			if (item.IsMale())
			{
				if (item.socket is StabilitySocket)
				{
					this.supports.Add(new StabilityEntity.Support(this, item, (item.socket as StabilitySocket).support));
				}
				if (item.socket is ConstructionSocket)
				{
					this.supports.Add(new StabilityEntity.Support(this, item, (item.socket as ConstructionSocket).support));
				}
			}
		}
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.stabilityEntity != null)
		{
			this.cachedStability = info.msg.stabilityEntity.stability;
			this.cachedDistanceFromGround = info.msg.stabilityEntity.distanceFromGround;
			if (this.cachedStability <= 0f)
			{
				this.cachedStability = 0f;
			}
			if (this.cachedDistanceFromGround <= 0)
			{
				this.cachedDistanceFromGround = 2147483647;
			}
		}
	}

	protected void OnPhysicsNeighbourChanged()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		this.StabilityCheck();
	}

	public override void ResetState()
	{
		base.ResetState();
		this.cachedStability = 0f;
		this.cachedDistanceFromGround = 2147483647;
		this.supports = null;
		this.stabilityStrikes = 0;
		this.dirty = false;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.stabilityEntity = Facepunch.Pool.Get<ProtoBuf.StabilityEntity>();
		info.msg.stabilityEntity.stability = this.cachedStability;
		info.msg.stabilityEntity.distanceFromGround = this.cachedDistanceFromGround;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!Rust.Application.isLoadingSave)
		{
			this.UpdateStability();
		}
	}

	public void StabilityCheck()
	{
		if (base.IsDestroyed)
		{
			return;
		}
		if (this.supports == null)
		{
			this.InitializeSupports();
		}
		bool flag = false;
		int num = this.DistanceFromGround(null);
		if (num != this.cachedDistanceFromGround)
		{
			this.cachedDistanceFromGround = num;
			flag = true;
		}
		float single = this.SupportValue(null);
		if (Mathf.Abs(this.cachedStability - single) > Stability.accuracy)
		{
			this.cachedStability = single;
			flag = true;
		}
		if (flag)
		{
			this.dirty = true;
			this.UpdateConnectedEntities();
			this.UpdateStability();
		}
		else if (this.dirty)
		{
			this.dirty = false;
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		if (single >= Stability.collapse)
		{
			this.stabilityStrikes = 0;
			return;
		}
		if (this.stabilityStrikes >= Stability.strikes)
		{
			base.Kill(BaseNetworkable.DestroyMode.Gib);
			return;
		}
		this.UpdateStability();
		this.stabilityStrikes++;
	}

	public float SupportValue(StabilityEntity ignoreEntity = null)
	{
		if (this.grounded)
		{
			return 1f;
		}
		if (this.supports == null)
		{
			return 1f;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		float single = 0f;
		for (int i = 0; i < this.supports.Count; i++)
		{
			StabilityEntity.Support item = this.supports[i];
			StabilityEntity stabilityEntity = item.SupportEntity(ignoreEntity);
			if (stabilityEntity != null)
			{
				float single1 = stabilityEntity.CachedSupportValue(ignoreEntity);
				if (single1 != 0f)
				{
					single = single + single1 * item.factor;
				}
			}
		}
		return Mathf.Clamp01(single);
	}

	public void UpdateConnectedEntities()
	{
		List<EntityLink> entityLinks = base.GetEntityLinks(true);
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink item = entityLinks[i];
			if (item.IsFemale())
			{
				for (int j = 0; j < item.connections.Count; j++)
				{
					StabilityEntity stabilityEntity = item.connections[j].owner as StabilityEntity;
					if (!(stabilityEntity == null) && !stabilityEntity.isClient && !stabilityEntity.IsDestroyed)
					{
						stabilityEntity.UpdateStability();
					}
				}
			}
		}
	}

	public void UpdateStability()
	{
		StabilityEntity.stabilityCheckQueue.Add(this);
	}

	public void UpdateSurroundingEntities()
	{
		StabilityEntity.updateSurroundingsQueue.Add(base.WorldSpaceBounds().ToBounds());
	}

	public class StabilityCheckWorkQueue : ObjectWorkQueue<StabilityEntity>
	{
		public StabilityCheckWorkQueue()
		{
		}

		protected override void RunJob(StabilityEntity entity)
		{
			if (!this.ShouldAdd(entity))
			{
				return;
			}
			entity.StabilityCheck();
		}

		protected override bool ShouldAdd(StabilityEntity entity)
		{
			if (!ConVar.Server.stability)
			{
				return false;
			}
			if (!entity.IsValid())
			{
				return false;
			}
			if (!entity.isServer)
			{
				return false;
			}
			return true;
		}
	}

	private class Support
	{
		public StabilityEntity parent;

		public EntityLink link;

		public float factor;

		public Support(StabilityEntity parent, EntityLink link, float factor)
		{
			this.parent = parent;
			this.link = link;
			this.factor = factor;
		}

		public StabilityEntity SupportEntity(StabilityEntity ignoreEntity = null)
		{
			StabilityEntity stabilityEntity = null;
			for (int i = 0; i < this.link.connections.Count; i++)
			{
				StabilityEntity item = this.link.connections[i].owner as StabilityEntity;
				if (!(item == null) && !(item == this.parent) && !(item == ignoreEntity) && !item.isClient && !item.IsDestroyed && (stabilityEntity == null || item.cachedDistanceFromGround < stabilityEntity.cachedDistanceFromGround))
				{
					stabilityEntity = item;
				}
			}
			return stabilityEntity;
		}
	}

	public class UpdateSurroundingsQueue : ObjectWorkQueue<Bounds>
	{
		public UpdateSurroundingsQueue()
		{
		}

		protected override void RunJob(Bounds bounds)
		{
			if (!ConVar.Server.stability)
			{
				return;
			}
			List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
			Vector3 vector3 = bounds.center;
			Vector3 vector31 = bounds.extents;
			Vis.Entities<BaseEntity>(vector3, vector31.magnitude + 1f, list, 2228480, QueryTriggerInteraction.Collide);
			foreach (BaseEntity baseEntity in list)
			{
				if (baseEntity.IsDestroyed || baseEntity.isClient)
				{
					continue;
				}
				if (!(baseEntity is StabilityEntity))
				{
					baseEntity.BroadcastMessage("OnPhysicsNeighbourChanged", SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					(baseEntity as StabilityEntity).OnPhysicsNeighbourChanged();
				}
			}
			Facepunch.Pool.FreeList<BaseEntity>(ref list);
		}
	}
}