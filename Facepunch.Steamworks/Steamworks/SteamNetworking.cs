using Steamworks.Data;
using System;
using System.Runtime.CompilerServices;

namespace Steamworks
{
	public static class SteamNetworking
	{
		private static ISteamNetworking _internal;

		public static Action<SteamId> OnP2PSessionRequest;

		public static Action<SteamId> OnP2PConnectionFailed;

		internal static ISteamNetworking Internal
		{
			get
			{
				if (SteamNetworking._internal == null)
				{
					SteamNetworking._internal = new ISteamNetworking();
					SteamNetworking._internal.Init();
				}
				return SteamNetworking._internal;
			}
		}

		public static bool AcceptP2PSessionWithUser(SteamId user)
		{
			return SteamNetworking.Internal.AcceptP2PSessionWithUser(user);
		}

		public static bool CloseP2PSessionWithUser(SteamId user)
		{
			return SteamNetworking.Internal.CloseP2PSessionWithUser(user);
		}

		internal static void InstallEvents()
		{
			P2PSessionRequest_t.Install((P2PSessionRequest_t x) => {
				Action<SteamId> onP2PSessionRequest = SteamNetworking.OnP2PSessionRequest;
				if (onP2PSessionRequest != null)
				{
					onP2PSessionRequest(x.SteamIDRemote);
				}
				else
				{
				}
			}, false);
			P2PSessionConnectFail_t.Install((P2PSessionConnectFail_t x) => {
				Action<SteamId> onP2PConnectionFailed = SteamNetworking.OnP2PConnectionFailed;
				if (onP2PConnectionFailed != null)
				{
					onP2PConnectionFailed(x.SteamIDRemote);
				}
				else
				{
				}
			}, false);
		}

		public static bool IsP2PPacketAvailable(int channel = 0)
		{
			uint num = 0;
			return SteamNetworking.Internal.IsP2PPacketAvailable(ref num, channel);
		}

		public static unsafe P2Packet? ReadP2PPacket(int channel = 0)
		{
			// 
			// Current member / type: System.Nullable`1<Steamworks.Data.P2Packet> Steamworks.SteamNetworking::ReadP2PPacket(System.Int32)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Nullable<Steamworks.Data.P2Packet> ReadP2PPacket(System.Int32)
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

		public static unsafe bool SendP2PPacket(SteamId steamid, byte[] data, int length = -1, int nChannel = 0, P2PSend sendType = 2)
		{
			// 
			// Current member / type: System.Boolean Steamworks.SteamNetworking::SendP2PPacket(Steamworks.SteamId,System.Byte[],System.Int32,System.Int32,Steamworks.P2PSend)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean SendP2PPacket(Steamworks.SteamId,System.Byte[],System.Int32,System.Int32,Steamworks.P2PSend)
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
			SteamNetworking._internal = null;
		}
	}
}