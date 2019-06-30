using Steamworks.Data;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Steamworks
{
	public static class SteamScreenshots
	{
		private static ISteamScreenshots _internal;

		public static bool Hooked
		{
			get
			{
				return SteamScreenshots.Internal.IsScreenshotsHooked();
			}
			set
			{
				SteamScreenshots.Internal.HookScreenshots(value);
			}
		}

		internal static ISteamScreenshots Internal
		{
			get
			{
				if (SteamScreenshots._internal == null)
				{
					SteamScreenshots._internal = new ISteamScreenshots();
					SteamScreenshots._internal.Init();
				}
				return SteamScreenshots._internal;
			}
		}

		public static Screenshot? AddScreenshot(string filename, string thumbnail, int width, int height)
		{
			Screenshot? nullable;
			ScreenshotHandle library = SteamScreenshots.Internal.AddScreenshotToLibrary(filename, thumbnail, width, height);
			if (library.Value != 0)
			{
				nullable = new Screenshot?(new Screenshot()
				{
					Value = library
				});
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		internal static void InstallEvents()
		{
			ScreenshotRequested_t.Install((ScreenshotRequested_t x) => {
				Action onScreenshotRequested = SteamScreenshots.OnScreenshotRequested;
				if (onScreenshotRequested != null)
				{
					onScreenshotRequested();
				}
				else
				{
				}
			}, false);
			ScreenshotReady_t.Install((ScreenshotReady_t x) => {
				if (x.Result == Result.OK)
				{
					Action<Screenshot> onScreenshotReady = SteamScreenshots.OnScreenshotReady;
					if (onScreenshotReady != null)
					{
						onScreenshotReady(new Screenshot()
						{
							Value = x.Local
						});
					}
					else
					{
					}
				}
				else
				{
					Action<Result> onScreenshotFailed = SteamScreenshots.OnScreenshotFailed;
					if (onScreenshotFailed != null)
					{
						onScreenshotFailed(x.Result);
					}
					else
					{
					}
				}
			}, false);
		}

		internal static void Shutdown()
		{
			SteamScreenshots._internal = null;
		}

		public static void TriggerScreenshot()
		{
			SteamScreenshots.Internal.TriggerScreenshot();
		}

		public static unsafe Screenshot? WriteScreenshot(byte[] data, int width, int height)
		{
			// 
			// Current member / type: System.Nullable`1<Steamworks.Data.Screenshot> Steamworks.SteamScreenshots::WriteScreenshot(System.Byte[],System.Int32,System.Int32)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Nullable<Steamworks.Data.Screenshot> WriteScreenshot(System.Byte[],System.Int32,System.Int32)
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

		public static event Action<Result> OnScreenshotFailed;

		public static event Action<Screenshot> OnScreenshotReady;

		public static event Action OnScreenshotRequested;
	}
}