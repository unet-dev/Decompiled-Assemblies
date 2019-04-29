using Apex.AI;
using System;
using System.Collections.Generic;

namespace Apex.Ai.HTN
{
	public interface ITask
	{
		List<ITask> Parents
		{
			get;
		}

		List<ICompositeScorer> Preconditions
		{
			get;
		}

		float Decompose(DomainSelector domain, ITask parent, IHTNContext context, ref List<PrimitiveTaskSelector> plan, ref int score, int scoreThreshold, out int localCost);

		void GetFullDecompositionCost(ref int cost);

		void RemoveAppliedEffects(IHTNContext context, ref List<PrimitiveTaskSelector> plan);

		void Reset();

		bool ValidatePreconditions(IHTNContext context);
	}
}