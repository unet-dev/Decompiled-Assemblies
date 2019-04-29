using ConVar;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	public class AiLocationManager : FacepunchBehaviour, IServerComponent
	{
		public static List<AiLocationManager> Managers;

		[SerializeField]
		public AiLocationSpawner MainSpawner;

		[SerializeField]
		public AiLocationSpawner.SquadSpawnerLocation LocationWhenMainSpawnerIsNull = AiLocationSpawner.SquadSpawnerLocation.None;

		public Transform CoverPointGroup;

		public Transform PatrolPointGroup;

		public CoverPointVolume DynamicCoverPointVolume;

		public bool SnapCoverPointsToGround;

		private List<PathInterestNode> patrolPoints;

		public AiLocationSpawner.SquadSpawnerLocation LocationType
		{
			get
			{
				if (this.MainSpawner == null)
				{
					return this.LocationWhenMainSpawnerIsNull;
				}
				return this.MainSpawner.Location;
			}
		}

		static AiLocationManager()
		{
			AiLocationManager.Managers = new List<AiLocationManager>();
		}

		public AiLocationManager()
		{
		}

		private void Awake()
		{
			NavMeshHit navMeshHit;
			AiLocationManager.Managers.Add(this);
			if (this.SnapCoverPointsToGround)
			{
				ManualCoverPoint[] componentsInChildren = this.CoverPointGroup.GetComponentsInChildren<ManualCoverPoint>();
				for (int i = 0; i < (int)componentsInChildren.Length; i++)
				{
					ManualCoverPoint manualCoverPoint = componentsInChildren[i];
					if (NavMesh.SamplePosition(manualCoverPoint.Position, out navMeshHit, 4f, -1))
					{
						manualCoverPoint.transform.position = navMeshHit.position;
					}
				}
			}
		}

		public PathInterestNode GetFirstPatrolPointInRange(Vector3 from, float minRange = 10f, float maxRange = 100f)
		{
			PathInterestNode pathInterestNode;
			if (this.PatrolPointGroup == null)
			{
				return null;
			}
			if (this.patrolPoints == null)
			{
				this.patrolPoints = new List<PathInterestNode>(this.PatrolPointGroup.GetComponentsInChildren<PathInterestNode>());
			}
			if (this.patrolPoints.Count == 0)
			{
				return null;
			}
			float single = minRange * minRange;
			float single1 = maxRange * maxRange;
			List<PathInterestNode>.Enumerator enumerator = this.patrolPoints.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PathInterestNode current = enumerator.Current;
					float single2 = (current.transform.position - from).sqrMagnitude;
					if (single2 < single || single2 > single1)
					{
						continue;
					}
					pathInterestNode = current;
					return pathInterestNode;
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return pathInterestNode;
		}

		public PathInterestNode GetRandomPatrolPointInRange(Vector3 from, float minRange = 10f, float maxRange = 100f, PathInterestNode currentPatrolPoint = null)
		{
			if (this.PatrolPointGroup == null)
			{
				return null;
			}
			if (this.patrolPoints == null)
			{
				this.patrolPoints = new List<PathInterestNode>(this.PatrolPointGroup.GetComponentsInChildren<PathInterestNode>());
			}
			if (this.patrolPoints.Count == 0)
			{
				return null;
			}
			float single = minRange * minRange;
			float single1 = maxRange * maxRange;
			for (int i = 0; i < 20; i++)
			{
				PathInterestNode item = this.patrolPoints[UnityEngine.Random.Range(0, this.patrolPoints.Count)];
				if (UnityEngine.Time.time >= item.NextVisitTime)
				{
					float single2 = (item.transform.position - from).sqrMagnitude;
					if (single2 >= single && single2 <= single1)
					{
						item.NextVisitTime = UnityEngine.Time.time + ConVar.AI.npc_patrol_point_cooldown;
						return item;
					}
				}
				else if (item == currentPatrolPoint)
				{
					return null;
				}
			}
			return null;
		}

		private void OnDestroy()
		{
			AiLocationManager.Managers.Remove(this);
		}
	}
}