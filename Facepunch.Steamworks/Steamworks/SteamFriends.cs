using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Steamworks
{
	public static class SteamFriends
	{
		private static ISteamFriends _internal;

		private static Dictionary<string, string> richPresence;

		private static bool _listenForFriendsMessages;

		internal static ISteamFriends Internal
		{
			get
			{
				if (SteamFriends._internal == null)
				{
					SteamFriends._internal = new ISteamFriends();
					SteamFriends._internal.Init();
					SteamFriends.richPresence = new Dictionary<string, string>();
				}
				return SteamFriends._internal;
			}
		}

		public static bool ListenForFriendsMessages
		{
			get
			{
				return SteamFriends._listenForFriendsMessages;
			}
			set
			{
				SteamFriends._listenForFriendsMessages = value;
				SteamFriends.Internal.SetListenForFriendsMessages(value);
			}
		}

		internal static async Task CacheUserInformationAsync(SteamId steamid, bool nameonly)
		{
			if (SteamFriends.RequestUserInformation(steamid, nameonly))
			{
				await Task.Delay(100);
				while (SteamFriends.RequestUserInformation(steamid, nameonly))
				{
					await Task.Delay(50);
				}
				await Task.Delay(500);
			}
		}

		public static void ClearRichPresence()
		{
			SteamFriends.richPresence.Clear();
			SteamFriends.Internal.ClearRichPresence();
		}

		public static IEnumerable<Friend> GetBlocked()
		{
			for (int i = 0; i < SteamFriends.Internal.GetFriendCount(1); i++)
			{
				yield return new Friend(SteamFriends.Internal.GetFriendByIndex(i, 65535));
			}
		}

		public static IEnumerable<Friend> GetFriends()
		{
			for (int i = 0; i < SteamFriends.Internal.GetFriendCount(4); i++)
			{
				yield return new Friend(SteamFriends.Internal.GetFriendByIndex(i, 65535));
			}
		}

		public static async Task<Image?> GetLargeAvatarAsync(SteamId steamid)
		{
			int i;
			await SteamFriends.CacheUserInformationAsync(steamid, false);
			for (i = SteamFriends.Internal.GetLargeFriendAvatar(steamid); i == -1; i = SteamFriends.Internal.GetLargeFriendAvatar(steamid))
			{
				await Task.Delay(50);
			}
			return SteamUtils.GetImage(i);
		}

		public static async Task<Image?> GetMediumAvatarAsync(SteamId steamid)
		{
			await SteamFriends.CacheUserInformationAsync(steamid, false);
			return SteamUtils.GetImage(SteamFriends.Internal.GetMediumFriendAvatar(steamid));
		}

		public static IEnumerable<Friend> GetPlayedWith()
		{
			for (int i = 0; i < SteamFriends.Internal.GetCoplayFriendCount(); i++)
			{
				yield return new Friend(SteamFriends.Internal.GetCoplayFriend(i));
			}
		}

		public static string GetRichPresence(string key)
		{
			string str;
			string str1;
			if (!SteamFriends.richPresence.TryGetValue(key, out str))
			{
				str1 = null;
			}
			else
			{
				str1 = str;
			}
			return str1;
		}

		public static async Task<Image?> GetSmallAvatarAsync(SteamId steamid)
		{
			await SteamFriends.CacheUserInformationAsync(steamid, false);
			return SteamUtils.GetImage(SteamFriends.Internal.GetSmallFriendAvatar(steamid));
		}

		internal static void InstallEvents()
		{
			FriendStateChange_t.Install((FriendStateChange_t x) => {
				Action<Friend> onPersonaStateChange = SteamFriends.OnPersonaStateChange;
				if (onPersonaStateChange != null)
				{
					onPersonaStateChange(new Friend(x.SteamID));
				}
				else
				{
				}
			}, false);
			GameRichPresenceJoinRequested_t.Install((GameRichPresenceJoinRequested_t x) => {
				Action<Friend, string> onGameRichPresenceJoinRequested = SteamFriends.OnGameRichPresenceJoinRequested;
				if (onGameRichPresenceJoinRequested != null)
				{
					onGameRichPresenceJoinRequested(new Friend(x.SteamIDFriend), x.Connect);
				}
				else
				{
				}
			}, false);
			GameConnectedFriendChatMsg_t.Install(new Action<GameConnectedFriendChatMsg_t>(SteamFriends.OnFriendChatMessage), false);
			GameOverlayActivated_t.Install((GameOverlayActivated_t x) => {
				Action onGameOverlayActivated = SteamFriends.OnGameOverlayActivated;
				if (onGameOverlayActivated != null)
				{
					onGameOverlayActivated();
				}
				else
				{
				}
			}, false);
			GameServerChangeRequested_t.Install((GameServerChangeRequested_t x) => {
				Action<string, string> onGameServerChangeRequested = SteamFriends.OnGameServerChangeRequested;
				if (onGameServerChangeRequested != null)
				{
					onGameServerChangeRequested(x.Server, x.Password);
				}
				else
				{
				}
			}, false);
			GameLobbyJoinRequested_t.Install((GameLobbyJoinRequested_t x) => {
				Action<Lobby, SteamId> onGameLobbyJoinRequested = SteamFriends.OnGameLobbyJoinRequested;
				if (onGameLobbyJoinRequested != null)
				{
					onGameLobbyJoinRequested(new Lobby(x.SteamIDLobby), x.SteamIDFriend);
				}
				else
				{
				}
			}, false);
			FriendRichPresenceUpdate_t.Install((FriendRichPresenceUpdate_t x) => {
				Action<Friend> onFriendRichPresenceUpdate = SteamFriends.OnFriendRichPresenceUpdate;
				if (onFriendRichPresenceUpdate != null)
				{
					onFriendRichPresenceUpdate(new Friend(x.SteamIDFriend));
				}
				else
				{
				}
			}, false);
		}

		private static unsafe void OnFriendChatMessage(GameConnectedFriendChatMsg_t data)
		{
			// 
			// Current member / type: System.Void Steamworks.SteamFriends::OnFriendChatMessage(Steamworks.Data.GameConnectedFriendChatMsg_t)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void OnFriendChatMessage(Steamworks.Data.GameConnectedFriendChatMsg_t)
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

		public static void OpenGameInviteOverlay(SteamId lobby)
		{
			SteamFriends.Internal.ActivateGameOverlayInviteDialog(lobby);
		}

		public static void OpenOverlay(string type)
		{
			SteamFriends.Internal.ActivateGameOverlay(type);
		}

		public static void OpenStoreOverlay(AppId id)
		{
			SteamFriends.Internal.ActivateGameOverlayToStore(id.Value, OverlayToStoreFlag.None);
		}

		public static void OpenUserOverlay(SteamId id, string type)
		{
			SteamFriends.Internal.ActivateGameOverlayToUser(type, id);
		}

		public static void OpenWebOverlay(string url, bool modal = false)
		{
			SteamFriends.Internal.ActivateGameOverlayToWebPage(url, (modal ? ActivateGameOverlayToWebPageMode.Modal : ActivateGameOverlayToWebPageMode.Default));
		}

		public static bool RequestUserInformation(SteamId steamid, bool nameonly = true)
		{
			return SteamFriends.Internal.RequestUserInformation(steamid, nameonly);
		}

		public static void SetPlayedWith(SteamId steamid)
		{
			SteamFriends.Internal.SetPlayedWith(steamid);
		}

		public static bool SetRichPresence(string key, string value)
		{
			SteamFriends.richPresence[key] = value;
			return SteamFriends.Internal.SetRichPresence(key, value);
		}

		internal static void Shutdown()
		{
			SteamFriends._internal = null;
		}

		public static event Action<Friend, string, string> OnChatMessage;

		public static event Action<Friend> OnFriendRichPresenceUpdate;

		public static event Action<Lobby, SteamId> OnGameLobbyJoinRequested;

		public static event Action OnGameOverlayActivated;

		public static event Action<Friend, string> OnGameRichPresenceJoinRequested;

		public static event Action<string, string> OnGameServerChangeRequested;

		public static event Action<Friend> OnPersonaStateChange;
	}
}