using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Steamworks
{
	public static class SteamServer
	{
		private static bool initialized;

		private static ISteamGameServer _internal;

		public static Action<Exception> OnCallbackException;

		private static List<SteamInterface> openIterfaces;

		private static bool _dedicatedServer;

		private static int _maxplayers;

		private static int _botcount;

		private static string _mapname;

		private static string _modDir;

		private static string _product;

		private static string _gameDescription;

		private static string _serverName;

		private static bool _passworded;

		private static string _gametags;

		private static Dictionary<string, string> KeyValue;

		public static int AutomaticHeartbeatRate
		{
			set
			{
				SteamServer.Internal.SetHeartbeatInterval(value);
			}
		}

		public static bool AutomaticHeartbeats
		{
			set
			{
				SteamServer.Internal.EnableHeartbeats(value);
			}
		}

		public static int BotCount
		{
			get
			{
				return SteamServer._botcount;
			}
			set
			{
				if (SteamServer._botcount != value)
				{
					SteamServer.Internal.SetBotPlayerCount(value);
					SteamServer._botcount = value;
				}
			}
		}

		public static bool DedicatedServer
		{
			get
			{
				return SteamServer._dedicatedServer;
			}
			set
			{
				if (SteamServer._dedicatedServer != value)
				{
					SteamServer.Internal.SetDedicatedServer(value);
					SteamServer._dedicatedServer = value;
				}
			}
		}

		public static string GameDescription
		{
			get
			{
				return SteamServer._gameDescription;
			}
			internal set
			{
				if (SteamServer._gameDescription != value)
				{
					SteamServer.Internal.SetGameDescription(value);
					SteamServer._gameDescription = value;
				}
			}
		}

		public static string GameTags
		{
			get
			{
				return SteamServer._gametags;
			}
			set
			{
				if (SteamServer._gametags != value)
				{
					SteamServer.Internal.SetGameTags(value);
					SteamServer._gametags = value;
				}
			}
		}

		internal static ISteamGameServer Internal
		{
			get
			{
				if (SteamServer._internal == null)
				{
					SteamServer._internal = new ISteamGameServer();
					SteamServer._internal.InitServer();
				}
				return SteamServer._internal;
			}
		}

		public static bool IsValid
		{
			get
			{
				return SteamServer.initialized;
			}
		}

		public static bool LoggedOn
		{
			get
			{
				return SteamServer.Internal.BLoggedOn();
			}
		}

		public static string MapName
		{
			get
			{
				return SteamServer._mapname;
			}
			set
			{
				if (SteamServer._mapname != value)
				{
					SteamServer.Internal.SetMapName(value);
					SteamServer._mapname = value;
				}
			}
		}

		public static int MaxPlayers
		{
			get
			{
				return SteamServer._maxplayers;
			}
			set
			{
				if (SteamServer._maxplayers != value)
				{
					SteamServer.Internal.SetMaxPlayerCount(value);
					SteamServer._maxplayers = value;
				}
			}
		}

		public static string ModDir
		{
			get
			{
				return SteamServer._modDir;
			}
			internal set
			{
				if (SteamServer._modDir != value)
				{
					SteamServer.Internal.SetModDir(value);
					SteamServer._modDir = value;
				}
			}
		}

		public static bool Passworded
		{
			get
			{
				return SteamServer._passworded;
			}
			set
			{
				if (SteamServer._passworded != value)
				{
					SteamServer.Internal.SetPasswordProtected(value);
					SteamServer._passworded = value;
				}
			}
		}

		public static string Product
		{
			get
			{
				return SteamServer._product;
			}
			internal set
			{
				if (SteamServer._product != value)
				{
					SteamServer.Internal.SetProduct(value);
					SteamServer._product = value;
				}
			}
		}

		public static IPAddress PublicIp
		{
			get
			{
				IPAddress ip;
				uint publicIP = SteamServer.Internal.GetPublicIP();
				if (publicIP != 0)
				{
					ip = Utility.Int32ToIp(publicIP);
				}
				else
				{
					ip = null;
				}
				return ip;
			}
		}

		public static string ServerName
		{
			get
			{
				return SteamServer._serverName;
			}
			set
			{
				if (SteamServer._serverName != value)
				{
					SteamServer.Internal.SetServerName(value);
					SteamServer._serverName = value;
				}
			}
		}

		static SteamServer()
		{
			SteamServer.openIterfaces = new List<SteamInterface>();
			SteamServer._maxplayers = 0;
			SteamServer._botcount = 0;
			SteamServer._modDir = "";
			SteamServer._product = "";
			SteamServer._gameDescription = "";
			SteamServer._serverName = "";
			SteamServer._gametags = "";
			SteamServer.KeyValue = new Dictionary<string, string>();
		}

		public static unsafe bool BeginAuthSession(byte[] data, SteamId steamid)
		{
			// 
			// Current member / type: System.Boolean Steamworks.SteamServer::BeginAuthSession(System.Byte[],Steamworks.SteamId)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean BeginAuthSession(System.Byte[],Steamworks.SteamId)
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

		public static void ClearKeys()
		{
			SteamServer.KeyValue.Clear();
			SteamServer.Internal.ClearAllKeyValues();
		}

		public static void EndSession(SteamId steamid)
		{
			SteamServer.Internal.EndAuthSession(steamid);
		}

		public static void ForceHeartbeat()
		{
			SteamServer.Internal.ForceHeartbeat();
		}

		public static unsafe bool GetOutgoingPacket(out OutgoingPacket packet)
		{
			// 
			// Current member / type: System.Boolean Steamworks.SteamServer::GetOutgoingPacket(Steamworks.Data.OutgoingPacket&)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean GetOutgoingPacket(Steamworks.Data.OutgoingPacket&)
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

		public static unsafe void HandleIncomingPacket(byte[] data, int size, uint address, ushort port)
		{
			// 
			// Current member / type: System.Void Steamworks.SteamServer::HandleIncomingPacket(System.Byte[],System.Int32,System.UInt32,System.UInt16)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void HandleIncomingPacket(System.Byte[],System.Int32,System.UInt32,System.UInt16)
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

		public static void HandleIncomingPacket(IntPtr ptr, int size, uint address, ushort port)
		{
			SteamServer.Internal.HandleIncomingPacket(ptr, size, address, port);
		}

		public static void Init(AppId appid, SteamServerInit init)
		{
			uint num = 0;
			if (init.SteamPort == 0)
			{
				init = init.WithRandomSteamPort();
			}
			if (init.IpAddress != null)
			{
				num = init.IpAddress.IpToInt32();
			}
			Environment.SetEnvironmentVariable("SteamAppId", appid.ToString());
			Environment.SetEnvironmentVariable("SteamGameId", appid.ToString());
			int num1 = (init.Secure ? 3 : 2);
			if (!SteamInternal.GameServer_Init(num, init.SteamPort, init.GamePort, init.QueryPort, num1, init.VersionString))
			{
				throw new Exception(String.Format("InitGameServer returned false ({0},{1},{2},{3},{4},\"{5}\")", new Object[] { num, init.SteamPort, init.GamePort, init.QueryPort, num1, init.VersionString }));
			}
			SteamServer.initialized = true;
			SteamServer.AutomaticHeartbeats = true;
			SteamServer.MaxPlayers = 32;
			SteamServer.BotCount = 0;
			SteamServer.Product = String.Format("{0}", appid.Value);
			SteamServer.ModDir = init.ModDir;
			SteamServer.GameDescription = init.GameDescription;
			SteamServer.Passworded = false;
			SteamServer.DedicatedServer = init.DedicatedServer;
			SteamServer.InstallEvents();
			SteamServer.RunCallbacksAsync();
		}

		internal static void InstallEvents()
		{
			SteamInventory.InstallEvents();
			ValidateAuthTicketResponse_t.Install((ValidateAuthTicketResponse_t x) => {
				Action<SteamId, SteamId, AuthResponse> onValidateAuthTicketResponse = SteamServer.OnValidateAuthTicketResponse;
				if (onValidateAuthTicketResponse != null)
				{
					onValidateAuthTicketResponse(x.SteamID, x.OwnerSteamID, x.AuthSessionResponse);
				}
				else
				{
				}
			}, true);
			SteamServersConnected_t.Install((SteamServersConnected_t x) => {
				Action onSteamServersConnected = SteamServer.OnSteamServersConnected;
				if (onSteamServersConnected != null)
				{
					onSteamServersConnected();
				}
				else
				{
				}
			}, true);
			SteamServerConnectFailure_t.Install((SteamServerConnectFailure_t x) => {
				Action<Result, bool> onSteamServerConnectFailure = SteamServer.OnSteamServerConnectFailure;
				if (onSteamServerConnectFailure != null)
				{
					onSteamServerConnectFailure(x.Result, x.StillRetrying);
				}
				else
				{
				}
			}, true);
			SteamServersDisconnected_t.Install((SteamServersDisconnected_t x) => {
				Action<Result> onSteamServersDisconnected = SteamServer.OnSteamServersDisconnected;
				if (onSteamServersDisconnected != null)
				{
					onSteamServersDisconnected(x.Result);
				}
				else
				{
				}
			}, true);
		}

		public static void LogOff()
		{
			SteamServer.Internal.LogOff();
		}

		public static void LogOnAnonymous()
		{
			SteamServer.Internal.LogOnAnonymous();
			SteamServer.ForceHeartbeat();
		}

		public static void RunCallbacks()
		{
			try
			{
				SteamGameServer.RunCallbacks();
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Action<Exception> onCallbackException = SteamServer.OnCallbackException;
				if (onCallbackException != null)
				{
					onCallbackException(exception);
				}
				else
				{
				}
			}
		}

		internal static async void RunCallbacksAsync()
		{
			while (SteamServer.IsValid)
			{
				SteamServer.RunCallbacks();
				await Task.Delay(16);
			}
		}

		public static void SetKey(string Key, string Value)
		{
			if (!SteamServer.KeyValue.ContainsKey(Key))
			{
				SteamServer.KeyValue.Add(Key, Value);
			}
			else
			{
				if (SteamServer.KeyValue[Key] == Value)
				{
					return;
				}
				SteamServer.KeyValue[Key] = Value;
			}
			SteamServer.Internal.SetKeyValue(Key, Value);
		}

		public static void Shutdown()
		{
			Event.DisposeAllServer();
			SteamServer.initialized = false;
			SteamServer._internal = null;
			SteamServer.ShutdownInterfaces();
			SteamNetworkingUtils.Shutdown();
			SteamNetworkingSockets.Shutdown();
			SteamInventory.Shutdown();
			SteamGameServer.Shutdown();
		}

		internal static void ShutdownInterfaces()
		{
			foreach (SteamInterface openIterface in SteamServer.openIterfaces)
			{
				openIterface.Shutdown();
			}
			SteamServer.openIterfaces.Clear();
		}

		public static void UpdatePlayer(SteamId steamid, string name, int score)
		{
			SteamServer.Internal.BUpdateUserData(steamid, name, (uint)score);
		}

		internal static void WatchInterface(SteamInterface steamInterface)
		{
			if (SteamServer.openIterfaces.Contains(steamInterface))
			{
				throw new Exception("openIterfaces already contains interface!");
			}
			SteamServer.openIterfaces.Add(steamInterface);
		}

		public static event Action<Result, bool> OnSteamServerConnectFailure;

		public static event Action OnSteamServersConnected;

		public static event Action<Result> OnSteamServersDisconnected;

		public static event Action<SteamId, SteamId, AuthResponse> OnValidateAuthTicketResponse;
	}
}