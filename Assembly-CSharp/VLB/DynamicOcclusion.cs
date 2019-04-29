using System;
using UnityEngine;

namespace VLB
{
	[DisallowMultipleComponent]
	[ExecuteInEditMode]
	[HelpURL("http://saladgamer.com/vlb-doc/comp-dynocclusion/")]
	[RequireComponent(typeof(VolumetricLightBeam))]
	public class DynamicOcclusion : MonoBehaviour
	{
		public LayerMask layerMask = -1;

		public float minOccluderArea;

		public int waitFrameCount = 3;

		public float minSurfaceRatio = 0.5f;

		public float maxSurfaceDot = 0.25f;

		public PlaneAlignment planeAlignment;

		public float planeOffset = 0.1f;

		private VolumetricLightBeam m_Master;

		private int m_FrameCountToWait;

		private float m_RangeMultiplier = 1f;

		private uint m_PrevNonSubHitDirectionId;

		public DynamicOcclusion()
		{
		}

		private RaycastHit GetBestHit(Vector3 rayPos, Vector3 rayDir)
		{
			RaycastHit[] raycastHitArray = Physics.RaycastAll(rayPos, rayDir, this.m_Master.fadeEnd * this.m_RangeMultiplier, this.layerMask.@value);
			int num = -1;
			float single = Single.MaxValue;
			for (int i = 0; i < (int)raycastHitArray.Length; i++)
			{
				if (!raycastHitArray[i].collider.isTrigger && raycastHitArray[i].collider.bounds.GetMaxArea2D() >= this.minOccluderArea && raycastHitArray[i].distance < single)
				{
					single = raycastHitArray[i].distance;
					num = i;
				}
			}
			if (num != -1)
			{
				return raycastHitArray[num];
			}
			return new RaycastHit();
		}

		private Vector3 GetDirection(uint dirInt)
		{
			dirInt %= Enum.GetValues(typeof(DynamicOcclusion.Direction)).Length;
			switch (dirInt)
			{
				case 0:
				{
					return base.transform.up;
				}
				case 1:
				{
					return base.transform.right;
				}
				case 2:
				{
					return -base.transform.up;
				}
				case 3:
				{
					return -base.transform.right;
				}
			}
			return Vector3.zero;
		}

		private Vector3 GetRandomVectorAround(Vector3 direction, float angleDiff)
		{
			float single = angleDiff * 0.5f;
			return Quaternion.Euler(UnityEngine.Random.Range(-single, single), UnityEngine.Random.Range(-single, single), UnityEngine.Random.Range(-single, single)) * direction;
		}

		private bool IsHitValid(RaycastHit hit)
		{
			if (!hit.collider)
			{
				return false;
			}
			return Vector3.Dot(hit.normal, -base.transform.forward) >= this.maxSurfaceDot;
		}

		private void LateUpdate()
		{
			if (this.m_FrameCountToWait <= 0)
			{
				this.ProcessRaycasts();
				this.m_FrameCountToWait = this.waitFrameCount;
			}
			this.m_FrameCountToWait--;
		}

		private void OnDisable()
		{
			this.SetHitNull();
		}

		private void OnEnable()
		{
			this.m_Master = base.GetComponent<VolumetricLightBeam>();
			Debug.Assert(this.m_Master);
		}

		private void OnValidate()
		{
			this.minOccluderArea = Mathf.Max(this.minOccluderArea, 0f);
			this.waitFrameCount = Mathf.Clamp(this.waitFrameCount, 1, 60);
		}

		private void ProcessRaycasts()
		{
			RaycastHit bestHit = this.GetBestHit(base.transform.position, base.transform.forward);
			if (!this.IsHitValid(bestHit))
			{
				this.SetHitNull();
				return;
			}
			if (this.minSurfaceRatio > 0.5f)
			{
				for (uint i = 0; i < Enum.GetValues(typeof(DynamicOcclusion.Direction)).Length; i++)
				{
					Vector3 direction = this.GetDirection(i + this.m_PrevNonSubHitDirectionId);
					Vector3 mMaster = base.transform.position + ((direction * this.m_Master.coneRadiusStart) * (this.minSurfaceRatio * 2f - 1f));
					Vector3 vector3 = (base.transform.position + (base.transform.forward * this.m_Master.fadeEnd)) + ((direction * this.m_Master.coneRadiusEnd) * (this.minSurfaceRatio * 2f - 1f));
					RaycastHit raycastHit = this.GetBestHit(mMaster, vector3 - mMaster);
					if (!this.IsHitValid(raycastHit))
					{
						this.m_PrevNonSubHitDirectionId = i;
						this.SetHitNull();
						return;
					}
					if (raycastHit.distance > bestHit.distance)
					{
						bestHit = raycastHit;
					}
				}
			}
			this.SetHit(bestHit);
		}

		private void SetClippingPlane(Plane planeWS)
		{
			planeWS = planeWS.TranslateCustom(planeWS.normal * this.planeOffset);
			this.m_Master.SetClippingPlane(planeWS);
		}

		private void SetClippingPlaneOff()
		{
			this.m_Master.SetClippingPlaneOff();
		}

		private void SetHit(RaycastHit hit)
		{
			PlaneAlignment planeAlignment = this.planeAlignment;
			if (planeAlignment == PlaneAlignment.Surface || planeAlignment != PlaneAlignment.Beam)
			{
				this.SetClippingPlane(new Plane(hit.normal, hit.point));
				return;
			}
			this.SetClippingPlane(new Plane(-base.transform.forward, hit.point));
		}

		private void SetHitNull()
		{
			this.SetClippingPlaneOff();
		}

		private void Start()
		{
			if (Application.isPlaying)
			{
				TriggerZone component = base.GetComponent<TriggerZone>();
				if (component)
				{
					this.m_RangeMultiplier = Mathf.Max(1f, component.rangeMultiplier);
				}
			}
		}

		private enum Direction
		{
			Up,
			Right,
			Down,
			Left
		}
	}
}