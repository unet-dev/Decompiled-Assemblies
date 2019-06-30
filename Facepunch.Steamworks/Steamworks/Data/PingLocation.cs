using Steamworks;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Explicit)]
	public struct PingLocation
	{
		public int EstimatePingTo(PingLocation target)
		{
			return SteamNetworkingUtils.Internal.EstimatePingTimeBetweenTwoLocations(this, ref target);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			SteamNetworkingUtils.Internal.ConvertPingLocationToString(this, stringBuilder, stringBuilder.Capacity);
			return stringBuilder.ToString();
		}

		public static PingLocation? TryParseFromString(string str)
		{
			PingLocation? nullable;
			PingLocation pingLocation = new PingLocation();
			if (SteamNetworkingUtils.Internal.ParsePingLocationString(str, ref pingLocation))
			{
				nullable = new PingLocation?(pingLocation);
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}
	}
}