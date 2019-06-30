using ConVar;
using EasyAntiCheat.Server.Cerberus;
using EasyAntiCheat.Server.Hydra;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Facepunch.Rust;
using Network;
using Network.Visibility;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using ProtoBuf;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

public class BasePlayer : BaseCombatEntity
{
	[Header("BasePlayer")]
	public GameObjectRef fallDamageEffect;

	public GameObjectRef drownEffect;

	[InspectorFlags]
	public BasePlayer.PlayerFlags playerFlags;

	[NonSerialized]
	public PlayerEyes eyes;

	[NonSerialized]
	public PlayerInventory inventory;

	[NonSerialized]
	public PlayerBlueprints blueprints;

	[NonSerialized]
	public PlayerMetabolism metabolism;

	[NonSerialized]
	public PlayerInput input;

	[NonSerialized]
	public BaseMovement movement;

	[NonSerialized]
	public BaseCollision collision;

	public PlayerBelt Belt;

	[NonSerialized]
	private Collider triggerCollider;

	[NonSerialized]
	private Rigidbody physicsRigidbody;

	[NonSerialized]
	public ulong userID;

	[NonSerialized]
	public string UserIDString;

	protected string _displayName;

	public ProtectionProperties cachedProtection;

	private const int displayNameMaxLength = 32;

	public bool clothingBlocksAiming;

	public float clothingMoveSpeedReduction;

	public float clothingWaterSpeedBonus;

	public float clothingAccuracyBonus;

	public bool equippingBlocked;

	public float eggVision;

	[NonSerialized]
	public bool isInAir;

	[NonSerialized]
	public bool isOnPlayer;

	[NonSerialized]
	public float violationLevel;

	[NonSerialized]
	public float lastViolationTime;

	[NonSerialized]
	public float lastAdminCheatTime;

	[NonSerialized]
	public AntiHackType lastViolationType;

	[NonSerialized]
	public float vehiclePauseTime;

	[NonSerialized]
	public float speedhackPauseTime;

	[NonSerialized]
	public float speedhackDistance;

	[NonSerialized]
	public float flyhackPauseTime;

	[NonSerialized]
	public float flyhackDistanceVertical;

	[NonSerialized]
	public float flyhackDistanceHorizontal;

	[NonSerialized]
	public PlayerModel playerModel;

	private const float drinkRange = 1.5f;

	private const float drinkMovementSpeed = 0.1f;

	[NonSerialized]
	private BasePlayer.NetworkQueueList[] networkQueue = new BasePlayer.NetworkQueueList[] { new BasePlayer.NetworkQueueList(), new BasePlayer.NetworkQueueList() };

	[NonSerialized]
	private BasePlayer.NetworkQueueList SnapshotQueue = new BasePlayer.NetworkQueueList();

	[NonSerialized]
	protected bool lightsOn = true;

	public ulong currentTeam;

	[NonSerialized]
	public ModelState modelState = new ModelState()
	{
		onground = true
	};

	[NonSerialized]
	private ModelState modelStateTick;

	[NonSerialized]
	private bool wantsSendModelState;

	[NonSerialized]
	private float nextModelStateUpdate;

	[NonSerialized]
	private EntityRef mounted;

	private float nextSeatSwapTime;

	public Dictionary<int, BasePlayer.FiredProjectile> firedProjectiles = new Dictionary<int, BasePlayer.FiredProjectile>();

	[NonSerialized]
	public PlayerStatistics stats;

	[NonSerialized]
	public uint svActiveItemID;

	[NonSerialized]
	public float NextChatTime;

	[NonSerialized]
	public float nextSuicideTime;

	public UnityEngine.Vector3 viewAngles;

	private float lastSubscriptionTick;

	private float lastPlayerTick;

	private const float playerTickRate = 0.0625f;

	private float sleepStartTime = -1f;

	private float fallTickRate = 0.1f;

	private float lastFallTime;

	private float fallVelocity;

	public static List<BasePlayer> activePlayerList;

	public static List<BasePlayer> sleepingPlayerList;

	public float cachedCraftLevel;

	public float nextCheckTime;

	[NonSerialized]
	public PlayerLifeStory lifeStory;

	[NonSerialized]
	public PlayerLifeStory previousLifeStory;

	private int SpectateOffset = 1000000;

	public string spectateFilter = "";

	private float lastUpdateTime = Single.NegativeInfinity;

	public float cachedThreatLevel;

	public float weaponDrawnDuration;

	[NonSerialized]
	public InputState serverInput = new InputState();

	[NonSerialized]
	private float lastTickTime;

	[NonSerialized]
	private float lastStallTime;

	[NonSerialized]
	private float lastInputTime;

	public PlayerTick lastReceivedTick = new PlayerTick();

	private float tickDeltaTime;

	private bool tickNeedsFinalizing;

	private UnityEngine.Vector3 tickViewAngles;

	private TickInterpolator tickInterpolator = new TickInterpolator();

	private float woundedDuration;

	private float woundedStartTime;

	private float lastWoundedTime = Single.NegativeInfinity;

	[NonSerialized]
	public Oxide.Core.Libraries.Covalence.IPlayer IPlayer;

	public Network.Connection Connection
	{
		get
		{
			if (this.net == null)
			{
				return null;
			}
			return this.net.connection;
		}
	}

	public float currentComfort
	{
		get
		{
			float comfort = 0f;
			if (this.isMounted)
			{
				comfort = this.GetMounted().GetComfort();
			}
			if (this.triggers == null)
			{
				return comfort;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerComfort item = this.triggers[i] as TriggerComfort;
				if (item != null)
				{
					float single = item.CalculateComfort(base.transform.position, this);
					if (single > comfort)
					{
						comfort = single;
					}
				}
			}
			return comfort;
		}
	}

