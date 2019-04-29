using System;
using System.Collections.Generic;
using System.IO;

namespace Oxide.Core
{
	public static class Cleanup
	{
		internal static HashSet<string> files;

		static Cleanup()
		{
			Cleanup.files = new HashSet<string>();
		}

		public static void Add(string file)
		{
			Cleanup.files.Add(file);
		}

		internal static void Run()
		{
			if (Cleanup.files == null)
			{
				return;
			}
			foreach (string file in Cleanup.files)
			{
				try
				{
					if (File.Exists(file))
					{
						Interface.Oxide.LogDebug("Cleanup file: {0}", new object[] { file });
						File.Delete(file);
					}
				}
				catch (Exception exception)
				{
					Interface.Oxide.LogWarning("Failed to cleanup file: {0}", new object[] { file });
				}
			}
			Cleanup.files = null;
		}
	}
}