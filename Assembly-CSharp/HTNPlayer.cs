using Apex.Ai.HTN;
using ConVar;
using Rust.Ai;
using Rust.Ai.HTN;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class HTNPlayer : BasePlayer, IHTNAgent
{
	[Header("Hierarchical Task Network")]
	public HTNDomain _aiDomain;

	[Header("Ai Definition")]
	public BaseNpcDefinition _aiDefinition;

	public string deathStatName = "kill_scientist";

	private bool isDormant;

	private float lastInvokedTickTime;

	private int serverMaxProjectileID;

	public BaseNpcDefinition AiDefinition
	{
		get
		{
			return this._aiDefinition;
		}
	}

	public HTNDomain AiDomain
	{
		get
		{
			return this._aiDomain;
		}
	}

	public BaseEntity Body
	{
		get
		{
			return this;
		}
	}

	public Vector3 BodyPosition
	{
		get
		{
			return base.transform.position;
		}
	}

	public Vector3 EyePosition
	{
		get
		{
			BaseEntity parentEntity = base.GetParentEntity();
			if (parentEntity == null)
			{
				return this.eyes.position;
			}
			return this.BodyPosition + (parentEntity.transform.up * PlayerEyes.EyeOffset.y);
		}
	}

	public Quaternion EyeRotation
	{
		get
		{
			return this.eyes.rotation;
		}
	}

	public override BaseNpc.AiStatistics.FamilyEnum Family
	{
		get
		{
			return this.AiDefinition.Info.ToFamily(this.AiDefinition.Info.Family);
		}
	}

	public bool IsDormant
	{
		get
		{
			return this.isDormant;
		}
		set
		{
			if (this.isDormant != value)
			{
				this.isDormant = value;
				if (this.isDormant)
				{
					this.Pause();
					return;
				}
				this.Resume();
			}
		}
	}

	public override bool IsNpc
	{
		get
		{
			return true;
		}
	}

	public BaseEntity MainTarget
	{
		get
		{
			if (this.AiDomain.NpcContext.OrientationType != NpcOrientation.LookAtAnimal)
			{
				return this.AiDomain.NpcContext.PrimaryEnemyPlayerInLineOfSight.Player;
			}
			return this.AiDomain.NpcContext.BaseMemory.PrimaryKnownAnimal.Animal;
		}
	}

	public bool OnlyRotateAroundYAxis
	{
		get;
		set;
	}

	protected override float PositionTickRate
	{
		get
		{
			return 0.05f;
		}
	}

	public HTNPlayer()
	{
	}

	private bool _ValidateNextPosition(ref Vector3 moveToPosition)
	{
		if (ValidBounds.Test(moveToPosition) || !(base.transform != null) || base.IsDestroyed)
		{
			return true;
		}
		Debug.Log(string.Format("Invalid NavAgent Position: {0} {1} (destroying)", this, moveToPosition.ToString()));
		base.Kill(BaseNetworkable.DestroyMode.None);
		return false;
	}

	public override string Categorize()
	{
		return "npc";
	}

	public override BaseCorpse CreateCorpse()
	{
		BaseCorpse baseCorpse = this.AiDefinition.OnCreateCorpse(this);
		if (baseCorpse)
		{
			return baseCorpse;
		}
		return base.CreateCorpse();
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.AiDomain.Dispose();
	}

	public override bool EligibleForWounding(HitInfo info)
	{
		return false;
	}

	public void ForceOrientationTick()
	{
		this.TickOrientation(UnityEngine.Time.deltaTime, UnityEngine.Time.time);
	}

	public override Vector3 GetLocalVelocityServer()
	{
		return base.estimatedVelocity - base.GetParentVelocity();
	}

	public override Quaternion GetNetworkRotation()
	{
		if (!base.isServer)
		{
			return Quaternion.identity;
		}
		return this.eyes.bodyRotation;
	}

	public override void Hurt(HitInfo info)
	{
		if (info.InitiatorPlayer != null && info.InitiatorPlayer.Family == this.Family)
		{
			return;
		}
		if (this.AiDomain != null && this.IsAlive())
		{
			this.AiDomain.OnPreHurt(info);
		}
		base.Hurt(info);
		if (this.AiDomain != null && this.IsAlive())
		{
			this.AiDomain.OnHurt(info);
		}
	}

	private void InvokedTick()
	{
		if (base.transform == null || base.IsDestroyed || this.IsDead())
		{
			return;
		}
		float single = UnityEngine.Time.time;
		float single1 = single - this.lastInvokedTickTime;
		this.lastInvokedTickTime = UnityEngine.Time.time;
		if (!this.IsDormant)
		{
			if (this.AiDomain != null)
			{
				this.AiDomain.TickDestinationTracker();
				if (this.AiDomain.PlannerContext.IsWorldStateDirty || this.AiDomain.PlannerContext.PlanState == PlanStateType.NoPlan)
				{
					this.AiDomain.Think();
				}
				this.AiDomain.Tick(UnityEngine.Time.time);
			}
			this.TickMovement(single1);
			this.TickOrientation(single1, single);
		}
	}

	public override float MaxHealth()
	{
		BaseNpcDefinition aiDefinition = this.AiDefinition;
		if (aiDefinition == null)
		{
			return 0f;
		}
		return aiDefinition.Vitals.HP;
	}

	public override float MaxVelocity()
	{
		BaseNpcDefinition aiDefinition = this.AiDefinition;
		if (aiDefinition == null)
		{
			return 0f;
		}
		return aiDefinition.Movement.RunSpeed;
	}

	public int NewServerProjectileID()
	{
		int num = this.serverMaxProjectileID + 1;
		this.serverMaxProjectileID = num;
		return num;
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if (this.AiDefinition != null)
		{
			this.AiDefinition.StopVoices(this);
		}
		if (info.InitiatorPlayer != null && !info.InitiatorPlayer.IsNpc)
		{
			info.InitiatorPlayer.stats.Add(this.deathStatName, 1, Stats.Steam);
		}
	}

	public override void OnSensation(Sensation sensation)
	{
		base.OnSensation(sensation);
		if (this.AiDomain != null && this.IsAlive())
		{
			this.AiDomain.OnSensation(sensation);
		}
	}

	public void Pause()
	{
		if (this.AiDomain != null)
		{
			this.AiDomain.Pause();
		}
		if (this.AiDefinition != null)
		{
			this.AiDefinition.StopVoices(this);
		}
	}

	public override void ResetState()
	{
		base.ResetState();
		if (this.AiDomain != null)
		{
			this.AiDomain.ResetState();
		}
	}

	public void Resume()
	{
		if (this.AiDomain != null)
		{
			this.AiDomain.Resume();
		}
		if (this.AiDefinition != null)
		{
			this.AiDefinition.StartVoices(this);
		}
	}

	Transform Rust.Ai.HTN.IHTNAgent.get_transform()
	{
		return base.transform;
	}

	public override void ServerInit()
	{
		if (base.isClient)
		{
			return;
		}
		base.ServerInit();
		this.UpdateNetworkGroup();
		if (this.AiDomain == null)
		{
			Debug.LogError(string.Concat(base.name, " requires an AI domain to be set."));
			base.DieInstantly();
			return;
		}
		this.AiDomain.Initialize(this);
		if (!AiManager.ai_htn_use_agency_tick)
		{
			base.InvokeRepeating(new Action(this.InvokedTick), 0f, 0.1f);
		}
		BaseNpcDefinition aiDefinition = this.AiDefinition;
		if (aiDefinition == null)
		{
			return;
		}
		aiDefinition.Loadout(this);
	}

	public override bool ShouldDropActiveItem()
	{
		return false;
	}

	public override float StartHealth()
	{
		BaseNpcDefinition aiDefinition = this.AiDefinition;
		if (aiDefinition == null)
		{
			return 0f;
		}
		return aiDefinition.Vitals.HP;
	}

	public override float StartMaxHealth()
	{
		BaseNpcDefinition aiDefinition = this.AiDefinition;
		if (aiDefinition == null)
		{
			return 0f;
		}
		return aiDefinition.Vitals.HP;
	}

	public void Tick()
	{
		this.InvokedTick();
	}

	private void TickMovement(float delta)
	{
		if (!AI.move || this.AiDomain == null)
		{
			return;
		}
		Vector3 nextPosition = this.AiDomain.GetNextPosition(delta);
		if (this._ValidateNextPosition(ref nextPosition))
		{
			BaseEntity parentEntity = base.GetParentEntity();
			if (!parentEntity)
			{
				base.transform.localPosition = nextPosition;
			}
			else
			{
				base.transform.localPosition = parentEntity.transform.InverseTransformPoint(nextPosition);
			}
			base.transform.hasChanged = true;
		}
	}

	private void TickOrientation(float delta, float time)
	{
		Quaternion quaternion;
		if (this.AiDomain == null || this.AiDomain.NpcContext == null)
		{
			return;
		}
		Vector3 headingDirection = base.transform.forward;
		switch (this.AiDomain.NpcContext.OrientationType)
		{
			case NpcOrientation.Heading:
			{
				headingDirection = this.AiDomain.GetHeadingDirection();
				break;
			}
			case NpcOrientation.PrimaryTargetBody:
			{
				headingDirection = this.AiDomain.NpcContext.GetDirectionToPrimaryEnemyPlayerTargetBody();
				break;
			}
			case NpcOrientation.PrimaryTargetHead:
			{
				headingDirection = this.AiDomain.NpcContext.GetDirectionToPrimaryEnemyPlayerTargetHead();
				break;
			}
			case NpcOrientation.LastKnownPrimaryTargetLocation:
			{
				headingDirection = this.AiDomain.NpcContext.GetDirectionToMemoryOfPrimaryEnemyPlayerTarget();
				break;
			}
			case NpcOrientation.LookAround:
			{
				headingDirection = this.AiDomain.NpcContext.GetDirectionLookAround();
				break;
			}
			case NpcOrientation.LastAttackedDirection:
			{
				headingDirection = this.AiDomain.NpcContext.GetDirectionLastAttackedDir();
				break;
			}
			case NpcOrientation.AudibleTargetDirection:
			{
				headingDirection = this.AiDomain.NpcContext.GetDirectionAudibleTarget();
				break;
			}
			case NpcOrientation.LookAtAnimal:
			{
				headingDirection = this.AiDomain.NpcContext.GetDirectionToAnimal();
				break;
			}
			case NpcOrientation.Home:
			{
				headingDirection = this.AiDomain.GetHomeDirection();
				break;
			}
			default:
			{
				return;
			}
		}
		if (Mathf.Approximately(headingDirection.sqrMagnitude, 0f))
		{
			return;
		}
		BaseEntity parentEntity = base.GetParentEntity();
		if (!parentEntity)
		{
			this.eyes.bodyRotation = Quaternion.LookRotation(headingDirection, base.transform.up);
			if (this.OnlyRotateAroundYAxis)
			{
				PlayerEyes playerEye = this.eyes;
				quaternion = this.eyes.bodyRotation;
				playerEye.bodyRotation = Quaternion.Euler(new Vector3(0f, quaternion.eulerAngles.y, 0f));
			}
			this.ServerRotation = this.eyes.bodyRotation;
			return;
		}
		Vector3 vector3 = parentEntity.transform.InverseTransformDirection(headingDirection);
		Vector3 vector31 = new Vector3(headingDirection.x, vector3.y, headingDirection.z);
		this.eyes.rotation = Quaternion.LookRotation(vector31, parentEntity.transform.up);
		if (this.OnlyRotateAroundYAxis)
		{
			PlayerEyes playerEye1 = this.eyes;
			quaternion = this.eyes.rotation;
			playerEye1.rotation = Quaternion.Euler(new Vector3(0f, quaternion.eulerAngles.y, 0f));
		}
		this.ServerRotation = this.eyes.bodyRotation;
	}
}