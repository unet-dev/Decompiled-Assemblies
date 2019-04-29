using Apex.AI;
using Apex.Serialization;
using System;
using System.Reflection;
using UnityEngine;

namespace Rust.Ai
{
	public abstract class WeightedScorerBase<T> : OptionScorerBase<T>
	{
		[ApexSerialization(defaultValue=false)]
		public bool InvertScore;

		[ApexSerialization(defaultValue=50f)]
		public float ScoreScale;

		private string DebugName;

		public WeightedScorerBase()
		{
			this.DebugName = base.GetType().Name;
		}

		public abstract float GetScore(BaseContext context, T option);

		protected float ProcessScore(float s)
		{
			s = Mathf.Clamp01(s);
			if (this.InvertScore)
			{
				s = 1f - s;
			}
			return s * this.ScoreScale;
		}

		public override float Score(IAIContext context, T option)
		{
			return this.ProcessScore(this.GetScore((BaseContext)context, option));
		}
	}
}