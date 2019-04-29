using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Threading;

namespace Newtonsoft.Json
{
	[Preserve]
	public class JsonSerializer
	{
		internal Newtonsoft.Json.TypeNameHandling _typeNameHandling;

		internal FormatterAssemblyStyle _typeNameAssemblyFormat;

		internal Newtonsoft.Json.PreserveReferencesHandling _preserveReferencesHandling;

		internal Newtonsoft.Json.ReferenceLoopHandling _referenceLoopHandling;

		internal Newtonsoft.Json.MissingMemberHandling _missingMemberHandling;

		internal Newtonsoft.Json.ObjectCreationHandling _objectCreationHandling;

		internal Newtonsoft.Json.NullValueHandling _nullValueHandling;

		internal Newtonsoft.Json.DefaultValueHandling _defaultValueHandling;

		internal Newtonsoft.Json.ConstructorHandling _constructorHandling;

		internal Newtonsoft.Json.MetadataPropertyHandling _metadataPropertyHandling;

		internal JsonConverterCollection _converters;

		internal IContractResolver _contractResolver;

		internal ITraceWriter _traceWriter;

		internal IEqualityComparer _equalityComparer;

		internal SerializationBinder _binder;

		internal StreamingContext _context;

		private IReferenceResolver _referenceResolver;

		private Newtonsoft.Json.Formatting? _formatting;

		private Newtonsoft.Json.DateFormatHandling? _dateFormatHandling;

		private Newtonsoft.Json.DateTimeZoneHandling? _dateTimeZoneHandling;

		private Newtonsoft.Json.DateParseHandling? _dateParseHandling;

		private Newtonsoft.Json.FloatFormatHandling? _floatFormatHandling;

		private Newtonsoft.Json.FloatParseHandling? _floatParseHandling;

		private Newtonsoft.Json.StringEscapeHandling? _stringEscapeHandling;

		private CultureInfo _culture;

		private int? _maxDepth;

		private bool _maxDepthSet;

		private bool? _checkAdditionalContent;

		private string _dateFormatString;

		private bool _dateFormatStringSet;

		public virtual SerializationBinder Binder
		{
			get
			{
				return this._binder;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Serialization binder cannot be null.");
				}
				this._binder = value;
			}
		}

		public virtual bool CheckAdditionalContent
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

