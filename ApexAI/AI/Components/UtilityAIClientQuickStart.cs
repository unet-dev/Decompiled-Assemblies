using Apex;
using System;
using UnityEngine;

namespace Apex.AI.Components
{
	[AddComponentMenu("Apex/Quick Starts/AI/Utility AI Client", 100)]
	public class UtilityAIClientQuickStart : ApexQuickStartComponent
	{
		public UtilityAIClientQuickStart()
		{
		}

		public override GameObject Apply(bool isPrefab)
		{
			AIQuickStarts.UtilityAIClient(base.gameObject, !isPrefab);
			return null;
		}
	}
}