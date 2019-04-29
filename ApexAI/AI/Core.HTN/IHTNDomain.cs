using Apex.AI;
using Apex.AI.Components;
using Apex.Ai.HTN;
using System;

namespace Apex.AI.Core.HTN
{
	public interface IHTNDomain
	{
		IUtilityAI PlannerAi
		{
			get;
		}

		IUtilityAIClient PlannerAiClient
		{
			get;
		}

		IHTNContext PlannerContext
		{
			get;
		}

		IAIContext GetContext(Guid aiId);
	}
}