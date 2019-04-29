using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Bson
{
	[Preserve]
	internal enum BsonType : sbyte
	{
		MinKey = -1,
		Number = 1,
		String = 2,
		Object = 3,
		Array = 4,
		Binary = 5,
		Undefined = 6,
		Oid = 7,
		Boolean = 8,
		Date = 9,
		Null = 10,
		Regex = 11,
		Reference = 12,
		Code = 13,
		Symbol = 14,
		CodeWScope = 15,
		Integer = 16,
		TimeStamp = 17,
		Long = 18,
		MaxKey = 127
	}
}