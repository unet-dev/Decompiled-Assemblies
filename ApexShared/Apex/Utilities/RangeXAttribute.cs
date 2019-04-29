using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.Utilities
{
	[AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
	public sealed class RangeXAttribute : PropertyAttribute
	{
		public readonly float min;

		public readonly float max;

		public string label
		{
			get;
			set;
		}

		public string tooltip
		{
			get;
			set;
		}

		public RangeXAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}