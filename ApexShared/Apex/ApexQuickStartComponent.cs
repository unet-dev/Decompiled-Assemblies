using System;
using UnityEngine;

namespace Apex
{
	public abstract class ApexQuickStartComponent : MonoBehaviour
	{
		protected ApexQuickStartComponent()
		{
		}

		public abstract GameObject Apply(bool isPrefab);
	}
}