using Steamworks;
using System;
using System.Text;

namespace Steamworks.Data
{
	public struct Connection
	{
		internal uint Id;

		public string ConnectionName
		{
			get
			{
				string str;
				StringBuilder stringBuilder = Helpers.TakeStringBuilder();
				str = (SteamNetworkingSockets.Internal.GetConnectionName(this, stringBuilder, stringBuilder.Capacity) ? stringBuilder.ToString() : "ERROR");
				return str;
			}
			set
			{
				SteamNetworkingSockets.Internal.SetConnectionName(this, value);
			}
		}

		public long UserData
		{
			get
			{
				return SteamNetworkingSockets.Internal.GetConnectionUserData(this);
			}
			set
			{
				SteamNetworkingSockets.Internal.SetConnectionUserData(this, value);
			}
		}

		public Result Accept()
		{
			return SteamNetworkingSockets.Internal.AcceptConnection(this);
		}

		public bool Close(bool linger = false, int reasonCode = 0, string debugString = "Closing Connection")
		{
			return SteamNetworkingSockets.Internal.CloseConnection(this, reasonCode, debugString, linger);
		}

		public string DetailedStatus()
		{
			string str;
			StringBuilder stringBuilder = Helpers.TakeStringBuilder();
			if (SteamNetworkingSockets.Internal.GetDetailedConnectionStatus(this, stringBuilder, stringBuilder.Capacity) == 0)
			{
				str = stringBuilder.ToString();
			}
			else
			{
				str = null;
			}
			return str;
		}

		public Result Flush()
		{
			return SteamNetworkingSockets.Internal.FlushMessagesOnConnection(this);
		}

		public Result SendMessage(IntPtr ptr, int size, SendType sendType = 8)
		{
			return SteamNetworkingSockets.Internal.SendMessageToConnection(this, ptr, (uint)size, (int)sendType);
		}

		public unsafe Result SendMessage(byte[] data, SendType sendType = 8)
		{
			// 
			// Current member / type: Steamworks.Result Steamworks.Data.Connection::SendMessage(System.Byte[],Steamworks.Data.SendType)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: Steamworks.Result SendMessage(System.Byte[],Steamworks.Data.SendType)
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

		public unsafe Result SendMessage(byte[] data, int offset, int length, SendType sendType = 8)
		{
			// 
			// Current member / type: Steamworks.Result Steamworks.Data.Connection::SendMessage(System.Byte[],System.Int32,System.Int32,Steamworks.Data.SendType)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: Steamworks.Result SendMessage(System.Byte[],System.Int32,System.Int32,Steamworks.Data.SendType)
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

		public Result SendMessage(string str, SendType sendType = 8)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			return this.SendMessage(bytes, sendType);
		}

		public override string ToString()
		{
			return this.Id.ToString();
		}
	}
}