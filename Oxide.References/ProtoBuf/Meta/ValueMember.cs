using ProtoBuf;
using ProtoBuf.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace ProtoBuf.Meta
{
	public class ValueMember
	{
		private const byte OPTIONS_IsStrict = 1;

		private const byte OPTIONS_IsPacked = 2;

		private const byte OPTIONS_IsRequired = 4;

		private const byte OPTIONS_OverwriteList = 8;

		private const byte OPTIONS_SupportNull = 16;

		private readonly int fieldNumber;

		private readonly MemberInfo member;

		private readonly Type parentType;

		private readonly Type itemType;

		private readonly Type defaultType;

		private readonly Type memberType;

		private object defaultValue;

		private readonly RuntimeTypeModel model;

		private IProtoSerializer serializer;

		private ProtoBuf.DataFormat dataFormat;

		private bool asReference;

		private bool dynamicType;

		private MethodInfo getSpecified;

		private MethodInfo setSpecified;

		private string name;

		private byte flags;

		public bool AsReference
		{
			get
			{
				return this.asReference;
			}
			set
			{
				this.ThrowIfFrozen();
				this.asReference = value;
			}
		}

		public ProtoBuf.DataFormat DataFormat
		{
			get
			{
				return this.dataFormat;
			}
			set
			{
				this.ThrowIfFrozen();
				this.dataFormat = value;
			}
		}

		public Type DefaultType
		{
			get
			{
				return this.defaultType;
			}
		}

		public object DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.ThrowIfFrozen();
				this.defaultValue = value;
			}
		}

		public bool DynamicType
		{
			get
			{
				return this.dynamicType;
			}
			set
			{
				this.ThrowIfFrozen();
				this.dynamicType = value;
			}
		}

		public int FieldNumber
		{
			get
			{
				return this.fieldNumber;
			}
		}

		public bool IsPacked
		{
			get
			{
				return this.HasFlag(2);
			}
			set
			{
				this.SetFlag(2, value, true);
			}
		}

		public bool IsRequired
		{
			get
			{
				return this.HasFlag(4);
			}
			set
			{
				this.SetFlag(4, value, true);
			}
		}

		public bool IsStrict
		{
			get
			{
				return this.HasFlag(1);
			}
			set
			{
				this.SetFlag(1, value, true);
			}
		}

		public Type ItemType
		{
			get
			{
				return this.itemType;
			}
		}

		public MemberInfo Member
		{
			get
			{
				return this.member;
			}
		}

		public Type MemberType
		{
			get
			{
				return this.memberType;
			}
		}

		public string Name
		{
			get
			{
				if (!Helpers.IsNullOrEmpty(this.name))
				{
					return this.name;
				}
				return this.member.Name;
			}
		}

		public bool OverwriteList
		{
			get
			{
				return this.HasFlag(8);
			}
			set
			{
				this.SetFlag(8, value, true);
			}
		}

		public Type ParentType
		{
			get
			{
				return this.parentType;
			}
		}

		internal IProtoSerializer Serializer
		{
			get
			{
				if (this.serializer == null)
				{
					this.serializer = this.BuildSerializer();
				}
				return this.serializer;
			}
		}

		public bool SupportNull
		{
			get
			{
				return this.HasFlag(16);
			}
			set
			{
				this.SetFlag(16, value, true);
			}
		}

		public ValueMember(RuntimeTypeModel model, Type parentType, int fieldNumber, MemberInfo member, Type memberType, Type itemType, Type defaultType, ProtoBuf.DataFormat dataFormat, object defaultValue) : this(model, fieldNumber, memberType, itemType, defaultType, dataFormat)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			if (parentType == null)
			{
				throw new ArgumentNullException("parentType");
			}
			if (fieldNumber < 1 && !Helpers.IsEnum(parentType))
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			this.member = member;
			this.parentType = parentType;
			if (fieldNumber < 1 && !Helpers.IsEnum(parentType))
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if (defaultValue != null && model.MapType(defaultValue.GetType()) != memberType)
			{
				defaultValue = ValueMember.ParseDefaultValue(memberType, defaultValue);
			}
			this.defaultValue = defaultValue;
			MetaType metaType = model.FindWithoutAdd(memberType);
			if (metaType != null)
			{
				this.asReference = metaType.AsReferenceDefault;
				return;
			}
			this.asReference = MetaType.GetAsReferenceDefault(model, memberType);
		}

		internal ValueMember(RuntimeTypeModel model, int fieldNumber, Type memberType, Type itemType, Type defaultType, ProtoBuf.DataFormat dataFormat)
		{
			if (memberType == null)
			{
				throw new ArgumentNullException("memberType");
			}
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			this.fieldNumber = fieldNumber;
			this.memberType = memberType;
			this.itemType = itemType;
			this.defaultType = defaultType;
			this.model = model;
			this.dataFormat = dataFormat;
		}

		private IProtoSerializer BuildSerializer()
		{
			WireType wireType;
			IProtoSerializer protoSerializer;
			int num = 0;
			try
			{
				this.model.TakeLock(ref num);
				Type type = (this.itemType == null ? this.memberType : this.itemType);
				IProtoSerializer tagDecorator = ValueMember.TryGetCoreSerializer(this.model, this.dataFormat, type, out wireType, this.asReference, this.dynamicType, this.OverwriteList, true);
				if (tagDecorator == null)
				{
					throw new InvalidOperationException(string.Concat("No serializer defined for type: ", type.FullName));
				}
				if (this.itemType == null || !this.SupportNull)
				{
					tagDecorator = new TagDecorator(this.fieldNumber, wireType, this.IsStrict, tagDecorator);
				}
				else
				{
					if (this.IsPacked)
					{
						throw new NotSupportedException("Packed encodings cannot support null values");
					}
					tagDecorator = new TagDecorator(1, wireType, this.IsStrict, tagDecorator);
					tagDecorator = new NullDecorator(this.model, tagDecorator);
					tagDecorator = new TagDecorator(this.fieldNumber, WireType.StartGroup, false, tagDecorator);
				}
				if (this.itemType != null)
				{
					if (!this.SupportNull)
					{
						Helpers.GetUnderlyingType(this.itemType);
					}
					if (!this.memberType.IsArray)
					{
						tagDecorator = ListDecorator.Create(this.model, this.memberType, this.defaultType, tagDecorator, this.fieldNumber, this.IsPacked, wireType, (this.member == null ? false : PropertyDecorator.CanWrite(this.model, this.member)), this.OverwriteList, this.SupportNull);
					}
					else
					{
						tagDecorator = new ArrayDecorator(this.model, tagDecorator, this.fieldNumber, this.IsPacked, wireType, this.memberType, this.OverwriteList, this.SupportNull);
					}
				}
				else if (this.defaultValue != null && !this.IsRequired && this.getSpecified == null)
				{
					tagDecorator = new DefaultValueDecorator(this.model, this.defaultValue, tagDecorator);
				}
				if (this.memberType == this.model.MapType(typeof(Uri)))
				{
					tagDecorator = new UriDecorator(this.model, tagDecorator);
				}
				if (this.member != null)
				{
					if (!(this.member is PropertyInfo))
					{
						if (!(this.member is FieldInfo))
						{
							throw new InvalidOperationException();
						}
						tagDecorator = new FieldDecorator(this.parentType, (FieldInfo)this.member, tagDecorator);
					}
					else
					{
						tagDecorator = new PropertyDecorator(this.model, this.parentType, (PropertyInfo)this.member, tagDecorator);
					}
					if (this.getSpecified != null || this.setSpecified != null)
					{
						tagDecorator = new MemberSpecifiedDecorator(this.getSpecified, this.setSpecified, tagDecorator);
					}
				}
				protoSerializer = tagDecorator;
			}
			finally
			{
				this.model.ReleaseLock(num);
			}
			return protoSerializer;
		}

		private static WireType GetDateTimeWireType(ProtoBuf.DataFormat format)
		{
			switch (format)
			{
				case ProtoBuf.DataFormat.Default:
				{
					return WireType.String;
				}
				case ProtoBuf.DataFormat.ZigZag:
				case ProtoBuf.DataFormat.TwosComplement:
				{
					throw new InvalidOperationException();
				}
				case ProtoBuf.DataFormat.FixedSize:
				{
					return WireType.Fixed64;
				}
				case ProtoBuf.DataFormat.Group:
				{
					return WireType.StartGroup;
				}
				default:
				{
					throw new InvalidOperationException();
				}
			}
		}

		private static WireType GetIntWireType(ProtoBuf.DataFormat format, int width)
		{
			switch (format)
			{
				case ProtoBuf.DataFormat.Default:
				case ProtoBuf.DataFormat.TwosComplement:
				{
					return WireType.Variant;
				}
				case ProtoBuf.DataFormat.ZigZag:
				{
					return WireType.SignedVariant;
				}
				case ProtoBuf.DataFormat.FixedSize:
				{
					if (width != 32)
					{
						return WireType.Fixed64;
					}
					return WireType.Fixed32;
				}
			}
			throw new InvalidOperationException();
		}

		internal object GetRawEnumValue()
		{
			return ((FieldInfo)this.member).GetRawConstantValue();
		}

		internal string GetSchemaTypeName(bool applyNetObjectProxy, ref bool requiresBclImport)
		{
			Type itemType = this.ItemType ?? this.MemberType;
			return this.model.GetSchemaTypeName(itemType, this.DataFormat, (!applyNetObjectProxy ? false : this.asReference), (!applyNetObjectProxy ? false : this.dynamicType), ref requiresBclImport);
		}

		private bool HasFlag(byte flag)
		{
			return (this.flags & flag) == flag;
		}

		private static object ParseDefaultValue(Type type, object value)
		{
			Type underlyingType = Helpers.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
			}
			if (value is string)
			{
				string str = (string)value;
				if (Helpers.IsEnum(type))
				{
					return Helpers.ParseEnum(type, str);
				}
				ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
				switch (typeCode)
				{
					case ProtoTypeCode.Boolean:
					{
						return bool.Parse(str);
					}
					case ProtoTypeCode.Char:
					{
						if (str.Length != 1)
						{
							throw new FormatException(string.Concat("Single character expected: \"", str, "\""));
						}
						return str[0];
					}
					case ProtoTypeCode.SByte:
					{
						return sbyte.Parse(str, NumberStyles.Integer, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.Byte:
					{
						return byte.Parse(str, NumberStyles.Integer, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.Int16:
					{
						return short.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.UInt16:
					{
						return ushort.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.Int32:
					{
						return int.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.UInt32:
					{
						return uint.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.Int64:
					{
						return long.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.UInt64:
					{
						return ulong.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.Single:
					{
						return float.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.Double:
					{
						return double.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.Decimal:
					{
						return decimal.Parse(str, NumberStyles.Any, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.DateTime:
					{
						return DateTime.Parse(str, CultureInfo.InvariantCulture);
					}
					case ProtoTypeCode.Unknown | ProtoTypeCode.DateTime:
					{
						break;
					}
					case ProtoTypeCode.String:
					{
						return str;
					}
					default:
					{
						switch (typeCode)
						{
							case ProtoTypeCode.TimeSpan:
							{
								return TimeSpan.Parse(str);
							}
							case ProtoTypeCode.Guid:
							{
								return new Guid(str);
							}
							case ProtoTypeCode.Uri:
							{
								return str;
							}
						}
						break;
					}
				}
			}
			if (Helpers.IsEnum(type))
			{
				return Enum.ToObject(type, value);
			}
			return Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
		}

		private void SetFlag(byte flag, bool value, bool throwIfFrozen)
		{
			if (throwIfFrozen && this.HasFlag(flag) != value)
			{
				this.ThrowIfFrozen();
			}
			if (value)
			{
				ValueMember valueMember = this;
				valueMember.flags = (byte)(valueMember.flags | flag);
				return;
			}
			this.flags = (byte)(this.flags & ~flag);
		}

		internal void SetName(string name)
		{
			this.ThrowIfFrozen();
			this.name = name;
		}

		public void SetSpecified(MethodInfo getSpecified, MethodInfo setSpecified)
		{
			if (getSpecified != null && (getSpecified.ReturnType != this.model.MapType(typeof(bool)) || getSpecified.IsStatic || (int)getSpecified.GetParameters().Length != 0))
			{
				throw new ArgumentException("Invalid pattern for checking member-specified", "getSpecified");
			}
			if (setSpecified != null)
			{
				if (setSpecified.ReturnType == this.model.MapType(typeof(void)) && !setSpecified.IsStatic)
				{
					ParameterInfo[] parameters = setSpecified.GetParameters();
					ParameterInfo[] parameterInfoArray = parameters;
					if ((int)parameters.Length == 1 && parameterInfoArray[0].ParameterType == this.model.MapType(typeof(bool)))
					{
						this.ThrowIfFrozen();
						this.getSpecified = getSpecified;
						this.setSpecified = setSpecified;
						return;
					}
				}
				throw new ArgumentException("Invalid pattern for setting member-specified", "setSpecified");
			}
			this.ThrowIfFrozen();
			this.getSpecified = getSpecified;
			this.setSpecified = setSpecified;
		}

		private void ThrowIfFrozen()
		{
			if (this.serializer != null)
			{
				throw new InvalidOperationException("The type cannot be changed once a serializer has been generated");
			}
		}

		internal static IProtoSerializer TryGetCoreSerializer(RuntimeTypeModel model, ProtoBuf.DataFormat dataFormat, Type type, out WireType defaultWireType, bool asReference, bool dynamicType, bool overwriteList, bool allowComplexTypes)
		{
			IProtoSerializer protoSerializer;
			Type underlyingType = Helpers.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
			}
			if (Helpers.IsEnum(type))
			{
				if (!allowComplexTypes || model == null)
				{
					defaultWireType = WireType.None;
					return null;
				}
				defaultWireType = WireType.Variant;
				return new EnumSerializer(type, model.GetEnumMap(type));
			}
			ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
			switch (typeCode)
			{
				case ProtoTypeCode.Boolean:
				{
					defaultWireType = WireType.Variant;
					return new BooleanSerializer(model);
				}
				case ProtoTypeCode.Char:
				{
					defaultWireType = WireType.Variant;
					return new CharSerializer(model);
				}
				case ProtoTypeCode.SByte:
				{
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new SByteSerializer(model);
				}
				case ProtoTypeCode.Byte:
				{
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new ByteSerializer(model);
				}
				case ProtoTypeCode.Int16:
				{
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new Int16Serializer(model);
				}
				case ProtoTypeCode.UInt16:
				{
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new UInt16Serializer(model);
				}
				case ProtoTypeCode.Int32:
				{
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new Int32Serializer(model);
				}
				case ProtoTypeCode.UInt32:
				{
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 32);
					return new UInt32Serializer(model);
				}
				case ProtoTypeCode.Int64:
				{
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 64);
					return new Int64Serializer(model);
				}
				case ProtoTypeCode.UInt64:
				{
					defaultWireType = ValueMember.GetIntWireType(dataFormat, 64);
					return new UInt64Serializer(model);
				}
				case ProtoTypeCode.Single:
				{
					defaultWireType = WireType.Fixed32;
					return new SingleSerializer(model);
				}
				case ProtoTypeCode.Double:
				{
					defaultWireType = WireType.Fixed64;
					return new DoubleSerializer(model);
				}
				case ProtoTypeCode.Decimal:
				{
					defaultWireType = WireType.String;
					return new DecimalSerializer(model);
				}
				case ProtoTypeCode.DateTime:
				{
					defaultWireType = ValueMember.GetDateTimeWireType(dataFormat);
					return new DateTimeSerializer(model);
				}
				case ProtoTypeCode.Unknown | ProtoTypeCode.DateTime:
				{
				Label0:
					if (model.AllowParseableTypes)
					{
						protoSerializer = ParseableSerializer.TryCreate(type, model);
					}
					else
					{
						protoSerializer = null;
					}
					IProtoSerializer protoSerializer1 = protoSerializer;
					if (protoSerializer1 != null)
					{
						defaultWireType = WireType.String;
						return protoSerializer1;
					}
					if (allowComplexTypes && model != null)
					{
						int key = model.GetKey(type, false, true);
						if (asReference || dynamicType)
						{
							defaultWireType = (dataFormat == ProtoBuf.DataFormat.Group ? WireType.StartGroup : WireType.String);
							BclHelpers.NetObjectOptions netObjectOption = BclHelpers.NetObjectOptions.None;
							if (asReference)
							{
								netObjectOption = (BclHelpers.NetObjectOptions)((byte)(netObjectOption | BclHelpers.NetObjectOptions.AsReference));
							}
							if (dynamicType)
							{
								netObjectOption = (BclHelpers.NetObjectOptions)((byte)(netObjectOption | BclHelpers.NetObjectOptions.DynamicType));
							}
							if (key >= 0)
							{
								if (asReference && Helpers.IsValueType(type))
								{
									string str = "AsReference cannot be used with value-types";
									str = (type.Name != "KeyValuePair`2" ? string.Concat(str, ": ", type.FullName) : string.Concat(str, "; please see http://stackoverflow.com/q/14436606/"));
									throw new InvalidOperationException(str);
								}
								MetaType item = model[type];
								if (asReference && item.IsAutoTuple)
								{
									netObjectOption = (BclHelpers.NetObjectOptions)((byte)(netObjectOption | BclHelpers.NetObjectOptions.LateSet));
								}
								if (item.UseConstructor)
								{
									netObjectOption = (BclHelpers.NetObjectOptions)((byte)(netObjectOption | BclHelpers.NetObjectOptions.UseConstructor));
								}
							}
							return new NetObjectSerializer(model, type, key, netObjectOption);
						}
						if (key >= 0)
						{
							defaultWireType = (dataFormat == ProtoBuf.DataFormat.Group ? WireType.StartGroup : WireType.String);
							return new SubItemSerializer(type, key, model[type], true);
						}
					}
					defaultWireType = WireType.None;
					return null;
				}
				case ProtoTypeCode.String:
				{
					defaultWireType = WireType.String;
					if (!asReference)
					{
						return new StringSerializer(model);
					}
					return new NetObjectSerializer(model, model.MapType(typeof(string)), 0, BclHelpers.NetObjectOptions.AsReference);
				}
				default:
				{
					switch (typeCode)
					{
						case ProtoTypeCode.TimeSpan:
						{
							defaultWireType = ValueMember.GetDateTimeWireType(dataFormat);
							return new TimeSpanSerializer(model);
						}
						case ProtoTypeCode.ByteArray:
						{
							defaultWireType = WireType.String;
							return new BlobSerializer(model, overwriteList);
						}
						case ProtoTypeCode.Guid:
						{
							defaultWireType = WireType.String;
							return new GuidSerializer(model);
						}
						case ProtoTypeCode.Uri:
						{
							defaultWireType = WireType.String;
							return new StringSerializer(model);
						}
						case ProtoTypeCode.Type:
						{
							defaultWireType = WireType.String;
							return new SystemTypeSerializer(model);
						}
						default:
						{
							goto Label0;
						}
					}
					break;
				}
			}
		}

		internal sealed class Comparer : IComparer, IComparer<ValueMember>
		{
			public readonly static ValueMember.Comparer Default;

			static Comparer()
			{
				ValueMember.Comparer.Default = new ValueMember.Comparer();
			}

			public Comparer()
			{
			}

			public int Compare(object x, object y)
			{
				return this.Compare(x as ValueMember, y as ValueMember);
			}

			public int Compare(ValueMember x, ValueMember y)
			{
				if (object.ReferenceEquals(x, y))
				{
					return 0;
				}
				if (x == null)
				{
					return -1;
				}
				if (y == null)
				{
					return 1;
				}
				return x.FieldNumber.CompareTo(y.FieldNumber);
			}
		}
	}
}