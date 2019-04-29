using Apex.Serialization;
using ConVar;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	public class HumanNavigateToOperator : BaseAction
	{
		[ApexSerialization]
		public HumanNavigateToOperator.OperatorType Operator;

		public HumanNavigateToOperator()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			NPCHumanContext nPCHumanContext = c as NPCHumanContext;
			if (c.GetFact(NPCPlayerApex.Facts.CanNotMove) == 1 || nPCHumanContext != null && nPCHumanContext.Human.NeverMove)
			{
				c.AIAgent.StopMoving();
				if (nPCHumanContext != null)
				{
					nPCHumanContext.Human.SetFact(NPCPlayerApex.Facts.PathToTargetStatus, 2, true, true);
				}
				return;
			}
			c.AIAgent.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, 0, true, true);
			c.AIAgent.SetFact(NPCPlayerApex.Facts.SidesteppedOutOfCover, 0, true, true);
			if (UnityEngine.Time.time - nPCHumanContext.LastNavigationTime < 1f)
			{
				return;
			}
			nPCHumanContext.LastNavigationTime = UnityEngine.Time.time;
			if (nPCHumanContext.Human.NavAgent.pathPending)
			{
				return;
			}
			switch (this.Operator)
			{
				case HumanNavigateToOperator.OperatorType.EnemyLoc:
				{
					HumanNavigateToOperator.NavigateToEnemy(nPCHumanContext);
					return;
				}
				case HumanNavigateToOperator.OperatorType.RandomLoc:
				{
					HumanNavigateToOperator.NavigateToRandomLoc(nPCHumanContext);
					return;
				}
				case HumanNavigateToOperator.OperatorType.SpawnLoc:
				{
					HumanNavigateToOperator.NavigateToSpawnLoc(nPCHumanContext);
					return;
				}
				case HumanNavigateToOperator.OperatorType.FleeEnemy:
				{
					HumanNavigateToOperator.FleeEnemy(nPCHumanContext);
					return;
				}
				case HumanNavigateToOperator.OperatorType.FleeHurtDir:
				{
					HumanNavigateToOperator.FleeHurtDir(nPCHumanContext);
					return;
				}
				case HumanNavigateToOperator.OperatorType.RetreatCover:
				{
					HumanNavigateToOperator.NavigateToCover(nPCHumanContext, HumanNavigateToOperator.TakeCoverIntention.Retreat);
					return;
				}
				case HumanNavigateToOperator.OperatorType.FlankCover:
				{
					HumanNavigateToOperator.NavigateToCover(nPCHumanContext, HumanNavigateToOperator.TakeCoverIntention.Flank);
					return;
				}
				case HumanNavigateToOperator.OperatorType.AdvanceCover:
				{
					HumanNavigateToOperator.NavigateToCover(nPCHumanContext, HumanNavigateToOperator.TakeCoverIntention.Advance);
					return;
				}
				case HumanNavigateToOperator.OperatorType.FleeExplosive:
				{
					HumanNavigateToOperator.FleeExplosive(nPCHumanContext);
					return;
				}
				case HumanNavigateToOperator.OperatorType.Sidestep:
				{
					HumanNavigateToOperator.Sidestep(nPCHumanContext);
					return;
				}
				case HumanNavigateToOperator.OperatorType.ClosestCover:
				{
					HumanNavigateToOperator.NavigateToCover(nPCHumanContext, HumanNavigateToOperator.TakeCoverIntention.Closest);
					return;
				}
				case HumanNavigateToOperator.OperatorType.PatrolLoc:
				{
					HumanNavigateToOperator.NavigateToPatrolLoc(nPCHumanContext);
					return;
				}
				case HumanNavigateToOperator.OperatorType.MountableChair:
				{
					HumanNavigateToOperator.NavigateToMountableLoc(nPCHumanContext, this.Operator);
					return;
				}
				case HumanNavigateToOperator.OperatorType.WaypointLoc:
				{
					HumanNavigateToOperator.NavigateToWaypointLoc(nPCHumanContext);
					return;
				}
				case HumanNavigateToOperator.OperatorType.LastEnemyLoc:
				{
					HumanNavigateToOperator.NavigateToLastEnemy(nPCHumanContext);
					return;
				}
				case HumanNavigateToOperator.OperatorType.HideoutLoc:
				{
					HumanNavigateToOperator.NavigateToHideout(nPCHumanContext);
					return;
				}
				default:
				{
					return;
				}
			}
		}

		public static void FleeEnemy(NPCHumanContext c)
		{
			if (c.AIAgent.IsNavRunning() && HumanNavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.RetreatFromTarget, c.AIAgent.GetStats.MinFleeRange, c.AIAgent.GetStats.MaxFleeRange))
			{
				c.SetFact(NPCPlayerApex.Facts.IsFleeing, 1, true, true);
			}
		}

		public static void FleeExplosive(NPCHumanContext c)
		{
			if (c.AIAgent.IsNavRunning() && HumanNavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.RetreatFromExplosive, c.AIAgent.GetStats.MinFleeRange, c.AIAgent.GetStats.MaxFleeRange))
			{
				c.SetFact(NPCPlayerApex.Facts.IsFleeing, 1, true, true);
				if (c.Human.OnFleeExplosive != null)
				{
					c.Human.OnFleeExplosive();
				}
			}
		}

		public static void FleeHurtDir(NPCHumanContext c)
		{
			if (c.AIAgent.IsNavRunning() && HumanNavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.RetreatFromDirection, c.AIAgent.GetStats.MinFleeRange, c.AIAgent.GetStats.MaxFleeRange))
			{
				c.SetFact(NPCPlayerApex.Facts.IsFleeing, 1, true, true);
			}
		}

		private static bool IsWaitingAtWaypoint(NPCHumanContext c, ref WaypointSet.Waypoint waypoint)
		{
			if (c.Human.IsWaitingAtWaypoint || waypoint.WaitTime <= 0f)
			{
				if (c.Human.IsWaitingAtWaypoint && UnityEngine.Time.time >= c.Human.WaypointDelayTime)
				{
					c.Human.IsWaitingAtWaypoint = false;
				}
				if (!c.Human.IsWaitingAtWaypoint)
				{
					return false;
				}
			}
			else
			{
				c.Human.WaypointDelayTime = UnityEngine.Time.time + waypoint.WaitTime;
				c.Human.IsWaitingAtWaypoint = true;
				c.SetFact(NPCPlayerApex.Facts.Speed, 0, true, true);
			}
			return true;
		}

		public static void MakeUnstuck(NPCHumanContext c)
		{
			c.Human.stuckDuration = 0f;
			c.Human.IsStuck = false;
		}

		private static bool NavigateInDirOfBestSample(NPCHumanContext c, NavPointSampler.SampleCount sampleCount, float radius, NavPointSampler.SampleFeatures features, float minRange, float maxRange)
		{
			bool flag;
			List<NavPointSample> navPointSamples = c.AIAgent.RequestNavPointSamplesInCircle(sampleCount, radius, features);
			if (navPointSamples == null)
			{
				return false;
			}
			List<NavPointSample>.Enumerator enumerator = navPointSamples.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Vector3 position = (enumerator.Current.Position - c.Position).normalized;
					Vector3 vector3 = c.Position + (position * minRange) + (position * ((maxRange - minRange) * UnityEngine.Random.@value));
					if (c.AIAgent.AttackTarget != null && !NavPointSampler.IsValidPointDirectness(vector3, c.Position, c.EnemyPosition))
					{
						continue;
					}
					NavPointSampler.SampleScoreParams sampleScoreParam = new NavPointSampler.SampleScoreParams()
					{
						WaterMaxDepth = c.AIAgent.GetStats.MaxWaterDepth,
						Agent = c.AIAgent,
						Features = features
					};
					NavPointSample navPointSample = NavPointSampler.SamplePoint(vector3, sampleScoreParam);
					if (Mathf.Approximately(navPointSample.Score, 0f) || Mathf.Approximately(navPointSample.Position.sqrMagnitude, 0f))
					{
						continue;
					}
					HumanNavigateToOperator.MakeUnstuck(c);
					vector3 = navPointSample.Position;
					c.AIAgent.GetNavAgent.destination = vector3;
					c.Human.SetTargetPathStatus(0.05f);
					c.AIAgent.SetFact(NPCPlayerApex.Facts.IsMoving, 1, true, false);
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public static void NavigateToCover(NPCHumanContext c, HumanNavigateToOperator.TakeCoverIntention intention)
		{
			if (!c.AIAgent.IsNavRunning())
			{
				return;
			}
			c.Human.TimeLastMovedToCover = UnityEngine.Time.realtimeSinceStartup;
			switch (intention)
			{
				case HumanNavigateToOperator.TakeCoverIntention.Advance:
				{
					if (c.CoverSet.Advance.ReservedCoverPoint != null)
					{
						HumanNavigateToOperator.PathToCover(c, c.CoverSet.Advance.ReservedCoverPoint.Position);
						return;
					}
					if (c.CoverSet.Closest.ReservedCoverPoint == null)
					{
						break;
					}
					HumanNavigateToOperator.PathToCover(c, c.CoverSet.Closest.ReservedCoverPoint.Position);
					return;
				}
				case HumanNavigateToOperator.TakeCoverIntention.Flank:
				{
					if (c.CoverSet.Flank.ReservedCoverPoint != null)
					{
						HumanNavigateToOperator.PathToCover(c, c.CoverSet.Flank.ReservedCoverPoint.Position);
						c.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, 1, true, true);
						return;
					}
					if (c.CoverSet.Closest.ReservedCoverPoint == null)
					{
						break;
					}
					HumanNavigateToOperator.PathToCover(c, c.CoverSet.Closest.ReservedCoverPoint.Position);
					c.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, 1, true, true);
					return;
				}
				case HumanNavigateToOperator.TakeCoverIntention.Retreat:
				{
					if (c.CoverSet.Retreat.ReservedCoverPoint != null)
					{
						HumanNavigateToOperator.PathToCover(c, c.CoverSet.Retreat.ReservedCoverPoint.Position);
						c.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, 1, true, true);
						return;
					}
					if (c.CoverSet.Closest.ReservedCoverPoint == null)
					{
						break;
					}
					HumanNavigateToOperator.PathToCover(c, c.CoverSet.Closest.ReservedCoverPoint.Position);
					c.SetFact(NPCPlayerApex.Facts.IsRetreatingToCover, 1, true, true);
					return;
				}
				case HumanNavigateToOperator.TakeCoverIntention.Closest:
				{
					if (c.CoverSet.Closest.ReservedCoverPoint == null)
					{
						break;
					}
					HumanNavigateToOperator.PathToCover(c, c.CoverSet.Closest.ReservedCoverPoint.Position);
					break;
				}
				default:
				{
					goto case HumanNavigateToOperator.TakeCoverIntention.Closest;
				}
			}
		}

		public static void NavigateToEnemy(NPCHumanContext c)
		{
			if (c.GetFact(NPCPlayerApex.Facts.HasEnemy) > 0 && c.AIAgent.IsNavRunning())
			{
				if (c.GetFact(NPCPlayerApex.Facts.HasLineOfSight) <= 0 || c.EnemyPosition.sqrMagnitude <= 0f)
				{
					Memory.SeenInfo info = c.Memory.GetInfo(c.AIAgent.AttackTarget);
					if (info.Entity != null && info.Position.sqrMagnitude > 0f)
					{
						HumanNavigateToOperator.MakeUnstuck(c);
						c.Human.StoppingDistance = 1.5f;
						c.Human.Destination = info.Position;
					}
				}
				else
				{
					HumanNavigateToOperator.MakeUnstuck(c);
					c.Human.StoppingDistance = 1.5f;
					c.Human.Destination = c.EnemyPosition;
				}
				c.Human.SetTargetPathStatus(0.05f);
			}
		}

		public static void NavigateToHideout(NPCHumanContext c)
		{
			if (c.EnemyHideoutGuess != null && c.AIAgent.IsNavRunning() && c.EnemyHideoutGuess.Position.sqrMagnitude > 0f)
			{
				HumanNavigateToOperator.MakeUnstuck(c);
				c.Human.StoppingDistance = 1f;
				c.Human.Destination = c.EnemyHideoutGuess.Position;
				c.Human.SetTargetPathStatus(0.05f);
			}
			HumanNavigateToOperator.UpdateRoamTime(c);
		}

		public static void NavigateToLastEnemy(NPCHumanContext c)
		{
			NavMeshHit navMeshHit;
			if (c.AIAgent.AttackTarget != null && c.AIAgent.IsNavRunning())
			{
				Memory.SeenInfo info = c.Memory.GetInfo(c.AIAgent.AttackTarget);
				if (info.Entity != null && info.Position.sqrMagnitude > 0f)
				{
					BasePlayer player = c.AIAgent.AttackTarget.ToPlayer();
					if (player != null && (player.IsAdmin || player.IsDeveloper) && player.IsFlying)
					{
						SetHumanSpeed.Set(c, NPCPlayerApex.SpeedEnum.StandStill);
						return;
					}
					if (!NavMesh.SamplePosition(info.Position, out navMeshHit, 1f, c.AIAgent.GetNavAgent.areaMask))
					{
						SetHumanSpeed.Set(c, NPCPlayerApex.SpeedEnum.StandStill);
						return;
					}
					HumanNavigateToOperator.MakeUnstuck(c);
					c.Human.StoppingDistance = 1f;
					c.Human.Destination = navMeshHit.position;
					c.Human.SetTargetPathStatus(0.05f);
				}
			}
			HumanNavigateToOperator.UpdateRoamTime(c);
		}

		public static void NavigateToMountableLoc(NPCHumanContext c, HumanNavigateToOperator.OperatorType mountableType)
		{
			NavMeshHit navMeshHit;
			if (mountableType == HumanNavigateToOperator.OperatorType.MountableChair && ConVar.AI.npc_ignore_chairs)
			{
				return;
			}
			BaseMountable chairTarget = c.ChairTarget;
			if (chairTarget == null)
			{
				return;
			}
			Vector3 vector3 = chairTarget.transform.position;
			if (NavMesh.SamplePosition(vector3, out navMeshHit, 10f, c.Human.NavAgent.areaMask))
			{
				vector3 = navMeshHit.position;
			}
			if (Mathf.Approximately(vector3.sqrMagnitude, 0f))
			{
				return;
			}
			HumanNavigateToOperator.MakeUnstuck(c);
			c.Human.StoppingDistance = 0.05f;
			c.Human.Destination = vector3;
			c.Human.SetTargetPathStatus(0.05f);
		}

		public static void NavigateToPatrolLoc(NPCHumanContext c)
		{
			if (c.AiLocationManager == null)
			{
				return;
			}
			if (IsHumanRoamReady.Evaluate(c) && c.AIAgent.IsNavRunning())
			{
				PathInterestNode randomPatrolPointInRange = c.AiLocationManager.GetRandomPatrolPointInRange(c.Position, c.AIAgent.GetStats.MinRoamRange, c.AIAgent.GetStats.MaxRoamRange, c.CurrentPatrolPoint);
				if (randomPatrolPointInRange != null && randomPatrolPointInRange.transform.position.sqrMagnitude > 0f)
				{
					HumanNavigateToOperator.MakeUnstuck(c);
					c.Human.Destination = randomPatrolPointInRange.transform.position;
					c.Human.SetTargetPathStatus(0.05f);
					c.CurrentPatrolPoint = randomPatrolPointInRange;
				}
				HumanNavigateToOperator.UpdateRoamTime(c);
				if (c.Human.OnChatter != null)
				{
					c.Human.OnChatter();
				}
			}
		}

		public static void NavigateToRandomLoc(NPCHumanContext c)
		{
			if (IsHumanRoamReady.Evaluate(c) && c.AIAgent.IsNavRunning() && HumanNavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.DiscourageSharpTurns, c.AIAgent.GetStats.MinRoamRange, c.AIAgent.GetStats.MaxRoamRange))
			{
				HumanNavigateToOperator.UpdateRoamTime(c);
				if (c.Human.OnChatter != null)
				{
					c.Human.OnChatter();
				}
			}
		}

		public static void NavigateToSpawnLoc(NPCHumanContext c)
		{
			if (IsHumanRoamReady.Evaluate(c) && c.AIAgent.IsNavRunning() && c.Human.SpawnPosition.sqrMagnitude > 0f)
			{
				HumanNavigateToOperator.MakeUnstuck(c);
				c.Human.StoppingDistance = 0.1f;
				c.Human.Destination = c.Human.SpawnPosition;
				c.Human.SetTargetPathStatus(0.05f);
				HumanNavigateToOperator.UpdateRoamTime(c);
			}
		}

		private static void NavigateToWaypointLoc(NPCHumanContext c)
		{
			if (c.GetFact(NPCPlayerApex.Facts.HasWaypoints) > 0 && c.Human.IsNavRunning())
			{
				c.Human.StoppingDistance = 0.3f;
				WaypointSet.Waypoint item = c.Human.WaypointSet.Points[c.Human.CurrentWaypointIndex];
				bool flag = false;
				Vector3 transform = item.Transform.position;
				if ((c.Human.Destination - transform).sqrMagnitude > 0.01f)
				{
					HumanNavigateToOperator.MakeUnstuck(c);
					c.Human.Destination = transform;
					c.Human.SetTargetPathStatus(0.05f);
					flag = true;
				}
				float single = 0f;
				int num = c.Human.PeekNextWaypointIndex();
				if (c.Human.WaypointSet.Points.Count > num && Mathf.Approximately(c.Human.WaypointSet.Points[num].WaitTime, 0f))
				{
					single = 1f;
				}
				if ((c.Position - c.Human.Destination).sqrMagnitude > c.Human.SqrStoppingDistance + single)
				{
					c.Human.LookAtPoint = null;
					c.Human.LookAtEyes = null;
					if (c.GetFact(NPCPlayerApex.Facts.IsMoving) != 0 || flag)
					{
						c.SetFact(NPCPlayerApex.Facts.IsMovingTowardWaypoint, 1, true, true);
						return;
					}
					c.Human.CurrentWaypointIndex = c.Human.GetNextWaypointIndex();
					c.SetFact(NPCPlayerApex.Facts.IsMovingTowardWaypoint, 0, true, true);
					return;
				}
				if (HumanNavigateToOperator.IsWaitingAtWaypoint(c, ref item))
				{
					if (!IsClosestPlayerWithinDistance.Test(c, 4f))
					{
						c.Human.LookAtEyes = null;
						c.Human.LookAtRandomPoint(5f);
					}
					else
					{
						LookAtClosestPlayer.Do(c);
					}
					c.SetFact(NPCPlayerApex.Facts.IsMovingTowardWaypoint, 0, true, true);
					return;
				}
				c.Human.CurrentWaypointIndex = c.Human.GetNextWaypointIndex();
				c.Human.LookAtPoint = null;
			}
		}

		public static void PathToCover(NPCHumanContext c, Vector3 coverPosition)
		{
			if (coverPosition.sqrMagnitude > 0f)
			{
				HumanNavigateToOperator.MakeUnstuck(c);
				c.AIAgent.GetNavAgent.destination = coverPosition;
				c.Human.SetTargetPathStatus(0.05f);
				c.SetFact(NPCPlayerApex.Facts.IsMovingToCover, 1, true, true);
				if (c.Human.OnTakeCover != null)
				{
					c.Human.OnTakeCover();
				}
			}
		}

		public static void Sidestep(NPCHumanContext c)
		{
			if (c.AIAgent.IsNavRunning())
			{
				c.Human.StoppingDistance = 0.1f;
				if (HumanNavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.FlankTarget, 2f, 5f) && c.AIAgent.GetFact(NPCPlayerApex.Facts.IsInCover) == 1)
				{
					c.AIAgent.SetFact(NPCPlayerApex.Facts.SidesteppedOutOfCover, 1, true, true);
				}
			}
		}

		private static void UpdateRoamTime(NPCHumanContext c)
		{
			float maxRoamDelay = c.AIAgent.GetStats.MaxRoamDelay - c.AIAgent.GetStats.MinRoamDelay;
			float single = c.AIAgent.GetStats.RoamDelayDistribution.Evaluate(UnityEngine.Random.@value) * maxRoamDelay;
			c.NextRoamTime = UnityEngine.Time.realtimeSinceStartup + c.AIAgent.GetStats.MinRoamDelay + single;
		}

		public enum OperatorType
		{
			EnemyLoc,
			RandomLoc,
			SpawnLoc,
			FleeEnemy,
			FleeHurtDir,
			RetreatCover,
			FlankCover,
			AdvanceCover,
			FleeExplosive,
			Sidestep,
			ClosestCover,
			PatrolLoc,
			MountableChair,
			WaypointLoc,
			LastEnemyLoc,
			HideoutLoc
		}

		public enum TakeCoverIntention
		{
			Advance,
			Flank,
			Retreat,
			Closest
		}
	}
}