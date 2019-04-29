using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Converters
{
	[Preserve]
	public abstract class DateTimeConverterBase : JsonConverter
	{
		protected DateTimeConverterBase()
		{
		}

		public override bool CanConvert(Type objectType)
		{
			if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
			{
				return true;
			}
			if (objectType != typeof(DateTimeOffset) && objectType != typeof(DateTimeOffset?))
			{
				return false;
			}
			return true;
		}
	}
}