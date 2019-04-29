using System;
using UnityEngine;

namespace Facepunch
{
	public class TickComponent : MonoBehaviour
	{
		public static TickComponent Instance;

		public TickComponent()
		{
		}

		public static void Init()
		{
			if (TickComponent.Instance != null)
			{
				return;
			}
			(new GameObject("Tick Manager")).AddComponent<TickComponent>();
		}

		private void LateUpdate()
		{
			Tick.OnFrameLate();
		}

		private void OnEnable()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			TickComponent.Instance = this;
		}

		private void Update()
		{
			Tick.OnFrame();
		}
	}
}