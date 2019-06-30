using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseRidableAnimal : BaseVehicle
{
	private Vector3 lastMoveDirection;

	public GameObjectRef saddlePrefab;

	public EntityRef saddleRef;

	public Transform movementLOSOrigin;

	public SoundPlayer sprintSounds;

	public SoundPlayer largeWhinny;

	public Transform animalFront;

	public GameObjectRef eatEffect;

	public GameObjectRef CorpsePrefab;

	[Header("Obstacles")]
	public float maxWaterDepth = 1.5f;

	public float roadSpeedBonus = 2f;

	[Header("Movement")]
	public BaseRidableAnimal.RunState currentRunState = BaseRidableAnimal.RunState.stopped;

	public float walkSpeed = 2f;

	public float trotSpeed = 7f;

	public float runSpeed = 14f;

	[Header("Stamina")]
	public float staminaSeconds = 10f;

	public float currentMaxStaminaSeconds = 10f;

	public float maxStaminaSeconds = 20f;

	public float staminaCoreLossRatio = 0.1f;

	public float staminaCoreSpeedBonus = 3f;

	public float staminaReplenishRatioMoving = 0.5f;

	public float staminaReplenishRatioStanding = 1f;

	public float calorieToStaminaRatio = 0.1f;

	public float hydrationToStaminaRatio = 0.5f;

	public float maxStaminaCoreFromWater = 0.5f;

	[ServerVar(Help="How long before a horse dies unattended")]
	public static float decayminutes;

	private float nextEatTime;

	private float lastEatTime = Single.NegativeInfinity;

	public float maxSpeed = 5f;

	public float currentSpeed;

	public float desiredRotation;

	public float turnSpeed = 30f;

	private float lastInputTime;

	private float forwardHeldSeconds;

	private float backwardHeldSeconds;

	private float sprintHeldSeconds;

	private float lastSprintPressedTime;

	private float lastForwardPressedTime;

	private float lastBackwardPressedTime;

	private float timeInMoveState;

	private bool onIdealTerrain;

	private float nextIdealTerrainCheckTime;

	private float nextStandTime;

	private float obstacleDetectionRadius = 0.25f;

	private Vector3 currentVelocity;

	private Vector3 averagedUp = Vector3.up;

	public Transform[] groundSampleOffsets;

	private float nextGroundNormalUpdateTime;

	private Vector3 targetUp = Vector3.up;

	public float animalPitchClamp = 90f;

	public float animalRollClamp;

	private float nextObstacleCheckTime;

	private float cachedObstacleDistance = Single.PositiveInfinity;

	private float timeAlive;

	static BaseRidableAnimal()
	{
		BaseRidableAnimal.decayminutes = 180f;
	}

	public BaseRidableAnimal()
	{
	}

	public void AnimalDecay()
	{
		if (base.healthFraction == 0f || base.IsDestroyed)
		{
			return;
		}
		if (Time.time < this.lastInputTime + 600f)
		{
			return;
		}
		if (Time.time < this.lastEatTime + 600f)
		{
			return;
		}
		float single = 1f / BaseRidableAnimal.decayminutes;
		float single1 = (!this.IsOutside() ? 2f : 1f);
		this.Hurt(this.MaxHealth() * single * single1, DamageType.Decay, this, false);
	}

	public bool CanInitiateSprint()
	{
		return this.staminaSeconds > 4f;
	}

	public bool CanSprint()
	{
		return this.staminaSeconds > 0f;
	}

	public bool CanStand()
	{
		if (this.nextStandTime > Time.time)
		{
			return false;
		}
		if (this.mountPoints[0].mountable == null)
		{
			return false;
		}
		bool flag = false;
		List<Collider> list = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(this.mountPoints[0].mountable.eyeOverride.transform.position - (base.transform.forward * 1f), 2f, list, 2162689, QueryTriggerInteraction.Collide);
		if (list.Count > 0)
		{
			flag = true;
		}
		Pool.FreeList<Collider>(ref list);
		return !flag;
	}

	public void DoNetworkUpdate()
	{
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	public void EatNearbyFood()
	{
		if (Time.time < this.nextEatTime)
		{
			return;
		}
		float single = this.StaminaCoreFraction();
		this.nextEatTime = Time.time + UnityEngine.Random.Range(2f, 3f) + Mathf.InverseLerp(0.5f, 1f, single) * 4f;
		if (single >= 1f)
		{
			return;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(base.transform.position + (base.transform.forward * 1.5f), 2f, list, 67109377, QueryTriggerInteraction.Collide);
		list.Sort((BaseEntity a, BaseEntity b) => b is DroppedItem.CompareTo(a is DroppedItem));
		foreach (BaseEntity baseEntity in list)
		{
			if (baseEntity.isClient)
			{
				continue;
			}
			DroppedItem droppedItem = baseEntity as DroppedItem;
			if (droppedItem && droppedItem.item != null && droppedItem.item.info.category == ItemCategory.Food)
			{
				ItemModConsumable component = droppedItem.item.info.GetComponent<ItemModConsumable>();
				if (component)
				{
					base.ClientRPC(null, "Eat");
					this.lastEatTime = Time.time;
					float ifType = component.GetIfType(MetabolismAttribute.Type.Calories);
					float ifType1 = component.GetIfType(MetabolismAttribute.Type.Hydration);
					float single1 = component.GetIfType(MetabolismAttribute.Type.Health) + component.GetIfType(MetabolismAttribute.Type.HealthOverTime);
					this.ReplenishStaminaCore(ifType, ifType1);
					this.Heal(single1 * 2f);
					droppedItem.item.UseItem(1);
					if (droppedItem.item.amount > 0)
					{
						break;
					}
					droppedItem.Kill(BaseNetworkable.DestroyMode.None);
					Pool.FreeList<BaseEntity>(ref list);
					return;
				}
			}
			CollectibleEntity collectibleEntity = baseEntity as CollectibleEntity;
			if (!collectibleEntity || !collectibleEntity.IsFood())
			{
				PlantEntity plantEntity = baseEntity as PlantEntity;
				if (!plantEntity || !plantEntity.CanPick())
				{
					continue;
				}
				plantEntity.PickFruit(null);
				Pool.FreeList<BaseEntity>(ref list);
				return;
			}
			else
			{
				collectibleEntity.DoPickup(null);
				Pool.FreeList<BaseEntity>(ref list);
				return;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	public new void FixedUpdate()
	{
		base.FixedUpdate();
		if (base.isClient)
		{
			return;
		}
		this.timeAlive += Time.fixedDeltaTime;
		this.UpdateOnIdealTerrain();
		this.UpdateStamina(Time.fixedDeltaTime);
		if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
		{
			this.EatNearbyFood();
		}
		this.UpdateMovement();
	}

	public float GetBreathingDelay()
	{
		switch (this.currentRunState)
		{
			case BaseRidableAnimal.RunState.walk:
			{
				return 8f;
			}
			case BaseRidableAnimal.RunState.run:
			{
				return 5f;
			}
			case BaseRidableAnimal.RunState.sprint:
			{
				return 2.5f;
			}
			default:
			{
				return -1f;
			}
		}
	}

	public float GetDesiredVelocity()
	{
		switch (this.currentRunState)
		{
			case BaseRidableAnimal.RunState.walk:
			{
				return this.walkSpeed;
			}
			case BaseRidableAnimal.RunState.run:
			{
				return this.trotSpeed;
			}
			case BaseRidableAnimal.RunState.sprint:
			{
				return this.GetRunSpeed();
			}
			default:
			{
				return 0f;
			}
		}
	}

	public override Vector3 GetLocalVelocityServer()
	{
		return this.currentVelocity;
	}

	public float GetObstacleDistance()
	{
		if (Time.time >= this.nextObstacleCheckTime)
		{
			if (this.currentSpeed > 0f || this.GetDesiredVelocity() > 0f)
			{
				this.cachedObstacleDistance = this.ObstacleDistanceCheck(this.GetDesiredVelocity() + 2f);
			}
			this.nextObstacleCheckTime = Time.time + UnityEngine.Random.Range(0.25f, 0.35f);
		}
		return this.cachedObstacleDistance;
	}

	public float GetRunSpeed()
	{
		float single = this.runSpeed;
		float single1 = Mathf.InverseLerp(this.maxStaminaSeconds * 0.5f, this.maxStaminaSeconds, this.currentMaxStaminaSeconds) * this.staminaCoreSpeedBonus;
		return this.runSpeed + single1 + (this.onIdealTerrain ? this.roadSpeedBonus : 0f);
	}

	public BaseMountable GetSaddle()
	{
		if (!this.saddleRef.IsValid(base.isServer))
		{
			return null;
		}
		return this.saddleRef.Get(base.isServer).GetComponent<BaseMountable>();
	}

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.ioEntity != null)
		{
			this.staminaSeconds = info.msg.ioEntity.genericFloat1;
			this.currentMaxStaminaSeconds = info.msg.ioEntity.genericFloat2;
			if (!info.fromDisk)
			{
				this.currentRunState = (BaseRidableAnimal.RunState)info.msg.ioEntity.genericInt1;
			}
		}
	}

	public virtual void MarkDistanceTravelled(float amount)
	{
	}

	public override float MaxVelocity()
	{
		return this.maxSpeed * 1.5f;
	}

	public void ModifyRunState(int dir)
	{
		if (this.currentRunState == BaseRidableAnimal.RunState.stopped && dir < 0 || this.currentRunState == BaseRidableAnimal.RunState.sprint && dir > 0)
		{
			return;
		}
		this.SwitchMoveState((int)this.currentRunState + dir);
	}

	private float NormalizeAngle(float angle)
	{
		if (angle > 180f)
		{
			angle -= 360f;
		}
		return angle;
	}

	public float ObstacleDistance()
	{
		RaycastHit raycastHit;
		float single = 10f;
		Vector3 vector3 = QuaternionEx.LookRotationForcedUp(base.transform.forward, base.transform.up) * Vector3.forward;
		if (Physics.SphereCast(this.movementLOSOrigin.transform.position, this.obstacleDetectionRadius, vector3, out raycastHit, single, 1218519297))
		{
			single = raycastHit.distance;
		}
		return single;
	}

	public float ObstacleDistanceCheck(float speed = 15f)
	{
		RaycastHit raycastHit;
		Vector3 vector3 = base.transform.position;
		int num = 1;
		int num1 = Mathf.CeilToInt((float)(Mathf.Max(2, Mathf.CeilToInt(speed)) / num));
		float single = 0f;
		Vector3 vector31 = QuaternionEx.LookRotationForcedUp(base.transform.forward, Vector3.up) * Vector3.forward;
		Vector3 vector32 = this.movementLOSOrigin.transform.position;
		vector32.y = base.transform.position.y;
		Vector3 vector33 = base.transform.up;
		for (int i = 0; i < num1; i++)
		{
			float single1 = (float)num;
			bool flag = false;
			float single2 = 0f;
			Vector3 vector34 = Vector3.zero;
			Vector3 vector35 = Vector3.up;
			Vector3 vector36 = vector32 + (Vector3.up * 0.5f);
			if (Physics.SphereCast(vector36, this.obstacleDetectionRadius, vector31, out raycastHit, single1, 1486954753))
			{
				single2 = raycastHit.distance;
				vector34 = raycastHit.point;
				vector35 = raycastHit.normal;
				flag = true;
			}
			if (!flag)
			{
				if (!TransformUtil.GetGroundInfo((vector36 + (vector31 * 1f)) + (Vector3.up * 2f), out vector34, out vector35, 4f, 278986753, null))
				{
					return single;
				}
				single2 = Vector3.Distance(vector36, vector34);
				if (WaterLevel.Test(vector34 + (Vector3.one * this.maxWaterDepth)))
				{
					vector35 = -base.transform.forward;
				}
				flag = true;
			}
			if (flag)
			{
				float single3 = Vector3.Angle(vector33, vector35);
				float single4 = Vector3.Angle(vector35, Vector3.up);
				if (single3 > 45f && single4 > 50f)
				{
					return single;
				}
			}
			single += single2;
			vector31 = QuaternionEx.LookRotationForcedUp(base.transform.forward, vector35) * Vector3.forward;
			vector32 = vector34;
		}
		return single;
	}

	public override void OnKilled(HitInfo hitInfo = null)
	{
		Assert.IsTrue(base.isServer, "OnKilled called on client!");
		BaseCorpse baseCorpse = base.DropCorpse(this.CorpsePrefab.resourcePath);
		if (baseCorpse)
		{
			baseCorpse.Spawn();
			baseCorpse.TakeChildren(this);
		}
		base.Invoke(new Action(this.KillMessage), 0.5f);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("BaseRidableAnimal.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override bool PhysicsDriven()
	{
		return true;
	}

	public override void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		base.SetFlag(BaseEntity.Flags.Reserved8, false, true, true);
	}

	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		base.SetFlag(BaseEntity.Flags.Reserved8, true, true, true);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		this.RiderInput(inputState, player);
	}

	public void ReplenishStamina(float amount)
	{
		float single = 1f + Mathf.InverseLerp(this.maxStaminaSeconds * 0.5f, this.maxStaminaSeconds, this.currentMaxStaminaSeconds);
		amount *= single;
		amount = Mathf.Min(this.currentMaxStaminaSeconds - this.staminaSeconds, amount);
		float single1 = Mathf.Min(this.currentMaxStaminaSeconds - this.staminaCoreLossRatio * amount, amount * this.staminaCoreLossRatio);
		this.currentMaxStaminaSeconds = Mathf.Clamp(this.currentMaxStaminaSeconds - single1, 0f, this.maxStaminaSeconds);
		this.staminaSeconds = Mathf.Clamp(this.staminaSeconds + single1 / this.staminaCoreLossRatio, 0f, this.currentMaxStaminaSeconds);
	}

	public void ReplenishStaminaCore(float calories, float hydration)
	{
		float single = calories * this.calorieToStaminaRatio;
		float single1 = hydration * this.hydrationToStaminaRatio;
		single1 = Mathf.Min(this.maxStaminaCoreFromWater - this.currentMaxStaminaSeconds, single1);
		if (single1 < 0f)
		{
			single1 = 0f;
		}
		float single2 = single + single1;
		this.currentMaxStaminaSeconds = Mathf.Clamp(this.currentMaxStaminaSeconds + single2, 0f, this.maxStaminaSeconds);
	}

	public virtual void RiderInput(InputState inputState, BasePlayer player)
	{
		float single = Time.time - this.lastInputTime;
		this.lastInputTime = Time.time;
		single = Mathf.Clamp(single, 0f, 1f);
		Vector3 vector3 = Vector3.zero;
		this.timeInMoveState += single;
		if (inputState != null)
		{
			if (!inputState.IsDown(BUTTON.FORWARD))
			{
				this.forwardHeldSeconds = 0f;
			}
			else
			{
				this.lastForwardPressedTime = Time.time;
				this.forwardHeldSeconds += single;
			}
			if (!inputState.IsDown(BUTTON.BACKWARD))
			{
				this.backwardHeldSeconds = 0f;
			}
			else
			{
				this.lastBackwardPressedTime = Time.time;
				this.backwardHeldSeconds += single;
			}
			if (!inputState.IsDown(BUTTON.SPRINT))
			{
				this.sprintHeldSeconds = 0f;
			}
			else
			{
				this.lastSprintPressedTime = Time.time;
				this.sprintHeldSeconds += single;
			}
			if (inputState.IsDown(BUTTON.DUCK) && this.CanStand() && (this.currentRunState == BaseRidableAnimal.RunState.stopped || this.currentRunState == BaseRidableAnimal.RunState.walk && this.currentSpeed < 1f))
			{
				base.ClientRPC(null, "Stand");
				this.nextStandTime = Time.time + 3f;
				this.currentSpeed = 0f;
			}
			if (Time.time < this.nextStandTime)
			{
				this.forwardHeldSeconds = 0f;
				this.backwardHeldSeconds = 0f;
			}
			if (this.forwardHeldSeconds <= 0f)
			{
				if (this.backwardHeldSeconds > 1f)
				{
					this.ModifyRunState(-1);
					this.backwardHeldSeconds = 0.1f;
				}
				else if (this.backwardHeldSeconds == 0f && this.forwardHeldSeconds == 0f && this.timeInMoveState > 1f && this.currentRunState != BaseRidableAnimal.RunState.stopped)
				{
					this.ModifyRunState(-1);
				}
			}
			else if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
			{
				this.SwitchMoveState(BaseRidableAnimal.RunState.walk);
			}
			else if (this.currentRunState == BaseRidableAnimal.RunState.walk)
			{
				if (this.sprintHeldSeconds > 0f)
				{
					this.SwitchMoveState(BaseRidableAnimal.RunState.run);
				}
			}
			else if (this.currentRunState == BaseRidableAnimal.RunState.run && this.sprintHeldSeconds > 1f && this.CanInitiateSprint())
			{
				this.SwitchMoveState(BaseRidableAnimal.RunState.sprint);
			}
			if (this.currentRunState == BaseRidableAnimal.RunState.sprint && (!this.CanSprint() || Time.time - this.lastSprintPressedTime > 5f))
			{
				this.ModifyRunState(-1);
			}
			if (inputState.IsDown(BUTTON.RIGHT))
			{
				if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
				{
					this.ModifyRunState(1);
				}
				this.desiredRotation = 1f;
				return;
			}
			if (inputState.IsDown(BUTTON.LEFT))
			{
				if (this.currentRunState == BaseRidableAnimal.RunState.stopped)
				{
					this.ModifyRunState(1);
				}
				this.desiredRotation = -1f;
				return;
			}
			this.desiredRotation = 0f;
		}
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		info.msg.ioEntity = Pool.Get<ProtoBuf.IOEntity>();
		info.msg.ioEntity.genericFloat1 = this.staminaSeconds;
		info.msg.ioEntity.genericFloat2 = this.currentMaxStaminaSeconds;
		if (!info.forDisk)
		{
			info.msg.ioEntity.genericInt1 = (int)this.currentRunState;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.DoNetworkUpdate), UnityEngine.Random.Range(0f, 0.2f), 0.333f);
		base.InvokeRandomized(new Action(this.AnimalDecay), UnityEngine.Random.Range(30f, 60f), 60f, 6f);
	}

	public float StaminaCoreFraction()
	{
		return Mathf.InverseLerp(0f, this.maxStaminaSeconds, this.currentMaxStaminaSeconds);
	}

	public void SwitchMoveState(BaseRidableAnimal.RunState newState)
	{
		if (newState == this.currentRunState)
		{
			return;
		}
		this.currentRunState = newState;
		this.timeInMoveState = 0f;
		base.SetFlag(BaseEntity.Flags.Reserved8, this.currentRunState == BaseRidableAnimal.RunState.sprint, false, false);
	}

	public void UpdateGroundNormal()
	{
		Vector3 vector3;
		Vector3 vector31;
		if (Time.time >= this.nextGroundNormalUpdateTime)
		{
			this.nextGroundNormalUpdateTime = Time.time + UnityEngine.Random.Range(0.2f, 0.35f);
			this.targetUp = this.averagedUp;
			Transform[] transformArrays = this.groundSampleOffsets;
			for (int i = 0; i < (int)transformArrays.Length; i++)
			{
				if (!TransformUtil.GetGroundInfo(transformArrays[i].position + (Vector3.up * 2f), out vector3, out vector31, 4f, 278986753, null))
				{
					this.targetUp += Vector3.up;
				}
				else
				{
					this.targetUp += vector31;
				}
			}
			this.targetUp /= (float)((int)this.groundSampleOffsets.Length + 1);
		}
		this.averagedUp = Vector3.Lerp(this.averagedUp, this.targetUp, Time.deltaTime * 4f);
	}

	public void UpdateMovement()
	{
		RaycastHit raycastHit;
		Vector3 vector3;
		Vector3 vector31;
		float single = this.WaterFactor();
		if (single > 1f && !base.IsDestroyed)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
			return;
		}
		if (single >= 0.3f && this.currentRunState > BaseRidableAnimal.RunState.run)
		{
			this.currentRunState = BaseRidableAnimal.RunState.run;
		}
		else if (single >= 0.45f && this.currentRunState > BaseRidableAnimal.RunState.walk)
		{
			this.currentRunState = BaseRidableAnimal.RunState.walk;
		}
		if (Time.time - this.lastInputTime > 3f)
		{
			this.currentRunState = BaseRidableAnimal.RunState.stopped;
			this.desiredRotation = 0f;
		}
		float desiredVelocity = this.GetDesiredVelocity();
		Vector3 vector32 = Vector3.forward * Mathf.Sign(desiredVelocity);
		base.transform.hasChanged = true;
		float single1 = Mathf.InverseLerp(1f, 12f, this.GetObstacleDistance());
		float single2 = Mathf.Min(Mathf.Clamp01(Mathf.Min(1f - Mathf.InverseLerp(20f, 35f, Vector3.Angle(Vector3.up, this.averagedUp)) + 0.2f, single1)) * this.GetRunSpeed(), desiredVelocity);
		float single3 = (single2 < this.currentSpeed ? 3f : 1f);
		this.currentSpeed = Mathf.Lerp(this.currentSpeed, single2, Time.deltaTime * single3);
		if (Mathf.Abs(this.currentSpeed) < 2f && desiredVelocity == 0f)
		{
			this.currentSpeed = Mathf.MoveTowards(this.currentSpeed, 0f, Time.deltaTime * 2f);
		}
		if (single1 == 0f)
		{
			this.currentSpeed = 0f;
		}
		float single4 = 1f - Mathf.InverseLerp(2f, 7f, this.currentSpeed);
		single4 = (single4 + 1f) / 2f;
		base.transform.Rotate(Vector3.up, this.desiredRotation * Time.fixedDeltaTime * this.turnSpeed * single4);
		Vector3 vector33 = base.transform.TransformDirection(vector32);
		Vector3 vector34 = vector33.normalized;
		float single5 = this.currentSpeed * Time.fixedDeltaTime;
		Vector3 vector35 = base.transform.position + ((vector33 * single5) * Mathf.Sign(this.currentSpeed));
		this.currentVelocity = vector33 * this.currentSpeed;
		this.UpdateGroundNormal();
		if (this.currentSpeed > 0f || this.timeAlive < 2f)
		{
			if (!Physics.SphereCast(this.animalFront.transform.position, this.obstacleDetectionRadius, vector34, out raycastHit, single5, 1503731969))
			{
				if (!TransformUtil.GetGroundInfo(vector35 + (Vector3.up * 1.25f), out vector3, out vector31, 3f, 278986753, null))
				{
					this.currentSpeed = 0f;
					return;
				}
				this.MarkDistanceTravelled(single5);
				base.transform.position = vector3;
				Quaternion quaternion = QuaternionEx.LookRotationForcedUp(base.transform.forward, this.averagedUp);
				Vector3 vector36 = quaternion.eulerAngles;
				if (vector36.z > 180f)
				{
					vector36.z -= 360f;
				}
				else if (vector36.z < -180f)
				{
					vector36.z += 360f;
				}
				vector36.z = Mathf.Clamp(vector36.z, -10f, 10f);
				base.transform.rotation = Quaternion.Euler(vector36);
				return;
			}
			this.currentSpeed = 0f;
		}
	}

	public void UpdateOnIdealTerrain()
	{
		if (Time.time < this.nextIdealTerrainCheckTime)
		{
			return;
		}
		this.nextIdealTerrainCheckTime = Time.time + UnityEngine.Random.Range(1f, 2f);
		this.onIdealTerrain = false;
		if (TerrainMeta.TopologyMap != null && (TerrainMeta.TopologyMap.GetTopology(base.transform.position) & 2048) != 0)
		{
			this.onIdealTerrain = true;
		}
	}

	public void UpdateStamina(float delta)
	{
		if (this.currentRunState == BaseRidableAnimal.RunState.sprint)
		{
			this.UseStamina(delta);
			return;
		}
		if (this.currentRunState == BaseRidableAnimal.RunState.run)
		{
			this.ReplenishStamina(this.staminaReplenishRatioMoving * delta);
			return;
		}
		this.ReplenishStamina(this.staminaReplenishRatioStanding * delta);
	}

	public void UseStamina(float amount)
	{
		if (this.onIdealTerrain)
		{
			amount *= 0.5f;
		}
		this.staminaSeconds -= amount;
		if (this.staminaSeconds <= 0f)
		{
			this.staminaSeconds = 0f;
		}
	}

	public enum RunState
	{
		stopped = 1,
		walk = 2,
		run = 3,
		sprint = 4
	}
}