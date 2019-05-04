using System;

namespace Rust
{
	public static class Protocol
	{
		public const int network = 2164;

		public const int save = 178;

		public const int report = 1;

		public const int persistance = 3;

		public const int storage = 0;

		public static string printable
		{
			get
			{
				return string.Concat(new object[] { 2164, ".", 178, ".", 1 });
			}
		}
	}
}