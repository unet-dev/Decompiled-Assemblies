using ConVar;
using Facepunch;
using Network;
using Oxide.Core;
using ProtoBuf;
using Rust;
using Rust.AI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BradleyAPC : BaseCombatEntity
{
	[Header("Sound")]
	public EngineAudioClip engineAudioClip;

	public SlicedGranularAudioClip treadAudioClip;

	public float treadGrainFreqMin = 0.025f;

	public float treadGrainFreqMax = 0.5f;

	public AnimationCurve treadFreqCurve;

	public SoundDefinition chasisLurchSoundDef;

	public float chasisLurchAngleDelta = 2f;

	public float chasisLurchSpeedDelta = 2f;

	private float lastAngle;

	private float lastSpeed;

	public SoundDefinition turretTurnLoopDef;

	public float turretLoopGainSpeed = 3f;

	public float turretLoopPitchSpeed = 3f;

	public float turretLoopMinAngleDelta;

	public float turretLoopMaxAngleDelta = 10f;

	public float turretLoopPitchMin = 0.5f;

	public float turretLoopPitchMax = 1f;

	public float turretLoopGainThreshold = 0.0001f;

	private Sound turretTurnLoop;

	private SoundModulation.Modulator turretTurnLoopGain;

	private SoundModulation.Modulator turretTurnLoopPitch;

	public float enginePitch = 0.9f;

	public float rpmMultiplier = 0.6f;

	private TreadAnimator treadAnimator;

	[Header("Wheels")]
	public WheelCollider[] leftWheels;

	public WheelCollider[] rightWheels;

	[Header("Movement Config")]
	public float moveForceMax = 2000f;

	public float brakeForce = 100f;

	public float turnForce = 2000f;

	public float sideStiffnessMax = 1f;

	public float sideStiffnessMin = 0.5f;

	public Transform centerOfMass;

	public float stoppingDist = 5f;

	[Header("Control")]
	public float throttle = 1f;

	public float turning;

	public float rightThrottle;

	public float leftThrottle;

	public bool brake;

	[Header("Other")]
	public Rigidbody myRigidBody;

	public Collider myCollider;

	public Vector3 destination;

	private Vector3 finalDestination;

	public Transform followTest;

	public TriggerHurtEx impactDamager;

	[Header("Weapons")]
	public Transform mainTurretEyePos;

	public Transform mainTurret;

	public Transform CannonPitch;

	public Transform CannonMuzzle;

	public Transform coaxPitch;

	public Transform coaxMuzzle;

	public Transform topTurretEyePos;

	public Transform topTurretYaw;

	public Transform topTurretPitch;

	public Transform topTurretMuzzle;

	public Vector3 turretAimVector = Vector3.forward;

	private Vector3 desiredAimVector = Vector3.forward;

	public Vector3 topTurretAimVector = Vector3.forward;

	private Vector3 desiredTopTurretAimVector = Vector3.forward;

	[Header("Effects")]
	public GameObjectRef explosionEffect;

	public GameObjectRef servergibs;

	public GameObjectRef fireBall;

	public GameObjectRef crateToDrop;

	public GameObjectRef debrisFieldMarker;

	[Header("Loot")]
	public int maxCratesToSpawn;

	public int patrolPathIndex;

	public BasePath patrolPath;

	public bool DoAI = true;

	public GameObjectRef mainCannonMuzzleFlash;

	public GameObjectRef mainCannonProjectile;

	private float nextFireTime = 10f;

	private int numBursted;

	public float recoilScale = 200f;

	public NavMeshPath navMeshPath;

	public int navMeshPathIndex;

	private float nextPatrolTime;

	private float nextEngagementPathTime;

	private float currentSpeedZoneLimit;

	[Header("Pathing")]
	public List<Vector3> currentPath;

	public int currentPathIndex;

	public bool pathLooping;

	[Header("Targeting")]
	public float viewDistance = 100f;

	public float searchRange = 100f;

	public float searchFrequency = 2f;

	public float memoryDuration = 20f;

	public static float sightUpdateRate;

	private BaseCombatEntity mainGunTarget;

	public List<BradleyAPC.TargetInfo> targetList = new List<BradleyAPC.TargetInfo>();

	private float nextCoaxTime;

	private float coaxFireRate = 0.06667f;

	private int numCoaxBursted;

	private float bulletDamage = 7f;

	public GameObjectRef gun_fire_effect;

	public GameObjectRef bulletEffect;

	private float lastLateUpdate;

	protected override float PositionTickRate
	{
		get
		{
			return 0.1f;
		}
	}

	static BradleyAPC()
	{
		BradleyAPC.sightUpdateRate = 0.5f;
	}

	public BradleyAPC()
	{
	}

	public void AddOrUpdateTarget(BaseEntity ent, Vector3 pos, float damageFrom = 0f)
	{
		BradleyAPC.TargetInfo targetInfo;
		if (!(ent is BasePlayer))
		{
			return;
		}
		BradleyAPC.TargetInfo targetInfo1 = null;
		foreach (BradleyAPC.TargetInfo targetInfo2 in this.targetList)
		{
			if (targetInfo2.entity != ent)
			{
				continue;
			}
			targetInfo1 = targetInfo2;
			if (targetInfo1 == null)
			{
				targetInfo1 = Facepunch.Pool.Get<BradleyAPC.TargetInfo>();
				targetInfo1.Setup(ent, UnityEngine.Time.time - 1f);
				this.targetList.Add(targetInfo1);
			}
			targetInfo1.lastSeenPosition = pos;
			targetInfo = targetInfo1;
			targetInfo.damageReceivedFrom += damageFrom;
			return;
		}
		if (targetInfo1 == null)
		{
			targetInfo1 = Facepunch.Pool.Get<BradleyAPC.TargetInfo>();
			targetInfo1.Setup(ent, UnityEngine.Time.time - 1f);
			this.targetList.Add(targetInfo1);
		}
		targetInfo1.lastSeenPosition = pos;
		targetInfo = targetInfo1;
		targetInfo.damageReceivedFrom += damageFrom;
	}

	public void AdvancePathMovement()
	{
		if (!this.HasPath())
		{
			return;
		}
		if (this.AtCurrentPathNode() || this.currentPathIndex == -1)
		{
			this.currentPathIndex = this.GetLoopedIndex(this.currentPathIndex + 1);
		}
		if (this.PathComplete())
		{
			this.ClearPath();
			return;
		}
		Vector3 vector3 = this.IdealPathPosition();
		float single = Vector3.Distance(vector3, this.currentPath[this.currentPathIndex]);
		float single1 = Vector3.Distance(base.transform.position, vector3);
		float single2 = Mathf.InverseLerp(8f, 0f, single1);
		vector3 = vector3 + (BradleyAPC.Direction2D(this.currentPath[this.currentPathIndex], vector3) * Mathf.Min(single, single2 * 20f));
		this.SetDestination(vector3);
	}

	public void AimWeaponAt(Transform weaponYaw, Transform weaponPitch, Vector3 direction, float minPitch = -360f, float maxPitch = 360f, float maxYaw = 360f, Transform parentOverride = null)
	{
		Vector3 vector3 = weaponYaw.parent.InverseTransformDirection(direction);
		Quaternion quaternion = Quaternion.LookRotation(vector3);
		Vector3 item = quaternion.eulerAngles;
		for (int i = 0; i < 3; i++)
		{
			item[i] = item[i] - (item[i] > 180f ? 360f : 0f);
		}
		Quaternion quaternion1 = Quaternion.Euler(0f, Mathf.Clamp(item.y, -maxYaw, maxYaw), 0f);
		Quaternion quaternion2 = Quaternion.Euler(Mathf.Clamp(item.x, minPitch, maxPitch), 0f, 0f);
		if (weaponYaw == null && weaponPitch != null)
		{
			weaponPitch.transform.localRotation = quaternion2;
			return;
		}
		if (weaponPitch == null && weaponYaw != null)
		{
			weaponYaw.transform.localRotation = quaternion;
			return;
		}
		weaponYaw.transform.localRotation = quaternion1;
		weaponPitch.transform.localRotation = quaternion2;
	}

	public void ApplyBrakes(float amount)
	{
		this.ApplyBrakeTorque(amount, true);
		this.ApplyBrakeTorque(amount, false);
	}

	public void ApplyBrakeTorque(float amount, bool rightSide)
	{
		WheelCollider[] wheelColliderArray = (rightSide ? this.rightWheels : this.leftWheels);
		for (int i = 0; i < (int)wheelColliderArray.Length; i++)
		{
			wheelColliderArray[i].brakeTorque = this.brakeForce * amount;
		}
	}

	private void ApplyDamage(BaseCombatEntity entity, Vector3 point, Vector3 normal)
	{
		float single = this.bulletDamage * UnityEngine.Random.Range(0.9f, 1.1f);
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

	public bool AtCurrentPathNode()
	{
		if (this.currentPathIndex < 0 || this.currentPathIndex >= this.currentPath.Count)
		{
			return false;
		}
		return Vector3.Distance(base.transform.position, this.currentPath[this.currentPathIndex]) <= this.stoppingDist;
	}

	public void ClearPath()
	{
		this.currentPath.Clear();
		this.currentPathIndex = -1;
	}

	public Vector3 ClosestPointAlongPath(Vector3 start, Vector3 end, Vector3 fromPos)
	{
		Vector3 vector3 = start;
		Vector3 vector31 = end;
		Vector3 vector32 = fromPos;
		Vector3 vector33 = vector31 - vector3;
		float single = Vector3.Dot(vector33, vector32 - vector3);
		float single1 = Vector3.SqrMagnitude(vector31 - vector3);
		float single2 = Mathf.Clamp01(single / single1);
		return vector3 + (vector33 * single2);
	}

	public void CreateExplosionMarker(float durationMinutes)
	{
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.debrisFieldMarker.resourcePath, base.transform.position, Quaternion.identity, true);
		baseEntity.Spawn();
		baseEntity.SendMessage("SetDuration", durationMinutes, SendMessageOptions.DontRequireReceiver);
	}

	public static Vector3 Direction2D(Vector3 aimAt, Vector3 aimFrom)
	{
		Vector3 vector3 = new Vector3(aimAt.x, 0f, aimAt.z) - new Vector3(aimFrom.x, 0f, aimFrom.z);
		return vector3.normalized;
	}

	public void DoHealing()
	{
		if (base.isClient)
		{
			return;
		}
		if (base.healthFraction < 1f && base.SecondsSinceAttacked > 600f)
		{
			float single = this.MaxHealth() / 300f * UnityEngine.Time.fixedDeltaTime;
			this.Heal(single);
		}
	}

	public void DoPhysicsMove()
	{
		if (base.isClient)
		{
			return;
		}
		Vector3 vector3 = this.myRigidBody.velocity;
		this.throttle = Mathf.Clamp(this.throttle, -1f, 1f);
		this.leftThrottle = this.throttle;
		this.rightThrottle = this.throttle;
		if (this.turning > 0f)
		{
			this.rightThrottle = -this.turning;
			this.leftThrottle = this.turning;
		}
		else if (this.turning < 0f)
		{
			this.leftThrottle = this.turning;
			this.rightThrottle = this.turning * -1f;
		}
		float single = Vector3.Distance(base.transform.position, this.GetFinalDestination());
		float single1 = Vector3.Distance(base.transform.position, this.GetCurrentPathDestination());
		float single2 = 15f;
		if (single1 < 20f)
		{
			float single3 = Vector3.Dot(this.PathDirection(this.currentPathIndex), this.PathDirection(this.currentPathIndex + 1));
			float single4 = Mathf.InverseLerp(2f, 10f, single1);
			float single5 = Mathf.InverseLerp(0.5f, 0.8f, single3);
			single2 = 15f - 14f * ((1f - single5) * (1f - single4));
		}
		if (this.patrolPath != null)
		{
			float single6 = single2;
			foreach (PathSpeedZone speedZone in this.patrolPath.speedZones)
			{
				if (!speedZone.WorldSpaceBounds().Contains(base.transform.position))
				{
					continue;
				}
				single6 = Mathf.Min(single6, speedZone.GetMaxSpeed());
			}
			this.currentSpeedZoneLimit = Mathf.Lerp(this.currentSpeedZoneLimit, single6, UnityEngine.Time.deltaTime);
			single2 = Mathf.Min(single2, this.currentSpeedZoneLimit);
		}
		if (this.PathComplete())
		{
			single2 = 0f;
		}
		if (ConVar.Global.developer > 1)
		{
			Debug.Log(string.Concat(new object[] { "velocity:", vector3.magnitude, "max : ", single2 }));
		}
		this.brake = vector3.magnitude >= single2;
		this.ApplyBrakes((this.brake ? 1f : 0f));
		float single7 = this.throttle;
		this.leftThrottle = Mathf.Clamp(this.leftThrottle + single7, -1f, 1f);
		this.rightThrottle = Mathf.Clamp(this.rightThrottle + single7, -1f, 1f);
		float single8 = Mathf.InverseLerp(2f, 1f, vector3.magnitude * Mathf.Abs(Vector3.Dot(vector3.normalized, base.transform.forward)));
		float single9 = Mathf.Lerp(this.moveForceMax, this.turnForce, single8);
		float single10 = Mathf.InverseLerp(5f, 1.5f, vector3.magnitude * Mathf.Abs(Vector3.Dot(vector3.normalized, base.transform.forward)));
		this.ScaleSidewaysFriction(1f - single10);
		this.SetMotorTorque(this.leftThrottle, false, single9);
		this.SetMotorTorque(this.rightThrottle, true, single9);
		this.impactDamager.damageEnabled = this.myRigidBody.velocity.magnitude > 2f;
	}

	public void DoSimpleAI()
	{
		if (base.isClient)
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.Reserved5, TOD_Sky.Instance.IsNight, false, true);
		if (!this.DoAI)
		{
			return;
		}
		if (this.targetList.Count <= 0)
		{
			this.mainGunTarget = null;
			this.UpdateMovement_Patrol();
		}
		else
		{
			if (!this.targetList[0].IsValid() || !this.targetList[0].IsVisible())
			{
				this.mainGunTarget = null;
			}
			else
			{
				this.mainGunTarget = this.targetList[0].entity as BaseCombatEntity;
			}
			this.UpdateMovement_Hunt();
		}
		this.AdvancePathMovement();
		float single = Vector3.Distance(base.transform.position, this.destination);
		float single1 = Vector3.Distance(base.transform.position, this.finalDestination);
		if (single > this.stoppingDist)
		{
			Vector3 vector3 = BradleyAPC.Direction2D(this.destination, base.transform.position);
			float single2 = Vector3.Dot(vector3, base.transform.right);
			float single3 = Vector3.Dot(vector3, base.transform.right);
			float single4 = Vector3.Dot(vector3, -base.transform.right);
			if (Vector3.Dot(vector3, -base.transform.forward) <= single2)
			{
				this.turning = Mathf.Clamp(single2 * 3f, -1f, 1f);
			}
			else if (single3 < single4)
			{
				this.turning = -1f;
			}
			else
			{
				this.turning = 1f;
			}
			float single5 = 1f - Mathf.InverseLerp(0f, 0.3f, Mathf.Abs(this.turning));
			float single6 = Mathf.InverseLerp(0.1f, 0.4f, Vector3.Dot(base.transform.forward, Vector3.up));
			this.throttle = (0.1f + Mathf.InverseLerp(0f, 20f, single1) * 1f) * single5 + single6;
		}
		this.DoWeaponAiming();
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void DoWeaponAiming()
	{
		Vector3 aimPoint;
		Vector3 vector3;
		Vector3 vector31;
		if (this.mainGunTarget != null)
		{
			aimPoint = this.GetAimPoint(this.mainGunTarget) - this.mainTurretEyePos.transform.position;
			vector3 = aimPoint.normalized;
		}
		else
		{
			vector3 = this.desiredAimVector;
		}
		this.desiredAimVector = vector3;
		BaseEntity item = null;
		if (this.targetList.Count > 0)
		{
			if (this.targetList.Count > 1 && this.targetList[1].IsValid() && this.targetList[1].IsVisible())
			{
				item = this.targetList[1].entity;
			}
			else if (this.targetList[0].IsValid() && this.targetList[0].IsVisible())
			{
				item = this.targetList[0].entity;
			}
		}
		if (item != null)
		{
			aimPoint = this.GetAimPoint(item) - this.topTurretEyePos.transform.position;
			vector31 = aimPoint.normalized;
		}
		else
		{
			vector31 = base.transform.forward;
		}
		this.desiredTopTurretAimVector = vector31;
	}

	public void DoWeapons()
	{
		if (this.mainGunTarget != null && Vector3.Dot(this.turretAimVector, (this.GetAimPoint(this.mainGunTarget) - this.mainTurretEyePos.transform.position).normalized) >= 0.99f)
		{
			bool flag = this.VisibilityTest(this.mainGunTarget);
			float single = Vector3.Distance(this.mainGunTarget.transform.position, base.transform.position);
			if (UnityEngine.Time.time > this.nextCoaxTime & flag && single <= 40f)
			{
				this.numCoaxBursted++;
				this.FireGun(this.GetAimPoint(this.mainGunTarget), 3f, true);
				this.nextCoaxTime = UnityEngine.Time.time + this.coaxFireRate;
				if (this.numCoaxBursted >= 10)
				{
					this.nextCoaxTime = UnityEngine.Time.time + 1f;
					this.numCoaxBursted = 0;
				}
			}
			if (single >= 10f & flag)
			{
				this.FireGunTest();
			}
		}
	}

	public void FireGun(Vector3 targetPos, float aimCone, bool isCoax)
	{
		Transform transforms = (isCoax ? this.coaxMuzzle : this.topTurretMuzzle);
		Vector3 vector3 = transforms.transform.position - (transforms.forward * 0.25f);
		Vector3 vector31 = targetPos - vector3;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, vector31.normalized, true);
		targetPos = vector3 + (modifiedAimConeDirection * 300f);
		List<RaycastHit> list = Facepunch.Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(vector3, modifiedAimConeDirection), 0f, list, 300f, 1219701521, QueryTriggerInteraction.UseGlobal);
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit item = list[i];
			BaseEntity entity = item.GetEntity();
			if (!(entity != null) || !(entity == this) && !entity.EqualNetID(this))
			{
				BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
				if (baseCombatEntity != null)
				{
					this.ApplyDamage(baseCombatEntity, item.point, modifiedAimConeDirection);
				}
				if (!(entity != null) || entity.ShouldBlockProjectiles())
				{
					targetPos = item.point;
					break;
				}
			}
		}
		base.ClientRPC<bool, Vector3>(null, "CLIENT_FireGun", isCoax, targetPos);
		Facepunch.Pool.FreeList<RaycastHit>(ref list);
	}

	public void FireGunTest()
	{
		if (UnityEngine.Time.time < this.nextFireTime)
		{
			return;
		}
		this.nextFireTime = UnityEngine.Time.time + 0.25f;
		this.numBursted++;
		if (this.numBursted >= 4)
		{
			this.nextFireTime = UnityEngine.Time.time + 5f;
			this.numBursted = 0;
		}
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(2f, this.CannonMuzzle.rotation * Vector3.forward, true);
		Vector3 cannonPitch = (this.CannonPitch.transform.rotation * Vector3.back) + (base.transform.up * -1f);
		Vector3 vector3 = cannonPitch.normalized;
		this.myRigidBody.AddForceAtPosition(vector3 * this.recoilScale, this.CannonPitch.transform.position, ForceMode.Impulse);
		Effect.server.Run(this.mainCannonMuzzleFlash.resourcePath, this, StringPool.Get(this.CannonMuzzle.gameObject.name), Vector3.zero, Vector3.zero, null, false);
		BaseEntity baseEntity = GameManager.server.CreateEntity(this.mainCannonProjectile.resourcePath, this.CannonMuzzle.transform.position, Quaternion.LookRotation(modifiedAimConeDirection), true);
		if (baseEntity == null)
		{
			return;
		}
		ServerProjectile component = baseEntity.GetComponent<ServerProjectile>();
		if (component)
		{
			component.InitializeVelocity(modifiedAimConeDirection * component.speed);
		}
		baseEntity.Spawn();
	}

	public void FixedUpdate()
	{
		this.DoSimpleAI();
		this.DoPhysicsMove();
		this.DoWeapons();
		this.DoHealing();
	}

	public BasePlayer FollowPlayer()
	{
		BasePlayer basePlayer;
		List<BasePlayer>.Enumerator enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				if (!current.IsAdmin || !current.IsAlive() || current.IsSleeping() || current.GetActiveItem() == null || !(current.GetActiveItem().info.shortname == "tool.binoculars"))
				{
					continue;
				}
				basePlayer = current;
				return basePlayer;
			}
			return null;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return basePlayer;
	}

	public Vector3 GetAimPoint(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if (basePlayer == null)
		{
			return ent.CenterPoint();
		}
		return basePlayer.eyes.position;
	}

	public Vector3 GetCurrentPathDestination()
	{
		if (!this.HasPath())
		{
			return base.transform.position;
		}
		return this.currentPath[this.currentPathIndex];
	}

	public bool GetEngagementPath(ref List<BasePathNode> nodes)
	{
		BasePathNode closestToPoint = this.patrolPath.GetClosestToPoint(base.transform.position);
		Vector3 vector3 = closestToPoint.transform.position - base.transform.position;
		Vector3 vector31 = vector3.normalized;
		if (Vector3.Dot(base.transform.forward, vector31) > 0f)
		{
			nodes.Add(closestToPoint);
			if (!closestToPoint.straightaway)
			{
				return true;
			}
		}
		return this.GetPathToClosestTurnableNode(closestToPoint, base.transform.forward, ref nodes);
	}

	public Vector3 GetFinalDestination()
	{
		if (this.HasPath())
		{
			return this.finalDestination;
		}
		return base.transform.position;
	}

	public int GetLoopedIndex(int index)
	{
		if (!this.HasPath())
		{
			Debug.LogWarning("Warning, GetLoopedIndex called without a path");
			return 0;
		}
		if (!this.pathLooping)
		{
			return Mathf.Clamp(index, 0, this.currentPath.Count - 1);
		}
		if (index >= this.currentPath.Count)
		{
			return index % this.currentPath.Count;
		}
		if (index >= 0)
		{
			return index;
		}
		return this.currentPath.Count - Mathf.Abs(index % this.currentPath.Count);
	}

	public float GetMotorTorque(bool rightSide)
	{
		float length = 0f;
		WheelCollider[] wheelColliderArray = (rightSide ? this.rightWheels : this.leftWheels);
		for (int i = 0; i < (int)wheelColliderArray.Length; i++)
		{
			length += wheelColliderArray[i].motorTorque;
		}
		length /= (float)((int)this.rightWheels.Length);
		return length;
	}

	public bool GetPathToClosestTurnableNode(BasePathNode start, Vector3 forward, ref List<BasePathNode> nodes)
	{
		Vector3 vector3;
		float single = Single.NegativeInfinity;
		BasePathNode basePathNode = null;
		foreach (BasePathNode basePathNode1 in start.linked)
		{
			vector3 = basePathNode1.transform.position - start.transform.position;
			float single1 = Vector3.Dot(forward, vector3.normalized);
			if (single1 <= single)
			{
				continue;
			}
			single = single1;
			basePathNode = basePathNode1;
		}
		if (basePathNode == null)
		{
			return false;
		}
		nodes.Add(basePathNode);
		if (!basePathNode.straightaway)
		{
			return true;
		}
		vector3 = basePathNode.transform.position - start.transform.position;
		return this.GetPathToClosestTurnableNode(basePathNode, vector3.normalized, ref nodes);
	}

	public bool HasPath()
	{
		if (this.currentPath == null)
		{
			return false;
		}
		return this.currentPath.Count > 0;
	}

	public Vector3 IdealPathPosition()
	{
		if (!this.HasPath())
		{
			return base.transform.position;
		}
		int loopedIndex = this.GetLoopedIndex(this.currentPathIndex - 1);
		if (loopedIndex == this.currentPathIndex)
		{
			return this.currentPath[this.currentPathIndex];
		}
		return this.ClosestPointAlongPath(this.currentPath[loopedIndex], this.currentPath[this.currentPathIndex], base.transform.position);
	}

	public bool IndexValid(int index)
	{
		if (!this.HasPath())
		{
			return false;
		}
		if (index < 0)
		{
			return false;
		}
		return index < this.currentPath.Count;
	}

	public void Initialize()
	{
		if (Interface.CallHook("OnBradleyApcInitialize", this) != null)
		{
			return;
		}
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		this.destination = base.transform.position;
		this.finalDestination = base.transform.position;
	}

	public void InstallPatrolPath(BasePath path)
	{
		this.patrolPath = path;
		this.currentPath = new List<Vector3>();
		this.currentPathIndex = -1;
	}

	public bool IsAtDestination()
	{
		return Vector3Ex.Distance2D(base.transform.position, this.destination) <= this.stoppingDist;
	}

	public bool IsAtFinalDestination()
	{
		return Vector3Ex.Distance2D(base.transform.position, this.finalDestination) <= this.stoppingDist;
	}

	public void LateUpdate()
	{
		float single = UnityEngine.Time.time - this.lastLateUpdate;
		this.lastLateUpdate = UnityEngine.Time.time;
		if (!base.isServer)
		{
			this.turretAimVector = Vector3.Lerp(this.turretAimVector, this.desiredAimVector, UnityEngine.Time.deltaTime * 10f);
		}
		else
		{
			float single1 = 2.09439516f;
			this.turretAimVector = Vector3.RotateTowards(this.turretAimVector, this.desiredAimVector, single1 * single, 0f);
		}
		this.AimWeaponAt(this.mainTurret, this.coaxPitch, this.turretAimVector, -90f, 90f, 360f, null);
		this.AimWeaponAt(this.mainTurret, this.CannonPitch, this.turretAimVector, -90f, 7f, 360f, null);
		this.topTurretAimVector = Vector3.Lerp(this.topTurretAimVector, this.desiredTopTurretAimVector, UnityEngine.Time.deltaTime * 5f);
		this.AimWeaponAt(this.topTurretYaw, this.topTurretPitch, this.topTurretAimVector, -360f, 360f, 360f, this.mainTurret);
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.bradley != null && !info.fromDisk)
		{
			this.throttle = info.msg.bradley.engineThrottle;
			this.rightThrottle = info.msg.bradley.throttleRight;
			this.leftThrottle = info.msg.bradley.throttleLeft;
			this.desiredAimVector = info.msg.bradley.mainGunVec;
			this.desiredTopTurretAimVector = info.msg.bradley.topTurretVec;
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		BasePlayer initiator = info.Initiator as BasePlayer;
		if (initiator != null)
		{
			this.AddOrUpdateTarget(initiator, info.PointStart, info.damageTypes.Total());
		}
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
	}

	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		base.OnHealthChanged(oldvalue, newvalue);
		base.SetFlag(BaseEntity.Flags.Reserved2, base.healthFraction <= 0.75f, false, true);
		base.SetFlag(BaseEntity.Flags.Reserved3, base.healthFraction < 0.4f, false, true);
	}

	public override void OnKilled(HitInfo info)
	{
		if (base.isClient)
		{
			return;
		}
		this.CreateExplosionMarker(10f);
		Effect.server.Run(this.explosionEffect.resourcePath, this.mainTurretEyePos.transform.position, Vector3.up, null, true);
		Vector3 vector3 = Vector3.zero;
		GameObject component = this.servergibs.Get().GetComponent<ServerGib>()._gibSource;
		List<ServerGib> serverGibs = ServerGib.CreateGibs(this.servergibs.resourcePath, base.gameObject, component, vector3, 3f);
		for (int i = 0; i < 12 - this.maxCratesToSpawn; i++)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireBall.resourcePath, base.transform.position, base.transform.rotation, true);
			if (baseEntity)
			{
				float single = 3f;
				float single1 = 10f;
				Vector3 vector31 = UnityEngine.Random.onUnitSphere;
				baseEntity.transform.position = (base.transform.position + new Vector3(0f, 1.5f, 0f)) + (vector31 * UnityEngine.Random.Range(-4f, 4f));
				Collider collider = baseEntity.GetComponent<Collider>();
				baseEntity.Spawn();
				baseEntity.SetVelocity(vector3 + (vector31 * UnityEngine.Random.Range(single, single1)));
				foreach (ServerGib serverGib in serverGibs)
				{
					UnityEngine.Physics.IgnoreCollision(collider, serverGib.GetCollider(), true);
				}
			}
		}
		for (int j = 0; j < this.maxCratesToSpawn; j++)
		{
			Vector3 vector32 = UnityEngine.Random.onUnitSphere;
			Vector3 vector33 = (base.transform.position + new Vector3(0f, 1.5f, 0f)) + (vector32 * UnityEngine.Random.Range(2f, 3f));
			BaseEntity baseEntity1 = GameManager.server.CreateEntity(this.crateToDrop.resourcePath, vector33, Quaternion.LookRotation(vector32), true);
			baseEntity1.Spawn();
			LootContainer lootContainer = baseEntity1 as LootContainer;
			if (lootContainer)
			{
				lootContainer.Invoke(new Action(lootContainer.RemoveMe), 1800f);
			}
			Collider component1 = baseEntity1.GetComponent<Collider>();
			Rigidbody rigidbody = baseEntity1.gameObject.AddComponent<Rigidbody>();
			rigidbody.useGravity = true;
			rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			rigidbody.mass = 2f;
			rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
			rigidbody.velocity = vector3 + (vector32 * UnityEngine.Random.Range(1f, 3f));
			rigidbody.angularVelocity = Vector3Ex.Range(-1.75f, 1.75f);
			rigidbody.drag = 0.5f * (rigidbody.mass / 5f);
			rigidbody.angularDrag = 0.2f * (rigidbody.mass / 5f);
			GameManager gameManager = GameManager.server;
			string str = this.fireBall.resourcePath;
			Vector3 vector34 = new Vector3();
			Quaternion quaternion = new Quaternion();
			FireBall fireBall = gameManager.CreateEntity(str, vector34, quaternion, true) as FireBall;
			if (fireBall)
			{
				fireBall.SetParent(baseEntity1, false, false);
				fireBall.Spawn();
				fireBall.GetComponent<Rigidbody>().isKinematic = true;
				fireBall.GetComponent<Collider>().enabled = false;
			}
			baseEntity1.SendMessage("SetLockingEnt", fireBall.gameObject, SendMessageOptions.DontRequireReceiver);
			foreach (ServerGib serverGib1 in serverGibs)
			{
				UnityEngine.Physics.IgnoreCollision(component1, serverGib1.GetCollider(), true);
			}
		}
		base.OnKilled(info);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("BradleyAPC.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public bool PathComplete()
	{
		if (!this.HasPath())
		{
			return true;
		}
		if (this.currentPathIndex != this.currentPath.Count - 1)
		{
			return false;
		}
		return this.AtCurrentPathNode();
	}

	public Vector3 PathDirection(int index)
	{
		if (!this.HasPath() || this.currentPath.Count <= 1)
		{
			return base.transform.forward;
		}
		index = this.GetLoopedIndex(index);
		Vector3 item = Vector3.zero;
		Vector3 vector3 = Vector3.zero;
		if (!this.pathLooping)
		{
			item = (index - 1 >= 0 ? this.currentPath[index - 1] : base.transform.position);
			vector3 = this.currentPath[index];
		}
		else
		{
			int loopedIndex = this.GetLoopedIndex(index - 1);
			item = this.currentPath[loopedIndex];
			vector3 = this.currentPath[this.GetLoopedIndex(index)];
		}
		return (vector3 - item).normalized;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.bradley = Facepunch.Pool.Get<ProtoBuf.BradleyAPC>();
			info.msg.bradley.engineThrottle = this.throttle;
			info.msg.bradley.throttleLeft = this.leftThrottle;
			info.msg.bradley.throttleRight = this.rightThrottle;
			info.msg.bradley.mainGunVec = this.turretAimVector;
			info.msg.bradley.topTurretVec = this.topTurretAimVector;
		}
	}

	public void ScaleSidewaysFriction(float scale)
	{
		int i;
		float single = 0.75f + 0.75f * scale;
		WheelCollider[] wheelColliderArray = this.rightWheels;
		for (i = 0; i < (int)wheelColliderArray.Length; i++)
		{
			WheelCollider wheelCollider = wheelColliderArray[i];
			WheelFrictionCurve wheelFrictionCurve = wheelCollider.sidewaysFriction;
			wheelFrictionCurve.stiffness = single;
			wheelCollider.sidewaysFriction = wheelFrictionCurve;
		}
		wheelColliderArray = this.leftWheels;
		for (i = 0; i < (int)wheelColliderArray.Length; i++)
		{
			WheelCollider wheelCollider1 = wheelColliderArray[i];
			WheelFrictionCurve wheelFrictionCurve1 = wheelCollider1.sidewaysFriction;
			wheelFrictionCurve1.stiffness = single;
			wheelCollider1.sidewaysFriction = wheelFrictionCurve1;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		this.Initialize();
		base.InvokeRepeating(new Action(this.UpdateTargetList), 0f, 2f);
		base.InvokeRepeating(new Action(this.UpdateTargetVisibilities), 0f, BradleyAPC.sightUpdateRate);
	}

	public void SetDestination(Vector3 dest)
	{
		this.destination = dest;
	}

	public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
	{
		int i;
		WheelHit wheelHit;
		WheelHit wheelHit1;
		newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
		float single = torqueAmount * newThrottle;
		int num = (rightSide ? (int)this.rightWheels.Length : (int)this.leftWheels.Length);
		int num1 = 0;
		WheelCollider[] wheelColliderArray = (rightSide ? this.rightWheels : this.leftWheels);
		for (i = 0; i < (int)wheelColliderArray.Length; i++)
		{
			if (wheelColliderArray[i].GetGroundHit(out wheelHit))
			{
				num1++;
			}
		}
		float single1 = 1f;
		if (num1 > 0)
		{
			single1 = (float)(num / num1);
		}
		wheelColliderArray = (rightSide ? this.rightWheels : this.leftWheels);
		for (i = 0; i < (int)wheelColliderArray.Length; i++)
		{
			WheelCollider wheelCollider = wheelColliderArray[i];
			if (!wheelCollider.GetGroundHit(out wheelHit1))
			{
				wheelCollider.motorTorque = single;
			}
			else
			{
				wheelCollider.motorTorque = single * single1;
			}
		}
	}

	public int SortTargets(BradleyAPC.TargetInfo t1, BradleyAPC.TargetInfo t2)
	{
		float priorityScore = t2.GetPriorityScore(this);
		return priorityScore.CompareTo(t1.GetPriorityScore(this));
	}

	public void UpdateMovement_Hunt()
	{
		float single;
		if (Interface.CallHook("OnBradleyApcHunt", this) != null)
		{
			return;
		}
		if (this.patrolPath == null)
		{
			return;
		}
		BradleyAPC.TargetInfo item = this.targetList[0];
		if (item.IsValid())
		{
			if (this.HasPath() && item.IsVisible())
			{
				if (this.currentPath.Count > 1)
				{
					Vector3 vector3 = this.currentPath[this.currentPathIndex];
					this.ClearPath();
					this.currentPath.Add(vector3);
					this.finalDestination = vector3;
					this.currentPathIndex = 0;
					return;
				}
			}
			else if (UnityEngine.Time.time > this.nextEngagementPathTime && !this.HasPath() && !item.IsVisible())
			{
				bool flag = false;
				BasePathNode closestToPoint = this.patrolPath.GetClosestToPoint(base.transform.position);
				List<BasePathNode> list = Facepunch.Pool.GetList<BasePathNode>();
				if (this.GetEngagementPath(ref list))
				{
					flag = true;
					closestToPoint = list[list.Count - 1];
				}
				BasePathNode basePathNode = null;
				List<BasePathNode> basePathNodes = Facepunch.Pool.GetList<BasePathNode>();
				this.patrolPath.GetNodesNear(item.lastSeenPosition, ref basePathNodes, 30f);
				Stack<BasePathNode> basePathNodes1 = null;
				float single1 = Single.PositiveInfinity;
				float single2 = this.mainTurretEyePos.localPosition.y;
				foreach (BasePathNode basePathNode1 in basePathNodes)
				{
					Stack<BasePathNode> basePathNodes2 = new Stack<BasePathNode>();
					if (!item.entity.IsVisible(basePathNode1.transform.position + new Vector3(0f, single2, 0f), Single.PositiveInfinity) || !AStarPath.FindPath(closestToPoint, basePathNode1, out basePathNodes2, out single) || single >= single1)
					{
						continue;
					}
					basePathNodes1 = basePathNodes2;
					single1 = single;
					basePathNode = basePathNode1;
				}
				if (basePathNodes1 != null)
				{
					this.currentPath.Clear();
					if (flag)
					{
						for (int i = 0; i < list.Count - 1; i++)
						{
							this.currentPath.Add(list[i].transform.position);
						}
					}
					foreach (BasePathNode basePathNode2 in basePathNodes1)
					{
						this.currentPath.Add(basePathNode2.transform.position);
					}
					this.currentPathIndex = -1;
					this.pathLooping = false;
					this.finalDestination = basePathNode.transform.position;
				}
				Facepunch.Pool.FreeList<BasePathNode>(ref basePathNodes);
				Facepunch.Pool.FreeList<BasePathNode>(ref list);
				this.nextEngagementPathTime = UnityEngine.Time.time + 5f;
			}
		}
	}

	public void UpdateMovement_Patrol()
	{
		float single;
		Stack<BasePathNode> basePathNodes;
		if (this.patrolPath == null)
		{
			return;
		}
		if (UnityEngine.Time.time < this.nextPatrolTime)
		{
			return;
		}
		this.nextPatrolTime = UnityEngine.Time.time + 20f;
		if (this.HasPath() && !this.IsAtFinalDestination())
		{
			return;
		}
		if (Interface.CallHook("OnBradleyApcPatrol", this) != null)
		{
			return;
		}
		PathInterestNode randomInterestNodeAwayFrom = this.patrolPath.GetRandomInterestNodeAwayFrom(base.transform.position, 10f);
		BasePathNode closestToPoint = this.patrolPath.GetClosestToPoint(randomInterestNodeAwayFrom.transform.position);
		BasePathNode item = null;
		bool flag = false;
		List<BasePathNode> list = Facepunch.Pool.GetList<BasePathNode>();
		if (!this.GetEngagementPath(ref list))
		{
			item = this.patrolPath.GetClosestToPoint(base.transform.position);
		}
		else
		{
			flag = true;
			item = list[list.Count - 1];
		}
		if (Vector3.Distance(this.finalDestination, closestToPoint.transform.position) > 2f)
		{
			if (closestToPoint == item)
			{
				this.currentPath.Clear();
				this.currentPath.Add(closestToPoint.transform.position);
				this.currentPathIndex = -1;
				this.pathLooping = false;
				this.finalDestination = closestToPoint.transform.position;
				return;
			}
			if (AStarPath.FindPath(item, closestToPoint, out basePathNodes, out single))
			{
				this.currentPath.Clear();
				if (flag)
				{
					for (int i = 0; i < list.Count - 1; i++)
					{
						this.currentPath.Add(list[i].transform.position);
					}
				}
				foreach (BasePathNode basePathNode in basePathNodes)
				{
					this.currentPath.Add(basePathNode.transform.position);
				}
				this.currentPathIndex = -1;
				this.pathLooping = false;
				this.finalDestination = closestToPoint.transform.position;
			}
		}
	}

	public void UpdateTargetList()
	{
		List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(base.transform.position, this.searchRange, list, 133120, QueryTriggerInteraction.Collide);
		foreach (BaseEntity baseEntity in list)
		{
			if (!(baseEntity is BasePlayer))
			{
				continue;
			}
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if (basePlayer.IsDead() || basePlayer is Scientist || !this.VisibilityTest(baseEntity))
			{
				continue;
			}
			bool flag = false;
			foreach (BradleyAPC.TargetInfo targetInfo in this.targetList)
			{
				if (targetInfo.entity != baseEntity)
				{
					continue;
				}
				targetInfo.lastSeenTime = UnityEngine.Time.time;
				flag = true;
				goto Label0;
			}
		Label0:
			if (flag)
			{
				continue;
			}
			BradleyAPC.TargetInfo targetInfo1 = Facepunch.Pool.Get<BradleyAPC.TargetInfo>();
			targetInfo1.Setup(baseEntity, UnityEngine.Time.time);
			this.targetList.Add(targetInfo1);
		}
		for (int i = this.targetList.Count - 1; i >= 0; i--)
		{
			BradleyAPC.TargetInfo item = this.targetList[i];
			BasePlayer basePlayer1 = item.entity as BasePlayer;
			if (item.entity == null || UnityEngine.Time.time - item.lastSeenTime > this.memoryDuration || basePlayer1.IsDead())
			{
				this.targetList.Remove(item);
				Facepunch.Pool.Free<BradleyAPC.TargetInfo>(ref item);
			}
		}
		Facepunch.Pool.FreeList<BaseEntity>(ref list);
		this.targetList.Sort(new Comparison<BradleyAPC.TargetInfo>(this.SortTargets));
	}

	public void UpdateTargetVisibilities()
	{
		foreach (BradleyAPC.TargetInfo targetInfo in this.targetList)
		{
			if (!targetInfo.IsValid() || !this.VisibilityTest(targetInfo.entity))
			{
				continue;
			}
			targetInfo.lastSeenTime = UnityEngine.Time.time;
			targetInfo.lastSeenPosition = targetInfo.entity.transform.position;
		}
	}

	public bool VisibilityTest(BaseEntity ent)
	{
		if (ent == null)
		{
			return false;
		}
		if (Vector3.Distance(ent.transform.position, base.transform.position) >= this.viewDistance)
		{
			return false;
		}
		bool flag = false;
		if (!(ent is BasePlayer))
		{
			Debug.LogWarning("Standard vis test!");
			flag = base.IsVisible(ent.CenterPoint(), Single.PositiveInfinity);
		}
		else
		{
			BasePlayer basePlayer = ent as BasePlayer;
			Vector3 vector3 = this.mainTurret.transform.position;
			flag = (base.IsVisible(basePlayer.eyes.position, vector3, Single.PositiveInfinity) ? true : base.IsVisible(basePlayer.transform.position, vector3, Single.PositiveInfinity));
		}
		object obj = Interface.CallHook("CanBradleyApcTarget", this, ent);
		if (!(obj as bool))
		{
			return flag;
		}
		return (bool)obj;
	}

	[Serializable]
	public class TargetInfo : Facepunch.Pool.IPooled
	{
		public float damageReceivedFrom;

		public BaseEntity entity;

		public float lastSeenTime;

		public Vector3 lastSeenPosition;

		public TargetInfo()
		{
		}

		public void EnterPool()
		{
			this.entity = null;
			this.lastSeenPosition = Vector3.zero;
			this.lastSeenTime = 0f;
		}

		public float GetPriorityScore(BradleyAPC apc)
		{
			BasePlayer basePlayer = this.entity as BasePlayer;
			if (!basePlayer)
			{
				return 0f;
			}
			float single = Vector3.Distance(this.entity.transform.position, apc.transform.position);
			float single1 = (1f - Mathf.InverseLerp(10f, 80f, single)) * 50f;
			float single2 = Mathf.InverseLerp(4f, 20f, (basePlayer.GetHeldEntity() == null ? 0f : basePlayer.GetHeldEntity().hostileScore)) * 100f;
			float single3 = Mathf.InverseLerp(10f, 3f, UnityEngine.Time.time - this.lastSeenTime) * 100f;
			float single4 = Mathf.InverseLerp(0f, 100f, this.damageReceivedFrom) * 50f;
			return single1 + single2 + single4 + single3;
		}

		public bool IsValid()
		{
			return this.entity != null;
		}

		public bool IsVisible()
		{
			if (this.lastSeenTime == -1f)
			{
				return false;
			}
			return UnityEngine.Time.time - this.lastSeenTime < BradleyAPC.sightUpdateRate * 2f;
		}

		public void LeavePool()
		{
		}

		public void Setup(BaseEntity ent, float time)
		{
			this.entity = ent;
			this.lastSeenTime = time;
		}
	}
}