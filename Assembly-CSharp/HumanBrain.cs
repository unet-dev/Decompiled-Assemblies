using System;
using UnityEngine;
using UnityEngine.AI;

public class HumanBrain : BaseAIBrain<HumanNPC>
{
	public const int HumanState_Idle = 1;

	public const int HumanState_Flee = 2;

	public const int HumanState_Cover = 3;

	public const int HumanState_Patrol = 4;

	public const int HumanState_Roam = 5;

	public const int HumanState_Chase = 6;

	public const int HumanState_Exfil = 7;

	public const int HumanState_Mounted = 8;

	public const int HumanState_Combat = 9;

	public const int HumanState_Traverse = 10;

	public const int HumanState_Alert = 11;

	public const int HumanState_Investigate = 12;

	private float thinkRate = 0.25f;

	private float lastThinkTime = 0f;

	public HumanBrain()
	{
	}

	public override void DoThink()
	{
		this.AIThink(Time.time - this.lastThinkTime);
		this.lastThinkTime = Time.time;
	}

	public override void InitializeAI()
	{
		base.InitializeAI();
		this.AIStates = new BaseAIBrain<HumanNPC>.BasicAIState[11];
		this.AddState(new HumanBrain.IdleState(), 1);
		this.AddState(new HumanBrain.RoamState(), 5);
		this.AddState(new HumanBrain.ChaseState(), 6);
		this.AddState(new HumanBrain.CoverState(), 3);
		this.AddState(new HumanBrain.CombatState(), 9);
		this.AddState(new HumanBrain.MountedState(), 8);
		this.AddState(new HumanBrain.ExfilState(), 7);
	}

	public override bool ShouldThink()
	{
		if (Time.time > this.lastThinkTime + this.thinkRate)
		{
			return true;
		}
		return false;
	}

	public class ChaseState : BaseAIBrain<HumanNPC>.BasicAIState
	{
		private float nextPositionUpdateTime;

		public ChaseState()
		{
		}

		public override float GetWeight()
		{
			float single = 0f;
			if (!base.GetEntity().HasTarget())
			{
				return 0f;
			}
			if (base.GetEntity().AmmoFractionRemaining() < 0.3f || base.GetEntity().IsReloading())
			{
				single -= 1f;
			}
			if (base.GetEntity().HasTarget())
			{
				single += 0.5f;
			}
			if (base.GetEntity().CanSeeTarget())
			{
				single -= 0.5f;
			}
			else
			{
				single += 1f;
			}
			if (base.GetEntity().DistanceToTarget() > base.GetEntity().GetIdealDistanceFromTarget())
			{
				single += 1f;
			}
			return single;
		}

		public override void StateEnter()
		{
			base.StateEnter();
			base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
		}

		public override void StateLeave()
		{
			base.StateLeave();
		}

