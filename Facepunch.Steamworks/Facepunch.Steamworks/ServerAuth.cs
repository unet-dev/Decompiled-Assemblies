using Facepunch.Steamworks.Interop;
using SteamNative;
using System;

namespace Facepunch.Steamworks
{
	public class ServerAuth
	{
		internal Server server;

		public Action<ulong, ulong, ServerAuth.Status> OnAuthChange;

		internal ServerAuth(Server s)
		{
			this.server = s;
			this.server.RegisterCallback<ValidateAuthTicketResponse_t>(new Action<ValidateAuthTicketResponse_t>(this.OnAuthTicketValidate));
		}

		public void EndSession(ulong steamid)
		{
			this.server.native.gameServer.EndAuthSession(steamid);
		}

		private void OnAuthTicketValidate(ValidateAuthTicketResponse_t data)
		{
			if (this.OnAuthChange != null)
			{
				this.OnAuthChange(data.SteamID, data.OwnerSteamID, data.AuthSessionResponse);
			}
		}

		public unsafe bool StartSession(byte[] data, ulong steamid)
		{
			// 
			// Current member / type: System.Boolean Facepunch.Steamworks.ServerAuth::StartSession(System.Byte[],System.UInt64)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean StartSession(System.Byte[],System.UInt64)
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

		public enum Status
		{
			OK,
			UserNotConnectedToSteam,
			NoLicenseOrExpired,
			VACBanned,
			LoggedInElseWhere,
			VACCheckTimedOut,
			AuthTicketCanceled,
			AuthTicketInvalidAlreadyUsed,
			AuthTicketInvalid,
			PublisherIssuedBan
		}
	}
}