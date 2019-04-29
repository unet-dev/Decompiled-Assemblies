using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	public sealed class MinAttribute : PropertyAttribute
	{
		public readonly float min;

		public MinAttribute(float min)
		{
			this.min = min;
		}
	}
}