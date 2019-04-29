using Apex.AI;
using Apex.Serialization;
using System;
using System.Reflection;
using UnityEngine;

namespace Rust.Ai
{
	public abstract class BaseScorer : ContextualScorerBase
	{
		[ApexSerialization(defaultValue=false)]
		public bool InvertScore;

		private string DebugName;

		public BaseScorer()
		{
			this.DebugName = base.GetType().Name;
		}

		public abstract float GetScore(BaseContext context);

		protected float ProcessScore(float s)
		{
			s = Mathf.Clamp01(s);
			if (this.InvertScore)
			{
				s = 1f - s;
			}
			return s * this.score;
		}

		public override float Score(IAIContext context)
		{
			return this.ProcessScore(this.GetScore((BaseContext)context));
		}
	}
}