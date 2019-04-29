using Apex.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	public abstract class OptionScorerBase<T, TContext> : IOptionScorer<T>, ICanBeDisabled
	where TContext : class, IAIContext
	{
		[ApexSerialization(hideInEditor=true, defaultValue=false)]
		public bool isDisabled
		{
			get;
			set;
		}

		protected OptionScorerBase()
		{
		}

		float Apex.AI.IOptionScorer<T>.Score(IAIContext context, T option)
		{
			return this.Score((TContext)context, option);
		}

		public abstract float Score(TContext context, T option);
	}
}