using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Steamworks
{
	public static class SteamParties
	{
		private static ISteamParties _internal;

		public static int ActiveBeaconCount
		{
			get
			{
				return (int)SteamParties.Internal.GetNumActiveBeacons();
			}
		}

		public static IEnumerable<PartyBeacon> ActiveBeacons
		{
			get
			{
				for (uint i = 0; (ulong)i < (long)SteamParties.ActiveBeaconCount; i++)
				{
					PartyBeacon partyBeacon = new PartyBeacon()
					{
						Id = SteamParties.Internal.GetBeaconByIndex(i)
					};
					yield return partyBeacon;
				}
			}
		}

		internal static ISteamParties Internal
		{
			get
			{
				if (SteamParties._internal == null)
				{
					SteamParties._internal = new ISteamParties();
					SteamParties._internal.Init();
				}
				return SteamParties._internal;
			}
		}

		internal static void InstallEvents()
		{
			AvailableBeaconLocationsUpdated_t.Install((AvailableBeaconLocationsUpdated_t x) => {
				Action onBeaconLocationsUpdated = SteamParties.OnBeaconLocationsUpdated;
				if (onBeaconLocationsUpdated != null)
				{
					onBeaconLocationsUpdated();
				}
				else
				{
				}
			}, false);
			ActiveBeaconsUpdated_t.Install((ActiveBeaconsUpdated_t x) => {
				Action onActiveBeaconsUpdated = SteamParties.OnActiveBeaconsUpdated;
				if (onActiveBeaconsUpdated != null)
				{
					onActiveBeaconsUpdated();
				}
				else
				{
				}
			}, false);
		}

		internal static void Shutdown()
		{
			SteamParties._internal = null;
		}

		public static event Action OnActiveBeaconsUpdated;

		public static event Action OnBeaconLocationsUpdated;
	}
}