using System;
using System.Diagnostics;
using System.IO;

namespace Facepunch.Utility
{
	public static class Os
	{
		public static void OpenFolder(string folder)
		{
			folder = folder.Replace("/", "\\");
			if (!Directory.Exists(folder))
			{
				Process.Start("explorer.exe", string.Concat("/select,", folder));
				return;
			}
			Process.Start("explorer.exe", folder ?? "");
		}
	}
}