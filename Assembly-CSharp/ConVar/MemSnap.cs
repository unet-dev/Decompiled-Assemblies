using System;
using System.IO;
using UnityEngine.Profiling.Memory.Experimental;

namespace ConVar
{
	[Factory("memsnap")]
	public class MemSnap : ConsoleSystem
	{
		public MemSnap()
		{
		}

		[ClientVar]
		[ServerVar]
		public static void full(ConsoleSystem.Arg arg)
		{
			string str = MemSnap.NeedProfileFolder();
			DateTime now = DateTime.Now;
			MemoryProfiler.TakeSnapshot(string.Concat(str, "/memdump-", now.ToString("MM-dd-yyyy-h-mm-ss"), ".snap"), null, CaptureFlags.ManagedObjects | CaptureFlags.NativeObjects | CaptureFlags.NativeAllocations | CaptureFlags.NativeAllocationSites | CaptureFlags.NativeStackTraces);
		}

		[ClientVar]
		[ServerVar]
		public static void managed(ConsoleSystem.Arg arg)
		{
			string str = MemSnap.NeedProfileFolder();
			DateTime now = DateTime.Now;
			MemoryProfiler.TakeSnapshot(string.Concat(str, "/memdump-", now.ToString("MM-dd-yyyy-h-mm-ss"), ".snap"), null, CaptureFlags.ManagedObjects);
		}

		[ClientVar]
		[ServerVar]
		public static void native(ConsoleSystem.Arg arg)
		{
			string str = MemSnap.NeedProfileFolder();
			DateTime now = DateTime.Now;
			MemoryProfiler.TakeSnapshot(string.Concat(str, "/memdump-", now.ToString("MM-dd-yyyy-h-mm-ss"), ".snap"), null, CaptureFlags.NativeObjects);
		}

		private static string NeedProfileFolder()
		{
			if (!Directory.Exists("profile"))
			{
				return Directory.CreateDirectory("profile").FullName;
			}
			return (new DirectoryInfo("profile")).FullName;
		}
	}
}