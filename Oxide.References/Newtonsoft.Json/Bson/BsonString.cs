using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Bson
{
	[Preserve]
	internal class BsonString : BsonValue
	{
		public int ByteCount
		{
			get;
			set;
		}

		public bool IncludeLength
		{
			get;
			set;
		}

		public BsonString(object value, bool includeLength) : base(value, BsonType.String)
		{
			this.IncludeLength = includeLength;
		}
	}
}