		public override void StateThink(float delta)
		{
			base.StateThink(delta);
			if (base.GetEntity().currentTarget == null)
			{
				return;
			}
			float single = Vector3.Distance(base.GetEntity().currentTarget.ServerPosition, base.GetEntity().ServerPosition);
			if (single < 5f)
			{
				base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.SlowWalk);
			}
			else if (single >= 10f)
			{
				base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Sprint);
			}
			else
			{
				base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
			}
			if (Time.time > this.nextPositionUpdateTime)
			{
				UnityEngine.Random.Range(1f, 2f);
				Vector3 serverPosition = base.GetEntity().ServerPosition;
				if (base.GetEntity().GetInformationZone() == null)
				{
					return;
				}
				AIMovePoint bestMovePointNear = base.GetEntity().GetInformationZone().GetBestMovePointNear(base.GetEntity().currentTarget.ServerPosition, base.GetEntity().ServerPosition, 0f, 35f, true, null);
				if (!bestMovePointNear)
				{
					base.GetEntity().GetRandomPositionAround(base.GetEntity().currentTarget.ServerPosition, 1f, 2f);
				}
				else
				{
					bestMovePointNear.MarkUsedForEngagement(5f, base.GetEntity());
					serverPosition = bestMovePointNear.transform.position;
					serverPosition = base.GetEntity().GetRandomPositionAround(serverPosition, 0f, bestMovePointNear.radius - 0.3f);
				}
				base.GetEntity().SetDestination(serverPosition);
			}
		}
	}

	public class CombatState : BaseAIBrain<HumanNPC>.BasicAIState
	{
		private float nextStrafeTime;

		public CombatState()
		{
		}

		public override float GetWeight()
		{
			if (!base.GetEntity().HasTarget())
			{
				return 0f;
			}
			if (!base.GetEntity().TargetInRange())
			{
				return 0f;
			}
			float single = 1f - Mathf.InverseLerp(base.GetEntity().GetIdealDistanceFromTarget(), base.GetEntity().EngagementRange(), base.GetEntity().DistanceToTarget());
			float single1 = 0.5f * single;
			if (base.GetEntity().CanSeeTarget())
			{
				single1 += 1f;
			}
			return single1;
		}

		public override void StateEnter()
		{
			base.StateEnter();
			this.brain.mainInterestPoint = base.GetEntity().ServerPosition;
			base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
		}

		public override void StateLeave()
		{
			base.GetEntity().SetDucked(false);
			base.StateLeave();
		}

		public override void StateThink(float delta)
		{
			base.StateThink(delta);
			if (Time.time > this.nextStrafeTime)
			{
				if (UnityEngine.Random.Range(0, 3) == 1)
				{
					this.nextStrafeTime = Time.time + UnityEngine.Random.Range(2f, 3f);
					base.GetEntity().SetDucked(true);
					base.GetEntity().Stop();
					return;
				}
				this.nextStrafeTime = Time.time + UnityEngine.Random.Range(3f, 4f);
				base.GetEntity().SetDucked(false);
				base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
				base.GetEntity().SetDestination(base.GetEntity().GetRandomPositionAround(this.brain.mainInterestPoint, 1f, 2f));
			}
		}
	}

	public class CoverState : BaseAIBrain<HumanNPC>.BasicAIState
	{
		private float lastCoverTime;

		private bool isFleeing;

		private bool inCover;

		private float timeInCover;

		public CoverState()
		{
		}

		public override bool CanInterrupt()
		{
			float single = (base.GetEntity().currentTarget ? 2f : 8f);
			if (base.TimeInState() <= 5f)
			{
				return false;
			}
			if (!this.inCover)
			{
				return true;
			}
			return this.timeInCover > single;
		}

		public override float GetWeight()
		{
			float single = 0f;
			if (!base.GetEntity().currentTarget && base.GetEntity().SecondsSinceAttacked < 2f)
			{
				return 4f;
			}
			if (base.GetEntity().DistanceToTarget() > base.GetEntity().EngagementRange() * 3f)
			{
				return 6f;
			}
			if (!base.IsInState() && base.TimeSinceState() < 2f)
			{
				return 0f;
			}
			if (base.GetEntity().SecondsSinceAttacked < 5f || base.GetEntity().healthFraction < 0.4f || base.GetEntity().DistanceToTarget() < 15f)
			{
				if (base.GetEntity().IsReloading())
				{
					single += 2f;
				}
				single = single + (1f - Mathf.Lerp(0.1f, 0.35f, base.GetEntity().AmmoFractionRemaining())) * 1.5f;
			}
			if (this.isFleeing)
			{
				single += 1f;
			}
			if (base.GetEntity().healthFraction < 1f)
			{
				float single1 = 1f - Mathf.InverseLerp(0.8f, 1f, base.GetEntity().healthFraction);
				single = single + (1f - Mathf.InverseLerp(1f, 2f, base.GetEntity().SecondsSinceAttacked)) * single1 * 2f;
			}
			return single;
		}

		public override void StateEnter()
		{
			base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
			this.lastCoverTime = -10f;
			this.isFleeing = false;
			this.inCover = false;
			this.timeInCover = -1f;
			base.GetEntity().ClearStationaryAimPoint();
			base.StateEnter();
		}

		public override void StateLeave()
		{
			base.StateLeave();
			base.GetEntity().SetDucked(false);
			base.GetEntity().ClearStationaryAimPoint();
		}

		public override void StateThink(float delta)
		{
			bool flag;
			base.StateThink(delta);
			float single = 2f;
			float single1 = 0f;
			if (Time.time > this.lastCoverTime + single && !this.isFleeing)
			{
				Vector3 vector3 = (base.GetEntity().currentTarget ? base.GetEntity().currentTarget.ServerPosition : base.GetEntity().ServerPosition + (base.GetEntity().LastAttackedDir * 30f));
				float single2 = (base.GetEntity().currentTarget != null ? base.GetEntity().DistanceToTarget() : 30f);
				AIInformationZone informationZone = base.GetEntity().GetInformationZone();
				if (informationZone != null)
				{
					float secondsSinceAttacked = base.GetEntity().SecondsSinceAttacked;
					float single3 = (secondsSinceAttacked < 2f ? 2f : 0f);
					float single4 = 20f;
					AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(base.GetEntity().ServerPosition, vector3, single3, single4, base.GetEntity());
					if (bestCoverPoint)
					{
						bestCoverPoint.SetUsedBy(base.GetEntity(), 5f);
					}
					Vector3 vector31 = (bestCoverPoint == null ? base.GetEntity().ServerPosition : bestCoverPoint.transform.position);
					base.GetEntity().SetDestination(vector31);
					float single5 = Vector3.Distance(vector31, base.GetEntity().ServerPosition);
					base.GetEntity().DistanceToTarget();
					int num = (int)((secondsSinceAttacked >= 4f ? false : base.GetEntity().AmmoFractionRemaining() <= 0.25f));
					int num1 = (int)((base.GetEntity().healthFraction >= 0.5f || secondsSinceAttacked >= 1f ? false : Time.time > single1));
					if (single2 > 6f && single5 > 6f || base.GetEntity().currentTarget == null)
					{
						this.isFleeing = true;
						single1 = Time.time + UnityEngine.Random.Range(4f, 7f);
					}
					if (single5 > 1f)
					{
						base.GetEntity().ClearStationaryAimPoint();
					}
				}
				this.lastCoverTime = Time.time;
			}
			bool flag1 = Vector3.Distance(base.GetEntity().ServerPosition, base.GetEntity().finalDestination) <= 0.25f;
			if (!this.inCover & flag1)
			{
				if (this.isFleeing)
				{
					base.GetEntity().SetStationaryAimPoint(base.GetEntity().finalDestination + (-base.GetEntity().eyes.BodyForward() * 5f));
				}
				else if (base.GetEntity().currentTarget)
				{
					base.GetEntity().SetStationaryAimPoint(base.GetEntity().ServerPosition + (Vector3Ex.Direction2D(base.GetEntity().currentTarget.ServerPosition, base.GetEntity().ServerPosition) * 5f));
				}
			}
			this.inCover = flag1;
			if (!this.inCover)
			{
				this.timeInCover = 0f;
			}
			else
			{
				this.timeInCover += delta;
			}
			base.GetEntity().SetDucked(this.inCover);
			if (this.inCover)
			{
				this.isFleeing = false;
			}
			if (base.GetEntity().AmmoFractionRemaining() == 0f || this.isFleeing)
			{
				flag = true;
			}
			else
			{
				flag = (base.GetEntity().CanSeeTarget() || !this.inCover || base.GetEntity().SecondsSinceDealtDamage <= 2f ? false : base.GetEntity().AmmoFractionRemaining() < 0.25f);
			}
			if (flag)
			{
				base.GetEntity().AttemptReload();
			}
			if (!this.inCover)
			{
				if (base.TimeInState() > 1f && this.isFleeing)
				{
					base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Sprint);
					return;
				}
				base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
			}
		}
	}

	public class ExfilState : BaseAIBrain<HumanNPC>.BasicAIState
	{
		public ExfilState()
		{
		}

		public override float GetWeight()
		{
			if (base.GetEntity().RecentlyDismounted() && base.GetEntity().SecondsSinceAttacked > 1f)
			{
				return 100f;
			}
			return 0f;
		}

		public override void StateEnter()
		{
			base.StateEnter();
			base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Sprint);
			AIInformationZone informationZone = base.GetEntity().GetInformationZone();
			if (informationZone != null)
			{
				AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(base.GetEntity().ServerPosition, base.GetEntity().ServerPosition, 25f, 50f, base.GetEntity());
				if (bestCoverPoint)
				{
					bestCoverPoint.SetUsedBy(base.GetEntity(), 10f);
				}
				Vector3 vector3 = (bestCoverPoint == null ? base.GetEntity().ServerPosition : bestCoverPoint.transform.position);
				base.GetEntity().SetDestination(vector3);
				this.brain.mainInterestPoint = vector3;
			}
		}

		public override void StateThink(float delta)
		{
			base.StateThink(delta);
			if (base.GetEntity().CanSeeTarget() && base.TimeInState() > 2f && base.GetEntity().DistanceToTarget() < 10f)
			{
				base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
				return;
			}
			base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Sprint);
		}
	}

	public class IdleState : BaseAIBrain<HumanNPC>.BasicAIState
	{
		public IdleState()
		{
		}

		public override float GetWeight()
		{
			return 0.1f;
		}

		public override void StateEnter()
		{
			base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.SlowWalk);
			base.StateEnter();
		}

		public override void StateThink(float delta)
		{
			base.StateThink(delta);
		}
	}

	public class MountedState : BaseAIBrain<HumanNPC>.BasicAIState
	{
		public MountedState()
		{
		}

		public override float GetWeight()
		{
			if (base.GetEntity().isMounted)
			{
				return 100f;
			}
			return 0f;
		}

		public override void StateEnter()
		{
			base.GetEntity().SetNavMeshEnabled(false);
			base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
			base.StateEnter();
		}

		public override void StateLeave()
		{
			base.GetEntity().SetNavMeshEnabled(true);
			base.StateLeave();
		}
	}

	public class RoamState : BaseAIBrain<HumanNPC>.BasicAIState
	{
		private float nextRoamPositionTime;

		private float lastDestinationTime;

		public RoamState()
		{
		}

		public override float GetWeight()
		{
			if (!base.GetEntity().HasTarget() && base.GetEntity().SecondsSinceAttacked > 10f)
			{
				return 5f;
			}
			return 0f;
		}

		public override void StateEnter()
		{
			base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.SlowWalk);
			base.GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, true);
			this.nextRoamPositionTime = -1f;
			this.lastDestinationTime = Time.time;
			base.StateEnter();
		}

		public override void StateLeave()
		{
			base.GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, false);
			base.StateLeave();
		}

		public override void StateThink(float delta)
		{
			Vector3 vector3;
			base.StateThink(delta);
			if (Vector3.Distance(base.GetEntity().finalDestination, base.GetEntity().ServerPosition) < 2f | Time.time - this.lastDestinationTime > 25f && this.nextRoamPositionTime == -1f)
			{
				this.nextRoamPositionTime = Time.time + UnityEngine.Random.Range(5f, 10f);
			}
			if (this.nextRoamPositionTime != -1f && Time.time > this.nextRoamPositionTime)
			{
				AIMovePoint bestRoamPosition = base.GetEntity().GetBestRoamPosition(base.GetEntity().ServerPosition);
				if (bestRoamPosition)
				{
					float single = Vector3.Distance(bestRoamPosition.transform.position, base.GetEntity().ServerPosition) / 1.5f;
					bestRoamPosition.MarkUsedForRoam(single + 11f, null);
				}
				this.lastDestinationTime = Time.time;
				vector3 = (bestRoamPosition == null ? base.GetEntity().ServerPosition : bestRoamPosition.transform.position);
				base.GetEntity().SetDestination(vector3);
				this.nextRoamPositionTime = -1f;
			}
		}
	}

	public class TraversalState : BaseAIBrain<HumanNPC>.BasicAIState
	{
		private Vector3 desiredDestination;

		public bool finished;

		private AITraversalArea area;

		private bool isTraversing;

		private bool waiting;

		public TraversalState()
		{
		}

		public override bool CanInterrupt()
		{
			return true;
		}

		public override float GetWeight()
		{
			if (this.finished)
			{
				return 0f;
			}
			AITraversalArea traversalArea = base.GetEntity().GetTraversalArea();
			if (this.isTraversing || this.waiting)
			{
				return 10000f;
			}
			if (base.GetEntity().IsInTraversalArea())
			{
				bool flag = false;
				bool flag1 = false;
				Vector3[] navAgent = base.GetEntity().NavAgent.path.corners;
				for (int i = 0; i < (int)navAgent.Length; i++)
				{
					Vector3 vector3 = navAgent[i];
					if (Vector3.Distance(vector3, traversalArea.entryPoint1.position) <= 2f)
					{
						flag = true;
					}
					else if (Vector3.Distance(vector3, traversalArea.entryPoint2.position) <= 2f)
					{
						flag1 = true;
					}
					if (traversalArea.movementArea.Contains(vector3))
					{
						return 10000f;
					}
					if (flag & flag1)
					{
						return 10000f;
					}
				}
			}
			return 0f;
		}

		public override void StateEnter()
		{
			base.StateEnter();
			base.GetEntity().SetDesiredSpeed(HumanNPC.SpeedType.Walk);
			this.finished = false;
			this.isTraversing = false;
			this.waiting = false;
			this.desiredDestination = base.GetEntity().finalDestination;
			this.area = base.GetEntity().GetTraversalArea();
			if (this.area && this.area.CanTraverse(base.GetEntity()))
			{
				this.area.SetBusyFor(2f);
			}
		}

		public override void StateLeave()
		{
			base.StateLeave();
			this.finished = false;
			this.area = null;
			this.isTraversing = false;
			this.waiting = false;
			base.GetEntity().SetDestination(this.desiredDestination);
		}

		public override void StateThink(float delta)
		{
			Vector3 vector3;
			Vector3 vector31;
			base.StateThink(delta);
			if (this.area)
			{
				if (this.isTraversing)
				{
					this.area.SetBusyFor(delta * 2f);
				}
				else if (!this.area.CanTraverse(base.GetEntity()))
				{
					AITraversalWaitPoint entryPointNear = this.area.GetEntryPointNear(base.GetEntity().ServerPosition);
					if (entryPointNear)
					{
						entryPointNear.Occupy(1f);
						base.GetEntity().SetStationaryAimPoint(this.area.GetClosestEntry(base.GetEntity().ServerPosition).position);
					}
					vector3 = (entryPointNear == null ? base.GetEntity().ServerPosition : entryPointNear.transform.position);
					base.GetEntity().SetDestination(vector3);
					this.waiting = true;
					this.isTraversing = false;
				}
				else
				{
					this.waiting = false;
					this.isTraversing = true;
					AITraversalWaitPoint aITraversalWaitPoint = this.area.GetEntryPointNear(this.area.GetFarthestEntry(base.GetEntity().ServerPosition).position);
					if (aITraversalWaitPoint)
					{
						aITraversalWaitPoint.Occupy(delta * 2f);
					}
					vector31 = (aITraversalWaitPoint == null ? this.desiredDestination : aITraversalWaitPoint.transform.position);
					base.GetEntity().SetDestination(vector31);
					this.area.SetBusyFor(delta * 2f);
				}
			}
			if (this.isTraversing && Vector3.Distance(base.GetEntity().ServerPosition, base.GetEntity().finalDestination) < 0.25f)
			{
				this.finished = true;
				this.isTraversing = false;
				this.waiting = false;
			}
		}
	}
}