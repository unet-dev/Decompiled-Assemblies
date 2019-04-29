using Oxide.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Oxide.Plugins
{
	[AttributeUsage(AttributeTargets.Class)]
	public class InfoAttribute : Attribute
	{
		public string Author
		{
			get;
		}

		public int ResourceId
		{
			get;
			set;
		}

		public string Title
		{
			get;
		}

		public VersionNumber Version
		{
			get;
			private set;
		}

		public InfoAttribute(string Title, string Author, string Version)
		{
			this.Title = Title;
			this.Author = Author;
			this.SetVersion(Version);
		}

		public InfoAttribute(string Title, string Author, double Version)
		{
			this.Title = Title;
			this.Author = Author;
			this.SetVersion(Version.ToString());
		}

		private void SetVersion(string version)
		{
			List<ushort> list = version.Split(new char[] { '.' }).Select<string, ushort>((string part) => {
				ushort num;
				if (!ushort.TryParse(part, out num))
				{
					num = 0;
				}
				return num;
			}).ToList<ushort>();
			while (list.Count < 3)
			{
				list.Add(0);
			}
			if (list.Count > 3)
			{
				Interface.Oxide.LogWarning(string.Concat(new string[] { "Version `", version, "` is invalid for ", this.Title, ", should be `major.minor.patch`" }), Array.Empty<object>());
			}
			this.Version = new VersionNumber((int)list[0], (int)list[1], (int)list[2]);
		}
	}
}