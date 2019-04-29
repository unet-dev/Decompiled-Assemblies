using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public sealed class BeingAimedAt : BaseScorer
	{
		[ApexSerialization]
		public float arc;

		[ApexSerialization]
		public BeingAimedAt.Equality EqualityType;

		public BeingAimedAt()
		{
		}

		public override float GetScore(BaseContext c)
		{
			float single = 0f;
			int num = 0;
			foreach (BaseEntity visible in c.Memory.Visible)
			{
				BasePlayer basePlayer = visible as BasePlayer;
				if (!(basePlayer != null) || basePlayer is IAIAgent)
				{
					continue;
				}
				Vector3 vector3 = basePlayer.eyes.BodyForward();
				float single1 = 0f;
				float single2 = Vector3.Dot(c.AIAgent.CurrentAimAngles, vector3);
				switch (this.EqualityType)
				{
					case BeingAimedAt.Equality.Equal:
					{
						single1 = (Mathf.Approximately(single2, this.arc) ? 1f : 0f);
						break;
					}
					case BeingAimedAt.Equality.LEqual:
					{
						single1 = (single2 <= this.arc ? 1f : 0f);
						break;
					}
					case BeingAimedAt.Equality.GEqual:
					{
						single1 = (single2 >= this.arc ? 1f : 0f);
						break;
					}
					case BeingAimedAt.Equality.NEqual:
					{
						single1 = (Mathf.Approximately(single2, this.arc) ? 0f : 1f);
						break;
					}
					case BeingAimedAt.Equality.Less:
					{
						single1 = (single2 < this.arc ? 1f : 0f);
						break;
					}
					case BeingAimedAt.Equality.Greater:
					{
						single1 = (single2 > this.arc ? 1f : 0f);
						break;
					}
				}
				single += single1;
				num++;
			}
			if (num > 0)
			{
				single /= (float)num;
			}
			return single;
		}

		public enum Equality
		{
			Equal,
			LEqual,
			GEqual,
			NEqual,
			Less,
			Greater
		}
	}
}