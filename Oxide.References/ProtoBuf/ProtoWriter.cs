using ProtoBuf.Meta;
using System;
using System.IO;
using System.Text;

namespace ProtoBuf
{
	public sealed class ProtoWriter : IDisposable
	{
		private const int RecursionCheckDepth = 25;

		private Stream dest;

		private TypeModel model;

		private readonly NetObjectCache netCache = new NetObjectCache();

		private int fieldNumber;

		private int flushLock;

		private ProtoBuf.WireType wireType;

		private int depth;

		private MutableList recursionStack;

		private readonly SerializationContext context;

		private byte[] ioBuffer;

		private int ioIndex;

		private int position;

		private readonly static UTF8Encoding encoding;

		private int packedFieldNumber;

		public SerializationContext Context
		{
			get
			{
				return this.context;
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

		internal ProtoBuf.WireType WireType
		{
			get
			{
				return this.wireType;
			}
		}

		static ProtoWriter()
		{
			ProtoWriter.encoding = new UTF8Encoding();
		}

		public ProtoWriter(Stream dest, TypeModel model, SerializationContext context)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			if (!dest.CanWrite)
			{
				throw new ArgumentException("Cannot write to stream", "dest");
			}
			this.dest = dest;
			this.ioBuffer = BufferPool.GetBuffer();
			this.model = model;
			this.wireType = ProtoBuf.WireType.None;
			if (context != null)
			{
				context.Freeze();
			}
			else
			{
				context = SerializationContext.Default;
			}
			this.context = context;
		}

		public static void AppendExtensionData(IExtensible instance, ProtoWriter writer)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.wireType != ProtoBuf.WireType.None)
			{
				throw ProtoWriter.CreateException(writer);
			}
			IExtension extensionObject = instance.GetExtensionObject(false);
			if (extensionObject != null)
			{
				Stream stream = extensionObject.BeginQuery();
				try
				{
					ProtoWriter.CopyRawFromStream(stream, writer);
				}
				finally
				{
					extensionObject.EndQuery(stream);
				}
			}
		}

		internal void CheckDepthFlushlock()
		{
			if (this.depth != 0 || this.flushLock != 0)
			{
				throw new InvalidOperationException("The writer is in an incomplete state");
			}
		}

		private void CheckRecursionStackAndPush(object instance)
		{
			if (this.recursionStack == null)
			{
				this.recursionStack = new MutableList();
			}
			else if (instance != null)
			{
				int num = this.recursionStack.IndexOfReference(instance);
				int num1 = num;
				if (num >= 0)
				{
					int count = this.recursionStack.Count - num1;
					throw new ProtoException(string.Concat("Possible recursion detected (offset: ", count.ToString(), " level(s)): ", instance.ToString()));
				}
			}
			this.recursionStack.Add(instance);
		}

		public void Close()
		{
			if (this.depth != 0 || this.flushLock != 0)
			{
				throw new InvalidOperationException("Unable to close stream in an incomplete state");
			}
			this.Dispose();
		}

		private static void CopyRawFromStream(Stream source, ProtoWriter writer)
		{
			byte[] numArray = writer.ioBuffer;
			int length = (int)numArray.Length - writer.ioIndex;
			int num = 1;
			while (length > 0)
			{
				int num1 = source.Read(numArray, writer.ioIndex, length);
				num = num1;
				if (num1 <= 0)
				{
					break;
				}
				writer.ioIndex += num;
				writer.position += num;
				length -= num;
			}
			if (num <= 0)
			{
				return;
			}
			if (writer.flushLock == 0)
			{
				ProtoWriter.Flush(writer);
				while (true)
				{
					int num2 = source.Read(numArray, 0, (int)numArray.Length);
					num = num2;
					if (num2 <= 0)
					{
						break;
					}
					writer.dest.Write(numArray, 0, num);
					writer.position += num;
				}
				return;
			}
			while (true)
			{
				ProtoWriter.DemandSpace(128, writer);
				int num3 = source.Read(writer.ioBuffer, writer.ioIndex, (int)writer.ioBuffer.Length - writer.ioIndex);
				num = num3;
				if (num3 <= 0)
				{
					break;
				}
				writer.position += num;
				writer.ioIndex += num;
			}
		}

