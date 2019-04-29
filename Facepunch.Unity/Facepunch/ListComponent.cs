using System;
using UnityEngine;

namespace Facepunch
{
	public abstract class ListComponent : MonoBehaviour
	{
		protected ListComponent()
		{
		}

		public abstract void Clear();

		protected virtual void OnDisable()
		{
			this.Clear();
		}

		protected virtual void OnEnable()
		{
			this.Setup();
		}

		public abstract void Setup();
	}
}