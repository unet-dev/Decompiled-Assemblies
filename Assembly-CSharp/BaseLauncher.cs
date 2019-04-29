using ConVar;
using Network;
using Oxide.Core;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseLauncher : BaseProjectile
{
	public BaseLauncher()
	{
	}

	public override bool ForceSendMagazine()
	{
		return true;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("BaseLauncher.OnRpcMessage", 0.1f))
		{
			if (rpc != 853319324 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SV_Launch "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("SV_Launch", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("SV_Launch", this, player))
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
							this.SV_Launch(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in SV_Launch");
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
	private void SV_Launch(BaseEntity.RPCMessage msg)
	{
		RaycastHit raycastHit;
		BasePlayer basePlayer = msg.player;
		if (!this.VerifyClientAttack(basePlayer))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (this.reloadFinished && base.HasReloadCooldown())
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Reloading (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "reload_cooldown");
			return;
		}
		this.reloadStarted = false;
		this.reloadFinished = false;
		if (this.primaryMagazine.contents <= 0)
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Magazine empty (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "magazine_empty");
			return;
		}
		this.primaryMagazine.contents--;
		base.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, basePlayer.net.connection);
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
		ItemModProjectile component = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>();
		if (!component)
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Item mod not found (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "mod_missing");
			return;
		}
		float aimCone = this.GetAimCone() + component.projectileSpread;
		if (aimCone > 0f)
		{
			modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, modifiedAimConeDirection, true);
		}
		float single = 1f;
		if (UnityEngine.Physics.Raycast(vector3, modifiedAimConeDirection, out raycastHit, single, 1236478737))
		{
			single = raycastHit.distance - 0.1f;
		}
		GameManager gameManager = GameManager.server;
		string str = component.projectileObject.resourcePath;
		Vector3 vector31 = vector3 + (modifiedAimConeDirection * single);
		Quaternion quaternion = new Quaternion();
		BaseEntity baseEntity = gameManager.CreateEntity(str, vector31, quaternion, true);
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.creatorEntity = basePlayer;
		ServerProjectile serverProjectile = baseEntity.GetComponent<ServerProjectile>();
		if (serverProjectile)
		{
			serverProjectile.InitializeVelocity(basePlayer.GetInheritedProjectileVelocity() + (modifiedAimConeDirection * serverProjectile.speed));
		}
		baseEntity.Spawn();
		base.StartAttackCooldown(base.ScaleRepeatDelay(this.repeatDelay));
		Interface.CallHook("OnRocketLaunched", basePlayer, baseEntity);
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(UnityEngine.Random.Range(1f, 2f));
	}
}