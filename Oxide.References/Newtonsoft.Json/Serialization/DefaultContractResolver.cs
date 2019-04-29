using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class DefaultContractResolver : IContractResolver
	{
		private readonly static IContractResolver _instance;

		private readonly static JsonConverter[] BuiltInConverters;

		private readonly static object TypeContractCacheLock;

		private readonly static DefaultContractResolverState _sharedState;

		private readonly DefaultContractResolverState _instanceState = new DefaultContractResolverState();

		private readonly bool _sharedCache;

		[Obsolete("DefaultMembersSearchFlags is obsolete. To modify the members serialized inherit from DefaultContractResolver and override the GetSerializableMembers method instead.")]
		public BindingFlags DefaultMembersSearchFlags
		{
			get;
			set;
		}

		public bool DynamicCodeGeneration
		{
			get
			{
				return JsonTypeReflector.DynamicCodeGeneration;
			}
		}

		public bool IgnoreSerializableAttribute
		{
			get;
			set;
		}

		public bool IgnoreSerializableInterface
		{
			get;
			set;
		}

		internal static IContractResolver Instance
		{
			get
			{
				return DefaultContractResolver._instance;
			}
		}

		public bool SerializeCompilerGeneratedMembers
		{
			get;
			set;
		}

		static DefaultContractResolver()
		{
			DefaultContractResolver._instance = new DefaultContractResolver(true);
			DefaultContractResolver.BuiltInConverters = new JsonConverter[] { new XmlNodeConverter(), new KeyValuePairConverter(), new BsonObjectIdConverter(), new RegexConverter() };
			DefaultContractResolver.TypeContractCacheLock = new object();
			DefaultContractResolver._sharedState = new DefaultContractResolverState();
		}

		public DefaultContractResolver()
		{
			this.IgnoreSerializableAttribute = true;
			this.DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.Public;
		}

		[Obsolete("DefaultContractResolver(bool) is obsolete. Use the parameterless constructor and cache instances of the contract resolver within your application for optimal performance.")]
		public DefaultContractResolver(bool shareCache) : this()
		{
			this._sharedCache = shareCache;
		}

		internal static bool CanConvertToString(Type type)
		{
			TypeConverter converter = ConvertUtils.GetConverter(type);
			if (converter != null && !(converter is ComponentConverter) && !(converter is ReferenceConverter) && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)))
			{
				return true;
			}
			if (type != typeof(Type) && !type.IsSubclassOf(typeof(Type)))
			{
				return false;
			}
			return true;
		}

		protected virtual JsonArrayContract CreateArrayContract(Type objectType)
		{
			JsonArrayContract jsonArrayContract = new JsonArrayContract(objectType);
			this.InitializeContract(jsonArrayContract);
			ConstructorInfo attributeConstructor = this.GetAttributeConstructor(jsonArrayContract.NonNullableUnderlyingType);
			if (attributeConstructor != null)
			{
				ParameterInfo[] parameters = attributeConstructor.GetParameters();
				Type type = (jsonArrayContract.CollectionItemType != null ? typeof(IEnumerable<>).MakeGenericType(new Type[] { jsonArrayContract.CollectionItemType }) : typeof(IEnumerable));
				if (parameters.Length != 0)
				{
					if ((int)parameters.Length != 1 || !type.IsAssignableFrom(parameters[0].ParameterType))
					{
						throw new JsonException("Constructor for '{0}' must have no parameters or a single parameter that implements '{1}'.".FormatWith(CultureInfo.InvariantCulture, jsonArrayContract.UnderlyingType, type));
					}
					jsonArrayContract.HasParameterizedCreator = true;
				}
				else
				{
					jsonArrayContract.HasParameterizedCreator = false;
				}
				jsonArrayContract.OverrideCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(attributeConstructor);
			}
			return jsonArrayContract;
		}

		protected virtual IList<JsonProperty> CreateConstructorParameters(ConstructorInfo constructor, JsonPropertyCollection memberProperties)
		{
			JsonProperty closestMatchProperty;
			ParameterInfo[] parameters = constructor.GetParameters();
			JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(constructor.DeclaringType);
			ParameterInfo[] parameterInfoArray = parameters;
			for (int i = 0; i < (int)parameterInfoArray.Length; i++)
			{
				ParameterInfo parameterInfo = parameterInfoArray[i];
				if (parameterInfo.Name != null)
				{
					closestMatchProperty = memberProperties.GetClosestMatchProperty(parameterInfo.Name);
				}
				else
				{
					closestMatchProperty = null;
				}
				JsonProperty jsonProperty = closestMatchProperty;
				if (jsonProperty != null && jsonProperty.PropertyType != parameterInfo.ParameterType)
				{
					jsonProperty = null;
				}
				if (jsonProperty != null || parameterInfo.Name != null)
				{
					JsonProperty jsonProperty1 = this.CreatePropertyFromConstructorParameter(jsonProperty, parameterInfo);
					if (jsonProperty1 != null)
					{
						jsonPropertyCollection.AddProperty(jsonProperty1);
					}
				}
			}
			return jsonPropertyCollection;
		}

		protected virtual JsonContract CreateContract(Type objectType)
		{
			if (DefaultContractResolver.IsJsonPrimitiveType(objectType))
			{
				return this.CreatePrimitiveContract(objectType);
			}
			Type type = ReflectionUtils.EnsureNotNullableType(objectType);
			JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(type);
			if (cachedAttribute is JsonObjectAttribute)
			{
				return this.CreateObjectContract(objectType);
			}
			if (cachedAttribute is JsonArrayAttribute)
			{
				return this.CreateArrayContract(objectType);
			}
			if (cachedAttribute is JsonDictionaryAttribute)
			{
				return this.CreateDictionaryContract(objectType);
			}
			if (type == typeof(JToken) || type.IsSubclassOf(typeof(JToken)))
			{
				return this.CreateLinqContract(objectType);
			}
			if (CollectionUtils.IsDictionaryType(type))
			{
				return this.CreateDictionaryContract(objectType);
			}
			if (typeof(IEnumerable).IsAssignableFrom(type))
			{
				return this.CreateArrayContract(objectType);
			}
			if (DefaultContractResolver.CanConvertToString(type))
			{
				return this.CreateStringContract(objectType);
			}
			if (!this.IgnoreSerializableInterface && typeof(ISerializable).IsAssignableFrom(type))
			{
				return this.CreateISerializableContract(objectType);
			}
			if (DefaultContractResolver.IsIConvertible(type))
			{
				return this.CreatePrimitiveContract(type);
			}
			return this.CreateObjectContract(objectType);
		}

		protected virtual JsonDictionaryContract CreateDictionaryContract(Type objectType)
		{
			JsonDictionaryContract jsonDictionaryContract = new JsonDictionaryContract(objectType);
			this.InitializeContract(jsonDictionaryContract);
			DefaultContractResolver defaultContractResolver = this;
			jsonDictionaryContract.DictionaryKeyResolver = new Func<string, string>(defaultContractResolver.ResolveDictionaryKey);
			ConstructorInfo attributeConstructor = this.GetAttributeConstructor(jsonDictionaryContract.NonNullableUnderlyingType);
			if (attributeConstructor != null)
			{
				ParameterInfo[] parameters = attributeConstructor.GetParameters();
				Type type = (jsonDictionaryContract.DictionaryKeyType == null || jsonDictionaryContract.DictionaryValueType == null ? typeof(IDictionary) : typeof(IEnumerable<>).MakeGenericType(new Type[] { typeof(KeyValuePair<,>).MakeGenericType(new Type[] { jsonDictionaryContract.DictionaryKeyType, jsonDictionaryContract.DictionaryValueType }) }));
				if (parameters.Length != 0)
				{
					if ((int)parameters.Length != 1 || !type.IsAssignableFrom(parameters[0].ParameterType))
					{
						throw new JsonException("Constructor for '{0}' must have no parameters or a single parameter that implements '{1}'.".FormatWith(CultureInfo.InvariantCulture, jsonDictionaryContract.UnderlyingType, type));
					}
					jsonDictionaryContract.HasParameterizedCreator = true;
				}
				else
				{
					jsonDictionaryContract.HasParameterizedCreator = false;
				}
				jsonDictionaryContract.OverrideCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(attributeConstructor);
			}
			return jsonDictionaryContract;
		}

		protected virtual JsonISerializableContract CreateISerializableContract(Type objectType)
		{
			JsonISerializableContract jsonISerializableContract = new JsonISerializableContract(objectType);
			this.InitializeContract(jsonISerializableContract);
			ConstructorInfo constructor = jsonISerializableContract.NonNullableUnderlyingType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
			if (constructor != null)
			{
				jsonISerializableContract.ISerializableCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
			}
			return jsonISerializableContract;
		}

		protected virtual JsonLinqContract CreateLinqContract(Type objectType)
		{
			JsonLinqContract jsonLinqContract = new JsonLinqContract(objectType);
			this.InitializeContract(jsonLinqContract);
			return jsonLinqContract;
		}

		protected virtual IValueProvider CreateMemberValueProvider(MemberInfo member)
		{
			return new ReflectionValueProvider(member);
		}

		protected virtual JsonObjectContract CreateObjectContract(Type objectType)
		{
			JsonObjectContract jsonObjectContract = new JsonObjectContract(objectType);
			this.InitializeContract(jsonObjectContract);
			bool ignoreSerializableAttribute = this.IgnoreSerializableAttribute;
			jsonObjectContract.MemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(jsonObjectContract.NonNullableUnderlyingType, ignoreSerializableAttribute);
			jsonObjectContract.Properties.AddRange<JsonProperty>(this.CreateProperties(jsonObjectContract.NonNullableUnderlyingType, jsonObjectContract.MemberSerialization));
			JsonObjectAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonObjectAttribute>(jsonObjectContract.NonNullableUnderlyingType);
			if (cachedAttribute != null)
			{
				jsonObjectContract.ItemRequired = cachedAttribute._itemRequired;
			}
			if (jsonObjectContract.IsInstantiable)
			{
				ConstructorInfo attributeConstructor = this.GetAttributeConstructor(jsonObjectContract.NonNullableUnderlyingType);
				if (attributeConstructor != null)
				{
					jsonObjectContract.OverrideConstructor = attributeConstructor;
					jsonObjectContract.CreatorParameters.AddRange<JsonProperty>(this.CreateConstructorParameters(attributeConstructor, jsonObjectContract.Properties));
				}
				else if (jsonObjectContract.MemberSerialization == MemberSerialization.Fields)
				{
					if (JsonTypeReflector.FullyTrusted)
					{
						jsonObjectContract.DefaultCreator = new Func<object>(jsonObjectContract.GetUninitializedObject);
					}
				}
				else if (jsonObjectContract.DefaultCreator == null || jsonObjectContract.DefaultCreatorNonPublic)
				{
					ConstructorInfo parameterizedConstructor = this.GetParameterizedConstructor(jsonObjectContract.NonNullableUnderlyingType);
					if (parameterizedConstructor != null)
					{
						jsonObjectContract.ParametrizedConstructor = parameterizedConstructor;
						jsonObjectContract.CreatorParameters.AddRange<JsonProperty>(this.CreateConstructorParameters(parameterizedConstructor, jsonObjectContract.Properties));
					}
				}
			}
			MemberInfo extensionDataMemberForType = this.GetExtensionDataMemberForType(jsonObjectContract.NonNullableUnderlyingType);
			if (extensionDataMemberForType != null)
			{
				DefaultContractResolver.SetExtensionDataDelegates(jsonObjectContract, extensionDataMemberForType);
			}
			return jsonObjectContract;
		}

		protected virtual JsonPrimitiveContract CreatePrimitiveContract(Type objectType)
		{
			JsonPrimitiveContract jsonPrimitiveContract = new JsonPrimitiveContract(objectType);
			this.InitializeContract(jsonPrimitiveContract);
			return jsonPrimitiveContract;
		}

		protected virtual IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			List<MemberInfo> serializableMembers = this.GetSerializableMembers(type);
			if (serializableMembers == null)
			{
				throw new JsonSerializationException("Null collection of seralizable members returned.");
			}
			JsonPropertyCollection jsonPropertyCollection = new JsonPropertyCollection(type);
			foreach (MemberInfo serializableMember in serializableMembers)
			{
				JsonProperty jsonProperty = this.CreateProperty(serializableMember, memberSerialization);
				if (jsonProperty == null)
				{
					continue;
				}
				DefaultContractResolverState state = this.GetState();
				PropertyNameTable nameTable = state.NameTable;
				Monitor.Enter(nameTable);
				try
				{
					jsonProperty.PropertyName = state.NameTable.Add(jsonProperty.PropertyName);
				}
				finally
				{
					Monitor.Exit(nameTable);
				}
				jsonPropertyCollection.AddProperty(jsonProperty);
			}
			return jsonPropertyCollection.OrderBy<JsonProperty, int>((JsonProperty p) => {
				int? order = p.Order;
				if (!order.HasValue)
				{
					return -1;
				}
				return order.GetValueOrDefault();
			}).ToList<JsonProperty>();
		}

		protected virtual JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			bool flag;
			JsonProperty jsonProperty = new JsonProperty()
			{
				PropertyType = ReflectionUtils.GetMemberUnderlyingType(member),
				DeclaringType = member.DeclaringType,
				ValueProvider = this.CreateMemberValueProvider(member),
				AttributeProvider = new ReflectionAttributeProvider(member)
			};
			this.SetPropertySettingsFromAttributes(jsonProperty, member, member.Name, member.DeclaringType, memberSerialization, out flag);
			if (memberSerialization == MemberSerialization.Fields)
			{
				jsonProperty.Readable = true;
				jsonProperty.Writable = true;
			}
			else
			{
				jsonProperty.Readable = ReflectionUtils.CanReadMemberValue(member, flag);
				jsonProperty.Writable = ReflectionUtils.CanSetMemberValue(member, flag, jsonProperty.HasMemberAttribute);
			}
			jsonProperty.ShouldSerialize = this.CreateShouldSerializeTest(member);
			this.SetIsSpecifiedActions(jsonProperty, member, flag);
			return jsonProperty;
		}

		protected virtual JsonProperty CreatePropertyFromConstructorParameter(JsonProperty matchingMemberProperty, ParameterInfo parameterInfo)
		{
			bool flag;
			JsonProperty jsonProperty = new JsonProperty()
			{
				PropertyType = parameterInfo.ParameterType,
				AttributeProvider = new ReflectionAttributeProvider(parameterInfo)
			};
			this.SetPropertySettingsFromAttributes(jsonProperty, parameterInfo, parameterInfo.Name, parameterInfo.Member.DeclaringType, MemberSerialization.OptOut, out flag);
			jsonProperty.Readable = false;
			jsonProperty.Writable = true;
			if (matchingMemberProperty != null)
			{
				jsonProperty.PropertyName = (jsonProperty.PropertyName != parameterInfo.Name ? jsonProperty.PropertyName : matchingMemberProperty.PropertyName);
				jsonProperty.Converter = jsonProperty.Converter ?? matchingMemberProperty.Converter;
				jsonProperty.MemberConverter = jsonProperty.MemberConverter ?? matchingMemberProperty.MemberConverter;
				if (!jsonProperty._hasExplicitDefaultValue && matchingMemberProperty._hasExplicitDefaultValue)
				{
					jsonProperty.DefaultValue = matchingMemberProperty.DefaultValue;
				}
				JsonProperty jsonProperty1 = jsonProperty;
				Required? nullable = jsonProperty._required;
				jsonProperty1._required = (nullable.HasValue ? nullable : matchingMemberProperty._required);
				JsonProperty jsonProperty2 = jsonProperty;
				bool? isReference = jsonProperty.IsReference;
				jsonProperty2.IsReference = (isReference.HasValue ? isReference : matchingMemberProperty.IsReference);
				JsonProperty jsonProperty3 = jsonProperty;
				NullValueHandling? nullValueHandling = jsonProperty.NullValueHandling;
				jsonProperty3.NullValueHandling = (nullValueHandling.HasValue ? nullValueHandling : matchingMemberProperty.NullValueHandling);
				JsonProperty jsonProperty4 = jsonProperty;
				DefaultValueHandling? defaultValueHandling = jsonProperty.DefaultValueHandling;
				jsonProperty4.DefaultValueHandling = (defaultValueHandling.HasValue ? defaultValueHandling : matchingMemberProperty.DefaultValueHandling);
				JsonProperty jsonProperty5 = jsonProperty;
				ReferenceLoopHandling? referenceLoopHandling = jsonProperty.ReferenceLoopHandling;
				jsonProperty5.ReferenceLoopHandling = (referenceLoopHandling.HasValue ? referenceLoopHandling : matchingMemberProperty.ReferenceLoopHandling);
				JsonProperty jsonProperty6 = jsonProperty;
				ObjectCreationHandling? objectCreationHandling = jsonProperty.ObjectCreationHandling;
				jsonProperty6.ObjectCreationHandling = (objectCreationHandling.HasValue ? objectCreationHandling : matchingMemberProperty.ObjectCreationHandling);
				JsonProperty jsonProperty7 = jsonProperty;
				TypeNameHandling? typeNameHandling = jsonProperty.TypeNameHandling;
				jsonProperty7.TypeNameHandling = (typeNameHandling.HasValue ? typeNameHandling : matchingMemberProperty.TypeNameHandling);
			}
			return jsonProperty;
		}

		private Predicate<object> CreateShouldSerializeTest(MemberInfo member)
		{
			MethodInfo method = member.DeclaringType.GetMethod(string.Concat("ShouldSerialize", member.Name), ReflectionUtils.EmptyTypes);
			if (method == null || method.ReturnType != typeof(bool))
			{
				return null;
			}
			MethodCall<object, object> methodCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
			return (object o) => (bool)methodCall(o, new object[0]);
		}

		protected virtual JsonStringContract CreateStringContract(Type objectType)
		{
			JsonStringContract jsonStringContract = new JsonStringContract(objectType);
			this.InitializeContract(jsonStringContract);
			return jsonStringContract;
		}

		private ConstructorInfo GetAttributeConstructor(Type objectType)
		{
			IList<ConstructorInfo> list = (
				from c in (IEnumerable<ConstructorInfo>)objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				where c.IsDefined(typeof(JsonConstructorAttribute), true)
				select c).ToList<ConstructorInfo>();
			if (list.Count > 1)
			{
				throw new JsonException("Multiple constructors with the JsonConstructorAttribute.");
			}
			if (list.Count == 1)
			{
				return list[0];
			}
			if (objectType != typeof(Version))
			{
				return null;
			}
			return objectType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) });
		}

		private void GetCallbackMethodsForType(Type type, out List<SerializationCallback> onSerializing, out List<SerializationCallback> onSerialized, out List<SerializationCallback> onDeserializing, out List<SerializationCallback> onDeserialized, out List<SerializationErrorCallback> onError)
		{
			onSerializing = null;
			onSerialized = null;
			onDeserializing = null;
			onDeserialized = null;
			onError = null;
			foreach (Type classHierarchyForType in this.GetClassHierarchyForType(type))
			{
				MethodInfo methodInfo = null;
				MethodInfo methodInfo1 = null;
				MethodInfo methodInfo2 = null;
				MethodInfo methodInfo3 = null;
				MethodInfo methodInfo4 = null;
				bool flag = DefaultContractResolver.ShouldSkipSerializing(classHierarchyForType);
				bool flag1 = DefaultContractResolver.ShouldSkipDeserialized(classHierarchyForType);
				MethodInfo[] methods = classHierarchyForType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				for (int i = 0; i < (int)methods.Length; i++)
				{
					MethodInfo methodInfo5 = methods[i];
					if (!methodInfo5.ContainsGenericParameters)
					{
						Type type1 = null;
						ParameterInfo[] parameters = methodInfo5.GetParameters();
						if (!flag && DefaultContractResolver.IsValidCallback(methodInfo5, parameters, typeof(OnSerializingAttribute), methodInfo, ref type1))
						{
							onSerializing = onSerializing ?? new List<SerializationCallback>();
							onSerializing.Add(JsonContract.CreateSerializationCallback(methodInfo5));
							methodInfo = methodInfo5;
						}
						if (DefaultContractResolver.IsValidCallback(methodInfo5, parameters, typeof(OnSerializedAttribute), methodInfo1, ref type1))
						{
							onSerialized = onSerialized ?? new List<SerializationCallback>();
							onSerialized.Add(JsonContract.CreateSerializationCallback(methodInfo5));
							methodInfo1 = methodInfo5;
						}
						if (DefaultContractResolver.IsValidCallback(methodInfo5, parameters, typeof(OnDeserializingAttribute), methodInfo2, ref type1))
						{
							onDeserializing = onDeserializing ?? new List<SerializationCallback>();
							onDeserializing.Add(JsonContract.CreateSerializationCallback(methodInfo5));
							methodInfo2 = methodInfo5;
						}
						if (!flag1 && DefaultContractResolver.IsValidCallback(methodInfo5, parameters, typeof(OnDeserializedAttribute), methodInfo3, ref type1))
						{
							onDeserialized = onDeserialized ?? new List<SerializationCallback>();
							onDeserialized.Add(JsonContract.CreateSerializationCallback(methodInfo5));
							methodInfo3 = methodInfo5;
						}
						if (DefaultContractResolver.IsValidCallback(methodInfo5, parameters, typeof(OnErrorAttribute), methodInfo4, ref type1))
						{
							onError = onError ?? new List<SerializationErrorCallback>();
							onError.Add(JsonContract.CreateSerializationErrorCallback(methodInfo5));
							methodInfo4 = methodInfo5;
						}
					}
				}
			}
		}

		private List<Type> GetClassHierarchyForType(Type type)
		{
			List<Type> types = new List<Type>();
			for (Type i = type; i != null && i != typeof(object); i = i.BaseType())
			{
				types.Add(i);
			}
			types.Reverse();
			return types;
		}

		internal static string GetClrTypeFullName(Type type)
		{
			if (type.IsGenericTypeDefinition() || !type.ContainsGenericParameters())
			{
				return type.FullName;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", new object[] { type.Namespace, type.Name });
		}

		private Func<object> GetDefaultCreator(Type createdType)
		{
			return JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(createdType);
		}

		private MemberInfo GetExtensionDataMemberForType(Type type)
		{
			return this.GetClassHierarchyForType(type).SelectMany<Type, MemberInfo>((Type baseType) => {
				List<MemberInfo> memberInfos = new List<MemberInfo>();
				memberInfos.AddRange<MemberInfo>(baseType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				memberInfos.AddRange<MemberInfo>(baseType.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic));
				return memberInfos;
			}).LastOrDefault<MemberInfo>((MemberInfo m) => {
				Type type1;
				MemberTypes memberType = m.MemberType();
				if (memberType != MemberTypes.Property && memberType != MemberTypes.Field)
				{
					return false;
				}
				if (!m.IsDefined(typeof(JsonExtensionDataAttribute), false))
				{
					return false;
				}
				if (!ReflectionUtils.CanReadMemberValue(m, true))
				{
					throw new JsonException("Invalid extension data attribute on '{0}'. Member '{1}' must have a getter.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(m.DeclaringType), m.Name));
				}
				if (ReflectionUtils.ImplementsGenericDefinition(ReflectionUtils.GetMemberUnderlyingType(m), typeof(IDictionary<,>), out type1))
				{
					Type genericArguments = type1.GetGenericArguments()[0];
					Type genericArguments1 = type1.GetGenericArguments()[1];
					if (genericArguments.IsAssignableFrom(typeof(string)) && genericArguments1.IsAssignableFrom(typeof(JToken)))
					{
						return true;
					}
				}
				throw new JsonException("Invalid extension data attribute on '{0}'. Member '{1}' type must implement IDictionary<string, JToken>.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(m.DeclaringType), m.Name));
			});
		}

		private ConstructorInfo GetParameterizedConstructor(Type objectType)
		{
			IList<ConstructorInfo> list = objectType.GetConstructors(BindingFlags.Instance | BindingFlags.Public).ToList<ConstructorInfo>();
			if (list.Count != 1)
			{
				return null;
			}
			return list[0];
		}

		public string GetResolvedPropertyName(string propertyName)
		{
			return this.ResolvePropertyName(propertyName);
		}

		protected virtual List<MemberInfo> GetSerializableMembers(Type objectType)
		{
			Type type;
			MemberSerialization objectMemberSerialization = JsonTypeReflector.GetObjectMemberSerialization(objectType, this.IgnoreSerializableAttribute);
			List<MemberInfo> list = (
				from m in ReflectionUtils.GetFieldsAndProperties(objectType, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
				where !ReflectionUtils.IsIndexedProperty(m)
				select m).ToList<MemberInfo>();
			List<MemberInfo> memberInfos = new List<MemberInfo>();
			if (objectMemberSerialization == MemberSerialization.Fields)
			{
				foreach (MemberInfo memberInfo in list)
				{
					FieldInfo fieldInfo = memberInfo as FieldInfo;
					if (fieldInfo == null || fieldInfo.IsStatic)
					{
						continue;
					}
					memberInfos.Add(memberInfo);
				}
			}
			else
			{
				DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(objectType);
				List<MemberInfo> list1 = (
					from m in ReflectionUtils.GetFieldsAndProperties(objectType, this.DefaultMembersSearchFlags)
					where !ReflectionUtils.IsIndexedProperty(m)
					select m).ToList<MemberInfo>();
				foreach (MemberInfo memberInfo1 in list)
				{
					if (!this.SerializeCompilerGeneratedMembers && memberInfo1.IsDefined(typeof(CompilerGeneratedAttribute), true))
					{
						continue;
					}
					if (list1.Contains(memberInfo1))
					{
						memberInfos.Add(memberInfo1);
					}
					else if (JsonTypeReflector.GetAttribute<JsonPropertyAttribute>((object)memberInfo1) != null)
					{
						memberInfos.Add(memberInfo1);
					}
					else if (JsonTypeReflector.GetAttribute<JsonRequiredAttribute>((object)memberInfo1) != null)
					{
						memberInfos.Add(memberInfo1);
					}
					else if (dataContractAttribute == null || JsonTypeReflector.GetAttribute<DataMemberAttribute>((object)memberInfo1) == null)
					{
						if (objectMemberSerialization != MemberSerialization.Fields || memberInfo1.MemberType() != MemberTypes.Field)
						{
							continue;
						}
						memberInfos.Add(memberInfo1);
					}
					else
					{
						memberInfos.Add(memberInfo1);
					}
				}
				if (objectType.AssignableToTypeName("System.Data.Objects.DataClasses.EntityObject", out type))
				{
					memberInfos = memberInfos.Where<MemberInfo>(new Func<MemberInfo, bool>(this.ShouldSerializeEntityMember)).ToList<MemberInfo>();
				}
			}
			return memberInfos;
		}

		internal DefaultContractResolverState GetState()
		{
			if (this._sharedCache)
			{
				return DefaultContractResolver._sharedState;
			}
			return this._instanceState;
		}

		private void InitializeContract(JsonContract contract)
		{
			JsonContainerAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonContainerAttribute>(contract.NonNullableUnderlyingType);
			if (cachedAttribute == null)
			{
				DataContractAttribute dataContractAttribute = JsonTypeReflector.GetDataContractAttribute(contract.NonNullableUnderlyingType);
				if (dataContractAttribute != null && dataContractAttribute.IsReference)
				{
					contract.IsReference = new bool?(true);
				}
			}
			else
			{
				contract.IsReference = cachedAttribute._isReference;
			}
			contract.Converter = this.ResolveContractConverter(contract.NonNullableUnderlyingType);
			contract.InternalConverter = JsonSerializer.GetMatchingConverter(DefaultContractResolver.BuiltInConverters, contract.NonNullableUnderlyingType);
			if (contract.IsInstantiable && (ReflectionUtils.HasDefaultConstructor(contract.CreatedType, true) || contract.CreatedType.IsValueType()))
			{
				contract.DefaultCreator = this.GetDefaultCreator(contract.CreatedType);
				contract.DefaultCreatorNonPublic = (contract.CreatedType.IsValueType() ? false : ReflectionUtils.GetDefaultConstructor(contract.CreatedType) == null);
			}
			this.ResolveCallbackMethods(contract, contract.NonNullableUnderlyingType);
		}

		internal static bool IsIConvertible(Type t)
		{
			if (!typeof(IConvertible).IsAssignableFrom(t) && (!ReflectionUtils.IsNullableType(t) || !typeof(IConvertible).IsAssignableFrom(Nullable.GetUnderlyingType(t))))
			{
				return false;
			}
			return !typeof(JToken).IsAssignableFrom(t);
		}

		internal static bool IsJsonPrimitiveType(Type t)
		{
			PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(t);
			if (typeCode == PrimitiveTypeCode.Empty)
			{
				return false;
			}
			return typeCode != PrimitiveTypeCode.Object;
		}

		private static bool IsValidCallback(MethodInfo method, ParameterInfo[] parameters, Type attributeType, MethodInfo currentCallback, ref Type prevAttributeType)
		{
			if (!method.IsDefined(attributeType, false))
			{
				return false;
			}
			if (currentCallback != null)
			{
				throw new JsonException("Invalid attribute. Both '{0}' and '{1}' in type '{2}' have '{3}'.".FormatWith(CultureInfo.InvariantCulture, method, currentCallback, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), attributeType));
			}
			if (prevAttributeType != null)
			{
				throw new JsonException("Invalid Callback. Method '{3}' in type '{2}' has both '{0}' and '{1}'.".FormatWith(CultureInfo.InvariantCulture, prevAttributeType, attributeType, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method));
			}
			if (method.IsVirtual)
			{
				throw new JsonException("Virtual Method '{0}' of type '{1}' cannot be marked with '{2}' attribute.".FormatWith(CultureInfo.InvariantCulture, method, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), attributeType));
			}
			if (method.ReturnType != typeof(void))
			{
				throw new JsonException("Serialization Callback '{1}' in type '{0}' must return void.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method));
			}
			if (attributeType == typeof(OnErrorAttribute))
			{
				if (parameters == null || (int)parameters.Length != 2 || parameters[0].ParameterType != typeof(StreamingContext) || parameters[1].ParameterType != typeof(ErrorContext))
				{
					throw new JsonException("Serialization Error Callback '{1}' in type '{0}' must have two parameters of type '{2}' and '{3}'.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext), typeof(ErrorContext)));
				}
			}
			else if (parameters == null || (int)parameters.Length != 1 || parameters[0].ParameterType != typeof(StreamingContext))
			{
				throw new JsonException("Serialization Callback '{1}' in type '{0}' must have a single parameter of type '{2}'.".FormatWith(CultureInfo.InvariantCulture, DefaultContractResolver.GetClrTypeFullName(method.DeclaringType), method, typeof(StreamingContext)));
			}
			prevAttributeType = attributeType;
			return true;
		}

		private void ResolveCallbackMethods(JsonContract contract, Type t)
		{
			List<SerializationCallback> serializationCallbacks;
			List<SerializationCallback> serializationCallbacks1;
			List<SerializationCallback> serializationCallbacks2;
			List<SerializationCallback> serializationCallbacks3;
			List<SerializationErrorCallback> serializationErrorCallbacks;
			this.GetCallbackMethodsForType(t, out serializationCallbacks, out serializationCallbacks1, out serializationCallbacks2, out serializationCallbacks3, out serializationErrorCallbacks);
			if (serializationCallbacks != null)
			{
				contract.OnSerializingCallbacks.AddRange<SerializationCallback>(serializationCallbacks);
			}
			if (serializationCallbacks1 != null)
			{
				contract.OnSerializedCallbacks.AddRange<SerializationCallback>(serializationCallbacks1);
			}
			if (serializationCallbacks2 != null)
			{
				contract.OnDeserializingCallbacks.AddRange<SerializationCallback>(serializationCallbacks2);
			}
			if (serializationCallbacks3 != null)
			{
				contract.OnDeserializedCallbacks.AddRange<SerializationCallback>(serializationCallbacks3);
			}
			if (serializationErrorCallbacks != null)
			{
				contract.OnErrorCallbacks.AddRange<SerializationErrorCallback>(serializationErrorCallbacks);
			}
		}

		public virtual JsonContract ResolveContract(Type type)
		{
			JsonContract jsonContract;
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			DefaultContractResolverState state = this.GetState();
			ResolverContractKey resolverContractKey = new ResolverContractKey(this.GetType(), type);
			Dictionary<ResolverContractKey, JsonContract> contractCache = state.ContractCache;
			if (contractCache == null || !contractCache.TryGetValue(resolverContractKey, out jsonContract))
			{
				jsonContract = this.CreateContract(type);
				object typeContractCacheLock = DefaultContractResolver.TypeContractCacheLock;
				Monitor.Enter(typeContractCacheLock);
				try
				{
					contractCache = state.ContractCache;
					Dictionary<ResolverContractKey, JsonContract> resolverContractKeys = (contractCache != null ? new Dictionary<ResolverContractKey, JsonContract>(contractCache) : new Dictionary<ResolverContractKey, JsonContract>());
					resolverContractKeys[resolverContractKey] = jsonContract;
					state.ContractCache = resolverContractKeys;
				}
				finally
				{
					Monitor.Exit(typeContractCacheLock);
				}
			}
			return jsonContract;
		}

		protected virtual JsonConverter ResolveContractConverter(Type objectType)
		{
			return JsonTypeReflector.GetJsonConverter(objectType);
		}

		protected virtual string ResolveDictionaryKey(string dictionaryKey)
		{
			return this.ResolvePropertyName(dictionaryKey);
		}

		protected virtual string ResolvePropertyName(string propertyName)
		{
			return propertyName;
		}

		private static void SetExtensionDataDelegates(JsonObjectContract contract, MemberInfo member)
		{
			Type type;
			Type type1;
			Action<object, object> action;
			JsonExtensionDataAttribute attribute = ReflectionUtils.GetAttribute<JsonExtensionDataAttribute>(member);
			if (attribute == null)
			{
				return;
			}
			Type memberUnderlyingType = ReflectionUtils.GetMemberUnderlyingType(member);
			ReflectionUtils.ImplementsGenericDefinition(memberUnderlyingType, typeof(IDictionary<,>), out type);
			Type genericArguments = type.GetGenericArguments()[0];
			Type genericArguments1 = type.GetGenericArguments()[1];
			type1 = (!ReflectionUtils.IsGenericDefinition(memberUnderlyingType, typeof(IDictionary<,>)) ? memberUnderlyingType : typeof(Dictionary<,>).MakeGenericType(new Type[] { genericArguments, genericArguments1 }));
			Func<object, object> func = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(member);
			if (attribute.ReadData)
			{
				if (ReflectionUtils.CanSetMemberValue(member, true, false))
				{
					action = JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(member);
				}
				else
				{
					action = null;
				}
				Action<object, object> action1 = action;
				Func<object> func1 = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type1);
				MethodInfo method = memberUnderlyingType.GetMethod("Add", new Type[] { genericArguments, genericArguments1 });
				MethodCall<object, object> methodCall = JsonTypeReflector.ReflectionDelegateFactory.CreateMethodCall<object>(method);
				contract.ExtensionDataSetter = (object o, string key, object value) => {
					object cSu0024u003cu003e8_locals1 = func(o);
					if (cSu0024u003cu003e8_locals1 == null)
					{
						if (action1 == null)
						{
							throw new JsonSerializationException("Cannot set value onto extension data member '{0}'. The extension data collection is null and it cannot be set.".FormatWith(CultureInfo.InvariantCulture, member.Name));
						}
						cSu0024u003cu003e8_locals1 = func1();
						action1(o, cSu0024u003cu003e8_locals1);
					}
					methodCall(cSu0024u003cu003e8_locals1, new object[] { key, value });
				};
			}
			if (attribute.WriteData)
			{
				ConstructorInfo constructorInfo = typeof(DefaultContractResolver.EnumerableDictionaryWrapper<,>).MakeGenericType(new Type[] { genericArguments, genericArguments1 }).GetConstructors().First<ConstructorInfo>();
				ObjectConstructor<object> objectConstructor = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructorInfo);
				contract.ExtensionDataGetter = (object o) => {
					object cSu0024u003cu003e8_locals2 = func(o);
					if (cSu0024u003cu003e8_locals2 == null)
					{
						return null;
					}
					return (IEnumerable<KeyValuePair<object, object>>)objectConstructor(new object[] { cSu0024u003cu003e8_locals2 });
				};
			}
			contract.ExtensionDataValueType = genericArguments1;
		}

		private void SetIsSpecifiedActions(JsonProperty property, MemberInfo member, bool allowNonPublicAccess)
		{
			MemberInfo field = member.DeclaringType.GetProperty(string.Concat(member.Name, "Specified"));
			if (field == null)
			{
				field = member.DeclaringType.GetField(string.Concat(member.Name, "Specified"));
			}
			if (field == null || ReflectionUtils.GetMemberUnderlyingType(field) != typeof(bool))
			{
				return;
			}
			Func<object, object> func = JsonTypeReflector.ReflectionDelegateFactory.CreateGet<object>(field);
			property.GetIsSpecified = (object o) => (bool)func(o);
			if (ReflectionUtils.CanSetMemberValue(field, allowNonPublicAccess, false))
			{
				property.SetIsSpecified = JsonTypeReflector.ReflectionDelegateFactory.CreateSet<object>(field);
			}
		}

		private void SetPropertySettingsFromAttributes(JsonProperty property, object attributeProvider, string name, Type declaringType, MemberSerialization memberSerialization, out bool allowNonPublicAccess)
		{
			DataMemberAttribute dataMemberAttribute;
			string propertyName;
			ReferenceLoopHandling? nullable;
			TypeNameHandling? nullable1;
			bool? nullable2;
			NullValueHandling? nullable3;
			ReferenceLoopHandling? nullable4;
			ObjectCreationHandling? nullable5;
			TypeNameHandling? nullable6;
			bool? nullable7;
			bool? nullable8;
			JsonConverter jsonConverter;
			ReferenceLoopHandling? nullable9;
			TypeNameHandling? nullable10;
			int? nullable11;
			DefaultValueHandling? nullable12;
			MemberInfo memberInfo = attributeProvider as MemberInfo;
			if (JsonTypeReflector.GetDataContractAttribute(declaringType) == null || memberInfo == null)
			{
				dataMemberAttribute = null;
			}
			else
			{
				dataMemberAttribute = JsonTypeReflector.GetDataMemberAttribute(memberInfo);
			}
			JsonPropertyAttribute attribute = JsonTypeReflector.GetAttribute<JsonPropertyAttribute>(attributeProvider);
			JsonRequiredAttribute jsonRequiredAttribute = JsonTypeReflector.GetAttribute<JsonRequiredAttribute>(attributeProvider);
			if (attribute == null || attribute.PropertyName == null)
			{
				propertyName = (dataMemberAttribute == null || dataMemberAttribute.Name == null ? name : dataMemberAttribute.Name);
			}
			else
			{
				propertyName = attribute.PropertyName;
			}
			property.PropertyName = this.ResolvePropertyName(propertyName);
			property.UnderlyingName = name;
			bool flag = false;
			if (attribute != null)
			{
				property._required = attribute._required;
				property.Order = attribute._order;
				property.DefaultValueHandling = attribute._defaultValueHandling;
				flag = true;
			}
			else if (dataMemberAttribute != null)
			{
				property._required = new Required?((dataMemberAttribute.IsRequired ? Required.AllowNull : Required.Default));
				JsonProperty jsonProperty = property;
				if (dataMemberAttribute.Order != -1)
				{
					nullable11 = new int?(dataMemberAttribute.Order);
				}
				else
				{
					nullable11 = null;
				}
				jsonProperty.Order = nullable11;
				JsonProperty jsonProperty1 = property;
				if (!dataMemberAttribute.EmitDefaultValue)
				{
					nullable12 = new DefaultValueHandling?(DefaultValueHandling.Ignore);
				}
				else
				{
					nullable12 = null;
				}
				jsonProperty1.DefaultValueHandling = nullable12;
				flag = true;
			}
			if (jsonRequiredAttribute != null)
			{
				property._required = new Required?(Required.Always);
				flag = true;
			}
			property.HasMemberAttribute = flag;
			bool flag1 = (JsonTypeReflector.GetAttribute<JsonIgnoreAttribute>(attributeProvider) != null || JsonTypeReflector.GetAttribute<JsonExtensionDataAttribute>(attributeProvider) != null ? true : JsonTypeReflector.GetAttribute<NonSerializedAttribute>(attributeProvider) != null);
			if (memberSerialization == MemberSerialization.OptIn)
			{
				property.Ignored = (flag1 ? true : !flag);
			}
			else
			{
				property.Ignored = flag1 | false;
			}
			property.Converter = JsonTypeReflector.GetJsonConverter(attributeProvider);
			property.MemberConverter = JsonTypeReflector.GetJsonConverter(attributeProvider);
			DefaultValueAttribute defaultValueAttribute = JsonTypeReflector.GetAttribute<DefaultValueAttribute>(attributeProvider);
			if (defaultValueAttribute != null)
			{
				property.DefaultValue = defaultValueAttribute.Value;
			}
			JsonProperty jsonProperty2 = property;
			if (attribute != null)
			{
				nullable3 = attribute._nullValueHandling;
			}
			else
			{
				nullable3 = null;
			}
			jsonProperty2.NullValueHandling = nullable3;
			JsonProperty jsonProperty3 = property;
			if (attribute != null)
			{
				nullable4 = attribute._referenceLoopHandling;
			}
			else
			{
				nullable = null;
				nullable4 = nullable;
			}
			jsonProperty3.ReferenceLoopHandling = nullable4;
			JsonProperty jsonProperty4 = property;
			if (attribute != null)
			{
				nullable5 = attribute._objectCreationHandling;
			}
			else
			{
				nullable5 = null;
			}
			jsonProperty4.ObjectCreationHandling = nullable5;
			JsonProperty jsonProperty5 = property;
			if (attribute != null)
			{
				nullable6 = attribute._typeNameHandling;
			}
			else
			{
				nullable1 = null;
				nullable6 = nullable1;
			}
			jsonProperty5.TypeNameHandling = nullable6;
			JsonProperty jsonProperty6 = property;
			if (attribute != null)
			{
				nullable7 = attribute._isReference;
			}
			else
			{
				nullable2 = null;
				nullable7 = nullable2;
			}
			jsonProperty6.IsReference = nullable7;
			JsonProperty jsonProperty7 = property;
			if (attribute != null)
			{
				nullable8 = attribute._itemIsReference;
			}
			else
			{
				nullable2 = null;
				nullable8 = nullable2;
			}
			jsonProperty7.ItemIsReference = nullable8;
			JsonProperty jsonProperty8 = property;
			if (attribute == null || attribute.ItemConverterType == null)
			{
				jsonConverter = null;
			}
			else
			{
				jsonConverter = JsonTypeReflector.CreateJsonConverterInstance(attribute.ItemConverterType, attribute.ItemConverterParameters);
			}
			jsonProperty8.ItemConverter = jsonConverter;
			JsonProperty jsonProperty9 = property;
			if (attribute != null)
			{
				nullable9 = attribute._itemReferenceLoopHandling;
			}
			else
			{
				nullable = null;
				nullable9 = nullable;
			}
			jsonProperty9.ItemReferenceLoopHandling = nullable9;
			JsonProperty jsonProperty10 = property;
			if (attribute != null)
			{
				nullable10 = attribute._itemTypeNameHandling;
			}
			else
			{
				nullable1 = null;
				nullable10 = nullable1;
			}
			jsonProperty10.ItemTypeNameHandling = nullable10;
			allowNonPublicAccess = false;
			if ((this.DefaultMembersSearchFlags & BindingFlags.NonPublic) == BindingFlags.NonPublic)
			{
				allowNonPublicAccess = true;
			}
			if (flag)
			{
				allowNonPublicAccess = true;
			}
			if (memberSerialization == MemberSerialization.Fields)
			{
				allowNonPublicAccess = true;
			}
		}

		private bool ShouldSerializeEntityMember(MemberInfo memberInfo)
		{
			PropertyInfo propertyInfo = memberInfo as PropertyInfo;
			if (propertyInfo != null && propertyInfo.PropertyType.IsGenericType() && propertyInfo.PropertyType.GetGenericTypeDefinition().FullName == "System.Data.Objects.DataClasses.EntityReference`1")
			{
				return false;
			}
			return true;
		}

		private static bool ShouldSkipDeserialized(Type t)
		{
			return false;
		}

		private static bool ShouldSkipSerializing(Type t)
		{
			return false;
		}

		internal class EnumerableDictionaryWrapper<TEnumeratorKey, TEnumeratorValue> : IEnumerable<KeyValuePair<object, object>>, IEnumerable
		{
			private readonly IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> _e;

			public EnumerableDictionaryWrapper(IEnumerable<KeyValuePair<TEnumeratorKey, TEnumeratorValue>> e)
			{
				ValidationUtils.ArgumentNotNull(e, "e");
				this._e = e;
			}

			public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
			{
				foreach (KeyValuePair<TEnumeratorKey, TEnumeratorValue> keyValuePair in this._e)
				{
					yield return new KeyValuePair<object, object>((object)keyValuePair.Key, (object)keyValuePair.Value);
				}
			}

			IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}
		}
	}
}