using System;
using System.IO;

namespace Steamworks
{
	public static class Config
	{
		private static OsType _os;

		public static OsType Os
		{
			get
			{
				if (Config._os == OsType.None)
				{
					string environmentVariable = Environment.GetEnvironmentVariable("windir");
					if ((String.IsNullOrEmpty(environmentVariable) || !environmentVariable.Contains("\\") ? false : Directory.Exists(environmentVariable)))
					{
						Config._os = OsType.Windows;
					}
					else if (!File.Exists("/System/Library/CoreServices/SystemVersion.plist"))
					{
						if (!File.Exists("/proc/sys/kernel/ostype"))
						{
							throw new Exception("Couldn't determine operating system");
						}
						Config._os = OsType.Posix;
					}
					else
					{
						Config._os = OsType.Posix;
					}
				}
				return Config._os;
			}
			set
			{
				Config._os = value;
			}
		}

		public static bool PackSmall
		{
			get
			{
				return Config.Os != OsType.Windows;
			}
		}
	}
}