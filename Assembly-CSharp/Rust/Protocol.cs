using System;

namespace Rust
{
	public static class Protocol
	{
		public const int network = 2163;

		public const int save = 177;

		public const int report = 1;

		public const int persistance = 3;

		public const int storage = 0;

		public static string printable
		{
			get
			{
				return string.Concat(new object[] { 2163, ".", 177, ".", 1 });
			}
		}
	}
}