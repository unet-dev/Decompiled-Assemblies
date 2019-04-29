using Apex.AI;
using System;

namespace Apex.Ai.HTN
{
	public interface IEffect : IAction
	{
		void Apply(IHTNContext context, bool fromPlanner, bool temporary);

		void Reverse(IHTNContext context, bool fromPlanner);
	}
}