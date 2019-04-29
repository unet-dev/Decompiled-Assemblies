using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	internal static class JsonTypeReflector
	{
		private static bool? _dynamicCodeGeneration;

		private static bool? _fullyTrusted;

		public const string IdPropertyName = "$id";

		public const string RefPropertyName = "$ref";

		public const string TypePropertyName = "$type";

		public const string ValuePropertyName = "$value";

		public const string ArrayValuesPropertyName = "$values";

		public const string ShouldSerializePrefix = "ShouldSerialize";

		public const string SpecifiedPostfix = "Specified";

		private readonly static ThreadSafeStore<Type, Func<object[], JsonConverter>> JsonConverterCreatorCache;

		private readonly static ThreadSafeStore<Type, Type> AssociatedMetadataTypesCache;

		private static ReflectionObject _metadataTypeAttributeReflectionObject;

		public static bool DynamicCodeGeneration
		{
			get
			{
				if (!JsonTypeReflector._dynamicCodeGeneration.HasValue)
				{
					try
					{
						(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess)).Demand();
						(new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess)).Demand();
						(new SecurityPermission(SecurityPermissionFlag.SkipVerification)).Demand();
						(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).Demand();
						(new SecurityPermission(PermissionState.Unrestricted)).Demand();
						JsonTypeReflector._dynamicCodeGeneration = new bool?(true);
					}
					catch (Exception exception)
					{
						JsonTypeReflector._dynamicCodeGeneration = new bool?(false);
					}
				}
				return JsonTypeReflector._dynamicCodeGeneration.GetValueOrDefault();
			}
		}

		public static bool FullyTrusted
		{
			get
			{
				if (!JsonTypeReflector._fullyTrusted.HasValue)
				{
					try
					{
						(new SecurityPermission(PermissionState.Unrestricted)).Demand();
						JsonTypeReflector._fullyTrusted = new bool?(true);
					}
					catch (Exception exception)
					{
						JsonTypeReflector._fullyTrusted = new bool?(false);
					}
				}
				return JsonTypeReflector._fullyTrusted.GetValueOrDefault();
			}
		}

		public static Newtonsoft.Json.Utilities.ReflectionDelegateFactory ReflectionDelegateFactory
		{
			get
			{
				return LateBoundReflectionDelegateFactory.Instance;
			}
		}

		static JsonTypeReflector()
		{
			JsonTypeReflector.JsonConverterCreatorCache = new ThreadSafeStore<Type, Func<object[], JsonConverter>>(new Func<Type, Func<object[], JsonConverter>>(JsonTypeReflector.GetJsonConverterCreator));
			JsonTypeReflector.AssociatedMetadataTypesCache = new ThreadSafeStore<Type, Type>(new Func<Type, Type>(JsonTypeReflector.GetAssociateMetadataTypeFromAttribute));
		}

		public static JsonConverter CreateJsonConverterInstance(Type converterType, object[] converterArgs)
		{
			return JsonTypeReflector.JsonConverterCreatorCache.Get(converterType)(converterArgs);
		}

		private static Type GetAssociatedMetadataType(Type type)
		{
			return JsonTypeReflector.AssociatedMetadataTypesCache.Get(type);
		}

		private static Type GetAssociateMetadataTypeFromAttribute(Type type)
		{
			Attribute[] attributes = ReflectionUtils.GetAttributes(type, null, true);
			for (int i = 0; i < (int)attributes.Length; i++)
			{
				Attribute attribute = attributes[i];
				Type type1 = attribute.GetType();
				if (string.Equals(type1.FullName, "System.ComponentModel.DataAnnotations.MetadataTypeAttribute", StringComparison.Ordinal))
				{
					if (JsonTypeReflector._metadataTypeAttributeReflectionObject == null)
					{
						JsonTypeReflector._metadataTypeAttributeReflectionObject = ReflectionObject.Create(type1, new string[] { "MetadataClassType" });
					}
					return (Type)JsonTypeReflector._metadataTypeAttributeReflectionObject.GetValue(attribute, "MetadataClassType");
				}
			}
			return null;
		}

		private static T GetAttribute<T>(Type type)
		where T : Attribute
		{
			T attribute;
			Type associatedMetadataType = JsonTypeReflector.GetAssociatedMetadataType(type);
			if (associatedMetadataType != null)
			{
				attribute = ReflectionUtils.GetAttribute<T>(associatedMetadataType, true);
				if (attribute != null)
				{
					return attribute;
				}
			}
			attribute = ReflectionUtils.GetAttribute<T>(type, true);
			if (attribute != null)
			{
				return attribute;
			}
			Type[] interfaces = type.GetInterfaces();
			for (int i = 0; i < (int)interfaces.Length; i++)
			{
				attribute = ReflectionUtils.GetAttribute<T>(interfaces[i], true);
				if (attribute != null)
				{
					return attribute;
				}
			}
			return default(T);
		}

		private static T GetAttribute<T>(MemberInfo memberInfo)
		where T : Attribute
		{
			T attribute;
			Type associatedMetadataType = JsonTypeReflector.GetAssociatedMetadataType(memberInfo.DeclaringType);
			if (associatedMetadataType != null)
			{
				MemberInfo memberInfoFromType = ReflectionUtils.GetMemberInfoFromType(associatedMetadataType, memberInfo);
				if (memberInfoFromType != null)
				{
					attribute = ReflectionUtils.GetAttribute<T>(memberInfoFromType, true);
					if (attribute != null)
					{
						return attribute;
					}
				}
			}
			attribute = ReflectionUtils.GetAttribute<T>(memberInfo, true);
			if (attribute != null)
			{
				return attribute;
			}
			if (memberInfo.DeclaringType != null)
			{
				Type[] interfaces = memberInfo.DeclaringType.GetInterfaces();
				for (int i = 0; i < (int)interfaces.Length; i++)
				{
					MemberInfo memberInfoFromType1 = ReflectionUtils.GetMemberInfoFromType(interfaces[i], memberInfo);
					if (memberInfoFromType1 != null)
					{
						attribute = ReflectionUtils.GetAttribute<T>(memberInfoFromType1, true);
						if (attribute != null)
						{
							return attribute;
						}
					}
				}
			}
			return default(T);
		}

		public static T GetAttribute<T>(object provider)
		where T : Attribute
		{
			Type type = provider as Type;
			if (type != null)
			{
				return JsonTypeReflector.GetAttribute<T>(type);
			}
			MemberInfo memberInfo = provider as MemberInfo;
			if (memberInfo != null)
			{
				return JsonTypeReflector.GetAttribute<T>(memberInfo);
			}
			return ReflectionUtils.GetAttribute<T>(provider, true);
		}

		public static T GetCachedAttribute<T>(object attributeProvider)
		where T : Attribute
		{
			return CachedAttributeGetter<T>.GetAttribute(attributeProvider);
		}

		public static DataContractAttribute GetDataContractAttribute(Type type)
		{
			for (Type i = type; i != null; i = i.BaseType())
			{
				DataContractAttribute attribute = CachedAttributeGetter<DataContractAttribute>.GetAttribute(i);
				if (attribute != null)
				{
					return attribute;
				}
			}
			return null;
		}

		public static DataMemberAttribute GetDataMemberAttribute(MemberInfo memberInfo)
		{
			if (memberInfo.MemberType() == MemberTypes.Field)
			{
				return CachedAttributeGetter<DataMemberAttribute>.GetAttribute(memberInfo);
			}
			PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
			DataMemberAttribute attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(propertyInfo);
			if (attribute == null && propertyInfo.IsVirtual())
			{
				for (Type i = propertyInfo.DeclaringType; attribute == null && i != null; i = i.BaseType())
				{
					PropertyInfo memberInfoFromType = (PropertyInfo)ReflectionUtils.GetMemberInfoFromType(i, propertyInfo);
					if (memberInfoFromType != null && memberInfoFromType.IsVirtual())
					{
						attribute = CachedAttributeGetter<DataMemberAttribute>.GetAttribute(memberInfoFromType);
					}
				}
			}
			return attribute;
		}

		public static JsonConverter GetJsonConverter(object attributeProvider)
		{
			JsonConverterAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonConverterAttribute>(attributeProvider);
			if (cachedAttribute != null)
			{
				Func<object[], JsonConverter> func = JsonTypeReflector.JsonConverterCreatorCache.Get(cachedAttribute.ConverterType);
				if (func != null)
				{
					return func(cachedAttribute.ConverterParameters);
				}
			}
			return null;
		}

		private static Func<object[], JsonConverter> GetJsonConverterCreator(Type converterType)
		{
			Func<object> func;
			if (ReflectionUtils.HasDefaultConstructor(converterType, false))
			{
				func = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(converterType);
			}
			else
			{
				func = null;
			}
			Func<object> func1 = func;
			return (object[] parameters) => {
				JsonConverter jsonConverter;
				try
				{
					if (parameters == null)
					{
						if (func1 == null)
						{
							throw new JsonException("No parameterless constructor defined for '{0}'.".FormatWith(CultureInfo.InvariantCulture, converterType));
						}
						jsonConverter = (JsonConverter)func1();
					}
					else
					{
						object[] objArray = parameters;
						Func<object, Type> u003cu003e9_181 = JsonTypeReflector.<>c.<>9__18_1;
						if (u003cu003e9_181 == null)
						{
							u003cu003e9_181 = (object param) => param.GetType();
							JsonTypeReflector.<>c.<>9__18_1 = u003cu003e9_181;
						}
						Type[] array = ((IEnumerable<object>)objArray).Select<object, Type>(u003cu003e9_181).ToArray<Type>();
						ConstructorInfo constructor = converterType.GetConstructor(array);
						if (constructor == null)
						{
							throw new JsonException("No matching parameterized constructor found for '{0}'.".FormatWith(CultureInfo.InvariantCulture, converterType));
						}
						jsonConverter = (JsonConverter)JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor)(parameters);
					}
				}
				catch (Exception exception)
				{
					throw new JsonException("Error creating '{0}'.".FormatWith(CultureInfo.InvariantCulture, converterType), exception);
				}
				return jsonConverter;
			};
		}

		public static MemberSerialization GetObjectMemberSerialization(Type objectType, bool ignoreSerializableAttribute)
		{
			JsonObjectAttribute cachedAttribute = JsonTypeReflector.GetCachedAttribute<JsonObjectAttribute>(objectType);
			if (cachedAttribute != null)
			{
				return cachedAttribute.MemberSerialization;
			}
			if (JsonTypeReflector.GetDataContractAttribute(objectType) != null)
			{
				return MemberSerialization.OptIn;
			}
			if (!ignoreSerializableAttribute && JsonTypeReflector.GetCachedAttribute<SerializableAttribute>(objectType) != null)
			{
				return MemberSerialization.Fields;
			}
			return MemberSerialization.OptOut;
		}

		public static TypeConverter GetTypeConverter(Type type)
		{
			return TypeDescriptor.GetConverter(type);
		}
	}
}