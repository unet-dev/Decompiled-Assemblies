using Rust;
using System;
using UnityEngine;

namespace ConVar
{
	[Factory("gc")]
	public class GC : ConsoleSystem
	{
		private static int m_buffer;

		[ClientVar]
		public static int buffer
		{
			get
			{
				return ConVar.GC.m_buffer;
			}
			set
			{
				ConVar.GC.m_buffer = Mathf.Clamp(value, 64, 2048);
			}
		}

		static GC()
		{
			ConVar.GC.m_buffer = 256;
		}

		public GC()
		{
		}

		[ClientVar]
		[ServerVar]
		public static void collect()
		{
			Rust.GC.Collect();
		}

		[ClientVar]
		[ServerVar]
		public static void unload()
		{
			Resources.UnloadUnusedAssets();
		}
	}
}