using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	public class NavigateToOperator : BaseAction
	{
		[ApexSerialization]
		public NavigateToOperator.OperatorType Operator;

		public NavigateToOperator()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			if (c.GetFact(BaseNpc.Facts.CanNotMove) == 1)
			{
				c.AIAgent.StopMoving();
				c.SetFact(BaseNpc.Facts.PathToTargetStatus, 2);
				return;
			}
			if (!c.AIAgent.IsNavRunning() || c.AIAgent.GetNavAgent.pathPending)
			{
				return;
			}
			switch (this.Operator)
			{
				case NavigateToOperator.OperatorType.EnemyLoc:
				{
					NavigateToOperator.NavigateToEnemy(c);
					return;
				}
				case NavigateToOperator.OperatorType.RandomLoc:
				{
					NavigateToOperator.NavigateToRandomLoc(c);
					return;
				}
				case NavigateToOperator.OperatorType.Spawn:
				{
					NavigateToOperator.NavigateToSpawn(c);
					return;
				}
				case NavigateToOperator.OperatorType.FoodLoc:
				{
					NavigateToOperator.NavigateToFood(c);
					return;
				}
				case NavigateToOperator.OperatorType.FleeEnemy:
				{
					NavigateToOperator.FleeEnemy(c);
					return;
				}
				case NavigateToOperator.OperatorType.FleeHurtDir:
				{
					NavigateToOperator.FleeHurtDir(c);
					return;
				}
				case NavigateToOperator.OperatorType.TopologyPreference:
				{
					NavigateToOperator.NavigateToTopologyPreference(c);
					return;
				}
				default:
				{
					return;
				}
			}
		}

		public static void FleeEnemy(BaseContext c)
		{
			if (c.AIAgent.IsNavRunning() && NavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.RetreatFromTarget, c.AIAgent.GetStats.MinFleeRange, c.AIAgent.GetStats.MaxFleeRange))
			{
				c.SetFact(BaseNpc.Facts.IsFleeing, 1);
			}
		}

		public static void FleeHurtDir(BaseContext c)
		{
			if (c.AIAgent.IsNavRunning() && NavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.RetreatFromDirection, c.AIAgent.GetStats.MinFleeRange, c.AIAgent.GetStats.MaxFleeRange))
			{
				c.SetFact(BaseNpc.Facts.IsFleeing, 1);
			}
		}

		public static void MakeUnstuck(BaseContext c)
		{
			BaseNpc entity = c.Entity as BaseNpc;
			if (entity)
			{
				entity.stuckDuration = 0f;
				entity.IsStuck = false;
			}
		}

		private static bool NavigateInDirOfBestSample(BaseContext c, NavPointSampler.SampleCount sampleCount, float radius, NavPointSampler.SampleFeatures features, float minRange, float maxRange)
		{
			Vector3 position;
			bool flag;
			List<NavPointSample> navPointSamples = c.AIAgent.RequestNavPointSamplesInCircle(sampleCount, radius, features);
			if (navPointSamples == null)
			{
				return false;
			}
			foreach (NavPointSample navPointSample in navPointSamples)
			{
				position = navPointSample.Position - c.Position;
				Vector3 vector3 = position.normalized;
				Vector3 position1 = c.Position + (vector3 * minRange) + (vector3 * ((maxRange - minRange) * UnityEngine.Random.@value));
				NavPointSampler.SampleScoreParams sampleScoreParam = new NavPointSampler.SampleScoreParams()
				{
					WaterMaxDepth = c.AIAgent.GetStats.MaxWaterDepth,
					Agent = c.AIAgent,
					Features = features
				};
				NavPointSample navPointSample1 = NavPointSampler.SamplePoint(position1, sampleScoreParam);
				if (Mathf.Approximately(navPointSample1.Score, 0f))
				{
					continue;
				}
				NavigateToOperator.MakeUnstuck(c);
				position1 = navPointSample1.Position;
				c.AIAgent.Destination = position1;
				c.AIAgent.SetTargetPathStatus(0.05f);
				flag = true;
				return flag;
			}
			float single = 2f;
			navPointSamples = c.AIAgent.RequestNavPointSamplesInCircleWaterDepthOnly(sampleCount, radius, single);
			if (navPointSamples == null)
			{
				return false;
			}
			List<NavPointSample>.Enumerator enumerator = navPointSamples.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					position = enumerator.Current.Position - c.Position;
					Vector3 vector31 = position.normalized;
					Vector3 position2 = c.Position + (vector31 * minRange) + (vector31 * ((maxRange - minRange) * UnityEngine.Random.@value));
					NavPointSample navPointSample2 = NavPointSampler.SamplePointWaterDepthOnly(position2, single);
					if (Mathf.Approximately(navPointSample2.Score, 0f))
					{
						continue;
					}
					NavigateToOperator.MakeUnstuck(c);
					position2 = navPointSample2.Position;
					c.AIAgent.Destination = position2;
					c.AIAgent.SetTargetPathStatus(0.05f);
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

		public static void NavigateToEnemy(BaseContext c)
		{
			if (c.GetFact(BaseNpc.Facts.HasEnemy) > 0 && c.AIAgent.IsNavRunning())
			{
				NavigateToOperator.MakeUnstuck(c);
				c.AIAgent.Destination = c.EnemyPosition;
				c.AIAgent.SetTargetPathStatus(0.05f);
			}
		}

		public static void NavigateToFood(BaseContext c)
		{
			if (c.AIAgent.FoodTarget != null && !c.AIAgent.FoodTarget.IsDestroyed && c.AIAgent.FoodTarget.transform != null && c.GetFact(BaseNpc.Facts.FoodRange) < 2 && c.AIAgent.IsNavRunning())
			{
				NavigateToOperator.MakeUnstuck(c);
				c.AIAgent.Destination = c.AIAgent.FoodTarget.ServerPosition;
				c.AIAgent.SetTargetPathStatus(0.05f);
			}
		}

		public static void NavigateToRandomLoc(BaseContext c)
		{
			if (IsRoamReady.Evaluate(c) && c.AIAgent.IsNavRunning())
			{
				if (NavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.DiscourageSharpTurns | NavPointSampler.SampleFeatures.RangeFromSpawn, c.AIAgent.GetStats.MinRoamRange, c.AIAgent.GetStats.MaxRoamRange))
				{
					float maxRoamDelay = c.AIAgent.GetStats.MaxRoamDelay - c.AIAgent.GetStats.MinRoamDelay;
					float single = UnityEngine.Random.@value * maxRoamDelay / maxRoamDelay;
					float single1 = c.AIAgent.GetStats.RoamDelayDistribution.Evaluate(single) * maxRoamDelay;
					c.NextRoamTime = Time.realtimeSinceStartup + c.AIAgent.GetStats.MinRoamDelay + single1;
					return;
				}
				NavigateToOperator.NavigateToSpawn(c);
			}
		}

		public static void NavigateToSpawn(BaseContext c)
		{
			if (c.AIAgent.IsNavRunning())
			{
				NavigateToOperator.MakeUnstuck(c);
				c.AIAgent.Destination = c.AIAgent.SpawnPosition;
				c.AIAgent.SetTargetPathStatus(0.05f);
			}
		}

		public static void NavigateToTopologyPreference(BaseContext c)
		{
			if (IsRoamReady.Evaluate(c) && c.AIAgent.IsNavRunning())
			{
				if (NavigateToOperator.NavigateInDirOfBestSample(c, NavPointSampler.SampleCount.Eight, 4f, NavPointSampler.SampleFeatures.DiscourageSharpTurns | NavPointSampler.SampleFeatures.TopologyPreference | NavPointSampler.SampleFeatures.RangeFromSpawn, c.AIAgent.GetStats.MinRoamRange, c.AIAgent.GetStats.MaxRoamRange))
				{
					float maxRoamDelay = c.AIAgent.GetStats.MaxRoamDelay - c.AIAgent.GetStats.MinRoamDelay;
					float single = UnityEngine.Random.@value * maxRoamDelay / maxRoamDelay;
					float single1 = c.AIAgent.GetStats.RoamDelayDistribution.Evaluate(single) * maxRoamDelay;
					c.NextRoamTime = Time.realtimeSinceStartup + c.AIAgent.GetStats.MinRoamDelay + single1;
					return;
				}
				NavigateToOperator.NavigateToRandomLoc(c);
			}
		}

		public enum OperatorType
		{
			EnemyLoc,
			RandomLoc,
			Spawn,
			FoodLoc,
			FleeEnemy,
			FleeHurtDir,
			TopologyPreference
		}
	}
}