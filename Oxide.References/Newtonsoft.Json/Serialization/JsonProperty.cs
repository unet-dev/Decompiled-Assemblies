using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class JsonProperty
	{
		internal Newtonsoft.Json.Required? _required;

		internal bool _hasExplicitDefaultValue;

		private object _defaultValue;

		private bool _hasGeneratedDefaultValue;

		private string _propertyName;

		internal bool _skipPropertyNameEscape;

		private Type _propertyType;

		public IAttributeProvider AttributeProvider
		{
			get;
			set;
		}

		public JsonConverter Converter
		{
			get;
			set;
		}

		public Type DeclaringType
		{
			get;
			set;
		}

		public object DefaultValue
		{
			get
			{
				if (!this._hasExplicitDefaultValue)
				{
					return null;
				}
				return this._defaultValue;
			}
			set
			{
				this._hasExplicitDefaultValue = true;
				this._defaultValue = value;
			}
		}

		public Newtonsoft.Json.DefaultValueHandling? DefaultValueHandling
		{
			get;
			set;
		}

		public Predicate<object> GetIsSpecified
		{
			get;
			set;
		}

		public bool HasMemberAttribute
		{
			get;
			set;
		}

		public bool Ignored
		{
			get;
			set;
		}

		public bool? IsReference
		{
			get;
			set;
		}

		public JsonConverter ItemConverter
		{
			get;
			set;
		}

		public bool? ItemIsReference
		{
			get;
			set;
		}

		public Newtonsoft.Json.ReferenceLoopHandling? ItemReferenceLoopHandling
		{
			get;
			set;
		}

		public Newtonsoft.Json.TypeNameHandling? ItemTypeNameHandling
		{
			get;
			set;
		}

		public JsonConverter MemberConverter
		{
			get;
			set;
		}

		public Newtonsoft.Json.NullValueHandling? NullValueHandling
		{
			get;
			set;
		}

		public Newtonsoft.Json.ObjectCreationHandling? ObjectCreationHandling
		{
			get;
			set;
		}

		public int? Order
		{
			get;
			set;
		}

		internal JsonContract PropertyContract
		{
			get;
			set;
		}

		public string PropertyName
		{
			get
			{
				return this._propertyName;
			}
			set
			{
				this._propertyName = value;
				this._skipPropertyNameEscape = !JavaScriptUtils.ShouldEscapeJavaScriptString(this._propertyName, JavaScriptUtils.HtmlCharEscapeFlags);
			}
		}

		public Type PropertyType
		{
			get
			{
				return this._propertyType;
			}
			set
			{
				if (this._propertyType != value)
				{
					this._propertyType = value;
					this._hasGeneratedDefaultValue = false;
				}
			}
		}

		public bool Readable
		{
			get;
			set;
		}

		public Newtonsoft.Json.ReferenceLoopHandling? ReferenceLoopHandling
		{
			get;
			set;
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

		public Action<object, object> SetIsSpecified
		{
			get;
			set;
		}

		public Predicate<object> ShouldDeserialize
		{
			get;
			set;
		}

		public Predicate<object> ShouldSerialize
		{
			get;
			set;
		}

		public Newtonsoft.Json.TypeNameHandling? TypeNameHandling
		{
			get;
			set;
		}

		public string UnderlyingName
		{
			get;
			set;
		}

		public IValueProvider ValueProvider
		{
			get;
			set;
		}

		public bool Writable
		{
			get;
			set;
		}

		public JsonProperty()
		{
		}

		internal object GetResolvedDefaultValue()
		{
			if (this._propertyType == null)
			{
				return null;
			}
			if (!this._hasExplicitDefaultValue && !this._hasGeneratedDefaultValue)
			{
				this._defaultValue = ReflectionUtils.GetDefaultValue(this.PropertyType);
				this._hasGeneratedDefaultValue = true;
			}
			return this._defaultValue;
		}

		public override string ToString()
		{
			return this.PropertyName;
		}

		internal void WritePropertyName(JsonWriter writer)
		{
			if (!this._skipPropertyNameEscape)
			{
				writer.WritePropertyName(this.PropertyName);
				return;
			}
			writer.WritePropertyName(this.PropertyName, false);
		}
	}
}