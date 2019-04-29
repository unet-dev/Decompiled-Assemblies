using Apex.AI;
using Apex.AI.Components;
using Apex.AI.Core.HTN;
using Apex.Ai.HTN;
using Rust.Ai;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN
{
	public abstract class HTNDomain : MonoBehaviour, IHTNDomain, IContextProvider, IDisposable
	{
		[ReadOnly]
		public HTNDomain.MovementRule Movement = HTNDomain.MovementRule.FreeMove;

		[ReadOnly]
		public float MovementRadius = -1f;

		private Vector3 _currentOffset = Vector3.zero;

		public abstract NavMeshAgent NavAgent
		{
			get;
		}

		public abstract BaseNpcContext NpcContext
		{
			get;
		}

		public Stack<PrimitiveTaskSelector> Plan
		{
			get
			{
				return this.PlannerContext.HtnPlan;
			}
		}

		public abstract IUtilityAI PlannerAi
		{
			get;
		}

		public abstract IUtilityAIClient PlannerAiClient
		{
			get;
		}

		public abstract IHTNContext PlannerContext
		{
			get;
		}

		public byte[] PreviousWorldState
		{
			get
			{
				return this.PlannerContext.PreviousWorldState;
			}
		}

		public abstract List<INpcReasoner> Reasoners
		{
			get;
		}

		public abstract List<INpcSensor> Sensors
		{
			get;
		}

		public float SqrMovementRadius
		{
			get
			{
				return this.MovementRadius * this.MovementRadius;
			}
		}

		public byte[] WorldState
		{
			get
			{
				return this.PlannerContext.WorldState;
			}
		}

		protected HTNDomain()
		{
		}

		protected virtual void AbortPlan()
		{
			this.PlannerContext.HtnPlan.Clear();
			this.PlannerContext.PlanState = PlanStateType.Aborted;
			this.PlannerContext.DecompositionScore = 2147483647;
			this.PlannerContext.CurrentTask = null;
		}

		public abstract bool AllowedMovementDestination(Vector3 destination);

		protected virtual bool CanTickReasoner(float deltaTime, INpcReasoner reasoner)
		{
			return deltaTime >= reasoner.TickFrequency;
		}

		protected virtual bool CanTickSensor(float deltaTime, INpcSensor sensor)
		{
			return deltaTime >= sensor.TickFrequency;
		}

		protected virtual void CompletePlan()
		{
			this.PlannerContext.PlanState = PlanStateType.Complete;
			this.PlannerContext.DecompositionScore = 2147483647;
			this.PlannerContext.CurrentTask = null;
		}

		public abstract void Dispose();

		public abstract void ForceProjectileOrientation();

		public abstract IAIContext GetContext(Guid aiId);

		public abstract Vector3 GetHeadingDirection();

		public abstract Vector3 GetHomeDirection();

		public Vector3 GetLookAroundDirection(float deltaTime)
		{
			return this.GetHeadingDirection() + this._currentOffset;
		}

		public abstract Vector3 GetNextPosition(float delta);

		public abstract void Initialize(BaseEntity body);

		public abstract void OnHurt(HitInfo info);

		public virtual void OnPreHurt(HitInfo info)
		{
		}

		public abstract void OnSensation(Sensation sensation);

		public abstract void Pause();

		public virtual void ResetState()
		{
			this.NpcContext.ResetState();
		}

		public abstract void Resume();

		public abstract float SqrDistanceToSpawn();

		public void Think()
		{
			this.PlannerContext.IsWorldStateDirty = false;
			this.PlannerAiClient.Execute();
			if ((this.PlannerContext.PlanResult == PlanResultType.FoundNewPlan || this.PlannerContext.PlanResult == PlanResultType.ReplacedPlan) && this.PlannerContext.CurrentTask != null)
			{
				foreach (IOperator @operator in this.PlannerContext.CurrentTask.Operators)
				{
					if (@operator == null)
					{
						continue;
					}
					@operator.Abort(this.PlannerContext, this.PlannerContext.CurrentTask);
				}
				this.PlannerContext.CurrentTask = null;
			}
		}

		public virtual void Tick(float time)
		{
			this.TickSensors(time);
			this.TickReasoners(time);
			this.TickPlan();
		}

		public abstract void TickDestinationTracker();

		public void TickPlan()
		{
			if (this.PlannerContext.PlanState == PlanStateType.Running)
			{
				if (this.PlannerContext.CurrentTask == null)
				{
					this.PlannerContext.CurrentTask = this.PlannerContext.HtnPlan.Pop();
				}
				else if (this.PlannerContext.CurrentTask.State == PrimitiveTaskStateType.Complete)
				{
					if (this.PlannerContext.HtnPlan.Count <= 0)
					{
						this.CompletePlan();
						this.Think();
						return;
					}
					this.PlannerContext.CurrentTask = this.PlannerContext.HtnPlan.Pop();
				}
				if (!this.PlannerContext.CurrentTask.ValidatePreconditions(this.PlannerContext))
				{
					this.AbortPlan();
					this.Think();
					return;
				}
				if (this.PlannerContext.CurrentTask != null)
				{
					if (this.PlannerContext.CurrentTask.State == PrimitiveTaskStateType.NotStarted)
					{
						if (TaskQualifier.TestPreconditions(this.PlannerContext.CurrentTask, this.PlannerContext) <= 0f)
						{
							this.PlannerContext.CurrentTask.State = PrimitiveTaskStateType.Aborted;
							this.AbortPlan();
							this.Think();
							return;
						}
						this.PlannerContext.CurrentTask.State = PrimitiveTaskStateType.Running;
						foreach (IOperator @operator in this.PlannerContext.CurrentTask.Operators)
						{
							@operator.Execute(this.PlannerContext);
						}
					}
					int num = 0;
					foreach (IOperator operator1 in this.PlannerContext.CurrentTask.Operators)
					{
						OperatorStateType operatorStateType = operator1.Tick(this.PlannerContext, this.PlannerContext.CurrentTask);
						if (operatorStateType != OperatorStateType.Aborted)
						{
							if (operatorStateType != OperatorStateType.Complete)
							{
								continue;
							}
							num++;
						}
						else
						{
							this.PlannerContext.CurrentTask.State = PrimitiveTaskStateType.Aborted;
							this.AbortPlan();
							this.Think();
							return;
						}
					}
					if (num >= this.PlannerContext.CurrentTask.Operators.Count)
					{
						this.PlannerContext.CurrentTask.State = PrimitiveTaskStateType.Complete;
					}
				}
			}
		}

		protected abstract void TickReasoner(INpcReasoner reasoner, float deltaTime, float time);

		public void TickReasoners(float time)
		{
			for (int i = 0; i < this.Reasoners.Count; i++)
			{
				INpcReasoner item = this.Reasoners[i];
				float single = time - item.LastTickTime;
				if (this.CanTickReasoner(single, item))
				{
					this.TickReasoner(item, single, time);
					item.LastTickTime = time + UnityEngine.Random.@value * 0.075f;
				}
			}
		}

		protected abstract void TickSensor(INpcSensor sensor, float deltaTime, float time);

		public void TickSensors(float time)
		{
			for (int i = 0; i < this.Sensors.Count; i++)
			{
				INpcSensor item = this.Sensors[i];
				float single = time - item.LastTickTime;
				if (this.CanTickSensor(single, item))
				{
					this.TickSensor(item, single, time);
					item.LastTickTime = time + UnityEngine.Random.@value * 0.075f;
				}
			}
		}

		public enum MovementRule
		{
			NeverMove,
			RestrainedMove,
			FreeMove
		}
	}
}