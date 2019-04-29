using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;

namespace Apex.Ai.HTN
{
	[ApexSerializedType]
	public abstract class OperatorBase<TContext> : IOperator, IAction
	where TContext : class, IHTNContext
	{
		protected OperatorBase()
		{
		}

		public abstract void Abort(TContext context, PrimitiveTaskSelector task);

		void Apex.Ai.HTN.IOperator.Abort(IHTNContext context, PrimitiveTaskSelector task)
		{
			TContext tContext = (TContext)(context as TContext);
			TContext tContext1 = tContext;
			if (tContext != null)
			{
				this.Abort(tContext1, task);
			}
		}

		void Apex.Ai.HTN.IOperator.ApplyExpectedEffects(IHTNContext context, PrimitiveTaskSelector task)
		{
			TContext tContext = (TContext)(context as TContext);
			TContext tContext1 = tContext;
			if (tContext != null)
			{
				this.ApplyExpectedEffects(tContext1, task);
			}
		}

		OperatorStateType Apex.Ai.HTN.IOperator.Tick(IHTNContext context, PrimitiveTaskSelector task)
		{
			TContext tContext = (TContext)(context as TContext);
			TContext tContext1 = tContext;
			if (tContext == null)
			{
				return OperatorStateType.Aborted;
			}
			return this.Tick(tContext1, task);
		}

		void Apex.AI.IAction.Execute(IAIContext context)
		{
			TContext tContext = (TContext)(context as TContext);
			TContext tContext1 = tContext;
			if (tContext != null)
			{
				this.Execute(tContext1);
			}
		}

		public void ApplyExpectedEffects(TContext context, PrimitiveTaskSelector task)
		{
			for (int i = 0; i < task.ExpectedEffects.Count; i++)
			{
				task.ExpectedEffects[i].Apply(context, false, false);
			}
		}

		public abstract void Execute(TContext context);

		public abstract OperatorStateType Tick(TContext context, PrimitiveTaskSelector task);
	}
}