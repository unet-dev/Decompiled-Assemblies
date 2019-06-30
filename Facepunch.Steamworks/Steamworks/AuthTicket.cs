using System;

namespace Steamworks
{
	public class AuthTicket : IDisposable
	{
		public byte[] Data;

		public uint Handle;

		public AuthTicket()
		{
		}

		public void Cancel()
		{
			if (this.Handle != 0)
			{
				SteamUser.Internal.CancelAuthTicket(this.Handle);
			}
			this.Handle = 0;
			this.Data = null;
		}

		public void Dispose()
		{
			this.Cancel();
		}
	}
}