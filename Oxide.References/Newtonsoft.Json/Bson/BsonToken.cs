using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Bson
{
	[Preserve]
	internal abstract class BsonToken
	{
		public int CalculatedSize
		{
			get;
			set;
		}

		public BsonToken Parent
		{
			get;
			set;
		}

		public abstract BsonType Type
		{
			get;
		}

		protected BsonToken()
		{
		}
	}
}