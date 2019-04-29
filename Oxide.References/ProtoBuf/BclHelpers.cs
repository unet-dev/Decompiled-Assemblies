using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace ProtoBuf
{
	public static class BclHelpers
	{
		private const int FieldTimeSpanValue = 1;

		private const int FieldTimeSpanScale = 2;

		private const int FieldDecimalLow = 1;

		private const int FieldDecimalHigh = 2;

		private const int FieldDecimalSignScale = 3;

		private const int FieldGuidLow = 1;

		private const int FieldGuidHigh = 2;

		private const int FieldExistingObjectKey = 1;

		private const int FieldNewObjectKey = 2;

		private const int FieldExistingTypeKey = 3;

		private const int FieldNewTypeKey = 4;

		private const int FieldTypeName = 8;

		private const int FieldObject = 10;

		internal readonly static DateTime EpochOrigin;

		static BclHelpers()
		{
			BclHelpers.EpochOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
		}

		public static object GetUninitializedObject(Type type)
		{
			return FormatterServices.GetUninitializedObject(type);
		}

		public static DateTime ReadDateTime(ProtoReader source)
		{
			long num = BclHelpers.ReadTimeSpanTicks(source);
			if (num == -9223372036854775808L)
			{
				return DateTime.MinValue;
			}
			if (num == 9223372036854775807L)
			{
				return DateTime.MaxValue;
			}
			return BclHelpers.EpochOrigin.AddTicks(num);
		}

		public static decimal ReadDecimal(ProtoReader reader)
		{
			ulong num = (ulong)0;
			uint num1 = 0;
			uint num2 = 0;
			SubItemToken subItemToken = ProtoReader.StartSubItem(reader);
			while (true)
			{
				int num3 = reader.ReadFieldHeader();
				int num4 = num3;
				if (num3 <= 0)
				{
					break;
				}
				switch (num4)
				{
					case 1:
					{
						num = reader.ReadUInt64();
						continue;
					}
					case 2:
					{
						num1 = reader.ReadUInt32();
						continue;
					}
					case 3:
					{
						num2 = reader.ReadUInt32();
						continue;
					}
					default:
					{
						reader.SkipField();
						continue;
					}
				}
			}
			ProtoReader.EndSubItem(subItemToken, reader);
			if (num == (long)0 && num1 == 0)
			{
				return new decimal(0);
			}
			int num5 = (int)(num & (ulong)-1);
			int num6 = (int)(num >> 32 & (ulong)-1);
			int num7 = (int)num1;
			bool flag = (num2 & 1) == 1;
			byte num8 = (byte)((num2 & 510) >> 1);
			return new decimal(num5, num6, num7, flag, num8);
		}

		public static Guid ReadGuid(ProtoReader source)
		{
			ulong num = (ulong)0;
			ulong num1 = (ulong)0;
			SubItemToken subItemToken = ProtoReader.StartSubItem(source);
			while (true)
			{
				int num2 = source.ReadFieldHeader();
				int num3 = num2;
				if (num2 <= 0)
				{
					break;
				}
				switch (num3)
				{
					case 1:
					{
						num = source.ReadUInt64();
						continue;
					}
					case 2:
					{
						num1 = source.ReadUInt64();
						continue;
					}
					default:
					{
						source.SkipField();
						continue;
					}
				}
			}
			ProtoReader.EndSubItem(subItemToken, source);
			if (num == (long)0 && num1 == (long)0)
			{
				return Guid.Empty;
			}
			uint num4 = (uint)(num >> 32);
			uint num5 = (uint)num;
			uint num6 = (uint)(num1 >> 32);
			uint num7 = (uint)num1;
			return new Guid((int)num5, (short)num4, (short)(num4 >> 16), (byte)num7, (byte)(num7 >> 8), (byte)(num7 >> 16), (byte)(num7 >> 24), (byte)num6, (byte)(num6 >> 8), (byte)(num6 >> 16), (byte)(num6 >> 24));
		}

		public static object ReadNetObject(object value, ProtoReader source, int key, Type type, BclHelpers.NetObjectOptions options)
		{
			int num;
			bool flag;
			SubItemToken subItemToken = ProtoReader.StartSubItem(source);
			int num1 = -1;
			int num2 = -1;
			do
			{
			Label0:
				int num3 = source.ReadFieldHeader();
				int num4 = num3;
				if (num3 <= 0)
				{
					if (num1 >= 0 && (byte)(options & BclHelpers.NetObjectOptions.AsReference) == 0)
					{
						throw new ProtoException("Object key in input stream, but reference-tracking was not expected");
					}
					ProtoReader.EndSubItem(subItemToken, source);
					return value;
				}
				switch (num4)
				{
					case 1:
					{
						num = source.ReadInt32();
						value = source.NetCache.GetKeyedObject(num);
						goto Label0;
					}
					case 2:
					{
						num1 = source.ReadInt32();
						goto Label0;
					}
					case 3:
					{
						num = source.ReadInt32();
						type = (Type)source.NetCache.GetKeyedObject(num);
						key = source.GetTypeKey(ref type);
						goto Label0;
					}
					case 4:
					{
						num2 = source.ReadInt32();
						goto Label0;
					}
					case 5:
					case 6:
					case 7:
					case 9:
					{
						source.SkipField();
						goto Label0;
					}
					case 8:
					{
						string str = source.ReadString();
						type = source.DeserializeType(str);
						if (type == null)
						{
							throw new ProtoException(string.Concat("Unable to resolve type: ", str, " (you can use the TypeModel.DynamicTypeFormatting event to provide a custom mapping)"));
						}
						if (type != typeof(string))
						{
							key = source.GetTypeKey(ref type);
							continue;
						}
						else
						{
							key = -1;
							goto Label0;
						}
					}
					case 10:
					{
						bool flag1 = type == typeof(string);
						bool flag2 = value == null;
						if (!flag2)
						{
							flag = false;
						}
						else
						{
							flag = (flag1 ? true : (byte)(options & BclHelpers.NetObjectOptions.LateSet) != 0);
						}
						bool flag3 = flag;
						if (num1 >= 0 && !flag3)
						{
							if (value != null)
							{
								source.NetCache.SetKeyedObject(num1, value);
							}
							else
							{
								source.TrapNextObject(num1);
							}
							if (num2 >= 0)
							{
								source.NetCache.SetKeyedObject(num2, type);
							}
						}
						object keyedObject = value;
						if (!flag1)
						{
							value = ProtoReader.ReadTypedObject(keyedObject, key, source, type);
						}
						else
						{
							value = source.ReadString();
						}
						if (num1 >= 0)
						{
							if (flag2 && !flag3)
							{
								keyedObject = source.NetCache.GetKeyedObject(num1);
							}
							if (flag3)
							{
								source.NetCache.SetKeyedObject(num1, value);
								if (num2 >= 0)
								{
									source.NetCache.SetKeyedObject(num2, type);
								}
							}
						}
						if (num1 >= 0 && !flag3 && !object.ReferenceEquals(keyedObject, value))
						{
							throw new ProtoException("A reference-tracked object changed reference during deserialization");
						}
						if (num1 >= 0 || num2 < 0)
						{
							goto Label0;
						}
						source.NetCache.SetKeyedObject(num2, type);
						goto Label0;
					}
					default:
					{
						goto case 9;
					}
				}
			}
			while (key >= 0);
			throw new InvalidOperationException(string.Concat("Dynamic type is not a contract-type: ", type.Name));
		}

		public static TimeSpan ReadTimeSpan(ProtoReader source)
		{
			long num = BclHelpers.ReadTimeSpanTicks(source);
			if (num == -9223372036854775808L)
			{
				return TimeSpan.MinValue;
			}
			if (num == 9223372036854775807L)
			{
				return TimeSpan.MaxValue;
			}
			return TimeSpan.FromTicks(num);
		}

		private static long ReadTimeSpanTicks(ProtoReader source)
		{
			TimeSpanScale timeSpanScale;
			switch (source.WireType)
			{
				case WireType.Fixed64:
				{
					return source.ReadInt64();
				}
				case WireType.String:
				case WireType.StartGroup:
				{
					SubItemToken subItemToken = ProtoReader.StartSubItem(source);
					timeSpanScale = TimeSpanScale.Days;
					long num = (long)0;
					while (true)
					{
						int num1 = source.ReadFieldHeader();
						int num2 = num1;
						if (num1 <= 0)
						{
							break;
						}
						switch (num2)
						{
							case 1:
							{
								source.Assert(WireType.SignedVariant);
								num = source.ReadInt64();
								continue;
							}
							case 2:
							{
								timeSpanScale = (TimeSpanScale)source.ReadInt32();
								continue;
							}
							default:
							{
								source.SkipField();
								continue;
							}
						}
					}
					ProtoReader.EndSubItem(subItemToken, source);
					TimeSpanScale timeSpanScale1 = timeSpanScale;
					switch (timeSpanScale1)
					{
						case TimeSpanScale.Days:
						{
							return num * 864000000000L;
						}
						case TimeSpanScale.Hours:
						{
							return num * 36000000000L;
						}
						case TimeSpanScale.Minutes:
						{
							return num * (long)600000000;
						}
						case TimeSpanScale.Seconds:
						{
							return num * (long)10000000;
						}
						case TimeSpanScale.Milliseconds:
						{
							return num * (long)10000;
						}
						case TimeSpanScale.Ticks:
						{
							return num;
						}
						default:
						{
							if (timeSpanScale1 == TimeSpanScale.MinMax)
							{
								break;
							}
							else
							{
								throw new ProtoException(string.Concat("Unknown timescale: ", timeSpanScale.ToString()));
							}
						}
					}
					long num3 = num;
					if (num3 <= (long)1 && num3 >= (long)-1)
					{
						switch ((int)(num3 - (long)-1))
						{
							case 0:
							{
								return -9223372036854775808L;
							}
							case 2:
							{
								return 9223372036854775807L;
							}
						}
					}
					throw new ProtoException(string.Concat("Unknown min/max value: ", num.ToString()));
				}
			}
			throw new ProtoException(string.Concat("Unexpected wire-type: ", source.WireType.ToString()));
			throw new ProtoException(string.Concat("Unknown timescale: ", timeSpanScale.ToString()));
		}

		public static void WriteDateTime(DateTime value, ProtoWriter dest)
		{
			TimeSpan maxValue;
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			switch (dest.WireType)
			{
				case WireType.String:
				case WireType.StartGroup:
				{
					if (value == DateTime.MaxValue)
					{
						maxValue = TimeSpan.MaxValue;
						break;
					}
					else if (value != DateTime.MinValue)
					{
						maxValue = value - BclHelpers.EpochOrigin;
						break;
					}
					else
					{
						maxValue = TimeSpan.MinValue;
						break;
					}
				}
				default:
				{
					maxValue = value - BclHelpers.EpochOrigin;
					break;
				}
			}
			BclHelpers.WriteTimeSpan(maxValue, dest);
		}

		public static void WriteDecimal(decimal value, ProtoWriter writer)
		{
			int[] bits = decimal.GetBits(value);
			ulong num = (ulong)((long)bits[1] << 32);
			ulong num1 = (long)bits[0] & (ulong)-1;
			ulong num2 = num | num1;
			uint num3 = (uint)bits[2];
			uint num4 = (uint)(bits[3] >> 15 & 510 | bits[3] >> 31 & 1);
			SubItemToken subItemToken = ProtoWriter.StartSubItem(null, writer);
			if (num2 != (long)0)
			{
				ProtoWriter.WriteFieldHeader(1, WireType.Variant, writer);
				ProtoWriter.WriteUInt64(num2, writer);
			}
			if (num3 != 0)
			{
				ProtoWriter.WriteFieldHeader(2, WireType.Variant, writer);
				ProtoWriter.WriteUInt32(num3, writer);
			}
			if (num4 != 0)
			{
				ProtoWriter.WriteFieldHeader(3, WireType.Variant, writer);
				ProtoWriter.WriteUInt32(num4, writer);
			}
			ProtoWriter.EndSubItem(subItemToken, writer);
		}

		public static void WriteGuid(Guid value, ProtoWriter dest)
		{
			byte[] byteArray = value.ToByteArray();
			SubItemToken subItemToken = ProtoWriter.StartSubItem(null, dest);
			if (value != Guid.Empty)
			{
				ProtoWriter.WriteFieldHeader(1, WireType.Fixed64, dest);
				ProtoWriter.WriteBytes(byteArray, 0, 8, dest);
				ProtoWriter.WriteFieldHeader(2, WireType.Fixed64, dest);
				ProtoWriter.WriteBytes(byteArray, 8, 8, dest);
			}
			ProtoWriter.EndSubItem(subItemToken, dest);
		}

		public static void WriteNetObject(object value, ProtoWriter dest, int key, BclHelpers.NetObjectOptions options)
		{
			bool flag;
			bool flag1;
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			bool flag2 = (byte)(options & BclHelpers.NetObjectOptions.DynamicType) != 0;
			bool flag3 = (byte)(options & BclHelpers.NetObjectOptions.AsReference) != 0;
			WireType wireType = dest.WireType;
			SubItemToken subItemToken = ProtoWriter.StartSubItem(null, dest);
			bool flag4 = true;
			if (flag3)
			{
				int num = dest.NetCache.AddObjectKey(value, out flag);
				ProtoWriter.WriteFieldHeader((flag ? 1 : 2), WireType.Variant, dest);
				ProtoWriter.WriteInt32(num, dest);
				if (flag)
				{
					flag4 = false;
				}
			}
			if (flag4)
			{
				if (flag2)
				{
					Type type = value.GetType();
					if (!(value is string))
					{
						key = dest.GetTypeKey(ref type);
						if (key < 0)
						{
							throw new InvalidOperationException(string.Concat("Dynamic type is not a contract-type: ", type.Name));
						}
					}
					int num1 = dest.NetCache.AddObjectKey(type, out flag1);
					ProtoWriter.WriteFieldHeader((flag1 ? 3 : 4), WireType.Variant, dest);
					ProtoWriter.WriteInt32(num1, dest);
					if (!flag1)
					{
						ProtoWriter.WriteFieldHeader(8, WireType.String, dest);
						ProtoWriter.WriteString(dest.SerializeType(type), dest);
					}
				}
				ProtoWriter.WriteFieldHeader(10, wireType, dest);
				if (!(value is string))
				{
					ProtoWriter.WriteObject(value, key, dest);
				}
				else
				{
					ProtoWriter.WriteString((string)value, dest);
				}
			}
			ProtoWriter.EndSubItem(subItemToken, dest);
		}

		public static void WriteTimeSpan(TimeSpan timeSpan, ProtoWriter dest)
		{
			TimeSpanScale timeSpanScale;
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			switch (dest.WireType)
			{
				case WireType.Fixed64:
				{
					ProtoWriter.WriteInt64(timeSpan.Ticks, dest);
					return;
				}
				case WireType.String:
				case WireType.StartGroup:
				{
					long ticks = timeSpan.Ticks;
					if (timeSpan == TimeSpan.MaxValue)
					{
						ticks = (long)1;
						timeSpanScale = TimeSpanScale.MinMax;
					}
					else if (timeSpan == TimeSpan.MinValue)
					{
						ticks = (long)-1;
						timeSpanScale = TimeSpanScale.MinMax;
					}
					else if (ticks % 864000000000L == (long)0)
					{
						timeSpanScale = TimeSpanScale.Days;
						ticks /= 864000000000L;
					}
					else if (ticks % 36000000000L == (long)0)
					{
						timeSpanScale = TimeSpanScale.Hours;
						ticks /= 36000000000L;
					}
					else if (ticks % (long)600000000 == (long)0)
					{
						timeSpanScale = TimeSpanScale.Minutes;
						ticks /= (long)600000000;
					}
					else if (ticks % (long)10000000 == (long)0)
					{
						timeSpanScale = TimeSpanScale.Seconds;
						ticks /= (long)10000000;
					}
					else if (ticks % (long)10000 != (long)0)
					{
						timeSpanScale = TimeSpanScale.Ticks;
					}
					else
					{
						timeSpanScale = TimeSpanScale.Milliseconds;
						ticks /= (long)10000;
					}
					SubItemToken subItemToken = ProtoWriter.StartSubItem(null, dest);
					if (ticks != (long)0)
					{
						ProtoWriter.WriteFieldHeader(1, WireType.SignedVariant, dest);
						ProtoWriter.WriteInt64(ticks, dest);
					}
					if (timeSpanScale != TimeSpanScale.Days)
					{
						ProtoWriter.WriteFieldHeader(2, WireType.Variant, dest);
						ProtoWriter.WriteInt32((int)timeSpanScale, dest);
					}
					ProtoWriter.EndSubItem(subItemToken, dest);
					return;
				}
			}
			throw new ProtoException(string.Concat("Unexpected wire-type: ", dest.WireType.ToString()));
		}

		[Flags]
		public enum NetObjectOptions : byte
		{
			None = 0,
			AsReference = 1,
			DynamicType = 2,
			UseConstructor = 4,
			LateSet = 8
		}
	}
}