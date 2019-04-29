using Facepunch.Steamworks;
using System;
using System.Reflection;

namespace SteamNative
{
	internal class CallResult<T> : CallResult
	{
		private static byte[] resultBuffer;

		private Action<T, bool> CallbackFunction;

		private CallResult<T>.ConvertFromPointer ConvertFromPointerFunction;

		internal int ResultSize;

		internal int CallbackId;

		static CallResult()
		{
			CallResult<T>.resultBuffer = new byte[16384];
		}

		internal CallResult(BaseSteamworks steamworks, SteamAPICall_t call, Action<T, bool> callbackFunction, CallResult<T>.ConvertFromPointer fromPointer, int resultSize, int callbackId) : base(steamworks, call)
		{
			this.ResultSize = resultSize;
			this.CallbackId = callbackId;
			this.CallbackFunction = callbackFunction;
			this.ConvertFromPointerFunction = fromPointer;
			this.Steamworks.RegisterCallResult(this);
		}

		internal override unsafe void RunCallback()
		{
			// 
			// Current member / type: System.Void SteamNative.CallResult`1::RunCallback()
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void RunCallback()
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

		public override string ToString()
		{
			return string.Format("CallResult( {0}, {1}, {2}b )", typeof(T).Name, this.CallbackId, this.ResultSize);
		}

		internal delegate T ConvertFromPointer(IntPtr p);
	}
}