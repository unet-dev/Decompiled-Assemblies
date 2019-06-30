using ConVar;
using Network;
using Oxide.Core;
using Rust;
using Rust.Ai;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ThrownWeapon : AttackEntity
{
	[Header("Throw Weapon")]
	public GameObjectRef prefabToThrow;

	public float maxThrowVelocity = 10f;

	public float tumbleVelocity;

	public Vector3 overrideAngle = Vector3.zero;

	public ThrownWeapon()
	{
	}

	[IsActiveItem]
	[RPC_Server]
	private void DoDrop(BaseEntity.RPCMessage msg)
	{
		RaycastHit raycastHit;
		if (!base.HasItemAmount() || base.HasAttackCooldown())
		{
			return;
		}
		Vector3 vector3 = msg.read.Vector3();
		Vector3 vector31 = msg.read.Vector3().normalized;
		if (msg.player.isMounted || msg.player.HasParent())
		{
			vector3 = msg.player.eyes.position;
		}
		else if (!base.ValidateEyePos(msg.player, vector3))
		{
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, vector3, Quaternion.LookRotation(Vector3.up), true);
		if (baseEntity == null)
		{
			return;
		}
		if (!UnityEngine.Physics.SphereCast(new Ray(vector3, vector31), 0.05f, out raycastHit, 1.5f, 1236478737))
		{
			baseEntity.SetVelocity(vector31);
		}
		else
		{
			Vector3 vector32 = raycastHit.point;
			Vector3 vector33 = raycastHit.normal;
			BaseEntity entity = raycastHit.GetEntity();
			if (!entity || !(entity is StabilityEntity) || !(baseEntity is TimedExplosive))
			{
				baseEntity.SetVelocity(vector31);
			}
			else
			{
				entity = entity.ToServer<BaseEntity>();
				TimedExplosive timedExplosive = baseEntity as TimedExplosive;
				timedExplosive.onlyDamageParent = true;
				timedExplosive.DoStick(vector32, vector33, entity);
			}
		}
		baseEntity.creatorEntity = msg.player;
		baseEntity.Spawn();
		base.StartAttackCooldown(this.repeatDelay);
		Interface.CallHook("OnExplosiveDropped", msg.player, baseEntity, this);
		base.UseItemAmount(1);
	}

	[IsActiveItem]
	[RPC_Server]
	private void DoThrow(BaseEntity.RPCMessage msg)
	{
		Sensation sensation;
		if (!base.HasItemAmount() || base.HasAttackCooldown())
		{
			return;
		}
		Vector3 vector3 = msg.read.Vector3();
		Vector3 vector31 = msg.read.Vector3().normalized;
		float single = Mathf.Clamp01(msg.read.Float());
		if (msg.player.isMounted || msg.player.HasParent())
		{
			vector3 = msg.player.eyes.position;
		}
		else if (!base.ValidateEyePos(msg.player, vector3))
		{
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, vector3, Quaternion.LookRotation((this.overrideAngle == Vector3.zero ? -vector31 : this.overrideAngle)), true);
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.creatorEntity = msg.player;
		baseEntity.SetVelocity((msg.player.GetInheritedThrowVelocity() + ((vector31 * this.maxThrowVelocity) * single)) + (msg.player.estimatedVelocity * 0.5f));
		if (this.tumbleVelocity > 0f)
		{
			baseEntity.SetAngularVelocity(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * this.tumbleVelocity);
		}
		baseEntity.Spawn();
		base.StartAttackCooldown(this.repeatDelay);
		Interface.CallHook("OnExplosiveThrown", msg.player, baseEntity, this);
		base.UseItemAmount(1);
		BasePlayer basePlayer = msg.player;
		if (basePlayer != null)
		{
			TimedExplosive timedExplosive = baseEntity as TimedExplosive;
			if (timedExplosive != null)
			{
				float single1 = 0f;
				foreach (DamageTypeEntry damageType in timedExplosive.damageTypes)
				{
					single1 += damageType.amount;
				}
				sensation = new Sensation()
				{
					Type = SensationType.ThrownWeapon,
					Position = basePlayer.transform.position,
					Radius = 50f,
					DamagePotential = single1,
					InitiatorPlayer = basePlayer,
					Initiator = basePlayer,
					UsedEntity = timedExplosive
				};
				Sense.Stimulate(sensation);
				return;
			}
			sensation = new Sensation()
			{
				Type = SensationType.ThrownWeapon,
				Position = basePlayer.transform.position,
				Radius = 50f,
				DamagePotential = 0f,
				InitiatorPlayer = basePlayer,
				Initiator = basePlayer,
				UsedEntity = this
			};
			Sense.Stimulate(sensation);
		}
	}

	private float GetThrowVelocity(Vector3 throwPos, Vector3 targetPos, Vector3 aimDir)
	{
		Vector3 vector3 = targetPos - throwPos;
		Vector2 vector2 = new Vector2(vector3.x, vector3.z);
		float single = vector2.magnitude;
		float single1 = vector3.y;
		vector2 = new Vector2(aimDir.x, aimDir.z);
		float single2 = vector2.magnitude;
		float single3 = aimDir.y;
		float single4 = UnityEngine.Physics.gravity.y;
		return Mathf.Sqrt(0.5f * single4 * single * single / (single2 * (single2 * single1 - single3 * single)));
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("ThrownWeapon.OnRpcMessage", 0.1f))
		{
			if (rpc == 1513023343 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoDrop "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("DoDrop", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoDrop", this, player))
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
							this.DoDrop(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in DoDrop");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != 1974840882 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - DoThrow "));
				}
				using (timeWarning1 = TimeWarning.New("DoThrow", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("DoThrow", this, player))
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
							this.DoThrow(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in DoThrow");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public void ServerThrow(Vector3 targetPosition)
	{
		Sensation sensation;
		if (base.isClient)
		{
			return;
		}
		if (!base.HasItemAmount() || base.HasAttackCooldown())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		Vector3 vector3 = ownerPlayer.eyes.position;
		Vector3 vector31 = ownerPlayer.eyes.BodyForward();
		float single = 1f;
		base.SignalBroadcast(BaseEntity.Signal.Throw, string.Empty, null);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.prefabToThrow.resourcePath, vector3, Quaternion.LookRotation((this.overrideAngle == Vector3.zero ? -vector31 : this.overrideAngle)), true);
		if (baseEntity == null)
		{
			return;
		}
		baseEntity.creatorEntity = ownerPlayer;
		Vector3 vector32 = vector31 + (Quaternion.AngleAxis(10f, Vector3.right) * Vector3.up);
		float throwVelocity = this.GetThrowVelocity(vector3, targetPosition, vector32);
		if (float.IsNaN(throwVelocity))
		{
			vector32 = vector31 + (Quaternion.AngleAxis(20f, Vector3.right) * Vector3.up);
			throwVelocity = this.GetThrowVelocity(vector3, targetPosition, vector32);
			if (float.IsNaN(throwVelocity))
			{
				throwVelocity = 5f;
			}
		}
		baseEntity.SetVelocity((vector32 * throwVelocity) * single);
		if (this.tumbleVelocity > 0f)
		{
			baseEntity.SetAngularVelocity(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * this.tumbleVelocity);
		}
		baseEntity.Spawn();
		base.StartAttackCooldown(this.repeatDelay);
		base.UseItemAmount(1);
		TimedExplosive timedExplosive = baseEntity as TimedExplosive;
		if (timedExplosive == null)
		{
			sensation = new Sensation()
			{
				Type = SensationType.ThrownWeapon,
				Position = ownerPlayer.transform.position,
				Radius = 50f,
				DamagePotential = 0f,
				InitiatorPlayer = ownerPlayer,
				Initiator = ownerPlayer,
				UsedEntity = this
			};
			Sense.Stimulate(sensation);
			return;
		}
		float single1 = 0f;
		foreach (DamageTypeEntry damageType in timedExplosive.damageTypes)
		{
			single1 += damageType.amount;
		}
		sensation = new Sensation()
		{
			Type = SensationType.ThrownWeapon,
			Position = ownerPlayer.transform.position,
			Radius = 50f,
			DamagePotential = single1,
			InitiatorPlayer = ownerPlayer,
			Initiator = ownerPlayer,
			UsedEntity = timedExplosive
		};
		Sense.Stimulate(sensation);
	}
}