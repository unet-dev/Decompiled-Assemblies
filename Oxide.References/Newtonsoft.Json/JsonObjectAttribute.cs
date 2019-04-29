using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple=false)]
	[Preserve]
	public sealed class JsonObjectAttribute : JsonContainerAttribute
	{
		private Newtonsoft.Json.MemberSerialization _memberSerialization;

		internal Required? _itemRequired;

		public Required ItemRequired
		{
			get
			{
				Required? nullable = this._itemRequired;
				if (!nullable.HasValue)
				{
					return Required.Default;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._itemRequired = new Required?(value);
			}
		}

		public Newtonsoft.Json.MemberSerialization MemberSerialization
		{
			get
			{
				return this._memberSerialization;
			}
			set
			{
				this._memberSerialization = value;
			}
		}

		public JsonObjectAttribute()
		{
		}

		public JsonObjectAttribute(Newtonsoft.Json.MemberSerialization memberSerialization)
		{
			this.MemberSerialization = memberSerialization;
		}

		public JsonObjectAttribute(string id) : base(id)
		{
		}
	}
}