		internal static Exception CreateException(ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			return new ProtoException(string.Concat("Invalid serialization operation with wire-type ", writer.wireType.ToString(), " at position ", writer.position.ToString()));
		}

		private static void DemandSpace(int required, ProtoWriter writer)
		{
			if ((int)writer.ioBuffer.Length - writer.ioIndex < required)
			{
				if (writer.flushLock == 0)
				{
					ProtoWriter.Flush(writer);
					if ((int)writer.ioBuffer.Length - writer.ioIndex >= required)
					{
						return;
					}
				}
				BufferPool.ResizeAndFlushLeft(ref writer.ioBuffer, required + writer.ioIndex, 0, writer.ioIndex);
			}
		}

		private void Dispose()
		{
			if (this.dest != null)
			{
				ProtoWriter.Flush(this);
				this.dest = null;
			}
			this.model = null;
			BufferPool.ReleaseBufferToPool(ref this.ioBuffer);
		}

		public static void EndSubItem(SubItemToken token, ProtoWriter writer)
		{
			ProtoWriter.EndSubItem(token, writer, PrefixStyle.Base128);
		}

		private static void EndSubItem(SubItemToken token, ProtoWriter writer, PrefixStyle style)
		{
			int num;
			UInt32 num1;
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.wireType != ProtoBuf.WireType.None)
			{
				throw ProtoWriter.CreateException(writer);
			}
			int num2 = token.@value;
			if (writer.depth <= 0)
			{
				throw ProtoWriter.CreateException(writer);
			}
			ProtoWriter protoWriter = writer;
			int num3 = protoWriter.depth;
			int num4 = num3;
			protoWriter.depth = num3 - 1;
			if (num4 > 25)
			{
				writer.PopRecursionStack();
			}
			writer.packedFieldNumber = 0;
			if (num2 < 0)
			{
				ProtoWriter.WriteHeaderCore(-num2, ProtoBuf.WireType.EndGroup, writer);
				writer.wireType = ProtoBuf.WireType.None;
				return;
			}
			switch (style)
			{
				case PrefixStyle.Base128:
				{
					num = writer.ioIndex - num2 - 1;
					int num5 = 0;
					uint num6 = (uint)num;
					while (true)
					{
						UInt32 num7 = num6 >> 7;
						num6 = num7;
						if (num7 == 0)
						{
							break;
						}
						num5++;
					}
					if (num5 != 0)
					{
						ProtoWriter.DemandSpace(num5, writer);
						byte[] numArray = writer.ioBuffer;
						Helpers.BlockCopy(numArray, num2 + 1, numArray, num2 + 1 + num5, num);
						num6 = (uint)num;
						do
						{
							int num8 = num2;
							num2 = num8 + 1;
							numArray[num8] = (byte)(num6 & 127 | 128);
							num1 = num6 >> 7;
							num6 = num1;
						}
						while (num1 != 0);
						numArray[num2 - 1] = (byte)(numArray[num2 - 1] & -129);
						writer.position += num5;
						writer.ioIndex += num5;
						break;
					}
					else
					{
						writer.ioBuffer[num2] = (byte)(num & 127);
						break;
					}
				}
				case PrefixStyle.Fixed32:
				{
					num = writer.ioIndex - num2 - 4;
					ProtoWriter.WriteInt32ToBuffer(num, writer.ioBuffer, num2);
					break;
				}
				case PrefixStyle.Fixed32BigEndian:
				{
					num = writer.ioIndex - num2 - 4;
					byte[] numArray1 = writer.ioBuffer;
					ProtoWriter.WriteInt32ToBuffer(num, numArray1, num2);
					byte num9 = numArray1[num2];
					numArray1[num2] = numArray1[num2 + 3];
					numArray1[num2 + 3] = num9;
					num9 = numArray1[num2 + 1];
					numArray1[num2 + 1] = numArray1[num2 + 2];
					numArray1[num2 + 2] = num9;
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException("style");
				}
			}
			ProtoWriter protoWriter1 = writer;
			int num10 = protoWriter1.flushLock - 1;
			int num11 = num10;
			protoWriter1.flushLock = num10;
			if (num11 == 0 && writer.ioIndex >= 1024)
			{
				ProtoWriter.Flush(writer);
			}
		}

		internal static void Flush(ProtoWriter writer)
		{
			if (writer.flushLock == 0 && writer.ioIndex != 0)
			{
				writer.dest.Write(writer.ioBuffer, 0, writer.ioIndex);
				writer.ioIndex = 0;
			}
		}

		internal static int GetPosition(ProtoWriter writer)
		{
			return writer.position;
		}

		internal int GetTypeKey(ref Type type)
		{
			return this.model.GetKey(ref type);
		}

		private static void IncrementedAndReset(int length, ProtoWriter writer)
		{
			writer.ioIndex += length;
			writer.position += length;
			writer.wireType = ProtoBuf.WireType.None;
		}

		private void PopRecursionStack()
		{
			this.recursionStack.RemoveLast();
		}

		internal string SerializeType(Type type)
		{
			return TypeModel.SerializeType(this.model, type);
		}

		public static void SetPackedField(int fieldNumber, ProtoWriter writer)
		{
			if (fieldNumber <= 0)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.packedFieldNumber = fieldNumber;
		}

		public void SetRootObject(object value)
		{
			this.NetCache.SetKeyedObject(0, value);
		}

		public static SubItemToken StartSubItem(object instance, ProtoWriter writer)
		{
			return ProtoWriter.StartSubItem(instance, writer, false);
		}

		private static SubItemToken StartSubItem(object instance, ProtoWriter writer, bool allowFixed)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ProtoWriter protoWriter = writer;
			int num = protoWriter.depth + 1;
			int num1 = num;
			protoWriter.depth = num;
			if (num1 > 25)
			{
				writer.CheckRecursionStackAndPush(instance);
			}
			if (writer.packedFieldNumber != 0)
			{
				throw new InvalidOperationException("Cannot begin a sub-item while performing packed encoding");
			}
			switch (writer.wireType)
			{
				case ProtoBuf.WireType.String:
				{
					writer.wireType = ProtoBuf.WireType.None;
					ProtoWriter.DemandSpace(32, writer);
					writer.flushLock++;
					writer.position++;
					ProtoWriter protoWriter1 = writer;
					int num2 = protoWriter1.ioIndex;
					int num3 = num2;
					protoWriter1.ioIndex = num2 + 1;
					return new SubItemToken(num3);
				}
				case ProtoBuf.WireType.StartGroup:
				{
					writer.wireType = ProtoBuf.WireType.None;
					return new SubItemToken(-writer.fieldNumber);
				}
				case ProtoBuf.WireType.EndGroup:
				{
					throw ProtoWriter.CreateException(writer);
				}
				case ProtoBuf.WireType.Fixed32:
				{
					if (!allowFixed)
					{
						throw ProtoWriter.CreateException(writer);
					}
					ProtoWriter.DemandSpace(32, writer);
					writer.flushLock++;
					SubItemToken subItemToken = new SubItemToken(writer.ioIndex);
					ProtoWriter.IncrementedAndReset(4, writer);
					return subItemToken;
				}
				default:
				{
					throw ProtoWriter.CreateException(writer);
				}
			}
		}

		void System.IDisposable.Dispose()
		{
			this.Dispose();
		}

		public static void ThrowEnumException(ProtoWriter writer, object enumValue)
		{
			if (writer != null)
			{
				string str = (enumValue == null ? "<null>" : string.Concat(enumValue.GetType().FullName, ".", enumValue.ToString()));
				throw new ProtoException(string.Concat("No wire-value is mapped to the enum ", str, " at position ", writer.position.ToString()));
			}
			throw new ArgumentNullException("writer");
		}

		public static void WriteBoolean(bool value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32((uint)((value ? 1 : 0)), writer);
		}

		public static void WriteByte(byte value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32(value, writer);
		}

		public static void WriteBytes(byte[] data, ProtoWriter writer)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			ProtoWriter.WriteBytes(data, 0, (int)data.Length, writer);
		}

		public static void WriteBytes(byte[] data, int offset, int length, ProtoWriter writer)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			switch (writer.wireType)
			{
				case ProtoBuf.WireType.Fixed64:
				{
					if (length == 8)
					{
						break;
					}
					throw new ArgumentException("length");
				}
				case ProtoBuf.WireType.String:
				{
					ProtoWriter.WriteUInt32Variant((uint)length, writer);
					writer.wireType = ProtoBuf.WireType.None;
					if (length == 0)
					{
						return;
					}
					if (writer.flushLock != 0 || length <= (int)writer.ioBuffer.Length)
					{
						break;
					}
					ProtoWriter.Flush(writer);
					writer.dest.Write(data, offset, length);
					writer.position += length;
					return;
				}
				case ProtoBuf.WireType.StartGroup:
				case ProtoBuf.WireType.EndGroup:
				{
					throw ProtoWriter.CreateException(writer);
				}
				case ProtoBuf.WireType.Fixed32:
				{
					if (length == 4)
					{
						break;
					}
					throw new ArgumentException("length");
				}
				default:
				{
					throw ProtoWriter.CreateException(writer);
				}
			}
			ProtoWriter.DemandSpace(length, writer);
			Helpers.BlockCopy(data, offset, writer.ioBuffer, writer.ioIndex, length);
			ProtoWriter.IncrementedAndReset(length, writer);
		}

		public static void WriteDouble(double value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ProtoBuf.WireType wireType = writer.wireType;
			if (wireType == ProtoBuf.WireType.Fixed64)
			{
				ProtoWriter.WriteInt64(BitConverter.ToInt64(BitConverter.GetBytes(value), 0), writer);
				return;
			}
			if (wireType != ProtoBuf.WireType.Fixed32)
			{
				throw ProtoWriter.CreateException(writer);
			}
			float single = (float)value;
			if (Helpers.IsInfinity(single) && !Helpers.IsInfinity(value))
			{
				throw new OverflowException();
			}
			ProtoWriter.WriteSingle(single, writer);
		}

		public static void WriteFieldHeader(int fieldNumber, ProtoBuf.WireType wireType, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.wireType != ProtoBuf.WireType.None)
			{
				string[] str = new string[] { "Cannot write a ", wireType.ToString(), " header until the ", writer.wireType.ToString(), " data has been written" };
				throw new InvalidOperationException(string.Concat(str));
			}
			if (fieldNumber < 0)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if (writer.packedFieldNumber == 0)
			{
				writer.fieldNumber = fieldNumber;
				writer.wireType = wireType;
				ProtoWriter.WriteHeaderCore(fieldNumber, wireType, writer);
				return;
			}
			if (writer.packedFieldNumber != fieldNumber)
			{
				throw new InvalidOperationException(string.Concat("Field mismatch during packed encoding; expected ", writer.packedFieldNumber.ToString(), " but received ", fieldNumber.ToString()));
			}
			ProtoBuf.WireType wireType1 = wireType;
			switch (wireType1)
			{
				case ProtoBuf.WireType.Variant:
				case ProtoBuf.WireType.Fixed64:
				{
					writer.fieldNumber = fieldNumber;
					writer.wireType = wireType;
					return;
				}
				default:
				{
					if (wireType1 == ProtoBuf.WireType.Fixed32 || wireType1 == ProtoBuf.WireType.SignedVariant)
					{
						writer.fieldNumber = fieldNumber;
						writer.wireType = wireType;
						return;
					}
					throw new InvalidOperationException(string.Concat("Wire-type cannot be encoded as packed: ", wireType.ToString()));
				}
			}
		}

		internal static void WriteHeaderCore(int fieldNumber, ProtoBuf.WireType wireType, ProtoWriter writer)
		{
			uint num = (uint)(fieldNumber << 3 | (int)(wireType & (ProtoBuf.WireType.Fixed64 | ProtoBuf.WireType.String | ProtoBuf.WireType.StartGroup | ProtoBuf.WireType.EndGroup | ProtoBuf.WireType.Fixed32)));
			ProtoWriter.WriteUInt32Variant(num, writer);
		}

		public static void WriteInt16(short value, ProtoWriter writer)
		{
			ProtoWriter.WriteInt32(value, writer);
		}

		public static void WriteInt32(int value, ProtoWriter writer)
		{
			byte[] numArray;
			int num;
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ProtoBuf.WireType wireType = writer.wireType;
			switch (wireType)
			{
				case ProtoBuf.WireType.Variant:
				{
					if (value >= 0)
					{
						ProtoWriter.WriteUInt32Variant((uint)value, writer);
						writer.wireType = ProtoBuf.WireType.None;
						return;
					}
					ProtoWriter.DemandSpace(10, writer);
					numArray = writer.ioBuffer;
					num = writer.ioIndex;
					numArray[num] = (byte)(value | 128);
					numArray[num + 1] = (byte)(value >> 7 | 128);
					numArray[num + 2] = (byte)(value >> 14 | 128);
					numArray[num + 3] = (byte)(value >> 21 | 128);
					numArray[num + 4] = (byte)(value >> 28 | 128);
					int num1 = 255;
					byte num2 = (byte)num1;
					numArray[num + 8] = (byte)num1;
					byte num3 = num2;
					byte num4 = num3;
					numArray[num + 7] = num3;
					byte num5 = num4;
					byte num6 = num5;
					numArray[num + 6] = num5;
					numArray[num + 5] = num6;
					numArray[num + 9] = 1;
					ProtoWriter.IncrementedAndReset(10, writer);
					return;
				}
				case ProtoBuf.WireType.Fixed64:
				{
					ProtoWriter.DemandSpace(8, writer);
					numArray = writer.ioBuffer;
					num = writer.ioIndex;
					numArray[num] = (byte)value;
					numArray[num + 1] = (byte)(value >> 8);
					numArray[num + 2] = (byte)(value >> 16);
					numArray[num + 3] = (byte)(value >> 24);
					int num7 = 0;
					byte num8 = (byte)num7;
					numArray[num + 7] = (byte)num7;
					byte num9 = num8;
					byte num10 = num9;
					numArray[num + 6] = num9;
					byte num11 = num10;
					byte num12 = num11;
					numArray[num + 5] = num11;
					numArray[num + 4] = num12;
					ProtoWriter.IncrementedAndReset(8, writer);
					return;
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
							throw ProtoWriter.CreateException(writer);
						}
						ProtoWriter.WriteUInt32Variant(ProtoWriter.Zig(value), writer);
						writer.wireType = ProtoBuf.WireType.None;
						return;
					}
				}
			}
			ProtoWriter.DemandSpace(4, writer);
			ProtoWriter.WriteInt32ToBuffer(value, writer.ioBuffer, writer.ioIndex);
			ProtoWriter.IncrementedAndReset(4, writer);
		}

		private static void WriteInt32ToBuffer(int value, byte[] buffer, int index)
		{
			buffer[index] = (byte)value;
			buffer[index + 1] = (byte)(value >> 8);
			buffer[index + 2] = (byte)(value >> 16);
			buffer[index + 3] = (byte)(value >> 24);
		}

		public static void WriteInt64(long value, ProtoWriter writer)
		{
			byte[] numArray;
			int num;
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ProtoBuf.WireType wireType = writer.wireType;
			switch (wireType)
			{
				case ProtoBuf.WireType.Variant:
				{
					if (value >= (long)0)
					{
						ProtoWriter.WriteUInt64Variant((ulong)value, writer);
						writer.wireType = ProtoBuf.WireType.None;
						return;
					}
					ProtoWriter.DemandSpace(10, writer);
					numArray = writer.ioBuffer;
					num = writer.ioIndex;
					numArray[num] = (byte)(value | (long)128);
					numArray[num + 1] = (byte)((int)(value >> 7) | 128);
					numArray[num + 2] = (byte)((int)(value >> 14) | 128);
					numArray[num + 3] = (byte)((int)(value >> 21) | 128);
					numArray[num + 4] = (byte)((int)(value >> 28) | 128);
					numArray[num + 5] = (byte)((int)(value >> 35) | 128);
					numArray[num + 6] = (byte)((int)(value >> 42) | 128);
					numArray[num + 7] = (byte)((int)(value >> 49) | 128);
					numArray[num + 8] = (byte)((int)(value >> 56) | 128);
					numArray[num + 9] = 1;
					ProtoWriter.IncrementedAndReset(10, writer);
					return;
				}
				case ProtoBuf.WireType.Fixed64:
				{
					ProtoWriter.DemandSpace(8, writer);
					numArray = writer.ioBuffer;
					num = writer.ioIndex;
					numArray[num] = (byte)value;
					numArray[num + 1] = (byte)(value >> 8);
					numArray[num + 2] = (byte)(value >> 16);
					numArray[num + 3] = (byte)(value >> 24);
					numArray[num + 4] = (byte)(value >> 32);
					numArray[num + 5] = (byte)(value >> 40);
					numArray[num + 6] = (byte)(value >> 48);
					numArray[num + 7] = (byte)(value >> 56);
					ProtoWriter.IncrementedAndReset(8, writer);
					return;
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
							throw ProtoWriter.CreateException(writer);
						}
						ProtoWriter.WriteUInt64Variant(ProtoWriter.Zig(value), writer);
						writer.wireType = ProtoBuf.WireType.None;
						return;
					}
				}
			}
			ProtoWriter.WriteInt32(checked((int)value), writer);
		}

		public static void WriteObject(object value, int key, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.model == null)
			{
				throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
			}
			SubItemToken subItemToken = ProtoWriter.StartSubItem(value, writer);
			if (key >= 0)
			{
				writer.model.Serialize(key, value, writer);
			}
			else if (writer.model == null || !writer.model.TrySerializeAuxiliaryType(writer, value.GetType(), DataFormat.Default, 1, value, false))
			{
				TypeModel.ThrowUnexpectedType(value.GetType());
			}
			ProtoWriter.EndSubItem(subItemToken, writer);
		}

		internal static void WriteObject(object value, int key, ProtoWriter writer, PrefixStyle style, int fieldNumber)
		{
			if (writer.model == null)
			{
				throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
			}
			if (writer.wireType != ProtoBuf.WireType.None)
			{
				throw ProtoWriter.CreateException(writer);
			}
			switch (style)
			{
				case PrefixStyle.Base128:
				{
					writer.wireType = ProtoBuf.WireType.String;
					writer.fieldNumber = fieldNumber;
					if (fieldNumber <= 0)
					{
						break;
					}
					ProtoWriter.WriteHeaderCore(fieldNumber, ProtoBuf.WireType.String, writer);
					break;
				}
				case PrefixStyle.Fixed32:
				case PrefixStyle.Fixed32BigEndian:
				{
					writer.fieldNumber = 0;
					writer.wireType = ProtoBuf.WireType.Fixed32;
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException("style");
				}
			}
			SubItemToken subItemToken = ProtoWriter.StartSubItem(value, writer, true);
			if (key >= 0)
			{
				writer.model.Serialize(key, value, writer);
			}
			else if (!writer.model.TrySerializeAuxiliaryType(writer, value.GetType(), DataFormat.Default, 1, value, false))
			{
				TypeModel.ThrowUnexpectedType(value.GetType());
			}
			ProtoWriter.EndSubItem(subItemToken, writer, style);
		}

		public static void WriteRecursionSafeObject(object value, int key, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.model == null)
			{
				throw new InvalidOperationException("Cannot serialize sub-objects unless a model is provided");
			}
			SubItemToken subItemToken = ProtoWriter.StartSubItem(null, writer);
			writer.model.Serialize(key, value, writer);
			ProtoWriter.EndSubItem(subItemToken, writer);
		}

		public static void WriteSByte(sbyte value, ProtoWriter writer)
		{
			ProtoWriter.WriteInt32(value, writer);
		}

		public static void WriteSingle(float value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ProtoBuf.WireType wireType = writer.wireType;
			if (wireType == ProtoBuf.WireType.Fixed64)
			{
				ProtoWriter.WriteDouble((double)value, writer);
				return;
			}
			if (wireType != ProtoBuf.WireType.Fixed32)
			{
				throw ProtoWriter.CreateException(writer);
			}
			ProtoWriter.WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(value), 0), writer);
		}

		public static void WriteString(string value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			if (writer.wireType != ProtoBuf.WireType.String)
			{
				throw ProtoWriter.CreateException(writer);
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.Length == 0)
			{
				ProtoWriter.WriteUInt32Variant(0, writer);
				writer.wireType = ProtoBuf.WireType.None;
				return;
			}
			int byteCount = ProtoWriter.encoding.GetByteCount(value);
			ProtoWriter.WriteUInt32Variant((uint)byteCount, writer);
			ProtoWriter.DemandSpace(byteCount, writer);
			int bytes = ProtoWriter.encoding.GetBytes(value, 0, value.Length, writer.ioBuffer, writer.ioIndex);
			ProtoWriter.IncrementedAndReset(bytes, writer);
		}

		public static void WriteType(Type value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ProtoWriter.WriteString(writer.SerializeType(value), writer);
		}

		public static void WriteUInt16(ushort value, ProtoWriter writer)
		{
			ProtoWriter.WriteUInt32(value, writer);
		}

		public static void WriteUInt32(uint value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ProtoBuf.WireType wireType = writer.wireType;
			switch (wireType)
			{
				case ProtoBuf.WireType.Variant:
				{
					ProtoWriter.WriteUInt32Variant(value, writer);
					writer.wireType = ProtoBuf.WireType.None;
					return;
				}
				case ProtoBuf.WireType.Fixed64:
				{
					ProtoWriter.WriteInt64((long)value, writer);
					return;
				}
				default:
				{
					if (wireType != ProtoBuf.WireType.Fixed32)
					{
						break;
					}
					else
					{
						ProtoWriter.WriteInt32((int)value, writer);
						return;
					}
				}
			}
			throw ProtoWriter.CreateException(writer);
		}

		private static void WriteUInt32Variant(uint value, ProtoWriter writer)
		{
			UInt32 num;
			ProtoWriter.DemandSpace(5, writer);
			int num1 = 0;
			do
			{
				byte[] numArray = writer.ioBuffer;
				ProtoWriter protoWriter = writer;
				int num2 = protoWriter.ioIndex;
				int num3 = num2;
				protoWriter.ioIndex = num2 + 1;
				numArray[num3] = (byte)(value & 127 | 128);
				num1++;
				num = value >> 7;
				value = num;
			}
			while (num != 0);
			ref byte numPointer = ref writer.ioBuffer[writer.ioIndex - 1];
			numPointer = (byte)(numPointer & 127);
			writer.position += num1;
		}

		public static void WriteUInt64(ulong value, ProtoWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			ProtoBuf.WireType wireType = writer.wireType;
			switch (wireType)
			{
				case ProtoBuf.WireType.Variant:
				{
					ProtoWriter.WriteUInt64Variant(value, writer);
					writer.wireType = ProtoBuf.WireType.None;
					return;
				}
				case ProtoBuf.WireType.Fixed64:
				{
					ProtoWriter.WriteInt64((long)value, writer);
					return;
				}
				default:
				{
					if (wireType == ProtoBuf.WireType.Fixed32)
					{
						break;
					}
					else
					{
						throw ProtoWriter.CreateException(writer);
					}
				}
			}
			ProtoWriter.WriteUInt32((void*)(checked((uint)value)), writer);
		}

		private static void WriteUInt64Variant(ulong value, ProtoWriter writer)
		{
			UInt64 num;
			ProtoWriter.DemandSpace(10, writer);
			int num1 = 0;
			do
			{
				byte[] numArray = writer.ioBuffer;
				ProtoWriter protoWriter = writer;
				int num2 = protoWriter.ioIndex;
				int num3 = num2;
				protoWriter.ioIndex = num2 + 1;
				numArray[num3] = (byte)(value & (long)127 | (long)128);
				num1++;
				num = value >> 7;
				value = num;
			}
			while (num != (long)0);
			ref byte numPointer = ref writer.ioBuffer[writer.ioIndex - 1];
			numPointer = (byte)(numPointer & 127);
			writer.position += num1;
		}

		internal static uint Zig(int value)
		{
			return (uint)(value << 1 ^ value >> 31);
		}

		internal static ulong Zig(long value)
		{
			return (ulong)(value << 1 ^ value >> 63);
		}
	}
}