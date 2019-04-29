using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class EasterBasket : AttackEntity
{
	public GameObjectRef eggProjectile;

	public ItemDefinition ammoType;

	public EasterBasket()
	{
	}

	public Item GetAmmo()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return null;
		}
		Item item = ownerPlayer.inventory.containerMain.FindItemByItemID(this.ammoType.itemid) ?? ownerPlayer.inventory.containerBelt.FindItemByItemID(this.ammoType.itemid);
		return item;
	}

	public bool HasAmmo()
	{
		return this.GetAmmo() != null;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("EasterBasket.OnRpcMessage", 0.1f))
		{
			if (rpc != -531375841 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ThrowEgg "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("ThrowEgg", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("ThrowEgg", this, player))
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
							this.ThrowEgg(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in ThrowEgg");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[IsActiveItem]
	[RPC_Server]
	public void ThrowEgg(BaseEntity.RPCMessage msg)
	{
		RaycastHit raycastHit;
		BasePlayer basePlayer = msg.player;
		if (!this.VerifyClientAttack(basePlayer))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (!this.HasAmmo())
		{
			return;
		}
		this.UseAmmo();
		Vector3 vector3 = msg.read.Vector3();
		Vector3 modifiedAimConeDirection = msg.read.Vector3().normalized;
		bool flag = msg.read.Bit();
		BaseEntity parentEntity = basePlayer.GetParentEntity();
		if (parentEntity == null)
		{
			parentEntity = basePlayer.GetMounted();
		}
		if (flag)
		{
			if (parentEntity == null)
			{
				vector3 = basePlayer.eyes.position;
				modifiedAimConeDirection = basePlayer.eyes.BodyForward();
			}
			else
			{
				vector3 = parentEntity.transform.TransformPoint(vector3);
				modifiedAimConeDirection = parentEntity.transform.TransformDirection(modifiedAimConeDirection);
			}
		}
		if (!base.ValidateEyePos(basePlayer, vector3))
		{
			return;
		}
		float single = 2f;
		if (single > 0f)
		{
			modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(single, modifiedAimConeDirection, true);
		}
		float single1 = 1f;
		if (UnityEngine.Physics.Raycast(vector3, modifiedAimConeDirection, out raycastHit, single1, 1236478737))
		{
			single1 = raycastHit.distance - 0.1f;
		}
		GameManager gameManager = GameManager.server;
		string str = this.eggProjectile.resourcePath;
		Vector3 vector31 = vector3 + (modifiedAimConeDirection * single1);
		Quaternion quaternion = new Quaternion();
		BaseEntity baseEntity = gameManager.CreateEntity(str, vector31, quaternion, true);
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.creatorEntity = basePlayer;
		ServerProjectile component = baseEntity.GetComponent<ServerProjectile>();
		if (component)
		{
			component.InitializeVelocity(basePlayer.GetInheritedProjectileVelocity() + (modifiedAimConeDirection * component.speed));
		}
		baseEntity.Spawn();
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(UnityEngine.Random.Range(1f, 2f));
	}

	public void UseAmmo()
	{
		Item ammo = this.GetAmmo();
		if (ammo != null)
		{
			ammo.UseItem(1);
		}
	}
}