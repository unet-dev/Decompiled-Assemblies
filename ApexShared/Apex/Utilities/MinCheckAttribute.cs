using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.Utilities
{
	[AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
	public sealed class MinCheckAttribute : PropertyAttribute
	{
		public string label
		{
			get;
			set;
		}

		public float min
		{
			get;
			private set;
		}

		public string tooltip
		{
			get;
			set;
		}

		public MinCheckAttribute(float min)
		{
			this.min = min;
		}

		public MinCheckAttribute(int min)
		{
			this.min = (float)min;
		}
	}
}