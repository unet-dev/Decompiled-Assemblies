using System;
using System.Diagnostics;
using UnityEngine;

namespace Rust
{
	public static class Global
	{
		public static Func<string, GameObject> FindPrefab;

		public static Func<string, GameObject> CreatePrefab;

		public static Action OpenMainMenu;

		private static MonoBehaviour _runner;

		private static System.Diagnostics.Process _process;

		public static System.Diagnostics.Process Process
		{
			get
			{
				if (Global._process == null)
				{
					Global._process = System.Diagnostics.Process.GetCurrentProcess();
				}
				return Global._process;
			}
		}

		public static MonoBehaviour Runner
		{
			get
			{
				if (Global._runner == null)
				{
					GameObject gameObject = new GameObject("Coroutine Runner");
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					Global._runner = gameObject.AddComponent<NullMonoBehaviour>();
				}
				return Global._runner;
			}
		}
	}
}