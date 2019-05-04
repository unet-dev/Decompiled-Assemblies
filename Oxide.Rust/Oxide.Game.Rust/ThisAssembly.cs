using System;

namespace Oxide.Game.Rust
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

			public const string Commit = "a656648";

			public const string Sha = "a6566487c6bf74d311c9d74b02c2a6610be4d6c1";

			public const string Commits = "1";

			public const string Tag = "";

			public const string BaseTag = "";

			public Git()
			{
			}

			public class BaseVersion
			{
				public const string Major = "0";

				public const string Minor = "0";

				public const string Patch = "0";

				public BaseVersion()
				{
				}
			}

			public class SemVer
			{
				public const string Major = "0";

				public const string Minor = "0";

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