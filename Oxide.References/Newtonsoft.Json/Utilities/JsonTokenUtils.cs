using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Utilities
{
	[Preserve]
	internal static class JsonTokenUtils
	{
		internal static bool IsEndToken(JsonToken token)
		{
			switch (token)
			{
				case JsonToken.EndObject:
				case JsonToken.EndArray:
				case JsonToken.EndConstructor:
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsPrimitiveToken(JsonToken token)
		{
			switch (token)
			{
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.String:
				case JsonToken.Boolean:
				case JsonToken.Null:
				case JsonToken.Undefined:
				case JsonToken.Date:
				case JsonToken.Bytes:
				{
					return true;
				}
				case JsonToken.EndObject:
				case JsonToken.EndArray:
				case JsonToken.EndConstructor:
				{
					return false;
				}
				default:
				{
					return false;
				}
			}
		}

		internal static bool IsStartToken(JsonToken token)
		{
			switch (token)
			{
				case JsonToken.StartObject:
				case JsonToken.StartArray:
				case JsonToken.StartConstructor:
				{
					return true;
				}
			}
			return false;
		}
	}
}