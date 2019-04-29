using System;
using UnityEngine;

namespace Apex.DataStructures
{
	public struct RectangleXZ
	{
		private float _minX;

		private float _minZ;

		private float _maxX;

		private float _maxZ;

		public Vector3 center
		{
			get
			{
				return new Vector3(this._minX + (this._maxX - this._minX) / 2f, 0f, this._minZ + (this._maxZ - this._minZ) / 2f);
			}
		}

		public float depth
		{
			get
			{
				return this._maxZ - this._minZ;
			}
		}

		public Vector3 size
		{
			get
			{
				return new Vector3(this.width, 0f, this.depth);
			}
		}

		public float width
		{
			get
			{
				return this._maxX - this._minX;
			}
		}

		public RectangleXZ(Vector3 center, float width, float depth)
		{
			this._minX = center.x - width / 2f;
			this._minZ = center.z - depth / 2f;
			this._maxX = center.x + width / 2f;
			this._maxZ = center.z + depth / 2f;
		}

		public RectangleXZ(float minX, float minZ, float width, float depth)
		{
			this._minX = minX;
			this._minZ = minZ;
			this._maxX = this._minX + width;
			this._maxZ = this._minZ + depth;
		}

		public bool Contains(Vector3 point)
		{
			if (point.x < this._minX || point.x > this._maxX)
			{
				return false;
			}
			if (point.z < this._minZ)
			{
				return false;
			}
			return point.z <= this._maxZ;
		}

		public bool Contains(RectangleXZ other)
		{
			if (other._maxZ > this._maxZ || other._minZ < this._minZ || other._maxX > this._maxX)
			{
				return false;
			}
			return other._minX >= this._minX;
		}

		public static RectangleXZ MinMaxRect(float minX, float minZ, float maxX, float maxZ)
		{
			RectangleXZ rectangleXZ = new RectangleXZ()
			{
				_minX = minX,
				_minZ = minZ,
				_maxX = maxX,
				_maxZ = maxZ
			};
			return rectangleXZ;
		}

		public bool Overlaps(RectangleXZ other)
		{
			if (other._maxX <= this._minX || other._minX >= this._maxX)
			{
				return false;
			}
			if (other._maxZ <= this._minZ)
			{
				return false;
			}
			return other._minZ < this._maxZ;
		}

		public bool Overlaps(Bounds b)
		{
			if (b.max.x <= this._minX || b.min.x >= this._maxX)
			{
				return false;
			}
			if (b.max.z <= this._minZ)
			{
				return false;
			}
			return b.min.z < this._maxZ;
		}
	}
}