	public float currentCraftLevel
	{
		get
		{
			if (this.triggers == null)
			{
				return 0f;
			}
			if (this.nextCheckTime > UnityEngine.Time.realtimeSinceStartup)
			{
				return this.cachedCraftLevel;
			}
			this.nextCheckTime = UnityEngine.Time.realtimeSinceStartup + UnityEngine.Random.Range(0.4f, 0.5f);
			float single = 0f;
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerWorkbench item = this.triggers[i] as TriggerWorkbench;
				if (!(item == null) && !(item.parentBench == null) && item.parentBench.IsVisible(this.eyes.position, Single.PositiveInfinity))
				{
					float single1 = item.WorkbenchLevel();
					if (single1 > single)
					{
						single = single1;
					}
				}
			}
			this.cachedCraftLevel = single;
			return single;
		}
	}

	public float currentSafeLevel
	{
		get
		{
			float single = 0f;
			if (this.triggers == null)
			{
				return single;
			}
			for (int i = 0; i < this.triggers.Count; i++)
			{
				TriggerSafeZone item = this.triggers[i] as TriggerSafeZone;
				if (item != null)
				{
					float safeLevel = item.GetSafeLevel(base.transform.position);
					if (safeLevel > single)
					{
						single = safeLevel;
					}
				}
			}
			return single;
		}
	}

	public float desyncTime
	{
		get;
		private set;
	}

	public string displayName
	{
		get
		{
			return this._displayName;
		}
		set
		{
			string str = value;
			str = str.Replace("<", "(");
			str = str.Replace(">", ")");
			str = str.Trim();
			if (str.Length == 0)
			{
				str = this.userID.ToString();
			}
			this._displayName = str;
		}
	}

	public float estimatedSpeed
	{
		get;
		private set;
	}

	public float estimatedSpeed2D
	{
		get;
		private set;
	}

	public UnityEngine.Vector3 estimatedVelocity
	{
		get
		{
			return JustDecompileGenerated_get_estimatedVelocity();
		}
		set
		{
			JustDecompileGenerated_set_estimatedVelocity(value);
		}
	}

	private UnityEngine.Vector3 JustDecompileGenerated_estimatedVelocity_k__BackingField;

	public UnityEngine.Vector3 JustDecompileGenerated_get_estimatedVelocity()
	{
		return this.JustDecompileGenerated_estimatedVelocity_k__BackingField;
	}

	private void JustDecompileGenerated_set_estimatedVelocity(UnityEngine.Vector3 value)
	{
		this.JustDecompileGenerated_estimatedVelocity_k__BackingField = value;
	}

	public virtual BaseNpc.AiStatistics.FamilyEnum Family
	{
		get
		{
			return BaseNpc.AiStatistics.FamilyEnum.Player;
		}
	}

	public bool hasPreviousLife
	{
		get
		{
			return this.previousLifeStory != null;
		}
	}

	public float IdleTime
	{
		get
		{
			if (this.lastInputTime == 0f)
			{
				return 0f;
			}
			return UnityEngine.Time.time - this.lastInputTime;
		}
	}

	public bool IsAdmin
	{
		get
		{
			return this.HasPlayerFlag(BasePlayer.PlayerFlags.IsAdmin);
		}
	}

	public bool IsAiming
	{
		get
		{
			return this.HasPlayerFlag(BasePlayer.PlayerFlags.Aiming);
		}
	}

	public bool IsConnected
	{
		get
		{
			if (!base.isServer)
			{
				return false;
			}
			if (Network.Net.sv == null)
			{
				return false;
			}
			if (this.net == null)
			{
				return false;
			}
			if (this.net.connection == null)
			{
				return false;
			}
			return true;
		}
	}

	public bool IsDeveloper
	{
		get
		{
			return this.HasPlayerFlag(BasePlayer.PlayerFlags.IsDeveloper);
		}
	}

	public bool IsFlying
	{
		get
		{
			if (this.modelState == null)
			{
				return false;
			}
			return this.modelState.flying;
		}
	}

	public bool isMounted
	{
		get
		{
			return this.mounted.IsValid(base.isServer);
		}
	}

	public bool IsReceivingSnapshot
	{
		get
		{
			return this.HasPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot);
		}
	}

	public bool isStalled
	{
		get
		{
			if (this.IsDead())
			{
				return false;
			}
			if (this.IsSleeping())
			{
				return false;
			}
			return this.timeSinceLastTick > 1f;
		}
	}

	protected override float PositionTickRate
	{
		get
		{
			return -1f;
		}
	}

	public int secondsConnected
	{
		get;
		private set;
	}

	public float secondsSinceWoundedStarted
	{
		get
		{
			return UnityEngine.Time.realtimeSinceStartup - this.woundedStartTime;
		}
	}

	public float secondsSleeping
	{
		get
		{
			if (this.sleepStartTime == -1f || !this.IsSleeping())
			{
				return 0f;
			}
			return UnityEngine.Time.time - this.sleepStartTime;
		}
	}

	public float timeSinceLastTick
	{
		get
		{
			if (this.lastTickTime == 0f)
			{
				return 0f;
			}
			return UnityEngine.Time.time - this.lastTickTime;
		}
	}

	public override BaseEntity.TraitFlag Traits
	{
		get
		{
			return base.Traits | BaseEntity.TraitFlag.Human | BaseEntity.TraitFlag.Food | BaseEntity.TraitFlag.Meat | BaseEntity.TraitFlag.Alive;
		}
	}

	public bool wasStalled
	{
		get
		{
			if (this.isStalled)
			{
				this.lastStallTime = UnityEngine.Time.time;
			}
			return UnityEngine.Time.time - this.lastStallTime < 1f;
		}
	}

	static BasePlayer()
	{
		BasePlayer.activePlayerList = new List<BasePlayer>();
		BasePlayer.sleepingPlayerList = new List<BasePlayer>();
	}

	public BasePlayer()
	{
	}

	public void AddWeaponDrawnDuration(float duration)
	{
		this.MarkWeaponDrawnDuration(this.weaponDrawnDuration + duration);
	}

	public static bool AnyPlayersVisibleToEntity(UnityEngine.Vector3 pos, float radius, BaseEntity source, UnityEngine.Vector3 entityEyePos, bool ignorePlayersWithPriv = false)
	{
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		List<BasePlayer> basePlayers = Facepunch.Pool.GetList<BasePlayer>();
		Vis.Entities<BasePlayer>(pos, radius, basePlayers, 131072, QueryTriggerInteraction.Collide);
		bool flag = false;
		foreach (BasePlayer basePlayer in basePlayers)
		{
			if (basePlayer.IsSleeping() || !basePlayer.IsAlive() || basePlayer.IsBuildingAuthed() && ignorePlayersWithPriv)
			{
				continue;
			}
			list.Clear();
			UnityEngine.Vector3 vector3 = basePlayer.eyes.position;
			UnityEngine.Vector3 vector31 = entityEyePos - basePlayer.eyes.position;
			GamePhysics.TraceAll(new Ray(vector3, vector31.normalized), 0f, list, 9f, 1218519297, QueryTriggerInteraction.UseGlobal);
			int num = 0;
			while (num < list.Count)
			{
				BaseEntity entity = list[num].GetEntity();
				if (!(entity != null) || !(entity == source) && !entity.EqualNetID(source))
				{
					if (!(entity != null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
					num++;
				}
				else
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			Facepunch.Pool.FreeList<RaycastHit>(ref list);
			Facepunch.Pool.FreeList<BasePlayer>(ref basePlayers);
			return flag;
		}
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
		Facepunch.Pool.FreeList<BasePlayer>(ref basePlayers);
		return flag;
	}

	public void ApplyFallDamageFromVelocity(float velocity)
	{
		float single = Mathf.InverseLerp(-15f, -100f, velocity);
		if (single == 0f)
		{
			return;
		}
		if (Interface.CallHook("OnPlayerLand", this, single) != null)
		{
			return;
		}
		this.metabolism.bleeding.Add(single * 0.5f);
		float single1 = single * 500f;
		this.Hurt(single1, DamageType.Fall, null, true);
		if (single1 > 20f && this.fallDamageEffect.isValid)
		{
			Effect.server.Run(this.fallDamageEffect.resourcePath, base.transform.position, UnityEngine.Vector3.zero, null, false);
		}
		Interface.CallHook("OnPlayerLanded", this, single);
	}

	public override void AttackerInfo(PlayerLifeStory.DeathInfo info)
	{
		info.attackerName = this.displayName;
		info.attackerSteamID = this.userID;
	}

	public bool CanAttack()
	{
		HeldEntity heldEntity = this.GetHeldEntity();
		if (heldEntity == null)
		{
			return false;
		}
		bool flag = this.IsSwimming();
		bool flag1 = heldEntity.CanBeUsedInWater();
		if (this.modelState.onLadder)
		{
			return false;
		}
		if (!flag && !this.modelState.onground)
		{
			return false;
		}
		if (flag && !flag1)
		{
			return false;
		}
		if (this.IsEnsnared())
		{
			return false;
		}
		return true;
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		object obj = Interface.CallHook("CanLootPlayer", this, player);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (player == this)
		{
			return false;
		}
		if (this.IsWounded())
		{
			return true;
		}
		return this.IsSleeping();
	}

	public bool CanBuild()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		if (buildingPrivilege == null)
		{
			return true;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool CanBuild(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, Bounds bounds)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(new OBB(position, rotation, bounds));
		if (buildingPrivilege == null)
		{
			return true;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool CanBuild(OBB obb)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(obb);
		if (buildingPrivilege == null)
		{
			return true;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool CanInteract()
	{
		if (this.IsDead() || this.IsSleeping())
		{
			return false;
		}
		return !this.IsWounded();
	}

	public bool CanPlaceBuildingPrivilege()
	{
		return this.GetBuildingPrivilege() == null;
	}

	public bool CanPlaceBuildingPrivilege(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, Bounds bounds)
	{
		return base.GetBuildingPrivilege(new OBB(position, rotation, bounds)) == null;
	}

	public bool CanPlaceBuildingPrivilege(OBB obb)
	{
		return base.GetBuildingPrivilege(obb) == null;
	}

	public bool CanSuicide()
	{
		if (this.IsAdmin || this.IsDeveloper)
		{
			return true;
		}
		return UnityEngine.Time.realtimeSinceStartup > this.nextSuicideTime;
	}

	public override bool CanUseNetworkCache(Network.Connection connection)
	{
		if (this.net == null)
		{
			return true;
		}
		if (this.net.connection != connection)
		{
			return true;
		}
		return false;
	}

	public override string Categorize()
	{
		return "player";
	}

	public void ChatMessage(string msg)
	{
		if (!base.isServer)
		{
			return;
		}
		if (Interface.CallHook("OnMessagePlayer", msg, this) != null)
		{
			return;
		}
		this.SendConsoleCommand("chat.add", new object[] { 0, msg });
	}

	public void CheckDeathCondition(HitInfo info = null)
	{
		Assert.IsTrue(base.isServer, "CheckDeathCondition called on client!");
		if (this.IsSpectating())
		{
			return;
		}
		if (this.IsDead())
		{
			return;
		}
		if (this.metabolism.ShouldDie())
		{
			this.Die(info);
		}
	}

	public void CleanupExpiredProjectiles()
	{
		foreach (KeyValuePair<int, BasePlayer.FiredProjectile> list in (
			from x in this.firedProjectiles
			where x.Value.firedTime < UnityEngine.Time.realtimeSinceStartup - 8f - 1f
			select x).ToList<KeyValuePair<int, BasePlayer.FiredProjectile>>())
		{
			this.firedProjectiles.Remove(list.Key);
		}
	}

	public void ClearEntityQueue(Group group = null)
	{
		this.SnapshotQueue.Clear(group);
		this.networkQueue[0].Clear(group);
		this.networkQueue[1].Clear(group);
	}

	public void ClearPendingInvite()
	{
		base.ClientRPCPlayer<string, int>(null, this, "CLIENT_PendingInvite", "", 0);
	}

	public void ClearTeam()
	{
		this.currentTeam = (ulong)0;
		base.ClientRPCPlayer(null, this, "CLIENT_ClearTeam");
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	[FromOwner]
	[RPC_Server]
	private void ClientKeepConnectionAlive(BaseEntity.RPCMessage msg)
	{
		this.lastTickTime = UnityEngine.Time.time;
	}

	[FromOwner]
	[RPC_Server]
	private void ClientLoadingComplete(BaseEntity.RPCMessage msg)
	{
	}

	public void Command(string strCommand, params object[] arguments)
	{
		if (this.net.connection == null)
		{
			return;
		}
		ConsoleNetwork.SendClientCommand(this.net.connection, strCommand, arguments);
	}

	private void ConnectedPlayerUpdate(float deltaTime)
	{
		if (this.IsReceivingSnapshot)
		{
			this.net.UpdateSubscriptions(2147483647, 2147483647);
		}
		else if (UnityEngine.Time.realtimeSinceStartup > this.lastSubscriptionTick + ConVar.Server.entitybatchtime && this.net.UpdateSubscriptions(ConVar.Server.entitybatchsize, ConVar.Server.entitybatchsize))
		{
			this.lastSubscriptionTick = UnityEngine.Time.realtimeSinceStartup;
		}
		this.SendEntityUpdate();
		if (this.IsReceivingSnapshot)
		{
			if (this.SnapshotQueue.Length == 0 && EACServer.IsAuthenticated(this.net.connection))
			{
				this.EnterGame();
			}
			return;
		}
		if (this.IsAlive())
		{
			this.metabolism.ServerUpdate(this, deltaTime);
			if (!this.InSafeZone())
			{
				this.MarkWeaponDrawnDuration(0f);
			}
			else
			{
				float single = 0f;
				HeldEntity heldEntity = this.GetHeldEntity();
				if (heldEntity && heldEntity.hostile)
				{
					single = deltaTime;
				}
				if (single != 0f)
				{
					this.AddWeaponDrawnDuration(single);
				}
				else
				{
					this.MarkWeaponDrawnDuration(0f);
				}
				if (this.weaponDrawnDuration >= 5f)
				{
					this.MarkHostileFor(30f);
				}
			}
			if (this.timeSinceLastTick > (float)ConVar.Server.playertimeout)
			{
				this.lastTickTime = 0f;
				this.Kick("Unresponsive");
				return;
			}
		}
		int secondsConnected = (int)this.net.connection.GetSecondsConnected();
		int num = secondsConnected - this.secondsConnected;
		if (num > 0)
		{
			this.stats.Add("time", num, Stats.Server);
			this.secondsConnected = secondsConnected;
		}
		this.SendModelState();
	}

	public void ConsoleMessage(string msg)
	{
		if (!base.isServer)
		{
			return;
		}
		this.SendConsoleCommand(string.Concat("echo ", msg), Array.Empty<object>());
	}

	public virtual BaseCorpse CreateCorpse()
	{
		BaseCorpse baseCorpse;
		using (TimeWarning timeWarning = TimeWarning.New("Create corpse", 0.1f))
		{
			LootableCorpse lootableCorpse = base.DropCorpse("assets/prefabs/player/player_corpse.prefab") as LootableCorpse;
			if (!lootableCorpse)
			{
				return null;
			}
			else
			{
				lootableCorpse.SetFlag(BaseEntity.Flags.Reserved5, this.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
				lootableCorpse.TakeFrom(new ItemContainer[] { this.inventory.containerMain, this.inventory.containerWear, this.inventory.containerBelt });
				lootableCorpse.playerName = this.displayName;
				lootableCorpse.playerSteamID = this.userID;
				lootableCorpse.Spawn();
				lootableCorpse.TakeChildren(this);
				ResourceDispenser component = lootableCorpse.GetComponent<ResourceDispenser>();
				int num = 2;
				if (this.lifeStory != null)
				{
					num += Mathf.Clamp(Mathf.FloorToInt(this.lifeStory.secondsAlive / 180f), 0, 20);
				}
				component.containedItems.Add(new ItemAmount(ItemManager.FindItemDefinition("fat.animal"), (float)num));
				baseCorpse = lootableCorpse;
			}
		}
		return baseCorpse;
	}

	protected virtual void CreateWorldProjectile(HitInfo info, ItemDefinition itemDef, ItemModProjectile itemMod, Projectile projectilePrefab, Item recycleItem)
	{
		if (Interface.CallHook("CanCreateWorldProjectile", info, itemDef) != null)
		{
			return;
		}
		UnityEngine.Vector3 projectileVelocity = info.ProjectileVelocity;
		Item item = (recycleItem != null ? recycleItem : ItemManager.Create(itemDef, 1, (ulong)0));
		if (Interface.CallHook("OnCreateWorldProjectile", info, item) != null)
		{
			return;
		}
		BaseEntity baseEntity = null;
		if (!info.DidHit)
		{
			baseEntity = item.CreateWorldObject(info.HitPositionWorld, UnityEngine.Quaternion.LookRotation(projectileVelocity.normalized), null, 0);
			baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);
			return;
		}
		if (projectilePrefab.breakProbability > 0f && UnityEngine.Random.@value <= projectilePrefab.breakProbability)
		{
			baseEntity = item.CreateWorldObject(info.HitPositionWorld, UnityEngine.Quaternion.LookRotation(projectileVelocity.normalized), null, 0);
			baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);
			return;
		}
		if (projectilePrefab.conditionLoss > 0f)
		{
			item.LoseCondition(projectilePrefab.conditionLoss * 100f);
			if (item.isBroken)
			{
				baseEntity = item.CreateWorldObject(info.HitPositionWorld, UnityEngine.Quaternion.LookRotation(projectileVelocity.normalized), null, 0);
				baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);
				return;
			}
		}
		if (projectilePrefab.stickProbability <= 0f || UnityEngine.Random.@value > projectilePrefab.stickProbability)
		{
			baseEntity = item.CreateWorldObject(info.HitPositionWorld, UnityEngine.Quaternion.LookRotation(projectileVelocity.normalized), null, 0);
			Rigidbody component = baseEntity.GetComponent<Rigidbody>();
			component.AddForce(projectileVelocity.normalized * 200f);
			component.WakeUp();
			return;
		}
		if (info.HitEntity != null)
		{
			baseEntity = (info.HitBone != 0 ? item.CreateWorldObject(info.HitPositionLocal, UnityEngine.Quaternion.LookRotation(info.HitNormalLocal * -1f), info.HitEntity, info.HitBone) : item.CreateWorldObject(info.HitPositionLocal, UnityEngine.Quaternion.LookRotation(info.HitEntity.transform.InverseTransformDirection(projectileVelocity.normalized)), info.HitEntity, 0));
		}
		else
		{
			baseEntity = item.CreateWorldObject(info.HitPositionWorld, UnityEngine.Quaternion.LookRotation(projectileVelocity.normalized), null, 0);
		}
		baseEntity.GetComponent<Rigidbody>().isKinematic = true;
	}

	public void DelayedRigidbodyDisable()
	{
		this.UpdatePlayerRigidbody(false);
	}

	private void DelayedServerFall()
	{
		this.EnableServerFall(true);
	}

	public void DelayedTeamUpdate()
	{
		this.UpdateTeam(this.currentTeam);
	}

	public override void DestroyShared()
	{
		UnityEngine.Object.Destroy(this.cachedProtection);
		UnityEngine.Object.Destroy(this.baseProtection);
		base.DestroyShared();
	}

	public override void Die(HitInfo info = null)
	{
		using (TimeWarning timeWarning = TimeWarning.New("Player.Die", 0.1f))
		{
			if (!this.IsDead())
			{
				if (this.Belt != null && this.ShouldDropActiveItem())
				{
					UnityEngine.Vector3 vector3 = new UnityEngine.Vector3(UnityEngine.Random.Range(-2f, 2f), 0.2f, UnityEngine.Random.Range(-2f, 2f));
					this.Belt.DropActive(this.GetDropPosition(), this.GetInheritedDropVelocity() + (vector3.normalized * 3f));
				}
				if (!this.WoundInsteadOfDying(info))
				{
					if (Interface.CallHook("OnPlayerDie", this, info) != null)
					{
						return;
					}
					base.Die(info);
				}
			}
		}
	}

	public virtual void DismountObject()
	{
		this.mounted.Set(null);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		BaseEntity.Query.Server.RemovePlayer(this);
		if (this.inventory)
		{
			this.inventory.DoDestroy();
		}
		BasePlayer.sleepingPlayerList.Remove(this);
	}

	private void EACStateUpdate()
	{
		if (this.net == null || this.net.connection == null)
		{
			return;
		}
		if (EACServer.playerTracker == null)
		{
			return;
		}
		if (this.IsReceivingSnapshot)
		{
			return;
		}
		UnityEngine.Vector3 vector3 = this.eyes.position;
		UnityEngine.Quaternion quaternion = this.eyes.rotation;
		EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(this.net.connection);
		EasyAntiCheat.Server.Cerberus.PlayerTick playerTick = new EasyAntiCheat.Server.Cerberus.PlayerTick()
		{
			Position = new EasyAntiCheat.Server.Cerberus.Vector3(vector3.x, vector3.y, vector3.z),
			ViewRotation = new EasyAntiCheat.Server.Cerberus.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w)
		};
		if (this.IsDucked())
		{
			ref PlayerTickFlags tickFlags = ref playerTick.TickFlags;
			tickFlags = (PlayerTickFlags)((int)tickFlags | 1);
		}
		if (this.isMounted)
		{
			ref PlayerTickFlags playerTickFlagsPointer = ref playerTick.TickFlags;
			playerTickFlagsPointer = (PlayerTickFlags)((int)playerTickFlagsPointer | 4);
		}
		if (this.IsWounded())
		{
			ref PlayerTickFlags tickFlags1 = ref playerTick.TickFlags;
			tickFlags1 = (PlayerTickFlags)((int)tickFlags1 | 8);
		}
		if (this.IsSwimming())
		{
			ref PlayerTickFlags playerTickFlagsPointer1 = ref playerTick.TickFlags;
			playerTickFlagsPointer1 = (PlayerTickFlags)((int)playerTickFlagsPointer1 | 16);
		}
		if (!this.IsOnGround())
		{
			ref PlayerTickFlags tickFlags2 = ref playerTick.TickFlags;
			tickFlags2 = (PlayerTickFlags)((int)tickFlags2 | 32);
		}
		if (this.OnLadder())
		{
			ref PlayerTickFlags playerTickFlagsPointer2 = ref playerTick.TickFlags;
			playerTickFlagsPointer2 = (PlayerTickFlags)((int)playerTickFlagsPointer2 | 64);
		}
		using (TimeWarning timeWarning = TimeWarning.New("playerTracker.LogPlayerState", 0.1f))
		{
			try
			{
				EACServer.playerTracker.LogPlayerTick(client, playerTick);
			}
			catch (Exception exception)
			{
				Debug.LogWarning("Disabling EAC Logging due to exception");
				EACServer.playerTracker = null;
				Debug.LogException(exception);
			}
		}
	}

	public virtual bool EligibleForWounding(HitInfo info)
	{
		object obj = Interface.CallHook("CanBeWounded", this, info);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (!ConVar.Server.woundingenabled)
		{
			return false;
		}
		if (this.IsSleeping())
		{
			return false;
		}
		if (this.isMounted)
		{
			return false;
		}
		if (info == null)
		{
			return false;
		}
		if (UnityEngine.Time.realtimeSinceStartup - this.lastWoundedTime < 60f)
		{
			return false;
		}
		if (info.WeaponPrefab is BaseMelee)
		{
			return true;
		}
		if (info.WeaponPrefab is BaseProjectile)
		{
			return !info.isHeadshot;
		}
		DamageType majorityDamageType = info.damageTypes.GetMajorityDamageType();
		if (majorityDamageType == DamageType.Suicide)
		{
			return false;
		}
		if (majorityDamageType == DamageType.Fall)
		{
			return true;
		}
		if (majorityDamageType == DamageType.Bite)
		{
			return true;
		}
		if (majorityDamageType == DamageType.Bleeding)
		{
			return true;
		}
		if (majorityDamageType == DamageType.Hunger)
		{
			return true;
		}
		if (majorityDamageType == DamageType.Thirst)
		{
			return true;
		}
		if (majorityDamageType == DamageType.Poison)
		{
			return true;
		}
		return false;
	}

	public void EnableServerFall(bool wantsOn)
	{
		if (!wantsOn || !ConVar.Server.playerserverfall)
		{
			base.CancelInvoke(new Action(this.ServerFall));
			this.SetPlayerFlag(BasePlayer.PlayerFlags.ServerFall, false);
		}
		else if (!base.IsInvoking(new Action(this.ServerFall)))
		{
			this.SetPlayerFlag(BasePlayer.PlayerFlags.ServerFall, true);
			this.lastFallTime = UnityEngine.Time.time - this.fallTickRate;
			base.InvokeRandomized(new Action(this.ServerFall), 0f, this.fallTickRate, this.fallTickRate * 0.1f);
			this.fallVelocity = this.estimatedVelocity.y;
			return;
		}
	}

	public virtual void EndLooting()
	{
		if (this.inventory.loot)
		{
			this.inventory.loot.Clear();
		}
	}

	public virtual void EndSleeping()
	{
		if (!this.IsSleeping())
		{
			return;
		}
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Sleeping, false);
		this.sleepStartTime = -1f;
		BasePlayer.sleepingPlayerList.Remove(this);
		base.InvokeRepeating(new Action(this.InventoryUpdate), 1f, 0.1f * UnityEngine.Random.Range(0.99f, 1.01f));
		if (RelationshipManager.TeamsEnabled())
		{
			base.InvokeRandomized(new Action(this.TeamUpdate), 1f, 4f, 1f);
		}
		this.UpdatePlayerCollider(true);
		this.UpdatePlayerRigidbody(true);
		this.EnableServerFall(false);
		Interface.CallHook("OnPlayerSleepEnded", this);
		if (EACServer.playerTracker != null && this.net.connection != null)
		{
			using (TimeWarning timeWarning = TimeWarning.New("playerTracker.LogPlayerSpawn", 0.1f))
			{
				EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(this.net.connection);
				EACServer.playerTracker.LogPlayerSpawn(client, 0, 0);
			}
		}
	}

	public void EnsureDismounted()
	{
		if (this.isMounted)
		{
			this.GetMounted().DismountPlayer(this, false);
		}
	}

	public void EnsureUpdated()
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastUpdateTime < 30f)
		{
			return;
		}
		this.lastUpdateTime = UnityEngine.Time.realtimeSinceStartup;
		this.cachedThreatLevel = 0f;
		if (!this.IsSleeping())
		{
			if (this.inventory.containerWear.itemList.Count > 2)
			{
				this.cachedThreatLevel += 1f;
			}
			foreach (Item item in this.inventory.containerBelt.itemList)
			{
				BaseEntity heldEntity = item.GetHeldEntity();
				if (!heldEntity || !(heldEntity is BaseProjectile) || heldEntity is BowWeapon)
				{
					continue;
				}
				this.cachedThreatLevel += 2f;
				return;
			}
		}
	}

	private void EnterGame()
	{
		this.SetPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot, false);
		base.ClientRPCPlayer(null, this, "FinishLoading");
		base.Invoke(new Action(this.DelayedTeamUpdate), 1f);
		if (this.net != null)
		{
			EACServer.OnFinishLoading(this.net.connection);
		}
		Debug.LogFormat("{0} has entered the game", new object[] { this });
	}

	private void EnterVisibility(Group group)
	{
		ServerMgr.OnEnterVisibility(this.net.connection, group);
		this.SendSnapshots(group.networkables);
	}

	private void FinalizeTick(float deltaTime)
	{
		this.tickDeltaTime += deltaTime;
		if (this.IsReceivingSnapshot)
		{
			return;
		}
		if (!this.tickNeedsFinalizing)
		{
			return;
		}
		this.tickNeedsFinalizing = false;
		using (TimeWarning timeWarning = TimeWarning.New("ModelState", 0.1f))
		{
			if (this.modelStateTick != null)
			{
				if (this.modelState != null)
				{
					if (this.modelStateTick.flying && !this.IsAdmin && !this.IsDeveloper)
					{
						AntiHack.NoteAdminHack(this);
					}
					if (ConVar.AntiHack.modelstate && this.TriggeredAntiHack(1f, Single.PositiveInfinity))
					{
						this.modelStateTick.ducked = this.modelState.ducked;
					}
					this.modelState.ResetToPool();
					this.modelState = null;
				}
				this.modelState = this.modelStateTick;
				this.modelStateTick = null;
				this.UpdateModelState();
			}
		}
		using (timeWarning = TimeWarning.New("Transform", 0.1f))
		{
			this.UpdateEstimatedVelocity(this.tickInterpolator.StartPoint, this.tickInterpolator.EndPoint, this.tickDeltaTime);
			bool startPoint = this.tickInterpolator.StartPoint != this.tickInterpolator.EndPoint;
			bool flag = this.tickViewAngles != this.viewAngles;
			if (startPoint)
			{
				if (!AntiHack.ValidateMove(this, this.tickInterpolator, this.tickDeltaTime))
				{
					startPoint = false;
					if (ConVar.AntiHack.forceposition)
					{
						base.ClientRPCPlayer<UnityEngine.Vector3, uint>(null, this, "ForcePositionToParentOffset", base.transform.localPosition, this.parentEntity.uid);
					}
				}
				else
				{
					base.transform.localPosition = this.tickInterpolator.EndPoint;
					AntiHack.FadeViolations(this, this.tickDeltaTime);
				}
			}
			this.tickInterpolator.Reset(base.transform.localPosition);
			if (flag)
			{
				this.viewAngles = this.tickViewAngles;
				base.transform.rotation = UnityEngine.Quaternion.identity;
				base.transform.hasChanged = true;
			}
			if (startPoint | flag)
			{
				this.eyes.NetworkUpdate(UnityEngine.Quaternion.Euler(this.viewAngles));
				base.NetworkPositionTick();
			}
		}
		using (timeWarning = TimeWarning.New("EACStateUpdate", 0.1f))
		{
			this.EACStateUpdate();
		}
		using (timeWarning = TimeWarning.New("AntiHack.EnforceViolations", 0.1f))
		{
			AntiHack.EnforceViolations(this);
		}
		this.tickDeltaTime = 0f;
	}

	private static BasePlayer Find(string strNameOrIDOrIP, List<BasePlayer> list)
	{
		BasePlayer basePlayer = list.Find((BasePlayer x) => x.UserIDString == strNameOrIDOrIP);
		if (basePlayer)
		{
			return basePlayer;
		}
		BasePlayer basePlayer1 = list.Find((BasePlayer x) => x.displayName.StartsWith(strNameOrIDOrIP, StringComparison.CurrentCultureIgnoreCase));
		if (basePlayer1)
		{
			return basePlayer1;
		}
		BasePlayer basePlayer2 = list.Find((BasePlayer x) => {
			if (x.net == null || x.net.connection == null)
			{
				return false;
			}
			return x.net.connection.ipaddress == strNameOrIDOrIP;
		});
		if (basePlayer2)
		{
			return basePlayer2;
		}
		return null;
	}

	public static BasePlayer Find(string strNameOrIDOrIP)
	{
		return BasePlayer.Find(strNameOrIDOrIP, BasePlayer.activePlayerList);
	}

	public static BasePlayer FindByID(ulong userID)
	{
		BasePlayer basePlayer;
		using (TimeWarning timeWarning = TimeWarning.New("BasePlayer.FindByID", 0.1f))
		{
			basePlayer = BasePlayer.activePlayerList.Find((BasePlayer x) => x.userID == userID);
		}
		return basePlayer;
	}

	public static BasePlayer FindSleeping(ulong userID)
	{
		BasePlayer basePlayer;
		using (TimeWarning timeWarning = TimeWarning.New("BasePlayer.FindSleeping", 0.1f))
		{
			basePlayer = BasePlayer.sleepingPlayerList.Find((BasePlayer x) => x.userID == userID);
		}
		return basePlayer;
	}

	public static BasePlayer FindSleeping(string strNameOrIDOrIP)
	{
		return BasePlayer.Find(strNameOrIDOrIP, BasePlayer.sleepingPlayerList);
	}

	public void ForceUpdateTriggers(bool enter = true, bool exit = true, bool invoke = true)
	{
		List<TriggerBase> list = Facepunch.Pool.GetList<TriggerBase>();
		List<TriggerBase> triggerBases = Facepunch.Pool.GetList<TriggerBase>();
		if (this.triggers != null)
		{
			list.AddRange(this.triggers);
		}
		CapsuleCollider component = base.GetComponent<CapsuleCollider>();
		UnityEngine.Vector3 vector3 = base.transform.position + new UnityEngine.Vector3(0f, this.GetRadius(), 0f);
		UnityEngine.Vector3 vector31 = base.transform.position + new UnityEngine.Vector3(0f, this.GetHeight() - this.GetRadius(), 0f);
		GamePhysics.OverlapCapsule<TriggerBase>(vector3, vector31, this.GetRadius(), triggerBases, 262144, QueryTriggerInteraction.Collide);
		if (exit)
		{
			foreach (TriggerBase triggerBase in list)
			{
				if (triggerBases.Contains(triggerBase))
				{
					continue;
				}
				triggerBase.OnTriggerExit(component);
			}
		}
		if (enter)
		{
			foreach (TriggerBase triggerBase1 in triggerBases)
			{
				if (list.Contains(triggerBase1))
				{
					if (!(triggerBase1 is TriggerParent))
					{
						continue;
					}
					triggerBase1.OnEntityEnter(this);
				}
				else
				{
					triggerBase1.OnTriggerEnter(component);
				}
			}
		}
		Facepunch.Pool.FreeList<TriggerBase>(ref list);
		Facepunch.Pool.FreeList<TriggerBase>(ref triggerBases);
		if (invoke)
		{
			base.Invoke(new Action(this.ForceUpdateTriggersAction), UnityEngine.Time.fixedDeltaTime * 1.5f);
		}
	}

	private void ForceUpdateTriggersAction()
	{
		if (!base.IsDestroyed)
		{
			this.ForceUpdateTriggers(false, true, false);
		}
	}

	public Item GetActiveItem()
	{
		if (this.svActiveItemID == 0)
		{
			return null;
		}
		if (this.IsDead())
		{
			return null;
		}
		if (this.inventory == null || this.inventory.containerBelt == null)
		{
			return null;
		}
		return this.inventory.containerBelt.FindItemByUID(this.svActiveItemID);
	}

	public int GetAntiHackKicks()
	{
		return AntiHack.GetKickRecord(this);
	}

	public Bounds GetBounds(bool ducked)
	{
		return new Bounds(base.transform.position + this.GetOffset(ducked), this.GetSize(ducked));
	}

	public Bounds GetBounds()
	{
		return this.GetBounds(this.modelState.ducked);
	}

	public UnityEngine.Vector3 GetCenter(bool ducked)
	{
		return base.transform.position + this.GetOffset(ducked);
	}

	public UnityEngine.Vector3 GetCenter()
	{
		return this.GetCenter(this.modelState.ducked);
	}

	public string GetDebugStatus()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("Entity: {0}\n", this.ToString());
		stringBuilder.AppendFormat("Name: {0}\n", this.displayName);
		stringBuilder.AppendFormat("SteamID: {0}\n", this.userID);
		foreach (BasePlayer.PlayerFlags value in Enum.GetValues(typeof(BasePlayer.PlayerFlags)))
		{
			stringBuilder.AppendFormat("{1}: {0}\n", this.HasPlayerFlag(value), value);
		}
		return stringBuilder.ToString();
	}

	public override UnityEngine.Vector3 GetDropPosition()
	{
		return this.eyes.position;
	}

	public override UnityEngine.Vector3 GetDropVelocity()
	{
		return (this.GetInheritedDropVelocity() + (this.eyes.BodyForward() * 4f)) + Vector3Ex.Range(-0.5f, 0.5f);
	}

	public float GetHeight(bool ducked)
	{
		if (ducked)
		{
			return 1.1f;
		}
		return 1.8f;
	}

	public float GetHeight()
	{
		return this.GetHeight(this.modelState.ducked);
	}

	public HeldEntity GetHeldEntity()
	{
		if (!base.isServer)
		{
			return null;
		}
		Item activeItem = this.GetActiveItem();
		if (activeItem == null)
		{
			return null;
		}
		return activeItem.GetHeldEntity() as HeldEntity;
	}

	public virtual int GetInfoInt(string key, int defaultVal)
	{
		if (!this.IsConnected)
		{
			return defaultVal;
		}
		return this.net.connection.info.GetInt(key, defaultVal);
	}

	public override UnityEngine.Vector3 GetInheritedDropVelocity()
	{
		BaseMountable mounted = this.GetMounted();
		if (!mounted)
		{
			return base.GetInheritedDropVelocity();
		}
		return mounted.GetInheritedDropVelocity();
	}

	public override UnityEngine.Vector3 GetInheritedProjectileVelocity()
	{
		BaseMountable mounted = this.GetMounted();
		if (!mounted)
		{
			return base.GetInheritedProjectileVelocity();
		}
		return mounted.GetInheritedProjectileVelocity();
	}

	public override UnityEngine.Vector3 GetInheritedThrowVelocity()
	{
		BaseMountable mounted = this.GetMounted();
		if (!mounted)
		{
			return base.GetInheritedThrowVelocity();
		}
		return mounted.GetInheritedThrowVelocity();
	}

	public override Item GetItem(uint itemId)
	{
		if (this.inventory == null)
		{
			return null;
		}
		return this.inventory.FindItemUID(itemId);
	}

	public float GetJumpHeight()
	{
		return 1.5f;
	}

	public float GetMaxSpeed()
	{
		return this.GetSpeed(1f, 0f);
	}

	public float GetMinSpeed()
	{
		return this.GetSpeed(0f, 1f);
	}

	public BaseMountable GetMounted()
	{
		return this.mounted.Get(base.isServer) as BaseMountable;
	}

	public BaseVehicle GetMountedVehicle()
	{
		BaseMountable mounted = this.GetMounted();
		if (mounted == null)
		{
			return null;
		}
		return mounted.VehicleParent();
	}

	public override UnityEngine.Quaternion GetNetworkRotation()
	{
		if (!base.isServer)
		{
			return UnityEngine.Quaternion.identity;
		}
		return UnityEngine.Quaternion.Euler(this.viewAngles);
	}

	public UnityEngine.Vector3 GetOffset(bool ducked)
	{
		if (ducked)
		{
			return new UnityEngine.Vector3(0f, 0.55f, 0f);
		}
		return new UnityEngine.Vector3(0f, 0.9f, 0f);
	}

	public UnityEngine.Vector3 GetOffset()
	{
		return this.GetOffset(this.modelState.ducked);
	}

	public int GetQueuedUpdateCount(BasePlayer.NetworkQueue queue)
	{
		return this.networkQueue[(int)queue].Length;
	}

	public float GetRadius()
	{
		return 0.5f;
	}

	public UnityEngine.Vector3 GetSize(bool ducked)
	{
		if (ducked)
		{
			return new UnityEngine.Vector3(1f, 1.1f, 1f);
		}
		return new UnityEngine.Vector3(1f, 1.8f, 1f);
	}

	public UnityEngine.Vector3 GetSize()
	{
		return this.GetSize(this.modelState.ducked);
	}

	public float GetSpeed(float running, float ducking)
	{
		float single = 1f;
		single -= this.clothingMoveSpeedReduction;
		if (this.IsSwimming())
		{
			single += this.clothingWaterSpeedBonus;
		}
		return Mathf.Lerp(Mathf.Lerp(2.8f, 5.5f, running), 1.7f, ducking) * single;
	}

	public string GetSubName(int maxlen = 32)
	{
		string str = this.displayName;
		if (str.Length > maxlen)
		{
			str = string.Concat(str.Substring(0, maxlen), "..");
		}
		return str;
	}

	public override float GetThreatLevel()
	{
		this.EnsureUpdated();
		return this.cachedThreatLevel;
	}

	internal void GiveAchievement(string name)
	{
		if (GameInfo.HasAchievements)
		{
			base.ClientRPCPlayer<string>(null, this, "RecieveAchievement", name);
		}
	}

	public override void GiveItem(Item item, BaseEntity.GiveItemReason reason = 0)
	{
		if (reason == BaseEntity.GiveItemReason.ResourceHarvested)
		{
			this.stats.Add(string.Format("harvest.{0}", item.info.shortname), item.amount, Stats.Steam);
		}
		int num = item.amount;
		if (!this.inventory.GiveItem(item, null))
		{
			item.Drop(this.inventory.containerMain.dropPosition, this.inventory.containerMain.dropVelocity, new UnityEngine.Quaternion());
			return;
		}
		if (string.IsNullOrEmpty(item.name))
		{
			this.Command("note.inv", new object[] { item.info.itemid, num, string.Empty, (int)reason });
			return;
		}
		this.Command("note.inv", new object[] { item.info.itemid, num, item.name, (int)reason });
	}

	public bool HasFiredProjectile(int id)
	{
		return this.firedProjectiles.ContainsKey(id);
	}

	public bool HasHostileItem()
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("BasePlayer.HasHostileItem", 0.1f))
		{
			foreach (Item item in this.inventory.containerBelt.itemList)
			{
				if (!this.IsHostileItem(item))
				{
					continue;
				}
				flag = true;
				return flag;
			}
			foreach (Item item1 in this.inventory.containerMain.itemList)
			{
				if (!this.IsHostileItem(item1))
				{
					continue;
				}
				flag = true;
				return flag;
			}
			flag = false;
		}
		return flag;
	}

	public bool HasPlayerFlag(BasePlayer.PlayerFlags f)
	{
		return (this.playerFlags & f) == f;
	}

	public override void Hurt(HitInfo info)
	{
		if (this.IsDead())
		{
			return;
		}
		if (this.IsImmortal())
		{
			return;
		}
		if (Interface.CallHook("IOnBasePlayerHurt", this, info) != null)
		{
			return;
		}
		if (ConVar.Server.pve && info.Initiator && info.Initiator is BasePlayer && info.Initiator != this)
		{
			(info.Initiator as BasePlayer).Hurt(info.damageTypes.Total(), DamageType.Generic, null, true);
			return;
		}
		if (info.damageTypes.Get(DamageType.Drowned) > 5f && this.drownEffect.isValid)
		{
			Effect.server.Run(this.drownEffect.resourcePath, this, StringPool.Get("head"), UnityEngine.Vector3.zero, UnityEngine.Vector3.zero, null, false);
		}
		this.metabolism.pending_health.Subtract(info.damageTypes.Total() * 10f);
		BasePlayer initiatorPlayer = info.InitiatorPlayer;
		if (initiatorPlayer && initiatorPlayer != this)
		{
			if (initiatorPlayer.InSafeZone() || this.InSafeZone())
			{
				initiatorPlayer.MarkHostileFor(1800f);
			}
			if (initiatorPlayer.IsNpc && initiatorPlayer.Family == BaseNpc.AiStatistics.FamilyEnum.Murderer && info.damageTypes.Get(DamageType.Explosion) > 0f)
			{
				info.damageTypes.ScaleAll(Halloween.scarecrow_beancan_vs_player_dmg_modifier);
			}
		}
		base.Hurt(info);
		if (EACServer.playerTracker != null && info.Initiator != null && info.Initiator is BasePlayer)
		{
			BasePlayer player = info.Initiator.ToPlayer();
			if (this.net.connection != null && player.net.connection != null)
			{
				EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(this.net.connection);
				EasyAntiCheat.Server.Hydra.Client client1 = EACServer.GetClient(player.net.connection);
				PlayerTakeDamage vector3 = new PlayerTakeDamage()
				{
					DamageTaken = (int)info.damageTypes.Total(),
					HitBoneID = (int)info.HitBone,
					WeaponID = 0,
					DamageFlags = (info.isHeadshot ? PlayerTakeDamageFlags.PlayerTakeDamageCriticalHit : PlayerTakeDamageFlags.PlayerTakeDamageNormalHit)
				};
				if (info.Weapon != null)
				{
					Item item = info.Weapon.GetItem();
					if (item != null)
					{
						vector3.WeaponID = item.info.itemid;
					}
				}
				UnityEngine.Vector3 vector31 = player.eyes.position;
				UnityEngine.Quaternion quaternion = player.eyes.rotation;
				UnityEngine.Vector3 vector32 = this.eyes.position;
				UnityEngine.Quaternion quaternion1 = this.eyes.rotation;
				vector3.AttackerPosition = new EasyAntiCheat.Server.Cerberus.Vector3(vector31.x, vector31.y, vector31.z);
				vector3.AttackerViewRotation = new EasyAntiCheat.Server.Cerberus.Quaternion(quaternion.x, quaternion.y, quaternion.z, quaternion.w);
				vector3.VictimPosition = new EasyAntiCheat.Server.Cerberus.Vector3(vector32.x, vector32.y, vector32.z);
				vector3.VictimViewRotation = new EasyAntiCheat.Server.Cerberus.Quaternion(quaternion1.x, quaternion1.y, quaternion1.z, quaternion1.w);
				EACServer.playerTracker.LogPlayerTakeDamage(client, client1, vector3);
			}
		}
		this.metabolism.SendChangesToClient();
		if (info.PointStart != UnityEngine.Vector3.zero)
		{
			base.ClientRPCPlayer<UnityEngine.Vector3, int>(null, this, "DirectionalDamage", info.PointStart, info.damageTypes.GetMajorityDamageType());
		}
	}

	public override void InitShared()
	{
		this.Belt = new PlayerBelt(this);
		this.cachedProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		this.baseProtection = ScriptableObject.CreateInstance<ProtectionProperties>();
		this.inventory = base.GetComponent<PlayerInventory>();
		this.blueprints = base.GetComponent<PlayerBlueprints>();
		this.metabolism = base.GetComponent<PlayerMetabolism>();
		this.eyes = base.GetComponent<PlayerEyes>();
		this.input = base.GetComponent<PlayerInput>();
		base.InitShared();
	}

	public bool InSafeZone()
	{
		return this.currentSafeLevel > 0f;
	}

	private void InventoryUpdate()
	{
		if (this.IsConnected && !this.IsDead())
		{
			this.inventory.ServerUpdate(0.1f);
		}
	}

	public bool IsAttacking()
	{
		HeldEntity heldEntity = this.GetHeldEntity();
		if (heldEntity == null)
		{
			return false;
		}
		AttackEntity attackEntity = heldEntity as AttackEntity;
		if (attackEntity == null)
		{
			return false;
		}
		return attackEntity.NextAttackTime - UnityEngine.Time.time > attackEntity.repeatDelay - 1f;
	}

	public bool IsBuildingAuthed()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		if (buildingPrivilege == null)
		{
			return false;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingAuthed(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, Bounds bounds)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(new OBB(position, rotation, bounds));
		if (buildingPrivilege == null)
		{
			return false;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingAuthed(OBB obb)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(obb);
		if (buildingPrivilege == null)
		{
			return false;
		}
		return buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingBlocked()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		if (buildingPrivilege == null)
		{
			return false;
		}
		return !buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingBlocked(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, Bounds bounds)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(new OBB(position, rotation, bounds));
		if (buildingPrivilege == null)
		{
			return false;
		}
		return !buildingPrivilege.IsAuthed(this);
	}

	public bool IsBuildingBlocked(OBB obb)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(obb);
		if (buildingPrivilege == null)
		{
			return false;
		}
		return !buildingPrivilege.IsAuthed(this);
	}

	public bool IsDucked()
	{
		if (this.modelState == null)
		{
			return false;
		}
		return this.modelState.ducked;
	}

	public bool IsEnsnared()
	{
		if (this.triggers == null)
		{
			return false;
		}
		for (int i = 0; i < this.triggers.Count; i++)
		{
			if (this.triggers[i] is TriggerEnsnare)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsHeadUnderwater()
	{
		return this.WaterFactor() > 0.75f;
	}

	public bool IsHoldingEntity<T>()
	{
		HeldEntity heldEntity = this.GetHeldEntity();
		if (heldEntity == null)
		{
			return false;
		}
		return heldEntity is T;
	}

	public bool IsHostileItem(Item item)
	{
		if (!item.info.isHoldable)
		{
			return false;
		}
		ItemModEntity component = item.info.GetComponent<ItemModEntity>();
		if (component == null)
		{
			return false;
		}
		GameObject gameObject = component.entityPrefab.Get();
		if (gameObject == null)
		{
			return false;
		}
		AttackEntity attackEntity = gameObject.GetComponent<AttackEntity>();
		if (attackEntity == null)
		{
			return false;
		}
		return attackEntity.hostile;
	}

	public bool IsImmortal()
	{
		if ((this.IsAdmin || this.IsDeveloper) && this.IsConnected && this.net.connection != null && this.net.connection.info.GetBool("global.god", false))
		{
			return true;
		}
		if (this.WoundingCausingImmportality())
		{
			return true;
		}
		return false;
	}

	public bool IsNearEnemyBase()
	{
		BuildingPrivlidge buildingPrivilege = this.GetBuildingPrivilege();
		if (buildingPrivilege == null)
		{
			return false;
		}
		if (buildingPrivilege.IsAuthed(this))
		{
			return false;
		}
		return buildingPrivilege.AnyAuthed();
	}

	public bool IsNearEnemyBase(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation, Bounds bounds)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(new OBB(position, rotation, bounds));
		if (buildingPrivilege == null)
		{
			return false;
		}
		if (buildingPrivilege.IsAuthed(this))
		{
			return false;
		}
		return buildingPrivilege.AnyAuthed();
	}

	public bool IsNearEnemyBase(OBB obb)
	{
		BuildingPrivlidge buildingPrivilege = base.GetBuildingPrivilege(obb);
		if (buildingPrivilege == null)
		{
			return false;
		}
		if (buildingPrivilege.IsAuthed(this))
		{
			return false;
		}
		return buildingPrivilege.AnyAuthed();
	}

	public bool IsNoob()
	{
		return !this.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash);
	}

	public bool IsOnGround()
	{
		return this.modelState.onground;
	}

	public bool IsRelaxed()
	{
		return this.HasPlayerFlag(BasePlayer.PlayerFlags.Relaxed);
	}

	public bool IsRunning()
	{
		if (this.modelState == null)
		{
			return false;
		}
		return this.modelState.sprinting;
	}

	public bool IsServerFalling()
	{
		return this.HasPlayerFlag(BasePlayer.PlayerFlags.ServerFall);
	}

	public bool IsSleeping()
	{
		return this.HasPlayerFlag(BasePlayer.PlayerFlags.Sleeping);
	}

	public bool IsSpectating()
	{
		return this.HasPlayerFlag(BasePlayer.PlayerFlags.Spectating);
	}

	public bool IsSwimming()
	{
		return this.WaterFactor() >= 0.65f;
	}

	public bool IsWounded()
	{
		return this.HasPlayerFlag(BasePlayer.PlayerFlags.Wounded);
	}

	public void Kick(string reason)
	{
		if (!this.IsConnected)
		{
			return;
		}
		Network.Net.sv.Kick(this.net.connection, reason);
		Interface.CallHook("OnPlayerKicked", this, reason);
	}

	private void LeaveVisibility(Group group)
	{
		ServerMgr.OnLeaveVisibility(this.net.connection, group);
		this.ClearEntityQueue(group);
	}

	public void LifeStoryEnd()
	{
		SingletonComponent<ServerMgr>.Instance.persistance.AddLifeStory(this.userID, this.lifeStory);
		this.previousLifeStory = this.lifeStory;
		this.lifeStory = null;
	}

	internal void LifeStoryLogDeath(HitInfo deathBlow, DamageType lastDamage)
	{
		if (this.lifeStory == null)
		{
			return;
		}
		this.lifeStory.timeDied = (uint)Epoch.Current;
		PlayerLifeStory.DeathInfo shortPrefabName = Facepunch.Pool.Get<PlayerLifeStory.DeathInfo>();
		shortPrefabName.lastDamageType = (int)lastDamage;
		if (deathBlow != null)
		{
			if (deathBlow.Initiator != null)
			{
				deathBlow.Initiator.AttackerInfo(shortPrefabName);
			}
			if (deathBlow.WeaponPrefab != null)
			{
				shortPrefabName.inflictorName = deathBlow.WeaponPrefab.ShortPrefabName;
			}
			if (deathBlow.HitBone <= 0)
			{
				shortPrefabName.hitBone = "";
			}
			else
			{
				shortPrefabName.hitBone = StringPool.Get(deathBlow.HitBone);
			}
		}
		else if (base.SecondsSinceAttacked <= 60f && this.lastAttacker != null)
		{
			this.lastAttacker.AttackerInfo(shortPrefabName);
		}
		this.lifeStory.deathInfo = shortPrefabName;
	}

	internal void LifeStoryStart()
	{
		Assert.IsTrue(this.lifeStory == null, "Stomping old lifeStory");
		this.lifeStory = new PlayerLifeStory()
		{
			ShouldPool = false,
			timeBorn = (uint)Epoch.Current
		};
	}

	internal void LifeStoryUpdate(float deltaTime)
	{
		if (this.lifeStory == null)
		{
			return;
		}
		this.lifeStory.secondsAlive += deltaTime;
		if (this.IsSleeping())
		{
			this.lifeStory.secondsSleeping += deltaTime;
		}
	}

	public void LightToggle()
	{
		this.lightsOn = !this.lightsOn;
		this.SetLightsOn(this.lightsOn);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.basePlayer != null)
		{
			ProtoBuf.BasePlayer basePlayer = info.msg.basePlayer;
			this.userID = basePlayer.userid;
			this.UserIDString = this.userID.ToString();
			if (basePlayer.name != null)
			{
				this._displayName = basePlayer.name;
				if (string.IsNullOrEmpty(this._displayName.Trim()))
				{
					this._displayName = "Blaster :D";
				}
			}
			this.playerFlags = (BasePlayer.PlayerFlags)basePlayer.playerFlags;
			this.currentTeam = basePlayer.currentTeam;
			if (basePlayer.metabolism != null)
			{
				this.metabolism.Load(basePlayer.metabolism);
			}
			if (basePlayer.inventory != null)
			{
				this.inventory.Load(basePlayer.inventory);
			}
			if (basePlayer.modelState != null)
			{
				if (this.modelState != null)
				{
					this.modelState.ResetToPool();
					this.modelState = null;
				}
				this.modelState = basePlayer.modelState;
				basePlayer.modelState = null;
			}
		}
		if (info.fromDisk)
		{
			this.lifeStory = info.msg.basePlayer.currentLife;
			if (this.lifeStory != null)
			{
				this.lifeStory.ShouldPool = false;
			}
			this.previousLifeStory = info.msg.basePlayer.previousLife;
			if (this.previousLifeStory != null)
			{
				this.previousLifeStory.ShouldPool = false;
			}
			this.SetPlayerFlag(BasePlayer.PlayerFlags.Sleeping, false);
			this.StartSleeping();
			this.SetPlayerFlag(BasePlayer.PlayerFlags.Connected, false);
			if (this.lifeStory == null && this.IsAlive())
			{
				this.LifeStoryStart();
			}
		}
	}

	public override void MarkHostileFor(float duration = 60f)
	{
		base.MarkHostileFor(duration);
		float single = this.unHostileTime - UnityEngine.Time.realtimeSinceStartup;
		base.ClientRPCPlayer<float>(null, this, "SetHostileLength", single);
	}

	public void MarkSuicide()
	{
		this.nextSuicideTime = UnityEngine.Time.realtimeSinceStartup + 60f;
	}

	public void MarkSwapSeat()
	{
		this.nextSeatSwapTime = UnityEngine.Time.time + 0.75f;
	}

	public void MarkWeaponDrawnDuration(float newDuration)
	{
		float single = this.weaponDrawnDuration;
		this.weaponDrawnDuration = newDuration;
		if ((float)Mathf.FloorToInt(newDuration) != single)
		{
			base.ClientRPCPlayer<float>(null, this, "SetWeaponDrawnDuration", this.weaponDrawnDuration);
		}
	}

	public float MaxDeployDistance(Item item)
	{
		return 8f;
	}

	public override float MaxHealth()
	{
		return this._maxHealth;
	}

	public override float MaxVelocity()
	{
		if (this.IsSleeping())
		{
			return 0f;
		}
		if (!this.isMounted)
		{
			return this.GetMaxSpeed();
		}
		return this.GetMounted().MaxVelocity();
	}

	public void MountObject(BaseMountable mount, int desiredSeat = 0)
	{
		this.mounted.Set(mount);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void MovePosition(UnityEngine.Vector3 newPos)
	{
		base.transform.position = newPos;
		this.tickInterpolator.Reset(newPos);
		base.NetworkPositionTick();
	}

	public void NoteFiredProjectile(int projectileid, UnityEngine.Vector3 startPos, UnityEngine.Vector3 startVel, AttackEntity attackEnt, ItemDefinition firedItemDef, Item pickupItem = null)
	{
		AttackEntity component;
		BaseProjectile baseProjectile = attackEnt as BaseProjectile;
		ItemModProjectile itemModProjectile = firedItemDef.GetComponent<ItemModProjectile>();
		Projectile projectile = itemModProjectile.projectileObject.Get().GetComponent<Projectile>();
		int projectileProtection = ConVar.AntiHack.projectile_protection;
		if (base.HasParent() || this.isMounted)
		{
			projectileProtection = Mathf.Min(projectileProtection, 3);
		}
		if (startPos.IsNaNOrInfinity() || startVel.IsNaNOrInfinity())
		{
			string str = projectile.name;
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Contains NaN (", str, ")"));
			this.stats.combat.Log(baseProjectile, "projectile_nan");
			return;
		}
		if (projectileProtection >= 1)
		{
			float projectileForgiveness = 1f + ConVar.AntiHack.projectile_forgiveness;
			float single = startVel.magnitude;
			float maxVelocity = itemModProjectile.GetMaxVelocity();
			BaseProjectile baseProjectile1 = attackEnt as BaseProjectile;
			if (baseProjectile1)
			{
				maxVelocity *= baseProjectile1.GetProjectileVelocityScale(true);
			}
			maxVelocity *= projectileForgiveness;
			if (single > maxVelocity)
			{
				string str1 = projectile.name;
				AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Velocity (", str1, " with ", single, " > ", maxVelocity, ")" }));
				this.stats.combat.Log(baseProjectile, "projectile_velocity");
				return;
			}
		}
		BasePlayer.FiredProjectile firedProjectile = new BasePlayer.FiredProjectile()
		{
			itemDef = firedItemDef,
			itemMod = itemModProjectile,
			projectilePrefab = projectile,
			firedTime = UnityEngine.Time.realtimeSinceStartup,
			travelTime = 0f,
			weaponSource = attackEnt
		};
		if (attackEnt == null)
		{
			component = null;
		}
		else
		{
			component = GameManager.server.FindPrefab(StringPool.Get(attackEnt.prefabID)).GetComponent<AttackEntity>();
		}
		firedProjectile.weaponPrefab = component;
		firedProjectile.projectileModifier = (baseProjectile == null ? Projectile.Modifier.Default : baseProjectile.GetProjectileModifier());
		firedProjectile.pickupItem = pickupItem;
		firedProjectile.integrity = 1f;
		firedProjectile.position = startPos;
		firedProjectile.velocity = startVel;
		firedProjectile.initialPosition = startPos;
		firedProjectile.initialVelocity = startVel;
		firedProjectile.protection = projectileProtection;
		this.firedProjectiles.Add(projectileid, firedProjectile);
	}

	public override void OnAttacked(HitInfo info)
	{
		Network.Connection connection;
		if (Interface.CallHook("IOnBasePlayerAttacked", this, info) != null)
		{
			return;
		}
		float single = base.health;
		if (base.isServer)
		{
			HitArea hitArea = info.boneArea;
			if (hitArea != (HitArea.Head | HitArea.Chest | HitArea.Stomach | HitArea.Arm | HitArea.Hand | HitArea.Leg | HitArea.Foot))
			{
				List<Item> list = Facepunch.Pool.GetList<Item>();
				list.AddRange(this.inventory.containerWear.itemList);
				for (int i = 0; i < list.Count; i++)
				{
					Item item = list[i];
					if (item != null)
					{
						ItemModWearable component = item.info.GetComponent<ItemModWearable>();
						if (!(component == null) && component.ProtectsArea(hitArea))
						{
							item.OnAttacked(info);
						}
					}
				}
				Facepunch.Pool.FreeList<Item>(ref list);
				this.inventory.ServerUpdate(0f);
			}
		}
		base.OnAttacked(info);
		if (base.isServer && base.isServer && info.hasDamage)
		{
			if (!info.damageTypes.Has(DamageType.Bleeding) && info.damageTypes.IsBleedCausing() && !this.IsWounded() && !this.IsImmortal())
			{
				this.metabolism.bleeding.Add(info.damageTypes.Total() * 0.2f);
			}
			if (this.isMounted)
			{
				this.GetMounted().MounteeTookDamage(this, info);
			}
			this.CheckDeathCondition(info);
			if (this.net != null && this.net.connection != null)
			{
				Effect effect = new Effect();
				effect.Init(Effect.Type.Generic, base.transform.position, base.transform.forward, null);
				effect.pooledString = "assets/bundled/prefabs/fx/takedamage_hit.prefab";
				EffectNetwork.Send(effect, this.net.connection);
			}
			string str = StringPool.Get(info.HitBone);
			bool flag = (UnityEngine.Vector3.Dot((info.PointEnd - info.PointStart).normalized, this.eyes.BodyForward()) > 0.4f ? true : false);
			if (info.isHeadshot)
			{
				if (!flag)
				{
					base.SignalBroadcast(BaseEntity.Signal.Flinch_Head, string.Empty, null);
				}
				else
				{
					base.SignalBroadcast(BaseEntity.Signal.Flinch_RearHead, string.Empty, null);
				}
				BasePlayer initiatorPlayer = info.InitiatorPlayer;
				UnityEngine.Vector3 vector3 = new UnityEngine.Vector3(0f, 2f, 0f);
				UnityEngine.Vector3 vector31 = UnityEngine.Vector3.zero;
				if (initiatorPlayer != null)
				{
					connection = initiatorPlayer.net.connection;
				}
				else
				{
					connection = null;
				}
				Effect.server.Run("assets/bundled/prefabs/fx/headshot.prefab", this, 0, vector3, vector31, connection, false);
				if (initiatorPlayer)
				{
					initiatorPlayer.stats.Add("headshot", 1, Stats.Steam);
				}
			}
			else if (flag)
			{
				base.SignalBroadcast(BaseEntity.Signal.Flinch_RearTorso, string.Empty, null);
			}
			else if (str == "spine" || str == "spine2")
			{
				base.SignalBroadcast(BaseEntity.Signal.Flinch_Stomach, string.Empty, null);
			}
			else
			{
				base.SignalBroadcast(BaseEntity.Signal.Flinch_Chest, string.Empty, null);
			}
		}
		if (this.stats != null)
		{
			if (this.IsWounded())
			{
				this.stats.combat.Log(info, single, base.health, "wounded");
				return;
			}
			if (this.IsDead())
			{
				this.stats.combat.Log(info, single, base.health, "killed");
				return;
			}
			this.stats.combat.Log(info, single, base.health, null);
		}
	}

	public virtual void OnDisconnected()
	{
		this.stats.Save();
		this.EndLooting();
		if (this.IsAlive() || this.IsSleeping())
		{
			this.StartSleeping();
		}
		else
		{
			this.TeamDeathCleanup();
			base.Invoke(new Action(this.KillMessage), 0f);
		}
		BasePlayer.activePlayerList.Remove(this);
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Connected, false);
		if (this.net != null)
		{
			this.net.OnDisconnected();
		}
		this.ResetAntiHack();
	}

	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		if (Interface.CallHook("OnPlayerHealthChange", this, oldvalue, newvalue) != null)
		{
			return;
		}
		base.OnHealthChanged(oldvalue, newvalue);
		this.metabolism.isDirty = true;
	}

	public override void OnInvalidPosition()
	{
		if (this.IsDead())
		{
			return;
		}
		this.Die(null);
	}

	public override void OnKilled(HitInfo info)
	{
		BasePlayer player;
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Unused2, false);
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Unused1, false);
		this.EnsureDismounted();
		this.EndSleeping();
		this.EndLooting();
		this.stats.Add("deaths", 1, Stats.All);
		this.UpdatePlayerCollider(false);
		this.UpdatePlayerRigidbody(false);
		this.StopWounded();
		this.inventory.crafting.CancelAll(true);
		if (EACServer.playerTracker != null && this.net.connection != null)
		{
			if (info == null || !(info.Initiator != null))
			{
				player = null;
			}
			else
			{
				player = info.Initiator.ToPlayer();
			}
			BasePlayer basePlayer = player;
			if (!(basePlayer != null) || basePlayer.net.connection == null)
			{
				using (TimeWarning timeWarning = TimeWarning.New("playerTracker.LogPlayerDespawn", 0.1f))
				{
					EasyAntiCheat.Server.Hydra.Client client = EACServer.GetClient(this.net.connection);
					EACServer.playerTracker.LogPlayerDespawn(client);
				}
			}
			else
			{
				using (timeWarning = TimeWarning.New("playerTracker.LogPlayerKill", 0.1f))
				{
					EasyAntiCheat.Server.Hydra.Client client1 = EACServer.GetClient(basePlayer.net.connection);
					EasyAntiCheat.Server.Hydra.Client client2 = EACServer.GetClient(this.net.connection);
					EACServer.playerTracker.LogPlayerKill(client2, client1);
				}
			}
		}
		BaseCorpse baseCorpse = this.CreateCorpse();
		if (baseCorpse != null && info != null)
		{
			Rigidbody component = baseCorpse.GetComponent<Rigidbody>();
			if (component != null)
			{
				UnityEngine.Vector3 vector3 = info.attackNormal + (UnityEngine.Vector3.up * 0.5f);
				component.AddForce(vector3.normalized * 1f, ForceMode.VelocityChange);
			}
		}
		this.inventory.Strip();
		if (this.lastDamage == DamageType.Fall)
		{
			this.stats.Add("death_fall", 1, Stats.Steam);
		}
		string str = "";
		string str1 = "";
		if (info == null)
		{
			str = string.Concat(new object[] { this.ToString(), " died (", this.lastDamage, ")" });
			str1 = string.Concat("You died: ", this.lastDamage.ToString());
		}
		else if (!info.Initiator)
		{
			if (this.lastDamage != DamageType.Fall)
			{
				string str2 = this.ToString();
				DamageType majorityDamageType = info.damageTypes.GetMajorityDamageType();
				str = string.Concat(str2, " was killed by ", majorityDamageType.ToString());
				majorityDamageType = info.damageTypes.GetMajorityDamageType();
				str1 = string.Concat("You died: ", majorityDamageType.ToString());
			}
			else
			{
				str = string.Concat(this.ToString(), " was killed by fall!");
				str1 = "You died: killed by fall!";
				Facepunch.Rust.Analytics.Death("fall");
			}
		}
		else if (info.Initiator == this)
		{
			str = string.Concat(this.ToString(), " was suicide by ", this.lastDamage);
			str1 = string.Concat("You died: suicide by ", this.lastDamage);
			if (this.lastDamage != DamageType.Suicide)
			{
				Facepunch.Rust.Analytics.Death("selfinflicted");
				this.stats.Add("death_selfinflicted", 1, Stats.Steam);
			}
			else
			{
				Facepunch.Rust.Analytics.Death("suicide");
				this.stats.Add("death_suicide", 1, Stats.All);
			}
		}
		else if (!(info.Initiator is BasePlayer))
		{
			str = string.Concat(new string[] { this.ToString(), " was killed by ", info.Initiator.ShortPrefabName, " (", info.Initiator.Categorize(), ")" });
			str1 = string.Concat("You died: killed by ", info.Initiator.Categorize());
			this.stats.Add(string.Concat("death_", info.Initiator.Categorize()), 1, Stats.Steam);
			Facepunch.Rust.Analytics.Death(info.Initiator.Categorize());
		}
		else
		{
			BasePlayer player1 = info.Initiator.ToPlayer();
			str = string.Concat(this.ToString(), " was killed by ", player1.ToString());
			str1 = string.Concat(new object[] { "You died: killed by ", player1.displayName, " (", player1.userID, ")" });
			player1.stats.Add("kill_player", 1, Stats.All);
			if (info.WeaponPrefab == null)
			{
				Facepunch.Rust.Analytics.Death("player");
			}
			else
			{
				Facepunch.Rust.Analytics.Death(info.WeaponPrefab.ShortPrefabName);
			}
		}
		using (timeWarning = TimeWarning.New("LogMessage", 0.1f))
		{
			DebugEx.Log(str, StackTraceLogType.None);
			this.ConsoleMessage(str1);
		}
		base.SendNetworkUpdateImmediate(false);
		this.LifeStoryLogDeath(info, this.lastDamage);
		this.LifeStoryEnd();
		if (this.net.connection != null)
		{
			this.SendRespawnOptions();
			this.SendDeathInformation();
			this.stats.Save();
			return;
		}
		this.TeamDeathCleanup();
		base.Invoke(new Action(this.KillMessage), 0f);
	}

	public bool OnLadder()
	{
		if (!this.modelState.onLadder)
		{
			return false;
		}
		return base.FindTrigger<TriggerLadder>();
	}

	public override void OnNetworkGroupEnter(Group group)
	{
		base.OnNetworkGroupEnter(group);
		this.EnterVisibility(group);
	}

	public override void OnNetworkGroupLeave(Group group)
	{
		base.OnNetworkGroupLeave(group);
		this.LeaveVisibility(group);
	}

	public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		if (oldParent != null)
		{
			this.TransformState(oldParent.transform.localToWorldMatrix);
		}
		if (newParent != null)
		{
			this.TransformState(newParent.transform.worldToLocalMatrix);
		}
	}

	internal override void OnParentRemoved()
	{
		if (this.IsNpc)
		{
			base.OnParentRemoved();
			return;
		}
		base.SetParent(null, true, true);
	}

	private void OnPhysicsNeighbourChanged()
	{
		if (this.IsSleeping() || this.IsWounded())
		{
			base.Invoke(new Action(this.DelayedServerFall), 0.05f);
		}
	}

	[FromOwner]
	[RPC_Server]
	private void OnPlayerLanded(BaseEntity.RPCMessage msg)
	{
		float single = msg.read.Float();
		if (float.IsNaN(single) || float.IsInfinity(single))
		{
			return;
		}
		this.ApplyFallDamageFromVelocity(single);
		this.fallVelocity = 0f;
	}

	[FromOwner]
	[RPC_Server]
	public void OnProjectileAttack(BaseEntity.RPCMessage msg)
	{
		BasePlayer.FiredProjectile hitPositionWorld;
		PlayerProjectileAttack playerProjectileAttack = PlayerProjectileAttack.Deserialize(msg.read);
		if (playerProjectileAttack == null)
		{
			return;
		}
		PlayerAttack playerAttack = playerProjectileAttack.playerAttack;
		HitInfo hitInfo = new HitInfo();
		hitInfo.LoadFromAttack(playerAttack.attack, true);
		hitInfo.Initiator = this;
		hitInfo.ProjectileID = playerAttack.projectileID;
		hitInfo.ProjectileDistance = playerProjectileAttack.hitDistance;
		hitInfo.ProjectileVelocity = playerProjectileAttack.hitVelocity;
		hitInfo.Predicted = msg.connection;
		if (hitInfo.IsNaNOrInfinity() || float.IsNaN(playerProjectileAttack.travelTime) || float.IsInfinity(playerProjectileAttack.travelTime))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Contains NaN (", playerAttack.projectileID, ")"));
			playerProjectileAttack.ResetToPool();
			playerProjectileAttack = null;
			this.stats.combat.Log(hitInfo, "projectile_nan");
			return;
		}
		if (!this.firedProjectiles.TryGetValue(playerAttack.projectileID, out hitPositionWorld))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Missing ID (", playerAttack.projectileID, ")"));
			playerProjectileAttack.ResetToPool();
			playerProjectileAttack = null;
			this.stats.combat.Log(hitInfo, "projectile_invalid");
			return;
		}
		if (hitPositionWorld.integrity <= 0f)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Integrity is zero (", playerAttack.projectileID, ")"));
			playerProjectileAttack.ResetToPool();
			playerProjectileAttack = null;
			this.stats.combat.Log(hitInfo, "projectile_integrity");
			return;
		}
		if (hitPositionWorld.firedTime < UnityEngine.Time.realtimeSinceStartup - 8f)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Lifetime is zero (", playerAttack.projectileID, ")"));
			playerProjectileAttack.ResetToPool();
			playerProjectileAttack = null;
			this.stats.combat.Log(hitInfo, "projectile_lifetime");
			return;
		}
		hitInfo.Weapon = hitPositionWorld.weaponSource;
		hitInfo.WeaponPrefab = hitPositionWorld.weaponPrefab;
		hitInfo.ProjectilePrefab = hitPositionWorld.projectilePrefab;
		hitInfo.damageProperties = hitPositionWorld.projectilePrefab.damageProperties;
		float single = 0.03125f;
		UnityEngine.Vector3 vector3 = hitPositionWorld.position;
		UnityEngine.Vector3 vector31 = hitPositionWorld.velocity;
		float single1 = hitPositionWorld.partialTime;
		float single2 = Mathf.Clamp(playerProjectileAttack.travelTime - hitPositionWorld.travelTime, 0f, 8f);
		UnityEngine.Vector3 vector32 = UnityEngine.Physics.gravity * hitPositionWorld.projectilePrefab.gravityModifier;
		float single3 = hitPositionWorld.projectilePrefab.drag;
		if (hitPositionWorld.protection > 0)
		{
			bool flag = true;
			float projectileForgiveness = 1f + ConVar.AntiHack.projectile_forgiveness;
			float projectileClientframes = ConVar.AntiHack.projectile_clientframes;
			float projectileServerframes = ConVar.AntiHack.projectile_serverframes;
			float single4 = Mathx.Decrement(hitPositionWorld.firedTime);
			float single5 = Mathx.Increment(UnityEngine.Time.realtimeSinceStartup) - single4;
			float single6 = projectileClientframes / 60f;
			float single7 = projectileServerframes * Mathx.Max(UnityEngine.Time.deltaTime, UnityEngine.Time.smoothDeltaTime, UnityEngine.Time.fixedDeltaTime);
			float single8 = (this.desyncTime + single5 + single6 + single7) * projectileForgiveness;
			if (hitPositionWorld.protection >= 2 && hitInfo.HitEntity != null)
			{
				float single9 = hitInfo.HitEntity.MaxVelocity() + hitInfo.HitEntity.GetParentVelocity().magnitude;
				float single10 = hitInfo.HitEntity.BoundsPadding() + single8 * single9;
				float single11 = hitInfo.HitEntity.Distance(hitInfo.HitPositionWorld);
				if (single11 > single10)
				{
					string projectilePrefab = hitInfo.ProjectilePrefab.name;
					string str = (hitInfo.HitEntity ? hitInfo.HitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Entity too far away (", projectilePrefab, " on ", str, " with ", single11, "m > ", single10, "m in ", single8, "s)" }));
					this.stats.combat.Log(hitInfo, "projectile_distance");
					flag = false;
				}
			}
			if (hitPositionWorld.protection >= 1)
			{
				float single12 = hitPositionWorld.initialVelocity.magnitude;
				float projectilePrefab1 = hitInfo.ProjectilePrefab.initialDistance + single8 * single12;
				float single13 = UnityEngine.Vector3.Distance(hitPositionWorld.initialPosition, hitInfo.HitPositionWorld);
				if (single13 > projectilePrefab1)
				{
					string str1 = hitInfo.ProjectilePrefab.name;
					string str2 = (hitInfo.HitEntity ? hitInfo.HitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Traveled too fast (", str1, " on ", str2, " with ", single13, "m > ", projectilePrefab1, "m in ", single8, "s)" }));
					this.stats.combat.Log(hitInfo, "projectile_speed");
					flag = false;
				}
			}
			if (hitPositionWorld.protection >= 3)
			{
				UnityEngine.Vector3 vector33 = hitPositionWorld.position + (hitPositionWorld.velocity.normalized * 0.001f);
				UnityEngine.Vector3 pointStart = hitInfo.PointStart;
				UnityEngine.Vector3 hitPositionWorld1 = hitInfo.HitPositionWorld + (hitInfo.HitNormalWorld.normalized * 0.001f);
				UnityEngine.Vector3 vector34 = hitInfo.PositionOnRay(hitPositionWorld1);
				bool flag1 = GamePhysics.LineOfSight(vector33, pointStart, vector34, hitPositionWorld1, 2162688, 0f);
				if (flag1)
				{
					this.stats.Add(string.Concat("hit_", (hitInfo.HitEntity ? hitInfo.HitEntity.Categorize() : "world"), "_direct_los"), 1, Stats.Server);
				}
				else
				{
					this.stats.Add(string.Concat("hit_", (hitInfo.HitEntity ? hitInfo.HitEntity.Categorize() : "world"), "_indirect_los"), 1, Stats.Server);
				}
				if (!flag1)
				{
					string projectilePrefab2 = hitInfo.ProjectilePrefab.name;
					string str3 = (hitInfo.HitEntity ? hitInfo.HitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Line of sight (", projectilePrefab2, " on ", str3, ") ", vector33, " ", pointStart, " ", vector34, " ", hitPositionWorld1 }));
					this.stats.combat.Log(hitInfo, "projectile_los");
					flag = false;
				}
				BasePlayer hitEntity = hitInfo.HitEntity as BasePlayer;
				if (hitEntity)
				{
					UnityEngine.Vector3 hitPositionWorld2 = hitInfo.HitPositionWorld + (hitInfo.HitNormalWorld.normalized * 0.001f);
					UnityEngine.Vector3 vector35 = hitEntity.eyes.position;
					UnityEngine.Vector3 vector36 = hitEntity.CenterPoint();
					if ((GamePhysics.LineOfSight(hitPositionWorld2, vector35, 2162688, 0f) ? false : !GamePhysics.LineOfSight(hitPositionWorld2, vector36, 2162688, 0f)))
					{
						string projectilePrefab3 = hitInfo.ProjectilePrefab.name;
						string str4 = (hitInfo.HitEntity ? hitInfo.HitEntity.ShortPrefabName : "world");
						AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Line of sight (", projectilePrefab3, " on ", str4, ") ", hitPositionWorld2, " ", vector35, " or ", hitPositionWorld2, " ", vector36 }));
						this.stats.combat.Log(hitInfo, "projectile_los");
						flag = false;
					}
				}
			}
			if (hitPositionWorld.protection >= 4)
			{
				this.SimulateProjectile(ref vector3, ref vector31, ref single1, single2, single, vector32, single3);
				UnityEngine.Vector3 hitPositionWorld3 = hitInfo.HitPositionWorld - vector3;
				float single14 = hitPositionWorld3.Magnitude2D();
				float single15 = Mathf.Abs(hitPositionWorld3.y);
				if (single14 > ConVar.AntiHack.projectile_trajectory_horizontal)
				{
					string str5 = hitPositionWorld.projectilePrefab.name;
					string str6 = (hitInfo.HitEntity ? hitInfo.HitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Horizontal trajectory (", str5, " on ", str6, " with ", single14, "m > ", ConVar.AntiHack.projectile_trajectory_horizontal, "m)" }));
					this.stats.combat.Log(hitInfo, "horizontal_trajectory");
					flag = false;
				}
				if (single15 > ConVar.AntiHack.projectile_trajectory_vertical)
				{
					string str7 = hitPositionWorld.projectilePrefab.name;
					string str8 = (hitInfo.HitEntity ? hitInfo.HitEntity.ShortPrefabName : "world");
					AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Vertical trajectory (", str7, " on ", str8, " with ", single15, "m > ", ConVar.AntiHack.projectile_trajectory_vertical, "m)" }));
					this.stats.combat.Log(hitInfo, "vertical_trajectory");
					flag = false;
				}
			}
			if (!flag)
			{
				AntiHack.AddViolation(this, AntiHackType.ProjectileHack, ConVar.AntiHack.projectile_penalty);
				playerProjectileAttack.ResetToPool();
				playerProjectileAttack = null;
				return;
			}
		}
		hitPositionWorld.position = hitInfo.HitPositionWorld;
		hitPositionWorld.velocity = playerProjectileAttack.hitVelocity;
		hitPositionWorld.travelTime = playerProjectileAttack.travelTime;
		hitPositionWorld.partialTime = single1;
		hitInfo.ProjectilePrefab.CalculateDamage(hitInfo, hitPositionWorld.projectileModifier, hitPositionWorld.integrity);
		if (hitInfo.HitEntity == null && hitInfo.HitMaterial == Projectile.WaterMaterialID())
		{
			hitPositionWorld.integrity = Mathf.Clamp01(hitPositionWorld.integrity - 0.1f);
		}
		else if (hitInfo.ProjectilePrefab.penetrationPower <= 0f || hitInfo.HitEntity == null)
		{
			hitPositionWorld.integrity = 0f;
		}
		else
		{
			float single16 = hitInfo.HitEntity.PenetrationResistance(hitInfo) / hitInfo.ProjectilePrefab.penetrationPower;
			hitPositionWorld.integrity = Mathf.Clamp01(hitPositionWorld.integrity - single16);
		}
		hitPositionWorld.itemMod.ServerProjectileHit(hitInfo);
		if (hitInfo.HitEntity)
		{
			this.stats.Add(string.Concat(hitPositionWorld.itemMod.category, "_hit_", hitInfo.HitEntity.Categorize()), 1, Stats.Steam);
		}
		if (hitPositionWorld.integrity <= 0f && hitInfo.ProjectilePrefab.remainInWorld)
		{
			this.CreateWorldProjectile(hitInfo, hitPositionWorld.itemDef, hitPositionWorld.itemMod, hitInfo.ProjectilePrefab, hitPositionWorld.pickupItem);
		}
		if (Interface.CallHook("OnPlayerAttack", this, hitInfo) != null)
		{
			return;
		}
		this.firedProjectiles[playerAttack.projectileID] = hitPositionWorld;
		if (hitInfo.HitEntity)
		{
			hitInfo.HitEntity.OnAttacked(hitInfo);
		}
		Effect.server.ImpactEffect(hitInfo);
		playerProjectileAttack.ResetToPool();
		playerProjectileAttack = null;
	}

	[FromOwner]
	[RPC_Server]
	public void OnProjectileRicochet(BaseEntity.RPCMessage msg)
	{
		BasePlayer.FiredProjectile firedProjectile;
		PlayerProjectileRicochet playerProjectileRicochet = PlayerProjectileRicochet.Deserialize(msg.read);
		if (playerProjectileRicochet == null)
		{
			return;
		}
		if (playerProjectileRicochet.hitPosition.IsNaNOrInfinity() || playerProjectileRicochet.inVelocity.IsNaNOrInfinity() || playerProjectileRicochet.outVelocity.IsNaNOrInfinity() || playerProjectileRicochet.hitNormal.IsNaNOrInfinity() || float.IsNaN(playerProjectileRicochet.travelTime) || float.IsInfinity(playerProjectileRicochet.travelTime))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Contains NaN (", playerProjectileRicochet.projectileID, ")"));
			playerProjectileRicochet.ResetToPool();
			playerProjectileRicochet = null;
			return;
		}
		if (!this.firedProjectiles.TryGetValue(playerProjectileRicochet.projectileID, out firedProjectile))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Missing ID (", playerProjectileRicochet.projectileID, ")"));
			playerProjectileRicochet.ResetToPool();
			playerProjectileRicochet = null;
			return;
		}
		if (firedProjectile.firedTime < UnityEngine.Time.realtimeSinceStartup - 8f)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Lifetime is zero (", playerProjectileRicochet.projectileID, ")"));
			playerProjectileRicochet.ResetToPool();
			playerProjectileRicochet = null;
			return;
		}
		if (firedProjectile.protection >= 3)
		{
			UnityEngine.Vector3 vector3 = firedProjectile.position + (firedProjectile.velocity.normalized * 0.001f);
			UnityEngine.Vector3 vector31 = playerProjectileRicochet.hitPosition - (playerProjectileRicochet.inVelocity.normalized * 0.001f);
			if (!GamePhysics.LineOfSight(vector3, vector31, playerProjectileRicochet.hitPosition + (playerProjectileRicochet.outVelocity.normalized * 0.001f), 2162688, 0f))
			{
				string str = firedProjectile.projectilePrefab.name;
				AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Line of sight (", str, " on ricochet) ", vector3, " ", vector31 }));
				playerProjectileRicochet.ResetToPool();
				playerProjectileRicochet = null;
				return;
			}
		}
		float single = 0.03125f;
		UnityEngine.Vector3 vector32 = firedProjectile.position;
		UnityEngine.Vector3 vector33 = firedProjectile.velocity;
		float single1 = firedProjectile.partialTime;
		float single2 = Mathf.Clamp(playerProjectileRicochet.travelTime - firedProjectile.travelTime, 0f, 8f);
		UnityEngine.Vector3 vector34 = UnityEngine.Physics.gravity * firedProjectile.projectilePrefab.gravityModifier;
		float single3 = firedProjectile.projectilePrefab.drag;
		if (firedProjectile.protection >= 4)
		{
			this.SimulateProjectile(ref vector32, ref vector33, ref single1, single2, single, vector34, single3);
			UnityEngine.Vector3 vector35 = playerProjectileRicochet.hitPosition - vector32;
			float single4 = vector35.Magnitude2D();
			float single5 = Mathf.Abs(vector35.y);
			if (single4 > ConVar.AntiHack.projectile_trajectory_horizontal)
			{
				string str1 = firedProjectile.projectilePrefab.name;
				AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Horizontal trajectory (", str1, " on ricochet with ", single4, "m > ", ConVar.AntiHack.projectile_trajectory_horizontal, "m)" }));
				playerProjectileRicochet.ResetToPool();
				playerProjectileRicochet = null;
				return;
			}
			if (single5 > ConVar.AntiHack.projectile_trajectory_vertical)
			{
				string str2 = firedProjectile.projectilePrefab.name;
				AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Vertical trajectory (", str2, " on ricochet with ", single5, "m > ", ConVar.AntiHack.projectile_trajectory_vertical, "m)" }));
				playerProjectileRicochet.ResetToPool();
				playerProjectileRicochet = null;
				return;
			}
		}
		if (firedProjectile.protection >= 5)
		{
			UnityEngine.Vector3 vector36 = firedProjectile.position;
			UnityEngine.Vector3 vector37 = playerProjectileRicochet.hitPosition;
			if (!GamePhysics.CheckSphere(vector37, 0.01f, 1269916433, QueryTriggerInteraction.UseGlobal))
			{
				string str3 = firedProjectile.projectilePrefab.name;
				AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Collider (", str3, " on ricochet) ", vector36, " ", vector37 }));
				playerProjectileRicochet.ResetToPool();
				playerProjectileRicochet = null;
				return;
			}
		}
		if (Interface.CallHook("OnProjectileRicochet", this, playerProjectileRicochet) != null)
		{
			return;
		}
		firedProjectile.position = playerProjectileRicochet.hitPosition;
		firedProjectile.velocity = playerProjectileRicochet.outVelocity;
		firedProjectile.travelTime = playerProjectileRicochet.travelTime;
		firedProjectile.partialTime = single1;
		this.firedProjectiles[playerProjectileRicochet.projectileID] = firedProjectile;
		playerProjectileRicochet.ResetToPool();
		playerProjectileRicochet = null;
	}

	[FromOwner]
	[RPC_Server]
	public void OnProjectileUpdate(BaseEntity.RPCMessage msg)
	{
		BasePlayer.FiredProjectile firedProjectile;
		PlayerProjectileUpdate playerProjectileUpdate = PlayerProjectileUpdate.Deserialize(msg.read);
		if (playerProjectileUpdate == null)
		{
			return;
		}
		if (playerProjectileUpdate.curPosition.IsNaNOrInfinity() || playerProjectileUpdate.curVelocity.IsNaNOrInfinity() || float.IsNaN(playerProjectileUpdate.travelTime) || float.IsInfinity(playerProjectileUpdate.travelTime))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Contains NaN (", playerProjectileUpdate.projectileID, ")"));
			playerProjectileUpdate.ResetToPool();
			playerProjectileUpdate = null;
			return;
		}
		if (!this.firedProjectiles.TryGetValue(playerProjectileUpdate.projectileID, out firedProjectile))
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Missing ID (", playerProjectileUpdate.projectileID, ")"));
			playerProjectileUpdate.ResetToPool();
			playerProjectileUpdate = null;
			return;
		}
		if (firedProjectile.firedTime < UnityEngine.Time.realtimeSinceStartup - 8f)
		{
			AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat("Lifetime is zero (", playerProjectileUpdate.projectileID, ")"));
			playerProjectileUpdate.ResetToPool();
			playerProjectileUpdate = null;
			return;
		}
		if (firedProjectile.protection >= 3)
		{
			UnityEngine.Vector3 vector3 = firedProjectile.position + (firedProjectile.velocity.normalized * 0.001f);
			UnityEngine.Vector3 vector31 = playerProjectileUpdate.curPosition;
			if (!GamePhysics.LineOfSight(vector3, vector31, 2162688, 0f))
			{
				string str = firedProjectile.projectilePrefab.name;
				AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Line of sight (", str, " on update) ", vector3, " ", vector31 }));
				playerProjectileUpdate.ResetToPool();
				playerProjectileUpdate = null;
				return;
			}
		}
		float single = 0.03125f;
		UnityEngine.Vector3 vector32 = firedProjectile.position;
		UnityEngine.Vector3 vector33 = firedProjectile.velocity;
		float single1 = firedProjectile.partialTime;
		float single2 = Mathf.Clamp(playerProjectileUpdate.travelTime - firedProjectile.travelTime, 0f, 8f);
		UnityEngine.Vector3 vector34 = UnityEngine.Physics.gravity * firedProjectile.projectilePrefab.gravityModifier;
		float single3 = firedProjectile.projectilePrefab.drag;
		if (firedProjectile.protection >= 4)
		{
			this.SimulateProjectile(ref vector32, ref vector33, ref single1, single2, single, vector34, single3);
			UnityEngine.Vector3 vector35 = playerProjectileUpdate.curPosition - vector32;
			float single4 = vector35.Magnitude2D();
			float single5 = Mathf.Abs(vector35.y);
			if (single4 > ConVar.AntiHack.projectile_trajectory_horizontal)
			{
				string str1 = firedProjectile.projectilePrefab.name;
				AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Horizontal trajectory (", str1, " on update with ", single4, "m > ", ConVar.AntiHack.projectile_trajectory_horizontal, "m)" }));
				playerProjectileUpdate.ResetToPool();
				playerProjectileUpdate = null;
				return;
			}
			if (single5 > ConVar.AntiHack.projectile_trajectory_vertical)
			{
				string str2 = firedProjectile.projectilePrefab.name;
				AntiHack.Log(this, AntiHackType.ProjectileHack, string.Concat(new object[] { "Vertical trajectory (", str2, " on update with ", single5, "m > ", ConVar.AntiHack.projectile_trajectory_vertical, "m)" }));
				playerProjectileUpdate.ResetToPool();
				playerProjectileUpdate = null;
				return;
			}
		}
		if (firedProjectile.protection >= 5)
		{
			playerProjectileUpdate.curVelocity = vector33;
		}
		firedProjectile.position = playerProjectileUpdate.curPosition;
		firedProjectile.velocity = playerProjectileUpdate.curVelocity;
		firedProjectile.travelTime = playerProjectileUpdate.travelTime;
		firedProjectile.partialTime = single1;
		this.firedProjectiles[playerProjectileUpdate.projectileID] = firedProjectile;
		playerProjectileUpdate.ResetToPool();
		playerProjectileUpdate = null;
	}

	public void OnReceivedTick(Stream stream)
	{
		using (TimeWarning timeWarning = TimeWarning.New("OnReceiveTickFromStream", 0.1f))
		{
			PlayerTick playerTick = null;
			using (TimeWarning timeWarning1 = TimeWarning.New("lastReceivedTick = data.Copy", 0.1f))
			{
				playerTick = PlayerTick.Deserialize(stream, this.lastReceivedTick, true);
			}
			using (timeWarning1 = TimeWarning.New("lastReceivedTick = data.Copy", 0.1f))
			{
				this.lastReceivedTick = playerTick.Copy();
			}
			using (timeWarning1 = TimeWarning.New("OnReceiveTick", 0.1f))
			{
				this.OnReceiveTick(playerTick, this.wasStalled);
			}
			this.lastTickTime = UnityEngine.Time.time;
			playerTick.Dispose();
		}
	}

	public void OnReceivedVoice(byte[] data)
	{
		if (Interface.CallHook("OnPlayerVoice", this, data) != null)
		{
			return;
		}
		if (Network.Net.sv.write.Start())
		{
			Network.Net.sv.write.PacketID(Message.Type.VoiceData);
			Network.Net.sv.write.UInt32(this.net.ID);
			Network.Net.sv.write.BytesWithSize(data);
			Write write = Network.Net.sv.write;
			SendInfo sendInfo = new SendInfo(BaseNetworkable.GetConnectionsWithin(base.transform.position, 100f))
			{
				priority = Priority.Immediate
			};
			write.Send(sendInfo);
		}
	}

	private void OnReceiveTick(PlayerTick msg, bool wasPlayerStalled)
	{
		if (msg.inputState != null)
		{
			this.serverInput.Flip(msg.inputState);
		}
		if (Interface.CallHook("OnPlayerTick", this, msg, wasPlayerStalled) != null)
		{
			return;
		}
		if (this.serverInput.current.buttons != this.serverInput.previous.buttons)
		{
			this.lastInputTime = UnityEngine.Time.time;
		}
		if (Interface.CallHook("OnPlayerInput", this, this.serverInput) != null)
		{
			return;
		}
		if (this.IsReceivingSnapshot)
		{
			return;
		}
		if (this.IsSpectating())
		{
			using (TimeWarning timeWarning = TimeWarning.New("Tick_Spectator", 0.1f))
			{
				this.Tick_Spectator();
			}
			return;
		}
		if (this.IsDead())
		{
			return;
		}
		if (this.IsSleeping())
		{
			if (this.serverInput.WasJustPressed(BUTTON.FIRE_PRIMARY) || this.serverInput.WasJustPressed(BUTTON.FIRE_SECONDARY) || this.serverInput.WasJustPressed(BUTTON.JUMP) || this.serverInput.WasJustPressed(BUTTON.DUCK))
			{
				this.EndSleeping();
				base.SendNetworkUpdateImmediate(false);
			}
			this.UpdateActiveItem(0);
			return;
		}
		this.UpdateActiveItem(msg.activeItem);
		this.UpdateModelStateFromTick(msg);
		if (this.IsWounded())
		{
			return;
		}
		if (this.isMounted)
		{
			this.GetMounted().PlayerServerInput(this.serverInput, this);
		}
		this.UpdatePositionFromTick(msg, wasPlayerStalled);
		this.UpdateRotationFromTick(msg);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		BaseEntity.RPCMessage rPCMessage;
		using (TimeWarning timeWarning = TimeWarning.New("BasePlayer.OnRpcMessage", 0.1f))
		{
			if (rpc == 935768323 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ClientKeepConnectionAlive "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("ClientKeepConnectionAlive", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("ClientKeepConnectionAlive", this, player))
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
							this.ClientKeepConnectionAlive(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in ClientKeepConnectionAlive");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
			else if (rpc == -512148402 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - ClientLoadingComplete "));
				}
				using (timeWarning1 = TimeWarning.New("ClientLoadingComplete", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("ClientLoadingComplete", this, player))
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
							this.ClientLoadingComplete(rPCMessage);
						}
					}
					catch (Exception exception1)
					{
						player.Kick("RPC Error in ClientLoadingComplete");
						Debug.LogException(exception1);
					}
				}
				flag = true;
			}
			else if (rpc == 1998170713 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - OnPlayerLanded "));
				}
				using (timeWarning1 = TimeWarning.New("OnPlayerLanded", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("OnPlayerLanded", this, player))
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
							this.OnPlayerLanded(rPCMessage);
						}
					}
					catch (Exception exception2)
					{
						player.Kick("RPC Error in OnPlayerLanded");
						Debug.LogException(exception2);
					}
				}
				flag = true;
			}
			else if (rpc == 363681694 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - OnProjectileAttack "));
				}
				using (timeWarning1 = TimeWarning.New("OnProjectileAttack", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("OnProjectileAttack", this, player))
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
							this.OnProjectileAttack(rPCMessage);
						}
					}
					catch (Exception exception3)
					{
						player.Kick("RPC Error in OnProjectileAttack");
						Debug.LogException(exception3);
					}
				}
				flag = true;
			}
			else if (rpc == 1500391289 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - OnProjectileRicochet "));
				}
				using (timeWarning1 = TimeWarning.New("OnProjectileRicochet", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("OnProjectileRicochet", this, player))
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
							this.OnProjectileRicochet(rPCMessage);
						}
					}
					catch (Exception exception4)
					{
						player.Kick("RPC Error in OnProjectileRicochet");
						Debug.LogException(exception4);
					}
				}
				flag = true;
			}
			else if (rpc == -1970776803 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - OnProjectileUpdate "));
				}
				using (timeWarning1 = TimeWarning.New("OnProjectileUpdate", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test("OnProjectileUpdate", this, player))
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
							this.OnProjectileUpdate(rPCMessage);
						}
					}
					catch (Exception exception5)
					{
						player.Kick("RPC Error in OnProjectileUpdate");
						Debug.LogException(exception5);
					}
				}
				flag = true;
			}
			else if (rpc == -1127179278 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - PerformanceReport "));
				}
				using (timeWarning1 = TimeWarning.New("PerformanceReport", 0.1f))
				{
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
							this.PerformanceReport(rPCMessage);
						}
					}
					catch (Exception exception6)
					{
						player.Kick("RPC Error in PerformanceReport");
						Debug.LogException(exception6);
					}
				}
				flag = true;
			}
			else if (rpc == 970468557 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_Assist "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_Assist", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_Assist", this, player, 3f))
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
							this.RPC_Assist(rPCMessage);
						}
					}
					catch (Exception exception7)
					{
						player.Kick("RPC Error in RPC_Assist");
						Debug.LogException(exception7);
					}
				}
				flag = true;
			}
			else if (rpc == -1031728755 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_KeepAlive "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_KeepAlive", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_KeepAlive", this, player, 3f))
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
							this.RPC_KeepAlive(rPCMessage);
						}
					}
					catch (Exception exception8)
					{
						player.Kick("RPC Error in RPC_KeepAlive");
						Debug.LogException(exception8);
					}
				}
				flag = true;
			}
			else if (rpc == -602572228 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_LootPlayer "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_LootPlayer", 0.1f))
				{
					using (timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.IsVisible.Test("RPC_LootPlayer", this, player, 3f))
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
							this.RPC_LootPlayer(rPCMessage);
						}
					}
					catch (Exception exception9)
					{
						player.Kick("RPC Error in RPC_LootPlayer");
						Debug.LogException(exception9);
					}
				}
				flag = true;
			}
			else if (rpc == 1539133504 && player != null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_StartClimb "));
				}
				using (timeWarning1 = TimeWarning.New("RPC_StartClimb", 0.1f))
				{
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
							this.RPC_StartClimb(rPCMessage);
						}
					}
					catch (Exception exception10)
					{
						player.Kick("RPC Error in RPC_StartClimb");
						Debug.LogException(exception10);
					}
				}
				flag = true;
			}
			else if (rpc != 970114602 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - SV_Drink "));
				}
				using (timeWarning1 = TimeWarning.New("SV_Drink", 0.1f))
				{
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
							this.SV_Drink(rPCMessage);
						}
					}
					catch (Exception exception11)
					{
						player.Kick("RPC Error in SV_Drink");
						Debug.LogException(exception11);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if (baseEntity.InSafeZone() && baseEntity.userID != this.userID)
		{
			return false;
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	public void OverrideViewAngles(UnityEngine.Vector3 newAng)
	{
		this.viewAngles = newAng;
	}

	public void PauseFlyHackDetection(float seconds = 1f)
	{
		this.flyhackPauseTime = Mathf.Max(this.flyhackPauseTime, seconds);
	}

	public void PauseSpeedHackDetection(float seconds = 1f)
	{
		this.speedhackPauseTime = Mathf.Max(this.speedhackPauseTime, seconds);
	}

	public void PauseVehicleNoClipDetection(float seconds = 1f)
	{
		this.vehiclePauseTime = Mathf.Max(this.vehiclePauseTime, seconds);
	}

	public override float PenetrationResistance(HitInfo info)
	{
		return 100f;
	}

	[RPC_Server]
	public void PerformanceReport(BaseEntity.RPCMessage msg)
	{
		int num = msg.read.Int32();
		int num1 = msg.read.Int32();
		float single = msg.read.Float();
		int num2 = msg.read.Int32();
		Debug.LogFormat("{0}{1}{2}{3}{4}", new object[] { string.Concat(num, "MB").PadRight(9), string.Concat(num1, "MB").PadRight(9), string.Concat(single.ToString("0"), "FPS").PadRight(8), ((long)num2).FormatSeconds().PadRight(9), this.displayName });
	}

	public void PlayerInit(Network.Connection c)
	{
		using (TimeWarning timeWarning = TimeWarning.New("PlayerInit", (long)10))
		{
			base.CancelInvoke(new Action(this.KillMessage));
			this.SetPlayerFlag(BasePlayer.PlayerFlags.Connected, true);
			BasePlayer.activePlayerList.Add(this);
			this.userID = c.userid;
			this.UserIDString = this.userID.ToString();
			this._displayName = c.username.ToPrintable(32);
			c.player = this;
			this.tickInterpolator.Reset(base.transform.position);
			this.lastTickTime = 0f;
			this.lastInputTime = 0f;
			this.SetPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot, true);
			this.stats.Init();
			base.InvokeRandomized(new Action(this.StatSave), UnityEngine.Random.Range(5f, 10f), 30f, UnityEngine.Random.Range(0f, 6f));
			this.previousLifeStory = SingletonComponent<ServerMgr>.Instance.persistance.GetLastLifeStory(this.userID);
			this.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, c.authLevel != 0);
			this.SetPlayerFlag(BasePlayer.PlayerFlags.IsDeveloper, DeveloperList.IsDeveloper(this));
			if (this.IsDead() && this.net.SwitchGroup(BaseNetworkable.LimboNetworkGroup))
			{
				base.SendNetworkGroupChange();
			}
			this.net.OnConnected(c);
			this.net.StartSubscriber();
			base.SendAsSnapshot(this.net.connection, false);
			Interface.CallHook("OnPlayerInit", this);
			base.ClientRPCPlayer(null, this, "StartLoading");
			if (this.net != null)
			{
				EACServer.OnStartLoading(this.net.connection);
			}
			if (this.IsAdmin)
			{
				if (ConVar.AntiHack.noclip_protection <= 0)
				{
					this.ChatMessage("antihack.noclip_protection is disabled!");
				}
				if (ConVar.AntiHack.speedhack_protection <= 0)
				{
					this.ChatMessage("antihack.speedhack_protection is disabled!");
				}
				if (ConVar.AntiHack.flyhack_protection <= 0)
				{
					this.ChatMessage("antihack.flyhack_protection is disabled!");
				}
				if (ConVar.AntiHack.projectile_protection <= 0)
				{
					this.ChatMessage("antihack.projectile_protection is disabled!");
				}
				if (ConVar.AntiHack.melee_protection <= 0)
				{
					this.ChatMessage("antihack.melee_protection is disabled!");
				}
				if (ConVar.AntiHack.eye_protection <= 0)
				{
					this.ChatMessage("antihack.eye_protection is disabled!");
				}
			}
		}
	}

	public void ProlongWounding(float delay)
	{
		this.woundedDuration = Mathf.Max(this.woundedDuration, Mathf.Min(this.secondsSinceWoundedStarted + delay, this.woundedDuration + delay));
	}

	public void QueueUpdate(BasePlayer.NetworkQueue queue, BaseNetworkable ent)
	{
		if (!this.IsConnected)
		{
			return;
		}
		if (queue == BasePlayer.NetworkQueue.Update)
		{
			this.networkQueue[0].Add(ent);
			return;
		}
		if (queue != BasePlayer.NetworkQueue.UpdateDistance)
		{
			return;
		}
		if (this.IsReceivingSnapshot)
		{
			return;
		}
		if (this.networkQueue[1].Contains(ent))
		{
			return;
		}
		if (this.networkQueue[0].Contains(ent))
		{
			return;
		}
		BasePlayer.NetworkQueueList networkQueueList = this.networkQueue[1];
		if (base.Distance(ent as BaseEntity) >= 20f)
		{
			networkQueueList.Add(ent);
			return;
		}
		this.QueueUpdate(BasePlayer.NetworkQueue.Update, ent);
	}

	public override float RadiationExposureFraction()
	{
		float single = Mathf.Clamp(this.baseProtection.amounts[17], 0f, 1f);
		return 1f - single;
	}

	public override float RadiationProtection()
	{
		return this.baseProtection.amounts[17] * 100f;
	}

	public void ResetAntiHack()
	{
		this.violationLevel = 0f;
		this.lastViolationTime = 0f;
		this.speedhackPauseTime = 0f;
		this.speedhackDistance = 0f;
		this.flyhackPauseTime = 0f;
		this.flyhackDistanceVertical = 0f;
		this.flyhackDistanceHorizontal = 0f;
	}

	public void Respawn()
	{
		BasePlayer.SpawnPoint spawnPoint = ServerMgr.FindSpawnPoint();
		object obj = Interface.CallHook("OnPlayerRespawn", this);
		if (obj is BasePlayer.SpawnPoint)
		{
			spawnPoint = (BasePlayer.SpawnPoint)obj;
		}
		this.RespawnAt(spawnPoint.pos, spawnPoint.rot);
	}

	public void RespawnAt(UnityEngine.Vector3 position, UnityEngine.Quaternion rotation)
	{
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Wounded, false);
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Unused2, false);
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Unused1, false);
		this.SetPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot, true);
		this.SetPlayerFlag(BasePlayer.PlayerFlags.DisplaySash, false);
		ServerPerformance.spawns += (long)1;
		base.SetParent(null, true, false);
		base.transform.position = position;
		base.transform.rotation = rotation;
		this.tickInterpolator.Reset(position);
		this.lastTickTime = 0f;
		this.StopWounded();
		this.StopSpectating();
		this.UpdateNetworkGroup();
		this.UpdatePlayerCollider(true);
		this.UpdatePlayerRigidbody(false);
		this.StartSleeping();
		this.LifeStoryStart();
		this.metabolism.Reset();
		this.InitializeHealth(this.StartHealth(), this.StartMaxHealth());
		this.inventory.GiveDefaultItems();
		base.SendNetworkUpdateImmediate(false);
		base.ClientRPCPlayer(null, this, "StartLoading");
		if (this.net != null)
		{
			EACServer.OnStartLoading(this.net.connection);
		}
		Interface.CallHook("OnPlayerRespawned", this);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_Assist(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (msg.player == this)
		{
			return;
		}
		if (!this.IsWounded())
		{
			return;
		}
		this.StopWounded();
		msg.player.stats.Add("wounded_assisted", 1, Stats.Steam);
		this.stats.Add("wounded_healed", 1, Stats.Steam);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_KeepAlive(BaseEntity.RPCMessage msg)
	{
		if (!msg.player.CanInteract())
		{
			return;
		}
		if (msg.player == this)
		{
			return;
		}
		if (!this.IsWounded())
		{
			return;
		}
		this.ProlongWounding(10f);
	}

	[IsVisible(3f)]
	[RPC_Server]
	public void RPC_LootPlayer(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		if (!basePlayer || !basePlayer.CanInteract())
		{
			return;
		}
		if (!this.CanBeLooted(basePlayer))
		{
			return;
		}
		if (basePlayer.inventory.loot.StartLootingEntity(this, true))
		{
			basePlayer.inventory.loot.AddContainer(this.inventory.containerMain);
			basePlayer.inventory.loot.AddContainer(this.inventory.containerWear);
			basePlayer.inventory.loot.AddContainer(this.inventory.containerBelt);
			basePlayer.inventory.loot.SendImmediate();
			basePlayer.ClientRPCPlayer<string>(null, basePlayer, "RPC_OpenLootPanel", "player_corpse");
		}
	}

	[RPC_Server]
	public void RPC_StartClimb(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		bool flag = msg.read.Bit();
		UnityEngine.Vector3 vector3 = msg.read.Vector3();
		uint num = msg.read.UInt32();
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(num);
		UnityEngine.Vector3 vector31 = (flag ? baseNetworkable.transform.TransformPoint(vector3) : vector3);
		if (!basePlayer.isMounted)
		{
			return;
		}
		if (!GamePhysics.LineOfSight(basePlayer.eyes.position, vector31, 1218519041, 0f))
		{
			return;
		}
		if (!GamePhysics.LineOfSight(vector31, vector31 + basePlayer.eyes.offset, 1218519041, 0f))
		{
			return;
		}
		if (AntiHack.TestNoClipping(basePlayer, vector31, vector31, true, 0f))
		{
			return;
		}
		basePlayer.EnsureDismounted();
		basePlayer.transform.position = vector31;
		Collider component = basePlayer.GetComponent<Collider>();
		component.enabled = false;
		component.enabled = true;
		basePlayer.ForceUpdateTriggers(true, true, true);
		if (!flag)
		{
			basePlayer.ClientRPCPlayer<UnityEngine.Vector3>(null, basePlayer, "ForcePositionTo", vector31);
			return;
		}
		basePlayer.ClientRPCPlayer<UnityEngine.Vector3, uint>(null, basePlayer, "ForcePositionToParentOffset", vector3, num);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		bool flag = (this.net == null ? false : this.net.connection == info.forConnection);
		info.msg.basePlayer = Facepunch.Pool.Get<ProtoBuf.BasePlayer>();
		info.msg.basePlayer.userid = this.userID;
		info.msg.basePlayer.name = this.displayName;
		info.msg.basePlayer.playerFlags = (int)this.playerFlags;
		info.msg.basePlayer.currentTeam = this.currentTeam;
		info.msg.basePlayer.heldEntity = this.svActiveItemID;
		if (!this.IsConnected || !this.IsAdmin && !this.IsDeveloper)
		{
			info.msg.basePlayer.skinCol = -1f;
			info.msg.basePlayer.skinTex = -1f;
			info.msg.basePlayer.skinMesh = -1f;
		}
		else
		{
			info.msg.basePlayer.skinCol = this.net.connection.info.GetFloat("global.skincol", -1f);
			info.msg.basePlayer.skinTex = this.net.connection.info.GetFloat("global.skintex", -1f);
			info.msg.basePlayer.skinMesh = this.net.connection.info.GetFloat("global.skinmesh", -1f);
		}
		if (info.forDisk | flag)
		{
			info.msg.basePlayer.metabolism = this.metabolism.Save();
		}
		if (!info.forDisk && !flag)
		{
			info.msg.basePlayer.playerFlags &= -5;
			info.msg.basePlayer.playerFlags &= -129;
		}
		info.msg.basePlayer.inventory = this.inventory.Save(info.forDisk | flag);
		this.modelState.sleeping = this.IsSleeping();
		this.modelState.relaxed = this.IsRelaxed();
		info.msg.basePlayer.modelState = this.modelState.Copy();
		if (!info.forDisk)
		{
			info.msg.basePlayer.mounted = this.mounted.uid;
		}
		if (flag)
		{
			info.msg.basePlayer.persistantData = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerInfo(this.userID);
		}
		if (info.forDisk)
		{
			info.msg.basePlayer.currentLife = this.lifeStory;
			info.msg.basePlayer.previousLife = this.previousLifeStory;
		}
	}

	public override void ScaleDamage(HitInfo info)
	{
		if (info.UseProtection)
		{
			HitArea hitArea = info.boneArea;
			if (hitArea == (HitArea.Head | HitArea.Chest | HitArea.Stomach | HitArea.Arm | HitArea.Hand | HitArea.Leg | HitArea.Foot))
			{
				this.baseProtection.Scale(info.damageTypes, 1f);
			}
			else
			{
				this.cachedProtection.Clear();
				this.cachedProtection.Add(this.inventory.containerWear.itemList, hitArea);
				this.cachedProtection.Multiply(DamageType.Arrow, ConVar.Server.arrowarmor);
				this.cachedProtection.Multiply(DamageType.Bullet, ConVar.Server.bulletarmor);
				this.cachedProtection.Multiply(DamageType.Slash, ConVar.Server.meleearmor);
				this.cachedProtection.Multiply(DamageType.Blunt, ConVar.Server.meleearmor);
				this.cachedProtection.Multiply(DamageType.Stab, ConVar.Server.meleearmor);
				this.cachedProtection.Multiply(DamageType.Bleeding, ConVar.Server.bleedingarmor);
				this.cachedProtection.Scale(info.damageTypes, 1f);
			}
		}
		if (info.damageProperties)
		{
			info.damageProperties.ScaleDamage(info);
		}
	}

	public void SendConsoleCommand(string command, params object[] obj)
	{
		ConsoleNetwork.SendClientCommand(this.net.connection, command, obj);
	}

	public void SendDeathInformation()
	{
		base.ClientRPCPlayer(null, this, "OnDied");
	}

	public void SendEntitySnapshot(BaseNetworkable ent)
	{
		using (TimeWarning timeWarning = TimeWarning.New("SendEntitySnapshot", 0.1f))
		{
			if (ent != null)
			{
				if (ent.net != null)
				{
					if (ent.ShouldNetworkTo(this))
					{
						if (Network.Net.sv.write.Start())
						{
							this.net.connection.validate.entityUpdates++;
							BaseNetworkable.SaveInfo saveInfo = new BaseNetworkable.SaveInfo()
							{
								forConnection = this.net.connection,
								forDisk = false
							};
							BaseNetworkable.SaveInfo saveInfo1 = saveInfo;
							Network.Net.sv.write.PacketID(Message.Type.Entities);
							Network.Net.sv.write.UInt32(this.net.connection.validate.entityUpdates);
							ent.ToStreamForNetwork(Network.Net.sv.write, saveInfo1);
							Network.Net.sv.write.Send(new SendInfo(this.net.connection));
						}
					}
				}
			}
		}
	}

	public void SendEntityUpdate()
	{
		using (TimeWarning timeWarning = TimeWarning.New("SendEntityUpdate", 0.1f))
		{
			this.SendEntityUpdates(this.SnapshotQueue);
			this.SendEntityUpdates(this.networkQueue[0]);
			this.SendEntityUpdates(this.networkQueue[1]);
		}
	}

	private void SendEntityUpdates(BasePlayer.NetworkQueueList queue)
	{
		if (queue.queueInternal.Count == 0)
		{
			return;
		}
		int num = (this.IsReceivingSnapshot ? ConVar.Server.updatebatchspawn : ConVar.Server.updatebatch);
		List<BaseNetworkable> list = Facepunch.Pool.GetList<BaseNetworkable>();
		using (TimeWarning timeWarning = TimeWarning.New("SendEntityUpdates.SendEntityUpdates", 0.1f))
		{
			int num1 = 0;
			foreach (BaseNetworkable baseNetworkable in queue.queueInternal)
			{
				this.SendEntitySnapshot(baseNetworkable);
				list.Add(baseNetworkable);
				num1++;
				if (num1 <= num)
				{
					continue;
				}
				goto Label0;
			}
		}
	Label0:
		if (num <= queue.queueInternal.Count)
		{
			using (timeWarning = TimeWarning.New("SendEntityUpdates.Remove", 0.1f))
			{
				for (int i = 0; i < list.Count; i++)
				{
					queue.queueInternal.Remove(list[i]);
				}
			}
		}
		else
		{
			queue.queueInternal.Clear();
		}
		if (queue.queueInternal.Count == 0 && queue.MaxLength > 2048)
		{
			queue.queueInternal.Clear();
			queue.queueInternal = new HashSet<BaseNetworkable>();
			queue.MaxLength = 0;
		}
		Facepunch.Pool.FreeList<BaseNetworkable>(ref list);
	}

	public void SendFullSnapshot()
	{
		using (TimeWarning timeWarning = TimeWarning.New("SendFullSnapshot", 0.1f))
		{
			foreach (Group group in this.net.subscriber.subscribed)
			{
				if (group.ID == 0)
				{
					continue;
				}
				this.EnterVisibility(group);
			}
		}
	}

	public void SendGlobalSnapshot()
	{
		using (TimeWarning timeWarning = TimeWarning.New("SendGlobalSnapshot", (long)10))
		{
			this.EnterVisibility(Network.Net.sv.visibility.Get(0));
		}
	}

	private void SendModelState()
	{
		if (!this.wantsSendModelState)
		{
			return;
		}
		if (this.nextModelStateUpdate > UnityEngine.Time.time)
		{
			return;
		}
		this.wantsSendModelState = false;
		this.nextModelStateUpdate = UnityEngine.Time.time + 0.1f;
		if (this.IsDead())
		{
			return;
		}
		if (this.IsSpectating())
		{
			return;
		}
		this.modelState.sleeping = this.IsSleeping();
		this.modelState.mounted = this.isMounted;
		this.modelState.relaxed = this.IsRelaxed();
		base.ClientRPC<ModelState>(null, "OnModelState", this.modelState);
	}

	public void SendRespawnOptions()
	{
		using (RespawnInformation respawnInformation = Facepunch.Pool.Get<RespawnInformation>())
		{
			respawnInformation.spawnOptions = Facepunch.Pool.Get<List<RespawnInformation.SpawnOptions>>();
			SleepingBag[] sleepingBagArray = SleepingBag.FindForPlayer(this.userID, true);
			for (int i = 0; i < (int)sleepingBagArray.Length; i++)
			{
				SleepingBag sleepingBag = sleepingBagArray[i];
				RespawnInformation.SpawnOptions d = Facepunch.Pool.Get<RespawnInformation.SpawnOptions>();
				d.id = sleepingBag.net.ID;
				d.name = sleepingBag.niceName;
				d.type = RespawnInformation.SpawnOptions.RespawnType.SleepingBag;
				d.unlockSeconds = sleepingBag.unlockSeconds;
				respawnInformation.spawnOptions.Add(d);
			}
			respawnInformation.previousLife = this.previousLifeStory;
			respawnInformation.fadeIn = (this.previousLifeStory == null ? false : (ulong)this.previousLifeStory.timeDied > (long)(Epoch.Current - 5));
			base.ClientRPCPlayer<RespawnInformation>(null, this, "OnRespawnInformation", respawnInformation);
		}
	}

	public void SendSnapshots(ListHashSet<Networkable> ents)
	{
		using (TimeWarning timeWarning = TimeWarning.New("SendSnapshots", 0.1f))
		{
			int count = ents.Values.Count;
			Networkable[] buffer = ents.Values.Buffer;
			for (int i = 0; i < count; i++)
			{
				this.SnapshotQueue.Add(buffer[i].handler as BaseNetworkable);
			}
		}
	}

	public static void ServerCycle(float deltaTime)
	{
		BasePlayer.activePlayerList.RemoveAll((BasePlayer x) => x == null);
		List<BasePlayer> basePlayers = Facepunch.Pool.Get<List<BasePlayer>>();
		basePlayers.AddRange(BasePlayer.activePlayerList);
		for (int i = 0; i < basePlayers.Count; i++)
		{
			if (basePlayers[i] != null)
			{
				basePlayers[i].ServerUpdate(deltaTime);
			}
		}
		if (ConVar.Server.idlekick > 0 && (ServerMgr.AvailableSlots <= 0 && ConVar.Server.idlekickmode == 1 || ConVar.Server.idlekickmode == 2))
		{
			for (int j = 0; j < basePlayers.Count; j++)
			{
				if (basePlayers[j].IdleTime >= (float)(ConVar.Server.idlekick * 60) && (!basePlayers[j].IsAdmin || ConVar.Server.idlekickadmins != 0) && (!basePlayers[j].IsDeveloper || ConVar.Server.idlekickadmins != 0))
				{
					basePlayers[j].Kick(string.Concat("Idle for ", ConVar.Server.idlekick, " minutes"));
				}
			}
		}
		Facepunch.Pool.FreeList<BasePlayer>(ref basePlayers);
	}

	public void ServerFall()
	{
		RaycastHit raycastHit;
		if (this.IsDead() || !this.IsWounded() && !this.IsSleeping())
		{
			this.EnableServerFall(false);
			return;
		}
		float single = UnityEngine.Time.time - this.lastFallTime;
		this.lastFallTime = UnityEngine.Time.time;
		float radius = this.GetRadius();
		float height = this.GetHeight(true) * 0.5f;
		float single1 = 2.5f;
		float single2 = 0.5f;
		this.fallVelocity = this.fallVelocity + UnityEngine.Physics.gravity.y * single1 * single2 * single;
		float single3 = Mathf.Abs(this.fallVelocity * single);
		UnityEngine.Vector3 vector3 = base.transform.position + (UnityEngine.Vector3.up * (radius + height));
		UnityEngine.Vector3 vector31 = base.transform.position;
		UnityEngine.Vector3 vector32 = base.transform.position;
		if (UnityEngine.Physics.SphereCast(vector3, radius, UnityEngine.Vector3.down, out raycastHit, single3 + height, 1537286401, QueryTriggerInteraction.Ignore))
		{
			this.EnableServerFall(false);
			if (raycastHit.distance > height)
			{
				vector32 = vector32 + (UnityEngine.Vector3.down * (raycastHit.distance - height));
			}
			this.ApplyFallDamageFromVelocity(this.fallVelocity);
			this.UpdateEstimatedVelocity(vector32, vector32, single);
			this.fallVelocity = 0f;
		}
		else if (!UnityEngine.Physics.Raycast(vector3, UnityEngine.Vector3.down, out raycastHit, single3 + radius + height, 1537286401, QueryTriggerInteraction.Ignore))
		{
			vector32 = vector32 + (UnityEngine.Vector3.down * single3);
			this.UpdateEstimatedVelocity(vector31, vector32, single);
			if (WaterLevel.Test(vector32))
			{
				this.EnableServerFall(false);
			}
		}
		else
		{
			this.EnableServerFall(false);
			if (raycastHit.distance > height - radius)
			{
				vector32 = vector32 + (UnityEngine.Vector3.down * (raycastHit.distance - height - radius));
			}
			this.ApplyFallDamageFromVelocity(this.fallVelocity);
			this.UpdateEstimatedVelocity(vector32, vector32, single);
			this.fallVelocity = 0f;
		}
		this.MovePosition(vector32);
	}

	public override void ServerInit()
	{
		this.stats = new PlayerStatistics(this);
		if (this.userID == 0)
		{
			this.userID = (ulong)UnityEngine.Random.Range(0, 10000000);
			this.UserIDString = this.userID.ToString();
			this.displayName = this.UserIDString;
		}
		this.UpdatePlayerCollider(true);
		this.UpdatePlayerRigidbody(!this.IsSleeping());
		base.ServerInit();
		BaseEntity.Query.Server.AddPlayer(this);
		this.inventory.ServerInit(this);
		this.metabolism.ServerInit(this);
	}

	public void ServerNoteFiredProjectile(int projectileid, UnityEngine.Vector3 startPos, UnityEngine.Vector3 startVel, AttackEntity attackEnt, ItemDefinition firedItemDef, Item pickupItem = null)
	{
		AttackEntity component;
		BaseProjectile baseProjectile = attackEnt as BaseProjectile;
		ItemModProjectile itemModProjectile = firedItemDef.GetComponent<ItemModProjectile>();
		Projectile projectile = itemModProjectile.projectileObject.Get().GetComponent<Projectile>();
		int num = 0;
		if (startPos.IsNaNOrInfinity() || startVel.IsNaNOrInfinity())
		{
			return;
		}
		BasePlayer.FiredProjectile firedProjectile = new BasePlayer.FiredProjectile()
		{
			itemDef = firedItemDef,
			itemMod = itemModProjectile,
			projectilePrefab = projectile,
			firedTime = UnityEngine.Time.realtimeSinceStartup,
			travelTime = 0f,
			weaponSource = attackEnt
		};
		if (attackEnt == null)
		{
			component = null;
		}
		else
		{
			component = GameManager.server.FindPrefab(StringPool.Get(attackEnt.prefabID)).GetComponent<AttackEntity>();
		}
		firedProjectile.weaponPrefab = component;
		firedProjectile.projectileModifier = (baseProjectile == null ? Projectile.Modifier.Default : baseProjectile.GetProjectileModifier());
		firedProjectile.pickupItem = pickupItem;
		firedProjectile.integrity = 1f;
		firedProjectile.position = startPos;
		firedProjectile.velocity = startVel;
		firedProjectile.initialPosition = startPos;
		firedProjectile.initialVelocity = startVel;
		firedProjectile.protection = num;
		this.firedProjectiles.Add(projectileid, firedProjectile);
	}

	protected void ServerUpdate(float deltaTime)
	{
		if (!Network.Net.sv.IsConnected())
		{
			return;
		}
		this.LifeStoryUpdate(deltaTime);
		this.FinalizeTick(deltaTime);
		this.desyncTime = Mathf.Max(this.timeSinceLastTick - deltaTime, 0f);
		if (UnityEngine.Time.realtimeSinceStartup < this.lastPlayerTick + 0.0625f)
		{
			return;
		}
		if (this.lastPlayerTick < UnityEngine.Time.realtimeSinceStartup - 6.25f)
		{
			this.lastPlayerTick = UnityEngine.Time.realtimeSinceStartup - UnityEngine.Random.Range(0f, 0.0625f);
		}
		while (this.lastPlayerTick < UnityEngine.Time.realtimeSinceStartup)
		{
			this.lastPlayerTick += 0.0625f;
		}
		if (this.IsConnected)
		{
			this.ConnectedPlayerUpdate(0.0625f);
		}
	}

	public virtual void SetInfo(string key, string val)
	{
		if (!this.IsConnected)
		{
			return;
		}
		this.net.connection.info.Set(key, val);
	}

	public void SetLightsOn(bool isOn)
	{
		Item activeItem = this.GetActiveItem();
		if (activeItem != null)
		{
			BaseEntity heldEntity = activeItem.GetHeldEntity();
			if (heldEntity != null)
			{
				HeldEntity component = heldEntity.GetComponent<HeldEntity>();
				if (component)
				{
					component.SendMessage("SetLightsOn", !component.LightsOn(), SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		foreach (Item item in this.inventory.containerWear.itemList)
		{
			ItemModWearable itemModWearable = item.info.GetComponent<ItemModWearable>();
			if (!itemModWearable || !itemModWearable.emissive)
			{
				continue;
			}
			item.SetFlag(Item.Flag.IsOn, !item.HasFlag(Item.Flag.IsOn));
			item.MarkDirty();
		}
		if (this.isMounted)
		{
			this.GetMounted().LightToggle(this);
		}
	}

	public void SetPlayerFlag(BasePlayer.PlayerFlags f, bool b)
	{
		if (!b)
		{
			if (!this.HasPlayerFlag(f))
			{
				return;
			}
			this.playerFlags &= ~f;
		}
		else
		{
			if (this.HasPlayerFlag(f))
			{
				return;
			}
			this.playerFlags |= f;
		}
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public virtual bool ShouldDropActiveItem()
	{
		object obj = Interface.CallHook("CanDropActiveItem", this);
		if (!(obj as bool))
		{
			return true;
		}
		return (bool)obj;
	}

	public override bool ShouldInheritNetworkGroup()
	{
		return this.IsSpectating();
	}

	public override bool ShouldNetworkTo(BasePlayer player)
	{
		if (this.IsSpectating() && player != this && !player.net.connection.info.GetBool("global.specnet", false))
		{
			return false;
		}
		return base.ShouldNetworkTo(player);
	}

	private void SimulateProjectile(ref UnityEngine.Vector3 position, ref UnityEngine.Vector3 velocity, ref float partialTime, float travelTime, float deltaTime, UnityEngine.Vector3 gravity, float drag)
	{
		if (partialTime > Mathf.Epsilon)
		{
			float single = deltaTime - partialTime;
			position = position + (velocity * single);
			velocity = velocity + (gravity * deltaTime);
			velocity = velocity - ((velocity * drag) * deltaTime);
			travelTime -= single;
		}
		int num = Mathf.FloorToInt(travelTime / deltaTime);
		for (int i = 0; i < num; i++)
		{
			position = position + (velocity * deltaTime);
			velocity = velocity + (gravity * deltaTime);
			velocity = velocity - ((velocity * drag) * deltaTime);
		}
		partialTime = travelTime - deltaTime * (float)num;
		if (partialTime > Mathf.Epsilon)
		{
			position = position + (velocity * partialTime);
		}
	}

	public override float StartHealth()
	{
		return UnityEngine.Random.Range(50f, 60f);
	}

	public override float StartMaxHealth()
	{
		return 100f;
	}

	public virtual void StartSleeping()
	{
		if (this.IsSleeping())
		{
			return;
		}
		Interface.CallHook("OnPlayerSleep", this);
		this.EnsureDismounted();
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Sleeping, true);
		this.sleepStartTime = UnityEngine.Time.time;
		if (!BasePlayer.sleepingPlayerList.Contains(this))
		{
			BasePlayer.sleepingPlayerList.Add(this);
		}
		base.CancelInvoke(new Action(this.InventoryUpdate));
		base.CancelInvoke(new Action(this.TeamUpdate));
		this.inventory.loot.Clear();
		this.inventory.crafting.CancelAll(true);
		this.UpdatePlayerCollider(true);
		this.UpdatePlayerRigidbody(false);
		this.EnableServerFall(true);
	}

	public void StartSpectating()
	{
		if (this.IsSpectating())
		{
			return;
		}
		if (Interface.CallHook("OnPlayerSpectate", this, this.spectateFilter) != null)
		{
			return;
		}
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Spectating, true);
		base.gameObject.SetLayerRecursive(10);
		base.CancelInvoke(new Action(this.InventoryUpdate));
		this.ChatMessage("Becoming Spectator");
		this.UpdateSpectateTarget(this.spectateFilter);
	}

	public void StartWounded()
	{
		if (this.IsWounded())
		{
			return;
		}
		if (Interface.CallHook("OnPlayerWound", this) != null)
		{
			return;
		}
		this.stats.Add("wounded", 1, Stats.Steam);
		this.woundedDuration = UnityEngine.Random.Range(40f, 50f);
		this.woundedStartTime = UnityEngine.Time.realtimeSinceStartup;
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Wounded, true);
		this.EnableServerFall(true);
		base.SendNetworkUpdateImmediate(false);
		base.Invoke(new Action(this.WoundingTick), 1f);
	}

	public void StatSave()
	{
		if (this.stats != null)
		{
			this.stats.Save();
		}
	}

	public void StopSpectating()
	{
		if (!this.IsSpectating())
		{
			return;
		}
		if (Interface.CallHook("OnPlayerSpectateEnd", this, this.spectateFilter) != null)
		{
			return;
		}
		base.SetParent(null, false, false);
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Spectating, false);
		base.gameObject.SetLayerRecursive(17);
	}

	public void StopWounded()
	{
		if (!this.IsDead() && Interface.CallHook("OnPlayerRecover", this) != null)
		{
			return;
		}
		this.SetPlayerFlag(BasePlayer.PlayerFlags.Wounded, false);
		base.CancelInvoke(new Action(this.WoundingTick));
	}

	public void SV_ClothingChanged()
	{
		this.UpdateProtectionFromClothing();
		this.UpdateMoveSpeedFromClothing();
	}

	[RPC_Server]
	private void SV_Drink(BaseEntity.RPCMessage msg)
	{
		BasePlayer basePlayer = msg.player;
		UnityEngine.Vector3 vector3 = msg.read.Vector3();
		if (vector3.IsNaNOrInfinity())
		{
			return;
		}
		if (!basePlayer)
		{
			return;
		}
		if (!basePlayer.metabolism.CanConsume())
		{
			return;
		}
		if (UnityEngine.Vector3.Distance(basePlayer.transform.position, vector3) > 5f)
		{
			return;
		}
		if (!WaterLevel.Test(vector3))
		{
			return;
		}
		ItemDefinition atPoint = WaterResource.GetAtPoint(vector3);
		if (atPoint == null)
		{
			return;
		}
		ItemModConsumable component = atPoint.GetComponent<ItemModConsumable>();
		Item item = ItemManager.Create(atPoint, component.amountToConsume, (ulong)0);
		ItemModConsume itemModConsume = item.info.GetComponent<ItemModConsume>();
		if (itemModConsume.CanDoAction(item, basePlayer))
		{
			itemModConsume.DoAction(item, basePlayer);
		}
		if (item != null)
		{
			item.Remove(0f);
		}
		basePlayer.metabolism.MarkConsumption();
	}

	public bool SwapSeatCooldown()
	{
		return UnityEngine.Time.time < this.nextSeatSwapTime;
	}

	public void TeamDeathCleanup()
	{
		if (this.currentTeam != 0)
		{
			RelationshipManager.PlayerTeam playerTeam = RelationshipManager.Instance.FindTeam(this.currentTeam);
			if (playerTeam != null)
			{
				playerTeam.RemovePlayer(this.userID);
			}
		}
	}

	public void TeamUpdate()
	{
		if (!RelationshipManager.TeamsEnabled())
		{
			return;
		}
		if (this.IsConnected && this.currentTeam != 0)
		{
			RelationshipManager.PlayerTeam playerTeam = RelationshipManager.Instance.FindTeam(this.currentTeam);
			if (playerTeam == null)
			{
				return;
			}
			using (PlayerTeam list = Facepunch.Pool.Get<PlayerTeam>())
			{
				list.teamLeader = playerTeam.teamLeader;
				list.teamID = playerTeam.teamID;
				list.teamName = playerTeam.teamName;
				list.members = Facepunch.Pool.GetList<PlayerTeam.TeamMember>();
				foreach (ulong member in playerTeam.members)
				{
					BasePlayer basePlayer = RelationshipManager.FindByID(member);
					PlayerTeam.TeamMember teamMember = Facepunch.Pool.Get<PlayerTeam.TeamMember>();
					teamMember.displayName = (basePlayer != null ? basePlayer.displayName : "DEAD");
					teamMember.healthFraction = (basePlayer != null ? basePlayer.healthFraction : 0f);
					teamMember.position = (basePlayer != null ? basePlayer.transform.position : UnityEngine.Vector3.zero);
					teamMember.online = (basePlayer != null ? !basePlayer.IsSleeping() : false);
					teamMember.userID = member;
					list.members.Add(teamMember);
				}
				base.ClientRPCPlayer<PlayerTeam>(null, this, "CLIENT_ReceiveTeamInfo", list);
			}
		}
	}

	public void Teleport(BasePlayer player)
	{
		this.Teleport(player.transform.position);
	}

	public void Teleport(string strName, bool playersOnly)
	{
		BaseEntity[] baseEntityArray = BaseEntity.Util.FindTargets(strName, playersOnly);
		if (baseEntityArray == null || baseEntityArray.Length == 0)
		{
			return;
		}
		BaseEntity baseEntity = baseEntityArray[UnityEngine.Random.Range(0, (int)baseEntityArray.Length)];
		this.Teleport(baseEntity.transform.position);
	}

	public void Teleport(UnityEngine.Vector3 position)
	{
		this.MovePosition(position);
		base.ClientRPCPlayer<UnityEngine.Vector3>(null, this, "ForcePositionTo", position);
	}

	private void Tick_Spectator()
	{
		int num = 0;
		if (this.serverInput.WasJustPressed(BUTTON.JUMP))
		{
			num++;
		}
		if (this.serverInput.WasJustPressed(BUTTON.DUCK))
		{
			num--;
		}
		if (num != 0)
		{
			this.SpectateOffset += num;
			using (TimeWarning timeWarning = TimeWarning.New("UpdateSpectateTarget", 0.1f))
			{
				this.UpdateSpectateTarget(this.spectateFilter);
			}
		}
	}

	public float TimeAlive()
	{
		return this.lifeStory.secondsAlive;
	}

	public override BasePlayer ToPlayer()
	{
		return this;
	}

	public override string ToString()
	{
		uint d;
		if (this._name == null)
		{
			if (!base.isServer)
			{
				this._name = base.ShortPrefabName;
			}
			else
			{
				if (this.net != null)
				{
					d = this.net.ID;
				}
				else
				{
					d = 0;
				}
				this._name = string.Format("{1}[{0}/{2}]", d, this.displayName, this.userID);
			}
		}
		return this._name;
	}

	private void TransformState(Matrix4x4 matrix)
	{
		this.tickInterpolator.TransformEntries(matrix);
		UnityEngine.Quaternion quaternion = matrix.rotation;
		UnityEngine.Vector3 vector3 = new UnityEngine.Vector3(0f, quaternion.eulerAngles.y, 0f);
		this.eyes.bodyRotation = UnityEngine.Quaternion.Euler(vector3) * this.eyes.bodyRotation;
	}

	public bool TriggeredAntiHack(float seconds = 1f, float score = Single.PositiveInfinity)
	{
		if (UnityEngine.Time.realtimeSinceStartup - this.lastViolationTime < seconds)
		{
			return true;
		}
		return this.violationLevel > score;
	}

	internal void UpdateActiveItem(uint itemID)
	{
		Assert.IsTrue(base.isServer, "Realm should be server!");
		if (this.svActiveItemID == itemID)
		{
			return;
		}
		if (this.equippingBlocked)
		{
			itemID = 0;
		}
		Item activeItem = this.GetActiveItem();
		this.svActiveItemID = 0;
		if (activeItem != null)
		{
			HeldEntity heldEntity = activeItem.GetHeldEntity() as HeldEntity;
			if (heldEntity != null)
			{
				heldEntity.SetHeld(false);
			}
		}
		this.svActiveItemID = itemID;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		Item item = this.GetActiveItem();
		if (item != null)
		{
			HeldEntity heldEntity1 = item.GetHeldEntity() as HeldEntity;
			if (heldEntity1 != null)
			{
				heldEntity1.SetHeld(true);
			}
		}
		this.inventory.UpdatedVisibleHolsteredItems();
		Interface.CallHook("OnPlayerActiveItemChanged", this, activeItem, item);
	}

	public void UpdateEstimatedVelocity(UnityEngine.Vector3 lastPos, UnityEngine.Vector3 currentPos, float deltaTime)
	{
		this.estimatedVelocity = (currentPos - lastPos) / deltaTime;
		this.estimatedSpeed = this.estimatedVelocity.magnitude;
		this.estimatedSpeed2D = this.estimatedVelocity.Magnitude2D();
		if (this.estimatedSpeed < 0.01f)
		{
			this.estimatedSpeed = 0f;
		}
		if (this.estimatedSpeed2D < 0.01f)
		{
			this.estimatedSpeed2D = 0f;
		}
	}

	private void UpdateModelState()
	{
		if (this.IsDead())
		{
			return;
		}
		if (this.IsSpectating())
		{
			return;
		}
		this.wantsSendModelState = true;
	}

	internal void UpdateModelStateFromTick(PlayerTick tick)
	{
		if (tick.modelState == null)
		{
			return;
		}
		if (ModelState.Equal(this.modelStateTick, tick.modelState))
		{
			return;
		}
		if (this.modelStateTick != null)
		{
			this.modelStateTick.ResetToPool();
		}
		this.modelStateTick = tick.modelState;
		tick.modelState = null;
		this.tickNeedsFinalizing = true;
	}

	private void UpdateMoveSpeedFromClothing()
	{
		float single = 0f;
		float single1 = 0f;
		float single2 = 0f;
		bool flag = false;
		bool flag1 = false;
		float single3 = 0f;
		this.eggVision = 0f;
		foreach (Item item in this.inventory.containerWear.itemList)
		{
			ItemModWearable component = item.info.GetComponent<ItemModWearable>();
			if (!component)
			{
				continue;
			}
			if (component.blocksAiming)
			{
				flag = true;
			}
			if (component.blocksEquipping)
			{
				flag1 = true;
			}
			single3 += component.accuracyBonus;
			this.eggVision += component.eggVision;
			if (component.movementProperties == null)
			{
				continue;
			}
			single1 += component.movementProperties.speedReduction;
			single = Mathf.Max(single, component.movementProperties.minSpeedReduction);
			single2 += component.movementProperties.waterSpeedBonus;
		}
		this.clothingAccuracyBonus = single3;
		this.clothingMoveSpeedReduction = Mathf.Max(single1, single);
		this.clothingBlocksAiming = flag;
		this.clothingWaterSpeedBonus = single2;
		this.equippingBlocked = flag1;
		if (base.isServer && this.equippingBlocked)
		{
			this.UpdateActiveItem(0);
		}
	}

	public void UpdatePlayerCollider(bool state)
	{
		if (this.triggerCollider == null)
		{
			this.triggerCollider = base.gameObject.GetComponent<Collider>();
		}
		if (this.triggerCollider.enabled != state)
		{
			base.RemoveFromTriggers();
		}
		this.triggerCollider.enabled = state;
	}

	public void UpdatePlayerRigidbody(bool state)
	{
		if (this.physicsRigidbody == null)
		{
			this.physicsRigidbody = base.gameObject.GetComponent<Rigidbody>();
		}
		if (!state)
		{
			base.RemoveFromTriggers();
			if (this.physicsRigidbody != null)
			{
				GameManager.Destroy(this.physicsRigidbody, 0f);
				this.physicsRigidbody = null;
			}
		}
		else if (this.physicsRigidbody == null)
		{
			this.physicsRigidbody = base.gameObject.AddComponent<Rigidbody>();
			this.physicsRigidbody.useGravity = false;
			this.physicsRigidbody.isKinematic = true;
			this.physicsRigidbody.mass = 1f;
			this.physicsRigidbody.interpolation = RigidbodyInterpolation.None;
			this.physicsRigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
			return;
		}
	}

	internal void UpdatePositionFromTick(PlayerTick tick, bool wasPlayerStalled)
	{
		if (tick.position.IsNaNOrInfinity() || tick.eyePos.IsNaNOrInfinity())
		{
			this.Kick("Kicked: Invalid Position");
			return;
		}
		if (tick.parentID != this.parentEntity.uid)
		{
			return;
		}
		if (this.isMounted || this.modelState != null && this.modelState.mounted || tick.modelState != null && tick.modelState.mounted)
		{
			return;
		}
		if (wasPlayerStalled)
		{
			float single = UnityEngine.Vector3.Distance(tick.position, this.tickInterpolator.EndPoint);
			if (single > 0.01f)
			{
				AntiHack.ResetTimer(this);
			}
			if (single > 0.5f)
			{
				base.ClientRPCPlayer<UnityEngine.Vector3, uint>(null, this, "ForcePositionToParentOffset", this.tickInterpolator.EndPoint, this.parentEntity.uid);
			}
			return;
		}
		if (this.modelState != null && this.modelState.flying && (this.IsAdmin || this.IsDeveloper) || UnityEngine.Vector3.Distance(tick.position, this.tickInterpolator.EndPoint) <= 5f)
		{
			this.tickInterpolator.AddPoint(tick.position);
			this.tickNeedsFinalizing = true;
			return;
		}
		AntiHack.ResetTimer(this);
		base.ClientRPCPlayer<UnityEngine.Vector3, uint>(null, this, "ForcePositionToParentOffset", this.tickInterpolator.EndPoint, this.parentEntity.uid);
	}

	public virtual void UpdateProtectionFromClothing()
	{
		this.baseProtection.Clear();
		this.baseProtection.Add(this.inventory.containerWear.itemList, HitArea.Head | HitArea.Chest | HitArea.Stomach | HitArea.Arm | HitArea.Hand | HitArea.Leg | HitArea.Foot);
		float single = 0.166666672f;
		for (int i = 0; i < (int)this.baseProtection.amounts.Length; i++)
		{
			if (i != 17)
			{
				this.baseProtection.amounts[i] *= single;
			}
		}
	}

	public void UpdateRadiation(float fAmount)
	{
		this.metabolism.radiation_level.Increase(fAmount);
	}

	internal void UpdateRotationFromTick(PlayerTick tick)
	{
		if (tick.inputState == null)
		{
			return;
		}
		if (tick.inputState.aimAngles.IsNaNOrInfinity())
		{
			this.Kick("Kicked: Invalid Rotation");
			return;
		}
		this.tickViewAngles = tick.inputState.aimAngles;
		this.tickNeedsFinalizing = true;
	}

	public void UpdateSpectateTarget(string strName)
	{
		this.spectateFilter = strName;
		IEnumerable<BaseEntity> baseEntities = null;
		if (!this.spectateFilter.StartsWith("@"))
		{
			IEnumerable<BasePlayer> basePlayers = BasePlayer.activePlayerList.Where<BasePlayer>((BasePlayer x) => {
				if (x.IsSpectating() || x.IsDead())
				{
					return false;
				}
				return !x.IsSleeping();
			});
			if (strName.Length > 0)
			{
				basePlayers = basePlayers.Where<BasePlayer>((BasePlayer x) => {
					if (x.displayName.Contains(this.spectateFilter, CompareOptions.IgnoreCase))
					{
						return true;
					}
					return x.UserIDString.Contains(this.spectateFilter);
				}).Where<BasePlayer>((BasePlayer x) => x != this);
			}
			basePlayers = 
				from x in basePlayers
				orderby x.displayName
				select x;
			baseEntities = basePlayers.Cast<BaseEntity>();
		}
		else
		{
			string str = this.spectateFilter.Substring(1);
			baseEntities = (
				from x in BaseNetworkable.serverEntities
				where x.name.Contains(str, CompareOptions.IgnoreCase)
				where x != this
				select x).Cast<BaseEntity>();
		}
		BaseEntity[] array = baseEntities.ToArray<BaseEntity>();
		if (array.Length == 0)
		{
			this.ChatMessage("No valid spectate targets!");
			return;
		}
		BaseEntity baseEntity = array[this.SpectateOffset % (int)array.Length];
		if (baseEntity != null)
		{
			if (!(baseEntity is BasePlayer))
			{
				this.ChatMessage(string.Concat("Spectating: ", baseEntity.ToString()));
			}
			else
			{
				this.ChatMessage(string.Concat("Spectating: ", (baseEntity as BasePlayer).displayName));
			}
			using (TimeWarning timeWarning = TimeWarning.New("SendEntitySnapshot", 0.1f))
			{
				this.SendEntitySnapshot(baseEntity);
			}
			base.gameObject.Identity();
			using (timeWarning = TimeWarning.New("SetParent", 0.1f))
			{
				base.SetParent(baseEntity, false, false);
			}
		}
	}

	public void UpdateTeam(ulong newTeam)
	{
		this.currentTeam = newTeam;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		if (RelationshipManager.Instance.FindTeam(newTeam) == null)
		{
			this.ClearTeam();
			return;
		}
		this.TeamUpdate();
	}

	public bool UsedAdminCheat(float seconds = 1f)
	{
		return UnityEngine.Time.realtimeSinceStartup - this.lastAdminCheatTime < seconds;
	}

	public override float WaterFactor()
	{
		if (!this.isMounted)
		{
			return base.WaterFactor();
		}
		return this.GetMounted().WaterFactorForPlayer(this);
	}

	private bool WoundingCausingImmportality()
	{
		if (!this.IsWounded())
		{
			return false;
		}
		if (this.secondsSinceWoundedStarted > 0.25f)
		{
			return false;
		}
		return true;
	}

	private void WoundingTick()
	{
		using (TimeWarning timeWarning = TimeWarning.New("WoundingTick", 0.1f))
		{
			if (!this.IsDead())
			{
				if (this.secondsSinceWoundedStarted < this.woundedDuration)
				{
					base.Invoke(new Action(this.WoundingTick), 1f);
				}
				else if (UnityEngine.Random.Range(0, 100) >= 20)
				{
					this.Die(null);
				}
				else
				{
					this.SetPlayerFlag(BasePlayer.PlayerFlags.Wounded, false);
					Interface.CallHook("OnPlayerRecovered", this);
				}
			}
		}
	}

	private bool WoundInsteadOfDying(HitInfo info)
	{
		if (this.IsWounded())
		{
			return false;
		}
		if (!this.EligibleForWounding(info))
		{
			return false;
		}
		this.lastWoundedTime = UnityEngine.Time.realtimeSinceStartup;
		base.health = (float)UnityEngine.Random.Range(2, 6);
		this.metabolism.bleeding.@value = 0f;
		this.StartWounded();
		return true;
	}

	public enum CameraMode
	{
		FirstPerson,
		ThirdPerson,
		Eyes
	}

	public struct FiredProjectile
	{
		public ItemDefinition itemDef;

		public ItemModProjectile itemMod;

		public Projectile projectilePrefab;

		public float firedTime;

		public float travelTime;

		public float partialTime;

		public AttackEntity weaponSource;

		public AttackEntity weaponPrefab;

		public Projectile.Modifier projectileModifier;

		public Item pickupItem;

		public float integrity;

		public UnityEngine.Vector3 position;

		public UnityEngine.Vector3 velocity;

		public UnityEngine.Vector3 initialPosition;

		public UnityEngine.Vector3 initialVelocity;

		public int protection;
	}

	public enum NetworkQueue
	{
		Update,
		UpdateDistance,
		Count
	}

	private class NetworkQueueList
	{
		public HashSet<BaseNetworkable> queueInternal;

		public int MaxLength;

		public int Length
		{
			get
			{
				return this.queueInternal.Count;
			}
		}

		public NetworkQueueList()
		{
		}

		public void Add(BaseNetworkable ent)
		{
			if (!this.Contains(ent))
			{
				this.queueInternal.Add(ent);
			}
			this.MaxLength = Mathf.Max(this.MaxLength, this.queueInternal.Count);
		}

		public void Add(BaseNetworkable[] ent)
		{
			BaseNetworkable[] baseNetworkableArray = ent;
			for (int i = 0; i < (int)baseNetworkableArray.Length; i++)
			{
				this.Add(baseNetworkableArray[i]);
			}
		}

		public void Clear(Group group)
		{
			using (TimeWarning timeWarning = TimeWarning.New("NetworkQueueList.Clear", 0.1f))
			{
				if (group == null)
				{
					this.queueInternal.RemoveWhere((BaseNetworkable x) => {
						if (x == null || x.net == null || x.net.@group == null)
						{
							return true;
						}
						return !x.net.@group.isGlobal;
					});
				}
				else if (!group.isGlobal)
				{
					this.queueInternal.RemoveWhere((BaseNetworkable x) => {
						if (x == null || x.net == null || x.net.@group == null)
						{
							return true;
						}
						return x.net.@group == group;
					});
				}
			}
		}

		public bool Contains(BaseNetworkable ent)
		{
			return this.queueInternal.Contains(ent);
		}
	}

	[Flags]
	public enum PlayerFlags
	{
		Unused1 = 1,
		Unused2 = 2,
		IsAdmin = 4,
		ReceivingSnapshot = 8,
		Sleeping = 16,
		Spectating = 32,
		Wounded = 64,
		IsDeveloper = 128,
		Connected = 256,
		VoiceMuted = 512,
		ThirdPersonViewmode = 1024,
		EyesViewmode = 2048,
		ChatMute = 4096,
		NoSprint = 8192,
		Aiming = 16384,
		DisplaySash = 32768,
		Relaxed = 65536,
		SafeZone = 131072,
		ServerFall = 262144,
		Workbench1 = 1048576,
		Workbench2 = 2097152,
		Workbench3 = 4194304
	}

	public class SpawnPoint
	{
		public UnityEngine.Vector3 pos;

		public UnityEngine.Quaternion rot;

		public SpawnPoint()
		{
		}
	}
}