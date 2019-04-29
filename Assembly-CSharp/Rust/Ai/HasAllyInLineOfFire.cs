using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class HasAllyInLineOfFire : BaseScorer
	{
		public HasAllyInLineOfFire()
		{
		}

		public override float GetScore(BaseContext ctx)
		{
			List<Scientist> scientists;
			float single;
			NPCHumanContext nPCHumanContext = ctx as NPCHumanContext;
			if (nPCHumanContext != null)
			{
				Scientist human = nPCHumanContext.Human as Scientist;
				if (human != null && human.GetAlliesInRange(out scientists) > 0)
				{
					List<Scientist>.Enumerator enumerator = scientists.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							Scientist current = enumerator.Current;
							Vector3 enemyPosition = nPCHumanContext.EnemyPosition - nPCHumanContext.Position;
							Vector3 serverPosition = current.Entity.ServerPosition - nPCHumanContext.Position;
							if (serverPosition.sqrMagnitude >= enemyPosition.sqrMagnitude || Vector3.Dot(enemyPosition.normalized, serverPosition.normalized) <= 0.9f)
							{
								continue;
							}
							single = 1f;
							return single;
						}
						return 0f;
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					return single;
				}
			}
			return 0f;
		}
	}
}