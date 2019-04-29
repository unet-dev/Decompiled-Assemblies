using Oxide.Core;
using Oxide.Core.Libraries;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Libraries.Covalence
{
	public class SaveInfo
	{
		private readonly Time time = Interface.Oxide.GetLibrary<Time>(null);

		private readonly string FullPath;

		public DateTime CreationTime
		{
			get;
			private set;
		}

		public uint CreationTimeUnix
		{
			get;
			private set;
		}

		public string SaveName
		{
			get;
			private set;
		}

		private SaveInfo(string filepath)
		{
			this.FullPath = filepath;
			this.SaveName = Utility.GetFileNameWithoutExtension(filepath);
			this.Refresh();
		}

		public static SaveInfo Create(string filepath)
		{
			if (!File.Exists(filepath))
			{
				return null;
			}
			return new SaveInfo(filepath);
		}

		public void Refresh()
		{
			if (!File.Exists(this.FullPath))
			{
				return;
			}
			this.CreationTime = File.GetCreationTime(this.FullPath);
			this.CreationTimeUnix = this.time.GetUnixFromDateTime(this.CreationTime);
		}
	}
}