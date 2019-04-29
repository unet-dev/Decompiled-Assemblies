using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Newtonsoft.Json.Bson
{
	[Preserve]
	public class BsonReader : JsonReader
	{
		private const int MaxCharBytesSize = 128;

		private readonly static byte[] SeqRange1;

		private readonly static byte[] SeqRange2;

		private readonly static byte[] SeqRange3;

		private readonly static byte[] SeqRange4;

		private readonly BinaryReader _reader;

		private readonly List<BsonReader.ContainerContext> _stack;

		private byte[] _byteBuffer;

		private char[] _charBuffer;

		private BsonType _currentElementType;

		private BsonReader.BsonReaderState _bsonReaderState;

		private BsonReader.ContainerContext _currentContext;

		private bool _readRootValueAsArray;

		private bool _jsonNet35BinaryCompatibility;

		private DateTimeKind _dateTimeKindHandling;

		public DateTimeKind DateTimeKindHandling
		{
			get
			{
				return this._dateTimeKindHandling;
			}
			set
			{
				this._dateTimeKindHandling = value;
			}
		}

		[Obsolete("JsonNet35BinaryCompatibility will be removed in a future version of Json.NET.")]
		public bool JsonNet35BinaryCompatibility
		{
			get
			{
				return this._jsonNet35BinaryCompatibility;
			}
			set
			{
				this._jsonNet35BinaryCompatibility = value;
			}
		}

		public bool ReadRootValueAsArray
		{
			get
			{
				return this._readRootValueAsArray;
			}
			set
			{
				this._readRootValueAsArray = value;
			}
		}

		static BsonReader()
		{
			BsonReader.SeqRange1 = new byte[] { 0, 127 };
			BsonReader.SeqRange2 = new byte[] { 194, 223 };
			BsonReader.SeqRange3 = new byte[] { 224, 239 };
			BsonReader.SeqRange4 = new byte[] { 240, 244 };
		}

		public BsonReader(Stream stream) : this(stream, false, DateTimeKind.Local)
		{
		}

		public BsonReader(BinaryReader reader) : this(reader, false, DateTimeKind.Local)
		{
		}

		public BsonReader(Stream stream, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
		{
			ValidationUtils.ArgumentNotNull(stream, "stream");
			this._reader = new BinaryReader(stream);
			this._stack = new List<BsonReader.ContainerContext>();
			this._readRootValueAsArray = readRootValueAsArray;
			this._dateTimeKindHandling = dateTimeKindHandling;
		}

		public BsonReader(BinaryReader reader, bool readRootValueAsArray, DateTimeKind dateTimeKindHandling)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			this._reader = reader;
			this._stack = new List<BsonReader.ContainerContext>();
			this._readRootValueAsArray = readRootValueAsArray;
			this._dateTimeKindHandling = dateTimeKindHandling;
		}

		private int BytesInSequence(byte b)
		{
			if (b <= BsonReader.SeqRange1[1])
			{
				return 1;
			}
			if (b >= BsonReader.SeqRange2[0] && b <= BsonReader.SeqRange2[1])
			{
				return 2;
			}
			if (b >= BsonReader.SeqRange3[0] && b <= BsonReader.SeqRange3[1])
			{
				return 3;
			}
			if (b >= BsonReader.SeqRange4[0] && b <= BsonReader.SeqRange4[1])
			{
				return 4;
			}
			return 0;
		}

		public override void Close()
		{
			base.Close();
			if (base.CloseInput && this._reader != null)
			{
				this._reader.Close();
			}
		}

		private void EnsureBuffers()
		{
			if (this._byteBuffer == null)
			{
				this._byteBuffer = new byte[128];
			}
			if (this._charBuffer == null)
			{
				int maxCharCount = Encoding.UTF8.GetMaxCharCount(128);
				this._charBuffer = new char[maxCharCount];
			}
		}

		private int GetLastFullCharStop(int start)
		{
			int num = start;
			int num1 = 0;
			while (num >= 0)
			{
				num1 = this.BytesInSequence(this._byteBuffer[num]);
				if (num1 != 0)
				{
					if (num1 == 1)
					{
						break;
					}
					num--;
					break;
				}
				else
				{
					num--;
				}
			}
			if (num1 == start - num)
			{
				return start;
			}
			return num;
		}

		private string GetString(int length)
		{
			if (length == 0)
			{
				return string.Empty;
			}
			this.EnsureBuffers();
			StringBuilder stringBuilder = null;
			int num = 0;
			int num1 = 0;
			do
			{
				int num2 = this._reader.Read(this._byteBuffer, num1, (length - num > 128 - num1 ? 128 - num1 : length - num));
				if (num2 == 0)
				{
					throw new EndOfStreamException("Unable to read beyond the end of the stream.");
				}
				num += num2;
				num2 += num1;
				if (num2 == length)
				{
					int chars = Encoding.UTF8.GetChars(this._byteBuffer, 0, num2, this._charBuffer, 0);
					return new string(this._charBuffer, 0, chars);
				}
				int lastFullCharStop = this.GetLastFullCharStop(num2 - 1);
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(length);
				}
				int chars1 = Encoding.UTF8.GetChars(this._byteBuffer, 0, lastFullCharStop + 1, this._charBuffer, 0);
				stringBuilder.Append(this._charBuffer, 0, chars1);
				if (lastFullCharStop >= num2 - 1)
				{
					num1 = 0;
				}
				else
				{
					num1 = num2 - lastFullCharStop - 1;
					Array.Copy(this._byteBuffer, lastFullCharStop + 1, this._byteBuffer, 0, num1);
				}
			}
			while (num < length);
			return stringBuilder.ToString();
		}

		private void MovePosition(int count)
		{
			this._currentContext.Position += count;
		}

		private void PopContext()
		{
			this._stack.RemoveAt(this._stack.Count - 1);
			if (this._stack.Count == 0)
			{
				this._currentContext = null;
				return;
			}
			this._currentContext = this._stack[this._stack.Count - 1];
		}

		private void PushContext(BsonReader.ContainerContext newContext)
		{
			this._stack.Add(newContext);
			this._currentContext = newContext;
		}

		public override bool Read()
		{
			bool flag;
			bool flag1;
			try
			{
				switch (this._bsonReaderState)
				{
					case BsonReader.BsonReaderState.Normal:
					{
						flag = this.ReadNormal();
						break;
					}
					case BsonReader.BsonReaderState.ReferenceStart:
					case BsonReader.BsonReaderState.ReferenceRef:
					case BsonReader.BsonReaderState.ReferenceId:
					{
						flag = this.ReadReference();
						break;
					}
					case BsonReader.BsonReaderState.CodeWScopeStart:
					case BsonReader.BsonReaderState.CodeWScopeCode:
					case BsonReader.BsonReaderState.CodeWScopeScope:
					case BsonReader.BsonReaderState.CodeWScopeScopeObject:
					case BsonReader.BsonReaderState.CodeWScopeScopeEnd:
					{
						flag = this.ReadCodeWScope();
						break;
					}
					default:
					{
						throw JsonReaderException.Create(this, "Unexpected state: {0}".FormatWith(CultureInfo.InvariantCulture, this._bsonReaderState));
					}
				}
				if (flag)
				{
					flag1 = true;
				}
				else
				{
					base.SetToken(JsonToken.None);
					flag1 = false;
				}
			}
			catch (EndOfStreamException endOfStreamException)
			{
				base.SetToken(JsonToken.None);
				flag1 = false;
			}
			return flag1;
		}

		private byte[] ReadBinary(out BsonBinaryType binaryType)
		{
			int num = this.ReadInt32();
			binaryType = (BsonBinaryType)this.ReadByte();
			if ((byte)binaryType == 2 && !this._jsonNet35BinaryCompatibility)
			{
				num = this.ReadInt32();
			}
			return this.ReadBytes(num);
		}

		private byte ReadByte()
		{
			this.MovePosition(1);
			return this._reader.ReadByte();
		}

		private byte[] ReadBytes(int count)
		{
			this.MovePosition(count);
			return this._reader.ReadBytes(count);
		}

		private bool ReadCodeWScope()
		{
			switch (this._bsonReaderState)
			{
				case BsonReader.BsonReaderState.CodeWScopeStart:
				{
					base.SetToken(JsonToken.PropertyName, "$code");
					this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeCode;
					return true;
				}
				case BsonReader.BsonReaderState.CodeWScopeCode:
				{
					this.ReadInt32();
					base.SetToken(JsonToken.String, this.ReadLengthString());
					this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScope;
					return true;
				}
				case BsonReader.BsonReaderState.CodeWScopeScope:
				{
					if (base.CurrentState == JsonReader.State.PostValue)
					{
						base.SetToken(JsonToken.PropertyName, "$scope");
						return true;
					}
					base.SetToken(JsonToken.StartObject);
					this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScopeObject;
					BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(BsonType.Object);
					this.PushContext(containerContext);
					containerContext.Length = this.ReadInt32();
					return true;
				}
				case BsonReader.BsonReaderState.CodeWScopeScopeObject:
				{
					bool flag = this.ReadNormal();
					if (flag && this.TokenType == JsonToken.EndObject)
					{
						this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeScopeEnd;
					}
					return flag;
				}
				case BsonReader.BsonReaderState.CodeWScopeScopeEnd:
				{
					base.SetToken(JsonToken.EndObject);
					this._bsonReaderState = BsonReader.BsonReaderState.Normal;
					return true;
				}
			}
			throw new ArgumentOutOfRangeException();
		}

		private double ReadDouble()
		{
			this.MovePosition(8);
			return this._reader.ReadDouble();
		}

		private string ReadElement()
		{
			this._currentElementType = this.ReadType();
			return this.ReadString();
		}

		private int ReadInt32()
		{
			this.MovePosition(4);
			return this._reader.ReadInt32();
		}

		private long ReadInt64()
		{
			this.MovePosition(8);
			return this._reader.ReadInt64();
		}

		private string ReadLengthString()
		{
			int num = this.ReadInt32();
			this.MovePosition(num);
			string str = this.GetString(num - 1);
			this._reader.ReadByte();
			return str;
		}

		private bool ReadNormal()
		{
			BsonType bsonType;
			switch (base.CurrentState)
			{
				case JsonReader.State.Start:
				{
					JsonToken jsonToken = (!this._readRootValueAsArray ? JsonToken.StartObject : JsonToken.StartArray);
					bsonType = (!this._readRootValueAsArray ? BsonType.Object : BsonType.Array);
					base.SetToken(jsonToken);
					BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(bsonType);
					this.PushContext(containerContext);
					containerContext.Length = this.ReadInt32();
					return true;
				}
				case JsonReader.State.Complete:
				case JsonReader.State.Closed:
				{
					return false;
				}
				case JsonReader.State.Property:
				{
					this.ReadType(this._currentElementType);
					return true;
				}
				case JsonReader.State.ObjectStart:
				case JsonReader.State.ArrayStart:
				case JsonReader.State.PostValue:
				{
					BsonReader.ContainerContext containerContext1 = this._currentContext;
					if (containerContext1 == null)
					{
						return false;
					}
					int length = containerContext1.Length - 1;
					if (containerContext1.Position < length)
					{
						if (containerContext1.Type != BsonType.Array)
						{
							base.SetToken(JsonToken.PropertyName, this.ReadElement());
							return true;
						}
						this.ReadElement();
						this.ReadType(this._currentElementType);
						return true;
					}
					if (containerContext1.Position != length)
					{
						throw JsonReaderException.Create(this, "Read past end of current container context.");
					}
					if (this.ReadByte() != 0)
					{
						throw JsonReaderException.Create(this, "Unexpected end of object byte value.");
					}
					this.PopContext();
					if (this._currentContext != null)
					{
						this.MovePosition(containerContext1.Length);
					}
					base.SetToken((containerContext1.Type == BsonType.Object ? JsonToken.EndObject : JsonToken.EndArray));
					return true;
				}
				case JsonReader.State.Object:
				case JsonReader.State.Array:
				{
					throw new ArgumentOutOfRangeException();
				}
				case JsonReader.State.ConstructorStart:
				case JsonReader.State.Constructor:
				case JsonReader.State.Error:
				case JsonReader.State.Finished:
				{
					return false;
				}
				default:
				{
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		private bool ReadReference()
		{
			JsonReader.State currentState = base.CurrentState;
			if (currentState == JsonReader.State.Property)
			{
				if (this._bsonReaderState == BsonReader.BsonReaderState.ReferenceRef)
				{
					base.SetToken(JsonToken.String, this.ReadLengthString());
					return true;
				}
				if (this._bsonReaderState != BsonReader.BsonReaderState.ReferenceId)
				{
					throw JsonReaderException.Create(this, string.Concat("Unexpected state when reading BSON reference: ", this._bsonReaderState));
				}
				base.SetToken(JsonToken.Bytes, this.ReadBytes(12));
				return true;
			}
			if (currentState == JsonReader.State.ObjectStart)
			{
				base.SetToken(JsonToken.PropertyName, "$ref");
				this._bsonReaderState = BsonReader.BsonReaderState.ReferenceRef;
				return true;
			}
			if (currentState != JsonReader.State.PostValue)
			{
				throw JsonReaderException.Create(this, string.Concat("Unexpected state when reading BSON reference: ", base.CurrentState));
			}
			if (this._bsonReaderState == BsonReader.BsonReaderState.ReferenceRef)
			{
				base.SetToken(JsonToken.PropertyName, "$id");
				this._bsonReaderState = BsonReader.BsonReaderState.ReferenceId;
				return true;
			}
			if (this._bsonReaderState != BsonReader.BsonReaderState.ReferenceId)
			{
				throw JsonReaderException.Create(this, string.Concat("Unexpected state when reading BSON reference: ", this._bsonReaderState));
			}
			base.SetToken(JsonToken.EndObject);
			this._bsonReaderState = BsonReader.BsonReaderState.Normal;
			return true;
		}

		private string ReadString()
		{
			this.EnsureBuffers();
			StringBuilder stringBuilder = null;
			int num = 0;
			int num1 = 0;
			while (true)
			{
				int num2 = num1;
				while (num2 < 128)
				{
					byte num3 = this._reader.ReadByte();
					byte num4 = num3;
					if (num3 <= 0)
					{
						break;
					}
					int num5 = num2;
					num2 = num5 + 1;
					this._byteBuffer[num5] = num4;
				}
				int num6 = num2 - num1;
				num += num6;
				if (num2 < 128 && stringBuilder == null)
				{
					int chars = Encoding.UTF8.GetChars(this._byteBuffer, 0, num6, this._charBuffer, 0);
					this.MovePosition(num + 1);
					return new string(this._charBuffer, 0, chars);
				}
				int lastFullCharStop = this.GetLastFullCharStop(num2 - 1);
				int chars1 = Encoding.UTF8.GetChars(this._byteBuffer, 0, lastFullCharStop + 1, this._charBuffer, 0);
				if (stringBuilder == null)
				{
					stringBuilder = new StringBuilder(256);
				}
				stringBuilder.Append(this._charBuffer, 0, chars1);
				if (lastFullCharStop >= num6 - 1)
				{
					if (num2 < 128)
					{
						break;
					}
					num1 = 0;
				}
				else
				{
					num1 = num6 - lastFullCharStop - 1;
					Array.Copy(this._byteBuffer, lastFullCharStop + 1, this._byteBuffer, 0, num1);
				}
			}
			this.MovePosition(num + 1);
			return stringBuilder.ToString();
		}

		private void ReadType(BsonType type)
		{
			BsonBinaryType bsonBinaryType;
			DateTime dateTime;
			object guid;
			switch (type)
			{
				case BsonType.Number:
				{
					double num = this.ReadDouble();
					if (this._floatParseHandling != Newtonsoft.Json.FloatParseHandling.Decimal)
					{
						base.SetToken(JsonToken.Float, num);
						return;
					}
					base.SetToken(JsonToken.Float, Convert.ToDecimal(num, CultureInfo.InvariantCulture));
					return;
				}
				case BsonType.String:
				case BsonType.Symbol:
				{
					base.SetToken(JsonToken.String, this.ReadLengthString());
					return;
				}
				case BsonType.Object:
				{
					base.SetToken(JsonToken.StartObject);
					BsonReader.ContainerContext containerContext = new BsonReader.ContainerContext(BsonType.Object);
					this.PushContext(containerContext);
					containerContext.Length = this.ReadInt32();
					return;
				}
				case BsonType.Array:
				{
					base.SetToken(JsonToken.StartArray);
					BsonReader.ContainerContext containerContext1 = new BsonReader.ContainerContext(BsonType.Array);
					this.PushContext(containerContext1);
					containerContext1.Length = this.ReadInt32();
					return;
				}
				case BsonType.Binary:
				{
					byte[] numArray = this.ReadBinary(out bsonBinaryType);
					if (bsonBinaryType != BsonBinaryType.Uuid)
					{
						guid = numArray;
					}
					else
					{
						guid = new Guid(numArray);
					}
					base.SetToken(JsonToken.Bytes, guid);
					return;
				}
				case BsonType.Undefined:
				{
					base.SetToken(JsonToken.Undefined);
					return;
				}
				case BsonType.Oid:
				{
					base.SetToken(JsonToken.Bytes, this.ReadBytes(12));
					return;
				}
				case BsonType.Boolean:
				{
					bool flag = Convert.ToBoolean(this.ReadByte());
					base.SetToken(JsonToken.Boolean, flag);
					return;
				}
				case BsonType.Date:
				{
					DateTime dateTime1 = DateTimeUtils.ConvertJavaScriptTicksToDateTime(this.ReadInt64());
					DateTimeKind dateTimeKindHandling = this.DateTimeKindHandling;
					if (dateTimeKindHandling == DateTimeKind.Unspecified)
					{
						dateTime = DateTime.SpecifyKind(dateTime1, DateTimeKind.Unspecified);
					}
					else
					{
						dateTime = (dateTimeKindHandling == DateTimeKind.Local ? dateTime1.ToLocalTime() : dateTime1);
					}
					base.SetToken(JsonToken.Date, dateTime);
					return;
				}
				case BsonType.Null:
				{
					base.SetToken(JsonToken.Null);
					return;
				}
				case BsonType.Regex:
				{
					string str = this.ReadString();
					string str1 = this.ReadString();
					string str2 = string.Concat("/", str, "/", str1);
					base.SetToken(JsonToken.String, str2);
					return;
				}
				case BsonType.Reference:
				{
					base.SetToken(JsonToken.StartObject);
					this._bsonReaderState = BsonReader.BsonReaderState.ReferenceStart;
					return;
				}
				case BsonType.Code:
				{
					base.SetToken(JsonToken.String, this.ReadLengthString());
					return;
				}
				case BsonType.CodeWScope:
				{
					base.SetToken(JsonToken.StartObject);
					this._bsonReaderState = BsonReader.BsonReaderState.CodeWScopeStart;
					return;
				}
				case BsonType.Integer:
				{
					base.SetToken(JsonToken.Integer, (long)this.ReadInt32());
					return;
				}
				case BsonType.TimeStamp:
				case BsonType.Long:
				{
					base.SetToken(JsonToken.Integer, this.ReadInt64());
					return;
				}
			}
			throw new ArgumentOutOfRangeException("type", string.Concat("Unexpected BsonType value: ", type));
		}

		private BsonType ReadType()
		{
			this.MovePosition(1);
			return (BsonType)this._reader.ReadSByte();
		}

		private enum BsonReaderState
		{
			Normal,
			ReferenceStart,
			ReferenceRef,
			ReferenceId,
			CodeWScopeStart,
			CodeWScopeCode,
			CodeWScopeScope,
			CodeWScopeScopeObject,
			CodeWScopeScopeEnd
		}

		private class ContainerContext
		{
			public readonly BsonType Type;

			public int Length;

			public int Position;

			public ContainerContext(BsonType type)
			{
				this.Type = type;
			}
		}
	}
}