using System;
using UnityEngine;

namespace Rust.Ai
{
	public class ManualCoverPoint : FacepunchBehaviour
	{
		public bool IsDynamic;

		public float Score = 2f;

		public CoverPointVolume Volume;

		public Vector3 Normal;

		public CoverPoint.CoverType NormalCoverType;

		public float DirectionMagnitude
		{
			get
			{
				if (this.Volume == null)
				{
					return 1f;
				}
				return this.Volume.CoverPointRayLength;
			}
		}

		public Vector3 Position
		{
			get
			{
				return base.transform.position;
			}
		}

		public ManualCoverPoint()
		{
		}

		private void Awake()
		{
			if (base.transform.parent != null)
			{
				this.Volume = base.transform.parent.GetComponent<CoverPointVolume>();
			}
		}

		public CoverPoint ToCoverPoint(CoverPointVolume volume)
		{
			Vector3 vector3;
			this.Volume = volume;
			if (!this.IsDynamic)
			{
				Vector3 normal = (base.transform.rotation * this.Normal).normalized;
				return new CoverPoint(this.Volume, this.Score)
				{
					IsDynamic = false,
					Position = base.transform.position,
					Normal = normal,
					NormalCoverType = this.NormalCoverType
				};
			}
			CoverPoint coverPoint = new CoverPoint(this.Volume, this.Score)
			{
				IsDynamic = true,
				SourceTransform = base.transform,
				NormalCoverType = this.NormalCoverType
			};
			Transform transforms = base.transform;
			if (transforms != null)
			{
				vector3 = transforms.position;
			}
			else
			{
				vector3 = Vector3.zero;
			}
			coverPoint.Position = vector3;
			return coverPoint;
		}
	}
}