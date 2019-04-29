using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Shims;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace Newtonsoft.Json
{
	[Preserve]
	public class JsonSerializerSettings
	{
		internal const Newtonsoft.Json.ReferenceLoopHandling DefaultReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;

		internal const Newtonsoft.Json.MissingMemberHandling DefaultMissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;

		internal const Newtonsoft.Json.NullValueHandling DefaultNullValueHandling = Newtonsoft.Json.NullValueHandling.Include;

		internal const Newtonsoft.Json.DefaultValueHandling DefaultDefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;

		internal const Newtonsoft.Json.ObjectCreationHandling DefaultObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Auto;

		internal const Newtonsoft.Json.PreserveReferencesHandling DefaultPreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;

		internal const Newtonsoft.Json.ConstructorHandling DefaultConstructorHandling = Newtonsoft.Json.ConstructorHandling.Default;

		internal const Newtonsoft.Json.TypeNameHandling DefaultTypeNameHandling = Newtonsoft.Json.TypeNameHandling.None;

		internal const Newtonsoft.Json.MetadataPropertyHandling DefaultMetadataPropertyHandling = Newtonsoft.Json.MetadataPropertyHandling.Default;

		internal const FormatterAssemblyStyle DefaultTypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;

		internal readonly static StreamingContext DefaultContext;

		internal const Newtonsoft.Json.Formatting DefaultFormatting = Newtonsoft.Json.Formatting.None;

		internal const Newtonsoft.Json.DateFormatHandling DefaultDateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;

		internal const Newtonsoft.Json.DateTimeZoneHandling DefaultDateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;

		internal const Newtonsoft.Json.DateParseHandling DefaultDateParseHandling = Newtonsoft.Json.DateParseHandling.DateTime;

		internal const Newtonsoft.Json.FloatParseHandling DefaultFloatParseHandling = Newtonsoft.Json.FloatParseHandling.Double;

		internal const Newtonsoft.Json.FloatFormatHandling DefaultFloatFormatHandling = Newtonsoft.Json.FloatFormatHandling.String;

		internal const Newtonsoft.Json.StringEscapeHandling DefaultStringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.Default;

		internal const FormatterAssemblyStyle DefaultFormatterAssemblyStyle = FormatterAssemblyStyle.Simple;

		internal readonly static CultureInfo DefaultCulture;

		internal const bool DefaultCheckAdditionalContent = false;

		internal const string DefaultDateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

		internal Newtonsoft.Json.Formatting? _formatting;

		internal Newtonsoft.Json.DateFormatHandling? _dateFormatHandling;

		internal Newtonsoft.Json.DateTimeZoneHandling? _dateTimeZoneHandling;

		internal Newtonsoft.Json.DateParseHandling? _dateParseHandling;

		internal Newtonsoft.Json.FloatFormatHandling? _floatFormatHandling;

		internal Newtonsoft.Json.FloatParseHandling? _floatParseHandling;

		internal Newtonsoft.Json.StringEscapeHandling? _stringEscapeHandling;

		internal CultureInfo _culture;

		internal bool? _checkAdditionalContent;

		internal int? _maxDepth;

		internal bool _maxDepthSet;

		internal string _dateFormatString;

		internal bool _dateFormatStringSet;

		internal FormatterAssemblyStyle? _typeNameAssemblyFormat;

		internal Newtonsoft.Json.DefaultValueHandling? _defaultValueHandling;

		internal Newtonsoft.Json.PreserveReferencesHandling? _preserveReferencesHandling;

		internal Newtonsoft.Json.NullValueHandling? _nullValueHandling;

		internal Newtonsoft.Json.ObjectCreationHandling? _objectCreationHandling;

		internal Newtonsoft.Json.MissingMemberHandling? _missingMemberHandling;

		internal Newtonsoft.Json.ReferenceLoopHandling? _referenceLoopHandling;

		internal StreamingContext? _context;

		internal Newtonsoft.Json.ConstructorHandling? _constructorHandling;

		internal Newtonsoft.Json.TypeNameHandling? _typeNameHandling;

		internal Newtonsoft.Json.MetadataPropertyHandling? _metadataPropertyHandling;

		public SerializationBinder Binder
		{
			get;
			set;
		}

		public bool CheckAdditionalContent
		{
			get
			{
				bool? nullable = this._checkAdditionalContent;
				if (!nullable.HasValue)
				{
					return false;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._checkAdditionalContent = new bool?(value);
			}
		}

		public Newtonsoft.Json.ConstructorHandling ConstructorHandling
		{
			get
			{
				Newtonsoft.Json.ConstructorHandling? nullable = this._constructorHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.ConstructorHandling.Default;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._constructorHandling = new Newtonsoft.Json.ConstructorHandling?(value);
			}
		}

		public StreamingContext Context
		{
			get
			{
				StreamingContext? nullable = this._context;
				if (!nullable.HasValue)
				{
					return JsonSerializerSettings.DefaultContext;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._context = new StreamingContext?(value);
			}
		}

		public IContractResolver ContractResolver
		{
			get;
			set;
		}

		public IList<JsonConverter> Converters
		{
			get;
			set;
		}

		public CultureInfo Culture
		{
			get
			{
				return this._culture ?? JsonSerializerSettings.DefaultCulture;
			}
			set
			{
				this._culture = value;
			}
		}

		public Newtonsoft.Json.DateFormatHandling DateFormatHandling
		{
			get
			{
				Newtonsoft.Json.DateFormatHandling? nullable = this._dateFormatHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._dateFormatHandling = new Newtonsoft.Json.DateFormatHandling?(value);
			}
		}

		public string DateFormatString
		{
			get
			{
				return this._dateFormatString ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
			}
			set
			{
				this._dateFormatString = value;
				this._dateFormatStringSet = true;
			}
		}

		public Newtonsoft.Json.DateParseHandling DateParseHandling
		{
			get
			{
				Newtonsoft.Json.DateParseHandling? nullable = this._dateParseHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.DateParseHandling.DateTime;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._dateParseHandling = new Newtonsoft.Json.DateParseHandling?(value);
			}
		}

		public Newtonsoft.Json.DateTimeZoneHandling DateTimeZoneHandling
		{
			get
			{
				Newtonsoft.Json.DateTimeZoneHandling? nullable = this._dateTimeZoneHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._dateTimeZoneHandling = new Newtonsoft.Json.DateTimeZoneHandling?(value);
			}
		}

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

		public IEqualityComparer EqualityComparer
		{
			get;
			set;
		}

		public EventHandler<ErrorEventArgs> Error
		{
			get;
			set;
		}

		public Newtonsoft.Json.FloatFormatHandling FloatFormatHandling
		{
			get
			{
				Newtonsoft.Json.FloatFormatHandling? nullable = this._floatFormatHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.FloatFormatHandling.String;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._floatFormatHandling = new Newtonsoft.Json.FloatFormatHandling?(value);
			}
		}

		public Newtonsoft.Json.FloatParseHandling FloatParseHandling
		{
			get
			{
				Newtonsoft.Json.FloatParseHandling? nullable = this._floatParseHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.FloatParseHandling.Double;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._floatParseHandling = new Newtonsoft.Json.FloatParseHandling?(value);
			}
		}

		public Newtonsoft.Json.Formatting Formatting
		{
			get
			{
				Newtonsoft.Json.Formatting? nullable = this._formatting;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.Formatting.None;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._formatting = new Newtonsoft.Json.Formatting?(value);
			}
		}

		public int? MaxDepth
		{
			get
			{
				return this._maxDepth;
			}
			set
			{
				int? nullable = value;
				if ((nullable.GetValueOrDefault() <= 0 ? nullable.HasValue : false))
				{
					throw new ArgumentException("Value must be positive.", "value");
				}
				this._maxDepth = value;
				this._maxDepthSet = true;
			}
		}

		public Newtonsoft.Json.MetadataPropertyHandling MetadataPropertyHandling
		{
			get
			{
				Newtonsoft.Json.MetadataPropertyHandling? nullable = this._metadataPropertyHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.MetadataPropertyHandling.Default;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._metadataPropertyHandling = new Newtonsoft.Json.MetadataPropertyHandling?(value);
			}
		}

		public Newtonsoft.Json.MissingMemberHandling MissingMemberHandling
		{
			get
			{
				Newtonsoft.Json.MissingMemberHandling? nullable = this._missingMemberHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.MissingMemberHandling.Ignore;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._missingMemberHandling = new Newtonsoft.Json.MissingMemberHandling?(value);
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

		public Newtonsoft.Json.PreserveReferencesHandling PreserveReferencesHandling
		{
			get
			{
				Newtonsoft.Json.PreserveReferencesHandling? nullable = this._preserveReferencesHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.PreserveReferencesHandling.None;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._preserveReferencesHandling = new Newtonsoft.Json.PreserveReferencesHandling?(value);
			}
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

		[Obsolete("ReferenceResolver property is obsolete. Use the ReferenceResolverProvider property to set the IReferenceResolver: settings.ReferenceResolverProvider = () => resolver")]
		public IReferenceResolver ReferenceResolver
		{
			get
			{
				if (this.ReferenceResolverProvider == null)
				{
					return null;
				}
				return this.ReferenceResolverProvider();
			}
			set
			{
				Func<IReferenceResolver> func;
				if (value != null)
				{
					func = () => value;
				}
				else
				{
					func = null;
				}
				this.ReferenceResolverProvider = func;
			}
		}

		public Func<IReferenceResolver> ReferenceResolverProvider
		{
			get;
			set;
		}

		public Newtonsoft.Json.StringEscapeHandling StringEscapeHandling
		{
			get
			{
				Newtonsoft.Json.StringEscapeHandling? nullable = this._stringEscapeHandling;
				if (!nullable.HasValue)
				{
					return Newtonsoft.Json.StringEscapeHandling.Default;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._stringEscapeHandling = new Newtonsoft.Json.StringEscapeHandling?(value);
			}
		}

		public ITraceWriter TraceWriter
		{
			get;
			set;
		}

		public FormatterAssemblyStyle TypeNameAssemblyFormat
		{
			get
			{
				FormatterAssemblyStyle? nullable = this._typeNameAssemblyFormat;
				if (!nullable.HasValue)
				{
					return FormatterAssemblyStyle.Simple;
				}
				return nullable.GetValueOrDefault();
			}
			set
			{
				this._typeNameAssemblyFormat = new FormatterAssemblyStyle?(value);
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

		static JsonSerializerSettings()
		{
			JsonSerializerSettings.DefaultContext = new StreamingContext();
			JsonSerializerSettings.DefaultCulture = CultureInfo.InvariantCulture;
		}

		public JsonSerializerSettings()
		{
			this.Converters = new List<JsonConverter>()
			{
				new VectorConverter(),
				new HashSetConverter()
			};
		}
	}
}