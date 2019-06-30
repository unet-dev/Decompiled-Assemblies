using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct SteamPartyBeaconLocation_t
	{
		internal SteamPartyBeaconLocationType Type;

		internal ulong LocationID;

		internal static SteamPartyBeaconLocation_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamPartyBeaconLocation_t)Marshal.PtrToStructure(p, typeof(SteamPartyBeaconLocation_t)) : (SteamPartyBeaconLocation_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamPartyBeaconLocation_t.Pack8)));
		}

		public struct Pack8
		{
			internal SteamPartyBeaconLocationType Type;

			internal ulong LocationID;

			public static implicit operator SteamPartyBeaconLocation_t(SteamPartyBeaconLocation_t.Pack8 d)
			{
				SteamPartyBeaconLocation_t steamPartyBeaconLocationT = new SteamPartyBeaconLocation_t()
				{
					Type = d.Type,
					LocationID = d.LocationID
				};
				return steamPartyBeaconLocationT;
			}

			public static implicit operator Pack8(SteamPartyBeaconLocation_t d)
			{
				SteamPartyBeaconLocation_t.Pack8 pack8 = new SteamPartyBeaconLocation_t.Pack8()
				{
					Type = d.Type,
					LocationID = d.LocationID
				};
				return pack8;
			}
		}
	}
}