using ProtoBuf;
using ProtoBuf.Compiler;
using ProtoBuf.Meta;
using System;

namespace ProtoBuf.Serializers
{
	internal sealed class EnumSerializer : IProtoSerializer
	{
		private readonly Type enumType;

		private readonly EnumSerializer.EnumPair[] map;

		public Type ExpectedType
		{
			get
			{
				return this.enumType;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.RequiresOldValue
		{
			get
			{
				return false;
			}
		}

		bool ProtoBuf.Serializers.IProtoSerializer.ReturnsValue
		{
			get
			{
				return true;
			}
		}

		public EnumSerializer(Type enumType, EnumSerializer.EnumPair[] map)
		{
			if (enumType == null)
			{
				throw new ArgumentNullException("enumType");
			}
			this.enumType = enumType;
			this.map = map;
			if (map != null)
			{
				for (int i = 1; i < (int)map.Length; i++)
				{
					for (int j = 0; j < i; j++)
					{
						if (map[i].WireValue == map[j].WireValue && !object.Equals(map[i].RawValue, map[j].RawValue))
						{
							int wireValue = map[i].WireValue;
							throw new ProtoException(string.Concat("Multiple enums with wire-value ", wireValue.ToString()));
						}
						if (object.Equals(map[i].RawValue, map[j].RawValue) && map[i].WireValue != map[j].WireValue)
						{
							throw new ProtoException(string.Concat("Multiple enums with deserialized-value ", map[i].RawValue));
						}
					}
				}
			}
		}

		private int EnumToWire(object value)
		{
			switch (this.GetTypeCode())
			{
				case ProtoTypeCode.SByte:
				{
					return (sbyte)value;
				}
				case ProtoTypeCode.Byte:
				{
					return (byte)value;
				}
				case ProtoTypeCode.Int16:
				{
					return (short)value;
				}
				case ProtoTypeCode.UInt16:
				{
					return (ushort)value;
				}
				case ProtoTypeCode.Int32:
				{
					return (int)value;
				}
				case ProtoTypeCode.UInt32:
				{
					return (int)value;
				}
				case ProtoTypeCode.Int64:
				{
					return (int)((long)value);
				}
				case ProtoTypeCode.UInt64:
				{
					return (int)((ulong)value);
				}
			}
			throw new InvalidOperationException();
		}

		private ProtoTypeCode GetTypeCode()
		{
			Type underlyingType = Helpers.GetUnderlyingType(this.enumType) ?? this.enumType;
			return Helpers.GetTypeCode(underlyingType);
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitRead(CompilerContext ctx, Local valueFrom)
		{
			ProtoTypeCode typeCode = this.GetTypeCode();
			if (this.map == null)
			{
				ctx.EmitBasicRead("ReadInt32", ctx.MapType(typeof(int)));
				ctx.ConvertFromInt32(typeCode, false);
				return;
			}
			int[] wireValue = new int[(int)this.map.Length];
			object[] rawValue = new object[(int)this.map.Length];
			for (int i = 0; i < (int)this.map.Length; i++)
			{
				wireValue[i] = this.map[i].WireValue;
				rawValue[i] = this.map[i].RawValue;
			}
			using (Local local = new Local(ctx, this.ExpectedType))
			{
				using (Local local1 = new Local(ctx, ctx.MapType(typeof(int))))
				{
					ctx.EmitBasicRead("ReadInt32", ctx.MapType(typeof(int)));
					ctx.StoreValue(local1);
					CodeLabel codeLabel = ctx.DefineLabel();
					BasicList.NodeEnumerator enumerator = BasicList.GetContiguousGroups(wireValue, rawValue).GetEnumerator();
					while (enumerator.MoveNext())
					{
						BasicList.Group current = (BasicList.Group)enumerator.Current;
						CodeLabel codeLabel1 = ctx.DefineLabel();
						int count = current.Items.Count;
						if (count != 1)
						{
							ctx.LoadValue(local1);
							ctx.LoadValue(current.First);
							ctx.Subtract();
							CodeLabel[] codeLabelArray = new CodeLabel[count];
							for (int j = 0; j < count; j++)
							{
								codeLabelArray[j] = ctx.DefineLabel();
							}
							ctx.Switch(codeLabelArray);
							ctx.Branch(codeLabel1, false);
							for (int k = 0; k < count; k++)
							{
								EnumSerializer.WriteEnumValue(ctx, typeCode, codeLabelArray[k], codeLabel, current.Items[k], local);
							}
						}
						else
						{
							ctx.LoadValue(local1);
							ctx.LoadValue(current.First);
							CodeLabel codeLabel2 = ctx.DefineLabel();
							ctx.BranchIfEqual(codeLabel2, true);
							ctx.Branch(codeLabel1, false);
							EnumSerializer.WriteEnumValue(ctx, typeCode, codeLabel2, codeLabel, current.Items[0], local);
						}
						ctx.MarkLabel(codeLabel1);
					}
					ctx.LoadReaderWriter();
					ctx.LoadValue(this.ExpectedType);
					ctx.LoadValue(local1);
					ctx.EmitCall(ctx.MapType(typeof(ProtoReader)).GetMethod("ThrowEnumException"));
					ctx.MarkLabel(codeLabel);
					ctx.LoadValue(local);
				}
			}
		}

		void ProtoBuf.Serializers.IProtoSerializer.EmitWrite(CompilerContext ctx, Local valueFrom)
		{
			ProtoTypeCode typeCode = this.GetTypeCode();
			if (this.map == null)
			{
				ctx.LoadValue(valueFrom);
				ctx.ConvertToInt32(typeCode, false);
				ctx.EmitBasicWrite("WriteInt32", null);
				return;
			}
			using (Local localWithValue = ctx.GetLocalWithValue(this.ExpectedType, valueFrom))
			{
				CodeLabel codeLabel = ctx.DefineLabel();
				for (int i = 0; i < (int)this.map.Length; i++)
				{
					CodeLabel codeLabel1 = ctx.DefineLabel();
					CodeLabel codeLabel2 = ctx.DefineLabel();
					ctx.LoadValue(localWithValue);
					EnumSerializer.WriteEnumValue(ctx, typeCode, this.map[i].RawValue);
					ctx.BranchIfEqual(codeLabel2, true);
					ctx.Branch(codeLabel1, true);
					ctx.MarkLabel(codeLabel2);
					ctx.LoadValue(this.map[i].WireValue);
					ctx.EmitBasicWrite("WriteInt32", null);
					ctx.Branch(codeLabel, false);
					ctx.MarkLabel(codeLabel1);
				}
				ctx.LoadReaderWriter();
				ctx.LoadValue(localWithValue);
				ctx.CastToObject(this.ExpectedType);
				ctx.EmitCall(ctx.MapType(typeof(ProtoWriter)).GetMethod("ThrowEnumException"));
				ctx.MarkLabel(codeLabel);
			}
		}

		public object Read(object value, ProtoReader source)
		{
			int num = source.ReadInt32();
			if (this.map == null)
			{
				return this.WireToEnum(num);
			}
			for (int i = 0; i < (int)this.map.Length; i++)
			{
				if (this.map[i].WireValue == num)
				{
					return this.map[i].TypedValue;
				}
			}
			source.ThrowEnumException(this.ExpectedType, num);
			return null;
		}

		private object WireToEnum(int value)
		{
			switch (this.GetTypeCode())
			{
				case ProtoTypeCode.SByte:
				{
					return Enum.ToObject(this.enumType, (sbyte)value);
				}
				case ProtoTypeCode.Byte:
				{
					return Enum.ToObject(this.enumType, (byte)value);
				}
				case ProtoTypeCode.Int16:
				{
					return Enum.ToObject(this.enumType, (short)value);
				}
				case ProtoTypeCode.UInt16:
				{
					return Enum.ToObject(this.enumType, (ushort)value);
				}
				case ProtoTypeCode.Int32:
				{
					return Enum.ToObject(this.enumType, value);
				}
				case ProtoTypeCode.UInt32:
				{
					return Enum.ToObject(this.enumType, (uint)value);
				}
				case ProtoTypeCode.Int64:
				{
					return Enum.ToObject(this.enumType, (long)value);
				}
				case ProtoTypeCode.UInt64:
				{
					return Enum.ToObject(this.enumType, (ulong)((long)value));
				}
			}
			throw new InvalidOperationException();
		}

		public void Write(object value, ProtoWriter dest)
		{
			if (this.map == null)
			{
				ProtoWriter.WriteInt32(this.EnumToWire(value), dest);
				return;
			}
			for (int i = 0; i < (int)this.map.Length; i++)
			{
				if (object.Equals(this.map[i].TypedValue, value))
				{
					ProtoWriter.WriteInt32(this.map[i].WireValue, dest);
					return;
				}
			}
			ProtoWriter.ThrowEnumException(dest, value);
		}

		private static void WriteEnumValue(CompilerContext ctx, ProtoTypeCode typeCode, object value)
		{
			switch (typeCode)
			{
				case ProtoTypeCode.SByte:
				{
					ctx.LoadValue((sbyte)value);
					return;
				}
				case ProtoTypeCode.Byte:
				{
					ctx.LoadValue((int)value);
					return;
				}
				case ProtoTypeCode.Int16:
				{
					ctx.LoadValue((short)value);
					return;
				}
				case ProtoTypeCode.UInt16:
				{
					ctx.LoadValue((int)value);
					return;
				}
				case ProtoTypeCode.Int32:
				{
					ctx.LoadValue((int)value);
					return;
				}
				case ProtoTypeCode.UInt32:
				{
					ctx.LoadValue((int)value);
					return;
				}
				case ProtoTypeCode.Int64:
				{
					ctx.LoadValue((long)value);
					return;
				}
				case ProtoTypeCode.UInt64:
				{
					ctx.LoadValue((long)value);
					return;
				}
			}
			throw new InvalidOperationException();
		}

		private static void WriteEnumValue(CompilerContext ctx, ProtoTypeCode typeCode, CodeLabel handler, CodeLabel @continue, object value, Local local)
		{
			ctx.MarkLabel(handler);
			EnumSerializer.WriteEnumValue(ctx, typeCode, value);
			ctx.StoreValue(local);
			ctx.Branch(@continue, false);
		}

		public struct EnumPair
		{
			public readonly object RawValue;

			public readonly Enum TypedValue;

			public readonly int WireValue;

			public EnumPair(int wireValue, object raw, Type type)
			{
				this.WireValue = wireValue;
				this.RawValue = raw;
				this.TypedValue = (Enum)Enum.ToObject(type, raw);
			}
		}
	}
}