using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apex.DataStructures
{
	public class Vector3EqualityComparer : IEqualityComparer<Vector3>
	{
		private float _equalityThreshold;

		public Vector3EqualityComparer(float equalityThreshold)
		{
			this._equalityThreshold = equalityThreshold;
		}

		public bool Equals(Vector3 x, Vector3 y)
		{
			return (x - y).sqrMagnitude < this._equalityThreshold;
		}

		public int GetHashCode(Vector3 obj)
		{
			return obj.GetHashCode();
		}
	}
}