using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class AutoTurret : StorageContainer
{
	public GameObjectRef gun_fire_effect;

	public GameObjectRef bulletEffect;

	public float bulletSpeed = 200f;

	public AmbienceEmitter ambienceEmitter;

	public BaseCombatEntity target;

	public Transform eyePos;

	public Transform muzzlePos;

	public Vector3 aimDir;

	public Transform gun_yaw;

	public Transform gun_pitch;

	public float sightRange = 30f;

	public SoundDefinition turnLoopDef;

	public SoundDefinition movementChangeDef;

	public SoundDefinition ambientLoopDef;

	public SoundDefinition focusCameraDef;

	public float focusSoundFreqMin = 2.5f;

	public float focusSoundFreqMax = 7f;

	public GameObjectRef peacekeeperToggleSound;

	public GameObjectRef onlineSound;

	public GameObjectRef offlineSound;

	public GameObjectRef targetAcquiredEffect;

	public GameObjectRef targetLostEffect;

	public float aimCone;

	public List<PlayerNameID> authorizedPlayers = new List<PlayerNameID>();

	public ItemDefinition ammoType;

	public TargetTrigger targetTrigger;

	private float nextShotTime;

	private float nextVisCheck;

	public float lastTargetSeenTime;

	private bool targetVisible = true;

	private bool booting;

	private float nextIdleAimTime;

	private Vector3 targetAimDir = Vector3.forward;

	private Item ammoItem;

	public const float bulletDamage = 15f;

	private float nextForcedAimTime;

	private Vector3 lastSentAimDir = Vector3.zero;

	private static float[] visibilityOffsets;

	static AutoTurret()
	{
		AutoTurret.visibilityOffsets = new float[] { default(float), 0.15f, -0.15f };
	}

	public AutoTurret()
	{
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void AddSelfAuthorize(BaseEntity.RPCMessage rpc)
	{
		if (this.IsOnline())
		{
			return;
		}
		if (Interface.CallHook("OnTurretAuthorize", this, rpc.player) != null)
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

	public Vector3 AimOffset(BaseCombatEntity aimat)
	{
		BasePlayer basePlayer = aimat as BasePlayer;
		if (basePlayer == null)
		{
			return aimat.transform.position + new Vector3(0f, 0.3f, 0f);
		}
		if (!basePlayer.IsSleeping())
		{
			return basePlayer.eyes.position;
		}
		return basePlayer.transform.position + (Vector3.up * 0.1f);
	}

	public float AngleToTarget(BaseCombatEntity potentialtarget)
	{
		Transform centerMuzzle = this.GetCenterMuzzle();
		Vector3 vector3 = this.AimOffset(potentialtarget);
		Vector3 vector31 = (vector3 - centerMuzzle.position).normalized;
		return Vector3.Angle(centerMuzzle.forward, vector31);
	}

	public bool AnyAuthed()
	{
		return this.authorizedPlayers.Count > 0;
	}

	private void ApplyDamage(BaseCombatEntity entity, Vector3 point, Vector3 normal)
	{
		float single = 15f * UnityEngine.Random.Range(0.9f, 1.1f);
		if (entity is BasePlayer && entity != this.target)
		{
			single *= 0.5f;
		}
		if (this.PeacekeeperMode() && entity == this.target)
		{
			this.target.MarkHostileFor(1800f);
		}
		entity.OnAttacked(new HitInfo(this, entity, DamageType.Bullet, single, point));
		if (entity is BasePlayer || entity is BaseNpc)
		{
			Effect.server.ImpactEffect(new HitInfo()
			{
				HitPositionWorld = point,
				HitNormalWorld = -normal,
				HitMaterial = StringPool.Get("Flesh")
			});
		}
	}

	public virtual bool CanChangeSettings(BasePlayer player)
	{
		if (!this.IsAuthed(player))
		{
			return false;
		}
		return this.IsOffline();
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!base.CanPickup(player) || !this.IsOffline())
		{
			return false;
		}
		return this.IsAuthed(player);
	}

	public virtual bool CheckPeekers()
	{
		return true;
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void ClearList(BaseEntity.RPCMessage rpc)
	{
		if (this.booting || this.IsOnline() || !this.IsAuthed(rpc.player))
		{
			return;
		}
		if (Interface.CallHook("OnTurretClearList", this, rpc.player) != null)
		{
			return;
		}
		this.authorizedPlayers.Clear();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void EnsureReloaded()
	{
		if (!this.HasAmmo())
		{
			this.Reload();
		}
	}

	public virtual void FireGun(Vector3 targetPos, float aimCone, Transform muzzleToUse = null, BaseCombatEntity target = null)
	{
		if (this.IsOffline())
		{
			return;
		}
		if (muzzleToUse == null)
		{
			muzzleToUse = this.muzzlePos;
		}
		Vector3 centerMuzzle = this.GetCenterMuzzle().transform.position - (this.GetCenterMuzzle().forward * 0.25f);
		Vector3 vector3 = this.GetCenterMuzzle().transform.forward;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, vector3, true);
		targetPos = centerMuzzle + (modifiedAimConeDirection * 300f);
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(centerMuzzle, modifiedAimConeDirection), 0f, list, 300f, 1219701521, QueryTriggerInteraction.UseGlobal);
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit item = list[i];
			BaseEntity entity = item.GetEntity();
			if ((!(entity != null) || !(entity == this) && !entity.EqualNetID(this)) && (!(target != null) || !(entity != null) || !(entity.GetComponent<BasePlayer>() != null) || entity.EqualNetID(target)))
			{
				BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
				if (baseCombatEntity != null)
				{
					this.ApplyDamage(baseCombatEntity, item.point, modifiedAimConeDirection);
				}
				if (!(entity != null) || entity.ShouldBlockProjectiles())
				{
					targetPos = item.point;
					vector3 = (targetPos - centerMuzzle).normalized;
					break;
				}
			}
		}
		base.ClientRPC<uint, Vector3>(null, "CLIENT_FireGun", StringPool.Get(muzzleToUse.gameObject.name), targetPos);
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void FlipAim(BaseEntity.RPCMessage rpc)
	{
		if (this.IsOnline() || !this.IsAuthed(rpc.player) || this.booting)
		{
			return;
		}
		base.transform.rotation = Quaternion.LookRotation(-base.transform.forward, base.transform.up);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public float GetAimSpeed()
	{
		if (this.HasTarget())
		{
			return 5f;
		}
		return 1f;
	}

	public virtual Transform GetCenterMuzzle()
	{
		return this.muzzlePos;
	}

	public virtual bool HasAmmo()
	{
		if (this.ammoItem == null || this.ammoItem.amount <= 0)
		{
			return false;
		}
		return this.ammoItem.parent == this.inventory;
	}

	public bool HasTarget()
	{
		if (this.target == null)
		{
			return false;
		}
		return this.target.IsAlive();
	}

	public void IdleTick()
	{
		if (UnityEngine.Time.realtimeSinceStartup > this.nextIdleAimTime)
		{
			this.nextIdleAimTime = UnityEngine.Time.realtimeSinceStartup + UnityEngine.Random.Range(4f, 5f);
			Quaternion quaternion = Quaternion.LookRotation(this.eyePos.forward, Vector3.up);
			quaternion *= Quaternion.AngleAxis(UnityEngine.Random.Range(-45f, 45f), Vector3.up);
			this.targetAimDir = quaternion * Vector3.forward;
		}
		if (!this.HasTarget())
		{
			this.aimDir = Vector3.Lerp(this.aimDir, this.targetAimDir, UnityEngine.Time.deltaTime * 2f);
		}
	}

	protected virtual bool Ignore(BasePlayer player)
	{
		return false;
	}

	public virtual bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return this.AngleToTarget(potentialtarget) <= 90f;
	}

	public void InitiateShutdown()
	{
		if (this.IsOffline())
		{
			return;
		}
		if (Interface.CallHook("OnTurretShutdown", this) != null)
		{
			return;
		}
		Effect.server.Run(this.offlineSound.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
		this.SetIsOnline(false);
	}

	public void InitiateStartup()
	{
		if (this.IsOnline() || this.booting)
		{
			return;
		}
		if (Interface.CallHook("OnTurretStartup", this) != null)
		{
			return;
		}
		Effect.server.Run(this.onlineSound.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
		base.Invoke(new Action(this.SetOnline), 2f);
		this.booting = true;
	}

	public bool IsAuthed(BasePlayer player)
	{
		return this.authorizedPlayers.Any<PlayerNameID>((PlayerNameID x) => x.userid == player.userID);
	}

	public virtual bool IsEntityHostile(BaseCombatEntity ent)
	{
		return ent.IsHostile();
	}

	public bool IsOffline()
	{
		return !this.IsOnline();
	}

	public bool IsOnline()
	{
		return base.IsOn();
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.autoturret != null)
		{
			this.authorizedPlayers = info.msg.autoturret.users;
			info.msg.autoturret.users = null;
		}
	}

	public bool ObjectVisible(BaseCombatEntity obj)
	{
		object obj1 = Interface.CallHook("CanBeTargeted", obj, this);
		if (obj1 as bool)
		{
			return (bool)obj1;
		}
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		Vector3 vector3 = this.eyePos.transform.position;
		Vector3 vector31 = this.AimOffset(obj);
		float single = Vector3.Distance(vector31, vector3);
		Vector3 vector32 = vector31 - vector3;
		Vector3 vector33 = Vector3.Cross(vector32.normalized, Vector3.up);
		int num = 0;
		while (true)
		{
			if ((float)num >= (this.CheckPeekers() ? 3f : 1f))
			{
				break;
			}
			vector32 = (vector31 + (vector33 * AutoTurret.visibilityOffsets[num])) - vector3;
			Vector3 vector34 = vector32.normalized;
			list.Clear();
			GamePhysics.TraceAll(new Ray(vector3, vector34), 0f, list, single * 1.1f, 1218652417, QueryTriggerInteraction.UseGlobal);
			for (int i = 0; i < list.Count; i++)
			{
				BaseEntity entity = list[i].GetEntity();
				if ((!(entity != null) || !entity.isClient) && (!(entity != null) || !(entity.ToPlayer() != null) || entity.EqualNetID(obj)) && (!(entity != null) || !entity.EqualNetID(this)))
				{
					if (entity != null && (entity == obj || entity.EqualNetID(obj)))
					{
						Facepunch.Pool.FreeList<RaycastHit>(ref list);
						return true;
					}
					if (!(entity != null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
				}
			}
			num++;
		}
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		return false;
	}

	public void OfflineTick()
	{
		this.aimDir = Vector3.up;
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (this.IsOnline() && !this.HasTarget() || !this.targetVisible)
		{
			if (info.Initiator is AutoTurret)
			{
				return;
			}
			BasePlayer initiator = info.Initiator as BasePlayer;
			if (!initiator || !this.IsAuthed(initiator))
			{
				this.SetTarget(info.Initiator as BaseCombatEntity);
			}
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("AutoTurret.OnRpcMessage", 0.1f))
		{
			if (rpc == 1092560690 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
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
				if (ConVar.Global.developer > 2)
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
			else if (rpc == 1500257773 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - FlipAim "));
				}
				using (timeWarning1 = TimeWarning.New("FlipAim", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("FlipAim", this, player, 3f))
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
							this.FlipAim(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in FlipAim");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
			else if (rpc == -676981327 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
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
					catch (Exception exception3)
					{
						player.Kick("RPC Error in RemoveSelfAuthorize");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
			else if (rpc == 1770263114 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SERVER_AttackAll "));
				}
				using (timeWarning1 = TimeWarning.New("SERVER_AttackAll", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_AttackAll", this, player, 3f))
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
							this.SERVER_AttackAll(rPCMessage);
						}
					}
					catch (Exception exception4)
					{
						player.Kick("RPC Error in SERVER_AttackAll");
						Debug.LogException(exception4);
					}
				}
				flag = true;
			}
			else if (rpc == -1029428465 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SERVER_Peacekeeper "));
				}
				using (timeWarning1 = TimeWarning.New("SERVER_Peacekeeper", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_Peacekeeper", this, player, 3f))
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
							this.SERVER_Peacekeeper(rPCMessage);
						}
					}
					catch (Exception exception5)
					{
						player.Kick("RPC Error in SERVER_Peacekeeper");
						Debug.LogException(exception5);
					}
				}
				flag = true;
			}
			else if (rpc == 190717079 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SERVER_TurnOff "));
				}
				using (timeWarning1 = TimeWarning.New("SERVER_TurnOff", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_TurnOff", this, player, 3f))
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
							this.SERVER_TurnOff(rPCMessage);
						}
					}
					catch (Exception exception6)
					{
						player.Kick("RPC Error in SERVER_TurnOff");
						Debug.LogException(exception6);
					}
				}
				flag = true;
			}
			else if (rpc != -1751444115 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SERVER_TurnOn "));
				}
				using (timeWarning1 = TimeWarning.New("SERVER_TurnOn", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("SERVER_TurnOn", this, player, 3f))
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
							this.SERVER_TurnOn(rPCMessage);
						}
					}
					catch (Exception exception7)
					{
						player.Kick("RPC Error in SERVER_TurnOn");
						Debug.LogException(exception7);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public bool PeacekeeperMode()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		this.EnsureReloaded();
		this.nextShotTime = UnityEngine.Time.time;
	}

	public override void PostSave(BaseNetworkable.SaveInfo info)
	{
		base.PostSave(info);
		info.msg.autoturret.users = null;
	}

	public void Reload()
	{
		foreach (Item item in this.inventory.itemList)
		{
			if (item.info.itemid != this.ammoType.itemid || item.amount <= 0)
			{
				continue;
			}
			this.ammoItem = item;
			return;
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void RemoveSelfAuthorize(BaseEntity.RPCMessage rpc)
	{
		if (this.booting || this.IsOnline() || !this.IsAuthed(rpc.player))
		{
			return;
		}
		if (Interface.CallHook("OnTurretDeauthorize", this, rpc.player) != null)
		{
			return;
		}
		this.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == rpc.player.userID);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.autoturret = Facepunch.Pool.Get<ProtoBuf.AutoTurret>();
		info.msg.autoturret.users = this.authorizedPlayers;
	}

	public void SendAimDir()
	{
		if (UnityEngine.Time.realtimeSinceStartup > this.nextForcedAimTime || this.HasTarget() || Vector3.Angle(this.lastSentAimDir, this.aimDir) > 0.03f)
		{
			this.lastSentAimDir = this.aimDir;
			base.ClientRPC<Vector3>(null, "CLIENT_ReceiveAimDir", this.aimDir);
			this.nextForcedAimTime = UnityEngine.Time.realtimeSinceStartup + 2f;
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void SERVER_AttackAll(BaseEntity.RPCMessage rpc)
	{
		if (this.IsAuthed(rpc.player))
		{
			this.SetPeacekeepermode(false);
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void SERVER_Peacekeeper(BaseEntity.RPCMessage rpc)
	{
		if (this.IsAuthed(rpc.player))
		{
			this.SetPeacekeepermode(true);
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void SERVER_TurnOff(BaseEntity.RPCMessage rpc)
	{
		if (this.IsAuthed(rpc.player))
		{
			this.InitiateShutdown();
		}
	}

	[IsVisible(3f)]
	[RPC_Server]
	private void SERVER_TurnOn(BaseEntity.RPCMessage rpc)
	{
		if (this.IsAuthed(rpc.player))
		{
			this.InitiateStartup();
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.ServerTick), UnityEngine.Random.Range(0f, 1f), 0.015f);
		base.InvokeRandomized(new Action(this.SendAimDir), UnityEngine.Random.Range(0f, 1f), 0.2f, 0.05f);
		base.InvokeRandomized(new Action(this.TargetScan), UnityEngine.Random.Range(0f, 1f), this.TargetScanRate(), 0.2f);
		this.targetTrigger.GetComponent<SphereCollider>().radius = this.sightRange;
	}

	public void ServerTick()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.IsDestroyed)
		{
			return;
		}
		if (!this.IsOnline())
		{
			this.OfflineTick();
		}
		else if (!this.HasTarget())
		{
			this.IdleTick();
		}
		else
		{
			this.TargetTick();
		}
		this.UpdateFacingToTarget();
	}

	public void SetIsOnline(bool online)
	{
		if (online == base.HasFlag(BaseEntity.Flags.On))
		{
			return;
		}
		if (Interface.CallHook("OnTurretToggle", this) != null)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, online, false, true);
		this.booting = false;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		if (!this.IsOffline())
		{
			this.isLootable = false;
			return;
		}
		this.SetTarget(null);
		this.isLootable = true;
	}

	public void SetOnline()
	{
		this.SetIsOnline(true);
	}

	public void SetPeacekeepermode(bool isOn)
	{
		if (this.PeacekeeperMode() == isOn)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved1, isOn, false, true);
		Effect.server.Run(this.peacekeeperToggleSound.resourcePath, this, 0, Vector3.zero, Vector3.zero, null, false);
		Interface.CallHook("OnTurretModeToggle", this);
	}

	public void SetTarget(BaseCombatEntity targ)
	{
		if (Interface.CallHook("OnTurretTarget", this, targ) != null)
		{
			return;
		}
		if (targ != this.target)
		{
			Effect.server.Run((targ == null ? this.targetLostEffect.resourcePath : this.targetAcquiredEffect.resourcePath), base.transform.position, Vector3.up, null, false);
		}
		this.target = targ;
	}

	public void TargetScan()
	{
		BaseCombatEntity component;
		if (this.HasTarget() || this.IsOffline())
		{
			return;
		}
		if (this.targetTrigger.entityContents != null)
		{
			HashSet<BaseEntity>.Enumerator enumerator = this.targetTrigger.entityContents.GetEnumerator();
			try
			{
				while (true)
				{
					if (enumerator.MoveNext())
					{
						BaseEntity current = enumerator.Current;
						if (current != null)
						{
							component = current.GetComponent<BaseCombatEntity>();
							if (!(component == null) && component.IsAlive() && this.InFiringArc(component) && this.ObjectVisible(component))
							{
								if (!Sentry.targetall)
								{
									BasePlayer basePlayer = component as BasePlayer;
									if (basePlayer && (this.IsAuthed(basePlayer) || this.Ignore(basePlayer)))
									{
										continue;
									}
								}
								if (!(component is AutoTurret))
								{
									if (!this.PeacekeeperMode())
									{
										break;
									}
									if (this.IsEntityHostile(component))
									{
										if (this.target != null)
										{
											break;
										}
										this.nextShotTime = UnityEngine.Time.time + 1f;
										break;
									}
								}
							}
						}
					}
					else
					{
						return;
					}
				}
				this.SetTarget(component);
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
	}

	public virtual float TargetScanRate()
	{
		return 1f;
	}

	public void TargetTick()
	{
		BaseCombatEntity baseCombatEntity;
		if (UnityEngine.Time.realtimeSinceStartup >= this.nextVisCheck)
		{
			this.nextVisCheck = UnityEngine.Time.realtimeSinceStartup + UnityEngine.Random.Range(0.2f, 0.3f);
			this.targetVisible = this.ObjectVisible(this.target);
			if (this.targetVisible)
			{
				this.lastTargetSeenTime = UnityEngine.Time.realtimeSinceStartup;
			}
		}
		if (UnityEngine.Time.time >= this.nextShotTime && this.targetVisible && this.AngleToTarget(this.target) < 10f)
		{
			this.EnsureReloaded();
			if (!this.HasAmmo())
			{
				this.nextShotTime = UnityEngine.Time.time + 60f;
			}
			else
			{
				Vector3 vector3 = this.AimOffset(this.target);
				float single = this.aimCone;
				if (this.PeacekeeperMode())
				{
					baseCombatEntity = this.target;
				}
				else
				{
					baseCombatEntity = null;
				}
				this.FireGun(vector3, single, null, baseCombatEntity);
				this.nextShotTime = UnityEngine.Time.time + 0.115f;
				if (this.ammoItem != null)
				{
					this.ammoItem.UseItem(1);
				}
			}
		}
		if (this.target.IsDead() || UnityEngine.Time.realtimeSinceStartup - this.lastTargetSeenTime > 3f || Vector3.Distance(base.transform.position, this.target.transform.position) > this.sightRange || this.PeacekeeperMode() && !this.IsEntityHostile(this.target))
		{
			this.SetTarget(null);
		}
	}

	public void UpdateAiming()
	{
		if (this.aimDir == Vector3.zero)
		{
			return;
		}
		float single = (base.isServer ? 16f : 5f);
		Quaternion quaternion = Quaternion.LookRotation(this.aimDir);
		Quaternion quaternion1 = Quaternion.Euler(0f, quaternion.eulerAngles.y, 0f);
		Quaternion quaternion2 = Quaternion.Euler(quaternion.eulerAngles.x, 0f, 0f);
		if (this.gun_yaw.transform.rotation != quaternion1)
		{
			this.gun_yaw.transform.rotation = Quaternion.Lerp(this.gun_yaw.transform.rotation, quaternion1, UnityEngine.Time.deltaTime * single);
		}
		if (this.gun_pitch.transform.localRotation != quaternion2)
		{
			this.gun_pitch.transform.localRotation = Quaternion.Lerp(this.gun_pitch.transform.localRotation, quaternion2, UnityEngine.Time.deltaTime * single);
		}
	}

	public void UpdateFacingToTarget()
	{
		if (this.target != null && this.targetVisible)
		{
			Vector3 vector3 = this.AimOffset(this.target);
			Vector3 vector31 = this.gun_pitch.transform.InverseTransformPoint(this.muzzlePos.transform.position);
			float single = 0f;
			float single1 = single;
			vector31.x = single;
			vector31.z = single1;
			Vector3 gunPitch = vector3 - (this.gun_pitch.position + vector31);
			this.aimDir = gunPitch;
		}
		this.UpdateAiming();
	}

	public static class TurretFlags
	{
		public const BaseEntity.Flags Peacekeeper = BaseEntity.Flags.Reserved1;
	}
}