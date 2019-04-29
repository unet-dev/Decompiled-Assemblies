using System;
using System.Runtime.InteropServices;

namespace SteamNative
{
	internal struct SteamItemDetails_t
	{
		internal ulong ItemId;

		internal int Definition;

		internal ushort Quantity;

		internal ushort Flags;

		internal static SteamItemDetails_t FromPointer(IntPtr p)
		{
			if (!Platform.PackSmall)
			{
				return (SteamItemDetails_t)Marshal.PtrToStructure(p, typeof(SteamItemDetails_t));
			}
			return (SteamItemDetails_t.PackSmall)Marshal.PtrToStructure(p, typeof(SteamItemDetails_t.PackSmall));
		}

		internal static int StructSize()
		{
			if (Platform.PackSmall)
			{
				return Marshal.SizeOf(typeof(SteamItemDetails_t.PackSmall));
			}
			return Marshal.SizeOf(typeof(SteamItemDetails_t));
		}

		internal struct PackSmall
		{
			internal ulong ItemId;

			internal int Definition;

			internal ushort Quantity;

			internal ushort Flags;

			public static implicit operator SteamItemDetails_t(SteamItemDetails_t.PackSmall d)
			{
				SteamItemDetails_t steamItemDetailsT = new SteamItemDetails_t()
				{
					ItemId = d.ItemId,
					Definition = d.Definition,
					Quantity = d.Quantity,
					Flags = d.Flags
				};
				return steamItemDetailsT;
			}
		}
	}
}