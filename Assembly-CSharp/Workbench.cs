using ConVar;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Workbench : StorageContainer
{
	public const int blueprintSlot = 0;

	public const int experimentSlot = 1;

	public int Workbenchlevel;

	public LootSpawn experimentalItems;

	public GameObjectRef experimentStartEffect;

	public GameObjectRef experimentSuccessEffect;

	public ItemDefinition experimentResource;

	public static ItemDefinition blueprintBaseDef;

	private ItemDefinition pendingBlueprint;

	private bool creatingBlueprint;

	public Workbench()
	{
	}

	public void ExperimentComplete()
	{
		Item experimentResourceItem = this.GetExperimentResourceItem();
		int scrapForExperiment = this.GetScrapForExperiment();
		if (this.pendingBlueprint == null)
		{
			Debug.LogWarning("Pending blueprint was null!");
		}
		if (experimentResourceItem != null && experimentResourceItem.amount >= scrapForExperiment && this.pendingBlueprint != null)
		{
			experimentResourceItem.UseItem(scrapForExperiment);
			Item item = ItemManager.Create(Workbench.GetBlueprintTemplate(), 1, (ulong)0);
			item.blueprintTarget = this.pendingBlueprint.itemid;
			this.creatingBlueprint = true;
			if (!item.MoveToContainer(this.inventory, 0, true))
			{
				item.Drop(this.GetDropPosition(), this.GetDropVelocity(), new Quaternion());
			}
			this.creatingBlueprint = false;
			if (this.experimentSuccessEffect.isValid)
			{
				Effect.server.Run(this.experimentSuccessEffect.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
			}
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.pendingBlueprint = null;
		this.inventory.SetLocked(false);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public int GetAvailableExperimentResources()
	{
		Item experimentResourceItem = this.GetExperimentResourceItem();
		if (experimentResourceItem == null || experimentResourceItem.info != this.experimentResource)
		{
			return 0;
		}
		return experimentResourceItem.amount;
	}

	public static ItemDefinition GetBlueprintTemplate()
	{
		if (Workbench.blueprintBaseDef == null)
		{
			Workbench.blueprintBaseDef = ItemManager.FindItemDefinition("blueprintbase");
		}
		return Workbench.blueprintBaseDef;
	}

	public Item GetExperimentResourceItem()
	{
		return this.inventory.GetSlot(1);
	}

	public int GetScrapForExperiment()
	{
		if (this.Workbenchlevel == 1)
		{
			return 75;
		}
		if (this.Workbenchlevel == 2)
		{
			return 300;
		}
		if (this.Workbenchlevel == 3)
		{
			return 1000;
		}
		Debug.LogWarning("GetScrapForExperiment fucked up big time.");
		return 0;
	}

	public bool IsWorking()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (targetSlot == 1 && item.info == this.experimentResource || targetSlot == 0 && this.creatingBlueprint)
		{
			return true;
		}
		return false;
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		base.CancelInvoke(new Action(this.ExperimentComplete));
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("Workbench.OnRpcMessage", 0.1f))
		{
			if (rpc == -1986172535 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_BeginExperiment "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_BeginExperiment", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_BeginExperiment", this, player, 3f))
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
							this.RPC_BeginExperiment(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_BeginExperiment");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != 2051750736 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Rotate "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_Rotate", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Rotate", this, player, 3f))
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
							this.RPC_Rotate(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in RPC_Rotate");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (this.inventory != null)
		{
			this.inventory.SetLocked(false);
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_BeginExperiment(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer == null)
		{
			return;
		}
		if (this.IsWorking())
		{
			return;
		}
		PersistantPlayer playerInfo = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerInfo(basePlayer.userID);
		int num = UnityEngine.Random.Range(0, (int)this.experimentalItems.subSpawn.Length);
		int num1 = 0;
		while (num1 < (int)this.experimentalItems.subSpawn.Length)
		{
			int length = num1 + num;
			if (length >= (int)this.experimentalItems.subSpawn.Length)
			{
				length -= (int)this.experimentalItems.subSpawn.Length;
			}
			ItemDefinition itemDefinition = this.experimentalItems.subSpawn[length].category.items[0].itemDef;
			if (!itemDefinition.Blueprint || itemDefinition.Blueprint.defaultBlueprint || !itemDefinition.Blueprint.userCraftable || !itemDefinition.Blueprint.isResearchable || itemDefinition.Blueprint.NeedsSteamItem || playerInfo.unlockedItems.Contains(itemDefinition.itemid))
			{
				num1++;
			}
			else
			{
				this.pendingBlueprint = itemDefinition;
				break;
			}
		}
		if (this.pendingBlueprint == null)
		{
			basePlayer.ChatMessage("You have already unlocked everything for this workbench tier.");
			return;
		}
		if (Interface.CallHook("CanExperiment", basePlayer, this) != null)
		{
			return;
		}
		Item slot = this.inventory.GetSlot(0);
		if (slot != null)
		{
			if (!slot.MoveToContainer(basePlayer.inventory.containerMain, -1, true))
			{
				slot.Drop(this.GetDropPosition(), this.GetDropVelocity(), new Quaternion());
			}
			basePlayer.inventory.loot.SendImmediate();
		}
		if (this.experimentStartEffect.isValid)
		{
			Effect.server.Run(this.experimentStartEffect.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.inventory.SetLocked(true);
		base.CancelInvoke(new Action(this.ExperimentComplete));
		base.Invoke(new Action(this.ExperimentComplete), 5f);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_Rotate(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer.CanBuild() && basePlayer.GetHeldEntity() && basePlayer.GetHeldEntity().GetComponent<Hammer>() != null)
		{
			base.transform.rotation = Quaternion.LookRotation(-base.transform.forward, base.transform.up);
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			Deployable component = base.GetComponent<Deployable>();
			if (component != null)
			{
				Effect.server.Run(component.placeEffect.resourcePath, base.transform.position, Vector3.up, null, false);
			}
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		Workbench workbench = this;
		this.inventory.canAcceptItem = new Func<Item, int, bool>(workbench.ItemFilter);
	}
}