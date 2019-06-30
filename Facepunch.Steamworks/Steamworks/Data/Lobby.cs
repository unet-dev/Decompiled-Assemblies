using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	public struct Lobby
	{
		public IEnumerable<KeyValuePair<string, string>> Data
		{
			get
			{
				int lobbyDataCount = SteamMatchmaking.Internal.GetLobbyDataCount(this.Id);
				StringBuilder stringBuilder = Helpers.TakeStringBuilder();
				StringBuilder stringBuilder1 = Helpers.TakeStringBuilder();
				for (int i = 0; i < lobbyDataCount; i++)
				{
					if (SteamMatchmaking.Internal.GetLobbyDataByIndex(this.Id, i, stringBuilder, stringBuilder.Capacity, stringBuilder1, stringBuilder1.Capacity))
					{
						yield return new KeyValuePair<string, string>(stringBuilder.ToString(), stringBuilder1.ToString());
					}
				}
			}
		}

		public SteamId Id
		{
			get;
			internal set;
		}

		public int MaxMembers
		{
			get
			{
				return SteamMatchmaking.Internal.GetLobbyMemberLimit(this.Id);
			}
			set
			{
				SteamMatchmaking.Internal.SetLobbyMemberLimit(this.Id, value);
			}
		}

		public int MemberCount
		{
			get
			{
				return SteamMatchmaking.Internal.GetNumLobbyMembers(this.Id);
			}
		}

		public IEnumerable<Friend> Members
		{
			get
			{
				for (int i = 0; i < this.MemberCount; i++)
				{
					yield return new Friend(SteamMatchmaking.Internal.GetLobbyMemberByIndex(this.Id, i));
				}
			}
		}

		public Friend Owner
		{
			get
			{
				return new Friend(SteamMatchmaking.Internal.GetLobbyOwner(this.Id));
			}
			set
			{
				SteamMatchmaking.Internal.SetLobbyOwner(this.Id, value.Id);
			}
		}

		internal Lobby(SteamId id)
		{
			this.Id = id;
		}

		public bool DeleteData(string key)
		{
			return SteamMatchmaking.Internal.DeleteLobbyData(this.Id, key);
		}

		public string GetData(string key)
		{
			return SteamMatchmaking.Internal.GetLobbyData(this.Id, key);
		}

		public string GetMemberData(Friend member, string key)
		{
			string lobbyMemberData = SteamMatchmaking.Internal.GetLobbyMemberData(this.Id, member.Id, key);
			return lobbyMemberData;
		}

		public bool InviteFriend(SteamId steamid)
		{
			return SteamMatchmaking.Internal.InviteUserToLobby(this.Id, steamid);
		}

		public async Task<RoomEnter> Join()
		{
			RoomEnter eChatRoomEnterResponse;
			LobbyEnter_t? nullable = await SteamMatchmaking.Internal.JoinLobby(this.Id);
			LobbyEnter_t? nullable1 = nullable;
			nullable = null;
			if (nullable1.HasValue)
			{
				eChatRoomEnterResponse = nullable1.Value.EChatRoomEnterResponse;
			}
			else
			{
				eChatRoomEnterResponse = RoomEnter.Error;
			}
			return eChatRoomEnterResponse;
		}

		public void Leave()
		{
			SteamMatchmaking.Internal.LeaveLobby(this.Id);
		}

		public bool Refresh()
		{
			return SteamMatchmaking.Internal.RequestLobbyData(this.Id);
		}

		internal unsafe bool SendChatBytes(byte[] data)
		{
			// 
			// Current member / type: System.Boolean Steamworks.Data.Lobby::SendChatBytes(System.Byte[])
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean SendChatBytes(System.Byte[])
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

		public bool SendChatString(string message)
		{
			return this.SendChatBytes(Encoding.UTF8.GetBytes(message));
		}

		public bool SetData(string key, string value)
		{
			if (key.Length > 255)
			{
				throw new ArgumentException("Key should be < 255 chars", "key");
			}
			if (value.Length > 8192)
			{
				throw new ArgumentException("Value should be < 8192 chars", "key");
			}
			return SteamMatchmaking.Internal.SetLobbyData(this.Id, key, value);
		}

		public bool SetFriendsOnly()
		{
			return SteamMatchmaking.Internal.SetLobbyType(this.Id, LobbyType.FriendsOnly);
		}

		public bool SetInvisible()
		{
			return SteamMatchmaking.Internal.SetLobbyType(this.Id, LobbyType.Invisible);
		}

		public bool SetJoinable(bool b)
		{
			return SteamMatchmaking.Internal.SetLobbyJoinable(this.Id, b);
		}

		public void SetMemberData(Friend member, string key, string value)
		{
			SteamMatchmaking.Internal.SetLobbyMemberData(this.Id, key, value);
		}

		public bool SetPrivate()
		{
			return SteamMatchmaking.Internal.SetLobbyType(this.Id, LobbyType.Private);
		}

		public bool SetPublic()
		{
			return SteamMatchmaking.Internal.SetLobbyType(this.Id, LobbyType.Public);
		}
	}
}