using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class NPCPlayer : BasePlayer
{
	public Vector3 finalDestination;

	[NonSerialized]
	private float randomOffset;

	[NonSerialized]
	private Vector3 spawnPos;

	public PlayerInventoryProperties[] loadouts;

	public LayerMask movementMask = 429990145;

	public NavMeshAgent NavAgent;

	public float damageScale = 1f;

	private bool _isDormant;

	protected float lastGunShotTime;

	private float triggerEndTime;

	protected float nextTriggerTime;

	private float lastThinkTime;

	private Vector3 lastPos;

	protected bool _traversingNavMeshLink;

	protected OffMeshLinkData _currentNavMeshLink;

	protected string _currentNavMeshLinkName;

	protected Quaternion _currentNavMeshLinkOrientation;

	protected Vector3 _currentNavMeshLinkEndPos;

	public bool AgencyUpdateRequired
	{
		get;
		set;
	}

	public virtual bool HasPath
	{
		get
		{
			if (!this.IsNavRunning())
			{
				return false;
			}
			return this.NavAgent.hasPath;
		}
	}

	public virtual bool IsDormant
	{
		get
		{
			return this._isDormant;
		}
		set
		{
			this._isDormant = value;
			bool flag = this._isDormant;
		}
	}

	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	public virtual bool IsOnNavMeshLink
	{
		get
		{
			if (!this.IsNavRunning())
			{
				return false;
			}
			return this.NavAgent.isOnOffMeshLink;
		}
	}

	public bool IsOnOffmeshLinkAndReachedNewCoord
	{
		get;
		set;
	}

	protected override float PositionTickRate
	{
		get
		{
			return 0.1f;
		}
	}

	public NPCPlayer()
	{
	}

	public virtual float AmmoFractionRemaining()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		if (!attackEntity)
		{
			return 0f;
		}
		return attackEntity.AmmoFraction();
	}

	public virtual void AttemptReload()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		if (attackEntity == null)
		{
			return;
		}
		if (attackEntity.CanReload())
		{
			attackEntity.ServerReload();
		}
	}

	public void CancelBurst(float delay = 0.2f)
	{
		if (this.triggerEndTime > Time.time + delay)
		{
			this.triggerEndTime = Time.time + delay;
		}
	}

	private void CompleteNavMeshLink()
	{
		this.NavAgent.ActivateCurrentOffMeshLink(true);
		this.NavAgent.CompleteOffMeshLink();
		this.NavAgent.isStopped = false;
		this.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
	}

	public virtual float DesiredMoveSpeed()
	{
		float single = Mathf.Sin(Time.time + this.randomOffset);
		return base.GetSpeed(single, 0f);
	}

	public override bool EligibleForWounding(HitInfo info)
	{
		return false;
	}

	public void EquipTest()
	{
		this.EquipWeapon();
	}

	public virtual void EquipWeapon()
	{
		Item slot = this.inventory.containerBelt.GetSlot(0);
		if (slot != null)
		{
			base.UpdateActiveItem(this.inventory.containerBelt.GetSlot(0).uid);
			BaseEntity heldEntity = slot.GetHeldEntity();
			if (heldEntity != null)
			{
				AttackEntity component = heldEntity.GetComponent<AttackEntity>();
				if (component != null)
				{
					component.TopUpAmmo();
				}
			}
		}
	}

	public virtual float GetAimConeScale()
	{
		return 1f;
	}

	public virtual Vector3 GetAimDirection()
	{
		if (Vector3Ex.Distance2D(this.finalDestination, this.GetPosition()) < 1f)
		{
			return this.eyes.BodyForward();
		}
		Vector3 position = (this.finalDestination - this.GetPosition()).normalized;
		return new Vector3(position.x, 0f, position.z);
	}

	public AttackEntity GetAttackEntity()
	{
		return base.GetHeldEntity() as AttackEntity;
	}

	public BaseProjectile GetGun()
	{
		AttackEntity heldEntity = base.GetHeldEntity() as AttackEntity;
		if (heldEntity == null)
		{
			return null;
		}
		BaseProjectile baseProjectile = heldEntity as BaseProjectile;
		if (baseProjectile)
		{
			return baseProjectile;
		}
		return null;
	}

	public Vector3 GetPosition()
	{
		return base.transform.position;
	}

	private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
	{
		if (!this._traversingNavMeshLink)
		{
			this.HandleNavMeshLinkTraversalStart(delta);
		}
		this.HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
		if (this.IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
		{
			this.CompleteNavMeshLink();
		}
	}

	private bool HandleNavMeshLinkTraversalStart(float delta)
	{
		OffMeshLinkData navAgent = this.NavAgent.currentOffMeshLinkData;
		if (!navAgent.valid || !navAgent.activated)
		{
			return false;
		}
		Vector3 vector3 = (navAgent.endPos - navAgent.startPos).normalized;
		vector3.y = 0f;
		Vector3 navAgent1 = this.NavAgent.desiredVelocity;
		navAgent1.y = 0f;
		if (Vector3.Dot(navAgent1, vector3) < 0.1f)
		{
			this.CompleteNavMeshLink();
			return false;
		}
		this._currentNavMeshLink = navAgent;
		this._currentNavMeshLinkName = navAgent.linkType.ToString();
		if ((this.ServerPosition - navAgent.startPos).sqrMagnitude <= (this.ServerPosition - navAgent.endPos).sqrMagnitude)
		{
			this._currentNavMeshLinkEndPos = navAgent.endPos;
			this._currentNavMeshLinkOrientation = Quaternion.LookRotation((navAgent.endPos + (Vector3.up * (navAgent.startPos.y - navAgent.endPos.y))) - navAgent.startPos);
		}
		else
		{
			this._currentNavMeshLinkEndPos = navAgent.startPos;
			this._currentNavMeshLinkOrientation = Quaternion.LookRotation((navAgent.startPos + (Vector3.up * (navAgent.endPos.y - navAgent.startPos.y))) - navAgent.endPos);
		}
		this._traversingNavMeshLink = true;
		this.NavAgent.ActivateCurrentOffMeshLink(false);
		this.NavAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
		if (!(this._currentNavMeshLinkName == "OpenDoorLink") && !(this._currentNavMeshLinkName == "JumpRockLink"))
		{
		}
		return true;
	}

	private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
	{
		if (this._currentNavMeshLinkName == "OpenDoorLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this._currentNavMeshLinkEndPos, this.NavAgent.speed * delta);
			return;
		}
		if (this._currentNavMeshLinkName == "JumpRockLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this._currentNavMeshLinkEndPos, this.NavAgent.speed * delta);
			return;
		}
		if (this._currentNavMeshLinkName == "JumpFoundationLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, this._currentNavMeshLinkEndPos, this.NavAgent.speed * delta);
			return;
		}
		moveToPosition = Vector3.MoveTowards(moveToPosition, this._currentNavMeshLinkEndPos, this.NavAgent.speed * delta);
	}

	public virtual bool IsLoadBalanced()
	{
		return false;
	}

	private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
	{
		if ((moveToPosition - this._currentNavMeshLinkEndPos).sqrMagnitude >= 0.01f)
		{
			return false;
		}
		moveToPosition = this._currentNavMeshLinkEndPos;
		this._traversingNavMeshLink = false;
		this._currentNavMeshLink = new OffMeshLinkData();
		this._currentNavMeshLinkName = string.Empty;
		this._currentNavMeshLinkOrientation = Quaternion.identity;
		this.CompleteNavMeshLink();
		return true;
	}

	public virtual bool IsNavRunning()
	{
		return false;
	}

	public virtual bool IsReloading()
	{
		AttackEntity attackEntity = this.GetAttackEntity();
		if (!attackEntity)
		{
			return false;
		}
		return attackEntity.ServerIsReloading();
	}

	public bool MeleeAttack()
	{
		AttackEntity heldEntity = base.GetHeldEntity() as AttackEntity;
		if (heldEntity == null)
		{
			return false;
		}
		BaseMelee baseMelee = heldEntity as BaseMelee;
		if (baseMelee == null)
		{
			return false;
		}
		baseMelee.ServerUse(this.damageScale);
		return true;
	}

	public virtual void MovementUpdate(float delta)
	{
		if (base.isClient)
		{
			return;
		}
		if (!this.IsAlive() || base.IsWounded() || !base.isMounted && !this.IsNavRunning())
		{
			return;
		}
		if (this.IsDormant || !this.syncPosition)
		{
			if (this.IsNavRunning())
			{
				this.NavAgent.destination = this.ServerPosition;
			}
			return;
		}
		Vector3 navAgent = base.transform.position;
		if (this.IsOnNavMeshLink)
		{
			this.HandleNavMeshLinkTraversal(delta, ref navAgent);
		}
		else if (this.HasPath)
		{
			navAgent = this.NavAgent.nextPosition;
		}
		if (!this.ValidateNextPosition(ref navAgent))
		{
			return;
		}
		this.UpdateSpeed(delta);
		this.UpdatePositionAndRotation(navAgent);
	}

	public void RandomMove()
	{
		Vector2 vector2 = UnityEngine.Random.insideUnitCircle * 8f;
		this.SetDestination(this.spawnPos + new Vector3(vector2.x, 0f, vector2.y));
	}

	public virtual void Resume()
	{
	}

	public override void ServerInit()
	{
		if (base.isClient)
		{
			return;
		}
		this.spawnPos = this.GetPosition();
		this.randomOffset = UnityEngine.Random.Range(0f, 1f);
		base.ServerInit();
		this.UpdateNetworkGroup();
		if (this.loadouts == null || this.loadouts.Length == 0)
		{
			Debug.LogWarningFormat("Loadout for NPC {0} was empty.", new object[] { base.name });
		}
		else
		{
			this.loadouts[UnityEngine.Random.Range(0, (int)this.loadouts.Length)].GiveToPlayer(this);
		}
		if (!this.IsLoadBalanced())
		{
			base.InvokeRepeating(new Action(this.ServerThink_Internal), 0f, 0.1f);
			this.lastThinkTime = Time.time;
		}
		base.Invoke(new Action(this.EquipTest), 0.25f);
		this.finalDestination = base.transform.position;
		this.AgencyUpdateRequired = false;
		this.IsOnOffmeshLinkAndReachedNewCoord = false;
		if (this.NavAgent == null)
		{
			this.NavAgent = base.GetComponent<NavMeshAgent>();
		}
		if (this.NavAgent)
		{
			this.NavAgent.updateRotation = false;
			this.NavAgent.updatePosition = false;
		}
	}

	public virtual void ServerThink(float delta)
	{
		this.TickAi(delta);
	}

	internal void ServerThink_Internal()
	{
		this.ServerThink(Time.time - this.lastThinkTime);
		this.lastThinkTime = Time.time;
	}

	public virtual void SetAimDirection(Vector3 newAim)
	{
		if (newAim == Vector3.zero)
		{
			return;
		}
		AttackEntity attackEntity = this.GetAttackEntity();
		if (attackEntity)
		{
			newAim = attackEntity.ModifyAIAim(newAim, 1f);
		}
		this.eyes.rotation = Quaternion.LookRotation(newAim, Vector3.up);
		this.viewAngles = this.eyes.rotation.eulerAngles;
		this.ServerRotation = this.eyes.rotation;
	}

	public virtual void SetDestination(Vector3 newDestination)
	{
		this.finalDestination = newDestination;
	}

	public virtual bool ShotTest()
	{
		AttackEntity heldEntity = base.GetHeldEntity() as AttackEntity;
		if (heldEntity == null)
		{
			return false;
		}
		BaseProjectile baseProjectile = heldEntity as BaseProjectile;
		if (baseProjectile)
		{
			if (baseProjectile.primaryMagazine.contents <= 0)
			{
				baseProjectile.ServerReload();
				NPCPlayerApex nPCPlayerApex = this as NPCPlayerApex;
				if (nPCPlayerApex && nPCPlayerApex.OnReload != null)
				{
					nPCPlayerApex.OnReload();
				}
				return false;
			}
			if (baseProjectile.NextAttackTime > Time.time)
			{
				return false;
			}
		}
		if (Mathf.Approximately(heldEntity.attackLengthMin, -1f))
		{
			heldEntity.ServerUse(this.damageScale);
			this.lastGunShotTime = Time.time;
			return true;
		}
		NPCPlayer nPCPlayer = this;
		if (base.IsInvoking(new Action(nPCPlayer.TriggerDown)))
		{
			return true;
		}
		if (Time.time < this.nextTriggerTime)
		{
			return true;
		}
		NPCPlayer nPCPlayer1 = this;
		base.InvokeRepeating(new Action(nPCPlayer1.TriggerDown), 0f, 0.01f);
		this.triggerEndTime = Time.time + UnityEngine.Random.Range(heldEntity.attackLengthMin, heldEntity.attackLengthMax);
		this.TriggerDown();
		return true;
	}

	public virtual void TickAi(float delta)
	{
		this.MovementUpdate(delta);
	}

	public virtual void TriggerDown()
	{
		AttackEntity heldEntity = base.GetHeldEntity() as AttackEntity;
		if (heldEntity != null)
		{
			heldEntity.ServerUse(this.damageScale);
		}
		this.lastGunShotTime = Time.time;
		if (Time.time > this.triggerEndTime)
		{
			NPCPlayer nPCPlayer = this;
			base.CancelInvoke(new Action(nPCPlayer.TriggerDown));
			this.nextTriggerTime = Time.time + (heldEntity != null ? heldEntity.attackSpacing : 1f);
		}
	}

	protected virtual void UpdatePositionAndRotation(Vector3 moveToPosition)
	{
		this.ServerPosition = moveToPosition;
		this.SetAimDirection(this.GetAimDirection());
	}

	private void UpdateSpeed(float delta)
	{
		float single = this.DesiredMoveSpeed();
		this.NavAgent.speed = Mathf.Lerp(this.NavAgent.speed, single, delta * 8f);
	}

	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		if (ValidBounds.Test(moveToPosition) || !(base.transform != null) || base.IsDestroyed)
		{
			return true;
		}
		Debug.Log(string.Concat(new object[] { "Invalid NavAgent Position: ", this, " ", moveToPosition.ToString(), " (destroying)" }));
		base.Kill(BaseNetworkable.DestroyMode.None);
		return false;
	}
}