using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Facepunch.Steamworks
{
	public class Friends
	{
		internal Client client;

		private byte[] buffer = new byte[131072];

		private bool _listenForFriendsMessages;

		private List<SteamFriend> _allFriends;

		private List<Friends.PersonaCallback> PersonaCallbacks = new List<Friends.PersonaCallback>();

		public IEnumerable<SteamFriend> All
		{
			get
			{
				if (this._allFriends == null)
				{
					this._allFriends = new List<SteamFriend>();
					this.Refresh();
				}
				return this._allFriends;
			}
		}

		public IEnumerable<SteamFriend> AllBlocked
		{
			get
			{
				Friends friend = null;
				foreach (SteamFriend all in friend.All)
				{
					if (!all.IsBlocked)
					{
						continue;
					}
					yield return all;
				}
			}
		}

		public IEnumerable<SteamFriend> AllFriends
		{
			get
			{
				Friends friend = null;
				foreach (SteamFriend all in friend.All)
				{
					if (!all.IsFriend)
					{
						continue;
					}
					yield return all;
				}
			}
		}

		public bool ListenForFriendsMessages
		{
			get
			{
				return this._listenForFriendsMessages;
			}
			set
			{
				this._listenForFriendsMessages = value;
				this.client.native.friends.SetListenForFriendsMessages(value);
			}
		}

		internal Friends(Client c)
		{
			this.client = c;
			this.client.RegisterCallback<AvatarImageLoaded_t>(new Action<AvatarImageLoaded_t>(this.OnAvatarImageLoaded));
			this.client.RegisterCallback<PersonaStateChange_t>(new Action<PersonaStateChange_t>(this.OnPersonaStateChange));
			this.client.RegisterCallback<GameRichPresenceJoinRequested_t>(new Action<GameRichPresenceJoinRequested_t>(this.OnGameJoinRequested));
			this.client.RegisterCallback<GameConnectedFriendChatMsg_t>(new Action<GameConnectedFriendChatMsg_t>(this.OnFriendChatMessage));
		}

		internal void Cycle()
		{
			if (this.PersonaCallbacks.Count == 0)
			{
				return;
			}
			DateTime dateTime = DateTime.Now.AddSeconds(-10);
			for (int i = this.PersonaCallbacks.Count - 1; i >= 0; i--)
			{
				Friends.PersonaCallback item = this.PersonaCallbacks[i];
				if (item.Time < dateTime)
				{
					if (item.Callback != null)
					{
						Image cachedAvatar = this.GetCachedAvatar(item.Size, item.SteamId);
						item.Callback(cachedAvatar);
					}
					this.PersonaCallbacks.Remove(item);
				}
			}
		}

		public SteamFriend Get(ulong steamid)
		{
			SteamFriend steamFriend = (
				from x in this.All
				where x.Id == steamid
				select x).FirstOrDefault<SteamFriend>();
			if (steamFriend != null)
			{
				return steamFriend;
			}
			SteamFriend steamFriend1 = new SteamFriend()
			{
				Id = steamid,
				Client = this.client
			};
			steamFriend1.Refresh();
			return steamFriend1;
		}

		public void GetAvatar(Friends.AvatarSize size, ulong steamid, Action<Image> callback)
		{
			Image cachedAvatar = this.GetCachedAvatar(size, steamid);
			if (cachedAvatar != null)
			{
				callback(cachedAvatar);
				return;
			}
			if (!this.client.native.friends.RequestUserInformation(steamid, false))
			{
				cachedAvatar = this.GetCachedAvatar(size, steamid);
				if (cachedAvatar != null)
				{
					callback(cachedAvatar);
					return;
				}
			}
			this.PersonaCallbacks.Add(new Friends.PersonaCallback()
			{
				SteamId = steamid,
				Size = size,
				Callback = callback,
				Time = DateTime.Now
			});
		}

		public Image GetCachedAvatar(Friends.AvatarSize size, ulong steamid)
		{
			int smallFriendAvatar = 0;
			switch (size)
			{
				case Friends.AvatarSize.Small:
				{
					smallFriendAvatar = this.client.native.friends.GetSmallFriendAvatar(steamid);
					break;
				}
				case Friends.AvatarSize.Medium:
				{
					smallFriendAvatar = this.client.native.friends.GetMediumFriendAvatar(steamid);
					break;
				}
				case Friends.AvatarSize.Large:
				{
					smallFriendAvatar = this.client.native.friends.GetLargeFriendAvatar(steamid);
					break;
				}
			}
			if (smallFriendAvatar == 1)
			{
				return null;
			}
			if (smallFriendAvatar == 2)
			{
				return null;
			}
			if (smallFriendAvatar == 3)
			{
				return null;
			}
			Image image = new Image()
			{
				Id = smallFriendAvatar
			};
			if (!image.TryLoad(this.client.native.utils))
			{
				return null;
			}
			return image;
		}

		public string GetName(ulong steamid)
		{
			this.client.native.friends.RequestUserInformation(steamid, true);
			return this.client.native.friends.GetFriendPersonaName(steamid);
		}

		private void LoadAvatarForSteamId(ulong Steamid)
		{
			for (int i = this.PersonaCallbacks.Count - 1; i >= 0; i--)
			{
				Friends.PersonaCallback item = this.PersonaCallbacks[i];
				if (item.SteamId == Steamid)
				{
					Image cachedAvatar = this.GetCachedAvatar(item.Size, item.SteamId);
					if (cachedAvatar != null)
					{
						this.PersonaCallbacks.Remove(item);
						if (item.Callback != null)
						{
							item.Callback(cachedAvatar);
						}
					}
				}
			}
		}

		private void OnAvatarImageLoaded(AvatarImageLoaded_t data)
		{
			this.LoadAvatarForSteamId(data.SteamID);
		}

		private unsafe void OnFriendChatMessage(GameConnectedFriendChatMsg_t data)
		{
			// 
			// Current member / type: System.Void Facepunch.Steamworks.Friends::OnFriendChatMessage(SteamNative.GameConnectedFriendChatMsg_t)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void OnFriendChatMessage(SteamNative.GameConnectedFriendChatMsg_t)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private void OnGameJoinRequested(GameRichPresenceJoinRequested_t data)
		{
			if (this.OnInvitedToGame != null)
			{
				this.OnInvitedToGame(this.Get(data.SteamIDFriend), data.Connect);
			}
		}

		private void OnPersonaStateChange(PersonaStateChange_t data)
		{
			if ((data.ChangeFlags & 64) == 64)
			{
				this.LoadAvatarForSteamId(data.SteamID);
			}
			foreach (SteamFriend all in this.All)
			{
				if (all.Id != data.SteamID)
				{
					continue;
				}
				all.Refresh();
			}
		}

		public void Refresh()
		{
			if (this._allFriends == null)
			{
				this._allFriends = new List<SteamFriend>();
			}
			this._allFriends.Clear();
			int num = 65535;
			int friendCount = this.client.native.friends.GetFriendCount(num);
			for (int i = 0; i < friendCount; i++)
			{
				ulong friendByIndex = this.client.native.friends.GetFriendByIndex(i, num);
				this._allFriends.Add(this.Get(friendByIndex));
			}
		}

		public bool UpdateInformation(ulong steamid)
		{
			return !this.client.native.friends.RequestUserInformation(steamid, false);
		}

		public event Friends.ChatMessageDelegate OnChatMessage;

		public event Friends.JoinRequestedDelegate OnInvitedToGame;

		public enum AvatarSize
		{
			Small,
			Medium,
			Large
		}

		public delegate void ChatMessageDelegate(SteamFriend friend, string type, string message);

		public delegate void JoinRequestedDelegate(SteamFriend friend, string connect);

		private class PersonaCallback
		{
			public ulong SteamId;

			public Friends.AvatarSize Size;

			public Action<Image> Callback;

			public DateTime Time;

			public PersonaCallback()
			{
			}
		}
	}
}