using System;
using UnityEngine;

namespace Apex
{
	public abstract class ExtendedMonoBehaviour : MonoBehaviour
	{
		private bool _hasStarted;

		protected ExtendedMonoBehaviour()
		{
		}

		protected virtual void OnEnable()
		{
			if (this._hasStarted)
			{
				this.OnStartAndEnable();
			}
		}

		protected virtual void OnStartAndEnable()
		{
		}

		protected virtual void Start()
		{
			this._hasStarted = true;
			this.OnStartAndEnable();
		}
	}
}