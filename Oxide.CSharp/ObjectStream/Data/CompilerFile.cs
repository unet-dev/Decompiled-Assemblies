using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace ObjectStream.Data
{
	[Serializable]
	internal class CompilerFile
	{
		public byte[] Data
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		internal CompilerFile(string name, byte[] data)
		{
			this.Name = name;
			this.Data = data;
		}

		internal CompilerFile(string directory, string name)
		{
			this.Name = name;
			this.Data = File.ReadAllBytes(Path.Combine(directory, this.Name));
		}

		internal CompilerFile(string path)
		{
			this.Name = Path.GetFileName(path);
			this.Data = File.ReadAllBytes(path);
		}
	}
}