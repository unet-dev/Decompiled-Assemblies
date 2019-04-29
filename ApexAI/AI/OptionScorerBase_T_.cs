using Apex.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	public abstract class OptionScorerBase<T> : IOptionScorer<T>, ICanBeDisabled
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

		public abstract float Score(IAIContext context, T option);
	}
}