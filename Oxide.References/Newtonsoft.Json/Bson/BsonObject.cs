using Newtonsoft.Json.Shims;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Bson
{
	[Preserve]
	internal class BsonObject : BsonToken, IEnumerable<BsonProperty>, IEnumerable
	{
		private readonly List<BsonProperty> _children = new List<BsonProperty>();

		public override BsonType Type
		{
			get
			{
				return BsonType.Object;
			}
		}

		public BsonObject()
		{
		}

		public void Add(string name, BsonToken token)
		{
			this._children.Add(new BsonProperty()
			{
				Name = new BsonString(name, false),
				Value = token
			});
			token.Parent = this;
		}

		public IEnumerator<BsonProperty> GetEnumerator()
		{
			return this._children.GetEnumerator();
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}