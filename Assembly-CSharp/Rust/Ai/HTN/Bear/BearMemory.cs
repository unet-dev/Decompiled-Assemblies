using Rust.Ai.HTN;
using System;
using UnityEngine;

namespace Rust.Ai.HTN.Bear
{
	public class BearMemory : BaseNpcMemory
	{
		[NonSerialized]
		public Rust.Ai.HTN.Bear.BearContext BearContext;

		public Vector3 CachedPreferredDistanceDestination;

		public float CachedPreferredDistanceDestinationTime;

		public override BaseNpcDefinition Definition
		{
			get
			{
				return this.BearContext.Body.AiDefinition;
			}
		}

		public BearMemory(Rust.Ai.HTN.Bear.BearContext context) : base(context)
		{
			this.BearContext = context;
		}

		protected override void OnSetPrimaryKnownEnemyPlayer(ref BaseNpcMemory.EnemyPlayerInfo info)
		{
			base.OnSetPrimaryKnownEnemyPlayer(ref info);
			if ((info.LastKnownPosition - this.BearContext.Body.transform.position).sqrMagnitude > 1f)
			{
				this.BearContext.HasVisitedLastKnownEnemyPlayerLocation = false;
			}
		}

		public override void ResetState()
		{
			base.ResetState();
		}
	}
}