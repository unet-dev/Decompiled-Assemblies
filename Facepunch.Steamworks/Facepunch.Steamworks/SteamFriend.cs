using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public class SteamFriend
	{
		public string Name;

		private PersonaState personaState;

		private FriendRelationship relationship;

		internal Facepunch.Steamworks.Client Client
		{
			get;
			set;
		}

		public ulong CurrentAppId
		{
			get;
			internal set;
		}

		public ulong Id
		{
			get;
			internal set;
		}

		public bool IsAway
		{
			get
			{
				return this.personaState == PersonaState.Away;
			}
		}

		public bool IsBlocked
		{
			get
			{
				return this.relationship == FriendRelationship.Blocked;
			}
		}

		public bool IsBusy
		{
			get
			{
				return this.personaState == PersonaState.Busy;
			}
		}

		public bool IsFriend
		{
			get
			{
				return this.relationship == FriendRelationship.Friend;
			}
		}

		public bool IsOnline
		{
			get
			{
				return this.personaState != PersonaState.Offline;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return this.CurrentAppId != (long)0;
			}
		}

		public bool IsPlayingThisGame
		{
			get
			{
				return this.CurrentAppId == (ulong)this.Client.AppId;
			}
		}

		public bool IsSnoozing
		{
			get
			{
				return this.personaState == PersonaState.Snooze;
			}
		}

		public int ServerGamePort
		{
			get;
			internal set;
		}

		public uint ServerIp
		{
			get;
			internal set;
		}

		public ulong ServerLobbyId
		{
			get;
			internal set;
		}

		public int ServerQueryPort
		{
			get;
			internal set;
		}

		public SteamFriend()
		{
		}

		public Image GetAvatar(Friends.AvatarSize size)
		{
			return this.Client.Friends.GetCachedAvatar(size, this.Id);
		}

		public string GetRichPresence(string key)
		{
			string friendRichPresence = this.Client.native.friends.GetFriendRichPresence(this.Id, key);
			if (string.IsNullOrEmpty(friendRichPresence))
			{
				return null;
			}
			return friendRichPresence;
		}

		public bool InviteToGame(string Text)
		{
			return this.Client.native.friends.InviteUserToGame(this.Id, Text);
		}

		public void Refresh()
		{
			this.Name = this.Client.native.friends.GetFriendPersonaName(this.Id);
			this.relationship = this.Client.native.friends.GetFriendRelationship(this.Id);
			this.personaState = this.Client.native.friends.GetFriendPersonaState(this.Id);
			this.CurrentAppId = (ulong)0;
			this.ServerIp = 0;
			this.ServerGamePort = 0;
			this.ServerQueryPort = 0;
			this.ServerLobbyId = (ulong)0;
			FriendGameInfo_t friendGameInfoT = new FriendGameInfo_t();
			if (this.Client.native.friends.GetFriendGamePlayed(this.Id, ref friendGameInfoT) && friendGameInfoT.GameID > (long)0)
			{
				this.CurrentAppId = friendGameInfoT.GameID;
				this.ServerIp = friendGameInfoT.GameIP;
				this.ServerGamePort = friendGameInfoT.GamePort;
				this.ServerQueryPort = friendGameInfoT.QueryPort;
				this.ServerLobbyId = friendGameInfoT.SteamIDLobby;
			}
			this.Client.native.friends.RequestFriendRichPresence(this.Id);
		}

		public bool SendMessage(string message)
		{
			return this.Client.native.friends.ReplyToFriendMessage(this.Id, message);
		}
	}
}