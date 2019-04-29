using Apex.AI;
using Apex.Serialization;
using System;

namespace Apex.Ai.HTN
{
	[ApexSerializedType]
	public abstract class EffectBase<TContext> : IEffect, IAction
	where TContext : class, IHTNContext
	{
		protected EffectBase()
		{
		}

		void Apex.Ai.HTN.IEffect.Reverse(IHTNContext context, bool fromPlanner)
		{
			TContext tContext = (TContext)(context as TContext);
			TContext tContext1 = tContext;
			if (tContext != null)
			{
				this.Reverse(tContext1, fromPlanner);
			}
		}

		void Apex.AI.IAction.Execute(IAIContext context)
		{
			throw new NotImplementedException("Use Apply(...) instead!");
		}

		public void Apply(IHTNContext context, bool fromPlanner, bool temporary)
		{
			TContext tContext = (TContext)(context as TContext);
			TContext tContext1 = tContext;
			if (tContext != null)
			{
				this.Apply(tContext1, fromPlanner, temporary);
			}
		}

		public abstract void Apply(TContext context, bool fromPlanner, bool temporary);

		public abstract void Reverse(TContext context, bool fromPlanner);
	}
}