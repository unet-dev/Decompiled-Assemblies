using System;
using System.Reflection;
using UnityEngine;

namespace Apex.DataStructures
{
	public class PolygonXZ
	{
		public readonly static PolygonXZ empty;

		private Vector3[] _points;

		public int count
		{
			get
			{
				return (int)this._points.Length;
			}
		}

		public Vector3 this[int idx]
		{
			get
			{
				return this._points[idx];
			}
			set
			{
				this._points[idx] = value;
			}
		}

		static PolygonXZ()
		{
			PolygonXZ.empty = new PolygonXZ(new Vector3[0]);
		}

		public PolygonXZ(params Vector3[] points)
		{
			this._points = points;
		}

		public PolygonXZ(int capacity)
		{
			this._points = new Vector3[capacity];
		}

		public Bounds CalculateBounds()
		{
			Vector3 vector3 = this._points[0];
			Vector3 vector31 = vector3;
			for (int i = 1; i < (int)this._points.Length; i++)
			{
				Vector3 vector32 = this._points[i];
				if (vector32.x > vector3.x)
				{
					vector3.x = vector32.x;
				}
				else if (vector32.x < vector31.x)
				{
					vector31.x = vector32.x;
				}
				if (vector32.z > vector3.z)
				{
					vector3.z = vector32.z;
				}
				else if (vector32.z < vector31.z)
				{
					vector31.z = vector32.z;
				}
				if (vector32.y > vector3.y)
				{
					vector3.y = vector32.y;
				}
				else if (vector32.y < vector31.y)
				{
					vector31.y = vector32.y;
				}
			}
			Vector3 vector33 = (vector3 - vector31) + new Vector3(0.1f, 0.1f, 0.1f);
			return new Bounds(vector31 + (vector33 / 2f), vector33);
		}

		public bool Contains(Vector3 test)
		{
			bool flag = false;
			int num = 0;
			int length = (int)this._points.Length - 1;
			while (num < (int)this._points.Length)
			{
				if (this._points[num].z > test.z != this._points[length].z > test.z && test.x < (this._points[length].x - this._points[num].x) * (test.z - this._points[num].z) / (this._points[length].z - this._points[num].z) + this._points[num].x)
				{
					flag = !flag;
				}
				int num1 = num;
				num = num1 + 1;
				length = num1;
			}
			return flag;
		}
	}
}