		public virtual Newtonsoft.Json.ConstructorHandling ConstructorHandling
		{
			get
			{
				return this._constructorHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.ConstructorHandling.Default || value > Newtonsoft.Json.ConstructorHandling.AllowNonPublicDefaultConstructor)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._constructorHandling = value;
			}
		}

		public virtual StreamingContext Context
		{
			get
			{
				return this._context;
			}
			set
			{
				this._context = value;
			}
		}

		public virtual IContractResolver ContractResolver
		{
			get
			{
				return this._contractResolver;
			}
			set
			{
				this._contractResolver = value ?? DefaultContractResolver.Instance;
			}
		}

		public virtual JsonConverterCollection Converters
		{
			get
			{
				if (this._converters == null)
				{
					this._converters = new JsonConverterCollection();
				}
				return this._converters;
			}
		}

		public virtual CultureInfo Culture
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

		public virtual Newtonsoft.Json.DateFormatHandling DateFormatHandling
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

		public virtual string DateFormatString
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

		public virtual Newtonsoft.Json.DateParseHandling DateParseHandling
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

		public virtual Newtonsoft.Json.DateTimeZoneHandling DateTimeZoneHandling
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

		public virtual Newtonsoft.Json.DefaultValueHandling DefaultValueHandling
		{
			get
			{
				return this._defaultValueHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.DefaultValueHandling.Include || value > Newtonsoft.Json.DefaultValueHandling.IgnoreAndPopulate)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._defaultValueHandling = value;
			}
		}

		public virtual IEqualityComparer EqualityComparer
		{
			get
			{
				return this._equalityComparer;
			}
			set
			{
				this._equalityComparer = value;
			}
		}

		public virtual Newtonsoft.Json.FloatFormatHandling FloatFormatHandling
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

		public virtual Newtonsoft.Json.FloatParseHandling FloatParseHandling
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

		public virtual Newtonsoft.Json.Formatting Formatting
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

		public virtual int? MaxDepth
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

		public virtual Newtonsoft.Json.MetadataPropertyHandling MetadataPropertyHandling
		{
			get
			{
				return this._metadataPropertyHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.MetadataPropertyHandling.Default || value > Newtonsoft.Json.MetadataPropertyHandling.Ignore)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._metadataPropertyHandling = value;
			}
		}

		public virtual Newtonsoft.Json.MissingMemberHandling MissingMemberHandling
		{
			get
			{
				return this._missingMemberHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.MissingMemberHandling.Ignore || value > Newtonsoft.Json.MissingMemberHandling.Error)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._missingMemberHandling = value;
			}
		}

		public virtual Newtonsoft.Json.NullValueHandling NullValueHandling
		{
			get
			{
				return this._nullValueHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.NullValueHandling.Include || value > Newtonsoft.Json.NullValueHandling.Ignore)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._nullValueHandling = value;
			}
		}

		public virtual Newtonsoft.Json.ObjectCreationHandling ObjectCreationHandling
		{
			get
			{
				return this._objectCreationHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.ObjectCreationHandling.Auto || value > Newtonsoft.Json.ObjectCreationHandling.Replace)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._objectCreationHandling = value;
			}
		}

		public virtual Newtonsoft.Json.PreserveReferencesHandling PreserveReferencesHandling
		{
			get
			{
				return this._preserveReferencesHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.PreserveReferencesHandling.None || value > Newtonsoft.Json.PreserveReferencesHandling.All)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._preserveReferencesHandling = value;
			}
		}

		public virtual Newtonsoft.Json.ReferenceLoopHandling ReferenceLoopHandling
		{
			get
			{
				return this._referenceLoopHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.ReferenceLoopHandling.Error || value > Newtonsoft.Json.ReferenceLoopHandling.Serialize)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._referenceLoopHandling = value;
			}
		}

		public virtual IReferenceResolver ReferenceResolver
		{
			get
			{
				return this.GetReferenceResolver();
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Reference resolver cannot be null.");
				}
				this._referenceResolver = value;
			}
		}

		public virtual Newtonsoft.Json.StringEscapeHandling StringEscapeHandling
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

		public virtual ITraceWriter TraceWriter
		{
			get
			{
				return this._traceWriter;
			}
			set
			{
				this._traceWriter = value;
			}
		}

		public virtual FormatterAssemblyStyle TypeNameAssemblyFormat
		{
			get
			{
				return this._typeNameAssemblyFormat;
			}
			set
			{
				if (value < FormatterAssemblyStyle.Simple || value > FormatterAssemblyStyle.Full)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._typeNameAssemblyFormat = value;
			}
		}

		public virtual Newtonsoft.Json.TypeNameHandling TypeNameHandling
		{
			get
			{
				return this._typeNameHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.TypeNameHandling.None || value > Newtonsoft.Json.TypeNameHandling.Auto)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._typeNameHandling = value;
			}
		}

		public JsonSerializer()
		{
			this._referenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Error;
			this._missingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
			this._nullValueHandling = Newtonsoft.Json.NullValueHandling.Include;
			this._defaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include;
			this._objectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Auto;
			this._preserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
			this._constructorHandling = Newtonsoft.Json.ConstructorHandling.Default;
			this._typeNameHandling = Newtonsoft.Json.TypeNameHandling.None;
			this._metadataPropertyHandling = Newtonsoft.Json.MetadataPropertyHandling.Default;
			this._context = JsonSerializerSettings.DefaultContext;
			this._binder = DefaultSerializationBinder.Instance;
			this._culture = JsonSerializerSettings.DefaultCulture;
			this._contractResolver = DefaultContractResolver.Instance;
		}

		private static void ApplySerializerSettings(JsonSerializer serializer, JsonSerializerSettings settings)
		{
			if (!CollectionUtils.IsNullOrEmpty<JsonConverter>(settings.Converters))
			{
				for (int i = 0; i < settings.Converters.Count; i++)
				{
					serializer.Converters.Insert(i, settings.Converters[i]);
				}
			}
			if (settings._typeNameHandling.HasValue)
			{
				serializer.TypeNameHandling = settings.TypeNameHandling;
			}
			if (settings._metadataPropertyHandling.HasValue)
			{
				serializer.MetadataPropertyHandling = settings.MetadataPropertyHandling;
			}
			if (settings._typeNameAssemblyFormat.HasValue)
			{
				serializer.TypeNameAssemblyFormat = settings.TypeNameAssemblyFormat;
			}
			if (settings._preserveReferencesHandling.HasValue)
			{
				serializer.PreserveReferencesHandling = settings.PreserveReferencesHandling;
			}
			if (settings._referenceLoopHandling.HasValue)
			{
				serializer.ReferenceLoopHandling = settings.ReferenceLoopHandling;
			}
			if (settings._missingMemberHandling.HasValue)
			{
				serializer.MissingMemberHandling = settings.MissingMemberHandling;
			}
			if (settings._objectCreationHandling.HasValue)
			{
				serializer.ObjectCreationHandling = settings.ObjectCreationHandling;
			}
			if (settings._nullValueHandling.HasValue)
			{
				serializer.NullValueHandling = settings.NullValueHandling;
			}
			if (settings._defaultValueHandling.HasValue)
			{
				serializer.DefaultValueHandling = settings.DefaultValueHandling;
			}
			if (settings._constructorHandling.HasValue)
			{
				serializer.ConstructorHandling = settings.ConstructorHandling;
			}
			if (settings._context.HasValue)
			{
				serializer.Context = settings.Context;
			}
			if (settings._checkAdditionalContent.HasValue)
			{
				serializer._checkAdditionalContent = settings._checkAdditionalContent;
			}
			if (settings.Error != null)
			{
				serializer.Error += settings.Error;
			}
			if (settings.ContractResolver != null)
			{
				serializer.ContractResolver = settings.ContractResolver;
			}
			if (settings.ReferenceResolverProvider != null)
			{
				serializer.ReferenceResolver = settings.ReferenceResolverProvider();
			}
			if (settings.TraceWriter != null)
			{
				serializer.TraceWriter = settings.TraceWriter;
			}
			if (settings.EqualityComparer != null)
			{
				serializer.EqualityComparer = settings.EqualityComparer;
			}
			if (settings.Binder != null)
			{
				serializer.Binder = settings.Binder;
			}
			if (settings._formatting.HasValue)
			{
				serializer._formatting = settings._formatting;
			}
			if (settings._dateFormatHandling.HasValue)
			{
				serializer._dateFormatHandling = settings._dateFormatHandling;
			}
			if (settings._dateTimeZoneHandling.HasValue)
			{
				serializer._dateTimeZoneHandling = settings._dateTimeZoneHandling;
			}
			if (settings._dateParseHandling.HasValue)
			{
				serializer._dateParseHandling = settings._dateParseHandling;
			}
			if (settings._dateFormatStringSet)
			{
				serializer._dateFormatString = settings._dateFormatString;
				serializer._dateFormatStringSet = settings._dateFormatStringSet;
			}
			if (settings._floatFormatHandling.HasValue)
			{
				serializer._floatFormatHandling = settings._floatFormatHandling;
			}
			if (settings._floatParseHandling.HasValue)
			{
				serializer._floatParseHandling = settings._floatParseHandling;
			}
			if (settings._stringEscapeHandling.HasValue)
			{
				serializer._stringEscapeHandling = settings._stringEscapeHandling;
			}
			if (settings._culture != null)
			{
				serializer._culture = settings._culture;
			}
			if (settings._maxDepthSet)
			{
				serializer._maxDepth = settings._maxDepth;
				serializer._maxDepthSet = settings._maxDepthSet;
			}
		}

		public static JsonSerializer Create()
		{
			return new JsonSerializer();
		}

		public static JsonSerializer Create(JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.Create();
			if (settings != null)
			{
				JsonSerializer.ApplySerializerSettings(jsonSerializer, settings);
			}
			return jsonSerializer;
		}

		public static JsonSerializer CreateDefault()
		{
			JsonSerializerSettings jsonSerializerSetting;
			Func<JsonSerializerSettings> defaultSettings = JsonConvert.DefaultSettings;
			if (defaultSettings != null)
			{
				jsonSerializerSetting = defaultSettings();
			}
			else
			{
				jsonSerializerSetting = null;
			}
			return JsonSerializer.Create(jsonSerializerSetting);
		}

		public static JsonSerializer CreateDefault(JsonSerializerSettings settings)
		{
			JsonSerializer jsonSerializer = JsonSerializer.CreateDefault();
			if (settings != null)
			{
				JsonSerializer.ApplySerializerSettings(jsonSerializer, settings);
			}
			return jsonSerializer;
		}

		public object Deserialize(JsonReader reader)
		{
			return this.Deserialize(reader, null);
		}

		public object Deserialize(TextReader reader, Type objectType)
		{
			return this.Deserialize(new JsonTextReader(reader), objectType);
		}

		public T Deserialize<T>(JsonReader reader)
		{
			return (T)this.Deserialize(reader, typeof(T));
		}

		public object Deserialize(JsonReader reader, Type objectType)
		{
			return this.DeserializeInternal(reader, objectType);
		}

		internal virtual object DeserializeInternal(JsonReader reader, Type objectType)
		{
			CultureInfo cultureInfo;
			Newtonsoft.Json.DateTimeZoneHandling? nullable;
			Newtonsoft.Json.DateParseHandling? nullable1;
			Newtonsoft.Json.FloatParseHandling? nullable2;
			int? nullable3;
			string str;
			TraceJsonReader traceJsonReader;
			ValidationUtils.ArgumentNotNull(reader, "reader");
			this.SetupReader(reader, out cultureInfo, out nullable, out nullable1, out nullable2, out nullable3, out str);
			if (this.TraceWriter == null || this.TraceWriter.LevelFilter < TraceLevel.Verbose)
			{
				traceJsonReader = null;
			}
			else
			{
				traceJsonReader = new TraceJsonReader(reader);
			}
			TraceJsonReader traceJsonReader1 = traceJsonReader;
			JsonSerializerInternalReader jsonSerializerInternalReader = new JsonSerializerInternalReader(this);
			JsonReader jsonReader = traceJsonReader1;
			if (jsonReader == null)
			{
				jsonReader = reader;
			}
			object obj = jsonSerializerInternalReader.Deserialize(jsonReader, objectType, this.CheckAdditionalContent);
			if (traceJsonReader1 != null)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, traceJsonReader1.GetDeserializedJsonMessage(), null);
			}
			this.ResetReader(reader, cultureInfo, nullable, nullable1, nullable2, nullable3, str);
			return obj;
		}

		internal JsonConverter GetMatchingConverter(Type type)
		{
			return JsonSerializer.GetMatchingConverter(this._converters, type);
		}

		internal static JsonConverter GetMatchingConverter(IList<JsonConverter> converters, Type objectType)
		{
			if (converters != null)
			{
				for (int i = 0; i < converters.Count; i++)
				{
					JsonConverter item = converters[i];
					if (item.CanConvert(objectType))
					{
						return item;
					}
				}
			}
			return null;
		}

		internal IReferenceResolver GetReferenceResolver()
		{
			if (this._referenceResolver == null)
			{
				this._referenceResolver = new DefaultReferenceResolver();
			}
			return this._referenceResolver;
		}

		internal bool IsCheckAdditionalContentSet()
		{
			return this._checkAdditionalContent.HasValue;
		}

		internal void OnError(Newtonsoft.Json.Serialization.ErrorEventArgs e)
		{
			EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> eventHandler = this.Error;
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public void Populate(TextReader reader, object target)
		{
			this.Populate(new JsonTextReader(reader), target);
		}

		public void Populate(JsonReader reader, object target)
		{
			this.PopulateInternal(reader, target);
		}

		internal virtual void PopulateInternal(JsonReader reader, object target)
		{
			CultureInfo cultureInfo;
			Newtonsoft.Json.DateTimeZoneHandling? nullable;
			Newtonsoft.Json.DateParseHandling? nullable1;
			Newtonsoft.Json.FloatParseHandling? nullable2;
			int? nullable3;
			string str;
			TraceJsonReader traceJsonReader;
			ValidationUtils.ArgumentNotNull(reader, "reader");
			ValidationUtils.ArgumentNotNull(target, "target");
			this.SetupReader(reader, out cultureInfo, out nullable, out nullable1, out nullable2, out nullable3, out str);
			if (this.TraceWriter == null || this.TraceWriter.LevelFilter < TraceLevel.Verbose)
			{
				traceJsonReader = null;
			}
			else
			{
				traceJsonReader = new TraceJsonReader(reader);
			}
			TraceJsonReader traceJsonReader1 = traceJsonReader;
			JsonSerializerInternalReader jsonSerializerInternalReader = new JsonSerializerInternalReader(this);
			JsonReader jsonReader = traceJsonReader1;
			if (jsonReader == null)
			{
				jsonReader = reader;
			}
			jsonSerializerInternalReader.Populate(jsonReader, target);
			if (traceJsonReader1 != null)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, traceJsonReader1.GetDeserializedJsonMessage(), null);
			}
			this.ResetReader(reader, cultureInfo, nullable, nullable1, nullable2, nullable3, str);
		}

		private void ResetReader(JsonReader reader, CultureInfo previousCulture, Newtonsoft.Json.DateTimeZoneHandling? previousDateTimeZoneHandling, Newtonsoft.Json.DateParseHandling? previousDateParseHandling, Newtonsoft.Json.FloatParseHandling? previousFloatParseHandling, int? previousMaxDepth, string previousDateFormatString)
		{
			if (previousCulture != null)
			{
				reader.Culture = previousCulture;
			}
			if (previousDateTimeZoneHandling.HasValue)
			{
				reader.DateTimeZoneHandling = previousDateTimeZoneHandling.GetValueOrDefault();
			}
			if (previousDateParseHandling.HasValue)
			{
				reader.DateParseHandling = previousDateParseHandling.GetValueOrDefault();
			}
			if (previousFloatParseHandling.HasValue)
			{
				reader.FloatParseHandling = previousFloatParseHandling.GetValueOrDefault();
			}
			if (this._maxDepthSet)
			{
				reader.MaxDepth = previousMaxDepth;
			}
			if (this._dateFormatStringSet)
			{
				reader.DateFormatString = previousDateFormatString;
			}
			JsonTextReader jsonTextReader = reader as JsonTextReader;
			if (jsonTextReader != null)
			{
				jsonTextReader.NameTable = null;
			}
		}

		public void Serialize(TextWriter textWriter, object value)
		{
			this.Serialize(new JsonTextWriter(textWriter), value);
		}

		public void Serialize(JsonWriter jsonWriter, object value, Type objectType)
		{
			this.SerializeInternal(jsonWriter, value, objectType);
		}

		public void Serialize(TextWriter textWriter, object value, Type objectType)
		{
			this.Serialize(new JsonTextWriter(textWriter), value, objectType);
		}

		public void Serialize(JsonWriter jsonWriter, object value)
		{
			this.SerializeInternal(jsonWriter, value, null);
		}

		internal virtual void SerializeInternal(JsonWriter jsonWriter, object value, Type objectType)
		{
			TraceJsonWriter traceJsonWriter;
			ValidationUtils.ArgumentNotNull(jsonWriter, "jsonWriter");
			Newtonsoft.Json.Formatting? nullable = null;
			if (this._formatting.HasValue)
			{
				Newtonsoft.Json.Formatting formatting = jsonWriter.Formatting;
				Newtonsoft.Json.Formatting? nullable1 = this._formatting;
				if ((formatting == nullable1.GetValueOrDefault() ? !nullable1.HasValue : true))
				{
					nullable = new Newtonsoft.Json.Formatting?(jsonWriter.Formatting);
					jsonWriter.Formatting = this._formatting.GetValueOrDefault();
				}
			}
			Newtonsoft.Json.DateFormatHandling? nullable2 = null;
			if (this._dateFormatHandling.HasValue)
			{
				Newtonsoft.Json.DateFormatHandling dateFormatHandling = jsonWriter.DateFormatHandling;
				Newtonsoft.Json.DateFormatHandling? nullable3 = this._dateFormatHandling;
				if ((dateFormatHandling == nullable3.GetValueOrDefault() ? !nullable3.HasValue : true))
				{
					nullable2 = new Newtonsoft.Json.DateFormatHandling?(jsonWriter.DateFormatHandling);
					jsonWriter.DateFormatHandling = this._dateFormatHandling.GetValueOrDefault();
				}
			}
			Newtonsoft.Json.DateTimeZoneHandling? nullable4 = null;
			if (this._dateTimeZoneHandling.HasValue)
			{
				Newtonsoft.Json.DateTimeZoneHandling dateTimeZoneHandling = jsonWriter.DateTimeZoneHandling;
				Newtonsoft.Json.DateTimeZoneHandling? nullable5 = this._dateTimeZoneHandling;
				if ((dateTimeZoneHandling == nullable5.GetValueOrDefault() ? !nullable5.HasValue : true))
				{
					nullable4 = new Newtonsoft.Json.DateTimeZoneHandling?(jsonWriter.DateTimeZoneHandling);
					jsonWriter.DateTimeZoneHandling = this._dateTimeZoneHandling.GetValueOrDefault();
				}
			}
			Newtonsoft.Json.FloatFormatHandling? nullable6 = null;
			if (this._floatFormatHandling.HasValue)
			{
				Newtonsoft.Json.FloatFormatHandling floatFormatHandling = jsonWriter.FloatFormatHandling;
				Newtonsoft.Json.FloatFormatHandling? nullable7 = this._floatFormatHandling;
				if ((floatFormatHandling == nullable7.GetValueOrDefault() ? !nullable7.HasValue : true))
				{
					nullable6 = new Newtonsoft.Json.FloatFormatHandling?(jsonWriter.FloatFormatHandling);
					jsonWriter.FloatFormatHandling = this._floatFormatHandling.GetValueOrDefault();
				}
			}
			Newtonsoft.Json.StringEscapeHandling? nullable8 = null;
			if (this._stringEscapeHandling.HasValue)
			{
				Newtonsoft.Json.StringEscapeHandling stringEscapeHandling = jsonWriter.StringEscapeHandling;
				Newtonsoft.Json.StringEscapeHandling? nullable9 = this._stringEscapeHandling;
				if ((stringEscapeHandling == nullable9.GetValueOrDefault() ? !nullable9.HasValue : true))
				{
					nullable8 = new Newtonsoft.Json.StringEscapeHandling?(jsonWriter.StringEscapeHandling);
					jsonWriter.StringEscapeHandling = this._stringEscapeHandling.GetValueOrDefault();
				}
			}
			CultureInfo culture = null;
			if (this._culture != null && !this._culture.Equals(jsonWriter.Culture))
			{
				culture = jsonWriter.Culture;
				jsonWriter.Culture = this._culture;
			}
			string dateFormatString = null;
			if (this._dateFormatStringSet && jsonWriter.DateFormatString != this._dateFormatString)
			{
				dateFormatString = jsonWriter.DateFormatString;
				jsonWriter.DateFormatString = this._dateFormatString;
			}
			if (this.TraceWriter == null || this.TraceWriter.LevelFilter < TraceLevel.Verbose)
			{
				traceJsonWriter = null;
			}
			else
			{
				traceJsonWriter = new TraceJsonWriter(jsonWriter);
			}
			TraceJsonWriter traceJsonWriter1 = traceJsonWriter;
			JsonSerializerInternalWriter jsonSerializerInternalWriter = new JsonSerializerInternalWriter(this);
			JsonWriter jsonWriter1 = traceJsonWriter1;
			if (jsonWriter1 == null)
			{
				jsonWriter1 = jsonWriter;
			}
			jsonSerializerInternalWriter.Serialize(jsonWriter1, value, objectType);
			if (traceJsonWriter1 != null)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, traceJsonWriter1.GetSerializedJsonMessage(), null);
			}
			if (nullable.HasValue)
			{
				jsonWriter.Formatting = nullable.GetValueOrDefault();
			}
			if (nullable2.HasValue)
			{
				jsonWriter.DateFormatHandling = nullable2.GetValueOrDefault();
			}
			if (nullable4.HasValue)
			{
				jsonWriter.DateTimeZoneHandling = nullable4.GetValueOrDefault();
			}
			if (nullable6.HasValue)
			{
				jsonWriter.FloatFormatHandling = nullable6.GetValueOrDefault();
			}
			if (nullable8.HasValue)
			{
				jsonWriter.StringEscapeHandling = nullable8.GetValueOrDefault();
			}
			if (this._dateFormatStringSet)
			{
				jsonWriter.DateFormatString = dateFormatString;
			}
			if (culture != null)
			{
				jsonWriter.Culture = culture;
			}
		}

		private void SetupReader(JsonReader reader, out CultureInfo previousCulture, out Newtonsoft.Json.DateTimeZoneHandling? previousDateTimeZoneHandling, out Newtonsoft.Json.DateParseHandling? previousDateParseHandling, out Newtonsoft.Json.FloatParseHandling? previousFloatParseHandling, out int? previousMaxDepth, out string previousDateFormatString)
		{
			JsonTextReader nameTable;
			DefaultContractResolver defaultContractResolver;
			if (this._culture == null || this._culture.Equals(reader.Culture))
			{
				previousCulture = null;
			}
			else
			{
				previousCulture = reader.Culture;
				reader.Culture = this._culture;
			}
			if (this._dateTimeZoneHandling.HasValue)
			{
				Newtonsoft.Json.DateTimeZoneHandling dateTimeZoneHandling = reader.DateTimeZoneHandling;
				Newtonsoft.Json.DateTimeZoneHandling? nullable = this._dateTimeZoneHandling;
				if ((dateTimeZoneHandling == nullable.GetValueOrDefault() ? nullable.HasValue : false))
				{
					goto Label1;
				}
				previousDateTimeZoneHandling = new Newtonsoft.Json.DateTimeZoneHandling?(reader.DateTimeZoneHandling);
				reader.DateTimeZoneHandling = this._dateTimeZoneHandling.GetValueOrDefault();
				goto Label0;
			}
		Label1:
			previousDateTimeZoneHandling = null;
		Label0:
			if (this._dateParseHandling.HasValue)
			{
				Newtonsoft.Json.DateParseHandling dateParseHandling = reader.DateParseHandling;
				Newtonsoft.Json.DateParseHandling? nullable1 = this._dateParseHandling;
				if ((dateParseHandling == nullable1.GetValueOrDefault() ? nullable1.HasValue : false))
				{
					goto Label3;
				}
				previousDateParseHandling = new Newtonsoft.Json.DateParseHandling?(reader.DateParseHandling);
				reader.DateParseHandling = this._dateParseHandling.GetValueOrDefault();
				goto Label2;
			}
		Label3:
			previousDateParseHandling = null;
		Label2:
			if (this._floatParseHandling.HasValue)
			{
				Newtonsoft.Json.FloatParseHandling floatParseHandling = reader.FloatParseHandling;
				Newtonsoft.Json.FloatParseHandling? nullable2 = this._floatParseHandling;
				if ((floatParseHandling == nullable2.GetValueOrDefault() ? nullable2.HasValue : false))
				{
					goto Label5;
				}
				previousFloatParseHandling = new Newtonsoft.Json.FloatParseHandling?(reader.FloatParseHandling);
				reader.FloatParseHandling = this._floatParseHandling.GetValueOrDefault();
				goto Label4;
			}
		Label5:
			previousFloatParseHandling = null;
		Label4:
			if (this._maxDepthSet)
			{
				int? maxDepth = reader.MaxDepth;
				int? nullable3 = this._maxDepth;
				if ((maxDepth.GetValueOrDefault() == nullable3.GetValueOrDefault() ? maxDepth.HasValue == nullable3.HasValue : false))
				{
					previousMaxDepth = null;
					if (!this._dateFormatStringSet || !(reader.DateFormatString != this._dateFormatString))
					{
						previousDateFormatString = null;
					}
					else
					{
						previousDateFormatString = reader.DateFormatString;
						reader.DateFormatString = this._dateFormatString;
					}
					nameTable = reader as JsonTextReader;
					if (nameTable != null)
					{
						defaultContractResolver = this._contractResolver as DefaultContractResolver;
						if (defaultContractResolver != null)
						{
							nameTable.NameTable = defaultContractResolver.GetState().NameTable;
						}
					}
					return;
				}
				previousMaxDepth = reader.MaxDepth;
				reader.MaxDepth = this._maxDepth;
				if (!this._dateFormatStringSet || !(reader.DateFormatString != this._dateFormatString))
				{
					previousDateFormatString = null;
				}
				else
				{
					previousDateFormatString = reader.DateFormatString;
					reader.DateFormatString = this._dateFormatString;
				}
				nameTable = reader as JsonTextReader;
				if (nameTable != null)
				{
					defaultContractResolver = this._contractResolver as DefaultContractResolver;
					if (defaultContractResolver != null)
					{
						nameTable.NameTable = defaultContractResolver.GetState().NameTable;
					}
				}
				return;
			}
			previousMaxDepth = null;
			if (!this._dateFormatStringSet || !(reader.DateFormatString != this._dateFormatString))
			{
				previousDateFormatString = null;
			}
			else
			{
				previousDateFormatString = reader.DateFormatString;
				reader.DateFormatString = this._dateFormatString;
			}
			nameTable = reader as JsonTextReader;
			if (nameTable != null)
			{
				defaultContractResolver = this._contractResolver as DefaultContractResolver;
				if (defaultContractResolver != null)
				{
					nameTable.NameTable = defaultContractResolver.GetState().NameTable;
				}
			}
		}

		public virtual event EventHandler<Newtonsoft.Json.Serialization.ErrorEventArgs> Error;
	}
}