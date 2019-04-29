using Facepunch;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SelfCheck
{
	private static bool Failed(string Message)
	{
		if (SingletonComponent<Bootstrap>.Instance)
		{
			SingletonComponent<Bootstrap>.Instance.messageString = "";
			SingletonComponent<Bootstrap>.Instance.ThrowError(Message);
		}
		Debug.LogError(string.Concat("SelfCheck Failed: ", Message));
		return false;
	}

	public static bool Run()
	{
		if (FileSystem.Backend.isError)
		{
			return SelfCheck.Failed(string.Concat("Asset Bundle Error: ", FileSystem.Backend.loadingError));
		}
		if (FileSystem.Load<GameManifest>("Assets/manifest.asset", true) == null)
		{
			return SelfCheck.Failed("Couldn't load game manifest - verify your game content!");
		}
		if (!SelfCheck.TestRustNative())
		{
			return false;
		}
		if (CommandLine.HasSwitch("-force-feature-level-9-3"))
		{
			return SelfCheck.Failed("Invalid command line argument: -force-feature-level-9-3");
		}
		if (CommandLine.HasSwitch("-force-feature-level-10-0"))
		{
			return SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-0");
		}
		if (!CommandLine.HasSwitch("-force-feature-level-10-1"))
		{
			return true;
		}
		return SelfCheck.Failed("Invalid command line argument: -force-feature-level-10-1");
	}

	[DllImport("RustNative", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern bool RustNative_VersionCheck(int version);

	private static bool TestRustNative()
	{
		bool flag;
		try
		{
			if (SelfCheck.RustNative_VersionCheck(5))
			{
				return true;
			}
			else
			{
				flag = SelfCheck.Failed("RustNative is wrong version!");
			}
		}
		catch (DllNotFoundException dllNotFoundException1)
		{
			DllNotFoundException dllNotFoundException = dllNotFoundException1;
			flag = SelfCheck.Failed(string.Concat("RustNative library couldn't load! ", dllNotFoundException.Message));
		}
		return flag;
	}
}