using Apex;
using Apex.Utilities;
using System;
using UnityEngine;

namespace Apex.AI.Components
{
	internal class AIQuickStarts
	{
		public AIQuickStarts()
		{
		}

		internal static void LoadBalancer(GameObject target)
		{
			if (ComponentHelper.FindFirstComponentInScene<LoadBalancerComponent>() != null)
			{
				return;
			}
			if (target != null)
			{
				target.AddComponent<LoadBalancerComponent>();
				return;
			}
			(new GameObject("Load Balancer")).AddComponent<LoadBalancerComponent>();
			Debug.Log("No Load Balancer found, creating one.");
		}

		internal static void UtilityAIClient(GameObject target, bool ensureLoadBalancer)
		{
			target.AddIfMissing<UtilityAIComponent>();
			if (ensureLoadBalancer)
			{
				AIQuickStarts.LoadBalancer(null);
			}
		}
	}
}