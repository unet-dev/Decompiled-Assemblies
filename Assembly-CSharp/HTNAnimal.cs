using Apex.Ai.HTN;
using ConVar;
using Network;
using Rust.Ai;
using Rust.Ai.HTN;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class HTNAnimal : BaseCombatEntity, IHTNAgent
{
	[Header("Client Animation")]
	public Vector3 HipFudge = new Vector3(-90f, 0f, 90f);

	public Transform HipBone;

	public Transform LookBone;

	public bool UpdateWalkSpeed = true;

	public bool UpdateFacingDirection = true;

	public bool UpdateGroundNormal = true;

	public Transform alignmentRoot;

	public bool LaggyAss = true;

	public bool LookAtTarget;

	public float MaxLaggyAssRotation = 70f;

	public float MaxWalkAnimSpeed = 25f;

	[Header("Hierarchical Task Network")]
	public HTNDomain _aiDomain;

	[Header("Ai Definition")]
	public BaseNpcDefinition _aiDefinition;

	private bool isDormant;

	private float lastInvokedTickTime;

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

	public Vector3 estimatedVelocity
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

	private Vector3 JustDecompileGenerated_estimatedVelocity_k__BackingField;

	public Vector3 JustDecompileGenerated_get_estimatedVelocity()
	{
		return this.JustDecompileGenerated_estimatedVelocity_k__BackingField;
	}

	public void JustDecompileGenerated_set_estimatedVelocity(Vector3 value)
	{
		this.JustDecompileGenerated_estimatedVelocity_k__BackingField = value;
	}

	public Vector3 EyePosition
	{
		get
		{
			return base.CenterPoint();
		}
	}

	public Quaternion EyeRotation
	{
		get
		{
			return base.transform.rotation;
		}
	}

	public BaseNpc.AiStatistics.FamilyEnum Family
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

	protected override float PositionTickRate
	{
		get
		{
			return 0.1f;
		}
	}

	public HTNAnimal()
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

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		this.AiDomain.Dispose();
	}

	public void ForceOrientationTick()
	{
		this.TickOrientation(UnityEngine.Time.deltaTime, UnityEngine.Time.time);
	}

	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
		if (this.AiDomain != null && this.IsAlive())
		{
			this.AiDomain.OnHurt(info);
		}
	}

	private void InvokedTick()
	{
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

	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	public override float MaxHealth()
	{
		return this.AiDefinition.Vitals.HP;
	}

	public override float MaxVelocity()
	{
		return this.AiDefinition.Movement.RunSpeed;
	}

	public override void OnKilled(HitInfo info)
	{
		BaseNpcDefinition aiDefinition = this.AiDefinition;
		if (aiDefinition != null)
		{
			aiDefinition.OnCreateCorpse(this);
		}
		else
		{
		}
		base.Invoke(new Action(this.KillMessage), 0.5f);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning timeWarning = TimeWarning.New("HTNAnimal.OnRpcMessage", 0.1f))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
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
	}

	Transform Rust.Ai.HTN.IHTNAgent.get_transform()
	{
		return base.transform;
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
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
	}

	public override float StartHealth()
	{
		return this.AiDefinition.Vitals.HP;
	}

	public override float StartMaxHealth()
	{
		return this.AiDefinition.Vitals.HP;
	}

	public void Tick()
	{
		this.InvokedTick();
	}

	private void TickMovement(float delta)
	{
		if (!AI.move || this.AiDomain == null || this.AiDomain.NavAgent == null || !this.AiDomain.NavAgent.isOnNavMesh)
		{
			return;
		}
		Vector3 navAgent = base.transform.position;
		if (this.AiDomain.NavAgent.hasPath)
		{
			navAgent = this.AiDomain.NavAgent.nextPosition;
		}
		if (this._ValidateNextPosition(ref navAgent))
		{
			base.transform.localPosition = base.transform.InverseTransformPoint(navAgent);
			base.transform.hasChanged = true;
		}
	}

	private void TickOrientation(float delta, float time)
	{
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
			default:
			{
				return;
			}
		}
		if (Mathf.Approximately(headingDirection.sqrMagnitude, 0f))
		{
			return;
		}
		this.ServerRotation = Quaternion.LookRotation(headingDirection, base.transform.up);
	}
}