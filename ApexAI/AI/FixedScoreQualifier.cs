using Apex.Serialization;
using System;

namespace Apex.AI
{
	[FriendlyName("Fixed Score", "Always scores a fixed score.")]
	public class FixedScoreQualifier : QualifierBase
	{
		[ApexSerialization(defaultValue=2f)]
		public float score = 2f;

		public FixedScoreQualifier()
		{
		}

		public override float Score(IAIContext context)
		{
			return this.score;
		}
	}
}