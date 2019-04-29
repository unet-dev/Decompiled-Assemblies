using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.Utilities
{
	[AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
	public sealed class MaxCheckAttribute : PropertyAttribute
	{
		public string label
		{
			get;
			set;
		}

		public float max
		{
			get;
			private set;
		}

		public string tooltip
		{
			get;
			set;
		}

		public MaxCheckAttribute(float max)
		{
			this.max = max;
		}

		public MaxCheckAttribute(int max)
		{
			this.max = (float)max;
		}
	}
}