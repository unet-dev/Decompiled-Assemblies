using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;

namespace Facepunch.Steamworks
{
	public class User : IDisposable
	{
		internal Client client;

		internal Dictionary<string, string> richPresence = new Dictionary<string, string>();

		internal User(Client c)
		{
			this.client = c;
		}

		public void ClearRichPresence()
		{
			this.richPresence.Clear();
			this.client.native.friends.ClearRichPresence();
		}

		public void Dispose()
		{
			this.client = null;
		}

		public string GetRichPresence(string key)
		{
			string str;
			if (this.richPresence.TryGetValue(key, out str))
			{
				return str;
			}
			return null;
		}

		public bool SetRichPresence(string key, string value)
		{
			this.richPresence[key] = value;
			return this.client.native.friends.SetRichPresence(key, value);
		}
	}
}