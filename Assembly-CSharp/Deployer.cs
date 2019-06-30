using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class Deployer : HeldEntity
{
	public Deployer()
	{
	}

	public bool CheckPlacement(Deployable deployable, Ray ray, float fDistance)
	{
		RaycastHit raycastHit;
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("Deploy.CheckPlacement", 0.1f))
		{
			if (UnityEngine.Physics.Raycast(ray, out raycastHit, fDistance, 1235288065))
			{
				DeployVolume[] deployVolumeArray = PrefabAttribute.server.FindAll<DeployVolume>(deployable.prefabID);
				Vector3 vector3 = raycastHit.point;
				Quaternion deployedRotation = this.GetDeployedRotation(raycastHit.normal, ray.direction);
				if (DeployVolume.Check(vector3, deployedRotation, deployVolumeArray, -1))
				{
					flag = false;
				}
				else if (this.IsPlacementAngleAcceptable(raycastHit.point, deployedRotation))
				{
					return true;
				}
				else
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
		}
		return flag;
	}

	[IsActiveItem]
	[RPC_Server]
	private void DoDeploy(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		Deployable deployable = this.GetDeployable();
		if (deployable == null)
		{
			return;
		}
		Ray ray = msg.read.Ray();
		uint num = msg.read.UInt32();
		if (Interface.CallHook("CanDeployItem", msg.player, this, num) != null)
		{
			return;
		}
		if (!deployable.toSlot)
		{
			this.DoDeploy_Regular(deployable, ray);
			return;
		}
		this.DoDeploy_Slot(deployable, ray, num);
	}

	public void DoDeploy_Regular(Deployable deployable, Ray ray)
	{
		RaycastHit raycastHit;
		if (!base.HasItemAmount())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (!ownerPlayer.CanBuild())
		{
			ownerPlayer.ChatMessage("Building is blocked!");
			return;
		}
		if (ConVar.AntiHack.objectplacement && ownerPlayer.TriggeredAntiHack(1f, Single.PositiveInfinity))
		{
			ownerPlayer.ChatMessage("AntiHack!");
			return;
		}
		if (!this.CheckPlacement(deployable, ray, 8f))
		{
			return;
		}
		if (!UnityEngine.Physics.Raycast(ray, out raycastHit, 8f, 1235288065))
		{
			return;
		}
		Quaternion deployedRotation = this.GetDeployedRotation(raycastHit.normal, ray.direction);
		Item ownerItem = base.GetOwnerItem();
		ItemModDeployable modDeployable = this.GetModDeployable();
		BaseEntity baseEntity = GameManager.server.CreateEntity(modDeployable.entityPrefab.resourcePath, raycastHit.point, deployedRotation, true);
		if (!baseEntity)
		{
			Debug.LogWarning(string.Concat("Couldn't create prefab:", modDeployable.entityPrefab.resourcePath));
			return;
		}
		baseEntity.skinID = ownerItem.skin;
		baseEntity.SendMessage("SetDeployedBy", ownerPlayer, SendMessageOptions.DontRequireReceiver);
		baseEntity.OwnerID = ownerPlayer.userID;
		baseEntity.Spawn();
		modDeployable.OnDeployed(baseEntity, ownerPlayer);
		Interface.CallHook("OnItemDeployed", this, baseEntity);
		base.UseItemAmount(1);
	}

	public void DoDeploy_Slot(Deployable deployable, Ray ray, uint entityID)
	{
		if (!base.HasItemAmount())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (!ownerPlayer.CanBuild())
		{
			return;
		}
		BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(entityID) as BaseEntity;
		if (baseEntity == null)
		{
			return;
		}
		if (!baseEntity.HasSlot(deployable.slot))
		{
			return;
		}
		if (baseEntity.GetSlot(deployable.slot) != null)
		{
			return;
		}
		Item ownerItem = base.GetOwnerItem();
		ItemModDeployable modDeployable = this.GetModDeployable();
		GameManager gameManager = GameManager.server;
		string str = modDeployable.entityPrefab.resourcePath;
		Vector3 vector3 = new Vector3();
		Quaternion quaternion = new Quaternion();
		BaseEntity baseEntity1 = gameManager.CreateEntity(str, vector3, quaternion, true);
		if (baseEntity1 != null)
		{
			baseEntity1.skinID = ownerItem.skin;
			baseEntity1.SetParent(baseEntity, baseEntity.GetSlotAnchorName(deployable.slot), false, false);
			baseEntity1.OwnerID = ownerPlayer.userID;
			baseEntity1.OnDeployed(baseEntity);
			baseEntity1.Spawn();
			baseEntity.SetSlot(deployable.slot, baseEntity1);
			if (deployable.placeEffect.isValid)
			{
				Effect.server.Run(deployable.placeEffect.resourcePath, baseEntity.transform.position, Vector3.up, null, false);
			}
		}
		modDeployable.OnDeployed(baseEntity1, ownerPlayer);
		Interface.CallHook("OnItemDeployed", this, baseEntity);
		base.UseItemAmount(1);
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

	public Quaternion GetDeployedRotation(Vector3 normal, Vector3 placeDir)
	{
		return Quaternion.LookRotation(normal, placeDir) * Quaternion.Euler(90f, 0f, 0f);
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

	public bool IsPlacementAngleAcceptable(Vector3 pos, Quaternion rot)
	{
		if (Mathf.Acos(Vector3.Dot(rot * Vector3.up, Vector3.up)) <= 0.610865235f)
		{
			return true;
		}
		return false;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("Deployer.OnRpcMessage", 0.1f))
		{
			if (rpc != -1293849390 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoDeploy "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("DoDeploy", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoDeploy", this, player))
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
							this.DoDeploy(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in DoDeploy");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}
}