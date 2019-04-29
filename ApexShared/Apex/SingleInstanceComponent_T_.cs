using System;
using System.Reflection;
using UnityEngine;

namespace Apex
{
	public abstract class SingleInstanceComponent<T> : MonoBehaviour
	where T : MonoBehaviour
	{
		private static int _instanceMark;

		protected SingleInstanceComponent()
		{
		}

		private void Awake()
		{
			if (SingleInstanceComponent<T>._instanceMark <= 0)
			{
				SingleInstanceComponent<T>._instanceMark = 1;
				this.OnAwake();
				return;
			}
			Debug.LogWarning(string.Format("Removing superfluous {0} from scene.", typeof(T).Name));
			UnityEngine.Object.Destroy(this);
		}

		protected virtual void OnAwake()
		{
		}

		protected virtual void OnDestroy()
		{
			SingleInstanceComponent<T>._instanceMark = 0;
		}
	}
}