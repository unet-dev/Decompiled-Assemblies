using Rust.Ai;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class NPCHumanContext : BaseNPCContext
{
	public List<BaseChair> Chairs = new List<BaseChair>();

	public BaseChair ChairTarget;

	public float LastNavigationTime;

	public NPCHumanContext.TacticalCoverPointSet CoverSet = new NPCHumanContext.TacticalCoverPointSet();

	public List<NPCHumanContext.HideoutPoint> CheckedHideoutPoints
	{
		get;
		set;
	}

	public CoverPointVolume CurrentCoverVolume
	{
		get;
		set;
	}

	public PathInterestNode CurrentPatrolPoint
	{
		get;
		set;
	}

	public List<CoverPoint> EnemyCoverPoints
	{
		get;
		private set;
	}

	public CoverPoint EnemyHideoutGuess
	{
		get;
		set;
	}

	public BaseEntity LastAttacker
	{
		get
		{
			return this.Human.lastAttacker;
		}
		set
		{
			this.Human.lastAttacker = value;
			this.Human.lastAttackedTime = Time.time;
			NPCPlayerApex human = this.Human;
			Vector3 serverPosition = this.Human.lastAttacker.ServerPosition - this.Human.ServerPosition;
			human.LastAttackedDir = serverPosition.normalized;
		}
	}

	public List<CoverPoint> sampledCoverPoints
	{
		get;
		private set;
	}

	public List<CoverPoint.CoverType> sampledCoverPointTypes
	{
		get;
		private set;
	}

	public NPCHumanContext(NPCPlayerApex human) : base(human)
	{
		this.sampledCoverPoints = new List<CoverPoint>();
		this.EnemyCoverPoints = new List<CoverPoint>();
		this.CheckedHideoutPoints = new List<NPCHumanContext.HideoutPoint>();
		this.sampledCoverPointTypes = new List<CoverPoint.CoverType>();
		this.CoverSet.Setup(human);
	}

	protected override void Finalize()
	{
		try
		{
			this.CoverSet.Shutdown();
		}
		finally
		{
			base.Finalize();
		}
	}

	public void ForgetCheckedHideouts(float forgetTime)
	{
		for (int i = 0; i < this.CheckedHideoutPoints.Count; i++)
		{
			NPCHumanContext.HideoutPoint item = this.CheckedHideoutPoints[i];
			if (Time.time - item.Time > forgetTime)
			{
				this.CheckedHideoutPoints.RemoveAt(i);
				i--;
			}
		}
	}

	public bool HasCheckedHideout(CoverPoint hideout)
	{
		for (int i = 0; i < this.CheckedHideoutPoints.Count; i++)
		{
			if (this.CheckedHideoutPoints[i].Hideout == hideout)
			{
				return true;
			}
		}
		return false;
	}

	public struct HideoutPoint
	{
		public CoverPoint Hideout;

		public float Time;
	}

	public struct TacticalCoverPoint
	{
		public NPCPlayerApex Human;

		public CoverPoint reservedCoverPoint;

		public CoverPoint.CoverType activeCoverType;

		public CoverPoint.CoverType ActiveCoverType
		{
			get
			{
				return this.activeCoverType;
			}
			set
			{
				this.activeCoverType = value;
			}
		}

		public CoverPoint ReservedCoverPoint
		{
			get
			{
				return this.reservedCoverPoint;
			}
			set
			{
				if (this.reservedCoverPoint != null)
				{
					this.reservedCoverPoint.ReservedFor = null;
				}
				this.reservedCoverPoint = value;
				if (this.reservedCoverPoint != null)
				{
					this.reservedCoverPoint.ReservedFor = this.Human;
				}
			}
		}
	}

	public class TacticalCoverPointSet
	{
		public NPCHumanContext.TacticalCoverPoint Retreat;

		public NPCHumanContext.TacticalCoverPoint Flank;

		public NPCHumanContext.TacticalCoverPoint Advance;

		public NPCHumanContext.TacticalCoverPoint Closest;

		public TacticalCoverPointSet()
		{
		}

		public void Reset()
		{
			if (this.Retreat.ReservedCoverPoint != null)
			{
				this.Retreat.ReservedCoverPoint = null;
			}
			if (this.Flank.ReservedCoverPoint != null)
			{
				this.Flank.ReservedCoverPoint = null;
			}
			if (this.Advance.ReservedCoverPoint != null)
			{
				this.Advance.ReservedCoverPoint = null;
			}
			if (this.Closest.ReservedCoverPoint != null)
			{
				this.Closest.ReservedCoverPoint = null;
			}
		}

		public void Setup(NPCPlayerApex human)
		{
			this.Retreat.Human = human;
			this.Flank.Human = human;
			this.Advance.Human = human;
			this.Closest.Human = human;
		}

		public void Shutdown()
		{
			this.Reset();
		}

		public void Update(CoverPoint retreat, CoverPoint flank, CoverPoint advance)
		{
			Vector3 position;
			this.Retreat.ReservedCoverPoint = retreat;
			this.Flank.ReservedCoverPoint = flank;
			this.Advance.ReservedCoverPoint = advance;
			this.Closest.ReservedCoverPoint = null;
			float single = Single.MaxValue;
			if (retreat != null)
			{
				position = retreat.Position - this.Retreat.Human.ServerPosition;
				float single1 = position.sqrMagnitude;
				if (single1 < single)
				{
					this.Closest.ReservedCoverPoint = retreat;
					single = single1;
				}
			}
			if (flank != null)
			{
				position = flank.Position - this.Flank.Human.ServerPosition;
				float single2 = position.sqrMagnitude;
				if (single2 < single)
				{
					this.Closest.ReservedCoverPoint = flank;
					single = single2;
				}
			}
			if (advance != null)
			{
				position = advance.Position - this.Advance.Human.ServerPosition;
				float single3 = position.sqrMagnitude;
				if (single3 < single)
				{
					this.Closest.ReservedCoverPoint = advance;
					single = single3;
				}
			}
		}
	}
}