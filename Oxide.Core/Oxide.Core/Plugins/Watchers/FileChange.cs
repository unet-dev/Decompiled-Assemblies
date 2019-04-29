using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Plugins.Watchers
{
	public sealed class FileChange
	{
		public WatcherChangeTypes ChangeType
		{
			get;
			private set;
		}

		public string Name
		{
			get;
			private set;
		}

		public FileChange(string name, WatcherChangeTypes changeType)
		{
			this.Name = name;
			this.ChangeType = changeType;
		}
	}
}