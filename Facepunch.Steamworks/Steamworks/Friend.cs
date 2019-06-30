using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks
{
	public struct Friend
	{
		public SteamId Id;

		public Friend.FriendGameInfo? GameInfo
		{
			get
			{
				Friend.FriendGameInfo? nullable;
				FriendGameInfo_t friendGameInfoT = new FriendGameInfo_t();
				if (SteamFriends.Internal.GetFriendGamePlayed(this.Id, ref friendGameInfoT))
				{
					nullable = new Friend.FriendGameInfo?(Friend.FriendGameInfo.From(friendGameInfoT));
				}
				else
				{
					nullable = null;
				}
				return nullable;
			}
		}

		public bool IsAway
		{
			get
			{
				return this.State == FriendState.Away;
			}
		}

		public bool IsBlocked
		{
			get
			{
				return this.Relationship == Steamworks.Relationship.Blocked;
			}
		}

		public bool IsBusy
		{
			get
			{
				return this.State == FriendState.Busy;
			}
		}

		public bool IsFriend
		{
			get
			{
				return this.Relationship == Steamworks.Relationship.Friend;
			}
		}

		public bool IsMe
		{
			get
			{
				return this.Id == SteamClient.SteamId;
			}
		}

		public bool IsOnline
		{
			get
			{
				return this.State != FriendState.Offline;
			}
		}

		public bool IsPlayingThisGame
		{
			get
			{
				ulong? nullable;
				Friend.FriendGameInfo? gameInfo = this.GameInfo;
				if (gameInfo.HasValue)
				{
					nullable = new ulong?(gameInfo.GetValueOrDefault().GameID);
				}
				else
				{
					nullable = null;
				}
				ulong? nullable1 = nullable;
				ulong appId = (ulong)SteamClient.AppId;
				return nullable1.GetValueOrDefault() == appId & nullable1.HasValue;
			}
		}

		public bool IsSnoozing
		{
			get
			{
				return this.State == FriendState.Snooze;
			}
		}

		public string Name
		{
			get
			{
				return SteamFriends.Internal.GetFriendPersonaName(this.Id);
			}
		}

		public IEnumerable<string> NameHistory
		{
			get
			{
				int num = 0;
				while (num < 32)
				{
					string friendPersonaNameHistory = SteamFriends.Internal.GetFriendPersonaNameHistory(this.Id, num);
					if (!String.IsNullOrEmpty(friendPersonaNameHistory))
					{
						yield return friendPersonaNameHistory;
						friendPersonaNameHistory = null;
						num++;
					}
					else
					{
						break;
					}
				}
			}
		}

		public Steamworks.Relationship Relationship
		{
			get
			{
				return SteamFriends.Internal.GetFriendRelationship(this.Id);
			}
		}

		public FriendState State
		{
			get
			{
				return SteamFriends.Internal.GetFriendPersonaState(this.Id);
			}
		}

		public int SteamLevel
		{
			get
			{
				return SteamFriends.Internal.GetFriendSteamLevel(this.Id);
			}
		}

		public Friend(SteamId steamid)
		{
			this.Id = steamid;
		}

		public async Task<Image?> GetLargeAvatarAsync()
		{
			return await SteamFriends.GetLargeAvatarAsync(this.Id);
		}

		public async Task<Image?> GetMediumAvatarAsync()
		{
			return await SteamFriends.GetMediumAvatarAsync(this.Id);
		}

		public string GetRichPresence(string key)
		{
			string str;
			string friendRichPresence = SteamFriends.Internal.GetFriendRichPresence(this.Id, key);
			if (!String.IsNullOrEmpty(friendRichPresence))
			{
				str = friendRichPresence;
			}
			else
			{
				str = null;
			}
			return str;
		}

		public async Task<Image?> GetSmallAvatarAsync()
		{
			return await SteamFriends.GetSmallAvatarAsync(this.Id);
		}

		public bool InviteToGame(string Text)
		{
			return SteamFriends.Internal.InviteUserToGame(this.Id, Text);
		}

		public bool IsIn(SteamId group_or_room)
		{
			return SteamFriends.Internal.IsUserInSource(this.Id, group_or_room);
		}

		public async Task RequestInfoAsync(int timeout = 5000)
		{
			await SteamFriends.CacheUserInformationAsync(this.Id, true);
		}

		public bool SendMessage(string message)
		{
			return SteamFriends.Internal.ReplyToFriendMessage(this.Id, message);
		}

		public override string ToString()
		{
			string str = String.Concat(this.Name, " (", this.Id.ToString(), ")");
			return str;
		}

		public struct FriendGameInfo
		{
			internal ulong GameID;

			internal uint GameIP;

			internal ulong SteamIDLobby;

			public int ConnectionPort;

			public int QueryPort;

			public IPAddress IpAddress
			{
				get
				{
					return Utility.Int32ToIp(this.GameIP);
				}
			}

			public uint IpAddressRaw
			{
				get
				{
					return this.GameIP;
				}
			}

			public Lobby? Lobby
			{
				get
				{
					Lobby? nullable;
					if (this.SteamIDLobby != (long)0)
					{
						nullable = new Lobby?(new Lobby(this.SteamIDLobby));
					}
					else
					{
						nullable = null;
					}
					return nullable;
				}
			}

			internal static Friend.FriendGameInfo From(FriendGameInfo_t i)
			{
				Friend.FriendGameInfo friendGameInfo = new Friend.FriendGameInfo()
				{
					GameID = i.GameID,
					GameIP = i.GameIP,
					ConnectionPort = i.GamePort,
					QueryPort = i.QueryPort,
					SteamIDLobby = i.SteamIDLobby
				};
				return friendGameInfo;
			}
		}
	}
}