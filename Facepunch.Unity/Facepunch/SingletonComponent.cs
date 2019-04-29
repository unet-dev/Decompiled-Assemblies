using System;
using UnityEngine;

namespace Facepunch
{
	public abstract class SingletonComponent : MonoBehaviour
	{
		protected SingletonComponent()
		{
		}

		protected virtual void Awake()
		{
			this.SingletonSetup();
		}

		protected virtual void OnDestroy()
		{
			this.SingletonClear();
		}

		public abstract void SingletonClear();

		public abstract void SingletonSetup();
	}
}