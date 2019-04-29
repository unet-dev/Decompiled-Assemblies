using Apex.AI;
using System;

namespace Rust.Ai
{
	public class EntityTargetContext : IAIContext
	{
		public IAIAgent Self;

		public BaseEntity[] Entities;

		public int EntityCount;

		public BaseNpc AnimalTarget;

		public float AnimalScore;

		public TimedExplosive ExplosiveTarget;

		public float ExplosiveScore;

		public EntityTargetContext()
		{
		}

		public void Refresh(IAIAgent self, BaseEntity[] entities, int entityCount)
		{
			this.Self = self;
			this.Entities = entities;
			this.EntityCount = entityCount;
			this.AnimalTarget = null;
			this.AnimalScore = 0f;
			this.ExplosiveTarget = null;
			this.ExplosiveScore = 0f;
		}
	}
}