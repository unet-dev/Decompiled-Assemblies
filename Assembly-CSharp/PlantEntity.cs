using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlantEntity : BaseCombatEntity, IInstanceDataReceiver
{
	public PlantProperties plantProperty;

	public int water = -1;

	public int consumedWater = -1;

	public PlantProperties.State state;

	public float realAge;

	public float growthAge;

	private float stageAge;

	private float groundConditions = 1f;

	private float lightExposure;

	private int genetics = -1;

	private int seasons;

	private int harvests;

	private PlantProperties.Stage currentStage
	{
		get
		{
			return this.plantProperty.stages[(int)this.state];
		}
	}

	protected float growDeltaTime
	{
		get
		{
			return ConVar.Server.planttick * ConVar.Server.planttickscale;
		}
	}

	public float stageAgeFraction
	{
		get
		{
			return this.stageAge / (this.currentStage.lifeLength * 60f);
		}
	}

	protected float thinkDeltaTime
	{
		get
		{
			return ConVar.Server.planttick;
		}
	}

	public PlantEntity()
	{
	}

	private void BecomeState(PlantProperties.State state, bool resetAge = true)
	{
		if (base.isServer && this.state == state)
		{
			return;
		}
		this.state = state;
		if (base.isServer)
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			if (resetAge)
			{
				this.stageAge = 0f;
			}
		}
	}

	public float CalculateArtificialLightExposure()
	{
		float single = 0f;
		List<CeilingLight> list = Facepunch.Pool.GetList<CeilingLight>();
		Vis.Entities<CeilingLight>(base.transform.position + new Vector3(0f, 2f, 0f), 2f, list, 256, QueryTriggerInteraction.Collide);
		foreach (CeilingLight ceilingLight in list)
		{
			if (!ceilingLight.IsOn())
			{
				continue;
			}
			single += 1f;
			Facepunch.Pool.FreeList<CeilingLight>(ref list);
			return single;
		}
		Facepunch.Pool.FreeList<CeilingLight>(ref list);
		return single;
	}

	public float CalculateSunExposure()
	{
		RaycastHit raycastHit;
		if (TOD_Sky.Instance.IsNight)
		{
			return 0f;
		}
		Vector3 vector3 = base.transform.position + new Vector3(0f, 1f, 0f);
		if (UnityEngine.Physics.Raycast(vector3, (TOD_Sky.Instance.Components.Sun.transform.position - vector3).normalized, out raycastHit, 100f, 10551297))
		{
			return 0f;
		}
		return 1f;
	}

	public bool CanClone()
	{
		if (this.currentStage.resources <= 0f)
		{
			return false;
		}
		return this.plantProperty.cloneItem != null;
	}

	public bool CanPick()
	{
		return this.currentStage.resources > 0f;
	}

	public override string DebugText()
	{
		return string.Format("State: {0}\nGenetics: {1:0.00}\nHealth: {2:0.00}\nGroundCondition: {3:0.00}\nHappiness: {4:0.00}\nWater: {5:0.00}\nAge: {6}", new object[] { this.state, this.genetics, base.health, this.groundConditions, this.Happiness(), this.water, ((long)this.realAge).FormatSeconds() });
	}

	private float Energy_Light()
	{
		return this.lightExposure;
	}

	private float Energy_Temperature()
	{
		float single = this.plantProperty.temperatureHappiness.Evaluate(this.GetTemperature());
		if (single <= 0f)
		{
			return single;
		}
		return single * 0.2f;
	}

	private float Energy_Water()
	{
		return (float)this.water;
	}

	public float GetGrowthAge()
	{
		return this.growthAge;
	}

	public float GetRealAge()
	{
		return this.realAge;
	}

	public float GetStageAge()
	{
		return this.stageAge;
	}

	private float GetTemperature()
	{
		float temperature = Climate.GetTemperature(base.transform.position);
		if (this.PlacedInPlanter() && temperature < 10f)
		{
			temperature = 10f;
		}
		return temperature;
	}

	private float Happiness()
	{
		bool flag = this.PlacedInPlanter();
		float single = 0f;
		single += this.Energy_Light();
		single += this.Energy_Temperature();
		single += this.Energy_Water();
		single = single + (flag ? 2f : this.groundConditions);
		single /= 4f;
		float single1 = (float)this.genetics / 10000f;
		single = Mathf.Clamp(single, -1f, 0.25f + single1 * 0.75f);
		if (single > -0.1f && single < 0.1f)
		{
			single = Mathf.Sign(single) * 0.1f;
		}
		return single;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.plantEntity != null)
		{
			this.genetics = info.msg.plantEntity.genetics;
			this.growthAge = info.msg.plantEntity.age;
			this.water = info.msg.plantEntity.water;
			this.realAge = info.msg.plantEntity.totalAge;
			this.growthAge = info.msg.plantEntity.growthAge;
			this.stageAge = info.msg.plantEntity.stageAge;
			this.BecomeState((PlantProperties.State)info.msg.plantEntity.state, false);
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("PlantEntity.OnRpcMessage", 0.1f))
		{
			if (rpc == 598660365 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_PickFruit "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_PickFruit", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_PickFruit", this, player, 3f))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_PickFruit(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_PickFruit");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -2072006462 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_TakeClone "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_TakeClone", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_TakeClone", this, player, 3f))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.RPC_TakeClone(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_TakeClone");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void PickFruit(BasePlayer receiver)
	{
		Quaternion quaternion;
		if (!this.CanPick())
		{
			return;
		}
		this.harvests++;
		float single = this.YieldBonusScale() * (float)this.plantProperty.waterYieldBonus;
		int num = Mathf.RoundToInt((this.currentStage.resources + single) * (float)this.plantProperty.pickupAmount);
		this.ResetSeason();
		if (!this.plantProperty.pickupItem.condition.enabled)
		{
			Item item = ItemManager.Create(this.plantProperty.pickupItem, num, (ulong)0);
			if (!receiver)
			{
				Vector3 vector3 = base.transform.position + (Vector3.up * 0.5f);
				Vector3 vector31 = Vector3.up * 1f;
				quaternion = new Quaternion();
				item.Drop(vector3, vector31, quaternion);
			}
			else
			{
				if (Interface.CallHook("OnCropGather", this, item, receiver) != null)
				{
					return;
				}
				receiver.GiveItem(item, BaseEntity.GiveItemReason.PickedUp);
			}
		}
		else
		{
			for (int i = 0; i < num; i++)
			{
				Item item1 = ItemManager.Create(this.plantProperty.pickupItem, 1, (ulong)0);
				item1.conditionNormalized = this.plantProperty.fruitCurve.Evaluate(this.stageAgeFraction);
				if (!receiver)
				{
					Vector3 vector32 = base.transform.position + (Vector3.up * 0.5f);
					Vector3 vector33 = Vector3.up * 1f;
					quaternion = new Quaternion();
					item1.Drop(vector32, vector33, quaternion);
				}
				else
				{
					if (Interface.CallHook("OnCropGather", this, item1, receiver) != null)
					{
						return;
					}
					receiver.GiveItem(item1, BaseEntity.GiveItemReason.PickedUp);
				}
			}
		}
		if (this.plantProperty.pickEffect.isValid)
		{
			Effect.server.Run(this.plantProperty.pickEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		if (this.harvests >= this.plantProperty.maxHarvests)
		{
			if (this.plantProperty.disappearAfterHarvest)
			{
				this.Die(null);
				return;
			}
			this.BecomeState(PlantProperties.State.Dying, true);
			return;
		}
		this.growthAge = this.plantProperty.waterConsumptionLifetime - this.plantProperty.stages[3].lifeLength;
		this.BecomeState(PlantProperties.State.Mature, true);
	}

	private bool PlacedInPlanter()
	{
		if (base.GetParentEntity() != null && base.GetParentEntity() is PlanterBox)
		{
			return true;
		}
		return false;
	}

	public void ReceiveInstanceData(ProtoBuf.Item.InstanceData data)
	{
		this.genetics = data.dataInt;
	}

	public void RefreshLightExposure()
	{
		if (!ConVar.Server.plantlightdetection)
		{
			this.lightExposure = this.plantProperty.timeOfDayHappiness.Evaluate(TOD_Sky.Instance.Cycle.Hour);
			return;
		}
		this.lightExposure = this.CalculateSunExposure() * this.plantProperty.timeOfDayHappiness.Evaluate(TOD_Sky.Instance.Cycle.Hour);
		if (this.lightExposure <= 0f)
		{
			this.lightExposure = this.CalculateArtificialLightExposure() * 2f;
		}
	}

	public void ResetSeason()
	{
		this.consumedWater = 0;
		if (this.water == -1)
		{
			this.water = Mathf.CeilToInt((float)this.plantProperty.maxHeldWater * 0.5f);
		}
	}

	public override void ResetState()
	{
		base.ResetState();
		this.state = PlantProperties.State.Seed;
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void RPC_PickFruit(BaseEntity.RPCMessage msg)
	{
		this.PickFruit(msg.player);
	}

	[MaxDistance(3f)]
	[RPC_Server]
	public void RPC_TakeClone(BaseEntity.RPCMessage msg)
	{
		if (!this.CanClone())
		{
			return;
		}
		int num = 1 + Mathf.Clamp(Mathf.CeilToInt(this.currentStage.resources * (1f + this.YieldBonusScale()) / 0.25f), 1, 4);
		for (int i = 0; i < num; i++)
		{
			Item instanceDatum = ItemManager.Create(this.plantProperty.cloneItem, 1, (ulong)0);
			instanceDatum.instanceData = new ProtoBuf.Item.InstanceData()
			{
				dataInt = Mathf.CeilToInt((float)this.genetics * 0.9f),
				ShouldPool = false
			};
			msg.player.GiveItem(instanceDatum, BaseEntity.GiveItemReason.PickedUp);
		}
		if (this.plantProperty.pickEffect.isValid)
		{
			Effect.server.Run(this.plantProperty.pickEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		this.Die(null);
	}

	private void RunUpdate()
	{
		if (this.IsDead())
		{
			return;
		}
		this.RefreshLightExposure();
		float single = this.Happiness();
		this.realAge += this.thinkDeltaTime;
		this.stageAge = this.stageAge + this.growDeltaTime * Mathf.Max(single, 0f);
		this.growthAge = this.growthAge + this.growDeltaTime * Mathf.Max(single, 0f);
		this.growthAge = Mathf.Clamp(this.growthAge, 0f, this.plantProperty.waterConsumptionLifetime * 60f);
		base.health = base.health + single * this.currentStage.health * this.growDeltaTime;
		if (base.health <= 0f)
		{
			this.Die(null);
			return;
		}
		if (this.stageAge > this.currentStage.lifeLength * 60f)
		{
			if (this.state == PlantProperties.State.Dying)
			{
				this.Die(null);
				return;
			}
			if (this.currentStage.nextState <= this.state)
			{
				this.seasons++;
			}
			if (this.seasons < this.plantProperty.maxSeasons)
			{
				this.BecomeState(this.currentStage.nextState, true);
			}
			else
			{
				this.BecomeState(PlantProperties.State.Dying, true);
			}
		}
		if (!this.PlacedInPlanter() || this.consumedWater >= this.plantProperty.lifetimeWaterConsumption || this.state >= PlantProperties.State.Fruiting)
		{
			this.water = this.plantProperty.maxHeldWater;
		}
		else
		{
			float single1 = this.thinkDeltaTime / (this.plantProperty.waterConsumptionLifetime * 60f) * (float)this.plantProperty.lifetimeWaterConsumption;
			int num = Mathf.CeilToInt(Mathf.Min((float)this.water, single1));
			this.water -= num;
			this.consumedWater += num;
			PlanterBox parentEntity = base.GetParentEntity() as PlanterBox;
			if (parentEntity && parentEntity.soilSaturationFraction > 0f)
			{
				int num1 = this.plantProperty.maxHeldWater - this.water;
				int num2 = parentEntity.UseWater(Mathf.Min(Mathf.CeilToInt(single1 * 10f), num1));
				this.water += num2;
			}
		}
		this.water = Mathf.Clamp(this.water, 0, this.plantProperty.maxHeldWater);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.plantEntity = Facepunch.Pool.Get<ProtoBuf.PlantEntity>();
		info.msg.plantEntity.state = (int)this.state;
		info.msg.plantEntity.age = this.growthAge;
		info.msg.plantEntity.genetics = this.genetics;
		info.msg.plantEntity.water = this.water;
		info.msg.plantEntity.totalAge = this.realAge;
		info.msg.plantEntity.growthAge = this.growthAge;
		info.msg.plantEntity.stageAge = this.stageAge;
		if (!info.forDisk)
		{
			info.msg.plantEntity.healthy = (float)this.consumedWater / (float)this.plantProperty.lifetimeWaterConsumption;
			info.msg.plantEntity.yieldFraction = (this.currentStage.resources == 0f ? 0f : this.YieldBonusScale() * (float)this.plantProperty.waterYieldBonus + this.currentStage.resources);
		}
	}

	public override void ServerInit()
	{
		if (this.genetics == -1)
		{
			this.genetics = UnityEngine.Random.Range(0, 10000);
		}
		this.groundConditions = PlantEntity.WorkoutGroundConditions(base.transform.position);
		base.ServerInit();
		base.InvokeRandomized(new Action(this.RunUpdate), this.thinkDeltaTime, this.thinkDeltaTime, this.thinkDeltaTime * 0.1f);
		base.health = 10f;
		this.ResetSeason();
	}

	public static float WorkoutGroundConditions(Vector3 pos)
	{
		if (WaterLevel.Test(pos))
		{
			return -1f;
		}
		TerrainSplat.Enum splatMaxType = (TerrainSplat.Enum)TerrainMeta.SplatMap.GetSplatMaxType(pos, -1);
		if (splatMaxType > TerrainSplat.Enum.Grass)
		{
			if (splatMaxType == TerrainSplat.Enum.Forest)
			{
				return 0.4f;
			}
			if (splatMaxType == TerrainSplat.Enum.Stones)
			{
				return -0.6f;
			}
			if (splatMaxType == TerrainSplat.Enum.Gravel)
			{
				return -0.6f;
			}
		}
		else
		{
			switch (splatMaxType)
			{
				case TerrainSplat.Enum.Dirt:
				{
					return 0.5f;
				}
				case TerrainSplat.Enum.Snow:
				{
					return -1f;
				}
				case TerrainSplat.Enum.Dirt | TerrainSplat.Enum.Snow:
				{
					break;
				}
				case TerrainSplat.Enum.Sand:
				{
					return -0.3f;
				}
				default:
				{
					if (splatMaxType == TerrainSplat.Enum.Rock)
					{
						return -0.7f;
					}
					if (splatMaxType == TerrainSplat.Enum.Grass)
					{
						return 0.5f;
					}
					break;
				}
			}
		}
		return 0.5f;
	}

	public float YieldBonusScale()
	{
		return (float)this.consumedWater / (float)this.plantProperty.lifetimeWaterConsumption;
	}
}