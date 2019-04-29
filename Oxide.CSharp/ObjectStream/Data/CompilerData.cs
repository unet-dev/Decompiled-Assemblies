using System;
using System.Runtime.CompilerServices;

namespace ObjectStream.Data
{
	[Serializable]
	internal class CompilerData
	{
		public bool LoadDefaultReferences
		{
			get;
			set;
		}

		public string OutputFile
		{
			get;
			set;
		}

		public CompilerPlatform Platform
		{
			get;
			set;
		}

		public CompilerFile[] ReferenceFiles
		{
			get;
			set;
		}

		public string SdkVersion
		{
			get;
			set;
		}

		public CompilerFile[] SourceFiles
		{
			get;
			set;
		}

		public bool StdLib
		{
			get;
			set;
		}

		public CompilerTarget Target
		{
			get;
			set;
		}

		public CompilerLanguageVersion Version
		{
			get;
			set;
		}

		public CompilerData()
		{
			this.StdLib = false;
			this.Target = CompilerTarget.Library;
			this.Platform = CompilerPlatform.AnyCPU;
			this.Version = CompilerLanguageVersion.V_6;
			this.LoadDefaultReferences = false;
			this.SdkVersion = "2";
		}
	}
}