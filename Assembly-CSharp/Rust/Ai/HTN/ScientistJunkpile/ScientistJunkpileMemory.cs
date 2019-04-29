using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile
{
	[Serializable]
	public class ScientistJunkpileMemory : BaseNpcMemory
	{
		[NonSerialized]
		public Rust.Ai.HTN.ScientistJunkpile.ScientistJunkpileContext ScientistJunkpileContext;

		public Vector3 CachedPreferredDistanceDestination;

		public float CachedPreferredDistanceDestinationTime;

		public Vector3 CachedCoverDestination;

		public float CachedCoverDestinationTime;

		public List<BasePlayer> MarkedEnemies = new List<BasePlayer>();

		public override BaseNpcDefinition Definition
		{
			get
			{
				return this.ScientistJunkpileContext.Body.AiDefinition;
			}
		}

		public ScientistJunkpileMemory(Rust.Ai.HTN.ScientistJunkpile.ScientistJunkpileContext context) : base(context)
		{
			this.ScientistJunkpileContext = context;
		}

		public void MarkEnemy(BasePlayer player)
		{
			if (player != null && !this.MarkedEnemies.Contains(player))
			{
				this.MarkedEnemies.Add(player);
			}
		}

		protected override void OnForget(BasePlayer player)
		{
			this.MarkedEnemies.Remove(player);
		}

		protected override void OnSetPrimaryKnownEnemyPlayer(ref BaseNpcMemory.EnemyPlayerInfo info)
		{
			if (!this.MarkedEnemies.Contains(info.PlayerInfo.Player))
			{
				return;
			}
			base.OnSetPrimaryKnownEnemyPlayer(ref info);
			if ((info.LastKnownPosition - this.ScientistJunkpileContext.BodyPosition).sqrMagnitude > 1f)
			{
				this.ScientistJunkpileContext.HasVisitedLastKnownEnemyPlayerLocation = false;
			}
		}

		public override void ResetState()
		{
			base.ResetState();
			this.MarkedEnemies.Clear();
		}

		public override bool ShouldRemoveOnPlayerForgetTimeout(float time, NpcPlayerInfo player)
		{
			if (player.Player == null || player.Player.transform == null || player.Player.IsDestroyed || player.Player.IsDead() || player.Player.IsWounded())
			{
				return true;
			}
			if (time <= player.Time + this.Definition.Memory.ForgetInRangeTime)
			{
				return false;
			}
			if (this.MarkedEnemies.Contains(player.Player) && (player.SqrDistance <= 0f || player.SqrDistance > this.Definition.Sensory.SqrVisionRange))
			{
				return false;
			}
			return true;
		}
	}
}