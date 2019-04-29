using SteamNative;
using System;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public class Image
	{
		public byte[] Data
		{
			get;
			internal set;
		}

		public int Height
		{
			get;
			internal set;
		}

		public int Id
		{
			get;
			internal set;
		}

		public bool IsError
		{
			get;
			internal set;
		}

		public bool IsLoaded
		{
			get;
			internal set;
		}

		public int Width
		{
			get;
			internal set;
		}

		public Image()
		{
		}

		public Color GetPixel(int x, int y)
		{
			if (!this.IsLoaded)
			{
				throw new Exception("Image not loaded");
			}
			if (x < 0 || x >= this.Width)
			{
				throw new Exception("x out of bounds");
			}
			if (y < 0 || y >= this.Height)
			{
				throw new Exception("y out of bounds");
			}
			Color data = new Color();
			int num = (y * this.Width + x) * 4;
			data.r = this.Data[num];
			data.g = this.Data[num + 1];
			data.b = this.Data[num + 2];
			data.a = this.Data[num + 3];
			return data;
		}

		internal unsafe bool TryLoad(SteamUtils utils)
		{
			// 
			// Current member / type: System.Boolean Facepunch.Steamworks.Image::TryLoad(SteamNative.SteamUtils)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean TryLoad(SteamNative.SteamUtils)
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
	}
}