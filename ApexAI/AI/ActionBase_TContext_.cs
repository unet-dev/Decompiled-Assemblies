using Apex.Serialization;
using System;

namespace Apex.AI
{
	[ApexSerializedType]
	public abstract class ActionBase<TContext> : IAction
	where TContext : class, IAIContext
	{
		protected ActionBase()
		{
		}

		void Apex.AI.IAction.Execute(IAIContext context)
		{
			this.Execute((TContext)context);
		}

		public abstract void Execute(TContext context);
	}
}