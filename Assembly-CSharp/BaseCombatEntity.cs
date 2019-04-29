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

public class BaseCombatEntity : BaseEntity
{
	[Header("BaseCombatEntity")]
	public SkeletonProperties skeletonProperties;

	public ProtectionProperties baseProtection;

	public float startHealth;

	public BaseCombatEntity.Pickup pickup;

	public BaseCombatEntity.Repair repair;

	public bool ShowHealthInfo = true;

	public BaseCombatEntity.LifeState lifestate;

	public bool sendsHitNotification;

	public bool sendsMeleeHitNotification = true;

	public bool markAttackerHostile = true;

	public float _health;

	public float _maxHealth = 100f;

	public float lastAttackedTime = Single.NegativeInfinity;

	public float lastDealtDamageTime = Single.NegativeInfinity;

	private int lastNotifyFrame;

	private const float MAX_HEALTH_REPAIR = 50f;

	[NonSerialized]
	public DamageType lastDamage;

	[NonSerialized]
	public BaseEntity lastAttacker;

	[NonSerialized]
	public Collider _collider;

	[NonSerialized]
	public bool ResetLifeStateOnSpawn = true;

	public DirectionProperties[] propDirection;

	public float unHostileTime;

	public float health
	{
		get
		{
			return this._health;
		}
		set
		{
			float single = this._health;
			this._health = Mathf.Clamp(value, 0f, this.MaxHealth());
			if (base.isServer && this._health != single)
			{
				this.OnHealthChanged(single, this._health);
			}
		}
	}

	public float healthFraction
	{
		get
		{
			return JustDecompileGenerated_get_healthFraction();
		}
		set
		{
			JustDecompileGenerated_set_healthFraction(value);
		}
	}

	public float JustDecompileGenerated_get_healthFraction()
	{
		return this.Health() / this.MaxHealth();
	}

	public void JustDecompileGenerated_set_healthFraction(float value)
	{
		this.health = this.MaxHealth() * value;
	}

	public Vector3 LastAttackedDir
	{
		get;
		set;
	}

	public float SecondsSinceAttacked
	{
		get
		{
			return UnityEngine.Time.time - this.lastAttackedTime;
		}
	}

	public float SecondsSinceDealtDamage
	{
		get
		{
			return UnityEngine.Time.time - this.lastDealtDamageTime;
		}
	}

	public BaseCombatEntity()
	{
	}

	public virtual List<ItemAmount> BuildCost()
	{
		if (this.repair.itemTarget == null)
		{
			return null;
		}
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(this.repair.itemTarget);
		if (itemBlueprint == null)
		{
			return null;
		}
		return itemBlueprint.ingredients;
	}

