using Apex.AI;
using System;
using System.Collections.Generic;

namespace Apex.Ai.HTN
{
	public class TaskQualifier : QualifierBase
	{
		private ITask _task;

		private float _lastScore;

		public TaskQualifier()
		{
		}

		public float Decompose(DomainSelector domain, ITask parent, IHTNContext context, ref List<PrimitiveTaskSelector> plan, ref int score, int scoreThreshold, out int localCost)
		{
			localCost = 0;
			if (this._task == null)
			{
				this.FindTaskOfQualifier();
			}
			if (this._task == null)
			{
				this._lastScore = -1f;
				return this._lastScore;
			}
			this._lastScore = this.DecomposeTask(domain, parent, this._task, context, ref plan, ref score, scoreThreshold, out localCost);
			score += localCost;
			return this._lastScore;
		}

		private float DecomposeTask(DomainSelector domain, ITask parent, ITask task, IHTNContext context, ref List<PrimitiveTaskSelector> plan, ref int score, int scoreThreshold, out int localCost)
		{
			localCost = 0;
			float single = TaskQualifier.TestPreconditions(task, context);
			if (single <= 0f)
			{
				this.GetFullDecompositionCost(ref localCost);
				return single;
			}
			return task.Decompose(domain, parent, context, ref plan, ref score, scoreThreshold, out localCost);
		}

		private ITask FindTaskOfQualifier()
		{
			if (this._task == null)
			{
				SelectorAction selectorAction = base.action as SelectorAction;
				if (selectorAction != null && selectorAction.selector != null)
				{
					this._task = selectorAction.selector as ITask;
					if (this._task != null)
					{
						return this._task;
					}
				}
				AILinkAction aILinkAction = base.action as AILinkAction;
				if (aILinkAction != null)
				{
					IUtilityAI aI = AIManager.GetAI(aILinkAction.aiId);
					if (aI.rootSelector is ITask)
					{
						this._task = aI.rootSelector as ITask;
						ITask task = this._task;
						return this._task;
					}
				}
			}
			return this._task;
		}

		public void GetFullDecompositionCost(ref int cost)
		{
			if (this._task == null)
			{
				this.FindTaskOfQualifier();
			}
			ITask task = this._task;
			if (task == null)
			{
				return;
			}
			task.GetFullDecompositionCost(ref cost);
		}

		public void RemoveAppliedEffects(IHTNContext context, ref List<PrimitiveTaskSelector> plan)
		{
			ITask task = this._task;
			if (task == null)
			{
				return;
			}
			task.RemoveAppliedEffects(context, ref plan);
		}

		public void Reset()
		{
			this._lastScore = 0f;
			ITask task = this._task;
			if (task == null)
			{
				return;
			}
			task.Reset();
		}

		public override float Score(IAIContext context)
		{
			return this._lastScore;
		}

		public static float TestPreconditions(ITask task, IHTNContext context)
		{
			for (int i = 0; i < task.Preconditions.Count; i++)
			{
				ICompositeScorer item = task.Preconditions[i];
				ICanBeDisabled canBeDisabled = item as ICanBeDisabled;
				ICanBeDisabled canBeDisabled1 = canBeDisabled;
				if ((canBeDisabled == null || !canBeDisabled1.isDisabled) && item.Score(context, item.scorers) <= 0f)
				{
					return 0f;
				}
			}
			return 1f;
		}
	}
}