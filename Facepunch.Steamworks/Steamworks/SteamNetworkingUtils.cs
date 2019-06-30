using Steamworks.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks
{
	public static class SteamNetworkingUtils
	{
		private static ISteamNetworkingUtils _internal;

		public static float FakeRecvPacketLag
		{
			get
			{
				return SteamNetworkingUtils.GetConfigFloat(NetConfig.FakePacketLag_Recv);
			}
			set
			{
				SteamNetworkingUtils.SetConfigFloat(NetConfig.FakePacketLag_Recv, value);
			}
		}

		public static float FakeRecvPacketLoss
		{
			get
			{
				return SteamNetworkingUtils.GetConfigFloat(NetConfig.FakePacketLoss_Recv);
			}
			set
			{
				SteamNetworkingUtils.SetConfigFloat(NetConfig.FakePacketLoss_Recv, value);
			}
		}

		public static float FakeSendPacketLag
		{
			get
			{
				return SteamNetworkingUtils.GetConfigFloat(NetConfig.FakePacketLag_Send);
			}
			set
			{
				SteamNetworkingUtils.SetConfigFloat(NetConfig.FakePacketLag_Send, value);
			}
		}

		public static float FakeSendPacketLoss
		{
			get
			{
				return SteamNetworkingUtils.GetConfigFloat(NetConfig.FakePacketLoss_Send);
			}
			set
			{
				SteamNetworkingUtils.SetConfigFloat(NetConfig.FakePacketLoss_Send, value);
			}
		}

		internal static ISteamNetworkingUtils Internal
		{
			get
			{
				if (SteamNetworkingUtils._internal == null)
				{
					SteamNetworkingUtils._internal = new ISteamNetworkingUtils();
					SteamNetworkingUtils._internal.InitUserless();
				}
				return SteamNetworkingUtils._internal;
			}
		}

		public static PingLocation? LocalPingLocation
		{
			get
			{
				PingLocation? nullable;
				PingLocation pingLocation = new PingLocation();
				if (SteamNetworkingUtils.Internal.GetLocalPingLocation(ref pingLocation) >= 0f)
				{
					nullable = new PingLocation?(pingLocation);
				}
				else
				{
					nullable = null;
				}
				return nullable;
			}
		}

		public static long LocalTimetamp
		{
			get
			{
				return SteamNetworkingUtils.Internal.GetLocalTimestamp();
			}
		}

		public static int EstimatePingTo(PingLocation target)
		{
			return SteamNetworkingUtils.Internal.EstimatePingTimeFromLocalHost(ref target);
		}

		internal static unsafe float GetConfigFloat(NetConfig type)
		{
			float single;
			float single1 = 0f;
			NetConfigType netConfigType = NetConfigType.Float;
			float* singlePointer = &single1;
			ulong num = (ulong)4;
			single = (SteamNetworkingUtils.Internal.GetConfigValue(type, NetScope.Global, (long)0, ref netConfigType, (IntPtr)singlePointer, ref num) == NetConfigResult.OK ? single1 : 0f);
			return single;
		}

		internal static unsafe bool GetConfigInt(NetConfig type, int value)
		{
			int* numPointer = &value;
			bool flag = SteamNetworkingUtils.Internal.SetConfigValue(type, NetScope.Global, (long)0, NetConfigType.Int32, (IntPtr)numPointer);
			return flag;
		}

		internal static unsafe int GetConfigInt(NetConfig type)
		{
			int num;
			int num1 = 0;
			NetConfigType netConfigType = NetConfigType.Int32;
			int* numPointer = &num1;
			ulong num2 = (ulong)4;
			num = (SteamNetworkingUtils.Internal.GetConfigValue(type, NetScope.Global, (long)0, ref netConfigType, (IntPtr)numPointer, ref num2) == NetConfigResult.OK ? num1 : 0);
			return num;
		}

		internal static unsafe bool SetConfigFloat(NetConfig type, float value)
		{
			float* singlePointer = &value;
			bool flag = SteamNetworkingUtils.Internal.SetConfigValue(type, NetScope.Global, (long)0, NetConfigType.Float, (IntPtr)singlePointer);
			return flag;
		}

		internal static unsafe bool SetConfigString(NetConfig type, string value)
		{
			// 
			// Current member / type: System.Boolean Steamworks.SteamNetworkingUtils::SetConfigString(Steamworks.Data.NetConfig,System.String)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean SetConfigString(Steamworks.Data.NetConfig,System.String)
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
			SteamNetworkingUtils._internal = null;
		}

		public static async Task WaitForPingDataAsync(float maxAgeInSeconds = 300f)
		{
			if (!SteamNetworkingUtils.Internal.CheckPingDataUpToDate(60f))
			{
				while (SteamNetworkingUtils.Internal.IsPingMeasurementInProgress())
				{
					await Task.Delay(10);
				}
			}
		}
	}
}