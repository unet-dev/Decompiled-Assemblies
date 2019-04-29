using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Bson
{
	[Preserve]
	internal class BsonProperty
	{
		public BsonString Name
		{
			get;
			set;
		}

		public BsonToken Value
		{
			get;
			set;
		}

		public BsonProperty()
		{
		}
	}
}