using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Steamworks
{
	public static class SteamUser
	{
		private static ISteamUser _internal;

		private static Dictionary<string, string> richPresence;

		private static bool _recordingVoice;

		private static byte[] readBuffer;

		private static uint sampleRate;

		public static bool HasVoiceData
		{
			get
			{
				bool flag;
				uint num = 0;
				uint num1 = 0;
				flag = (SteamUser.Internal.GetAvailableVoice(ref num, ref num1, 0) == VoiceResult.OK ? num != 0 : false);
				return flag;
			}
		}

		internal static ISteamUser Internal
		{
			get
			{
				if (SteamUser._internal == null)
				{
					SteamUser._internal = new ISteamUser();
					SteamUser._internal.Init();
					SteamUser.richPresence = new Dictionary<string, string>();
					SteamUser.SampleRate = SteamUser.OptimalSampleRate;
				}
				return SteamUser._internal;
			}
		}

		public static bool IsBehindNAT
		{
			get
			{
				return SteamUser.Internal.BIsBehindNAT();
			}
		}

		public static bool IsPhoneIdentifying
		{
			get
			{
				return SteamUser.Internal.BIsPhoneIdentifying();
			}
		}

		public static bool IsPhoneRequiringVerification
		{
			get
			{
				return SteamUser.Internal.BIsPhoneRequiringVerification();
			}
		}

		public static bool IsPhoneVerified
		{
			get
			{
				return SteamUser.Internal.BIsPhoneVerified();
			}
		}

		public static bool IsTwoFactorEnabled
		{
			get
			{
				return SteamUser.Internal.BIsTwoFactorEnabled();
			}
		}

		public static uint OptimalSampleRate
		{
			get
			{
				return SteamUser.Internal.GetVoiceOptimalSampleRate();
			}
		}

		public static uint SampleRate
		{
			get
			{
				return SteamUser.sampleRate;
			}
			set
			{
				if (SteamUser.SampleRate < 11025)
				{
					throw new Exception("Sample Rate must be between 11025 and 48000");
				}
				if (SteamUser.SampleRate > 48000)
				{
					throw new Exception("Sample Rate must be between 11025 and 48000");
				}
				SteamUser.sampleRate = value;
			}
		}

		public static int SteamLevel
		{
			get
			{
				return SteamUser.Internal.GetPlayerSteamLevel();
			}
		}

		public static bool VoiceRecord
		{
			get
			{
				return SteamUser._recordingVoice;
			}
			set
			{
				SteamUser._recordingVoice = value;
				if (!value)
				{
					SteamUser.Internal.StopVoiceRecording();
				}
				else
				{
					SteamUser.Internal.StartVoiceRecording();
				}
			}
		}

		static SteamUser()
		{
			SteamUser.readBuffer = new Byte[131072];
			SteamUser.sampleRate = 48000;
		}

		public static unsafe BeginAuthResult BeginAuthSession(byte[] ticketData, SteamId steamid)
		{
			// 
			// Current member / type: Steamworks.BeginAuthResult Steamworks.SteamUser::BeginAuthSession(System.Byte[],Steamworks.SteamId)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: Steamworks.BeginAuthResult BeginAuthSession(System.Byte[],Steamworks.SteamId)
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

		public static unsafe int DecompressVoice(Stream input, int length, Stream output)
		{
			// 
			// Current member / type: System.Int32 Steamworks.SteamUser::DecompressVoice(System.IO.Stream,System.Int32,System.IO.Stream)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Int32 DecompressVoice(System.IO.Stream,System.Int32,System.IO.Stream)
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

		public static unsafe int DecompressVoice(byte[] from, Stream output)
		{
			// 
			// Current member / type: System.Int32 Steamworks.SteamUser::DecompressVoice(System.Byte[],System.IO.Stream)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Int32 DecompressVoice(System.Byte[],System.IO.Stream)
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

		public static void EndAuthSession(SteamId steamid)
		{
			SteamUser.Internal.EndAuthSession(steamid);
		}

		public static unsafe AuthTicket GetAuthSessionTicket()
		{
			// 
			// Current member / type: Steamworks.AuthTicket Steamworks.SteamUser::GetAuthSessionTicket()
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: Steamworks.AuthTicket GetAuthSessionTicket()
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

		public static async Task<AuthTicket> GetAuthSessionTicketAsync(double timeoutSeconds = 10)
		{
			SteamUser.<GetAuthSessionTicketAsync>d__51 variable = null;
			AsyncTaskMethodBuilder<AuthTicket> asyncTaskMethodBuilder = AsyncTaskMethodBuilder<AuthTicket>.Create();
			asyncTaskMethodBuilder.Start<SteamUser.<GetAuthSessionTicketAsync>d__51>(ref variable);
			return asyncTaskMethodBuilder.Task;
		}

		public static async Task<string> GetStoreAuthUrlAsync(string url)
		{
			string uRL;
			StoreAuthURLResponse_t? nullable = await SteamUser.Internal.RequestStoreAuthURL(url);
			StoreAuthURLResponse_t? nullable1 = nullable;
			nullable = null;
			if (nullable1.HasValue)
			{
				uRL = nullable1.Value.URL;
			}
			else
			{
				uRL = null;
			}
			return uRL;
		}

		internal static void InstallEvents()
		{
			SteamServersConnected_t.Install((SteamServersConnected_t x) => {
				Action onSteamServersConnected = SteamUser.OnSteamServersConnected;
				if (onSteamServersConnected != null)
				{
					onSteamServersConnected();
				}
				else
				{
				}
			}, false);
			SteamServerConnectFailure_t.Install((SteamServerConnectFailure_t x) => {
				Action onSteamServerConnectFailure = SteamUser.OnSteamServerConnectFailure;
				if (onSteamServerConnectFailure != null)
				{
					onSteamServerConnectFailure();
				}
				else
				{
				}
			}, false);
			SteamServersDisconnected_t.Install((SteamServersDisconnected_t x) => {
				Action onSteamServersDisconnected = SteamUser.OnSteamServersDisconnected;
				if (onSteamServersDisconnected != null)
				{
					onSteamServersDisconnected();
				}
				else
				{
				}
			}, false);
			ClientGameServerDeny_t.Install((ClientGameServerDeny_t x) => {
				Action onClientGameServerDeny = SteamUser.OnClientGameServerDeny;
				if (onClientGameServerDeny != null)
				{
					onClientGameServerDeny();
				}
				else
				{
				}
			}, false);
			LicensesUpdated_t.Install((LicensesUpdated_t x) => {
				Action onLicensesUpdated = SteamUser.OnLicensesUpdated;
				if (onLicensesUpdated != null)
				{
					onLicensesUpdated();
				}
				else
				{
				}
			}, false);
			ValidateAuthTicketResponse_t.Install((ValidateAuthTicketResponse_t x) => {
				Action<SteamId, SteamId, AuthResponse> onValidateAuthTicketResponse = SteamUser.OnValidateAuthTicketResponse;
				if (onValidateAuthTicketResponse != null)
				{
					onValidateAuthTicketResponse(x.SteamID, x.OwnerSteamID, x.AuthSessionResponse);
				}
				else
				{
				}
			}, false);
			MicroTxnAuthorizationResponse_t.Install((MicroTxnAuthorizationResponse_t x) => {
				Action<AppId, ulong, bool> onMicroTxnAuthorizationResponse = SteamUser.OnMicroTxnAuthorizationResponse;
				if (onMicroTxnAuthorizationResponse != null)
				{
					onMicroTxnAuthorizationResponse(x.AppID, x.OrderID, x.Authorized != 0);
				}
				else
				{
				}
			}, false);
			GameWebCallback_t.Install((GameWebCallback_t x) => {
				Action<string> onGameWebCallback = SteamUser.OnGameWebCallback;
				if (onGameWebCallback != null)
				{
					onGameWebCallback(x.URL);
				}
				else
				{
				}
			}, false);
			GetAuthSessionTicketResponse_t.Install((GetAuthSessionTicketResponse_t x) => {
				Action<GetAuthSessionTicketResponse_t> onGetAuthSessionTicketResponse = SteamUser.OnGetAuthSessionTicketResponse;
				if (onGetAuthSessionTicketResponse != null)
				{
					onGetAuthSessionTicketResponse(x);
				}
				else
				{
				}
			}, false);
		}

		public static unsafe int ReadVoiceData(Stream stream)
		{
			// 
			// Current member / type: System.Int32 Steamworks.SteamUser::ReadVoiceData(System.IO.Stream)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Int32 ReadVoiceData(System.IO.Stream)
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

		public static unsafe byte[] ReadVoiceDataBytes()
		{
			// 
			// Current member / type: System.Byte[] Steamworks.SteamUser::ReadVoiceDataBytes()
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Byte[] ReadVoiceDataBytes()
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

		public static async Task<byte[]> RequestEncryptedAppTicketAsync(byte[] dataToInclude)
		{
			SteamUser.<RequestEncryptedAppTicketAsync>d__67 variable = null;
			AsyncTaskMethodBuilder<byte[]> asyncTaskMethodBuilder = AsyncTaskMethodBuilder<byte[]>.Create();
			asyncTaskMethodBuilder.Start<SteamUser.<RequestEncryptedAppTicketAsync>d__67>(ref variable);
			return asyncTaskMethodBuilder.Task;
		}

		public static async Task<byte[]> RequestEncryptedAppTicketAsync()
		{
			byte[] numArray;
			bool flag;
			EncryptedAppTicketResponse_t? nullable = await SteamUser.Internal.RequestEncryptedAppTicket(IntPtr.Zero, 0);
			EncryptedAppTicketResponse_t? nullable1 = nullable;
			nullable = null;
			flag = (!nullable1.HasValue ? true : nullable1.Value.Result != Result.OK);
			if (!flag)
			{
				IntPtr intPtr = Marshal.AllocHGlobal(1024);
				uint num = 0;
				byte[] numArray1 = null;
				if (SteamUser.Internal.GetEncryptedAppTicket(intPtr, 1024, ref num))
				{
					numArray1 = new Byte[num];
					Marshal.Copy(intPtr, numArray1, 0, (int)num);
				}
				Marshal.FreeHGlobal(intPtr);
				numArray = numArray1;
			}
			else
			{
				numArray = null;
			}
			return numArray;
		}

		internal static void Shutdown()
		{
			SteamUser._internal = null;
		}

		public static event Action OnClientGameServerDeny;

		public static event Action<string> OnGameWebCallback;

		internal static event Action<GetAuthSessionTicketResponse_t> OnGetAuthSessionTicketResponse;

		public static event Action OnLicensesUpdated;

		public static event Action<AppId, ulong, bool> OnMicroTxnAuthorizationResponse;

		public static event Action OnSteamServerConnectFailure;

		public static event Action OnSteamServersConnected;

		public static event Action OnSteamServersDisconnected;

		public static event Action<SteamId, SteamId, AuthResponse> OnValidateAuthTicketResponse;
	}
}