using System;
using UnityEngine;

namespace Rust
{
	public static class Application
	{
		public static bool isQuitting;

		public static bool isLoading;

		public static bool isReceiving;

		public static bool isLoadingSave;

		public static bool isLoadingPrefabs;

		public static string dataPath
		{
			get
			{
				return UnityEngine.Application.dataPath;
			}
		}

		public static string installPath
		{
			get
			{
				if (UnityEngine.Application.platform == RuntimePlatform.OSXPlayer)
				{
					return string.Concat(UnityEngine.Application.dataPath, "/../..");
				}
				return string.Concat(UnityEngine.Application.dataPath, "/..");
			}
		}

		static Application()
		{
		}

		public static void Quit()
		{
			Rust.Application.isQuitting = true;
			UnityEngine.Application.Quit();
		}
	}
}