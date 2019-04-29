using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple=false)]
	[Preserve]
	public sealed class JsonPropertyAttribute : Attribute
	{
		internal Newtonsoft.Json.NullValueHandling? _nullValueHandling;

		internal Newtonsoft.Json.DefaultValueHandling? _defaultValueHandling;

		internal Newtonsoft.Json.ReferenceLoopHandling? _referenceLoopHandling;

		internal Newtonsoft.Json.ObjectCreationHandling? _objectCreationHandling;

		internal Newtonsoft.Json.TypeNameHandling? _typeNameHandling;

		internal bool? _isReference;

		internal int? _order;

		internal Newtonsoft.Json.Required? _required;

		internal bool? _itemIsReference;

		internal Newtonsoft.Json.ReferenceLoopHandling? _itemReferenceLoopHandling;

		internal Newtonsoft.Json.TypeNameHandling? _itemTypeNameHandling;

		public Newtonsoft.Json.DefaultValueHandling DefaultValueHandling
		{
			get
			{
				Newtonsoft.Json.DefaultValueHandling? nullable = this._defaultValueHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.DefaultValueHandling.Include;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._defaultValueHandling = new Newtonsoft.Json.DefaultValueHandling?(value);
			}
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

		public Newtonsoft.Json.ReferenceLoopHandling ItemReferenceLoopHandling
		{
			get
			{
				Newtonsoft.Json.ReferenceLoopHandling? nullable = this._itemReferenceLoopHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.ReferenceLoopHandling.Error;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._itemReferenceLoopHandling = new Newtonsoft.Json.ReferenceLoopHandling?(value);
			}
		}

		public Newtonsoft.Json.TypeNameHandling ItemTypeNameHandling
		{
			get
			{
				Newtonsoft.Json.TypeNameHandling? nullable = this._itemTypeNameHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.TypeNameHandling.None;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._itemTypeNameHandling = new Newtonsoft.Json.TypeNameHandling?(value);
			}
		}

		public Newtonsoft.Json.NullValueHandling NullValueHandling
		{
			get
			{
				Newtonsoft.Json.NullValueHandling? nullable = this._nullValueHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.NullValueHandling.Include;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._nullValueHandling = new Newtonsoft.Json.NullValueHandling?(value);
			}
		}

		public Newtonsoft.Json.ObjectCreationHandling ObjectCreationHandling
		{
			get
			{
				Newtonsoft.Json.ObjectCreationHandling? nullable = this._objectCreationHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.ObjectCreationHandling.Auto;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._objectCreationHandling = new Newtonsoft.Json.ObjectCreationHandling?(value);
			}
		}

		public int Order
		{
			get
			{
				int? nullable = this._order;
				if (!nullable.HasValue)
				{
					return 0;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._order = new int?(value);
			}
		}

		public string PropertyName
		{
			get;
			set;
		}

		public Newtonsoft.Json.ReferenceLoopHandling ReferenceLoopHandling
		{
			get
			{
				Newtonsoft.Json.ReferenceLoopHandling? nullable = this._referenceLoopHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.ReferenceLoopHandling.Error;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._referenceLoopHandling = new Newtonsoft.Json.ReferenceLoopHandling?(value);
			}
		}

		public Newtonsoft.Json.Required Required
		{
			get
			{
				Newtonsoft.Json.Required? nullable = this._required;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.Required.Default;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._required = new Newtonsoft.Json.Required?(value);
			}
		}

		public Newtonsoft.Json.TypeNameHandling TypeNameHandling
		{
			get
			{
				Newtonsoft.Json.TypeNameHandling? nullable = this._typeNameHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.TypeNameHandling.None;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._typeNameHandling = new Newtonsoft.Json.TypeNameHandling?(value);
			}
		}

		public JsonPropertyAttribute()
		{
		}

		public JsonPropertyAttribute(string propertyName)
		{
			this.PropertyName = propertyName;
		}
	}
}