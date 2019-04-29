using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public sealed class ReloadWeaponNeed : BaseScorer
	{
		[ApexSerialization]
		private AnimationCurve ResponseCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		[ApexSerialization]
		private bool UseResponseCurve = true;

		public ReloadWeaponNeed()
		{
		}

		public override float GetScore(BaseContext c)
		{
			BasePlayer aIAgent = c.AIAgent as BasePlayer;
			if (aIAgent != null)
			{
				AttackEntity heldEntity = aIAgent.GetHeldEntity() as AttackEntity;
				if (heldEntity != null)
				{
					BaseProjectile baseProjectile = heldEntity as BaseProjectile;
					if (baseProjectile)
					{
						float single = (float)baseProjectile.primaryMagazine.contents / (float)baseProjectile.primaryMagazine.capacity;
						if (!this.UseResponseCurve)
						{
							return single;
						}
						return this.ResponseCurve.Evaluate(single);
					}
				}
			}
			return 0f;
		}
	}
}