using Apex.LoadBalancing;
using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Rust.Ai
{
	public class CoverPointVolume : MonoBehaviour, IServerComponent, ILoadBalanced
	{
		public float DefaultCoverPointScore = 1f;

		public float CoverPointRayLength = 1f;

		public LayerMask CoverLayerMask;

		public Transform BlockerGroup;

		public Transform ManualCoverPointGroup;

		[ServerVar(Help="cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)")]
		public static float cover_point_sample_step_size;

		[ServerVar(Help="cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)")]
		public static float cover_point_sample_step_height;

		public readonly List<CoverPoint> CoverPoints = new List<CoverPoint>();

		private readonly List<CoverPointBlockerVolume> _coverPointBlockers = new List<CoverPointBlockerVolume>();

		private float _dynNavMeshBuildCompletionTime = -1f;

		private int _genAttempts;

		private Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

		public bool repeat
		{
			get
			{
				return true;
			}
		}

		static CoverPointVolume()
		{
			CoverPointVolume.cover_point_sample_step_size = 6f;
			CoverPointVolume.cover_point_sample_step_height = 2f;
		}

		public CoverPointVolume()
		{
		}

		private CoverPoint CalculateCoverPoint(NavMeshHit info)
		{
			RaycastHit raycastHit;
			CoverPointVolume.CoverType coverType = this.ProvidesCoverInDir(new Ray(info.position, -info.normal), this.CoverPointRayLength, out raycastHit);
			if (coverType == CoverPointVolume.CoverType.None)
			{
				return null;
			}
			CoverPoint coverPoint = new CoverPoint(this, this.DefaultCoverPointScore)
			{
				Position = info.position,
				Normal = -info.normal
			};
			if (coverType == CoverPointVolume.CoverType.Full)
			{
				coverPoint.NormalCoverType = CoverPoint.CoverType.Full;
			}
			else if (coverType == CoverPointVolume.CoverType.Partial)
			{
				coverPoint.NormalCoverType = CoverPoint.CoverType.Partial;
			}
			return coverPoint;
		}

		[ContextMenu("Clear Cover Points")]
		private void ClearCoverPoints()
		{
			this.CoverPoints.Clear();
			this._coverPointBlockers.Clear();
		}

		public bool Contains(Vector3 point)
		{
			Bounds bound = new Bounds(base.transform.position, base.transform.localScale);
			return bound.Contains(point);
		}

		[ContextMenu("Convert to Manual Cover Points")]
		public void ConvertToManualCoverPoints()
		{
			foreach (CoverPoint coverPoint in this.CoverPoints)
			{
				ManualCoverPoint position = (new GameObject("MCP")).AddComponent<ManualCoverPoint>();
				position.transform.localPosition = Vector3.zero;
				position.transform.position = coverPoint.Position;
				position.Normal = coverPoint.Normal;
				position.NormalCoverType = coverPoint.NormalCoverType;
				position.Volume = this;
			}
		}

		public float? ExecuteUpdate(float deltaTime, float nextInterval)
		{
			float? nullable;
			if (this.CoverPoints.Count == 0)
			{
				if (this._dynNavMeshBuildCompletionTime < 0f)
				{
					if (SingletonComponent<DynamicNavMesh>.Instance == null || !SingletonComponent<DynamicNavMesh>.Instance.enabled || !SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
					{
						this._dynNavMeshBuildCompletionTime = Time.realtimeSinceStartup;
					}
				}
				else if (this._genAttempts < 4 && Time.realtimeSinceStartup - this._dynNavMeshBuildCompletionTime > 0.25f)
				{
					this.GenerateCoverPoints(null);
					if (this.CoverPoints.Count != 0)
					{
						LoadBalancer.defaultBalancer.Remove(this);
						nullable = null;
						return nullable;
					}
					this._dynNavMeshBuildCompletionTime = Time.realtimeSinceStartup;
					this._genAttempts++;
					if (this._genAttempts >= 4)
					{
						UnityEngine.Object.Destroy(base.gameObject);
						LoadBalancer.defaultBalancer.Remove(this);
						nullable = null;
						return nullable;
					}
				}
			}
			return new float?(1f + UnityEngine.Random.@value * 2f);
		}

		public void GenerateCoverPoints(Transform coverPointGroup)
		{
			NavMeshHit navMeshHit;
			NavMeshHit vector3;
			float single = Time.realtimeSinceStartup;
			this.ClearCoverPoints();
			if (this.ManualCoverPointGroup == null)
			{
				this.ManualCoverPointGroup = coverPointGroup;
			}
			if (this.ManualCoverPointGroup == null)
			{
				this.ManualCoverPointGroup = base.transform;
			}
			if (this.ManualCoverPointGroup.childCount > 0)
			{
				ManualCoverPoint[] componentsInChildren = this.ManualCoverPointGroup.GetComponentsInChildren<ManualCoverPoint>();
				for (int i = 0; i < (int)componentsInChildren.Length; i++)
				{
					CoverPoint coverPoint = componentsInChildren[i].ToCoverPoint(this);
					this.CoverPoints.Add(coverPoint);
				}
			}
			if (this._coverPointBlockers.Count == 0 && this.BlockerGroup != null)
			{
				CoverPointBlockerVolume[] coverPointBlockerVolumeArray = this.BlockerGroup.GetComponentsInChildren<CoverPointBlockerVolume>();
				if (coverPointBlockerVolumeArray != null && coverPointBlockerVolumeArray.Length != 0)
				{
					this._coverPointBlockers.AddRange(coverPointBlockerVolumeArray);
				}
			}
			if (this.CoverPoints.Count == 0 && NavMesh.SamplePosition(base.transform.position, out navMeshHit, base.transform.localScale.y * CoverPointVolume.cover_point_sample_step_height, -1))
			{
				Vector3 vector31 = base.transform.position;
				Vector3 vector32 = base.transform.lossyScale * 0.5f;
				for (float j = vector31.x - vector32.x + 1f; j < vector31.x + vector32.x - 1f; j += CoverPointVolume.cover_point_sample_step_size)
				{
					for (float k = vector31.z - vector32.z + 1f; k < vector31.z + vector32.z - 1f; k += CoverPointVolume.cover_point_sample_step_size)
					{
						for (float l = vector31.y - vector32.y; l < vector31.y + vector32.y; l += CoverPointVolume.cover_point_sample_step_height)
						{
							if (NavMesh.FindClosestEdge(new Vector3(j, l, k), out vector3, navMeshHit.mask))
							{
								vector3.position = new Vector3(vector3.position.x, vector3.position.y + 0.5f, vector3.position.z);
								bool flag = true;
								foreach (CoverPoint coverPoint1 in this.CoverPoints)
								{
									if ((coverPoint1.Position - vector3.position).sqrMagnitude >= CoverPointVolume.cover_point_sample_step_size * CoverPointVolume.cover_point_sample_step_size)
									{
										continue;
									}
									flag = false;
									goto Label0;
								}
							Label0:
								if (flag)
								{
									CoverPoint coverPoint2 = this.CalculateCoverPoint(vector3);
									if (coverPoint2 != null)
									{
										this.CoverPoints.Add(coverPoint2);
									}
								}
							}
						}
					}
				}
			}
		}

		public Bounds GetBounds()
		{
			if (Mathf.Approximately(this.bounds.center.sqrMagnitude, 0f))
			{
				this.bounds = new Bounds(base.transform.position, base.transform.localScale);
			}
			return this.bounds;
		}

		private void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			LoadBalancer.defaultBalancer.Remove(this);
		}

		private void OnEnable()
		{
			LoadBalancer.defaultBalancer.Add(this);
		}

		[ContextMenu("Pre-Generate Cover Points")]
		public void PreGenerateCoverPoints()
		{
			this.GenerateCoverPoints(null);
		}

		internal CoverPointVolume.CoverType ProvidesCoverInDir(Ray ray, float maxDistance, out RaycastHit rayHit)
		{
			rayHit = new RaycastHit();
			if (ray.origin.IsNaNOrInfinity())
			{
				return CoverPointVolume.CoverType.None;
			}
			if (ray.direction.IsNaNOrInfinity())
			{
				return CoverPointVolume.CoverType.None;
			}
			if (ray.direction == Vector3.zero)
			{
				return CoverPointVolume.CoverType.None;
			}
			ray.origin = ray.origin + PlayerEyes.EyeOffset;
			if (Physics.Raycast(ray.origin, ray.direction, out rayHit, maxDistance, this.CoverLayerMask))
			{
				return CoverPointVolume.CoverType.Full;
			}
			ray.origin = ray.origin + PlayerEyes.DuckOffset;
			if (Physics.Raycast(ray.origin, ray.direction, out rayHit, maxDistance, this.CoverLayerMask))
			{
				return CoverPointVolume.CoverType.Partial;
			}
			return CoverPointVolume.CoverType.None;
		}

		internal enum CoverType
		{
			None,
			Partial,
			Full
		}
	}
}