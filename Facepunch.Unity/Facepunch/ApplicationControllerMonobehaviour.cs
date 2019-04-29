using System;
using UnityEngine;

namespace Facepunch
{
	internal class ApplicationControllerMonobehaviour : MonoBehaviour
	{
		public ApplicationControllerMonobehaviour()
		{
		}

		public void OnApplicationQuit()
		{
			if (Facepunch.Application.Analytics != null)
			{
				Facepunch.Application.Analytics.OnQuit();
				Facepunch.Application.Analytics = null;
			}
		}

		public void Update()
		{
			Performance.Frame();
			Feedback.Frame();
			Threading.RunQueuedFunctionsOnMainThread();
		}
	}
}