	public virtual bool CanPickup(BasePlayer player)
	{
		object obj = Interface.CallHook("CanPickupEntity", player, this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (!this.pickup.enabled)
		{
			return false;
		}
		if (!this.pickup.requireBuildingPrivilege)
		{
			return true;
		}
		if (!player.CanBuild())
		{
			return false;
		}
		if (!this.pickup.requireHammer)
		{
			return true;
		}
		return player.IsHoldingEntity<Hammer>();
	}

	public virtual void ChangeHealth(float amount)
	{
		if (amount == 0f)
		{
			return;
		}
		if (amount > 0f)
		{
			this.Heal(amount);
			return;
		}
		this.Hurt(Mathf.Abs(amount));
	}

	private void DebugHurt(HitInfo info)
	{
		if (!ConVar.Vis.damage)
		{
			return;
		}
		if (info.PointStart != info.PointEnd)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.arrow", new object[] { 60, Color.cyan, info.PointStart, info.PointEnd, 0.1f });
			ConsoleNetwork.BroadcastToAllClients("ddraw.sphere", new object[] { 60, Color.cyan, info.HitPositionWorld, 0.01f });
		}
		string str = "";
		for (int i = 0; i < (int)info.damageTypes.types.Length; i++)
		{
			float single = info.damageTypes.types[i];
			if (single != 0f)
			{
				string[] strArrays = new string[] { str, " ", null, null, null };
				DamageType damageType = (DamageType)i;
				strArrays[2] = damageType.ToString().PadRight(10);
				strArrays[3] = single.ToString("0.00");
				strArrays[4] = "\n";
				str = string.Concat(strArrays);
			}
		}
		object[] initiator = new object[18];
		initiator[0] = "<color=lightblue>Damage:</color>".PadRight(10);
		float single1 = info.damageTypes.Total();
		initiator[1] = single1.ToString("0.00");
		initiator[2] = "\n<color=lightblue>Health:</color>".PadRight(10);
		single1 = this.health;
		initiator[3] = single1.ToString("0.00");
		initiator[4] = " / ";
		initiator[5] = (this.health - info.damageTypes.Total() <= 0f ? "<color=red>" : "<color=green>");
		single1 = this.health - info.damageTypes.Total();
		initiator[6] = single1.ToString("0.00");
		initiator[7] = "</color>";
		initiator[8] = "\n<color=lightblue>HitEnt:</color>".PadRight(10);
		initiator[9] = this;
		initiator[10] = "\n<color=lightblue>HitBone:</color>".PadRight(10);
		initiator[11] = info.boneName;
		initiator[12] = "\n<color=lightblue>Attacker:</color>".PadRight(10);
		initiator[13] = info.Initiator;
		initiator[14] = "\n<color=lightblue>WeaponPrefab:</color>".PadRight(10);
		initiator[15] = info.WeaponPrefab;
		initiator[16] = "\n<color=lightblue>Damages:</color>\n";
		initiator[17] = str;
		string str1 = string.Concat(initiator);
		ConsoleNetwork.BroadcastToAllClients("ddraw.text", new object[] { 60, Color.white, info.HitPositionWorld, str1 });
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			this.UpdateSurroundings();
		}
	}

	public virtual void Die(HitInfo info = null)
	{
		if (this.IsDead())
		{
			return;
		}
		Interface.CallHook("OnEntityDeath", this, info);
		if (ConVar.Global.developer > 1)
		{
			Debug.Log(string.Concat("[Combat]".PadRight(10), base.gameObject.name, " died"));
		}
		this.health = 0f;
		this.lifestate = BaseCombatEntity.LifeState.Dead;
		using (TimeWarning timeWarning = TimeWarning.New("OnKilled", 0.1f))
		{
			this.OnKilled(info);
		}
	}

	public void DieInstantly()
	{
		if (this.IsDead())
		{
			return;
		}
		if (ConVar.Global.developer > 1)
		{
			Debug.Log(string.Concat("[Combat]".PadRight(10), base.gameObject.name, " died"));
		}
		this.health = 0f;
		this.lifestate = BaseCombatEntity.LifeState.Dead;
		this.OnKilled(null);
	}

	public void DoHitNotify(HitInfo info)
	{
		using (TimeWarning timeWarning = TimeWarning.New("DoHitNotify", 0.1f))
		{
			if (this.sendsHitNotification && !(info.Initiator == null) && info.Initiator is BasePlayer && !info.isHeadshot && !(this == info.Initiator))
			{
				if (UnityEngine.Time.frameCount != this.lastNotifyFrame)
				{
					this.lastNotifyFrame = UnityEngine.Time.frameCount;
					bool weapon = info.Weapon is BaseMelee;
					if (base.isServer && (!weapon || this.sendsMeleeHitNotification))
					{
						bool initiator = info.Initiator.net.connection == info.Predicted;
						base.ClientRPCPlayer<bool>(null, info.Initiator as BasePlayer, "HitNotify", initiator);
					}
				}
			}
		}
	}

	public virtual void DoRepair(BasePlayer player)
	{
		if (!this.repair.enabled)
		{
			return;
		}
		if (Interface.CallHook("OnStructureRepair", this, player) != null)
		{
			return;
		}
		if (this.SecondsSinceAttacked <= 30f)
		{
			this.OnRepairFailed();
			return;
		}
		float single = this.MaxHealth() - this.health;
		float single1 = single / this.MaxHealth();
		if (single <= 0f || single1 <= 0f)
		{
			this.OnRepairFailed();
			return;
		}
		List<ItemAmount> itemAmounts = this.RepairCost(single1);
		if (itemAmounts == null)
		{
			return;
		}
		float single2 = itemAmounts.Sum<ItemAmount>((ItemAmount x) => x.amount);
		if (single2 <= 0f)
		{
			this.health = this.health + single;
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		else
		{
			float single3 = itemAmounts.Min<ItemAmount>((ItemAmount x) => Mathf.Clamp01((float)player.inventory.GetAmount(x.itemid) / x.amount));
			single3 = Mathf.Min(single3, 50f / single);
			if (single3 <= 0f)
			{
				this.OnRepairFailed();
				return;
			}
			int num = 0;
			foreach (ItemAmount itemAmount in itemAmounts)
			{
				int num1 = Mathf.CeilToInt(single3 * itemAmount.amount);
				int num2 = player.inventory.Take(null, itemAmount.itemid, num1);
				if (num2 <= 0)
				{
					continue;
				}
				num += num2;
				player.Command("note.inv", new object[] { itemAmount.itemid, num2 * -1 });
			}
			float single4 = (float)num / (float)single2;
			this.health = this.health + single * single4;
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
		}
		if (this.health >= this.MaxHealth())
		{
			this.OnRepairFinished();
			return;
		}
		this.OnRepair();
	}

	public virtual float GetThreatLevel()
	{
		return 0f;
	}

	public virtual void Heal(float amount)
	{
		if (ConVar.Global.developer > 1)
		{
			Debug.Log(string.Concat("[Combat]".PadRight(10), base.gameObject.name, " healed"));
		}
		this.health = Mathf.Clamp(this.health + amount, 0f, this.MaxHealth());
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public override float Health()
	{
		return this._health;
	}

	public virtual void Hurt(float amount)
	{
		this.Hurt(Mathf.Abs(amount), DamageType.Generic, null, true);
	}

	public virtual void Hurt(float amount, DamageType type, BaseEntity attacker = null, bool useProtection = true)
	{
		using (TimeWarning timeWarning = TimeWarning.New("Hurt", 0.1f))
		{
			HitInfo hitInfo = new HitInfo(attacker, this, type, amount, base.transform.position)
			{
				UseProtection = useProtection
			};
			this.Hurt(hitInfo);
		}
	}

	public virtual void Hurt(HitInfo info)
	{
		Assert.IsTrue(base.isServer, "This should be called serverside only");
		if (this.IsDead())
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("Hurt( HitInfo )", (long)50))
		{
			float single = this.health;
			this.ScaleDamage(info);
			if (info.PointStart != Vector3.zero)
			{
				for (int i = 0; i < (int)this.propDirection.Length; i++)
				{
					if (!(this.propDirection[i].extraProtection == null) && !this.propDirection[i].IsWeakspot(base.transform, info))
					{
						this.propDirection[i].extraProtection.Scale(info.damageTypes, 1f);
					}
				}
			}
			info.damageTypes.Scale(DamageType.Arrow, ConVar.Server.arrowdamage);
			info.damageTypes.Scale(DamageType.Bullet, ConVar.Server.bulletdamage);
			info.damageTypes.Scale(DamageType.Slash, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Blunt, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Stab, ConVar.Server.meleedamage);
			info.damageTypes.Scale(DamageType.Bleeding, ConVar.Server.bleedingdamage);
			if (Interface.CallHook("IOnBaseCombatEntityHurt", this, info) != null)
			{
				return;
			}
			this.DebugHurt(info);
			this.health = single - info.damageTypes.Total();
			base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
			if (ConVar.Global.developer > 1)
			{
				object[] str = new object[] { "[Combat]".PadRight(10), base.gameObject.name, " hurt ", info.damageTypes.GetMajorityDamageType(), "/", info.damageTypes.Total(), " - ", null, null };
				str[7] = this.health.ToString("0");
				str[8] = " health left";
				Debug.Log(string.Concat(str));
			}
			this.lastDamage = info.damageTypes.GetMajorityDamageType();
			this.lastAttacker = info.Initiator;
			if (this.lastAttacker != null)
			{
				BaseCombatEntity baseCombatEntity = this.lastAttacker as BaseCombatEntity;
				if (baseCombatEntity != null)
				{
					baseCombatEntity.lastDealtDamageTime = UnityEngine.Time.time;
				}
			}
			BaseCombatEntity baseCombatEntity1 = this.lastAttacker as BaseCombatEntity;
			if (this.markAttackerHostile && baseCombatEntity1 != null && baseCombatEntity1 != this)
			{
				baseCombatEntity1.MarkHostileFor(60f);
			}
			if (this.lastDamage != DamageType.Decay)
			{
				this.lastAttackedTime = UnityEngine.Time.time;
				if (this.lastAttacker != null)
				{
					Vector3 vector3 = this.lastAttacker.transform.position - base.transform.position;
					this.LastAttackedDir = vector3.normalized;
				}
			}
			if (this.health <= 0f)
			{
				this.Die(info);
			}
			BasePlayer initiatorPlayer = info.InitiatorPlayer;
			if (initiatorPlayer)
			{
				if (!this.IsDead())
				{
					initiatorPlayer.stats.combat.Log(info, single, this.health, null);
				}
				else
				{
					initiatorPlayer.stats.combat.Log(info, single, this.health, "killed");
					return;
				}
			}
		}
	}

	public virtual void InitializeHealth(float newhealth, float newmax)
	{
		this._maxHealth = newmax;
		this._health = newhealth;
		this.lifestate = BaseCombatEntity.LifeState.Alive;
	}

	public virtual bool IsAlive()
	{
		return this.lifestate == BaseCombatEntity.LifeState.Alive;
	}

	public virtual bool IsDead()
	{
		return this.lifestate == BaseCombatEntity.LifeState.Dead;
	}

	public bool IsHostile()
	{
		object obj = Interface.CallHook("CanEntityBeHostile", this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		return this.unHostileTime > UnityEngine.Time.realtimeSinceStartup;
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		if (base.isServer)
		{
			this.lifestate = BaseCombatEntity.LifeState.Alive;
		}
		if (info.msg.baseCombat != null)
		{
			this.lifestate = (BaseCombatEntity.LifeState)info.msg.baseCombat.state;
			this._health = info.msg.baseCombat.health;
		}
		base.Load(info);
	}

	public virtual void MarkHostileFor(float duration = 60f)
	{
		if (Interface.CallHook("OnEntityMarkHostile", this, duration) != null)
		{
			return;
		}
		float single = UnityEngine.Time.realtimeSinceStartup + duration;
		this.unHostileTime = Mathf.Max(this.unHostileTime, single);
	}

	public override float MaxHealth()
	{
		return this._maxHealth;
	}

	public override void OnAttacked(HitInfo info)
	{
		using (TimeWarning timeWarning = TimeWarning.New("BaseCombatEntity.OnAttacked", 0.1f))
		{
			if (!this.IsDead())
			{
				this.DoHitNotify(info);
			}
			if (base.isServer)
			{
				this.Hurt(info);
			}
		}
		base.OnAttacked(info);
	}

	public virtual void OnHealthChanged(float oldvalue, float newvalue)
	{
	}

	public virtual void OnKilled(HitInfo info)
	{
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	public virtual void OnPickedUp(Item createdItem, BasePlayer player)
	{
	}

	public virtual void OnRepair()
	{
		Effect.server.Run("assets/bundled/prefabs/fx/build/repair.prefab", this, 0, Vector3.zero, Vector3.zero, null, false);
	}

	public virtual void OnRepairFailed()
	{
		Effect.server.Run("assets/bundled/prefabs/fx/build/repair_failed.prefab", this, 0, Vector3.zero, Vector3.zero, null, false);
	}

	public virtual void OnRepairFinished()
	{
		Effect.server.Run("assets/bundled/prefabs/fx/build/repair_full.prefab", this, 0, Vector3.zero, Vector3.zero, null, false);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("BaseCombatEntity.OnRpcMessage", 0.1f))
		{
			if (rpc != 1191093595 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (ConVar.Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - RPC_PickupStart "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("RPC_PickupStart", 0.1f))
				{
					using (TimeWarning timeWarning2 = TimeWarning.New("Conditions", 0.1f))
					{
						if (!BaseEntity.RPC_Server.MaxDistance.Test("RPC_PickupStart", this, player, 3f))
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
							this.RPC_PickupStart(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in RPC_PickupStart");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	public override float PenetrationResistance(HitInfo info)
	{
		if (!this.baseProtection)
		{
			return 1f;
		}
		return this.baseProtection.density;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.Health() > this.MaxHealth())
		{
			this.health = this.MaxHealth();
		}
		if (float.IsNaN(this.Health()))
		{
			this.health = this.MaxHealth();
		}
	}

	public virtual List<ItemAmount> RepairCost(float healthMissingFraction)
	{
		List<ItemAmount> itemAmounts = this.BuildCost();
		if (itemAmounts == null)
		{
			return null;
		}
		List<ItemAmount> itemAmounts1 = new List<ItemAmount>();
		foreach (ItemAmount itemAmount in itemAmounts)
		{
			int num = Mathf.RoundToInt(itemAmount.amount * this.RepairCostFraction() * healthMissingFraction);
			if (num <= 0)
			{
				continue;
			}
			itemAmounts1.Add(new ItemAmount(itemAmount.itemDef, (float)num));
		}
		return itemAmounts1;
	}

	public virtual float RepairCostFraction()
	{
		return 0.5f;
	}

	public override void ResetState()
	{
		base.ResetState();
		this._health = this._maxHealth;
		this.lastAttackedTime = Single.NegativeInfinity;
		this.lastDealtDamageTime = Single.NegativeInfinity;
	}

	[MaxDistance(3f)]
	[RPC_Server]
	private void RPC_PickupStart(BaseEntity.RPCMessage rpc)
	{
		if (!rpc.player.CanInteract())
		{
			return;
		}
		if (!this.CanPickup(rpc.player))
		{
			return;
		}
		Item item = ItemManager.Create(this.pickup.itemTarget, this.pickup.itemCount, this.skinID);
		if (this.pickup.setConditionFromHealth && item.hasCondition)
		{
			item.conditionNormalized = Mathf.Clamp01(this.healthFraction - this.pickup.subtractCondition);
		}
		rpc.player.GiveItem(item, BaseEntity.GiveItemReason.PickedUp);
		this.OnPickedUp(item, rpc.player);
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.baseCombat = Facepunch.Pool.Get<BaseCombat>();
		info.msg.baseCombat.state = (int)this.lifestate;
		info.msg.baseCombat.health = this._health;
	}

	public virtual void ScaleDamage(HitInfo info)
	{
		if (info.UseProtection && this.baseProtection != null)
		{
			this.baseProtection.Scale(info.damageTypes, 1f);
		}
	}

	public override void ServerInit()
	{
		this._collider = base.transform.GetComponentInChildrenIncludeDisabled<Collider>();
		this.propDirection = PrefabAttribute.server.FindAll<DirectionProperties>(this.prefabID);
		if (this.ResetLifeStateOnSpawn)
		{
			this.InitializeHealth(this.StartHealth(), this.StartMaxHealth());
			this.lifestate = BaseCombatEntity.LifeState.Alive;
		}
		base.ServerInit();
	}

	public HitArea SkeletonLookup(uint boneID)
	{
		if (this.skeletonProperties == null)
		{
			return HitArea.Head | HitArea.Chest | HitArea.Stomach | HitArea.Arm | HitArea.Hand | HitArea.Leg | HitArea.Foot;
		}
		SkeletonProperties.BoneProperty boneProperty = this.skeletonProperties.FindBone(boneID);
		if (boneProperty == null)
		{
			return HitArea.Head | HitArea.Chest | HitArea.Stomach | HitArea.Arm | HitArea.Hand | HitArea.Leg | HitArea.Foot;
		}
		return boneProperty.area;
	}

	public virtual float StartHealth()
	{
		return this.startHealth;
	}

	public virtual float StartMaxHealth()
	{
		return this.StartHealth();
	}

	public void UpdateSurroundings()
	{
		StabilityEntity.updateSurroundingsQueue.Add(base.WorldSpaceBounds().ToBounds());
	}

	public enum LifeState
	{
		Alive,
		Dead
	}

	[Serializable]
	public struct Pickup
	{
		public bool enabled;

		[ItemSelector(ItemCategory.All)]
		public ItemDefinition itemTarget;

		public int itemCount;

		[Tooltip("Should we set the condition of the item based on the health of the picked up entity")]
		public bool setConditionFromHealth;

		[Tooltip("How much to reduce the item condition when picking up")]
		public float subtractCondition;

		[Tooltip("Must have building access to pick up")]
		public bool requireBuildingPrivilege;

		[Tooltip("Must have hammer equipped to pick up")]
		public bool requireHammer;

		[Tooltip("Inventory Must be empty (if applicable) to be picked up")]
		public bool requireEmptyInv;
	}

	[Serializable]
	public struct Repair
	{
		public bool enabled;

		[ItemSelector(ItemCategory.All)]
		public ItemDefinition itemTarget;
	}
}