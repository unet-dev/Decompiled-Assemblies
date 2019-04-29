using Apex.AI;
using Apex.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	[FriendlyName("Scan for Positions", "Scanning positions and storing them in the context")]
	public sealed class ScanForPositions : BaseAction
	{
		[ApexSerialization(defaultValue=12f)]
		[FriendlyName("Sampling Range", "How large a range points are sampled in, in a square with the entity in the center")]
		public float SamplingRange = 12f;

		[ApexSerialization(defaultValue=1.5f)]
		[FriendlyName("Sampling Density", "How much distance there is between individual samples")]
		public int SampleRings = 3;

		[ApexSerialization(defaultValue=false)]
		[FriendlyName("Calculate Path", "Calculating the path to each position ensures connectivity, but is expensive. Should be used for fallbacks/stuck-detection only?")]
		public bool CalculatePath;

		[ApexSerialization(defaultValue=false)]
		[FriendlyName("Percentage of Inner Circle for Calculate Path", "Calculating the path to each position ensures connectivity, but is expensive. Here we can define what percentage of the sampling range (it's inner circle) we want to calculate paths for.")]
		public float CalculatePathInnerCirclePercentageThreshold = 0.1f;

		[ApexSerialization]
		public bool ScanAllAreas = true;

		[ApexSerialization]
		public string AreaName;

		[ApexSerialization]
		public bool SampleTerrainHeight = true;

		private static NavMeshPath reusablePath;

		static ScanForPositions()
		{
			ScanForPositions.reusablePath = new NavMeshPath();
		}

		public ScanForPositions()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			if (c.sampledPositions == null)
			{
				return;
			}
			if (c.sampledPositions.Count > 0)
			{
				c.sampledPositions.Clear();
			}
			Vector3 position = c.Position;
			float single = Time.time * 1f;
			float samplingRange = this.SamplingRange / (float)this.SampleRings;
			for (float i = this.SamplingRange; i > 0.5f; i -= samplingRange)
			{
				single += 10f;
				for (float j = single % 35f; j < 360f; j += 35f)
				{
					Vector3 vector3 = new Vector3(position.x + Mathf.Sin(j * 0.0174532924f) * i, position.y, position.z + Mathf.Cos(j * 0.0174532924f) * i);
					if (!this.CalculatePath || i >= this.SamplingRange * this.CalculatePathInnerCirclePercentageThreshold)
					{
						ScanForPositions.TryAddPoint(c, vector3, false, this.ScanAllAreas, this.AreaName, this.SampleTerrainHeight);
					}
					else
					{
						ScanForPositions.TryAddPoint(c, vector3, true, this.ScanAllAreas, this.AreaName, this.SampleTerrainHeight);
					}
				}
			}
		}

		private static void TryAddPoint(BaseContext c, Vector3 p, bool calculatePath, bool scanAllAreas, string areaName, bool sampleTerrainHeight)
		{
			NavMeshHit navMeshHit;
			int num = (scanAllAreas || string.IsNullOrEmpty(areaName) ? -1 : 1 << (NavMesh.GetAreaFromName(areaName) & 31));
			if (sampleTerrainHeight)
			{
				p.y = TerrainMeta.HeightMap.GetHeight(p);
			}
			if (!NavMesh.SamplePosition(p, out navMeshHit, 4f, num))
			{
				return;
			}
			if (!navMeshHit.hit)
			{
				return;
			}
			if (!calculatePath && !c.AIAgent.IsStuck)
			{
				c.sampledPositions.Add(navMeshHit.position);
			}
			else if (NavMesh.CalculatePath(navMeshHit.position, c.Position, num, ScanForPositions.reusablePath) && ScanForPositions.reusablePath.status == NavMeshPathStatus.PathComplete)
			{
				c.sampledPositions.Add(navMeshHit.position);
				return;
			}
		}
	}
}