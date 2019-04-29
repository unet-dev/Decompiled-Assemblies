using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public class Lobby : IDisposable
	{
		internal Client client;

		public Action<bool> OnLobbyJoined;

		internal Lobby.Type createdLobbyType;

		public Action<bool> OnLobbyCreated;

		public Action OnLobbyDataUpdated;

		public Action<ulong> OnLobbyMemberDataUpdated;

		private static byte[] chatMessageData;

		public Action<ulong, byte[], int> OnChatMessageRecieved;

		public Action<ulong, string> OnChatStringRecieved;

		public Action<Lobby.MemberStateChange, ulong, ulong> OnLobbyStateChanged;

		public Action<ulong, ulong> OnUserInvitedToLobby;

		public Action<ulong> OnLobbyJoinRequested;

		public ulong CurrentLobby
		{
			get;
			private set;
		}

		public Lobby.LobbyData CurrentLobbyData
		{
			get;
			private set;
		}

		public IPAddress GameServerIp
		{
			get
			{
				uint num;
				ushort num1;
				CSteamID cSteamID;
				if (!this.client.native.matchmaking.GetLobbyGameServer(this.CurrentLobby, out num, out num1, out cSteamID) || num == 0)
				{
					return null;
				}
				return new IPAddress(IPAddress.HostToNetworkOrder((long)num));
			}
		}

		public int GameServerPort
		{
			get
			{
				uint num;
				ushort num1;
				CSteamID cSteamID;
				if (!this.client.native.matchmaking.GetLobbyGameServer(this.CurrentLobby, out num, out num1, out cSteamID))
				{
					return 0;
				}
				return num1;
			}
		}

		public ulong GameServerSteamId
		{
			get
			{
				uint num;
				ushort num1;
				CSteamID cSteamID;
				if (!this.client.native.matchmaking.GetLobbyGameServer(this.CurrentLobby, out num, out num1, out cSteamID))
				{
					return (ulong)0;
				}
				return cSteamID;
			}
		}

		public bool IsOwner
		{
			get
			{
				return this.Owner == this.client.SteamId;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.CurrentLobby != (long)0;
			}
		}

		public bool Joinable
		{
			get
			{
				if (!this.IsValid)
				{
					return false;
				}
				string data = this.CurrentLobbyData.GetData("joinable");
				if (data == "true")
				{
					return true;
				}
				if (data == "false")
				{
					return false;
				}
				return false;
			}
			set
			{
				if (!this.IsValid)
				{
					return;
				}
				if (this.client.native.matchmaking.SetLobbyJoinable(this.CurrentLobby, value))
				{
					this.CurrentLobbyData.SetData("joinable", value.ToString());
				}
			}
		}

		public Lobby.Type LobbyType
		{
			get
			{
				if (!this.IsValid)
				{
					return Lobby.Type.Error;
				}
				string data = this.CurrentLobbyData.GetData("lobbytype");
				if (data == "Private")
				{
					return Lobby.Type.Private;
				}
				if (data == "FriendsOnly")
				{
					return Lobby.Type.FriendsOnly;
				}
				if (data == "Invisible")
				{
					return Lobby.Type.Invisible;
				}
				if (data == "Public")
				{
					return Lobby.Type.Public;
				}
				return Lobby.Type.Error;
			}
			set
			{
				if (!this.IsValid)
				{
					return;
				}
				if (this.client.native.matchmaking.SetLobbyType(this.CurrentLobby, (SteamNative.LobbyType)value))
				{
					this.CurrentLobbyData.SetData("lobbytype", value.ToString());
				}
			}
		}

		public int MaxMembers
		{
			get
			{
				if (!this.IsValid)
				{
					return 0;
				}
				return this.client.native.matchmaking.GetLobbyMemberLimit(this.CurrentLobby);
			}
			set
			{
				if (!this.IsValid)
				{
					return;
				}
				this.client.native.matchmaking.SetLobbyMemberLimit(this.CurrentLobby, value);
			}
		}

		public string Name
		{
			get
			{
				if (!this.IsValid)
				{
					return "";
				}
				return this.CurrentLobbyData.GetData("name");
			}
			set
			{
				if (!this.IsValid)
				{
					return;
				}
				this.CurrentLobbyData.SetData("name", value);
			}
		}

		public int NumMembers
		{
			get
			{
				return this.client.native.matchmaking.GetNumLobbyMembers(this.CurrentLobby);
			}
		}

		public ulong Owner
		{
			get
			{
				if (!this.IsValid)
				{
					return (ulong)0;
				}
				return this.client.native.matchmaking.GetLobbyOwner(this.CurrentLobby);
			}
			set
			{
				if (this.Owner == value)
				{
					return;
				}
				this.client.native.matchmaking.SetLobbyOwner(this.CurrentLobby, value);
			}
		}

		static Lobby()
		{
			Lobby.chatMessageData = new byte[4096];
		}

		public Lobby(Client c)
		{
			this.client = c;
			this.OnLobbyJoinRequested = new Action<ulong>(this.Join);
			this.client.RegisterCallback<LobbyDataUpdate_t>(new Action<LobbyDataUpdate_t>(this.OnLobbyDataUpdatedAPI));
			this.client.RegisterCallback<LobbyChatMsg_t>(new Action<LobbyChatMsg_t>(this.OnLobbyChatMessageRecievedAPI));
			this.client.RegisterCallback<LobbyChatUpdate_t>(new Action<LobbyChatUpdate_t>(this.OnLobbyStateUpdatedAPI));
			this.client.RegisterCallback<GameLobbyJoinRequested_t>(new Action<GameLobbyJoinRequested_t>(this.OnLobbyJoinRequestedAPI));
			this.client.RegisterCallback<LobbyInvite_t>(new Action<LobbyInvite_t>(this.OnUserInvitedToLobbyAPI));
			this.client.RegisterCallback<PersonaStateChange_t>(new Action<PersonaStateChange_t>(this.OnLobbyMemberPersonaChangeAPI));
		}

		public void Create(Lobby.Type lobbyType, int maxMembers)
		{
			this.client.native.matchmaking.CreateLobby((SteamNative.LobbyType)lobbyType, maxMembers, new Action<LobbyCreated_t, bool>(this.OnLobbyCreatedAPI));
			this.createdLobbyType = lobbyType;
		}

		public void Dispose()
		{
			this.client = null;
		}

		public string GetMemberData(ulong steamID, string key)
		{
			if (this.CurrentLobby == 0)
			{
				return "ERROR: NOT IN ANY LOBBY";
			}
			return this.client.native.matchmaking.GetLobbyMemberData(this.CurrentLobby, steamID, key);
		}

		public ulong[] GetMemberIDs()
		{
			ulong[] lobbyMemberByIndex = new ulong[this.NumMembers];
			for (int i = 0; i < this.NumMembers; i++)
			{
				lobbyMemberByIndex[i] = this.client.native.matchmaking.GetLobbyMemberByIndex(this.CurrentLobby, i);
			}
			return lobbyMemberByIndex;
		}

		public bool InviteUserToLobby(ulong friendID)
		{
			return this.client.native.matchmaking.InviteUserToLobby(this.CurrentLobby, friendID);
		}

		public void Join(ulong lobbyID)
		{
			this.Leave();
			this.client.native.matchmaking.JoinLobby(lobbyID, new Action<LobbyEnter_t, bool>(this.OnLobbyJoinedAPI));
		}

		public void Leave()
		{
			if (this.CurrentLobby != 0)
			{
				this.client.native.matchmaking.LeaveLobby(this.CurrentLobby);
			}
			this.CurrentLobby = (ulong)0;
			this.CurrentLobbyData = null;
		}

		private unsafe void OnLobbyChatMessageRecievedAPI(LobbyChatMsg_t callback)
		{
			// 
			// Current member / type: System.Void Facepunch.Steamworks.Lobby::OnLobbyChatMessageRecievedAPI(SteamNative.LobbyChatMsg_t)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void OnLobbyChatMessageRecievedAPI(SteamNative.LobbyChatMsg_t)
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

		internal void OnLobbyCreatedAPI(LobbyCreated_t callback, bool error)
		{
			if (error || callback.Result != Result.OK)
			{
				if (this.OnLobbyCreated != null)
				{
					this.OnLobbyCreated(false);
				}
				return;
			}
			this.Owner = this.client.SteamId;
			this.CurrentLobby = callback.SteamIDLobby;
			this.CurrentLobbyData = new Lobby.LobbyData(this.client, this.CurrentLobby);
			this.Name = string.Concat(this.client.Username, "'s Lobby");
			Lobby.LobbyData currentLobbyData = this.CurrentLobbyData;
			uint appId = this.client.AppId;
			currentLobbyData.SetData("appid", appId.ToString());
			this.LobbyType = this.createdLobbyType;
			Lobby.LobbyData lobbyDatum = this.CurrentLobbyData;
			Lobby.Type lobbyType = this.LobbyType;
			lobbyDatum.SetData("lobbytype", lobbyType.ToString());
			this.Joinable = true;
			if (this.OnLobbyCreated != null)
			{
				this.OnLobbyCreated(true);
			}
		}

		internal void OnLobbyDataUpdatedAPI(LobbyDataUpdate_t callback)
		{
			if (callback.SteamIDLobby != this.CurrentLobby)
			{
				return;
			}
			if (callback.SteamIDLobby == this.CurrentLobby)
			{
				this.UpdateLobbyData();
			}
			if (this.UserIsInCurrentLobby(callback.SteamIDMember) && this.OnLobbyMemberDataUpdated != null)
			{
				this.OnLobbyMemberDataUpdated(callback.SteamIDMember);
			}
		}

		private void OnLobbyJoinedAPI(LobbyEnter_t callback, bool error)
		{
			if (error || callback.EChatRoomEnterResponse != 1)
			{
				if (this.OnLobbyJoined != null)
				{
					this.OnLobbyJoined(false);
				}
				return;
			}
			this.CurrentLobby = callback.SteamIDLobby;
			this.UpdateLobbyData();
			if (this.OnLobbyJoined != null)
			{
				this.OnLobbyJoined(true);
			}
		}

		internal void OnLobbyJoinRequestedAPI(GameLobbyJoinRequested_t callback)
		{
			if (this.OnLobbyJoinRequested != null)
			{
				this.OnLobbyJoinRequested(callback.SteamIDLobby);
			}
		}

		internal void OnLobbyMemberPersonaChangeAPI(PersonaStateChange_t callback)
		{
			if (!this.UserIsInCurrentLobby(callback.SteamID))
			{
				return;
			}
			if (this.OnLobbyMemberDataUpdated != null)
			{
				this.OnLobbyMemberDataUpdated(callback.SteamID);
			}
		}

		internal void OnLobbyStateUpdatedAPI(LobbyChatUpdate_t callback)
		{
			if (callback.SteamIDLobby != this.CurrentLobby)
			{
				return;
			}
			Lobby.MemberStateChange gfChatMemberStateChange = (Lobby.MemberStateChange)callback.GfChatMemberStateChange;
			ulong steamIDMakingChange = callback.SteamIDMakingChange;
			ulong steamIDUserChanged = callback.SteamIDUserChanged;
			Action<Lobby.MemberStateChange, ulong, ulong> onLobbyStateChanged = this.OnLobbyStateChanged;
			if (onLobbyStateChanged == null)
			{
				return;
			}
			onLobbyStateChanged(gfChatMemberStateChange, steamIDMakingChange, steamIDUserChanged);
		}

		internal void OnUserInvitedToLobbyAPI(LobbyInvite_t callback)
		{
			if (callback.GameID != (ulong)this.client.AppId)
			{
				return;
			}
			if (this.OnUserInvitedToLobby != null)
			{
				this.OnUserInvitedToLobby(callback.SteamIDLobby, callback.SteamIDUser);
			}
		}

		public void OpenFriendInviteOverlay()
		{
			this.client.native.friends.ActivateGameOverlayInviteDialog(this.CurrentLobby);
		}

		public unsafe bool SendChatMessage(string message)
		{
			// 
			// Current member / type: System.Boolean Facepunch.Steamworks.Lobby::SendChatMessage(System.String)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean SendChatMessage(System.String)
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

		public bool SetGameServer(IPAddress ip, int port, ulong serverSteamId = 0L)
		{
			if (!this.IsValid || !this.IsOwner)
			{
				return false;
			}
			long hostOrder = IPAddress.NetworkToHostOrder(ip.Address);
			this.client.native.matchmaking.SetLobbyGameServer(this.CurrentLobby, (uint)hostOrder, (ushort)port, serverSteamId);
			return true;
		}

		public void SetMemberData(string key, string value)
		{
			if (this.CurrentLobby == 0)
			{
				return;
			}
			this.client.native.matchmaking.SetLobbyMemberData(this.CurrentLobby, key, value);
		}

		internal void UpdateLobbyData()
		{
			string str;
			string str1;
			int lobbyDataCount = this.client.native.matchmaking.GetLobbyDataCount(this.CurrentLobby);
			this.CurrentLobbyData = new Lobby.LobbyData(this.client, this.CurrentLobby);
			for (int i = 0; i < lobbyDataCount; i++)
			{
				if (this.client.native.matchmaking.GetLobbyDataByIndex(this.CurrentLobby, i, out str, out str1))
				{
					this.CurrentLobbyData.SetData(str, str1);
				}
			}
			if (this.OnLobbyDataUpdated != null)
			{
				this.OnLobbyDataUpdated();
			}
		}

		public bool UserIsInCurrentLobby(ulong steamID)
		{
			if (this.CurrentLobby == 0)
			{
				return false;
			}
			ulong[] memberIDs = this.GetMemberIDs();
			for (int i = 0; i < (int)memberIDs.Length; i++)
			{
				if (memberIDs[i] == steamID)
				{
					return true;
				}
			}
			return false;
		}

		public class LobbyData
		{
			internal Client client;

			internal ulong lobby;

			internal Dictionary<string, string> data;

			public LobbyData(Client c, ulong l)
			{
				this.client = c;
				this.lobby = l;
				this.data = new Dictionary<string, string>();
			}

			public Dictionary<string, string> GetAllData()
			{
				Dictionary<string, string> strs = new Dictionary<string, string>();
				foreach (KeyValuePair<string, string> datum in this.data)
				{
					strs.Add(datum.Key, datum.Value);
				}
				return strs;
			}

			public string GetData(string k)
			{
				if (!this.data.ContainsKey(k))
				{
					return "ERROR: key not found";
				}
				return this.data[k];
			}

			public bool RemoveData(string k)
			{
				if (!this.data.ContainsKey(k) || !this.client.native.matchmaking.DeleteLobbyData(this.lobby, k))
				{
					return false;
				}
				this.data.Remove(k);
				return true;
			}

			public bool SetData(string k, string v)
			{
				if (this.data.ContainsKey(k))
				{
					if (this.data[k] == v)
					{
						return true;
					}
					if (this.client.native.matchmaking.SetLobbyData(this.lobby, k, v))
					{
						this.data[k] = v;
						return true;
					}
				}
				else if (this.client.native.matchmaking.SetLobbyData(this.lobby, k, v))
				{
					this.data.Add(k, v);
					return true;
				}
				return false;
			}
		}

		public enum MemberStateChange
		{
			Entered = 1,
			Left = 2,
			Disconnected = 4,
			Kicked = 8,
			Banned = 16
		}

		public enum Type
		{
			Private,
			FriendsOnly,
			Public,
			Invisible,
			Error
		}
	}
}