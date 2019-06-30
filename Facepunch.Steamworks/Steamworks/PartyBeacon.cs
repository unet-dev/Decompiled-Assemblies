using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks
{
	public struct PartyBeacon
	{
		internal PartyBeaconID_t Id;

		private static ISteamParties Internal
		{
			get
			{
				return SteamParties.Internal;
			}
		}

		public string MetaData
		{
			get
			{
				SteamId steamId = new SteamId();
				SteamPartyBeaconLocation_t steamPartyBeaconLocationT = new SteamPartyBeaconLocation_t();
				StringBuilder stringBuilder = Helpers.TakeStringBuilder();
				PartyBeacon.Internal.GetBeaconDetails(this.Id, ref steamId, ref steamPartyBeaconLocationT, stringBuilder, stringBuilder.Capacity);
				return stringBuilder.ToString();
			}
		}

		public SteamId Owner
		{
			get
			{
				SteamId steamId = new SteamId();
				SteamPartyBeaconLocation_t steamPartyBeaconLocationT = new SteamPartyBeaconLocation_t();
				StringBuilder stringBuilder = Helpers.TakeStringBuilder();
				PartyBeacon.Internal.GetBeaconDetails(this.Id, ref steamId, ref steamPartyBeaconLocationT, stringBuilder, stringBuilder.Capacity);
				return steamId;
			}
		}

		public void CancelReservation(SteamId steamid)
		{
			PartyBeacon.Internal.CancelReservation(this.Id, steamid);
		}

		public bool Destroy()
		{
			return PartyBeacon.Internal.DestroyBeacon(this.Id);
		}

		public async Task<string> JoinAsync()
		{
			string connectString;
			bool flag;
			JoinPartyCallback_t? nullable = await PartyBeacon.Internal.JoinParty(this.Id);
			JoinPartyCallback_t? nullable1 = nullable;
			nullable = null;
			flag = (!nullable1.HasValue ? true : nullable1.Value.Result != Result.OK);
			if (!flag)
			{
				connectString = nullable1.Value.ConnectString;
			}
			else
			{
				connectString = null;
			}
			return connectString;
		}

		public void OnReservationCompleted(SteamId steamid)
		{
			PartyBeacon.Internal.OnReservationCompleted(this.Id, steamid);
		}
	}
}