using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ResearchTable : StorageContainer
{
	[NonSerialized]
	public float researchFinishedTime;

	public float researchCostFraction = 1f;

	public float researchDuration = 10f;

	public int requiredPaper = 10;

	public GameObjectRef researchStartEffect;

	public GameObjectRef researchFailEffect;

	public GameObjectRef researchSuccessEffect;

	public ItemDefinition researchResource;

	public BasePlayer user;

	public static ItemDefinition blueprintBaseDef;

	public ResearchTable()
	{
	}

	public void CancelResearch()
	{
	}

	[RPC_Server]
	public void DoResearch(BaseEntity.RPCMessage msg)
	{
		if (this.IsResearching())
		{
			return;
		}
		BasePlayer basePlayer = msg.player;
		Item targetItem = this.GetTargetItem();
		if (targetItem == null)
		{
			return;
		}
		if (Interface.CallHook("CanResearchItem", basePlayer, targetItem) != null)
		{
			return;
		}
		if (targetItem.amount > 1)
		{
			return;
		}
		if (!this.IsItemResearchable(targetItem))
		{
			return;
		}
		Interface.CallHook("OnItemResearch", this, targetItem, basePlayer);
		targetItem.CollectedForCrafting(basePlayer);
		this.researchFinishedTime = UnityEngine.Time.realtimeSinceStartup + this.researchDuration;
		base.Invoke(new Action(this.ResearchAttemptFinished), this.researchDuration);
		this.inventory.SetLocked(true);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		basePlayer.inventory.loot.SendImmediate();
		if (this.researchStartEffect.isValid)
		{
			Effect.server.Run(this.researchStartEffect.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
		}
		msg.player.GiveAchievement("RESEARCH_ITEM");
	}

	public void EndResearch()
	{
		this.inventory.SetLocked(false);
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.researchFinishedTime = 0f;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		if (this.user != null)
		{
			this.user.inventory.loot.SendImmediate();
		}
	}

	public int GetBlueprintStacksize(Item sourceItem)
	{
		int num = this.RarityMultiplier(sourceItem.info.rarity);
		if (sourceItem.info.category == ItemCategory.Ammunition)
		{
			num = Mathf.FloorToInt((float)sourceItem.info.stackable / (float)sourceItem.info.Blueprint.amountToCreate) * 2;
		}
		return num;
	}

	public static ItemDefinition GetBlueprintTemplate()
	{
		if (ResearchTable.blueprintBaseDef == null)
		{
			ResearchTable.blueprintBaseDef = ItemManager.FindItemDefinition("blueprintbase");
		}
		return ResearchTable.blueprintBaseDef;
	}

	public Item GetScrapItem()
	{
		Item slot = this.inventory.GetSlot(1);
		if (slot.info != this.researchResource)
		{
			return null;
		}
		return slot;
	}

	public Item GetTargetItem()
	{
		return this.inventory.GetSlot(0);
	}

	public bool IsItemResearchable(Item item)
	{
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(item.info);
		if (!(itemBlueprint == null) && itemBlueprint.isResearchable && !itemBlueprint.defaultBlueprint)
		{
			return true;
		}
		return false;
	}

	public bool IsResearching()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (targetSlot == 1 && item.info != this.researchResource)
		{
			return false;
		}
		return base.ItemFilter(item, targetSlot);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.researchTable != null)
		{
			this.researchFinishedTime = UnityEngine.Time.realtimeSinceStartup + info.msg.researchTable.researchTimeLeft;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("ResearchTable.OnRpcMessage", 0.1f))
		{
			if (rpc != -1117257201 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoResearch "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("DoResearch", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoResearch(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in DoResearch");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override bool PlayerOpenLoot(BasePlayer player)
	{
		this.user = player;
		return base.PlayerOpenLoot(player);
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		this.user = null;
		base.PlayerStoppedLooting(player);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.HasFlag(BaseEntity.Flags.On) && this.researchFinishedTime != 0f)
		{
			base.Invoke(new Action(this.ResearchAttemptFinished), this.researchFinishedTime - UnityEngine.Time.realtimeSinceStartup);
		}
		this.inventory.SetLocked(false);
	}

	public int RarityMultiplier(Rarity rarity)
	{
		if (rarity == Rarity.Common)
		{
			return 20;
		}
		if (rarity == Rarity.Uncommon)
		{
			return 15;
		}
		if (rarity == Rarity.Rare)
		{
			return 10;
		}
		return 5;
	}

	public void ResearchAttemptFinished()
	{
		Item targetItem = this.GetTargetItem();
		Item scrapItem = this.GetScrapItem();
		if (targetItem != null && scrapItem != null)
		{
			int num = this.ScrapForResearch(targetItem);
			object obj = Interface.CallHook("OnItemResearched", this, num);
			if (obj is int)
			{
				num = (int)obj;
			}
			if (scrapItem.amount >= num)
			{
				if (scrapItem.amount > num)
				{
					scrapItem.UseItem(num);
				}
				else
				{
					this.inventory.Remove(scrapItem);
					scrapItem.RemoveFromContainer();
					scrapItem.Remove(0f);
				}
				this.inventory.Remove(targetItem);
				targetItem.Remove(0f);
				Item item = ItemManager.Create(ResearchTable.GetBlueprintTemplate(), 1, (ulong)0);
				item.blueprintTarget = targetItem.info.itemid;
				if (!item.MoveToContainer(this.inventory, 0, true))
				{
					item.Drop(this.GetDropPosition(), this.GetDropVelocity(), new Quaternion());
				}
				if (this.researchSuccessEffect.isValid)
				{
					Effect.server.Run(this.researchSuccessEffect.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
				}
			}
		}
		base.SendNetworkUpdateImmediate(false);
		if (this.user != null)
		{
			this.user.inventory.loot.SendImmediate();
		}
		this.EndResearch();
	}

	public override void ResetState()
	{
		base.ResetState();
		this.researchFinishedTime = 0f;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.researchTable = Facepunch.Pool.Get<ProtoBuf.ResearchTable>();
		info.msg.researchTable.researchTimeLeft = this.researchFinishedTime - UnityEngine.Time.realtimeSinceStartup;
	}

	public int ScrapForResearch(Item item)
	{
		object obj = Interface.CallHook("OnItemScrap", this, item);
		if (obj is int)
		{
			return (int)obj;
		}
		int num = 0;
		if (item.info.rarity == Rarity.Common)
		{
			num = 20;
		}
		if (item.info.rarity == Rarity.Uncommon)
		{
			num = 75;
		}
		if (item.info.rarity == Rarity.Rare)
		{
			num = 250;
		}
		if (item.info.rarity == Rarity.VeryRare || item.info.rarity == Rarity.None)
		{
			num = 750;
		}
		return num;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		ResearchTable researchTable = this;
		this.inventory.canAcceptItem = new Func<Item, int, bool>(researchTable.ItemFilter);
	}
}