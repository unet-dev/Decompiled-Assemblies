using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public sealed class EntityDangerLevel : WeightedScorerBase<BaseEntity>
	{
		[ApexSerialization]
		public float MinScore;

		public EntityDangerLevel()
		{
		}

		public override float GetScore(BaseContext c, BaseEntity target)
		{
			float single;
			List<Memory.SeenInfo>.Enumerator enumerator = c.Memory.All.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Memory.SeenInfo current = enumerator.Current;
					if (current.Entity != target)
					{
						continue;
					}
					single = Mathf.Max(current.Danger, this.MinScore);
					return single;
				}
				return this.MinScore;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return single;
		}
	}
}