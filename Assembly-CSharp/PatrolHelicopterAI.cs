using ConVar;
using Oxide.Core;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PatrolHelicopterAI : BaseMonoBehaviour
{
	public Vector3 interestZoneOrigin;

	public Vector3 destination;

	public bool hasInterestZone;

	public float moveSpeed;

	public float maxSpeed = 25f;

	public float courseAdjustLerpTime = 2f;

	public Quaternion targetRotation;

	public Vector3 windVec;

	public Vector3 targetWindVec;

	public float windForce = 5f;

	public float windFrequency = 1f;

	public float targetThrottleSpeed;

	public float throttleSpeed;

	public float maxRotationSpeed = 90f;

	public float rotationSpeed;

	public float terrainPushForce = 100f;

	public float obstaclePushForce = 100f;

	public HelicopterTurret leftGun;

	public HelicopterTurret rightGun;

	public static PatrolHelicopterAI heliInstance;

	public BaseHelicopter helicopterBase;

	public PatrolHelicopterAI.aiState _currentState;

	private Vector3 _aimTarget;

	private bool movementLockingAiming;

	private bool hasAimTarget;

	private bool aimDoorSide;

	private Vector3 pushVec = Vector3.zero;

	private Vector3 _lastPos;

	private Vector3 _lastMoveDir;

	public bool isDead;

	private bool isRetiring;

	public float spawnTime;

	public float lastDamageTime;

	public List<PatrolHelicopterAI.targetinfo> _targetList = new List<PatrolHelicopterAI.targetinfo>();

	private float deathTimeout;

	private float destination_min_dist = 2f;

	private float currentOrbitDistance;

	private float currentOrbitTime;

	private bool hasEnteredOrbit;

	private float orbitStartTime;

	private float maxOrbitDuration = 30f;

	private bool breakingOrbit;

	public List<MonumentInfo> _visitedMonuments;

	public float arrivalTime;

	public GameObjectRef rocketProjectile;

	public GameObjectRef rocketProjectile_Napalm;

	public bool leftTubeFiredLast;

	public float lastRocketTime;

	public float timeBetweenRockets = 0.2f;

	public int numRocketsLeft = 12;

	public const int maxRockets = 12;

	public Vector3 strafe_target_position;

	private bool puttingDistance;

	public const float strafe_approach_range = 175f;

	public const float strafe_firing_range = 150f;

	private bool useNapalm;

	[NonSerialized]
	public float lastNapalmTime = Single.NegativeInfinity;

	[NonSerialized]
	public float lastStrafeTime = Single.NegativeInfinity;

	private float _lastThinkTime;

	public PatrolHelicopterAI()
	{
	}

	public void AIThink()
	{
		float time = this.GetTime();
		float single = time - this._lastThinkTime;
		this._lastThinkTime = time;
		switch (this._currentState)
		{
			case PatrolHelicopterAI.aiState.MOVE:
			{
				this.State_Move_Think(single);
				return;
			}
			case PatrolHelicopterAI.aiState.ORBIT:
			{
				this.State_Orbit_Think(single);
				return;
			}
			case PatrolHelicopterAI.aiState.STRAFE:
			{
				this.State_Strafe_Think(single);
				return;
			}
			case PatrolHelicopterAI.aiState.PATROL:
			{
				this.State_Patrol_Think(single);
				return;
			}
			case PatrolHelicopterAI.aiState.DEATH:
			{
				this.State_Death_Think(single);
				return;
			}
			default:
			{
				this.State_Idle_Think(single);
				return;
			}
		}
	}

	public bool AtDestination()
	{
		return Vector3.Distance(base.transform.position, this.destination) < this.destination_min_dist;
	}

	public void Awake()
	{
		if (PatrolHelicopter.lifetimeMinutes == 0f)
		{
			base.Invoke(new Action(this.DestroyMe), 1f);
			return;
		}
		base.InvokeRepeating(new Action(this.UpdateWind), 0f, 1f / this.windFrequency);
		this._lastPos = base.transform.position;
		this.spawnTime = UnityEngine.Time.realtimeSinceStartup;
		this.InitializeAI();
	}

	public bool CanInterruptState()
	{
		if (this._currentState == PatrolHelicopterAI.aiState.STRAFE)
		{
			return false;
		}
		return this._currentState != PatrolHelicopterAI.aiState.DEATH;
	}

	public bool CanStrafe()
	{
		object obj = Interface.CallHook("CanHelicopterStrafe", this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		if (UnityEngine.Time.realtimeSinceStartup - this.lastStrafeTime < 20f)
		{
			return false;
		}
		return this.CanInterruptState();
	}

	public bool CanUseNapalm()
	{
		object obj = Interface.CallHook("CanHelicopterUseNapalm", this);
		if (obj as bool)
		{
			return (bool)obj;
		}
		return UnityEngine.Time.realtimeSinceStartup - this.lastNapalmTime >= 30f;
	}

	public void ClearAimTarget()
	{
		this.hasAimTarget = false;
		this._aimTarget = Vector3.zero;
	}

	public int ClipRocketsLeft()
	{
		return this.numRocketsLeft;
	}

	public void CriticalDamage()
	{
		this.isDead = true;
		this.ExitCurrentState();
		this.State_Death_Enter();
	}

	public void DestroyMe()
	{
		this.helicopterBase.Kill(BaseNetworkable.DestroyMode.None);
	}

	public void DoMachineGuns()
	{
		if (this._targetList.Count > 0)
		{
			if (this.leftGun.NeedsNewTarget())
			{
				this.leftGun.UpdateTargetFromList(this._targetList);
			}
			if (this.rightGun.NeedsNewTarget())
			{
				this.rightGun.UpdateTargetFromList(this._targetList);
			}
		}
		this.leftGun.TurretThink();
		this.rightGun.TurretThink();
	}

	public void ExitCurrentState()
	{
		this.OnCurrentStateExit();
		this._currentState = PatrolHelicopterAI.aiState.IDLE;
	}

	public void FireGun(Vector3 targetPos, float aimCone, bool left)
	{
		RaycastHit raycastHit;
		if (PatrolHelicopter.guns == 0)
		{
			return;
		}
		Vector3 vector3 = ((left ? this.helicopterBase.left_gun_muzzle.transform : this.helicopterBase.right_gun_muzzle.transform)).position;
		Vector3 vector31 = (targetPos - vector3).normalized;
		vector3 = vector3 + (vector31 * 2f);
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, vector31, true);
		if (!GamePhysics.Trace(new Ray(vector3, modifiedAimConeDirection), 0f, out raycastHit, 300f, 1219701521, QueryTriggerInteraction.UseGlobal))
		{
			targetPos = vector3 + (modifiedAimConeDirection * 300f);
		}
		else
		{
			targetPos = raycastHit.point;
			if (raycastHit.collider)
			{
				BaseEntity entity = raycastHit.GetEntity();
				if (entity && entity != this.helicopterBase)
				{
					BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
					HitInfo hitInfo = new HitInfo(this.helicopterBase, entity, DamageType.Bullet, this.helicopterBase.bulletDamage * PatrolHelicopter.bulletDamageScale, raycastHit.point);
					if (!baseCombatEntity)
					{
						entity.OnAttacked(hitInfo);
					}
					else
					{
						baseCombatEntity.OnAttacked(hitInfo);
						if (baseCombatEntity is BasePlayer)
						{
							Effect.server.ImpactEffect(new HitInfo()
							{
								HitPositionWorld = raycastHit.point - (modifiedAimConeDirection * 0.25f),
								HitNormalWorld = -modifiedAimConeDirection,
								HitMaterial = StringPool.Get("Flesh")
							});
						}
					}
				}
			}
		}
		this.helicopterBase.ClientRPC<bool, Vector3>(null, "FireGun", left, targetPos);
	}

	public void FireRocket()
	{
		RaycastHit raycastHit;
		string str;
		this.numRocketsLeft--;
		this.lastRocketTime = UnityEngine.Time.realtimeSinceStartup;
		float single = 4f;
		bool flag = this.leftTubeFiredLast;
		this.leftTubeFiredLast = !this.leftTubeFiredLast;
		Transform transforms = (flag ? this.helicopterBase.rocket_tube_left.transform : this.helicopterBase.rocket_tube_right.transform);
		Vector3 vector3 = transforms.position + (transforms.forward * 1f);
		Vector3 strafeTargetPosition = (this.strafe_target_position - vector3).normalized;
		if (single > 0f)
		{
			strafeTargetPosition = AimConeUtil.GetModifiedAimConeDirection(single, strafeTargetPosition, true);
		}
		if (UnityEngine.Physics.Raycast(vector3, strafeTargetPosition, out raycastHit, 1f, 1236478737))
		{
		}
		Effect.server.Run(this.helicopterBase.rocket_fire_effect.resourcePath, this.helicopterBase, StringPool.Get((flag ? "rocket_tube_left" : "rocket_tube_right")), Vector3.zero, Vector3.forward, null, true);
		GameManager gameManager = GameManager.server;
		str = (this.useNapalm ? this.rocketProjectile_Napalm.resourcePath : this.rocketProjectile.resourcePath);
		Quaternion quaternion = new Quaternion();
		BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
		if (baseEntity == null)
		{
			return;
		}
		ServerProjectile component = baseEntity.GetComponent<ServerProjectile>();
		if (component)
		{
			component.InitializeVelocity(strafeTargetPosition * component.speed);
		}
		baseEntity.Spawn();
	}

	public Vector3 GetAppropriatePosition(Vector3 origin, float minHeight = 20f, float maxHeight = 30f)
	{
		RaycastHit raycastHit;
		float single = 100f;
		Ray ray = new Ray(origin + new Vector3(0f, single, 0f), Vector3.down);
		float single1 = 5f;
		if (UnityEngine.Physics.SphereCast(ray, single1, out raycastHit, single * 2f - single1, LayerMask.GetMask(new string[] { "Terrain", "World", "Construction", "Water" })))
		{
			origin = raycastHit.point;
		}
		origin.y += UnityEngine.Random.Range(minHeight, maxHeight);
		return origin;
	}

	public Vector3 GetLastMoveDir()
	{
		return this._lastMoveDir;
	}

	public float GetMaxRotationSpeed()
	{
		return this.maxRotationSpeed;
	}

	public Vector3 GetMoveDirection()
	{
		return (this.destination - base.transform.position).normalized;
	}

	public float GetMoveSpeed()
	{
		return this.moveSpeed;
	}

	public Vector3 GetOrbitPosition(float rate)
	{
		float single = Mathf.Sin(6.28318548f * rate) * this.currentOrbitDistance;
		float single1 = Mathf.Cos(6.28318548f * rate) * this.currentOrbitDistance;
		Vector3 vector3 = new Vector3(single, 20f, single1);
		vector3 = this.interestZoneOrigin + vector3;
		return vector3;
	}

	public Vector3 GetRandomOffset(Vector3 origin, float minRange, float maxRange = 0f, float minHeight = 20f, float maxHeight = 30f)
	{
		Vector3 vector3 = UnityEngine.Random.onUnitSphere;
		vector3.y = 0f;
		vector3.Normalize();
		maxRange = Mathf.Max(minRange, maxRange);
		Vector3 vector31 = origin + (vector3 * UnityEngine.Random.Range(minRange, maxRange));
		return this.GetAppropriatePosition(vector31, minHeight, maxHeight);
	}

	public Vector3 GetRandomPatrolDestination()
	{
		RaycastHit raycastHit;
		Vector3 height = Vector3.zero;
		if (!(TerrainMeta.Path != null) || TerrainMeta.Path.Monuments == null || TerrainMeta.Path.Monuments.Count <= 0)
		{
			float size = TerrainMeta.Size.x;
			float single = 30f;
			height = Vector3Ex.Range(-1f, 1f);
			height.y = 0f;
			height.Normalize();
			height = height * (size * UnityEngine.Random.Range(0f, 0.75f));
			height.y = single;
		}
		else
		{
			MonumentInfo item = null;
			if (this._visitedMonuments.Count > 0)
			{
				foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
				{
					bool flag = false;
					foreach (MonumentInfo _visitedMonument in this._visitedMonuments)
					{
						if (monument != _visitedMonument)
						{
							continue;
						}
						flag = true;
					}
					if (flag)
					{
						continue;
					}
					item = monument;
					goto Label0;
				}
			}
		Label0:
			if (item == null)
			{
				this._visitedMonuments.Clear();
				item = TerrainMeta.Path.Monuments[UnityEngine.Random.Range(0, TerrainMeta.Path.Monuments.Count)];
			}
			if (item)
			{
				height = item.transform.position;
				this._visitedMonuments.Add(item);
				height.y = TerrainMeta.HeightMap.GetHeight(height) + 200f;
				if (TransformUtil.GetGroundInfo(height, out raycastHit, 300f, 1235288065, null))
				{
					height.y = raycastHit.point.y;
				}
				height.y += 30f;
			}
		}
		return height;
	}

	public float GetThrottleForDistance(float distToTarget)
	{
		float single = 0f;
		if (distToTarget >= 75f)
		{
			single = 1f;
		}
		else if (distToTarget >= 50f)
		{
			single = 0.75f;
		}
		else if (distToTarget < 25f)
		{
			single = (distToTarget < 5f ? 0.05f * (1f - distToTarget / 5f) : 0.05f);
		}
		else
		{
			single = 0.33f;
		}
		return single;
	}

	public float GetTime()
	{
		return UnityEngine.Time.realtimeSinceStartup;
	}

	public Quaternion GetYawRotationTo(Vector3 targetDest)
	{
		Vector3 vector3 = targetDest;
		vector3.y = 0f;
		Vector3 vector31 = base.transform.position;
		vector31.y = 0f;
		return Quaternion.LookRotation((vector3 - vector31).normalized);
	}

	public void InitializeAI()
	{
		this._lastThinkTime = UnityEngine.Time.realtimeSinceStartup;
	}

	public bool IsAlive()
	{
		return !this.isDead;
	}

	public bool IsTargeting()
	{
		return this.hasAimTarget;
	}

	public void MoveToDestination()
	{
		Vector3 vector3 = this._lastMoveDir;
		Vector3 vector31 = this.destination - base.transform.position;
		Vector3 vector32 = Vector3.Lerp(vector3, vector31.normalized, UnityEngine.Time.deltaTime / this.courseAdjustLerpTime);
		this._lastMoveDir = vector32;
		this.throttleSpeed = Mathf.Lerp(this.throttleSpeed, this.targetThrottleSpeed, UnityEngine.Time.deltaTime / 3f);
		float single = this.throttleSpeed * this.maxSpeed;
		this.TerrainPushback();
		Transform transforms = base.transform;
		transforms.position = transforms.position + ((vector32 * single) * UnityEngine.Time.deltaTime);
		this.windVec = Vector3.Lerp(this.windVec, this.targetWindVec, UnityEngine.Time.deltaTime);
		Transform transforms1 = base.transform;
		transforms1.position = transforms1.position + ((this.windVec * this.windForce) * UnityEngine.Time.deltaTime);
		this.moveSpeed = Mathf.Lerp(this.moveSpeed, Vector3.Distance(this._lastPos, base.transform.position) / UnityEngine.Time.deltaTime, UnityEngine.Time.deltaTime * 2f);
		this._lastPos = base.transform.position;
	}

	public void OnCurrentStateExit()
	{
		switch (this._currentState)
		{
			case PatrolHelicopterAI.aiState.MOVE:
			{
				this.State_Move_Leave();
				return;
			}
			case PatrolHelicopterAI.aiState.ORBIT:
			{
				this.State_Orbit_Leave();
				return;
			}
			case PatrolHelicopterAI.aiState.STRAFE:
			{
				this.State_Strafe_Leave();
				return;
			}
			case PatrolHelicopterAI.aiState.PATROL:
			{
				this.State_Patrol_Leave();
				return;
			}
			default:
			{
				this.State_Idle_Leave();
				return;
			}
		}
	}

	public bool PlayerVisible(BasePlayer ply)
	{
		RaycastHit raycastHit;
		object obj = Interface.CallHook("CanHelicopterTarget", this, ply);
		if (obj as bool)
		{
			return (bool)obj;
		}
		Vector3 vector3 = ply.eyes.position;
		if (TOD_Sky.Instance.IsNight && Vector3.Distance(vector3, this.interestZoneOrigin) > 40f)
		{
			return false;
		}
		Vector3 vector31 = base.transform.position - (Vector3.up * 6f);
		float single = Vector3.Distance(vector3, vector31);
		Vector3 vector32 = (vector3 - vector31).normalized;
		if (GamePhysics.Trace(new Ray(vector31 + (vector32 * 5f), vector32), 0f, out raycastHit, single * 1.1f, 1218652417, QueryTriggerInteraction.UseGlobal) && raycastHit.collider.gameObject.ToBaseEntity() == ply)
		{
			return true;
		}
		return false;
	}

	public void Retire()
	{
		if (this.isRetiring)
		{
			return;
		}
		this.isRetiring = true;
		base.Invoke(new Action(this.DestroyMe), 240f);
		float size = TerrainMeta.Size.x;
		float single = 200f;
		Vector3 vector3 = Vector3Ex.Range(-1f, 1f);
		vector3.y = 0f;
		vector3.Normalize();
		vector3 = vector3 * (size * 20f);
		vector3.y = single;
		this.ExitCurrentState();
		this.State_Move_Enter(vector3);
	}

	public void SetAimTarget(Vector3 aimTarg, bool isDoorSide)
	{
		if (this.movementLockingAiming)
		{
			return;
		}
		this.hasAimTarget = true;
		this._aimTarget = aimTarg;
		this.aimDoorSide = isDoorSide;
	}

	public void SetIdealRotation(Quaternion newTargetRot, float rotationSpeedOverride = -1f)
	{
		this.rotationSpeed = (rotationSpeedOverride == -1f ? Mathf.Clamp01(this.moveSpeed / (this.maxSpeed * 0.5f)) : rotationSpeedOverride) * this.maxRotationSpeed;
		this.targetRotation = newTargetRot;
	}

	public void SetInitialDestination(Vector3 dest, float mapScaleDistance = 0.25f)
	{
		this.hasInterestZone = true;
		this.interestZoneOrigin = dest;
		float size = TerrainMeta.Size.x;
		float single = dest.y + 25f;
		Vector3 vector3 = Vector3Ex.Range(-1f, 1f);
		vector3.y = 0f;
		vector3.Normalize();
		vector3 = vector3 * (size * mapScaleDistance);
		vector3.y = single;
		if (mapScaleDistance == 0f)
		{
			vector3 = this.interestZoneOrigin + new Vector3(0f, 10f, 0f);
		}
		base.transform.position = vector3;
		this.ExitCurrentState();
		this.State_Move_Enter(dest);
	}

	public void SetTargetDestination(Vector3 targetDest, float minDist = 5f, float minDistForFacingRotation = 30f)
	{
		this.destination = targetDest;
		this.destination_min_dist = minDist;
		float single = Vector3.Distance(targetDest, base.transform.position);
		if (single > minDistForFacingRotation && !this.IsTargeting())
		{
			this.SetIdealRotation(this.GetYawRotationTo(this.destination), -1f);
		}
		this.targetThrottleSpeed = this.GetThrottleForDistance(single);
	}

	public void State_Death_Enter()
	{
		Vector3 vector3;
		Vector3 vector31;
		this.maxRotationSpeed *= 8f;
		this._currentState = PatrolHelicopterAI.aiState.DEATH;
		int num = 1236478737;
		TransformUtil.GetGroundInfo(this.GetRandomOffset(base.transform.position, 20f, 60f, 20f, 30f) - (Vector3.up * 2f), out vector3, out vector31, 500f, num, null);
		this.SetTargetDestination(vector3, 5f, 30f);
		this.targetThrottleSpeed = 0.5f;
		this.deathTimeout = UnityEngine.Time.realtimeSinceStartup + 10f;
	}

	public void State_Death_Leave()
	{
	}

	public void State_Death_Think(float timePassed)
	{
		RaycastHit raycastHit;
		float single = UnityEngine.Time.realtimeSinceStartup * 0.25f;
		float single1 = Mathf.Sin(6.28318548f * single) * 10f;
		float single2 = Mathf.Cos(6.28318548f * single) * 10f;
		Vector3 vector3 = new Vector3(single1, 0f, single2);
		this.SetAimTarget(base.transform.position + vector3, true);
		Ray ray = new Ray(base.transform.position, this.GetLastMoveDir());
		if (UnityEngine.Physics.SphereCast(ray, 3f, out raycastHit, 5f, LayerMask.GetMask(new string[] { "Terrain", "World", "Construction", "Water" })) || UnityEngine.Time.realtimeSinceStartup > this.deathTimeout)
		{
			this.helicopterBase.Hurt(this.helicopterBase.health * 2f, DamageType.Generic, null, false);
		}
	}

	public void State_Idle_Enter()
	{
		this._currentState = PatrolHelicopterAI.aiState.IDLE;
	}

	public void State_Idle_Leave()
	{
	}

	public void State_Idle_Think(float timePassed)
	{
		this.ExitCurrentState();
		this.State_Patrol_Enter();
	}

	public void State_Move_Enter(Vector3 newPos)
	{
		this._currentState = PatrolHelicopterAI.aiState.MOVE;
		this.destination_min_dist = 5f;
		this.SetTargetDestination(newPos, 5f, 30f);
		float single = Vector3.Distance(base.transform.position, this.destination);
		this.targetThrottleSpeed = this.GetThrottleForDistance(single);
	}

	public void State_Move_Leave()
	{
	}

	public void State_Move_Think(float timePassed)
	{
		float single = Vector3.Distance(base.transform.position, this.destination);
		this.targetThrottleSpeed = this.GetThrottleForDistance(single);
		if (this.AtDestination())
		{
			this.ExitCurrentState();
			this.State_Idle_Enter();
		}
	}

	public void State_Orbit_Enter(float orbitDistance)
	{
		this._currentState = PatrolHelicopterAI.aiState.ORBIT;
		this.breakingOrbit = false;
		this.hasEnteredOrbit = false;
		this.orbitStartTime = UnityEngine.Time.realtimeSinceStartup;
		Vector3 vector3 = base.transform.position - this.interestZoneOrigin;
		this.currentOrbitTime = Mathf.Atan2(vector3.x, vector3.z);
		this.currentOrbitDistance = orbitDistance;
		this.ClearAimTarget();
		this.SetTargetDestination(this.GetOrbitPosition(this.currentOrbitTime), 20f, 0f);
	}

	public void State_Orbit_Leave()
	{
		this.breakingOrbit = false;
		this.hasEnteredOrbit = false;
		this.currentOrbitTime = 0f;
		this.ClearAimTarget();
	}

	public void State_Orbit_Think(float timePassed)
	{
		if (!this.breakingOrbit)
		{
			if (Vector3Ex.Distance2D(base.transform.position, this.destination) > 15f)
			{
				return;
			}
			if (!this.hasEnteredOrbit)
			{
				this.hasEnteredOrbit = true;
				this.orbitStartTime = UnityEngine.Time.realtimeSinceStartup;
			}
			float single = 6.28318548f * this.currentOrbitDistance / (0.5f * this.maxSpeed);
			this.currentOrbitTime = this.currentOrbitTime + timePassed / (single * 1.01f);
			Vector3 orbitPosition = this.GetOrbitPosition(this.currentOrbitTime);
			this.ClearAimTarget();
			this.SetTargetDestination(orbitPosition, 0f, 1f);
			this.targetThrottleSpeed = 0.5f;
		}
		else if (this.AtDestination())
		{
			this.ExitCurrentState();
			this.State_Idle_Enter();
		}
		if (UnityEngine.Time.realtimeSinceStartup - this.orbitStartTime > this.maxOrbitDuration && !this.breakingOrbit)
		{
			this.breakingOrbit = true;
			Vector3 appropriatePosition = this.GetAppropriatePosition(base.transform.position + (base.transform.forward * 75f), 40f, 50f);
			this.SetTargetDestination(appropriatePosition, 10f, 0f);
		}
	}

	public void State_Patrol_Enter()
	{
		this._currentState = PatrolHelicopterAI.aiState.PATROL;
		Vector3 randomPatrolDestination = this.GetRandomPatrolDestination();
		this.SetTargetDestination(randomPatrolDestination, 10f, 30f);
		this.interestZoneOrigin = randomPatrolDestination;
		this.arrivalTime = 0f;
	}

	public void State_Patrol_Leave()
	{
	}

	public void State_Patrol_Think(float timePassed)
	{
		float single = Vector3.Distance(base.transform.position, this.destination);
		if (single > 25f)
		{
			this.targetThrottleSpeed = 0.5f;
		}
		else
		{
			this.targetThrottleSpeed = this.GetThrottleForDistance(single);
		}
		if (this.AtDestination() && this.arrivalTime == 0f)
		{
			this.arrivalTime = UnityEngine.Time.realtimeSinceStartup;
			this.ExitCurrentState();
			this.maxOrbitDuration = 20f;
			this.State_Orbit_Enter(75f);
		}
		if (this._targetList.Count > 0)
		{
			this.interestZoneOrigin = this._targetList[0].ply.transform.position + new Vector3(0f, 20f, 0f);
			this.ExitCurrentState();
			this.maxOrbitDuration = 10f;
			this.State_Orbit_Enter(75f);
		}
	}

	public void State_Strafe_Enter(Vector3 strafePos, bool shouldUseNapalm = false)
	{
		Vector3 vector3;
		Vector3 vector31;
		if (this.CanUseNapalm() & shouldUseNapalm)
		{
			this.useNapalm = shouldUseNapalm;
			this.lastNapalmTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.lastStrafeTime = UnityEngine.Time.realtimeSinceStartup;
		this._currentState = PatrolHelicopterAI.aiState.STRAFE;
		if (!TransformUtil.GetGroundInfo(strafePos, out vector3, out vector31, 100f, LayerMask.GetMask(new string[] { "Terrain", "World", "Construction", "Water" }), base.transform))
		{
			this.strafe_target_position = strafePos;
		}
		else
		{
			this.strafe_target_position = vector3;
		}
		this.numRocketsLeft = 12;
		this.lastRocketTime = 0f;
		this.movementLockingAiming = true;
		Vector3 randomOffset = this.GetRandomOffset(strafePos, 175f, 192.5f, 20f, 30f);
		this.SetTargetDestination(randomOffset, 10f, 30f);
		this.SetIdealRotation(this.GetYawRotationTo(randomOffset), -1f);
		this.puttingDistance = true;
	}

	public void State_Strafe_Leave()
	{
		this.lastStrafeTime = UnityEngine.Time.realtimeSinceStartup;
		if (this.useNapalm)
		{
			this.lastNapalmTime = UnityEngine.Time.realtimeSinceStartup;
		}
		this.useNapalm = false;
		this.movementLockingAiming = false;
	}

	public void State_Strafe_Think(float timePassed)
	{
		if (!this.puttingDistance)
		{
			this.SetIdealRotation(this.GetYawRotationTo(this.strafe_target_position), -1f);
			float single = Vector3Ex.Distance2D(this.strafe_target_position, base.transform.position);
			if (single <= 150f && this.ClipRocketsLeft() > 0 && UnityEngine.Time.realtimeSinceStartup - this.lastRocketTime > this.timeBetweenRockets)
			{
				float single1 = Vector3.Distance(this.strafe_target_position, base.transform.position) - 10f;
				if (single1 < 0f)
				{
					single1 = 0f;
				}
				Vector3 vector3 = base.transform.position;
				Vector3 strafeTargetPosition = this.strafe_target_position - base.transform.position;
				if (!UnityEngine.Physics.Raycast(vector3, strafeTargetPosition.normalized, single1, LayerMask.GetMask(new string[] { "Terrain", "World" })))
				{
					this.FireRocket();
				}
			}
			if (this.ClipRocketsLeft() <= 0 || single <= 15f)
			{
				this.ExitCurrentState();
				this.State_Move_Enter(this.GetAppropriatePosition(this.strafe_target_position + (base.transform.forward * 120f), 20f, 30f));
			}
		}
		else if (this.AtDestination())
		{
			this.puttingDistance = false;
			this.SetTargetDestination(this.strafe_target_position + new Vector3(0f, 40f, 0f), 10f, 30f);
			this.SetIdealRotation(this.GetYawRotationTo(this.strafe_target_position), -1f);
			return;
		}
	}

	public void TerrainPushback()
	{
		RaycastHit raycastHit;
		RaycastHit raycastHit1;
		if (this._currentState == PatrolHelicopterAI.aiState.DEATH)
		{
			return;
		}
		Vector3 vector3 = base.transform.position + new Vector3(0f, 2f, 0f);
		Vector3 vector31 = (this.destination - vector3).normalized;
		float single = Vector3.Distance(this.destination, base.transform.position);
		Ray ray = new Ray(vector3, vector31);
		float single1 = 5f;
		float single2 = Mathf.Min(100f, single);
		int mask = LayerMask.GetMask(new string[] { "Terrain", "World", "Construction" });
		Vector3 vector32 = Vector3.zero;
		if (UnityEngine.Physics.SphereCast(ray, single1, out raycastHit, single2 - single1 * 0.5f, mask))
		{
			float single3 = 1f - raycastHit.distance / single2;
			vector32 = Vector3.up * (this.terrainPushForce * single3);
		}
		Ray ray1 = new Ray(vector3, this._lastMoveDir);
		float single4 = Mathf.Min(10f, single);
		if (UnityEngine.Physics.SphereCast(ray1, single1, out raycastHit1, single4 - single1 * 0.5f, mask))
		{
			float single5 = 1f - raycastHit1.distance / single4;
			float single6 = this.obstaclePushForce * single5;
			vector32 = vector32 + ((this._lastMoveDir * single6) * -1f);
			vector32 = vector32 + (Vector3.up * single6);
		}
		this.pushVec = Vector3.Lerp(this.pushVec, vector32, UnityEngine.Time.deltaTime);
		Transform transforms = base.transform;
		transforms.position = transforms.position + (this.pushVec * UnityEngine.Time.deltaTime);
	}

	public void Update()
	{
		if (this.helicopterBase.isClient)
		{
			return;
		}
		PatrolHelicopterAI.heliInstance = this;
		this.UpdateTargetList();
		this.MoveToDestination();
		this.UpdateRotation();
		this.UpdateSpotlight();
		this.AIThink();
		this.DoMachineGuns();
		if (!this.isRetiring)
		{
			float single = Mathf.Max(this.spawnTime + PatrolHelicopter.lifetimeMinutes * 60f, this.lastDamageTime + 120f);
			if (UnityEngine.Time.realtimeSinceStartup > single)
			{
				this.Retire();
			}
		}
	}

	public void UpdateRotation()
	{
		if (this.hasAimTarget)
		{
			Vector3 vector3 = base.transform.position;
			vector3.y = 0f;
			Vector3 vector31 = this._aimTarget;
			vector31.y = 0f;
			Vector3 vector32 = (vector31 - vector3).normalized;
			Vector3 vector33 = Vector3.Cross(vector32, Vector3.up);
			float single = Vector3.Angle(vector32, base.transform.right);
			float single1 = Vector3.Angle(vector32, -base.transform.right);
			if (!this.aimDoorSide)
			{
				this.targetRotation = Quaternion.LookRotation(vector32);
			}
			else if (single >= single1)
			{
				this.targetRotation = Quaternion.LookRotation(-vector33);
			}
			else
			{
				this.targetRotation = Quaternion.LookRotation(vector33);
			}
		}
		this.rotationSpeed = Mathf.Lerp(this.rotationSpeed, this.maxRotationSpeed, UnityEngine.Time.deltaTime / 2f);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, this.targetRotation, this.rotationSpeed * UnityEngine.Time.deltaTime);
	}

	public void UpdateSpotlight()
	{
		if (!this.hasInterestZone)
		{
			this.helicopterBase.spotlightTarget = Vector3.zero;
			return;
		}
		this.helicopterBase.spotlightTarget = new Vector3(this.interestZoneOrigin.x, TerrainMeta.HeightMap.GetHeight(this.interestZoneOrigin), this.interestZoneOrigin.z);
	}

	public void UpdateTargetList()
	{
		bool flag;
		Vector3 vector3 = Vector3.zero;
		bool flag1 = false;
		bool flag2 = false;
		for (int i = this._targetList.Count - 1; i >= 0; i--)
		{
			PatrolHelicopterAI.targetinfo item = this._targetList[i];
			if (item == null || item.ent == null)
			{
				this._targetList.Remove(item);
			}
			else
			{
				if (UnityEngine.Time.realtimeSinceStartup > item.nextLOSCheck)
				{
					item.nextLOSCheck = UnityEngine.Time.realtimeSinceStartup + 1f;
					if (!this.PlayerVisible(item.ply))
					{
						item.visibleFor = 0f;
					}
					else
					{
						item.lastSeenTime = UnityEngine.Time.realtimeSinceStartup;
						item.visibleFor += 1f;
					}
				}
				bool flag3 = (item.ply ? item.ply.IsDead() : item.ent.Health() <= 0f);
				if (item.TimeSinceSeen() >= 6f | flag3)
				{
					bool flag4 = UnityEngine.Random.Range(0f, 1f) >= 0f;
					if ((this.CanStrafe() || this.CanUseNapalm()) && this.IsAlive() && !flag1 && !flag3)
					{
						flag = (item.ply == this.leftGun._target ? true : item.ply == this.rightGun._target);
					}
					else
					{
						flag = false;
					}
					if (flag & flag4)
					{
						flag2 = (!this.ValidStrafeTarget(item.ply) ? true : UnityEngine.Random.Range(0f, 1f) > 0.75f);
						flag1 = true;
						vector3 = item.ply.transform.position;
					}
					this._targetList.Remove(item);
				}
			}
		}
		foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
		{
			if (basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.SafeZone) || Vector3Ex.Distance2D(base.transform.position, basePlayer.transform.position) > 150f)
			{
				continue;
			}
			bool flag5 = false;
			foreach (PatrolHelicopterAI.targetinfo _targetinfo in this._targetList)
			{
				if (_targetinfo.ply != basePlayer)
				{
					continue;
				}
				flag5 = true;
				goto Label0;
			}
		Label0:
			if (flag5 || basePlayer.GetThreatLevel() <= 0.5f || !this.PlayerVisible(basePlayer))
			{
				continue;
			}
			this._targetList.Add(new PatrolHelicopterAI.targetinfo(basePlayer, basePlayer));
		}
		if (flag1)
		{
			this.ExitCurrentState();
			this.State_Strafe_Enter(vector3, flag2);
		}
	}

	public void UpdateWind()
	{
		this.targetWindVec = UnityEngine.Random.onUnitSphere;
	}

	public bool ValidStrafeTarget(BasePlayer ply)
	{
		object obj = Interface.CallHook("CanHelicopterStrafeTarget", this, ply);
		if (obj as bool)
		{
			return (bool)obj;
		}
		return !ply.IsNearEnemyBase();
	}

	public void WasAttacked(HitInfo info)
	{
		BasePlayer initiator = info.Initiator as BasePlayer;
		if (initiator != null)
		{
			this._targetList.Add(new PatrolHelicopterAI.targetinfo(initiator, initiator));
		}
	}

	public void WeakspotDamaged(BaseHelicopter.weakspot weak, HitInfo info)
	{
		float single = UnityEngine.Time.realtimeSinceStartup - this.lastDamageTime;
		this.lastDamageTime = UnityEngine.Time.realtimeSinceStartup;
		BasePlayer initiator = info.Initiator as BasePlayer;
		bool flag = this.ValidStrafeTarget(initiator);
		bool flag1 = (!flag ? false : this.CanStrafe());
		bool flag2 = (flag ? false : this.CanUseNapalm());
		if (single < 5f && initiator != null && flag1 | flag2)
		{
			this.ExitCurrentState();
			this.State_Strafe_Enter(info.Initiator.transform.position, flag2);
		}
	}

	public enum aiState
	{
		IDLE,
		MOVE,
		ORBIT,
		STRAFE,
		PATROL,
		GUARD,
		DEATH
	}

	public class targetinfo
	{
		public BasePlayer ply;

		public BaseEntity ent;

		public float lastSeenTime;

		public float visibleFor;

		public float nextLOSCheck;

		public targetinfo(BaseEntity initEnt, BasePlayer initPly = null)
		{
			this.ply = initPly;
			this.ent = initEnt;
			this.lastSeenTime = Single.PositiveInfinity;
			this.nextLOSCheck = UnityEngine.Time.realtimeSinceStartup + 1.5f;
		}

		public bool IsVisible()
		{
			return this.TimeSinceSeen() < 1.5f;
		}

		public float TimeSinceSeen()
		{
			return UnityEngine.Time.realtimeSinceStartup - this.lastSeenTime;
		}
	}
}