using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	[Preserve]
	public abstract class JsonReader : IDisposable
	{
		private JsonToken _tokenType;

		private object _value;

		internal char _quoteChar;

		internal JsonReader.State _currentState;

		private JsonPosition _currentPosition;

		private CultureInfo _culture;

		private Newtonsoft.Json.DateTimeZoneHandling _dateTimeZoneHandling;

		private int? _maxDepth;

		private bool _hasExceededMaxDepth;

		internal Newtonsoft.Json.DateParseHandling _dateParseHandling;

		internal Newtonsoft.Json.FloatParseHandling _floatParseHandling;

		private string _dateFormatString;

		private List<JsonPosition> _stack;

		public bool CloseInput
		{
			get;
			set;
		}

		public CultureInfo Culture
		{
			get
			{
				return this._culture ?? CultureInfo.InvariantCulture;
			}
			set
			{
				this._culture = value;
			}
		}

		protected JsonReader.State CurrentState
		{
			get
			{
				return this._currentState;
			}
		}

		public string DateFormatString
		{
			get
			{
				return this._dateFormatString;
			}
			set
			{
				this._dateFormatString = value;
			}
		}

		public Newtonsoft.Json.DateParseHandling DateParseHandling
		{
			get
			{
				return this._dateParseHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.DateParseHandling.None || value > Newtonsoft.Json.DateParseHandling.DateTimeOffset)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._dateParseHandling = value;
			}
		}

		public Newtonsoft.Json.DateTimeZoneHandling DateTimeZoneHandling
		{
			get
			{
				return this._dateTimeZoneHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.DateTimeZoneHandling.Local || value > Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._dateTimeZoneHandling = value;
			}
		}

		public virtual int Depth
		{
			get
			{
				int num = (this._stack != null ? this._stack.Count : 0);
				if (JsonTokenUtils.IsStartToken(this.TokenType) || this._currentPosition.Type == JsonContainerType.None)
				{
					return num;
				}
				return num + 1;
			}
		}

		public Newtonsoft.Json.FloatParseHandling FloatParseHandling
		{
			get
			{
				return this._floatParseHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.FloatParseHandling.Double || value > Newtonsoft.Json.FloatParseHandling.Decimal)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._floatParseHandling = value;
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
			}
		}

		public virtual string Path
		{
			get
			{
				JsonPosition? nullable;
				if (this._currentPosition.Type == JsonContainerType.None)
				{
					return string.Empty;
				}
				if ((this._currentState == JsonReader.State.ArrayStart || this._currentState == JsonReader.State.ConstructorStart ? false : this._currentState != JsonReader.State.ObjectStart))
				{
					nullable = new JsonPosition?(this._currentPosition);
				}
				else
				{
					nullable = null;
				}
				return JsonPosition.BuildPath(this._stack, nullable);
			}
		}

		public virtual char QuoteChar
		{
			get
			{
				return this._quoteChar;
			}
			protected internal set
			{
				this._quoteChar = value;
			}
		}

		public bool SupportMultipleContent
		{
			get;
			set;
		}

		public virtual JsonToken TokenType
		{
			get
			{
				return this._tokenType;
			}
		}

		public virtual object Value
		{
			get
			{
				return this._value;
			}
		}

		public virtual Type ValueType
		{
			get
			{
				if (this._value == null)
				{
					return null;
				}
				return this._value.GetType();
			}
		}

		protected JsonReader()
		{
			this._currentState = JsonReader.State.Start;
			this._dateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
			this._dateParseHandling = Newtonsoft.Json.DateParseHandling.DateTime;
			this._floatParseHandling = Newtonsoft.Json.FloatParseHandling.Double;
			this.CloseInput = true;
		}

		public virtual void Close()
		{
			this._currentState = JsonReader.State.Closed;
			this._tokenType = JsonToken.None;
			this._value = null;
		}

		internal JsonReaderException CreateUnexpectedEndException()
		{
			return JsonReaderException.Create(this, "Unexpected end when reading JSON.");
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._currentState != JsonReader.State.Closed & disposing)
			{
				this.Close();
			}
		}

		private JsonToken GetContentToken()
		{
			JsonToken tokenType;
			do
			{
				if (!this.Read())
				{
					this.SetToken(JsonToken.None);
					return JsonToken.None;
				}
				tokenType = this.TokenType;
			}
			while (tokenType == JsonToken.Comment);
			return tokenType;
		}

		internal JsonPosition GetPosition(int depth)
		{
			if (this._stack == null || depth >= this._stack.Count)
			{
				return this._currentPosition;
			}
			return this._stack[depth];
		}

		private JsonContainerType GetTypeForCloseToken(JsonToken token)
		{
			switch (token)
			{
				case JsonToken.EndObject:
				{
					return JsonContainerType.Object;
				}
				case JsonToken.EndArray:
				{
					return JsonContainerType.Array;
				}
				case JsonToken.EndConstructor:
				{
					return JsonContainerType.Constructor;
				}
			}
			throw JsonReaderException.Create(this, "Not a valid close JsonToken: {0}".FormatWith(CultureInfo.InvariantCulture, token));
		}

		internal bool MoveToContent()
		{
			for (JsonToken i = this.TokenType; i == JsonToken.None || i == JsonToken.Comment; i = this.TokenType)
			{
				if (!this.Read())
				{
					return false;
				}
			}
			return true;
		}

		private JsonContainerType Peek()
		{
			return this._currentPosition.Type;
		}

		private JsonContainerType Pop()
		{
			JsonPosition jsonPosition;
			if (this._stack == null || this._stack.Count <= 0)
			{
				jsonPosition = this._currentPosition;
				this._currentPosition = new JsonPosition();
			}
			else
			{
				jsonPosition = this._currentPosition;
				this._currentPosition = this._stack[this._stack.Count - 1];
				this._stack.RemoveAt(this._stack.Count - 1);
			}
			if (this._maxDepth.HasValue)
			{
				int depth = this.Depth;
				int? nullable = this._maxDepth;
				if ((depth <= nullable.GetValueOrDefault() ? nullable.HasValue : false))
				{
					this._hasExceededMaxDepth = false;
				}
			}
			return jsonPosition.Type;
		}

		private void Push(JsonContainerType value)
		{
			this.UpdateScopeWithFinishedValue();
			if (this._currentPosition.Type == JsonContainerType.None)
			{
				this._currentPosition = new JsonPosition(value);
				return;
			}
			if (this._stack == null)
			{
				this._stack = new List<JsonPosition>();
			}
			this._stack.Add(this._currentPosition);
			this._currentPosition = new JsonPosition(value);
			if (this._maxDepth.HasValue)
			{
				int? nullable = this._maxDepth;
				if ((this.Depth + 1 > nullable.GetValueOrDefault() ? nullable.HasValue : false) && !this._hasExceededMaxDepth)
				{
					this._hasExceededMaxDepth = true;
					throw JsonReaderException.Create(this, "The reader's MaxDepth of {0} has been exceeded.".FormatWith(CultureInfo.InvariantCulture, this._maxDepth));
				}
			}
		}

		public abstract bool Read();

		internal void ReadAndAssert()
		{
			if (!this.Read())
			{
				throw JsonSerializationException.Create(this, "Unexpected end when reading JSON.");
			}
		}

		internal bool ReadAndMoveToContent()
		{
			if (!this.Read())
			{
				return false;
			}
			return this.MoveToContent();
		}

		internal byte[] ReadArrayIntoByteArray()
		{
			List<byte> nums = new List<byte>();
			while (true)
			{
				JsonToken contentToken = this.GetContentToken();
				if (contentToken == JsonToken.None)
				{
					break;
				}
				if (contentToken != JsonToken.Integer)
				{
					if (contentToken != JsonToken.EndArray)
					{
						throw JsonReaderException.Create(this, "Unexpected token when reading bytes: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
					}
					byte[] array = nums.ToArray();
					this.SetToken(JsonToken.Bytes, array, false);
					return array;
				}
				nums.Add(Convert.ToByte(this.Value, CultureInfo.InvariantCulture));
			}
			throw JsonReaderException.Create(this, "Unexpected end when reading bytes.");
		}

		public virtual bool? ReadAsBoolean()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken != JsonToken.None)
			{
				switch (contentToken)
				{
					case JsonToken.Integer:
					case JsonToken.Float:
					{
						bool flag = Convert.ToBoolean(this.Value, CultureInfo.InvariantCulture);
						this.SetToken(JsonToken.Boolean, flag, false);
						return new bool?(flag);
					}
					case JsonToken.String:
					{
						return this.ReadBooleanString((string)this.Value);
					}
					case JsonToken.Boolean:
					{
						return new bool?((bool)this.Value);
					}
					case JsonToken.Null:
					case JsonToken.EndArray:
					{
						break;
					}
					case JsonToken.Undefined:
					case JsonToken.EndObject:
					{
						throw JsonReaderException.Create(this, "Error reading boolean. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
					}
					default:
					{
						throw JsonReaderException.Create(this, "Error reading boolean. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
					}
				}
			}
			return null;
		}

		public virtual byte[] ReadAsBytes()
		{
			byte[] numArray;
			Guid guid;
			JsonToken contentToken = this.GetContentToken();
			if (contentToken == JsonToken.None)
			{
				return null;
			}
			if (this.TokenType == JsonToken.StartObject)
			{
				this.ReadIntoWrappedTypeObject();
				byte[] numArray1 = this.ReadAsBytes();
				this.ReaderReadAndAssert();
				if (this.TokenType != JsonToken.EndObject)
				{
					throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
				}
				this.SetToken(JsonToken.Bytes, numArray1, false);
				return numArray1;
			}
			if (contentToken > JsonToken.String)
			{
				if (contentToken == JsonToken.Null || contentToken == JsonToken.EndArray)
				{
					return null;
				}
				if (contentToken == JsonToken.Bytes)
				{
					if (this.ValueType != typeof(Guid))
					{
						return (byte[])this.Value;
					}
					byte[] byteArray = ((Guid)this.Value).ToByteArray();
					this.SetToken(JsonToken.Bytes, byteArray, false);
					return byteArray;
				}
			}
			else
			{
				if (contentToken == JsonToken.StartArray)
				{
					return this.ReadArrayIntoByteArray();
				}
				if (contentToken == JsonToken.String)
				{
					string value = (string)this.Value;
					if (value.Length != 0)
					{
						numArray = (!ConvertUtils.TryConvertGuid(value, out guid) ? Convert.FromBase64String(value) : guid.ToByteArray());
					}
					else
					{
						numArray = new byte[0];
					}
					this.SetToken(JsonToken.Bytes, numArray, false);
					return numArray;
				}
			}
			throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
		}

		public virtual DateTime? ReadAsDateTime()
		{
			DateTime? nullable;
			JsonToken contentToken = this.GetContentToken();
			if (contentToken > JsonToken.String)
			{
				if (contentToken == JsonToken.Null || contentToken == JsonToken.EndArray)
				{
					nullable = null;
					return nullable;
				}
				if (contentToken == JsonToken.Date)
				{
					if (this.Value is DateTimeOffset)
					{
						DateTimeOffset value = (DateTimeOffset)this.Value;
						this.SetToken(JsonToken.Date, value.DateTime, false);
					}
					return new DateTime?((DateTime)this.Value);
				}
				throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
			}
			else
			{
				if (contentToken == JsonToken.None)
				{
					nullable = null;
					return nullable;
				}
				if (contentToken == JsonToken.String)
				{
					return this.ReadDateTimeString((string)this.Value);
				}
				throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, this.TokenType));
			}
			nullable = null;
			return nullable;
		}

		public virtual DateTimeOffset? ReadAsDateTimeOffset()
		{
			DateTimeOffset? nullable;
			JsonToken contentToken = this.GetContentToken();
			if (contentToken > JsonToken.String)
			{
				if (contentToken == JsonToken.Null || contentToken == JsonToken.EndArray)
				{
					nullable = null;
					return nullable;
				}
				if (contentToken == JsonToken.Date)
				{
					if (this.Value is DateTime)
					{
						this.SetToken(JsonToken.Date, new DateTimeOffset((DateTime)this.Value), false);
					}
					return new DateTimeOffset?((DateTimeOffset)this.Value);
				}
				throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
			}
			else
			{
				if (contentToken == JsonToken.None)
				{
					nullable = null;
					return nullable;
				}
				if (contentToken == JsonToken.String)
				{
					return this.ReadDateTimeOffsetString((string)this.Value);
				}
				throw JsonReaderException.Create(this, "Error reading date. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
			}
			nullable = null;
			return nullable;
		}

		public virtual decimal? ReadAsDecimal()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken != JsonToken.None)
			{
				switch (contentToken)
				{
					case JsonToken.Integer:
					case JsonToken.Float:
					{
						if (!(this.Value is decimal))
						{
							this.SetToken(JsonToken.Float, Convert.ToDecimal(this.Value, CultureInfo.InvariantCulture), false);
						}
						return new decimal?((decimal)this.Value);
					}
					case JsonToken.String:
					{
						return this.ReadDecimalString((string)this.Value);
					}
					case JsonToken.Boolean:
					case JsonToken.Undefined:
					case JsonToken.EndObject:
					{
						throw JsonReaderException.Create(this, "Error reading decimal. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
					}
					case JsonToken.Null:
					case JsonToken.EndArray:
					{
						break;
					}
					default:
					{
						throw JsonReaderException.Create(this, "Error reading decimal. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
					}
				}
			}
			return null;
		}

		public virtual double? ReadAsDouble()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken != JsonToken.None)
			{
				switch (contentToken)
				{
					case JsonToken.Integer:
					case JsonToken.Float:
					{
						if (!(this.Value is double))
						{
							double num = Convert.ToDouble(this.Value, CultureInfo.InvariantCulture);
							this.SetToken(JsonToken.Float, num, false);
						}
						return new double?((double)this.Value);
					}
					case JsonToken.String:
					{
						return this.ReadDoubleString((string)this.Value);
					}
					case JsonToken.Boolean:
					case JsonToken.Undefined:
					case JsonToken.EndObject:
					{
						throw JsonReaderException.Create(this, "Error reading double. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
					}
					case JsonToken.Null:
					case JsonToken.EndArray:
					{
						break;
					}
					default:
					{
						throw JsonReaderException.Create(this, "Error reading double. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
					}
				}
			}
			return null;
		}

		public virtual int? ReadAsInt32()
		{
			JsonToken contentToken = this.GetContentToken();
			if (contentToken != JsonToken.None)
			{
				switch (contentToken)
				{
					case JsonToken.Integer:
					case JsonToken.Float:
					{
						if (!(this.Value is int))
						{
							this.SetToken(JsonToken.Integer, Convert.ToInt32(this.Value, CultureInfo.InvariantCulture), false);
						}
						return new int?((int)this.Value);
					}
					case JsonToken.String:
					{
						return this.ReadInt32String((string)this.Value);
					}
					case JsonToken.Boolean:
					case JsonToken.Undefined:
					case JsonToken.EndObject:
					{
						throw JsonReaderException.Create(this, "Error reading integer. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
					}
					case JsonToken.Null:
					case JsonToken.EndArray:
					{
						break;
					}
					default:
					{
						throw JsonReaderException.Create(this, "Error reading integer. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
					}
				}
			}
			return null;
		}

		public virtual string ReadAsString()
		{
			string str;
			JsonToken contentToken = this.GetContentToken();
			if (contentToken <= JsonToken.String)
			{
				if (contentToken == JsonToken.None)
				{
					return null;
				}
				if (contentToken == JsonToken.String)
				{
					return (string)this.Value;
				}
				if (!JsonTokenUtils.IsPrimitiveToken(contentToken) || this.Value == null)
				{
					throw JsonReaderException.Create(this, "Error reading string. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
				}
				if (!(this.Value is IFormattable))
				{
					str = (!(this.Value is Uri) ? this.Value.ToString() : ((Uri)this.Value).OriginalString);
				}
				else
				{
					str = ((IFormattable)this.Value).ToString(null, this.Culture);
				}
				this.SetToken(JsonToken.String, str, false);
				return str;
			}
			else if (contentToken != JsonToken.Null && contentToken != JsonToken.EndArray)
			{
				if (!JsonTokenUtils.IsPrimitiveToken(contentToken) || this.Value == null)
				{
					throw JsonReaderException.Create(this, "Error reading string. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, contentToken));
				}
				if (!(this.Value is IFormattable))
				{
					str = (!(this.Value is Uri) ? this.Value.ToString() : ((Uri)this.Value).OriginalString);
				}
				else
				{
					str = ((IFormattable)this.Value).ToString(null, this.Culture);
				}
				this.SetToken(JsonToken.String, str, false);
				return str;
			}
			return null;
		}

		internal bool? ReadBooleanString(string s)
		{
			bool flag;
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			if (!bool.TryParse(s, out flag))
			{
				this.SetToken(JsonToken.String, s, false);
				throw JsonReaderException.Create(this, "Could not convert string to boolean: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
			}
			this.SetToken(JsonToken.Boolean, flag, false);
			return new bool?(flag);
		}

		internal DateTimeOffset? ReadDateTimeOffsetString(string s)
		{
			DateTimeOffset dateTimeOffset;
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			if (DateTimeUtils.TryParseDateTimeOffset(s, this._dateFormatString, this.Culture, out dateTimeOffset))
			{
				this.SetToken(JsonToken.Date, dateTimeOffset, false);
				return new DateTimeOffset?(dateTimeOffset);
			}
			if (!DateTimeOffset.TryParse(s, this.Culture, DateTimeStyles.RoundtripKind, out dateTimeOffset))
			{
				this.SetToken(JsonToken.String, s, false);
				throw JsonReaderException.Create(this, "Could not convert string to DateTimeOffset: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
			}
			this.SetToken(JsonToken.Date, dateTimeOffset, false);
			return new DateTimeOffset?(dateTimeOffset);
		}

		internal DateTime? ReadDateTimeString(string s)
		{
			DateTime dateTime;
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			if (DateTimeUtils.TryParseDateTime(s, this.DateTimeZoneHandling, this._dateFormatString, this.Culture, out dateTime))
			{
				dateTime = DateTimeUtils.EnsureDateTime(dateTime, this.DateTimeZoneHandling);
				this.SetToken(JsonToken.Date, dateTime, false);
				return new DateTime?(dateTime);
			}
			if (!DateTime.TryParse(s, this.Culture, DateTimeStyles.RoundtripKind, out dateTime))
			{
				throw JsonReaderException.Create(this, "Could not convert string to DateTime: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
			}
			dateTime = DateTimeUtils.EnsureDateTime(dateTime, this.DateTimeZoneHandling);
			this.SetToken(JsonToken.Date, dateTime, false);
			return new DateTime?(dateTime);
		}

		internal decimal? ReadDecimalString(string s)
		{
			decimal num;
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			if (!decimal.TryParse(s, NumberStyles.Number, this.Culture, out num))
			{
				this.SetToken(JsonToken.String, s, false);
				throw JsonReaderException.Create(this, "Could not convert string to decimal: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
			}
			this.SetToken(JsonToken.Float, num, false);
			return new decimal?(num);
		}

		internal double? ReadDoubleString(string s)
		{
			double num;
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			if (!double.TryParse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.Integer | NumberStyles.Float, this.Culture, out num))
			{
				this.SetToken(JsonToken.String, s, false);
				throw JsonReaderException.Create(this, "Could not convert string to double: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
			}
			this.SetToken(JsonToken.Float, num, false);
			return new double?(num);
		}

		internal void ReaderReadAndAssert()
		{
			if (!this.Read())
			{
				throw this.CreateUnexpectedEndException();
			}
		}

		internal int? ReadInt32String(string s)
		{
			int num;
			if (string.IsNullOrEmpty(s))
			{
				this.SetToken(JsonToken.Null, null, false);
				return null;
			}
			if (!int.TryParse(s, NumberStyles.Integer, this.Culture, out num))
			{
				this.SetToken(JsonToken.String, s, false);
				throw JsonReaderException.Create(this, "Could not convert string to integer: {0}.".FormatWith(CultureInfo.InvariantCulture, s));
			}
			this.SetToken(JsonToken.Integer, num, false);
			return new int?(num);
		}

		internal void ReadIntoWrappedTypeObject()
		{
			this.ReaderReadAndAssert();
			if (this.Value.ToString() == "$type")
			{
				this.ReaderReadAndAssert();
				if (this.Value != null && this.Value.ToString().StartsWith("System.Byte[]", StringComparison.Ordinal))
				{
					this.ReaderReadAndAssert();
					if (this.Value.ToString() == "$value")
					{
						return;
					}
				}
			}
			throw JsonReaderException.Create(this, "Error reading bytes. Unexpected token: {0}.".FormatWith(CultureInfo.InvariantCulture, JsonToken.StartObject));
		}

		private void SetFinished()
		{
			if (this.SupportMultipleContent)
			{
				this._currentState = JsonReader.State.Start;
				return;
			}
			this._currentState = JsonReader.State.Finished;
		}

		internal void SetPostValueState(bool updateIndex)
		{
			if (this.Peek() == JsonContainerType.None)
			{
				this.SetFinished();
			}
			else
			{
				this._currentState = JsonReader.State.PostValue;
			}
			if (updateIndex)
			{
				this.UpdateScopeWithFinishedValue();
			}
		}

		protected void SetStateBasedOnCurrent()
		{
			JsonContainerType jsonContainerType = this.Peek();
			switch (jsonContainerType)
			{
				case JsonContainerType.None:
				{
					this.SetFinished();
					return;
				}
				case JsonContainerType.Object:
				{
					this._currentState = JsonReader.State.Object;
					return;
				}
				case JsonContainerType.Array:
				{
					this._currentState = JsonReader.State.Array;
					return;
				}
				case JsonContainerType.Constructor:
				{
					this._currentState = JsonReader.State.Constructor;
					return;
				}
			}
			throw JsonReaderException.Create(this, "While setting the reader state back to current object an unexpected JsonType was encountered: {0}".FormatWith(CultureInfo.InvariantCulture, jsonContainerType));
		}

		protected void SetToken(JsonToken newToken)
		{
			this.SetToken(newToken, null, true);
		}

		protected void SetToken(JsonToken newToken, object value)
		{
			this.SetToken(newToken, value, true);
		}

		internal void SetToken(JsonToken newToken, object value, bool updateIndex)
		{
			this._tokenType = newToken;
			this._value = value;
			switch (newToken)
			{
				case JsonToken.StartObject:
				{
					this._currentState = JsonReader.State.ObjectStart;
					this.Push(JsonContainerType.Object);
					return;
				}
				case JsonToken.StartArray:
				{
					this._currentState = JsonReader.State.ArrayStart;
					this.Push(JsonContainerType.Array);
					return;
				}
				case JsonToken.StartConstructor:
				{
					this._currentState = JsonReader.State.ConstructorStart;
					this.Push(JsonContainerType.Constructor);
					return;
				}
				case JsonToken.PropertyName:
				{
					this._currentState = JsonReader.State.Property;
					this._currentPosition.PropertyName = (string)value;
					return;
				}
				case JsonToken.Comment:
				{
					return;
				}
				case JsonToken.Raw:
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.String:
				case JsonToken.Boolean:
				case JsonToken.Null:
				case JsonToken.Undefined:
				case JsonToken.Date:
				case JsonToken.Bytes:
				{
					this.SetPostValueState(updateIndex);
					return;
				}
				case JsonToken.EndObject:
				{
					this.ValidateEnd(JsonToken.EndObject);
					return;
				}
				case JsonToken.EndArray:
				{
					this.ValidateEnd(JsonToken.EndArray);
					return;
				}
				case JsonToken.EndConstructor:
				{
					this.ValidateEnd(JsonToken.EndConstructor);
					return;
				}
				default:
				{
					return;
				}
			}
		}

		public void Skip()
		{
			if (this.TokenType == JsonToken.PropertyName)
			{
				this.Read();
			}
			if (JsonTokenUtils.IsStartToken(this.TokenType))
			{
				int depth = this.Depth;
				while (this.Read() && depth < this.Depth)
				{
				}
			}
		}

		void System.IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void UpdateScopeWithFinishedValue()
		{
			if (this._currentPosition.HasIndex)
			{
				this._currentPosition.Position++;
			}
		}

		private void ValidateEnd(JsonToken endToken)
		{
			JsonContainerType jsonContainerType = this.Pop();
			if (this.GetTypeForCloseToken(endToken) != jsonContainerType)
			{
				throw JsonReaderException.Create(this, "JsonToken {0} is not valid for closing JsonType {1}.".FormatWith(CultureInfo.InvariantCulture, endToken, jsonContainerType));
			}
			if (this.Peek() == JsonContainerType.None)
			{
				this.SetFinished();
				return;
			}
			this._currentState = JsonReader.State.PostValue;
		}

		protected internal enum State
		{
			Start,
			Complete,
			Property,
			ObjectStart,
			Object,
			ArrayStart,
			Array,
			Closed,
			PostValue,
			ConstructorStart,
			Constructor,
			Error,
			Finished
		}
	}
}