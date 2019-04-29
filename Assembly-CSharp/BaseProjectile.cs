using ConVar;
using EasyAntiCheat.Server.Cerberus;
using EasyAntiCheat.Server.Hydra;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Rust.Ai;
using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseProjectile : AttackEntity
{
	[Header("NPC Info")]
	public float NoiseRadius = 100f;

	[Header("Projectile")]
	public float damageScale = 1f;

	public float distanceScale = 1f;

	public float projectileVelocityScale = 1f;

	public bool automatic;

	[Header("Effects")]
	public GameObjectRef attackFX;

	public GameObjectRef silencedAttack;

	public GameObjectRef muzzleBrakeAttack;

	public Transform MuzzlePoint;

	[Header("Reloading")]
	public float reloadTime = 1f;

	public bool canUnloadAmmo = true;

	public BaseProjectile.Magazine primaryMagazine;

	public bool fractionalReload;

	public float reloadStartDuration;

	public float reloadFractionDuration;

	public float reloadEndDuration;

	[Header("Recoil")]
	public float aimSway = 3f;

	public float aimSwaySpeed = 1f;

	public RecoilProperties recoil;

	[Header("Aim Cone")]
	public AnimationCurve aimconeCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 1f), new Keyframe(1f, 1f) });

	public float aimCone;

	public float hipAimCone = 1.8f;

	public float aimconePenaltyPerShot;

	public float aimConePenaltyMax;

	public float aimconePenaltyRecoverTime = 0.1f;

	public float aimconePenaltyRecoverDelay = 0.1f;

	public float stancePenaltyScale = 1f;

	[Header("Iconsights")]
	public bool hasADS = true;

	public bool noAimingWhileCycling;

	public bool manualCycle;

	[NonSerialized]
	protected bool needsCycle;

	[NonSerialized]
	protected bool isCycling;

	[NonSerialized]
	public bool aiming;

	[NonSerialized]
	private float nextReloadTime = Single.NegativeInfinity;

	[NonSerialized]
	private float startReloadTime = Single.NegativeInfinity;

	private float lastReloadTime = -10f;

	private float stancePenalty;

	private float aimconePenalty;

	protected bool reloadStarted;

	protected bool reloadFinished;

	private int fractionalInsertCounter;

	private readonly static Effect reusableInstance;

	private bool UsingInfiniteAmmoCheat
	{
		get
		{
			return false;
		}
	}

	static BaseProjectile()
	{
		BaseProjectile.reusableInstance = new Effect();
	}

	public BaseProjectile()
	{
	}

	public override float AmmoFraction()
	{
		return (float)this.primaryMagazine.contents / (float)this.primaryMagazine.capacity;
	}

	public bool CanAiAttack()
	{
		return true;
	}

	public override bool CanReload()
	{
		return this.primaryMagazine.contents < this.primaryMagazine.capacity;
	}

	public override bool CanUseNetworkCache(Connection sendingTo)
	{
		Connection ownerConnection = base.GetOwnerConnection();
		if (sendingTo == null || ownerConnection == null)
		{
			return true;
		}
		return sendingTo != ownerConnection;
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
		if (this.reloadFinished && this.HasReloadCooldown())
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Reloading (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "reload_cooldown");
			return;
		}
		this.reloadStarted = false;
		this.reloadFinished = false;
		if (this.primaryMagazine.contents <= 0 && !this.UsingInfiniteAmmoCheat)
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Magazine empty (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "ammo_missing");
			return;
		}
		ItemDefinition itemDefinition = this.primaryMagazine.ammoType;
		ProjectileShoot projectileShoot = ProjectileShoot.Deserialize(msg.read);
		if (itemDefinition.itemid != projectileShoot.ammoType)
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Ammo mismatch (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "ammo_mismatch");
			return;
		}
		if (!this.UsingInfiniteAmmoCheat)
		{
			this.primaryMagazine.contents--;
		}
		ItemModProjectile component = itemDefinition.GetComponent<ItemModProjectile>();
		if (component == null)
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Item mod not found (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "mod_missing");
			return;
		}
		if (projectileShoot.projectiles.Count > component.numProjectiles)
		{
			AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Count mismatch (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "count_mismatch");
			return;
		}
		Interface.CallHook("OnWeaponFired", this, msg.player, component, projectileShoot);
		base.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, msg.connection);
		basePlayer.CleanupExpiredProjectiles();
		foreach (ProjectileShoot.Projectile projectile in projectileShoot.projectiles)
		{
			if (!basePlayer.HasFiredProjectile(projectile.projectileID))
			{
				if (!base.ValidateEyePos(basePlayer, projectile.startPos))
				{
					continue;
				}
				basePlayer.NoteFiredProjectile(projectile.projectileID, projectile.startPos, projectile.startVel, this, itemDefinition, null);
				this.CreateProjectileEffectClientside(component.projectileObject.resourcePath, projectile.startPos, projectile.startVel, projectile.seed, msg.connection, this.IsSilenced(), false);
			}
			else
			{
				AntiHack.Log(basePlayer, AntiHackType.ProjectileHack, string.Concat("Duplicate ID (", projectile.projectileID, ")"));
				basePlayer.stats.combat.Log(this, "duplicate_id");
			}
		}
		basePlayer.stats.Add(string.Concat(component.category, "_fired"), projectileShoot.projectiles.Count<ProjectileShoot.Projectile>(), Stats.Steam);
		base.StartAttackCooldown(this.ScaleRepeatDelay(this.repeatDelay) + this.animationDelay);
		basePlayer.MarkHostileFor(60f);
		this.UpdateItemCondition();
		this.DidAttackServerside();
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
		float noiseRadius = this.NoiseRadius;
		if (this.IsSilenced())
		{
			noiseRadius *= AI.npc_gun_noise_silencer_modifier;
		}
		Sensation sensation = new Sensation()
		{
			Type = SensationType.Gunshot,
			Position = basePlayer.transform.position,
			Radius = noiseRadius,
			DamagePotential = single,
			InitiatorPlayer = basePlayer,
			Initiator = basePlayer
		};
		Sense.Stimulate(sensation);
		if (EACServer.playerTracker != null)
		{
			using (TimeWarning timeWarning = TimeWarning.New("LogPlayerShooting", 0.1f))
			{
				UnityEngine.Vector3 networkPosition = basePlayer.GetNetworkPosition();
				UnityEngine.Quaternion networkRotation = basePlayer.GetNetworkRotation();
				Item item = this.GetItem();
				int num = (item != null ? item.info.itemid : 0);
				EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(basePlayer.net.connection);
				PlayerUseWeapon playerUseWeapon = new PlayerUseWeapon()
				{
					Position = new EasyAntiCheat.Server.Cerberus.Vector3(networkPosition.x, networkPosition.y, networkPosition.z),
					ViewRotation = new EasyAntiCheat.Server.Cerberus.Quaternion(networkRotation.x, networkRotation.y, networkRotation.z, networkRotation.w),
					WeaponID = num
				};
				EACServer.playerTracker.LogPlayerUseWeapon(client, playerUseWeapon);
			}
		}
	}

	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		if (crafter == null || item == null)
		{
			return;
		}
		this.UnloadAmmo(item, crafter);
	}

	private void CreateProjectileEffectClientside(string prefabName, UnityEngine.Vector3 pos, UnityEngine.Vector3 velocity, int seed, Connection sourceConnection, bool silenced = false, bool forceClientsideEffects = false)
	{
		Effect effect = BaseProjectile.reusableInstance;
		effect.Clear();
		effect.Init(Effect.Type.Projectile, pos, velocity, sourceConnection);
		effect.scale = (silenced ? 0f : 1f);
		if (forceClientsideEffects)
		{
			effect.scale = 2f;
		}
		effect.pooledString = prefabName;
		effect.number = seed;
		EffectNetwork.Send(effect);
	}

	public virtual void DidAttackServerside()
	{
	}

	public virtual bool ForceSendMagazine()
	{
		return false;
	}

	public float GetAIAimcone()
	{
		NPCPlayer ownerPlayer = base.GetOwnerPlayer() as NPCPlayer;
		if (!ownerPlayer)
		{
			return this.aiAimCone;
		}
		return ownerPlayer.GetAimConeScale() * this.aiAimCone;
	}

	public virtual float GetAimCone()
	{
		float single = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.sightAimCone, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
		float single1 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.sightAimCone, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		float single2 = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.hipAimCone, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
		float single3 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.hipAimCone, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		if (this.aiming || base.isServer)
		{
			return (this.aimCone + this.aimconePenalty + this.stancePenalty * this.stancePenaltyScale) * single + single1;
		}
		return (this.aimCone + this.aimconePenalty + this.stancePenalty * this.stancePenaltyScale) * single + single1 + this.hipAimCone * single2 + single3;
	}

	public int GetAvailableAmmo()
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (ownerPlayer == null)
		{
			return 0;
		}
		List<Item> list = Facepunch.Pool.GetList<Item>();
		ownerPlayer.inventory.FindAmmo(list, this.primaryMagazine.definition.ammoTypes);
		int num = 0;
		if (list.Count != 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				Item item = list[i];
				if (item.info == this.primaryMagazine.ammoType)
				{
					num += item.amount;
				}
			}
		}
		Facepunch.Pool.FreeList<Item>(ref list);
		return num;
	}

	public virtual float GetDamageScale(bool getMax = false)
	{
		return this.damageScale;
	}

	public virtual float GetDistanceScale(bool getMax = false)
	{
		return this.distanceScale;
	}

	public Projectile.Modifier GetProjectileModifier()
	{
		Projectile.Modifier modifier = new Projectile.Modifier()
		{
			damageOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.projectileDamage, (ProjectileWeaponMod.Modifier y) => y.offset, 0f),
			damageScale = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.projectileDamage, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * this.GetDamageScale(false),
			distanceOffset = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.projectileDistance, (ProjectileWeaponMod.Modifier y) => y.offset, 0f),
			distanceScale = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.projectileDistance, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f) * this.GetDistanceScale(false)
		};
		return modifier;
	}

	public virtual float GetProjectileVelocityScale(bool getMax = false)
	{
		return this.projectileVelocityScale;
	}

	public virtual RecoilProperties GetRecoil()
	{
		return this.recoil;
	}

	protected float GetReloadCooldown()
	{
		return Mathf.Max(this.nextReloadTime - UnityEngine.Time.time, 0f);
	}

	public float GetReloadDuration()
	{
		if (!this.fractionalReload)
		{
			return this.reloadTime;
		}
		int num = Mathf.Min(this.primaryMagazine.capacity - this.primaryMagazine.contents, this.GetAvailableAmmo());
		return this.reloadStartDuration + this.reloadEndDuration + this.reloadFractionDuration * (float)num;
	}

	protected float GetReloadIdle()
	{
		return Mathf.Max(UnityEngine.Time.time - this.nextReloadTime, 0f);
	}

	protected bool HasReloadCooldown()
	{
		return UnityEngine.Time.time < this.nextReloadTime;
	}

	public bool IsSilenced()
	{
		bool flag;
		if (this.children != null)
		{
			List<BaseEntity>.Enumerator enumerator = this.children.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					ProjectileWeaponMod current = enumerator.Current as ProjectileWeaponMod;
					if (!(current != null) || !current.isSilencer || current.IsBroken())
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}
		return false;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			this.primaryMagazine.Load(info.msg.baseProjectile.primaryMagazine);
		}
	}

	public override UnityEngine.Vector3 ModifyAIAim(UnityEngine.Vector3 eulerInput, float swayModifier = 1f)
	{
		float single = UnityEngine.Time.time * (this.aimSwaySpeed * 1f + this.aiAimSwayOffset);
		float single1 = Mathf.Sin(UnityEngine.Time.time * 2f);
		float single2 = (single1 < 0f ? 1f - Mathf.Clamp(Mathf.Abs(single1) / 1f, 0f, 1f) : 1f);
		float single3 = (0 != 0 ? 0.6f : 1f);
		float single4 = (this.aimSway * 1f + this.aiAimSwayOffset) * single3 * single2 * swayModifier;
		ref float singlePointer = ref eulerInput.y;
		singlePointer = singlePointer + (Mathf.PerlinNoise(single, single) - 0.5f) * single4 * UnityEngine.Time.deltaTime;
		ref float singlePointer1 = ref eulerInput.x;
		singlePointer1 = singlePointer1 + (Mathf.PerlinNoise(single + 0.1f, single + 0.2f) - 0.5f) * single4 * UnityEngine.Time.deltaTime;
		return eulerInput;
	}

	private void OnDrawGizmos()
	{
		if (!base.isClient)
		{
			return;
		}
		if (this.MuzzlePoint != null)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(this.MuzzlePoint.position, this.MuzzlePoint.position + (this.MuzzlePoint.forward * 10f));
			BasePlayer ownerPlayer = base.GetOwnerPlayer();
			if (ownerPlayer)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(this.MuzzlePoint.position, this.MuzzlePoint.position + ((ownerPlayer.eyes.rotation * UnityEngine.Vector3.forward) * 10f));
			}
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("BaseProjectile.OnRpcMessage", 0.1f))
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
			else if (rpc == 1720368164 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Reload "));
				}
				using (timeWarning1 = TimeWarning.New("Reload", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("Reload", this, player))
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
							this.Reload(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in Reload");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc == 240404208 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ServerFractionalReloadInsert "));
				}
				using (timeWarning1 = TimeWarning.New("ServerFractionalReloadInsert", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("ServerFractionalReloadInsert", this, player))
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
							this.ServerFractionalReloadInsert(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in ServerFractionalReloadInsert");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
			else if (rpc == 555589155 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - StartReload "));
				}
				using (timeWarning1 = TimeWarning.New("StartReload", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("StartReload", this, player))
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
							this.StartReload(rPCMessage);
						}
					}
					catch (Exception exception3)
					{
						player.Kick("RPC Error in StartReload");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
			else if (rpc != 1918419884 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SwitchAmmoTo "));
				}
				using (timeWarning1 = TimeWarning.New("SwitchAmmoTo", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsActiveItem.Test("SwitchAmmoTo", this, player))
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
							this.SwitchAmmoTo(rPCMessage);
						}
					}
					catch (Exception exception4)
					{
						player.Kick("RPC Error in SwitchAmmoTo");
						Debug.LogException(exception4);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[IsActiveItem]
	[RPC_Server]
	private void Reload(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!base.VerifyClientRPC(basePlayer))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (!this.reloadStarted)
		{
			AntiHack.Log(basePlayer, AntiHackType.ReloadHack, string.Concat("Request skipped (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "reload_skip");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (!this.fractionalReload)
		{
			if (this.GetReloadCooldown() > 1f)
			{
				AntiHack.Log(basePlayer, AntiHackType.ReloadHack, string.Concat(new object[] { "T-", this.GetReloadCooldown(), "s (", base.ShortPrefabName, ")" }));
				basePlayer.stats.combat.Log(this, "reload_time");
				this.reloadStarted = false;
				this.reloadFinished = false;
				return;
			}
			if (this.GetReloadIdle() > 1f)
			{
				AntiHack.Log(basePlayer, AntiHackType.ReloadHack, string.Concat(new object[] { "T+", this.GetReloadIdle(), "s (", base.ShortPrefabName, ")" }));
				basePlayer.stats.combat.Log(this, "reload_time");
				this.reloadStarted = false;
				this.reloadFinished = false;
				return;
			}
		}
		if (this.fractionalReload)
		{
			this.ResetReloadCooldown();
		}
		this.reloadStarted = false;
		this.reloadFinished = true;
		if (!this.fractionalReload)
		{
			this.ReloadMagazine(-1);
		}
	}

	protected void ReloadMagazine(int desiredAmount = -1)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		if (Interface.CallHook("OnReloadMagazine", ownerPlayer, this) != null)
		{
			return;
		}
		this.primaryMagazine.Reload(ownerPlayer, desiredAmount);
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	protected void ResetReloadCooldown()
	{
		this.nextReloadTime = Single.NegativeInfinity;
	}

	public override void ReturnedFromCancelledCraft(Item item, BasePlayer crafter)
	{
		if (crafter == null || item == null)
		{
			return;
		}
		BaseProjectile component = item.GetHeldEntity().GetComponent<BaseProjectile>();
		if (component)
		{
			component.primaryMagazine.contents = 0;
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Facepunch.Pool.Get<ProtoBuf.BaseProjectile>();
		if (info.forDisk || info.SendingTo(base.GetOwnerConnection()) || this.ForceSendMagazine())
		{
			info.msg.baseProjectile.primaryMagazine = this.primaryMagazine.Save();
		}
	}

	public float ScaleRepeatDelay(float delay)
	{
		float single = ProjectileWeaponMod.Average(this, (ProjectileWeaponMod x) => x.repeatDelay, (ProjectileWeaponMod.Modifier y) => y.scalar, 1f);
		float single1 = ProjectileWeaponMod.Sum(this, (ProjectileWeaponMod x) => x.repeatDelay, (ProjectileWeaponMod.Modifier y) => y.offset, 0f);
		return delay * single + single1;
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (item == null)
		{
			return;
		}
		if (command == "unload_ammo" && !this.HasReloadCooldown())
		{
			this.UnloadAmmo(item, player);
		}
	}

	[IsActiveItem]
	[RPC_Server]
	private void ServerFractionalReloadInsert(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!base.VerifyClientRPC(basePlayer))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (!this.reloadStarted)
		{
			AntiHack.Log(basePlayer, AntiHackType.ReloadHack, string.Concat("Fractional reload request skipped (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "reload_skip");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (this.GetReloadIdle() > 3f)
		{
			AntiHack.Log(basePlayer, AntiHackType.ReloadHack, string.Concat(new object[] { "T+", this.GetReloadIdle(), "s (", base.ShortPrefabName, ")" }));
			basePlayer.stats.combat.Log(this, "reload_time");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (UnityEngine.Time.time < this.startReloadTime + this.reloadStartDuration)
		{
			AntiHack.Log(basePlayer, AntiHackType.ReloadHack, string.Concat("Fractional reload too early (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "reload_fraction_too_early");
			this.reloadStarted = false;
			this.reloadFinished = false;
		}
		if (UnityEngine.Time.time < this.startReloadTime + this.reloadStartDuration + (float)this.fractionalInsertCounter * this.reloadFractionDuration)
		{
			AntiHack.Log(basePlayer, AntiHackType.ReloadHack, string.Concat("Fractional reload rate too high (", base.ShortPrefabName, ")"));
			basePlayer.stats.combat.Log(this, "reload_fraction_rate");
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		this.fractionalInsertCounter++;
		if (this.primaryMagazine.contents < this.primaryMagazine.capacity)
		{
			this.ReloadMagazine(1);
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.primaryMagazine.ServerInit();
	}

	public override bool ServerIsReloading()
	{
		return UnityEngine.Time.time < this.lastReloadTime + this.reloadTime;
	}

	public override void ServerReload()
	{
		if (this.ServerIsReloading())
		{
			return;
		}
		this.lastReloadTime = UnityEngine.Time.time;
		base.StartAttackCooldown(this.reloadTime);
		base.GetOwnerPlayer().SignalBroadcast(BaseEntity.Signal.Reload, null);
		this.primaryMagazine.contents = this.primaryMagazine.capacity;
	}

	public override void ServerUse()
	{
		this.ServerUse(1f);
	}

	public override void ServerUse(float damageModifier)
	{
		UnityEngine.Vector3 vector3;
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
		if (this.primaryMagazine.contents <= 0)
		{
			base.SignalBroadcast(BaseEntity.Signal.DryFire, null);
			base.StartAttackCooldown(1f);
		}
		this.primaryMagazine.contents--;
		if (this.primaryMagazine.contents < 0)
		{
			this.primaryMagazine.contents = 0;
		}
		if (ownerPlayer.IsNpc && (ownerPlayer.isMounted || ownerPlayer.GetParentEntity() != null))
		{
			NPCPlayer nPCPlayer = ownerPlayer as NPCPlayer;
			if (nPCPlayer == null)
			{
				HTNPlayer hTNPlayer = ownerPlayer as HTNPlayer;
				if (hTNPlayer != null)
				{
					hTNPlayer.AiDomain.ForceProjectileOrientation();
					hTNPlayer.ForceOrientationTick();
				}
			}
			else
			{
				nPCPlayer.SetAimDirection(nPCPlayer.GetAimDirection());
			}
		}
		base.StartAttackCooldown(this.repeatDelay);
		UnityEngine.Vector3 vector31 = ownerPlayer.eyes.position;
		ItemModProjectile component = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>();
		base.SignalBroadcast(BaseEntity.Signal.Attack, string.Empty, null);
		Projectile projectile = component.projectileObject.Get().GetComponent<Projectile>();
		BaseEntity mainTarget = null;
		if (ownerPlayer.IsNpc && AI.npc_only_hurt_active_target_in_safezone && ownerPlayer.InSafeZone())
		{
			IAIAgent aIAgent = ownerPlayer as IAIAgent;
			if (aIAgent == null)
			{
				IHTNAgent hTNAgent = ownerPlayer as IHTNAgent;
				if (hTNAgent != null)
				{
					mainTarget = hTNAgent.MainTarget;
				}
			}
			else
			{
				mainTarget = aIAgent.AttackTarget;
			}
		}
		bool flag = ownerPlayer is IHTNAgent;
		for (int i = 0; i < component.numProjectiles; i++)
		{
			vector3 = (!flag ? AimConeUtil.GetModifiedAimConeDirection(component.projectileSpread + this.aimCone + this.GetAIAimcone() * 1f, ownerPlayer.eyes.BodyForward(), true) : AimConeUtil.GetModifiedAimConeDirection(component.projectileSpread + this.aimCone, ownerPlayer.eyes.rotation * UnityEngine.Vector3.forward, true));
			List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
			GamePhysics.TraceAll(new Ray(vector31, vector3), 0f, list, 300f, 1219701521, QueryTriggerInteraction.UseGlobal);
			for (int j = 0; j < list.Count; j++)
			{
				BaseEntity entity = list[j].GetEntity();
				if ((!(entity != null) || !(entity == this) && !entity.EqualNetID(this)) && (!(entity != null) || !entity.isClient))
				{
					BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
					if (baseCombatEntity != null && (mainTarget == null || entity == mainTarget || entity.EqualNetID(mainTarget)))
					{
						float single = 0f;
						foreach (DamageTypeEntry damageType in projectile.damageTypes)
						{
							single += damageType.amount;
						}
						single *= damageModifier;
						baseCombatEntity.Hurt(single * this.npcDamageScale, DamageType.Bullet, ownerPlayer, true);
					}
					if (!(entity != null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
				}
			}
			UnityEngine.Vector3 vector32 = (ownerPlayer.isMounted ? vector3 * 6f : UnityEngine.Vector3.zero);
			this.CreateProjectileEffectClientside(component.projectileObject.resourcePath, ownerPlayer.eyes.position + vector32, vector3 * component.projectileVelocity, UnityEngine.Random.Range(1, 100), null, this.IsSilenced(), true);
		}
	}

	public override void SetLightsOn(bool isOn)
	{
		base.SetLightsOn(isOn);
		if (this.children != null)
		{
			foreach (ProjectileWeaponMod projectileWeaponMod in this.children.Cast<ProjectileWeaponMod>().Where<ProjectileWeaponMod>((ProjectileWeaponMod x) => {
				if (x == null)
				{
					return false;
				}
				return x.isLight;
			}))
			{
				projectileWeaponMod.SetFlag(BaseEntity.Flags.On, isOn, false, true);
			}
		}
	}

	[IsActiveItem]
	[RPC_Server]
	private void StartReload(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!base.VerifyClientRPC(basePlayer))
		{
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			this.reloadStarted = false;
			this.reloadFinished = false;
			return;
		}
		if (Interface.CallHook("OnReloadWeapon", basePlayer, this) != null)
		{
			return;
		}
		this.reloadFinished = false;
		this.reloadStarted = true;
		this.fractionalInsertCounter = 0;
		this.primaryMagazine.SwitchAmmoTypesIfNeeded(basePlayer);
		this.StartReloadCooldown(this.GetReloadDuration());
	}

	protected void StartReloadCooldown(float cooldown)
	{
		this.nextReloadTime = base.CalculateCooldownTime(this.nextReloadTime, cooldown, false);
		this.startReloadTime = this.nextReloadTime - cooldown;
	}

	[IsActiveItem]
	[RPC_Server]
	private void SwitchAmmoTo(BaseEntity.RPCMessage msg)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		if (!ownerPlayer)
		{
			return;
		}
		int num = msg.read.Int32();
		if (num == this.primaryMagazine.ammoType.itemid)
		{
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(num);
		if (itemDefinition == null)
		{
			return;
		}
		ItemModProjectile component = itemDefinition.GetComponent<ItemModProjectile>();
		if (!component || !component.IsAmmo(this.primaryMagazine.definition.ammoTypes))
		{
			return;
		}
		if (Interface.CallHook("OnSwitchAmmo", ownerPlayer, this) != null)
		{
			return;
		}
		if (this.primaryMagazine.contents > 0)
		{
			ownerPlayer.GiveItem(ItemManager.CreateByItemID(this.primaryMagazine.ammoType.itemid, this.primaryMagazine.contents, (ulong)0), BaseEntity.GiveItemReason.Generic);
			this.primaryMagazine.contents = 0;
		}
		this.primaryMagazine.ammoType = itemDefinition;
		base.SendNetworkUpdateImmediate(false);
		ItemManager.DoRemoves();
		ownerPlayer.inventory.ServerUpdate(0f);
	}

	public override void TopUpAmmo()
	{
		this.primaryMagazine.contents = this.primaryMagazine.capacity;
	}

	public void UnloadAmmo(Item item, BasePlayer player)
	{
		BaseProjectile component = item.GetHeldEntity().GetComponent<BaseProjectile>();
		if (!component.canUnloadAmmo)
		{
			return;
		}
		if (component)
		{
			int num = component.primaryMagazine.contents;
			if (num > 0)
			{
				component.primaryMagazine.contents = 0;
				base.SendNetworkUpdateImmediate(false);
				Item item1 = ItemManager.Create(component.primaryMagazine.ammoType, num, (ulong)0);
				if (!item1.MoveToContainer(player.inventory.containerMain, -1, true))
				{
					item1.Drop(player.GetDropPosition(), player.GetDropVelocity(), new UnityEngine.Quaternion());
				}
			}
		}
	}

	public void UpdateItemCondition()
	{
		Item ownerItem = base.GetOwnerItem();
		if (ownerItem == null)
		{
			return;
		}
		float component = this.primaryMagazine.ammoType.GetComponent<ItemModProjectile>().barrelConditionLoss;
		float single = 0.25f;
		ownerItem.LoseCondition(single + component);
		if (ownerItem.contents != null && ownerItem.contents.itemList != null)
		{
			for (int i = ownerItem.contents.itemList.Count - 1; i >= 0; i--)
			{
				Item item = ownerItem.contents.itemList[i];
				if (item != null)
				{
					item.LoseCondition(single + component);
				}
			}
		}
	}

	[Serializable]
	public class Magazine
	{
		public BaseProjectile.Magazine.Definition definition;

		public int capacity;

		public int contents;

		[ItemSelector(ItemCategory.All)]
		public ItemDefinition ammoType;

		public Magazine()
		{
		}

		public bool CanAiReload(BasePlayer owner)
		{
			if (this.contents >= this.capacity)
			{
				return false;
			}
			return true;
		}

		public bool CanReload(BasePlayer owner)
		{
			if (this.contents >= this.capacity)
			{
				return false;
			}
			return owner.inventory.HasAmmo(this.definition.ammoTypes);
		}

		public void Load(ProtoBuf.Magazine mag)
		{
			this.contents = mag.contents;
			this.capacity = mag.capacity;
			this.ammoType = ItemManager.FindItemDefinition(mag.ammoType);
		}

		public bool Reload(BasePlayer owner, int desiredAmount = -1)
		{
			List<Item> list = owner.inventory.FindItemIDs(this.ammoType.itemid).ToList<Item>();
			if (list.Count == 0)
			{
				List<Item> items = new List<Item>();
				owner.inventory.FindAmmo(items, this.definition.ammoTypes);
				if (items.Count == 0)
				{
					return false;
				}
				list = owner.inventory.FindItemIDs(items[0].info.itemid).ToList<Item>();
				if (list == null || list.Count == 0)
				{
					return false;
				}
				if (this.contents > 0)
				{
					owner.GiveItem(ItemManager.CreateByItemID(this.ammoType.itemid, this.contents, (ulong)0), BaseEntity.GiveItemReason.Generic);
					this.contents = 0;
				}
				this.ammoType = list[0].info;
			}
			int num = desiredAmount;
			if (num == -1)
			{
				num = this.capacity - this.contents;
			}
			foreach (Item item in list)
			{
				int num1 = item.amount;
				int num2 = Mathf.Min(num, item.amount);
				item.UseItem(num2);
				this.contents += num2;
				num -= num2;
				if (num > 0)
				{
					continue;
				}
				return false;
			}
			return false;
		}

		public ProtoBuf.Magazine Save()
		{
			ProtoBuf.Magazine magazine = Facepunch.Pool.Get<ProtoBuf.Magazine>();
			if (this.ammoType != null)
			{
				magazine.capacity = this.capacity;
				magazine.contents = this.contents;
				magazine.ammoType = this.ammoType.itemid;
			}
			else
			{
				magazine.capacity = this.capacity;
				magazine.contents = 0;
				magazine.ammoType = 0;
			}
			return magazine;
		}

		public void ServerInit()
		{
			if (this.definition.builtInSize > 0)
			{
				this.capacity = this.definition.builtInSize;
			}
		}

		public void SwitchAmmoTypesIfNeeded(BasePlayer owner)
		{
			List<Item> list = owner.inventory.FindItemIDs(this.ammoType.itemid).ToList<Item>();
			if (list.Count == 0)
			{
				List<Item> items = new List<Item>();
				owner.inventory.FindAmmo(items, this.definition.ammoTypes);
				if (items.Count == 0)
				{
					return;
				}
				list = owner.inventory.FindItemIDs(items[0].info.itemid).ToList<Item>();
				if (list == null || list.Count == 0)
				{
					return;
				}
				if (this.contents > 0)
				{
					owner.GiveItem(ItemManager.CreateByItemID(this.ammoType.itemid, this.contents, (ulong)0), BaseEntity.GiveItemReason.Generic);
					this.contents = 0;
				}
				this.ammoType = list[0].info;
			}
		}

		[Serializable]
		public struct Definition
		{
			[Tooltip("Set to 0 to not use inbuilt mag")]
			public int builtInSize;

			[InspectorFlags]
			[Tooltip("If using inbuilt mag, will accept these types of ammo")]
			public AmmoTypes ammoTypes;
		}
	}
}