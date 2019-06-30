using Rust;
using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace ConVar
{
	[Factory("gc")]
	public class GC : ConsoleSystem
	{
		[ClientVar]
		public static bool buffer_enabled;

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

		[ClientVar]
		[ServerVar]
		public static bool enabled
		{
			get
			{
				return Rust.GC.Enabled;
			}
			set
			{
				Debug.LogWarning("Cannot set gc.enabled as it is read only");
			}
		}

		[ClientVar]
		[ServerVar]
		public static bool incremental_enabled
		{
			get
			{
				return GarbageCollector.isIncremental;
			}
			set
			{
				Debug.LogWarning("Cannot set gc.incremental as it is read only");
			}
		}

		[ClientVar]
		[ServerVar]
		public static int incremental_milliseconds
		{
			get
			{
				return (int)(GarbageCollector.incrementalTimeSliceNanoseconds / (long)1000000);
			}
			set
			{
				GarbageCollector.incrementalTimeSliceNanoseconds = (ulong)((long)1000000 * (long)Mathf.Max(value, 0));
			}
		}

		static GC()
		{
			ConVar.GC.buffer_enabled = true;
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