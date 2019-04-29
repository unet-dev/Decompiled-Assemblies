using Apex.AI;
using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apex.Ai.HTN
{
	public class DomainSelector : Selector
	{
		private List<TaskQualifier> _tasks = new List<TaskQualifier>(50);

		private List<PrimitiveTaskSelector> _plan = new List<PrimitiveTaskSelector>(5);

		public int DecompositionScore = 2147483647;

		public DomainSelector()
		{
		}

		private bool _UpdateTaskQualifiers()
		{
			if (base.qualifiers.Count == 0)
			{
				return false;
			}
			if (this._tasks.Count == 0)
			{
				for (int i = 0; i < base.qualifiers.Count; i++)
				{
					TaskQualifier item = base.qualifiers[i] as TaskQualifier;
					if (item != null)
					{
						this._tasks.Add(item);
					}
				}
			}
			return true;
		}

		private float ScoreOfDecomposition(List<float> scores)
		{
			float item = 0f;
			for (int i = 0; i < scores.Count; i++)
			{
				item += scores[i];
			}
			return item;
		}

		public override IQualifier Select(IAIContext context, IList<IQualifier> qualifiers, IDefaultQualifier defaultQualifier)
		{
			int num;
			if (!this._UpdateTaskQualifiers())
			{
				return defaultQualifier;
			}
			for (int i = 0; i < this._tasks.Count; i++)
			{
				this._tasks[i].Reset();
			}
			IQualifier qualifier = defaultQualifier;
			IHTNContext worldState = context as IHTNContext;
			bool flag = false;
			this._plan.Clear();
			if (worldState != null)
			{
				int decompositionScore = worldState.DecompositionScore;
				this.DecompositionScore = 0;
				worldState.PlanResult = PlanResultType.NoPlan;
				for (int j = 0; j < (int)worldState.WorldStateChanges.Length; j++)
				{
					if (worldState.WorldStateChanges[j].Count > 0)
					{
						worldState.WorldStateChanges[j].Clear();
					}
				}
				int num1 = 0;
				worldState.StartDomainDecomposition();
				for (int k = 0; k < this._tasks.Count; k++)
				{
					TaskQualifier item = this._tasks[k];
					if (!item.isDisabled)
					{
						int num2 = num1;
						if (num1 >= decompositionScore)
						{
							worldState.PlanResult = PlanResultType.KeepCurrentPlan;
							break;
						}
						else if (item.Decompose(this, null, worldState, ref this._plan, ref num2, decompositionScore, out num) <= 0f)
						{
							num = 0;
							item.GetFullDecompositionCost(ref num);
							num1 += num;
							num1++;
						}
						else
						{
							num1 += num;
							if (worldState.PlanState != PlanStateType.Running || num1 < decompositionScore)
							{
								flag = true;
								worldState.DecompositionScore = num1;
								for (int l = 0; l < qualifiers.Count; l++)
								{
									IQualifier item1 = qualifiers[l];
									if (item1.Score(context) > 0f)
									{
										IConnectorAction connectorAction = item1.action as IConnectorAction;
										if (connectorAction != null)
										{
											connectorAction.Select(context);
										}
										else
										{
										}
										qualifier = item1;
									}
								}
								break;
							}
							else
							{
								worldState.PlanResult = PlanResultType.KeepCurrentPlan;
								break;
							}
						}
					}
				}
			}
			if (flag)
			{
				if (worldState != null)
				{
					worldState.HtnPlan.Clear();
					for (int m = this._plan.Count - 1; m >= 0; m--)
					{
						this._plan[m].State = PrimitiveTaskStateType.NotStarted;
						worldState.HtnPlan.Push(this._plan[m]);
					}
					if (Application.isEditor)
					{
						worldState.DebugPlan.Clear();
						for (int n = 0; n < this._plan.Count; n++)
						{
							PrimitiveTaskSelector primitiveTaskSelector = this._plan[n];
							worldState.DebugPlan.Add(primitiveTaskSelector);
						}
					}
					if (worldState.PlanState != PlanStateType.Running)
					{
						worldState.PlanResult = PlanResultType.FoundNewPlan;
					}
					else
					{
						worldState.PlanResult = PlanResultType.ReplacedPlan;
					}
					worldState.PlanState = PlanStateType.Running;
					foreach (KeyValuePair<Guid, Stack<IEffect>> appliedExpectedEffect in worldState.AppliedExpectedEffects)
					{
						Stack<IEffect> value = appliedExpectedEffect.Value;
						value.Clear();
						Pool.Free<Stack<IEffect>>(ref value);
					}
					worldState.AppliedExpectedEffects.Clear();
					foreach (KeyValuePair<Guid, Stack<IEffect>> appliedEffect in worldState.AppliedEffects)
					{
						Stack<IEffect> effects = appliedEffect.Value;
						effects.Clear();
						Pool.Free<Stack<IEffect>>(ref effects);
					}
					worldState.AppliedEffects.Clear();
					for (int o = 0; o < (int)worldState.WorldStateChanges.Length; o++)
					{
						Stack<WorldStateInfo> worldStateChanges = worldState.WorldStateChanges[o];
						while (worldStateChanges.Count > 0 && worldStateChanges.Peek().Temporary)
						{
							worldStateChanges.Pop();
						}
						if (worldStateChanges.Count > 0)
						{
							WorldStateInfo worldStateInfo = worldStateChanges.Peek();
							worldState.PreviousWorldState[o] = worldState.WorldState[o];
							worldState.WorldState[o] = worldStateInfo.Value;
						}
					}
				}
			}
			else if (worldState != null)
			{
				if (worldState.PlanState != PlanStateType.Running)
				{
					worldState.PlanResult = PlanResultType.NoPlan;
				}
				else
				{
					worldState.PlanResult = PlanResultType.KeepCurrentPlan;
				}
			}
			return qualifier;
		}
	}
}