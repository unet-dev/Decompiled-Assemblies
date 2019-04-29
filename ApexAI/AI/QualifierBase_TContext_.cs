using Apex.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	public abstract class QualifierBase<TContext> : IQualifier, ICanBeDisabled
	where TContext : class, IAIContext
	{
		[ApexSerialization(hideInEditor=true)]
		public IAction action
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

		protected QualifierBase()
		{
		}

		float Apex.AI.IQualifier.Score(IAIContext context)
		{
			return this.Score((TContext)context);
		}

		public abstract float Score(TContext context);
	}
}