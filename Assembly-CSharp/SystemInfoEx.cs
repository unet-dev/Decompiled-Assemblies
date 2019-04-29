using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class SystemInfoEx
{
	private static bool[] supportedRenderTextureFormats;

	public static int systemMemoryUsed
	{
		get
		{
			return (int)(SystemInfoEx.System_GetMemoryUsage() / (long)1024 / (long)1024);
		}
	}

	static SystemInfoEx()
	{
	}

	public static bool SupportsRenderTextureFormat(RenderTextureFormat format)
	{
		if (SystemInfoEx.supportedRenderTextureFormats == null)
		{
			Array values = Enum.GetValues(typeof(RenderTextureFormat));
			int value = (int)values.GetValue(values.Length - 1);
			SystemInfoEx.supportedRenderTextureFormats = new bool[value + 1];
			for (int i = 0; i <= value; i++)
			{
				bool flag = Enum.IsDefined(typeof(RenderTextureFormat), i);
				SystemInfoEx.supportedRenderTextureFormats[i] = (flag ? SystemInfo.SupportsRenderTextureFormat((RenderTextureFormat)i) : false);
			}
		}
		return SystemInfoEx.supportedRenderTextureFormats[(int)format];
	}

	[DllImport("RustNative", CharSet=CharSet.None, ExactSpelling=false)]
	private static extern ulong System_GetMemoryUsage();
}