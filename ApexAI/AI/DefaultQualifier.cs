using Apex.Serialization;
using System;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	[FriendlyName("Default Action")]
	[Hidden]
	public sealed class DefaultQualifier : QualifierBase, IDefaultQualifier, IQualifier, ICanBeDisabled
	{
		[ApexSerialization(defaultValue=1f)]
		public float score
		{
			get;
			set;
		}

		public DefaultQualifier()
		{
			this.score = 1f;
		}

		public override float Score(IAIContext context)
		{
			return this.score;
		}
	}
}