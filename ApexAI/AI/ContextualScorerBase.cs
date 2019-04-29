using Apex.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	public abstract class ContextualScorerBase : IContextualScorer, ICanBeDisabled
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

		public abstract float Score(IAIContext context);
	}
}