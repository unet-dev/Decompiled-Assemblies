using ProtoBuf;
using ProtoBuf.Serializers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ProtoBuf.Meta
{
	public class MetaType : ISerializerProxy
	{
		private const byte OPTIONS_Pending = 1;

		private const byte OPTIONS_EnumPassThru = 2;

		private const byte OPTIONS_Frozen = 4;

		private const byte OPTIONS_PrivateOnApi = 8;

		private const byte OPTIONS_SkipConstructor = 16;

		private const byte OPTIONS_AsReferenceDefault = 32;

		private const byte OPTIONS_AutoTuple = 64;

		private const byte OPTIONS_IgnoreListHandling = 128;

		private MetaType baseType;

		private BasicList subTypes;

		internal readonly static System.Type ienumerable;

		private CallbackSet callbacks;

		private string name;

		private MethodInfo factory;

		private readonly RuntimeTypeModel model;

		private readonly System.Type type;

		private IProtoTypeSerializer serializer;

		private System.Type constructType;

		private System.Type surrogate;

		private readonly BasicList fields = new BasicList();

		private volatile byte flags;

		public bool AsReferenceDefault
		{
			get
			{
				return this.HasFlag(32);
			}
			set
			{
				this.SetFlag(32, value, true);
			}
		}

		public MetaType BaseType
		{
			get
			{
				return this.baseType;
			}
		}

		public CallbackSet Callbacks
		{
			get
			{
				if (this.callbacks == null)
				{
					this.callbacks = new CallbackSet(this);
				}
				return this.callbacks;
			}
		}

		public System.Type ConstructType
		{
			get
			{
				return this.constructType;
			}
			set
			{
				this.ThrowIfFrozen();
				this.constructType = value;
			}
		}

		public bool EnumPassthru
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

		internal IEnumerable Fields
		{
			get
			{
				return this.fields;
			}
		}

		public bool HasCallbacks
		{
			get
			{
				if (this.callbacks == null)
				{
					return false;
				}
				return this.callbacks.NonTrivial;
			}
		}

		public bool HasSubtypes
		{
			get
			{
				if (this.subTypes == null)
				{
					return false;
				}
				return this.subTypes.Count != 0;
			}
		}

		public bool IgnoreListHandling
		{
			get
			{
				return this.HasFlag(128);
			}
			set
			{
				this.SetFlag(128, value, true);
			}
		}

		public bool IncludeSerializerMethod
		{
			get
			{
				return !this.HasFlag(8);
			}
			set
			{
				this.SetFlag(8, !value, true);
			}
		}

		internal bool IsAutoTuple
		{
			get
			{
				return this.HasFlag(64);
			}
		}

		internal bool IsList
		{
			get
			{
				System.Type listItemType;
				if (this.IgnoreListHandling)
				{
					listItemType = null;
				}
				else
				{
					listItemType = TypeModel.GetListItemType(this.model, this.type);
				}
				return listItemType != null;
			}
		}

		private bool IsValueType
		{
			get
			{
				return this.type.IsValueType;
			}
		}

		public ValueMember this[int fieldNumber]
		{
			get
			{
				BasicList.NodeEnumerator enumerator = this.fields.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ValueMember current = (ValueMember)enumerator.Current;
					if (current.FieldNumber != fieldNumber)
					{
						continue;
					}
					return current;
				}
				return null;
			}
		}

		public ValueMember this[MemberInfo member]
		{
			get
			{
				if (member == null)
				{
					return null;
				}
				BasicList.NodeEnumerator enumerator = this.fields.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ValueMember current = (ValueMember)enumerator.Current;
					if (current.Member != member)
					{
						continue;
					}
					return current;
				}
				return null;
			}
		}

		internal TypeModel Model
		{
			get
			{
				return this.model;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.ThrowIfFrozen();
				this.name = value;
			}
		}

		internal bool Pending
		{
			get
			{
				return this.HasFlag(1);
			}
			set
			{
				this.SetFlag(1, value, false);
			}
		}

		IProtoSerializer ProtoBuf.Serializers.ISerializerProxy.Serializer
		{
			get
			{
				return this.Serializer;
			}
		}

		internal IProtoTypeSerializer Serializer
		{
			get
			{
				if (this.serializer == null)
				{
					int num = 0;
					try
					{
						this.model.TakeLock(ref num);
						if (this.serializer == null)
						{
							this.SetFlag(4, true, false);
							this.serializer = this.BuildSerializer();
							if (this.model.AutoCompile)
							{
								this.CompileInPlace();
							}
						}
					}
					finally
					{
						this.model.ReleaseLock(num);
					}
				}
				return this.serializer;
			}
		}

		public System.Type Type
		{
			get
			{
				return this.type;
			}
		}

		public bool UseConstructor
		{
			get
			{
				return !this.HasFlag(16);
			}
			set
			{
				this.SetFlag(16, !value, true);
			}
		}

		static MetaType()
		{
			MetaType.ienumerable = typeof(IEnumerable);
		}

		internal MetaType(RuntimeTypeModel model, System.Type type, MethodInfo factory)
		{
			this.factory = factory;
			if (model == null)
			{
				throw new ArgumentNullException("model");
			}
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (model.TryGetBasicTypeSerializer(type) != null)
			{
				throw MetaType.InbuiltType(type);
			}
			this.type = type;
			this.model = model;
			if (Helpers.IsEnum(type))
			{
				this.EnumPassthru = type.IsDefined(model.MapType(typeof(FlagsAttribute)), false);
			}
		}

		public MetaType Add(int fieldNumber, string memberName)
		{
			this.AddField(fieldNumber, memberName, null, null, null);
			return this;
		}

		public MetaType Add(string memberName)
		{
			this.Add(this.GetNextFieldNumber(), memberName);
			return this;
		}

		public MetaType Add(params string[] memberNames)
		{
			if (memberNames == null)
			{
				throw new ArgumentNullException("memberNames");
			}
			int nextFieldNumber = this.GetNextFieldNumber();
			for (int i = 0; i < (int)memberNames.Length; i++)
			{
				int num = nextFieldNumber;
				nextFieldNumber = num + 1;
				this.Add(num, memberNames[i]);
			}
			return this;
		}

		public MetaType Add(int fieldNumber, string memberName, object defaultValue)
		{
			this.AddField(fieldNumber, memberName, null, null, defaultValue);
			return this;
		}

		public MetaType Add(int fieldNumber, string memberName, System.Type itemType, System.Type defaultType)
		{
			this.AddField(fieldNumber, memberName, itemType, defaultType, null);
			return this;
		}

		private void Add(ValueMember member)
		{
			int num = 0;
			try
			{
				this.model.TakeLock(ref num);
				this.ThrowIfFrozen();
				this.fields.Add(member);
			}
			finally
			{
				this.model.ReleaseLock(num);
			}
		}

		public ValueMember AddField(int fieldNumber, string memberName)
		{
			return this.AddField(fieldNumber, memberName, null, null, null);
		}

		public ValueMember AddField(int fieldNumber, string memberName, System.Type itemType, System.Type defaultType)
		{
			return this.AddField(fieldNumber, memberName, itemType, defaultType, null);
		}

		private ValueMember AddField(int fieldNumber, string memberName, System.Type itemType, System.Type defaultType, object defaultValue)
		{
			System.Type fieldType;
			MemberInfo memberInfo = null;
			MemberInfo[] member = this.type.GetMember(memberName, (Helpers.IsEnum(this.type) ? BindingFlags.Static | BindingFlags.Public : BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			if (member != null && (int)member.Length == 1)
			{
				memberInfo = member[0];
			}
			if (memberInfo == null)
			{
				throw new ArgumentException(string.Concat("Unable to determine member: ", memberName), "memberName");
			}
			MemberTypes memberType = memberInfo.MemberType;
			if (memberType == MemberTypes.Field)
			{
				fieldType = ((FieldInfo)memberInfo).FieldType;
			}
			else
			{
				if (memberType != MemberTypes.Property)
				{
					throw new NotSupportedException(memberInfo.MemberType.ToString());
				}
				fieldType = ((PropertyInfo)memberInfo).PropertyType;
			}
			MetaType.ResolveListTypes(this.model, fieldType, ref itemType, ref defaultType);
			ValueMember valueMember = new ValueMember(this.model, this.type, fieldNumber, memberInfo, fieldType, itemType, defaultType, DataFormat.Default, defaultValue);
			this.Add(valueMember);
			return valueMember;
		}

		public MetaType AddSubType(int fieldNumber, System.Type derivedType)
		{
			return this.AddSubType(fieldNumber, derivedType, DataFormat.Default);
		}

		public MetaType AddSubType(int fieldNumber, System.Type derivedType, DataFormat dataFormat)
		{
			if (derivedType == null)
			{
				throw new ArgumentNullException("derivedType");
			}
			if (fieldNumber < 1)
			{
				throw new ArgumentOutOfRangeException("fieldNumber");
			}
			if (!this.type.IsClass && !this.type.IsInterface || this.type.IsSealed)
			{
				throw new InvalidOperationException("Sub-types can only be added to non-sealed classes");
			}
			if (!this.IsValidSubType(derivedType))
			{
				throw new ArgumentException(string.Concat(derivedType.Name, " is not a valid sub-type of ", this.type.Name), "derivedType");
			}
			MetaType item = this.model[derivedType];
			this.ThrowIfFrozen();
			item.ThrowIfFrozen();
			SubType subType = new SubType(fieldNumber, item, dataFormat);
			this.ThrowIfFrozen();
			item.SetBaseType(this);
			if (this.subTypes == null)
			{
				this.subTypes = new BasicList();
			}
			this.subTypes.Add(subType);
			return this;
		}

		internal void ApplyDefaultBehaviour()
		{
			object obj;
			bool isPublic;
			bool flag;
			System.Type fieldType;
			System.Type baseType = MetaType.GetBaseType(this);
			if (baseType != null && this.model.FindWithoutAdd(baseType) == null && MetaType.GetContractFamily(this.model, baseType, null) != MetaType.AttributeFamily.None)
			{
				this.model.FindOrAddAuto(baseType, true, false, false);
			}
			AttributeMap[] attributeMapArray = AttributeMap.Create(this.model, this.type, false);
			MetaType.AttributeFamily contractFamily = MetaType.GetContractFamily(this.model, this.type, attributeMapArray);
			if (contractFamily == MetaType.AttributeFamily.AutoTuple)
			{
				this.SetFlag(64, true, true);
			}
			bool flag1 = (this.EnumPassthru ? false : Helpers.IsEnum(this.type));
			if (contractFamily == MetaType.AttributeFamily.None && !flag1)
			{
				return;
			}
			BasicList basicLists = null;
			BasicList basicLists1 = null;
			int num = 0;
			int num1 = 1;
			bool inferTagFromNameDefault = this.model.InferTagFromNameDefault;
			ImplicitFields implicitField = ImplicitFields.None;
			string str = null;
			for (int i = 0; i < (int)attributeMapArray.Length; i++)
			{
				AttributeMap attributeMap = attributeMapArray[i];
				string fullName = attributeMap.AttributeType.FullName;
				if (!flag1 && fullName == "ProtoBuf.ProtoIncludeAttribute")
				{
					int num2 = 0;
					if (attributeMap.TryGet("tag", out obj))
					{
						num2 = (int)obj;
					}
					DataFormat dataFormat = DataFormat.Default;
					if (attributeMap.TryGet("DataFormat", out obj))
					{
						dataFormat = (DataFormat)((int)obj);
					}
					System.Type type = null;
					try
					{
						if (attributeMap.TryGet("knownTypeName", out obj))
						{
							type = this.model.GetType((string)obj, this.type.Assembly);
						}
						else if (attributeMap.TryGet("knownType", out obj))
						{
							type = (System.Type)obj;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						throw new InvalidOperationException(string.Concat("Unable to resolve sub-type of: ", this.type.FullName), exception);
					}
					if (type == null)
					{
						throw new InvalidOperationException(string.Concat("Unable to resolve sub-type of: ", this.type.FullName));
					}
					if (this.IsValidSubType(type))
					{
						this.AddSubType(num2, type, dataFormat);
					}
				}
				if (fullName == "ProtoBuf.ProtoPartialIgnoreAttribute" && attributeMap.TryGet("MemberName", out obj) && obj != null)
				{
					if (basicLists == null)
					{
						basicLists = new BasicList();
					}
					basicLists.Add((string)obj);
				}
				if (!flag1 && fullName == "ProtoBuf.ProtoPartialMemberAttribute")
				{
					if (basicLists1 == null)
					{
						basicLists1 = new BasicList();
					}
					basicLists1.Add(attributeMap);
				}
				if (fullName == "ProtoBuf.ProtoContractAttribute")
				{
					if (attributeMap.TryGet("Name", out obj))
					{
						str = (string)obj;
					}
					if (!Helpers.IsEnum(this.type))
					{
						if (attributeMap.TryGet("DataMemberOffset", out obj))
						{
							num = (int)obj;
						}
						if (attributeMap.TryGet("InferTagFromNameHasValue", false, out obj) && (bool)obj && attributeMap.TryGet("InferTagFromName", out obj))
						{
							inferTagFromNameDefault = (bool)obj;
						}
						if (attributeMap.TryGet("ImplicitFields", out obj) && obj != null)
						{
							implicitField = (ImplicitFields)((int)obj);
						}
						if (attributeMap.TryGet("SkipConstructor", out obj))
						{
							this.UseConstructor = !(bool)obj;
						}
						if (attributeMap.TryGet("IgnoreListHandling", out obj))
						{
							this.IgnoreListHandling = (bool)obj;
						}
						if (attributeMap.TryGet("AsReferenceDefault", out obj))
						{
							this.AsReferenceDefault = (bool)obj;
						}
						if (attributeMap.TryGet("ImplicitFirstTag", out obj) && (int)obj > 0)
						{
							num1 = (int)obj;
						}
					}
					else if (attributeMap.TryGet("EnumPassthruHasValue", false, out obj) && (bool)obj && attributeMap.TryGet("EnumPassthru", out obj))
					{
						this.EnumPassthru = (bool)obj;
						if (this.EnumPassthru)
						{
							flag1 = false;
						}
					}
				}
				if (fullName == "System.Runtime.Serialization.DataContractAttribute" && str == null && attributeMap.TryGet("Name", out obj))
				{
					str = (string)obj;
				}
				if (fullName == "System.Xml.Serialization.XmlTypeAttribute" && str == null && attributeMap.TryGet("TypeName", out obj))
				{
					str = (string)obj;
				}
			}
			if (!Helpers.IsNullOrEmpty(str))
			{
				this.Name = str;
			}
			if (implicitField != ImplicitFields.None)
			{
				contractFamily &= MetaType.AttributeFamily.ProtoBuf;
			}
			MethodInfo[] methodInfoArray = null;
			BasicList basicLists2 = new BasicList();
			MemberInfo[] members = this.type.GetMembers((flag1 ? BindingFlags.Static | BindingFlags.Public : BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
			for (int j = 0; j < (int)members.Length; j++)
			{
				MemberInfo memberInfo = members[j];
				if (memberInfo.DeclaringType == this.type && !memberInfo.IsDefined(this.model.MapType(typeof(ProtoIgnoreAttribute)), true) && (basicLists == null || !basicLists.Contains(memberInfo.Name)))
				{
					bool flag2 = false;
					PropertyInfo propertyInfo = memberInfo as PropertyInfo;
					PropertyInfo propertyInfo1 = propertyInfo;
					if (propertyInfo == null)
					{
						FieldInfo fieldInfo = memberInfo as FieldInfo;
						FieldInfo fieldInfo1 = fieldInfo;
						if (fieldInfo == null)
						{
							MethodInfo methodInfo = memberInfo as MethodInfo;
							MethodInfo methodInfo1 = methodInfo;
							if (methodInfo != null && !flag1)
							{
								AttributeMap[] attributeMapArray1 = AttributeMap.Create(this.model, methodInfo1, false);
								if (attributeMapArray1 != null && (int)attributeMapArray1.Length > 0)
								{
									MetaType.CheckForCallback(methodInfo1, attributeMapArray1, "ProtoBuf.ProtoBeforeSerializationAttribute", ref methodInfoArray, 0);
									MetaType.CheckForCallback(methodInfo1, attributeMapArray1, "ProtoBuf.ProtoAfterSerializationAttribute", ref methodInfoArray, 1);
									MetaType.CheckForCallback(methodInfo1, attributeMapArray1, "ProtoBuf.ProtoBeforeDeserializationAttribute", ref methodInfoArray, 2);
									MetaType.CheckForCallback(methodInfo1, attributeMapArray1, "ProtoBuf.ProtoAfterDeserializationAttribute", ref methodInfoArray, 3);
									MetaType.CheckForCallback(methodInfo1, attributeMapArray1, "System.Runtime.Serialization.OnSerializingAttribute", ref methodInfoArray, 4);
									MetaType.CheckForCallback(methodInfo1, attributeMapArray1, "System.Runtime.Serialization.OnSerializedAttribute", ref methodInfoArray, 5);
									MetaType.CheckForCallback(methodInfo1, attributeMapArray1, "System.Runtime.Serialization.OnDeserializingAttribute", ref methodInfoArray, 6);
									MetaType.CheckForCallback(methodInfo1, attributeMapArray1, "System.Runtime.Serialization.OnDeserializedAttribute", ref methodInfoArray, 7);
								}
							}
						}
						else
						{
							fieldType = fieldInfo1.FieldType;
							isPublic = fieldInfo1.IsPublic;
							flag = true;
							if (!flag1 || fieldInfo1.IsStatic)
							{
								MetaType.ApplyDefaultBehaviour_AddMembers(this.model, contractFamily, flag1, basicLists1, num, inferTagFromNameDefault, implicitField, basicLists2, memberInfo, ref flag2, isPublic, flag, ref fieldType);
							}
						}
					}
					else if (!flag1)
					{
						fieldType = propertyInfo1.PropertyType;
						isPublic = Helpers.GetGetMethod(propertyInfo1, false, false) != null;
						flag = false;
						MetaType.ApplyDefaultBehaviour_AddMembers(this.model, contractFamily, flag1, basicLists1, num, inferTagFromNameDefault, implicitField, basicLists2, memberInfo, ref flag2, isPublic, flag, ref fieldType);
					}
				}
			}
			ProtoMemberAttribute[] protoMemberAttributeArray = new ProtoMemberAttribute[basicLists2.Count];
			basicLists2.CopyTo(protoMemberAttributeArray, 0);
			if (inferTagFromNameDefault || implicitField != ImplicitFields.None)
			{
				Array.Sort<ProtoMemberAttribute>(protoMemberAttributeArray);
				int num3 = num1;
				ProtoMemberAttribute[] protoMemberAttributeArray1 = protoMemberAttributeArray;
				for (int k = 0; k < (int)protoMemberAttributeArray1.Length; k++)
				{
					ProtoMemberAttribute protoMemberAttribute = protoMemberAttributeArray1[k];
					if (!protoMemberAttribute.TagIsPinned)
					{
						int num4 = num3;
						num3 = num4 + 1;
						protoMemberAttribute.Rebase(num4);
					}
				}
			}
			ProtoMemberAttribute[] protoMemberAttributeArray2 = protoMemberAttributeArray;
			for (int l = 0; l < (int)protoMemberAttributeArray2.Length; l++)
			{
				ValueMember valueMember = this.ApplyDefaultBehaviour(flag1, protoMemberAttributeArray2[l]);
				if (valueMember != null)
				{
					this.Add(valueMember);
				}
			}
			if (methodInfoArray != null)
			{
				this.SetCallbacks(MetaType.Coalesce(methodInfoArray, 0, 4), MetaType.Coalesce(methodInfoArray, 1, 5), MetaType.Coalesce(methodInfoArray, 2, 6), MetaType.Coalesce(methodInfoArray, 3, 7));
			}
		}

		private ValueMember ApplyDefaultBehaviour(bool isEnum, ProtoMemberAttribute normalizedAttribute)
		{
			object obj;
			ValueMember valueMember;
			if (normalizedAttribute != null)
			{
				MemberInfo member = normalizedAttribute.Member;
				MemberInfo memberInfo = member;
				if (member != null)
				{
					System.Type memberType = Helpers.GetMemberType(memberInfo);
					System.Type type = null;
					System.Type type1 = null;
					MetaType.ResolveListTypes(this.model, memberType, ref type, ref type1);
					if (type != null && this.model.FindOrAddAuto(memberType, false, true, false) >= 0 && this.model[memberType].IgnoreListHandling)
					{
						type = null;
						type1 = null;
					}
					AttributeMap[] attributeMapArray = AttributeMap.Create(this.model, memberInfo, true);
					object num = null;
					if (this.model.UseImplicitZeroDefaults)
					{
						ProtoTypeCode typeCode = Helpers.GetTypeCode(memberType);
						switch (typeCode)
						{
							case ProtoTypeCode.Boolean:
							{
								num = false;
								break;
							}
							case ProtoTypeCode.Char:
							{
								num = '\0';
								break;
							}
							case ProtoTypeCode.SByte:
							{
								num = (sbyte)0;
								break;
							}
							case ProtoTypeCode.Byte:
							{
								num = (byte)0;
								break;
							}
							case ProtoTypeCode.Int16:
							{
								num = (short)0;
								break;
							}
							case ProtoTypeCode.UInt16:
							{
								num = (ushort)0;
								break;
							}
							case ProtoTypeCode.Int32:
							{
								num = 0;
								break;
							}
							case ProtoTypeCode.UInt32:
							{
								num = (uint)0;
								break;
							}
							case ProtoTypeCode.Int64:
							{
								num = (long)0;
								break;
							}
							case ProtoTypeCode.UInt64:
							{
								num = (ulong)((long)0);
								break;
							}
							case ProtoTypeCode.Single:
							{
								num = 0f;
								break;
							}
							case ProtoTypeCode.Double:
							{
								num = 0;
								break;
							}
							case ProtoTypeCode.Decimal:
							{
								num = new decimal(0);
								break;
							}
							default:
							{
								switch (typeCode)
								{
									case ProtoTypeCode.TimeSpan:
									{
										num = TimeSpan.Zero;
										break;
									}
									case ProtoTypeCode.Guid:
									{
										num = Guid.Empty;
										break;
									}
								}
								break;
							}
						}
					}
					AttributeMap attribute = MetaType.GetAttribute(attributeMapArray, "System.ComponentModel.DefaultValueAttribute");
					AttributeMap attributeMap = attribute;
					if (attribute != null && attributeMap.TryGet("Value", out obj))
					{
						num = obj;
					}
					if (isEnum || normalizedAttribute.Tag > 0)
					{
						valueMember = new ValueMember(this.model, this.type, normalizedAttribute.Tag, memberInfo, memberType, type, type1, normalizedAttribute.DataFormat, num);
					}
					else
					{
						valueMember = null;
					}
					ValueMember isPacked = valueMember;
					if (isPacked != null)
					{
						System.Type type2 = this.type;
						PropertyInfo property = Helpers.GetProperty(type2, string.Concat(memberInfo.Name, "Specified"), true);
						MethodInfo getMethod = Helpers.GetGetMethod(property, true, true);
						if (getMethod == null || getMethod.IsStatic)
						{
							property = null;
						}
						if (property == null)
						{
							MethodInfo instanceMethod = Helpers.GetInstanceMethod(type2, string.Concat("ShouldSerialize", memberInfo.Name), Helpers.EmptyTypes);
							if (instanceMethod != null && instanceMethod.ReturnType == this.model.MapType(typeof(bool)))
							{
								isPacked.SetSpecified(instanceMethod, null);
							}
						}
						else
						{
							isPacked.SetSpecified(getMethod, Helpers.GetSetMethod(property, true, true));
						}
						if (!Helpers.IsNullOrEmpty(normalizedAttribute.Name))
						{
							isPacked.SetName(normalizedAttribute.Name);
						}
						isPacked.IsPacked = normalizedAttribute.IsPacked;
						isPacked.IsRequired = normalizedAttribute.IsRequired;
						isPacked.OverwriteList = normalizedAttribute.OverwriteList;
						if (normalizedAttribute.AsReferenceHasValue)
						{
							isPacked.AsReference = normalizedAttribute.AsReference;
						}
						isPacked.DynamicType = normalizedAttribute.DynamicType;
					}
					return isPacked;
				}
			}
			return null;
		}

		private static void ApplyDefaultBehaviour_AddMembers(TypeModel model, MetaType.AttributeFamily family, bool isEnum, BasicList partialMembers, int dataMemberOffset, bool inferTagByName, ImplicitFields implicitMode, BasicList members, MemberInfo member, ref bool forced, bool isPublic, bool isField, ref System.Type effectiveType)
		{
			switch (implicitMode)
			{
				case ImplicitFields.AllPublic:
				{
					if (!isPublic)
					{
						break;
					}
					forced = true;
					break;
				}
				case ImplicitFields.AllFields:
				{
					if (!isField)
					{
						break;
					}
					forced = true;
					break;
				}
			}
			if (effectiveType.IsSubclassOf(model.MapType(typeof(Delegate))))
			{
				effectiveType = null;
			}
			if (effectiveType != null)
			{
				ProtoMemberAttribute protoMemberAttribute = MetaType.NormalizeProtoMember(model, member, family, forced, isEnum, partialMembers, dataMemberOffset, inferTagByName);
				if (protoMemberAttribute != null)
				{
					members.Add(protoMemberAttribute);
				}
			}
		}

		private IProtoTypeSerializer BuildSerializer()
		{
			MemberInfo[] memberInfoArray;
			System.Type listItemType;
			MethodInfo beforeDeserialize;
			if (Helpers.IsEnum(this.type))
			{
				return new TagDecorator(1, WireType.Variant, false, new EnumSerializer(this.type, this.GetEnumMap()));
			}
			if (this.IgnoreListHandling)
			{
				listItemType = null;
			}
			else
			{
				listItemType = TypeModel.GetListItemType(this.model, this.type);
			}
			System.Type type = listItemType;
			if (type != null)
			{
				if (this.surrogate != null)
				{
					throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot use a surrogate");
				}
				if (this.subTypes != null && this.subTypes.Count != 0)
				{
					throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be subclassed");
				}
				System.Type type1 = null;
				MetaType.ResolveListTypes(this.model, this.type, ref type, ref type1);
				ValueMember valueMember = new ValueMember(this.model, 1, this.type, type, type1, DataFormat.Default);
				RuntimeTypeModel runtimeTypeModel = this.model;
				System.Type type2 = this.type;
				int[] numArray = new int[] { 1 };
				IProtoSerializer[] serializer = new IProtoSerializer[] { valueMember.Serializer };
				return new TypeSerializer(runtimeTypeModel, type2, numArray, serializer, null, true, true, null, this.constructType, this.factory);
			}
			if (this.surrogate != null)
			{
				MetaType item = this.model[this.surrogate];
				while (true)
				{
					MetaType metaType = item.baseType;
					MetaType metaType1 = metaType;
					if (metaType == null)
					{
						break;
					}
					item = metaType1;
				}
				return new SurrogateSerializer(this.type, this.surrogate, item.Serializer);
			}
			if (this.IsAutoTuple)
			{
				ConstructorInfo constructorInfo = MetaType.ResolveTupleConstructor(this.type, out memberInfoArray);
				if (constructorInfo == null)
				{
					throw new InvalidOperationException();
				}
				return new TupleSerializer(this.model, constructorInfo, memberInfoArray);
			}
			this.fields.Trim();
			int count = this.fields.Count;
			int num = (this.subTypes == null ? 0 : this.subTypes.Count);
			int[] fieldNumber = new int[count + num];
			IProtoSerializer[] protoSerializerArray = new IProtoSerializer[count + num];
			int num1 = 0;
			if (num != 0)
			{
				BasicList.NodeEnumerator enumerator = this.subTypes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SubType current = (SubType)enumerator.Current;
					if (!current.DerivedType.IgnoreListHandling && this.model.MapType(MetaType.ienumerable).IsAssignableFrom(current.DerivedType.Type))
					{
						throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be used as a subclass");
					}
					fieldNumber[num1] = current.FieldNumber;
					int num2 = num1;
					num1 = num2 + 1;
					protoSerializerArray[num2] = current.Serializer;
				}
			}
			if (count != 0)
			{
				BasicList.NodeEnumerator nodeEnumerator = this.fields.GetEnumerator();
				while (nodeEnumerator.MoveNext())
				{
					ValueMember current1 = (ValueMember)nodeEnumerator.Current;
					fieldNumber[num1] = current1.FieldNumber;
					int num3 = num1;
					num1 = num3 + 1;
					protoSerializerArray[num3] = current1.Serializer;
				}
			}
			BasicList basicLists = null;
			for (MetaType i = this.BaseType; i != null; i = i.BaseType)
			{
				if (i.HasCallbacks)
				{
					beforeDeserialize = i.Callbacks.BeforeDeserialize;
				}
				else
				{
					beforeDeserialize = null;
				}
				MethodInfo methodInfo = beforeDeserialize;
				if (methodInfo != null)
				{
					if (basicLists == null)
					{
						basicLists = new BasicList();
					}
					basicLists.Add(methodInfo);
				}
			}
			MethodInfo[] methodInfoArray = null;
			if (basicLists != null)
			{
				methodInfoArray = new MethodInfo[basicLists.Count];
				basicLists.CopyTo(methodInfoArray, 0);
				Array.Reverse(methodInfoArray);
			}
			return new TypeSerializer(this.model, this.type, fieldNumber, protoSerializerArray, methodInfoArray, this.baseType == null, this.UseConstructor, this.callbacks, this.constructType, this.factory);
		}

		private static void CheckForCallback(MethodInfo method, AttributeMap[] attributes, string callbackTypeName, ref MethodInfo[] callbacks, int index)
		{
			for (int i = 0; i < (int)attributes.Length; i++)
			{
				if (attributes[i].AttributeType.FullName == callbackTypeName)
				{
					if (callbacks == null)
					{
						callbacks = new MethodInfo[8];
					}
					else if (callbacks[index] != null)
					{
						System.Type reflectedType = method.ReflectedType;
						throw new ProtoException(string.Concat("Duplicate ", callbackTypeName, " callbacks on ", reflectedType.FullName));
					}
					callbacks[index] = method;
				}
			}
		}

		private static MethodInfo Coalesce(MethodInfo[] arr, int x, int y)
		{
			return arr[x] ?? arr[y];
		}

		public void CompileInPlace()
		{
			this.serializer = CompiledSerializer.Wrap(this.Serializer, this.model);
		}

		internal static bool GetAsReferenceDefault(RuntimeTypeModel model, System.Type type)
		{
			object obj;
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (Helpers.IsEnum(type))
			{
				return false;
			}
			AttributeMap[] attributeMapArray = AttributeMap.Create(model, type, false);
			for (int i = 0; i < (int)attributeMapArray.Length; i++)
			{
				if (attributeMapArray[i].AttributeType.FullName == "ProtoBuf.ProtoContractAttribute" && attributeMapArray[i].TryGet("AsReferenceDefault", out obj))
				{
					return (bool)obj;
				}
			}
			return false;
		}

		private static AttributeMap GetAttribute(AttributeMap[] attribs, string fullName)
		{
			for (int i = 0; i < (int)attribs.Length; i++)
			{
				AttributeMap attributeMap = attribs[i];
				if (attributeMap != null && attributeMap.AttributeType.FullName == fullName)
				{
					return attributeMap;
				}
			}
			return null;
		}

		private static System.Type GetBaseType(MetaType type)
		{
			return type.type.BaseType;
		}

		internal static MetaType.AttributeFamily GetContractFamily(RuntimeTypeModel model, System.Type type, AttributeMap[] attributes)
		{
			MemberInfo[] memberInfoArray;
			MetaType.AttributeFamily attributeFamily = MetaType.AttributeFamily.None;
			if (attributes == null)
			{
				attributes = AttributeMap.Create(model, type, false);
			}
			for (int i = 0; i < (int)attributes.Length; i++)
			{
				string fullName = attributes[i].AttributeType.FullName;
				string str = fullName;
				if (fullName != null)
				{
					if (str == "ProtoBuf.ProtoContractAttribute")
					{
						bool flag = false;
						MetaType.GetFieldBoolean(ref flag, attributes[i], "UseProtoMembersOnly");
						if (flag)
						{
							return MetaType.AttributeFamily.ProtoBuf;
						}
						attributeFamily |= MetaType.AttributeFamily.ProtoBuf;
					}
					else if (str != "System.Xml.Serialization.XmlTypeAttribute")
					{
						if (str == "System.Runtime.Serialization.DataContractAttribute")
						{
							if (!model.AutoAddProtoContractTypesOnly)
							{
								attributeFamily |= MetaType.AttributeFamily.DataContractSerialier;
							}
						}
					}
					else if (!model.AutoAddProtoContractTypesOnly)
					{
						attributeFamily |= MetaType.AttributeFamily.XmlSerializer;
					}
				}
			}
			if (attributeFamily == MetaType.AttributeFamily.None && MetaType.ResolveTupleConstructor(type, out memberInfoArray) != null)
			{
				attributeFamily |= MetaType.AttributeFamily.AutoTuple;
			}
			return attributeFamily;
		}

		private static void GetDataFormat(ref DataFormat value, AttributeMap attrib, string memberName)
		{
			object obj;
			if (attrib == null || (int)value != 0)
			{
				return;
			}
			if (attrib.TryGet(memberName, out obj) && obj != null)
			{
				value = (DataFormat)obj;
			}
		}

		internal EnumSerializer.EnumPair[] GetEnumMap()
		{
			if (this.HasFlag(2))
			{
				return null;
			}
			EnumSerializer.EnumPair[] enumPair = new EnumSerializer.EnumPair[this.fields.Count];
			for (int i = 0; i < (int)enumPair.Length; i++)
			{
				ValueMember item = (ValueMember)this.fields[i];
				int fieldNumber = item.FieldNumber;
				object rawEnumValue = item.GetRawEnumValue();
				enumPair[i] = new EnumSerializer.EnumPair(fieldNumber, rawEnumValue, item.MemberType);
			}
			return enumPair;
		}

		private static void GetFieldBoolean(ref bool value, AttributeMap attrib, string memberName)
		{
			MetaType.GetFieldBoolean(ref value, attrib, memberName, true);
		}

		private static bool GetFieldBoolean(ref bool value, AttributeMap attrib, string memberName, bool publicOnly)
		{
			object obj;
			if (attrib == null)
			{
				return false;
			}
			if (value)
			{
				return true;
			}
			if (!attrib.TryGet(memberName, publicOnly, out obj) || obj == null)
			{
				return false;
			}
			value = (bool)obj;
			return true;
		}

		private static void GetFieldName(ref string name, AttributeMap attrib, string memberName)
		{
			object obj;
			if (attrib == null || !Helpers.IsNullOrEmpty(name))
			{
				return;
			}
			if (attrib.TryGet(memberName, out obj) && obj != null)
			{
				name = (string)obj;
			}
		}

		private static void GetFieldNumber(ref int value, AttributeMap attrib, string memberName)
		{
			object obj;
			if (attrib == null || value > 0)
			{
				return;
			}
			if (attrib.TryGet(memberName, out obj) && obj != null)
			{
				value = (int)obj;
			}
		}

		public ValueMember[] GetFields()
		{
			ValueMember[] valueMemberArray = new ValueMember[this.fields.Count];
			this.fields.CopyTo(valueMemberArray, 0);
			Array.Sort<ValueMember>(valueMemberArray, ValueMember.Comparer.Default);
			return valueMemberArray;
		}

		private static void GetIgnore(ref bool ignore, AttributeMap attrib, AttributeMap[] attribs, string fullName)
		{
			if (ignore || attrib == null)
			{
				return;
			}
			ignore = MetaType.GetAttribute(attribs, fullName) != null;
		}

		internal int GetKey(bool demand, bool getBaseKey)
		{
			return this.model.GetKey(this.type, demand, getBaseKey);
		}

		private int GetNextFieldNumber()
		{
			int fieldNumber = 0;
			BasicList.NodeEnumerator enumerator = this.fields.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ValueMember current = (ValueMember)enumerator.Current;
				if (current.FieldNumber <= fieldNumber)
				{
					continue;
				}
				fieldNumber = current.FieldNumber;
			}
			if (this.subTypes != null)
			{
				BasicList.NodeEnumerator nodeEnumerator = this.subTypes.GetEnumerator();
				while (nodeEnumerator.MoveNext())
				{
					SubType subType = (SubType)nodeEnumerator.Current;
					if (subType.FieldNumber <= fieldNumber)
					{
						continue;
					}
					fieldNumber = subType.FieldNumber;
				}
			}
			return fieldNumber + 1;
		}

		internal static MetaType GetRootType(MetaType source)
		{
			MetaType metaType;
			while (source.serializer != null)
			{
				MetaType metaType1 = source.baseType;
				if (metaType1 == null)
				{
					return source;
				}
				source = metaType1;
			}
			RuntimeTypeModel runtimeTypeModel = source.model;
			int num = 0;
			try
			{
				runtimeTypeModel.TakeLock(ref num);
				while (true)
				{
					MetaType metaType2 = source.baseType;
					MetaType metaType3 = metaType2;
					if (metaType2 == null)
					{
						break;
					}
					source = metaType3;
				}
				metaType = source;
			}
			finally
			{
				runtimeTypeModel.ReleaseLock(num);
			}
			return metaType;
		}

		internal string GetSchemaTypeName()
		{
			if (this.surrogate != null)
			{
				return this.model[this.surrogate].GetSchemaTypeName();
			}
			if (!Helpers.IsNullOrEmpty(this.name))
			{
				return this.name;
			}
			string name = this.type.Name;
			if (!this.type.IsGenericType)
			{
				return name;
			}
			StringBuilder stringBuilder = new StringBuilder(name);
			int num = name.IndexOf('\u0060');
			if (num >= 0)
			{
				stringBuilder.Length = num;
			}
			System.Type[] genericArguments = this.type.GetGenericArguments();
			for (int i = 0; i < (int)genericArguments.Length; i++)
			{
				System.Type type = genericArguments[i];
				stringBuilder.Append('\u005F');
				System.Type type1 = type;
				if (this.model.GetKey(ref type1) >= 0)
				{
					MetaType item = this.model[type1];
					MetaType metaType = item;
					if (item == null || metaType.surrogate != null)
					{
						goto Label1;
					}
					stringBuilder.Append(metaType.GetSchemaTypeName());
					goto Label0;
				}
			Label1:
				stringBuilder.Append(type1.Name);
			Label0:
			}
			return stringBuilder.ToString();
		}

		public SubType[] GetSubtypes()
		{
			if (this.subTypes == null || this.subTypes.Count == 0)
			{
				return new SubType[0];
			}
			SubType[] subTypeArray = new SubType[this.subTypes.Count];
			this.subTypes.CopyTo(subTypeArray, 0);
			Array.Sort<SubType>(subTypeArray, SubType.Comparer.Default);
			return subTypeArray;
		}

		internal MetaType GetSurrogateOrBaseOrSelf(bool deep)
		{
			MetaType metaType;
			if (this.surrogate != null)
			{
				return this.model[this.surrogate];
			}
			MetaType metaType1 = this.baseType;
			if (metaType1 == null)
			{
				return this;
			}
			if (!deep)
			{
				return metaType1;
			}
			do
			{
				metaType = metaType1;
				metaType1 = metaType1.baseType;
			}
			while (metaType1 != null);
			return metaType;
		}

		internal MetaType GetSurrogateOrSelf()
		{
			if (this.surrogate == null)
			{
				return this;
			}
			return this.model[this.surrogate];
		}

		private static bool HasFamily(MetaType.AttributeFamily value, MetaType.AttributeFamily required)
		{
			return (value & required) == required;
		}

		private bool HasFlag(byte flag)
		{
			return (this.flags & flag) == flag;
		}

		internal static Exception InbuiltType(System.Type type)
		{
			return new ArgumentException(string.Concat("Data of this type has inbuilt behaviour, and cannot be added to a model in this way: ", type.FullName));
		}

		internal bool IsDefined(int fieldNumber)
		{
			BasicList.NodeEnumerator enumerator = this.fields.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (((ValueMember)enumerator.Current).FieldNumber != fieldNumber)
				{
					continue;
				}
				return true;
			}
			return false;
		}

		internal bool IsPrepared()
		{
			return this.serializer is CompiledSerializer;
		}

		private bool IsValidSubType(System.Type subType)
		{
			return this.type.IsAssignableFrom(subType);
		}

		internal static StringBuilder NewLine(StringBuilder builder, int indent)
		{
			return Helpers.AppendLine(builder).Append(' ', indent * 3);
		}

		private static ProtoMemberAttribute NormalizeProtoMember(TypeModel model, MemberInfo member, MetaType.AttributeFamily family, bool forced, bool isEnum, BasicList partialMembers, int dataMemberOffset, bool inferByTagName)
		{
			AttributeMap attribute;
			object obj;
			object obj1;
			bool flag;
			if (member == null || family == MetaType.AttributeFamily.None && !isEnum)
			{
				return null;
			}
			int num = -2147483648;
			int num1 = (inferByTagName ? -1 : 1);
			string str = null;
			bool flag1 = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool fieldBoolean = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			DataFormat dataFormat = DataFormat.Default;
			if (isEnum)
			{
				forced = true;
			}
			AttributeMap[] attributeMapArray = AttributeMap.Create(model, member, true);
			if (isEnum)
			{
				attribute = MetaType.GetAttribute(attributeMapArray, "ProtoBuf.ProtoIgnoreAttribute");
				if (attribute == null)
				{
					attribute = MetaType.GetAttribute(attributeMapArray, "ProtoBuf.ProtoEnumAttribute");
					num = Convert.ToInt32(((FieldInfo)member).GetRawConstantValue());
					if (attribute != null)
					{
						MetaType.GetFieldName(ref str, attribute, "Name");
						if ((bool)Helpers.GetInstanceMethod(attribute.AttributeType, "HasValue").Invoke(attribute.Target, null) && attribute.TryGet("Value", out obj))
						{
							num = (int)obj;
						}
					}
				}
				else
				{
					flag2 = true;
				}
				flag3 = true;
			}
			if (!flag2 && !flag3)
			{
				attribute = MetaType.GetAttribute(attributeMapArray, "ProtoBuf.ProtoMemberAttribute");
				MetaType.GetIgnore(ref flag2, attribute, attributeMapArray, "ProtoBuf.ProtoIgnoreAttribute");
				if (!flag2 && attribute != null)
				{
					MetaType.GetFieldNumber(ref num, attribute, "Tag");
					MetaType.GetFieldName(ref str, attribute, "Name");
					MetaType.GetFieldBoolean(ref flag4, attribute, "IsRequired");
					MetaType.GetFieldBoolean(ref flag1, attribute, "IsPacked");
					MetaType.GetFieldBoolean(ref flag8, attribute, "OverwriteList");
					MetaType.GetDataFormat(ref dataFormat, attribute, "DataFormat");
					MetaType.GetFieldBoolean(ref fieldBoolean, attribute, "AsReferenceHasValue", false);
					if (fieldBoolean)
					{
						fieldBoolean = MetaType.GetFieldBoolean(ref flag5, attribute, "AsReference", true);
					}
					MetaType.GetFieldBoolean(ref flag6, attribute, "DynamicType");
					bool flag9 = num > 0;
					flag7 = flag9;
					flag3 = flag9;
				}
				if (!flag3 && partialMembers != null)
				{
					BasicList.NodeEnumerator enumerator = partialMembers.GetEnumerator();
					do
					{
					Label0:
						if (!enumerator.MoveNext())
						{
							break;
						}
						AttributeMap current = (AttributeMap)enumerator.Current;
						if (current.TryGet("MemberName", out obj1) && (string)obj1 == member.Name)
						{
							MetaType.GetFieldNumber(ref num, current, "Tag");
							MetaType.GetFieldName(ref str, current, "Name");
							MetaType.GetFieldBoolean(ref flag4, current, "IsRequired");
							MetaType.GetFieldBoolean(ref flag1, current, "IsPacked");
							MetaType.GetFieldBoolean(ref flag8, attribute, "OverwriteList");
							MetaType.GetDataFormat(ref dataFormat, current, "DataFormat");
							MetaType.GetFieldBoolean(ref fieldBoolean, attribute, "AsReferenceHasValue", false);
							if (fieldBoolean)
							{
								fieldBoolean = MetaType.GetFieldBoolean(ref flag5, current, "AsReference", true);
							}
							MetaType.GetFieldBoolean(ref flag6, current, "DynamicType");
							flag = num > 0;
							flag7 = flag;
							flag3 = flag;
						}
						else
						{
							goto Label0;
						}
					}
					while (!flag);
				}
			}
			if (!flag2 && !flag3 && MetaType.HasFamily(family, MetaType.AttributeFamily.DataContractSerialier))
			{
				attribute = MetaType.GetAttribute(attributeMapArray, "System.Runtime.Serialization.DataMemberAttribute");
				if (attribute != null)
				{
					MetaType.GetFieldNumber(ref num, attribute, "Order");
					MetaType.GetFieldName(ref str, attribute, "Name");
					MetaType.GetFieldBoolean(ref flag4, attribute, "IsRequired");
					flag3 = num >= num1;
					if (flag3)
					{
						num += dataMemberOffset;
					}
				}
			}
			if (!flag2 && !flag3 && MetaType.HasFamily(family, MetaType.AttributeFamily.XmlSerializer))
			{
				attribute = MetaType.GetAttribute(attributeMapArray, "System.Xml.Serialization.XmlElementAttribute") ?? MetaType.GetAttribute(attributeMapArray, "System.Xml.Serialization.XmlArrayAttribute");
				MetaType.GetIgnore(ref flag2, attribute, attributeMapArray, "System.Xml.Serialization.XmlIgnoreAttribute");
				if (attribute != null && !flag2)
				{
					MetaType.GetFieldNumber(ref num, attribute, "Order");
					MetaType.GetFieldName(ref str, attribute, "ElementName");
					flag3 = num >= num1;
				}
			}
			if (!flag2 && !flag3 && MetaType.GetAttribute(attributeMapArray, "System.NonSerializedAttribute") != null)
			{
				flag2 = true;
			}
			if (flag2 || num < num1 && !forced)
			{
				return null;
			}
			ProtoMemberAttribute protoMemberAttribute = new ProtoMemberAttribute(num, (forced ? true : inferByTagName))
			{
				AsReference = flag5,
				AsReferenceHasValue = fieldBoolean,
				DataFormat = dataFormat,
				DynamicType = flag6,
				IsPacked = flag1,
				OverwriteList = flag8,
				IsRequired = flag4,
				Name = (Helpers.IsNullOrEmpty(str) ? member.Name : str),
				Member = member,
				TagIsPinned = flag7
			};
			return protoMemberAttribute;
		}

		internal static void ResolveListTypes(TypeModel model, System.Type type, ref System.Type itemType, ref System.Type defaultType)
		{
			if (type == null)
			{
				return;
			}
			if (type.IsArray)
			{
				if (type.GetArrayRank() != 1)
				{
					throw new NotSupportedException("Multi-dimension arrays are supported");
				}
				itemType = type.GetElementType();
				if (itemType != model.MapType(typeof(byte)))
				{
					defaultType = type;
				}
				else
				{
					object obj = null;
					System.Type type1 = (System.Type)obj;
					itemType = (System.Type)obj;
					defaultType = type1;
				}
			}
			if (itemType == null)
			{
				itemType = TypeModel.GetListItemType(model, type);
			}
			if (itemType != null)
			{
				System.Type type2 = null;
				System.Type type3 = null;
				MetaType.ResolveListTypes(model, itemType, ref type2, ref type3);
				if (type2 != null)
				{
					throw TypeModel.CreateNestedListsNotSupported();
				}
			}
			if (itemType != null && defaultType == null)
			{
				if (type.IsClass && !type.IsAbstract && Helpers.GetConstructor(type, Helpers.EmptyTypes, true) != null)
				{
					defaultType = type;
				}
				if (defaultType == null && type.IsInterface)
				{
					if (type.IsGenericType && type.GetGenericTypeDefinition() == model.MapType(typeof(IDictionary<,>)))
					{
						System.Type type4 = model.MapType(typeof(KeyValuePair<,>));
						System.Type[] genericArguments = type.GetGenericArguments();
						System.Type[] typeArray = genericArguments;
						if (itemType != type4.MakeGenericType(genericArguments))
						{
							goto Label1;
						}
						defaultType = model.MapType(typeof(Dictionary<,>)).MakeGenericType(typeArray);
						goto Label0;
					}
				Label1:
					System.Type type5 = model.MapType(typeof(List<>));
					System.Type[] typeArray1 = new System.Type[] { itemType };
					defaultType = type5.MakeGenericType(typeArray1);
				}
			Label0:
				if (defaultType != null && !Helpers.IsAssignableFrom(type, defaultType))
				{
					defaultType = null;
				}
			}
		}

		private MethodInfo ResolveMethod(string name, bool instance)
		{
			if (Helpers.IsNullOrEmpty(name))
			{
				return null;
			}
			if (!instance)
			{
				return Helpers.GetStaticMethod(this.type, name);
			}
			return Helpers.GetInstanceMethod(this.type, name);
		}

		internal static ConstructorInfo ResolveTupleConstructor(System.Type type, out MemberInfo[] mappedMembers)
		{
			mappedMembers = null;
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type.IsAbstract)
			{
				return null;
			}
			ConstructorInfo[] constructors = Helpers.GetConstructors(type, false);
			if ((int)constructors.Length == 0 || (int)constructors.Length == 1 && (int)constructors[0].GetParameters().Length == 0)
			{
				return null;
			}
			MemberInfo[] instanceFieldsAndProperties = Helpers.GetInstanceFieldsAndProperties(type, true);
			BasicList basicLists = new BasicList();
			for (int i = 0; i < (int)instanceFieldsAndProperties.Length; i++)
			{
				PropertyInfo propertyInfo = instanceFieldsAndProperties[i] as PropertyInfo;
				if (propertyInfo == null)
				{
					FieldInfo fieldInfo = instanceFieldsAndProperties[i] as FieldInfo;
					if (fieldInfo != null)
					{
						if (!fieldInfo.IsInitOnly)
						{
							return null;
						}
						basicLists.Add(fieldInfo);
					}
				}
				else
				{
					if (!propertyInfo.CanRead)
					{
						return null;
					}
					if (propertyInfo.CanWrite && Helpers.GetSetMethod(propertyInfo, false, false) != null)
					{
						return null;
					}
					basicLists.Add(propertyInfo);
				}
			}
			if (basicLists.Count == 0)
			{
				return null;
			}
			MemberInfo[] memberInfoArray = new MemberInfo[basicLists.Count];
			basicLists.CopyTo(memberInfoArray, 0);
			int[] numArray = new int[(int)memberInfoArray.Length];
			int num = 0;
			ConstructorInfo constructorInfo = null;
			mappedMembers = new MemberInfo[(int)numArray.Length];
			for (int j = 0; j < (int)constructors.Length; j++)
			{
				ParameterInfo[] parameters = constructors[j].GetParameters();
				if ((int)parameters.Length == (int)memberInfoArray.Length)
				{
					for (int k = 0; k < (int)numArray.Length; k++)
					{
						numArray[k] = -1;
					}
					for (int l = 0; l < (int)parameters.Length; l++)
					{
						string lower = parameters[l].Name.ToLower();
						for (int m = 0; m < (int)memberInfoArray.Length; m++)
						{
							if (!(memberInfoArray[m].Name.ToLower() != lower) && Helpers.GetMemberType(memberInfoArray[m]) == parameters[l].ParameterType)
							{
								numArray[l] = m;
							}
						}
					}
					bool flag = false;
					int num1 = 0;
					while (num1 < (int)numArray.Length)
					{
						if (numArray[num1] >= 0)
						{
							mappedMembers[num1] = memberInfoArray[numArray[num1]];
							num1++;
						}
						else
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						num++;
						constructorInfo = constructors[j];
					}
				}
			}
			if (num != 1)
			{
				return null;
			}
			return constructorInfo;
		}

		private void SetBaseType(MetaType baseType)
		{
			if (baseType == null)
			{
				throw new ArgumentNullException("baseType");
			}
			if (this.baseType == baseType)
			{
				return;
			}
			if (this.baseType != null)
			{
				throw new InvalidOperationException("A type can only participate in one inheritance hierarchy");
			}
			for (MetaType i = baseType; i != null; i = i.baseType)
			{
				if (object.ReferenceEquals(i, this))
				{
					throw new InvalidOperationException("Cyclic inheritance is not allowed");
				}
			}
			this.baseType = baseType;
		}

		public MetaType SetCallbacks(MethodInfo beforeSerialize, MethodInfo afterSerialize, MethodInfo beforeDeserialize, MethodInfo afterDeserialize)
		{
			CallbackSet callbacks = this.Callbacks;
			callbacks.BeforeSerialize = beforeSerialize;
			callbacks.AfterSerialize = afterSerialize;
			callbacks.BeforeDeserialize = beforeDeserialize;
			callbacks.AfterDeserialize = afterDeserialize;
			return this;
		}

		public MetaType SetCallbacks(string beforeSerialize, string afterSerialize, string beforeDeserialize, string afterDeserialize)
		{
			if (this.IsValueType)
			{
				throw new InvalidOperationException();
			}
			CallbackSet callbacks = this.Callbacks;
			callbacks.BeforeSerialize = this.ResolveMethod(beforeSerialize, true);
			callbacks.AfterSerialize = this.ResolveMethod(afterSerialize, true);
			callbacks.BeforeDeserialize = this.ResolveMethod(beforeDeserialize, true);
			callbacks.AfterDeserialize = this.ResolveMethod(afterDeserialize, true);
			return this;
		}

		public MetaType SetFactory(MethodInfo factory)
		{
			this.model.VerifyFactory(factory, this.type);
			this.ThrowIfFrozen();
			this.factory = factory;
			return this;
		}

		public MetaType SetFactory(string factory)
		{
			return this.SetFactory(this.ResolveMethod(factory, false));
		}

		private void SetFlag(byte flag, bool value, bool throwIfFrozen)
		{
			if (throwIfFrozen && this.HasFlag(flag) != value)
			{
				this.ThrowIfFrozen();
			}
			if (value)
			{
				MetaType metaType = this;
				metaType.flags = (byte)(metaType.flags | flag);
				return;
			}
			this.flags = (byte)(this.flags & ~flag);
		}

		public void SetSurrogate(System.Type surrogateType)
		{
			if (surrogateType == this.type)
			{
				surrogateType = null;
			}
			if (surrogateType != null && surrogateType != null && Helpers.IsAssignableFrom(this.model.MapType(typeof(IEnumerable)), surrogateType))
			{
				throw new ArgumentException("Repeated data (a list, collection, etc) has inbuilt behaviour and cannot be used as a surrogate");
			}
			this.ThrowIfFrozen();
			this.surrogate = surrogateType;
		}

		protected internal void ThrowIfFrozen()
		{
			if ((this.flags & 4) != 0)
			{
				throw new InvalidOperationException(string.Concat("The type cannot be changed once a serializer has been generated for ", this.type.FullName));
			}
		}

		public override string ToString()
		{
			return this.type.ToString();
		}

		internal void WriteSchema(StringBuilder builder, int indent, ref bool requiresBclImport)
		{
			MemberInfo[] memberInfoArray;
			System.Type fieldType;
			string str;
			if (this.surrogate != null)
			{
				return;
			}
			ValueMember[] valueMemberArray = new ValueMember[this.fields.Count];
			this.fields.CopyTo(valueMemberArray, 0);
			Array.Sort<ValueMember>(valueMemberArray, ValueMember.Comparer.Default);
			if (this.IsList)
			{
				string schemaTypeName = this.model.GetSchemaTypeName(TypeModel.GetListItemType(this.model, this.type), DataFormat.Default, false, false, ref requiresBclImport);
				MetaType.NewLine(builder, indent).Append("message ").Append(this.GetSchemaTypeName()).Append(" {");
				MetaType.NewLine(builder, indent + 1).Append("repeated ").Append(schemaTypeName).Append(" items = 1;");
				MetaType.NewLine(builder, indent).Append('}');
				return;
			}
			if (!this.IsAutoTuple)
			{
				if (Helpers.IsEnum(this.type))
				{
					MetaType.NewLine(builder, indent).Append("enum ").Append(this.GetSchemaTypeName()).Append(" {");
					if ((int)valueMemberArray.Length != 0 || !this.EnumPassthru)
					{
						ValueMember[] valueMemberArray1 = valueMemberArray;
						for (int i = 0; i < (int)valueMemberArray1.Length; i++)
						{
							ValueMember valueMember = valueMemberArray1[i];
							MetaType.NewLine(builder, indent + 1).Append(valueMember.Name).Append(" = ").Append(valueMember.FieldNumber).Append(';');
						}
					}
					else
					{
						if (!this.type.IsDefined(this.model.MapType(typeof(FlagsAttribute)), false))
						{
							MetaType.NewLine(builder, indent + 1).Append("// this enumeration will be passed as a raw value");
						}
						else
						{
							MetaType.NewLine(builder, indent + 1).Append("// this is a composite/flags enumeration");
						}
						FieldInfo[] fields = this.type.GetFields();
						for (int j = 0; j < (int)fields.Length; j++)
						{
							FieldInfo fieldInfo = fields[j];
							if (fieldInfo.IsStatic && fieldInfo.IsLiteral)
							{
								object rawConstantValue = fieldInfo.GetRawConstantValue();
								MetaType.NewLine(builder, indent + 1).Append(fieldInfo.Name).Append(" = ").Append(rawConstantValue).Append(";");
							}
						}
					}
					MetaType.NewLine(builder, indent).Append('}');
					return;
				}
				MetaType.NewLine(builder, indent).Append("message ").Append(this.GetSchemaTypeName()).Append(" {");
				ValueMember[] valueMemberArray2 = valueMemberArray;
				for (int k = 0; k < (int)valueMemberArray2.Length; k++)
				{
					ValueMember valueMember1 = valueMemberArray2[k];
					if (valueMember1.ItemType != null)
					{
						str = "repeated";
					}
					else
					{
						str = (valueMember1.IsRequired ? "required" : "optional");
					}
					string str1 = str;
					MetaType.NewLine(builder, indent + 1).Append(str1).Append(' ');
					if (valueMember1.DataFormat == DataFormat.Group)
					{
						builder.Append("group ");
					}
					string schemaTypeName1 = valueMember1.GetSchemaTypeName(true, ref requiresBclImport);
					builder.Append(schemaTypeName1).Append(" ").Append(valueMember1.Name).Append(" = ").Append(valueMember1.FieldNumber);
					if (valueMember1.DefaultValue != null)
					{
						if (valueMember1.DefaultValue is string)
						{
							builder.Append(" [default = \"").Append(valueMember1.DefaultValue).Append("\"]");
						}
						else if (!(valueMember1.DefaultValue is bool))
						{
							builder.Append(" [default = ").Append(valueMember1.DefaultValue).Append(']');
						}
						else
						{
							builder.Append(((bool)valueMember1.DefaultValue ? " [default = true]" : " [default = false]"));
						}
					}
					if (valueMember1.ItemType != null && valueMember1.IsPacked)
					{
						builder.Append(" [packed=true]");
					}
					builder.Append(';');
					if (schemaTypeName1 == "bcl.NetObjectProxy" && valueMember1.AsReference && !valueMember1.DynamicType)
					{
						builder.Append(" // reference-tracked ").Append(valueMember1.GetSchemaTypeName(false, ref requiresBclImport));
					}
				}
				if (this.subTypes != null && this.subTypes.Count != 0)
				{
					MetaType.NewLine(builder, indent + 1).Append("// the following represent sub-types; at most 1 should have a value");
					SubType[] subTypeArray = new SubType[this.subTypes.Count];
					this.subTypes.CopyTo(subTypeArray, 0);
					Array.Sort<SubType>(subTypeArray, SubType.Comparer.Default);
					SubType[] subTypeArray1 = subTypeArray;
					for (int l = 0; l < (int)subTypeArray1.Length; l++)
					{
						SubType subType = subTypeArray1[l];
						string schemaTypeName2 = subType.DerivedType.GetSchemaTypeName();
						MetaType.NewLine(builder, indent + 1).Append("optional ").Append(schemaTypeName2).Append(" ").Append(schemaTypeName2).Append(" = ").Append(subType.FieldNumber).Append(';');
					}
				}
				MetaType.NewLine(builder, indent).Append('}');
			}
			else if (MetaType.ResolveTupleConstructor(this.type, out memberInfoArray) != null)
			{
				MetaType.NewLine(builder, indent).Append("message ").Append(this.GetSchemaTypeName()).Append(" {");
				for (int m = 0; m < (int)memberInfoArray.Length; m++)
				{
					if (!(memberInfoArray[m] is PropertyInfo))
					{
						if (!(memberInfoArray[m] is FieldInfo))
						{
							throw new NotSupportedException(string.Concat("Unknown member type: ", memberInfoArray[m].GetType().Name));
						}
						fieldType = ((FieldInfo)memberInfoArray[m]).FieldType;
					}
					else
					{
						fieldType = ((PropertyInfo)memberInfoArray[m]).PropertyType;
					}
					MetaType.NewLine(builder, indent + 1).Append("optional ").Append(this.model.GetSchemaTypeName(fieldType, DataFormat.Default, false, false, ref requiresBclImport).Replace('.', '\u005F')).Append(' ').Append(memberInfoArray[m].Name).Append(" = ").Append(m + 1).Append(';');
				}
				MetaType.NewLine(builder, indent).Append('}');
				return;
			}
		}

		[Flags]
		internal enum AttributeFamily
		{
			None = 0,
			ProtoBuf = 1,
			DataContractSerialier = 2,
			XmlSerializer = 4,
			AutoTuple = 8
		}

		internal sealed class Comparer : IComparer, IComparer<MetaType>
		{
			public readonly static MetaType.Comparer Default;

			static Comparer()
			{
				MetaType.Comparer.Default = new MetaType.Comparer();
			}

			public Comparer()
			{
			}

			public int Compare(object x, object y)
			{
				return this.Compare(x as MetaType, y as MetaType);
			}

			public int Compare(MetaType x, MetaType y)
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
				return string.Compare(x.GetSchemaTypeName(), y.GetSchemaTypeName(), StringComparison.Ordinal);
			}
		}
	}
}