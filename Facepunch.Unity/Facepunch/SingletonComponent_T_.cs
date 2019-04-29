using System;
using UnityEngine;

namespace Facepunch
{
	public abstract class SingletonComponent<T> : SingletonComponent
	where T : MonoBehaviour
	{
		private static T instance;

		public static T Instance
		{
			get
			{
				return SingletonComponent<T>.instance;
			}
		}

		static SingletonComponent()
		{
		}

		protected SingletonComponent()
		{
		}

		public override void SingletonClear()
		{
			if (SingletonComponent<T>.instance == this)
			{
				SingletonComponent<T>.instance = default(T);
			}
		}

		public override void SingletonSetup()
		{
			if (SingletonComponent<T>.instance != this)
			{
				SingletonComponent<T>.instance = (T)(this as T);
			}
		}
	}
}