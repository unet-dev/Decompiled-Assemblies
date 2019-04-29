using Rust.Ai.HTN;
using System;
using UnityEngine;

namespace Rust.Ai.HTN.Murderer
{
	[Serializable]
	public class MurdererMemory : BaseNpcMemory
	{
		[NonSerialized]
		public Rust.Ai.HTN.Murderer.MurdererContext MurdererContext;

		public Vector3 CachedPreferredDistanceDestination;

		public float CachedPreferredDistanceDestinationTime;

		public Vector3 CachedRoamDestination;

		public float CachedRoamDestinationTime;

		public override BaseNpcDefinition Definition
		{
			get
			{
				return this.MurdererContext.Body.AiDefinition;
			}
		}

		public MurdererMemory(Rust.Ai.HTN.Murderer.MurdererContext context) : base(context)
		{
			this.MurdererContext = context;
		}

		protected override void OnSetPrimaryKnownEnemyPlayer(ref BaseNpcMemory.EnemyPlayerInfo info)
		{
			base.OnSetPrimaryKnownEnemyPlayer(ref info);
			if ((info.LastKnownPosition - this.MurdererContext.BodyPosition).sqrMagnitude > 1f)
			{
				this.MurdererContext.HasVisitedLastKnownEnemyPlayerLocation = false;
			}
		}

		public override void ResetState()
		{
			base.ResetState();
		}
	}
}