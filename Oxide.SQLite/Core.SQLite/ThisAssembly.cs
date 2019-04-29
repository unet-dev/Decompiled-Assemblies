using System;

namespace Oxide.Core.SQLite
{
	internal class ThisAssembly
	{
		public ThisAssembly()
		{
		}

		public class Git
		{
			public const bool IsDirty = true;

			public const string IsDirtyString = "true";

			public const string Branch = "master";

			public const string Commit = "ff4062f";

			public const string Sha = "ff4062fe883133187d32116c3a1793509c0485a7";

			public const string Commits = "1";

			public const string Tag = "";

			public const string BaseTag = "";

			public Git()
			{
			}

			public class BaseVersion
			{
				public const string Major = "0";

				public const string Minor = "1";

				public const string Patch = "0";

				public BaseVersion()
				{
				}
			}

			public class SemVer
			{
				public const string Major = "0";

				public const string Minor = "1";

				public const string Patch = "1";

				public const string Label = "";

				public const string DashLabel = "";

				public const string Source = "Default";

				public SemVer()
				{
				}
			}
		}
	}
}