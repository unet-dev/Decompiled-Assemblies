using ProtoBuf.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProtoBuf
{
	public sealed class ProtoReader : IDisposable
	{
		internal const int TO_EOF = -1;

		private const long Int64Msb = -9223372036854775808L;

		private const int Int32Msb = -2147483648;

		private Stream source;

		private byte[] ioBuffer;

		private TypeModel model;

		private int fieldNumber;

		private int depth;

		private int dataRemaining;

		private int ioIndex;

		private int position;

		private int available;

		private int blockEnd;

		private ProtoBuf.WireType wireType;

		private bool isFixedLength;

		private bool internStrings;

		private NetObjectCache netCache;

		private uint trapCount;

		private SerializationContext context;

		private Dictionary<string, string> stringInterner;

		private readonly static UTF8Encoding encoding;

		private readonly static byte[] EmptyBlob;

		[ThreadStatic]
		private static ProtoReader lastReader;

		public SerializationContext Context
		{
			get
			{
				return this.context;
			}
		}

		public int FieldNumber
		{
			get
			{
				return this.fieldNumber;
			}
		}

		public bool InternStrings
		{
			get
			{
				return this.internStrings;
			}
			set
			{
				this.internStrings = value;
			}
		}

		public TypeModel Model
		{
			get
			{
				return this.model;
			}
		}

		internal NetObjectCache NetCache
		{
			get
			{
				return this.netCache;
			}
		}

		public int Position
		{
			get
			{
				return this.position;
			}
		}

		public ProtoBuf.WireType WireType
		{
			get
			{
				return this.wireType;
			}
		}

		static ProtoReader()
		{
			ProtoReader.encoding = new UTF8Encoding();
			ProtoReader.EmptyBlob = new byte[0];
		}

		public ProtoReader(Stream source, TypeModel model, SerializationContext context)
		{
			ProtoReader.Init(this, source, model, context, -1);
		}

		public ProtoReader(Stream source, TypeModel model, SerializationContext context, int length)
		{
			ProtoReader.Init(this, source, model, context, length);
		}

		internal static Exception AddErrorData(Exception exception, ProtoReader source)
		{
			if (exception != null && source != null && !exception.Data.Contains("protoSource"))
			{
				IDictionary data = exception.Data;
				object[] objArray = new object[] { source.fieldNumber, source.wireType, source.position, source.depth };
				data.Add("protoSource", string.Format("tag={0}; wire-type={1}; offset={2}; depth={3}", objArray));
			}
			return exception;
		}

		public static byte[] AppendBytes(byte[] value, ProtoReader reader)
		{
			int length;
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.wireType != ProtoBuf.WireType.String)
			{
				throw reader.CreateWireTypeException();
			}
			int num = (int)reader.ReadUInt32Variant(false);
			reader.wireType = ProtoBuf.WireType.None;
			if (num == 0)
			{
				if (value != null)
				{
					return value;
				}
				return ProtoReader.EmptyBlob;
			}
			if (value == null || (int)value.Length == 0)
			{
				length = 0;
				value = new byte[num];
			}
			else
			{
				length = (int)value.Length;
				byte[] numArray = new byte[(int)value.Length + num];
				Helpers.BlockCopy(value, 0, numArray, 0, (int)value.Length);
				value = numArray;
			}
			reader.position += num;
			while (num > reader.available)
			{
				if (reader.available > 0)
				{
					Helpers.BlockCopy(reader.ioBuffer, reader.ioIndex, value, length, reader.available);
					num -= reader.available;
					length += reader.available;
					int num1 = 0;
					int num2 = num1;
					reader.available = num1;
					reader.ioIndex = num2;
				}
				int num3 = (num > (int)reader.ioBuffer.Length ? (int)reader.ioBuffer.Length : num);
				if (num3 <= 0)
				{
					continue;
				}
				reader.Ensure(num3, true);
			}
			if (num > 0)
			{
				Helpers.BlockCopy(reader.ioBuffer, reader.ioIndex, value, length, num);
				reader.ioIndex += num;
				reader.available -= num;
			}
			return value;
		}

		public void AppendExtensionData(IExtensible instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			IExtension extensionObject = instance.GetExtensionObject(true);
			bool flag = false;
			Stream stream = extensionObject.BeginAppend();
			try
			{
				using (ProtoWriter protoWriter = new ProtoWriter(stream, this.model, null))
				{
					this.AppendExtensionField(protoWriter);
					protoWriter.Close();
				}
				flag = true;
			}
			finally
			{
				extensionObject.EndAppend(stream, flag);
			}
		}

		private void AppendExtensionField(ProtoWriter writer)
		{
			ProtoWriter.WriteFieldHeader(this.fieldNumber, this.wireType, writer);
			switch (this.wireType)
			{
				case ProtoBuf.WireType.None:
				case ProtoBuf.WireType.EndGroup:
				case ProtoBuf.WireType.String | ProtoBuf.WireType.EndGroup:
				case ProtoBuf.WireType.Fixed64 | ProtoBuf.WireType.String | ProtoBuf.WireType.StartGroup | ProtoBuf.WireType.EndGroup | ProtoBuf.WireType.Fixed32:
				{
					throw this.CreateWireTypeException();
				}
				case ProtoBuf.WireType.Variant:
				case ProtoBuf.WireType.Fixed64:
				case ProtoBuf.WireType.SignedVariant:
				{
					ProtoWriter.WriteInt64(this.ReadInt64(), writer);
					return;
				}
				case ProtoBuf.WireType.String:
				{
					ProtoWriter.WriteBytes(ProtoReader.AppendBytes(null, this), writer);
					return;
				}
				case ProtoBuf.WireType.StartGroup:
				{
					SubItemToken subItemToken = ProtoReader.StartSubItem(this);
					SubItemToken subItemToken1 = ProtoWriter.StartSubItem(null, writer);
					while (this.ReadFieldHeader() > 0)
					{
						this.AppendExtensionField(writer);
					}
					ProtoReader.EndSubItem(subItemToken, this);
					ProtoWriter.EndSubItem(subItemToken1, writer);
					return;
				}
				case ProtoBuf.WireType.Fixed32:
				{
					ProtoWriter.WriteInt32(this.ReadInt32(), writer);
					return;
				}
				default:
				{
					throw this.CreateWireTypeException();
				}
			}
		}

		public void Assert(ProtoBuf.WireType wireType)
		{
			if (this.wireType == wireType)
			{
				return;
			}
			if ((wireType & (ProtoBuf.WireType.Fixed64 | ProtoBuf.WireType.String | ProtoBuf.WireType.StartGroup | ProtoBuf.WireType.EndGroup | ProtoBuf.WireType.Fixed32)) != this.wireType)
			{
				throw this.CreateWireTypeException();
			}
			this.wireType = wireType;
		}

		internal void CheckFullyConsumed()
		{
			if (this.isFixedLength)
			{
				if (this.dataRemaining != 0)
				{
					throw new ProtoException("Incorrect number of bytes consumed");
				}
			}
			else if (this.available != 0)
			{
				throw new ProtoException("Unconsumed data left in the buffer; this suggests corrupt input");
			}
		}

		internal static ProtoReader Create(Stream source, TypeModel model, SerializationContext context, int len)
		{
			ProtoReader recycled = ProtoReader.GetRecycled();
			if (recycled == null)
			{
				return new ProtoReader(source, model, context, len);
			}
			ProtoReader.Init(recycled, source, model, context, len);
			return recycled;
		}

		private Exception CreateException(string message)
		{
			return ProtoReader.AddErrorData(new ProtoException(message), this);
		}

		private Exception CreateWireTypeException()
		{
			return this.CreateException("Invalid wire-type; this usually means you have over-written a file without truncating or setting the length; see http://stackoverflow.com/q/2152978/23354");
		}

		internal Type DeserializeType(string value)
		{
			return TypeModel.DeserializeType(this.model, value);
		}

		public static int DirectReadBigEndianInt32(Stream source)
		{
			return ProtoReader.ReadByteOrThrow(source) << 24 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source);
		}

		public static void DirectReadBytes(Stream source, byte[] buffer, int offset, int count)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			while (count > 0)
			{
				int num = source.Read(buffer, offset, count);
				int num1 = num;
				if (num <= 0)
				{
					break;
				}
				count -= num1;
				offset += num1;
			}
			if (count > 0)
			{
				throw ProtoReader.EoF(null);
			}
		}

		public static byte[] DirectReadBytes(Stream source, int count)
		{
			byte[] numArray = new byte[count];
			ProtoReader.DirectReadBytes(source, numArray, 0, count);
			return numArray;
		}

		public static int DirectReadLittleEndianInt32(Stream source)
		{
			return ProtoReader.ReadByteOrThrow(source) | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 24;
		}

		public static string DirectReadString(Stream source, int length)
		{
			byte[] numArray = new byte[length];
			ProtoReader.DirectReadBytes(source, numArray, 0, length);
			return Encoding.UTF8.GetString(numArray, 0, length);
		}

		public static int DirectReadVarintInt32(Stream source)
		{
			uint num;
			if (ProtoReader.TryReadUInt32Variant(source, out num) <= 0)
			{
				throw ProtoReader.EoF(null);
			}
			return (int)num;
		}

		public void Dispose()
		{
			this.source = null;
			this.model = null;
			BufferPool.ReleaseBufferToPool(ref this.ioBuffer);
			if (this.stringInterner != null)
			{
				this.stringInterner.Clear();
			}
			if (this.netCache != null)
			{
				this.netCache.Clear();
			}
		}

		public static void EndSubItem(SubItemToken token, ProtoReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			int num = token.@value;
			if (reader.wireType == ProtoBuf.WireType.EndGroup)
			{
				if (num >= 0)
				{
					throw ProtoReader.AddErrorData(new ArgumentException("token"), reader);
				}
				if (-num != reader.fieldNumber)
				{
					throw reader.CreateException("Wrong group was ended");
				}
				reader.wireType = ProtoBuf.WireType.None;
				reader.depth--;
				return;
			}
			if (num < reader.position)
			{
				throw reader.CreateException("Sub-message not read entirely");
			}
			if (reader.blockEnd != reader.position && reader.blockEnd != 2147483647)
			{
				throw reader.CreateException("Sub-message not read correctly");
			}
			reader.blockEnd = num;
			reader.depth--;
		}

		internal void Ensure(int count, bool strict)
		{
			if (count > (int)this.ioBuffer.Length)
			{
				BufferPool.ResizeAndFlushLeft(ref this.ioBuffer, count, this.ioIndex, this.available);
				this.ioIndex = 0;
			}
			else if (this.ioIndex + count >= (int)this.ioBuffer.Length)
			{
				Helpers.BlockCopy(this.ioBuffer, this.ioIndex, this.ioBuffer, 0, this.available);
				this.ioIndex = 0;
			}
			count -= this.available;
			int num = this.ioIndex + this.available;
			int length = (int)this.ioBuffer.Length - num;
			if (this.isFixedLength && this.dataRemaining < length)
			{
				length = this.dataRemaining;
			}
			while (count > 0 && length > 0)
			{
				int num1 = this.source.Read(this.ioBuffer, num, length);
				int num2 = num1;
				if (num1 <= 0)
				{
					break;
				}
				this.available += num2;
				count -= num2;
				length -= num2;
				num += num2;
				if (!this.isFixedLength)
				{
					continue;
				}
				this.dataRemaining -= num2;
			}
			if (strict && count > 0)
			{
				throw ProtoReader.EoF(this);
			}
		}

		private static Exception EoF(ProtoReader source)
		{
			return ProtoReader.AddErrorData(new EndOfStreamException(), source);
		}

		private static ProtoReader GetRecycled()
		{
			ProtoReader protoReader = ProtoReader.lastReader;
			ProtoReader.lastReader = null;
			return protoReader;
		}

		internal int GetTypeKey(ref Type type)
		{
			return this.model.GetKey(ref type);
		}

		public static bool HasSubValue(ProtoBuf.WireType wireType, ProtoReader source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (source.blockEnd <= source.position || wireType == ProtoBuf.WireType.EndGroup)
			{
				return false;
			}
			source.wireType = wireType;
			return true;
		}

		public void Hint(ProtoBuf.WireType wireType)
		{
			if (this.wireType == wireType)
			{
				return;
			}
			if ((wireType & (ProtoBuf.WireType.Fixed64 | ProtoBuf.WireType.String | ProtoBuf.WireType.StartGroup | ProtoBuf.WireType.EndGroup | ProtoBuf.WireType.Fixed32)) == this.wireType)
			{
				this.wireType = wireType;
			}
		}

		private static void Init(ProtoReader reader, Stream source, TypeModel model, SerializationContext context, int length)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!source.CanRead)
			{
				throw new ArgumentException("Cannot read from stream", "source");
			}
			reader.source = source;
			reader.ioBuffer = BufferPool.GetBuffer();
			reader.model = model;
			bool flag = length >= 0;
			reader.isFixedLength = flag;
			reader.dataRemaining = (flag ? length : 0);
			if (context != null)
			{
				context.Freeze();
			}
			else
			{
				context = SerializationContext.Default;
			}
			reader.context = context;
			int num = 0;
			int num1 = num;
			reader.ioIndex = num;
			int num2 = num1;
			int num3 = num2;
			reader.fieldNumber = num2;
			int num4 = num3;
			int num5 = num4;
			reader.depth = num4;
			int num6 = num5;
			int num7 = num6;
			reader.available = num6;
			reader.position = num7;
			reader.blockEnd = 2147483647;
			reader.internStrings = true;
			reader.wireType = ProtoBuf.WireType.None;
			reader.trapCount = 1;
			if (reader.netCache == null)
			{
				reader.netCache = new NetObjectCache();
			}
		}

		private string Intern(string value)
		{
			string str;
			if (value == null)
			{
				return null;
			}
			if (value.Length == 0)
			{
				return "";
			}
			if (this.stringInterner == null)
			{
				this.stringInterner = new Dictionary<string, string>()
				{
					{ value, value }
				};
			}
			else if (!this.stringInterner.TryGetValue(value, out str))
			{
				this.stringInterner.Add(value, value);
			}
			else
			{
				value = str;
			}
			return value;
		}

		public static object Merge(ProtoReader parent, object from, object to)
		{
			object obj;
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			TypeModel model = parent.Model;
			SerializationContext context = parent.Context;
			if (model == null)
			{
				throw new InvalidOperationException("Types cannot be merged unless a type-model has been specified");
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				model.Serialize(memoryStream, from, context);
				memoryStream.Position = (long)0;
				obj = model.Deserialize(memoryStream, to, null);
			}
			return obj;
		}

		public static void NoteObject(object value, ProtoReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			if (reader.trapCount != 0)
			{
				reader.netCache.RegisterTrappedObject(value);
				reader.trapCount--;
			}
		}

		public bool ReadBoolean()
		{
			switch (this.ReadUInt32())
			{
				case 0:
				{
					return false;
				}
				case 1:
				{
					return true;
				}
			}
			throw this.CreateException("Unexpected boolean value");
		}

		public byte ReadByte()
		{
			return (void*)(checked((byte)this.ReadUInt32()));
		}

		private static int ReadByteOrThrow(Stream source)
		{
			int num = source.ReadByte();
			if (num < 0)
			{
				throw ProtoReader.EoF(null);
			}
			return num;
		}

		public double ReadDouble()
		{
			ProtoBuf.WireType wireType = this.wireType;
			if (wireType == ProtoBuf.WireType.Fixed64)
			{
				long num = this.ReadInt64();
				return BitConverter.ToDouble(BitConverter.GetBytes(num), 0);
			}
			if (wireType != ProtoBuf.WireType.Fixed32)
			{
				throw this.CreateWireTypeException();
			}
			return (double)this.ReadSingle();
		}

		public int ReadFieldHeader()
		{
			uint num;
			if (this.blockEnd <= this.position || this.wireType == ProtoBuf.WireType.EndGroup)
			{
				return 0;
			}
			if (!this.TryReadUInt32Variant(out num))
			{
				this.wireType = ProtoBuf.WireType.None;
				this.fieldNumber = 0;
			}
			else
			{
				this.wireType = (ProtoBuf.WireType)(num & 7);
				this.fieldNumber = (int)(num >> 3);
				if (this.fieldNumber < 1)
				{
					throw new ProtoException(string.Concat("Invalid field in source data: ", this.fieldNumber.ToString()));
				}
			}
			if (this.wireType != ProtoBuf.WireType.EndGroup)
			{
				return this.fieldNumber;
			}
			if (this.depth <= 0)
			{
				throw new ProtoException("Unexpected end-group in source data; this usually means the source data is corrupt");
			}
			return 0;
		}

		public short ReadInt16()
		{
			return checked((short)this.ReadInt32());
		}

		public int ReadInt32()
		{
			ProtoBuf.WireType wireType = this.wireType;
			switch (wireType)
			{
				case ProtoBuf.WireType.Variant:
				{
					return (int)this.ReadUInt32Variant(true);
				}
				case ProtoBuf.WireType.Fixed64:
				{
					return checked((int)this.ReadInt64());
				}
				default:
				{
					if (wireType == ProtoBuf.WireType.Fixed32)
					{
						break;
					}
					else
					{
						if (wireType != ProtoBuf.WireType.SignedVariant)
						{
							throw this.CreateWireTypeException();
						}
						return ProtoReader.Zag(this.ReadUInt32Variant(true));
					}
				}
			}
			if (this.available < 4)
			{
				this.Ensure(4, true);
			}
			this.position += 4;
			this.available -= 4;
			byte[] numArray = this.ioBuffer;
			ProtoReader protoReader = this;
			int num = protoReader.ioIndex;
			int num1 = num;
			protoReader.ioIndex = num + 1;
			byte num2 = numArray[num1];
			byte[] numArray1 = this.ioBuffer;
			ProtoReader protoReader1 = this;
			int num3 = protoReader1.ioIndex;
			int num4 = num3;
			protoReader1.ioIndex = num3 + 1;
			byte[] numArray2 = this.ioBuffer;
			ProtoReader protoReader2 = this;
			int num5 = protoReader2.ioIndex;
			int num6 = num5;
			protoReader2.ioIndex = num5 + 1;
			byte[] numArray3 = this.ioBuffer;
			ProtoReader protoReader3 = this;
			int num7 = protoReader3.ioIndex;
			int num8 = num7;
			protoReader3.ioIndex = num7 + 1;
			return num2 | numArray1[num4] << 8 | numArray2[num6] << 16 | numArray3[num8] << 24;
		}

		public long ReadInt64()
		{
			ProtoBuf.WireType wireType = this.wireType;
			switch (wireType)
			{
				case ProtoBuf.WireType.Variant:
				{
					return (long)this.ReadUInt64Variant();
				}
				case ProtoBuf.WireType.Fixed64:
				{
					if (this.available < 8)
					{
						this.Ensure(8, true);
					}
					this.position += 8;
					this.available -= 8;
					byte[] numArray = this.ioBuffer;
					ProtoReader protoReader = this;
					int num = protoReader.ioIndex;
					int num1 = num;
					protoReader.ioIndex = num + 1;
					ulong num2 = (ulong)numArray[num1];
					byte[] numArray1 = this.ioBuffer;
					ProtoReader protoReader1 = this;
					int num3 = protoReader1.ioIndex;
					int num4 = num3;
					protoReader1.ioIndex = num3 + 1;
					byte[] numArray2 = this.ioBuffer;
					ProtoReader protoReader2 = this;
					int num5 = protoReader2.ioIndex;
					int num6 = num5;
					protoReader2.ioIndex = num5 + 1;
					byte[] numArray3 = this.ioBuffer;
					ProtoReader protoReader3 = this;
					int num7 = protoReader3.ioIndex;
					int num8 = num7;
					protoReader3.ioIndex = num7 + 1;
					byte[] numArray4 = this.ioBuffer;
					ProtoReader protoReader4 = this;
					int num9 = protoReader4.ioIndex;
					int num10 = num9;
					protoReader4.ioIndex = num9 + 1;
					byte[] numArray5 = this.ioBuffer;
					ProtoReader protoReader5 = this;
					int num11 = protoReader5.ioIndex;
					int num12 = num11;
					protoReader5.ioIndex = num11 + 1;
					byte[] numArray6 = this.ioBuffer;
					ProtoReader protoReader6 = this;
					int num13 = protoReader6.ioIndex;
					int num14 = num13;
					protoReader6.ioIndex = num13 + 1;
					byte[] numArray7 = this.ioBuffer;
					ProtoReader protoReader7 = this;
					int num15 = protoReader7.ioIndex;
					int num16 = num15;
					protoReader7.ioIndex = num15 + 1;
					return (long)(num2 | (ulong)numArray1[num4] << 8 | (ulong)numArray2[num6] << 16 | (ulong)numArray3[num8] << 24 | (ulong)numArray4[num10] << 32 | (ulong)numArray5[num12] << 40 | (ulong)numArray6[num14] << 48 | (ulong)numArray7[num16] << 56);
				}
				default:
				{
					if (wireType == ProtoBuf.WireType.Fixed32)
					{
						break;
					}
					else
					{
						if (wireType != ProtoBuf.WireType.SignedVariant)
						{
							throw this.CreateWireTypeException();
						}
						return ProtoReader.Zag(this.ReadUInt64Variant());
					}
				}
			}
			return (long)this.ReadInt32();
		}

		public static int ReadLengthPrefix(Stream source, bool expectHeader, PrefixStyle style, out int fieldNumber)
		{
			int num;
			return ProtoReader.ReadLengthPrefix(source, expectHeader, style, out fieldNumber, out num);
		}

		public static int ReadLengthPrefix(Stream source, bool expectHeader, PrefixStyle style, out int fieldNumber, out int bytesRead)
		{
			uint num;
			int num1;
			fieldNumber = 0;
			switch (style)
			{
				case PrefixStyle.None:
				{
					bytesRead = 0;
					return 2147483647;
				}
				case PrefixStyle.Base128:
				{
					bytesRead = 0;
					if (!expectHeader)
					{
						num1 = ProtoReader.TryReadUInt32Variant(source, out num);
						bytesRead += num1;
						if (bytesRead >= 0)
						{
							return (int)num;
						}
						return -1;
					}
					num1 = ProtoReader.TryReadUInt32Variant(source, out num);
					bytesRead += num1;
					if (num1 <= 0)
					{
						bytesRead = 0;
						return -1;
					}
					if ((num & 7) != 2)
					{
						throw new InvalidOperationException();
					}
					fieldNumber = (int)(num >> 3);
					num1 = ProtoReader.TryReadUInt32Variant(source, out num);
					bytesRead += num1;
					if (bytesRead == 0)
					{
						throw ProtoReader.EoF(null);
					}
					return (int)num;
				}
				case PrefixStyle.Fixed32:
				{
					int num2 = source.ReadByte();
					if (num2 < 0)
					{
						bytesRead = 0;
						return -1;
					}
					bytesRead = 4;
					return num2 | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 24;
				}
				case PrefixStyle.Fixed32BigEndian:
				{
					int num3 = source.ReadByte();
					if (num3 < 0)
					{
						bytesRead = 0;
						return -1;
					}
					bytesRead = 4;
					return num3 << 24 | ProtoReader.ReadByteOrThrow(source) << 16 | ProtoReader.ReadByteOrThrow(source) << 8 | ProtoReader.ReadByteOrThrow(source);
				}
			}
			throw new ArgumentOutOfRangeException("style");
		}

		public static object ReadObject(object value, int key, ProtoReader reader)
		{
			return ProtoReader.ReadTypedObject(value, key, reader, null);
		}

		public sbyte ReadSByte()
		{
			return checked((sbyte)this.ReadInt32());
		}

		public float ReadSingle()
		{
			ProtoBuf.WireType wireType = this.wireType;
			if (wireType != ProtoBuf.WireType.Fixed64)
			{
				if (wireType != ProtoBuf.WireType.Fixed32)
				{
					throw this.CreateWireTypeException();
				}
				int num = this.ReadInt32();
				return BitConverter.ToSingle(BitConverter.GetBytes(num), 0);
			}
			double num1 = this.ReadDouble();
			float single = (float)num1;
			if (Helpers.IsInfinity(single) && !Helpers.IsInfinity(num1))
			{
				throw ProtoReader.AddErrorData(new OverflowException(), this);
			}
			return single;
		}

		public string ReadString()
		{
			if (this.wireType != ProtoBuf.WireType.String)
			{
				throw this.CreateWireTypeException();
			}
			int num = (int)this.ReadUInt32Variant(false);
			if (num == 0)
			{
				return "";
			}
			if (this.available < num)
			{
				this.Ensure(num, true);
			}
			string str = ProtoReader.encoding.GetString(this.ioBuffer, this.ioIndex, num);
			if (this.internStrings)
			{
				str = this.Intern(str);
			}
			this.available -= num;
			this.position += num;
			this.ioIndex += num;
			return str;
		}

		public Type ReadType()
		{
			return TypeModel.DeserializeType(this.model, this.ReadString());
		}

		internal static object ReadTypedObject(object value, int key, ProtoReader reader, Type type)
		{
			if (reader.model == null)
			{
				throw ProtoReader.AddErrorData(new InvalidOperationException("Cannot deserialize sub-objects unless a model is provided"), reader);
			}
			SubItemToken subItemToken = ProtoReader.StartSubItem(reader);
			if (key >= 0)
			{
				value = reader.model.Deserialize(key, value, reader);
			}
			else if (type == null || !reader.model.TryDeserializeAuxiliaryType(reader, DataFormat.Default, 1, type, ref value, true, false, true, false))
			{
				TypeModel.ThrowUnexpectedType(type);
			}
			ProtoReader.EndSubItem(subItemToken, reader);
			return value;
		}

		public ushort ReadUInt16()
		{
			return (void*)(checked((ushort)this.ReadUInt32()));
		}

		public uint ReadUInt32()
		{
			ProtoBuf.WireType wireType = this.wireType;
			switch (wireType)
			{
				case ProtoBuf.WireType.Variant:
				{
					return this.ReadUInt32Variant(false);
				}
				case ProtoBuf.WireType.Fixed64:
				{
					return (void*)(checked((uint)this.ReadUInt64()));
				}
				default:
				{
					if (wireType == ProtoBuf.WireType.Fixed32)
					{
						break;
					}
					else
					{
						throw this.CreateWireTypeException();
					}
				}
			}
			if (this.available < 4)
			{
				this.Ensure(4, true);
			}
			this.position += 4;
			this.available -= 4;
			byte[] numArray = this.ioBuffer;
			ProtoReader protoReader = this;
			int num = protoReader.ioIndex;
			int num1 = num;
			protoReader.ioIndex = num + 1;
			byte num2 = numArray[num1];
			byte[] numArray1 = this.ioBuffer;
			ProtoReader protoReader1 = this;
			int num3 = protoReader1.ioIndex;
			int num4 = num3;
			protoReader1.ioIndex = num3 + 1;
			byte[] numArray2 = this.ioBuffer;
			ProtoReader protoReader2 = this;
			int num5 = protoReader2.ioIndex;
			int num6 = num5;
			protoReader2.ioIndex = num5 + 1;
			byte[] numArray3 = this.ioBuffer;
			ProtoReader protoReader3 = this;
			int num7 = protoReader3.ioIndex;
			int num8 = num7;
			protoReader3.ioIndex = num7 + 1;
			return (uint)(num2 | numArray1[num4] << 8 | numArray2[num6] << 16 | numArray3[num8] << 24);
		}

		private uint ReadUInt32Variant(bool trimNegative)
		{
			uint num;
			int num1 = this.TryReadUInt32VariantWithoutMoving(trimNegative, out num);
			if (num1 <= 0)
			{
				throw ProtoReader.EoF(this);
			}
			this.ioIndex += num1;
			this.available -= num1;
			this.position += num1;
			return num;
		}

		public ulong ReadUInt64()
		{
			ProtoBuf.WireType wireType = this.wireType;
			switch (wireType)
			{
				case ProtoBuf.WireType.Variant:
				{
					return this.ReadUInt64Variant();
				}
				case ProtoBuf.WireType.Fixed64:
				{
					if (this.available < 8)
					{
						this.Ensure(8, true);
					}
					this.position += 8;
					this.available -= 8;
					byte[] numArray = this.ioBuffer;
					ProtoReader protoReader = this;
					int num = protoReader.ioIndex;
					int num1 = num;
					protoReader.ioIndex = num + 1;
					ulong num2 = (ulong)numArray[num1];
					byte[] numArray1 = this.ioBuffer;
					ProtoReader protoReader1 = this;
					int num3 = protoReader1.ioIndex;
					int num4 = num3;
					protoReader1.ioIndex = num3 + 1;
					byte[] numArray2 = this.ioBuffer;
					ProtoReader protoReader2 = this;
					int num5 = protoReader2.ioIndex;
					int num6 = num5;
					protoReader2.ioIndex = num5 + 1;
					byte[] numArray3 = this.ioBuffer;
					ProtoReader protoReader3 = this;
					int num7 = protoReader3.ioIndex;
					int num8 = num7;
					protoReader3.ioIndex = num7 + 1;
					byte[] numArray4 = this.ioBuffer;
					ProtoReader protoReader4 = this;
					int num9 = protoReader4.ioIndex;
					int num10 = num9;
					protoReader4.ioIndex = num9 + 1;
					byte[] numArray5 = this.ioBuffer;
					ProtoReader protoReader5 = this;
					int num11 = protoReader5.ioIndex;
					int num12 = num11;
					protoReader5.ioIndex = num11 + 1;
					byte[] numArray6 = this.ioBuffer;
					ProtoReader protoReader6 = this;
					int num13 = protoReader6.ioIndex;
					int num14 = num13;
					protoReader6.ioIndex = num13 + 1;
					byte[] numArray7 = this.ioBuffer;
					ProtoReader protoReader7 = this;
					int num15 = protoReader7.ioIndex;
					int num16 = num15;
					protoReader7.ioIndex = num15 + 1;
					return num2 | (ulong)numArray1[num4] << 8 | (ulong)numArray2[num6] << 16 | (ulong)numArray3[num8] << 24 | (ulong)numArray4[num10] << 32 | (ulong)numArray5[num12] << 40 | (ulong)numArray6[num14] << 48 | (ulong)numArray7[num16] << 56;
				}
				default:
				{
					if (wireType == ProtoBuf.WireType.Fixed32)
					{
						break;
					}
					else
					{
						throw this.CreateWireTypeException();
					}
				}
			}
			return (ulong)this.ReadUInt32();
		}

		private ulong ReadUInt64Variant()
		{
			ulong num;
			int num1 = this.TryReadUInt64VariantWithoutMoving(out num);
			if (num1 <= 0)
			{
				throw ProtoReader.EoF(this);
			}
			this.ioIndex += num1;
			this.available -= num1;
			this.position += num1;
			return num;
		}

		internal static void Recycle(ProtoReader reader)
		{
			if (reader != null)
			{
				reader.Dispose();
				ProtoReader.lastReader = reader;
			}
		}

		internal static void Seek(Stream source, int count, byte[] buffer)
		{
			int num;
			int num1;
			if (source.CanSeek)
			{
				source.Seek((long)count, SeekOrigin.Current);
				count = 0;
			}
			else if (buffer == null)
			{
				buffer = BufferPool.GetBuffer();
				try
				{
					while (count > (int)buffer.Length)
					{
						int num2 = source.Read(buffer, 0, (int)buffer.Length);
						num1 = num2;
						if (num2 > 0)
						{
							count -= num1;
						}
						else
						{
							break;
						}
					}
					while (count > 0)
					{
						int num3 = source.Read(buffer, 0, count);
						num1 = num3;
						if (num3 <= 0)
						{
							break;
						}
						count -= num1;
					}
				}
				finally
				{
					BufferPool.ReleaseBufferToPool(ref buffer);
				}
			}
			else
			{
				while (count > (int)buffer.Length)
				{
					int num4 = source.Read(buffer, 0, (int)buffer.Length);
					num = num4;
					if (num4 > 0)
					{
						count -= num;
					}
					else
					{
						break;
					}
				}
				while (count > 0)
				{
					int num5 = source.Read(buffer, 0, count);
					num = num5;
					if (num5 > 0)
					{
						count -= num;
					}
					else
					{
						break;
					}
				}
			}
			if (count > 0)
			{
				throw ProtoReader.EoF(null);
			}
		}

		internal void SetRootObject(object value)
		{
			this.netCache.SetKeyedObject(0, value);
			this.trapCount--;
		}

		public void SkipField()
		{
			switch (this.wireType)
			{
				case ProtoBuf.WireType.None:
				case ProtoBuf.WireType.EndGroup:
				case ProtoBuf.WireType.String | ProtoBuf.WireType.EndGroup:
				case ProtoBuf.WireType.Fixed64 | ProtoBuf.WireType.String | ProtoBuf.WireType.StartGroup | ProtoBuf.WireType.EndGroup | ProtoBuf.WireType.Fixed32:
				{
					throw this.CreateWireTypeException();
				}
				case ProtoBuf.WireType.Variant:
				case ProtoBuf.WireType.SignedVariant:
				{
					this.ReadUInt64Variant();
					return;
				}
				case ProtoBuf.WireType.Fixed64:
				{
					if (this.available < 8)
					{
						this.Ensure(8, true);
					}
					this.available -= 8;
					this.ioIndex += 8;
					this.position += 8;
					return;
				}
				case ProtoBuf.WireType.String:
				{
					int num = (int)this.ReadUInt32Variant(false);
					if (num <= this.available)
					{
						this.available -= num;
						this.ioIndex += num;
						this.position += num;
						return;
					}
					this.position += num;
					num -= this.available;
					int num1 = 0;
					int num2 = num1;
					this.available = num1;
					this.ioIndex = num2;
					if (this.isFixedLength)
					{
						if (num > this.dataRemaining)
						{
							throw ProtoReader.EoF(this);
						}
						this.dataRemaining -= num;
					}
					ProtoReader.Seek(this.source, num, this.ioBuffer);
					return;
				}
				case ProtoBuf.WireType.StartGroup:
				{
					int num3 = this.fieldNumber;
					this.depth++;
					while (this.ReadFieldHeader() > 0)
					{
						this.SkipField();
					}
					this.depth--;
					if (this.wireType != ProtoBuf.WireType.EndGroup || this.fieldNumber != num3)
					{
						throw this.CreateWireTypeException();
					}
					this.wireType = ProtoBuf.WireType.None;
					return;
				}
				case ProtoBuf.WireType.Fixed32:
				{
					if (this.available < 4)
					{
						this.Ensure(4, true);
					}
					this.available -= 4;
					this.ioIndex += 4;
					this.position += 4;
					return;
				}
				default:
				{
					throw this.CreateWireTypeException();
				}
			}
		}

		public static SubItemToken StartSubItem(ProtoReader reader)
		{
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			switch (reader.wireType)
			{
				case ProtoBuf.WireType.String:
				{
					int num = (int)reader.ReadUInt32Variant(false);
					if (num < 0)
					{
						throw ProtoReader.AddErrorData(new InvalidOperationException(), reader);
					}
					int num1 = reader.blockEnd;
					reader.blockEnd = reader.position + num;
					reader.depth++;
					return new SubItemToken(num1);
				}
				case ProtoBuf.WireType.StartGroup:
				{
					reader.wireType = ProtoBuf.WireType.None;
					reader.depth++;
					return new SubItemToken(-reader.fieldNumber);
				}
			}
			throw reader.CreateWireTypeException();
		}

		public void ThrowEnumException(Type type, int value)
		{
			string str = (type == null ? "<null>" : type.FullName);
			throw ProtoReader.AddErrorData(new ProtoException(string.Concat("No ", str, " enum is mapped to the wire-value ", value.ToString())), this);
		}

		internal void TrapNextObject(int newObjectKey)
		{
			this.trapCount++;
			this.netCache.SetKeyedObject(newObjectKey, null);
		}

		public bool TryReadFieldHeader(int field)
		{
			uint num;
			if (this.blockEnd <= this.position || this.wireType == ProtoBuf.WireType.EndGroup)
			{
				return false;
			}
			int num1 = this.TryReadUInt32VariantWithoutMoving(false, out num);
			if (num1 > 0 && num >> 3 == field)
			{
				UInt32 num2 = num & 7;
				ProtoBuf.WireType wireType = (ProtoBuf.WireType)num2;
				if (num2 != 4)
				{
					this.wireType = wireType;
					this.fieldNumber = field;
					this.position += num1;
					this.ioIndex += num1;
					this.available -= num1;
					return true;
				}
			}
			return false;
		}

		private bool TryReadUInt32Variant(out uint value)
		{
			int num = this.TryReadUInt32VariantWithoutMoving(false, out value);
			if (num <= 0)
			{
				return false;
			}
			this.ioIndex += num;
			this.available -= num;
			this.position += num;
			return true;
		}

		private static int TryReadUInt32Variant(Stream source, out uint value)
		{
			value = 0;
			int num = source.ReadByte();
			if (num < 0)
			{
				return 0;
			}
			value = (uint)num;
			if ((value & 128) == 0)
			{
				return 1;
			}
			value &= 127;
			num = source.ReadByte();
			if (num < 0)
			{
				throw ProtoReader.EoF(null);
			}
			value = value | (num & 127) << 7;
			if ((num & 128) == 0)
			{
				return 2;
			}
			num = source.ReadByte();
			if (num < 0)
			{
				throw ProtoReader.EoF(null);
			}
			value = value | (num & 127) << 14;
			if ((num & 128) == 0)
			{
				return 3;
			}
			num = source.ReadByte();
			if (num < 0)
			{
				throw ProtoReader.EoF(null);
			}
			value = value | (num & 127) << 21;
			if ((num & 128) == 0)
			{
				return 4;
			}
			num = source.ReadByte();
			if (num < 0)
			{
				throw ProtoReader.EoF(null);
			}
			value = value | num << 28;
			if ((num & 240) != 0)
			{
				throw new OverflowException();
			}
			return 5;
		}

		internal int TryReadUInt32VariantWithoutMoving(bool trimNegative, out uint value)
		{
			if (this.available < 10)
			{
				this.Ensure(10, false);
			}
			if (this.available == 0)
			{
				value = 0;
				return 0;
			}
			int num = this.ioIndex;
			int num1 = num;
			num = num1 + 1;
			value = this.ioBuffer[num1];
			if ((value & 128) == 0)
			{
				return 1;
			}
			value &= 127;
			if (this.available == 1)
			{
				throw ProtoReader.EoF(this);
			}
			int num2 = num;
			num = num2 + 1;
			uint num3 = this.ioBuffer[num2];
			value = value | (num3 & 127) << 7;
			if ((num3 & 128) == 0)
			{
				return 2;
			}
			if (this.available == 2)
			{
				throw ProtoReader.EoF(this);
			}
			int num4 = num;
			num = num4 + 1;
			num3 = this.ioBuffer[num4];
			value = value | (num3 & 127) << 14;
			if ((num3 & 128) == 0)
			{
				return 3;
			}
			if (this.available == 3)
			{
				throw ProtoReader.EoF(this);
			}
			int num5 = num;
			num = num5 + 1;
			num3 = this.ioBuffer[num5];
			value = value | (num3 & 127) << 21;
			if ((num3 & 128) == 0)
			{
				return 4;
			}
			if (this.available == 4)
			{
				throw ProtoReader.EoF(this);
			}
			num3 = this.ioBuffer[num];
			value = value | num3 << 28;
			if ((num3 & 240) == 0)
			{
				return 5;
			}
			if (trimNegative && (num3 & 240) == 240 && this.available >= 10)
			{
				int num6 = num + 1;
				num = num6;
				if (this.ioBuffer[num6] == 255)
				{
					int num7 = num + 1;
					num = num7;
					if (this.ioBuffer[num7] == 255)
					{
						int num8 = num + 1;
						num = num8;
						if (this.ioBuffer[num8] == 255)
						{
							int num9 = num + 1;
							num = num9;
							if (this.ioBuffer[num9] == 255)
							{
								int num10 = num + 1;
								num = num10;
								if (this.ioBuffer[num10] == 1)
								{
									return 10;
								}
							}
						}
					}
				}
			}
			throw ProtoReader.AddErrorData(new OverflowException(), this);
		}

		private int TryReadUInt64VariantWithoutMoving(out ulong value)
		{
			if (this.available < 10)
			{
				this.Ensure(10, false);
			}
			if (this.available == 0)
			{
				value = (ulong)0;
				return 0;
			}
			int num = this.ioIndex;
			int num1 = num;
			num = num1 + 1;
			value = (ulong)this.ioBuffer[num1];
			if (((long)value & (long)128) == (long)0)
			{
				return 1;
			}
			value = (ulong)((long)value & (long)127);
			if (this.available == 1)
			{
				throw ProtoReader.EoF(this);
			}
			int num2 = num;
			num = num2 + 1;
			ulong num3 = (ulong)this.ioBuffer[num2];
			value = (long)value | (num3 & (long)127) << 7;
			if ((num3 & (long)128) == (long)0)
			{
				return 2;
			}
			if (this.available == 2)
			{
				throw ProtoReader.EoF(this);
			}
			int num4 = num;
			num = num4 + 1;
			num3 = (ulong)this.ioBuffer[num4];
			value = (long)value | (num3 & (long)127) << 14;
			if ((num3 & (long)128) == (long)0)
			{
				return 3;
			}
			if (this.available == 3)
			{
				throw ProtoReader.EoF(this);
			}
			int num5 = num;
			num = num5 + 1;
			num3 = (ulong)this.ioBuffer[num5];
			value = (long)value | (num3 & (long)127) << 21;
			if ((num3 & (long)128) == (long)0)
			{
				return 4;
			}
			if (this.available == 4)
			{
				throw ProtoReader.EoF(this);
			}
			int num6 = num;
			num = num6 + 1;
			num3 = (ulong)this.ioBuffer[num6];
			value = (long)value | (num3 & (long)127) << 28;
			if ((num3 & (long)128) == (long)0)
			{
				return 5;
			}
			if (this.available == 5)
			{
				throw ProtoReader.EoF(this);
			}
			int num7 = num;
			num = num7 + 1;
			num3 = (ulong)this.ioBuffer[num7];
			value = (long)value | (num3 & (long)127) << 35;
			if ((num3 & (long)128) == (long)0)
			{
				return 6;
			}
			if (this.available == 6)
			{
				throw ProtoReader.EoF(this);
			}
			int num8 = num;
			num = num8 + 1;
			num3 = (ulong)this.ioBuffer[num8];
			value = (long)value | (num3 & (long)127) << 42;
			if ((num3 & (long)128) == (long)0)
			{
				return 7;
			}
			if (this.available == 7)
			{
				throw ProtoReader.EoF(this);
			}
			int num9 = num;
			num = num9 + 1;
			num3 = (ulong)this.ioBuffer[num9];
			value = (long)value | (num3 & (long)127) << 49;
			if ((num3 & (long)128) == (long)0)
			{
				return 8;
			}
			if (this.available == 8)
			{
				throw ProtoReader.EoF(this);
			}
			int num10 = num;
			num = num10 + 1;
			num3 = (ulong)this.ioBuffer[num10];
			value = (long)value | (num3 & (long)127) << 56;
			if ((num3 & (long)128) == (long)0)
			{
				return 9;
			}
			if (this.available == 9)
			{
				throw ProtoReader.EoF(this);
			}
			num3 = (ulong)this.ioBuffer[num];
			value = (long)value | num3 << 63;
			if ((num3 & (long)-2) != (long)0)
			{
				throw ProtoReader.AddErrorData(new OverflowException(), this);
			}
			return 10;
		}

		private static int Zag(uint ziggedValue)
		{
			int num = (int)ziggedValue;
			return -(num & 1) ^ num >> 1 & 2147483647;
		}

		private static long Zag(ulong ziggedValue)
		{
			long num = (long)ziggedValue;
			return -(num & (long)1) ^ num >> 1 & 9223372036854775807L;
		}
	}
}