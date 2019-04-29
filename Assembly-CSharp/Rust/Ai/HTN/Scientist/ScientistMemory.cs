using Rust.Ai.HTN;
using System;
using UnityEngine;

namespace Rust.Ai.HTN.Scientist
{
	[Serializable]
	public class ScientistMemory : BaseNpcMemory
	{
		[NonSerialized]
		public Rust.Ai.HTN.Scientist.ScientistContext ScientistContext;

		public Vector3 CachedPreferredDistanceDestination;

		public float CachedPreferredDistanceDestinationTime;

		public Vector3 CachedCoverDestination;

		public float CachedCoverDestinationTime;

		public override BaseNpcDefinition Definition
		{
			get
			{
				return this.ScientistContext.Body.AiDefinition;
			}
		}

		public ScientistMemory(Rust.Ai.HTN.Scientist.ScientistContext context) : base(context)
		{
			this.ScientistContext = context;
		}

		protected override void OnSetPrimaryKnownEnemyPlayer(ref BaseNpcMemory.EnemyPlayerInfo info)
		{
			base.OnSetPrimaryKnownEnemyPlayer(ref info);
			if ((info.LastKnownPosition - this.ScientistContext.BodyPosition).sqrMagnitude > 1f)
			{
				this.ScientistContext.HasVisitedLastKnownEnemyPlayerLocation = false;
			}
		}

		public override void ResetState()
		{
			base.ResetState();
		}
	}
}