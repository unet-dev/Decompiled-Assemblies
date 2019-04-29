using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace ProtoBuf.Meta
{
	public abstract class TypeModel
	{
		private readonly static Type ilist;

		static TypeModel()
		{
			TypeModel.ilist = typeof(IList);
		}

		protected TypeModel()
		{
		}

		public bool CanSerialize(Type type)
		{
			return this.CanSerialize(type, true, true, true);
		}

		private bool CanSerialize(Type type, bool allowBasic, bool allowContract, bool allowLists)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Type underlyingType = Helpers.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
			}
			switch (Helpers.GetTypeCode(type))
			{
				case ProtoTypeCode.Empty:
				case ProtoTypeCode.Unknown:
				{
					if (this.GetKey(ref type) >= 0)
					{
						return allowContract;
					}
					if (allowLists)
					{
						Type listItemType = null;
						if (!type.IsArray)
						{
							listItemType = TypeModel.GetListItemType(this, type);
						}
						else if (type.GetArrayRank() == 1)
						{
							listItemType = type.GetElementType();
						}
						if (listItemType != null)
						{
							return this.CanSerialize(listItemType, allowBasic, allowContract, false);
						}
					}
					return false;
				}
				default:
				{
					return allowBasic;
				}
			}
		}

		public bool CanSerializeBasicType(Type type)
		{
			return this.CanSerialize(type, true, false, true);
		}

		public bool CanSerializeContractType(Type type)
		{
			return this.CanSerialize(type, false, true, true);
		}

		private static bool CheckDictionaryAccessors(TypeModel model, Type pair, Type value)
		{
			if (!pair.IsGenericType || pair.GetGenericTypeDefinition() != model.MapType(typeof(KeyValuePair<,>)))
			{
				return false;
			}
			return pair.GetGenericArguments()[1] == value;
		}

		public static RuntimeTypeModel Create()
		{
			return new RuntimeTypeModel(false);
		}

		public IFormatter CreateFormatter(Type type)
		{
			return new TypeModel.Formatter(this, type);
		}

		private static object CreateListInstance(Type listType, Type itemType)
		{
			Type type = listType;
			if (listType.IsArray)
			{
				return Array.CreateInstance(itemType, 0);
			}
			if (!listType.IsClass || listType.IsAbstract || Helpers.GetConstructor(listType, Helpers.EmptyTypes, true) == null)
			{
				bool flag = false;
				if (listType.IsInterface)
				{
					string fullName = listType.FullName;
					string str = fullName;
					if (fullName != null && str.IndexOf("Dictionary") >= 0)
					{
						if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
						{
							Type[] genericArguments = listType.GetGenericArguments();
							type = typeof(Dictionary<,>).MakeGenericType(genericArguments);
							flag = true;
						}
						if (!flag && listType == typeof(IDictionary))
						{
							type = typeof(Hashtable);
							flag = true;
						}
					}
				}
				if (!flag)
				{
					type = typeof(List<>).MakeGenericType(new Type[] { itemType });
					flag = true;
				}
				if (!flag)
				{
					type = typeof(ArrayList);
					flag = true;
				}
			}
			return Activator.CreateInstance(type);
		}

		internal static Exception CreateNestedListsNotSupported()
		{
			return new NotSupportedException("Nested or jagged lists and arrays are not supported");
		}

		public object DeepClone(object value)
		{
			int num;
			object obj;
			if (value == null)
			{
				return null;
			}
			Type type = value.GetType();
			int key = this.GetKey(ref type);
			if (key < 0 || Helpers.IsEnum(type))
			{
				if (type == typeof(byte[]))
				{
					byte[] numArray = (byte[])value;
					byte[] numArray1 = new byte[(int)numArray.Length];
					Helpers.BlockCopy(numArray, 0, numArray1, 0, (int)numArray.Length);
					return numArray1;
				}
				if (this.GetWireType(Helpers.GetTypeCode(type), DataFormat.Default, ref type, out num) != WireType.None && num < 0)
				{
					return value;
				}
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (ProtoWriter protoWriter = new ProtoWriter(memoryStream, this, null))
					{
						if (!this.TrySerializeAuxiliaryType(protoWriter, type, DataFormat.Default, 1, value, false))
						{
							TypeModel.ThrowUnexpectedType(type);
						}
						protoWriter.Close();
					}
					memoryStream.Position = (long)0;
					ProtoReader protoReader = null;
					try
					{
						protoReader = ProtoReader.Create(memoryStream, this, null, -1);
						value = null;
						this.TryDeserializeAuxiliaryType(protoReader, DataFormat.Default, 1, type, ref value, true, false, true, false);
						obj = value;
					}
					finally
					{
						ProtoReader.Recycle(protoReader);
					}
				}
			}
			else
			{
				using (MemoryStream memoryStream1 = new MemoryStream())
				{
					using (ProtoWriter protoWriter1 = new ProtoWriter(memoryStream1, this, null))
					{
						protoWriter1.SetRootObject(value);
						this.Serialize(key, value, protoWriter1);
						protoWriter1.Close();
					}
					memoryStream1.Position = (long)0;
					ProtoReader protoReader1 = null;
					try
					{
						protoReader1 = ProtoReader.Create(memoryStream1, this, null, -1);
						obj = this.Deserialize(key, null, protoReader1);
					}
					finally
					{
						ProtoReader.Recycle(protoReader1);
					}
				}
			}
			return obj;
		}

		public object Deserialize(Stream source, object value, Type type)
		{
			return this.Deserialize(source, value, type, null);
		}

		public object Deserialize(Stream source, object value, Type type, SerializationContext context)
		{
			object obj;
			bool flag = this.PrepareDeserialize(value, ref type);
			ProtoReader protoReader = null;
			try
			{
				protoReader = ProtoReader.Create(source, this, context, -1);
				if (value != null)
				{
					protoReader.SetRootObject(value);
				}
				object obj1 = this.DeserializeCore(protoReader, type, value, flag);
				protoReader.CheckFullyConsumed();
				obj = obj1;
			}
			finally
			{
				ProtoReader.Recycle(protoReader);
			}
			return obj;
		}

		public object Deserialize(Stream source, object value, Type type, int length)
		{
			return this.Deserialize(source, value, type, length, null);
		}

		public object Deserialize(Stream source, object value, Type type, int length, SerializationContext context)
		{
			object obj;
			bool flag = this.PrepareDeserialize(value, ref type);
			ProtoReader protoReader = null;
			try
			{
				protoReader = ProtoReader.Create(source, this, context, length);
				if (value != null)
				{
					protoReader.SetRootObject(value);
				}
				object obj1 = this.DeserializeCore(protoReader, type, value, flag);
				protoReader.CheckFullyConsumed();
				obj = obj1;
			}
			finally
			{
				ProtoReader.Recycle(protoReader);
			}
			return obj;
		}

		public object Deserialize(ProtoReader source, object value, Type type)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			bool flag = this.PrepareDeserialize(value, ref type);
			if (value != null)
			{
				source.SetRootObject(value);
			}
			object obj = this.DeserializeCore(source, type, value, flag);
			source.CheckFullyConsumed();
			return obj;
		}

		protected internal abstract object Deserialize(int key, object value, ProtoReader source);

		private object DeserializeCore(ProtoReader reader, Type type, object value, bool noAutoCreate)
		{
			int key = this.GetKey(ref type);
			if (key >= 0 && !Helpers.IsEnum(type))
			{
				return this.Deserialize(key, value, reader);
			}
			this.TryDeserializeAuxiliaryType(reader, DataFormat.Default, 1, type, ref value, true, false, noAutoCreate, false);
			return value;
		}

		public IEnumerable DeserializeItems(Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver)
		{
			return this.DeserializeItems(source, type, style, expectedField, resolver, null);
		}

		public IEnumerable DeserializeItems(Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, SerializationContext context)
		{
			return new TypeModel.DeserializeItemsIterator(this, source, type, style, expectedField, resolver, context);
		}

		public IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int expectedField)
		{
			return this.DeserializeItems<T>(source, style, expectedField, null);
		}

		public IEnumerable<T> DeserializeItems<T>(Stream source, PrefixStyle style, int expectedField, SerializationContext context)
		{
			return new TypeModel.DeserializeItemsIterator<T>(this, source, style, expectedField, context);
		}

		internal static Type DeserializeType(TypeModel model, string value)
		{
			if (model != null)
			{
				TypeFormatEventHandler typeFormatEventHandler = model.DynamicTypeFormatting;
				if (typeFormatEventHandler != null)
				{
					TypeFormatEventArgs typeFormatEventArg = new TypeFormatEventArgs(value);
					typeFormatEventHandler(model, typeFormatEventArg);
					if (typeFormatEventArg.Type != null)
					{
						return typeFormatEventArg.Type;
					}
				}
			}
			return Type.GetType(value);
		}

		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int fieldNumber)
		{
			int num;
			return this.DeserializeWithLengthPrefix(source, value, type, style, fieldNumber, null, out num);
		}

		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver)
		{
			int num;
			return this.DeserializeWithLengthPrefix(source, value, type, style, expectedField, resolver, out num);
		}

		public object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, out int bytesRead)
		{
			bool flag;
			return this.DeserializeWithLengthPrefix(source, value, type, style, expectedField, resolver, out bytesRead, out flag, null);
		}

		private object DeserializeWithLengthPrefix(Stream source, object value, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, out int bytesRead, out bool haveObject, SerializationContext context)
		{
			bool flag;
			int num;
			int num1;
			int num2;
			object obj;
			haveObject = false;
			bytesRead = 0;
			if (type == null && (style != PrefixStyle.Base128 || resolver == null))
			{
				throw new InvalidOperationException("A type must be provided unless base-128 prefixing is being used in combination with a resolver");
			}
			do
			{
				bool flag1 = (expectedField > 0 ? true : resolver != null);
				num = ProtoReader.ReadLengthPrefix(source, flag1, style, out num2, out num1);
				if (num1 == 0)
				{
					return value;
				}
				bytesRead += num1;
				if (num < 0)
				{
					return value;
				}
				if (style != PrefixStyle.Base128)
				{
					flag = false;
				}
				else if (!flag1 || expectedField != 0 || type != null || resolver == null)
				{
					flag = expectedField != num2;
				}
				else
				{
					type = resolver(num2);
					flag = type == null;
				}
				if (!flag)
				{
					continue;
				}
				if (num == 2147483647)
				{
					throw new InvalidOperationException();
				}
				ProtoReader.Seek(source, num, null);
				bytesRead += num;
			}
			while (flag);
			ProtoReader protoReader = null;
			try
			{
				protoReader = ProtoReader.Create(source, this, context, num);
				int key = this.GetKey(ref type);
				if (key >= 0 && !Helpers.IsEnum(type))
				{
					value = this.Deserialize(key, value, protoReader);
				}
				else if (!this.TryDeserializeAuxiliaryType(protoReader, DataFormat.Default, 1, type, ref value, true, false, true, false) && num != 0)
				{
					TypeModel.ThrowUnexpectedType(type);
				}
				bytesRead += protoReader.Position;
				haveObject = true;
				obj = value;
			}
			finally
			{
				ProtoReader.Recycle(protoReader);
			}
			return obj;
		}

		protected internal int GetKey(ref Type type)
		{
			if (type == null)
			{
				return -1;
			}
			int keyImpl = this.GetKeyImpl(type);
			if (keyImpl < 0)
			{
				Type type1 = TypeModel.ResolveProxies(type);
				if (type1 != null)
				{
					type = type1;
					keyImpl = this.GetKeyImpl(type);
				}
			}
			return keyImpl;
		}

		protected abstract int GetKeyImpl(Type type);

		internal static Type GetListItemType(TypeModel model, Type listType)
		{
			bool flag;
			if (listType == model.MapType(typeof(string)) || listType.IsArray || !model.MapType(typeof(IEnumerable)).IsAssignableFrom(listType))
			{
				return null;
			}
			BasicList basicLists = new BasicList();
			MethodInfo[] methods = listType.GetMethods();
			for (int i = 0; i < (int)methods.Length; i++)
			{
				MethodInfo methodInfo = methods[i];
				if (!methodInfo.IsStatic && !(methodInfo.Name != "Add"))
				{
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if ((int)parameters.Length == 1)
					{
						Type parameterType = parameters[0].ParameterType;
						Type type = parameterType;
						if (!basicLists.Contains(parameterType))
						{
							basicLists.Add(type);
						}
					}
				}
			}
			string name = listType.Name;
			if (name == null)
			{
				flag = false;
			}
			else
			{
				flag = (name.IndexOf("Queue") >= 0 ? true : name.IndexOf("Stack") >= 0);
			}
			if (!flag)
			{
				TypeModel.TestEnumerableListPatterns(model, basicLists, listType);
				Type[] interfaces = listType.GetInterfaces();
				for (int j = 0; j < (int)interfaces.Length; j++)
				{
					TypeModel.TestEnumerableListPatterns(model, basicLists, interfaces[j]);
				}
			}
			PropertyInfo[] properties = listType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			for (int k = 0; k < (int)properties.Length; k++)
			{
				PropertyInfo propertyInfo = properties[k];
				if (!(propertyInfo.Name != "Item") && !basicLists.Contains(propertyInfo.PropertyType))
				{
					ParameterInfo[] indexParameters = propertyInfo.GetIndexParameters();
					if ((int)indexParameters.Length == 1 && indexParameters[0].ParameterType == model.MapType(typeof(int)))
					{
						basicLists.Add(propertyInfo.PropertyType);
					}
				}
			}
			switch (basicLists.Count)
			{
				case 0:
				{
					return null;
				}
				case 1:
				{
					return (Type)basicLists[0];
				}
				case 2:
				{
					if (TypeModel.CheckDictionaryAccessors(model, (Type)basicLists[0], (Type)basicLists[1]))
					{
						return (Type)basicLists[0];
					}
					if (!TypeModel.CheckDictionaryAccessors(model, (Type)basicLists[1], (Type)basicLists[0]))
					{
						break;
					}
					return (Type)basicLists[1];
				}
			}
			return null;
		}

		public virtual string GetSchema(Type type)
		{
			throw new NotSupportedException();
		}

		internal virtual Type GetType(string fullName, Assembly context)
		{
			return TypeModel.ResolveKnownType(fullName, this, context);
		}

		private WireType GetWireType(ProtoTypeCode code, DataFormat format, ref Type type, out int modelKey)
		{
			int num;
			int key;
			modelKey = -1;
			if (Helpers.IsEnum(type))
			{
				modelKey = this.GetKey(ref type);
				return WireType.Variant;
			}
			ProtoTypeCode protoTypeCode = code;
			switch (protoTypeCode)
			{
				case ProtoTypeCode.Boolean:
				case ProtoTypeCode.Char:
				case ProtoTypeCode.SByte:
				case ProtoTypeCode.Byte:
				case ProtoTypeCode.Int16:
				case ProtoTypeCode.UInt16:
				case ProtoTypeCode.Int32:
				case ProtoTypeCode.UInt32:
				{
					if (format != DataFormat.FixedSize)
					{
						return WireType.Variant;
					}
					return WireType.Fixed32;
				}
				case ProtoTypeCode.Int64:
				case ProtoTypeCode.UInt64:
				{
					if (format != DataFormat.FixedSize)
					{
						return WireType.Variant;
					}
					return WireType.Fixed64;
				}
				case ProtoTypeCode.Single:
				{
					return WireType.Fixed32;
				}
				case ProtoTypeCode.Double:
				{
					return WireType.Fixed64;
				}
				case ProtoTypeCode.Decimal:
				case ProtoTypeCode.DateTime:
				case ProtoTypeCode.String:
				{
					return WireType.String;
				}
				case ProtoTypeCode.Unknown | ProtoTypeCode.DateTime:
				{
					key = this.GetKey(ref type);
					num = key;
					modelKey = key;
					if (num >= 0)
					{
						return WireType.String;
					}
					return WireType.None;
				}
				default:
				{
					switch (protoTypeCode)
					{
						case ProtoTypeCode.TimeSpan:
						case ProtoTypeCode.ByteArray:
						case ProtoTypeCode.Guid:
						case ProtoTypeCode.Uri:
						{
							return WireType.String;
						}
						default:
						{
							key = this.GetKey(ref type);
							num = key;
							modelKey = key;
							if (num >= 0)
							{
								return WireType.String;
							}
							return WireType.None;
						}
					}
					break;
				}
			}
		}

		public bool IsDefined(Type type)
		{
			return this.GetKey(ref type) >= 0;
		}

		protected internal Type MapType(Type type)
		{
			return this.MapType(type, true);
		}

		protected internal virtual Type MapType(Type type, bool demand)
		{
			return type;
		}

		private bool PrepareDeserialize(object value, ref Type type)
		{
			if (type == null)
			{
				if (value == null)
				{
					throw new ArgumentNullException("type");
				}
				type = this.MapType(value.GetType());
			}
			bool flag = true;
			Type underlyingType = Helpers.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				type = underlyingType;
				flag = false;
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static Type ResolveKnownType(string name, TypeModel model, Assembly assembly)
		{
			Type type;
			Type type1;
			if (Helpers.IsNullOrEmpty(name))
			{
				return null;
			}
			try
			{
				Type type2 = Type.GetType(name);
				if (type2 != null)
				{
					type = type2;
					return type;
				}
			}
			catch
			{
			}
			try
			{
				int num = name.IndexOf(',');
				string str = ((num > 0 ? name.Substring(0, num) : name)).Trim();
				if (assembly == null)
				{
					assembly = Assembly.GetCallingAssembly();
				}
				if (assembly == null)
				{
					type1 = null;
				}
				else
				{
					type1 = assembly.GetType(str);
				}
				Type type3 = type1;
				if (type3 != null)
				{
					type = type3;
					return type;
				}
			}
			catch
			{
			}
			return null;
		}

		internal static MethodInfo ResolveListAdd(TypeModel model, Type listType, Type itemType, out bool isList)
		{
			Type type = listType;
			isList = model.MapType(TypeModel.ilist).IsAssignableFrom(type);
			Type[] typeArray = new Type[] { itemType };
			MethodInfo instanceMethod = Helpers.GetInstanceMethod(type, "Add", typeArray);
			if (instanceMethod == null)
			{
				bool flag = (!type.IsInterface ? false : type == model.MapType(typeof(IEnumerable<>)).MakeGenericType(typeArray));
				Type type1 = model.MapType(typeof(ICollection<>)).MakeGenericType(typeArray);
				if (flag || type1.IsAssignableFrom(type))
				{
					instanceMethod = Helpers.GetInstanceMethod(type1, "Add", typeArray);
				}
			}
			if (instanceMethod == null)
			{
				Type[] interfaces = type.GetInterfaces();
				for (int i = 0; i < (int)interfaces.Length; i++)
				{
					Type type2 = interfaces[i];
					if (type2.Name == "IProducerConsumerCollection`1" && type2.IsGenericType && type2.GetGenericTypeDefinition().FullName == "System.Collections.Concurrent.IProducerConsumerCollection`1")
					{
						instanceMethod = Helpers.GetInstanceMethod(type2, "TryAdd", typeArray);
						if (instanceMethod != null)
						{
							break;
						}
					}
				}
			}
			if (instanceMethod == null)
			{
				typeArray[0] = model.MapType(typeof(object));
				instanceMethod = Helpers.GetInstanceMethod(type, "Add", typeArray);
			}
			if (instanceMethod == null && isList)
			{
				instanceMethod = Helpers.GetInstanceMethod(model.MapType(TypeModel.ilist), "Add", typeArray);
			}
			return instanceMethod;
		}

		protected internal static Type ResolveProxies(Type type)
		{
			if (type == null)
			{
				return null;
			}
			if (type.IsGenericParameter)
			{
				return null;
			}
			Type underlyingType = Helpers.GetUnderlyingType(type);
			if (underlyingType != null)
			{
				return underlyingType;
			}
			string fullName = type.FullName;
			if (fullName != null && fullName.StartsWith("System.Data.Entity.DynamicProxies."))
			{
				return type.BaseType;
			}
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < (int)interfaces.Length; i++)
			{
				string str = interfaces[i].FullName;
				string str1 = str;
				if (str != null && (str1 == "NHibernate.Proxy.INHibernateProxy" || str1 == "NHibernate.Proxy.DynamicProxy.IProxy" || str1 == "NHibernate.Intercept.IFieldInterceptorAccessor"))
				{
					return type.BaseType;
				}
			}
			return null;
		}

		public void Serialize(Stream dest, object value)
		{
			this.Serialize(dest, value, null);
		}

		public void Serialize(Stream dest, object value, SerializationContext context)
		{
			using (ProtoWriter protoWriter = new ProtoWriter(dest, this, context))
			{
				protoWriter.SetRootObject(value);
				this.SerializeCore(protoWriter, value);
				protoWriter.Close();
			}
		}

		public void Serialize(ProtoWriter dest, object value)
		{
			if (dest == null)
			{
				throw new ArgumentNullException("dest");
			}
			dest.CheckDepthFlushlock();
			dest.SetRootObject(value);
			this.SerializeCore(dest, value);
			dest.CheckDepthFlushlock();
			ProtoWriter.Flush(dest);
		}

		protected internal abstract void Serialize(int key, object value, ProtoWriter dest);

		private void SerializeCore(ProtoWriter writer, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Type type = value.GetType();
			int key = this.GetKey(ref type);
			if (key >= 0)
			{
				this.Serialize(key, value, writer);
				return;
			}
			if (!this.TrySerializeAuxiliaryType(writer, type, DataFormat.Default, 1, value, false))
			{
				TypeModel.ThrowUnexpectedType(type);
			}
		}

		internal static string SerializeType(TypeModel model, Type type)
		{
			if (model != null)
			{
				TypeFormatEventHandler typeFormatEventHandler = model.DynamicTypeFormatting;
				if (typeFormatEventHandler != null)
				{
					TypeFormatEventArgs typeFormatEventArg = new TypeFormatEventArgs(type);
					typeFormatEventHandler(model, typeFormatEventArg);
					if (!Helpers.IsNullOrEmpty(typeFormatEventArg.FormattedName))
					{
						return typeFormatEventArg.FormattedName;
					}
				}
			}
			return type.AssemblyQualifiedName;
		}

		public void SerializeWithLengthPrefix(Stream dest, object value, Type type, PrefixStyle style, int fieldNumber)
		{
			this.SerializeWithLengthPrefix(dest, value, type, style, fieldNumber, null);
		}

		public void SerializeWithLengthPrefix(Stream dest, object value, Type type, PrefixStyle style, int fieldNumber, SerializationContext context)
		{
			if (type == null)
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				type = this.MapType(value.GetType());
			}
			int key = this.GetKey(ref type);
			using (ProtoWriter protoWriter = new ProtoWriter(dest, this, context))
			{
				switch (style)
				{
					case PrefixStyle.None:
					{
						this.Serialize(key, value, protoWriter);
						break;
					}
					case PrefixStyle.Base128:
					case PrefixStyle.Fixed32:
					case PrefixStyle.Fixed32BigEndian:
					{
						ProtoWriter.WriteObject(value, key, protoWriter, style, fieldNumber);
						break;
					}
					default:
					{
						throw new ArgumentOutOfRangeException("style");
					}
				}
				protoWriter.Close();
			}
		}

		private static void TestEnumerableListPatterns(TypeModel model, BasicList candidates, Type iType)
		{
			if (iType.IsGenericType)
			{
				Type genericTypeDefinition = iType.GetGenericTypeDefinition();
				if (genericTypeDefinition == model.MapType(typeof(IEnumerable<>)) || genericTypeDefinition == model.MapType(typeof(ICollection<>)) || genericTypeDefinition.FullName == "System.Collections.Concurrent.IProducerConsumerCollection`1")
				{
					Type[] genericArguments = iType.GetGenericArguments();
					if (!candidates.Contains(genericArguments[0]))
					{
						candidates.Add(genericArguments[0]);
					}
				}
			}
		}

		public static void ThrowCannotCreateInstance(Type type)
		{
			throw new ProtoException(string.Concat("No parameterless constructor found for ", (type == null ? "(null)" : type.Name)));
		}

		protected internal static void ThrowUnexpectedSubtype(Type expected, Type actual)
		{
			if (expected != TypeModel.ResolveProxies(actual))
			{
				throw new InvalidOperationException(string.Concat("Unexpected sub-type: ", actual.FullName));
			}
		}

		protected internal static void ThrowUnexpectedType(Type type)
		{
			string str = (type == null ? "(unknown)" : type.FullName);
			if (type != null)
			{
				Type baseType = type.BaseType;
				if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition().Name == "GeneratedMessage`2")
				{
					throw new InvalidOperationException(string.Concat("Are you mixing protobuf-net and protobuf-csharp-port? See http://stackoverflow.com/q/11564914; type: ", str));
				}
			}
			throw new InvalidOperationException(string.Concat("Type is not expected, and no contract can be inferred: ", str));
		}

		internal bool TryDeserializeAuxiliaryType(ProtoReader reader, DataFormat format, int tag, Type type, ref object value, bool skipOtherFields, bool asListItem, bool autoCreate, bool insideList)
		{
			int num;
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			Type listItemType = null;
			ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
			WireType wireType = this.GetWireType(typeCode, format, ref type, out num);
			bool flag = false;
			if (wireType == WireType.None)
			{
				listItemType = TypeModel.GetListItemType(this, type);
				if (listItemType == null && type.IsArray && type.GetArrayRank() == 1 && type != typeof(byte[]))
				{
					listItemType = type.GetElementType();
				}
				if (listItemType != null)
				{
					if (insideList)
					{
						throw TypeModel.CreateNestedListsNotSupported();
					}
					flag = this.TryDeserializeList(this, reader, format, tag, type, listItemType, ref value);
					if (!flag && autoCreate)
					{
						value = TypeModel.CreateListInstance(type, listItemType);
					}
					return flag;
				}
				TypeModel.ThrowUnexpectedType(type);
			}
			while (!flag || !asListItem)
			{
				int num1 = reader.ReadFieldHeader();
				if (num1 <= 0)
				{
					break;
				}
				if (num1 == tag)
				{
					flag = true;
					reader.Hint(wireType);
					if (num < 0)
					{
						ProtoTypeCode protoTypeCode = typeCode;
						switch (protoTypeCode)
						{
							case ProtoTypeCode.Boolean:
							{
								value = reader.ReadBoolean();
								continue;
							}
							case ProtoTypeCode.Char:
							{
								value = (char)reader.ReadUInt16();
								continue;
							}
							case ProtoTypeCode.SByte:
							{
								value = reader.ReadSByte();
								continue;
							}
							case ProtoTypeCode.Byte:
							{
								value = reader.ReadByte();
								continue;
							}
							case ProtoTypeCode.Int16:
							{
								value = reader.ReadInt16();
								continue;
							}
							case ProtoTypeCode.UInt16:
							{
								value = reader.ReadUInt16();
								continue;
							}
							case ProtoTypeCode.Int32:
							{
								value = reader.ReadInt32();
								continue;
							}
							case ProtoTypeCode.UInt32:
							{
								value = reader.ReadUInt32();
								continue;
							}
							case ProtoTypeCode.Int64:
							{
								value = reader.ReadInt64();
								continue;
							}
							case ProtoTypeCode.UInt64:
							{
								value = reader.ReadUInt64();
								continue;
							}
							case ProtoTypeCode.Single:
							{
								value = reader.ReadSingle();
								continue;
							}
							case ProtoTypeCode.Double:
							{
								value = reader.ReadDouble();
								continue;
							}
							case ProtoTypeCode.Decimal:
							{
								value = BclHelpers.ReadDecimal(reader);
								continue;
							}
							case ProtoTypeCode.DateTime:
							{
								value = BclHelpers.ReadDateTime(reader);
								continue;
							}
							case ProtoTypeCode.Unknown | ProtoTypeCode.DateTime:
							{
								continue;
							}
							case ProtoTypeCode.String:
							{
								value = reader.ReadString();
								continue;
							}
							default:
							{
								switch (protoTypeCode)
								{
									case ProtoTypeCode.TimeSpan:
									{
										value = BclHelpers.ReadTimeSpan(reader);
										continue;
									}
									case ProtoTypeCode.ByteArray:
									{
										value = ProtoReader.AppendBytes((byte[])value, reader);
										continue;
									}
									case ProtoTypeCode.Guid:
									{
										value = BclHelpers.ReadGuid(reader);
										continue;
									}
									case ProtoTypeCode.Uri:
									{
										value = new Uri(reader.ReadString());
										continue;
									}
									default:
									{
										continue;
									}
								}
								break;
							}
						}
					}
					else
					{
						switch (wireType)
						{
							case WireType.String:
							case WireType.StartGroup:
							{
								SubItemToken subItemToken = ProtoReader.StartSubItem(reader);
								value = this.Deserialize(num, value, reader);
								ProtoReader.EndSubItem(subItemToken, reader);
								continue;
							}
						}
						value = this.Deserialize(num, value, reader);
					}
				}
				else
				{
					if (!skipOtherFields)
					{
						throw ProtoReader.AddErrorData(new InvalidOperationException(string.Concat("Expected field ", tag.ToString(), ", but found ", num1.ToString())), reader);
					}
					reader.SkipField();
				}
			}
			if (!flag && !asListItem && autoCreate && type != typeof(string))
			{
				value = Activator.CreateInstance(type);
			}
			return flag;
		}

		private bool TryDeserializeList(TypeModel model, ProtoReader reader, DataFormat format, int tag, Type listType, Type itemType, ref object value)
		{
			bool flag;
			Array arrays;
			object[] objArray;
			BasicList basicLists;
			MethodInfo methodInfo = TypeModel.ResolveListAdd(model, listType, itemType, out flag);
			if (methodInfo == null)
			{
				throw new NotSupportedException(string.Concat("Unknown list variant: ", listType.FullName));
			}
			bool flag1 = false;
			object obj = null;
			IList lists = value as IList;
			if (flag)
			{
				objArray = null;
			}
			else
			{
				objArray = new object[1];
			}
			object[] objArray1 = objArray;
			if (listType.IsArray)
			{
				basicLists = new BasicList();
			}
			else
			{
				basicLists = null;
			}
			BasicList basicLists1 = basicLists;
			while (this.TryDeserializeAuxiliaryType(reader, format, tag, itemType, ref obj, true, true, true, true))
			{
				flag1 = true;
				if (value == null && basicLists1 == null)
				{
					value = TypeModel.CreateListInstance(listType, itemType);
					lists = value as IList;
				}
				if (lists != null)
				{
					lists.Add(obj);
				}
				else if (basicLists1 == null)
				{
					objArray1[0] = obj;
					methodInfo.Invoke(value, objArray1);
				}
				else
				{
					basicLists1.Add(obj);
				}
				obj = null;
			}
			if (basicLists1 != null)
			{
				if (value == null)
				{
					arrays = Array.CreateInstance(itemType, basicLists1.Count);
					basicLists1.CopyTo(arrays, 0);
					value = arrays;
				}
				else if (basicLists1.Count != 0)
				{
					Array arrays1 = (Array)value;
					arrays = Array.CreateInstance(itemType, arrays1.Length + basicLists1.Count);
					Array.Copy(arrays1, arrays, arrays1.Length);
					basicLists1.CopyTo(arrays, arrays1.Length);
					value = arrays;
				}
			}
			return flag1;
		}

		internal bool TrySerializeAuxiliaryType(ProtoWriter writer, Type type, DataFormat format, int tag, object value, bool isInsideList)
		{
			int num;
			if (type == null)
			{
				type = value.GetType();
			}
			ProtoTypeCode typeCode = Helpers.GetTypeCode(type);
			WireType wireType = this.GetWireType(typeCode, format, ref type, out num);
			if (num >= 0)
			{
				if (Helpers.IsEnum(type))
				{
					this.Serialize(num, value, writer);
					return true;
				}
				ProtoWriter.WriteFieldHeader(tag, wireType, writer);
				switch (wireType)
				{
					case WireType.None:
					{
						throw ProtoWriter.CreateException(writer);
					}
					case WireType.Variant:
					case WireType.Fixed64:
					{
						this.Serialize(num, value, writer);
						return true;
					}
					case WireType.String:
					case WireType.StartGroup:
					{
						SubItemToken subItemToken = ProtoWriter.StartSubItem(value, writer);
						this.Serialize(num, value, writer);
						ProtoWriter.EndSubItem(subItemToken, writer);
						return true;
					}
					default:
					{
						this.Serialize(num, value, writer);
						return true;
					}
				}
			}
			if (wireType != WireType.None)
			{
				ProtoWriter.WriteFieldHeader(tag, wireType, writer);
			}
			ProtoTypeCode protoTypeCode = typeCode;
			switch (protoTypeCode)
			{
				case ProtoTypeCode.Boolean:
				{
					ProtoWriter.WriteBoolean((bool)value, writer);
					return true;
				}
				case ProtoTypeCode.Char:
				{
					ProtoWriter.WriteUInt16((char)value, writer);
					return true;
				}
				case ProtoTypeCode.SByte:
				{
					ProtoWriter.WriteSByte((sbyte)value, writer);
					return true;
				}
				case ProtoTypeCode.Byte:
				{
					ProtoWriter.WriteByte((byte)value, writer);
					return true;
				}
				case ProtoTypeCode.Int16:
				{
					ProtoWriter.WriteInt16((short)value, writer);
					return true;
				}
				case ProtoTypeCode.UInt16:
				{
					ProtoWriter.WriteUInt16((ushort)value, writer);
					return true;
				}
				case ProtoTypeCode.Int32:
				{
					ProtoWriter.WriteInt32((int)value, writer);
					return true;
				}
				case ProtoTypeCode.UInt32:
				{
					ProtoWriter.WriteUInt32((uint)value, writer);
					return true;
				}
				case ProtoTypeCode.Int64:
				{
					ProtoWriter.WriteInt64((long)value, writer);
					return true;
				}
				case ProtoTypeCode.UInt64:
				{
					ProtoWriter.WriteUInt64((ulong)value, writer);
					return true;
				}
				case ProtoTypeCode.Single:
				{
					ProtoWriter.WriteSingle((float)value, writer);
					return true;
				}
				case ProtoTypeCode.Double:
				{
					ProtoWriter.WriteDouble((double)value, writer);
					return true;
				}
				case ProtoTypeCode.Decimal:
				{
					BclHelpers.WriteDecimal((decimal)value, writer);
					return true;
				}
				case ProtoTypeCode.DateTime:
				{
					BclHelpers.WriteDateTime((DateTime)value, writer);
					return true;
				}
				case ProtoTypeCode.Unknown | ProtoTypeCode.DateTime:
				{
				Label1:
					IEnumerable enumerable = value as IEnumerable;
					if (enumerable == null)
					{
						return false;
					}
					if (isInsideList)
					{
						throw TypeModel.CreateNestedListsNotSupported();
					}
					foreach (object obj in enumerable)
					{
						if (obj == null)
						{
							throw new NullReferenceException();
						}
						if (this.TrySerializeAuxiliaryType(writer, null, format, tag, obj, true))
						{
							continue;
						}
						TypeModel.ThrowUnexpectedType(obj.GetType());
					}
					return true;
				}
				case ProtoTypeCode.String:
				{
					ProtoWriter.WriteString((string)value, writer);
					return true;
				}
				default:
				{
					switch (protoTypeCode)
					{
						case ProtoTypeCode.TimeSpan:
						{
							BclHelpers.WriteTimeSpan((TimeSpan)value, writer);
							return true;
						}
						case ProtoTypeCode.ByteArray:
						{
							ProtoWriter.WriteBytes((byte[])value, writer);
							return true;
						}
						case ProtoTypeCode.Guid:
						{
							BclHelpers.WriteGuid((Guid)value, writer);
							return true;
						}
						case ProtoTypeCode.Uri:
						{
							ProtoWriter.WriteString(((Uri)value).AbsoluteUri, writer);
							return true;
						}
						default:
						{
							goto Label1;
						}
					}
					break;
				}
			}
		}

		public event TypeFormatEventHandler DynamicTypeFormatting;

		protected internal enum CallbackType
		{
			BeforeSerialize,
			AfterSerialize,
			BeforeDeserialize,
			AfterDeserialize
		}

		private class DeserializeItemsIterator : IEnumerator, IEnumerable
		{
			private bool haveObject;

			private object current;

			private readonly Stream source;

			private readonly Type type;

			private readonly PrefixStyle style;

			private readonly int expectedField;

			private readonly Serializer.TypeResolver resolver;

			private readonly TypeModel model;

			private readonly SerializationContext context;

			public object Current
			{
				get
				{
					return this.current;
				}
			}

			public DeserializeItemsIterator(TypeModel model, Stream source, Type type, PrefixStyle style, int expectedField, Serializer.TypeResolver resolver, SerializationContext context)
			{
				this.haveObject = true;
				this.source = source;
				this.type = type;
				this.style = style;
				this.expectedField = expectedField;
				this.resolver = resolver;
				this.model = model;
				this.context = context;
			}

			public bool MoveNext()
			{
				int num;
				if (this.haveObject)
				{
					this.current = this.model.DeserializeWithLengthPrefix(this.source, null, this.type, this.style, this.expectedField, this.resolver, out num, out this.haveObject, this.context);
				}
				return this.haveObject;
			}

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this;
			}

			void System.Collections.IEnumerator.Reset()
			{
				throw new NotSupportedException();
			}
		}

		private sealed class DeserializeItemsIterator<T> : TypeModel.DeserializeItemsIterator, IEnumerator<T>, IDisposable, IEnumerator, IEnumerable<T>, IEnumerable
		{
			public new T Current
			{
				get
				{
					return (T)base.Current;
				}
			}

			public DeserializeItemsIterator(TypeModel model, Stream source, PrefixStyle style, int expectedField, SerializationContext context) : base(model, source, model.MapType(typeof(T)), style, expectedField, null, context)
			{
			}

			IEnumerator<T> System.Collections.Generic.IEnumerable<T>.GetEnumerator()
			{
				return this;
			}

			void System.IDisposable.Dispose()
			{
			}
		}

		internal sealed class Formatter : IFormatter
		{
			private readonly TypeModel model;

			private readonly Type type;

			private SerializationBinder binder;

			private StreamingContext context;

			private ISurrogateSelector surrogateSelector;

			public SerializationBinder Binder
			{
				get
				{
					return this.binder;
				}
				set
				{
					this.binder = value;
				}
			}

			public StreamingContext Context
			{
				get
				{
					return this.context;
				}
				set
				{
					this.context = value;
				}
			}

			public ISurrogateSelector SurrogateSelector
			{
				get
				{
					return this.surrogateSelector;
				}
				set
				{
					this.surrogateSelector = value;
				}
			}

			internal Formatter(TypeModel model, Type type)
			{
				if (model == null)
				{
					throw new ArgumentNullException("model");
				}
				if (type == null)
				{
					throw new ArgumentNullException("type");
				}
				this.model = model;
				this.type = type;
			}

			public object Deserialize(Stream source)
			{
				return this.model.Deserialize(source, null, this.type, -1, this.Context);
			}

			public void Serialize(Stream destination, object graph)
			{
				this.model.Serialize(destination, graph, this.Context);
			}
		}
	}
}