using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct MatchMakingKeyValuePair_t
	{
		internal string Key;

		internal string Value;

		internal static MatchMakingKeyValuePair_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (MatchMakingKeyValuePair_t)Marshal.PtrToStructure(p, typeof(MatchMakingKeyValuePair_t));
			}
			return (MatchMakingKeyValuePair_t.PackSmall)Marshal.PtrToStructure(p, typeof(MatchMakingKeyValuePair_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(MatchMakingKeyValuePair_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(MatchMakingKeyValuePair_t));
		}

		internal struct PackSmall
		{
			internal string Key;

			internal string Value;

			public static implicit operator MatchMakingKeyValuePair_t(MatchMakingKeyValuePair_t.PackSmall d)
			{
				MatchMakingKeyValuePair_t matchMakingKeyValuePairT = new MatchMakingKeyValuePair_t()
				{
					Key = d.Key,
					Value = d.Value
				};
				return matchMakingKeyValuePairT;
			}
		}
	}
}