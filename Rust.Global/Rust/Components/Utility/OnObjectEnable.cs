using System;
using UnityEngine;
using UnityEngine.Events;

namespace Rust.Components.Utility
{
	internal class OnObjectEnable : MonoBehaviour
	{
		public UnityEvent Action;

		public OnObjectEnable()
		{
		}

		private void OnEnable()
		{
			this.Action.Invoke();
		}
	}
}