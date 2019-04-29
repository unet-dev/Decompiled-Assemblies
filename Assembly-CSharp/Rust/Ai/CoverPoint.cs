using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai
{
	public class CoverPoint
	{
		public CoverPoint.CoverType NormalCoverType;

		public bool IsDynamic;

		public Transform SourceTransform;

		private Vector3 _staticPosition;

		private Vector3 _staticNormal;

		public bool IsCompromised
		{
			get;
			set;
		}

		public bool IsReserved
		{
			get
			{
				return this.ReservedFor != null;
			}
		}

		public Vector3 Normal
		{
			get
			{
				if (!this.IsDynamic || !(this.SourceTransform != null))
				{
					return this._staticNormal;
				}
				return this.SourceTransform.forward;
			}
			set
			{
				this._staticNormal = value;
			}
		}

		public Vector3 Position
		{
			get
			{
				if (!this.IsDynamic || !(this.SourceTransform != null))
				{
					return this._staticPosition;
				}
				return this.SourceTransform.position;
			}
			set
			{
				this._staticPosition = value;
			}
		}

		public BaseEntity ReservedFor
		{
			get;
			set;
		}

		public float Score
		{
			get;
			set;
		}

		public CoverPointVolume Volume
		{
			get;
			private set;
		}

		public CoverPoint(CoverPointVolume volume, float score)
		{
			this.Volume = volume;
			this.Score = score;
		}

		public void CoverIsCompromised(float cooldown)
		{
			if (this.IsCompromised)
			{
				return;
			}
			if (this.Volume != null)
			{
				this.Volume.StartCoroutine(this.StartCooldown(cooldown));
			}
		}

		public bool IsValidFor(BaseEntity entity)
		{
			if (this.IsCompromised)
			{
				return false;
			}
			if (this.ReservedFor == null)
			{
				return true;
			}
			return this.ReservedFor == entity;
		}

		public bool ProvidesCoverFromPoint(Vector3 point, float arcThreshold)
		{
			Vector3 position = (this.Position - point).normalized;
			return Vector3.Dot(this.Normal, position) < arcThreshold;
		}

		private IEnumerator StartCooldown(float cooldown)
		{
			CoverPoint coverPoint = null;
			coverPoint.IsCompromised = true;
			yield return CoroutineEx.waitForSeconds(cooldown);
			coverPoint.IsCompromised = false;
		}

		public enum CoverType
		{
			Full,
			Partial,
			None
		}
	}
}