using Apex.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class CoverContext : IAIContext
	{
		public IAIAgent Self;

		public Vector3 DangerPoint;

		public List<CoverPoint> SampledCoverPoints;

		public float BestRetreatValue;

		public float BestFlankValue;

		public float BestAdvanceValue;

		public CoverPoint BestRetreatCP;

		public CoverPoint BestFlankCP;

		public CoverPoint BestAdvanceCP;

		public float HideoutValue;

		public CoverPoint HideoutCP;

		public CoverContext()
		{
		}

		public void Refresh(IAIAgent self, Vector3 dangerPoint, List<CoverPoint> sampledCoverPoints)
		{
			this.Self = self;
			this.DangerPoint = dangerPoint;
			this.SampledCoverPoints = sampledCoverPoints;
			this.BestRetreatValue = 0f;
			this.BestFlankValue = 0f;
			this.BestAdvanceValue = 0f;
			this.BestRetreatCP = null;
			this.BestFlankCP = null;
			this.BestAdvanceCP = null;
			this.HideoutValue = 0f;
			this.HideoutCP = null;
		}
	}
}