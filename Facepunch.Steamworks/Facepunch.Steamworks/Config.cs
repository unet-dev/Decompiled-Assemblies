using SteamNative;
using System;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public static class Config
	{
		public static bool UseThisCall
		{
			get;
			set;
		}

		static Config()
		{
			Config.UseThisCall = true;
		}

		public static void ForcePlatform(Facepunch.Steamworks.OperatingSystem os, Architecture arch)
		{
			Platform.Os = os;
			Platform.Arch = arch;
		}

		public static void ForUnity(string platform)
		{
			if (platform == "WindowsEditor" || platform == "WindowsPlayer")
			{
				if (IntPtr.Size == 4)
				{
					Config.UseThisCall = false;
				}
				Config.ForcePlatform(Facepunch.Steamworks.OperatingSystem.Windows, (IntPtr.Size == 4 ? Architecture.x86 : Architecture.x64));
			}
			if (platform == "OSXEditor" || platform == "OSXPlayer" || platform == "OSXDashboardPlayer")
			{
				Config.ForcePlatform(Facepunch.Steamworks.OperatingSystem.macOS, (IntPtr.Size == 4 ? Architecture.x86 : Architecture.x64));
			}
			if (platform == "LinuxPlayer" || platform == "LinuxEditor")
			{
				Config.ForcePlatform(Facepunch.Steamworks.OperatingSystem.Linux, (IntPtr.Size == 4 ? Architecture.x86 : Architecture.x64));
			}
			Console.WriteLine(string.Concat("Facepunch.Steamworks Unity: ", platform));
			Console.WriteLine(string.Concat("Facepunch.Steamworks Os: ", Platform.Os));
			Console.WriteLine(string.Concat("Facepunch.Steamworks Arch: ", Platform.Arch));
		}
	}
}