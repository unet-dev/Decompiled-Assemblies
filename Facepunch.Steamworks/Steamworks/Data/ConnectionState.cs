using System;

namespace Steamworks.Data
{
	public enum ConnectionState
	{
		Dead = -3,
		Linger = -2,
		FinWait = -1,
		None = 0,
		Connecting = 1,
		FindingRoute = 2,
		Connected = 3,
		ClosedByPeer = 4,
		ProblemDetectedLocally = 5,
		Force32Bit = 2147483647
	}
}