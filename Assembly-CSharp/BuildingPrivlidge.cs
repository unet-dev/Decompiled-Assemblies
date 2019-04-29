using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class BuildingPrivlidge : StorageContainer
{
	public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	private float cachedProtectedMinutes;

	private float nextProtectedCalcTime;

	private static BuildingPrivlidge.UpkeepBracket[] upkeepBrackets;

	private List<ItemAmount> upkeepBuffer = new List<ItemAmount>();

	static BuildingPrivlidge()
	{
		BuildingPrivlidge.upkeepBrackets = new BuildingPrivlidge.UpkeepBracket[] { new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_0_blockcount, ConVar.Decay.bracket_0_costfraction), new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_1_blockcount, ConVar.Decay.bracket_1_costfraction), new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_2_blockcount, ConVar.Decay.bracket_2_costfraction), new BuildingPrivlidge.UpkeepBracket(ConVar.Decay.bracket_3_blockcount, ConVar.Decay.bracket_3_costfraction) };
	}

	public BuildingPrivlidge()
	{
	}

	public void AddDelayedUpdate()
	{
		if (base.IsInvoking(new Action(this.DelayedUpdate)))
		{
			base.CancelInvoke(new Action(this.DelayedUpdate));
		}
		base.Invoke(new Action(this.DelayedUpdate), 1f);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void AddSelfAuthorize(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		if (Interface.CallHook("OnCupboardAuthorize", this, rpc.player) != null)
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
		PlayerNameID playerNameID = new PlayerNameID()
		{
			userid = rpc.player.userID,
			username = rpc.player.displayName
		};
		this.authorizedPlayers.Add(playerNameID);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public bool AnyAuthed()
	{
		return this.authorizedPlayers.Count > 0;
	}

	private void ApplyUpkeepPayment()
	{
		List<Item> list = Facepunch.Pool.GetList<Item>();
		for (int i = 0; i < this.upkeepBuffer.Count; i++)
		{
			ItemAmount item = this.upkeepBuffer[i];
			int num = (int)item.amount;
			if (num >= 1)
			{
				this.inventory.Take(list, item.itemid, num);
				foreach (Item item1 in list)
				{
					if (this.IsDebugging())
					{
						Debug.Log(string.Concat(new object[] { this.ToString(), ": Using ", item1.amount, " of ", item1.info.shortname }));
					}
					item1.UseItem(item1.amount);
				}
				list.Clear();
				item.amount -= (float)num;
				this.upkeepBuffer[i] = item;
			}
		}
		Facepunch.Pool.FreeList<Item>(ref list);
	}

	public void BuildingDirty()
	{
		if (base.isServer)
		{
			this.AddDelayedUpdate();
		}
	}

	public float CalculateBuildingTaxRate()
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building == null)
		{
			return ConVar.Decay.bracket_0_costfraction;
		}
		if (!building.HasBuildingBlocks())
		{
			return ConVar.Decay.bracket_0_costfraction;
		}
		int count = building.buildingBlocks.Count;
		int num = count;
		for (int i = 0; i < (int)BuildingPrivlidge.upkeepBrackets.Length; i++)
		{
			BuildingPrivlidge.UpkeepBracket upkeepBracket = BuildingPrivlidge.upkeepBrackets[i];
			upkeepBracket.blocksTaxPaid = 0f;
			if (num > 0)
			{
				int num1 = 0;
				num1 = (i != (int)BuildingPrivlidge.upkeepBrackets.Length - 1 ? Mathf.Min(num, BuildingPrivlidge.upkeepBrackets[i].objectsUpTo) : num);
				num -= num1;
				upkeepBracket.blocksTaxPaid = (float)num1 * upkeepBracket.fraction;
			}
		}
		float single = 0f;
		for (int j = 0; j < (int)BuildingPrivlidge.upkeepBrackets.Length; j++)
		{
			BuildingPrivlidge.UpkeepBracket upkeepBracket1 = BuildingPrivlidge.upkeepBrackets[j];
			if (upkeepBracket1.blocksTaxPaid <= 0f)
			{
				break;
			}
			single += upkeepBracket1.blocksTaxPaid;
		}
		single /= (float)count;
		return single;
	}

	public void CalculateUpkeepCostAmounts(List<ItemAmount> itemAmounts)
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building == null)
		{
			return;
		}
		if (!building.HasDecayEntities())
		{
			return;
		}
		float single = this.CalculateUpkeepCostFraction();
		foreach (DecayEntity decayEntity in building.decayEntities)
		{
			decayEntity.CalculateUpkeepCostAmounts(itemAmounts, single);
		}
	}

	public float CalculateUpkeepCostFraction()
	{
		if (!base.isServer)
		{
			return 0f;
		}
		return this.CalculateBuildingTaxRate();
	}

	public float CalculateUpkeepPeriodMinutes()
	{
		if (base.isServer)
		{
			return ConVar.Decay.upkeep_period_minutes;
		}
		return 0f;
	}

	private bool CanAdministrate(BasePlayer player)
	{
		BaseLock slot = base.GetSlot(BaseEntity.Slot.Lock) as BaseLock;
		if (slot == null)
		{
			return true;
		}
		return slot.OnTryToOpen(player);
	}

	private bool CanAffordUpkeepPayment(List<ItemAmount> itemAmounts)
	{
		for (int i = 0; i < itemAmounts.Count; i++)
		{
			ItemAmount item = itemAmounts[i];
			if ((float)this.inventory.GetAmount(item.itemid, true) < item.amount)
			{
				if (this.IsDebugging())
				{
					Debug.Log(string.Concat(new object[] { this.ToString(), ": Can't afford ", item.amount, " of ", item.itemDef.shortname }));
				}
				return false;
			}
		}
		return true;
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void ClearList(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		if (Interface.CallHook("OnCupboardClearList", this, rpc.player) != null)
		{
			return;
		}
		this.authorizedPlayers.Clear();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void DecayTick()
	{
		if (this.EnsurePrimary())
		{
			base.DecayTick();
		}
	}

	public void DelayedUpdate()
	{
		this.MarkProtectedMinutesDirty(0f);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	private bool EnsurePrimary()
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building != null)
		{
			BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
			if (dominatingBuildingPrivilege != null && dominatingBuildingPrivilege != this)
			{
				base.Kill(BaseNetworkable.DestroyMode.Gib);
				return false;
			}
		}
		return true;
	}

	public float GetProtectedMinutes(bool force = false)
	{
		if (!base.isServer)
		{
			return 0f;
		}
		if (!force && UnityEngine.Time.realtimeSinceStartup < this.nextProtectedCalcTime)
		{
			return this.cachedProtectedMinutes;
		}
		this.nextProtectedCalcTime = UnityEngine.Time.realtimeSinceStartup + 60f;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		this.CalculateUpkeepCostAmounts(list);
		float single = this.CalculateUpkeepPeriodMinutes();
		float single1 = -1f;
		if (this.inventory != null)
		{
			foreach (ItemAmount itemAmount in list)
			{
				int num = this.inventory.FindItemsByItemID(itemAmount.itemid).Sum<Item>((Item x) => x.amount);
				if (num <= 0 || itemAmount.amount <= 0f)
				{
					single1 = 0f;
				}
				else
				{
					float single2 = (float)num / itemAmount.amount * single;
					if (single1 != -1f && single2 >= single1)
					{
						continue;
					}
					single1 = single2;
				}
			}
			if (single1 == -1f)
			{
				single1 = 0f;
			}
		}
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		this.cachedProtectedMinutes = single1;
		return this.cachedProtectedMinutes;
	}

	public override bool HasSlot(BaseEntity.Slot slot)
	{
		if (slot == BaseEntity.Slot.Lock)
		{
			return true;
		}
		return base.HasSlot(slot);
	}

	public bool IsAuthed(BasePlayer player)
	{
		return this.authorizedPlayers.Any<PlayerNameID>((PlayerNameID x) => x.userid == player.userID);
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		return base.ItemFilter(item, targetSlot);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.authorizedPlayers.Clear();
		if (info.msg.buildingPrivilege != null && info.msg.buildingPrivilege.users != null)
		{
			this.authorizedPlayers = info.msg.buildingPrivilege.users;
			if (!info.fromDisk)
			{
				this.cachedProtectedMinutes = info.msg.buildingPrivilege.protectedMinutes;
			}
			info.msg.buildingPrivilege.users = null;
		}
	}

	public void MarkProtectedMinutesDirty(float delay = 0f)
	{
		this.nextProtectedCalcTime = UnityEngine.Time.realtimeSinceStartup + delay;
	}

	protected override void OnInventoryDirty()
	{
		base.OnInventoryDirty();
		this.AddDelayedUpdate();
	}

	public override void OnItemAddedOrRemoved(Item item, bool bAdded)
	{
		base.OnItemAddedOrRemoved(item, bAdded);
		this.AddDelayedUpdate();
	}

	public override void OnKilled(HitInfo info)
	{
		if (ConVar.Decay.upkeep_grief_protection > 0f)
		{
			this.PurchaseUpkeepTime(ConVar.Decay.upkeep_grief_protection * 60f);
		}
		base.OnKilled(info);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("BuildingPrivlidge.OnRpcMessage", 0.1f))
		{
			if (rpc == 1092560690 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - AddSelfAuthorize "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("AddSelfAuthorize", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("AddSelfAuthorize", this, player, 3f))
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
							this.AddSelfAuthorize(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in AddSelfAuthorize");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == 253307592 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ClearList "));
				}
				using (timeWarning1 = TimeWarning.New("ClearList", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("ClearList", this, player, 3f))
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
							this.ClearList(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in ClearList");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc == -676981327 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RemoveSelfAuthorize "));
				}
				using (timeWarning1 = TimeWarning.New("RemoveSelfAuthorize", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RemoveSelfAuthorize", this, player, 3f))
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
							this.RemoveSelfAuthorize(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in RemoveSelfAuthorize");
						Debug.LogException(exception2);
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
					catch (Exception exception3)
					{
						player.Kick("RPC Error in RPC_Rotate");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override void PostSave(BaseNetworkable.SaveInfo info)
	{
		info.msg.buildingPrivilege.users = null;
	}

	public float PurchaseUpkeepTime(DecayEntity entity, float deltaTime)
	{
		float single = this.CalculateUpkeepPeriodMinutes() * 60f;
		float single1 = this.CalculateUpkeepCostFraction() * deltaTime / single;
		List<ItemAmount> list = Facepunch.Pool.GetList<ItemAmount>();
		entity.CalculateUpkeepCostAmounts(list, single1);
		bool flag = this.CanAffordUpkeepPayment(list);
		this.QueueUpkeepPayment(list);
		Facepunch.Pool.FreeList<ItemAmount>(ref list);
		this.ApplyUpkeepPayment();
		if (!flag)
		{
			return 0f;
		}
		return deltaTime;
	}

	public void PurchaseUpkeepTime(float deltaTime)
	{
		BuildingManager.Building building = base.GetBuilding();
		if (building != null && building.HasDecayEntities())
		{
			float single = Mathf.Min(this.GetProtectedMinutes(true) * 60f, deltaTime);
			if (single > 0f)
			{
				foreach (DecayEntity decayEntity in building.decayEntities)
				{
					float protectedSeconds = decayEntity.GetProtectedSeconds();
					if (single <= protectedSeconds)
					{
						continue;
					}
					float single1 = this.PurchaseUpkeepTime(decayEntity, single - protectedSeconds);
					decayEntity.AddUpkeepTime(single1);
					if (!this.IsDebugging())
					{
						continue;
					}
					Debug.Log(string.Concat(new object[] { this.ToString(), " purchased upkeep time for ", decayEntity.ToString(), ": ", protectedSeconds, " + ", single1, " = ", decayEntity.GetProtectedSeconds() }));
				}
			}
		}
	}

	private void QueueUpkeepPayment(List<ItemAmount> itemAmounts)
	{
		for (int i = 0; i < itemAmounts.Count; i++)
		{
			ItemAmount item = itemAmounts[i];
			bool flag = false;
			foreach (ItemAmount itemAmount in this.upkeepBuffer)
			{
				if (itemAmount.itemDef != item.itemDef)
				{
					continue;
				}
				itemAmount.amount += item.amount;
				if (this.IsDebugging())
				{
					Debug.Log(string.Concat(new object[] { this.ToString(), ": Adding ", item.amount, " of ", item.itemDef.shortname, " to ", itemAmount.amount }));
				}
				flag = true;
				goto Label0;
			}
		Label0:
			if (!flag)
			{
				if (this.IsDebugging())
				{
					Debug.Log(string.Concat(new object[] { this.ToString(), ": Adding ", item.amount, " of ", item.itemDef.shortname }));
				}
				this.upkeepBuffer.Add(new ItemAmount(item.itemDef, item.amount));
			}
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RemoveSelfAuthorize(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanAdministrate(rpc.player))
		{
			return;
		}
		if (Interface.CallHook("OnCupboardDeauthorize", this, rpc.player) != null)
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void ResetState()
	{
		base.ResetState();
		this.authorizedPlayers.Clear();
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_Rotate(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (basePlayer.CanBuild() && basePlayer.GetHeldEntity() && basePlayer.GetHeldEntity().GetComponent<Hammer>() != null && (base.GetSlot(BaseEntity.Slot.Lock) == null || !base.GetSlot(BaseEntity.Slot.Lock).IsLocked()))
		{
			base.transform.rotation = Quaternion.LookRotation(-base.transform.forward, base.transform.up);
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			Deployable component = base.GetComponent<Deployable>();
			if (component != null && component.placeEffect.isValid)
			{
				Effect.server.Run(component.placeEffect.resourcePath, base.transform.position, Vector3.up, null, false);
			}
		}
		BaseEntity slot = base.GetSlot(BaseEntity.Slot.Lock);
		if (slot != null)
		{
			slot.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.buildingPrivilege = Facepunch.Pool.Get<BuildingPrivilege>();
		info.msg.buildingPrivilege.users = this.authorizedPlayers;
		if (!info.forDisk)
		{
			info.msg.buildingPrivilege.upkeepPeriodMinutes = this.CalculateUpkeepPeriodMinutes();
			info.msg.buildingPrivilege.costFraction = this.CalculateUpkeepCostFraction();
			info.msg.buildingPrivilege.protectedMinutes = this.GetProtectedMinutes(false);
		}
	}

	public class UpkeepBracket
	{
		public int objectsUpTo;

		public float fraction;

		public float blocksTaxPaid;

		public UpkeepBracket(int numObjs, float frac)
		{
			this.objectsUpTo = numObjs;
			this.fraction = frac;
			this.blocksTaxPaid = 0f;
		}
	}
}