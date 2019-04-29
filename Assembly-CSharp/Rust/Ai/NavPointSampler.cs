using ConVar;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	public static class NavPointSampler
	{
		private const float HalfPI = 0.0174532924f;

		private readonly static Rust.Ai.NavPointSampleComparer NavPointSampleComparer;

		static NavPointSampler()
		{
			NavPointSampler.NavPointSampleComparer = new Rust.Ai.NavPointSampleComparer();
		}

		private static float _DiscourageSharpTurns(Vector3 pos, IAIAgent agent)
		{
			Vector3 vector3 = (pos - agent.Entity.ServerPosition).normalized;
			float single = Vector3.Dot(agent.Entity.transform.forward, vector3);
			if (single > 0.45f)
			{
				return 1f;
			}
			if (single > 0f)
			{
				return single;
			}
			return 0f;
		}

		private static bool _SampleNavMesh(ref Vector3 pos, IAIAgent agent)
		{
			NavMeshHit navMeshHit;
			if (!NavMesh.SamplePosition(pos, out navMeshHit, agent.GetNavAgent.height * 2f, agent.GetNavAgent.areaMask))
			{
				return false;
			}
			pos = navMeshHit.position;
			return true;
		}

		private static float _WaterDepth(Vector3 pos, float maxDepth)
		{
			float waterDepth = WaterLevel.GetWaterDepth(pos);
			if (Mathf.Approximately(waterDepth, 0f))
			{
				return 1f;
			}
			waterDepth = Mathf.Min(waterDepth, maxDepth);
			return 1f - waterDepth / maxDepth;
		}

		public static float ApproachPointValue(Vector3 point, IAIAgent agent)
		{
			if (agent.AttackTarget == null)
			{
				return 0f;
			}
			float single = 0f;
			if (!NavPointSampler.PointDirectnessToTarget(point, agent.Entity.ServerPosition, agent.AttackTarget.ServerPosition, out single))
			{
				return 0f;
			}
			if (single >= 0.5f)
			{
				return single;
			}
			return 0f;
		}

		public static float FlankPointValue(Vector3 point, IAIAgent agent)
		{
			if (agent.AttackTarget == null)
			{
				return 0f;
			}
			float single = 0f;
			if (!NavPointSampler.PointDirectnessToTarget(point, agent.Entity.ServerPosition, agent.AttackTarget.ServerPosition, out single))
			{
				return 0f;
			}
			if (single >= -0.1f && single <= 0.1f)
			{
				return 1f;
			}
			return 0f;
		}

		public static int GetFeatureCount(int features)
		{
			int num = 0;
			while (features != 0)
			{
				features = features & features - 1;
				num++;
			}
			return num;
		}

		public static Vector3 GetPointOnCircle(Vector3 center, float radius, float degrees)
		{
			float single = center.x + radius * Mathf.Cos(degrees * 0.0174532924f);
			float single1 = center.z + radius * Mathf.Sin(degrees * 0.0174532924f);
			return new Vector3(single, center.y, single1);
		}

		public static bool IsValidPointDirectness(Vector3 point, Vector3 pos, Vector3 targetPos)
		{
			Vector3 vector3 = pos - targetPos;
			Vector3 vector31 = pos - point;
			if (Vector3.Dot(vector3.normalized, vector31.normalized) > 0.5f && vector31.sqrMagnitude > vector3.sqrMagnitude)
			{
				return false;
			}
			return true;
		}

		public static bool PointDirectnessToTarget(Vector3 point, Vector3 pos, Vector3 targetPos, out float value)
		{
			Vector3 vector3 = point - pos;
			Vector3 vector31 = targetPos - pos;
			value = Vector3.Dot(vector3.normalized, vector31.normalized);
			if (value <= 0.5f || vector3.sqrMagnitude <= vector31.sqrMagnitude)
			{
				return true;
			}
			value = 0f;
			return false;
		}

		public static float RangeFromHome(Vector3 point, IAIAgent agent)
		{
			if ((point - agent.SpawnPosition).sqrMagnitude > agent.GetStats.MaxRoamRange * ConVar.AI.npc_max_roam_multiplier)
			{
				return 0f;
			}
			return 1f;
		}

		public static float RetreatFromDirection(Vector3 point, IAIAgent agent)
		{
			if (agent.Entity.LastAttackedDir == Vector3.zero)
			{
				return 0f;
			}
			Vector3 vector3 = (point - agent.Entity.ServerPosition).normalized;
			if (Vector3.Dot(agent.Entity.LastAttackedDir, vector3) > -0.5f)
			{
				return 0f;
			}
			return 1f;
		}

		public static float RetreatPointValue(Vector3 point, IAIAgent agent)
		{
			if (agent.AttackTarget == null)
			{
				return 0f;
			}
			float single = 0f;
			if (!NavPointSampler.PointDirectnessToTarget(point, agent.Entity.ServerPosition, agent.AttackTarget.ServerPosition, out single))
			{
				return 0f;
			}
			if (single > -0.5f)
			{
				return 0f;
			}
			return single * -1f;
		}

		public static float RetreatPointValueExplosive(Vector3 point, IAIAgent agent)
		{
			BaseContext context = agent.GetContext(Guid.Empty) as BaseContext;
			if (context == null || context.DeployedExplosives.Count == 0 || context.DeployedExplosives[0] == null || context.DeployedExplosives[0].IsDestroyed)
			{
				return 0f;
			}
			float single = 0f;
			if (!NavPointSampler.PointDirectnessToTarget(point, agent.Entity.ServerPosition, context.DeployedExplosives[0].ServerPosition, out single))
			{
				return 0f;
			}
			if (single > -0.5f)
			{
				return 0f;
			}
			return single * -1f;
		}

		public static bool SampleCircle(NavPointSampler.SampleCount sampleCount, Vector3 center, float radius, NavPointSampler.SampleScoreParams scoreParams, ref List<NavPointSample> samples)
		{
			if (scoreParams.Agent == null || scoreParams.Agent.GetNavAgent == null)
			{
				return false;
			}
			float single = 90f;
			if (sampleCount == NavPointSampler.SampleCount.Eight)
			{
				single = 45f;
			}
			else if (sampleCount == NavPointSampler.SampleCount.Sixteen)
			{
				single = 22.5f;
			}
			float featureCount = 2f + (float)NavPointSampler.GetFeatureCount((int)scoreParams.Features);
			for (float i = 0f; i < 360f; i += single)
			{
				NavPointSample navPointSample = NavPointSampler.SamplePoint(NavPointSampler.GetPointOnCircle(center, radius, i), scoreParams);
				if (navPointSample.Score > 0f)
				{
					samples.Add(navPointSample);
					if (navPointSample.Score >= featureCount)
					{
						break;
					}
				}
			}
			if (samples.Count == 0)
			{
				for (float j = 0f; j < 360f; j += single)
				{
					NavPointSample navPointSample1 = NavPointSampler.SamplePointWaterDepthOnly(NavPointSampler.GetPointOnCircle(center, radius, j), 2f);
					if (navPointSample1.Score > 0f)
					{
						samples.Add(navPointSample1);
					}
				}
			}
			if (samples.Count > 0)
			{
				samples.Sort(NavPointSampler.NavPointSampleComparer);
			}
			return samples.Count > 0;
		}

		public static bool SampleCircleWaterDepthOnly(NavPointSampler.SampleCount sampleCount, Vector3 center, float radius, NavPointSampler.SampleScoreParams scoreParams, ref List<NavPointSample> samples)
		{
			if (scoreParams.Agent == null || scoreParams.Agent.GetNavAgent == null)
			{
				return false;
			}
			float single = 90f;
			if (sampleCount == NavPointSampler.SampleCount.Eight)
			{
				single = 45f;
			}
			else if (sampleCount == NavPointSampler.SampleCount.Sixteen)
			{
				single = 22.5f;
			}
			for (float i = 0f; i < 360f; i += single)
			{
				NavPointSample navPointSample = NavPointSampler.SamplePointWaterDepthOnly(NavPointSampler.GetPointOnCircle(center, radius, i), 2f);
				if (navPointSample.Score > 0f)
				{
					samples.Add(navPointSample);
				}
			}
			if (samples.Count > 0)
			{
				samples.Sort(NavPointSampler.NavPointSampleComparer);
			}
			return samples.Count > 0;
		}

		public static NavPointSample SamplePoint(Vector3 pos, NavPointSampler.SampleScoreParams scoreParams)
		{
			if (TerrainMeta.HeightMap != null)
			{
				pos.y = TerrainMeta.HeightMap.GetHeight(pos);
			}
			float single = NavPointSampler._WaterDepth(pos, scoreParams.WaterMaxDepth) * 2f;
			if (single > 0f && NavPointSampler._SampleNavMesh(ref pos, scoreParams.Agent))
			{
				if ((scoreParams.Features & NavPointSampler.SampleFeatures.DiscourageSharpTurns) > NavPointSampler.SampleFeatures.None)
				{
					single += NavPointSampler._DiscourageSharpTurns(pos, scoreParams.Agent);
				}
				if ((scoreParams.Features & NavPointSampler.SampleFeatures.RetreatFromTarget) > NavPointSampler.SampleFeatures.None)
				{
					single += NavPointSampler.RetreatPointValue(pos, scoreParams.Agent);
				}
				if ((scoreParams.Features & NavPointSampler.SampleFeatures.ApproachTarget) > NavPointSampler.SampleFeatures.None)
				{
					single += NavPointSampler.ApproachPointValue(pos, scoreParams.Agent);
				}
				if ((scoreParams.Features & NavPointSampler.SampleFeatures.FlankTarget) > NavPointSampler.SampleFeatures.None)
				{
					single += NavPointSampler.FlankPointValue(pos, scoreParams.Agent);
				}
				if ((scoreParams.Features & NavPointSampler.SampleFeatures.RetreatFromDirection) > NavPointSampler.SampleFeatures.None)
				{
					single += NavPointSampler.RetreatFromDirection(pos, scoreParams.Agent);
				}
				if ((scoreParams.Features & NavPointSampler.SampleFeatures.RetreatFromExplosive) > NavPointSampler.SampleFeatures.None)
				{
					single += NavPointSampler.RetreatPointValue(pos, scoreParams.Agent);
				}
				if ((scoreParams.Features & NavPointSampler.SampleFeatures.TopologyPreference) > NavPointSampler.SampleFeatures.None)
				{
					single += NavPointSampler.TopologyPreference(pos, scoreParams.Agent);
				}
				if ((scoreParams.Features & NavPointSampler.SampleFeatures.RangeFromSpawn) > NavPointSampler.SampleFeatures.None)
				{
					single *= NavPointSampler.RangeFromHome(pos, scoreParams.Agent);
				}
			}
			return new NavPointSample()
			{
				Position = pos,
				Score = single
			};
		}

		public static NavPointSample SamplePointWaterDepthOnly(Vector3 pos, float depth)
		{
			if (TerrainMeta.HeightMap != null)
			{
				pos.y = TerrainMeta.HeightMap.GetHeight(pos);
			}
			float single = NavPointSampler._WaterDepth(pos, 2f) * 2f;
			return new NavPointSample()
			{
				Position = pos,
				Score = single
			};
		}

		public static float TopologyPreference(Vector3 point, IAIAgent agent)
		{
			if (TerrainMeta.TopologyMap != null)
			{
				int topology = TerrainMeta.TopologyMap.GetTopology(point);
				if ((agent.TopologyPreference() & topology) > 0)
				{
					return 1f;
				}
			}
			return 0f;
		}

		public enum SampleCount
		{
			Four,
			Eight,
			Sixteen
		}

		[Flags]
		public enum SampleFeatures
		{
			None = 0,
			DiscourageSharpTurns = 1,
			RetreatFromTarget = 2,
			ApproachTarget = 4,
			FlankTarget = 8,
			RetreatFromDirection = 16,
			RetreatFromExplosive = 32,
			TopologyPreference = 64,
			RangeFromSpawn = 128
		}

		public struct SampleScoreParams
		{
			public float WaterMaxDepth;

			public IAIAgent Agent;

			public NavPointSampler.SampleFeatures Features;
		}
	}
}