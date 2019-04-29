using Rust.Ai;
using Rust.Ai.HTN;
using Rust.Ai.HTN.Reasoning;
using Rust.Ai.HTN.ScientistJunkpile;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai.HTN.ScientistJunkpile.Reasoners
{
	public class FollowWaypointsReasoner : INpcReasoner
	{
		private bool isFollowingWaypoints;

		private bool isFirstTick = true;

		private bool hasAlreadyPassedOnPrevCheck;

		private int CurrentWaypointIndex
		{
			get;
			set;
		}

		private bool IsWaitingAtWaypoint
		{
			get;
			set;
		}

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

		private float WaypointDelayTime
		{
			get;
			set;
		}

		private int WaypointDirection
		{
			get;
			set;
		}

		private Rust.Ai.WaypointSet WaypointSet
		{
			get;
			set;
		}

		public FollowWaypointsReasoner()
		{
		}

		public int GetClosestWaypointIndex(Vector3 position)
		{
			if (this.WaypointSet == null || this.WaypointSet.Points.Count == 0)
			{
				return this.CurrentWaypointIndex;
			}
			Rust.Ai.WaypointSet.Waypoint item = this.WaypointSet.Points[this.CurrentWaypointIndex];
			item.IsOccupied = false;
			this.WaypointSet.Points[this.CurrentWaypointIndex] = item;
			float single = Single.MaxValue;
			int num = -1;
			for (int i = 0; i < this.WaypointSet.Points.Count; i++)
			{
				Rust.Ai.WaypointSet.Waypoint waypoint = this.WaypointSet.Points[i];
				if (waypoint.Transform != null)
				{
					float transform = (waypoint.Transform.position - position).sqrMagnitude;
					if (transform < single)
					{
						single = transform;
						num = i;
					}
				}
			}
			if (num >= 0)
			{
				return num;
			}
			return this.CurrentWaypointIndex;
		}

		public int GetNextWaypointIndex()
		{
			if (this.WaypointSet == null || this.WaypointSet.Points.Count == 0 || this.WaypointSet.Points[this.PeekNextWaypointIndex()].IsOccupied)
			{
				return this.CurrentWaypointIndex;
			}
			int currentWaypointIndex = this.CurrentWaypointIndex;
			if (currentWaypointIndex >= 0 && currentWaypointIndex < this.WaypointSet.Points.Count)
			{
				Rust.Ai.WaypointSet.Waypoint item = this.WaypointSet.Points[currentWaypointIndex];
				item.IsOccupied = false;
				this.WaypointSet.Points[currentWaypointIndex] = item;
			}
			Rust.Ai.WaypointSet.NavModes navMode = this.WaypointSet.NavMode;
			if (navMode == Rust.Ai.WaypointSet.NavModes.Loop)
			{
				currentWaypointIndex++;
				if (currentWaypointIndex >= this.WaypointSet.Points.Count)
				{
					currentWaypointIndex = 0;
				}
				else if (currentWaypointIndex < 0)
				{
					currentWaypointIndex = this.WaypointSet.Points.Count - 1;
				}
			}
			else
			{
				if (navMode != Rust.Ai.WaypointSet.NavModes.PingPong)
				{
					throw new ArgumentOutOfRangeException();
				}
				currentWaypointIndex += this.WaypointDirection;
				if (currentWaypointIndex >= this.WaypointSet.Points.Count)
				{
					currentWaypointIndex = this.CurrentWaypointIndex - 1;
					this.WaypointDirection = -1;
				}
				else if (currentWaypointIndex < 0)
				{
					currentWaypointIndex = 0;
					this.WaypointDirection = 1;
				}
			}
			if (currentWaypointIndex >= 0 && currentWaypointIndex < this.WaypointSet.Points.Count)
			{
				Rust.Ai.WaypointSet.Waypoint waypoint = this.WaypointSet.Points[currentWaypointIndex];
				waypoint.IsOccupied = true;
				this.WaypointSet.Points[currentWaypointIndex] = waypoint;
			}
			return currentWaypointIndex;
		}

		public int PeekNextWaypointIndex()
		{
			if (this.WaypointSet == null || this.WaypointSet.Points.Count == 0)
			{
				return this.CurrentWaypointIndex;
			}
			int currentWaypointIndex = this.CurrentWaypointIndex;
			Rust.Ai.WaypointSet.NavModes navMode = this.WaypointSet.NavMode;
			if (navMode == Rust.Ai.WaypointSet.NavModes.Loop)
			{
				currentWaypointIndex++;
				if (currentWaypointIndex >= this.WaypointSet.Points.Count)
				{
					currentWaypointIndex = 0;
				}
				else if (currentWaypointIndex < 0)
				{
					currentWaypointIndex = this.WaypointSet.Points.Count - 1;
				}
			}
			else if (navMode == Rust.Ai.WaypointSet.NavModes.PingPong)
			{
				currentWaypointIndex += this.WaypointDirection;
				if (currentWaypointIndex >= this.WaypointSet.Points.Count)
				{
					currentWaypointIndex = this.CurrentWaypointIndex - 1;
				}
				else if (currentWaypointIndex < 0)
				{
					currentWaypointIndex = 0;
				}
			}
			return currentWaypointIndex;
		}

		public void Tick(IHTNAgent npc, float deltaTime, float time)
		{
			NavMeshHit navMeshHit;
			ScientistJunkpileContext npcContext = npc.AiDomain.NpcContext as ScientistJunkpileContext;
			if (npcContext == null || npcContext.Domain.NavAgent == null)
			{
				return;
			}
			if (npcContext.Location == null || npcContext.Location.PatrolPointGroup == null)
			{
				return;
			}
			if (this.WaypointSet == null)
			{
				this.WaypointSet = npcContext.Location.PatrolPointGroup.GetComponent<Rust.Ai.WaypointSet>();
			}
			if (this.WaypointSet == null || this.WaypointSet.Points.Count == 0)
			{
				return;
			}
			if (npcContext.IsFact(Facts.IsReturningHome) || npcContext.IsFact(Facts.HasEnemyTarget) || npcContext.IsFact(Facts.NearbyAnimal) || !npcContext.IsFact(Facts.IsUsingTool))
			{
				this.isFollowingWaypoints = false;
				this.hasAlreadyPassedOnPrevCheck = false;
				return;
			}
			if (!this.isFollowingWaypoints)
			{
				if (!this.hasAlreadyPassedOnPrevCheck && (npcContext.GetPreviousFact(Facts.HasEnemyTarget) == 1 || npcContext.GetPreviousFact(Facts.NearbyAnimal) == 1) || this.isFirstTick)
				{
					this.CurrentWaypointIndex = this.GetClosestWaypointIndex(npcContext.BodyPosition);
					if (!this.isFirstTick)
					{
						this.hasAlreadyPassedOnPrevCheck = true;
					}
					else
					{
						this.isFirstTick = false;
					}
				}
				Rust.Ai.WaypointSet.Waypoint item = this.WaypointSet.Points[this.CurrentWaypointIndex];
				if (item.Transform == null)
				{
					this.CurrentWaypointIndex = this.GetNextWaypointIndex();
					this.isFollowingWaypoints = false;
					return;
				}
				Vector3 transform = item.Transform.position;
				if ((npcContext.Memory.TargetDestination - transform).sqrMagnitude > 0.1f && NavMesh.SamplePosition(transform + (Vector3.up * 2f), out navMeshHit, 4f, npcContext.Domain.NavAgent.areaMask))
				{
					item.Transform.position = navMeshHit.position;
					npcContext.Domain.SetDestination(navMeshHit.position);
					this.isFollowingWaypoints = true;
					npcContext.SetFact(Facts.IsNavigating, true, true, true, true);
					return;
				}
			}
			float single = 2f;
			float navAgent = npcContext.Domain.NavAgent.stoppingDistance * npcContext.Domain.NavAgent.stoppingDistance;
			if ((npcContext.BodyPosition - npcContext.Memory.TargetDestination).sqrMagnitude > navAgent + single)
			{
				return;
			}
			this.CurrentWaypointIndex = this.GetNextWaypointIndex();
			this.isFollowingWaypoints = false;
		}
	}
}