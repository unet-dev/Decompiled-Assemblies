using ConVar;
using Facepunch;
using Rust;
using Rust.Ai;
using Rust.AI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HumanNPC : NPCPlayer, IThinker
{
	[Header("Loot")]
	public LootContainer.LootSpawnSlot[] LootSpawnSlots;

	public HumanNPC.SpeedType desiredSpeed = HumanNPC.SpeedType.SlowWalk;

	[Header("Detection")]
	public float sightRange = 30f;

	public float sightRangeLarge = 200f;

	public float visionCone = -0.8f;

	[Header("Damage")]
	public float aimConeScale = 2f;

	private List<BaseCombatEntity> _targets = new List<BaseCombatEntity>();

	public BaseCombatEntity currentTarget;

	private HumanBrain _brain;

	public float lastDismountTime;

	private bool navmeshEnabled;

	private const float TargetUpdateRate = 0.5f;

	private const float TickItemRate = 0.1f;

	private float nextZoneSearchTime;

	private AIInformationZone cachedInfoZone;

	public bool currentTargetLOS;

	public BaseEntity[] QueryResults = new BaseEntity[64];

	private SimpleAIMemory myMemory = new SimpleAIMemory();

	public float memoryDuration = 10f;

	private bool pendingDucked;

	private float timeSinceItemTick = 0.1f;

	private float timeSinceTargetUpdate = 0.5f;

	private float targetAimedDuration;

	private Vector3 aimOverridePosition = Vector3.zero;

	public HumanNPC()
	{
	}

	private static bool AiCaresAbout(BaseEntity ent)
	{
		if (ent is BasePlayer)
		{
			return true;
		}
		return false;
	}

	public Vector3 AimOffset(BaseCombatEntity aimat)
	{
		BasePlayer basePlayer = aimat as BasePlayer;
		if (basePlayer == null)
		{
			return aimat.CenterPoint();
		}
		if (basePlayer.IsSleeping())
		{
			return basePlayer.transform.position + (Vector3.up * 0.1f);
		}
		return basePlayer.eyes.position - (Vector3.up * 0.05f);
	}

	public void ApplyPendingDucked()
	{
		if (this.pendingDucked)
		{
			this.SetDesiredSpeed(HumanNPC.SpeedType.Crouch);
		}
		this.modelState.ducked = this.pendingDucked;
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public bool CanSeeTarget()
	{
		if (!this.HasTarget())
		{
			return false;
		}
		return this.currentTargetLOS;
	}

	public void ClearStationaryAimPoint()
	{
		this.aimOverridePosition = Vector3.zero;
	}

	public override BaseCorpse CreateCorpse()
	{
		int i;
		BaseCorpse baseCorpse;
		using (TimeWarning timeWarning = TimeWarning.New("Create corpse", 0.1f))
		{
			NPCPlayerCorpse navAgent = base.DropCorpse("assets/prefabs/npc/scientist/scientist_corpse.prefab") as NPCPlayerCorpse;
			if (navAgent)
			{
				navAgent.transform.position = navAgent.transform.position + (Vector3.down * this.NavAgent.baseOffset);
				navAgent.SetLootableIn(2f);
				navAgent.SetFlag(BaseEntity.Flags.Reserved5, base.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash), false, true);
				navAgent.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
				navAgent.TakeFrom(new ItemContainer[] { this.inventory.containerMain, this.inventory.containerWear, this.inventory.containerBelt });
				navAgent.playerName = base.displayName;
				navAgent.playerSteamID = this.userID;
				navAgent.Spawn();
				navAgent.TakeChildren(this);
				ItemContainer[] itemContainerArray = navAgent.containers;
				for (i = 0; i < (int)itemContainerArray.Length; i++)
				{
					itemContainerArray[i].Clear();
				}
				if (this.LootSpawnSlots.Length != 0)
				{
					LootContainer.LootSpawnSlot[] lootSpawnSlots = this.LootSpawnSlots;
					for (i = 0; i < (int)lootSpawnSlots.Length; i++)
					{
						LootContainer.LootSpawnSlot lootSpawnSlot = lootSpawnSlots[i];
						for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
						{
							if (UnityEngine.Random.Range(0f, 1f) <= lootSpawnSlot.probability)
							{
								lootSpawnSlot.definition.SpawnIntoContainer(navAgent.containers[0]);
							}
						}
					}
				}
			}
			baseCorpse = navAgent;
		}
		return baseCorpse;
	}

	public override float DesiredMoveSpeed()
	{
		return this.SpeedFromEnum(this.desiredSpeed);
	}

	public override void DismountObject()
	{
		base.DismountObject();
		this.lastDismountTime = UnityEngine.Time.time;
	}

	public float DistanceToTarget()
	{
		if (this.currentTarget == null)
		{
			return -1f;
		}
		return Vector3.Distance(this.ServerPosition, this.currentTarget.ServerPosition);
	}

	internal override void DoServerDestroy()
	{
		AIThinkManager.Remove(this);
		base.DoServerDestroy();
	}

	public void EnableNavAgent()
	{
		if (!base.isMounted)
		{
			this.SetNavMeshEnabled(true);
		}
	}

	public float EngagementRange()
	{
		AttackEntity heldEntity = base.GetHeldEntity() as AttackEntity;
		if (heldEntity)
		{
			return heldEntity.effectiveRange;
		}
		return this.sightRange;
	}

	public override void EquipWeapon()
	{
		base.EquipWeapon();
	}

	public override float GetAimConeScale()
	{
		return this.aimConeScale;
	}

	public override Vector3 GetAimDirection()
	{
		bool flag = this.currentTarget != null;
		bool flag1 = (!flag ? false : this.currentTargetLOS);
		bool flag2 = Vector3Ex.Distance2D(this.finalDestination, base.GetPosition()) > 0.5f;
		Vector3 navAgent = this.NavAgent.desiredVelocity;
		navAgent.y = 0f;
		navAgent.Normalize();
		if (!flag)
		{
			if (flag2)
			{
				return navAgent;
			}
			return Vector3.zero;
		}
		if (flag1 && this.desiredSpeed != HumanNPC.SpeedType.Sprint)
		{
			Vector3 vector3 = this.AimOffset(this.currentTarget) - this.eyes.position;
			return vector3.normalized;
		}
		if (flag2)
		{
			return navAgent;
		}
		if (this.aimOverridePosition != Vector3.zero)
		{
			return Vector3Ex.Direction2D(this.aimOverridePosition, this.ServerPosition);
		}
		return Vector3Ex.Direction2D(this.ServerPosition + (this.eyes.BodyForward() * 1000f), this.ServerPosition);
	}

	public float GetAimSwayScalar()
	{
		return 1f - Mathf.InverseLerp(1f, 3f, UnityEngine.Time.time - this.lastGunShotTime);
	}

	public AIMovePoint GetBestRoamPosition(Vector3 start)
	{
		AIInformationZone informationZone = this.GetInformationZone();
		if (informationZone == null)
		{
			return null;
		}
		float single = -1f;
		AIMovePoint aIMovePoint = null;
		foreach (AIMovePoint movePoint in informationZone.movePoints)
		{
			if (!movePoint.transform.parent.gameObject.activeSelf)
			{
				continue;
			}
			float single1 = 0f;
			float single2 = Vector3.Dot(this.eyes.BodyForward(), Vector3Ex.Direction2D(movePoint.transform.position, this.eyes.position));
			single1 = single1 + Mathf.InverseLerp(-1f, 1f, single2) * 100f;
			float single3 = Vector3.Distance(this.ServerPosition, movePoint.transform.position);
			if (movePoint.IsUsedForRoaming())
			{
				continue;
			}
			float single4 = Mathf.Abs(this.ServerPosition.y - movePoint.transform.position.y);
			single1 = single1 + (1f - Mathf.InverseLerp(1f, 10f, single4)) * 100f;
			if (single4 > 5f)
			{
				continue;
			}
			if (single3 > 5f)
			{
				single1 = single1 + (1f - Mathf.InverseLerp(5f, 20f, single3)) * 50f;
			}
			if (single1 <= single)
			{
				continue;
			}
			aIMovePoint = movePoint;
			single = single1;
		}
		return aIMovePoint;
	}

	public virtual float GetIdealDistanceFromTarget()
	{
		return Mathf.Max(5f, this.EngagementRange() * 0.75f);
	}

	public Vector3 GetIdealPositionNear(Vector3 position, float maxDistFrom)
	{
		if (position == base.transform.position)
		{
			Vector2 vector2 = UnityEngine.Random.insideUnitCircle * maxDistFrom;
			return position + new Vector3(vector2.x, 0f, vector2.y);
		}
		Vector3 vector3 = base.transform.position - position;
		return position + (vector3.normalized * maxDistFrom);
	}

	public AIInformationZone GetInformationZone()
	{
		if (this.cachedInfoZone == null || UnityEngine.Time.time > this.nextZoneSearchTime)
		{
			this.cachedInfoZone = AIInformationZone.GetForPoint(this.ServerPosition, this);
			this.nextZoneSearchTime = UnityEngine.Time.time + 5f;
		}
		return this.cachedInfoZone;
	}

	public Vector3 GetRandomPositionAround(Vector3 position, float minDistFrom = 0f, float maxDistFrom = 2f)
	{
		if (maxDistFrom < 0f)
		{
			maxDistFrom = 0f;
		}
		Vector2 vector2 = UnityEngine.Random.insideUnitCircle * maxDistFrom;
		float single = Mathf.Clamp(Mathf.Max(Mathf.Abs(vector2.x), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign(vector2.x);
		float single1 = Mathf.Clamp(Mathf.Max(Mathf.Abs(vector2.y), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign(vector2.y);
		return position + new Vector3(single, 0f, single1);
	}

	public List<BaseCombatEntity> GetTargets()
	{
		if (this._targets == null)
		{
			this._targets = Facepunch.Pool.GetList<BaseCombatEntity>();
		}
		return this._targets;
	}

	public AITraversalArea GetTraversalArea()
	{
		AITraversalArea aITraversalArea;
		if (this.triggers == null)
		{
			return null;
		}
		List<TriggerBase>.Enumerator enumerator = this.triggers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				AITraversalArea component = enumerator.Current.GetComponent<AITraversalArea>();
				if (component == null)
				{
					continue;
				}
				aITraversalArea = component;
				return aITraversalArea;
			}
			return null;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return aITraversalArea;
	}

	public bool HasAnyTargets()
	{
		return this.GetTargets().Count > 0;
	}

	public bool HasTarget()
	{
		return this.currentTarget != null;
	}

	public override void Hurt(HitInfo info)
	{
		if (base.isMounted)
		{
			info.damageTypes.ScaleAll(0.1f);
		}
		base.Hurt(info);
		BaseEntity initiator = info.Initiator;
		if (initiator != null && !initiator.EqualNetID(this))
		{
			this.myMemory.Update(initiator);
		}
	}

	public bool IsInTraversalArea()
	{
		bool flag;
		if (this.triggers == null)
		{
			return false;
		}
		List<TriggerBase>.Enumerator enumerator = this.triggers.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.GetComponent<AITraversalArea>())
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

	public override bool IsLoadBalanced()
	{
		return true;
	}

	public override bool IsNavRunning()
	{
		if (base.isMounted)
		{
			return false;
		}
		return this.navmeshEnabled;
	}

	public bool IsVisibleCrouched(BasePlayer player)
	{
		Vector3 vector3 = this.eyes.worldCrouchedPosition;
		if (!player.IsVisible(vector3, player.CenterPoint(), Single.PositiveInfinity) && !player.IsVisible(vector3, player.transform.position, Single.PositiveInfinity) && !player.IsVisible(vector3, player.eyes.position, Single.PositiveInfinity))
		{
			return false;
		}
		if (!base.IsVisible(player.CenterPoint(), vector3, Single.PositiveInfinity) && !base.IsVisible(player.transform.position, vector3, Single.PositiveInfinity) && !base.IsVisible(player.eyes.position, vector3, Single.PositiveInfinity))
		{
			return false;
		}
		return true;
	}

	public bool IsVisibleMounted(BasePlayer player)
	{
		Vector3 vector3 = this.eyes.worldMountedPosition;
		if (!player.IsVisible(vector3, player.CenterPoint(), Single.PositiveInfinity) && !player.IsVisible(vector3, player.transform.position, Single.PositiveInfinity) && !player.IsVisible(vector3, player.eyes.position, Single.PositiveInfinity))
		{
			return false;
		}
		if (!base.IsVisible(player.CenterPoint(), vector3, Single.PositiveInfinity) && !base.IsVisible(player.transform.position, vector3, Single.PositiveInfinity) && !base.IsVisible(player.eyes.position, vector3, Single.PositiveInfinity))
		{
			return false;
		}
		return true;
	}

	public bool IsVisibleStanding(BasePlayer player)
	{
		Vector3 vector3 = this.eyes.worldStandingPosition;
		if (!player.IsVisible(vector3, player.CenterPoint(), Single.PositiveInfinity) && !player.IsVisible(vector3, player.transform.position, Single.PositiveInfinity) && !player.IsVisible(vector3, player.eyes.position, Single.PositiveInfinity))
		{
			return false;
		}
		if (!base.IsVisible(player.CenterPoint(), vector3, Single.PositiveInfinity) && !base.IsVisible(player.transform.position, vector3, Single.PositiveInfinity) && !base.IsVisible(player.eyes.position, vector3, Single.PositiveInfinity))
		{
			return false;
		}
		return true;
	}

	public bool IsVisibleToUs(BasePlayer player)
	{
		if (base.isMounted)
		{
			return this.IsVisibleMounted(player);
		}
		if (base.IsDucked())
		{
			return this.IsVisibleCrouched(player);
		}
		return this.IsVisibleStanding(player);
	}

	public void LightCheck()
	{
		if (TOD_Sky.Instance.IsNight && !this.lightsOn)
		{
			base.SetLightsOn(true);
			return;
		}
		if (this.lightsOn)
		{
			base.SetLightsOn(false);
		}
	}

	public void LogAttacker(BaseEntity attacker)
	{
	}

	public override float MaxHealth()
	{
		return this.startHealth;
	}

	public bool RecentlyDismounted()
	{
		return UnityEngine.Time.time < this.lastDismountTime + 10f;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this._brain = base.GetComponent<HumanBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.Add(this);
		base.Invoke(new Action(this.EnableNavAgent), 0.25f);
	}

	public override void ServerThink(float delta)
	{
		base.ServerThink(delta);
		if (this._brain.ShouldThink())
		{
			this._brain.DoThink();
		}
		this.timeSinceItemTick += delta;
		this.timeSinceTargetUpdate += delta;
		if (this.timeSinceItemTick > 0.1f)
		{
			this.TickItems(this.timeSinceItemTick);
			this.timeSinceItemTick = 0f;
		}
		if (this.timeSinceTargetUpdate > 0.5f)
		{
			this.UpdateTargets(this.timeSinceTargetUpdate);
			this.timeSinceTargetUpdate = 0f;
		}
	}

	public override void SetAimDirection(Vector3 newAim)
	{
		Quaternion quaternion;
		if (newAim == Vector3.zero)
		{
			return;
		}
		AttackEntity attackEntity = base.GetAttackEntity();
		if (attackEntity)
		{
			newAim = attackEntity.ModifyAIAim(newAim, this.GetAimSwayScalar());
		}
		if (base.isMounted)
		{
			BaseMountable mounted = base.GetMounted();
			Vector3 vector3 = mounted.transform.eulerAngles;
			quaternion = Quaternion.LookRotation(newAim, mounted.transform.up);
			Quaternion quaternion1 = Quaternion.Euler(quaternion.eulerAngles);
			Quaternion quaternion2 = Quaternion.LookRotation(base.transform.InverseTransformDirection(quaternion1 * Vector3.forward), base.transform.up);
			Vector3 vector31 = quaternion2.eulerAngles;
			vector31 = BaseMountable.ConvertVector(vector31);
			Quaternion quaternion3 = Quaternion.Euler(Mathf.Clamp(vector31.x, mounted.pitchClamp.x, mounted.pitchClamp.y), Mathf.Clamp(vector31.y, mounted.yawClamp.x, mounted.yawClamp.y), vector3.z);
			Quaternion quaternion4 = Quaternion.LookRotation(base.transform.TransformDirection(quaternion3 * Vector3.forward), base.transform.up);
			newAim = BaseMountable.ConvertVector(quaternion4.eulerAngles);
		}
		this.eyes.rotation = (base.isMounted ? Quaternion.Slerp(this.eyes.rotation, Quaternion.Euler(newAim), UnityEngine.Time.smoothDeltaTime * 70f) : Quaternion.Lerp(this.eyes.rotation, Quaternion.LookRotation(newAim, base.transform.up), UnityEngine.Time.deltaTime * 25f));
		quaternion = this.eyes.rotation;
		this.viewAngles = quaternion.eulerAngles;
		this.ServerRotation = this.eyes.rotation;
	}

	public void SetDesiredSpeed(HumanNPC.SpeedType newSpeed)
	{
		if (newSpeed == this.desiredSpeed)
		{
			return;
		}
		this.desiredSpeed = newSpeed;
	}

	public override void SetDestination(Vector3 newDestination)
	{
		if (this.IsNavRunning())
		{
			base.SetDestination(newDestination);
			this.NavAgent.SetDestination(newDestination);
		}
	}

	public void SetDucked(bool wantsDucked)
	{
		this.pendingDucked = wantsDucked;
		this.ApplyPendingDucked();
	}

	public void SetNavMeshEnabled(bool on)
	{
		NavMeshHit navMeshHit;
		if (this.NavAgent.enabled == on)
		{
			return;
		}
		if (AiManager.nav_disable)
		{
			this.NavAgent.enabled = false;
			this.navmeshEnabled = false;
			return;
		}
		this.NavAgent.agentTypeID = this.NavAgent.agentTypeID;
		if (on)
		{
			if (!NavMesh.SamplePosition(this.ServerPosition + (Vector3.up * 1f), out navMeshHit, 5f, -1))
			{
				Debug.Log("Failed to sample navmesh");
			}
			else
			{
				this.NavAgent.Warp(navMeshHit.position);
				this.NavAgent.enabled = true;
				this.ServerPosition = navMeshHit.position;
			}
		}
		this.navmeshEnabled = on;
		if (!on)
		{
			this.NavAgent.isStopped = true;
			this.NavAgent.enabled = false;
			return;
		}
		this.NavAgent.enabled = true;
		this.NavAgent.isStopped = false;
		this.SetDestination(this.ServerPosition);
	}

	public void SetStationaryAimPoint(Vector3 aimAt)
	{
		this.aimOverridePosition = aimAt;
	}

	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	public float SpeedFromEnum(HumanNPC.SpeedType newSpeed)
	{
		switch (newSpeed)
		{
			case HumanNPC.SpeedType.Crouch:
			{
				return 0.8f;
			}
			case HumanNPC.SpeedType.SlowWalk:
			{
				return 1.5f;
			}
			case HumanNPC.SpeedType.Walk:
			{
				return 2.5f;
			}
			case HumanNPC.SpeedType.Sprint:
			{
				return 5f;
			}
		}
		return 0f;
	}

	public override float StartHealth()
	{
		return this.startHealth;
	}

	public override float StartMaxHealth()
	{
		return this.startHealth;
	}

	public void Stop()
	{
		if (this.IsNavRunning())
		{
			this.NavAgent.SetDestination(this.ServerPosition);
		}
	}

	public bool TargetInRange()
	{
		if (!this.HasTarget())
		{
			return false;
		}
		return this.DistanceToTarget() <= this.EngagementRange();
	}

	public void TickItems(float delta)
	{
		if (this.desiredSpeed == HumanNPC.SpeedType.Sprint || this.currentTarget == null)
		{
			this.targetAimedDuration = 0f;
			base.CancelBurst(0f);
			return;
		}
		if (!this.currentTargetLOS)
		{
			this.targetAimedDuration = 0f;
		}
		else if (Vector3.Dot(this.eyes.BodyForward(), this.currentTarget.CenterPoint() - this.eyes.position) > 0.8f)
		{
			this.targetAimedDuration += delta;
		}
		if (this.targetAimedDuration <= 0.2f)
		{
			base.CancelBurst(0.2f);
		}
		else
		{
			AttackEntity attackEntity = base.GetAttackEntity();
			if (attackEntity)
			{
				if (this.DistanceToTarget() < attackEntity.effectiveRange * (attackEntity.aiOnlyInRange ? 1f : 2f))
				{
					this.ShotTest();
					return;
				}
			}
		}
	}

	public virtual void TryThink()
	{
		base.ServerThink_Internal();
	}

	public void UpdateMemory()
	{
		int inSphere = BaseEntity.Query.Server.GetInSphere(this.ServerPosition, this.sightRange, this.QueryResults, new Func<BaseEntity, bool>(HumanNPC.AiCaresAbout));
		for (int i = 0; i < inSphere; i++)
		{
			BaseEntity queryResults = this.QueryResults[i];
			if (!(queryResults == null) && !queryResults.EqualNetID(this) && queryResults.isServer && this.WithinVisionCone(queryResults))
			{
				BasePlayer basePlayer = queryResults as BasePlayer;
				if (!(basePlayer != null) || queryResults.IsNpc || !AI.ignoreplayers && this.IsVisibleToUs(basePlayer))
				{
					this.myMemory.Update(queryResults);
				}
			}
		}
		this.myMemory.Forget(this.memoryDuration);
	}

	public void UpdateTargets(float delta)
	{
		this.UpdateMemory();
		int num = -1;
		float single = -1f;
		Vector3 serverPosition = this.ServerPosition;
		for (int i = 0; i < this.myMemory.All.Count; i++)
		{
			SimpleAIMemory.SeenInfo item = this.myMemory.All[i];
			if (item.Entity != null)
			{
				float single1 = 0f;
				float single2 = Vector3.Distance(item.Entity.ServerPosition, serverPosition);
				if (!item.Entity.IsNpc && item.Entity.Health() > 0f)
				{
					single1 = single1 + (1f - Mathf.InverseLerp(10f, this.sightRange, single2));
					Vector3 vector3 = item.Entity.ServerPosition - this.eyes.position;
					float single3 = Vector3.Dot(vector3.normalized, this.eyes.BodyForward());
					single1 += Mathf.InverseLerp(this.visionCone, 1f, single3);
					float timestamp = item.Timestamp - UnityEngine.Time.realtimeSinceStartup;
					single1 = single1 + (1f - Mathf.InverseLerp(0f, 3f, timestamp));
					if (single1 > single)
					{
						num = i;
						single = single1;
					}
				}
			}
		}
		if (num == -1)
		{
			this.currentTarget = null;
			this.currentTargetLOS = false;
		}
		else
		{
			SimpleAIMemory.SeenInfo seenInfo = this.myMemory.All[num];
			if (seenInfo.Entity != null && seenInfo.Entity is BasePlayer)
			{
				BasePlayer component = seenInfo.Entity.GetComponent<BasePlayer>();
				this.currentTarget = component;
				this.currentTargetLOS = this.IsVisibleToUs(component);
				return;
			}
		}
	}

	public bool WithinVisionCone(BaseEntity other)
	{
		Vector3 vector3 = Vector3Ex.Direction(other.transform.position, base.transform.position);
		if (Vector3.Dot(this.eyes.BodyForward(), vector3) < this.visionCone)
		{
			return false;
		}
		return true;
	}

	public enum SpeedType
	{
		Crouch = 1,
		SlowWalk = 2,
		Walk = 3,
		Sprint = 4
	}
}