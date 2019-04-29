using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	[FriendlyName("Scan for Cover", "Scanning for cover volumes and the cover points within the relevant ones.")]
	public sealed class ScanForCover : BaseAction
	{
		[ApexSerialization]
		public float MaxDistanceToCover = 15f;

		[ApexSerialization]
		public float CoverArcThreshold = -0.75f;

		public ScanForCover()
		{
		}

		public override void DoExecute(BaseContext ctx)
		{
			if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || !SingletonComponent<AiManager>.Instance.UseCover || ctx.AIAgent.AttackTarget == null)
			{
				return;
			}
			NPCHumanContext coverVolumeContaining = ctx as NPCHumanContext;
			if (coverVolumeContaining == null)
			{
				return;
			}
			if (coverVolumeContaining.sampledCoverPoints.Count > 0)
			{
				coverVolumeContaining.sampledCoverPoints.Clear();
				coverVolumeContaining.sampledCoverPointTypes.Clear();
			}
			if (!(coverVolumeContaining.AIAgent.AttackTarget is BasePlayer))
			{
				return;
			}
			if (coverVolumeContaining.CurrentCoverVolume == null || !coverVolumeContaining.CurrentCoverVolume.Contains(coverVolumeContaining.Position))
			{
				coverVolumeContaining.CurrentCoverVolume = SingletonComponent<AiManager>.Instance.GetCoverVolumeContaining(coverVolumeContaining.Position);
				if (coverVolumeContaining.CurrentCoverVolume == null)
				{
					coverVolumeContaining.CurrentCoverVolume = AiManager.CreateNewCoverVolume(coverVolumeContaining.Position, null);
				}
			}
			if (coverVolumeContaining.CurrentCoverVolume != null)
			{
				foreach (CoverPoint coverPoint in coverVolumeContaining.CurrentCoverVolume.CoverPoints)
				{
					if (coverPoint.IsReserved)
					{
						continue;
					}
					Vector3 position = coverPoint.Position;
					if ((coverVolumeContaining.Position - position).sqrMagnitude > this.MaxDistanceToCover || !ScanForCover.ProvidesCoverFromDirection(coverPoint, (position - coverVolumeContaining.AIAgent.AttackTargetMemory.Position).normalized, this.CoverArcThreshold))
					{
						continue;
					}
					coverVolumeContaining.sampledCoverPointTypes.Add(coverPoint.NormalCoverType);
					coverVolumeContaining.sampledCoverPoints.Add(coverPoint);
				}
			}
		}

		public static bool ProvidesCoverFromDirection(CoverPoint cp, Vector3 directionTowardCover, float arcThreshold)
		{
			if (Vector3.Dot(cp.Normal, directionTowardCover) < arcThreshold)
			{
				return true;
			}
			return false;
		}
	}
}