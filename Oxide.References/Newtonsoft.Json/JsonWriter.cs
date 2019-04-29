using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	[Preserve]
	public abstract class JsonWriter : IDisposable
	{
		private readonly static JsonWriter.State[][] StateArray;

		internal readonly static JsonWriter.State[][] StateArrayTempate;

		private List<JsonPosition> _stack;

		private JsonPosition _currentPosition;

		private JsonWriter.State _currentState;

		private Newtonsoft.Json.Formatting _formatting;

		private Newtonsoft.Json.DateFormatHandling _dateFormatHandling;

		private Newtonsoft.Json.DateTimeZoneHandling _dateTimeZoneHandling;

		private Newtonsoft.Json.StringEscapeHandling _stringEscapeHandling;

		private Newtonsoft.Json.FloatFormatHandling _floatFormatHandling;

		private string _dateFormatString;

		private CultureInfo _culture;

		public bool CloseOutput
		{
			get;
			set;
		}

		internal string ContainerPath
		{
			get
			{
				if (this._currentPosition.Type == JsonContainerType.None || this._stack == null)
				{
					return string.Empty;
				}
				return JsonPosition.BuildPath(this._stack, null);
			}
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

		public Newtonsoft.Json.DateFormatHandling DateFormatHandling
		{
			get
			{
				return this._dateFormatHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.DateFormatHandling.IsoDateFormat || value > Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._dateFormatHandling = value;
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

		public Newtonsoft.Json.FloatFormatHandling FloatFormatHandling
		{
			get
			{
				return this._floatFormatHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.FloatFormatHandling.String || value > Newtonsoft.Json.FloatFormatHandling.DefaultValue)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._floatFormatHandling = value;
			}
		}

		public Newtonsoft.Json.Formatting Formatting
		{
			get
			{
				return this._formatting;
			}
			set
			{
				if (value < Newtonsoft.Json.Formatting.None || value > Newtonsoft.Json.Formatting.Indented)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._formatting = value;
			}
		}

		public string Path
		{
			get
			{
				JsonPosition? nullable;
				if (this._currentPosition.Type == JsonContainerType.None)
				{
					return string.Empty;
				}
				if ((this._currentState == JsonWriter.State.ArrayStart || this._currentState == JsonWriter.State.ConstructorStart ? false : this._currentState != JsonWriter.State.ObjectStart))
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

		public Newtonsoft.Json.StringEscapeHandling StringEscapeHandling
		{
			get
			{
				return this._stringEscapeHandling;
			}
			set
			{
				if (value < Newtonsoft.Json.StringEscapeHandling.Default || value > Newtonsoft.Json.StringEscapeHandling.EscapeHtml)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this._stringEscapeHandling = value;
				this.OnStringEscapeHandlingChanged();
			}
		}

		protected internal int Top
		{
			get
			{
				int num = (this._stack != null ? this._stack.Count : 0);
				if (this.Peek() != JsonContainerType.None)
				{
					num++;
				}
				return num;
			}
		}

		public Newtonsoft.Json.WriteState WriteState
		{
			get
			{
				switch (this._currentState)
				{
					case JsonWriter.State.Start:
					{
						return Newtonsoft.Json.WriteState.Start;
					}
					case JsonWriter.State.Property:
					{
						return Newtonsoft.Json.WriteState.Property;
					}
					case JsonWriter.State.ObjectStart:
					case JsonWriter.State.Object:
					{
						return Newtonsoft.Json.WriteState.Object;
					}
					case JsonWriter.State.ArrayStart:
					case JsonWriter.State.Array:
					{
						return Newtonsoft.Json.WriteState.Array;
					}
					case JsonWriter.State.ConstructorStart:
					case JsonWriter.State.Constructor:
					{
						return Newtonsoft.Json.WriteState.Constructor;
					}
					case JsonWriter.State.Closed:
					{
						return Newtonsoft.Json.WriteState.Closed;
					}
					case JsonWriter.State.Error:
					{
						return Newtonsoft.Json.WriteState.Error;
					}
				}
				throw JsonWriterException.Create(this, string.Concat("Invalid state: ", this._currentState), null);
			}
		}

		static JsonWriter()
		{
			JsonWriter.StateArrayTempate = new JsonWriter.State[][] { new JsonWriter.State[] { JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.ObjectStart, JsonWriter.State.ObjectStart, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.ObjectStart, JsonWriter.State.ObjectStart, JsonWriter.State.ObjectStart, JsonWriter.State.ObjectStart, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.ArrayStart, JsonWriter.State.ArrayStart, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.ArrayStart, JsonWriter.State.ArrayStart, JsonWriter.State.ArrayStart, JsonWriter.State.ArrayStart, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.ConstructorStart, JsonWriter.State.ConstructorStart, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.ConstructorStart, JsonWriter.State.ConstructorStart, JsonWriter.State.ConstructorStart, JsonWriter.State.ConstructorStart, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.Property, JsonWriter.State.Error, JsonWriter.State.Property, JsonWriter.State.Property, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.Start, JsonWriter.State.Property, JsonWriter.State.ObjectStart, JsonWriter.State.Object, JsonWriter.State.ArrayStart, JsonWriter.State.Array, JsonWriter.State.Constructor, JsonWriter.State.Constructor, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.Start, JsonWriter.State.Property, JsonWriter.State.ObjectStart, JsonWriter.State.Object, JsonWriter.State.ArrayStart, JsonWriter.State.Array, JsonWriter.State.Constructor, JsonWriter.State.Constructor, JsonWriter.State.Error, JsonWriter.State.Error }, new JsonWriter.State[] { JsonWriter.State.Start, JsonWriter.State.Object, JsonWriter.State.Error, JsonWriter.State.Error, JsonWriter.State.Array, JsonWriter.State.Array, JsonWriter.State.Constructor, JsonWriter.State.Constructor, JsonWriter.State.Error, JsonWriter.State.Error } };
			JsonWriter.StateArray = JsonWriter.BuildStateArray();
		}

		protected JsonWriter()
		{
			this._currentState = JsonWriter.State.Start;
			this._formatting = Newtonsoft.Json.Formatting.None;
			this._dateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;
			this.CloseOutput = true;
		}

		internal void AutoComplete(JsonToken tokenBeingWritten)
		{
			JsonWriter.State stateArray = JsonWriter.StateArray[(int)tokenBeingWritten][(int)this._currentState];
			if (stateArray == JsonWriter.State.Error)
			{
				throw JsonWriterException.Create(this, "Token {0} in state {1} would result in an invalid JSON object.".FormatWith(CultureInfo.InvariantCulture, tokenBeingWritten.ToString(), this._currentState.ToString()), null);
			}
			if ((this._currentState == JsonWriter.State.Object || this._currentState == JsonWriter.State.Array || this._currentState == JsonWriter.State.Constructor) && tokenBeingWritten != JsonToken.Comment)
			{
				this.WriteValueDelimiter();
			}
			if (this._formatting == Newtonsoft.Json.Formatting.Indented)
			{
				if (this._currentState == JsonWriter.State.Property)
				{
					this.WriteIndentSpace();
				}
				if (this._currentState == JsonWriter.State.Array || this._currentState == JsonWriter.State.ArrayStart || this._currentState == JsonWriter.State.Constructor || this._currentState == JsonWriter.State.ConstructorStart || tokenBeingWritten == JsonToken.PropertyName && this._currentState != JsonWriter.State.Start)
				{
					this.WriteIndent();
				}
			}
			this._currentState = stateArray;
		}

		private void AutoCompleteAll()
		{
			while (this.Top > 0)
			{
				this.WriteEnd();
			}
		}

		private void AutoCompleteClose(JsonContainerType type)
		{
			int num = 0;
			if (this._currentPosition.Type != type)
			{
				int top = this.Top - 2;
				int num1 = top;
				while (num1 >= 0)
				{
					if (this._stack[top - num1].Type != type)
					{
						num1--;
					}
					else
					{
						num = num1 + 2;
						break;
					}
				}
			}
			else
			{
				num = 1;
			}
			if (num == 0)
			{
				throw JsonWriterException.Create(this, "No token to close.", null);
			}
			for (int i = 0; i < num; i++)
			{
				JsonToken closeTokenForType = this.GetCloseTokenForType(this.Pop());
				if (this._currentState == JsonWriter.State.Property)
				{
					this.WriteNull();
				}
				if (this._formatting == Newtonsoft.Json.Formatting.Indented && this._currentState != JsonWriter.State.ObjectStart && this._currentState != JsonWriter.State.ArrayStart)
				{
					this.WriteIndent();
				}
				this.WriteEnd(closeTokenForType);
				JsonContainerType jsonContainerType = this.Peek();
				switch (jsonContainerType)
				{
					case JsonContainerType.None:
					{
						this._currentState = JsonWriter.State.Start;
						break;
					}
					case JsonContainerType.Object:
					{
						this._currentState = JsonWriter.State.Object;
						break;
					}
					case JsonContainerType.Array:
					{
						this._currentState = JsonWriter.State.Array;
						break;
					}
					case JsonContainerType.Constructor:
					{
						this._currentState = JsonWriter.State.Array;
						break;
					}
					default:
					{
						throw JsonWriterException.Create(this, string.Concat("Unknown JsonType: ", jsonContainerType), null);
					}
				}
			}
		}

		internal static JsonWriter.State[][] BuildStateArray()
		{
			List<JsonWriter.State[]> list = JsonWriter.StateArrayTempate.ToList<JsonWriter.State[]>();
			JsonWriter.State[] stateArrayTempate = JsonWriter.StateArrayTempate[0];
			JsonWriter.State[] stateArray = JsonWriter.StateArrayTempate[7];
			foreach (JsonToken value in EnumUtils.GetValues(typeof(JsonToken)))
			{
				if (list.Count > (int)value)
				{
					continue;
				}
				switch (value)
				{
					case JsonToken.Integer:
					case JsonToken.Float:
					case JsonToken.String:
					case JsonToken.Boolean:
					case JsonToken.Null:
					case JsonToken.Undefined:
					case JsonToken.Date:
					case JsonToken.Bytes:
					{
						list.Add(stateArray);
						continue;
					}
					case JsonToken.EndObject:
					case JsonToken.EndArray:
					case JsonToken.EndConstructor:
					{
						list.Add(stateArrayTempate);
						continue;
					}
					default:
					{
						goto case JsonToken.EndConstructor;
					}
				}
			}
			return list.ToArray();
		}

		public virtual void Close()
		{
			this.AutoCompleteAll();
		}

		private static JsonWriterException CreateUnsupportedTypeException(JsonWriter writer, object value)
		{
			return JsonWriterException.Create(writer, "Unsupported type: {0}. Use the JsonSerializer class to get the object's JSON representation.".FormatWith(CultureInfo.InvariantCulture, value.GetType()), null);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this._currentState != JsonWriter.State.Closed & disposing)
			{
				this.Close();
			}
		}

		public abstract void Flush();

		private JsonToken GetCloseTokenForType(JsonContainerType type)
		{
			switch (type)
			{
				case JsonContainerType.Object:
				{
					return JsonToken.EndObject;
				}
				case JsonContainerType.Array:
				{
					return JsonToken.EndArray;
				}
				case JsonContainerType.Constructor:
				{
					return JsonToken.EndConstructor;
				}
			}
			throw JsonWriterException.Create(this, string.Concat("No close token for type: ", type), null);
		}

		internal void InternalWriteComment()
		{
			this.AutoComplete(JsonToken.Comment);
		}

		internal void InternalWriteEnd(JsonContainerType container)
		{
			this.AutoCompleteClose(container);
		}

		internal void InternalWritePropertyName(string name)
		{
			this._currentPosition.PropertyName = name;
			this.AutoComplete(JsonToken.PropertyName);
		}

		internal void InternalWriteRaw()
		{
		}

		internal void InternalWriteStart(JsonToken token, JsonContainerType container)
		{
			this.UpdateScopeWithFinishedValue();
			this.AutoComplete(token);
			this.Push(container);
		}

		internal void InternalWriteValue(JsonToken token)
		{
			this.UpdateScopeWithFinishedValue();
			this.AutoComplete(token);
		}

		internal void InternalWriteWhitespace(string ws)
		{
			if (ws != null && !StringUtils.IsWhiteSpace(ws))
			{
				throw JsonWriterException.Create(this, "Only white space characters should be used.", null);
			}
		}

		internal virtual void OnStringEscapeHandlingChanged()
		{
		}

		private JsonContainerType Peek()
		{
			return this._currentPosition.Type;
		}

		private JsonContainerType Pop()
		{
			JsonPosition jsonPosition = this._currentPosition;
			if (this._stack == null || this._stack.Count <= 0)
			{
				this._currentPosition = new JsonPosition();
			}
			else
			{
				this._currentPosition = this._stack[this._stack.Count - 1];
				this._stack.RemoveAt(this._stack.Count - 1);
			}
			return jsonPosition.Type;
		}

		private void Push(JsonContainerType value)
		{
			if (this._currentPosition.Type != JsonContainerType.None)
			{
				if (this._stack == null)
				{
					this._stack = new List<JsonPosition>();
				}
				this._stack.Add(this._currentPosition);
			}
			this._currentPosition = new JsonPosition(value);
		}

		protected void SetWriteState(JsonToken token, object value)
		{
			switch (token)
			{
				case JsonToken.StartObject:
				{
					this.InternalWriteStart(token, JsonContainerType.Object);
					return;
				}
				case JsonToken.StartArray:
				{
					this.InternalWriteStart(token, JsonContainerType.Array);
					return;
				}
				case JsonToken.StartConstructor:
				{
					this.InternalWriteStart(token, JsonContainerType.Constructor);
					return;
				}
				case JsonToken.PropertyName:
				{
					if (!(value is string))
					{
						throw new ArgumentException("A name is required when setting property name state.", "value");
					}
					this.InternalWritePropertyName((string)value);
					return;
				}
				case JsonToken.Comment:
				{
					this.InternalWriteComment();
					return;
				}
				case JsonToken.Raw:
				{
					this.InternalWriteRaw();
					return;
				}
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.String:
				case JsonToken.Boolean:
				case JsonToken.Null:
				case JsonToken.Undefined:
				case JsonToken.Date:
				case JsonToken.Bytes:
				{
					this.InternalWriteValue(token);
					return;
				}
				case JsonToken.EndObject:
				{
					this.InternalWriteEnd(JsonContainerType.Object);
					return;
				}
				case JsonToken.EndArray:
				{
					this.InternalWriteEnd(JsonContainerType.Array);
					return;
				}
				case JsonToken.EndConstructor:
				{
					this.InternalWriteEnd(JsonContainerType.Constructor);
					return;
				}
			}
			throw new ArgumentOutOfRangeException("token");
		}

		void System.IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal void UpdateScopeWithFinishedValue()
		{
			if (this._currentPosition.HasIndex)
			{
				this._currentPosition.Position++;
			}
		}

		public virtual void WriteComment(string text)
		{
			this.InternalWriteComment();
		}

		private void WriteConstructorDate(JsonReader reader)
		{
			if (!reader.Read())
			{
				throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
			}
			if (reader.TokenType != JsonToken.Integer)
			{
				throw JsonWriterException.Create(this, string.Concat("Unexpected token when reading date constructor. Expected Integer, got ", reader.TokenType), null);
			}
			DateTime dateTime = DateTimeUtils.ConvertJavaScriptTicksToDateTime((long)reader.Value);
			if (!reader.Read())
			{
				throw JsonWriterException.Create(this, "Unexpected end when reading date constructor.", null);
			}
			if (reader.TokenType != JsonToken.EndConstructor)
			{
				throw JsonWriterException.Create(this, string.Concat("Unexpected token when reading date constructor. Expected EndConstructor, got ", reader.TokenType), null);
			}
			this.WriteValue(dateTime);
		}

		public virtual void WriteEnd()
		{
			this.WriteEnd(this.Peek());
		}

		private void WriteEnd(JsonContainerType type)
		{
			switch (type)
			{
				case JsonContainerType.Object:
				{
					this.WriteEndObject();
					return;
				}
				case JsonContainerType.Array:
				{
					this.WriteEndArray();
					return;
				}
				case JsonContainerType.Constructor:
				{
					this.WriteEndConstructor();
					return;
				}
			}
			throw JsonWriterException.Create(this, string.Concat("Unexpected type when writing end: ", type), null);
		}

		protected virtual void WriteEnd(JsonToken token)
		{
		}

		public virtual void WriteEndArray()
		{
			this.InternalWriteEnd(JsonContainerType.Array);
		}

		public virtual void WriteEndConstructor()
		{
			this.InternalWriteEnd(JsonContainerType.Constructor);
		}

		public virtual void WriteEndObject()
		{
			this.InternalWriteEnd(JsonContainerType.Object);
		}

		protected virtual void WriteIndent()
		{
		}

		protected virtual void WriteIndentSpace()
		{
		}

		public virtual void WriteNull()
		{
			this.InternalWriteValue(JsonToken.Null);
		}

		public virtual void WritePropertyName(string name)
		{
			this.InternalWritePropertyName(name);
		}

		public virtual void WritePropertyName(string name, bool escape)
		{
			this.WritePropertyName(name);
		}

		public virtual void WriteRaw(string json)
		{
			this.InternalWriteRaw();
		}

		public virtual void WriteRawValue(string json)
		{
			this.UpdateScopeWithFinishedValue();
			this.AutoComplete(JsonToken.Undefined);
			this.WriteRaw(json);
		}

		public virtual void WriteStartArray()
		{
			this.InternalWriteStart(JsonToken.StartArray, JsonContainerType.Array);
		}

		public virtual void WriteStartConstructor(string name)
		{
			this.InternalWriteStart(JsonToken.StartConstructor, JsonContainerType.Constructor);
		}

		public virtual void WriteStartObject()
		{
			this.InternalWriteStart(JsonToken.StartObject, JsonContainerType.Object);
		}

		public void WriteToken(JsonReader reader)
		{
			this.WriteToken(reader, true);
		}

		public void WriteToken(JsonReader reader, bool writeChildren)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			this.WriteToken(reader, writeChildren, true, true);
		}

		public void WriteToken(JsonToken token, object value)
		{
			string str;
			string str1;
			switch (token)
			{
				case JsonToken.None:
				{
					return;
				}
				case JsonToken.StartObject:
				{
					this.WriteStartObject();
					return;
				}
				case JsonToken.StartArray:
				{
					this.WriteStartArray();
					return;
				}
				case JsonToken.StartConstructor:
				{
					ValidationUtils.ArgumentNotNull(value, "value");
					this.WriteStartConstructor(value.ToString());
					return;
				}
				case JsonToken.PropertyName:
				{
					ValidationUtils.ArgumentNotNull(value, "value");
					this.WritePropertyName(value.ToString());
					return;
				}
				case JsonToken.Comment:
				{
					if (value != null)
					{
						str = value.ToString();
					}
					else
					{
						str = null;
					}
					this.WriteComment(str);
					return;
				}
				case JsonToken.Raw:
				{
					if (value != null)
					{
						str1 = value.ToString();
					}
					else
					{
						str1 = null;
					}
					this.WriteRawValue(str1);
					return;
				}
				case JsonToken.Integer:
				{
					ValidationUtils.ArgumentNotNull(value, "value");
					this.WriteValue(Convert.ToInt64(value, CultureInfo.InvariantCulture));
					return;
				}
				case JsonToken.Float:
				{
					ValidationUtils.ArgumentNotNull(value, "value");
					if (value is decimal)
					{
						this.WriteValue((decimal)value);
						return;
					}
					if (value is double)
					{
						this.WriteValue((double)value);
						return;
					}
					if (value is float)
					{
						this.WriteValue((float)value);
						return;
					}
					this.WriteValue(Convert.ToDouble(value, CultureInfo.InvariantCulture));
					return;
				}
				case JsonToken.String:
				{
					ValidationUtils.ArgumentNotNull(value, "value");
					this.WriteValue(value.ToString());
					return;
				}
				case JsonToken.Boolean:
				{
					ValidationUtils.ArgumentNotNull(value, "value");
					this.WriteValue(Convert.ToBoolean(value, CultureInfo.InvariantCulture));
					return;
				}
				case JsonToken.Null:
				{
					this.WriteNull();
					return;
				}
				case JsonToken.Undefined:
				{
					this.WriteUndefined();
					return;
				}
				case JsonToken.EndObject:
				{
					this.WriteEndObject();
					return;
				}
				case JsonToken.EndArray:
				{
					this.WriteEndArray();
					return;
				}
				case JsonToken.EndConstructor:
				{
					this.WriteEndConstructor();
					return;
				}
				case JsonToken.Date:
				{
					ValidationUtils.ArgumentNotNull(value, "value");
					if (value is DateTimeOffset)
					{
						this.WriteValue((DateTimeOffset)value);
						return;
					}
					this.WriteValue(Convert.ToDateTime(value, CultureInfo.InvariantCulture));
					return;
				}
				case JsonToken.Bytes:
				{
					ValidationUtils.ArgumentNotNull(value, "value");
					if (value is Guid)
					{
						this.WriteValue((Guid)value);
						return;
					}
					this.WriteValue((byte[])value);
					return;
				}
			}
			throw MiscellaneousUtils.CreateArgumentOutOfRangeException("token", token, "Unexpected token type.");
		}

		public void WriteToken(JsonToken token)
		{
			this.WriteToken(token, null);
		}

		internal virtual void WriteToken(JsonReader reader, bool writeChildren, bool writeDateConstructorAsDate, bool writeComments)
		{
			int num;
			int depth;
			int num1;
			int num2;
			if (reader.TokenType != JsonToken.None)
			{
				num = (JsonTokenUtils.IsStartToken(reader.TokenType) ? reader.Depth : reader.Depth + 1);
			}
			else
			{
				num = -1;
			}
			do
			{
				if (writeDateConstructorAsDate && reader.TokenType == JsonToken.StartConstructor && string.Equals(reader.Value.ToString(), "Date", StringComparison.Ordinal))
				{
					this.WriteConstructorDate(reader);
				}
				else if (writeComments || reader.TokenType != JsonToken.Comment)
				{
					this.WriteToken(reader.TokenType, reader.Value);
				}
				num2 = num - 1;
				depth = reader.Depth;
				num1 = (JsonTokenUtils.IsEndToken(reader.TokenType) ? 1 : 0);
			}
			while (num2 < depth - num1 & writeChildren && reader.Read());
		}

		public virtual void WriteUndefined()
		{
			this.InternalWriteValue(JsonToken.Undefined);
		}

		public virtual void WriteValue(string value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		public virtual void WriteValue(int value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(uint value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		public virtual void WriteValue(long value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ulong value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		public virtual void WriteValue(float value)
		{
			this.InternalWriteValue(JsonToken.Float);
		}

		public virtual void WriteValue(double value)
		{
			this.InternalWriteValue(JsonToken.Float);
		}

		public virtual void WriteValue(bool value)
		{
			this.InternalWriteValue(JsonToken.Boolean);
		}

		public virtual void WriteValue(short value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ushort value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		public virtual void WriteValue(char value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		public virtual void WriteValue(byte value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(sbyte value)
		{
			this.InternalWriteValue(JsonToken.Integer);
		}

		public virtual void WriteValue(decimal value)
		{
			this.InternalWriteValue(JsonToken.Float);
		}

		public virtual void WriteValue(DateTime value)
		{
			this.InternalWriteValue(JsonToken.Date);
		}

		public virtual void WriteValue(DateTimeOffset value)
		{
			this.InternalWriteValue(JsonToken.Date);
		}

		public virtual void WriteValue(Guid value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		public virtual void WriteValue(TimeSpan value)
		{
			this.InternalWriteValue(JsonToken.String);
		}

		public virtual void WriteValue(int? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(uint? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(long? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ulong? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(float? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(double? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(bool? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(short? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(ushort? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(char? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(byte? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		[CLSCompliant(false)]
		public virtual void WriteValue(sbyte? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(decimal? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(DateTime? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(DateTimeOffset? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(Guid? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(TimeSpan? value)
		{
			if (!value.HasValue)
			{
				this.WriteNull();
				return;
			}
			this.WriteValue(value.GetValueOrDefault());
		}

		public virtual void WriteValue(byte[] value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.InternalWriteValue(JsonToken.Bytes);
		}

		public virtual void WriteValue(Uri value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			this.InternalWriteValue(JsonToken.String);
		}

		public virtual void WriteValue(object value)
		{
			if (value == null)
			{
				this.WriteNull();
				return;
			}
			JsonWriter.WriteValue(this, ConvertUtils.GetTypeCode(value.GetType()), value);
		}

		internal static void WriteValue(JsonWriter writer, PrimitiveTypeCode typeCode, object value)
		{
			TypeInformation typeInformation;
			PrimitiveTypeCode primitiveTypeCode;
			object type;
			char? nullable;
			bool? nullable1;
			sbyte? nullable2;
			short? nullable3;
			ushort? nullable4;
			int? nullable5;
			byte? nullable6;
			uint? nullable7;
			long? nullable8;
			ulong? nullable9;
			float? nullable10;
			double? nullable11;
			DateTime? nullable12;
			DateTimeOffset? nullable13;
			decimal? nullable14;
			Guid? nullable15;
			TimeSpan? nullable16;
			IConvertible convertible;
			PrimitiveTypeCode primitiveTypeCode1;
			Type type1;
			switch (typeCode)
			{
				case PrimitiveTypeCode.Char:
				{
					writer.WriteValue((char)value);
					return;
				}
				case PrimitiveTypeCode.CharNullable:
				{
					JsonWriter jsonWriter = writer;
					if (value == null)
					{
						nullable = null;
					}
					else
					{
						nullable = new char?((char)value);
					}
					jsonWriter.WriteValue(nullable);
					return;
				}
				case PrimitiveTypeCode.Boolean:
				{
					writer.WriteValue((bool)value);
					return;
				}
				case PrimitiveTypeCode.BooleanNullable:
				{
					JsonWriter jsonWriter1 = writer;
					if (value == null)
					{
						nullable1 = null;
					}
					else
					{
						nullable1 = new bool?((bool)value);
					}
					jsonWriter1.WriteValue(nullable1);
					return;
				}
				case PrimitiveTypeCode.SByte:
				{
					writer.WriteValue((sbyte)value);
					return;
				}
				case PrimitiveTypeCode.SByteNullable:
				{
					JsonWriter jsonWriter2 = writer;
					if (value == null)
					{
						nullable2 = null;
					}
					else
					{
						nullable2 = new sbyte?((sbyte)value);
					}
					jsonWriter2.WriteValue(nullable2);
					return;
				}
				case PrimitiveTypeCode.Int16:
				{
					writer.WriteValue((short)value);
					return;
				}
				case PrimitiveTypeCode.Int16Nullable:
				{
					JsonWriter jsonWriter3 = writer;
					if (value == null)
					{
						nullable3 = null;
					}
					else
					{
						nullable3 = new short?((short)value);
					}
					jsonWriter3.WriteValue(nullable3);
					return;
				}
				case PrimitiveTypeCode.UInt16:
				{
					writer.WriteValue((ushort)value);
					return;
				}
				case PrimitiveTypeCode.UInt16Nullable:
				{
					JsonWriter jsonWriter4 = writer;
					if (value == null)
					{
						nullable4 = null;
					}
					else
					{
						nullable4 = new ushort?((ushort)value);
					}
					jsonWriter4.WriteValue(nullable4);
					return;
				}
				case PrimitiveTypeCode.Int32:
				{
					writer.WriteValue((int)value);
					return;
				}
				case PrimitiveTypeCode.Int32Nullable:
				{
					JsonWriter jsonWriter5 = writer;
					if (value == null)
					{
						nullable5 = null;
					}
					else
					{
						nullable5 = new int?((int)value);
					}
					jsonWriter5.WriteValue(nullable5);
					return;
				}
				case PrimitiveTypeCode.Byte:
				{
					writer.WriteValue((byte)value);
					return;
				}
				case PrimitiveTypeCode.ByteNullable:
				{
					JsonWriter jsonWriter6 = writer;
					if (value == null)
					{
						nullable6 = null;
					}
					else
					{
						nullable6 = new byte?((byte)value);
					}
					jsonWriter6.WriteValue(nullable6);
					return;
				}
				case PrimitiveTypeCode.UInt32:
				{
					writer.WriteValue((uint)value);
					return;
				}
				case PrimitiveTypeCode.UInt32Nullable:
				{
					JsonWriter jsonWriter7 = writer;
					if (value == null)
					{
						nullable7 = null;
					}
					else
					{
						nullable7 = new uint?((uint)value);
					}
					jsonWriter7.WriteValue(nullable7);
					return;
				}
				case PrimitiveTypeCode.Int64:
				{
					writer.WriteValue((long)value);
					return;
				}
				case PrimitiveTypeCode.Int64Nullable:
				{
					JsonWriter jsonWriter8 = writer;
					if (value == null)
					{
						nullable8 = null;
					}
					else
					{
						nullable8 = new long?((long)value);
					}
					jsonWriter8.WriteValue(nullable8);
					return;
				}
				case PrimitiveTypeCode.UInt64:
				{
					writer.WriteValue((ulong)value);
					return;
				}
				case PrimitiveTypeCode.UInt64Nullable:
				{
					JsonWriter jsonWriter9 = writer;
					if (value == null)
					{
						nullable9 = null;
					}
					else
					{
						nullable9 = new ulong?((ulong)value);
					}
					jsonWriter9.WriteValue(nullable9);
					return;
				}
				case PrimitiveTypeCode.Single:
				{
					writer.WriteValue((float)value);
					return;
				}
				case PrimitiveTypeCode.SingleNullable:
				{
					JsonWriter jsonWriter10 = writer;
					if (value == null)
					{
						nullable10 = null;
					}
					else
					{
						nullable10 = new float?((float)value);
					}
					jsonWriter10.WriteValue(nullable10);
					return;
				}
				case PrimitiveTypeCode.Double:
				{
					writer.WriteValue((double)value);
					return;
				}
				case PrimitiveTypeCode.DoubleNullable:
				{
					JsonWriter jsonWriter11 = writer;
					if (value == null)
					{
						nullable11 = null;
					}
					else
					{
						nullable11 = new double?((double)value);
					}
					jsonWriter11.WriteValue(nullable11);
					return;
				}
				case PrimitiveTypeCode.DateTime:
				{
					writer.WriteValue((DateTime)value);
					return;
				}
				case PrimitiveTypeCode.DateTimeNullable:
				{
					JsonWriter jsonWriter12 = writer;
					if (value == null)
					{
						nullable12 = null;
					}
					else
					{
						nullable12 = new DateTime?((DateTime)value);
					}
					jsonWriter12.WriteValue(nullable12);
					return;
				}
				case PrimitiveTypeCode.DateTimeOffset:
				{
					writer.WriteValue((DateTimeOffset)value);
					return;
				}
				case PrimitiveTypeCode.DateTimeOffsetNullable:
				{
					JsonWriter jsonWriter13 = writer;
					if (value == null)
					{
						nullable13 = null;
					}
					else
					{
						nullable13 = new DateTimeOffset?((DateTimeOffset)value);
					}
					jsonWriter13.WriteValue(nullable13);
					return;
				}
				case PrimitiveTypeCode.Decimal:
				{
					writer.WriteValue((decimal)value);
					return;
				}
				case PrimitiveTypeCode.DecimalNullable:
				{
					JsonWriter jsonWriter14 = writer;
					if (value == null)
					{
						nullable14 = null;
					}
					else
					{
						nullable14 = new decimal?((decimal)value);
					}
					jsonWriter14.WriteValue(nullable14);
					return;
				}
				case PrimitiveTypeCode.Guid:
				{
					writer.WriteValue((Guid)value);
					return;
				}
				case PrimitiveTypeCode.GuidNullable:
				{
					JsonWriter jsonWriter15 = writer;
					if (value == null)
					{
						nullable15 = null;
					}
					else
					{
						nullable15 = new Guid?((Guid)value);
					}
					jsonWriter15.WriteValue(nullable15);
					return;
				}
				case PrimitiveTypeCode.TimeSpan:
				{
					writer.WriteValue((TimeSpan)value);
					return;
				}
				case PrimitiveTypeCode.TimeSpanNullable:
				{
					JsonWriter jsonWriter16 = writer;
					if (value == null)
					{
						nullable16 = null;
					}
					else
					{
						nullable16 = new TimeSpan?((TimeSpan)value);
					}
					jsonWriter16.WriteValue(nullable16);
					return;
				}
				case PrimitiveTypeCode.BigInteger:
				case PrimitiveTypeCode.BigIntegerNullable:
				{
					if (!(value is IConvertible))
					{
						throw JsonWriter.CreateUnsupportedTypeException(writer, value);
					}
					convertible = (IConvertible)value;
					typeInformation = ConvertUtils.GetTypeInformation(convertible);
					primitiveTypeCode1 = (typeInformation.TypeCode == PrimitiveTypeCode.Object ? PrimitiveTypeCode.String : typeInformation.TypeCode);
					primitiveTypeCode = primitiveTypeCode1;
					type1 = (typeInformation.TypeCode == PrimitiveTypeCode.Object ? typeof(string) : typeInformation.Type);
					type = convertible.ToType(type1, CultureInfo.InvariantCulture);
					JsonWriter.WriteValue(writer, primitiveTypeCode, type);
					return;
				}
				case PrimitiveTypeCode.Uri:
				{
					writer.WriteValue((Uri)value);
					return;
				}
				case PrimitiveTypeCode.String:
				{
					writer.WriteValue((string)value);
					return;
				}
				case PrimitiveTypeCode.Bytes:
				{
					writer.WriteValue((byte[])value);
					return;
				}
				case PrimitiveTypeCode.DBNull:
				{
					writer.WriteNull();
					return;
				}
				default:
				{
					if (!(value is IConvertible))
					{
						throw JsonWriter.CreateUnsupportedTypeException(writer, value);
					}
					convertible = (IConvertible)value;
					typeInformation = ConvertUtils.GetTypeInformation(convertible);
					primitiveTypeCode1 = (typeInformation.TypeCode == PrimitiveTypeCode.Object ? PrimitiveTypeCode.String : typeInformation.TypeCode);
					primitiveTypeCode = primitiveTypeCode1;
					type1 = (typeInformation.TypeCode == PrimitiveTypeCode.Object ? typeof(string) : typeInformation.Type);
					type = convertible.ToType(type1, CultureInfo.InvariantCulture);
					JsonWriter.WriteValue(writer, primitiveTypeCode, type);
					return;
				}
			}
		}

		protected virtual void WriteValueDelimiter()
		{
		}

		public virtual void WriteWhitespace(string ws)
		{
			this.InternalWriteWhitespace(ws);
		}

		internal enum State
		{
			Start,
			Property,
			ObjectStart,
			Object,
			ArrayStart,
			Array,
			ConstructorStart,
			Constructor,
			Closed,
			Error
		}
	}
}