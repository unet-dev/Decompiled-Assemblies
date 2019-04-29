using Apex.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	public abstract class ContextualScorerBase<TContext> : IContextualScorer, ICanBeDisabled
	where TContext : class, IAIContext
	{
		[ApexSerialization(defaultValue=0f)]
		public float score;

		[ApexSerialization]
		public bool CanInvalidatePlan
		{
			get;
			set;
		}

		[ApexSerialization(hideInEditor=true, defaultValue=false)]
		public bool isDisabled
		{
			get;
			set;
		}

		protected ContextualScorerBase()
		{
		}

		float Apex.AI.IContextualScorer.Score(IAIContext context)
		{
			return this.Score((TContext)context);
		}

		public abstract float Score(TContext context);
	}
}