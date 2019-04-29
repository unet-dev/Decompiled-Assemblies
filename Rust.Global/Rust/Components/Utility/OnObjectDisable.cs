using Rust;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Rust.Components.Utility
{
	internal class OnObjectDisable : MonoBehaviour
	{
		public UnityEvent Action;

		public OnObjectDisable()
		{
		}

		private void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			this.Action.Invoke();
		}
	}
}