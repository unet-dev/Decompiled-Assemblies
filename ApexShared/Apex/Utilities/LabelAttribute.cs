using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.Utilities
{
	[AttributeUsage(AttributeTargets.Field, Inherited=true, AllowMultiple=false)]
	public sealed class LabelAttribute : PropertyAttribute
	{
		public string label
		{
			get;
			private set;
		}

		public string tooltip
		{
			get;
			private set;
		}

		public LabelAttribute(string label)
		{
			this.label = label;
		}

		public LabelAttribute(string label, string tooltip)
		{
			this.label = label;
			this.tooltip = tooltip;
		}
	}
}