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
	public static class SteamMatchmaking
	{
		private static ISteamMatchmaking _internal;

		internal static ISteamMatchmaking Internal
		{
			get
			{
				if (SteamMatchmaking._internal == null)
				{
					SteamMatchmaking._internal = new ISteamMatchmaking();
					SteamMatchmaking._internal.Init();
				}
				return SteamMatchmaking._internal;
			}
		}

		public static LobbyQuery LobbyList
		{
			get
			{
				return new LobbyQuery();
			}
		}

		public static async Task<Lobby?> CreateLobbyAsync(int maxMembers = 100)
		{
			Lobby? nullable;
			bool flag;
			LobbyCreated_t? nullable1 = await SteamMatchmaking.Internal.CreateLobby(LobbyType.Invisible, maxMembers);
			LobbyCreated_t? nullable2 = nullable1;
			nullable1 = null;
			flag = (!nullable2.HasValue ? true : nullable2.Value.Result != Result.OK);
			if (!flag)
			{
				nullable = new Lobby?(new Lobby()
				{
					Id = nullable2.Value.SteamIDLobby
				});
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static IEnumerable<ServerInfo> GetFavoriteServers()
		{
			int favoriteGameCount = SteamMatchmaking.Internal.GetFavoriteGameCount();
			for (int i = 0; i < favoriteGameCount; i++)
			{
				uint num = 0;
				uint num1 = 0;
				ushort num2 = 0;
				ushort num3 = 0;
				uint num4 = 0;
				AppId appId = new AppId();
				if (SteamMatchmaking.Internal.GetFavoriteGame(i, ref appId, ref num4, ref num3, ref num2, ref num1, ref num))
				{
					if ((num1 & 1) == 0)
					{
						goto Label0;
					}
					yield return new ServerInfo(num4, num3, num2, num);
				}
			Label0:
			}
			yield break;
			goto Label0;
		}

		public static IEnumerable<ServerInfo> GetHistoryServers()
		{
			int favoriteGameCount = SteamMatchmaking.Internal.GetFavoriteGameCount();
			for (int i = 0; i < favoriteGameCount; i++)
			{
				uint num = 0;
				uint num1 = 0;
				ushort num2 = 0;
				ushort num3 = 0;
				uint num4 = 0;
				AppId appId = new AppId();
				if (SteamMatchmaking.Internal.GetFavoriteGame(i, ref appId, ref num4, ref num3, ref num2, ref num1, ref num))
				{
					if ((num1 & 2) == 0)
					{
						goto Label0;
					}
					yield return new ServerInfo(num4, num3, num2, num);
				}
			Label0:
			}
			yield break;
			goto Label0;
		}

		internal static void InstallEvents()
		{
			LobbyInvite_t.Install((LobbyInvite_t x) => {
				Action<Friend, Lobby> onLobbyInvite = SteamMatchmaking.OnLobbyInvite;
				if (onLobbyInvite != null)
				{
					onLobbyInvite(new Friend(x.SteamIDUser), new Lobby(x.SteamIDLobby));
				}
				else
				{
				}
			}, false);
			LobbyDataUpdate_t.Install((LobbyDataUpdate_t x) => {
				if (x.Success != 0)
				{
					if (x.SteamIDLobby != x.SteamIDMember)
					{
						Action<Lobby, Friend> onLobbyMemberDataChanged = SteamMatchmaking.OnLobbyMemberDataChanged;
						if (onLobbyMemberDataChanged != null)
						{
							onLobbyMemberDataChanged(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDMember));
						}
						else
						{
						}
					}
					else
					{
						Action<Lobby> onLobbyDataChanged = SteamMatchmaking.OnLobbyDataChanged;
						if (onLobbyDataChanged != null)
						{
							onLobbyDataChanged(new Lobby(x.SteamIDLobby));
						}
						else
						{
						}
					}
				}
			}, false);
			LobbyChatUpdate_t.Install((LobbyChatUpdate_t x) => {
				if ((x.GfChatMemberStateChange & 1) != 0)
				{
					Action<Lobby, Friend> onLobbyMemberJoined = SteamMatchmaking.OnLobbyMemberJoined;
					if (onLobbyMemberJoined != null)
					{
						onLobbyMemberJoined(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDUserChanged));
					}
					else
					{
					}
				}
				if ((x.GfChatMemberStateChange & 2) != 0)
				{
					Action<Lobby, Friend> onLobbyMemberLeave = SteamMatchmaking.OnLobbyMemberLeave;
					if (onLobbyMemberLeave != null)
					{
						onLobbyMemberLeave(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDUserChanged));
					}
					else
					{
					}
				}
				if ((x.GfChatMemberStateChange & 4) != 0)
				{
					Action<Lobby, Friend> onLobbyMemberDisconnected = SteamMatchmaking.OnLobbyMemberDisconnected;
					if (onLobbyMemberDisconnected != null)
					{
						onLobbyMemberDisconnected(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDUserChanged));
					}
					else
					{
					}
				}
				if ((x.GfChatMemberStateChange & 8) != 0)
				{
					Action<Lobby, Friend, Friend> onLobbyMemberKicked = SteamMatchmaking.OnLobbyMemberKicked;
					if (onLobbyMemberKicked != null)
					{
						onLobbyMemberKicked(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDUserChanged), new Friend(x.SteamIDMakingChange));
					}
					else
					{
					}
				}
				if ((x.GfChatMemberStateChange & 16) != 0)
				{
					Action<Lobby, Friend, Friend> onLobbyMemberBanned = SteamMatchmaking.OnLobbyMemberBanned;
					if (onLobbyMemberBanned != null)
					{
						onLobbyMemberBanned(new Lobby(x.SteamIDLobby), new Friend(x.SteamIDUserChanged), new Friend(x.SteamIDMakingChange));
					}
					else
					{
					}
				}
			}, false);
			LobbyChatMsg_t.Install(new Action<LobbyChatMsg_t>(SteamMatchmaking.OnLobbyChatMessageRecievedAPI), false);
		}

		private static unsafe void OnLobbyChatMessageRecievedAPI(LobbyChatMsg_t callback)
		{
			// 
			// Current member / type: System.Void Steamworks.SteamMatchmaking::OnLobbyChatMessageRecievedAPI(Steamworks.Data.LobbyChatMsg_t)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void OnLobbyChatMessageRecievedAPI(Steamworks.Data.LobbyChatMsg_t)
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

		internal static void Shutdown()
		{
			SteamMatchmaking._internal = null;
		}

		public static event Action<Lobby, Friend, string> OnChatMessage;

		public static event Action<Lobby> OnLobbyDataChanged;

		public static event Action<Friend, Lobby> OnLobbyInvite;

		public static event Action<Lobby, Friend, Friend> OnLobbyMemberBanned;

		public static event Action<Lobby, Friend> OnLobbyMemberDataChanged;

		public static event Action<Lobby, Friend> OnLobbyMemberDisconnected;

		public static event Action<Lobby, Friend> OnLobbyMemberJoined;

		public static event Action<Lobby, Friend, Friend> OnLobbyMemberKicked;

		public static event Action<Lobby, Friend> OnLobbyMemberLeave;
	}
}