using Apex.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	public abstract class QualifierBase : IQualifier, ICanBeDisabled
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

		public abstract float Score(IAIContext context);
	}
}