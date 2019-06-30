using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct MatchMakingKeyValuePair_t
	{
		internal string Key;

		internal string Value;

		internal static MatchMakingKeyValuePair_t Fill(IntPtr p)
		{
			return (MatchMakingKeyValuePair_t)Marshal.PtrToStructure(p, typeof(MatchMakingKeyValuePair_t));
		}
	}
}