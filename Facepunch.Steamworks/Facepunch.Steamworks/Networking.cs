using SteamNative;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public class Networking : IDisposable
	{
		private static byte[] ReceiveBuffer;

		public Networking.OnRecievedP2PData OnP2PData;

		public Func<ulong, bool> OnIncomingConnection;

		public Action<ulong, Networking.SessionError> OnConnectionFailed;

		private List<int> ListenChannels = new List<int>();

		private Stopwatch UpdateTimer = Stopwatch.StartNew();

		internal SteamNetworking networking;

		static Networking()
		{
			Networking.ReceiveBuffer = new byte[65536];
		}

		internal Networking(BaseSteamworks steamworks, SteamNetworking networking)
		{
			this.networking = networking;
			steamworks.RegisterCallback<P2PSessionRequest_t>(new Action<P2PSessionRequest_t>(this.onP2PConnectionRequest));
			steamworks.RegisterCallback<P2PSessionConnectFail_t>(new Action<P2PSessionConnectFail_t>(this.onP2PConnectionFailed));
		}

		public bool CloseSession(ulong steamId)
		{
			return this.networking.CloseP2PSessionWithUser(steamId);
		}

		public void Dispose()
		{
			this.networking = null;
			this.OnIncomingConnection = null;
			this.OnConnectionFailed = null;
			this.OnP2PData = null;
			this.ListenChannels.Clear();
		}

		private void onP2PConnectionFailed(P2PSessionConnectFail_t o)
		{
			if (this.OnConnectionFailed != null)
			{
				this.OnConnectionFailed(o.SteamIDRemote, o.P2PSessionError);
			}
		}

		private void onP2PConnectionRequest(P2PSessionRequest_t o)
		{
			if (this.OnIncomingConnection == null)
			{
				this.networking.CloseP2PSessionWithUser(o.SteamIDRemote);
				return;
			}
			if (this.OnIncomingConnection(o.SteamIDRemote))
			{
				this.networking.AcceptP2PSessionWithUser(o.SteamIDRemote);
				return;
			}
			this.networking.CloseP2PSessionWithUser(o.SteamIDRemote);
		}

		private unsafe bool ReadP2PPacket(int channel)
		{
			// 
			// Current member / type: System.Boolean Facepunch.Steamworks.Networking::ReadP2PPacket(System.Int32)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean ReadP2PPacket(System.Int32)
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

		public unsafe bool SendP2PPacket(ulong steamid, byte[] data, int length, Networking.SendType eP2PSendType = 2, int nChannel = 0)
		{
			// 
			// Current member / type: System.Boolean Facepunch.Steamworks.Networking::SendP2PPacket(System.UInt64,System.Byte[],System.Int32,Facepunch.Steamworks.Networking/SendType,System.Int32)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean SendP2PPacket(System.UInt64,System.Byte[],System.Int32,Facepunch.Steamworks.Networking/SendType,System.Int32)
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

		public void SetListenChannel(int ChannelId, bool Listen)
		{
			this.ListenChannels.RemoveAll((int x) => x == ChannelId);
			if (Listen)
			{
				this.ListenChannels.Add(ChannelId);
			}
		}

		public void Update()
		{
			if (this.OnP2PData == null)
			{
				return;
			}
			if (this.UpdateTimer.Elapsed.TotalSeconds < 0.0166666666666667)
			{
				return;
			}
			this.UpdateTimer.Reset();
			this.UpdateTimer.Start();
			foreach (int listenChannel in this.ListenChannels)
			{
				while (this.ReadP2PPacket(listenChannel))
				{
				}
			}
		}

		public delegate void OnRecievedP2PData(ulong steamid, byte[] data, int dataLength, int channel);

		public enum SendType
		{
			Unreliable,
			UnreliableNoDelay,
			Reliable,
			ReliableWithBuffering
		}

		public enum SessionError : byte
		{
			None,
			NotRunningApp,
			NoRightsToApp,
			DestinationNotLoggedIn,
			Timeout,
			Max
		}
	}
}