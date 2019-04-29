using ConVar;
using Rust.Ai;
using Rust.Ai.HTN;
using Rust.Ai.HTN.ScientistJunkpile;
using Rust.Ai.HTN.Sensors;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile.Sensors
{
	[Serializable]
	public class CoverPointsInRangeSensor : INpcSensor
	{
		private CoverPointsInRangeSensor.CoverPointComparer coverPointComparer;

		private float nextCoverPosInfoTick;

		public float LastTickTime
		{
			get;
			set;
		}

		public float TickFrequency
		{
			get;
			set;
		}

		public CoverPointsInRangeSensor()
		{
		}

		private bool _FindCoverPointsInVolume(Vector3 position, List<CoverPoint> coverPoints, ref CoverPointVolume volume, ref float nextTime, float time, AiLocationManager location, float maxDistanceToCoverSqr)
		{
			Transform coverPointGroup;
			if (SingletonComponent<AiManager>.Instance == null || !SingletonComponent<AiManager>.Instance.enabled || !SingletonComponent<AiManager>.Instance.UseCover)
			{
				return false;
			}
			if (time > nextTime)
			{
				nextTime = time + this.TickFrequency * ConVar.AI.npc_cover_info_tick_rate_multiplier;
				if (location.DynamicCoverPointVolume != null)
				{
					volume = location.DynamicCoverPointVolume;
				}
				else if ((volume == null || !volume.Contains(position)) && SingletonComponent<AiManager>.Instance != null && SingletonComponent<AiManager>.Instance.enabled && SingletonComponent<AiManager>.Instance.UseCover)
				{
					volume = SingletonComponent<AiManager>.Instance.GetCoverVolumeContaining(position);
					if (volume == null)
					{
						Vector3 vector3 = position;
						if (location != null)
						{
							coverPointGroup = location.CoverPointGroup;
						}
						else
						{
							coverPointGroup = null;
						}
						volume = AiManager.CreateNewCoverVolume(vector3, coverPointGroup);
					}
				}
			}
			if (volume == null)
			{
				return false;
			}
			if (coverPoints.Count > 0)
			{
				coverPoints.Clear();
			}
			foreach (CoverPoint coverPoint in volume.CoverPoints)
			{
				if (coverPoint.IsReserved || coverPoint.IsCompromised || (position - coverPoint.Position).sqrMagnitude > maxDistanceToCoverSqr)
				{
					continue;
				}
				coverPoints.Add(coverPoint);
			}
			if (coverPoints.Count > 1)
			{
				coverPoints.Sort(this.coverPointComparer);
			}
			return true;
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			ScientistJunkpileDomain aiDomain = npc.AiDomain as ScientistJunkpileDomain;
			if (aiDomain == null || aiDomain.ScientistContext == null)
			{
				return;
			}
			if (this.coverPointComparer == null)
			{
				this.coverPointComparer = new CoverPointsInRangeSensor.CoverPointComparer(npc);
			}
			float allowedCoverRangeSqr = aiDomain.GetAllowedCoverRangeSqr();
			this._FindCoverPointsInVolume(npc.transform.position, aiDomain.ScientistContext.CoverPoints, ref aiDomain.ScientistContext.CoverVolume, ref this.nextCoverPosInfoTick, time, aiDomain.ScientistContext.Location, allowedCoverRangeSqr);
		}

		public class CoverPointComparer : IComparer<CoverPoint>
		{
			private readonly IHTNAgent compareTo;

			public CoverPointComparer(IHTNAgent compareTo)
			{
				this.compareTo = compareTo;
			}

			public int Compare(CoverPoint a, CoverPoint b)
			{
				if (this.compareTo == null || a == null || b == null)
				{
					return 0;
				}
				Vector3 position = this.compareTo.transform.position - a.Position;
				float single = position.sqrMagnitude;
				if (single < 0.01f)
				{
					return -1;
				}
				position = this.compareTo.transform.position - b.Position;
				float single1 = position.sqrMagnitude;
				if (single < single1)
				{
					return -1;
				}
				if (single > single1)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}