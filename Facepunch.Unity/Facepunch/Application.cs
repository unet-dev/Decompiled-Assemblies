using Facepunch.Models;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	public static class Application
	{
		public static Facepunch.Analytics Analytics;

		private static MonoBehaviour _controller;

		public static Facepunch.Models.Manifest Manifest;

		public static MonoBehaviour Controller
		{
			get
			{
				if (Facepunch.Application._controller == null)
				{
					GameObject gameObject = new GameObject("Facepunch.Application");
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					Facepunch.Application._controller = gameObject.AddComponent<ApplicationControllerMonobehaviour>();
				}
				return Facepunch.Application._controller;
			}
		}

		public static BaseIntegration Integration
		{
			get;
			set;
		}

		public static void Initialize(BaseIntegration integration)
		{
			Mono.FixHttpsValidation();
			Facepunch.Application.Integration = integration;
			ExceptionReporter.InstallHooks();
			Facepunch.Manifest.Download();
		}
	}
}