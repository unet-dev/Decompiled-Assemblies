using ConVar;
using Network;
using Oxide.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class Planner : HeldEntity
{
	public BaseEntity[] buildableList;

	public bool isTypeDeployable
	{
		get
		{
			return this.GetModDeployable() != null;
		}
	}

	public Planner()
	{
	}

	public bool CanAffordToPlace(Construction component)
	{
		bool flag;
		if (this.isTypeDeployable)
		{
			return true;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return false;
		}
		List<ItemAmount>.Enumerator enumerator = component.defaultGrade.costToBuild.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				ItemAmount current = enumerator.Current;
				if ((float)ownerPlayer.inventory.GetAmount(current.itemDef.itemid) >= current.amount)
				{
					continue;
				}
				flag = false;
				return flag;
			}
			return true;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return flag;
	}

	public void DoBuild(CreateBuilding msg)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack(1f, Single.PositiveInfinity))
		{
			ownerPlayer.ChatMessage("AntiHack!");
			return;
		}
		Construction construction = PrefabAttribute.server.Find<Construction>(msg.blockID);
		if (construction == null)
		{
			ownerPlayer.ChatMessage(string.Concat("Couldn't find Construction ", msg.blockID));
			return;
		}
		if (!this.CanAffordToPlace(construction))
		{
			ownerPlayer.ChatMessage("Can't afford to place!");
			return;
		}
		if (!ownerPlayer.CanBuild() && !construction.canBypassBuildingPermission)
		{
			ownerPlayer.ChatMessage("Building is blocked!");
			return;
		}
		Deployable deployable = this.GetDeployable();
		Construction.Target target = new Construction.Target();
		BaseEntity baseEntity = null;
		if (msg.entity > 0)
		{
			baseEntity = BaseNetworkable.serverEntities.Find(msg.entity) as BaseEntity;
			if (!baseEntity)
			{
				ownerPlayer.ChatMessage(string.Concat("Couldn't find entity ", msg.entity));
				return;
			}
			msg.position = baseEntity.transform.TransformPoint(msg.position);
			msg.normal = baseEntity.transform.TransformDirection(msg.normal);
			msg.rotation = baseEntity.transform.rotation * msg.rotation;
			if (msg.socket == 0)
			{
				if (deployable && deployable.setSocketParent && baseEntity.Distance(msg.position) > 1f)
				{
					ownerPlayer.ChatMessage(string.Concat("Parent too far away: ", baseEntity.Distance(msg.position)));
					return;
				}
				if (baseEntity is Door)
				{
					ownerPlayer.ChatMessage("Can't deploy on door");
					return;
				}
			}
			target.entity = baseEntity;
			if (msg.socket > 0)
			{
				string str = StringPool.Get(msg.socket);
				if (!(str != "") || !(target.entity != null))
				{
					ownerPlayer.ChatMessage("Invalid Socket!");
				}
				else
				{
					target.socket = this.FindSocket(str, target.entity.prefabID);
				}
			}
		}
		target.ray = msg.ray;
		target.onTerrain = msg.onterrain;
		target.position = msg.position;
		target.normal = msg.normal;
		target.rotation = msg.rotation;
		target.player = ownerPlayer;
		target.valid = true;
		if (Interface.CallHook("CanBuild", this, construction, target) != null)
		{
			return;
		}
		if (deployable && deployable.placeEffect.isValid)
		{
			if (!baseEntity || msg.socket <= 0)
			{
				Effect.server.Run(deployable.placeEffect.resourcePath, msg.position, msg.normal, null, false);
			}
			else
			{
				Effect.server.Run(deployable.placeEffect.resourcePath, baseEntity.transform.TransformPoint(target.socket.worldPosition), baseEntity.transform.up, null, false);
			}
		}
		this.DoBuild(target, construction);
	}

	public void DoBuild(Construction.Target target, Construction component)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (target.ray.IsNaNOrInfinity())
		{
			return;
		}
		if (target.position.IsNaNOrInfinity())
		{
			return;
		}
		if (target.normal.IsNaNOrInfinity())
		{
			return;
		}
		if (target.socket != null)
		{
			if (!target.socket.female)
			{
				ownerPlayer.ChatMessage(string.Concat("Target socket is not female. (", target.socket.socketName, ")"));
				return;
			}
			if (target.entity != null && target.entity.IsOccupied(target.socket))
			{
				ownerPlayer.ChatMessage(string.Concat("Target socket is occupied. (", target.socket.socketName, ")"));
				return;
			}
		}
		else if (ConVar.AntiHack.eye_protection >= 2 && !GamePhysics.LineOfSight(ownerPlayer.eyes.center, ownerPlayer.eyes.position, target.ray.origin, target.position, 2162688, 0.01f))
		{
			ownerPlayer.ChatMessage("Line of sight blocked.");
			return;
		}
		Construction.lastPlacementError = "No Error";
		GameObject gameObject = this.DoPlacement(target, component);
		if (gameObject == null)
		{
			ownerPlayer.ChatMessage(string.Concat("Can't place: ", Construction.lastPlacementError));
		}
		if (gameObject != null)
		{
			Interface.CallHook("OnEntityBuilt", this, gameObject);
			Deployable deployable = this.GetDeployable();
			if (deployable != null)
			{
				BaseEntity baseEntity = gameObject.ToBaseEntity();
				if (deployable.setSocketParent && target.entity != null && target.entity.SupportsChildDeployables() && baseEntity)
				{
					baseEntity.SetParent(target.entity, true, false);
				}
				if (deployable.wantsInstanceData && base.GetOwnerItem().instanceData != null)
				{
					(baseEntity as IInstanceDataReceiver).ReceiveInstanceData(base.GetOwnerItem().instanceData);
				}
				if (deployable.copyInventoryFromItem)
				{
					StorageContainer storageContainer = baseEntity.GetComponent<StorageContainer>();
					if (storageContainer)
					{
						storageContainer.ReceiveInventoryFromItem(base.GetOwnerItem());
					}
				}
				ItemModDeployable modDeployable = this.GetModDeployable();
				if (modDeployable != null)
				{
					modDeployable.OnDeployed(baseEntity, ownerPlayer);
				}
			}
			this.PayForPlacement(ownerPlayer, component);
		}
	}

	[IsActiveItem]
	[RPC_Server]
	private void DoPlace(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		using (CreateBuilding createBuilding = CreateBuilding.Deserialize(msg.read))
		{
			this.DoBuild(createBuilding);
		}
	}

	public GameObject DoPlacement(Construction.Target placement, Construction component)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		BaseEntity baseEntity = component.CreateConstruction(placement, true);
		if (!baseEntity)
		{
			return null;
		}
		float single = 1f;
		float single1 = 0f;
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem != null)
		{
			baseEntity.skinID = ownerItem.skin;
			if (ownerItem.hasCondition)
			{
				single = ownerItem.conditionNormalized;
			}
		}
		baseEntity.gameObject.AwakeFromInstantiate();
		BuildingBlock buildingBlock = baseEntity as BuildingBlock;
		if (buildingBlock)
		{
			buildingBlock.blockDefinition = PrefabAttribute.server.Find<Construction>(buildingBlock.prefabID);
			if (!buildingBlock.blockDefinition)
			{
				Debug.LogError("Placing a building block that has no block definition!");
				return null;
			}
			buildingBlock.SetGrade(buildingBlock.blockDefinition.defaultGrade.gradeBase.type);
			single1 = buildingBlock.currentGrade.maxHealth;
		}
		BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
		if (baseCombatEntity)
		{
			single1 = (buildingBlock != null ? buildingBlock.currentGrade.maxHealth : baseCombatEntity.startHealth);
			baseCombatEntity.ResetLifeStateOnSpawn = false;
			baseCombatEntity.InitializeHealth(single1 * single, single1);
		}
		baseEntity.gameObject.SendMessage("SetDeployedBy", ownerPlayer, SendMessageOptions.DontRequireReceiver);
		baseEntity.OwnerID = ownerPlayer.userID;
		baseEntity.Spawn();
		if (buildingBlock)
		{
			Effect.server.Run("assets/bundled/prefabs/fx/build/frame_place.prefab", baseEntity, 0, Vector3.zero, Vector3.zero, null, false);
		}
		StabilityEntity stabilityEntity = baseEntity as StabilityEntity;
		if (stabilityEntity)
		{
			stabilityEntity.UpdateSurroundingEntities();
		}
		return baseEntity.gameObject;
	}

	public Socket_Base FindSocket(string name, uint prefabIDToFind)
	{
		return PrefabAttribute.server.FindAll<Socket_Base>(prefabIDToFind).FirstOrDefault<Socket_Base>((Socket_Base s) => s.socketName == name);
	}

	public Deployable GetDeployable()
	{
		ItemModDeployable modDeployable = this.GetModDeployable();
		if (modDeployable == null)
		{
			return null;
		}
		return modDeployable.GetDeployable(this);
	}

	public ItemModDeployable GetModDeployable()
	{
		ItemDefinition ownerItemDefinition = base.GetOwnerItemDefinition();
		if (ownerItemDefinition == null)
		{
			return null;
		}
		return ownerItemDefinition.GetComponent<ItemModDeployable>();
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("Planner.OnRpcMessage", 0.1f))
		{
			if (rpc != 1872774636 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoPlace "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("DoPlace", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoPlace", this, player))
						{
							flag = true;
							return flag;
						}
					}
					try
					{
						using (timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.DoPlace(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in DoPlace");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void PayForPlacement(BasePlayer player, Construction component)
	{
		if (this.isTypeDeployable)
		{
			this.GetItem().UseItem(1);
			return;
		}
		List<Item> items = new List<Item>();
		foreach (ItemAmount itemAmount in component.defaultGrade.costToBuild)
		{
			player.inventory.Take(items, itemAmount.itemDef.itemid, (int)itemAmount.amount);
			player.Command("note.inv", new object[] { itemAmount.itemDef.itemid, itemAmount.amount * -1f });
		}
		foreach (Item item in items)
		{
			item.Remove(0f);
		}
	}
}