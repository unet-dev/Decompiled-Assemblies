using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Apex.Ai.HTN
{
	public class CompoundTaskSelector : Selector, ITask
	{
		[ApexSerialization]
		[FriendlyName("Decomposition Type", "Should we decompose the children taking only the first valid primitive task, or an order of primitive tasks?")]
		private DecompositionType _decomposition;

		[ApexSerialization]
		[FriendlyName("Preconditions", "Requirements of state that must be in place for this task to be valid")]
		private List<ICompositeScorer> _preconditions = new List<ICompositeScorer>(2);

		private List<TaskQualifier> _taskQualifiers = new List<TaskQualifier>(2);

		public DecompositionType Decomposition
		{
			get
			{
				return this._decomposition;
			}
		}

		public List<ITask> Parents { get; } = new List<ITask>();

		public List<ICompositeScorer> Preconditions
		{
			get
			{
				return this._preconditions;
			}
		}

		public CompoundTaskSelector()
		{
		}

		private bool _UpdateTasksQualifiers()
		{
			if (base.qualifiers.Count == 0)
			{
				return false;
			}
			if (this._taskQualifiers.Count != base.qualifiers.Count)
			{
				this._taskQualifiers.Clear();
				for (int i = 0; i < base.qualifiers.Count; i++)
				{
					TaskQualifier item = base.qualifiers[i] as TaskQualifier;
					if (item != null)
					{
						this._taskQualifiers.Add(item);
					}
				}
			}
			return true;
		}

		public float Decompose(DomainSelector domain, ITask parent, IHTNContext context, ref List<PrimitiveTaskSelector> plan, ref int score, int scoreThreshold, out int localCost)
		{
			int num;
			localCost = 0;
			if (parent != null && !this.Parents.Contains(parent))
			{
				this.Parents.Clear();
				this.Parents.Add(parent);
			}
			if (!this._UpdateTasksQualifiers())
			{
				return 0f;
			}
			localCost++;
			for (int i = 0; i < this._taskQualifiers.Count; i++)
			{
				TaskQualifier item = this._taskQualifiers[i];
				if (!item.isDisabled)
				{
					int num1 = score + localCost;
					if (num1 >= scoreThreshold)
					{
						this.RemovalAtFailedDecomposition(context, ref plan);
						return 0f;
					}
					float single = item.Decompose(domain, this, context, ref plan, ref num1, scoreThreshold, out num);
					if (this._decomposition == DecompositionType.One && single > 0f)
					{
						localCost += num;
						return 1f;
					}
					if (this._decomposition == DecompositionType.All && single <= 0f)
					{
						this.RemovalAtFailedDecomposition(context, ref plan);
						this.GetDecompositionCostFromIndex(ref localCost, i);
						return 0f;
					}
					localCost += num;
				}
			}
			if (this._decomposition == DecompositionType.All)
			{
				return 1f;
			}
			return 0f;
		}

		public void GetDecompositionCostFromIndex(ref int cost, int index)
		{
			cost++;
			if (base.qualifiers.Count == 0)
			{
				return;
			}
			if (this._taskQualifiers.Count > 0)
			{
				for (int i = index; i < this._taskQualifiers.Count; i++)
				{
					TaskQualifier item = this._taskQualifiers[i];
					if (!item.isDisabled && item != null)
					{
						item.GetFullDecompositionCost(ref cost);
					}
				}
				return;
			}
			for (int j = index; j < base.qualifiers.Count; j++)
			{
				TaskQualifier taskQualifier = base.qualifiers[j] as TaskQualifier;
				if ((taskQualifier == null || !taskQualifier.isDisabled) && taskQualifier != null)
				{
					taskQualifier.GetFullDecompositionCost(ref cost);
				}
			}
		}

		public void GetFullDecompositionCost(ref int cost)
		{
			cost++;
			if (base.qualifiers.Count == 0)
			{
				return;
			}
			if (this._taskQualifiers.Count > 0)
			{
				for (int i = 0; i < this._taskQualifiers.Count; i++)
				{
					TaskQualifier item = this._taskQualifiers[i];
					if (!item.isDisabled && item != null)
					{
						item.GetFullDecompositionCost(ref cost);
					}
				}
				return;
			}
			this._taskQualifiers.Clear();
			for (int j = 0; j < base.qualifiers.Count; j++)
			{
				TaskQualifier taskQualifier = base.qualifiers[j] as TaskQualifier;
				if (taskQualifier != null)
				{
					if (!taskQualifier.isDisabled)
					{
						taskQualifier.GetFullDecompositionCost(ref cost);
					}
					this._taskQualifiers.Add(taskQualifier);
				}
			}
		}

		private void RemovalAtFailedDecomposition(IHTNContext context, ref List<PrimitiveTaskSelector> plan)
		{
			this.RemoveAppliedEffects(context, ref plan);
		}

		public void RemoveAppliedEffects(IHTNContext context, ref List<PrimitiveTaskSelector> plan)
		{
			for (int i = 0; i < this._taskQualifiers.Count; i++)
			{
				TaskQualifier item = this._taskQualifiers[i];
				if (!item.isDisabled)
				{
					item.RemoveAppliedEffects(context, ref plan);
				}
			}
		}

		public void Reset()
		{
			if (!this._UpdateTasksQualifiers())
			{
				return;
			}
			for (int i = 0; i < this._taskQualifiers.Count; i++)
			{
				this._taskQualifiers[i].Reset();
			}
		}

		public override IQualifier Select(IAIContext context, IList<IQualifier> qualifiers, IDefaultQualifier defaultQualifier)
		{
			IQualifier qualifier = defaultQualifier;
			for (int i = 0; i < qualifiers.Count; i++)
			{
				IQualifier item = qualifiers[i];
				if (!item.isDisabled)
				{
					float single = item.Score(context);
					if (this._decomposition == DecompositionType.One && single > 0f)
					{
						return item;
					}
					if (this._decomposition == DecompositionType.All && single <= 0f)
					{
						return base.defaultQualifier;
					}
					qualifier = item;
					IConnectorAction connectorAction = item.action as IConnectorAction;
					if (connectorAction != null)
					{
						connectorAction.Select(context);
					}
				}
			}
			if (this._decomposition == DecompositionType.All)
			{
				return qualifier;
			}
			return defaultQualifier;
		}

		public bool ValidatePreconditions(IHTNContext context)
		{
			for (int i = 0; i < this.Preconditions.Count; i++)
			{
				ICompositeScorer item = this.Preconditions[i];
				ICanBeDisabled canBeDisabled = item as ICanBeDisabled;
				ICanBeDisabled canBeDisabled1 = canBeDisabled;
				if ((canBeDisabled == null || !canBeDisabled1.isDisabled) && !item.Validate(context, item.scorers))
				{
					return false;
				}
			}
			for (int j = 0; j < this.Parents.Count; j++)
			{
				if (!this.Parents[j].ValidatePreconditions(context))
				{
					return false;
				}
			}
			return true;
		}
	}
}