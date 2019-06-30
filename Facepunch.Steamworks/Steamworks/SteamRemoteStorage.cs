using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Steamworks
{
	public static class SteamRemoteStorage
	{
		private static ISteamRemoteStorage _internal;

		public static int FileCount
		{
			get
			{
				return SteamRemoteStorage.Internal.GetFileCount();
			}
		}

		public static IEnumerable<string> Files
		{
			get
			{
				int num = 0;
				for (int i = 0; i < SteamRemoteStorage.FileCount; i++)
				{
					string fileNameAndSize = SteamRemoteStorage.Internal.GetFileNameAndSize(i, ref num);
					yield return fileNameAndSize;
					fileNameAndSize = null;
				}
			}
		}

		internal static ISteamRemoteStorage Internal
		{
			get
			{
				if (SteamRemoteStorage._internal == null)
				{
					SteamRemoteStorage._internal = new ISteamRemoteStorage();
					SteamRemoteStorage._internal.Init();
				}
				return SteamRemoteStorage._internal;
			}
		}

		public static bool IsCloudEnabled
		{
			get
			{
				return (!SteamRemoteStorage.IsCloudEnabledForAccount ? false : SteamRemoteStorage.IsCloudEnabledForApp);
			}
		}

		public static bool IsCloudEnabledForAccount
		{
			get
			{
				return SteamRemoteStorage.Internal.IsCloudEnabledForAccount();
			}
		}

		public static bool IsCloudEnabledForApp
		{
			get
			{
				return SteamRemoteStorage.Internal.IsCloudEnabledForApp();
			}
			set
			{
				SteamRemoteStorage.Internal.SetCloudEnabledForApp(value);
			}
		}

		public static ulong QuotaBytes
		{
			get
			{
				ulong num = (ulong)0;
				ulong num1 = (ulong)0;
				SteamRemoteStorage.Internal.GetQuota(ref num, ref num1);
				return num;
			}
		}

		public static ulong QuotaRemainingBytes
		{
			get
			{
				ulong num = (ulong)0;
				ulong num1 = (ulong)0;
				SteamRemoteStorage.Internal.GetQuota(ref num, ref num1);
				return num1;
			}
		}

		public static ulong QuotaUsedBytes
		{
			get
			{
				ulong num = (ulong)0;
				ulong num1 = (ulong)0;
				SteamRemoteStorage.Internal.GetQuota(ref num, ref num1);
				return num - num1;
			}
		}

		public static bool FileDelete(string filename)
		{
			return SteamRemoteStorage.Internal.FileDelete(filename);
		}

		public static bool FileExists(string filename)
		{
			return SteamRemoteStorage.Internal.FileExists(filename);
		}

		public static bool FileForget(string filename)
		{
			return SteamRemoteStorage.Internal.FileForget(filename);
		}

		public static bool FilePersisted(string filename)
		{
			return SteamRemoteStorage.Internal.FilePersisted(filename);
		}

		public static unsafe byte[] FileRead(string filename)
		{
			// 
			// Current member / type: System.Byte[] Steamworks.SteamRemoteStorage::FileRead(System.String)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Byte[] FileRead(System.String)
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

		public static int FileSize(string filename)
		{
			return SteamRemoteStorage.Internal.GetFileSize(filename);
		}

		public static DateTime FileTime(string filename)
		{
			return Epoch.ToDateTime(SteamRemoteStorage.Internal.GetFileTimestamp(filename));
		}

		public static unsafe bool FileWrite(string filename, byte[] data)
		{
			// 
			// Current member / type: System.Boolean Steamworks.SteamRemoteStorage::FileWrite(System.String,System.Byte[])
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean FileWrite(System.String,System.Byte[])
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
			SteamRemoteStorage._internal = null;
		}
	}
}