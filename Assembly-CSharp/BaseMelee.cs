using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Rust.Ai;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseMelee : AttackEntity
{
	[Header("Melee")]
	public DamageProperties damageProperties;

	public List<DamageTypeEntry> damageTypes;

	public float maxDistance = 1.5f;

	public float attackRadius = 0.3f;

	public bool isAutomatic = true;

	public bool blockSprintOnAttack = true;

	[Header("Effects")]
	public GameObjectRef strikeFX;

	public bool useStandardHitEffects = true;

	[Header("NPCUsage")]
	public float aiStrikeDelay = 0.2f;

	public GameObjectRef swingEffect;

	public List<BaseMelee.MaterialFX> materialStrikeFX = new List<BaseMelee.MaterialFX>();

	[Header("Other")]
	[Range(0f, 1f)]
	public float heartStress = 0.5f;

	public ResourceDispenser.GatherProperties gathering;

	[Header("Throwing")]
	public bool canThrowAsProjectile;

	public bool canAiHearIt;

	public bool onlyThrowAsProjectile;

	public BaseMelee()
	{
	}

	public override bool CanBeUsedInWater()
	{
		return true;
	}

	public virtual bool CanHit(HitTest info)
	{
		return true;
	}

	[FromOwner]
	[RPC_Server]
	private void CLProject(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!this.VerifyClientAttack(basePlayer))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		if (basePlayer == null)
		{
			return;
		}
		if (basePlayer.IsHeadUnderwater())
		{
			return;
		}
		if (!this.canThrowAsProjectile)
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Not throwable (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "not_throwable");
			return;
		}
		Item item = this.GetItem();
		if (item == null)
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Item not found (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "item_missing");
			return;
		}
		ItemModProjectile component = item.info.GetComponent<ItemModProjectile>();
		if (component == null)
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Item mod not found (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "mod_missing");
			return;
		}
		ProjectileShoot projectileShoot = ProjectileShoot.Deserialize(msg.read);
		if (projectileShoot.projectiles.Count != 1)
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Projectile count mismatch (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "count_mismatch");
			return;
		}
		basePlayer.CleanupExpiredProjectiles();
		foreach (ProjectileShoot.Projectile projectile in projectileShoot.projectiles)
		{
			if (!basePlayer.HasFiredProjectile(projectile.projectileID))
			{
				if (!base.ValidateEyePos(basePlayer, projectile.startPos))
				{
					continue;
				}
				basePlayer.NoteFiredProjectile(projectile.projectileID, projectile.startPos, projectile.startVel, this, item.info, item);
				Effect effect = new Effect();
				effect.Init(Effect.Type.Projectile, projectile.startPos, projectile.startVel, msg.connection);
				effect.scale = 1f;
				effect.pooledString = component.projectileObject.resourcePath;
				effect.number = projectile.seed;
				EffectNetwork.Send(effect);
			}
			else
			{
				AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Duplicate ID (", projectile.projectileID, ")"));
				basePlayer.stats.combat.Log(this, "duplicate_id");
			}
		}
		item.SetParent(null);
		Interface.CallHook("OnMeleeThrown", basePlayer, item);
		if (this.canAiHearIt)
		{
			float single = 0f;
			if (component.projectileObject != null)
			{
				GameObject gameObject = component.projectileObject.Get();
				if (gameObject != null)
				{
					Projectile component1 = gameObject.GetComponent<Projectile>();
					if (component1 != null)
					{
						foreach (DamageTypeEntry damageType in component1.damageTypes)
						{
							single += damageType.amount;
						}
					}
				}
			}
			if (basePlayer != null)
			{
				Sensation sensation = new Sensation()
				{
					Type = SensationType.ThrownWeapon,
					Position = basePlayer.transform.position,
					Radius = 50f,
					DamagePotential = single,
					InitiatorPlayer = basePlayer,
					Initiator = basePlayer
				};
				Sense.Stimulate(sensation);
			}
		}
	}

	public virtual void DoAttackShared(HitInfo info)
	{
		if (Interface.CallHook("OnPlayerAttack", this.GetOwnerPlayer(), info) != null)
		{
			return;
		}
		this.GetAttackStats(info);
		if (info.HitEntity != null)
		{
			using (TimeWarning timeWarning = TimeWarning.New("OnAttacked", (long)50))
			{
				info.HitEntity.OnAttacked(info);
			}
		}
		if (info.DoHitEffects)
		{
			if (!base.isServer)
			{
				using (timeWarning = TimeWarning.New("ImpactEffect", (long)20))
				{
					Effect.client.ImpactEffect(info);
				}
			}
			else
			{
				using (timeWarning = TimeWarning.New("ImpactEffect", (long)20))
				{
					Effect.server.ImpactEffect(info);
				}
			}
		}
		if (base.isServer && !base.IsDestroyed)
		{
			using (timeWarning = TimeWarning.New("UpdateItemCondition", (long)50))
			{
				this.UpdateItemCondition(info);
			}
			base.StartAttackCooldown(this.repeatDelay);
		}
	}

	public override void GetAttackStats(HitInfo info)
	{
		info.damageTypes.Add(this.damageTypes);
		info.CanGather = this.gathering.Any();
	}

	public virtual float GetConditionLoss()
	{
		return 1f;
	}

	public ResourceDispenser.GatherPropertyEntry GetGatherInfoFromIndex(ResourceDispenser.GatherType index)
	{
		return this.gathering.GetFromIndex(index);
	}

	public string GetStrikeEffectPath(string materialName)
	{
		for (int i = 0; i < this.materialStrikeFX.Count; i++)
		{
			if (this.materialStrikeFX[i].materialName == materialName && this.materialStrikeFX[i].fx.isValid)
			{
				return this.materialStrikeFX[i].fx.resourcePath;
			}
		}
		return this.strikeFX.resourcePath;
	}

	public bool IsItemBroken()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return true;
		}
		return ownerItem.isBroken;
	}

	public void LoseCondition(float amount)
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		ownerItem.LoseCondition(amount);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("BaseMelee.OnRpcMessage", 0.1f))
		{
			if (rpc == -1126684375 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - CLProject "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("CLProject", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("CLProject", this, player))
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
							this.CLProject(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in CLProject");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc != -206640447 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - PlayerAttack "));
				}
				using (timeWarning1 = TimeWarning.New("PlayerAttack", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("PlayerAttack", this, player))
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
							this.PlayerAttack(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in PlayerAttack");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[IsActiveItem]
	[RPC_Server]
	public void PlayerAttack(BaseEntity.RPCMessage msg)
	{
		Vector3 parentVelocity;
		BasePlayer basePlayer = msg.player;
		if (!this.VerifyClientAttack(basePlayer))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("PlayerAttack", (long)50))
		{
			using (PlayerAttack playerAttack = PlayerAttack.Deserialize(msg.read))
			{
				if (playerAttack != null)
				{
					HitInfo hitInfo = Facepunch.Pool.Get<HitInfo>();
					hitInfo.LoadFromAttack(playerAttack.attack, true);
					hitInfo.Initiator = basePlayer;
					hitInfo.Weapon = this;
					hitInfo.WeaponPrefab = this;
					hitInfo.Predicted = msg.connection;
					hitInfo.damageProperties = this.damageProperties;
					if (Interface.CallHook("OnMeleeAttack", basePlayer, hitInfo) != null)
					{
						return;
					}
					if (!hitInfo.IsNaNOrInfinity())
					{
						if (ConVar.AntiHack.melee_protection > 0 && hitInfo.HitEntity)
						{
							bool flag = true;
							float meleeForgiveness = 1f + ConVar.AntiHack.melee_forgiveness;
							float meleeClientframes = ConVar.AntiHack.melee_clientframes;
							float meleeServerframes = ConVar.AntiHack.melee_serverframes;
							float single = meleeClientframes / 60f;
							float single1 = meleeServerframes * Mathx.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime, UnityEngine.Time.fixedDeltaTime);
							float single2 = (basePlayer.desyncTime + single + single1) * meleeForgiveness;
							if (ConVar.AntiHack.projectile_protection >= 2)
							{
								float single3 = hitInfo.HitEntity.MaxVelocity();
								parentVelocity = hitInfo.HitEntity.GetParentVelocity();
								float single4 = single3 + parentVelocity.magnitude;
								float single5 = hitInfo.HitEntity.BoundsPadding() + single2 * single4;
								float single6 = hitInfo.HitEntity.Distance(hitInfo.HitPositionWorld);
								if (single6 > single5)
								{
									string shortPrefabName = base.ShortPrefabName;
									string str = hitInfo.HitEntity.ShortPrefabName;
									AntiHack.Log(basePlayer, AntiHackType.MeleeHack, string.Concat(new object[] { "Entity too far away (", shortPrefabName, " on ", str, " with ", single6, "m > ", single5, "m in ", single2, "s)" }));
									basePlayer.stats.combat.Log(hitInfo, "melee_distance");
									flag = false;
								}
							}
							if (ConVar.AntiHack.melee_protection >= 1)
							{
								float single7 = hitInfo.Initiator.MaxVelocity();
								parentVelocity = hitInfo.Initiator.GetParentVelocity();
								float single8 = single7 + parentVelocity.magnitude;
								float single9 = hitInfo.Initiator.BoundsPadding() + single2 * single8 + meleeForgiveness * this.maxDistance;
								float single10 = hitInfo.Initiator.Distance(hitInfo.HitPositionWorld);
								if (single10 > single9)
								{
									string shortPrefabName1 = base.ShortPrefabName;
									string str1 = hitInfo.HitEntity.ShortPrefabName;
									AntiHack.Log(basePlayer, AntiHackType.MeleeHack, string.Concat(new object[] { "Initiator too far away (", shortPrefabName1, " on ", str1, " with ", single10, "m > ", single9, "m in ", single2, "s)" }));
									basePlayer.stats.combat.Log(hitInfo, "melee_distance");
									flag = false;
								}
							}
							if (ConVar.AntiHack.melee_protection >= 3)
							{
								Vector3 pointStart = hitInfo.PointStart;
								Vector3 hitPositionWorld = hitInfo.HitPositionWorld + (hitInfo.HitNormalWorld.normalized * 0.001f);
								Vector3 vector3 = basePlayer.eyes.center;
								Vector3 vector31 = basePlayer.eyes.position;
								Vector3 vector32 = pointStart;
								Vector3 vector33 = hitInfo.PositionOnRay(hitPositionWorld);
								Vector3 vector34 = hitPositionWorld;
								bool flag1 = GamePhysics.LineOfSight(vector3, vector31, vector32, vector33, vector34, 2162688, 0f);
								if (flag1)
								{
									basePlayer.stats.Add(string.Concat("hit_", hitInfo.HitEntity.Categorize(), "_direct_los"), 1, Stats.Server);
								}
								else
								{
									basePlayer.stats.Add(string.Concat("hit_", hitInfo.HitEntity.Categorize(), "_indirect_los"), 1, Stats.Server);
								}
								if (!flag1)
								{
									string shortPrefabName2 = base.ShortPrefabName;
									string str2 = hitInfo.HitEntity.ShortPrefabName;
									AntiHack.Log(basePlayer, AntiHackType.MeleeHack, string.Concat(new object[] { "Line of sight (", shortPrefabName2, " on ", str2, ") ", vector3, " ", vector31, " ", vector32, " ", vector33, " ", vector34 }));
									basePlayer.stats.combat.Log(hitInfo, "melee_los");
									flag = false;
								}
							}
							if (!flag)
							{
								AntiHack.AddViolation(basePlayer, AntiHackType.MeleeHack, ConVar.AntiHack.melee_penalty);
								return;
							}
						}
						basePlayer.metabolism.UseHeart(this.heartStress * 0.2f);
						using (TimeWarning timeWarning1 = TimeWarning.New("DoAttackShared", (long)50))
						{
							this.DoAttackShared(hitInfo);
						}
					}
					else
					{
						string shortPrefabName3 = base.ShortPrefabName;
						AntiHack.Log(basePlayer, AntiHackType.MeleeHack, string.Concat("Contains NaN (", shortPrefabName3, ")"));
						basePlayer.stats.combat.Log(hitInfo, "melee_nan");
					}
				}
			}
		}
	}

	public override void ServerUse()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.HasAttackCooldown())
		{
			return;
		}
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		base.StartAttackCooldown(this.repeatDelay * 2f);
		ownerPlayer.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, null);
		if (this.swingEffect.isValid)
		{
			Effect.server.Run(this.swingEffect.resourcePath, base.transform.position, Vector3.forward, ownerPlayer.net.connection, false);
		}
		if (base.IsInvoking(new Action(this.ServerUse_Strike)))
		{
			base.CancelInvoke(new Action(this.ServerUse_Strike));
		}
		base.Invoke(new Action(this.ServerUse_Strike), this.aiStrikeDelay);
	}

	public void ServerUse_Strike()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return;
		}
		Vector3 vector3 = ownerPlayer.eyes.position;
		Vector3 vector31 = ownerPlayer.eyes.BodyForward();
		for (int i = 0; i < 2; i++)
		{
			List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
			GamePhysics.TraceAll(new Ray(vector3 - (vector31 * (i == 0 ? 0f : 0.2f)), vector31), (i == 0 ? 0f : this.attackRadius), list, this.effectiveRange + 0.2f, 1219701521, QueryTriggerInteraction.UseGlobal);
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				RaycastHit item = list[j];
				BaseEntity entity = item.GetEntity();
				if (!(entity == null) && (!(entity != null) || !(entity == ownerPlayer) && !entity.EqualNetID(ownerPlayer)) && (!(entity != null) || !entity.isClient) && !(entity.Categorize() == ownerPlayer.Categorize()))
				{
					float single = 0f;
					foreach (DamageTypeEntry damageType in this.damageTypes)
					{
						single += damageType.amount;
					}
					entity.OnAttacked(new HitInfo(ownerPlayer, entity, DamageType.Slash, single * this.npcDamageScale));
					HitInfo hitInfo = Facepunch.Pool.Get<HitInfo>();
					hitInfo.HitPositionWorld = item.point;
					hitInfo.HitNormalWorld = -vector31;
					if (entity is BaseNpc || entity is BasePlayer)
					{
						hitInfo.HitMaterial = StringPool.Get("Flesh");
					}
					else
					{
						hitInfo.HitMaterial = StringPool.Get((item.GetCollider().sharedMaterial != null ? item.GetCollider().sharedMaterial.GetName() : "generic"));
					}
					Effect.server.ImpactEffect(hitInfo);
					Facepunch.Pool.Free<HitInfo>(ref hitInfo);
					flag = true;
					if (!(entity != null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
				}
			}
			Facepunch.Pool.FreeList<RaycastHit>(ref list);
			if (flag)
			{
				break;
			}
		}
	}

	public float TotalDamage()
	{
		float single = 0f;
		foreach (DamageTypeEntry damageType in this.damageTypes)
		{
			if (damageType.amount <= 0f)
			{
				continue;
			}
			single += damageType.amount;
		}
		return single;
	}

	public void UpdateItemCondition(HitInfo info)
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null || !ownerItem.hasCondition)
		{
			return;
		}
		if (info != null && info.DidHit && !info.DidGather)
		{
			float conditionLoss = this.GetConditionLoss();
			float single = 0f;
			foreach (DamageTypeEntry damageType in this.damageTypes)
			{
				if (damageType.amount <= 0f)
				{
					continue;
				}
				single += Mathf.Clamp(damageType.amount - info.damageTypes.Get(damageType.type), 0f, damageType.amount);
			}
			conditionLoss = conditionLoss + single * 0.2f;
			ownerItem.LoseCondition(conditionLoss);
		}
	}

	[Serializable]
	public class MaterialFX
	{
		public string materialName;

		public GameObjectRef fx;

		public MaterialFX()
		{
		}
	}
}