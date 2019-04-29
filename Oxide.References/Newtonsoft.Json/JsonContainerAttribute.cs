using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple=false)]
	[Preserve]
	public abstract class JsonContainerAttribute : Attribute
	{
		internal bool? _isReference;

		internal bool? _itemIsReference;

		internal ReferenceLoopHandling? _itemReferenceLoopHandling;

		internal TypeNameHandling? _itemTypeNameHandling;

		public string Description
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public bool IsReference
		{
			get
			{
				bool? nullable = this._isReference;
				if (!nullable.HasValue)
				{
					return false;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._isReference = new bool?(value);
			}
		}

		public object[] ItemConverterParameters
		{
			get;
			set;
		}

		public Type ItemConverterType
		{
			get;
			set;
		}

		public bool ItemIsReference
		{
			get
			{
				bool? nullable = this._itemIsReference;
				if (!nullable.HasValue)
				{
					return false;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._itemIsReference = new bool?(value);
			}
		}

		public ReferenceLoopHandling ItemReferenceLoopHandling
		{
			get
			{
				ReferenceLoopHandling? nullable = this._itemReferenceLoopHandling;
				if (!nullable.HasValue)
				{
					return ReferenceLoopHandling.Error;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._itemReferenceLoopHandling = new ReferenceLoopHandling?(value);
			}
		}

		public TypeNameHandling ItemTypeNameHandling
		{
			get
			{
				TypeNameHandling? nullable = this._itemTypeNameHandling;
				if (!nullable.HasValue)
				{
					return TypeNameHandling.None;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._itemTypeNameHandling = new TypeNameHandling?(value);
			}
		}

		public string Title
		{
			get;
			set;
		}

		protected JsonContainerAttribute()
		{
		}

		protected JsonContainerAttribute(string id)
		{
			this.Id = id;
		}
	}
}