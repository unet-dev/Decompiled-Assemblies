using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Bson
{
	[Preserve]
	internal class BsonValue : BsonToken
	{
		private readonly object _value;

		private readonly BsonType _type;

		public override BsonType Type
		{
			get
			{
				return this._type;
			}
		}

		public object Value
		{
			get
			{
				return this._value;
			}
		}

		public BsonValue(object value, BsonType type)
		{
			this._value = value;
			this._type = type;
		}
	}
}