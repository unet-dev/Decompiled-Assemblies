using System;

namespace SteamNative
{
	internal struct HServerQuery
	{
		public int Value;

		public static implicit operator HServerQuery(int value)
		{
			return new HServerQuery()
			{
				Value = value
			};
		}

		public static implicit operator Int32(HServerQuery value)
		{
			return value.Value;
		}
	}
}