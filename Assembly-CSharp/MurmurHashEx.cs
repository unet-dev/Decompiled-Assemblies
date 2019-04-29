using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

public static class MurmurHashEx
{
	public static int MurmurHashSigned(this string str)
	{
		return MurmurHash.Signed(MurmurHashEx.StringToStream(str));
	}

	public static uint MurmurHashUnsigned(this string str)
	{
		return MurmurHash.Unsigned(MurmurHashEx.StringToStream(str));
	}

	private static MemoryStream StringToStream(string str)
	{
		return new MemoryStream(Encoding.UTF8.GetBytes(str ?? string.Empty));
	}
}