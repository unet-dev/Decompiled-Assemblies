using Apex.AI;
using System;
using System.Collections.Generic;

namespace Apex.Ai.HTN
{
	public interface IHTNContext : IAIContext
	{
		Dictionary<Guid, Stack<IEffect>> AppliedEffects
		{
			get;
		}

		Dictionary<Guid, Stack<IEffect>> AppliedExpectedEffects
		{
			get;
		}

		PrimitiveTaskSelector CurrentTask
		{
			get;
			set;
		}

		List<PrimitiveTaskSelector> DebugPlan
		{
			get;
		}

		int DecompositionScore
		{
			get;
			set;
		}

		Stack<PrimitiveTaskSelector> HtnPlan
		{
			get;
		}

		bool IsWorldStateDirty
		{
			get;
			set;
		}

		PlanResultType PlanResult
		{
			get;
			set;
		}

		PlanStateType PlanState
		{
			get;
			set;
		}

		byte[] PreviousWorldState
		{
			get;
		}

		byte[] WorldState
		{
			get;
		}

		Stack<WorldStateInfo>[] WorldStateChanges
		{
			get;
		}

		void StartDomainDecomposition();
	}
}