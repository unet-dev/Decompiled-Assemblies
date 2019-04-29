using System;

namespace Rust
{
	public static class Defines
	{
		public static uint appID;

		public const string resourceFolder = "assets/bundled";

		static Defines()
		{
			Defines.appID = 252490;
		}

		public static class Connection
		{
			public const byte mode_steam = 228;
		}

		public static class Tags
		{
			public const string NotPlayerUsable = "Not Player Usable";

			public const string MeshColliderBatch = "MeshColliderBatch";
		}
	}
}