using Apex.AI;
using System;

namespace Apex.Ai.HTN
{
	public interface IOperator : IAction
	{
		void Abort(IHTNContext context, PrimitiveTaskSelector task);

		void ApplyExpectedEffects(IHTNContext context, PrimitiveTaskSelector task);

		OperatorStateType Tick(IHTNContext context, PrimitiveTaskSelector task);
	}
}