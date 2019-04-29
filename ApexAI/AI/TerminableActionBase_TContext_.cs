using Apex.Serialization;
using System;

namespace Apex.AI
{
	[ApexSerializedType]
	public abstract class TerminableActionBase<TContext> : ActionBase<TContext>, IRequireTermination
	where TContext : class, IAIContext
	{
		protected TerminableActionBase()
		{
		}

		void Apex.AI.IRequireTermination.Terminate(IAIContext context)
		{
			this.Terminate((TContext)context);
		}

		public abstract void Terminate(TContext context);
	}
}