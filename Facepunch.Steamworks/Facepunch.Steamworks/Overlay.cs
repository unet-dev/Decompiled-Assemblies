using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public class Overlay
	{
		internal Client client;

		public bool Enabled
		{
			get
			{
				return this.client.native.utils.IsOverlayEnabled();
			}
		}

		public bool IsOpen
		{
			get;
			private set;
		}

		internal Overlay(Client c)
		{
			this.client = c;
			c.RegisterCallback<GameOverlayActivated_t>(new Action<GameOverlayActivated_t>(this.OverlayStateChange));
		}

		public void AcceptFriendRequest(ulong steamid)
		{
			this.OpenUserPage("friendrequestaccept", steamid);
		}

		public void AddFriend(ulong steamid)
		{
			this.OpenUserPage("friendadd", steamid);
		}

		public void IgnoreFriendRequest(ulong steamid)
		{
			this.OpenUserPage("friendrequestignore", steamid);
		}

		public void OpenAchievements(ulong steamid)
		{
			this.OpenUserPage("achievements", steamid);
		}

		public void OpenChat(ulong steamid)
		{
			this.OpenUserPage("chat", steamid);
		}

		public void OpenProfile(ulong steamid)
		{
			this.OpenUserPage("steamid", steamid);
		}

		public void OpenStats(ulong steamid)
		{
			this.OpenUserPage("stats", steamid);
		}

		public void OpenTrade(ulong steamid)
		{
			this.OpenUserPage("jointrade", steamid);
		}

		public void OpenUrl(string url)
		{
			this.client.native.friends.ActivateGameOverlayToWebPage(url);
		}

		public void OpenUserPage(string name, ulong steamid)
		{
			this.client.native.friends.ActivateGameOverlayToUser(name, steamid);
		}

		private void OverlayStateChange(GameOverlayActivated_t activation)
		{
			this.IsOpen = activation.Active == 1;
		}

		public void RemoveFriend(ulong steamid)
		{
			this.OpenUserPage("friendremove", steamid);
		}
	}
}